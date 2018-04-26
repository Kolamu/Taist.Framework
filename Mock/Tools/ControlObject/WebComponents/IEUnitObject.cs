namespace Mock.Tools.Controls
{
    using mshtml;
    using Mock.Tools.Exception;
    using System.Collections.Generic;
    internal class IEUnitObject : WebObject
    {
        public IEUnitObject(string windowName, string unitName)
        {
            _windowName = windowName;
            _elementName = unitName;
            EWindow.Search(windowName);
            if (!string.IsNullOrEmpty(_elementName))
            {
                GetElement(windowName, unitName);
            }
        }

        internal void Click()
        {
            if (string.IsNullOrEmpty(_elementName))
            {
                throw new UnSupportOperationException();
            }
            element.click();
        }

        internal void Click(int x, int y)
        {
            if (string.IsNullOrEmpty(_elementName))
            {
                throw new UnSupportOperationException();
            }
            IHTMLElement2 el2 = (IHTMLElement2)element;
            IHTMLRect rc = el2.getBoundingClientRect();
            WindowsUnit server = RobotContext.InternetExploreServer;
            System.Windows.Rect rect = server.Current.BoundingRectangle;
            int left = rc.left + (int)rect.Left;
            int top = rc.top + (int)rect.Top;
            LogManager.DebugFormat("[x {0}, y {1}]", left, top);
            Mouse.Click(left + x, top + y);
        }

        internal string innerText()
        {
            if (string.IsNullOrEmpty(_elementName))
            {
                throw new UnSupportOperationException();
            }
            string val = element.getAttribute("value").ToString();
            if (string.IsNullOrEmpty(val))
            {
                return element.innerText;
            }
            else
            {
                return val;
            }
        }

        internal void Download(string url, Dictionary<string, string> param)
        {
            
        }
    }
}
