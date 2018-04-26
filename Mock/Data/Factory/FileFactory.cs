namespace Mock.Data
{
    using System.IO;
    using System.Collections.Generic;
    public class FileFactory
    {
        #region 创建文件
        public static void CreateFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                string dirPath = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                using (FileStream fs = File.Create(fileName))
                {
                    fs.Close();
                }
            }
        }
        #endregion

        #region 获取完整路径
        public static string GetRealPath(string fileName)
        {
            if (fileName == null) return null;
            LogManager.Debug(fileName);
            try
            {
                if (fileName.ToLower().StartsWith("file:///"))
                {
                    return fileName.Substring(8);
                }
                else
                {
                    return Path.GetFullPath(fileName);
                }
            }
            catch
            {
                return fileName;
            }
        }
        #endregion

        #region 清空文件夹
        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="dirName">文件夹所在的路径</param>
        public static void ClearDirectory(string dirName)
        {
            dirName = Path.GetFullPath(dirName);
            if (Directory.Exists(dirName))
            {
                Directory.Delete(dirName, true);
            }
            Directory.CreateDirectory(dirName);
        }
        #endregion

        #region 删除目录
        public static void DeleteDirecotry(string dirName)
        {
            string[] fileArray = Directory.GetFiles(dirName);
            foreach (string fileName in fileArray)
            {
                File.Delete(fileName);
            }

            string[] dirArray = Directory.GetDirectories(dirName);

            foreach (string name in dirArray)
            {
                DeleteDirecotry(name);
            }

            Directory.Delete(dirName);
        }
        #endregion

        #region 获取文件夹下的所有文件
        public static List<string> GetAllFileNames(string path, string extension = null)
        {
            try
            {
                List<string> fileList = new List<string>();
                if (!Directory.Exists(path))
                {
                    return fileList;
                }
                string[] fileArray = Directory.GetFiles(path);
                foreach (string fileName in fileArray)
                {
                    if (string.IsNullOrEmpty(extension)
                        || string.Equals(Path.GetExtension(fileName).ToLower().Trim('.'),
                                        extension.Trim('.').ToLower()))
                    {
                        fileList.Add(Path.GetFullPath(fileName));
                    }
                }

                string[] directoryArray = Directory.GetDirectories(path);
                foreach (string directory in directoryArray)
                {
                    List<string> descentFileNameList = GetAllFileNames(directory, extension);
                    fileList.AddRange(descentFileNameList);
                }
                return fileList;
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex);
                throw;
            }
        }
        #endregion

        #region 移动文件
        public static void Move(string srcPath, string dstPath)
        {
            CreateFile(dstPath);
            File.Copy(srcPath, dstPath, true);
            File.Delete(srcPath);
        }
        #endregion
    }
}
