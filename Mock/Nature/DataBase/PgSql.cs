namespace Mock.Nature.DataBase
{
    using System;
    using System.Text;
    using System.Xml;
    using System.Data;

    using Npgsql;
    using System.Collections.Generic;

    using Mock.Nature.Exception;
    public partial class DataBase
    {
        #region Pgsql数据库
        private class PgSql : IDb
        {
            private NpgsqlConnection conn = null;
            private NpgsqlConnectionStringBuilder connString = null;
            internal PgSql()
            {

            }

            public void Open()
            {
                connString = new NpgsqlConnectionStringBuilder();
                connString.Database = _dbName;
                connString.UserName = _userName;
                connString.Password = _passWord;
                connString.Host = _host;
                connString.Port = int.Parse(_port);

                conn = new NpgsqlConnection(connString.ToString());
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
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                    {
                        conn.Close();
                        conn.Open();
                    }
                    NpgsqlCommand command = conn.CreateCommand();
                    command.CommandText = sql;
                    int n = command.ExecuteNonQuery();
                    //if (n != 1)
                    //{
                    //    throw new DataBaseRuntimeException(string.Format("执行[{0}]失败！", sql));
                    //}
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(string.Format("{0} {1}", conn.State, ex.Message));
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
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                    {
                        conn.Close();
                        conn.Open();
                    }
                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(sql, conn);
                    DataSet resultDataSet = new DataSet("Result");
                    dataAdapter.Fill(resultDataSet, "Data");
                    DataTable resultTable = resultDataSet.Tables["Data"];

                    XmlWriter xw = XmlWriter.Create(result);
                    resultTable.WriteXml(xw);

                    resultTable.WriteXml("Pgsql.xml");
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(string.Format("{0} {1}\n{2}", conn.State, ex.Message, ex.StackTrace));
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
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                    {
                        conn.Close();
                        conn.Open();
                    }
                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(string.Format("select * from {0}", TableName), conn);
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

                string sql = string.Format("select col_description(a.attrelid,a.attnum) as comment,pg_type.typname as typename,a.attname as name, a.attnotnull as notnull FROM pg_class as c,pg_attribute as a inner join pg_type on pg_type.oid = a.atttypid where c.relname = '{0}' and a.attrelid = c.oid and a.attnum>0", TableName);
                string result = ExecuteQuery(sql);
                ResultSet rs = new ResultSetClass(result);
                while (rs.Next)
                {
                    TableKeyInfo keyInfo = new TableKeyInfo();
                    keyInfo.Name = rs.GetString("name");
                    keyInfo.Type = rs.GetString("typename");
                    keyInfoList.Add(keyInfo);
                }
                return keyInfoList;
            }

            public List<TableKeyInfo> GetPrimaryKeyInfo(string TableName)
            {
                string sqlFormat = "select pg_constraint.conname as pk_name,pg_attribute.attname as colname,pg_type.typname as typename from pg_constraint inner join pg_class on pg_constraint.conrelid = pg_class.oid inner join pg_attribute on pg_attribute.attrelid = pg_class.oid and pg_attribute.attnum = pg_constraint.conkey[{0}] inner join pg_type on pg_type.oid = pg_attribute.atttypid where pg_class.relname = '{1}' and pg_constraint.contype='p'";

                List<TableKeyInfo> tableKeyInfo = new List<TableKeyInfo>();
                for(int i=1;i<20;i++)
                {
                    string sql = string.Format(sqlFormat, i, TableName);
                    ResultSet rs = new ResultSetClass(ExecuteQuery(sql));
                    if (rs.Next)
                    {
                        if(string.IsNullOrEmpty(rs.GetString("pk_name")))
                        {
                            break;
                        }
                        else
                        {
                            TableKeyInfo keyInfo = new TableKeyInfo();
                            keyInfo.Name = rs.GetString("colname");
                            keyInfo.Type = rs.GetString("typename");
                            tableKeyInfo.Add(keyInfo);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return tableKeyInfo;
            }

            public List<string> GetPrimaryKeyName(string TableName)
            {
                string sqlFormat = "select pg_attribute.attname as colname from pg_constraint inner join pg_class on pg_constraint.conrelid = pg_class.oid inner join pg_attribute on pg_attribute.attrelid = pg_class.oid and pg_attribute.attnum = pg_constraint.conkey[{0}] inner join pg_type on pg_type.oid = pg_attribute.atttypid where pg_class.relname = '{1}' and pg_constraint.contype='p'";
                List<string> name = new List<string>();
                for(int i=1;i<20;i++)
                {
                    string sql = string.Format(sqlFormat, i, TableName);
                    ResultSet rs = new ResultSetClass(ExecuteQuery(sql));
                    if (rs.Next)
                    {
                        if (string.IsNullOrEmpty(rs.GetString("colname")))
                        {
                            break;
                        }
                        else
                        {
                            name.Add(rs.GetString("colname"));
                        }
                    }
                    else
                    {
                        break;
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

                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Open();
                }
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("select relname from pg_stat_user_tables", conn);
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
