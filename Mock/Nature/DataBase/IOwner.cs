namespace Mock.Nature.DataBase
{
    /// <summary>
    /// 表示数据库表所属的数据库的信息
    /// </summary>
    interface IOwner
    {
        DataBase DataBase { get; }
    }
}
