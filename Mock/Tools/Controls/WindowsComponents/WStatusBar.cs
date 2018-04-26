namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示状态栏操作对象
    /// </summary>
    public class WStatusBar
    {
        /// <summary>
        /// 获取状态栏信息操作
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="index">序号</param>
        /// <param name="statusBarName">状态栏名称</param>
        /// <returns></returns>
        public static string getItemValue(string windowName, int index, string statusBarName = "状态栏")
        {
            LogManager.Debug(string.Format("Get item value of statusbar named {2}[{1}] in {0} windows", windowName, index, statusBarName));
            StatusBarObject statusObj = new StatusBarObject(windowName, statusBarName);
            string ret = statusObj.GetValue(index);
            statusObj = null;
            return ret;
        }

        /// <summary>
        /// 查看菜单栏是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="statusBarName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string statusBarName)
        {
            try
            {
                new StatusBarObject(windowName, statusBarName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
