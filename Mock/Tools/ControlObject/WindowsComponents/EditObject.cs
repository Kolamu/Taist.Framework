namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using System.Windows;
    using Mock.Tools.Exception;
    using Nature.Native;
    using Accessibility;
    using System;
    using System.Runtime.InteropServices;
    internal sealed class EditObject : WinObject
    {
        internal EditObject(string windowName, string editName)
        {
            _windowName = windowName;
            _elementName = editName;
            WWindow.SearchWindow(windowName);

            GetElement(windowName, editName);
            ControlType ct = element.Current.ControlType;
            WindowsUnit pe = element;
            if (!ct.Equals(ControlType.Edit) && !ct.Equals(ControlType.Document))
            {
                element = pe.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                if (element == null)
                {
                    element = pe.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));
                    if (element == null)
                    {
                        throw new NullControlException(editName);
                    }
                }
            }
        }

        public EditObject(WindowsUnit e)
        {
            element = e;
            if (element == null)
            {
                LogManager.Debug("element is null");
                throw new NullControlException(_windowName, _elementName);
            }
            ControlType ct = element.Current.ControlType;
            if (!ct.Equals(ControlType.Edit) && !ct.Equals(ControlType.Document))
            {
                WindowsUnit tmpElement = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                if (tmpElement == null)
                {
                    tmpElement = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));
                    if (tmpElement == null)
                    {
                        throw new NullControlException(_windowName, _elementName);
                    }
                }
                element = tmpElement;
            }
        }

        public void Input(string msg)
        {
            ControlType ct = element.Current.ControlType;

            //for (int i = 0; i < Config.RedoCount; i++)
            //{
            //    if (element.Current.IsKeyboardFocusable)
            //    {
            //        break;
            //    }
            //    Robot.Recess(500);
            //}
            //if (!element.Current.IsKeyboardFocusable)
            //{
            //    throw new ControlUnableException(_windowName, _elementName);
            //}
            if (msg == null) msg = "";
            if (ct.Equals(ControlType.Edit))
            {
                ValuePattern valuePattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (valuePattern == null)
                {
                    throw new UnSupportPatternException();
                }
                SetCursor(element);
                Robot.ExecuteWithTimeOut(() =>
                    {
                        valuePattern.SetValue(msg);
                    }, 2000, false);
                Wait(10);
                //NativeMethods.PostMessage((IntPtr)_CurrentWindow.Current.NativeWindowHandle, 0x0111, 0x0300 << 8 + element.Current.NativeWindowHandle, 0);
                NativeMethods.PostMessage((IntPtr)element.Current.NativeWindowHandle, 0x0100, 9, 0);
                NativeMethods.PostMessage((IntPtr)element.Current.NativeWindowHandle, 0x0101, 9, 0);
            }
            else
            {
                TextPattern textPattern = element.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                if (textPattern == null)
                {
                    throw new UnSupportPatternException();
                }

                string name = string.Empty;
                while (!string.Equals(name, msg))
                {
                    LogManager.Debug(string.Format("{0} != {1}", name, msg));
                    LogManager.Debug(System.BitConverter.ToString(System.Text.Encoding.ASCII.GetBytes(name)));
                    LogManager.Debug(System.BitConverter.ToString(System.Text.Encoding.ASCII.GetBytes(msg)));
                    element.SetFocus();
                    textPattern.DocumentRange.Select();
                    Keybord.Input(msg);
                    //有些机器执行的很快，需要预留一点时间来让textPattern 更新
                    Wait(100);
                    name = textPattern.DocumentRange.GetText(msg.Length);
                }
            }
            //Keybord.Press(Data.VK.TAB);
            
        }

        internal void Click()
        {
            if (!element.Current.IsKeyboardFocusable)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            Rect rect = element.Current.BoundingRectangle;
            int x = (int)rect.Right - 15;
            int y = (int)rect.Top + 5;
            Mouse.Click(x, y);
        }

        internal string GetValue()
        {
            ControlType ct = element.Current.ControlType;
            if (!element.Current.IsKeyboardFocusable)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            if (ct.Equals(ControlType.Edit))
            {
                ValuePattern valuePattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (valuePattern == null)
                {
                    throw new UnSupportPatternException();
                }
                return valuePattern.Current.Value;
            }
            else
            {
                TextPattern textPattern = element.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                if (textPattern == null)
                {
                    throw new UnSupportPatternException();
                }
                return textPattern.DocumentRange.GetText(-1);
            }
        }
    }
}
