namespace Mock.Data
{
    using System;
    using System.Xml;
    using System.Reflection;
    using System.Collections.Generic;

    using Mock.Nature.Exception;
    using Mock.Nature.DataBase;
    using Mock.Data.TaistDataCenter;
    using Mock.Data.Exception;
    using Mock.Data.Attributes;

    public class DbData : IFormatData, ICheckData
    {
        public DbData()
        {
            _data = new List<Dao>();
        }

        private List<Dao> _data = null;
        /// <summary>
        /// 所有的数据库数据对象
        /// </summary>
        [FieldProperty(isIgnore=true)]
        public List<Dao> Data
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// 源数据表名称
        /// </summary>
        public string SourceTableName { get; set; }

        /// <summary>
        /// 源数据表所在数据库信息名称
        /// </summary>
        public string SourceOwner { get; set; }

        /// <summary>
        /// 目标数据表名称
        /// </summary>
        public string TargetTableName { get; set; }

        /// <summary>
        /// 目标数据表所在数据库信息名称
        /// </summary>
        public string TargetOwner { get; set; }

        private string _compareNameList = null;
        public string CompareContextNameList
        {
            get
            {
                return _compareNameList;
            }
            set
            {
                _compareNameList = value;
                if (compareContext == null)
                {
                    compareContext = new Dictionary<string, Dictionary<string, string>>();
                }
                List<string> nameList = DataFactory.ParseBH(value);
                foreach (string name in nameList)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }
                    if (!compareContext.ContainsKey(name))
                    {
                        compareContext.Add(name, new Dictionary<string, string>());
                    }

                    DbData dbData = TestCasePool.GetData<DbData>(name);
                    compareContext[name] = dbData.compareContext["DefaultDatabaseCheck"];
                    foreach (KeyValuePair<string, Dictionary<string, string>> kv in dbData.compareContext)
                    {
                        if(compareContext.ContainsKey(kv.Key)) continue;
                        if (string.Equals(kv.Key, "DefaultDatabaseCheck")) continue;
                        compareContext.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        private Dictionary<string, string> sourceCondition = null;
        private Dictionary<string, string> targetCondition = null;
        private Dictionary<string, Dictionary<string, string>> compareContext = null;

        public void setProperty(XmlNode node)
        {
            if (string.Equals(node.Name, "SourceCondition"))
            {
                if (sourceCondition == null) sourceCondition = new Dictionary<string, string>();
                
                foreach (XmlNode child in node.ChildNodes)
                {
                    sourceCondition.Add(child.Name, child.InnerText);
                }

                foreach (XmlAttribute xa in node.Attributes)
                {
                    sourceCondition.Add(xa.Name, xa.Value);
                }
            }
            else if (string.Equals(node.Name, "TargetCondition"))
            {
                if (targetCondition == null) targetCondition = new Dictionary<string, string>();

                foreach (XmlNode child in node.ChildNodes)
                {
                    targetCondition.Add(child.Name, child.InnerText);
                }

                foreach (XmlAttribute xa in node.Attributes)
                {
                    targetCondition.Add(xa.Name, xa.Value);
                }
            }
            else
            {
                if (compareContext == null)
                {
                    compareContext = new Dictionary<string, Dictionary<string, string>>();
                }

                if (!compareContext.ContainsKey("DefaultDatabaseCheck"))
                {
                    compareContext.Add("DefaultDatabaseCheck", new Dictionary<string, string>());
                }

                compareContext["DefaultDatabaseCheck"].Add(node.Name, node.InnerXml);
            }
        }

        public override IFormatData FromXml(System.Xml.XmlNode doc, Dictionary<string, string> condition)
        {
            string conditionString = DataFactory.GetXmlConditionString("DbData", condition);
            //if (condition == null || condition.Count < 1)
            //{
            //    conditionString = "//DbData";
            //}
            //else
            //{
            //    foreach (KeyValuePair<string, string> kv in condition)
            //    {
            //        if (conditionString == string.Empty)
            //        {
            //            conditionString = string.Format("//DbData[{0}='{1}']", kv.Key, kv.Value);
            //        }
            //        else
            //        {
            //            conditionString = string.Format("{0} and {1}='{2}']", conditionString.TrimEnd(']'), kv.Key, kv.Value);
            //        }
            //    }
            //}
            
            XmlNode dbNode = doc.SelectSingleNode(conditionString);
            if (dbNode == null)
            {
                LogManager.Debug("dbNode is null");
                return null;
            }

            InputNode = dbNode;
            foreach (XmlNode xn in dbNode.ChildNodes)
            {
                if (xn is XmlComment)
                {
                    continue;
                }

                string name = xn.Name;
                
                LogManager.Debug(string.Format("DbData set value {0}", xn.Name));
                if (string.Equals(name, "Bh"))
                {
                    Bh = xn.InnerText;
                }
                else
                {
                    Type type = null;
                    try
                    {
                        ClassInfo ci = DataFactory.GetClassInfo(xn.Name);
                        type = ci.Type;
                    }
                    catch (CanNotFindDataException)
                    {
                        setProperty(xn);
                        continue;
                    }
                    LogManager.Debug(type.Name);
                    object obj = Activator.CreateInstance(type);
                    if (obj == null)
                    {
                        LogManager.Message("Create instance of " + type.Name + " failed.");
                    }

                    if (obj is Dao)
                    {
                        Dao dao = obj as Dao;
                        if (dao == null)
                        {
                            LogManager.Message("Change to Dao failed");
                            continue;
                        }
                        dao.InputNode = xn;
                        XmlAttribute ownerAttr = xn.Attributes["Owner"];
                        if (ownerAttr != null)
                        {
                            if (!string.IsNullOrEmpty(ownerAttr.Value))
                            {   
                                dao.Owner = ownerAttr.Value;
                            }
                        }
                        foreach (XmlNode tmp in xn.ChildNodes)
                        {
                            if (tmp is XmlComment)
                            {
                                continue;
                            }
                            PropertyInfo pi = type.GetProperty(tmp.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                            if (pi == null)
                            {
                                LogManager.Debug("Can not find property named " + tmp.Name);
                                //throw new InvalidDataTypeException(xn.Name);
                                continue;
                            }
                            LogManager.Debug(string.Format("{0} set property {1} = {2}", xn.Name, tmp.Name, tmp.InnerText));
                            pi.SetValue(obj, tmp.InnerText, null);
                        }

                        _data.Add((Dao)obj);
                    }
                    else
                    {
                        LogManager.Debug(xn.Name + " is not instance of Dao");
                        throw new InvalidDataTypeException(xn.Name);
                    }
                }
            }
            return (IFormatData)this;
        }

        public override string ToXml()
        {
            return "";
        }

        public universal Check(Cache cache)
        {
            ClassInfo sourceClass = DataFactory.GetClassInfo(SourceTableName);
            ClassInfo targetClass = DataFactory.GetClassInfo(TargetTableName);

            Dao sourceDao = (Dao)Activator.CreateInstance(sourceClass.Type);
            sourceDao.Owner = SourceOwner;
            Dao targetDao = (Dao)Activator.CreateInstance(targetClass.Type);
            targetDao.Owner = TargetOwner;

            InitDao(sourceDao, sourceCondition, cache);
            InitDao(targetDao, targetCondition, cache);
            
            if (compareContext == null)
            {
                LogManager.Debug("没有要比对的数据");
                return "没有要比对的数据";
            }
            List<string> resultList = new List<string>();

            foreach (KeyValuePair<string, Dictionary<string, string>> kv1 in compareContext)
            {
                foreach (KeyValuePair<string, string> kv in kv1.Value)
                {
                    PropertyInfo sp = sourceClass.Type.GetProperty(kv.Key);
                    if (sp == null)
                    {
                        throw new FieldIsNotInTableException(sourceClass.Type.Name, kv.Key);
                    }

                    PropertyInfo tp = targetClass.Type.GetProperty(kv.Value);
                    if (tp == null)
                    {
                        throw new FieldIsNotInTableException(targetClass.Type.Name, kv.Value);
                    }

                    string sfv = DataFactory.GetFormatedValue(kv.Key, sp.GetValue(sourceDao, null));
                    string tfv = DataFactory.GetFormatedValue(kv.Value, tp.GetValue(targetDao, null));
                    if (!string.Equals(sfv, tfv))
                    {
                        resultList.Add(string.Format("{6} {0}.{1} = {2}, {3}.{4} = {5}",
                            sourceClass.Type.Name,
                            kv.Key,
                            sfv,
                            targetClass.Type.Name,
                            kv.Value,
                            tfv,
                            kv1.Key));
                    }
                }
            }
            if (resultList.Count == 0) return true;
            return string.Join("\n", resultList);
        }

        private void InitDao(Dao dao, Dictionary<string, string> condition, Cache cache)
        {
            if (condition == null)
            {
                condition = new Dictionary<string, string>();
            }
            List<string> primaryKeys = dao.GetPrimaryKeys();
            if (cache == null)
            {
                foreach (string primaryKey in primaryKeys)
                {
                    if (!condition.ContainsKey(primaryKey))
                    {
                        throw new PrimaryKeyIsNullException(primaryKey);
                    }

                    PropertyInfo pi = dao.GetType().GetProperty(primaryKey);
                    if (pi == null)
                    {
                        throw new CanNotFindPropertyException(dao.GetType().Name, primaryKey);
                    }
                    pi.SetValue(dao, condition[primaryKey], null);
                }
            }
            else
            {
                foreach (string primaryKey in primaryKeys)
                {
                    string value = null;
                    if (!condition.ContainsKey(primaryKey))
                    {
                        string cacheValue = cache[primaryKey];
                        if (string.IsNullOrEmpty(cacheValue))
                        {
                            throw new PrimaryKeyIsNullException(primaryKey);
                        }
                        value = cache[primaryKey];
                    }
                    else
                    {
                        value = condition[primaryKey];
                    }

                    PropertyInfo pi = dao.GetType().GetProperty(primaryKey);
                    if (pi == null)
                    {
                        throw new CanNotFindPropertyException(dao.GetType().Name, primaryKey);
                    }
                    pi.SetValue(dao, value, null);
                }
            }
            dao.SelectSingle();
        }

        public void Writeback(XmlNode node)
        {
            //throw new NotImplementedException();
        }
    }
}
