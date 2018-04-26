namespace Mock.Tools.Controls
{
    using System;
    using Mock.Tools.Exception;
    /// <summary>
    /// 表示组合框操作对象
    /// </summary>
    public class WComboBox
    {
        /// <summary>
        /// 选择组合框中的指定项
        /// </summary>
        /// <param name="windowName">组合框所在窗口名称</param>
        /// <param name="comboxName">组合框名称</param>
        /// <param name="itemName">要选择的项名称</param>
        public static void Select(string windowName, string comboxName, string itemName)
        {
            if (string.Equals(itemName, "NOTINPUT", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            LogManager.Debug(string.Format("Select {0} item in {1} combox in {2} window", itemName, comboxName, windowName));
            try
            {
                ComboBoxObject comboxObj = new ComboBoxObject(windowName, comboxName);
                comboxObj.Select(itemName);
                comboxObj = null;
            }
            catch (Exception)
            {
                Robot.Recess(2000);
                ComboBoxObject comboxObj = new ComboBoxObject(windowName, comboxName);
                comboxObj.Select(itemName);
                comboxObj = null;
                //throw ex;
            }
        }

        /// <summary>
        /// 查看组合框是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="comboxName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string comboxName)
        {
            try
            {
                new ComboBoxObject(windowName, comboxName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查看组合框是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="comboxName"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string comboxName, string itemName)
        {
            try
            {
                ComboBoxObject cbo = new ComboBoxObject(windowName, comboxName);
                return cbo.Exist(itemName);
            }
            catch
            {
                return false;
            }
        }

        public static string GetValue(string windowName, string comboxName)
        {
            try
            {
                ComboBoxObject comboxObj = new ComboBoxObject(windowName, comboxName);
                return comboxObj.Content;
            }
            catch (Exception)
            {
                Robot.Recess(2000);
                ComboBoxObject comboxObj = new ComboBoxObject(windowName, comboxName);
                return comboxObj.Content;
                //throw ex;
            }
        }
    }
}
