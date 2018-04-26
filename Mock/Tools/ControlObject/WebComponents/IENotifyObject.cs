namespace Mock.Tools.Controls
{
    using System.Windows.Automation;

    using Mock.Tools.Exception;
    internal class IENotifyObject : WebObject
    {
        internal IENotifyObject(string windowName)
        {
            EWindow.Search(windowName);
        }

        internal void Save()
        {
            WindowsUnit unit = getPopToolBar();
            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "保存"));

            if (unit == null)
            {
                throw new NullControlException("保存工具栏", "保存");
            }

            InvokePattern pattern = unit.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (pattern == null)
            {
                throw new UnSupportPatternException();
            }

            pattern.Invoke();

        }

        internal void SaveAs()
        {
            WindowsUnit unit = getPopToolBar();
            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "保存"));

            if (unit == null)
            {
                throw new NullControlException("保存工具栏", "保存");
            }

            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.SplitButton));

            if (unit == null)
            {
                throw new NullControlException("保存工具栏", "保存拆分按钮");
            }

            InvokePattern pattern = unit.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (pattern == null)
            {
                throw new UnSupportPatternException();
            }

            pattern.Invoke();

            unit = null;
            for (int i = 0; i < 30; i++)
            {
                unit = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "菜单"));
                if (unit != null)
                {
                    break;
                }
            }

            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "另存为(A)"));

            if (unit == null)
            {
                throw new NullControlException("弹出菜单", "另存为");
            }

            pattern = unit.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (pattern == null)
            {
                throw new UnSupportPatternException();
            }

            pattern.Invoke();
        }

        internal void Open()
        {
            WindowsUnit unit = getPopToolBar();
            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "打开"));

            if (unit == null)
            {
                throw new NullControlException("保存工具栏", "打开");
            }

            InvokePattern pattern = unit.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (pattern == null)
            {
                throw new UnSupportPatternException();
            }

            pattern.Invoke();
        }

        internal void Cancel()
        {
            WindowsUnit unit = getPopToolBar();
            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "取消"));

            if (unit == null)
            {
                throw new NullControlException("保存工具栏", "取消");
            }

            InvokePattern pattern = unit.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (pattern == null)
            {
                throw new UnSupportPatternException();
            }

            pattern.Invoke();
        }

        internal void Close()
        {
            WindowsUnit unit = getPopToolBar();
            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "关闭"));

            if (unit == null)
            {
                throw new NullControlException("保存工具栏", "关闭");
            }

            InvokePattern pattern = unit.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (pattern == null)
            {
                throw new UnSupportPatternException();
            }

            pattern.Invoke();
        }

        internal string Message
        {
            get
            {
                string msg = null;

                WindowsUnit unit = getPopToolBar();
                unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "通知栏文本"));

                if (unit == null)
                {
                    throw new NullControlException("保存工具栏", "通知栏文本");
                }

                ValuePattern pattern = unit.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

                if (pattern == null)
                {
                    throw new UnSupportPatternException();
                }

                msg = pattern.Current.Value;
                return msg;
            }
        }

        private WindowsUnit getPopToolBar()
        {
            WindowsUnit unit = RobotContext.InternetExploreWindow.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "Frame Notification Bar"));
            if (unit == null)
            {
                throw new NullControlException("保存工具栏");
            }

            unit = unit.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "DirectUIHWND"));
            if (unit == null)
            {
                throw new NullControlException("保存工具栏", "DirectUIHWND");
            }

            return unit;
        }
    }
}
