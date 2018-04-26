namespace Mock.Nature.DataBase
{
    using System.Collections.Generic;
    public partial class DataBase
    {
        #region Access数据库
        private class Access : IDb
        {
            internal Access()
            {

            }

            public void Open()
            {

            }

            public void Close()
            {
            }

            public void ExecuteNoneQuery(string sql)
            {
            }

            public string ExecuteQuery(string sql)
            {
                return string.Empty;
            }
            public List<string> GetColumnNames(string TableName)
            {
                return null;
            }
            private string _userName;
            private string _passWord;
            private string _host;
            private string _port;
            private string _dbName;

            public string UserName
            {
                get
                {
                    return _userName;
                }
                set
                {
                    _userName = value;
                }
            }

            public string PassWord
            {
                get
                {
                    return _passWord;
                }
                set
                {
                    _passWord = value;
                }
            }

            public string Host
            {
                get
                {
                    return _host;
                }
                set
                {
                    _host = value;
                }
            }

            public string Port
            {
                get
                {
                    return _port;
                }
                set
                {
                    _port = value;
                }
            }

            public string DataBaseName
            {
                get
                {
                    return _dbName;
                }
                set
                {
                    _dbName = value;
                }
            }

            public List<TableKeyInfo> GetPrimaryKeyInfo(string TableName)
            {
                throw new System.NotImplementedException();
            }

            public List<string> GetPrimaryKeyName(string TableName)
            {
                throw new System.NotImplementedException();
            }


            public List<TableKeyInfo> GetColumnInfo(string TableName)
            {
                throw new System.NotImplementedException();
            }


            public List<string> GetTableNames()
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion
    }
}
