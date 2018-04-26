namespace Mock.Tools.Controls
{
    public class EButton
    {
        public static void Click(string windowName, string buttonName)
        {
            LogManager.Debug(string.Format("Click {1} button in {0} window.", windowName, buttonName));
            IEButtonObject btnObj = new IEButtonObject(windowName, buttonName);
            btnObj.Click();
        }
    }
}
