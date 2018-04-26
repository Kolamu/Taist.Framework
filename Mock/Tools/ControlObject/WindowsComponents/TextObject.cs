namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    internal class TextObject : WinObject
    {
        internal TextObject(string windowName, string textName)
        {
            WWindow.SearchWindow(windowName);
            GetElement(windowName, textName);
            if (!element.Current.ControlType.Equals(ControlType.Text))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                if (element == null)
                {
                    throw new NullControlException(textName);
                }
            }
        }

        public TextObject(WindowsUnit e)
        {
            element = e;
        }

        internal string GetValue()
        {
            return element.Current.Name;
        }
    }
}
