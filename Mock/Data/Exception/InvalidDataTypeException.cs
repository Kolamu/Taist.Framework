namespace Mock.Data.Exception
{
    /// <summary>
    /// 表示无效数据类型异常
    /// </summary>
    public class InvalidDataTypeException : TaistException
    {
        /// <summary>
        /// 构造无效数据类型异常的新实例
        /// </summary>
        /// <param name="dataType">数据类型</param>
        public InvalidDataTypeException(string dataType) : base(string.Format("Invalid data type {0}", dataType)) { }
    }
}
