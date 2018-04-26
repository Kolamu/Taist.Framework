namespace Mock.Nature.DataBase
{
    using System;
    using System.Text;
    using System.Xml;
    using System.Data;
    using System.Collections.Generic;

    using Mock.Nature.Exception;
    using MySql.Data.MySqlClient;
    public partial class DataBase
    {
        #region Mysql数据库
        private class Mysql : IDb
        {
            private MySqlConnection conn = null;
            private MySqlConnectionStringBuilder connString = null;
            internal Mysql()
            {

            }

            public void Open()
            {
                connString = new MySqlConnectionStringBuilder();
                connString.Server = _host;
                connString.Port = uint.Parse(_port);
                connString.UserID = _userName;
                connString.Password = _passWord;
                connString.Database = _dbName;
                //string constr = string.Format("server={0}; Port={1};User Id={2};password={3};Database={4}", _host, int.Parse(_port), _userName, _passWord, _dbName);
                conn = new MySqlConnection(connString.ToString());
                conn.Open();
            }

            public void Close()
            {
                if (conn != null)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch { }
                    conn = null;
                }
            }

            public void ExecuteNoneQuery(string sql)
            {
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                try
                {
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = sql;
                    int n = command.ExecuteNonQuery();
                    //if (n != 1)
                    //{
                    //    throw new DataBaseRuntimeException(string.Format("执行[{0}]失败！", sql));
                    //}
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message);
                }
            }

            public string ExecuteQuery(string sql)
            {
                StringBuilder result = new StringBuilder();
                result.Clear();
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                try
                {
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = sql;

                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
                    dataAdapter.SelectCommand = command;
                    DataSet resultDataSet = new DataSet("Result");
                    dataAdapter.Fill(resultDataSet, "Data");

                    DataTable resultTable = resultDataSet.Tables["Data"];

                    XmlWriter xw = XmlWriter.Create(result);
                    resultTable.WriteXml(xw);

                    resultTable.WriteXml("Mysql.xml");
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message + "\n" + ex.StackTrace);
                }
                return result.ToString();
            }

            public List<string> GetColumnNames(string TableName)
            {
                List<string> columnNames = new List<string>();
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                try
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(string.Format("select * from {0} limit 0, 1", TableName), conn);
                    DataSet resultDataSet = new DataSet("Result");
                    dataAdapter.Fill(resultDataSet, "Data");
                    DataTable resultTable = resultDataSet.Tables["Data"];

                    for (int i = 0; i < resultTable.Columns.Count; i++)
                    {
                        columnNames.Add(resultTable.Columns[i].ColumnName);
                    }
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message);
                }
                return columnNames;
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

            public List<TableKeyInfo> GetColumnInfo(string TableName)
            {
                List<TableKeyInfo> keyInfoList = new List<TableKeyInfo>();

                string sql = string.Format("SELECT Column_Name as name,data_type as typename FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME ='{0}'", TableName);
                string result = ExecuteQuery(sql);
                ResultSet rs = new ResultSetClass(result);
                if (rs.Count == 0)
                {
                    sql = string.Format("SELECT distinct Column_Name as name,data_type as typename FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME like '{0}%'", TableName);
                    result = ExecuteQuery(sql);
                    rs = new ResultSetClass(result);
                    if (rs.Count == 0)
                    {
                        throw new DataBaseRuntimeException(string.Format("Can not find column infomation from {0} table", TableName));
                    }
                    List<string> columnName = GetColumnNames(TableName);
                    while (rs.Next)
                    {
                        TableKeyInfo keyInfo = new TableKeyInfo();
                        keyInfo.Name = rs.GetString("name");
                        keyInfo.Type = rs.GetString("typename");
                        if (columnName.Contains(keyInfo.Name))
                        {
                            keyInfoList.Add(keyInfo);
                        }
                    }
                }
                else
                {
                    while (rs.Next)
                    {
                        TableKeyInfo keyInfo = new TableKeyInfo();
                        keyInfo.Name = rs.GetString("name");
                        keyInfo.Type = rs.GetString("typename");
                        keyInfoList.Add(keyInfo);
                    }
                }
                return keyInfoList;
            }

            public List<TableKeyInfo> GetPrimaryKeyInfo(string TableName)
            {
                string sql = string.Format("SELECT a.COLUMN_NAME as colname,b.DATA_TYPE FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE as a LEFT JOIN INFORMATION_SCHEMA.COLUMNS as b on a.COLUMN_NAME=b.COLUMN_NAME and a.TABLE_NAME=b.TABLE_NAME WHERE a.TABLE_NAME='{0}'", TableName);

                List<TableKeyInfo> tableKeyInfo = new List<TableKeyInfo>();
                ResultSet rs = new ResultSetClass(ExecuteQuery(sql));
                if (rs.Count == 0)
                {
                    sql = string.Format("SELECT a.COLUMN_NAME as colname,b.DATA_TYPE FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE as a LEFT JOIN INFORMATION_SCHEMA.COLUMNS as b on a.COLUMN_NAME=b.COLUMN_NAME and a.TABLE_NAME=b.TABLE_NAME WHERE a.TABLE_NAME like '{0}%'", TableName);
                    rs = new ResultSetClass(ExecuteQuery(sql));
                    if (rs.Count == 0)
                    {
                        throw new DataBaseRuntimeException(string.Format("Can not find primary key infomation from {0} table", TableName));
                    }

                    List<string> columnName = GetColumnNames(TableName);
                    while (rs.Next)
                    {
                        TableKeyInfo keyInfo = new TableKeyInfo();
                        keyInfo.Name = rs.GetString("colname");
                        keyInfo.Type = rs.GetString("typename");
                        
                        if (columnName.Contains(keyInfo.Name))
                        {
                            tableKeyInfo.Add(keyInfo);
                        }
                    }
                }
                else
                {
                    while (rs.Next)
                    {
                        if (!string.IsNullOrEmpty(rs.GetString("colname")))
                        {
                            TableKeyInfo keyInfo = new TableKeyInfo();
                            keyInfo.Name = rs.GetString("colname");
                            keyInfo.Type = rs.GetString("typename");
                            tableKeyInfo.Add(keyInfo);
                        }
                    }
                }
                return tableKeyInfo;
            }

            public List<string> GetPrimaryKeyName(string TableName)
            {
                string sql = string.Format("SELECT COLUMN_NAME as colname FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME='{0}'", TableName);
                List<string> name = new List<string>();

                ResultSet rs = new ResultSetClass(ExecuteQuery(sql));
                if (rs.Count == 0)
                {
                    sql = string.Format("SELECT distinct COLUMN_NAME as colname FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME like '{0}%'", TableName);
                    rs = new ResultSetClass(ExecuteQuery(sql));
                    if (rs.Count == 0)
                    {
                        throw new DataBaseRuntimeException(string.Format("Can not find primary key infomation from {0} table", TableName));
                    }

                    List<string> columnName = GetColumnNames(TableName);
                    while (rs.Next)
                    {
                        string tmp = rs.GetString("colname");
                        if (columnName.Contains(tmp))
                        {
                            name.Add(tmp);
                        }
                    }
                }
                else
                {
                    while (rs.Next)
                    {
                        if (!string.IsNullOrEmpty(rs.GetString("colname")))
                        {
                            name.Add(rs.GetString("colname"));
                        }
                    }
                }
                return name;
            }

            public List<string> GetTableNames()
            {
                List<string> tableNames = new List<string>();
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(string.Format("select table_name from information_schema.tables where table_schema='{0}'", DataBaseName), conn);
                DataSet resultDataSet = new DataSet("Result");
                dataAdapter.Fill(resultDataSet, "Data");
                DataTable resultTable = resultDataSet.Tables["Data"];

                for (int i = 0; i < resultTable.Rows.Count; i++)
                {
                    tableNames.Add(resultTable.Rows[i][0].ToString());
                }
                return tableNames;
            }
        }
        #endregion


    }
}
