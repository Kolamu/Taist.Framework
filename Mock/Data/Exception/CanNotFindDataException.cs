namespace Mock.Data.Exception
{
    /// <summary>
    /// 找不到数据异常
    /// </summary>
    public class CanNotFindDataException : TaistException
    {
        /// <summary>
        /// 构造找不到数据异常的新实例
        /// </summary>
        public CanNotFindDataException(string name) : base(string.Format("Can not find data {0}", name)) { }
    }
}
