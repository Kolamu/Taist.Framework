
namespace Mock.Data
{
    using System;
    using System.Xml;
    using System.Collections.Generic;
    using System.Text;

    using Mock.Data.Exception;
    public enum JsonNodeType
    {
        ARRAY,
        NODE,
        TEXT,
        DOCUMENT
    }

    public class JsonNode
    {
        protected string _id = null;
        protected string _name = null;
        protected bool _isDocument = false;
        protected JsonNodeList _childs = new JsonNodeList();

        private JsonNode _parentNode = null;
        private JsonDocument _documentNode = null;
        protected JsonNodeType nodeType = JsonNodeType.NODE;
        public JsonNodeList ChildNodes { get { return _childs; } }
        private string _value = null;

        public JsonDocument DocumentNode { get { return _documentNode; } }

        public JsonNode ParentNode { get { return _parentNode; } internal set { _parentNode = value; if (_parentNode._isDocument) _documentNode = (JsonDocument)value; else _documentNode = value._documentNode; } }

        public string Name { get { return _name; } internal set { _name = value; } }

        public bool IsStringValue { get; set; }

        public virtual JsonNode this[string name]
        {
            get
            {
                foreach (JsonNode node in _childs)
                {
                    if (string.Equals(node._name, name))
                    {
                        return node;
                    }
                }
                throw new CanNotFindDataException(name);
            }
        }

        public virtual JsonNode this[int index]
        {
            get
            {
                if (_childs.Count > index)
                {
                    return _childs[index];
                }
                throw new CanNotFindDataException("index " + index);
            }
        }

        public string Value
        {
            get
            {
                switch (nodeType)
                {
                    case JsonNodeType.TEXT:
                        {
                            string tmp = _value.Trim('\"');
                            if (string.Equals("null", tmp.ToLower()))
                            {
                                return null;
                            }
                            else
                            {
                                return tmp;
                            }
                        }
                    case JsonNodeType.DOCUMENT:
                        {
                            return string.Join("", _childs.ConvertAll(x => x.OuterJson));
                        }
                    case JsonNodeType.NODE:
                        {
                            return string.Join("", _childs.ConvertAll(x => x.OuterJson));
                        }
                    case JsonNodeType.ARRAY:
                        {
                            return string.Join("", _childs.ConvertAll(x => x.OuterJson));
                        }
                    default:
                        {
                            throw new System.Exception();
                        }
                }
            }
            set
            {
                if (nodeType == JsonNodeType.TEXT)
                {
                    _value = value;
                }
                else
                {
                    throw new InvalidDataTypeException(nodeType.ToString());
                }
            }
        }

        public JsonNodeType NodeType
        {
            get
            {
                return nodeType;
            }
            internal set
            {
                nodeType = value;
            }
        }

        public virtual string OuterJson
        {
            get
            {
                return getJson();
            }
            set
            {
                setJson(value);
            }
        }

        public virtual string OuterXml
        {
            get
            {
                return getXml();
            }
            set
            {
                setXml(value);
            }
        }

        public XmlNode XmlNode
        {
            get
            {
                return XmlFactory.LoadXml(OuterXml).DocumentElement;
            }
        }

        public void AppendChild(JsonNode node)
        {
            if (string.Equals(node._id, _id))
            {
                node.ParentNode = this;
                _childs.Add(node);
            }
            else
            {
                throw new System.Exception();
            }
        }

