namespace Mock.Tools.Controls
{
    using Mock.Tools.Exception;
    /// <summary>
    /// 表示复选框操作对象
    /// </summary>
    public class WCheckBox
    {
        /// <summary>
        /// 选择复选框操作
        /// </summary>
        /// <param name="windowName">复选框所在窗口名称</param>
        /// <param name="checkBoxName">复选框名称</param>
        public static void Check(string windowName, string checkBoxName)
        {
            CheckBoxObject checkBoxObj = new CheckBoxObject(windowName, checkBoxName);
            checkBoxObj.Check();
            checkBoxObj = null;
        }

        /// <summary>
        /// 取消选择复选框操作
        /// </summary>
        /// <param name="windowName">复选框所在窗口名称</param>
        /// <param name="checkBoxName">复选框名称</param>
        public static void UnCheck(string windowName, string checkBoxName)
        {
            CheckBoxObject checkBoxObj = new CheckBoxObject(windowName, checkBoxName);
            checkBoxObj.UnCheck();
            checkBoxObj = null;
        }

        /// <summary>
        /// 设置复选框到不确定状态操作
        /// </summary>
        /// <param name="windowName">复选框所在窗口名称</param>
        /// <param name="checkBoxName">复选框名称</param>
        public static void Indeterminate(string windowName, string checkBoxName)
        {
            CheckBoxObject checkBoxObj = new CheckBoxObject(windowName, checkBoxName);
            checkBoxObj.Indeterminate();
            checkBoxObj = null;
        }

        /// <summary>
        /// 指示复选框是否存在
        /// </summary>
        /// <param name="windowName">复选框所在窗口名称</param>
        /// <param name="checkBoxName">复选框名称</param>
        /// <returns></returns>
        public static bool Exist(string windowName, string checkBoxName)
        {
            try
            {
                new CheckBoxObject(windowName, checkBoxName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
