namespace Mock.Nature.Exception
{
    /// <summary>
    /// 非法的数据库类型异常
    /// </summary>
    public class InvalidDataBaseTypeException : TaistException
    {
        /// <summary>
        /// 构造非法数据库类型异常的新实例
        /// </summary>
        public InvalidDataBaseTypeException(string typeName) : base(string.Format("The data base type named {0} is invalid", typeName)) { }
    }
}
