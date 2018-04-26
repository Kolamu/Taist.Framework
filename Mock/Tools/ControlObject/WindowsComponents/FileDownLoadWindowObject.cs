namespace Mock.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;

    using Mock.Nature.Native;
    using Mock.Tools.Exception;
    internal class FileDownLoadWindowObject : WinObject
    {
        internal FileDownLoadWindowObject(string windowName)
        {
            if (string.IsNullOrEmpty(windowName))
            {
                _windowName = "WFileDownLoadWindow";
            }
            else
            {
                _windowName = windowName;
            }
            RobotContext.WindowName = _windowName;
            bool find = false;
            List<WindowsUnit> activeWindowList = GetActiveWindow();

            while (activeWindowList.Count == 0)
            {
                System.Console.WriteLine(activeWindowList.Count);
                Robot.Recess(100);
                activeWindowList = GetActiveWindow();
            }

            for (int i = 0; i < activeWindowList.Count; i++)
            {
                WindowsUnit window = activeWindowList[i];

                if (string.Equals(window.Current.Name, windowName) && string.Equals(window.Current.ClassName, "#32770"))
                {
                    find = true;
                    RobotContext.Window = window;
                    element = window;
                    break;
                }
            }

            if (!find)
            {
                throw new NullControlException(_windowName);
            }
        }

        internal void Search()
        {

        }

        internal void Save()
        {
            //DoAction((state) =>
            //    {
                    for (int i = 0; i < 1; i++)
                    {
                        try
                        {
                            Mock.Nature.Native.NativeMethods.SetForegroundWindow((System.IntPtr)element.Current.NativeWindowHandle);
                            
                            AutomationElementCollection buttons = element.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                            foreach (AutomationElement button in buttons)
                            {
                                if (button.Current.Name.Contains("保存"))
                                {
                                    //Mock.Nature.Native.NativeMethods.PostMessage((System.IntPtr)button.Current.NativeWindowHandle, 0x00F5, 0, 0);
                                    //int n =  Mock.Nature.Native.NativeMethods.PostMessage((System.IntPtr)button.Current.NativeWindowHandle, 0x00F5, 0, 0);
                                    //int x = (int)button.Current.BoundingRectangle.X + 10;
                                    //int y = (int)button.Current.BoundingRectangle.Y + 10;
                                    //IntPtr hWnd = (IntPtr)button.Current.NativeWindowHandle;
                                    //Mouse.Move(x, y);
                                    //NativeMethods.SendMessage(hWnd, WindowsMessages.WM_NCHITTEST, 0, y << 16 | x);
                                    //NativeMethods.SendMessage((IntPtr)element.Current.NativeWindowHandle, WindowsMessages.WM_SETCURSOR, button.Current.NativeWindowHandle, (int)WindowsMessages.WM_MOUSEMOVE << 16 | 1);
                                    //NativeMethods.PostMessage(hWnd, WindowsMessages.WM_MOUSEMOVE, 0, 10 << 16 | 10);
                                    //Robot.Recess(60);

                                    //NativeMethods.SendMessage(hWnd, WindowsMessages.WM_NCHITTEST, 0, y << 16 | x);
                                    //NativeMethods.SendMessage(hWnd, (int)WindowsMessages.WM_MOUSEACTIVATE, element.Current.NativeWindowHandle, (int)WindowsMessages.WM_LBUTTONDOWN << 16 | 1);
                                    //NativeMethods.SendMessage((IntPtr)element.Current.NativeWindowHandle, WindowsMessages.WM_SETCURSOR, button.Current.NativeWindowHandle, (int)WindowsMessages.WM_LBUTTONDOWN << 16 | 1);
                                    
                                    //x = 10;
                                    //y = 10;
                                    //int point = y << 16 | x;

                                    //NativeMethods.PostMessage(hWnd, WindowsMessages.WM_LBUTTONDOWN, 0x0001, point);
                                    //Robot.Recess(10);
                                    //NativeMethods.PostMessage(hWnd, WindowsMessages.WM_LBUTTONUP, 0, point);

                                    //SetCursor(button, true);

                                    //int buttonId = NativeMethods.GetDlgCtrlID(button.Current.NativeWindowHandle);
                                    //Console.WriteLine(buttonId);

                                    //NativeMethods.PostMessage((IntPtr)element.Current.NativeWindowHandle, WindowsMessages.WM_COMMAND, buttonId << 16 | 0, button.Current.NativeWindowHandle);
                                    InvokePattern pattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                                    pattern.Invoke();

                                    Wait(100);
                                    Keybord.KeyDown(Data.VK.RETURN);
                                    Wait(100);
                                    Keybord.KeyDown(Data.VK.RETURN);
                                }
                            }
                        }
                        catch { }
                    }
                //}, null);
        }
    }
}
