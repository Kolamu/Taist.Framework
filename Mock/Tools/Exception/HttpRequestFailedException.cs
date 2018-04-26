namespace Mock.Tools.Exception
{
    /// <summary>
    /// Http 请求发送失败异常
    /// </summary>
    public class HttpRequestFailedException : TaistException
    {
        /// <summary>
        /// 构造Http 请求发送失败异常的新实例
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        public HttpRequestFailedException(string code, string description) : base(string.Format("Http request failed, response code [{0}], description {1}", code, description)) { }
    }
}
