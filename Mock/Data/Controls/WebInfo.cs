namespace Mock.Data
{
    using System;
    using System.Linq;
    using System.Xml;
    using System.Collections.Generic;

    using Mock.Data;
    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    public class WebInfo : IFormatData
    {
        /// <summary>
        /// 构造对象
        /// </summary>
        public WebInfo()
        {
            Bh = string.Empty;
            FriendlyName = string.Empty;
            id = string.Empty;
            Name = string.Empty;
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
        private List<WebElementInfo> _elementList = null;

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
        [FieldProperty(isIgnore = true)]
        public List<WebElementInfo> ElementInfo
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
            if (condition == null || condition.Count < 1)
            {
                return null;
            }

            List<string> keys = condition.Keys.ToList<string>();
            string conditionString = string.Empty;
            foreach (string key in keys)
            {
                if (conditionString == string.Empty)
                {
                    conditionString = string.Format("//WebInfo[{0}='{1}']", key, condition[key]);
                }
                else
                {
                    conditionString = string.Format("{0} and {1}='{2}']", conditionString.TrimEnd(']'), key, condition[key]);
                }
            }

            Type objType = typeof(WinInfo);

            XmlNodeList winInfoNodeList = doc.SelectNodes(conditionString);
            if (winInfoNodeList == null || winInfoNodeList.Count < 1)
            {
                throw new CanNotFindDataException(string.Format("WebInfo {0}", conditionString));
            }
            else if (winInfoNodeList.Count > 1)
            {
                throw new NotUniqueDataException(string.Format("WebInfo {0}", conditionString));
            }

            XmlNode winInfoNode = winInfoNodeList[0];
            WebInfo winfo = DataFactory.XmlToObject<WebInfo>(winInfoNode);

            XmlNodeList elementInfoNodeList = winInfoNode.SelectNodes("WebElementInfo");
            foreach (XmlNode eiNode in elementInfoNodeList)
            {
                WebElementInfo ei = DataFactory.XmlToObject<WebElementInfo>(eiNode);
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
            string winInfoString = DataFactory.ObjectToXml(this);
            winInfoDocument.LoadXml(winInfoString);
            XmlElement winInfoNode = winInfoDocument.DocumentElement;

            if (_elementList != null && _elementList.Count > 0)
            {
                XmlDocument elementInfoDocument = new XmlDocument();
                foreach (WebElementInfo ei in _elementList)
                {
                    elementInfoDocument.LoadXml(DataFactory.ObjectToXml(ei));
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
        public void AddElementInfo(WebElementInfo element)
        {
            if (_elementList == null)
            {
                _elementList = new List<WebElementInfo>();
            }
            _elementList.Add(element);
        }

        /// <summary>
        /// 获取子控件的描述
        /// </summary>
        /// <returns></returns>
        public List<WebElementInfo> GetElementInfo()
        {
            return _elementList;
        }
        public override void Init()
        {
        }
    }
}
