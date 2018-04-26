namespace Mock.Nature.DataBase
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.IO;
    using System.Xml;
    using Mock.Nature.Exception;

    using Mock.Data;
    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    using Mock.Data.TaistDataCenter;
    /// <summary>
    /// Database operation class base class
    /// </summary>
    public abstract class Dao : IFormatData, IOwner
    {
        public Dao(DataBaseType dbType)
        {
            _dbType = dbType;
        }

        private string _owner = null;
        [FieldProperty(Name = "Owner", isAttribute = true)]
        public string Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
                if (string.IsNullOrEmpty(_owner)) return;
                LogManager.Debug(string.Format("{0} set data source {1}", this.GetType().Name, _owner));
                try
                {
                    XmlDocument doc = XmlFactory.LoadXml(Path.Combine(Config.WorkingDirectory, "Data\\User\\Environment.xml"));
                    if (doc == null)
                    {
                        LogManager.Debug("Can not find file Data\\User\\Environment.xml");
                    }
                    else
                    {
                        XmlNode node = doc.SelectSingleNode(string.Format("//ConnectionString[@name='{0}']", value));
                        if (node == null)
                        {
                            LogManager.Debug(string.Format("Can not find ConnectionString named {0} from file Data\\User\\Environment.xml", value));
                        }
                        else
                        {
                            ConnectionString conString = DataFactory.GetData<ConnectionString>(node);
                            if (_db != null)
                            {
                                _db.Close();
                            }
                            _db = new DataBase(conString.DatabaseName, conString.User, conString.Password, conString.Host, conString.Port);
                            _db.Connect(_dbType);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogManager.Debug(string.Format("Parse Owner occur error, {0}", ex.Message));
                }
            }
        }

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        [FieldProperty(isIgnore=true)]
        public ConnectionString ConnectionString
        {
            get
            {
                if (_db == null) return null;
                ConnectionString conString = new ConnectionString();
                conString.DatabaseName = _db.DataBaseName;
                conString.Host = _db.Host;
                conString.Port = _db.Port;
                conString.User = _db.UserName;
                conString.Password = _db.PassWord;
                return conString;
            }
            set
            {
                if (value == null) return;
                ConnectionString conString = value;
                if (_db != null)
                {
                    _db.Close();
                }
                _db = new DataBase(conString.DatabaseName, conString.User, conString.Password, conString.Host, conString.Port);
                _db.Connect(_dbType);
            }
        }

        ~Dao()
        {
            if (_db != null)
            {
                _db.Close();
                _db = null;
            }
        }

        private ResultSet _rs = null;
        private DataBase _db = null;
        private DataBaseType _dbType = DataBaseType.SQLite;

        /// <summary>
        /// 查询数据库表数据
        /// </summary>
        public void Select(string condition = null)
        {
            _rs = null;
            string sql = string.Empty;

            if (string.IsNullOrEmpty(condition))
            {
                condition = conditionString;
            }

            if (string.IsNullOrEmpty(condition))
            {
                sql = string.Format("select * from {0}", this.GetType().Name);
            }
            else
            {
                sql = string.Format("select * from {0} where {1}", this.GetType().Name, condition.Trim());
            }
            _rs = Db.ExecuteQuery(sql);

            if (_rs.Next)
            {
                object obj = _rs.GetObject(GetType());
                copy(obj);
            }
            else
            {
                throw new CanNotFindDataException(sql);
            }
        }

        /// <summary>
        /// 查询数据库表数据
        /// </summary>
        public void SelectSingle()
        {
            _rs = null;
            string sql = string.Empty;

            string condition = primaryConditionString;

            if (string.IsNullOrEmpty(condition))
            {
                throw new DataBaseRuntimeException(string.Format("未找到{0}表的主键信息", this.GetType().Name));
            }
            else
            {
                sql = string.Format("select * from {0} where {1}", this.GetType().Name, condition.Trim());
            }
            
            _rs = Db.ExecuteQuery(sql);

            if (_rs.Next)
            {
                object obj = _rs.GetObject(GetType());
                copy(obj);
            }
            else
            {
                throw new CanNotFindDataException(sql);
            }
        }

        /// <summary>
        /// 将数据更新到数据库表中
        /// </summary>
        public void Update(string condition = null)
        {
            _rs = null;
            string sql = string.Empty;
            string selectSql = string.Empty;
            if (string.IsNullOrEmpty(condition))
            {
                condition = primaryConditionString;
            }
            if (string.IsNullOrEmpty(condition))
            {
                throw new DataBaseRuntimeException("condition is null");
                //selectSql = string.Format("select * from {0}", this.GetType().Name);
            }
            else
            {
                selectSql = string.Format("select * from {0} where {1}", this.GetType().Name, condition.Trim());
            }
            ResultSet rs = Db.ExecuteQuery(selectSql);
            Type t = GetType();

            List<TableKeyInfo> columnName = Columns;
            if (rs.Count > 0)
            {
                //查找到数据， 执行UPDATE语句
                LogManager.Message("Update...");
                string dataString = string.Empty;
                foreach (TableKeyInfo keyInfo in columnName)
                {
                    if (!IsFromUser(keyInfo.Name)) continue;
                    PropertyInfo pi = t.GetProperty(keyInfo.Name);
                    
                    if (pi == null)
                    {
                        throw new DataBaseRuntimeException(string.Format("未找到属性[{0}]", keyInfo.Name));
                    }

                    object value = pi.GetValue(this, null);
                    if (value == null) continue;
                    if (string.Equals(value.ToString().Trim(), ""))
                    {
                        if (string.IsNullOrEmpty(dataString))
                        {
                            dataString = string.Format("{0} = ''", pi.Name);
                        }
                        else
                        {
                            dataString = string.Format("{0}, {1} = ''", dataString, pi.Name);
                        }
                        continue;
                    }

                    if (string.Equals(keyInfo.Type, "STRING"))
                    {
                        if (string.IsNullOrEmpty(dataString))
                        {
                            dataString = string.Format("{0} = '{1}'", pi.Name, pi.GetValue(this, null));
                        }
                        else
                        {
                            dataString = string.Format("{0}, {1} = '{2}'", dataString, pi.Name, pi.GetValue(this, null));
                        }
                    }
                    else if (string.Equals(keyInfo.Type, "DATE"))
                    {
                        switch (_dbType)
                        {
                            case DataBaseType.Oracle:
                                {
                                    if (string.IsNullOrEmpty(dataString))
                                    {
                                        dataString = string.Format("{0} = to_date('{1}', 'yyyy-mm-dd hh24:mi:ss')", pi.Name, ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    else
                                    {
                                        dataString = string.Format("{0}, {1} = to_date('{2}', 'yyyy-mm-dd hh24:mi:ss')", dataString, pi.Name, ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (string.IsNullOrEmpty(dataString))
                                    {
                                        dataString = string.Format("{0} = '{1}'", pi.Name, ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    else
                                    {
                                        dataString = string.Format("{0}, {1} = '{2}'", dataString, pi.Name, ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(dataString))
                        {
                            dataString = string.Format("{0} = {1}", pi.Name, pi.GetValue(this, null));
                        }
                        else
                        {
                            dataString = string.Format("{0}, {1} = {2}", dataString, pi.Name, pi.GetValue(this, null));
                        }
                    }
                }
                sql = string.Format("update {0} set {1} where {2}", this.GetType().Name, dataString, condition);
                Db.ExecuteNoneQuery(sql);
                try
                {
                    _rs = Db.ExecuteQuery(selectSql);
                }
                catch (Exception ex)
                {
                    LogManager.ErrorOnlyPrint(ex);
                }
            }
            else
            {
                //未查找到数据，执行INSERT语句
                Insert();
            }
        }

        /// <summary>
        /// 将数据插入到数据库表中
        /// </summary>
        public void Insert()
        {
            _rs = null;
            LogManager.Debug("Insert...");
            string sql = string.Empty;
            string keyString = string.Empty;
            string valueString = string.Empty;
            List<TableKeyInfo> columnName = Columns;
            Type t = GetType();

            foreach (TableKeyInfo keyInfo in columnName)
            {
                if (string.IsNullOrEmpty(keyString))
                {
                    keyString = string.Format("{0}", keyInfo.Name);
                }
                else
                {
                    keyString = string.Format("{0}, {1}", keyString, keyInfo.Name);
                }

                PropertyInfo pi = t.GetProperty(keyInfo.Name);
                if (pi == null)
                {
                    throw new DataBaseRuntimeException(string.Format("未找到属性[{0}]", keyInfo.Name));
                }

                object value = pi.GetValue(this, null);
                if (value == null)
                {
                    if (string.IsNullOrEmpty(valueString))
                    {
                        valueString = "null";
                    }
                    else
                    {
                        valueString = string.Format("{0}, null", valueString);
                    }
                    continue;
                }

                if (string.Equals(keyInfo.Type, "STRING"))
                {
                    if (string.IsNullOrEmpty(valueString))
                    {
                        valueString = string.Format("'{0}'", value);
                    }
                    else
                    {
                        valueString = string.Format("{0}, '{1}'", valueString, value);
                    }
                }
                else if (string.Equals(keyInfo.Type, "DATE"))
                {
                    switch (_dbType)
                    {
                        case DataBaseType.Oracle:
                            {
                                if (string.IsNullOrEmpty(valueString))
                                {
                                    valueString = string.Format("to_date('{0}', 'yyyy-mm-dd hh24:mi:ss')", ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    valueString = string.Format("{0}, to_date('{1}', 'yyyy-mm-dd hh24:mi:ss')", valueString, ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                break;
                            }
                        default:
                            {
                                if (string.IsNullOrEmpty(valueString))
                                {
                                    valueString = string.Format("'{0}'", ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    valueString = string.Format("{0}, '{1}'", valueString, ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                break;
                            }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(valueString))
                    {
                        valueString = string.Format("{0}", value);
                    }
                    else
                    {
                        valueString = string.Format("{0}, {1}", valueString, value);
                    }
                }
            }
            sql = string.Format("insert into {0} ({1}) values ({2})", this.GetType().Name, keyString, valueString);
            Db.ExecuteNoneQuery(sql);
            try
            {
                SelectSingle();
            }
            catch (Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
            }
        }

        private DateTime ToDateTime(string dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return DateTime.Now;

            dateString = dateString.Replace('/', '-');
            dateString = dateString.Replace('年', '-');
            dateString = dateString.Replace('月', '-');
            dateString = dateString.Replace('日', '-');
            dateString = dateString.Replace('时', ':');
            dateString = dateString.Replace('分', ':');
            dateString = dateString.Replace('秒', ':');
            return DateTime.Parse(dateString);
        }

        /// <summary>
        /// 删除数据库表中的数据
        /// </summary>
        public void Delete(string condition = null)
        {
            _rs = null;

            if (string.IsNullOrEmpty(condition))
            {
                condition = primaryConditionString;
            }

            try
            {
                Select(condition);
            }
            catch (Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
            }
            string tableName = this.GetType().Name;
            Db.ExecuteNoneQuery(string.Format("delete from {0} where {1}", tableName, condition));
        }

        /// <summary>
        /// 删除该表的所有数据
        /// </summary>
        public void DeleteAll(string condition = null)
        {
            string sql = string.Empty;

            if (string.IsNullOrEmpty(condition))
            {
                condition = conditionString;
            }

            if (string.IsNullOrEmpty(condition))
            {
                sql = string.Format("delete from {0}", this.GetType().Name);
            }
            else
            {
                sql = string.Format("delete from {0} where {1}", this.GetType().Name, condition.Trim());
            }
            Db.ExecuteNoneQuery(sql);
        }

        /// <summary>
        /// 设置数据库表列值
        /// </summary>
        /// <param name="columnName">数据库表列名</param>
        /// <param name="columnValue">数据库表列值</param>
        public void SetColumnValue(string columnName, string columnValue)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ParameterIsNullException("columnName");

            TaistProperty property = TaistProperty.GetDataProperty(columnName, this);
            property.Set(columnValue);
        }

        /// <summary>
        /// 获取数据库表列值
        /// </summary>
        /// <param name="columnName">数据库表列名</param>
        /// <returns></returns>
        public string GetColumnValue(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ParameterIsNullException("columnName");

            TaistProperty property = TaistProperty.GetDataProperty(columnName, this);
            object columnValue = property.Get();
            if (columnValue == null) return null;
            else return columnValue.ToString();
        }

        /// <summary>
        /// 克隆一个对象的副本
        /// </summary>
        /// <returns></returns>
        public Dao Clone()
        {
            //Type objType = this.GetType();
            //object obj = Activator.CreateInstance(objType);

            //PropertyInfo[] pArray = objType.GetProperties();

            //foreach (PropertyInfo pi in pArray)
            //{
            //    try
            //    {
            //        //if (string.Equals("Owner", pi.Name) || string.Equals("DataBase", pi.Name)) continue;
            //        if (string.Equals("DataBase", pi.Name)) continue;
            //        pi.SetValue(obj, pi.GetValue(this, null), null);
            //    }
            //    catch (Exception ex)
            //    {
            //        while (ex is TargetInvocationException)
            //        {
            //            ex = ex.InnerException;
            //        }
            //        LogManager.Message(string.Format("Set property {1} throw exception {0}", ex.Message, pi.Name));
            //    }
            //}
            //return (Dao)obj;
            return Clone<Dao>();
        }

        private string conditionString
        {
            get
            {
                Type t = GetType();
                string sql = string.Empty;
                List<TableKeyInfo> keyInfoList = Db.GetColumnInfo(t.Name);
                foreach (TableKeyInfo tableKey in keyInfoList)
                {
                    PropertyInfo pi = t.GetProperty(tableKey.Name);
                    if (pi == null)
                    {
                        throw new DataBaseRuntimeException(string.Format("未找到{1}表的字段[{0}]", tableKey.Name, this.GetType().Name));
                    }
                    object value = pi.GetValue(this, null);
                    if (value == null)
                    {
                        continue;
                    }

                    if (string.Equals(tableKey.Type, "STRING"))
                    {
                        if (string.IsNullOrEmpty(sql))
                        {
                            sql = string.Format("{0} = '{1}'", pi.Name, pi.GetValue(this, null));
                        }
                        else
                        {
                            sql = string.Format("{0} and {1} = '{2}'", sql, pi.Name, pi.GetValue(this, null));
                        }
                    }
                    else if (string.Equals(tableKey.Type, "DATE"))
                    {
                        switch (_dbType)
                        {
                            case DataBaseType.Oracle:
                                {
                                    if (string.IsNullOrEmpty(sql))
                                    {
                                        sql = string.Format("{0} = to_date('{1}', 'yyyy-mm-dd hh24:mi:ss')", pi.Name, ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    else
                                    {
                                        sql = string.Format("{0} and {1} = to_date('{2}', 'yyyy-mm-dd hh24:mi:ss')", sql, pi.Name, ToDateTime(value.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (string.IsNullOrEmpty(sql))
                                    {
                                        sql = string.Format("{0} = '{1}'", pi.Name, pi.GetValue(this, null));
                                    }
                                    else
                                    {
                                        sql = string.Format("{0} and {1} = '{2}'", sql, pi.Name, pi.GetValue(this, null));
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        object obj = pi.GetValue(this, null);
                        string v = obj == null ? null : obj.ToString();
                        v = string.IsNullOrEmpty(v) ? "0" : v;
                        if (string.IsNullOrEmpty(sql))
                        {
                            sql = string.Format("{0} = {1}", pi.Name, v);
                        }
                        else
                        {
                            sql = string.Format("{0} and {1} = {2}", sql, pi.Name, v);
                        }
                    }
                }
                return sql;
            }
        }

        private string primaryConditionString
        {
            get
            {
                Type t = GetType();

                string sql = string.Empty;
                PropertyInfo[] piArray = this.GetType().GetProperties();
                foreach (PropertyInfo pi in piArray)
                {
                    object[] attrArray = pi.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                    if (attrArray == null || attrArray.Length == 0) continue;

                    PrimaryKeyAttribute pkAttr = attrArray[0] as PrimaryKeyAttribute;

                    object value = pi.GetValue(this, null);
                    if (value == null)
                    {
                        throw new DataBaseRuntimeException(string.Format("未找到{1}表的主键[{0}]信息", pi.Name, this.GetType().Name));
                    }

                    if (pkAttr.StringType)
                    {
                        if (string.IsNullOrEmpty(sql))
                        {
                            sql = string.Format("{0} = '{1}'", pi.Name, value);
                        }
                        else
                        {
                            sql = string.Format("{0} and {1} = '{2}'", sql, pi.Name, value);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(sql))
                        {
                            sql = string.Format("{0} = {1}", pi.Name, value);
                        }
                        else
                        {
                            sql = string.Format("{0} and {1} = {2}", sql, pi.Name, value);
                        }
                    }
                }
                
                if (string.IsNullOrEmpty(sql))
                {
                    List<TableKeyInfo> primaryKeyInfoList = Db.GetPrimaryKeyInfo(t.Name);
                    foreach (TableKeyInfo primaryKey in primaryKeyInfoList)
                    {
                        PropertyInfo pi = null;
                        try
                        {
                            pi = t.GetProperty(primaryKey.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                        }
                        catch (AmbiguousMatchException)
                        {
                            pi = t.GetProperty(primaryKey.Name, BindingFlags.Instance | BindingFlags.Public);
                        }
                        if (pi == null)
                        {
                            throw new DataBaseRuntimeException(string.Format("未找到{1}表的主键[{0}]", primaryKey.Name, this.GetType().Name));
                        }
                        object value = pi.GetValue(this, null);
                        if (value == null)
                        {
                            throw new DataBaseRuntimeException(string.Format("未给{1}表的主键[{0}]设置信息", primaryKey.Name, this.GetType().Name));
                        }

                        if (string.Equals(primaryKey.Type, "STRING"))
                        {
                            if (string.IsNullOrEmpty(sql))
                            {
                                sql = string.Format("{0} = '{1}'", pi.Name, pi.GetValue(this, null));
                            }
                            else
                            {
                                sql = string.Format("{0} and {1} = '{2}'", sql, pi.Name, pi.GetValue(this, null));
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sql))
                            {
                                sql = string.Format("{0} = {1}", pi.Name, pi.GetValue(this, null));
                            }
                            else
                            {
                                sql = string.Format("{0} and {1} = {2}", sql, pi.Name, pi.GetValue(this, null));
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(sql))
                {
                    throw new PrimaryKeyNotExistException(t.Name);
                }
                return sql;
            }
        }

        /// <summary>
        /// 获取下一个查询结果值，结果存储在当前实例中
        /// </summary>
        /// <returns>存在下一个结果值，返回true，反之，返回false</returns>
        public bool Next()
        {
            if (_rs == null)
            {
                return false;
            }
            if (_rs.Next)
            {
                object obj = _rs.GetObject(this.GetType());
                copy(obj);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void copy(object dao)
        {
            Type t = this.GetType();
            PropertyInfo[] pArray = t.GetProperties();
            List<string> ExceptList = new List<string>();
            ExceptList.Add("ResultString");
            ExceptList.Add("Owner");
            ExceptList.Add("DataBase");
            ExceptList.Add("DataFilePath");
            if (dao == null)
            {
                LogManager.Message("dao is null");
                foreach (PropertyInfo pi in pArray)
                {
                    if (ExceptList.Contains(pi.Name)) continue;
                    pi.SetValue(this, null, null);
                }
            }
            else
            {
                foreach (PropertyInfo pi in pArray)
                {
                    if (ExceptList.Contains(pi.Name)) continue;
                    
                    pi.SetValue(this, pi.GetValue(dao, null), null);
                }
            }
        }

        public List<string> GetPrimaryKeys()
        {
            List<string> ls = new List<string>();
            PropertyInfo[] piArray = this.GetType().GetProperties();
            foreach (PropertyInfo pi in piArray)
            {
                object[] attrArray = pi.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                if (attrArray == null || attrArray.Length == 0) continue;

                ls.Add(pi.Name);
            }
            
            if (ls == null || ls.Count == 0)
            {
                ls = Db.GetPrimaryKeyName(this.GetType().Name);
            }

            return ls;
        }

        private DataBase Db
        {
            get
            {
                if (_db == null)
                {
                    LogManager.Debug("Not set owner, use default data source");
                    IOwner owner = this as IOwner;
                    if (owner == null)
                    {
                        throw new NotImplementingInterfaceException(this.GetType().Name, "IOwner");
                    }
                    else
                    {
                        _db = owner.DataBase;
                        _db.Connect(_dbType);
                    }
                }
                else
                {
                    if (!_db.IsOpen)
                    {
                        _db.Connect(_dbType);
                    }
                }

                return _db;
            }
        }

        private List<TableKeyInfo> Columns
        {
            get
            {
                return Db.GetColumnInfo(GetType().Name);
            }
        }

        /// <summary>
        /// 返回查询数据的查询结果
        /// </summary>
        [FieldProperty(isIgnore = true)]
        public string ResultString
        {
            get
            {
                if (_rs == null)
                {
                    return null;
                }
                else
                {
                    string rsString = _rs.ToString();
                    if (string.IsNullOrEmpty(rsString))
                    {
                        return "<ResultString />";
                    }
                    else
                    {
                        XmlDocument doc = XmlFactory.LoadXml(rsString);
                        XmlNodeList nodeList = doc.SelectNodes("//Data");
                        if (nodeList == null || nodeList.Count == 0)
                        {
                            return "<ResultString />";
                        }
                        else
                        {
                            string result = string.Empty;
                            foreach (XmlNode node in nodeList)
                            {
                                result = string.Format("{2}<{0}>{1}</{0}>", this.GetType().Name, node.InnerXml, result);
                            }
                            return string.Format("<ResultString>{0}</ResultString>", result); 
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 默认的数据库连接
        /// </summary>
        [FieldProperty(Name = "Owner", isIgnore=true)]
        public abstract DataBase DataBase { get; }
    }
}
