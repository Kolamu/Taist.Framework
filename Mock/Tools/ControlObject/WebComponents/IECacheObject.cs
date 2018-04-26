namespace Mock.Tools.Controls
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    using Mock.Nature.Native;
    class IECacheObject
    {
        const int ERROR_NO_MORE_ITEMS = 259;

        #region FileTimeToSystemTime

        private string FILETIMEtoDataTime(FILETIME time)
        {
            IntPtr filetime = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FILETIME)));
            IntPtr systime = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SYSTEMTIME)));
            Marshal.StructureToPtr(time, filetime, true);
            NativeMethods.FileTimeToSystemTime(filetime, systime);
            SYSTEMTIME st = (SYSTEMTIME)Marshal.PtrToStructure(systime, typeof(SYSTEMTIME));
            string Time = st.wYear.ToString() + "." + st.wMonth.ToString() + "." + st.wDay.ToString() + "." + st.wHour.ToString() + "." + st.wMinute.ToString() + "." + st.wSecond.ToString();
            return Time;
        }

        #endregion

        #region 加载数据
        internal byte[] GetFile(string fileName)
        {
            int nNeeded = 0, nBufSize;
            IntPtr buf;
            INTERNET_CACHE_ENTRY_INFO CacheItem;
            IntPtr hEnum;
            bool r;
            byte[] fileBytes = new byte[0];

            NativeMethods.FindFirstUrlCacheEntry(null, IntPtr.Zero, ref nNeeded);

            if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
                return fileBytes;

            nBufSize = nNeeded;
            buf = Marshal.AllocHGlobal(nBufSize);
            hEnum = NativeMethods.FindFirstUrlCacheEntry(null, buf, ref nNeeded);
            while (true)
            {
                CacheItem = (INTERNET_CACHE_ENTRY_INFO)Marshal.PtrToStructure(buf,
                typeof(INTERNET_CACHE_ENTRY_INFO));

                string modifiedTime = FILETIMEtoDataTime(CacheItem.LastModifiedTime);
                string expireTime = FILETIMEtoDataTime(CacheItem.ExpireTime);
                string accessTime = FILETIMEtoDataTime(CacheItem.LastAccessTime);
                string syncTime = FILETIMEtoDataTime(CacheItem.LastSyncTime);

                try
                {
                    string s1 = Marshal.PtrToStringAuto(CacheItem.lpszLocalFileName);
                    string s2 = Marshal.PtrToStringAuto(CacheItem.lpszSourceUrlName);
                    if (string.Equals(Path.GetFileName(s2), fileName))
                    {
                        using (FileStream fs = File.Open(s1, FileMode.Open))
                        {
                            fileBytes = new byte[fs.Length];
                            fs.Read(fileBytes, 0, (int)fs.Length);
                            fs.Close();
                        }
                        break;
                    }
                }
                catch { }

                nNeeded = nBufSize;
                r = NativeMethods.FindNextUrlCacheEntry(hEnum, buf, ref nNeeded);

                if (!r && Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
                    break;

                if (!r && nNeeded > nBufSize)
                {
                    nBufSize = nNeeded;
                    buf = Marshal.ReAllocHGlobal(buf, (IntPtr)nBufSize);
                    NativeMethods.FindNextUrlCacheEntry(hEnum, buf, ref nNeeded);
                }
            }
            Marshal.FreeHGlobal(buf);
            return fileBytes;
        }

        #endregion
    }
}
