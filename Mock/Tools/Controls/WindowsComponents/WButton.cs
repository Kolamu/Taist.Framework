namespace Mock.Tools.Controls
{
    using System;
    /// <summary>
    /// 表示按钮对象操作类
    /// </summary>
    public class WButton
    {
        /// <summary>
        /// 表示点击按钮操作
        /// </summary>
        /// <param name="windowName">按钮所在窗口名称</param>
        /// <param name="buttonName">按钮名称</param>
        public static void Click(string windowName, string buttonName)
        {
            LogManager.Debug(string.Format("Click {0} button in {1} window", buttonName, windowName));
            //try
            //{
                ButtonObject btnObj = new ButtonObject(windowName, buttonName);
                btnObj.Click();
                btnObj = null;
            //}
            //catch (Exception ex)
            //{
            //    Robot.Recess(5000);
            //    LogManager.Error(ex);
            //    ButtonObject btnObj = new ButtonObject(windowName, buttonName);
            //    btnObj.Click();
            //    btnObj = null;
            //    //throw ex;
            //}
        }

        /// <summary>
        /// 表示点击按钮操作
        /// </summary>
        /// <param name="windowName">按钮所在窗口名称</param>
        /// <param name="buttonName">按钮名称</param>
        public static void ClickByMouse(string windowName, string buttonName)
        {
            LogManager.Debug(string.Format("ClickByMouse {0} button in {1} window", buttonName, windowName));
            ButtonObject btnObj = new ButtonObject(windowName, buttonName);
            btnObj.ClickByMouse();
            btnObj = null;
        }

        /// <summary>
        /// 查看按钮是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string buttonName)
        {
            try
            {
                new ButtonObject(windowName, buttonName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
