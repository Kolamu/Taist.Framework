namespace Mock.Tools.Exception
{
    /// <summary>
    /// WebService请求URL格式异常
    /// </summary>
    public class WebServiceUrlFormatErrorException : TaistException
    {
        /// <summary>
        /// 构造WebService请求URL格式异常的新实例
        /// </summary>
        /// <param name="url"></param>
        public WebServiceUrlFormatErrorException(string url) : base(string.Format("WebService url {0} format error.", url)) { }

    }
}
