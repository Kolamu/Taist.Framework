namespace Mock.Data.Exception
{
    /// <summary>
    /// 表示参数输入错误异常
    /// </summary>
    public class InvalidParamValueException : TaistException
    {
        /// <summary>
        /// 构造参数输入错误异常的新实例
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="paramName"></param>
        public InvalidParamValueException(string methodName, string paramName) : base(string.Format("The param named {0} in {1} method is invalid", paramName, methodName)) { }

        /// <summary>
        /// 构造参数输入错误异常的新实例
        /// </summary>
        /// <param name="message"></param>
        public InvalidParamValueException(string message) : base(message) { }
    }
}
