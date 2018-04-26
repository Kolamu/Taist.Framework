using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Nature.DataBase
{
    /// <summary>
    /// 表示数据库类型
    /// </summary>
    public enum DataBaseType
    {
        /// <summary>
        /// Oracle数据库类型
        /// </summary>
        Oracle,

        /// <summary>
        /// SQLite数据库类型
        /// </summary>
        SQLite,

        /// <summary>
        /// Access数据库类型
        /// </summary>
        Access,

        /// <summary>
        /// PgSql数据库类型
        /// </summary>
        PgSql,

        /// <summary>
        /// Paradox数据库类型
        /// </summary>
        Paradox,

        /// <summary>
        /// Mysql数据库类型
        /// </summary>
        Mysql
    };
}
