namespace Mock.Tools.Controls
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows.Automation;
    using System.Xml;
    using System.Threading;
    using System.Diagnostics;
    using System.Windows;
    using Mock.Tools.Exception;
    using Mock.Data;
    using Mock.Nature.Native;
    internal abstract class WinObject
    {
        #region 变量
        protected WindowsUnit element = null;
        //private ElementInfo elementInfo;
        //protected static string _CurrentWindowName = string.Empty;
        private List<PropertyCondition> _ConditionList = new List<PropertyCondition>();
        private AutomationElementCollection elements;
        protected string _windowName = null;
        protected string _elementName = null;
        //private static Thread updateWindowThread = null;
        #endregion

        #region 控件方法

        #region 获取窗口

        /// <summary>
        /// 获取窗口元素
        /// </summary>
        /// <param name="windowName"></param>
        protected void GetElement(string windowName)
        {
            LogManager.Start();
            if (RobotContext.ProgramAbort)
            {
                LogManager.Debug("被测程序已崩溃");
                throw new ProgramAbortException();
            }
            while (RobotContext.LatestWindow != null && RobotContext.LatestWindow.FriendlyName != windowName)
            {
                LogManager.Debug("search window is not current window");
                Wait(100);
            }
            //LogManager.Debug("Start get {0}", windowName);

            if (!RobotContext.IsWindowsObjectInitilized)
            {
                Init();
            }

            RobotContext.WindowName = windowName;
            if (!CheckWindow())
            {
                //LogManager.Debug("Check window failed, start find window");
                //RobotContext.Window = null;
                element = FindWindow(windowName);
                RobotContext.Window = element;
            }
            else
            {
                //LogManager.Debug("Check window success, use old window");
                element = RobotContext.Window;
            }

            //_CurrentWindowName = windowName;
            //element = RobotContext.Window;
            //uint windowLong = NativeMethods.GetWindowLong((IntPtr)element.Current.NativeWindowHandle, SetWindowLongOffsets.GWL_STYLE);
            //if ((windowLong & (uint)WindowStyles.WS_DISABLED) == (uint)WindowStyles.WS_DISABLED)
            //{
            //    throw new WindowIsInactiveException(windowName);
            //}
            if (element == null)
            {
                throw new NullControlException(windowName);
            }

            //韩志强修改于2017.06.26
            //解决FindWindow 时报WarningWindowExistExption异常，如果报此异常，说明窗口存在
            //韩志强修改于2017.09.29
            //解决CloseTo时触发关闭窗口的问题
            if (RobotContext.IsWarningWindowExist)
            {
                throw new WarningWindowExistException();
            }

            if (!element.Current.IsEnabled)
            {
                throw new WindowIsInactiveException(windowName);
            }


            IntPtr popupWindow = NativeMethods.GetWindow(element.Current.NativeWindowHandle, GetWindowOffsets.GW_ENABLEDPOPUP);
            if (popupWindow != IntPtr.Zero)
            {
                WindowsUnit unit = WindowsUnit.FromHandle(popupWindow);
                if (unit.Current.ControlType == ControlType.Window)
                {
                    LogManager.Debug("有子窗口？？？？？？？？？？？？？？？？？？？");
                    throw new WindowIsInactiveException(windowName);
                }
            }

            if (NativeMethods.IsHungAppWindow(element.Current.NativeWindowHandle))
            {
                throw new WindowNoResponseException(windowName);
            }

            //LogManager.Debug("Get window element success");
        }

        protected List<WindowsUnit> GetActiveWindow()
        {
            LogManager.Start();
            if (!RobotContext.IsWindowsObjectInitilized)
            {
                Init();
            }

            List<WindowsUnit> windowList = new List<WindowsUnit>();
            List<WindowBaseInfo> tmpList = RobotContext.getAllWindow();
            //tmpList.Clear();
            //tmpList.AddRange(activeWindowHandleList);
            if (tmpList == null || tmpList.Count == 0)
            {
                ManualUpdateActiveWindow();
            }
            for (int i = 0; i < tmpList.Count; i++)
            {
                IntPtr hWnd = tmpList[i].Handle;
                try
                {
                    uint windowLong = NativeMethods.GetWindowLong(hWnd, SetWindowLongOffsets.GWL_STYLE);
                    if ((windowLong & (uint)WindowStyles.WS_DISABLED) == (uint)WindowStyles.WS_DISABLED)
                    {
                        continue;
                    }
                    else
                    {
                        windowList.Add(AutomationElement.FromHandle(hWnd));
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch { }
            }

            return windowList;
        }
        #endregion

        #region 依据窗体名称和控件名称获取控件
        /// <summary>
        /// 依据窗体名称和控件名称获取控件
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="elementName"></param>
        protected void GetElement(string windowName, string elementName)
        {
            LogManager.Start();
            Exception ex = null;
            for (int i = 0; i < Config.RedoCount; i++)
            {
                try
                {
                    if (!RobotContext.IsWindowsObjectInitilized)
                    {
                        Init();
                    }

                    RobotContext.WindowName = windowName;
                    RobotContext.ElementName = elementName;
                    if (!CheckWindow())
                    {
                        try
                        {
                            RobotContext.Window = FindWindow(windowName);
                        }
                        catch (NullConditionException)
                        {
                            ManualUpdateActiveWindow();
                            RobotContext.Window = FindWindow(windowName);
                        }
                    }

                    ElementInfo info = GetElementByFriendlyName(windowName, elementName);
                    //elementInfo = info;

                    if (!string.IsNullOrEmpty(info.RelativePath))
                    {
                        WindowsUnit parent = RobotContext.getRelativePath(info.RelativePath);
                        FindElement(parent, info, info.RelativePath);
                    }
                    else
                    {
                        FindElement(info, windowName);
                    }

                    //for (int j = 0; j < Config.RedoCount; j++)
                    //{
                    //    //try
                    //    //{
                    //    //    element.SetFocus();
                    //    //}
                    //    //catch
                    //    //{
                    //    //}
                    //    if (element.Current.IsEnabled)
                    //    {
                    //        break;
                    //    }
                    //    Robot.Recess(100);
                    //}
                    return;
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex1)
                {
                    ex = ex1;
                    //LogManager.ErrorOnlyPrint(ex);
                    Robot.Recess(100);
                }
            }
            if (ex != null)
            {
                LogManager.ErrorOnlyPrint(ex);
            }
            throw new NullControlException(windowName, elementName);
        }
        #endregion

        #region 依据指定的条件从父控件获取子控件
        /// <summary>
        /// 依据指定的条件从父控件获取子控件
        /// </summary>
        /// <param name="parent">父控件</param>
        /// <param name="cond">查询条件</param>
        /// <param name="parentName">父窗体名称</param>
        /// <param name="friendlyName">窗体名称</param>
        /// <returns>子控件</returns>
        protected WindowsUnit FindChild(WindowsUnit parent, Condition cond, string parentName, string friendlyName)
        {
            for (int i = 0; i < Config.RedoCount; i++)
            {
                AutomationElementCollection childs = parent.FindAll(TreeScope.Children, cond);
                Wait(10);
                if (childs.Count == 1)
                {
                    return childs[0];
                }
                else if (childs.Count > 1)
                {
                    throw new MultiControlException(parentName, friendlyName);
                }
                else
                {
                    Wait(100);
                    //throw new NullControlException(parentName, friendlyName);
                }
            }
            throw new NullControlException(parentName, friendlyName);
        }
        #endregion

        #region 等待
        protected void Wait(int millionSecond)
        {
            Thread.Sleep(millionSecond);
        }
        #endregion

        #region 比较窗体是否为所查窗体
        protected bool CheckControl(string windowName, WindowsUnit windowElement)
        {
            LogManager.Start();
            if (windowElement == null)
            {
                return false;
            }

            WinInfo info = GetWindowByFriendlyName(windowName);

            if (!info.AutomationId.Equals(string.Empty) && !info.AutomationId.Equals(windowElement.Current.AutomationId))
            {
                return false;
            }
            if (info.Type != null && !info.Type.Equals(windowElement.Current.ControlType))
            {
                return false;
            }
            if (!info.Name.Equals(string.Empty) && !info.Name.Equals(windowElement.Current.Name))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 获取控件内的控件
        protected WindowsUnit InnerElement(WindowsUnit parent, Condition cond, Rect rect, string parentName, string friendlyName)
        {
            LogManager.Start();
            AutomationElementCollection childs = parent.FindAll(TreeScope.Children, cond);
            List<WindowsUnit> resList = new List<WindowsUnit>();
            foreach (AutomationElement ae in childs)
            {
                Rect cr = ae.Current.BoundingRectangle;

                Point p1 = new Point(cr.Left + 3, cr.Top + 3);
                Point p2 = new Point(cr.Right - 5, cr.Bottom - 5);
                if (rect.Contains(p1) && rect.Contains(p2))
                {
                    resList.Add(ae);
                }
                Wait(10);
            }
            if (resList.Count == 1)
            {
                return resList[0];
            }
            else if (resList.Count > 1)
            {
                throw new MultiControlException(parentName, friendlyName);
            }
            else
            {
                throw new NullControlException(parentName, friendlyName);
            }
        }
        #endregion

        #region 获取控件名称
        protected List<string> GetControlName(string type, string name)
        {
            LogManager.Start();
            List<string> nameList = new List<string>();
            nameList.Add(name);
            XmlDocument doc = GetXml("ControlName");
            VerInfo vi = Robot.GetSoftwareVersion();
            
            XmlNodeList nodeList = doc.SelectNodes(string.Format("//{0}/ControlName[@friendlyName='{1}']", type, name));
            if (nodeList == null)
            {
                return nameList;
            }
            string tmpName = name;
            foreach (XmlNode node in nodeList)
            {
                XmlAttribute xa = node.Attributes["Version"];
                if (xa == null)
                {
                    continue;
                }
                string[] tmp = xa.Value.Split(',');
                if (tmp.Contains(vi.VerId))
                {
                    tmpName = node.InnerText;
                    break;
                }
            }

            string []tmpN = tmpName.Split(',');
            foreach (string n in tmpN)
            {
                if (!string.IsNullOrEmpty(n))
                {
                    nameList.Add(n);
                }
            }
            return nameList;
        }
        #endregion

        #region 设置鼠标指针
        protected void SetCursor(WindowsUnit automationElement, bool click = false)
        {
            LogManager.Start();
            Rect rect = automationElement.Current.BoundingRectangle;
            if (click)
            {
                Mouse.Click((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            }
            else
            {
                //Mouse.Move((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            }
            //Wait(20);
        }
        #endregion

        #region 更新活动窗口
        protected void UpdateActiveWindow(object state)
        {
            RobotContext.UpdateActiveWindowEvent.Set();
        }


        #endregion

        #region 执行动作
        //protected AutoResetEvent reset = new AutoResetEvent(false);
        //protected Exception ActionException = null;
        //protected void DoAction1(ParameterizedThreadStart action, object param)
        //{
        //    LogManager.Start();
        //    ActionException = null;
        //    reset.Reset();
        //    Thread t = new Thread(action);
        //    t.IsBackground = true;
        //    t.Start(param);
            
        //    bool ret = reset.WaitOne(2000);
        //    if (ret)
        //    {
        //        //韩志强 2017.04.21
        //        //点击按钮会阻塞测试脚本执行，故不在此判断
        //        //if (RobotContext.IsWarningWindowExist)
        //        //{
        //        //    throw new WarningWindowExistException();
        //        //}
        //        LogManager.Debug("wait true");
        //        if (ActionException != null)
        //        {

        //            throw ActionException;
        //        }
        //    }
        //    else
        //    {
        //        if (t.ThreadState != System.Threading.ThreadState.Stopped)
        //        {
        //            try
        //            {
        //                t.Abort();
        //            }
        //            catch { }
        //        }
        //    }
        //}
        #endregion

        #region 手动更新
        protected void ManualUpdateActiveWindow()
        {
            NativeMethods.EnumWindows(EnumWindowsProc, 0);
        }
        #endregion

        #endregion

        #region 私有方法

        #region 初始化
        private void Init()
        {
            LogManager.Start();
            ManualUpdateActiveWindow();
            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Descendants, OnWindowOpen);
            Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent, AutomationElement.RootElement, TreeScope.Subtree, OnWindowClose);
            
            RobotContext.UpdateActiveWindowEvent = new AutoResetEvent(false);
            RobotContext.UpdateTimer = new Timer(UpdateActiveWindow, null, 1, 1000);

            Thread t = new Thread(new ThreadStart(CheckWindowError));
            t.IsBackground = true;
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            RobotContext.IsWindowsObjectInitilized = true;
        }
        #endregion

        #region 获取控件的方法
        private void FindElement(ElementInfo info, string windowName)
        {
            LogManager.Start();
            _ConditionList.Clear();
            if (!string.IsNullOrEmpty(info.AutomationId))
            {
                PropertyCondition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, info.AutomationId);
                elements = RobotContext.Window.FindAll(TreeScope.Children, condition);
                if (elements.Count == 1)
                {
                    element = elements[0];
                    return;
                }
                else if (elements.Count > 1)
                {
                    throw new MultiControlException(windowName, info.FriendlyName);
                }
                else
                {
                    throw new NullControlException(windowName, info.FriendlyName);
                }
            }
            else
            {
                int index = -1;
                if (string.IsNullOrEmpty(info.RelativePosition) || !int.TryParse(info.RelativePosition, out index))
                {
                    throw new NullControlException(windowName, info.FriendlyName);
                }
                else
                {
                    AutomationElement tmp = TreeWalker.ControlViewWalker.GetFirstChild(RobotContext.Window);
                    List<AutomationElement> childList = new List<AutomationElement>();
                    childList.Clear();
                    while (tmp != null)
                    {
                        childList.Insert(0, tmp);
                        tmp = TreeWalker.ControlViewWalker.GetNextSibling(tmp);
                    }
                    element = childList[index - 1];
                    childList.Clear();
                }
            }
        }

        private void FindElement(WindowsUnit parent, ElementInfo info, string parendName)
        {
            LogManager.Start();
            if (parent == null)
            {
                throw new NullControlException(parendName);
            }
            _ConditionList.Clear();

            if (!string.IsNullOrEmpty(info.AutomationId))
            {
                PropertyCondition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, info.AutomationId);
                elements = parent.FindAll(TreeScope.Children, condition);
                if (elements.Count == 1)
                {
                    element = elements[0];
                    return;
                }
                else if (elements.Count > 1)
                {
                    throw new MultiControlException(parendName, info.FriendlyName);
                }
                else
                {
                    throw new NullControlException(parendName, info.FriendlyName);
                }
            }
            else
            {
                int index = -1;
                if (string.IsNullOrEmpty(info.RelativePosition) || !int.TryParse(info.RelativePosition, out index))
                {
                    throw new NullControlException(parendName, info.FriendlyName);
                }
                else
                {
                    WindowsUnit tmp = TreeWalker.ControlViewWalker.GetFirstChild(parent);
                    List<WindowsUnit> childList = new List<WindowsUnit>();
                    childList.Clear();
                    while (tmp != null)
                    {
                        childList.Insert(0, tmp);
                        tmp = TreeWalker.ControlViewWalker.GetNextSibling(tmp);
                    }
                    element = childList[index - 1];
                    childList.Clear();
                }
            }
        }

        private ElementInfo GetElementByFriendlyName(string windowName, string elementName)
        {
            LogManager.Start();
            WinInfo winfo = GetWindowByFriendlyName(windowName);
            XmlDocument winDoc = new XmlDocument();
            winDoc.LoadXml(winfo.ToXml());
            XmlNode node = winDoc.DocumentElement;
            node = node.SelectSingleNode(string.Format("//ElementInfo[@FriendlyName='{0}']", elementName));
            if (node == null)
            {
                throw new CanNotFindNodeException(Config.SoftwareProcessName, string.Format("{0}/{1}", windowName, elementName));
            }
            ElementInfo info = DataFactory.XmlToObject<ElementInfo>(node);
            return info;
        }

        private ElementInfo GetElementById(string windowName, int elementId)
        {
            LogManager.Start();
            WinInfo winfo = GetWindowByFriendlyName(windowName);
            XmlDocument winDoc = new XmlDocument();
            winDoc.LoadXml(winfo.ToXml());
            XmlNode node = winDoc.DocumentElement;
            node = node.SelectSingleNode(string.Format("ElementInfo[@id={0}]", elementId));
            if (node == null)
            {
                throw new CanNotFindNodeException(Config.SoftwareProcessName, windowName, elementId);
            }
            ElementInfo info = DataFactory.XmlToObject<ElementInfo>(node);
            return info;
        }
        #endregion

        #region 获取窗口的方法
        private bool CheckWindow()
        {
            LogManager.Start();
            if (RobotContext.Window == null)
            {
                return false;
            }

            WindowsUnit unit = RobotContext.Window;

            if (!RobotContext.containWindow(RobotContext.Window))
            {
                return false;
            }

            try
            {
                WinInfo info = GetWindowByFriendlyName(RobotContext.WindowName);
                if (info == null)
                {
                    return false;
                }
                if (string.IsNullOrEmpty(info.AutomationId))
                {
                    if (string.Equals(info.Name, unit.Current.Name) && unit.Current.ControlType == GetType(info.Type))
                    {
                        if (unit.Current.IsOffscreen)
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (string.Equals(unit.Current.AutomationId, info.AutomationId))
                    {
                        if (unit.Current.IsOffscreen)
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogManager.Debug("Check failed : " + ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }

        private WindowsUnit FindWindow(string windowName)
        {
            LogManager.Start();
            //LogManager.Debug("start find window {0}", windowName);
            try
            {
                WinInfo info = GetWindowByFriendlyName(windowName);
                List<WindowBaseInfo> tmpList = RobotContext.getAllWindow();
                if (tmpList.Count > 0)
                {
                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        WindowBaseInfo wb = tmpList[i];
                        try
                        {
                            if (NativeMethods.IsWindow(wb.Handle))
                            {
                                //WindowsUnit e = AutomationElement.FromHandle(windowHandle);
                                if (string.Equals(wb.FriendlyName, windowName))
                                {
                                    return WindowsUnit.FromHandle(wb.Handle);
                                }
                                if (string.IsNullOrEmpty(info.AutomationId))
                                {
                                    if (string.Equals(info.Name, wb.WindowName) && string.Equals(GetType(info.Type).ProgrammaticName, wb.Type))
                                    {
                                        return WindowsUnit.FromHandle(wb.Handle);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    //2017.09.30 部分时候AutomationId为空，需要进一步通过名称验证
                                    if (string.IsNullOrEmpty(wb.AutomationId))
                                    {
                                        if (string.Equals(wb.WindowName, windowName))
                                        {
                                            try
                                            {
                                                WindowsUnit tmpWindow = WindowsUnit.FromHandle(wb.Handle);
                                                FindElement(tmpWindow, info.ElementInfo[0], windowName);
                                                return tmpWindow;
                                            }
                                            catch(NullControlException)
                                            {
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (string.Equals(wb.AutomationId, info.AutomationId))
                                        {
                                            return AutomationElement.FromHandle(wb.Handle);
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        catch (ThreadAbortException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            LogManager.ErrorOnlyPrint(ex);
                        }
                    }

                    //2017.07.04 韩志强修改
                    //对于对象库中存在的窗口严格按照对象库内容搜索
                    if (!string.IsNullOrEmpty(info.Name))
                    {
                        IntPtr hWnd = NativeMethods.FindWindow(null, info.Name);
                        if (hWnd != IntPtr.Zero)
                        {
                            try
                            {
                                WindowsUnit unit = AutomationElement.FromHandle(hWnd);
                                if (string.IsNullOrEmpty(info.AutomationId))
                                {
                                    if (unit.Current.ControlType == GetType(info.Type))
                                    {
                                        return unit;
                                    }
                                }
                                else
                                {
                                    if (string.Equals(unit.Current.AutomationId, info.AutomationId))
                                    {
                                        return unit;
                                    }
                                }
                            }
                            catch (ThreadAbortException)
                            {
                                throw;
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch(Exception ex)
            {
                //LogManager.Error(ex);
                LogManager.ErrorOnlyPrint(ex);
                IntPtr hWnd = NativeMethods.FindWindow(null, windowName);
                try
                {
                    return AutomationElement.FromHandle(hWnd);
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception) { }
            }
            //ManualUpdateActiveWindow();
            throw new NullControlException(windowName);
        }

        private WindowsUnit NativeFindWindow(string windowName)
        {
            Process[] ps = Process.GetProcessesByName(Config.SoftwareProcessName);
            if (ps == null || ps.Length == 0)
            {
                throw new NullControlException(windowName);
            }

            int pid = ps[0].Id;

            IntPtr hWnd = NativeMethods.FindWindow(null, windowName);
            if (hWnd == IntPtr.Zero)
            {
                throw new NullControlException(windowName);
            }
            
                WindowsUnit unit = null;
            try
            {
                unit = AutomationElement.FromHandle(hWnd);
            }
            catch
            {
                throw new NullControlException(windowName);
            }

            if (pid != unit.Current.NativeWindowHandle)
            {
                throw new NullControlException(windowName);
            }

            return unit;
        }

        private static bool EnumWindowsProc(IntPtr hWnd, int lParam)
        {
            uint windowLong = NativeMethods.GetWindowLong(hWnd, SetWindowLongOffsets.GWL_STYLE);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if ((windowLong & (uint)WindowStyles.WS_VISIBLE) == (uint)WindowStyles.WS_VISIBLE)
            {
                if (!RobotContext.containWindow(hWnd))
                {
                    AutomationElement w = AutomationElement.FromHandle(hWnd);
                    GC.Collect();
                    RobotContext.setWindow(w);
                    NativeMethods.EnumChildWindows(hWnd, EnumChildWindowsProc, lParam);
                }
            }
            return true;
        }

        private static bool EnumChildWindowsProc(IntPtr hWnd, int lParam)
        {
            //if (!RobotContext.containWindow(hWnd))
            //{
            //    try
            //    {
            //        //AutomationElement e = AutomationElement.FromHandle(hWnd);
            //        uint windowLong = 0;
            //        if (NativeMethods.IsWindow(hWnd))
            //        {
            //            WindowsUnit e = AutomationElement.FromHandle(hWnd);

            //            if (e.Current.ControlType == ControlType.Window)
            //            {
            //                windowLong = NativeMethods.GetWindowLong(hWnd, SetWindowLongOffsets.GWL_STYLE);
            //                if ((windowLong & (uint)WindowStyles.WS_VISIBLE) == (uint)WindowStyles.WS_VISIBLE)
            //                {
            //                    if ((windowLong & (uint)WindowStyles.WS_DISABLED) != (uint)WindowStyles.WS_DISABLED)
            //                    {
            //                        RobotContext.setWindow(AutomationElement.FromHandle(hWnd));
            //                        NativeMethods.EnumChildWindows(hWnd, EnumChildWindowsProc, lParam);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch { }
            uint windowLong = NativeMethods.GetWindowLong(hWnd, SetWindowLongOffsets.GWL_STYLE);
            if ((windowLong & (uint)WindowStyles.WS_VISIBLE) == (uint)WindowStyles.WS_VISIBLE)
            {
                windowLong = NativeMethods.GetWindowLong(hWnd, SetWindowLongOffsets.GWL_EXSTYLE);
                if ((windowLong & 0x00000040L) == 0x00000040L)
                {
                    if (!RobotContext.containWindow(hWnd))
                    {
                        AutomationElement w = AutomationElement.FromHandle(hWnd);
                        GC.Collect();
                        RobotContext.setWindow(w);
                    }
                }
            }
            return true;
        }

        private WinInfo GetWindowByFriendlyName(string windowName)
        {
            LogManager.Start();
            XmlDocument doc = GetXml(Config.SoftwareProcessName);
            if (doc == null)
            {
                throw new CanNotFindNodeException(Config.SoftwareProcessName, windowName);
            }
            XmlNodeList windowList = doc.SelectNodes(string.Format("//WinInfo[@FriendlyName='{0}']", windowName));
            if (windowList == null || windowList.Count < 1)
            {
                throw new CanNotFindNodeException(Config.SoftwareProcessName, windowName);
            }
            XmlNode node = null;
            VerInfo vi = Robot.GetSoftwareVersion();
            foreach (XmlNode window in windowList)
            {
                string []Vers = window.Attributes["Version"].Value.Split(',');
                List<string> verList = new List<string>();
                foreach (string ver in Vers)
                {
                    verList.Add(ver);
                }
                if (verList.Contains(vi.VerId))
                {
                    node = window;
                    break;
                }
            }
            if (node == null)
            {
                throw new CanNotFindNodeException(Config.SoftwareProcessName, windowName);
            }
            XmlDocument winDoc = new XmlDocument();
            winDoc.LoadXml(node.OuterXml);
            WinInfo info = DataFactory.GetData<WinInfo>(winDoc, windowName);
            return info;
        }

        private WinInfo GetWindowById(int id)
        {
            LogManager.Start();
            XmlDocument doc = GetXml(Config.SoftwareProcessName);
            XmlNodeList windowList = doc.SelectNodes(string.Format("//Window[@id={0}]", id));
            if (windowList == null || windowList.Count < 1)
            {
                throw new CanNotFindNodeException(Config.SoftwareProcessName, "Window", id);
            }

            XmlNode node = null;

            foreach (XmlNode window in windowList)
            {
                string[] Vers = window.Attributes["Version"].Value.Split(',');
                if (Vers.Contains(Robot.GetSoftwareVersion().VerId))
                {
                    node = window;
                    break;
                }
            }

            if (node == null)
            {
                throw new CanNotFindNodeException(Config.SoftwareProcessName, "Window", id);
            }

            XmlDocument winDoc = new XmlDocument();
            winDoc.LoadXml(node.OuterXml);
            WinInfo info = DataFactory.GetData<WinInfo>(winDoc, node.Attributes["FriendlyName"].Value);
            return info;
        }
        #endregion

        #region 获取控件类型
        /// <summary>
        /// 获取控件的类型
        /// </summary>
        /// <param name="type">类型名称</param>
        /// <returns>控件类型，为<c>System.Windows.Automation.ControlType</c>的一个类型</returns>
        private ControlType GetType(string type)
        {
            Type t = typeof(ControlType);
            System.Reflection.FieldInfo mis = t.GetField(type);
            if (mis == null)
            {
                throw new InvalidTypeException(type);
            }

            return (ControlType)mis.GetValue(null);
        }
        #endregion

        private Rect GetBoundingRect(Rect parentRect, string relativePosition)
        {
            LogManager.Start();
            Rect rect = new Rect();
            string tmp = string.Empty;
            int value = 0;
            string[] s = relativePosition.Split(',');
            if (s.Length != 4)
            {
                throw new InvalidInputException("RelativePosition");
            }

            #region SetX
            if (s[0].StartsWith("s"))
            {
                tmp = s[0].Substring(1);
                if (tmp.StartsWith("-"))
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.X = 0 - value;
                }
                else
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.X = value;
                }
            }
            else
            {
                tmp = s[0];
                if (tmp.StartsWith("-"))
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.X = 0 - value + parentRect.X;
                }
                else
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.X = value + parentRect.X;
                }
            }
            #endregion

            #region SetY
            if (s[1].StartsWith("s"))
            {
                tmp = s[1].Substring(1);
                if (tmp.StartsWith("-"))
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Y = 0 - value;
                }
                else
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Y = value;
                }
            }
            else
            {
                tmp = s[1];
                if (tmp.StartsWith("-"))
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Y = 0 - value + parentRect.Y;
                }
                else
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Y = value + parentRect.Y;
                }
            }
            #endregion

            #region SetWidth
            if (s[2].StartsWith("s"))
            {
                tmp = s[2].Substring(1);
                if (tmp.StartsWith("-"))
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Width = 0 - value;
                }
                else
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Width = value;
                }
            }
            else
            {
                tmp = s[2];
                if (tmp.StartsWith("-"))
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Width = 0 - value + parentRect.Width;
                }
                else
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Width = value + parentRect.Width;
                }
            }
            #endregion

            #region SetHeight
            if (s[3].StartsWith("s"))
            {
                tmp = s[3].Substring(1);
                if (tmp.StartsWith("-"))
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Height = 0 - value;
                }
                else
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Height = value;
                }
            }
            else
            {
                tmp = s[3];
                if (tmp.StartsWith("-"))
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Height = 0 - value + parentRect.Height;
                }
                else
                {
                    tmp = tmp.Substring(1);
                    value = int.Parse(tmp);
                    rect.Height = value + parentRect.Height;
                }
            }
            #endregion

            return rect;
        }

        private XmlDocument GetXml(string name)
        {
            XmlDocument doc = ControlInfo.getInstance()[name];
            if (doc == null)
            {
                throw new CanNotFindNodeException(Config.SoftwareProcessName, _windowName);
            }
            return doc;
        }

        private void OnWindowOpen(object src, AutomationEventArgs e)
        {
            if (e.EventId == WindowPattern.WindowOpenedEvent)
            {
                try
                {
                    DateTime dt = DateTime.Now;
                    AutomationElement activeWindowElement = src as AutomationElement;
                    string tmpName = "no name";
                    try
                    {
                        tmpName = activeWindowElement.Current.Name;
                        if (string.IsNullOrEmpty(tmpName))
                        {
                            tmpName = "no name";
                        }
                    }
                    catch
                    {

                    }
                    LogManager.Debug(string.Format("Window named {0} open", tmpName));

                    RobotContext.InvokeIgnoreWindow(activeWindowElement.Current.Name, activeWindowElement.Current.AutomationId, activeWindowElement.Current.ClassName);
                    
                    //2017.09.14 存在打开的窗口马上关闭，第一次触发时的状态不对等情况，故记录最后一次
                    if (!RobotContext.containWindow(activeWindowElement))
                    {
                        RobotContext.setWindow(activeWindowElement, dt);
                    }
                }
                catch (Exception)
                {
                    //LogManager.Error(ex);
                }
            }
        }

        private void OnWindowClose(object src, AutomationEventArgs e)
        {
            if (e.EventId == WindowPattern.WindowClosedEvent)
            {
                try
                {
                    WindowClosedEventArgs wcea = e as WindowClosedEventArgs;
                    string runtimeId = string.Join(",", wcea.GetRuntimeId());
                    if (RobotContext.containWindow(runtimeId))
                    {
                        RobotContext.RunCloseEvent(runtimeId);
                        RobotContext.rmvWindow(runtimeId);
                    }
                }
                catch { }
            }
        }

        private static void CheckWindowError()
        {
            while (true)
            {
                //2017.09.08 如果vista 以上系统出现此进程，有窗口已经不再可用，需先关闭此窗口
                Process[] psList = Process.GetProcessesByName("werfault");
                if (psList != null && psList.Length > 0)
                {
                    //LogManager.Debug("程序中有未处理的异常");
                    foreach (Process ps in psList)
                    {
                        try
                        {
                            ps.Kill();
                        }
                        catch { }
                    }
                    Robot.Recess(1000);
                    Process[] psl = Process.GetProcessesByName(Config.SoftwareProcessName);
                    if (psl == null || psl.Length == 0)
                    {
                        RobotContext.ReportAbort();
                    }
                }

                //2017.10.10 如果winXP 以上系统出现此进程，有窗口已经不再可用，需先关闭此窗口
                
                psList = Process.GetProcessesByName("dwwin");
                if (psList != null && psList.Length > 0)
                {
                    foreach (Process ps in psList)
                    {
                        try
                        {
                            ps.Kill();
                        }
                        catch { }
                    }
                    //LogManager.Debug("程序中有未处理的异常");
                    Robot.Recess(1000);
                    Process[] psl = Process.GetProcessesByName(Config.SoftwareProcessName);
                    if (psl == null || psl.Length == 0)
                    {
                        RobotContext.ReportAbort();
                    }
                }

                RobotContext.UpdateActiveWindowEvent.Reset();
                RobotContext.UpdateActiveWindowEvent.WaitOne();
            }
        }
        #endregion
    }
}
