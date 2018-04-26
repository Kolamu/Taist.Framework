namespace Mock.Data.Exception
{
    /// <summary>
    /// 找不到数据异常
    /// </summary>
    public class CanNotFindPropertyException : TaistException
    {
        /// <summary>
        /// 构造找不到数据异常的新实例
        /// </summary>
        public CanNotFindPropertyException(string className, string propertyName) : base(string.Format("Can not find property {0} in class {1}", propertyName, className)) { }
    }
}
