namespace Mock.Tools.Exception
{
    /// <summary>
    /// 非法调用WebService异常
    /// </summary>
    public class InvalidWebServiceRequestException : TaistException
    {
        /// <summary>
        /// 构造非法调用WebService异常的新实例
        /// </summary>
        /// <param name="message"></param>
        public InvalidWebServiceRequestException(string message) : base(message) { }
    }
}
