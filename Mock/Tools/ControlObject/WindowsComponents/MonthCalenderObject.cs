namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    internal class MonthCalenderObject : WinObject
    {
        public MonthCalenderObject(string windowName)
        {
            Robot.Recess(1000);
            _windowName = windowName;
            _elementName = "日历控件";
            GetElement(windowName);
            element = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "日历控件"));
            if (element == null)
            {
                throw new NullControlException(windowName, "日历控件");
            }
        }
        public void NextMonth()
        {
            
            AutomationElement buttonNext = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "下一个按钮"));
            if (buttonNext == null)
            {
                throw new NullControlException(_windowName, _elementName, "下一个按钮");
            }

            SetCursor(buttonNext, true);
            Robot.Recess(200);
            Mouse.Click((int)element.Current.BoundingRectangle.Left - 10, (int)element.Current.BoundingRectangle.Top - 10);
        }

    }
}
