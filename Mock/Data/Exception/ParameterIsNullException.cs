namespace Mock.Data.Exception
{
    /// <summary>
    /// 属性为空异常
    /// </summary>
    public class ParameterIsNullException : TaistException
    {
        /// <summary>
        /// 构造属性为空异常的新实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public ParameterIsNullException(string propertyName) : base(string.Format("The parameter named {0} is null", propertyName)) { }
    }
}
