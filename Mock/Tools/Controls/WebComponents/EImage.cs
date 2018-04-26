namespace Mock.Tools.Controls
{
    public class EImage
    {
        public static void Download(string windowName, string buttonName)
        {
            IEImageObject btnObj = new IEImageObject(windowName, buttonName);
            btnObj.Download("c:\\123.bmp");
        }
    }
}
