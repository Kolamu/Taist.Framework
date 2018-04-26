namespace Mock.Tools.Controls
{
    public class WFileDownLoadWindow
    {
        public static bool Exist(string windowName)
        {
            try
            {
                new FileDownLoadWindowObject(windowName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Save(string windowName)
        {
            FileDownLoadWindowObject obj = new FileDownLoadWindowObject(windowName);
            obj.Save();
        }
    }
}
