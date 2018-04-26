namespace Mock.Tools.Controls
{
    public class EMenuBar
    {
        public static void Click(string windowName, string menuBarName, string menuItemName)
        {
            LogManager.DebugFormat(string.Format("Click {0} item in {1} menubar in {2} window", menuItemName, menuBarName, windowName), NoteType.DEBUG);
            IEMenuObject menuObj = new IEMenuObject(windowName, menuBarName);
            menuObj.Click(menuItemName);
        }
    }
}
