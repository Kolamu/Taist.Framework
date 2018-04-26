using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Nature.DataBase
{
    internal interface IDb
    {
        /// <summary>
        /// Database username
        /// </summary>
        string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Database password
        /// </summary>
        string PassWord
        {
            get;
            set;
        }

        /// <summary>
        /// Database host
        /// </summary>
        string Host
        {
            get;
            set;
        }

        /// <summary>
        /// Database port
        /// </summary>
        string Port
        {
            get;
            set;
        }

        /// <summary>
        /// Database name
        /// </summary>
        string DataBaseName
        {
            get;
            set;
        }

        /// <summary>
        /// Open database
        /// </summary>
        void Open();

        /// <summary>
        /// Close database
        /// </summary>
        void Close();

        /// <summary>
        /// Execute SQL but not return result
        /// </summary>
        /// <param name="sql"></param>
        void ExecuteNoneQuery(string sql);

        /// <summary>
        /// Execute SQL and return result
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string ExecuteQuery(string sql);

        /// <summary>
        /// Get all column's name which in table named <code>TableName</code>
        /// </summary>
        /// <param name="TableName">table name</param>
        /// <returns></returns>
        List<string> GetColumnNames(string TableName);

        /// <summary>
        /// Get all column's infomation which in table named <code>TableName</code>
        /// </summary>
        /// <param name="TableName">table name</param>
        /// <returns></returns>
        List<TableKeyInfo> GetColumnInfo(string TableName);

        /// <summary>
        /// Get all primary key's info which in table named <code>TableName</code>
        /// </summary>
        /// <param name="TableName">table name</param>
        /// <returns></returns>
        List<TableKeyInfo> GetPrimaryKeyInfo(string TableName);

        /// <summary>
        /// Get all primary key's name which in table named <code>TableName</code>
        /// </summary>
        /// <param name="TableName">table name</param>
        /// <returns></returns>
        List<string> GetPrimaryKeyName(string TableName);

        /// <summary>
        /// Get all table name's name in this database
        /// </summary>
        /// <returns></returns>
        List<string> GetTableNames();
    }
}
