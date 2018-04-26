namespace Mock.Tools.Controls
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;
    using Mock.Tools.Exception;
    using System.Windows.Automation;
    using Mock.Nature.Native;
    using Accessibility;
    internal sealed class StatusBarObject : WinObject
    {
        internal StatusBarObject(string windowName, string statusBarName = "状态栏")
        {
            _windowName = windowName;
            _elementName = statusBarName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, statusBarName);
            if (!element.Current.ControlType.Equals(ControlType.StatusBar))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.StatusBar));
                if (element == null)
                {
                    throw new NullControlException(windowName, statusBarName);
                }
            }
        }

        internal string GetValue(int index)
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }
            
            IAccessible _statusStrip = null;
            object obj = null;
            Guid g = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");
            NativeMethods.AccessibleObjectFromWindow(element.Current.NativeWindowHandle, OBJID.CLIENT, ref g, ref obj);

            if (obj == null)
            {
                throw new Exception("Can not find statusbar");
            }
            else
            {
                _statusStrip = (IAccessible)obj;

                Object[] childs = new Object[_statusStrip.accChildCount];
                int obtain;
                NativeMethods.AccessibleChildren(_statusStrip, 0, _statusStrip.accChildCount, childs, out obtain);
                IAccessible data = (IAccessible)childs[index - 1];
                return data.accName;
            }
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam, StringBuilder lParam);
    }
}
