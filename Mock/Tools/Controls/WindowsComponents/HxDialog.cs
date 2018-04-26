namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示航信定制的对话框对象
    /// </summary>
    public class HxDialog
    {
        /// <summary>
        /// 对话框中的描述信息
        /// </summary>
        /// <returns></returns>
        public static string Message()
        {
            HxDialogObject hxDiaObj = new HxDialogObject();
            return hxDiaObj.Message;
        }

        /// <summary>
        /// 点击对话框中指定名称的按钮
        /// </summary>
        /// <param name="buttonName">按钮名称</param>
        public static void Click(string buttonName)
        {
            if (buttonName == null)
            {
                Click(true);
            }
            LogManager.Debug(string.Format("Click HxDialog {0} button", buttonName));
            HxDialogObject hxDiaObj = new HxDialogObject();
            hxDiaObj.Click(buttonName);
            hxDiaObj = null;
        }

        /// <summary>
        /// 点击对话框中指定名称的按钮
        /// </summary>
        /// <param name="ok">按钮名称</param>
        public static void Click(bool ok)
        {
            LogManager.Debug(string.Format("Click HxDialog {0} button", ok));
            HxDialogObject hxDiaObj = new HxDialogObject();
            hxDiaObj.Click(ok);
            hxDiaObj = null;
        }

        /// <summary>
        /// 检测指定窗体的子窗体中是否存在航信定制的对话框
        /// </summary>
        /// <param name="TimeOut">超时时间</param>
        /// <returns></returns>
        public static bool Exist(int TimeOut = 3000)
        {
            LogManager.DebugTimer("Check HxDialog", 10);
            for (int i = 0; i < TimeOut; i += 1000)
            {
                try
                {
                    HxDialogObject hDlgObj = new HxDialogObject();
                    if (hDlgObj.Find())
                    {
                        return true;
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch (System.Exception) { }
                if (TimeOut > 1000) Robot.Recess(1000);
            }
            return false;
        }
    }
}
