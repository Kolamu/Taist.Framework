namespace Mock.Data.Exception
{
    /// <summary>
    /// 数据不唯一异常
    /// </summary>
    public class NotUniqueDataException : TaistException
    {
        /// <summary>
        /// 构造数据不唯一异常的新实例
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public NotUniqueDataException(string name, string data) : base(string.Format("The data {0}[{1}] is not unique.", name, data)) { }
        
        /// <summary>
        /// 构造数据不唯一异常的新实例
        /// </summary>
        /// <param name="name"></param>
        public NotUniqueDataException(string name) : base(string.Format("The data {0} is not unique.", name)) { }
    }
}
