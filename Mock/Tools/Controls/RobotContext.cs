namespace Mock.Tools.Controls
{
    using System;
    using System.Xml;
    using System.Collections;
    using System.Threading;
    using System.Collections.Generic;
    using System.Windows.Automation;
    using System.Diagnostics;

    using Mock.Data;
    using Mock.Nature.Native;
    using Mock.Data.Exception;
    using Mock.Tools.Exception;
    using Mock.Tools.Tasks;

    public delegate void IgnoreWindowEventHandler(WindowBaseInfo windowBaseInfo);
    public class RobotContext
    {
        static RobotContext()
        {
            new Thread(DoLast) { IsBackground = true }.Start();
            new Thread(WindowCollect) { IsBackground = true }.Start();
        }

        public static event IgnoreWindowEventHandler OnIgnoreWindow = null;

        internal static void InvokeIgnoreWindow(string name, string automationId, string className)
        {
            if (OnIgnoreWindow != null)
            {
                WindowBaseInfo baseInfo = new WindowBaseInfo()
                    {
                        WindowName = name,
                        AutomationId = automationId,
                        ClassName = className
                    };
                
                OnIgnoreWindow(baseInfo);
            }
        }

        private static bool _abort = false;
        public static bool ProgramAbort
        {
            get
            {
                return _abort;
            }
        }

        internal static void ReportAbort()
        {
            _abort = true;
            new Thread(() =>
                {
                    while (true)
                    {
                        Console.WriteLine(Config.SoftwareProcessName);
                        Process[] ps = Process.GetProcessesByName(Config.SoftwareProcessName);
                        if (ps != null && ps.Length > 0)
                        {
                            _abort = false;
                            break;
                        }
                        else
                        {
                            Robot.Recess(100);
                        }
                    }
                })
                {
                    IsBackground = true
                }.Start();
        }

        private static volatile int write = 0;

        private static ControlInfo controlInfo = null;
        
        private static string _windowName = null;
        /// <summary>
        /// 当前窗口名称
        /// </summary>
        internal static string WindowName
        {
            get
            {
                return _windowName;
            }
            set
            {
                LogManager.Start();
                LockMethod(() =>
                    {
                        do
                        {
                            if (string.Equals(_windowName, value))
                            {
                                //LogManager.Debug("window name equal");
                                break;
                            }

                            _windowName = value;
                            _window = null;

                            if (!containWindow())
                            {
                                //LogManager.Debug("not contain window");
                                break;
                            }
                            //LogManager.Debug("get old window");
                            IntPtr hWnd = _namedWindowDictionary[_windowName];
                            if (!NativeMethods.IsWindow(hWnd))
                            {
                                //LogManager.Debug("old window is not window");
                                break;
                            }

                            //LogManager.Debug("Handle to automation element");
                            _window = AutomationElement.FromHandle(hWnd);

                            if (_relativePathDictionary != null)
                            {
                                _relativePathDictionary.Clear();
                            }
                        } while (false);
                        return null;
                    });
            }
        }

        private static readonly object _controlLock = new object();

        internal static AutoResetEvent UpdateActiveWindowEvent { get; set; }

        /// <summary>
        /// 当前控件名称
        /// </summary>
        internal static string ElementName
        {
            get;
            set;
        }

        /// <summary>
        /// 当前控件缓存
        /// </summary>
        private static WindowsUnit _element;
        internal static WindowsUnit Element
        {
            get
            {
                return (WindowsUnit)LockMethod(() =>
                    {
                        return _element;
                    });
            }
            set
            {
                LockMethod(() =>
                    {
                        _element = value;
                        RSAA.Cell = _element;
                        return null;
                    });
            }
        }

        private static System.Threading.Timer _updateTimer = null;
        internal static System.Threading.Timer UpdateTimer
        {
            set
            {
                if (value == null)
                {
                    if (_updateTimer != null)
                    {
                        _updateTimer.Dispose();
                    }
                }
                else
                {
                    _updateTimer = value;
                }
            }
        }

        private static WindowsUnit _window;
        /// <summary>
        /// 当前窗口缓存
        /// </summary>
        internal static WindowsUnit Window
        {
            get
            {
                return (WindowsUnit)LockMethod(() =>
                    {
                        return _window;
                    });
            }
            set
            {
                LockMethod(() =>
                    {
                        _window = value;

                        if (_window == null) return null;

                        if (_namedWindowDictionary == null) _namedWindowDictionary = new Dictionary<string, IntPtr>();

                        if (_namedWindowDictionary.ContainsKey(_windowName))
                        {
                            _namedWindowDictionary[_windowName] = (IntPtr)_window.Current.NativeWindowHandle;
                        }
                        else
                        {
                            _namedWindowDictionary.Add(_windowName, (IntPtr)_window.Current.NativeWindowHandle);
                        }

                        if (_relativePathDictionary != null)
                        {
                            _relativePathDictionary.Clear();
                        }
                        return null;
                    });
            }
        }

        private static Dictionary<string, WindowsUnit> _relativePathDictionary = null;

        /// <summary>
        /// 获取缓存的相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static WindowsUnit getRelativePath(string path)
        {
            LogManager.Start();
            try
            {
                if (_relativePathDictionary == null)
                {
                    _relativePathDictionary = new Dictionary<string, WindowsUnit>();
                }
                if (_relativePathDictionary.ContainsKey(path))
                {
                    return (WindowsUnit)LockMethod(() =>
                        {
                            WindowsUnit element = _relativePathDictionary[path];
                            if (element.Current.ControlType == ControlType.TabItem)
                            {
                                SelectionItemPattern sip = element.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                                if (sip.Current.IsSelected)
                                {
                                    return element;
                                }
                                else
                                {
                                    WindowsUnit parent = TreeWalker.ControlViewWalker.GetParent(element);
                                    element = parent.FindFirst(TreeScope.Children, new PropertyCondition(SelectionItemPattern.IsSelectedProperty, true));
                                    return element;
                                }
                            }
                            else
                            {
                                return _relativePathDictionary[path];
                            }
                        });
                }
                else
                {
                    return findRelativePath(path);
                }
            }
            catch (NullControlException) { throw; }
            catch (MultiControlException) { throw; }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                throw new NullControlException(path);
            }
        }

        private static WindowsUnit findRelativePath(string path)
        {
            LogManager.Start();
            return (WindowsUnit)LockMethod(() =>
                {
                    if (path == null)
                    {
                        throw new NullControlException("null");
                    }
                    string[] parentsName = path.Split('/');
                    WindowsUnit parent = _window;
                    if (parent == null)
                    {
                        LogManager.Debug("parent is null");
                        throw new NullControlException(path);
                    }
                    PropertyCondition condition = null;
                    foreach (string parentName in parentsName)
                    {
                        if (parentName.ToUpper().StartsWith("ID:"))
                        {
                            int id = int.Parse(parentName.Substring(3).Trim());
                            WindowsUnit tmp = TreeWalker.ControlViewWalker.GetFirstChild(parent);
                            List<WindowsUnit> childList = new List<WindowsUnit>();
                            childList.Clear();
                            while (tmp != null)
                            {
                                childList.Insert(0, tmp);
                                tmp = TreeWalker.ControlViewWalker.GetNextSibling(tmp);
                            }
                            parent = childList[id - 1];
                            childList.Clear();
                        }
                        else
                        {
                            condition = new PropertyCondition(AutomationElement.AutomationIdProperty, parentName);
                            AutomationElementCollection childs = parent.FindAll(TreeScope.Children, condition);
                            if (childs.Count == 1)
                            {
                                parent = childs[0];
                            }
                            else if (childs.Count > 1)
                            {
                                throw new MultiControlException(path);
                            }
                            else
                            {
                                throw new NullControlException(path);
                            }
                            if (parent.Current.ControlType == ControlType.Tab)
                            {
                                parent = parent.FindFirst(TreeScope.Children, new PropertyCondition(SelectionItemPattern.IsSelectedProperty, true));
                                if (parent == null)
                                {
                                    throw new NullControlException(path);
                                }
                            }
                        }
                    }
                    _relativePathDictionary.Add(path, parent);
                    return parent;
                });
        }

        //private static int _internetexploreDepth = 0;
        /// <summary>
        /// IE元素相对高度
        /// </summary>
        internal static WindowsUnit InternetExploreServer
        {
            get
            {
                Condition cond = new PropertyCondition(AutomationElement.ClassNameProperty, "Internet Explorer_Server");
                WindowsUnit _server = RobotContext.InternetExploreWindow.FindFirst(TreeScope.Descendants, cond);
                if (_server == null)
                {
                    throw new NullControlException("Internet Explore Server");
                }
                return _server;
            }
        }

        private static WindowsUnit _internetexploreWindow;
        /// <summary>
        /// IE窗口
        /// </summary>
        internal static WindowsUnit InternetExploreWindow
        {
            get
            {
                return _internetexploreWindow;
            }
            set
            {
                _internetexploreWindow = value;
            }
        }

        //private static bool _isWebObjectInitilized = false;
        ///// <summary>
        ///// IE控件初始化标志
        ///// </summary>
        //internal static bool IsWebObjectInitilized
        //{
        //    get
        //    {
        //        return _isWebObjectInitilized;
        //    }
        //    set
        //    {
        //        _isWebObjectInitilized = value;
        //    }
        //}

        private static bool _isWindowsObjectInitilized = false;
        /// <summary>
        /// windows 窗体控件初始化标志
        /// </summary>
        internal static bool IsWindowsObjectInitilized
        {
            get
            {
                return _isWindowsObjectInitilized;
            }
            set
            {
                _isWindowsObjectInitilized = value;
            }
        }

        private static List<string> warningTable = new List<string>();
        /// <summary>
        /// 属于警告窗口列表中的窗口出现标志
        /// </summary>
        public static bool IsWarningWindowExist
        {
            get
            {
                try
                {
                    if (warningTable == null) return false;
                    bool ret = warningTable != null && warningTable.Count > 0;
                    //2017.09.07 再次确认是否真实存在
                    if (ret)
                    {
                        Dictionary<string, IntPtr> currentWindowDictionary = new Dictionary<string, IntPtr>();
                        lock (_controlLock)
                        {
                            if (_allWindowDictionary == null) return false;
                            foreach (object key in _allWindowDictionary.Keys)
                            {
                                currentWindowDictionary.Add((string)key, (IntPtr)_allWindowDictionary[key]);
                            }
                        }
                        foreach (KeyValuePair<string, IntPtr> kv in currentWindowDictionary)
                        {
                            if (NativeMethods.IsWindow(kv.Value) && NativeMethods.IsWindowVisible(kv.Value))
                            {
                                try
                                {
                                    WindowsUnit element = AutomationElement.FromHandle(kv.Value);
                                    if (Config.WarningWindowList.Contains(element.Current.AutomationId) || Config.WarningWindowList.Contains(element.Current.Name))
                                    {
                                        return true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogManager.Error(ex);
                                }
                            }
                        }
                    }
                    warningTable.Clear();
                }
                catch (Exception ex)
                {
                    LogManager.DebugX("IsWarningWindowExist exception {0}\n{1}", ex.Message, ex.StackTrace);
                }
                return false;
            }
        }

        private static Hashtable _allWindowDictionary = null;
        private static Dictionary<string, WindowBaseInfo> _windowBaseInfoDictionary = null;
        private static Dictionary<string, IntPtr> _namedWindowDictionary = null;
        
        /// <summary>
        /// 获取所有活动窗口
        /// </summary>
        /// <returns></returns>
        public static List<WindowBaseInfo> getAllWindow()
        {
            List<WindowBaseInfo> tmp = new List<WindowBaseInfo>();
            LockMethod(() =>
                {
                    if (_allWindowDictionary != null)
                    {
                        foreach (object value in _windowBaseInfoDictionary.Values)
                        {
                            tmp.Add((WindowBaseInfo)value);
                        }
                    }
                    return null;
                });
            //LogManager.Debug("All window count : {0}", tmp.Count.ToString());
            return tmp;
        }

        /// <summary>
        /// 获取窗口的基础信息
        /// </summary>
        /// <param name="runtimeId"></param>
        /// <returns></returns>
        internal static WindowBaseInfo getWindowBaseInfo(string runtimeId)
        {
            LogManager.Start();
            return (WindowBaseInfo)LockMethod(() =>
                {
                    if (_windowBaseInfoDictionary == null)
                    {
                        throw new CanNotFindDataException("WindowBaseInfo" + runtimeId);
                    }

                    if (_windowBaseInfoDictionary.ContainsKey(runtimeId))
                    {
                        return _windowBaseInfoDictionary[runtimeId];
                    }
                    else
                    {
                        throw new CanNotFindDataException("WindowBaseInfo" + runtimeId);
                    }
                });
        }

        internal static void setWindow(WindowsUnit element)
        {
            if (element == null) return;
            LogManager.Start();
            write++;
            if (_allWindowDictionary == null)
            {
                _allWindowDictionary = new Hashtable();
                _namedWindowDictionary = new Dictionary<string, IntPtr>();
                _windowBaseInfoDictionary = new Dictionary<string, WindowBaseInfo>();
            }
            try
            {
                LockMethod(() =>
                {
                    string runtimeId = string.Join(",", element.GetRuntimeId());
                    //Stopwatch watch = new Stopwatch();
                    //watch.Start();
                    WindowBaseInfo baseInfo = new WindowBaseInfo();
                    baseInfo.WindowName = element.Current.Name;
                    baseInfo.AutomationId = element.Current.AutomationId;
                    baseInfo.ClassName = element.Current.ClassName;
                    baseInfo.Type = element.Current.ControlType.ProgrammaticName;
                    baseInfo.Handle = (IntPtr)element.Current.NativeWindowHandle;
                    baseInfo.OpenTime = new DateTime(1, 1, 1);
                    baseInfo.RuntimeId = runtimeId;
                    if (_allWindowDictionary.ContainsKey(runtimeId))
                    {
                        LogManager.Debug(string.Format("Window named {0} update", element.Current.Name), 3);
                        _allWindowDictionary[runtimeId] = baseInfo.Handle;
                        _windowBaseInfoDictionary[runtimeId] = baseInfo;
                    }
                    else
                    {
                        LogManager.Debug(string.Format("Window named {0} set to list with none event", element.Current.Name), 3);
                        try
                        {
                            _allWindowDictionary.Add(runtimeId, baseInfo.Handle);
                        }
                        catch
                        {
                            LogManager.DebugFormat("当前窗口总数 {0}", _allWindowDictionary.Count.ToString());
                            Robot.Recess(20);
                            Random r = new Random();
                            int n = r.Next(_allWindowDictionary.Count);
                            
                            _allWindowDictionary.Add(runtimeId, baseInfo.Handle);
                        }
                        try
                        {
                            _windowBaseInfoDictionary.Add(runtimeId, baseInfo);
                        }
                        catch
                        {
                            Robot.Recess(20);
                            _windowBaseInfoDictionary.Add(runtimeId, baseInfo);
                        }
                        //2017.09.04 添加窗口时检查是否为警告窗口
                        //2017.09.07 新增窗口时加入，避免重复加入
                        if (Config.WarningWindowList.Contains(baseInfo.AutomationId) || Config.WarningWindowList.Contains(baseInfo.WindowName))
                        {
                            warningTable.Add(runtimeId);
                        }
                    }

                    return null;
                }, true);
                LogManager.End();
                write--;
            }
            catch(Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
                //2017.08.16增加，修改了写异常退出后重置写标志
                write--;
                throw;
            }
        }

        public static string GetWindowFriendlyName(string autoId)
        {
            LogManager.Start();
            LogManager.Debug(autoId);
            if (controlInfo == null) controlInfo = ControlInfo.getInstance();
            XmlDocument doc = controlInfo[Config.SoftwareProcessName];
            if (doc == null) return null;
            if (string.IsNullOrEmpty(autoId))
            {
                return null;
            }
            else
            {
                XmlNodeList xnList = doc.SelectNodes(string.Format("//WinInfo[AutomationId='{0}']", autoId));
                if (xnList == null || xnList.Count == 0)
                {
                    return null;
                }
                VerInfo vi = Robot.GetSoftwareVersion();
                
                foreach(XmlNode xn in xnList)
                {
                    try
                    {
                        string version = xn.Attributes["Version"].Value;
                        if (version.Contains(vi.VerId))
                        {
                            LogManager.Debug(xn.Attributes["FriendlyName"].Value);
                            return xn.Attributes["FriendlyName"].Value;
                        }
                    }
                    catch { }
                }
                return null;
            }
        }

        private static string getString(TimeSpan ts)
        {
            return string.Format("{0}天{1}时{2}分{3}秒{4}毫秒", ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        //private static string _latestWindowId = null;
        internal static void setWindow(WindowsUnit element, DateTime openTime)
        {
            if (element == null) return;
            write++;
            LogManager.Start();
            if (_allWindowDictionary == null)
            {
                _allWindowDictionary = new Hashtable();
                _namedWindowDictionary = new Dictionary<string, IntPtr>();
                _windowBaseInfoDictionary = new Dictionary<string, WindowBaseInfo>();
            }
            try
            {
                string runtimeId = string.Join(",", element.GetRuntimeId());
                LockMethod(() =>
                    {
                        WindowBaseInfo baseInfo = new WindowBaseInfo();
                        baseInfo.WindowName = element.Current.Name;
                        baseInfo.AutomationId = element.Current.AutomationId;
                        baseInfo.ClassName = element.Current.ClassName;
                        baseInfo.Handle = (IntPtr)element.Current.NativeWindowHandle;
                        baseInfo.OpenTime = openTime;
                        baseInfo.Type = element.Current.ControlType.ProgrammaticName;
                        baseInfo.RuntimeId = runtimeId;
                        if (_allWindowDictionary.ContainsKey(runtimeId))
                        {
                            LogManager.Debug(string.Format("Window named {0} update", element.Current.Name), 3);
                            _allWindowDictionary[runtimeId] = baseInfo.Handle;
                            _windowBaseInfoDictionary[runtimeId] = baseInfo;
                        }
                        else
                        {
                            LogManager.Debug(string.Format("Window named {0} set to list with event", element.Current.Name), 3);
                            try
                            {
                                _allWindowDictionary.Add(runtimeId, baseInfo.Handle);
                            }
                            catch
                            {
                                Robot.Recess(20);
                                _allWindowDictionary.Add(runtimeId, baseInfo.Handle);
                            }
                            try
                            {
                                _windowBaseInfoDictionary.Add(runtimeId, baseInfo);
                            }
                            catch
                            {
                                Robot.Recess(20);
                                _windowBaseInfoDictionary.Add(runtimeId, baseInfo);
                            }
                        }

                        //2017.09.04 添加窗口时检查是否为警告窗口
                        if (Config.WarningWindowList.Contains(baseInfo.AutomationId) || Config.WarningWindowList.Contains(baseInfo.WindowName))
                        {
                            warningTable.Add(runtimeId);
                        }
                        return null;
                    }, true);
                LogManager.End();
                write--;

                RunOpenEvent(runtimeId);
            }
            catch (Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
                //2017.08.16增加，修改了写异常退出后重置写标志
                write--;
                throw;
            }
        }

        internal static void rmvWindow(string runtimeId)
        {
            if (string.IsNullOrEmpty(runtimeId)) return;
            write++;
            LogManager.Start();
            try
            {
                LockMethod(() =>
                    {
                        if (_allWindowDictionary == null) return null;
                        try
                        {
                            if (_allWindowDictionary.ContainsKey(runtimeId))
                            {
                                WindowBaseInfo baseInfo = _windowBaseInfoDictionary[runtimeId];
                                if(NativeMethods.IsWindow(baseInfo.Handle))
                                {
                                    if (NativeMethods.IsHungAppWindow(baseInfo.Handle))
                                    {
                                        return null;
                                    }
                                    else
                                    {
                                        LogManager.DebugFormat("System remove window named {0}, friendlyName {1}, but IsWindow is true", baseInfo.WindowName, baseInfo.FriendlyName);
                                    }
                                    return null;
                                }
                                LogManager.DebugFormat("Remove window named {0}, friendlyName {1}", baseInfo.WindowName, baseInfo.FriendlyName);
                                
                                if(_latestWindow != null && string.Equals(_latestWindow.RuntimeId, runtimeId))
                                {
                                    _latestWindow = null;
                                }
                                try
                                {
                                    _allWindowDictionary.Remove(runtimeId);
                                }
                                catch { }
                                try
                                {
                                    _namedWindowDictionary.Remove(baseInfo.FriendlyName);
                                }
                                catch { }
                                try
                                {
                                    _windowBaseInfoDictionary.Remove(runtimeId);
                                }
                                catch { }

                                if (warningTable.Contains(runtimeId))
                                {
                                    warningTable.Remove(runtimeId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.Error(ex);
                        }
                        return null;
                    }, true);
                LogManager.End();
                write--;
            }
            catch
            {
                //2017.08.16增加，修改了写异常退出后重置写标志
                write--;
                throw;
            }
        }

        private static void WindowCollect()
        {
            while (true)
            {
                try
                {
                    LockMethod(() =>
                    {
                        if (_allWindowDictionary == null) return null;
                        foreach (KeyValuePair<string, IntPtr> kv in _allWindowDictionary)
                        {
                            if (!NativeMethods.IsWindow(kv.Value))
                            {
                                string runtimeId = kv.Key;
                                if (string.Equals(_latestWindow.RuntimeId, runtimeId))
                                {
                                    _latestWindow = null;
                                }
                                try
                                {
                                    _allWindowDictionary.Remove(runtimeId);
                                }
                                catch { }
                                try
                                {
                                    _namedWindowDictionary.Remove(_windowBaseInfoDictionary[runtimeId].FriendlyName);
                                }
                                catch { }
                                try
                                {
                                    _windowBaseInfoDictionary.Remove(runtimeId);
                                }
                                catch { }

                                if (warningTable.Contains(runtimeId))
                                {
                                    warningTable.Remove(runtimeId);
                                }
                            }
                        }

                        return null;
                    });
                }
                catch { }
                Robot.Recess(2000);
            }
        }

        /// <summary>
        /// 查看是否存在指定的窗口
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool containWindow(WindowsUnit element)
        {
            if (element == null)
            {
                return false;
            }
            try
            {
                if (_allWindowDictionary == null) return false;
                if (_allWindowDictionary.ContainsValue((IntPtr)element.Current.NativeWindowHandle)) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查看是否存在指定句柄的窗口
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        public static bool containWindow(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                return false;
            }
            try
            {
                if (_allWindowDictionary == null) return false;
                if (_allWindowDictionary.ContainsValue(windowHandle)) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        internal static bool containWindow(string windowRuntimeId)
        {
            if (string.IsNullOrEmpty(windowRuntimeId))
            {
                return false;
            }
            try
            {
                if (_allWindowDictionary == null) return false;
                if (_allWindowDictionary.ContainsKey(windowRuntimeId)) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查看是否存在WindowName设置的窗口
        /// </summary>
        /// <returns></returns>
        public static bool containWindow()
        {
            if (string.IsNullOrEmpty(_windowName))
            {
                return false;
            }
            try
            {
                if (_namedWindowDictionary == null) return false;
                if (_namedWindowDictionary.ContainsKey(_windowName)) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        internal static void Close()
        {
            lock (_controlLock)
            {
                Automation.RemoveAllEventHandlers();
                RobotContext.UpdateTimer = null;
                IsWindowsObjectInitilized = false;
            }
        }

        #region 窗口事件
        private static WindowBaseInfo _latestWindow = null;
        internal static WindowBaseInfo LatestWindow
        {
            get
            {
                return _latestWindow;
            }
        }
        /// <summary>
        /// 窗口打开事件
        /// </summary>
        internal static event WindowOpenEventHandler WindowOpenEvent;

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        internal static event WindowCloseEventHandler WindowCloseEvent;

        internal static void RunOpenEvent(string runtimeId)
        {
            LogManager.Debug("Invoke WindowOpen event");
            if (WindowOpenEvent != null)
            {
                LogManager.Debug("Run WindowOpen event");
                TaistTaskCollection tasks = new TaistTaskCollection();
                
                _latestWindow = getWindowBaseInfo(runtimeId);
                tasks.Add(() =>
                    {
                        WindowOpenEvent(_latestWindow);
                    });
                tasks.Add(() =>
                    {
                        Robot.ExecuteWithTimeOut(() =>
                            {
                                while (_latestWindow != null)
                                {
                                    Robot.Recess(100);
                                }
                            }, 600000);
                    });
                try
                {
                    tasks.Run(TaistTaskType.COMPETE);
                }
                finally
                {
                    _latestWindow = null;
                }
            }
        }

        internal static void RunCloseEvent(string runtimeId)
        {
            if (WindowCloseEvent != null)
            {
                _latestWindow = getWindowBaseInfo(runtimeId);
                TaistTaskCollection tasks = new TaistTaskCollection();

                _latestWindow = getWindowBaseInfo(runtimeId);
                tasks.Add(() =>
                {
                    WindowCloseEvent(_latestWindow);
                });
                tasks.Add(() =>
                {
                    Robot.ExecuteWithTimeOut(() =>
                    {
                        while (_latestWindow != null)
                        {
                            Robot.Recess(100);
                        }
                    }, 600000);
                });
                try
                {
                    tasks.Run(TaistTaskType.COMPETE);
                }
                finally
                {
                    _latestWindow = null;
                }
                //try
                //{
                //    WindowCloseEvent(getWindowBaseInfo(runtimeId));
                //}
                //finally
                //{
                //    _latestWindow = null;
                //}
            }
        }
        #endregion

        #region 控件事件
        internal static event TaistControlEventHandler ControlEvent = null;
        internal static void RunControlEvent(TaistControlEventInfo eventInfo)
        {
            if (ControlEvent != null)
            {
                LogManager.Start();
                eventInfo.WindowName = _windowName;
                eventInfo.ControlName = ElementName;
                if (eventInfo.EventTime == new DateTime(1, 1, 1))
                {
                    eventInfo.EventTime = DateTime.Now;
                }
                ControlEvent(eventInfo);
                LogManager.End();
            }
        }
        #endregion

        internal delegate object ContextMethodHandler();

        private static object LockMethod(ContextMethodHandler handler, bool writeAction = false)
        {
            while (!writeAction && write > 0)
            {
                LogManager.DebugFormat("The control list is being revised, left {0}", write, "c");
                Robot.Recess(100);
            }
            lock (_controlLock)
            {
                return handler();
            }
        }

        #region LastAction

        private static bool lastSuccess = true;
        internal static bool LastActionSuccess
        {
            get
            {
                return lastSuccess;
            }
            set
            {
                lastSuccess = value;
                if (lastSuccess)
                {
                    LastActionHandler = null;
                }
                else
                {
                    lastAction.Set();
                }
            }
        }

        private static AutoResetEvent lastAction = new AutoResetEvent(false);
        private static int actionCount = 0;
        private static bool breakLast = true;
        private static ContextMethodHandler LastActionHandler = null;
        private static object actionLock = new object();
        internal static void LastAction(ContextMethodHandler handler)
        {
            breakLast = true;
            try
            {
                lock (actionLock)
                {
                    handler();
                    LastActionHandler = handler;
                }
            }
            catch(ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            
            lastAction.Set();
        }

        private static void DoLast()
        {
            while (true)
            {
                lastAction.Reset();
                lastAction.WaitOne();

                if (breakLast)
                {
                    actionCount = 0;
                    lastSuccess = false;
                    breakLast = false;
                    continue;
                }

                if (LastActionSuccess)
                {
                    LogManager.Debug("=========>　Last action success");
                    continue;
                }

                if (actionCount > Config.RedoCount)
                {
                    LogManager.Debug("=========>　Last action max");
                    LastActionHandler = null;
                    continue;
                }

                if (LastActionHandler == null)
                {
                    LogManager.Debug("=========>　Last action handler is null");
                    continue;
                }

                try
                {
                    LogManager.DebugFormat("=========>　Redo last action {0}", actionCount.ToString());
                    LastActionHandler();
                }
                catch (Exception ex)
                {
                    LogManager.ErrorOnlyPrint(ex);
                    break;
                }

                actionCount++;
            }
        }

        #endregion

        #region Process Manager
        public static int CurrentProcessMemory
        {
            get
            {
                Process[] ps = Process.GetProcessesByName(Config.SoftwareProcessName);
                if (ps == null || ps.Length == 0)
                {
                    return 0;
                }

                Process p = ps[0];
                PROCESS_MEMORY_COUNTERS counter = new PROCESS_MEMORY_COUNTERS();
                NativeMethods.GetProcessMemoryInfo(p.Handle, out counter, System.Runtime.InteropServices.Marshal.SizeOf(counter));

                return (int)counter.PagefileUsage / 1024;
            }
        }
        #endregion
    }
}
