namespace Mock.Data
{
    using System;
    using System.IO;
    using System.Text;
    using System.Runtime.InteropServices;

    using Mock.Data.Exception;
    using Mock.Nature.Native;
    public class IniFile
    {
        private static string _path = null;
        /// <summary>
        /// ini file full path
        /// </summary>
        public static string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        /// <summary>
        /// Get section key value
        /// </summary>
        /// <param name="section"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string GetValue(string section, string keyName)
        {
            if (string.IsNullOrEmpty(_path))
            {
                throw new CanNotFindFileException(_path);
            }

            if (!File.Exists(_path))
            {
                throw new CanNotFindFileException(_path);
            }
            StringBuilder sb = new StringBuilder();
            Int64 ret = NativeMethods.GetPrivateProfileString(section, keyName, null, sb, 0, _path);
            return sb.ToString();
        }

        /// <summary>
        /// Write key value to section
        /// </summary>
        /// <param name="section"></param>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        public static void SetValue(string section, string keyName, string value)
        {
            if (string.IsNullOrEmpty(_path))
            {
                throw new CanNotFindFileException(_path);
            }

            if (!File.Exists(_path))
            {
                throw new CanNotFindFileException(_path);
            }

            NativeMethods.WritePrivateProfileString(section, keyName, value, _path);
        }
    }
}
