namespace Mock.Tools.Exception
{
    /// <summary>
    /// 无效类型异常
    /// </summary>
    public class InvalidTypeException : TaistException
    {
        /// <summary>
        /// 构造无效类型异常的新实例
        /// </summary>
        /// <param name="typeName"></param>
        public InvalidTypeException(string typeName) : base(string.Format("The type named {0} is invalid", typeName)) { }
    }
}
