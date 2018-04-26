namespace Mock.Tools.Exception
{
    /// <summary>
    /// 非活动窗口异常
    /// </summary>
    public class WindowIsInactiveException : TaistException
    {
        /// <summary>
        /// 非活动窗口异常
        /// </summary>
        /// <param name="windowName">窗口名称</param>
        public WindowIsInactiveException(string windowName) : base(string.Format("{0} window is inactive.", windowName)) { }
    }
}
