namespace Mock.Nature.DataBase
{
    using System;
    using System.Text;
    using System.Data;
    using System.Xml;
    using System.Data.Odbc;
    using System.Collections.Generic;

    using Mock.Nature.Exception;
    public partial class DataBase
    {
        private class Paradox : IDb
        {
            private OdbcConnection conn = null;
            private OdbcConnectionStringBuilder stringBuilder = null;
            internal Paradox()
            {
                
            }
            //string ConnectionString = @" Driver={Microsoft Paradox Driver (*.db )};DriverID=538;Fil=Paradox 5.X;DefaultDir=C:Paradox;Dbq=C:Paradox;CollatingSequence=ASCII;PWD=; ";
            public void Open()
            {
                stringBuilder = new OdbcConnectionStringBuilder();
                stringBuilder.Driver = "Microsoft Paradox Driver (*.db )";
                stringBuilder.Add("DriverID", 538);
                stringBuilder.Add("Fil", "Paradox 5.X");
                stringBuilder.Add("DefaultDir", _dbName);
                stringBuilder.Add("Dbq", _dbName);
                stringBuilder.Add("CollatingSequence", "ASCII");
                stringBuilder.Add("PWD", _passWord);

                conn = new OdbcConnection(stringBuilder.ConnectionString);
                conn.Open();
            }

            public void Close()
            {
                if (conn != null)
                {
                    conn.Close();
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
                    OdbcCommand command = conn.CreateCommand();
                    command.CommandText = sql;
                    int n = command.ExecuteNonQuery();
                    if (n != 1)
                    {
                        throw new DataBaseRuntimeException(string.Format("执行[{0}]失败！", sql));
                    }
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
                    OdbcDataAdapter dataAdapter = new OdbcDataAdapter(sql, conn);
                    DataSet resultDataSet = new DataSet("Result");
                    dataAdapter.Fill(resultDataSet, "Data");
                    DataTable resultTable = resultDataSet.Tables["Data"];

                    XmlWriter xw = XmlWriter.Create(result);
                    resultTable.WriteXml(xw);
                    //resultTable.WriteXml("SQLite.xml");
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
                    OdbcDataAdapter dataAdapter = new OdbcDataAdapter(string.Format("select * from {0}", TableName), conn);
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


            public List<TableKeyInfo> GetPrimaryKeyNames(string TableName)
            {
                throw new NotImplementedException();
            }


            public List<TableKeyInfo> GetPrimaryKeyInfo(string TableName)
            {
                throw new NotImplementedException();
            }

            public List<string> GetPrimaryKeyName(string TableName)
            {
                throw new NotImplementedException();
            }


            public List<TableKeyInfo> GetColumnInfo(string TableName)
            {
                throw new NotImplementedException();
            }


            public List<string> GetTableNames()
            {
                throw new NotImplementedException();
            }
        }
    }
}
