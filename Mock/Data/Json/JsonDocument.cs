
namespace Mock.Data
{
    using System;

    using Mock.Data.Exception;
    public class JsonDocument : JsonNode
    {
        public JsonDocument()
        {
            _name = "#document";
            _id = Guid.NewGuid().ToString();
            _isDocument = true;
            this.ParentNode = this;
            nodeType = JsonNodeType.DOCUMENT;
        }

        public JsonDocument(string rootName)
            : this()
        {
            _rootName = rootName;
        }

        public void LoadJson(string jsonString)
        {
            OuterJson = jsonString;
        }

        public void LoadXml(string xmlString)
        {
            OuterXml = xmlString;
        }

        private string _rootName = "Joot";
        public string RootName
        {
            get
            {
                return _rootName;
            }
            set
            {
                _rootName = value;
            }
        }

        public JsonNode CreateNode(string name)
        {
            JsonNode node = new JsonNode();
            node.Name = name;
            node.Id = _id;
            node.NodeType = JsonNodeType.NODE;
            return node;
        }

        public JsonNode CreateArray(string name)
        {
            JsonNode node = new JsonNode();
            node.Name = name;
            node.Id = _id;
            node.NodeType = JsonNodeType.ARRAY;
            return node;
        }

        public JsonNode CreateText(string name)
        {
            JsonNode node = new JsonNode();
            node.Name = name;
            node.Id = _id;
            node.NodeType = JsonNodeType.TEXT;
            return node;
        }

        public override JsonNode this[string name]
        {
            get
            {
                JsonNode root = _childs[0];
                if (root.NodeType == JsonNodeType.ARRAY && root.ChildNodes.Count > 0 && !string.Equals(name, _rootName))
                {
                    root = root.ChildNodes[0];
                }
                foreach (JsonNode node in root.ChildNodes)
                {
                    if (string.Equals(node.Name, name))
                    {
                        return node;
                    }
                }
                
                throw new CanNotFindDataException(name);
            }
        }

        public override JsonNode this[int index]
        {
            get
            {
                JsonNode root = _childs[0];
                if (root.ChildNodes.Count > index)
                {
                    return root.ChildNodes[index];
                }
                throw new CanNotFindDataException("index " + index);
            }
        }
    }

}
