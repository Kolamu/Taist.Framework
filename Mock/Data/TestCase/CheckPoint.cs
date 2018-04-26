namespace Mock.Data
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    using Mock.Data;
    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    using Mock.Data.TaistDataCenter;

    using Mock.Nature.DataBase;
    using Mock.Nature.Exception;

    public sealed class CheckPoint : IFormatData
    {
        private class CheckObject
        {
            internal CheckObject(int index, object obj)
            {
                nodeIndex = index;
                objData = obj;
            }
            internal int nodeIndex = 0;
            internal object objData = null;
        }

        private string originalString = null;
        private List<CheckObject> _checkObjectList = null;
        private string filePath = null;
        private string origPath = null;
        public CheckPoint()
        {
            _checkObjectList = new List<CheckObject>();
        }

        private string _dataBh = null;

        public string DataBh
        {
            get
            {
                if (string.IsNullOrEmpty(_dataBh))
                {
                    return Bh;
                }
                else
                {
                    return _dataBh;
                }
            }
            set
            {
                _dataBh = value;
            }
        }

        /// <summary>
        /// 精准比对
        /// </summary>
        public bool IsStrict { get; set; }

        /// <summary>
        /// 反向比对
        /// </summary>
        public bool IsNegative { get; set; }

        /// <summary>
        /// 比对类型
        /// </summary>
        public string CheckType { get; set; }

        [FieldProperty(isIgnore = true)]
        public List<object> DataList
        {
            get
            {
                return _checkObjectList.ConvertAll(x =>
                    {
                        XmlNode newNode = XmlFactory.LoadXml(originalString).DocumentElement;
                        object val = x.objData;
                        if (val is Dao)
                        {
                            XmlNode node = newNode.ChildNodes[x.nodeIndex];
                            InitDao((Dao)val, node);
                        }
                        return val;
                    });
            }
        }

        public void Check()
        {
            bool failed = false;
            LogManager.Message(string.Format("开始比对编号为：{0}的数据, 数据编号{1}", Bh, DataBh));

            XmlNode newNode = XmlFactory.LoadXml(originalString).DocumentElement;
            if (newNode == null)
            {
                LogManager.Debug("Orz ：" + originalString);
            }
            foreach (CheckObject checkObject in _checkObjectList)
            {
                object item = checkObject.objData;
                ReportDetail report = new ReportDetail();
                XmlNode node = newNode.ChildNodes[checkObject.nodeIndex];
                
                if (item is Dao)
                {
                    string name = item.GetType().Name;
                    report.Set("name", "数据库表" + name);
                    LogManager.Message("开始比对数据库表 " + name);
                    try
                    {
                        Dao origin = (Dao)item;
                        Dao copy = origin.Clone();
                        GetDao(copy, node);
                        universal result = CompareDao(copy, node);
                        if (result)
                        {
                            LogManager.Message(string.Format("数据库表[{0}]比对成功", name));
                            report.SetResult(true);
                            report.Set("state", "S");
                        }
                        else
                        {
                            failed = true;
                            WriteDao(copy, node);
                            report.SetResult(false);
                            report.Set("state", "F");
                            report.Set("msg", result);
                            LogManager.Message(string.Format("数据库表比对失败 ： {0}", result.StringValue));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        report.SetResult(false);
                        report.Set("state", "F");
                        report.Set("msg", ex.Message);
                        LogManager.Message(string.Format("数据库表[{0}]比对失败 ： {1}\n{2}", name, ex.Message, ex.StackTrace));
                    }
                    TestCasePool.SetReportDetail(report);
                }
                else if (item is Cache)
                {
                    report.Set("name", "缓存");
                    LogManager.Message("开始比对缓存");
                    try
                    {
                        Cache cache = item as Cache;
                        universal result = "";
                        Cache cache1 = new Cache();
                        cache1.Bh = DataBh;
                        cache1.Get();
                        Cache cache2 = new Cache();
                        try
                        {
                            string bh = cache["TargetDataBh"];
                            if (string.IsNullOrEmpty(bh))
                            {
                                cache2.Bh = DataBh;
                            }
                            else
                            {
                                cache2.Bh = bh;
                            }
                        }
                        catch (CanNotFindDataException)
                        {
                            cache2.Bh = DataBh;
                        }
                        cache2.Get();
                        Dictionary<string, string> cond1 = new Dictionary<string, string>();

                        if (!string.IsNullOrEmpty(cache.Position))
                        {
                            cond1.Add("position", cache.Position);
                        }

                        Dictionary<string, string> cond2 = new Dictionary<string, string>();

                        if(!string.IsNullOrEmpty(cache.SelectCondition))
                        {
                            string[] condArray = cache.SelectCondition.Split(',');
                            foreach (string conString in condArray)
                            {
                                string key = cache[conString];
                                key = key.Substring(key.LastIndexOf('.') + 1);
                                cond2.Add(key, cache1[conString, cond1]);
                            }
                        }
                        Dictionary<string, string> Properties = cache.getProperty();

                        result = true;
                        foreach (KeyValuePair<string, string> kv in Properties)
                        {
                            if (string.Equals(kv.Key, "TargetDataBh")) continue;
                            try
                            {
                                if (IsStrict)
                                {
                                    if (!string.Equals(cache1[kv.Key, cond1], cache2[kv.Value, cond2]))
                                    {
                                        if (!IsNegative)
                                        {
                                            result = string.Format("比对缓存失败 {4}.{0} = {1}, {5}.{2} = {3}", kv.Key, cache1[kv.Key, cond1], kv.Value, cache2[kv.Value, cond2], cache1.Bh, cache2.Bh);
                                        }
                                    }
                                    else
                                    {
                                        if (IsNegative)
                                        {
                                            result = string.Format("比对缓存失败 {4}.{0} = {1}, {5}.{2} = {3}", kv.Key, cache1[kv.Key, cond1], kv.Value, cache2[kv.Value, cond2], cache1.Bh, cache2.Bh);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!string.Equals(DataFactory.GetFormatedValue(kv.Key, cache1[kv.Key, cond1]), DataFactory.GetFormatedValue(kv.Value, cache2[kv.Value, cond2])))
                                    {
                                        if (!IsNegative)
                                        {
                                            result = string.Format("比对缓存失败 {4}.{0} = {1}, {5}.{2} = {3}", kv.Key, cache1[kv.Key, cond1], kv.Value, cache2[kv.Value, cond2], cache1.Bh, cache2.Bh);
                                        }
                                    }
                                    else
                                    {
                                        if (IsNegative)
                                        {
                                            result = string.Format("比对缓存失败 {4}.{0} = {1}, {5}.{2} = {3}", kv.Key, cache1[kv.Key, cond1], kv.Value, cache2[kv.Value, cond2], cache1.Bh, cache2.Bh);
                                        }
                                    }
                                }
                            }
                            catch (CanNotFindDataException ex)
                            {
                                if (!IsNegative)
                                {
                                    result = string.Format("比对缓存失败 {2}.{0} != {3}.{1} {4}", kv.Key, kv.Value, cache1.Bh, cache2.Bh, ex.Message);
                                }
                            }
                            catch (TaistException ex)
                            {
                                result = string.Format("比对缓存失败 {2}.{0} != {3}.{1} {4}", kv.Key, kv.Value, cache1.Bh, cache2.Bh, ex.Message);
                            }
                        }

                        if (result)
                        {
                            LogManager.Message("缓存比对成功");
                            report.SetResult(true);
                            report.Set("state", "S");
                        }
                        else
                        {
                            report.SetResult(false);
                            report.Set("state", "F");
                            report.Set("msg", result);
                            LogManager.Message(string.Format("缓存比对失败 ： {0}", result.StringValue));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        report.SetResult(false);
                        report.Set("state", "F");
                        report.Set("msg", ex.Message);
                        LogManager.Error(string.Format("缓存比对失败 ： {0}\n{1}", ex.Message, ex.StackTrace));
                    }
                    TestCasePool.SetReportDetail(report);
                }
                else if (item is XmlDocument)
                {
                    XmlDocument doc = item as XmlDocument;
                    if (doc == null)
                    {
                        LogManager.Debug("CheckPoint documenmt to check is null");
                        continue;
                    }
                    string ret = CheckXml(doc.DocumentElement);
                    if (!string.Equals(ret, doc.DocumentElement.InnerText))
                    {
                        failed = true;
                        node.InnerText = ret;
                    }
                }
                else if (item is ICheckData)
                {
                    string name = item.GetType().Name;
                    report.Set("name", "ICheckData数据" + name);
                    LogManager.Message("开始比对ICheckData数据 " + name);
                    ICheckData check = (ICheckData)item;
                    Cache cache = null;
                    if (!string.IsNullOrEmpty(DataBh))
                    {
                        cache = new Cache();
                        cache.Bh = DataBh;
                        cache.Get();
                    }
                    
                    universal result = check.Check(cache);
                    if (result)
                    {
                        report.SetResult(true);
                        report.Set("state", "S");
                        LogManager.Message(string.Format("自定义数据[{0}]比对成功", name));
                    }
                    else
                    {
                        failed = true;
                        check.Writeback(node);
                        report.SetResult(false);
                        report.Set("state", "F");
                        if (result == null)
                        {
                            report.Set("msg", "null");
                            LogManager.Message(string.Format("自定义数据[{0}]比对失败 ： {1}", name, result.StringValue));
                        }
                        else
                        {
                            report.Set("msg", result);
                            LogManager.Message(string.Format("自定义数据[{0}]比对失败 ： {1}", name, result.StringValue));
                        }
                    }
                    TestCasePool.SetReportDetail(report);
                }
                else
                {
                    string name = item.GetType().Name;
                    report.Set("name", "自定义数据" + name);
                    LogManager.Message("开始比对自定义数据 " + name);
                    try
                    {
                        string cacheString = string.Format("<{0}>{1}</{0}>", item.GetType().Name, DataFactory.GetCache(DataBh));
                        XmlDocument cDoc = new XmlDocument();
                        cDoc.LoadXml(cacheString);
                        object obj = DataFactory.GetData(cDoc);

                        universal result = DataFactory.Compare(obj, item, IsNegative);
                        if (result)
                        {
                            report.SetResult(true);
                            report.Set("state", "S");
                            LogManager.Message(string.Format("自定义数据[{0}]比对成功", name));
                        }
                        else
                        {
                            failed = true;
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                child.InnerText = GetValue(obj, child.Name);
                            }
                            report.SetResult(false);
                            report.Set("state", "F");
                            report.Set("msg", result);
                            LogManager.Message(string.Format("自定义数据[{0}]比对失败 ： {1}", name, result.StringValue));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        report.SetResult(false);
                        report.Set("state", "F");
                        report.Set("msg", ex.Message);
                        LogManager.Message(string.Format("自定义数据[{0}]比对失败 ： {1}\n{2}", name, ex.Message, ex.StackTrace));
                    }
                    TestCasePool.SetReportDetail(report);
                }
            }
            if (failed)
            {
                if (Config.RepairCheckPoint)
                {
                    Repair(newNode);
                }
            }
            LogManager.Message("比对结束");
        }

        private string GetValue(object obj, string name)
        {
            Type daoType = obj.GetType();
            try
            {
                PropertyInfo pi = daoType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi == null)
                {
                    //throw new CanNotFindPropertyException(daoType.Name, name);
                    return "NotFound";
                }
                object o = pi.GetValue(obj, null);
                if (o == null)
                {
                    return null;
                }
                return o.ToString();
            }
            catch (System.Exception ex)
            {
                LogManager.Warning(string.Format("Get {0}.{1} value exception {2}", daoType.Name, name, ex.Message));
                return "Error";
            }
        }

        private string CheckXml(XmlNode tnode)
        {
            string name = tnode.Name;
            ReportDetail report = new ReportDetail();
            report.Set("name", "比对XML节点" + name);
            LogManager.Message("开始比对XML节点 " + name);

            try
            {
                LogManager.Message("DataBh " + DataBh + " tnode.name " + tnode.Name);
                Cache cache = new Cache();
                cache.Bh = DataBh;
                cache.Get();
                //XmlNode node = null;
                string ret = tnode.InnerText;

                Dictionary<string, string> condition = new Dictionary<string, string>();
                if (tnode.Attributes.Count > 0)
                {
                    foreach (XmlAttribute xa in tnode.Attributes)
                    {
                        condition.Add(xa.Name, xa.Value);
                    }
                }
                try
                {
                    string cacheString = cache[name, condition];
                    if (cacheString == null)
                    {
                        cacheString = "";
                    }

                    string nodeString = tnode.InnerXml;
                    if (!IsStrict)
                    {
                        cacheString = DataFactory.GetFormatedValue(tnode.Name, cacheString);
                        nodeString = DataFactory.GetFormatedValue(tnode.Name, nodeString);
                    }

                    if (string.Equals(nodeString.Trim('\r'), cacheString.Trim('\r')))
                    {
                        if (IsNegative)
                        {
                            ret = cacheString;
                            LogManager.Message(string.Format("Compare {0} failed {1} == {2}, bytes {3} == {4}", tnode.Name, tnode.InnerXml, cacheString, ByteString(tnode.InnerXml), ByteString(cacheString)));
                            report.SetResult(false);
                            report.Set("state", "F");
                            report.Set("msg", string.Format("{0} == {1}", tnode.InnerXml, cacheString));
                            LogManager.Message(string.Format("XML节点[{0}]比对失败", name));
                        }
                        else
                        {
                            report.SetResult(true);
                            report.Set("state", "S");
                            LogManager.Message(string.Format("XML节点[{0}]比对成功", name));
                        }
                    }
                    else
                    {
                        if (IsNegative)
                        {
                            report.SetResult(true);
                            report.Set("state", "S");
                            LogManager.Message(string.Format("XML节点[{0}]比对成功", name));
                        }
                        else
                        {
                            ret = cacheString;
                            LogManager.Message(string.Format("Compare {0} failed {1} != {2}, bytes {3} != {4}", tnode.Name, tnode.InnerXml, cacheString, ByteString(tnode.InnerXml), ByteString(cacheString)));
                            report.SetResult(false);
                            report.Set("state", "F");
                            report.Set("msg", string.Format("{0} != {1}", tnode.InnerXml, cacheString));
                            LogManager.Message(string.Format("XML节点[{0}]比对失败", name));
                        }
                    }
                    //}
                }
                catch (System.Exception ex)
                {
                    if (IsNegative)
                    {
                        report.SetResult(true);
                        report.Set("state", "S");
                        LogManager.Message(string.Format("XML节点[{0}]比对成功", name));
                    }
                    else
                    {
                        ret = "NotFound";
                        report.SetResult(false);
                        report.Set("state", "F");
                        report.Set("msg", ex.Message);
                        LogManager.Message(string.Format("XML节点[{0}]比对失败 : {1}\n{2}", name, ex.Message, ex.StackTrace));
                    }
                }

                TestCasePool.SetReportDetail(report);
                return ret;
            }
            catch(System.Exception ex)
            {
                report.SetResult(false);
                report.Set("state", "F");
                report.Set("msg", ex.Message);
                TestCasePool.SetReportDetail(report);
                LogManager.Error(ex);
                return "NotFound";
            }
        }

        private string ByteString(string s)
        {
            return BitConverter.ToString(Encoding.Default.GetBytes(s));
        }

        private void WriteDao(Dao dao, XmlNode xn)
        {
            Type type = dao.GetType();
            foreach (XmlNode tmp in xn.ChildNodes)
            {
                if (tmp is XmlComment)
                {
                    continue;
                }
                PropertyInfo pi = type.GetProperty(tmp.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (pi == null)
                {
                    LogManager.Error("Can not find property named " + tmp.Name);
                    //throw new InvalidDataTypeException(xn.Name);
                    continue;
                }
                try
                {
                    object obj = pi.GetValue(dao, null);
                    if (obj == null)
                    {
                        tmp.InnerText = "";
                    }
                    else
                    {
                        tmp.InnerText = obj.ToString();
                    }
                }
                catch (System.Exception ex)
                {
                    LogManager.Error(xn.Name + " SetProperty " + tmp.Name + " occur exception : " + ex.Message);
                }
            }
        }

        private universal CompareDao(Dao dao, XmlNode xn)
        {
            Type type = dao.GetType();
            StringBuilder sb = new StringBuilder();
            foreach (XmlNode tmp in xn.ChildNodes)
            {
                if (tmp is XmlComment) continue;
                PropertyInfo pi = type.GetProperty(tmp.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (pi == null)
                {
                    throw new CanNotFindPropertyException(type.Name, tmp.Name);
                }
                try
                {
                    //LogManager.Debug(string.Format("{0} set {1} = {2}", xn.Name, tmp.Name, tmp.InnerText));
                    universal ret;
                    if (IsStrict)
                    {
                        ret = DataFactory.Compare(tmp.InnerText, pi.GetValue(dao, null), IsNegative);
                    }
                    else
                    {
                        ret = DataFactory.Compare(DataFactory.GetFormatedValue(tmp.Name, tmp.InnerText), DataFactory.GetFormatedValue(tmp.Name, pi.GetValue(dao, null)), IsNegative);
                    }

                    if (!ret)
                    {
                        if (IsNegative)
                        {
                            sb.AppendFormat("Compare {0}.{1} failed value is {2}\n", type.Name, tmp.Name, tmp.InnerText);
                        }
                        else
                        {
                            string preResult = tmp.InnerText;
                            object executeResult = pi.GetValue(dao, null);

                            if ((executeResult == null
                                || string.IsNullOrEmpty(executeResult.ToString().Trim()))
                                && string.IsNullOrEmpty(preResult))
                            {
                                continue;
                            }

                            if (string.Equals(preResult, executeResult.ToString().Trim()))
                            {
                                continue;
                            }
                            sb.AppendFormat("Compare {0}.{1} failed {2} != {3} bytes {4} != {5}\n", type.Name, tmp.Name, preResult, executeResult, ByteString(preResult), ByteString(executeResult.ToString()));
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    sb.AppendFormat("{0} SetProperty {1} occur exception : {2}\n", xn.Name, tmp.Name, ex.Message);
                    LogManager.Error(xn.Name + " SetProperty " + tmp.Name + " occur exception : " + ex.Message);
                }
            }

            return sb.ToString().Trim();
        }

        private void InitDao(Dao dao, XmlNode xn)
        {
            Type type = dao.GetType();
            foreach (XmlNode tmp in xn.ChildNodes)
            {
                if (tmp is XmlComment)
                {
                    continue;
                }
                PropertyInfo pi = type.GetProperty(tmp.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (pi == null)
                {
                    throw new CanNotFindPropertyException(type.Name, tmp.Name);
                }
                try
                {
                    LogManager.Debug(string.Format("{0} set {1} = {2}", xn.Name, tmp.Name, tmp.InnerText));
                    string value = tmp.InnerText;
                    if (!IsStrict)
                    {
                        value = DataFactory.GetFormatedValue(tmp.Name, value);
                    }

                    if (pi.PropertyType.Equals(typeof(universal)))
                    {
                        pi.SetValue(dao, new universal(value), null);
                    }
                    else
                    {
                        pi.SetValue(dao, Convert.ChangeType(value, pi.PropertyType), null);
                    }
                }
                catch (System.Exception ex)
                {
                    LogManager.Error(xn.Name + " SetProperty " + tmp.Name + " occur exception : " + ex.Message);
                }
            }
        }

        private void GetDao(Dao dao, XmlNode xn)
        {
            Type type = dao.GetType();
            List<string> primaryKeyList = dao.GetPrimaryKeys();

            //bool primaryKeyNotSet = false;
            foreach (string primaryKey in primaryKeyList)
            {
                LogManager.Debug("Set primary key " + primaryKey);
                PropertyInfo pi = type.GetProperty(primaryKey, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (pi == null)
                {
                    throw new CanNotFindPropertyException(type.Name, primaryKey);
                }
                try
                {
                    XmlNode tmp = XmlFactory.SelectSingleNodeIgnoreCase(xn, primaryKey);
                    pi.SetValue(dao, tmp.InnerText, null);
                }
                catch
                {
                    LogManager.Debug(string.Format("CheckPoint not set primarykey {0}", primaryKey));
                    try
                    {
                        string value = DataFactory.GetCache(DataBh, primaryKey);
                        pi.SetValue(dao, value, null);
                    }
                    catch (CanNotFindDataException)
                    {
                        LogManager.Debug(string.Format("Cache not set primarykey {0}", primaryKey));
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.Message("CheckPoint SetProperty " + primaryKey + " : " + ex.Message);
                    }
                }

                object v = pi.GetValue(dao, null);
                if (v == null || string.Equals(v.ToString(), ""))
                {
                    LogManager.Error(string.Format("Property named [{0}] is an affirmatively setting item", primaryKey));
                    //primaryKeyNotSet = true;
                    throw new PrimaryKeyIsNullException(primaryKey);
                }
            }

            dao.SelectSingle();
        }

        private void Repair(XmlNode newNode)
        {
            if(string.IsNullOrEmpty(filePath)) return;
            XmlDocument doc = new XmlDocument();
            if (File.Exists(filePath))
            {
                doc.Load(filePath);
            }
            else
            {
                if(!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                doc.Load(origPath);
            }

            XmlNode oldNode = doc.SelectSingleNode(string.Format("//CheckPoint[Bh='{0}']", Bh));
            XmlNode tmpNode = doc.ImportNode(newNode, true);
            oldNode.ParentNode.ReplaceChild(tmpNode, oldNode);

            doc.Save(filePath);
        }

        public void setProperty(XmlNode propertyNode)
        {

        }

        public override IFormatData FromXml(System.Xml.XmlNode doc, Dictionary<string, string> condition)
        {
            origPath = FileFactory.GetRealPath(doc.OwnerDocument.BaseURI);
            if (string.IsNullOrEmpty(origPath))
            {
                filePath = null;
            }
            else
            {
                filePath = origPath.Substring(origPath.IndexOf("CheckData") + 10);
                filePath = System.IO.Path.Combine(Config.WorkingDirectory, "Data\\NewCheck", filePath);
            }
            string conditionString = DataFactory.GetXmlConditionString("CheckPoint", condition);
            
            XmlNode dbNode = doc.SelectSingleNode(conditionString);
            if (dbNode == null)
            {
                throw new CanNotFindDataException(conditionString);
            }
            originalString = dbNode.OuterXml;
            XmlNode bhNode = dbNode.SelectSingleNode("DataBh");
            if (bhNode != null)
            {
                DataBh = bhNode.InnerText;
            }
            int index = -1;
            foreach (XmlNode xn in dbNode.ChildNodes)
            {
                index++;
                if (xn is XmlComment)
                {
                    continue;
                }
                string name = xn.Name;
                if (string.Equals(name, "Bh"))
                {
                    Bh = xn.InnerText;
                }
                else if (string.Equals(name, "DataBh"))
                {
                    DataBh = xn.InnerText;
                }
                else if (string.Equals(name, "IsNegative"))
                {
                    try
                    {
                        IsNegative = bool.Parse(xn.InnerText);
                    }
                    catch
                    {
                        IsNegative = false;
                    }
                }
                else if (string.Equals(name, "Cache"))
                {
                    Cache cache = DataFactory.XmlToObject<Cache>(xn);
                    _checkObjectList.Add(new CheckObject(index, cache));
                }
                else if (string.Equals(name, "DbData"))
                {
                    DbData dbData = DataFactory.XmlToObject<DbData>(xn);
                    _checkObjectList.Add(new CheckObject(index, dbData));
                }
                else
                {
                    if (!XmlFactory.HasChild(xn))
                    {
                        XmlDocument docCheck = new XmlDocument();
                        docCheck.LoadXml(xn.OuterXml);
                        LogManager.Debug(string.Format("CheckPoint FromXml {0} set XmlDocument = {1}", xn.Name, xn.OuterXml));
                        _checkObjectList.Add(new CheckObject(index, docCheck));
                        continue;
                    }

                    ClassInfo ci = new ClassInfo();
                    try
                    {
                        ci = DataFactory.GetClassInfo(xn.Name);
                    }
                    catch (System.Exception)
                    {
                        //LogManager.ErrorOnlyPrint(ex);
                        XmlDocument docCheck = new XmlDocument();
                        docCheck.LoadXml(xn.OuterXml);
                        LogManager.Debug(string.Format("CheckPoint FromXml {0} set XmlDocument = {1}", xn.Name, xn.OuterXml));
                        _checkObjectList.Add(new CheckObject(index, docCheck));
                        continue;
                    }

                    Type type = ci.Type;

                    LogManager.DebugFormat("Get type named {0}", type.Name);
                    object obj;
                    try
                    {
                        obj = Activator.CreateInstance(type);
                    }
                    catch (System.Exception ex1)
                    {
                        LogManager.Error("Create instance exception: " + ex1.Message + "\n" + ex1.StackTrace);
                        if (ex1.InnerException != null)
                        {
                            LogManager.Error(ex1.InnerException.Message + "\n" + ex1.InnerException.StackTrace);
                        }
                        throw;
                    }
                    if (obj == null)
                    {
                        LogManager.Message("Create instance of " + type.Name + " failed.");
                        continue;
                    }
                    if (obj is Dao)
                    {
                        Dao dao = obj as Dao;
                        if (dao == null)
                        {
                            LogManager.Message("Change to Dao failed");
                            continue;
                        }

                        XmlAttribute ownerAttr = xn.Attributes["Owner"];
                        if (ownerAttr != null)
                        {
                            if (!string.IsNullOrEmpty(ownerAttr.Value))
                            {
                                dao.Owner = ownerAttr.Value;
                            }
                        }
                    }
                    else
                    {
                        string cacheString = "";
                        try
                        {
                            Cache cache = new Cache();
                            cache.Bh = DataBh;
                            cache.Get();
                            cacheString = cache.ToString();
                        }
                        catch
                        {
                            cacheString = "";
                        }
                        XmlDocument cDoc = new XmlDocument();
                        cDoc.LoadXml(string.Format("<{0}>{1}</{0}>", xn.Name, cacheString));
                        obj = DataFactory.GetData(cDoc);
                        foreach (XmlNode tmp in xn.ChildNodes)
                        {
                            if (tmp is XmlComment)
                            {
                                continue;
                            }
                            PropertyInfo pi = type.GetProperty(tmp.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                            if (pi == null)
                            {
                                LogManager.Message("Can not find property named " + tmp.Name);
                                //throw new InvalidDataTypeException(xn.Name);
                                continue;
                            }
                            try
                            {
                                LogManager.Debug(string.Format("CheckPoint FromXml {0} set {1} = {2}", xn.Name, tmp.Name, tmp.InnerText));
                                pi.SetValue(obj, tmp.InnerText, null);
                            }
                            catch (System.Exception ex)
                            {
                                LogManager.Error(xn.Name + " SetProperty " + tmp.Name + " : " + ex.Message);
                            }
                        }
                    }

                    _checkObjectList.Add(new CheckObject(index, obj));
                }
            }
            return (IFormatData)this;
        }

        public override string ToXml()
        {
            return "";
        }

        public override void Init()
        {
            
        }
    }
}
