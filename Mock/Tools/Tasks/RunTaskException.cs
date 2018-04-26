namespace Mock.Tools.Tasks
{
    /// <summary>
    /// 运行任务错误
    /// </summary>
    public class RunTaskException : TaistException
    {
        /// <summary>
        /// 构造运行任务异常的新实例
        /// </summary>
        public RunTaskException(string message) : base(message) { }

        /// <summary>
        /// 构造运行任务异常的新实例
        /// </summary>
        /// <param name="innerException"></param>
        public RunTaskException(System.Exception innerException) : base("Run task occur exception.", innerException) { }
    }
}
