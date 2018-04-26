namespace Mock.Tools.Exception
{
    /// <summary>
    /// 操作超时错误
    /// </summary>
    public class HttpPreviewPostCanceledException : TaistException
    {
        /// <summary>
        /// 构造操作超时异常的新实例
        /// </summary>
        public HttpPreviewPostCanceledException() : base("The user cancels the request before posting it.") { }
    }
}
