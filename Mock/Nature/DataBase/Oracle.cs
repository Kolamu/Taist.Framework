namespace Mock.Nature.DataBase
{
    using System;
    using Mock.Nature.Exception;
    using System.Xml;
    using System.Text;
    using oracle.jdbc.driver;
    using java.sql;
    using sun.io;
    using System.Collections.Generic;
    public partial class DataBase
    {

        #region Oracle数据库
        private class Oracle : IDb
        {
            private Connection conn = null;
            private Statement stmt = null;
            private java.sql.ResultSet rs = null;

            internal Oracle()
            {
            }

            ~Oracle()
            {
                Close();
            }

            public void Open()
            {
                OracleDriver oDriver = new OracleDriver();
                DriverManager.registerDriver(oDriver);
                conn = DriverManager.getConnection(string.Format("jdbc:oracle:thin:@{0}:{1}:{2}", _host, _port, _dbName), _userName, _passWord);
            }

            public void Close()
            {
                if (rs != null)
                {
                    try
                    {
                        rs.close();
                    }
                    catch { }
                    finally
                    {
                        rs = null;
                    }
                }
                if (stmt != null)
                {
                    try
                    {
                        stmt.close();
                    }
                    catch { }
                    finally
                    {
                        stmt = null;
                    }
                }
                if (conn != null)
                {
                    try
                    {
                        conn.close();
                    }
                    catch { }
                    finally
                    {
                        conn = null;
                    }
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
                    stmt = conn.createStatement();
                    stmt.execute(ClientToDB(sql));
                    stmt.close();
                    stmt = null;
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
                    stmt = conn.createStatement();
                    rs = stmt.executeQuery(ClientToDB(sql));
                    ResultSetMetaData meta = rs.getMetaData();
                    int columnCount = meta.getColumnCount();

                    XmlDocument doc = new XmlDocument();
                    XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GBK", null);
                    doc.AppendChild(dec);
                    XmlElement root = doc.CreateElement("Result");
                    doc.AppendChild(root);
                    while (rs.next())
                    {
                        XmlElement data = doc.CreateElement("Data");
                        root.AppendChild(data);
                        for (int i = 1; i <= columnCount; i++)
                        {
                            string name = meta.getColumnName(i);
                            XmlElement tmp = doc.CreateElement(name);
                            tmp.InnerText = DBToClient(rs.getString(name));
                            data.AppendChild(tmp);
                        }
                    }
                    result.Append(doc.OuterXml);
                    doc.Save("ora.xml");
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message);
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
                    DatabaseMetaData meta = conn.getMetaData();
                    rs = meta.getColumns(null, null, TableName, null);
                    while (rs.next())
                    {
                        columnNames.Add(rs.getString(4));
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

            private string DBToClient(string s)
            {
                if (s == null)
                {
                    return s;
                }
                CharToByteISO8859_1 ctbISO = new CharToByteISO8859_1();
                byte[] res = ctbISO.convertAll(s.ToCharArray());

                ByteToCharGBK btcGBK = new ByteToCharGBK();
                char[] tmp = btcGBK.convertAll(res);
                string s1 = new string(tmp);
                return s1;
            }

            private string ClientToDB(string s)
            {
                if (s == null)
                {
                    return s;
                }
                CharToByteGBK ctbGBK = new CharToByteGBK();
                byte[] res = ctbGBK.convertAll(s.ToCharArray());
                ByteToCharISO8859_1 btcISO = new ByteToCharISO8859_1();
                char[] tmp = btcISO.convertAll(res);
                string s1 = new string(tmp);
                return s1;
            }


            public List<TableKeyInfo> GetPrimaryKeyInfo(string TableName)
            {
                List<TableKeyInfo> primaryKeyList = new List<TableKeyInfo>();
                List<string> primaryKeyNameList = new List<string>();
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                try
                {
                    //stmt = conn.createStatement();
                    //stmt.execute(ClientToDB(sql));
                    //stmt.close();
                    //stmt = null;
                    DatabaseMetaData meta = conn.getMetaData();
                    rs = meta.getPrimaryKeys(null, null, TableName);
                    while (rs.next())
                    {
                        primaryKeyNameList.Add(rs.getString(4));
                    }

                    foreach (string keyName in primaryKeyNameList)
                    {
                        rs = meta.getColumns(null, null, TableName, keyName);
                        if (rs.next())
                        {
                            string keyType = rs.getString(6);
                            TableKeyInfo keyInfo = new TableKeyInfo();
                            keyInfo.Name = keyName;
                            keyInfo.Type = keyType.ToUpper();
                            primaryKeyList.Add(keyInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message);
                }
                return primaryKeyList;
            }

            public List<string> GetPrimaryKeyName(string TableName)
            {
                List<string> primaryKeyNameList = new List<string>();
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                try
                {
                    //stmt = conn.createStatement();
                    //stmt.execute(ClientToDB(sql));
                    //stmt.close();
                    //stmt = null;
                    DatabaseMetaData meta = conn.getMetaData();
                    rs = meta.getPrimaryKeys(null, null, TableName);
                    while (rs.next())
                    {
                        primaryKeyNameList.Add(rs.getString(4));
                    }
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message);
                }
                return primaryKeyNameList;
            }


            public List<TableKeyInfo> GetColumnInfo(string TableName)
            {
                List<TableKeyInfo> keyInfoList = new List<TableKeyInfo>();
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                try
                {
                    DatabaseMetaData meta = conn.getMetaData();
                    rs = meta.getColumns(null, null, TableName, null);
                    while (rs.next())
                    {
                        string keyType = rs.getString(6);
                        TableKeyInfo keyInfo = new TableKeyInfo();
                        keyInfo.Name = rs.getString(4);
                        keyInfo.Type = keyType.ToUpper();
                        keyInfoList.Add(keyInfo);
                    }
                }
                catch (Exception ex)
                {
                    throw new DataBaseRuntimeException(ex.Message);
                }
                return keyInfoList;
            }

            public List<string> GetTableNames()
            {
                List<string> tableNames = new List<string>();
                string sql = "SELECT TABLE_NAME FROM USER_TABLES";
                if (conn == null)
                {
                    throw new DataBaseNotOpenException(this._dbName);
                }
                stmt = conn.createStatement();
                rs = stmt.executeQuery(ClientToDB(sql));
                while (rs.next())
                {
                    tableNames.Add(rs.getString(1));
                }
                return tableNames;
            }
        }
        #endregion

    }
}
