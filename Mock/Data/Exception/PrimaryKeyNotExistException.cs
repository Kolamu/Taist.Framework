namespace Mock.Data.Exception
{
    /// <summary>
    /// 属性为空异常
    /// </summary>
    public class PrimaryKeyNotExistException : TaistException
    {
        /// <summary>
        /// 构造属性为空异常的新实例
        /// </summary>
        /// <param name="tableName">属性名称</param>
        public PrimaryKeyNotExistException(string tableName) : base(string.Format("Can not get primary key of table named {0}", tableName)) { }
    }
}
