namespace Mock.Tools.Controls
{
    using System;
    using Mock.Tools.Exception;

    /// <summary>
    /// 表示Windows系统中的窗口对象
    /// </summary>
    public class WWindow
    {
        /// <summary>
        /// 搜索窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public static void SearchWindow(string windowName)
        {
            LogManager.DebugFormat("Start search window named {0}", windowName);
            try
            {
                WindowObject windowObj = new WindowObject(windowName);
                windowObj.Search();
                windowObj = null;
            }
            catch (TaistException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 置顶窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public static void SetTopMost(string windowName)
        {
            LogManager.DebugFormat("Set window named {0} to foreground", windowName);
            try
            {
                WindowObject windowObj = new WindowObject(windowName);
                windowObj.SetTopMost();
                windowObj = null;
            }
            catch (TaistException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="waitMillionSecond">超时时间</param>
        /// <returns></returns>
        public static bool FindWindow(string windowName, int waitMillionSecond = 1000)
        {
            LogManager.DebugFormat("Find window named {0} timeout {1}", windowName, waitMillionSecond);
            bool exist = false;

            //先找一次
            WindowObject windowObj = new WindowObject(windowName);
            if (windowObj.Find())
            {
                windowObj = null;
                return true;
            }

            //再等超时
            try
            {
                Robot.ExecuteWithTimeOut(() =>
                    {
                        while (true)
                        {
                            if (windowObj.Find())
                            {
                                exist = true;
                                break;
                            }
                            Robot.Recess(500);
                        }
                    }, waitMillionSecond, false);
            }
            catch (TimeOutException)
            {
                exist = false;
            }
            finally
            {
                windowObj = null;
            }
            return exist;
        }

        /// <summary>
        /// 检查窗口关闭
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="waitMillionSecond"></param>
        /// <returns></returns>
        public static bool Disappear(string windowName, int waitMillionSecond = 1000)
        {
            LogManager.DebugFormat("Check window named {0} disappear timeout {1}", windowName, waitMillionSecond);
            WindowObject windowObj = new WindowObject(windowName);
            int count = waitMillionSecond / 500 + 1;
            for (int i = 0; i < count; i++)
            {
                if (!windowObj.Find())
                {
                    windowObj = null;
                    return true;
                }
                Robot.Recess(500);
            }
            windowObj = null;
            return false;
        }

        /// <summary>
        /// 关闭窗口到指定窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public static void CloseTo(string windowName)
        {
            WindowObject windowObj = new WindowObject(windowName);
            windowObj.CloseTo();
            windowObj = null;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public static void Close(string windowName)
        {
            LogManager.Debug(string.Format("Close {0} window", windowName));
            WindowObject windowObj = new WindowObject(windowName);
            windowObj.Close();
            windowObj = null;
        }

        /// <summary>
        /// 开启窗口监听
        /// </summary>
        internal static void Listen()
        {
            WindowObject windowObj = new WindowObject("taist");
            windowObj.Listen();
        }

        /// <summary>
        /// 点击窗口上的子控件
        /// 不建议使用该方法
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="elementName"></param>
        public static void Click(string windowName, string elementName)
        {
            LogManager.Debug(string.Format("Click {0} element in {1} window", elementName, windowName));
            WindowObject windowObj = new WindowObject(windowName);
            windowObj.Click(elementName);
            windowObj = null;
        }

        /// <summary>
        /// 添加窗口打开事件的监听
        /// </summary>
        /// <param name="handler"></param>
        public static void AddWindowOpenEvent(WindowOpenEventHandler handler)
        {
            LogManager.Debug("Add window open event");
            RobotContext.WindowOpenEvent += handler;
        }

        /// <summary>
        /// 添加窗口关闭事件的监听
        /// </summary>
        /// <param name="handler"></param>
        public static void AddWindowCloseEvent(WindowCloseEventHandler handler)
        {
            RobotContext.WindowCloseEvent += handler;
        }

        /// <summary>
        /// 移除窗口打开事件的监听
        /// </summary>
        /// <param name="handler"></param>
        public static void RemoveWindowOpenEvent(WindowOpenEventHandler handler)
        {
            LogManager.Debug("Remove window open event");
            RobotContext.WindowOpenEvent -= handler;
        }

        /// <summary>
        /// 移除窗口关闭事件的监听
        /// </summary>
        /// <param name="handler"></param>
        public static void RemoveWindowCloseEvent(WindowCloseEventHandler handler)
        {
            RobotContext.WindowCloseEvent -= handler;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopFindWindow()
        {
            RobotContext.Close();
        }
    }
}
