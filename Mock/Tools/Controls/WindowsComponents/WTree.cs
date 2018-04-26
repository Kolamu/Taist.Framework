namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示树控件对象
    /// </summary>
    public class WTree
    {
        /// <summary>
        /// 选择树节点
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="treeName"></param>
        /// <param name="itemName"></param>
        public static void Select(string windowName, string treeName, string itemName)
        {
            TreeObject to = new TreeObject(windowName, treeName);
            to.Select(itemName);
        }

        /// <summary>
        /// 双击树节点
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="treeName"></param>
        /// <param name="itemName"></param>
        public static void DbClick(string windowName, string treeName, string itemName)
        {
            TreeObject to = new TreeObject(windowName, treeName);
            to.DbClick(itemName);
        }

        /// <summary>
        /// 折叠树节点
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="treeName"></param>
        /// <param name="itemName"></param>
        public static void Collapse(string windowName, string treeName, string itemName)
        {
            TreeObject to = new TreeObject(windowName, treeName);
            to.Collapse(itemName);
        }

        /// <summary>
        /// 展开树节点
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="treeName"></param>
        /// <param name="itemName"></param>
        public static void Expand(string windowName, string treeName, string itemName)
        {
            TreeObject to = new TreeObject(windowName, treeName);
            to.Expand(itemName);
        }
    }
}
