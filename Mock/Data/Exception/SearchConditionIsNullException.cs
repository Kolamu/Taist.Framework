namespace Mock.Data.Exception
{
    /// <summary>
    /// 表示查询条件为空异常
    /// </summary>
    public class SearchConditionIsNullException : TaistException
    {
        /// <summary>
        /// 构造查询条件为空异常的一个新实例
        /// </summary>
        /// <param name="sName">查询的数据名称</param>
        public SearchConditionIsNullException(string sName) : base(string.Format("The condition to search {0} data is null, please specify at least one condition.", sName)) { }
    }
}
