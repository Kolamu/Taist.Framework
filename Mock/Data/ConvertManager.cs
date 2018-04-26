namespace Mock.Data
{
    using System;
    using System.Xml;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;

    using Mock.Data.TaistDataCenter;
    using Mock.Data.Exception;

    /// <summary>
    /// 提供类之间转换的类
    /// </summary>
    public class ConvertManager
    {
        private int srcIndex = 1;
        private int dstIndex = 2;
        private Dictionary<string, XmlNode> convDic = new Dictionary<string, XmlNode>();

        /// <summary>
        /// 构造数据转换类的新实例
        /// </summary>
        /// <param name="xmlPath">记录了转换规则的XML文件名</param>
        public ConvertManager(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            XmlNodeList ciList = doc.SelectNodes("//ConvertInformation");
            foreach (XmlNode ci in ciList)
            {
                convDic.Add(ci.Attributes["name"].Value.ToUpper(), ci);
            }
        }

        public object Convert(object src, object dst, string convertName)
        {
            if (src == null)
            {
                return dst;
            }
            if (!convDic.ContainsKey(convertName.ToUpper()))
            {
                throw new CanNotFindDataException(convertName);
            }
            XmlNode infoNode = convDic[convertName.ToUpper()];

            XmlAttribute typeAttr = infoNode.Attributes["type"];

            if (typeAttr == null)
            {
                throw new InvalidConvertTypeException("type is null");
            }

            ConvertType ct = (ConvertType)Enum.Parse(typeof(ConvertType), infoNode.Attributes["type"].Value.ToUpper());
            switch (ct)
            {
                case ConvertType.CLASS:
                    {
                        return doConvertClass(src, dst, infoNode);
                    }
                case ConvertType.VALUE:
                    {
                        return doConvertValue(src, infoNode);
                    }
                case ConvertType.DECRYPT:
                    {
                        return doConvertDecrypt(src, infoNode);
                    }
                case ConvertType.ENCRYPT:
                    {
                        return doConvertEncrypt(src, infoNode);
                    }
                default:
                    {
                        throw new InvalidConvertTypeException(ct.ToString());
                    }
            }
        }

        private object doConvertClass(object src, object dst, XmlNode infoNode)
        {
            if (src == null)
            {
                return dst;
            }

            if (infoNode == null)
            {
                throw new InvalidConvertTypeException("info node is null");
            }

            Type srcType = src.GetType();
            Type dstType = null;
            string srcName = srcType.Name;
            string dstName = dst.GetType().Name;
            XmlNode typeNode = infoNode.SelectSingleNode("Type");
            if (typeNode == null)
            {
                throw new InvalidConvertTypeException("Can not find Type node");
            }

            if (typeNode.Attributes["name1"] == null)
            {
                throw new InvalidConvertTypeException("Can not find name1 attribute from Type node");
            }

            if (typeNode.Attributes["name2"] == null)
            {
                throw new InvalidConvertTypeException("Can not find name2 attribute from Type node");
            }

            string name1 = typeNode.Attributes["name1"].Value;
            string name2 = typeNode.Attributes["name2"].Value;
            if (string.Equals(srcType.Name, name1) || string.Equals(srcType.Name, name1 + "Object"))
            {
                srcIndex = 1;
                dstIndex = 2;
                if (dst == null)
                {
                    ClassInfo ci = DataFactory.GetClassInfo(name2);
                    dstType = ci.Type;
                    dst = Activator.CreateInstance(dstType);
                }
                else
                {
                    dstType = dst.GetType();
                    if (!string.Equals(dstType.Name, name2) && !string.Equals(dstType.Name, name2 + "Object"))
                    {
                        throw new InvalidDataTypeException(srcType.Name);
                    }
                }
            }
            else if (string.Equals(srcType.Name, name2) || string.Equals(srcType.Name, name2 + "Object"))
            {
                srcIndex = 2;
                dstIndex = 1;
                if (dst == null)
                {
                    ClassInfo ci = DataFactory.GetClassInfo(name1);
                    dstType = ci.Type;
                    dst = Activator.CreateInstance(dstType);
                }
                else
                {
                    dstType = dst.GetType();
                    if (!string.Equals(dstType.Name, name1) && !string.Equals(dstType.Name, name1 + "Object"))
                    {
                        throw new InvalidDataTypeException(srcType.Name);
                    }
                }
            }
            else
            {
                throw new InvalidDataTypeException(srcType.Name);
            }

            XmlNodeList pNodeList = infoNode.SelectNodes("Property");
            
            foreach (XmlNode pNode in pNodeList)
            {
                if (pNode.Attributes["name1"] == null)
                {
                    throw new InvalidConvertTypeException("Can not find name1 attribute from Type node");
                }

                if (pNode.Attributes["name2"] == null)
                {
                    throw new InvalidConvertTypeException("Can not find name2 attribute from Type node");
                }
                string srcPropertyName = pNode.Attributes["name" + srcIndex].Value;
                string dstPropertyName = pNode.Attributes["name" + dstIndex].Value;

                PropertyInfo srcInfo = srcType.GetProperty(srcPropertyName);
                if (srcInfo == null)
                {
                    throw new CanNotFindPropertyException(srcType.Name, srcPropertyName);
                }

                PropertyInfo dstInfo = dstType.GetProperty(dstPropertyName);
                if (dstInfo == null)
                {
                    throw new CanNotFindPropertyException(dstType.Name, dstPropertyName);
                }

                LogManager.DebugFormat("Convert {0}.{1} to {2}.{3} {4}", srcName, srcPropertyName, dstName, dstPropertyName, srcInfo.GetValue(src, null)); 
                
                XmlAttribute cAttr = pNode.Attributes["convert"];
                if (cAttr == null || string.IsNullOrEmpty(cAttr.Value))
                {
                    if (srcInfo.PropertyType.Equals(typeof(universal)))
                    {
                        universal uv = (universal)srcInfo.GetValue(src, null);
                        if (uv == null)
                        {
                            continue;
                            //dstInfo.SetValue(dst, System.Convert.ChangeType(null, dstInfo.PropertyType), null);
                        }
                        else
                        {
                            setValue(dstInfo, dst, uv.StringValue);
                            //dstInfo.SetValue(dst, System.Convert.ChangeType(uv.StringValue, dstInfo.PropertyType), null);
                        }
                    }
                    else
                    {
                        //object value = System.Convert.ChangeType(srcInfo.GetValue(src, null), dstInfo.PropertyType);
                        //dstInfo.SetValue(dst, value, null);
                        setValue(dstInfo, dst, srcInfo.GetValue(src, null));
                    }
                }
                else
                {
                    if (srcInfo.PropertyType.Equals(typeof(universal)))
                    {
                        universal uv = (universal)srcInfo.GetValue(src, null);
                        if (uv == null)
                        {
                            continue;
                            //setValue(dstInfo, dst, srcInfo.GetValue(src, null));
                            //dstInfo.SetValue(dst, System.Convert.ChangeType(null, dstInfo.PropertyType), null);
                        }
                        else
                        {
                            object tmp = Convert(uv.StringValue, "", cAttr.Value);
                            setValue(dstInfo, dst, tmp);
                            //dstInfo.SetValue(dst, System.Convert.ChangeType(tmp, dstInfo.PropertyType), null);
                        }
                    }
                    else
                    {
                        object tmp = Convert(srcInfo.GetValue(src, null), "", cAttr.Value);
                        setValue(dstInfo, dst, tmp);
                        //dstInfo.SetValue(dst, System.Convert.ChangeType(tmp, dstInfo.PropertyType), null);
                    }
                }
            }
            return dst;
        }

        private object doConvertValue(object src, XmlNode infoNode)
        {
            if (src == null) return null;
            Hashtable table = new Hashtable();

            XmlNodeList vNodeList = infoNode.SelectNodes("Value");
            foreach(XmlNode vNode in vNodeList)
            {
                string srcString = vNode.Attributes["val" + srcIndex].Value.ToUpper();
                string dstString = vNode.Attributes["val" + dstIndex].Value;
                table.Add(srcString, dstString);
            }
            string key = src.ToString().ToUpper();
            if (table.ContainsKey(key))
            {
                return table[key];
            }
            else
            {
                return src;
            }
        }

        private object doConvertEncrypt(object src, XmlNode infoNode)
        {
            if (src == null) return null;

            Encoding encoding = Encoding.Default;
            if (infoNode != null)
            {
                XmlNode eNode = infoNode.SelectSingleNode("Encoding");
                
                if (eNode == null || string.IsNullOrEmpty(eNode.InnerText))
                {
                    encoding = Encoding.Default;
                }
                else
                {
                    encoding = Encoding.GetEncoding(eNode.InnerText.Trim());
                }
            }
            return System.Convert.ToBase64String(encoding.GetBytes(src.ToString()));
        }

        private object doConvertDecrypt(object src, XmlNode infoNode)
        {
            if (src == null) return null;

            Encoding encoding = Encoding.Default;
            if (infoNode != null)
            {
                XmlNode eNode = infoNode.SelectSingleNode("Encoding");
                if (eNode == null || string.IsNullOrEmpty(eNode.InnerText))
                {
                    encoding = Encoding.Default;
                }
                else
                {
                    encoding = Encoding.GetEncoding(eNode.InnerText.Trim());
                }
            }

            return encoding.GetString(System.Convert.FromBase64String(src.ToString()));
        }

        private void setValue(PropertyInfo pi, object dst, object value)
        {
            if (pi.PropertyType == typeof(universal))
            {
                pi.SetValue(dst, new universal(value), null);
            }
            else
            {
                if (value == null)
                {
                    value = GetDefaultObject(pi.PropertyType);
                    pi.SetValue(dst, value, null);
                }
                else
                {
                    value = System.Convert.ChangeType(value, pi.PropertyType);
                    pi.SetValue(dst, value, null);
                }
            }
        }

        private object GetDefaultObject(Type type)
        {
            Type t = typeof(ConvertManager);
            MethodInfo mi = t.GetMethod("GetDefault", BindingFlags.Instance | BindingFlags.NonPublic);

            mi = mi.MakeGenericMethod(new Type[] { type });

            return mi.Invoke(this, null);
        }

        private T GetDefault<T>()
        {
            return default(T);
        }
    }
}
