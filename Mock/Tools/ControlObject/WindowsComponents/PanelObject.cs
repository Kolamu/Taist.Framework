namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using System.Windows;
    using System.Runtime.InteropServices;
    using System;
    internal class PanelObject : WinObject
    {
        internal PanelObject(string windowName, string panelName)
        {
            _windowName = windowName;
            _elementName = panelName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, panelName);
        }

        internal void Click()
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            Rect rect = element.Current.BoundingRectangle;
            int x = (int)rect.Right - 15;
            int y = (int)rect.Top + 5;
            Mouse.Click(x, y);
        }

        internal void Click(int x, int y)
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            Rect rect = element.Current.BoundingRectangle;
            int x1 = (int)rect.Left + x;
            int y1 = (int)rect.Top + y;
            Mouse.Click(x1, y1);
        }

        internal void SetFocus()
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }
            element.SetFocus();
        }

        internal string GetText()
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            ValuePattern valuePattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            if (valuePattern == null)
            {
                throw new UnSupportPatternException();
            }
            return valuePattern.Current.Value;
        }

        internal void Input(string txt)
        {
            Keybord.Input(txt);
        }
    }
}
