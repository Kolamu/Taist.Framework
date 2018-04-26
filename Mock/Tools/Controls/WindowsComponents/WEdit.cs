namespace Mock.Tools.Controls
{
    using System;
    using Mock.Tools.Exception;
    /// <summary>
    /// 表示输入框操作对象
    /// </summary>
    public class WEdit
    {
        /// <summary>
        /// 向输入框中输入文本
        /// </summary>
        /// <param name="windowName">输入框所在窗口名称</param>
        /// <param name="inputName">输入框名称</param>
        /// <param name="value">输入的值</param>
        public static void Input(string windowName, string inputName, string value)
        {
            if (string.Equals(value, "NOTINPUT", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (value == null)
            {
                value = "";
                LogManager.Debug(string.Format("Input null to {0} edit in {1} window", inputName, windowName));
            }
            else
            {
                LogManager.Debug(string.Format("Input {0} to {1} edit in {2} window", value, inputName, windowName));
            }
            
            try
            {
                EditObject editObj = new EditObject(windowName, inputName);
                editObj.Input(value);
                editObj = null;
            }
            catch (Exception ex)
            {
                LogManager.Debug(ex.Message + ex.StackTrace);
                LogManager.Debug("等待重做...");
                Robot.Recess(5000);
                EditObject editObj = new EditObject(windowName, inputName);
                editObj.Input(value);
                //throw ex;
                editObj = null;
            }
        }

        /// <summary>
        /// 点击输入框操作
        /// </summary>
        /// <param name="windowName">输入框所在的窗口名称</param>
        /// <param name="inputName">输入框名称</param>
        public static void Click(string windowName, string inputName)
        {
            LogManager.Debug(string.Format("Click {0} edit in {1} window", inputName, windowName));
            try
            {
                EditObject editObj = new EditObject(windowName, inputName);
                editObj.Click();
                editObj = null;
            }
            catch (Exception)
            {
                Robot.Recess(5000);
                EditObject editObj = new EditObject(windowName, inputName);
                editObj.Click();
                editObj = null;
                //throw ex;
            }
        }

        /// <summary>
        /// 查看输入框是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string inputName)
        {
            try
            {
                new EditObject(windowName, inputName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取输入框中的文本
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static string GetValue(string windowName, string inputName)
        {
            LogManager.Debug(string.Format("Get value from {0} edit in {1} window", inputName, windowName));
            try
            {
                EditObject editObj = new EditObject(windowName, inputName);
                return editObj.GetValue();
            }
            catch (Exception)
            {
                Robot.Recess(5000);
                EditObject editObj = new EditObject(windowName, inputName);
                return editObj.GetValue();
            }
        }
    }
}
