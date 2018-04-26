namespace Mock.Nature.Exception
{
    /// <summary>
    /// 非法的读操作异常
    /// </summary>
    public class IllegalReadOperationException : TaistException
    {
        /// <summary>
        /// 构造非法读操作异常的新实例
        /// </summary>
        /// <param name="message">异常信息</param>
        public IllegalReadOperationException(string message) : base(string.Format("{0}", message)) { }
    }
}
