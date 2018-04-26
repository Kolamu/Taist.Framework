namespace Mock.Tools.Controls
{
    using System;
    using Mock.Tools.Exception;
    /// <summary>
    /// 表示工具栏控件
    /// </summary>
    public class WToolBar
    {
        /// <summary>
        /// 工具栏控件的点击事件
        /// </summary>
        /// <param name="WindowName">工具栏所属窗体名称</param>
        /// <param name="ToolBarName">工具栏名称</param>
        /// <param name="ItemName">工具栏选项名称</param>
        public static void Click(string WindowName, string ToolBarName, string ItemName)
        {
            try
            {
                LogManager.Debug(string.Format("Click {2} item in toolbar named {0} in {1} window", ToolBarName, WindowName, ItemName));
                ToolBarObject toolbarObj = new ToolBarObject(WindowName, ToolBarName);
                toolbarObj.Click(ItemName);
                toolbarObj = null;
            }
            catch (Exception)
            {
                Robot.Recess(5000);
                ToolBarObject toolbarObj = new ToolBarObject(WindowName, ToolBarName);
                toolbarObj.Click(ItemName);
                toolbarObj = null;
                //throw ex;
            }
        }
    }
}
