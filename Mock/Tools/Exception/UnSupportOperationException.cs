namespace Mock.Tools.Exception
{
    /// <summary>
    /// 操作模式不支持异常
    /// </summary>
    public class UnSupportOperationException : TaistException
    {
        public UnSupportOperationException(string message) : base(message) { }
        public UnSupportOperationException() : base("Operation not supported") { }
    }
}