        private List<string> parseArray(string jsonString)
        {
            List<string> jArray = new List<string>();
            if (jsonString.StartsWith("[") && jsonString.EndsWith("]"))
            {
                string json = jsonString.Substring(1, jsonString.Length - 2);
                List<char> stack = new List<char>();
                StringBuilder sb = new StringBuilder();

                foreach (char c in json)
                {
                    sb.Append(c);
                    switch (c)
                    {
                        case '{':
                            {
                                stack.Insert(0, c);
                                break;
                            }
                        case '}':
                            {
                                if (stack[0] == '{')
                                {
                                    stack.RemoveAt(0);
                                }
                                else
                                {
                                    throw new System.Exception();
                                }
                                break;
                            }
                        case '[':
                            {
                                stack.Insert(0, c);
                                break;
                            }
                        case ']':
                            {
                                if (stack[0] == '[')
                                {
                                    stack.RemoveAt(0);
                                }
                                else
                                {
                                    throw new System.Exception();
                                }
                                break;
                            }
                        case ',':
                            {
                                if (stack.Count == 0)
                                {
                                    sb.Remove(sb.Length - 1, 1);
                                    jArray.Add(sb.ToString());
                                    sb.Clear();
                                }
                                break;
                            }

                    }
                }
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    jArray.Add(sb.ToString());
                }
                nodeType = JsonNodeType.ARRAY;
            }
            else if (jsonString.StartsWith("{") && jsonString.EndsWith("}"))
            {
                jArray.Add(jsonString);
            }
            else
            {
                nodeType = JsonNodeType.TEXT;
                return null;
            }
            return jArray;
        }

        private Dictionary<string, string> parseObject(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }
            if (!jsonString.Contains(":"))
            {
                return null;
            }

            if (jsonString.StartsWith("{") && jsonString.EndsWith("}"))
            {
                jsonString = jsonString.Substring(1, jsonString.Length - 2);
            }
            Dictionary<string, string> keyValue = new Dictionary<string, string>();

            List<char> stack = new List<char>();
            StringBuilder sb = new StringBuilder();

            string key = null;
            foreach (char c in jsonString)
            {
                sb.Append(c);
                switch (c)
                {
                    case '{':
                        {
                            stack.Insert(0, c);
                            break;
                        }
                    case '}':
                        {
                            if (stack[0] == '{')
                            {
                                stack.RemoveAt(0);
                            }
                            else
                            {
                                throw new System.Exception();
                            }
                            break;
                        }
                    case '[':
                        {
                            stack.Insert(0, c);
                            break;
                        }
                    case ']':
                        {
                            if (stack[0] == '[')
                            {
                                stack.RemoveAt(0);
                            }
                            else
                            {
                                throw new System.Exception();
                            }
                            break;
                        }
                    case ':':
                        {
                            if (string.IsNullOrEmpty(key))
                            {
                                sb.Remove(sb.Length - 1, 1);
                                key = sb.ToString().Trim().Trim('"');
                                sb.Clear();
                            }
                            break;
                        }
                    case '"':
                        {
                            if (!string.IsNullOrEmpty(key))
                            {
                                if (stack.Count == 0)
                                {
                                    stack.Insert(0, '"');
                                }
                                else if (stack[0] == '"')
                                {
                                    stack.RemoveAt(0);
                                }
                                else
                                {
                                    stack.Insert(0, '"');
                                }
                            }
                            break;
                        }
                    case ',':
                        {
                            if (stack.Count == 0)
                            {
                                sb.Remove(sb.Length - 1, 1);
                                keyValue.Add(key, sb.ToString());
                                sb.Clear();
                                key = null;
                            }
                            break;
                        }

                }
            }

            if (stack.Count != 0) throw new System.Exception();

