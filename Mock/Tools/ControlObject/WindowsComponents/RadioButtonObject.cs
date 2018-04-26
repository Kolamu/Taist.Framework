namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    internal class RadioButtonObject : WinObject
    {
        internal RadioButtonObject(string windowName, string radiobuttonName)
        {
            _windowName = windowName;
            _elementName = radiobuttonName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, radiobuttonName);
            if (!element.Current.ControlType.Equals(ControlType.RadioButton))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.RadioButton));
                if (element == null)
                {
                    throw new NullControlException(windowName, radiobuttonName);
                }
            }
        }

        internal RadioButtonObject(WindowsUnit e)
        {
            element = e;
        }

        internal void Select()
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }
            SelectionItemPattern selectPattern = element.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;

            if (selectPattern == null)
            {
                throw new UnClickableException();
            }

            if (!selectPattern.Current.IsSelected)
            {
                selectPattern.Select();
                Wait(100);
            }
        }
    }
}
