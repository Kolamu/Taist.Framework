namespace Mock.Tools.Controls
{
    using System;
    using Mock.Tools.Exception;
    /// <summary>
    /// 表示菜单栏操作对象
    /// </summary>
    public class WMenuBar
    {
        /// <summary>
        /// 点击菜单栏操作
        /// </summary>
        /// <param name="windowName">菜单栏所在窗口名称</param>
        /// <param name="menuBarName">菜单栏名称</param>
        /// <param name="ItemName">要点击的菜单项名称</param>
        public static void Click(string windowName, string menuBarName, string ItemName)
        {
            LogManager.Debug(string.Format("Click {0} menu item in {1} window", ItemName, windowName));
            try
            {
                MenuBarObject menuBarObj = new MenuBarObject(windowName, menuBarName);
                menuBarObj.Click(ItemName);
                menuBarObj = null;
            }
            catch (Exception)
            {
                Robot.Recess(5000);
                MenuBarObject menuBarObj = new MenuBarObject(windowName, menuBarName);
                menuBarObj.Click(ItemName);
                menuBarObj = null;
            }
        }

        /// <summary>
        /// 点击菜单栏操作
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="menuBarName"></param>
        /// <param name="ItemNameArray"></param>
        public static void Click(string windowName, string menuBarName, params string[] ItemNameArray)
        {
            LogManager.Debug(string.Format("Click menu item in {0} window", windowName));
            try
            {
                MenuBarObject menuBarObj = new MenuBarObject(windowName, menuBarName);
                menuBarObj.Click(ItemNameArray);
                menuBarObj = null;
            }
            catch (Exception)
            {
                Robot.Recess(5000);
                MenuBarObject menuBarObj = new MenuBarObject(windowName, menuBarName);
                menuBarObj.Click(ItemNameArray);
                menuBarObj = null;
            }
        }

        /// <summary>
        /// 查看菜单栏是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="menuBarName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string menuBarName)
        {
            try
            {
                new MenuBarObject(windowName, menuBarName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
