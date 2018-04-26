namespace Mock.Tools.Controls
{
    using System;
    using System.Threading;
    using System.Diagnostics;
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using System.Collections.Generic;
    internal sealed class ComboBoxObject : WinObject
    {
        internal ComboBoxObject(string windowName, string comboxName)
        {
            _windowName = windowName;
            _elementName = comboxName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, comboxName);
            if (!element.Current.ControlType.Equals(ControlType.ComboBox))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox));
                if (element == null)
                {
                    throw new NullControlException(comboxName);
                }
            }
        }

        public ComboBoxObject(WindowsUnit e)
        {
            if (e == null)
            {
                throw new NullControlException(_windowName, _elementName);
            }
            element = e;
            if (!element.Current.ControlType.Equals(ControlType.ComboBox))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox));
                if (element == null)
                {
                    throw new NullControlException(_windowName, _elementName);
                }
            }
        }

        internal void Select(string itemName)
        {
            if (!element.Current.IsKeyboardFocusable)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }
            List<string> nameList = GetControlName("ComboBox", itemName);
            WindowsUnit item = null;
            WindowsUnit selection = FindChild(element, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List), _elementName, "下拉列表");
            foreach (string name in nameList)
            {
                try
                {
                    item = FindChild(selection, new PropertyCondition(AutomationElement.NameProperty, name), string.Format("复选框<{0}>下拉列表", _elementName), itemName);
                    break;
                }
                catch (NullControlException)
                {
                }
                catch (MultiControlException)
                {
                    throw new MultiControlException(_windowName, _elementName, itemName);
                }
            }

            if (item == null)
            {
                throw new NullControlException(_windowName, _elementName, itemName);
            }

            //WindowsUnit selectionItem = FindChild(selection, new PropertyCondition(AutomationElement.NameProperty, ItemName), string.Format("复选框<{0}>下拉列表", _elementName), ItemName);
            SelectionItemPattern selectionPattern = item.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            if (selectionPattern == null)
            {
                throw new UnClickableException();
            }
            selectionPattern.Select();
            Wait(100);
        }

        internal bool Exist(string ItemName)
        {
            try
            {
                if (!element.Current.IsKeyboardFocusable)
                {
                    throw new ControlUnableException(_windowName, _elementName);
                }
                WindowsUnit selection = FindChild(element, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List), _elementName, "List");
                WindowsUnit selectionItem = FindChild(selection, new PropertyCondition(AutomationElement.NameProperty, ItemName), "List", ItemName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal string Content
        {
            get
            {
                if (!element.Current.IsKeyboardFocusable)
                {
                    throw new ControlUnableException(_windowName, _elementName);
                }
                try
                {
                    SelectionPattern pattern = null;
                    try
                    {
                        pattern = element.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;
                    }
                    catch
                    {
                        throw new UnSupportPatternException();
                    }

                    AutomationElement[] selectArray = pattern.Current.GetSelection();
                    if (selectArray == null || selectArray.Length == 0)
                    {
                        return string.Empty;
                    }

                    AutomationElement selected = selectArray[0];
                    return selected.Current.Name;
                }
                catch (NullControlException)
                {
                    throw new NullControlException(_windowName, _elementName, "Content");
                }
                catch (MultiControlException)
                {
                    throw new MultiControlException(_windowName, _elementName, "Content");
                }
            }
        }
    }
}
