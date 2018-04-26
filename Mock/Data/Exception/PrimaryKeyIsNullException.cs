namespace Mock.Data.Exception
{
    /// <summary>
    /// 属性为空异常
    /// </summary>
    public class PrimaryKeyIsNullException : TaistException
    {
        /// <summary>
        /// 构造属性为空异常的新实例
        /// </summary>
        /// <param name="primaryKeyName">属性名称</param>
        public PrimaryKeyIsNullException(string primaryKeyName) : base(string.Format("The primary key {0} is null", primaryKeyName)) { }
    }
}
