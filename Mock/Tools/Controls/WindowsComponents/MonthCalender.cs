namespace Mock.Tools.Controls
{
    /// <summary>
    /// 表示日历控件操作
    /// </summary>
    public class MonthCalender
    {
        /// <summary>
        /// 设置日历控件到下个月
        /// </summary>
        /// <param name="windowName">日历控件所在窗体名称</param>
        public static void NextMonth(string windowName)
        {
            MonthCalenderObject mcObj = new MonthCalenderObject(windowName);
            mcObj.NextMonth();
        }
    }
}
