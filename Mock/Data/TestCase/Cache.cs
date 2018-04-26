using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Mock.Data.Exception;
using Mock.Data.Attributes;

namespace Mock.Data
{
    /// <summary>
    /// 缓存对象类(测试实际数据)
    /// </summary>
    public class Cache
    {
        private string path = null;
        public Cache()
        {
            try
            {
                path = Path.Combine(Config.WorkingDirectory, "Temp", CaseManager.GetIdentification() + ".temp");
            }
            catch(System.Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
                path = Path.Combine(Config.WorkingDirectory, "Temp\\taist.temp");
            }
        }

        public Cache(string cacheBh)
        {
            Bh = cacheBh;
        }

        /// <summary>
        /// 编号
        /// </summary>
        [FieldProperty("Bh", false)]
        public string Bh
        {
            get;
            set;
        }

        /// <summary>
        /// 检索条件
        /// </summary>
        [FieldProperty("SelectCondition", true)]
        public string SelectCondition
        {
            get;
            set;
        }

        /// <summary>
        /// 检索条件
        /// </summary>
        [FieldProperty("Position", true)]
        public string Position
        {
            get;
            set;
        }

        private Dictionary<string, string> data = null;

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void setProperty(string name, string value)
        {
            LogManager.Debug(string.Format("setProperty {0} {1}", name, value));
            if (data == null)
            {
                data = new Dictionary<string, string>();
            }
            if (data.ContainsKey(name))
            {
                data[name] = value;
            }
            else
            {
                data.Add(name, value);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="node"></param>
        public void setProperty(System.Xml.XmlNode node)
        {
            if (node is XmlComment) return;
            if (data == null)
            {
                data = new Dictionary<string, string>();
            }
            if (data.ContainsKey(node.Name))
            {
                if (node.ChildNodes.Count > 1)
                {
                    data[node.Name] = node.InnerXml;
                }
                else
                {
                    XmlNode child = node.ChildNodes[0];
                    if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CDATA)
                    {
                        data[node.Name] = node.InnerText;
                    }
                    else
                    {
                        data[node.Name] = node.InnerXml;
                    }
                }
            }
            else
            {
                if (node.ChildNodes.Count > 1)
                {
                    data.Add(node.Name, node.InnerXml);
                }
                else if(node.ChildNodes.Count == 1)
                {
                    XmlNode child = node.ChildNodes[0];
                    if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CDATA)
                    {
                        data.Add(node.Name, node.InnerText);
                    }
                    else
                    {
                        data.Add(node.Name, node.InnerXml);
                    }
                }
                else
                {
                    data.Add(node.Name, node.InnerText);
                }
            }

        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> getProperty()
        {
            return data;
        }

        /// <summary>
        /// 获取Cache内容
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [FieldProperty(isIgnore= true)]
        public string this[string name]
        {
            get
            {
                if (data == null)
                {
                    throw new CanNotFindDataException(name);
                }
                if (data.ContainsKey(name))
                {
                    return data[name];
                }
                else
                {
                    if (name.Contains("."))
                    {
                        string infoName = name.Substring(0, name.IndexOf('.'));
                        if (data.ContainsKey(infoName))
                        {
                            string infoString = string.Format("<{1}>{0}</{1}>", data[infoName], infoName);
                            if (string.IsNullOrEmpty(infoString)) return null;
                            XmlDocument infoDoc = new XmlDocument();
                            infoDoc.LoadXml(infoString);
                            string xpath = string.Format("//{0}", name.Replace('.', '/'));
                            XmlNode node = infoDoc.SelectSingleNode(xpath);
                            if (node == null)
                            {
                                throw new CanNotFindDataException(name);
                            }
                            return node.InnerXml;
                        }
                        else
                        {
                            throw new CanNotFindDataException(name);
                        }
                    }
                    else
                    {
                        throw new CanNotFindDataException(name);
                    }
                }
            }
            set
            {
                if (data == null)
                {
                    data = new Dictionary<string, string>();
                }
                if (data.ContainsKey(name))
                {
                    data[name] = value;
                }
                else
                {
                    data.Add(name, value);
                }
            }
        }

        [FieldProperty(isIgnore = true)]
        public string this[string name, params string[] nameArray]
        {
            get
            {
                try
                {
                    return this[name];
                }
                catch (CanNotFindDataException)
                {
                    foreach (string cacheName in nameArray)
                    {
                        try
                        {
                            return this[cacheName];
                        }
                        catch (CanNotFindDataException) { }
                    }
                }

                throw new CanNotFindDataException(name);
            }
        }

        [FieldProperty(isIgnore=true)]
        public string this[string name, Dictionary<string, string> condition]
        {
            get
            {
                if (data == null)
                {
                    throw new CanNotFindDataException(name);
                }
                if (data.ContainsKey(name))
                {
                    return data[name];
                }
                else
                {
                    if (name.Contains("."))
                    {
                        string infoName = name.Substring(0, name.IndexOf('.'));
                        if (data.ContainsKey(infoName))
                        {
                            string infoString = string.Format("<{1}>{0}</{1}>", data[infoName], infoName);
                            if (string.IsNullOrEmpty(infoString))
                            {
                                throw new CanNotFindDataException(name);
                            }
                            else
                            {
                                XmlDocument infoDoc = XmlFactory.LoadXml(infoString);
                                string xpath = string.Format("//{0}", name.Replace(".", "/"));
                                if (condition != null && condition.Count > 0)
                                {
                                    string conString = string.Empty;
                                    if (condition.ContainsKey("position"))
                                    {
                                        conString = condition["position"];
                                    }
                                    else
                                    {
                                        foreach (KeyValuePair<string, string> kv in condition)
                                        {
                                            if (string.Equals(kv.Key, "position")) continue;

                                            if (string.IsNullOrEmpty(conString))
                                            {
                                                if (kv.Value != null && kv.Value.StartsWith("@"))
                                                {
                                                    conString += string.Format("@{0}='{1}'", kv.Key, kv.Value.Remove(0, 1));
                                                }
                                                else
                                                {
                                                    conString = string.Format("{0}='{1}'", kv.Key, kv.Value);
                                                }
                                            }
                                            else
                                            {
                                                if (kv.Value != null && kv.Value.StartsWith("@"))
                                                {
                                                    conString += string.Format("{2} and @{0}='{1}'", kv.Key, kv.Value.Remove(0, 1), conString);
                                                }
                                                else
                                                {
                                                    conString = string.Format("{2} and {0}='{1}'", kv.Key, kv.Value, conString);
                                                }
                                            }
                                        }
                                    }

                                    xpath = string.Format("{0}[{1}]/{2}", xpath.Substring(0, xpath.LastIndexOf('/')), conString, xpath.Substring(xpath.LastIndexOf('/')));
                                }

                                XmlNode node = infoDoc.SelectSingleNode(xpath);
                                if (node == null)
                                {
                                    throw new CanNotFindDataException(name);
                                }
                                return node.InnerXml;
                            }
                        }
                        else
                        {
                            throw new CanNotFindDataException(name);
                        }
                    }
                    else
                    {
                        throw new CanNotFindDataException(name);
                    }
                }
            }
            set
            {
                if (data.ContainsKey(name))
                {
                    data[name] = value;
                }
                else
                {
                    data.Add(name, value);
                }
            }
        }

        public void Get()
        {
            //string path = Path.Combine(Config.WorkingDirectory, "Temp\\taist.temp");

            if (!File.Exists(path))
            {
                throw new CanNotFindDataException(path);
            }

            XmlDocument doc = XmlFactory.LoadXml(path);

            XmlNode xn = doc.SelectSingleNode(string.Format("//Cache[Bh='{0}']", Bh));
            if (xn == null)
            {
                throw new CanNotFindDataException(Bh);
            }
            LogManager.Debug(xn.OuterXml);
            Cache cache = DataFactory.XmlToObject<Cache>(xn);
            this.data = cache.data;
        }

        public void Save()
        {
            //string path = Path.Combine(Config.WorkingDirectory, "Temp\\taist.temp");

            XmlDocument doc = new XmlDocument();
            if (!File.Exists(path))
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                using (FileStream fs = File.Create(path))
                {
                    fs.Close();
                }
                doc.LoadXml("<?xml version=\"1.0\" encoding=\"gbk\"?><Data></Data>");
            }
            else
            {
                doc.Load(path);
            }

            XmlDocument cache = XmlFactory.LoadXml(DataFactory.ObjectToXml(this));
            
            XmlNode cacheNode = doc.ImportNode(cache.DocumentElement, true);
            XmlNode xn = doc.SelectSingleNode(string.Format("//Cache[Bh='{0}']", Bh));
            if (xn == null)
            {
                doc.DocumentElement.AppendChild(cacheNode);
            }
            else
            {
                foreach (XmlNode tmp in cache.DocumentElement.ChildNodes)
                {
                    XmlNode cacheChild = cacheNode.SelectSingleNode(tmp.Name);
                    XmlNode child = xn.SelectSingleNode(cacheChild.Name);
                    if (child == null)
                    {
                        xn.AppendChild(cacheChild);
                    }
                    else
                    {
                        xn.ReplaceChild(cacheChild, child);
                    }
                }
            }
            doc.Save(path);
        }

        public override string ToString()
        {
            //string path = Path.Combine(Config.WorkingDirectory, "Temp\\taist.temp");

            if (!File.Exists(path))
            {
                throw new CanNotFindDataException(Bh);
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode xn = doc.SelectSingleNode(string.Format("//Cache[Bh='{0}']", Bh));
            if (xn == null)
            {
                throw new CanNotFindDataException(Bh);
            }
            return xn.InnerXml;
        }
    }
}
