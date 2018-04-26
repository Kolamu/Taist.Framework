namespace Mock.Data
{
    using System.Collections.Generic;
    using Mock.Data.Attributes;
    public class WebElementInfo
    {
        /// <summary>
        /// 构造新对象
        /// </summary>
        public WebElementInfo()
        {
            this.FriendlyName = string.Empty;
            this.Description = string.Empty;
            this.Inherit = false;
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
        /// 继承属性
        /// </summary>
        [FieldProperty("Inherit", true)]
        public bool Inherit
        {
            get;
            set;
        }

        /// <summary>
        /// 控件ID
        /// </summary>
        [FieldProperty("elementId", true)]
        public string elementId
        {
            get;
            set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        [FieldProperty("TagName", false)]
        public string TagName
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

        private Dictionary<string, string> _attributes = new Dictionary<string,string>();

        /// <summary>
        /// 属性字典
        /// </summary>
        [FieldProperty(isIgnore=true)]
        public Dictionary<string, string> Attributes { get { return _attributes; } }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="node"></param>
        public void setProperty(System.Xml.XmlNode node)
        {
            if (_attributes == null)
            {
                _attributes = new Dictionary<string, string>();
            }

            if (_attributes.ContainsKey(node.Name))
            {
                _attributes[node.Name] = node.InnerText;
            }
            else
            {
                _attributes.Add(node.Name, node.InnerText);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void setProperty(string name, string value)
        {
            if (_attributes == null)
            {
                _attributes = new Dictionary<string, string>();
            }

            if (_attributes.ContainsKey(name))
            {
                _attributes[name] = value;
            }
            else
            {
                _attributes.Add(name, value);
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> getProperty()
        {
            return _attributes;
        }
    }
}
