namespace Mock.Data
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Reflection;
    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    using System.Collections.Generic;
    /// <summary>
    /// 表示自动化测试数据接口
    /// </summary>
    public abstract class IFormatData
    {
        private string dataFilePath = null;
        public string DataFilePath
        {
            get
            {
                return dataFilePath;
            }
        }
        protected string NodeName = null;
        /// <summary>
        /// 数据编号
        /// </summary>
        public virtual string Bh { get; set; }

        /// <summary>
        /// 关联数据的编号
        /// </summary>
        public virtual string RelateDataBh { get; set; }

        /// <summary>
        /// 参考数据编号
        /// </summary>
        public virtual string DefaultDataBh { get; set; }

        /// <summary>
        /// 从格式文档中获取数据，格式文档对应的数据类型必须继承此接口
        /// </summary>
        /// <param name="doc">格式文档数据中对应的XmlDocument对象</param>
        /// <param name="condition">查询数据的条件</param>
        /// <returns></returns>
        public virtual IFormatData FromXml(XmlNode doc, Dictionary<string, string> condition)
        {
            TaistType type = TaistType.GetDataType(this);
            List<string> candidateName = new List<string>();
            SetDefault(NodeName, type.FriendlyName);
            candidateName.Add(NodeName);
            candidateName.Add(type.Name);
            candidateName.AddRange(type.InterfaceName);
            XmlNode dataNode = null;
            foreach (string typeName in candidateName)
            {
                string conditionString = DataFactory.GetXmlConditionString(typeName, condition);
                dataNode = doc.SelectSingleNode(conditionString);
                if (dataNode != null)
                {
                    break;
                }
            }

            if (dataNode == null)
            {
                throw new CanNotFindDataException(type.FriendlyName);
            }

            this.InputNode = dataNode;

            dataFilePath = FileFactory.GetRealPath(dataNode.OwnerDocument.BaseURI);
            return (IFormatData)DataFactory.XmlToObject(dataNode, this);
        }

        /// <summary>
        /// 将数据转化为Xml格式
        /// </summary>
        /// <returns></returns>
        public virtual string ToXml()
        {
            return DataFactory.ObjectToXml(this);
        }

        /// <summary>
        /// 数据赋值后执行的初始化操作
        /// </summary>
        public virtual void Init() { }

        public virtual T Clone<T>()
        {
            object obj = this;
            return (T)DataFactory.CopyObject<T>((T)obj);
        }

        public universal SetDefault(universal Property, object value)
        {
            if (Property == null || Property.StringValue == null)
            {
                return new universal(value);
            }
            else
            {
                return Property;
            }
        }

        /// <summary>
        /// 获取处理后的数据
        /// </summary>
        /// <param name="propertyName">字段名称</param>
        /// <param name="originalValue">字段原始值</param>
        /// <param name="processedValue">处理后的字段值</param>
        /// <returns>如果原始值来自输入数据，返回原始值，否则，返回处理后的值</returns>
        public universal GetProcessedValue(string propertyName, universal originalValue, universal processedValue)
        {
            if (IsFromUser(propertyName))
            {
                return originalValue;
            }
            else
            {
                return processedValue;
            }
        }

        /// <summary>
        /// 用户配置数据路径，通常为Taist.Lib\Data\User目录下存放数据的文件名
        /// </summary>
        protected string UserDataName { get; set; }

        /// <summary>
        /// 获取用户配置中的数据项
        /// </summary>
        /// <param name="name">数据项名称</param>
        /// <param name="xpath">//Fpj[Zl='0']</param>
        /// <returns></returns>
        protected string getUserConfig(string name, string xpath = null)
        {
            LogManager.Debug(string.Format("name : {0};xpath : {1}", name, xpath));
            try
            {
                if (UserDataName == null)
                {
                    LogManager.Debug("Not set UserDataName, return null");
                    return null;
                }
                string path = Path.Combine(Config.WorkingDirectory, "Data\\User", UserDataName);
                LogManager.Debug("UserConfig path : " + path);
                if (!File.Exists(path)) return null;
                XmlDocument doc = XmlFactory.LoadXml(path);
                if (string.IsNullOrEmpty(xpath))
                {
                    XmlNode node = doc.SelectSingleNode(string.Format("//{0}", name));
                    if (node == null)
                    {
                        LogManager.DebugFormat(string.Format("Can not find node {0} from {1}", name, path), NoteType.DEBUG);
                        return null;
                    }
                    else
                    {
                        return node.InnerText;
                    }
                }
                else
                {
                    XmlNode parentNode = doc.SelectSingleNode(xpath);
                    if (parentNode == null)
                    {
                        LogManager.Debug(string.Format("Can not find node {0} from {1}", xpath, path));
                        return null;
                    }
                    else
                    {
                        XmlNode node = parentNode.SelectSingleNode(name);
                        if (node == null)
                        {
                            LogManager.Debug(string.Format("Can not find node {0} from {1}", name, path));
                            return null;
                        }
                        else
                        {
                            return node.InnerText;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
                return null;
            }
        }

        /// <summary>
        /// 设置用户配置数据项
        /// </summary>
        /// <param name="name">配置项名称</param>
        /// <param name="value">配置项值</param>
        /// <param name="xpath"></param>
        protected void setUserConfig(string name, string value, string xpath)
        {
            LogManager.Debug(string.Format("name : {0};value : {1};xpath : {2}", name, value, xpath), 2);
            XmlDocument doc = null;
            try
            {
                doc = XmlFactory.LoadXml(Path.Combine(Config.WorkingDirectory, "Data\\User", UserDataName));
            }
            catch(System.Exception ex)
            {
                LogManager.Debug(ex.Message);
                return;
                //doc = new XmlDocument();
                //doc.LoadXml("<?xml version=\"1.0\" encoding=\"gbk\"?><Data></Data>");
            }
            LogManager.Debug("UserConfig path : " + Path.Combine(Config.WorkingDirectory, "Data\\User", UserDataName));
            if (string.IsNullOrEmpty(xpath))
            {
                XmlNode dataNode = doc.SelectSingleNode(string.Format("//{0}", name));
                if (dataNode == null)
                {
                    dataNode = doc.CreateElement(name);
                    dataNode.InnerText = value;
                    doc.DocumentElement.AppendChild(dataNode);
                }
                else
                {
                    dataNode.InnerText = value;
                }
            }
            else
            {
                XmlNode dataNode = doc.SelectSingleNode(xpath);
                if (dataNode == null)
                {
                    LogManager.Debug("dataNode is null");
                    throw new CanNotFindDataException(xpath);
                }
                else
                {
                    XmlNode tmpNode = dataNode.SelectSingleNode(name);
                    if (tmpNode == null)
                    {
                        LogManager.DebugFormat(dataNode.OuterXml, NoteType.DEBUG);
                        throw new CanNotFindDataException(name);
                    }
                    else
                    {
                        tmpNode.InnerText = value;
                    }
                }
            }

            doc.Save(Path.Combine(Config.WorkingDirectory, "Data\\User", UserDataName));
        }

        private string inputNodeString = null;
        /// <summary>
        /// 设置用户输入的数据节点
        /// </summary>
        [FieldProperty(isIgnore = true)]
        public XmlNode InputNode
        {
            get
            {
                if (string.IsNullOrEmpty(inputNodeString))
                {
                    return XmlFactory.LoadXml(string.Format("<{0} />", this.GetType().Name)).DocumentElement;
                }
                else
                {
                    return XmlFactory.LoadXml(inputNodeString).DocumentElement;
                }
            }
            set
            {
                if (value == null) inputNodeString = null;
                inputNodeString = value.OuterXml;
            }
        }

        public bool IsFromUser(string name)
        {
            if (InputNode == null) return false;
            XmlNode node = InputNode.SelectSingleNode(name);
            return node != null;
        }

    }
}
