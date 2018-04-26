namespace Mock.Tools.Controls
{
    public class ENotify
    {
        public static void Save(string windowName)
        {
            IENotifyObject notifyObj = new IENotifyObject(windowName);
            notifyObj.Save();
            notifyObj = null;
        }

        public static void SaveAs(string windowName)
        {
            IENotifyObject notifyObj = new IENotifyObject(windowName);
            notifyObj.SaveAs();
            notifyObj = null;
        }

        public static void Open(string windowName)
        {
            IENotifyObject notifyObj = new IENotifyObject(windowName);
            notifyObj.Open();
            notifyObj = null;
        }

        public static void Close(string windowName)
        {
            IENotifyObject notifyObj = new IENotifyObject(windowName);
            notifyObj.Close();
            notifyObj = null;
        }

        public static void Cancel(string windowName)
        {
            IENotifyObject notifyObj = new IENotifyObject(windowName);
            notifyObj.Cancel();
            notifyObj = null;
        }

        public static string GetContent(string windowName)
        {
            IENotifyObject notifyObj = new IENotifyObject(windowName);
            return notifyObj.Message;
        }
    }
}
