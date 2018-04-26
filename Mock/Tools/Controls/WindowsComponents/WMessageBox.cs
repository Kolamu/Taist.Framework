namespace Mock.Tools.Controls
{
    using System;

    /// <summary>
    /// 表示windows提示框操作对象
    /// </summary>
    public class WMessageBox
    {
        /// <summary>
        /// 点击提示框中的按钮操作
        /// </summary>
        /// <param name="messageboxName"></param>
        /// <param name="buttonName"></param>
        public static void Click(string messageboxName, string buttonName)
        {
            string mName = messageboxName;
            if (string.IsNullOrEmpty(mName))
            {
                mName = "first messagebox";
            }
            LogManager.Debug(string.Format("Click button named {1} in {0} window", mName, buttonName));
            MessageBoxObject msgBoxObject = new MessageBoxObject(messageboxName);
            msgBoxObject.Click(buttonName);
            msgBoxObject = null;
        }

        /// <summary>
        /// 点击提示框中的按钮操作
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="messageboxName"></param>
        /// <param name="buttonName"></param>
        public static void Click(int pId, string messageboxName, string buttonName)
        {
            string mName = messageboxName;
            if (string.IsNullOrEmpty(mName))
            {
                mName = "first messagebox";
            }
            LogManager.Debug(string.Format("Click button named {1} in {0} window from process {2}", mName, buttonName, pId));
            MessageBoxObject msgBoxObject = new MessageBoxObject(pId, messageboxName);
            msgBoxObject.Click(buttonName);
            msgBoxObject = null;
        }

        /// <summary>
        /// 点击提示框中的按钮操作
        /// </summary>
        /// <param name="buttonName"></param>
        public static void Click(string buttonName)
        {
            LogManager.Debug(string.Format("Click button named {0} in WMessageBox", buttonName));
            MessageBoxObject msgBoxObject = new MessageBoxObject(string.Empty);
            msgBoxObject.Click(buttonName);
            msgBoxObject = null;
        }

        /// <summary>
        /// 点击提示框中的按钮操作
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="buttonName"></param>
        public static void Click(int pId, string buttonName)
        {
            LogManager.Debug(string.Format("Click button named {0} in WMessageBox with process {1}", buttonName, pId));
            MessageBoxObject msgBoxObject = new MessageBoxObject(pId, string.Empty);
            msgBoxObject.Click(buttonName);
            msgBoxObject = null;
        }

        /// <summary>
        /// 点击提示框中的按钮操作
        /// </summary>
        /// <param name="ok">肯定意义的按钮</param>
        public static void Click(bool ok)
        {
            LogManager.Debug(string.Format("Click WMessageBox {0} button", ok));
            MessageBoxObject msgBoxObject = new MessageBoxObject(string.Empty);
            msgBoxObject.Click(ok);
            msgBoxObject = null;
        }

        /// <summary>
        /// 点击提示框中的按钮操作
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="ok">肯定意义的按钮</param>
        public static void Click(int pId, bool ok)
        {
            LogManager.Debug(string.Format("Click WMessageBox {0} button from process {1}", ok, pId));
            MessageBoxObject msgBoxObject = new MessageBoxObject(pId, string.Empty);
            msgBoxObject.Click(ok);
            msgBoxObject = null;
        }

        /// <summary>
        /// 获取提示框中提示文本操作
        /// </summary>
        /// <param name="messageboxName"></param>
        /// <returns></returns>
        public static string Message(string messageboxName = "")
        {
            string mName = messageboxName;
            if (string.IsNullOrEmpty(mName))
            {
                mName = "first messagebox";
            }
            LogManager.Debug(string.Format("Get Message in {0} window", mName));
            MessageBoxObject msgBoxObject = new MessageBoxObject(messageboxName);
            string ret = msgBoxObject.Message;
            msgBoxObject = null;
            return ret;
        }

        /// <summary>
        /// 获取提示框中提示文本操作
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="messageboxName"></param>
        /// <returns></returns>
        public static string Message(int pId, string messageboxName = "")
        {
            string mName = messageboxName;
            if (string.IsNullOrEmpty(mName))
            {
                mName = "first messagebox";
            }
            LogManager.Debug(string.Format("Get Message in {0} window from process {1}", mName, pId));
            MessageBoxObject msgBoxObject = new MessageBoxObject(pId, messageboxName);
            string ret = msgBoxObject.Message;
            msgBoxObject = null;
            return ret;
        }

        /// <summary>
        /// 检测提示框是否存在操作
        /// </summary>
        /// <param name="messageboxName"></param>
        /// <param name="millionSecondTimeout"></param>
        /// <returns></returns>
        public static bool Exist(string messageboxName = "", int millionSecondTimeout = 3000)
        {
            string mName = messageboxName;
            if (string.IsNullOrEmpty(mName))
            {
                mName = "first messagebox";
            }
            LogManager.DebugTimer(string.Format("Check {0} window", mName), 10);
            for (int i = 0; i < millionSecondTimeout; i += 1000)
            {
                try
                {
                    MessageBoxObject msgBoxObject = new MessageBoxObject(messageboxName);
                    if (msgBoxObject.Find())
                    {
                        return true;
                    }
                }
                catch(Exception) { }
                if (millionSecondTimeout > 1000) Robot.Recess(1000);
            }
            return false;
        }

        /// <summary>
        /// 检测提示框是否存在操作
        /// </summary>
        /// <param name="messageboxName"></param>
        /// <param name="pId"></param>
        /// <param name="millionSecondTimeout"></param>
        /// <returns></returns>
        public static bool Exist(int pId, string messageboxName = "", int millionSecondTimeout = 3000)
        {
            string mName = messageboxName;
            if (string.IsNullOrEmpty(mName))
            {
                mName = "first messagebox";
            }
            LogManager.DebugTimer(string.Format("Check {0} window from process {1}", mName, pId), 10);
            for (int i = 0; i < millionSecondTimeout; i += 1000)
            {
                try
                {
                    MessageBoxObject msgBoxObject = new MessageBoxObject(pId, messageboxName);
                    if (msgBoxObject.Find())
                    {
                        return true;
                    }
                    return true;
                }
                catch (Exception) { }
                if (millionSecondTimeout > 1000) Robot.Recess(1000);
            }
            return false;
        }

        /// <summary>
        /// 关闭提示框操作
        /// </summary>
        /// <param name="messageboxName"></param>
        public static void Close(string messageboxName = "")
        {
            string mName = messageboxName;
            if (string.IsNullOrEmpty(mName))
            {
                mName = "first messagebox";
            }
            LogManager.Debug(string.Format("Close {0} window", mName));
            MessageBoxObject msgBoxObject = new MessageBoxObject(messageboxName);
            msgBoxObject.Close();
            msgBoxObject = null;
        }

        /// <summary>
        /// 关闭提示框操作
        /// </summary>
        /// <param name="messageboxName"></param>
        /// <param name="pId"></param>
        public static void Close(int pId, string messageboxName = "")
        {
            string mName = messageboxName;
            if (string.IsNullOrEmpty(mName))
            {
                mName = "first messagebox";
            }
            LogManager.Debug(string.Format("Close {0} window from process {1}", mName, pId));
            MessageBoxObject msgBoxObject = new MessageBoxObject(pId, messageboxName);
            msgBoxObject.Close();
            msgBoxObject = null;
        }
    }
}
