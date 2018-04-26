namespace Mock.Tools.Controls
{
    using System;
    using Mock.Tools.Exception;
    /// <summary>
    /// 表示选项卡操作对象
    /// </summary>
    public class WTab
    {
        /// <summary>
        /// 选择标签页操作
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="tabName">选项卡控件名称</param>
        /// <param name="ItemName">标签页名称</param>
        public static void Select(string windowName, string tabName, string ItemName)
        {
            LogManager.DebugFormat("Select {2} item in {1} TabControl in {0} window", windowName, tabName, ItemName);
            try
            {
                TabObject tabObj = new TabObject(windowName, tabName);
                tabObj.Select(ItemName);
                tabObj = null;
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
        /// 查看选项卡控件是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="tabName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string tabName)
        {
            try
            {
                new TabObject(windowName, tabName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查看选项卡标签页是否存在
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="tabName">选项卡名称</param>
        /// <param name="itemName">标签页名称</param>
        /// <returns>存在返回 True， 否则返回 False</returns>
        public static bool Exist(string windowName, string tabName, string itemName)
        {
            try
            {
                TabObject tabObj = new TabObject(windowName, tabName);
                tabObj.Exist(itemName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetSelectedItemName(string windowName, string tabName)
        {
            LogManager.DebugFormat("Get selected item name in {1} TabControl in {0} window", windowName, tabName);
            
            TabObject tabObj = new TabObject(windowName, tabName);
            return tabObj.GetSelectedItemName();
        }
    }
}
