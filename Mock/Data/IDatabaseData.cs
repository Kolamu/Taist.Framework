namespace Mock.Data
{
    using System.Collections.Generic;
    /// <summary>
    /// 表示所有数据库数据对象接口
    /// </summary>
    public interface IDatabaseData
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        void Get();

        /// <summary>
        /// 保存数据
        /// </summary>
        void Save();

        /// <summary>
        /// 删除数据
        /// </summary>
        void Delete();
    }
}
