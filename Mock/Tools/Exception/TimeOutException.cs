namespace Mock.Tools.Exception
{
    /// <summary>
    /// 操作超时错误
    /// </summary>
    public class TimeOutException : TaistException
    {
        /// <summary>
        /// 构造操作超时异常的新实例
        /// </summary>
        /// <param name="opName"></param>
        public TimeOutException(string opName) : base(string.Format("{0} operation time out.", opName)) { }
    }
}
