namespace Mock
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;
    using System.Collections.Concurrent;

    using Mock.Data;
    internal class TaistLog : IDisposable
    {
        private ConcurrentDictionary<string, LogFileInfo> logStreamList = new ConcurrentDictionary<string, LogFileInfo>();
        private bool stop = false;

        ~TaistLog()
        {
            Dispose();
        }

        public void Start()
        {
            stop = false;
        }

        public void Stop()
        {
            stop = true;
        }

        private readonly object logObj = new object();
        public void Write(string message, string path)
        {
            lock (logObj)
            {
                if (stop)
                {
                    throw new LogException("Logger has stopped");
                }

                path = Path.GetFullPath(path);
                LogFileInfo info = null;

                if (logStreamList.ContainsKey(path))
                {
                    info = logStreamList[path];
                }
                else
                {
                    info = new LogFileInfo(path);
                    logStreamList.GetOrAdd(path, info);
                }

                info.Write(message);
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            stop = true;
            if (logStreamList != null)
            {
                foreach (LogFileInfo info in logStreamList.Values)
                {
                    info.Stop();
                }
                logStreamList.Clear();
            }
        }

        public void Close(string path)
        {
            if (logStreamList != null && logStreamList.ContainsKey(path))
            {
                LogFileInfo fileInfo = logStreamList[path];
                fileInfo.Stop();
                logStreamList.TryRemove(path, out fileInfo);
            }
        }

        private class LogFileInfo
        {
            private string _path = null;

            ~LogFileInfo()
            {

            }

            internal LogFileInfo(string filePath)
            {
                _path = filePath;
                FileFactory.CreateFile(_path);
                messageList = new List<string>();
                stopped = false;
                new Thread(WriteThread) { IsBackground = true }.Start();
            }

            internal string FilePath
            {
                get
                {
                    return _path;
                }
            }

            public void Stop()
            {
                while (!canStop)
                {
                    Robot.Recess(10);
                }
                stopped = true;
                reset.Set();
                lock (logLock)
                {
                    using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                        {
                            foreach (string s in messageList)
                            {
                                sw.WriteLine(s);
                                sw.Flush();
                            }
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
                Robot.Recess(20);
            }

            private List<string> messageList = new List<string>();
            private AutoResetEvent reset = new AutoResetEvent(false);
            private readonly object logLock = new object();
            private bool stopped = false;
            private bool canStop = false;

            public void Write(string message)
            {
                lock (logLock)
                {
                    messageList.Add(message);
                }
                if (canStop)
                {
                    reset.Set();
                }
            }

            private void WriteThread()
            {
                using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    fs.Seek(0, SeekOrigin.End);
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                    {
                        while (!stopped)
                        {
                            List<string> list = new List<string>();
                            lock (logLock)
                            {
                                list.AddRange(messageList);
                                messageList.Clear();
                            }
                            if (list.Count == 0)
                            {
                                canStop = true;
                                reset.Reset();
                                reset.WaitOne();
                            }
                            else
                            {
                                canStop = false;
                                try
                                {
                                    FileInfo fi = new FileInfo(_path);
                                    DateTime dt = DateTime.Now;
                                    if (fi.Length > 10485760)
                                    {
                                        string bakpath = string.Format("{0}\\{1}{2}.log", Path.GetDirectoryName(_path), Path.GetFileNameWithoutExtension(_path), dt.ToString("yyyyMMddHHmmss"));
                                        try
                                        {
                                            fi.CopyTo(bakpath);
                                            fs.SetLength(0);
                                        }
                                        catch { }
                                    }
                                    foreach (string s in list)
                                    {
                                        sw.WriteLine(s);
                                        sw.Flush();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    using (StreamWriter err = new StreamWriter(_path + ".err", true, Encoding.Default))
                                    {
                                        string message = string.Format("[{2}] {0} {1}", ex.Message, ex.StackTrace, DateTime.Now);
                                        err.WriteLine(message);
                                        err.Flush();
                                        err.Close();
                                    }
                                }
                            }
                        }
                        sw.Close();
                    }
                    fs.Close();
                }
            }
        }

    }


    public class LogException : TaistException
    {
        public LogException(string message) : base(message) { }
    }
}
