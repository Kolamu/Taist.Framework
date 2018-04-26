namespace Mock.Tools.Tasks
{
    /// <summary>
    /// 操作超时错误
    /// </summary>
    public class TaskIsRunningException : TaistException
    {
        /// <summary>
        /// 构造操作超时异常的新实例
        /// </summary>
        public TaskIsRunningException() { }
    }
}
