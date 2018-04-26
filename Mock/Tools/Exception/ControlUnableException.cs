namespace Mock.Tools.Exception
{
    /// <summary>
    /// 控件无法操作时触发此异常
    /// </summary>
    public class ControlUnableException : TaistException
    {
        /// <summary>
        /// 构造控件无法操作异常的新实例
        /// </summary>
        /// <param name="controlName"></param>
        public ControlUnableException(string controlName) : base(string.Format("Current control named {0} is unable", controlName)) { }
        
        /// <summary>
        /// 构造控件无法操作异常的新实例
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="controlName"></param>
        public ControlUnableException(string windowName, string controlName) : base(string.Format("Current control named {0} in {1} window is unable", controlName, windowName)) { }
    }
}
