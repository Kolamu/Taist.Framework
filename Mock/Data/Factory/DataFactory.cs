namespace Mock.Data
{
    using System;
    using System.Reflection;
    using System.Xml;
    using Mock.Data.Exception;
    using Mock.Data.TaistDataCenter;
    using System.Linq;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using Mock.Data.Attributes;
    using Mock.Tools.Exception;
    using System.IO.Compression;

    /// <summary>
    /// 表示自动化测试数据工厂对象
    /// </summary>
    public class DataFactory
    {
        internal static readonly List<byte> ConstantUrlCode = new List<byte>() { 33, 40, 41, 42, 45, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 95, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122 };

        #region 数据缓存
        private static object libLock = new object();
        private static Dictionary<string, Keyword> keyWordInfo = null;
        private static Dictionary<string, Keyword> defaultKeywordInfo = null;
        private static Dictionary<string, Keyword> firstKeywordInfo = null;
        private static Dictionary<string, ClassInfo> classInfo = new Dictionary<string,ClassInfo>();
        #endregion

        #region 初始化信息
        static DataFactory()
        {
            try
            {
                List<string> fileNameList = FileFactory.GetAllFileNames(Config.WorkingDirectory, ".dll");

                Assembly assembly = null;
                foreach (string fileName in fileNameList)
                {
                    try
                    {
                        //2017.10.12 解决部分系统出现加载失败的问题
                        byte[] buffer = System.IO.File.ReadAllBytes(fileName);
                        //Load assembly using byte array
                        assembly = Assembly.Load(buffer);
                        //assembly = Assembly.LoadFrom(fileName);
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }

                    if (assembly == null)
                    {
                        continue;
                    }

                    object[] caList = assembly.GetCustomAttributes(typeof(DataAssemblyAttribute), false);

                    if (caList != null && caList.Length > 0)
                    {
                        SaveDataAssemblyType(assembly);
                    }

                    caList = assembly.GetCustomAttributes(typeof(BusinessAssemblyAttribute), false);
                    if (caList != null && caList.Length > 0)
                    {
                        SaveKeyWord(assembly);
                    }
                }

                assembly = Assembly.GetEntryAssembly();
                SaveKeyWord(assembly);
                SaveDataAssemblyType(assembly);

                try
                {
                    checkFormatDocument = XmlFactory.LoadXml(Path.Combine(Config.WorkingDirectory, @"Data\Context\FormatValue.xml"));
                }
                catch
                {
                    checkFormatDocument = XmlFactory.LoadXml(XmlFactory.XmlRootString);
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex);
            }
        }

        /// <summary>
        /// 获取所有关键字
        /// </summary>
        /// <param name="assembly"></param>
        private static void SaveKeyWord(Assembly assembly)
        {
            try
            {
                if (keyWordInfo == null)
                {
                    keyWordInfo = new Dictionary<string, Keyword>();
                }
                if (defaultKeywordInfo == null)
                {
                    defaultKeywordInfo = new Dictionary<string, Keyword>();
                }
                if (firstKeywordInfo == null)
                {
                    firstKeywordInfo = new Dictionary<string, Keyword>();
                }
                Type[] typeArray = assembly.GetTypes();
                foreach (Type type in typeArray)
                {
                    object[] attributesArray = type.GetCustomAttributes(typeof(BusinessClassAttribute), false);
                    if (attributesArray == null || attributesArray.Length == 0)
                    {
                        continue;
                    }

                    MethodInfo[] miArray = type.GetMethods();
                    foreach (MethodInfo mi in miArray)
                    {
                        object[] mAttrArray = mi.GetCustomAttributes(typeof(BusinessMethodAttribute), false);
                        if (mAttrArray == null || mAttrArray.Length == 0)
                        {
                            continue;
                        }

                        BusinessMethodAttribute bma = mAttrArray[0] as BusinessMethodAttribute;
                        if (keyWordInfo.ContainsKey(bma))
                        {
                            //Dictionary<string, KeywordInfo> subKeyInfo = keyWordInfo[bma.Keywords];
                            //Keyword kw = keyWordInfo[bma];
                            //SubKeyword ki = null;
                            //if (mi.IsStatic)
                            //{
                            //    ki = new SubKeyword(bma.Mode, null, mi);
                            //}
                            //else
                            //{
                            //    ki = new SubKeyword(bma.Mode, Activator.CreateInstance(type), mi);
                            //}
                            //kw[bma.Mode] = ki;
                            //throw new NotUniqueDataException(bma.Keywords, bma.SubKeyword);
                            LogManager.DebugFormat("关键字{0}重复", (string)bma);
                            continue;
                        }
                        else
                        {
                            Keyword kw = null;
                            if (mi.IsStatic)
                            {
                                kw = new Keyword(bma, null, mi);
                            }
                            else
                            {
                                kw = new Keyword(bma, Activator.CreateInstance(type), mi);
                            }
                            keyWordInfo.Add(bma, kw);
                            if (bma.Default)
                            {
                                if (defaultKeywordInfo.ContainsKey(bma.Keywords))
                                {
                                    LogManager.DebugFormat("The data {0}[{1}] is not unique.", bma.Keywords, "default");
                                }
                                else
                                {
                                    defaultKeywordInfo.Add(bma.Keywords, kw);
                                }
                            }

                            if (!firstKeywordInfo.ContainsKey(bma.Keywords))
                            {
                                firstKeywordInfo.Add(bma.Keywords, kw);
                            }
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex);
            }
        }

        /// <summary>
        /// 存储数据类的类信息
        /// </summary>
        /// <param name="assembly"></param>
        private static void SaveDataAssemblyType(Assembly assembly)
        {
            try
            {
                Type[] typeArray = null;
                try
                {
                    typeArray = assembly.GetTypes();
                }
                catch (System.Exception ex)
                {
                    LogManager.Error("The data assembly file {0} is invalid. {1}\n{2}", assembly.Location, ex.Message, ex.StackTrace);
                    //throw new InvalidDataAssemblyFileException(assembly.Location, ex);
                    return;
                }
                foreach (Type type in typeArray)
                {
                    ClassInfo ci = new ClassInfo();
                    ci.Name = type.Name;
                    ci.ClassName = type.FullName;
                    ci.Assembly = assembly;
                    //ci.Save();
                    classInfo.Add(ci.Name, ci);
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex);
            }
        }
        #endregion

        internal static ClassInfo GetClassInfo(string name)
        {
            if (classInfo.ContainsKey(name))
            {
                return classInfo[name];
            }
            else if (classInfo.ContainsKey(name.ToUpper()))
            {
                return classInfo[name.ToUpper()];
            }
            else if(classInfo.ContainsKey(name.ToLower()))
            {
                return classInfo[name.ToLower()];
            }
            else
            {
                foreach (KeyValuePair<string, ClassInfo> kv in classInfo)
                {
                    if (string.Equals(kv.Key, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return kv.Value;
                    }
                }
                throw new CanNotFindDataException(name);
            }
        }

        private static XmlDocument checkFormatDocument = null;
        /// <summary>
        /// 获取预配置中的格式化数据
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFormatedValue(string keyName, object obj)
        {
            XmlNodeList formatNodeList = checkFormatDocument.SelectNodes(string.Format("//FormatValue[KeyName='{0}']", keyName));

            foreach (XmlNode formatNode in formatNodeList)
            {
                FormatValue fv = XmlToObject<FormatValue>(formatNode);
                fv.Value = obj;
                string formatedValue = fv.FormatedValue;
                if (formatedValue != null)
                {
                    LogManager.DebugFormat("Formated value {0}", formatedValue);
                    return formatedValue;
                }
            }

            if (obj == null) return null;
            return obj.ToString();
        }

        /// <summary>
        /// 深拷贝一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CopyObject<T>(T source)
        {
            T target = default(T);
            CopyObject<T>(source, ref target);
            return target;
        }

        /// <summary>
        /// 深拷贝一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyObject<T>(T source, ref T target)
        {
            if (source == null)
            {
                target = default(T);
                return;
            }

            if (source is string || source.GetType().IsValueType)
            {
                target = source;
                return;
            }

            if (source is universal)
            {
                target = (T)(object)new universal(((universal)(object)source).ObjValue);
                return;
            }

            
            Type sourceType = GetTypeFromInstance(typeof(T), source);

            if (target == null)
            {
                target = (T)CreateInstance(sourceType);
            }
            Type targetType = GetTypeFromInstance(typeof(T), target);
            
            if (sourceType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
            {
                MethodInfo mi = sourceType.GetMethod("GetEnumerator");
                System.Collections.IEnumerator enumerator = (System.Collections.IEnumerator)mi.Invoke(source, null);

                MethodInfo addMethod = targetType.GetMethod("Add");

                while (enumerator.MoveNext())
                {
                    addMethod.Invoke(target, new object[] { CopyObject(enumerator.Current) });
                }
            }
            else
            {
                FieldInfo[] sourceFieldArry = sourceType.GetFields(
                      BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.Static);
                foreach (FieldInfo sourceField in sourceFieldArry)
                {
                    BindingFlags flags = BindingFlags.Instance;
                    if (sourceField.IsPublic)
                    {
                        flags |= BindingFlags.Public;
                    }
                    else
                    {
                        flags |= BindingFlags.NonPublic;
                    }

                    if (sourceField.IsStatic)
                    {
                        flags |= BindingFlags.Static;
                    }
                    FieldInfo targetField = targetType.GetField(sourceField.Name, flags);
                    try
                    {
                        targetField.SetValue(target, CopyObject(sourceField.GetValue(source)));
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 将<c>Dictionary</c>类型转化为<c>string</c>类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static string GetXmlConditionString(string typeName, Dictionary<string, string> conditions)
        {
            string conditionString = string.Empty;
            if (conditions == null || conditions.Count < 1)
            {
                //return null;
                conditionString = string.Format("//{0}", typeName);
            }
            else
            {
                List<string> keys = conditions.Keys.ToList<string>();
                foreach (string key in keys)
                {
                    if (conditionString == string.Empty)
                    {
                        conditionString = string.Format("//{2}[{0}='{1}']", key, conditions[key], typeName);
                    }
                    else
                    {
                        conditionString = string.Format("{0} and {1}='{2}']", conditionString.TrimEnd(']'), key, conditions[key]);
                    }
                }
            }
            return conditionString;
        }

        #region object <-> xml
        /// <summary>
        /// 按照格式化数据格式实例化对应类型的实例
        /// </summary>
        /// <typeparam name="T">格式化数据对应的类型</typeparam>
        /// <param name="node">格式化数据</param>
        /// <param name="instance">对应类型的实例的初始值</param>
        /// <returns>对应类型的实例</returns>
        public static T XmlToObject<T>(XmlNode node, T instance = default(T))
        {
            XmlNode objectXmlNode = InitXmlNode(node);
            TaistType objType = GetRealType(typeof(T), objectXmlNode.Name, instance);
            return (T)XmlToObject(objectXmlNode, objType);
        }

        /// <summary>
        /// 按照格式化数据格式实例化对应类型的实例
        /// </summary>
        /// <param name="objectNode">格式化数据</param>
        /// <param name="objectType">格式化数据对应的类型</param>
        /// <returns>对应类型的实例</returns>
        public static object XmlToObject(XmlNode objectNode, Type objectType = null)
        {
            XmlNode objectXmlNode = InitXmlNode(objectNode);
            TaistType objType = GetRealType(objectType, objectXmlNode.Name, null);
            return XmlToObject(objectXmlNode, objType);
        }

        /// <summary>
        /// 按照格式化数据格式实例化对应类型的实例
        /// </summary>
        /// <param name="objectNode">格式化数据</param>
        /// <param name="instance">格式化数据对应的类型</param>
        /// <returns>对应类型的实例</returns>
        public static object XmlToObject(XmlNode objectNode, object instance)
        {
            XmlNode objectXmlNode = InitXmlNode(objectNode);
            TaistType objType = null;
            if (instance == null)
            {
                objType = GetRealType(null, objectXmlNode.Name, instance);
            }
            else
            {
                objType = TaistType.GetDataType(instance);
            }
            return XmlToObject(objectXmlNode, objType);
        }
        
        /// <summary>
        /// 按照格式化数据格式实例化对应类型的实例
        /// </summary>
        /// <typeparam name="T">格式化数据类型</typeparam>
        /// <param name="bh">格式化数据的编号</param>
        /// <returns>对应类型的实例</returns>
        public static T XmlToObject<T>(string bh)
        {
            TaistType objType = GetRealType(typeof(T), null, null);
            XmlNode dataNode = TestCasePool.GetDataNode(bh);
            if (objType.Instance is IFormatData)
            {
                IFormatData ret = (IFormatData)objType.Instance;
                ret = ret.FromXml(NodeStandalone(dataNode), null);
                return (T)(object)ret;
            }
            else
            {
                return XmlToObject<T>(dataNode, (T)objType.Instance);
            }
        }

        /// <summary>
        /// 获取数据基础方法
        /// </summary>
        /// <param name="objectXmlNode"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static object XmlToObject(XmlNode objectXmlNode, TaistType dataType)
        {
            if(objectXmlNode == null)
                return dataType.Instance;

            dataType = GetDefaultData(objectXmlNode, dataType);

            for (int i = 0; i < objectXmlNode.ChildNodes.Count; i++)
            {
                XmlNode n = objectXmlNode.ChildNodes[i];
                if (n is XmlComment) continue;
                //获取属性
                try
                {
                    TaistProperty dataProperty = dataType.GetProperty(n.Name);
                    if (dataProperty.Ignore) continue;
                    dataProperty.Set(n.InnerText);
                }
                catch (CanNotFindPropertyException)
                {
                    dataType.CallClassSetPropertyFunction(n);
                }
            }

            for (int i = 0; i < objectXmlNode.Attributes.Count; i++)
            {
                XmlAttribute xa = objectXmlNode.Attributes[i];

                try
                {
                    TaistProperty dataProperty = dataType.GetProperty(xa.Name);
                    if (dataProperty.Ignore) continue;
                    dataProperty.Set(xa.Value);
                }
                catch (CanNotFindPropertyException) { }
            }
            return dataType.Instance;
        }

        /// <summary>
        /// 将数据实例转化为格式化的数据
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static string ObjectToXml(object instance, bool inherit = false)
        {
            string xml = string.Empty;
            if (instance == null)
            {
                return xml;
            }
            
            Type objType = instance.GetType();
            string rootName = objType.Name.Replace("Object", "").Trim();

            object[] cattr = objType.GetCustomAttributes(typeof(DataClassAttribute), false);

            if (cattr.Length > 0)
            {
                DataClassAttribute dca = (DataClassAttribute)cattr[0];
                rootName = dca.Name;
            }

            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement(rootName);

            PropertyInfo[] pInfoArray = objType.GetProperties();
            foreach (PropertyInfo pinfo in pInfoArray)
            {
                if (!inherit && pinfo.DeclaringType != objType) continue;
                object[] attrs = pinfo.GetCustomAttributes(typeof(FieldPropertyAttribute), false);

                string pName = pinfo.Name;
                if (attrs.Length > 0)
                {
                    FieldPropertyAttribute fpa = attrs[0] as FieldPropertyAttribute;
                    if (fpa.isIgnore) continue;

                    if (!string.IsNullOrEmpty(fpa.Name))
                    {
                        pName = fpa.Name;
                    }
                    if (fpa.isAttribute)
                    {
                        object pVal = pinfo.GetValue(instance, null);
                        if (pVal != null)
                        {
                            XmlAttribute attr = doc.CreateAttribute(pName);
                            attr.Value = pinfo.GetValue(instance, null).ToString();
                            element.Attributes.Append(attr);
                        }
                    }
                    else
                    {
                        XmlElement tmp = doc.CreateElement(pName);
                        object val = pinfo.GetValue(instance, null);
                        if (val == null)
                        {
                            tmp.InnerText = "";
                        }
                        else
                        {
                            tmp.InnerText = pinfo.GetValue(instance, null).ToString();
                        }
                        element.AppendChild(tmp);
                    }
                }
                else
                {
                    XmlElement tmp = doc.CreateElement(pinfo.Name);
                    object val = pinfo.GetValue(instance, null);
                    if (val == null)
                    {
                        tmp.InnerText = "";
                    }
                    else
                    {
                        tmp.InnerText = pinfo.GetValue(instance, null).ToString();
                    }
                    element.AppendChild(tmp);
                }
            }

            MethodInfo mi = objType.GetMethod("getProperty");
            if (mi != null)
            {
                object obj = mi.Invoke(instance, null);
                Dictionary<string, string> propertyDic = obj as Dictionary<string, string>;
                if (propertyDic != null)
                {
                    foreach (string name in propertyDic.Keys)
                    {
                        PropertyInfo pinfo = objType.GetProperty(name);
                        if (pinfo == null)
                        {
                            XmlElement tmp = doc.CreateElement(name);
                            string value = propertyDic[name];

                            if (value == null)
                            {
                                tmp.InnerXml = null;
                            }
                            else
                            {
                                if (value.StartsWith("<") && value.EndsWith(">"))
                                {
                                    try
                                    {
                                        XmlDocument td = new XmlDocument();
                                        td.LoadXml(string.Format("<Data>{0}</Data>", value));
                                        tmp.InnerXml = value;
                                    }
                                    catch
                                    {
                                        tmp.InnerText = value;
                                    }
                                }
                                else
                                {
                                    tmp.InnerText = value;
                                }
                            }
                            element.AppendChild(tmp);
                        }
                    }
                }
            }
            doc.AppendChild(element);
            xml = doc.OuterXml;
            return xml;
        }

        /// <summary>
        /// 将数据实例转化为格式化的数据
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="dataTemplate"></param>
        /// <returns></returns>
        public static string ObjectToXml(object instance, string dataTemplate)
        {
            XmlDocument oriDocument = new XmlDocument();
            if (instance is IFormatData)
            {
                oriDocument.LoadXml(((IFormatData)instance).ToXml());
            }
            else
            {
                oriDocument.LoadXml(ObjectToXml(instance));
            }

            XmlDocument templateDocument = XmlFactory.LoadXml(dataTemplate);

            

            return null;
        }

        public void SetValueTo(XmlNode node, XmlDocument oriDoc)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                XmlAttribute xa = xn.Attributes["$"];
                if (xa == null)
                {

                }
                if (xn.ChildNodes.Count == 0)
                {
                    Console.WriteLine("child node is 0");
                }
                else if (xn.ChildNodes.Count == 1)
                {
                    
                }
                else
                {
                    SetValueTo(xn, oriDoc);
                }
            }
        }
        #endregion

        #region 比较对象
        /// <summary>
        /// 比较两个数据是否相同(该函数只比对基本数据类型及string类型的属性)
        /// </summary>
        /// <param name="obj1">比较对象1</param>
        /// <param name="obj2">比较对象2</param>
        /// <param name="isNegative">反向比对</param>
        /// <returns>比对结果</returns>
        public static universal Compare(object obj1, object obj2, bool isNegative = false)
        {
            if (obj1 == null)
            {
                if (obj2 == null)
                {
                    if (isNegative)
                    {
                        return "Compare value failed both value is null";
                    }
                    return true;
                }
                else
                {
                    if (isNegative)
                    {
                        return true;
                    }
                    return string.Format("Compare value failed null != {0}", obj2);
                }
            }

            //obj1 不为null 执行下面的代码
            if (obj2 == null)
            {
                if (isNegative)
                {
                    return true;
                }
                return string.Format("Compare value failed {0} != null", obj1);
            }

            Type T1 = obj1.GetType();
            Type T2 = obj2.GetType();
            if (!T1.Equals(T2))
            {
                if (isNegative)
                {
                    return true;
                }
                return string.Format("Type not equal {0} != {1}", T1.Name, T2.Name);
            }

            if (T1 == typeof(string))
            {
                //部分返回结果中会存在'\r'字符，因此去掉
                if (string.Equals(obj1.ToString().Trim('\r'), obj2.ToString().Trim('\r')))
                {
                    if (isNegative)
                    {
                        return string.Format("Compare string failed {0} == {1}", obj1, obj2);
                    }
                    return true;
                }
                else
                {
                    if (isNegative)
                    {
                        return true;
                    }
                    return string.Format("Compare string failed {0} != {1}", obj1, obj2);
                }
            }

            if (T1.IsValueType && T1.IsPrimitive)
            {
                if (obj1.Equals(obj2))
                {
                    if (isNegative)
                    {
                        return string.Format("Compare number failed {0} == {1}", obj1, obj2);
                    }
                    return true;
                }
                else
                {
                    if (isNegative)
                    {
                        return true;
                    }
                    return string.Format("Compare number failed {0} != {1}", obj1, obj2);
                }
            }

            List<string> resultList = new List<string>();
            PropertyInfo[] pinfos = T1.GetProperties();
            foreach (PropertyInfo pinfo in pinfos)
            {
                object[] paArray = pinfo.GetCustomAttributes(typeof(FieldPropertyAttribute), true);
                if (paArray != null && paArray.Length != 0)
                {
                    FieldPropertyAttribute pa = paArray[0] as FieldPropertyAttribute;
                    if (pa.isIgnore)
                    {
                        continue;
                    }
                }
                if (!pinfo.PropertyType.IsPrimitive && !string.Equals("string", pinfo.PropertyType.Name.ToLower()))
                {
                    LogManager.DebugFormat(string.Format("Property type[{0}] is not primitive and string", pinfo.PropertyType.Name), NoteType.WARNING);
                    continue;
                }
                object val1 = pinfo.GetValue(obj1, null);
                object val2 = pinfo.GetValue(obj2, null);
                if (pinfo.PropertyType == typeof(string))
                {
                    string v1 = val1 as string;
                    if (v1 != null)
                    {
                        v1 = v1.Trim('\r');
                    }
                    string v2 = val2 as string;
                    if (v2 != null)
                    {
                        v2 = v2.Trim('\r');
                    }

                    if (string.IsNullOrEmpty(v1))
                    {
                        if (!string.IsNullOrEmpty(v2))
                        {
                            if (!isNegative)
                            {
                                resultList.Add(string.Format("Compare {3}.{0} failed {1} != {2}", pinfo.Name, val1, val2, T1.Name));
                            }
                        }
                        else
                        {
                            if (isNegative)
                            {
                                resultList.Add(string.Format("Compare {3}.{0} failed {1} == {2}", pinfo.Name, val1, val2, T1.Name));
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(v2))
                        {
                            if (!isNegative)
                            {
                                resultList.Add(string.Format("Compare {3}.{0} failed {1} != {2}", pinfo.Name, val1, val2, T1.Name));
                            }
                        }
                        else
                        {
                            if (!string.Equals(v1, v2))
                            {
                                if (!isNegative)
                                {
                                    resultList.Add(string.Format("Compare {3}.{0} failed {1} != {2}", pinfo.Name, val1, val2, T1.Name));
                                }
                            }
                            else
                            {
                                if(isNegative)
                                {
                                    resultList.Add(string.Format("Compare {3}.{0} failed {1} == {2}", pinfo.Name, val1, val2, T1.Name));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (val1 != null && !val1.Equals(""))
                    {
                        if (!val1.Equals(val2))
                        {
                            if (!isNegative)
                            {
                                resultList.Add(string.Format("Compare {3}.{0} failed {1} != {2}", pinfo.Name, val1, val2, T1.Name));
                            }
                        }
                        else
                        {
                            if (isNegative)
                            {
                                resultList.Add(string.Format("Compare {3}.{0} failed {1} == {2}", pinfo.Name, val1, val2, T1.Name));
                            }
                        }
                    }
                    else
                    {
                        if (val2 != null && !val2.Equals(""))
                        {
                            if (!isNegative)
                            {
                                resultList.Add(string.Format("Compare {3}.{0} failed {1} != {2}", pinfo.Name, val1, val2, T1.Name));
                            }
                        }
                        else
                        {
                            if (isNegative)
                            {
                                resultList.Add(string.Format("Compare {3}.{0} failed {1} == {2}", pinfo.Name, val1, val2, T1.Name));
                            }
                        }
                    }
                }
            }
            if (resultList.Count < 1)
            {
                return true;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (string result in resultList)
                {
                    sb.AppendLine(result);
                    LogManager.Message(result);
                }
                return sb.ToString();
            }
        }

        #endregion

        #region 获取数据
        public static object GetData(XmlNode dataXmlNode, TaistType objType = null)
        {
            XmlNode dataNode = InitXmlNode(dataXmlNode);

            if (objType == null)
            {
                objType = GetRealType(null, dataNode.Name, null);
            }
            if(objType.Instance is IFormatData)
            {
                IFormatData ret = (IFormatData)objType.Instance;
                ret = ret.FromXml(NodeStandalone(dataNode), null);
                if (ret != null)
                {
                    ret.Init();
                }
                return ret;
            }
            else
            {
                object ret = XmlToObject(dataNode, objType);
                return ret;
            }
        }

        /// <summary>
        /// 获取数据的默认数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetData<T>()
        {
            T obj = CreateInstance<T>();

            if (obj is IFormatData)
            {
                IFormatData ret = (IFormatData)(object)obj;
                if (ret != null)
                {
                    ret.Init();
                }
                return (T)(object)ret;
            }
            else
            {
                return (T)obj;
            }
        }

        /// <summary>
        /// 获取指定类型的数据对象实例
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="node">数据所在的XmlDocument实例</param>
        /// <returns>指定类型数据的对象实例</returns>
        public static T GetData<T>(XmlNode node)
        {
            return GetData<T>(node, new Dictionary<string, string>());
        }

        /// <summary>
        /// 获取指定类型的数据对象实例
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="doc">数据所在的XmlDocument实例</param>
        /// <param name="bh">数据编号</param>
        /// <returns>指定类型数据的对象实例</returns>
        public static T GetData<T>(XmlNode doc, string bh)
        {
            //LogManager.Debug("GetData " + bh);
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("Bh", bh);
            T obj = GetData<T>(doc, args);
            return obj;
        }

        /// <summary>
        /// 获取指定类型的数据对象实例
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="objNode">数据的Xml格式文档</param>
        /// <param name="args">构造方法的参数</param>
        /// <returns>指定类型数据的对象实例</returns>
        public static T GetData<T>(XmlNode objNode, Dictionary<string, string> args)
        {
            XmlNode node = InitXmlNode(objNode);
            TaistType objType = GetRealType(typeof(T), null, null);
            if (objType.Instance is IFormatData)
            {
                IFormatData ret = (IFormatData)objType.Instance;
                ret = ret.FromXml(NodeStandalone(node), args);
                if (ret != null)
                {
                    ret.Init();
                }
                return (T)(object)ret;
            }
            else
            {
                //LogManager.Warning(string.Format("{0} class does not implementing {1} interface.", typeof(T).Name, "IFormatData"));
                string conditionString = GetXmlConditionString(objType.FriendlyName, args);
                LogManager.Message("Data node information : ", node.OuterXml);

                XmlNode pNode = node.OwnerDocument.CreateElement("Data");
                pNode.AppendChild(node.Clone());
                XmlNode dNode = pNode.SelectSingleNode(conditionString);
                if (dNode == null)
                {
                    throw new CanNotFindDataException(conditionString);
                }
                return (T)XmlToObject(dNode, objType);
            }
        }
        #endregion

        #region 实例化
        /// <summary>
        /// 创建一个类的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// 创建一个类的实例
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static object CreateInstance(Type interfaceType)
        {
            Type objType = GetTypeFromInstance(interfaceType, null);
            object obj = null;

            try
            {
                obj = Activator.CreateInstance(objType);
            }
            catch (System.Exception ex)
            {
                throw GetRealException(ex);
            }

            if (obj == null)
            {
                throw new InvalidDataTypeException(objType.Name);
            }

            return obj;
        }
        #endregion

        #region 读库
        /// <summary>
        /// 从库文件中读取数据内容
        /// </summary>
        /// <param name="FileName">文件名（包含路径）</param>
        /// <param name="ContentName">数据名称</param>
        /// <returns>数据内容</returns>
        public static string ReadLibrary(string FileName, string ContentName)
        {
            lock (libLock)
            {
                string result = string.Empty;

                List<string> _nameList = new List<string>();
                _nameList.Clear();
                if (!File.Exists(FileName))
                {
                    throw new CanNotFindFileException(FileName);
                }

                FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                try
                {

                    string tmp;
                    int n = int.Parse(Decode(br.ReadString()));
                    for (int i = 0; i < n; i++)
                    {
                        tmp = Decode(br.ReadString());
                        _nameList.Add(tmp);
                    }
                    if (_nameList.Contains(ContentName))
                    {
                        n = _nameList.IndexOf(ContentName);
                    }
                    else
                    {
                        throw new CanNotFindFileException(ContentName);
                    }
                    while (n > 0)
                    {
                        n--;
                        br.ReadString();
                    }

                    result = Decode(br.ReadString());
                    br.Close();
                    fs.Close();
                    br.Dispose();
                    fs.Dispose();
                }
                catch
                {
                    br.Close();
                    fs.Close();
                    br.Dispose();
                    fs.Dispose();
                    result = string.Empty;
                }

                return result;
            }
        }
        #endregion

        #region 写库
        /// <summary>
        /// 将数据内容写入指定库文件
        /// </summary>
        /// <param name="FileName">文件名（包含路径）</param>
        /// <param name="ContentName">数据名称</param>
        /// <param name="data">数据内容</param>
        public static void WriteLibrary(string FileName, string ContentName, string data)
        {
            lock (libLock)
            {
                FileName = Path.GetFullPath(FileName);
                List<string> _nameList = new List<string>();
                _nameList.Clear();
                int index = FileName.LastIndexOf('\\') + 1;
                string tmpName = string.Format("{0}_{1}.tmp", FileName.Substring(0, index), FileName.Substring(index));
                    
                using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    try
                    {
                        File.Delete(tmpName);
                    }
                    catch
                    {
                        Robot.Recess(50);
                        File.Delete(tmpName);
                    }

                    using (FileStream fstmp = new FileStream(tmpName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fstmp))
                            {
                                string tmp;
                                int n = 0;
                                try
                                {
                                    n = int.Parse(Decode(br.ReadString()));
                                }
                                catch (EndOfStreamException)
                                {
                                    n = 0;
                                }
                                for (int i = 0; i < n; i++)
                                {
                                    tmp = Decode(br.ReadString());
                                    _nameList.Add(tmp);
                                }
                                if (_nameList.Contains(ContentName))
                                {
                                    bw.Write(Encode(n.ToString()));
                                    for (int i = 0; i < _nameList.Count; i++)
                                    {
                                        bw.Write(Encode(_nameList[i]));
                                    }
                                    for (int i = 0; i < _nameList.Count; i++)
                                    {
                                        if (_nameList[i] == ContentName)
                                        {
                                            br.ReadString();
                                            bw.Write(Encode(data));
                                        }
                                        else
                                        {
                                            bw.Write(br.ReadString());
                                        }
                                    }
                                }
                                else
                                {
                                    n = n + 1;
                                    _nameList.Add(ContentName);
                                    bw.Write(Encode(n.ToString()));
                                    for (int i = 0; i < _nameList.Count; i++)
                                    {
                                        bw.Write(Encode(_nameList[i]));
                                    }
                                    for (int i = 0; i < _nameList.Count - 1; i++)
                                    {
                                        bw.Write(br.ReadString());
                                    }
                                    bw.Write(Encode(data));

                                }
                                bw.Close();
                            }
                            br.Close();
                        }
                        fstmp.Close();
                    }
                    fs.Close();
                }
                try
                {
                    File.Delete(FileName);
                }
                catch
                {
                    Robot.Recess(50);
                    File.Delete(FileName);
                }

                try
                {
                    File.Copy(tmpName, FileName);
                }
                catch
                {
                    Robot.Recess(50);
                    File.Copy(tmpName, FileName);
                }

                try
                {
                    File.Delete(tmpName);
                }
                catch
                {
                    Robot.Recess(50);
                    File.Delete(tmpName);
                }
            }
        }
        #endregion

        #region Base64加密
        /// <summary>
        /// 对字符串进行Base64加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="encoding">编码</param>
        /// <returns>密文</returns>
        public static string Base64Encrypt(string data, string encoding = null)
        {
            return Encode(data, encoding);
        }

        /// <summary>
        /// 对字符串进行Base64加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="encoding">编码</param>
        /// <returns>密文</returns>
        private static string Encode(string data, string encoding = null)
        {
            Encoding code = Encoding.Default;
            if (!string.IsNullOrEmpty(encoding))
            {
                code = Encoding.GetEncoding(encoding);
            }
            string res = string.Empty;
            
            byte[] bytes = code.GetBytes(data);
            res = Convert.ToBase64String(bytes);
            
            return res;
        }
        #endregion

        #region Base64解密
        /// <summary>
        /// 对Base64加密的密文进行解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="encoding">编码</param>
        /// <returns>明文</returns>
        public static string Base64Decrypt(string data, string encoding = null)
        {
            return Decode(data, encoding);
        }

        /// <summary>
        /// 对Base64加密的密文进行解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="encoding">编码</param>
        /// <returns>明文</returns>
        private static string Decode(string data, string encoding = null)
        {
            Encoding code = Encoding.Default;
            if (!string.IsNullOrEmpty(encoding))
            {
                code = Encoding.GetEncoding(encoding);
            }
            string res = string.Empty;

            byte[] outputb = Convert.FromBase64String(data);
            res = code.GetString(outputb);
            return res;
        }
        #endregion

        internal static Encoding GetEncoding(Encoding encoding)
        {
            if (encoding == null)
            {
                return Encoding.Default;
            }
            else
            {
                return encoding;
            }
        }

        #region 指定长度分割字符串
        /// <summary>
        /// 将字符串按指定字节长度分割，存入列表
        /// </summary>
        /// <param name="content">待分割的字符串</param>
        /// <param name="bytesNum">字节长度</param>
        /// <param name="encoding">字符编码</param>
        /// <returns>分割后的列表对象</returns>
        public static List<string> GetString(string content, int bytesNum, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            List<string> result = new List<string>();
            result.Clear();
            char[] cs = content.ToCharArray();
            string tmp = string.Empty;
            int count = 0;
            foreach (char c in cs)
            {
                if (string.IsNullOrEmpty(c.ToString().Trim()))
                {
                    continue;
                }
                tmp = tmp + c;
                count = count + encoding.GetByteCount(c.ToString());
                if (count == bytesNum)
                {
                    count = 0;
                    result.Add(tmp);
                    tmp = string.Empty;
                }

                if (count > bytesNum)
                {
                    count = encoding.GetByteCount(tmp.Substring(tmp.Length - 1));
                    result.Add(tmp.Substring(0, tmp.Length - 1));
                    tmp = tmp.Substring(tmp.Length - 1);
                }
            }
            if (count > 0)
            {
                result.Add(tmp);
            }
            return result;
        }
        #endregion

        #region 获取GBK字符集所有字符
        /// <summary>
        /// 获取GBK字符集
        /// </summary>
        /// <returns></returns>
        public static string GetGBKString()
        {
            return ReadLibrary("Data.dll", "GBK");
        }
        #endregion

        #region 获取关键字
        /// <summary>
        /// 获取关键字信息
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public static Keyword GetKeyword(string keyword)
        {
            if (keyWordInfo == null)
            {
                throw new CanNotFindKeywordException("GetKeyword : " + keyword);
            }
            if (keyWordInfo.ContainsKey(keyword))
            {
                return keyWordInfo[keyword];
            }
            else
            {
                throw new CanNotFindKeywordException("GetKeyword : " + keyword);
            }
        }

        public static Keyword GetDefaultKeyword(string keyword)
        {
            if (defaultKeywordInfo == null)
            {
                throw new CanNotFindKeywordException("GetDefaultKeyword : " + keyword);
            }
            if (defaultKeywordInfo.ContainsKey(keyword))
            {
                return defaultKeywordInfo[keyword];
            }
            else
            {
                throw new CanNotFindKeywordException("GetDefaultKeyword : " + keyword);
            }
        }

        public static Keyword GetFirstKeyword(string keyword)
        {
            if (firstKeywordInfo == null)
            {
                throw new CanNotFindKeywordException("GetFirstKeyword : " + keyword);
            }
            if (firstKeywordInfo.ContainsKey(keyword))
            {
                return firstKeywordInfo[keyword];
            }
            else
            {
                throw new CanNotFindKeywordException("GetFirstKeyword : " + keyword);
            }
        }
        #endregion

        #region 处理编号
        /// <summary>
        /// 编号处理
        /// </summary>
        /// <param name="number">输入编号</param>
        /// <returns>处理过的编号List</returns>
        public static List<string> ParseBH(string number)
        {
            LogManager.DebugFormat("Parse {0}", number);
            List<string> result = new List<string>();
            if (number == null)
            {
                LogManager.Warning("ParseBH Bh is null");
                return result;
            }
            string[] temp;
            if (number.Contains(","))
            {
                result.AddRange(number.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
            else if (number.Contains("-"))
            {
                temp = number.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                if (temp[0].Length != temp[1].Length)
                {
                    result.AddRange(temp);
                    return result;
                }

                string prifx1 = GetBhPrifx(temp[0]);
                if (string.Equals(prifx1, temp[0]))
                {
                    result.AddRange(temp);
                    return result;
                }

                string prifx2 = GetBhPrifx(temp[1]);

                if (!string.Equals(prifx1, prifx2))
                {
                    result.AddRange(temp);
                    return result;
                }

                int startNumber = GetBhNumber(temp[0]);
                int stopNumber = GetBhNumber(temp[1]);

                if (startNumber > stopNumber)
                {
                    result.AddRange(temp);
                    return result;
                }

                if (startNumber == stopNumber)
                {
                    result.Add(temp[0]);
                    return result;
                }

                int padding = temp[0].Length - prifx1.Length;

                for (int realNumber = startNumber; realNumber <= stopNumber; realNumber++)
                {
                    result.Add(prifx1 + realNumber.ToString("D" + padding));
                }
            }
            else
            {
                result.Add(number);
            }
            return result.ConvertAll(x => x.Trim());
        }

        private static string GetBhPrifx(string bhString)
        {
            int charIndex;
            for (charIndex = bhString.Length - 1; charIndex >= 0; charIndex--)
            {
                char c = bhString[charIndex];
                if (c < '0' || c > '9')
                {
                    break;
                }
            }

            return bhString.Substring(0, charIndex + 1);
        }

        private static int GetBhNumber(string bhString)
        {
            StringBuilder numberString = new StringBuilder();
            for (int charIndex = bhString.Length - 1; charIndex >= 0; charIndex--)
            {
                char c = bhString[charIndex];

                if (c >= '0' && c <= '9')
                {
                    numberString.Insert(0, c);
                }
                else
                {
                    break;
                }
            }
            return int.Parse(numberString.ToString());
        }
        #endregion

        #region 获取缓存
        public static string GetCache(string bh, string name = null)
        {
            Cache cache = new Cache();
            cache.Bh = bh;
            cache.Get();

            if (string.IsNullOrEmpty(name))
            {
                return cache.ToString();
            }
            else
            {
                return cache[name];
            }
            //string path = Path.Combine(Config.WorkingDirectory, "Temp\\taist.temp");

            //try
            //{
            //    path = Path.Combine(Config.WorkingDirectory, "Temp", CaseManager.GetIdentification() + ".temp");
            //}
            //catch (System.Exception)
            //{
            //    path = Path.Combine(Config.WorkingDirectory, "Temp\\taist.temp");
            //}

            //LogManager.DebugFormat("Get cache from {0} named {1}, {2}", bh, name, path);
            //if (!File.Exists(path))
            //{
            //    return null;
            //}

            //XmlDocument doc = new XmlDocument();
            //doc.Load(path);

            //XmlNode xn = doc.SelectSingleNode(string.Format("//Cache[Bh='{0}']", bh));
            //if (xn == null)
            //{
            //    throw new CanNotFindDataException(bh);
            //}

            //if (string.IsNullOrEmpty(name))
            //{
            //    return xn.InnerXml;
            //}
            //else
            //{
            //    XmlNode data = DataFactory.SelectSingleNodeIgnoreCase(xn, name);
            //    return data.InnerXml;
            //}
        }
        #endregion

        #region 解压缩

        #region Zip
        private static List<byte[]> EntryList(byte[] data)
        {
            List<byte[]> list = new List<byte[]>();
            int offset = 0;
            int len = 0;
            byte[] tmp;
            for (int i = 0; i < data.Length - 4; i++)
            {
                if (data[i] == 0x50 && data[i + 1] == 0x4B && data[i + 2] == 0x03 && data[i + 3] == 0x04)
                {
                    len = i - offset;
                    tmp = new byte[len];
                    Array.Copy(data, offset, tmp, 0, len);
                    list.Add(tmp);
                    offset = i;
                }
            }
            len = data.Length - offset;
            tmp = new byte[len];
            Array.Copy(data, offset, tmp, 0, len);
            list.Add(tmp);
            list.RemoveAt(0);
            return list;
        }

        private static List<byte[]> DirList(byte[] data)
        {
            List<byte[]> list = new List<byte[]>();
            int offset = 0;
            int len = 0;
            byte[] tmp;
            for (int i = 0; i < data.Length - 4; i++)
            {
                if (data[i] == 0x50 && data[i + 1] == 0x4B && data[i + 2] == 0x01 && data[i + 3] == 0x02)
                {
                    len = i - offset;
                    tmp = new byte[len];
                    Array.Copy(data, offset, tmp, 0, len);
                    list.Add(tmp);
                    offset = i;
                }
            }
            len = data.Length - offset;
            tmp = new byte[len];
            Array.Copy(data, offset, tmp, 0, len);
            list.Add(tmp);
            list.RemoveAt(0);
            return list;
        }

        /// <summary>
        /// 解压缩ZIP文件到制定目录
        /// </summary>
        /// <param name="zipFileName">ZIP文件名</param>
        /// <param name="savePath">解压缩后的文件要保存到的文件夹名</param>
        /// <param name="encoding">解压缩使用的字符集</param>
        public static void ZipDecompress(string zipFileName, string savePath, Encoding encoding = null)
        {
            if(!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            FileInfo fi = new FileInfo(zipFileName);
            byte[] data = new byte[0];
            using (FileStream fs = new FileStream(zipFileName, FileMode.Open))
            {
                byte[] tmp = new byte[1024];
                int len = 0;
                while ((len = fs.Read(tmp, 0, 1024)) > 0)
                {
                    int offset = data.Length;
                    byte[] dataTmp = data;
                    data = new byte[offset + len];
                    Array.Copy(dataTmp, 0, data, 0, offset);
                    Array.Copy(tmp, 0, data, offset, len);
                }
            }

            //_originalData = new Dictionary<string, string>();
            //_originalExtraData = new Dictionary<string, byte[]>();
            List<byte[]> entryList = EntryList(data);
            List<byte[]> dirList = DirList(data);
            for (int i=0;i<entryList.Count;i++)
            {
                byte[] entryData = entryList[i];
                byte[] dirData = dirList[i];
                int nameSize = Binary.ToInt(entryData[26], entryData[27]);
                byte[] nameBytes = new byte[nameSize];
                Array.Copy(entryData, 30, nameBytes, 0, nameSize);
                string name = encoding.GetString(nameBytes);

                int extraDataSize = Binary.ToInt(entryData[28], entryData[29]);

                int dataOffset = 30 + nameSize + extraDataSize;
                int dataLen = entryData.Length - dataOffset;
                byte[] dataTmp = new byte[dataLen];
                Array.Copy(entryData, dataOffset, dataTmp, 0, dataLen);

                if (extraDataSize > 0)
                {
                    byte[] extralData = new byte[extraDataSize];
                    Array.Copy(entryData, 30 + nameSize, extralData, 0, extraDataSize);
                    //_originalExtraData.Add(name, extralData);
                    Binary.Print(BitConverter.ToString(extralData));
                }

                int dataSize = Binary.ToInt(entryData[22], entryData[23], entryData[24], entryData[25]);
                if (dataSize == 0)
                {
                    dataSize = Binary.ToInt(dirData[24], dirData[25], dirData[26], dirData[27]);
                }
                byte[] dataDecompress = new byte[dataSize];
                using (MemoryStream ms = new MemoryStream(dataTmp))
                {
                    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        using (MemoryStream ow = new MemoryStream(dataDecompress))
                        {
                            ds.CopyTo(ow);
                        }
                    }
                }
                long l1 = dataDecompress.Length;
                if ((l1 & -4294967296L) != 0)
                {
                    Console.WriteLine(true);
                }
                else
                {
                    Console.WriteLine(false);
                }
                //string dataString = encoding.GetString(dataDecompress).Trim();
                //_originalData.Add(name, dataString);
                string path = Path.Combine(savePath, name);
                if (path.EndsWith("\\") || path.EndsWith("/"))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(dataDecompress, 0, dataDecompress.Length);
                        fs.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 将制定文件夹下的文件压缩成制定ZIP文件
        /// </summary>
        /// <param name="dirPath">要压缩的文件夹路径</param>
        /// <param name="zipFileName">压缩后保存的文件名</param>
        /// <param name="encoding">压缩使用的字符集</param>
        public static void ZipCompress(string dirPath, string zipFileName, Encoding encoding = null)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region Gzip
        /// <summary>
        /// GZip解压函数
        /// </summary>
        /// <param name="compressData"></param>
        /// <returns></returns>
        public static byte[] GZipDecompress(byte[] compressData)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(new MemoryStream(compressData), CompressionMode.Decompress))
                {
                    byte[] bytes = new byte[40960];
                    int n;
                    while ((n = gZipStream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        stream.Write(bytes, 0, n);
                    }
                    gZipStream.Close();
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// GZip压缩函数
        /// </summary>
        /// <param name="decompressData"></param>
        /// <returns></returns>
        public static byte[] GZipCompress(byte[] decompressData)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    gZipStream.Write(decompressData, 0, decompressData.Length);
                    gZipStream.Close();
                }
                return stream.ToArray();
            }
        }
        #endregion

        #region Deflate
        /// <summary>
        /// Deflate解压函数
        /// JS:var details = eval_r('(' + utf8to16(zip_depress(base64decode(hidEnCode.value))) + ')')对应的C#压缩方法
        /// </summary>
        /// <param name="compressData"></param>
        /// <returns></returns>
        public static byte[] DeflateDecompress(byte[] compressData)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ms.Write(compressData, 0, compressData.Length);
                ms.Position = 0;
                using (System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Decompress))
                {
                    stream.Flush();
                    int nSize = 16 * 1024 + 256;

                    byte[] decompressData = new byte[0];
                    byte[] decompressBuffer = new byte[nSize];
                    int nSizeIncept = 0;
                    while ((nSizeIncept = stream.Read(decompressBuffer, 0, nSize)) > 0)
                    {
                        byte[] tmp = decompressData;
                        decompressData = new byte[tmp.Length + nSize];

                        tmp.CopyTo(decompressData, 0);
                        Array.Copy(decompressBuffer, 0, decompressData, tmp.Length, nSizeIncept);
                    }
                    stream.Close();
                    return decompressData;
                }
            }
        }

        /// <summary>
        /// Deflate压缩函数
        /// </summary>
        /// <param name="decompressData"></param>
        /// <returns></returns>
        public static byte[] DeflateCompress(byte[] decompressData)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Compress, true))
                {
                    stream.Write(decompressData, 0, decompressData.Length);
                    stream.Close();
                }
                byte[] compressedData = ms.ToArray();
                ms.Close();
                return compressedData;
            }
        }
        #endregion 

        private static Encoding ValidateEncoding(Encoding encoding)
        {
            if (encoding == null) return Encoding.Default;
            return encoding;
        }

        #endregion

        public static System.Exception GetRealException(System.Exception ex)
        {
            while (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        #region PrivateFunctions

        #region Xml<->Object

        /// <summary>
        /// 获取真实的可以创建实例的类名
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="nodeName"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static TaistType GetRealType(Type objectType, string nodeName, object instance)
        {
            if (objectType == null
                && string.IsNullOrEmpty(nodeName)
                && instance == null)
            {
                throw new InvalidDataTypeException("null");
            }

            Type realType = GetTypeFromInstance(objectType, instance);

            if (realType == null)
            {
                realType = DataFactory.GetClassInfo(nodeName).Type;
            }

            TaistType retType = null;
            if (instance == null)
            {
                retType = TaistType.GetDataType(realType);
            }
            else
            {
                retType = TaistType.GetDataType(instance);
            }

            if (objectType != null && realType != objectType && !retType.ExtendsFrom(objectType))
            {
                throw new InvalidDataTypeException(realType.Name);
            }

            if (string.IsNullOrEmpty(nodeName))
            {
                return retType;
            }

            if (retType.FriendlyName.ToUpper().Equals(nodeName.ToUpper()))
            {
                return retType;
            }

            if (retType.Name.ToUpper().Equals(nodeName.ToUpper()))
            {
                return retType;
            }

            if (retType.Name.Replace("Object", "").Trim().ToUpper().Equals(nodeName.ToUpper()))
            {
                return retType;
            }

            throw new InvalidDataTypeException(realType.Name);
        }

        private static Type GetTypeFromInstance(Type objectType, object objectInstance)
        {
            if (objectInstance == null)
            {
                if (objectType == null)
                {
                    return null;
                }
                else
                {
                    if (objectType.IsAbstract || objectType.IsInterface)
                    {
                        string fullName = objectType.FullName.Replace("Object", "").Trim();
                        return objectType.Assembly.GetType(fullName + "Object");
                    }
                    else
                    {
                        return objectType;
                    }
                }
            }
            else
            {
                return objectInstance.GetType();
            }
        }

        /// <summary>
        /// 校验输入node的正确性
        /// </summary>
        /// <param name="objectNode"></param>
        /// <returns></returns>
        private static XmlNode InitXmlNode(XmlNode objectNode)
        {
            XmlNode initNode = null;
            if (objectNode == null)
            {
                throw new InvalidParamValueException("The objectNode is null");
            }

            if (objectNode is XmlComment)
            {
                throw new InvalidParamValueException("The objectNode is XmlComment");
            }

            initNode = objectNode;
            if (objectNode is XmlDocument)
            {
                initNode = ((XmlDocument)objectNode).DocumentElement;
            }
            return initNode;
        }

        private static XmlNode NodeStandalone(XmlNode xmlNode)
        {
            XmlNode retNode = xmlNode.OwnerDocument.CreateElement("Data");
            retNode.AppendChild(xmlNode.Clone());
            return retNode;
        }

        private static TaistType GetDefaultData(XmlNode objectXmlNode, TaistType dataType)
        {
            XmlNode defaultDataBhNode = objectXmlNode.SelectSingleNode("DefaultDataBh");

            if (defaultDataBhNode == null)
            {
                return dataType;
            }

            if (string.IsNullOrEmpty(defaultDataBhNode.InnerText))
            {
                return dataType;
            }

            try
            {
                XmlNode defaultDataNode = TestCasePool.GetDataNode(defaultDataBhNode.InnerText.Trim());
                object defaultData = GetData(defaultDataNode, dataType);
                return DataType.GetDataType(defaultData);
            }
            catch (CanNotFindDataException)
            {
                return dataType;
            }
        }

        #endregion

        #endregion
    }
}
