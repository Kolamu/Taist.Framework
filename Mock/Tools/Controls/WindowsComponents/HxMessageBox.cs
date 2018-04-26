namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示航信定制的消息框对象
    /// </summary>
    public class HxMessageBox
    {
        /// <summary>
        /// 关闭消息框
        /// </summary>
        /// <param name="parentName">父窗体名称</param>
        public static void Close(string parentName = null)
        {
            LogManager.Debug("Close HxMessageBox");
            HxMessageBoxObject hxmsgObj = new HxMessageBoxObject(parentName);
            hxmsgObj.Close();
            hxmsgObj = null;
        }

        /// <summary>
        /// 检测指定窗体的子窗体中是否存在航信定制的消息框
        /// </summary>
        /// <param name="parentName">父窗体名称</param>
        /// <param name="TimeOut">超时时间</param>
        /// <returns></returns>
        public static bool Exists(string parentName = null, int TimeOut = 3000)
        {
            LogManager.Debug("Check HxMessageBox");
            for (int i = 0; i < TimeOut; i += 1000)
            {
                try
                {
                    HxMessageBoxObject hMsgboxObj = new HxMessageBoxObject(parentName);
                    if (hMsgboxObj.Find())
                    {
                        LogManager.Debug("HxMessageBox exist");
                        return true;
                    }
                }
                catch(System.Exception) { }
                if (TimeOut > 1000) Robot.Recess(1000);
            }
            LogManager.Debug("HxMessageBox not exist");
            return false;
        }

        /// <summary>
        /// 消息框中的事件描述信息
        /// </summary>
        /// <param name="parentName">父窗体名称</param>
        /// <returns>事件描述信息</returns>
        public static string EventDescription(string parentName = null)
        {
            LogManager.Debug("Get HxMessageBox EventDescription");
            HxMessageBoxObject hxmsgObj = new HxMessageBoxObject(parentName);
            string ret = hxmsgObj.EventDescription();
            hxmsgObj = null;
            return ret;
        }

        /// <summary>
        /// 点击消息框中的按钮
        /// </summary>
        /// <param name="buttonName">按钮名称</param>
        /// <param name="parentName">父窗体名称</param>
        public static void Click(string buttonName, string parentName = null)
        {
            LogManager.Debug(string.Format("Click {0} button in HxMessageBox", buttonName));
            HxMessageBoxObject hxmsgObj = new HxMessageBoxObject(parentName);
            hxmsgObj.Click(buttonName);
            hxmsgObj = null;
        }
    }
}
