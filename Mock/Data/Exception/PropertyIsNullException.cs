namespace Mock.Data.Exception
{
    /// <summary>
    /// 属性为空异常
    /// </summary>
    public class PropertyIsNullException : TaistException
    {
        /// <summary>
        /// 构造属性为空异常的新实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public PropertyIsNullException(string propertyName) : base(string.Format("The {0} property is null", propertyName)) { }
    }
}
