namespace Mock.Tools.Controls
{
    using System.Windows.Automation;
    using Mock.Tools.Exception;
    using System.Collections.Generic;
    internal class TreeObject : WinObject
    {
        internal TreeObject(string windowName, string treeName)
        {
            _windowName = windowName;
            _elementName = treeName;
            WWindow.SearchWindow(windowName);
            GetElement(windowName, treeName);
            if (!element.Current.ControlType.Equals(ControlType.Tree))
            {
                element = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                if (element == null)
                {
                    throw new NullControlException(windowName, treeName);
                }
            }
        }

        public TreeObject(WindowsUnit e)
        {
            element = e;
            ControlType ct = element.Current.ControlType;
            if (!ct.Equals(ControlType.Tree))
            {
                WindowsUnit tmpElement = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Tree));
                if (tmpElement == null)
                {
                    throw new NullControlException(_windowName, _elementName);
                }
                element = tmpElement;
            }
        }

        internal void Select(string itemName)
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

            SelectionItemPattern selectPattern = child.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;

            if (selectPattern == null)
            {
                throw new UnSupportPatternException();
            }

            selectPattern.Select();
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

        internal void Expand(string itemName)
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

            ExpandCollapsePattern expandCollapsePattern = child.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;

            if (expandCollapsePattern == null)
            {
                throw new UnSupportPatternException();
            }

            expandCollapsePattern.Expand();
        }

        internal void Collapse(string itemName)
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

            ExpandCollapsePattern expandCollapsePattern = child.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;

            if (expandCollapsePattern == null)
            {
                throw new UnSupportPatternException();
            }
            expandCollapsePattern.Collapse();
        }
    }
}
