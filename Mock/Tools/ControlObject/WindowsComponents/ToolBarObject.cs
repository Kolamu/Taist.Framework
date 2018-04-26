namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using System.Collections.Generic;
    internal sealed class ToolBarObject : WinObject
    {
        private InvokePattern invokePattern = null;
        internal ToolBarObject(string windowName,string toolBarName)
        {
            _windowName = windowName;
            _elementName = toolBarName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, toolBarName);
            /*if (!element.Current.ControlType.Equals(ControlType.ToolBar))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
                if (element == null)
                {
                    throw new NullControlException(toolBarName);
                }
            }*/
        }

        public ToolBarObject(WindowsUnit e)
        {
            element = e;
        }

        internal void Click(string itemName)
        {
            List<string> nameList = GetControlName("ToolBar", itemName);
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }
            WindowsUnit toolBarItem = null;
            foreach (string name in nameList)
            {
                toolBarItem = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, name));
                if (toolBarItem != null)
                {
                    break;
                }
            }
            if (toolBarItem == null)
            {
                foreach (string name in nameList)
                {
                    toolBarItem = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, name));
                    if (toolBarItem != null)
                    {
                        break;
                    }
                }
                if (toolBarItem == null)
                {
                    throw new NullControlException(itemName);
                }
            }

            invokePattern = toolBarItem.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invokePattern == null)
            {
                throw new UnSupportPatternException();
            }
            TaistControlEventInfo info = new TaistControlEventInfo();
            info.Resverd1 = itemName;
            info.ControlType = TaistControlType.ToolBar;
            info.EventType = TaistEventType.Click;
            info.EventTime = System.DateTime.Now;
            //DoAction(ClickAction, itemName);

            RobotContext.LastAction(() =>
            {
                try
                {
                    Robot.ExecuteWithTimeOut(() =>
                    {
                        invokePattern.Invoke();
                    }, 2000);
                }
                catch (TimeOutException) { }
                return null;
            });
            RobotContext.RunControlEvent(info);

        }
    }
}
