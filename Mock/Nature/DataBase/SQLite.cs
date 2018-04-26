namespace Mock.Nature.DataBase
{
    using System;
    using Mock.Nature.Exception;
    using System.Data.SQLite;
    using System.Data;
    using System.Xml;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    public partial class DataBase
    {
        #region SQLite数据库
        private class SQLite : IDb
        {
            private SQLiteConnection conn = null;
            private SQLiteConnectionStringBuilder stringBuilder = null;
            internal SQLite()
            {
            }
            public void Open()
            {
                stringBuilder = new SQLiteConnectionStringBuilder();
                stringBuilder.DataSource = _dbName;
                stringBuilder.Password = PassWord;

                conn = new SQLiteConnection(stringBuilder.ConnectionString);
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
                    SQLiteCommand command = conn.CreateCommand();
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
                    //SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
                    //DataSet resultDataSet = new DataSet("Result");
                    //dataAdapter.Fill(resultDataSet, "Data");
                    //DataTable resultTable = resultDataSet.Tables["Data"];

                    //XmlWriter xw = XmlWriter.Create(result);
                    //resultTable.WriteXml(xw);

                    //resultTable.WriteXml("SQLite.xml");
                    #region 不好用
                    XmlDocument doc = new XmlDocument();
                    XmlDeclaration xd = doc.CreateXmlDeclaration("1.0", "GBK", "");
                    doc.AppendChild(xd);

                    XmlElement resDoc = doc.CreateElement("Result");
                    doc.AppendChild(resDoc);

                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        XmlElement data = doc.CreateElement("Data");
                        resDoc.AppendChild(data);
                        for (int i = 0; i < reader.GetValues().Count; i++)
                        {
                            string name = reader.GetName(i);
                            Type t = reader.GetFieldType(i);
                            string value = reader[name].ToString();
                            switch (t.Name.ToLower())
                            {
                                case "boolean":
                                case "bool":
                                    {
                                        if (string.Equals(value.ToLower(), "true"))
                                        {
                                            value = "1";
                                        }
                                        else
                                        {
                                            value = "0";
                                        }
                                        break;
                                    }
                            }
                            XmlElement xe = doc.CreateElement(name);
                            xe.InnerText = value;
                            data.AppendChild(xe);
                        }
                    }
                    doc.Save("SQLite.xml");
                    result.Append(doc.OuterXml);
                    #endregion
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message +"\n"+ ex.StackTrace);
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
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(string.Format("select * from {0}", TableName), conn);
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

            #region 属性
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
            #endregion


            public List<TableKeyInfo> GetPrimaryKeyInfo(string TableName)
            {
                List<TableKeyInfo> primaryNameList = new List<TableKeyInfo>();
                List<string> ls = GetTableSql(TableName);

                foreach (string s in ls)
                {
                    if (!s.ToUpper().Contains("PRIMARY KEY")) continue;
                    int index = s.IndexOf('(', s.ToUpper().IndexOf("PRIMARY KEY"));
                    if (index > 0)
                    {
                        string primaryKeyString = s.Substring(index + 1, s.IndexOf(')', index) - index - 1);
                        string[] primaryKeyArray = primaryKeyString.Split(',');
                        foreach (string primaryKey in primaryKeyArray)
                        {
                            TableKeyInfo keyInfo = new TableKeyInfo();
                            keyInfo.Name = primaryKey.Trim().Trim(']').Trim('[').Trim('"').Trim('\'');
                            keyInfo.Type = GetKeyType(ls, keyInfo.Name);
                            primaryNameList.Add(keyInfo);
                        }
                    }
                    else
                    {
                        TableKeyInfo keyInfo = new TableKeyInfo();
                        string[] sArray = s.Split(' ');
                        keyInfo.Name = sArray[0].Trim().Trim('[').Trim(']').Trim('\"').Trim();
                        keyInfo.Type = sArray[1].ToUpper().Trim();
                        primaryNameList.Add(keyInfo);
                    }
                }
                return primaryNameList;
            }

            public List<string> GetPrimaryKeyName(string TableName)
            {
                List<string> primaryNameList = new List<string>();

                List<string> ls = GetTableSql(TableName);
                foreach (string s in ls)
                {
                    if (!s.ToUpper().Contains("PRIMARY KEY")) continue;
                    int index = s.IndexOf('(', s.ToUpper().IndexOf("PRIMARY KEY"));
                    if (index > 0)
                    {
                        string primaryKeyString = s.Substring(index + 1, s.IndexOf(')', index) - index - 1);
                        string[] primaryKeyArray = primaryKeyString.Split(',');
                        foreach (string primaryKey in primaryKeyArray)
                        {
                            primaryNameList.Add(primaryKey.Trim().Trim(']').Trim('[').Trim('"').Trim('\''));
                        }
                    }
                    else
                    {
                        TableKeyInfo keyInfo = new TableKeyInfo();
                        string[] sArray = s.Split(' ');
                        primaryNameList.Add(sArray[0].Trim().Trim('[').Trim(']').Trim('\"').Trim());
                    }
                }
                return primaryNameList;
            }

            private string GetKeyType(List<string>keyList, string keyName)
            {
                foreach (string tmp in keyList)
                {
                    string keyString = tmp.Trim().Trim('\"').Trim('[').Trim();
                    if (keyString.StartsWith(keyName))
                    {
                        return keyString.Split(' ')[1].ToUpper().Trim();
                    }
                }
                throw new Exception("GetKeyType不会出现这个异常");
            }

            public List<TableKeyInfo> GetColumnInfo(string TableName)
            {
                List<TableKeyInfo> keyInfoList = new List<TableKeyInfo>();

                List<string> keyArray = GetTableSql(TableName);
                foreach (string tmp in keyArray)
                {
                    string keyString = tmp.Trim().Trim(',');
                    if (keyString.StartsWith("["))
                    {
                        keyString = keyString.Substring(keyString.IndexOf('['));
                        string[] keyInfoString = keyString.Split(' ');
                        TableKeyInfo keyInfo = new TableKeyInfo();
                        keyInfo.Name = keyInfoString[0].Trim().Trim(']').Trim('[').Trim('"').Trim('\'');
                        keyInfo.Type = keyInfoString[1].ToUpper().Trim();
                        keyInfoList.Add(keyInfo);
                    }
                }
                return keyInfoList;
            }

            private List<string> GetTableSql(string TableName)
            {
                string sql = string.Format("select sql from sqlite_master where tbl_name = '{0}' and type = 'table'", TableName);
                string result = ExecuteQuery(sql);
                ResultSet rs = new ResultSetClass(result);
                List<string> colList = new List<string>();
                if (rs.Next)
                {
                    sql = rs.GetString("sql");
                    int start = sql.IndexOf('(')+1;
                    sql = sql.Substring(start, sql.LastIndexOf(')') - start);

                    StringBuilder sb = new StringBuilder();
                    bool flag = false;
                    foreach (char c in sql)
                    {
                        sb.Append(c);
                        switch (c)
                        {
                            case ',':
                                {
                                    if (!flag)
                                    {
                                        colList.Add(sb.ToString().Trim().Trim(','));
                                        sb.Clear();
                                    }
                                    break;
                                }
                            case '(':
                                {
                                    flag = true;
                                    break;
                                }
                            case ')':
                                {
                                    flag = false;
                                    break;
                                }
                        }
                    }
                    string lst = sb.ToString().Trim().Trim(',');
                    if(!string.IsNullOrEmpty(lst))
                    {
                        colList.Add(lst);
                    }
                    return colList;
                }

                throw new DataBaseRuntimeException("GetTableSql failed");
            }

            public List<string> GetTableNames()
            {
                List<string> tableNames = new List<string>();
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                try
                {
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT name FROM sqlite_master where type = 'table'", conn);
                    DataSet resultDataSet = new DataSet("Result");
                    dataAdapter.Fill(resultDataSet, "Data");
                    DataTable resultTable = resultDataSet.Tables["Data"];

                    for (int i = 0; i < resultTable.Rows.Count; i++)
                    {
                        tableNames.Add(resultTable.Rows[i][0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message + "\n" + ex.StackTrace);
                }
                return tableNames;
            }
        }
        #endregion
    }
}
