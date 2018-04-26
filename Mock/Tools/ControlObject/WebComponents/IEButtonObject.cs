namespace Mock.Tools.Controls
{
    using System;
    using System.Threading;
    internal class IEButtonObject : WebObject
    {
        internal IEButtonObject(string windowName, string buttonName)
        {
            _windowName = windowName;
            _elementName = buttonName;
            //GetInternetExploreWindow(windowName);
            EWindow.Search(windowName);
            GetElement(windowName, buttonName);
        }
        
        internal void Click()
        {
            Exception ex = null;
            Thread t = new Thread(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        ex = null;
                        element.click();
                        break;
                    }
                    catch (Exception e)
                    {
                        Robot.Recess(500);
                        ex = e;
                    }
                }
            });
            t.IsBackground = true;
            t.Start();
            if (ex != null)
            {
                throw ex;
            }
        }
    }
}
