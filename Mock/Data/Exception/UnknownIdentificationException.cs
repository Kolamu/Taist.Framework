namespace Mock.Data.Exception
{
    /// <summary>
    /// 找不到数据异常
    /// </summary>
    public class UnknownIdentificationException : TaistException
    {
        /// <summary>
        /// 构造找不到数据异常的新实例
        /// </summary>
        public UnknownIdentificationException(int id) : base(string.Format("Unknown identification {0}", id)) { }
    }
}
