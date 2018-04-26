namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示弹出菜单操作对象
    /// </summary>
    public sealed partial class WPopMenu
    {
        public sealed class TreeUnit
        {
            /// <summary>
            /// 选择树节点
            /// </summary>
            /// <param name="windowName"></param>
            /// <param name="itemName"></param>
            public static void Select(string windowName, string itemName)
            {
                PopMenuObject popMenuObj = new PopMenuObject(windowName);
                TreeObject to = popMenuObj.GetItemObject<TreeObject>("Tree");
                to.Select(itemName);
            }

            /// <summary>
            /// 双击树节点
            /// </summary>
            /// <param name="windowName"></param>
            /// <param name="itemName"></param>
            public static void DbClick(string windowName, string itemName)
            {
                PopMenuObject popMenuObj = new PopMenuObject(windowName);
                TreeObject to = popMenuObj.GetItemObject<TreeObject>("Tree");
                to.DbClick(itemName);
            }

            /// <summary>
            /// 折叠树节点
            /// </summary>
            /// <param name="windowName"></param>
            /// <param name="itemName"></param>
            public static void Collapse(string windowName, string itemName)
            {
                PopMenuObject popMenuObj = new PopMenuObject(windowName);
                TreeObject to = popMenuObj.GetItemObject<TreeObject>("Tree");
                to.Collapse(itemName);
            }

            /// <summary>
            /// 展开树节点
            /// </summary>
            /// <param name="windowName"></param>
            /// <param name="itemName"></param>
            public static void Expand(string windowName, string itemName)
            {
                PopMenuObject popMenuObj = new PopMenuObject(windowName);
                TreeObject to = popMenuObj.GetItemObject<TreeObject>("Tree");
                to.Expand(itemName);
            }
        }
    }
}
