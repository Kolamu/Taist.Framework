namespace Mock.Data.Exception
{
    /// <summary>
    /// 找不到关键字异常
    /// </summary>
    public class CanNotFindKeywordException : TaistException
    {
        /// <summary>
        /// 构造找不到关键字异常的新实例
        /// </summary>
        public CanNotFindKeywordException(string name) : base(string.Format("Can not find keyword {0}", name)) { }

        /// <summary>
        /// 构造找不到关键字异常的新实例
        /// </summary>
        public CanNotFindKeywordException(string name, string mode) : base(string.Format("Can not find keyword {0}[{1}]", name, mode)) { }
    }
}
