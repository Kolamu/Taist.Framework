#pragma warning disable 0618
namespace Mock
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Collections.Generic;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Text;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Mock.Data;
    using Mock.Tools.Controls;
    using Mock.Nature.Native;
    using Mock.Data.Exception;
    using Mock.Tools.Exception;
    using System.Windows.Automation;
    using Microsoft.Win32;
    using TaskScheduler;

    using SHDocVw;

    /// <summary>
    /// 表示测试机线程执行工作的句柄
    /// </summary>
    public delegate void Job();

    /// <summary>
    /// 表示日志类型
    /// </summary>
    public enum NoteType
    {
        /// <summary>
        /// 错误类型
        /// </summary>
        ERROR,

        /// <summary>
        /// 成功类型
        /// </summary>
        SUCCESS,

        /// <summary>
        /// 失败类型
        /// </summary>
        FAILED,

        /// <summary>
        /// 警告类型
        /// </summary>
        WARNING,

        /// <summary>
        /// 异常类型
        /// </summary>
        EXCEPTION,

        /// <summary>
        /// 提示类型
        /// </summary>
        MESSAGE,

        /// <summary>
        /// 调试日志
        /// </summary>
        DEBUG,

        /// <summary>
        /// 可以忽略的日志
        /// </summary>
        IGNORE,
    }

    /// <summary>
    /// 表示测试机器对象
    /// </summary>
    public class Robot
    {
        private static Process kpProcess = null;
        //private static string name = "Robot";
        private static Dictionary<string, Thread> threadList = new Dictionary<string, Thread>();
        private static long total = 0;
        private static string mac = null;
        private static string ip = null;
        private readonly static TaistLog log = null;
        private delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            //Console.WriteLine(CtrlType);
            //switch (CtrlType)
            //{

            //    case 0:

            //        Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭 

            //        break;

            //    case 2:
            //        Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭 
            //        break;

            //}
            //try
            //{
            //    foreach (KeyValuePair<string, TelenetClient> kv in telnetDic)
            //    {
            //        try
            //        {
            //            kv.Value.Logout();
            //        }
            //        catch { }
            //    }
            //    serverSocket.Shutdown(SocketShutdown.Both);
            //    serverSocket.Close();
            //}
            //catch { }
            Robot.Note(">>>>>>>>>>Control Handler");
            log.Close();
            return false;

        }

        static Robot()
        {
            log = new TaistLog();
            SetConsoleCtrlHandler(HandlerRoutine, true);

            CaseManager.CaseRemoved += CaseManager_CaseRemoved;
        }

        static void CaseManager_CaseRemoved(string identification)
        {
            log.Close(Path.Combine(Config.WorkingDirectory, identification + ".log"));
        }

        /// <summary>
        /// 判断当前是否存在测试程序进程
        /// </summary>
        public static bool IsSoftwareRunning
        {
            get
            {
                Process[] ps = Process.GetProcessesByName(Config.SoftwareProcessName);
                if (ps == null || ps.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 使用IE打开指定的地址
        /// </summary>
        /// <param name="url"></param>
        public static void StartInternetExplore(string url)
        {
            Process p = Process.Start("iexplore.exe", url);
            Recess(1000);
            ShellWindows shellWindows = new SHDocVw.ShellWindows();
            foreach (SHDocVw.InternetExplorer ie in shellWindows)
            {
                try
                {
                    if (ie.FullName.ToUpper().IndexOf("IEXPLORE.EXE") > 0)
                    {
                        AutomationElement e = AutomationElement.FromHandle((IntPtr)ie.HWND);
                        if (e.Current.ProcessId == p.Id)
                        {
                            while (ie.Busy)
                            {
                                Recess(100);
                            }
                            break;
                        }
                    }
                }
                catch { }
            }
            shellWindows = null;
        }

        /// <summary>
        /// 关闭所有IE窗口
        /// </summary>
        public static void CloseInternetExplore()
        {
            Process[] ps = Process.GetProcessesByName("iexplore");
            foreach (Process p in ps)
            {
                try
                {
                    p.Kill();
                }
                catch { }
            }
        }

        /// <summary>
        /// 获取当前CPU使用率
        /// </summary>
        public static int CPU
        {
            get
            {
                PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                cpu.NextValue();
                return (int)cpu.NextValue();
            }
        }

        /// <summary>
        /// 获取当前内存使用值
        /// </summary>
        public static int Memory
        {
            get
            {

                if (total == 0)
                {
                    ManagementClass mosTotal = new ManagementClass("Win32_ComputerSystem");
                    ManagementObjectCollection moc = mosTotal.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        if (mo["TotalPhysicalMemory"] != null)
                        {
                            total = long.Parse(mo["TotalPhysicalMemory"].ToString());
                        }
                    }
                }

                ManagementClass mosFree = new ManagementClass("Win32_OperatingSystem");
                ManagementObjectCollection mocFree = mosFree.GetInstances();
                ManagementObjectCollection.ManagementObjectEnumerator moeFree = mocFree.GetEnumerator();
                moeFree.MoveNext();
                ManagementObject moFree = (ManagementObject)moeFree.Current;
                long free = long.Parse(moFree["FreePhysicalMemory"].ToString()) * 1024;

                return (int)(((total - free) * 100 / total));
            }
        }

        /// <summary>
        /// 获取本地MAC地址
        /// </summary>
        public static string MAC
        {
            get
            {
                if (mac == null)
                {
                    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            mac = (string)mo["MacAddress"];
                            mac = mac.Replace(':','-');
                        }
                    }
                }
                return mac;
            }
        }

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        public static string IP
        {
            get
            {
                if (ip == null)
                {
                    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            string []ips = mo["IPAddress"] as string[];
                            foreach (string tmp in ips)
                            {
                                System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(tmp);
                                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    ip = tmp;
                                }
                            }
                        }
                    }
                }
                return ip;
            }
        }

        /// <summary>
        /// 关闭被测软件
        /// </summary>
        public static void CloseSoftware()
        {
            Process[] tmp = Process.GetProcessesByName(Config.SoftwareProcessName);
            if (tmp.Length > 0)
            {
                kpProcess = tmp[0];
                kpProcess.Kill();
            }
            Recess(2000);
        }

        private static Dictionary<Job, Exception> JobExceptionList = new Dictionary<Job, Exception>();

        /// <summary>
        /// 执行一段代码直到结束或超时
        /// </summary>
        /// <param name="job">函数</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="throwOnTimeout"></param>
        public static void ExecuteWithTimeOut(Job job, int timeout = -1, bool throwOnTimeout = true)
        {
            if (timeout == -1)
            {
                timeout = Config.TimeOut;
            }
            //AutoResetEvent JobTimeOutEvent = new AutoResetEvent(false);
            //JobTimeOutEvent.Reset();
            //if (JobTimeOutEventList == null)
            //{
            //    JobTimeOutEventList = new Dictionary<Job, AutoResetEvent>();
            //}
            //JobTimeOutEventList.Add(job, JobTimeOutEvent);
            if (JobExceptionList == null)
            {
                JobExceptionList = new Dictionary<Job, Exception>();
            }
            JobExceptionList.Add(job, null);
            Thread t = new Thread(new ParameterizedThreadStart(RunJob));
            t.IsBackground = true;
            t.Start(job);

            bool b = true;
            try
            {
                if (timeout == 0)
                {
                    //b = JobTimeOutEvent.WaitOne();
                    t.Join();
                }
                else
                {
                    //b = JobTimeOutEvent.WaitOne(timeout);
                    b = t.Join(timeout);
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            finally
            {
                t.Abort();
                Exception JobException = JobExceptionList[job];
                JobExceptionList.Remove(job);
                //JobTimeOutEventList.Remove(job);
                if (!b && throwOnTimeout)
                {
                    StackTrace st = new StackTrace(true);
                    string methodName = st.GetFrame(1).GetMethod().Name;
                    string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                    throw new TimeOutException(string.Format("{0}.{1} Run job", className, methodName));
                }

                if (JobException != null && !(JobException is ThreadAbortException))
                {
                    throw JobException;
                }
            }
        }

        private static void RunJob(object job)
        {
            Job j = job as Job;
            try
            {
                j();
                //try
                //{
                //    JobTimeOutEventList[j].Set();
                //}
                //catch { }
            }
            catch (ThreadInterruptedException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogManager.DebugFormat("RunJob exception {0}\n {1}", ex.Message, ex.StackTrace);
                try
                {
                    JobExceptionList[j] = ex;
                }
                catch { }
                //try
                //{
                //    JobTimeOutEventList[j].Set();
                //}
                //catch { }
            }
        }

        #region 线程操作
        /// <summary>
        /// 创建一个新的任务
        /// </summary>
        /// <param name="jobName">任务名</param>
        /// <param name="job">任务方法体</param>
        public static void NewJob(string jobName, Job job)
        {
            if (ThreadException == null)
            {
                ThreadException += new RobotExceptionEventHandler(OnRobotExceptionThrowed);
            }
            //Console.WriteLine(string.Format("{0,10}: Hi, new job [{1}] comming!", "Manager", jobName));
            if (threadList.ContainsKey(jobName))
            {
                if (threadList[jobName].ThreadState != System.Threading.ThreadState.Stopped && threadList[jobName].ThreadState != System.Threading.ThreadState.Aborted)
                {
                    threadList[jobName].Abort();
                }
                threadList.Remove(jobName);
            }
            Thread thread1 = new Thread(new ThreadStart(job));
            //thread1.SetApartmentState(ApartmentState.STA);
            thread1.IsBackground = true;
            thread1.Name = jobName;
            thread1.Start();
            
            threadList.Add(jobName, thread1);
        }
        
        /// <summary>
        /// 调用此方法使一个或所有任务处于等待状态
        /// </summary>
        /// <param name="jobName">等待的任务名称，若为null，则代表所有任务</param>
        public static void JobsWait(string jobName = null)
        {
            Thread thread;
            if (jobName == null)
            {
                string []keys = new string[threadList.Keys.Count];
                threadList.Keys.CopyTo(keys, 0);
                foreach (string names in keys)
                {
                    thread = threadList[names];
                    if (thread == Thread.CurrentThread)
                    {
                        continue;
                    }
                    if (thread != null)
                    {
                        if (thread.ThreadState == System.Threading.ThreadState.Stopped || thread.ThreadState == System.Threading.ThreadState.Aborted)
                        {
                            threadList.Remove(names);
                            continue;
                        }

                        if (thread.ThreadState != System.Threading.ThreadState.Suspended)
                        {
                            thread.Suspend();
                        }
                    }
                }
            }
            else
            {
                if (!threadList.ContainsKey(jobName))
                {
                    return;
                }
                thread = threadList[jobName];
                if (thread.ThreadState == System.Threading.ThreadState.Stopped || thread.ThreadState == System.Threading.ThreadState.Aborted)
                {
                    threadList.Remove(jobName);
                    return;
                }
                if (thread != null)
                {
                    if (thread.ThreadState != System.Threading.ThreadState.Suspended)
                    {
                        thread.Suspend();
                    }
                }
            }
        }

        /// <summary>
        /// 调用此方法使一个或所有处于等待状态的任务处于运行状态
        /// </summary>
        /// <param name="jobName">等待状态的任务名称，若为null，则代表所有任务</param>
        public static void StartJobs(string jobName = null)
        {
            if (ThreadException == null)
            {
                ThreadException += new RobotExceptionEventHandler(OnRobotExceptionThrowed);
            }
            Thread thread;
            if (jobName == null)
            {
                foreach (string names in threadList.Keys)
                {
                    thread = threadList[names];
                    if (thread == Thread.CurrentThread)
                    {
                        continue;
                    }
                    if (thread != null)
                    {
                        if (thread.ThreadState == System.Threading.ThreadState.Stopped || thread.ThreadState == System.Threading.ThreadState.Aborted)
                        {
                            threadList.Remove(names);
                            continue;
                        }

                        if (thread.ThreadState != System.Threading.ThreadState.Running)
                        {
                            thread.Resume();
                        }
                    }
                }
            }
            else
            {
                if (!threadList.ContainsKey(jobName))
                {
                    return;
                }
                thread = threadList[jobName];
                if (thread.ThreadState == System.Threading.ThreadState.Stopped || thread.ThreadState == System.Threading.ThreadState.Aborted)
                {
                    threadList.Remove(jobName);
                    return;
                }
                if (thread != null)
                {
                    if (thread.ThreadState != System.Threading.ThreadState.Running)
                    {
                        thread.Resume();
                    }
                }
            }
        }

        /// <summary>
        /// 调用此方法停止一个或所有任务
        /// </summary>
        /// <param name="jobName">任务名称，若为null，则代表所有任务</param>
        public static void StopJobs(string jobName = null)
        {
            Thread thread;
            if (jobName == null)
            {
                string[] keys = new string[threadList.Keys.Count];
                threadList.Keys.CopyTo(keys, 0);
                foreach (string names in keys)
                {
                    thread = threadList[names];
                    if (thread == Thread.CurrentThread)
                    {
                        continue;
                    }
                    if (thread != null)
                    {
                        if (thread.ThreadState == System.Threading.ThreadState.Stopped || thread.ThreadState == System.Threading.ThreadState.Aborted)
                        {
                            continue;
                        }

                        if (thread.ThreadState != System.Threading.ThreadState.Stopped)
                        {
                            thread.Abort();
                        }
                    }
                    threadList.Remove(names);
                }
            }
            else
            {
                if (!threadList.ContainsKey(jobName))
                {
                    return;
                }
                thread = threadList[jobName];
                if (thread.ThreadState == System.Threading.ThreadState.Stopped || thread.ThreadState == System.Threading.ThreadState.Aborted)
                {
                    return;
                }
                if (thread != null)
                {
                    if (thread.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        thread.Abort();
                    }
                }
                threadList.Remove(jobName);
            }
        }

        /// <summary>
        /// 判断指定名称的任务是否已经停止
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <returns></returns>
        public static bool JobStoped(string name)
        {
            if(!threadList.ContainsKey(name))
            {
                return true;
            }

            Thread thread = threadList[name];
            return thread.ThreadState == System.Threading.ThreadState.Stopped || thread.ThreadState == System.Threading.ThreadState.Aborted;
        }

        /// <summary>
        /// 将任务从任务链中移除，但不终止任务
        /// </summary>
        /// <param name="jobName">任务名称，若为null，则代表所有任务</param>
        public static void RemoveJob(string jobName = null)
        {
            if (jobName == null)
            {
                threadList.Clear();
                return;
            }
            if (threadList.ContainsKey(jobName))
            {
                threadList.Remove(jobName);
            }
        }

        #endregion

        /// <summary>
        /// 写日志信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="ntype">类型</param>
        /// <param name="path">日志路径</param>
        /// <param name="ignoreConfig">忽略配置</param>
        public static void Note(string message, NoteType ntype = NoteType.MESSAGE, string path = null, bool ignoreConfig = false)
        {
            string directory = null;
            if (ignoreConfig)
            {
                directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                if (!Config.Debug && ntype == NoteType.DEBUG)
                {
                    return;
                }
                directory = Config.WorkingDirectory;
            }
            string logpath = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                try
                {
                    logpath = Path.Combine(directory, CaseManager.GetIdentification() + ".log");
                }
                catch
                {
                    logpath = Path.Combine(directory, "Robot.log");
                }
            }
            else
            {
                if (path.Contains(":"))
                {
                    logpath = path;
                }
                else
                {
                    logpath = Path.Combine(directory, path);
                }
            }
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string strT = currentTime.ToString("u");

            
            string msg = string.Format("[{0,20}] [{1}] {2}", strT, Enum.GetName(typeof(NoteType), ntype), message);
            
            log.Write(msg, logpath);
        }

        public static void Recess(int millionSecond)
        {
            Thread.Sleep(millionSecond);
        }

        /// <summary>
        /// 等待指定时间
        /// </summary>
        /// <param name="millionSecond">等待的时间</param>
        public static void Recess(double millionSecond)
        {
            Thread.Sleep((int)millionSecond);
        }

        private delegate void RobotExceptionEventHandler(object sender, TaistException exception);

        private static void OnRobotExceptionThrowed(object sender, TaistException exception)
        {
            if (ThreadException != null)
            {
                LogManager.Error(exception);
            }
        }

        private static event RobotExceptionEventHandler ThreadException = null;

        /// <summary>
        /// 预留方法
        /// </summary>
        /// <param name="exception"></param>
        public static void ThrowRobotException(TaistException exception)
        {
            if (ThreadException != null)
            {
                ThreadException(Thread.CurrentThread, exception);
            }
        }

        /// <summary>
        /// 添加自动化测试控件事件
        /// </summary>
        /// <param name="ControlEvent"></param>
        public static void AddTaistControEventListener(TaistControlEventHandler ControlEvent)
        {
            RobotContext.ControlEvent += ControlEvent;
        }

        /// <summary>
        /// 移除自动化测试控件事件
        /// </summary>
        /// <param name="ControlEvent"></param>
        public static void RemoveTaistControlEventListener(TaistControlEventHandler ControlEvent)
        {
            RobotContext.ControlEvent -= ControlEvent;
        }

        #region 获取本地安装文件日期
        /// <summary>
        /// 获取被测软件启动文件的创建日期
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFileTime()
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Process[] ps = Process.GetProcessesByName(Config.SoftwareProcessName);
                    if (ps != null && ps.Length > 0)
                    {
                        Process p = ps[0];
                        
                        string path = p.MainModule.FileName;
                        System.IO.FileInfo fInfo = new System.IO.FileInfo(path);
                        return fInfo.LastWriteTime;
                    }
                    else
                    {
                        return Config.SoftwareVersionDate;
                    }
                }
                catch
                {
                    Recess(500);
                }
            }
            return Config.SoftwareVersionDate;
        }
        #endregion

        #region 获取软件版本
        /// <summary>
        /// 获取测试软件的版本
        /// </summary>
        /// <returns></returns>
        public static VerInfo GetSoftwareVersion()
        {
            DateTime date = GetFileTime();
            if (!File.Exists(Config.ControlLibraryPath))
            {
                VerInfo vi = new VerInfo();
                vi.VerId = "1";
                return vi;
            }
            string VersionXmlString = DataFactory.ReadLibrary(Config.ControlLibraryPath, "Version");

            XmlDocument doc = new XmlDocument();
            
            if (string.IsNullOrEmpty(VersionXmlString))
            {
                VerInfo vi = new VerInfo();
                vi.VerId = "1";
                return vi;
            }
            else
            {
                doc.LoadXml(VersionXmlString);
            }

            XmlNode node = doc.SelectSingleNode(string.Format("//VerInfo[StartDate <= {0} and EndDate >= {0}]", date.ToString("yyyyMMdd")));
            if (node == null)
            {
                throw new CanNotFindNodeException("Version", string.Format("//{1}/VerInfo[StartDate <= {0} and EndDate >= {0}]", date.ToString("yyyyMMdd"), Config.SoftwareProcessName));
            }
            else
            {
                return DataFactory.XmlToObject<VerInfo>(node);
            }
        }
        #endregion

        #region 运行软件
        public static int Exec(string fileName, int delay, string args = null, bool highLevel =false, ExecRunningMethod method = null, int timeout = 1800000)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            Process p = null;
            if (Environment.OSVersion.Version.Major == 5)
            {
                p = new Process();
                p.StartInfo.FileName = fileName;
                p.StartInfo.Arguments = args;
                p.Start();
            }
            else
            {
                TaskScheduler scheduler = new TaskScheduler();
                scheduler.Connect(null, null, null, null);

                ITaskDefinition task = scheduler.NewTask(0);
                task.RegistrationInfo.Author = "Robot Exec";
                task.RegistrationInfo.Description = "Robot Exec";
                task.Settings.RunOnlyIfIdle = false;
                if (highLevel)
                {
                    task.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
                }

                ITimeTrigger trigger = (ITimeTrigger)task.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
                trigger.Id = "ExecTrigger";
                DateTime time = DateTime.Now.AddSeconds(delay);
                trigger.StartBoundary = string.Format("{0}T{1}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss"));
                trigger.EndBoundary = string.Format("{0}T{1}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss"));

                IExecAction action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                action.Id = "Exec action";
                action.Path = Path.GetFullPath(fileName);
                if (!string.IsNullOrEmpty(args))
                {
                    action.Arguments = args;
                }

                ITaskFolder folder = scheduler.GetFolder("\\");
                IRegisteredTask regTask = folder.RegisterTaskDefinition(
                Path.GetFileNameWithoutExtension(fileName),
                task,
                (int)_TASK_CREATION.TASK_CREATE_OR_UPDATE,
                null, //user
                null, // password
                _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN,
                "");

                IRunningTask runTask = regTask.Run(null);
                for (int i = 0; i < 60; i++)
                {
                    Process[] installProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fileName));
                    if (installProcesses.Length > 0)
                    {
                        p = installProcesses[0];
                        break;
                    }
                    Robot.Recess(1000);
                }
                Exception ex = null;
                if (p == null)
                {
                    ex = new Exception(string.Format("Exec {0} failed", fileName));
                }
                try
                {
                    folder.DeleteTask(Path.GetFileNameWithoutExtension(fileName), 0);
                }
                catch { }
                if (ex != null) throw ex;
            }
            if (timeout < 0)
            {
                return 0;
            }
            int id = execId;
            execId++;
            Timer actionTimer = new Timer(TimerCallback, id, timeout, 10000);
            if (actionTimerDictionary == null)
            {
                actionTimerDictionary = new Dictionary<int, Timer>();
            }
            actionTimerDictionary.Add(id, actionTimer);
            int pHandle = NativeMethods.OpenProcess(0x0400, false, p.Id);
            while (!p.HasExited)
            {
                if (!actionTimerDictionary.Keys.Contains(id))
                {
                    p.Kill();
                    throw new Exception(string.Format("Exec {0} timeout", fileName));
                }
                if (method != null)
                {
                    method();
                }
                Recess(1000);
            }
            int exitCode = 0;
            NativeMethods.GetExitCodeProcess(pHandle, ref exitCode);
            NativeMethods.CloseHandle(pHandle);
            return exitCode;
        }
        private static Dictionary<int,Timer> actionTimerDictionary = null;
        private static int execId = 0;
        private static void TimerCallback(object state)
        {
            try
            {
                int id = (int)state;
                if (actionTimerDictionary[id] != null)
                {
                    actionTimerDictionary[id].Dispose();
                    actionTimerDictionary.Remove(id);
                }
            }
            catch { }
        }

        #endregion

        #region SavePrintedImage
        /// <summary>
        /// 保存打印文件到Tiff格式图片并删除打印任务
        /// </summary>
        public static void SavePrintedImage()
        {
            string path = @"C:\Windows\System32\spool\PRINTERS";
            string tmpPath = string.Format("{0}\\tmp", path);
            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }
            string[] fileNameArray = Directory.GetFiles(path);
            List<string> fileNameList = new List<string>();
            foreach (string fileName in fileNameArray)
            {
                if (fileName.ToLower().EndsWith(".spl"))
                {
                    string dstPath = string.Format("{0}\\{1}", tmpPath, Path.GetFileName(fileName));
                    fileNameList.Add(dstPath);
                    File.Copy(fileName, dstPath);
                }
            }
            DeletePrintJobs();
            foreach (string fileName in fileNameList)
            {
                Spl2Tiff(fileName);
            }
            Directory.Delete(tmpPath, true);
        }

        private static void Spl2Tiff(string path)
        {
            FileStream fs = File.OpenRead(path);
            byte[] dword = new byte[4];
            byte[] buf = new byte[1024];
            fs.Seek(4, SeekOrigin.Begin);
            fs.Read(dword, 0, 4);
            int emfPos = DWORD2INT(dword);

            fs.Seek(16, SeekOrigin.Begin);

            byte[] bs = new byte[256];
            bool zero = false;
            int count = 0;
            for (int i = 0; i < 256; i++)
            {
                int b = fs.ReadByte();

                if (b == 0)
                {
                    if (zero)
                    {
                        break;
                    }
                    else
                    {
                        zero = true;
                        bs[i] = (byte)b;
                    }
                }
                else
                {
                    zero = false;
                    bs[i] = (byte)b;
                }
                count++;
            }
            string name = Encoding.Unicode.GetString(bs, 0, count);

            fs.Seek(emfPos + 4, SeekOrigin.Begin);
            fs.Read(dword, 0, 4);
            int length = DWORD2INT(dword);
            FileStream tmpEmf = File.Create(@"C:\Windows\System32\spool\PRINTERS\tmp\tmp.emf");
            for (int i = 0; i < length / 1024; i++)
            {
                fs.Read(buf, 0, 1024);
                //fs.Seek(1024, SeekOrigin.Current);
                tmpEmf.Write(buf, 0, 1024);
                tmpEmf.Flush();
            }

            fs.Read(buf, 0, length % 1024);
            tmpEmf.Write(buf, 0, length % 1024);
            tmpEmf.Flush();
            fs.Close();
            tmpEmf.Close();

            //EMF -> Tiff
            Metafile meta = new Metafile(@"C:\Windows\System32\spool\PRINTERS\tmp\tmp.emf");
            Bitmap bitmap = new Bitmap(meta, 2543, 1646);
            bitmap.SetResolution(300, 300);
            Image background = Properties.Resources.bb;
            Bitmap newBit = new Bitmap(background.Width, background.Height, PixelFormat.Format24bppRgb);
            newBit.SetResolution(300, 300);
            Graphics g = Graphics.FromImage(newBit);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawImage(background, 0, 0);
            g.RotateTransform(0.10f);
            g.DrawImage(bitmap, 254, 38);
            //g.Dispose();
            newBit.RotateFlip(RotateFlipType.Rotate90FlipNone);
            if (!Directory.Exists(Config.PrintImageDirectory))
            {
                Directory.CreateDirectory(Config.PrintImageDirectory);
            }
            newBit.Save(string.Format("{0}\\{1}.tif", Config.PrintImageDirectory.TrimEnd('\\'), name), ImageFormat.Tiff);
            g.Dispose();
            newBit.Dispose();
            background.Dispose();
            bitmap.Dispose();
            meta.Dispose();

            File.Delete(@"C:\Windows\System32\spool\PRINTERS\tmp\tmp.emf");
        }

        static int DWORD2INT(byte[] dword)
        {
            string value = string.Empty;
            for (int i = 3; i >= 0; i--)
            {
                value += dword[i].ToString("X2");
            }
            return Convert.ToInt32(value, 16);
        }

        static string DWORD2STRING(byte[] dword)
        {
            string value = string.Empty;
            for (int i = 3; i >= 0; i--)
            {
                value += dword[i].ToString("X2");
            }
            return value;
        }

        #region 删除打印任务
        private static void DeletePrintJobs()
        {
            ManagementObjectSearcher printJobSearcher;
            ManagementObjectCollection printJobCollection;
            try
            {
                printJobSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PrintJob");
                printJobCollection = printJobSearcher.Get();
                foreach (ManagementObject printJob in printJobCollection)
                {
                    printJob.Delete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
        #endregion

        #endregion

        #region 转换测试软件
        /// <summary>
        /// 转换测试软件
        /// 
        /// 执行该操作来设置taist.config中的SoftwareProcessName项，执行后，自动化测试框架
        /// 只负责操作指定进程的窗口，其他进程
        /// 
        /// </summary>
        /// <param name="softwareProcessName">要转换的软件进程名称（进程名不包含.exe后缀）</param>
        /// <param name="versionDate">要转换的软件可执行文件日期（格式：yyyyMMdd）若该项为null或空，将不进行版本设置，若要执行最新版本，请输入99991231</param>
        public static void SwitchTestSoftware(string softwareProcessName, string versionDate = "99991231")
        {
            Config.SoftwareProcessName = softwareProcessName;
            if (string.IsNullOrEmpty(versionDate))
            {
                Config.SoftwareVersionDate = DateTime.ParseExact(versionDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        #endregion
    }

    public delegate void ExecRunningMethod();
}
