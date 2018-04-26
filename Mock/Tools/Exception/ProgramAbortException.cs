namespace Mock.Tools.Exception
{
    /// <summary>
    /// 窗口无响应异常
    /// </summary>
    public class ProgramAbortException : TaistException
    {
        /// <summary>
        /// 构造窗口无响应异常的新实例
        /// </summary>
        public ProgramAbortException() : base("被测程序已崩溃") { }
    }
}
