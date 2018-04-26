namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示弹出菜单操作对象
    /// </summary>
    public sealed partial class WPopMenu
    {
        /// <summary>
        /// 点击弹出菜单操作
        /// </summary>
        /// <param name="windowName">弹出菜单所在窗口名称</param>
        /// <param name="menuName">菜单项名称</param>
        public static void Click(string windowName, string menuName)
        {
            PopMenuObject popMenuObj = new PopMenuObject(windowName);
            popMenuObj.Click(menuName);
            popMenuObj = null;
        }

        /// <summary>
        /// 查看弹出菜单是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName)
        {
            try
            {
                new PopMenuObject(windowName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 鼠标左键双击菜单子项
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="itemName"></param>
        public static void DbClick(string windowName, string itemName)
        {
            PopMenuObject popMenuObj = new PopMenuObject(windowName);
            popMenuObj.DbClick(itemName);
            popMenuObj = null;
        }
    }
}
