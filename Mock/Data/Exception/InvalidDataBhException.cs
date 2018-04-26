namespace Mock.Data.Exception
{
    /// <summary>
    /// 数据编号异常
    /// </summary>
    public class InvalidDataBhException:TaistException
    {
        /// <summary>
        /// 构造数据编号异常类的新实例
        /// </summary>
        /// <param name="message"></param>
        public InvalidDataBhException(string message) : base(message) { }
    }
}
