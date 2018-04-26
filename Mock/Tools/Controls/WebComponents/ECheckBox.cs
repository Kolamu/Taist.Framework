namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示IE中的复选框按钮对象
    /// </summary>
    public class ECheckBox
    {
        /// <summary>
        /// 选中复选框
        /// </summary>
        /// <param name="WindowName">窗口名称</param>
        /// <param name="CheckBoxName">复选框名称</param>
        public static void Check(string WindowName, string CheckBoxName)
        {
            IECheckBoxObject cbObj = new IECheckBoxObject(WindowName, CheckBoxName);
            cbObj.Check();
        }

        /// <summary>
        /// 取消选中复选框
        /// </summary>
        /// <param name="WindowName">窗口名称</param>
        /// <param name="CheckBoxName">复选框名称</param>
        public static void UnCheck(string WindowName, string CheckBoxName)
        {
            IECheckBoxObject cbObj = new IECheckBoxObject(WindowName, CheckBoxName);
            cbObj.UnCheck();
        }
    }
}
