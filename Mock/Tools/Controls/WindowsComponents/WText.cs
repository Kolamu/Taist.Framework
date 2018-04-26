namespace Mock.Tools.Controls
{
    using System;
    using Mock.Tools.Exception;
    /// <summary>
    /// 表示标签操作对象
    /// </summary>
    public class WText
    {
        /// <summary>
        /// 获取标签文本操作
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="textName"></param>
        /// <returns></returns>
        public static string GetValue(string windowName, string textName)
        {
            try
            {
                TextObject txtObj = new TextObject(windowName, textName);
                string ret = txtObj.GetValue();
                txtObj = null;
                return ret;
            }
            catch (Exception)
            {
                Robot.Recess(5000);
                TextObject txtObj = new TextObject(windowName, textName);
                string ret = txtObj.GetValue();
                txtObj = null;
                return ret;
            }
        }

        /// <summary>
        /// 判断标签控件是否存在
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="textName"></param>
        /// <returns></returns>
        public static bool Exist(string windowName, string textName)
        {
            try
            {
                new TextObject(windowName, textName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
