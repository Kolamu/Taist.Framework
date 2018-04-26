namespace Mock.Nature.DataBase
{
    using System;
    using System.Reflection;
    using Mock.Nature.Exception;
    using System.Collections.Generic;
    using Mock.Data.Exception;
    using System.Diagnostics;
    /// <summary>
    /// 表示数据库对象
    /// </summary>
    public partial class DataBase
    {
        #region Database 类体
        private IDb db = null;
        private bool isClose = true;
        private string host;
        private string port;
        private string dbName;
        private string userName;
        private string passWord;
        //private static bool initialized = false;
        //private static Dictionary<string, Assembly> assDictionary = null;

        /// <summary>
        /// Create a database object
        /// </summary>
        /// <param name="DBName"></param>
        /// <param name="UserName"></param>
        /// <param name="PassWord"></param>
        /// <param name="Host"></param>
        /// <param name="Port"></param>
        public DataBase(string DBName, string UserName = "", string PassWord = "", string Host = "", string Port = "")
        {
            this.host = Host;
            this.port = Port;
            this.dbName = DBName;
            this.userName = UserName;
            this.passWord = PassWord;
            //if (!initialized)
            //{
            //    initialized = true;
            //    assDictionary = new Dictionary<string, Assembly>();
            //    var ass = this.GetType().Assembly;
            //    var res = ass.GetManifestResourceNames();
            //    foreach (var r in res)
            //    {
            //        if (r.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            //        {
            //            try
            //            {
            //                using (var s = ass.GetManifestResourceStream(r))
            //                {
            //                    var bts = new byte[s.Length];
            //                    s.Read(bts, 0, (int)s.Length);
            //                    var da = Assembly.Load(bts);
            //                    assDictionary.Add(da.FullName, da);
            //                }
            //            }
            //            catch(Exception ex)
            //            {
            //                LogManager.Message(string.Format("{0}加载失败 {1}", r, ex.Message));
            //            }
            //        }
            //    }

            //    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            //}
        }

        /// <summary>
        /// Database username
        /// </summary>
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }

        /// <summary>
        /// Database password
        /// </summary>
        public string PassWord
        {
            get
            {
                return passWord;
            }
            set
            {
                passWord = value;
            }
        }

        /// <summary>
        /// Database host
        /// </summary>
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
            }
        }

        /// <summary>
        /// Database port
        /// </summary>
        public string Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }

        /// <summary>
        /// Database name
        /// </summary>
        public string DataBaseName
        {
            get
            {
                return dbName;
            }
            set
            {
                dbName = value;
            }
        }

        /// <summary>
        /// Connect to database
        /// </summary>
        /// <param name="Type">Database type</param>
        public void Connect(DataBaseType Type)
        {
            if (!isClose)
            {
                db.Close();
            }
            string TypeName = Enum.GetName(typeof(DataBaseType), Type);
            Type t = typeof(DataBase);
            MemberInfo[] mi = t.GetMember(TypeName, BindingFlags.NonPublic);
            if (mi.Length < 1)
            {
                throw new InvalidDataBaseTypeException(TypeName);
            }
            Type dbType = mi[0] as Type;
            if (dbType == null)
            {
                throw new InvalidDataBaseTypeException(TypeName);
            }
            try
            {
                db = Activator.CreateInstance(dbType, true) as IDb;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                throw ex;
            }

            db.Host = host;
            db.Port = port;
            db.DataBaseName = dbName;
            db.UserName = userName;
            db.PassWord = passWord;
            LogManager.Debug(string.Format("Connect {0} {1}:{2}@{3} {4} {5}", dbType, host, port, dbName, userName, passWord));
            db.Open();
            isClose = false;
        }

        /// <summary>
        /// Check database opened
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return !isClose;
            }
        }

        /// <summary>
        /// Close database
        /// </summary>
        public void Close()
        {
            if (db != null)
            {
                db.Close();
                db = null;
            }
            isClose = true;
        }

        /// <summary>
        /// Execute sql with none result
        /// </summary>
        /// <param name="sql"></param>
        public void ExecuteNoneQuery(string sql)
        {
            if (db != null)
            {
                LogManager.Debug(string.Format("ExecuteNoneQuery : {0}", sql));
                db.ExecuteNoneQuery(sql);
            }
            else
            {
                throw new DataBaseNotOpenException(this.dbName);
            }
        }

        /// <summary>
        /// Execute sql with a result
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ResultSet ExecuteQuery(string sql)
        {
            if (db != null)
            {
                LogManager.Debug(string.Format("ExecuteQuery : {0}", sql));
                string result = db.ExecuteQuery(sql);
                ResultSet rs = new ResultSetClass(result);
                return rs;
            }
            else
            {
                throw new DataBaseNotOpenException(this.dbName);
            }
        }

        /// <summary>
        /// Get all column's name which in table named <code>TableName</code>
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public List<string> GetColumnNames(string TableName)
        {
            if (db != null)
            {
                return db.GetColumnNames(TableName);
            }
            else
            {
                throw new DataBaseNotOpenException(this.dbName);
            }
        }

        public List<TableKeyInfo> GetColumnInfo(string TableName)
        {
            if (db != null)
            {
                return db.GetColumnInfo(TableName);
            }
            else
            {
                throw new DataBaseNotOpenException(this.dbName);
            }
        }

        /// <summary>
        /// Get all primary key's infomation which in table named <code>TableName</code>
        /// </summary>
        /// <param name="TableName">table name</param>
        /// <returns></returns>
        public List<TableKeyInfo> GetPrimaryKeyInfo(string TableName)
        {
            if (db != null)
            {
                return db.GetPrimaryKeyInfo(TableName);
            }
            else
            {
                throw new DataBaseNotOpenException(this.dbName);
            }
        }

        /// <summary>
        /// Get all primary key's name which in table named <code>TableName</code>
        /// </summary>
        /// <param name="TableName">table name</param>
        /// <returns></returns>
        public List<string> GetPrimaryKeyName(string TableName)
        {
            if (db != null)
            {
                return db.GetPrimaryKeyName(TableName);
            }
            else
            {
                throw new DataBaseNotOpenException(this.dbName);
            }
        }

        /// <summary>
        /// Get all table name's name in this database
        /// </summary>
        /// <returns></returns>
        public List<string> GetTableNames()
        {
            if (db != null)
            {
                return db.GetTableNames();
            }
            else
            {
                throw new DataBaseNotOpenException(this.dbName);
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //var assName = new AssemblyName(args.Name).FullName;
            //Assembly ass;
            //if (assDictionary.TryGetValue(assName, out ass) && ass != null)
            //{
            //    assDictionary[assName] = null;//如果有则置空并返回
            //    return ass;
            //}
            //else
            //{
            //    return null;
            //}
            return null;
        }
        #endregion
    }
}
