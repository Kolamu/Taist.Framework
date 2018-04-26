namespace Mock.Tools.Controls
{
    using System;
    using Mock.Tools.Exception;
    public class EWindow
    {
        /// <summary>
        /// 搜索窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public static void Search(string windowName)
        {
            LogManager.Debug(string.Format("Search {0} window.", windowName));
            IEWindowObject windowObj = new IEWindowObject(windowName);
            windowObj.Search();
            windowObj = null;
        }

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="waitMillionSecond">超时时间</param>
        /// <returns></returns>
        public static bool FindWindow(string windowName, int waitMillionSecond = 1000)
        {
            LogManager.Debug(string.Format("Find {0} window.", windowName));
            IEWindowObject windowObj = new IEWindowObject(windowName);
            int count = waitMillionSecond / 500 + 1;
            for (int i = 0; i < count; i++)
            {
                if (windowObj.Find())
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
        /// 置顶窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public static void SetTopMost(string windowName)
        {
            try
            {
                IEWindowObject windowObj = new IEWindowObject(windowName);
                windowObj.SetTopMost();
                windowObj = null;
            }
            catch (TaistException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UnExpectedException(ex);
            }
        }

        /// <summary>
        /// 最大化窗口
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public static void SetMaximize(string windowName)
        {
            try
            {
                IEWindowObject windowObj = new IEWindowObject(windowName);
                windowObj.SetMaximize();
                windowObj = null;
            }
            catch (TaistException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UnExpectedException(ex);
            }
        }
    }
}
