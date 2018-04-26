namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using System.Threading;
    using System.Diagnostics;
    using System;
    using System.Runtime.InteropServices;
    using Mock.Tools.Exception;
    using Mock.Nature.Native;
    using System.Collections.Generic;
    using Mock.Tools.Tasks;

    /// <summary>
    /// 表示窗口打开时的事件处理程序
    /// </summary>
    /// <param name="baseInfo"></param>
    public delegate void WindowOpenEventHandler(WindowBaseInfo baseInfo);
    
    /// <summary>
    /// 表示窗口关闭时的事件处理程序
    /// </summary>
    /// <param name="baseInfo"></param>
    public delegate void WindowCloseEventHandler(WindowBaseInfo baseInfo);

    internal class WindowObject : WinObject
    {
        //private object locker = new object();
        private bool closeTo;
        //2017.09.15 加入真实的要查找的窗口名称
        private string _realName;
        internal WindowObject(string winName)
        {
            _realName = winName;
            _windowName = winName;
            _elementName = null;
            closeTo = false;
        }

        internal WindowObject(AutomationElement e)
        {
            element = e;
        }

        internal void Search()
        {
            // 先检查窗口名和当前获得的窗口的名称是否一致
            if (string.Equals(_realName, RobotContext.WindowName))
            {
                //一致的话看看窗口是否有效
                try
                {
                    GetElement(_realName);
                    //窗口可用，返回
                    //hWnd = (IntPtr)element.Current.NativeWindowHandle;

                    ////2017.09.14 清除所有弹出窗口
                    ////NativeMethods.ShowOwnedPopups(hWnd, false);
                    //element.SetFocus();
                    //if (NativeMethods.IsIconic(hWnd))
                    //{
                    //    NativeMethods.ShowWindowAsync(hWnd, 9);
                    //}
                    //NativeMethods.SetForegroundWindow(hWnd);
                    //System.Windows.Rect r = element.Current.BoundingRectangle;
                    //Mouse.Click(r.Right - 2, r.Bottom - 2);
                    SetTop();
                    return;
                }
                catch (WarningWindowExistException)
                {
                    throw;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (ProgramAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (!closeTo)
                    {
                        LogManager.ErrorOnlyPrint(ex);
                    }
                }
                //其他异常先不处理
            }

            //窗口名变化或者窗口已经不再可用，重新查找
            //RobotContext.WindowName = _realName;

            if (closeTo)
            {
                try
                {
                    Robot.ExecuteWithTimeOut(() =>
                    {
                        while (true)
                        {
                            try
                            {
                                GetElement(_realName);
                                break;
                            }
                            catch (NullControlException)
                            {
                                Robot.Recess(500);
                            }
                        }
                    }, 60000);
                }
                catch (TimeOutException)
                {
                    throw new NullControlException(_realName);
                }
            }
            else
            {
                //开始查找窗口，60s为间隔
                for (int i = 0; i < Config.RedoCount; i++)
                {
                    try
                    {
                        Robot.ExecuteWithTimeOut(() =>
                            {
                                while (!Find())
                                {
                                    if (RobotContext.IsWarningWindowExist)
                                    {
                                        throw new WarningWindowExistException();
                                    }
                                    Wait(200);
                                }
                            }, 60000);
                        break;
                    }
                    catch (TimeOutException)
                    {
                        //超时重做
                        ManualUpdateActiveWindow();
                        RobotContext.LastActionSuccess = false;
                    }
                }

                //已经找到窗口或重试次数已到，判断窗口状态是否为Inactive
                try
                {
                    GetElement(_realName);
                }
                catch (WindowIsInactiveException)
                {
                    //窗口无法激活
                    for (int i = 0; i < Config.RedoCount; i++)
                    {
                        try
                        {
                            Robot.ExecuteWithTimeOut(() =>
                            {
                                while (true)
                                {
                                    try
                                    {
                                        //先关闭弹出窗口
                                        //NativeMethods.ShowOwnedPopups(element.Current.NativeWindowHandle, false);
                                        GetElement(_realName);
                                        break;
                                    }
                                    catch (WindowIsInactiveException)
                                    {
                                        NativeMethods.SetActiveWindow(element.Current.NativeWindowHandle);
                                        Robot.Recess(100);
                                    }
                                }
                            }, 10000);
                            break;
                        }
                        catch (TimeOutException)
                        {
                            RobotContext.LastActionSuccess = false;
                            while (RobotContext.LatestWindow != null && RobotContext.LatestWindow.Handle != (IntPtr)element.Current.NativeWindowHandle)
                            {
                                Robot.Recess(100);
                            }
                        }
                    }

                    GetElement(_realName);
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch (ProgramAbortException)
                {
                    throw;
                }
                catch (NullControlException ex)
                {
                    LogManager.DebugX("window {0} occur exception {1} \n {2}", _realName, ex.Message, ex.StackTrace);
                    throw new TimeOutException("Search " + _windowName);
                }
                finally
                {
                    RobotContext.LastActionSuccess = true;
                }
            }

            //try
            //{
            //    element.SetFocus();
            //}
            //catch (Exception) { }
            //hWnd = (IntPtr)element.Current.NativeWindowHandle;
            //if (NativeMethods.IsIconic(hWnd))
            //{
            //    NativeMethods.ShowWindowAsync(hWnd, 9);
            //}
            //NativeMethods.SetForegroundWindow(hWnd);
            ////2017.09.14 清除所有弹出窗口
            ////NativeMethods.ShowOwnedPopups(hWnd, false);
            //System.Windows.Rect r1 = element.Current.BoundingRectangle;
            //Mouse.Click(r1.Right - 2, r1.Bottom - 2);

            SetTop();
        }

        internal bool Find()
        {
            try
            {
                GetElement(_realName);
                if (NativeMethods.IsHungAppWindow((IntPtr)element.Current.NativeWindowHandle))
                {
                    throw new WindowNoResponseException(_realName);
                }
                return true;
            }
            catch (NullControlException)
            {
                return false;
            }
            catch (WindowIsInactiveException)
            {
                return true;
            }
            catch (WarningWindowExistException)
            {
                //韩志强修改于2017.06.26
                //在GetElement中先查找窗口，如果找不到，将抛出NullControlException

                return true;
            }
        }

        internal bool IsActive()
        {
            if (element == null)
            {
                throw new NullControlException(_realName);
            }

            uint windowLong = NativeMethods.GetWindowLong((IntPtr)element.Current.NativeWindowHandle, SetWindowLongOffsets.GWL_STYLE);
            if ((windowLong & (uint)WindowStyles.WS_DISABLED) == (uint)WindowStyles.WS_DISABLED)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal void SetTopMost()
        {
            Search();
            SetTop();
            //if (!element.Current.IsEnabled)
            //{
            //    throw new ControlUnableException(_realName);
            //}
            //IntPtr hWnd = (IntPtr)element.Current.NativeWindowHandle;
            
            //if (NativeMethods.IsIconic(hWnd))
            //{
            //    //NativeMethods.PostMessage(hWnd, WindowsMessages.WM_SYSCOMMAND, (int)WindowsMessages.SC_RESTORE, 0);
            //    NativeMethods.ShowWindow(hWnd, 4);
            //}
            //NativeMethods.SetForegroundWindow(hWnd);
            //WindowPattern wp = element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            //if (wp == null)
            //{
            //    throw new UnSupportPatternException();
            //}
            //if (wp.Current.CanMaximize)
            //{
            //    wp.SetWindowVisualState(WindowVisualState.Maximized);
            //}
            //else
            //{
            //    wp.SetWindowVisualState(WindowVisualState.Normal);
            //}
        }

        private void SetTop()
        {
            if (element == null) return;
            IntPtr hWnd = (IntPtr)element.Current.NativeWindowHandle;

            //2017.09.14 清除所有弹出窗口
            //NativeMethods.ShowOwnedPopups(hWnd, false);
            element.SetFocus();
            if (NativeMethods.IsIconic(hWnd))
            {
                NativeMethods.ShowWindowAsync(hWnd, 9);
            }
            NativeMethods.SetForegroundWindow(hWnd);
            //if (NativeMethods.GetForegroundWindow() != hWnd)
            //{
            //    NativeMethods.SetWindowPos(hWnd, SetWindowPosParameters.HWND_TOPMOST, 0, 0, 0, 0, SetWindowPosParameters.SWP_NOSIZE | SetWindowPosParameters.SWP_ASNCWINDOWPOS | SetWindowPosParameters.SWP_NOMOVE);
            //}
            System.Windows.Rect r = element.Current.BoundingRectangle;
            Mouse.Click(r.Left + 1, r.Top + r.Height / 2);
        }

        internal void SetMaximize()
        {
            Search();
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_realName);
            }
            WindowPattern windowPattern = element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            if (windowPattern == null)
            {
                throw new UnSupportPatternException();
            }
            if (!windowPattern.Current.CanMaximize)
            {
                throw new WindowCanNotMaximizeException();
            }

            windowPattern.SetWindowVisualState(WindowVisualState.Maximized);
        }

        private WindowsUnit GetEndWindow(WindowsUnit parent)
        {
            if (parent == null) return null;
            AutomationElementCollection collection = parent.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
            if (collection == null || collection.Count == 0)
            {
                return null;
            }

            WindowsUnit endWindow = null;
            foreach (AutomationElement child in collection)
            {
                WindowsUnit unit = GetEndWindow(child);
                
                if (unit == null)
                {
                    //此处判断多文档窗口
                    if (endWindow != null && endWindow.Current.IsEnabled && !endWindow.Current.IsOffscreen)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    endWindow = unit;
                }
            }
            return endWindow;
        }

        internal void Close()
        {
            Search();
            if (Find())
            {
                if (!element.Current.IsEnabled)
                {
                    throw new ControlUnableException(_realName);
                }

                WindowPattern wp = element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;

                if (wp == null)
                {
                    throw new UnSupportPatternException();
                }

                wp.Close();
            }
        }

        internal void Listen()
        {
            GetActiveWindow();
        }

        internal void Click(string elementName)
        {
            Search();

            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_realName);
            }

            AutomationElementCollection childCollection = element.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, elementName));
            if (childCollection == null || childCollection.Count == 0)
            {
                throw new NullControlException(_realName, elementName);
            }

            foreach (AutomationElement child in childCollection)
            {
                if (child.Current.ControlType == ControlType.Button)
                {
                    NativeMethods.PostMessage((System.IntPtr)child.Current.NativeWindowHandle, 0x00F5, 0, 0);
                }
                else
                {
                    SetCursor(child, true);
                }
            }
        }

    }
}
