namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示单选按钮操作兑现
    /// </summary>
    public class WRadioButton
    {
        /// <summary>
        /// 选择单选按钮操作
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        /// <param name="radioButtonName">单选按钮名称</param>
        public static void Select(string windowName, string radioButtonName)
        {
            RadioButtonObject radioBtnObj = new RadioButtonObject(windowName, radioButtonName);
            radioBtnObj.Select();
            radioBtnObj = null;
        }

        /// <summary>
        /// 查看单选按钮是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="radioButtonName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string radioButtonName)
        {
            try
            {
                new RadioButtonObject(windowName, radioButtonName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
