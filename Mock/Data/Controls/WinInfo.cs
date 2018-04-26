namespace Mock.Data
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml;
    using System.Windows.Automation;

    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    /// <summary>
    /// 表示对象库中描述窗口的结构对象
    /// </summary>
    public class WinInfo : IFormatData
    {
        /// <summary>
        /// 构造对象
        /// </summary>
        public WinInfo()
        {
            Bh = string.Empty;
            FriendlyName = string.Empty;
            id = string.Empty;
            Name = string.Empty;
            Type = string.Empty;
            AutomationId = string.Empty;
            this.Description = string.Empty;
            this.Version = string.Empty;
        }

        /// <summary>
        /// 编号，此处等同于FriendlyName
        /// </summary>
        [FieldProperty("Bh", false)]
        public override string Bh
        {
            get
            {
                return FriendlyName;
            }
            set
            {
                FriendlyName = value;
            }
        }
        private List<ElementInfo> _elementList = null;

        /// <summary>
        /// Id号
        /// </summary>
        [FieldProperty("id", true)]
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// 别名
        /// </summary>
        [FieldProperty("FriendlyName", true)]
        public string FriendlyName
        {
            get;
            set;
        }

        /// <summary>
        /// 实际名称
        /// </summary>
        [FieldProperty("Name", false)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        [FieldProperty("Type", false)]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// 窗口唯一标识
        /// </summary>
        [FieldProperty("AutomationId", false)]
        public string AutomationId
        {
            get;
            set;
        }

        /// <summary>
        /// 说明
        /// </summary>
        [FieldProperty("Description", false)]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 控件集合
        /// </summary>
        [FieldProperty(isIgnore= true)]
        public List<ElementInfo> ElementInfo
        {
            get
            {
                return _elementList;
            }
            set
            {
                _elementList = value;
            }
        }

        /// <summary>
        /// 版本标识
        /// </summary>
        [FieldProperty("Version", true)]
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// 从格式文档中获取数据
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public override IFormatData FromXml(XmlNode doc, Dictionary<string, string> condition)
        {
            string conditionString = DataFactory.GetXmlConditionString("WinInfo", condition);
            XmlNodeList winInfoNodeList = doc.SelectNodes(conditionString);
            if (winInfoNodeList == null || winInfoNodeList.Count < 1)
            {
                throw new CanNotFindDataException(string.Format("WinInfo {0}", conditionString));
            }
            else if (winInfoNodeList.Count > 1)
            {
                throw new NotUniqueDataException(string.Format("WinInfo {0}", conditionString));
            }

            XmlNode winInfoNode = winInfoNodeList[0];
            WinInfo winfo = DataFactory.XmlToObject<WinInfo>(winInfoNode);

            XmlNodeList elementInfoNodeList = winInfoNode.SelectNodes("ElementInfo");
            foreach (XmlNode eiNode in elementInfoNodeList)
            {
                ElementInfo ei = DataFactory.XmlToObject<ElementInfo>(eiNode);
                winfo.AddElementInfo(ei);
            }
            return winfo;
        }

        /// <summary>
        /// 将数据转化为Xml格式
        /// </summary>
        /// <returns></returns>
        public override string ToXml()
        {
            XmlDocument winInfoDocument = new XmlDocument();
            string winInfoString = DataFactory.ObjectToXml(this, false);
            winInfoDocument.LoadXml(winInfoString);
            XmlElement winInfoNode = winInfoDocument.DocumentElement;

            if (_elementList != null && _elementList.Count > 0)
            {
                XmlDocument elementInfoDocument = new XmlDocument();
                foreach (ElementInfo ei in _elementList)
                {
                    elementInfoDocument.LoadXml(DataFactory.ObjectToXml(ei, false));
                    XmlNode elementNode = winInfoDocument.ImportNode(elementInfoDocument.DocumentElement, true);
                    winInfoNode.AppendChild(elementNode);
                }
            }
            return winInfoDocument.OuterXml;
        }

        /// <summary>
        /// 添加子控件的描述
        /// </summary>
        /// <param name="element"></param>
        public void AddElementInfo(ElementInfo element)
        {
            if (_elementList == null)
            {
                _elementList = new List<ElementInfo>();
            }
            _elementList.Add(element);
        }

        /// <summary>
        /// 获取子控件的描述
        /// </summary>
        /// <returns></returns>
        public List<ElementInfo> GetElementInfo()
        {
            return _elementList;
        }

        public override void Init()
        {
        }
    }
}