            if (!string.IsNullOrEmpty(key))
            {
                keyValue.Add(key, sb.ToString());
            }
            return keyValue;
        }

        internal string Id
        {
            set
            {
                _id = value;
            }
        }

        private string getJson()
        {
            switch (nodeType)
            {
                case JsonNodeType.TEXT:
                    {
                        if (_parentNode.nodeType == JsonNodeType.ARRAY)
                        {
                            return _value;
                        }
                        if (_value == null || string.Equals(_value, "null"))
                        {
                            return string.Format("\"{0}\":null", _name);
                        }
                        else
                        {
                            if (IsStringValue)
                            {
                                return string.Format("\"{0}\":\"{1}\"", _name, _value.Trim('\"'));
                            }
                            else
                            {
                                return string.Format("\"{0}\":{1}", _name, _value);
                            }
                        }
                    }
                case JsonNodeType.DOCUMENT:
                    {
                        if (_childs.Count == 0)
                        {
                            return "{}";
                        }
                        else if (_childs.Count == 1)
                        {
                            return _childs[0].OuterJson;//"{" + string.Join(",", _childs.ConvertAll(x => x.OuterJson)) + "}";
                        }
                        else
                        {
                            return "[" + string.Join(",", _childs.ConvertAll(x => x.OuterJson)) + "]";
                        }
                    }
                case JsonNodeType.NODE:
                    {
                        if (string.Equals(_name, _documentNode.RootName))
                        {
                            return "{" + string.Join(",", _childs.ConvertAll(x => x.OuterJson)) + "}";
                        }
                        return "\"" + _name.Trim('\"') + "\":{" + string.Join(",", _childs.ConvertAll(x => x.OuterJson)) + "}";
                    }
                case JsonNodeType.ARRAY:
                    {
                        if (string.Equals(_name, "#document"))
                        {
                            return "[" + string.Join(",", _childs.ConvertAll(x => x.OuterJson)) + "]";
                        }
                        return "\"" + _name.Trim('\"') + "\":[" + string.Join(",", _childs.ConvertAll(x => x.OuterJson)) + "]";
                    }
                default:
                    {
                        throw new InvalidDataTypeException(nodeType.ToString());
                    }
            }
        }

        private void setJson(string jsonString)
        {
            if (nodeType == JsonNodeType.DOCUMENT)
            {
                JsonDocument doc = (JsonDocument)this;
                _childs.Clear();
                JsonNode node = doc.CreateNode(doc.RootName);
                AppendChild(node);
                node.OuterJson = jsonString;
                return;
            }
            _childs.Clear();
            List<string> jArray = parseArray(jsonString);
            switch (nodeType)
            {
                case JsonNodeType.TEXT:
                    {
                        _value = jsonString;
                        break;
                    }
                case JsonNodeType.ARRAY:
                    {
                        foreach (string s in jArray)
                        {
                            JsonNode node = _documentNode.CreateNode(_name);
                            AppendChild(node);
                            node.OuterJson = s;
                        }
                        break;
                    }
                case JsonNodeType.NODE:
                    {
                        Dictionary<string, string> dic = parseObject(jArray[0]);
                        if (dic != null)
                        {
                            foreach (KeyValuePair<string, string> kv in dic)
                            {
                                JsonNode node = _documentNode.CreateNode(kv.Key);
                                AppendChild(node);
                                node.OuterJson = kv.Value;
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new InvalidDataTypeException(nodeType.ToString());
                    }
            }
        }

        private string getXml()
        {
            string xName = _name;
            string xValue;
            switch (nodeType)
            {
                case JsonNodeType.TEXT:
                    {
                        if (_parentNode.nodeType == JsonNodeType.ARRAY)
                        {
                            xName = _parentNode._name;
                        }
                        if (_value == null || string.Equals(_value, "null"))
                        {
                            xValue = null;
                        }
                        else
                        {
                            xValue = _value.Trim('\"');
                        }
                        break;
                    }
                case JsonNodeType.DOCUMENT:
                    {
                        //xName = "Json";
                        //xValue = string.Join("", _childs.ConvertAll(x => x.OuterXml));
                        //break;
                        if (_childs[0].nodeType == JsonNodeType.ARRAY)
                        {
                            return string.Format("<JsonDocument>{0}</JsonDocument>", _childs[0].OuterXml);
                        }
                        else
                        {
                            return _childs[0].OuterXml;
                        }
                    }
                case JsonNodeType.NODE:
                    {
                        if (_parentNode.nodeType == JsonNodeType.ARRAY)
                        {
                            xName = _parentNode._name;
                        }
                        xValue = string.Join("", _childs.ConvertAll(x => x.OuterXml));
                        break;
                    }
                case JsonNodeType.ARRAY:
                    {
                        return string.Join("", _childs.ConvertAll(x => x.OuterXml));
                    }
                default:
                    {
                        throw new System.Exception();
                    }
            }

            if (xValue == null)
            {
                return string.Format("<{0} />", xName);
            }

            XmlNode dataNode = XmlFactory.LoadXml(string.Format("<{0}></{0}>", xName)).DocumentElement;
            if (nodeType == JsonNodeType.TEXT)
            {
                dataNode.InnerText = xValue;
            }
            else
            {
                dataNode.InnerXml = xValue;
            }
            return dataNode.OuterXml;
        }

        private void setXml(string xmlString)
        {
            if (xmlString == null)
            {
                nodeType = JsonNodeType.TEXT;
                _value = null;
                return;
            }

            if (!xmlString.Trim().StartsWith("<") || !xmlString.EndsWith(">"))
            {
                nodeType = JsonNodeType.TEXT;
                _value = "\"" + xmlString + "\"";
                return;
            }

            XmlNode rootNode = XmlFactory.LoadXml(xmlString).DocumentElement;
            if (nodeType == JsonNodeType.DOCUMENT)
            {
                JsonDocument doc = (JsonDocument)this;
                doc.RootName = rootNode.Name;
                _childs.Clear();
                JsonNode node = doc.CreateNode(doc.RootName);
                AppendChild(node);
                node.OuterXml = rootNode.OuterXml;
                return;
            }
            else
            {
                _name = rootNode.Name;
            }
            if (rootNode.Attributes.Count > 0)
            {
                foreach (XmlAttribute xa in rootNode.Attributes)
                {
                    XmlElement xe = rootNode.OwnerDocument.CreateElement(xa.Name);
                    rootNode.AppendChild(xe);
                    xe.InnerText = xa.Value;
                }
            }

            if (rootNode.ChildNodes.Count > 1)
            {
                Dictionary<string, List<XmlNode>> nodeList = new Dictionary<string, List<XmlNode>>();
                foreach (XmlNode childNode in rootNode.ChildNodes)
                {
                    if (nodeList.ContainsKey(childNode.Name))
                    {
                        nodeList[childNode.Name].Add(childNode);
                    }
                    else
                    {
                        List<XmlNode> tmp = new List<XmlNode>();
                        tmp.Add(childNode);
                        nodeList.Add(childNode.Name, tmp);
                    }
                }

                if(nodeList.Count == 1)
                {
                    nodeType = JsonNodeType.ARRAY;
                    foreach(XmlNode xn in rootNode.ChildNodes)
                    {
                        setChild(xn);
                    }
                }
                else
                {
                    nodeType = JsonNodeType.NODE;
                    foreach (KeyValuePair<string, List<XmlNode>> kv in nodeList)
                    {
                        if (kv.Value.Count > 1)
                        {
                            JsonNode node = _documentNode.CreateArray(kv.Key);
                            AppendChild(node);
                            foreach (XmlNode xn in kv.Value)
                            {
                                JsonNode cnode = _documentNode.CreateNode(xn.Name);
                                node.OuterXml = xn.OuterXml;
                                AppendChild(cnode);
                            }
                        }
                        else
                        {
                            setChild(kv.Value[0]);
                        }
                    }
                }
            }
            else if (rootNode.ChildNodes.Count == 0)
            {
                nodeType = JsonNodeType.TEXT;
                if (rootNode.OuterXml.Contains("/>"))
                {
                    _value = null;
                }
                else
                {
                    _value = "\"\"";
                }
            }
            else
            {
                XmlNode childNode = rootNode.ChildNodes[0];
                if (childNode.NodeType == XmlNodeType.Text || childNode.NodeType == XmlNodeType.CDATA)
                {
                    nodeType = JsonNodeType.TEXT;
                    _value = "\"" + childNode.InnerText + "\"";
                }
                else
                {
                    setChild(childNode);
                }
            }
        }

        private void setChild(XmlNode xNode)
        {
            JsonNode node = _documentNode.CreateNode(xNode.Name);
            AppendChild(node);
            node.OuterXml = xNode.OuterXml;
        }
    }
}
