namespace Mock.Tools.Controls
{
    using System;
    using System.Reflection;
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using System.Collections.Generic;

    using Mock.Nature.Native;
    using Mock.Data.Exception;
    internal class PopMenuObject : WinObject
    {
        internal PopMenuObject(string windowName)
        {
            //WWindow.SearchWindow(windowName);
            //NativeMethods.ShowOwnedPopups(RobotContext.Window.Current.NativeWindowHandle, true);
            try
            {
                Robot.ExecuteWithTimeOut(() =>
                    {
                        while (!WWindow.FindWindow(windowName, 1))
                        {
                            Robot.Recess(100);
                        }
                    });
            }
            catch (TimeOutException)
            {
                throw new NullControlException(windowName);
            }
            GetElement(windowName);

            element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "DropDown"));
            if (element == null)
            {
                List<WindowsUnit> allWindow = GetActiveWindow();
                foreach (WindowsUnit unit in allWindow)
                {
                    if (string.Equals(unit.Current.Name, "DropDown"))
                    {
                        element = unit;
                        break;
                    }
                }
                //throw new NullControlException(windowName, "PopMenu");
            }

            if (element == null)
            {
                element = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "DropDown"));

                if (element == null)
                {
                    throw new NullControlException(windowName, "PopMenu");
                }
            }

            _windowName = windowName;
            _elementName = "PopMenu";
        }

        internal void DbClick(string itemName)
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            List<string> nameList = GetControlName("Menu", itemName);
            WindowsUnit child = null;
            foreach(string name in nameList)
            {
                child = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, name));
                if (child != null) break;
            }

            if (child == null)
            {
                throw new NullControlException(_windowName, _elementName, itemName);
            }

            Mouse.DbClick((int)child.Current.BoundingRectangle.Left + 5, (int)child.Current.BoundingRectangle.Top + 5);
        }

        internal void Click(string itemName)
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            List<string> nameList = GetControlName("Menu", itemName);
            WindowsUnit child = null;
            foreach (string name in nameList)
            {
                child = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, name));
                if (child != null)
                {
                    break;
                }
            }

            if (child == null)
            {
                throw new NullControlException(_windowName, _elementName, itemName);
            }

            InvokePattern invokePattern = child.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (invokePattern == null)
            {
                throw new UnClickableException();
            }

            TaistControlEventInfo info = new TaistControlEventInfo();
            info.ControlType = TaistControlType.PopMenu;
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

        internal void ClickByMouse(string itemName)
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            List<string> nameList = GetControlName("Menu", itemName);
            WindowsUnit child = null;
            foreach(string name in nameList)
            {
                child = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, name));
                if (child != null)
                {
                    break;
                }
            }

            if (child == null)
            {
                throw new NullControlException(_windowName, _elementName, itemName);
            }

            Mouse.Click((int)child.Current.BoundingRectangle.Left + 5, (int)child.Current.BoundingRectangle.Top + 5);
        }

        internal T GetItemObject<T>(string controlType)
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }

            //NativeMethods.ShowOwnedPopups((IntPtr)lastWindow.Current.NativeWindowHandle, false);

            ControlType type;
            Type t = typeof(ControlType);
            FieldInfo f = t.GetField(controlType);

            if (f == null)
            {
                throw new InvalidParamValueException("Invalid ControlType : " + controlType);
            }

            type = (ControlType)f.GetValue(null);

            WindowsUnit child = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, type));

            if (child == null)
            {
                throw new NullControlException(_windowName, "Pop " + controlType);
            }

            T obj = (T)Activator.CreateInstance(typeof(T), child);
            return obj;

        }
    }
}
