namespace Mock.Tools.Exception
{
    /// <summary>
    /// 窗口无响应异常
    /// </summary>
    public class WindowNoResponseException : TaistException
    {
        /// <summary>
        /// 构造窗口无响应异常的新实例
        /// </summary>
        /// <param name="windowName"></param>
        public WindowNoResponseException(string windowName) : base(string.Format("The window named {0} is no response", windowName)) { }
    }
}
