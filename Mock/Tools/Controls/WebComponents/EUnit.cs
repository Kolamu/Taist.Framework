namespace Mock.Tools.Controls
{
    using System.Collections.Generic;
    public class EUnit
    {
        public static void Click(string windowName, string unitName)
        {
            LogManager.Debug(string.Format("Click {1} unit in {0} window.", windowName, unitName));
            IEUnitObject unitObj = new IEUnitObject(windowName, unitName);
            unitObj.Click();
        }

        public static void Click(string windowName, string unitName, int x, int y)
        {
            LogManager.Debug(string.Format("Click {1} unit({2}, {3}) in {0} window.", windowName, unitName, x, y));
            IEUnitObject unitObj = new IEUnitObject(windowName, unitName);
            unitObj.Click(x, y);
        }

        public static bool Exist(string windowName, string unitName)
        {
            try
            {
                new IEUnitObject(windowName, unitName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetValue(string windowName, string unitName)
        {
            IEUnitObject unitObj = new IEUnitObject(windowName, unitName);
            return unitObj.innerText();
        }

        public static void Download(string windowName, string url, Dictionary<string, string> param)
        {
            IEUnitObject unitObj = new IEUnitObject(windowName, null);
            unitObj.Download(url, param);
        }
    }
}
