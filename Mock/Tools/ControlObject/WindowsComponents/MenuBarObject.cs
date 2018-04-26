namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using System.Collections.Generic;
    internal class MenuBarObject : WinObject
    {
        internal MenuBarObject(string windowName, string menuName)
        {
            _windowName = windowName;
            _elementName = menuName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, menuName);
        }

        public MenuBarObject(WindowsUnit e)
        {
            element = e;
        }

        internal void Click(string itemName)
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            List<string> nameList = GetControlName("MenuBar", itemName);
            AndCondition condition = null;
            WindowsUnit item = null;
            foreach (string name in nameList)
            {
                condition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, name), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem));
                item = element.FindFirst(TreeScope.Descendants, condition);
                if (item != null)
                {
                    break;
                }
            }

            if (item == null)
            {
                throw new NullControlException(itemName);
            }

            InvokePattern invokePattern = item.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invokePattern == null)
            {
                throw new UnClickableException();
            }

            TaistControlEventInfo info = new TaistControlEventInfo();
            info.ControlType = TaistControlType.Menu;
            info.EventType = TaistEventType.Click;
            info.EventTime = System.DateTime.Now;
            info.Resverd1 = itemName;

            RobotContext.LastAction(() =>
            {
                invokePattern.Invoke();
                return null;
            });
            RobotContext.RunControlEvent(info);
        }

        internal void Click(params string[] itemNameArray)
        {
            LogManager.Debug("Click Child");
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            WindowsUnit item = element;
            foreach (string itemName in itemNameArray)
            {
                List<string> nameList = GetControlName("MenuBar", itemName);
                AndCondition condition = null;

                foreach (string name in nameList)
                {
                    condition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, name), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem));
                    item = item.FindFirst(TreeScope.Children, condition);
                    if (item != null)
                    {
                        break;
                    }
                }

                if (item == null)
                {
                    throw new NullControlException(itemName);
                }

            }
            InvokePattern invokePattern = item.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            if (invokePattern == null)
            {
                throw new UnClickableException();
            }

            TaistControlEventInfo info = new TaistControlEventInfo();
            info.ControlType = TaistControlType.PopMenu;
            info.EventType = TaistEventType.Click;
            info.EventTime = System.DateTime.Now;
            info.Resverd1 = string.Join(",", itemNameArray);

            RobotContext.LastAction(() =>
            {
                invokePattern.Invoke();
                return null;
            });
            RobotContext.RunControlEvent(info);
        }
    }
}
