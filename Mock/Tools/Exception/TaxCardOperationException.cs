namespace Mock.Tools.Exception
{
    /// <summary>
    /// 表示组件接口操作异常对象
    /// </summary>
    public class TaxCardOperationException : TaistException
    {
        /// <summary>
        /// 构造组件接口操作异常的新实例
        /// </summary>
        /// <param name="message"></param>
        public TaxCardOperationException(string message) : base(message) { }
    }
}
