namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using System.Collections.Generic;
    internal class TabObject : WinObject
    {
        internal TabObject(string windowName, string tabName)
        {
            _windowName = windowName;
            _elementName = tabName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, tabName);
            if (!element.Current.ControlType.Equals(ControlType.Tab))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Tab));
                if (element == null)
                {
                    throw new NullControlException(windowName, tabName);
                }
            }
        }

        public TabObject(WindowsUnit e)
        {
            element = e;
        }

        internal void Select(string itemName)
        {
            List<string> nameList = GetControlName("Tab", itemName);
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }
            WindowsUnit selectionItem = null;
            foreach (string name in nameList)
            {
                try
                {
                    selectionItem = FindChild(element, new PropertyCondition(AutomationElement.NameProperty, name), _elementName, itemName);
                }
                catch (NullControlException)
                {
                    selectionItem = null;
                }
            }

            if (selectionItem == null)
            {
                throw new NullControlException(RobotContext.WindowName, RobotContext.ElementName, itemName);
            }
            
            SelectionItemPattern selectionPattern = selectionItem.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
            if (selectionPattern == null)
            {
                throw new UnClickableException();
            }
            selectionPattern.Select();
            
            Wait(100);
        }

        internal bool Exist(string itemName)
        {
            List<string> nameList = GetControlName("Tab", itemName);
            WindowsUnit selectionItem = null;
            foreach (string name in nameList)
            {
                try
                {
                    selectionItem = FindChild(element, new PropertyCondition(AutomationElement.NameProperty, name), _elementName, itemName);
                }
                catch (NullControlException)
                {
                    selectionItem = null;
                }
            }

            if (selectionItem == null)
            {
                return false;
            }

            return true;
        }

        internal string GetSelectedItemName()
        {
            if (!element.Current.IsEnabled)
            {
                throw new ControlUnableException(_windowName, _elementName);
            }
            WindowsUnit selectionItem = TreeWalker.RawViewWalker.GetFirstChild(element);
            while(selectionItem != null)
            {
                SelectionItemPattern selectionPattern = selectionItem.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                if (selectionPattern != null && selectionPattern.Current.IsSelected)
                {
                    return selectionItem.Current.Name;
                }
                selectionItem = TreeWalker.RawViewWalker.GetNextSibling(selectionItem);
            }
            return null;
        }
    }
}
