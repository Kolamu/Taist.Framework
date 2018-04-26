namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using Mock.Nature.Native;
    using System;
    using Accessibility;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Data;
    using System.Linq;
    internal class TableObject : WinObject
    {
        private static DataTable _table = null;
        internal TableObject(string windowName, string dataGridName)
        {
            _windowName = windowName;
            _elementName = dataGridName;
            
            if (!string.Equals(windowName, RobotContext.WindowName) || !string.Equals(RobotContext.ElementName, dataGridName) || RobotContext.Window == null || RobotContext.Element == null || RobotContext.LatestWindow != null)
            {
                WWindow.SearchWindow(windowName);
                GetElement(windowName, dataGridName);
                if (element == null) throw new NullControlException(windowName, dataGridName);
                if (element.Current.ControlType != ControlType.Table)
                {
                    element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Table));
                    if (element == null)
                    {
                        throw new NullControlException(windowName, dataGridName);
                    }
                }
                RobotContext.Element = element;
                UpdateTable();
            }

            if (element == null)
            {
                element = RobotContext.Element;
                if (element == null)
                {
                    WWindow.SearchWindow(windowName);
                    GetElement(windowName, dataGridName);
                    if (!element.Current.ControlType.Equals(ControlType.Table))
                    {
                        element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Table));
                        if (element == null)
                        {
                            throw new NullControlException(windowName, dataGridName);
                        }
                    }
                    RobotContext.Element = element;
                    UpdateTable();
                }
                if (RobotContext.IsWarningWindowExist)
                {
                    throw new WarningWindowExistException();
                }
            }

            if (RobotContext.Window == null || !RobotContext.Window.Current.IsEnabled)
            {
                WWindow.SearchWindow(windowName);
                GetElement(windowName, dataGridName);
                if (!element.Current.ControlType.Equals(ControlType.Table))
                {
                    element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Table));
                    if (element == null)
                    {
                        throw new NullControlException(windowName, dataGridName);
                    }
                }
                RobotContext.Element = element;
            }
        }

        private void UpdateTable()
        {
            _table = new DataTable("Data");
            IAccessible accElement = null;
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(_windowName, _elementName);
            }

            accElement = (IAccessible)obj;

            Object[] childs = new Object[accElement.accChildCount];
            int obtain;
            NativeMethods.AccessibleChildren(accElement, 0, accElement.accChildCount, childs, out obtain);

            List<IAccessible> rowDatas = new List<IAccessible>();
            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accRole.ToString() == "28")
                {
                    rowDatas.Add(child);
                }
            }

            IAccessible columnNameObj = rowDatas[0];
            childs = new Object[columnNameObj.accChildCount];
            NativeMethods.AccessibleChildren(columnNameObj, 0, columnNameObj.accChildCount, childs, out obtain);

            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accValue == null) continue;
                string colName = child.accValue.Replace(' ', '_');
                if (!string.IsNullOrEmpty(colName))
                {
                    _table.Columns.Add(colName, typeof(string));
                }
            }

            for (int i = 1; i < rowDatas.Count; i++)
            {
                string val = rowDatas[i].accValue;
                if (val == null) continue;
                _table.Rows.Add(rowDatas[i].accValue.Split(';'));
            }

            rowDatas = null;
            childs = null;
            obj = null;
        }

        public TableObject(WindowsUnit e)
        {
            element = e;
            UpdateTable();
        }

        internal void Input(string columnName, string value, int rowIndex)
        {
            //List<string> paramList = new List<string>();
            //paramList.Add(columnName);
            //paramList.Add(value);
            //paramList.Add(rowIndex.ToString());
            //DoAction(InputAction, paramList);
            List<string> nameList = GetControlName("Table", columnName);
            IAccessible inputCell = null;
            foreach (string name in nameList)
            {
                inputCell = GetAccesibleElement(rowIndex, name);
                if (inputCell != null)
                {
                    break;
                }
            }
            if (inputCell == null)
            {
                throw new NullControlException(_windowName, _elementName, columnName);
            }

            //for (int i = 0; i < 60; i++)
            //{
            //    try
            //    {
            //        inputCell.accSelect(3);
            //    }
            //    catch
            //    {
            //        Robot.Recess(1000);
            //        continue;
            //    }
            //    if (!element.Current.IsEnabled)
            //    {
            //        Robot.Recess(1000);
            //        continue;
            //    }
            //    break;
            //}
            Robot.ExecuteWithTimeOut(() =>
                {
                    while (true)
                    {
                        try
                        {
                            inputCell.accSelect(3);
                            if (element.Current.IsEnabled)
                            {
                                break;
                            }
                        }
                        catch { }
                        Robot.Recess(500);
                    }
                }, 120000);
            
            try
            {
                Robot.ExecuteWithTimeOut(() =>
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            inputCell.accValue = value;
                            inputCell.accDoDefaultAction();
                            Robot.Recess(10);
                        }
                    }, 2000);
            }
            catch (TimeOutException) { }
            //DoAction(InputAction, value);
            //RSAA.Cell = cell;
            //for (int i = 0; i < 3; i++)
            //{
            //    if (RobotContext.IsWarningWindowExist)
            //    {
            //        throw new WarningWindowExistException();
            //    }
            //    cell.set_accValue(Type.Missing, value);
            //    cell.accDoDefaultAction();
            //}
            
        }

        //private void InputAction(object state)
        //{
        //    try
        //    {
        //        //List<string> paramList = (List<string>)state;
        //        //string columnName = paramList[0];
        //        //string value = paramList[1];
        //        //int rowIndex = int.Parse(paramList[2]);
        //        //List<string> nameList = GetControlName1("Table", columnName);
        //        //IAccessible cell = null;
        //        //foreach (string name in nameList)
        //        //{
        //        //    cell = GetAccesibleElement(rowIndex, name);
        //        //    if (cell != null)
        //        //    {
        //        //        break;
        //        //    }
        //        //}
        //        //if (cell == null)
        //        //{
        //        //    throw new NullControlException(_windowName, _elementName, columnName);
        //        //}

        //        //try
        //        //{
        //        //    cell.accSelect(3);
        //        //}
        //        //catch
        //        //{
        //        //    throw new ControlUnableException(_windowName, _elementName);
        //        //}
        //        //if (!element.Current.IsEnabled)
        //        //{
        //        //    throw new ControlUnableException(_windowName, _elementName);
        //        //}
                
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.Error(ex);
        //        ActionException = ex;
        //    }
        //    reset.Set();
        //}

        internal T GetItemObject<T>(string itemName, int rowIndex)
        {
            //element.SetFocus();
            List<string> nameList = GetControlName("Table", itemName);
            IAccessible cell = null;

            for (int i = 0; i < Config.RedoCount; i++)
            {
                foreach (string name in nameList)
                {
                    cell = GetAccesibleElement(rowIndex, name);
                    if (cell != null) break;
                }

                if (cell != null)
                {
                    break;
                }
                else
                {
                    Robot.Recess(1000);
                }
            }

            if (cell == null)
            {
                throw new NullControlException(_windowName, _elementName, itemName);
            }
            //NativeMethods.ShowOwnedPopups((IntPtr)lastWindow.Current.NativeWindowHandle, false);

            if (cell.accChildCount == 0)
            {
                //System.Windows.Rect rect = GetCellRect(cell);
                //Rect rc = element.Current.BoundingRectangle;
                //if (rect.Bottom > rc.Bottom || rect.Top < rc.Top)
                //{
                //    RollScrollBar(rect.Bottom, true);
                //    rect = GetCellRect(cell);
                //}

                //if (rect.Right > rc.Right || rect.Left < rc.Left)
                //{
                //    RollScrollBar(rect.Left, false);
                //    rect = GetCellRect(cell);
                //}
                RollScrollBar(cell);
                Rect rect = GetCellRect(cell);
                int x = (int)(rect.X + rect.Width / 2);
                int y = (int)(rect.Y + rect.Height / 2);
                WindowsUnit window = RobotContext.Window;
                if (window != null)
                {
                    IntPtr hWnd = (IntPtr)window.Current.NativeWindowHandle;

                    if (NativeMethods.IsIconic(hWnd))
                    {
                        NativeMethods.ShowWindow(hWnd, 4);
                    }
                    NativeMethods.SetForegroundWindow(hWnd);
                }
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        cell.accSelect(3, cell.accHitTest(x, y));
                        Mouse.Click(x, y);
                        Robot.Recess(100);
                        AutomationElement ef = AutomationElement.FocusedElement;
                        if (ef == null)
                        {
                            Robot.Recess(NativeMethods.GetDoubleClickTime() + 20);
                            continue;
                        }
                        int n = 0;
                        Rect fr = ef.Current.BoundingRectangle;
                        if (rect.Contains(fr.TopLeft))
                        {
                            LogManager.Debug("TopLeft");
                            n++;
                        }

                        if (rect.Contains(fr.TopRight))
                        {
                            LogManager.Debug("TopRight");
                            n++;
                        }

                        if (rect.Contains(fr.BottomLeft))
                        {
                            LogManager.Debug("BottomLeft");
                            n++;
                        }
                        if (rect.Contains(fr.BottomRight))
                        {
                            LogManager.Debug("BottomRight");
                            n++;
                        }

                        if (n == 0)
                        {
                            Robot.Recess(NativeMethods.GetDoubleClickTime() + 20);
                            continue;
                        }

                        T obj = (T)Activator.CreateInstance(typeof(T), (WindowsUnit)AutomationElement.FocusedElement);
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        LogManager.ErrorOnlyPrint(ex);
                        Robot.Recess(NativeMethods.GetDoubleClickTime() + 20);
                    }
                }
                throw new NullControlException(_windowName, _elementName, itemName);
            }
            else
            {
                LogManager.Debug("has child");
                int hWnd = 0;
                Object[] childs = new Object[cell.accChildCount];
                int obtain;
                NativeMethods.AccessibleChildren(cell, 0, cell.accChildCount, childs, out obtain);

                IAccessible child = (IAccessible)childs[0];

                NativeMethods.WindowFromAccessibleObject((IAccessible)child, out hWnd);
                
                T obj = (T)Activator.CreateInstance(typeof(T), (WindowsUnit)AutomationElement.FromHandle((IntPtr)hWnd));
                return obj;
            }
        }

        internal void Select(int rowIndex)
        {
            //element.SetFocus();
            //int count = GetRowCount();
            //RollScrollBar((double)rowIndex / (double)count * 100);
            //IAccessible cell = GetAccesibleElement(rowIndex);
            //cell.accHitTest(1, 1);
            //System.Windows.Rect rect = GetCellRect(cell);
            //int x = rect.Width / 2 > 300 ? 300 : (int)rect.Width / 2;
            //Mouse.Click((int)(rect.X + x), (int)(rect.Y + rect.Height / 2));
            //cell.accSelect(3);
            //Wait(500);
            //cell.accSelect(3);
            Select(rowIndex, false);
        }

        internal void Select(List<int> rowIndexList)
        {
            if (rowIndexList == null || rowIndexList.Count == 0) return;
            Select(rowIndexList[0], false);
            for (int i = 1; i < rowIndexList.Count; i++)
            {
                Select(rowIndexList[i], true);
            }
        }

        private void Select(int rowIndex, bool multi)
        {
            IAccessible cell = GetAccesibleElement(rowIndex);
            cell.accHitTest(1, 1);
            System.Windows.Rect rect = GetCellRect(cell);
            Rect rc = element.Current.BoundingRectangle;
            
            if (rect.Bottom > rc.Bottom || rect.Top < rc.Top)
            {
                RollScrollBar(((double)rowIndex) / ((double)_table.Rows.Count), true);
                RollScrollBar(cell, true);
                rect = GetCellRect(cell);
            }

            if (rect.Right > rc.Right || rect.Left < rc.Left)
            {
                RollScrollBar(1, false);
                RollScrollBar(cell, false);
                rect = GetCellRect(cell);
            }
            int x = rect.Width / 2 > 300 ? 300 : (int)rect.Width / 2;
            if (multi)
            {
                Keybord.KeyDown(Data.VK.CTRL);
            }
            Mouse.Click((int)(rect.X + x), (int)(rect.Y + rect.Height / 2));

            if (multi)
            {
                Keybord.KeyUp(Data.VK.CTRL);
                cell.accSelect(8);
                Wait(500);
                cell.accSelect(8);
            }
            else
            {
                cell.accSelect(3);
                Wait(500);
                cell.accSelect(3);
            }

        }

        internal bool HasColumn(string columnName)
        {
            List<string> nameList = GetTableColumnNameList();
            List<string> nameList1 = GetControlName("Table", columnName);
            foreach (string name in nameList1)
            {
                if (nameList.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        private IAccessible GetAccesibleElement(int rowIndex, string columnName = null)
        {
            IAccessible accElement = null;
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(_windowName, _elementName);
            }

            accElement = (IAccessible)obj;
            Object[] childs = new Object[accElement.accChildCount];
            int obtain;
            NativeMethods.AccessibleChildren(accElement, 0, accElement.accChildCount, childs, out obtain);

            List<IAccessible> rowDatas = new List<IAccessible>();
            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accRole.ToString() == "28")
                {
                    rowDatas.Add(child);
                }
                //Console.WriteLine("{0} {1} {2}", child.accName, child.accChildCount, child.accRole);
            }

            IAccessible rowData = null;
            if (rowIndex == 0)
            {
                rowData = rowDatas[rowDatas.Count - 1];
            }
            else
            {
                rowData = rowDatas[rowIndex];
            }

            IAccessible retData = null;
            if (columnName == null)
            {
                retData = rowData;
            }
            else
            {
                childs = new Object[rowData.accChildCount];
                NativeMethods.AccessibleChildren(rowData, 0, rowData.accChildCount, childs, out obtain);

                foreach (object o in childs)
                {
                    bool find = true;
                    IAccessible child = (IAccessible)o;
                    string[] name = child.accName.ToString().Split(' ');
                    string[] columnNames = columnName.Split(' ');
                    try
                    {
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            if (!columnNames[i].Equals(name[i]))
                            {
                                find = false;
                                break;
                            }
                        }
                        if (find)
                        {
                            retData = child;
                            break;
                        }
                    }
                    catch { }
                }
            }
            rowData = null;
            rowDatas = null;
            childs = null;
            obj = null;
            return retData;
        }

        private IAccessible GetAccesibleElement(int rowIndex, int columnIndex)
        {
            IAccessible accElement = null;
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(_windowName, _elementName);
            }

            accElement = (IAccessible)obj;

            Object[] childs = new Object[accElement.accChildCount];
            int obtain;
            NativeMethods.AccessibleChildren(accElement, 0, accElement.accChildCount, childs, out obtain);

            List<IAccessible> rowDatas = new List<IAccessible>();
            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accRole.ToString() == "28")
                {
                    rowDatas.Add(child);
                }
                //Console.WriteLine("{0} {1} {2}", child.accName, child.accChildCount, child.accRole);
            }

            IAccessible rowData = null;
            if (rowIndex == 0)
            {
                rowData = rowDatas[rowDatas.Count - 1];
            }
            else
            {
                rowData = rowDatas[rowIndex];
            }

            IAccessible retData = null;

            childs = new Object[rowData.accChildCount];
            NativeMethods.AccessibleChildren(rowData, 0, rowData.accChildCount, childs, out obtain);

            retData = (IAccessible)childs[columnIndex - 1];

            rowData = null;
            rowDatas = null;
            childs = null;
            obj = null;
            return retData;
        }

        internal List<string> GetTableColumnNameList()
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            IAccessible accElement = null;
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.WINDOW, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(_windowName, _elementName);
            }

            accElement = (IAccessible)obj;

            Object[] childs = new Object[accElement.accChildCount];
            int obtain;
            NativeMethods.AccessibleChildren(accElement, 0, accElement.accChildCount, childs, out obtain);
            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accName == accElement.accName)
                {
                    childs = new Object[child.accChildCount];
                    NativeMethods.AccessibleChildren(child, 0, child.accChildCount, childs, out obtain);
                    break;
                }
            }

            List<string> columnNameList = new List<string>();

            IAccessible rowData = (IAccessible)childs[0];

            childs = new Object[rowData.accChildCount];
            NativeMethods.AccessibleChildren(rowData, 0, rowData.accChildCount, childs, out obtain);

            foreach (object o in childs)
            {
                IAccessible child = (IAccessible)o;
                string name = (child.accName.ToString().Split(' '))[0];
                columnNameList.Add(name);
            }

            obj = null;
            rowData = null;
            childs = null;

            return columnNameList;
        }

        private System.Windows.Rect GetCellRect(IAccessible cell)
        {
            System.Windows.Rect rect = new System.Windows.Rect();
            int x = 0, y = 0, width = 0, height = 0;
            cell.accLocation(out x, out y, out width, out height);
            rect.X = x;
            rect.Y = y;
            rect.Width = width;
            rect.Height = height;
            return rect;
        }

        internal int GetRowIndexOld(Dictionary<string, string> param)
        {
            int rowIndex = 0;
            bool find = false;
            IAccessible accElement = null;
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(_windowName, _elementName);
            }

            accElement = (IAccessible)obj;

            Object[] childs = new Object[accElement.accChildCount];
            int obtain;
            NativeMethods.AccessibleChildren(accElement, 0, accElement.accChildCount, childs, out obtain);

            IAccessible sh = (IAccessible)childs[0];
            Object[] columnNames = new Object[sh.accChildCount];
            NativeMethods.AccessibleChildren(sh, 0, sh.accChildCount, columnNames, out obtain);

            string regString = string.Empty;
            List<string> nameList = new List<string>();
            foreach (object o in columnNames)
            {
                IAccessible child = (IAccessible)o;
                string name = (child.accName.ToString().Split(' '))[0];
                nameList.Add(name);
            }

            foreach (string name in nameList)
            {
                if (param.ContainsKey(name))
                {
                    regString = regString + ";" + param[name];
                }
                else
                {
                    regString = regString + ";.*";
                }
            }
            List<string> ls = new List<string>();
            if (string.IsNullOrEmpty(regString))
            {
                throw new NullControlException(string.Join(",", param.ToList().ConvertAll((x) =>
                {
                    return string.Format("{0}={1}", x.Key, x.Value);
                }).ToArray()), _elementName);
            }

            regString = regString.TrimStart(';');
            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accRole.ToString() == "28")
                {
                    Match m = Regex.Match(child.accValue, regString, RegexOptions.Compiled);

                    if (m.Success)
                    {
                        find = true;
                        break;
                    }
                    else
                    {
                        rowIndex++;
                    }
                }
            }

            if (find)
            {
                return rowIndex;
            }
            else
            {
                string msg = string.Empty;
                foreach (string key in param.Keys)
                {
                    if (msg == string.Empty)
                    {
                        msg = string.Format("{0} = '{1}'", key, param[key]);
                    }
                    else
                    {
                        msg = string.Format("{0} and {1} = '{2}'", msg, key, param[key]);
                    }
                }
                throw new NullControlException(_windowName, _elementName, msg);
            }
        }

        internal int GetRowIndex(Dictionary<string, string> param)
        {
            string filterString = string.Join(" and ", param.ToList().ConvertAll((x) =>
                {
                    return string.Format("{0}='{1}'", x.Key, x.Value);
                }));
            DataRow[] rowArray = _table.Select(filterString);
            if (rowArray.Length > 1)
            {
                throw new MultiControlException(filterString, _elementName);
            }
            else if (rowArray.Length == 0)
            {
                throw new NullControlException(filterString, _elementName);
            }
            else
            {
                return _table.Rows.IndexOf(rowArray[0]) + 1;
            }
        }

        internal void RollScrollBar(double percent, bool vertical = true)
        {
            IAccessible accElement = null;
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(_windowName, _elementName);
            }

            accElement = (IAccessible)obj;

            Object[] childs = new Object[accElement.accChildCount];
            int obtain;
            NativeMethods.AccessibleChildren(accElement, 0, accElement.accChildCount, childs, out obtain);

            if (percent <= 1) percent = percent * 100;
            if (percent == 0) percent = 100;
            if (percent > 100) percent = 100;
            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accRole.ToString() == "3")
                {
                    System.Windows.Rect r = GetCellRect(child);
                    if (r.Width < r.Height && vertical)
                    {
                        element.SetFocus();
                        child.accValue = ((int)percent).ToString();
                        child.accDoDefaultAction();
                        break;
                    }
                    else if (r.Width > r.Height && !vertical)
                    {
                        element.SetFocus();
                        child.accValue = ((int)percent).ToString();
                        child.accDoDefaultAction();
                        break;
                    }
                }
            }
        }

        internal void RollScrollBar(IAccessible cell, bool vertical = true)
        {
            IAccessible accElement = null;
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);
            if (obj == null)
            {
                throw new NullControlException(_windowName, _elementName);
            }

            accElement = (IAccessible)obj;

            Object[] childs = new Object[accElement.accChildCount];
            int obtain;
            NativeMethods.AccessibleChildren(accElement, 0, accElement.accChildCount, childs, out obtain);

            Rect rc = element.Current.BoundingRectangle;
            foreach (Object o in childs)
            {
                IAccessible child = (IAccessible)o;
                if (child.accRole.ToString() == "3")
                {
                    System.Windows.Rect r = GetCellRect(child);
                    if (r.Width < r.Height && vertical)
                    {
                        Rect crc = GetCellRect(cell);
                        int val = int.Parse(child.accValue);
                        element.SetFocus();
                        while (true)
                        {
                            //child.accValue = val.ToString();
                            //child.accDoDefaultAction();
                            crc = GetCellRect(cell);
                            if (rc.Top > crc.Top)
                            {
                                Mouse.Roll(true);
                            }
                            else if (rc.Bottom < crc.Bottom)
                            {
                                Mouse.Roll(false);
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                    else if (r.Width > r.Height && !vertical)
                    {
                        Rect crc = GetCellRect(cell);
                        int val = int.Parse(child.accValue);
                        element.SetFocus();
                        while (true)
                        {
                            //child.accValue = val.ToString();
                            //child.accDoDefaultAction();
                            
                            crc = GetCellRect(cell);
                            if (rc.Left > crc.Left)
                            {
                                Mouse.Click((int)rc.Left + 5, (int)rc.Top + 5);
                                Robot.Recess(20);
                            }
                            else if (rc.Right < crc.Right)
                            {
                                Mouse.Click((int)rc.Right - 5, (int)rc.Bottom - 5);
                                Robot.Recess(20);
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }


        internal int GetRowCount()
        {
            //int count = 0;
            //IAccessible accElement = null;
            //object obj = null;
            //Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");

            //NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);
            //if (obj == null)
            //{
            //    throw new NullControlException(_windowName, _elementName);
            //}

            //accElement = (IAccessible)obj;

            //Object[] childs = new Object[accElement.accChildCount];
            //int obtain;
            //NativeMethods.AccessibleChildren(accElement, 0, accElement.accChildCount, childs, out obtain);

            //foreach (Object o in childs)
            //{
            //    IAccessible child = (IAccessible)o;
            //    if (child.accRole.ToString() == "28")
            //    {
            //        count++;
            //    }
            //}
            return _table.Rows.Count;
        }

        internal string GetContent(int rowIndex, int columnIndex)
        {
            //IAccessible cell = GetAccesibleElement(rowIndex, columnIndex);
            //return cell.accValue;
            if (rowIndex == 0)
            {
                rowIndex = _table.Rows.Count - 1;
            }
            return _table.Rows[rowIndex-1][_table.Columns[columnIndex-1]].ToString();
        }

        internal string GetContent(int rowIndex, string columnName)
        {
            //IAccessible cell = GetAccesibleElement(rowIndex, columnName);
            //return cell.accValue;
            if (rowIndex == 0)
            {
                rowIndex = _table.Rows.Count - 1;
            }
            return _table.Rows[rowIndex - 1][_table.Columns[columnName]].ToString();
        }

        internal DataTable GetData()
        {
            return _table;
        }

        internal void Click(string columnName, int rowIndex, int x, int y)
        {
            IAccessible cell = GetAccesibleElement(rowIndex, columnName);

            Rect rc = GetCellRect(cell);

            x = (int)rc.Left + x;
            y = (int)rc.Top + y;

            Mouse.Click(x, y);
        }

        internal void Click(string columnName, List<int> rowIndexList, int x, int y)
        {
            foreach (int rowIndex in rowIndexList)
            {
                IAccessible cell = GetAccesibleElement(rowIndex, columnName);

                if (!IsCellVisible(cell))
                {
                    RollScrollBar(cell);
                }
                Rect rc = GetCellRect(cell);

                int nx = (int)rc.Left + x;
                int ny = (int)rc.Top + y;

                Mouse.Click(nx, ny);
            }
        }

        private bool IsCellVisible(IAccessible cell)
        {
            Rect rc = GetCellRect(cell);
            Rect wc = element.Current.BoundingRectangle;
            if (rc.Left < wc.Left || rc.Left > wc.Right)
            {
                return false;
            }

            if (rc.Top < wc.Top || rc.Top > wc.Bottom)
            {
                return false;
            }
            return true;
        }
    }
}
