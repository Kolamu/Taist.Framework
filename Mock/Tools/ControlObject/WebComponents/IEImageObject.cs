
namespace Mock.Tools.Controls
{
    using System;
    using System.Threading;
    using mshtml;
    using System.IO;
    internal class IEImageObject : WebObject
    {
        internal IEImageObject(string windowName, string imgName)
        {
            _windowName = windowName;
            _elementName = imgName;
            //GetInternetExploreWindow(windowName);
            EWindow.Search(windowName);
            GetElement(windowName, imgName);
        }

        internal void Download(string path)
        {
            HTMLImg image = (HTMLImg)element;
            string name = image.nameProp;
            IECacheObject cacheObj = new IECacheObject();
            byte[] fileBytes = cacheObj.GetFile(name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {

            }
            using (FileStream fs = File.Create(path))
            {
                fs.Write(fileBytes, 0, fileBytes.Length);
                fs.Flush();
                fs.Close();
            }
        }
    }
}
