namespace Mock.Data
{
    using System.Xml;
    using Mock.Data.Exception;
    using System.IO;
    using Mock.Tools.Exception;
    public class XmlFactory
    {
        /// <summary>
        /// 所有XML数据的
        /// </summary>
        public static string XmlRootString
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"gbk\"?><Data></Data>";
            }
        }

        #region 加载XML
        /// <summary>
        /// 加载XML文件/XML格式字符串到XmlDocument
        /// </summary>
        /// <param name="arg">XML文件路径/XML格式字符串</param>
        /// <returns></returns>
        public static XmlDocument LoadXml(string arg)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                if (File.Exists(arg))
                {
                    doc.Load(arg);
                }
                else
                {
                    doc.LoadXml(arg);
                }
            }
            catch
            {
                throw new XmlFormatErrorException(arg);
            }

            return doc;
        }
        #endregion

        #region 忽略大小写查询XML子节点
        public static XmlNode SelectSingleNodeIgnoreCase(XmlNode parent, string name)
        {
            if (parent == null)
            {
                throw new CanNotFindNodeException("ParentName", name);
            }
            string xpath = string.Format("*[translate(name(), 'abcdefghijklmnopqrsquvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ') = '{0}']", name.ToUpper());
            XmlNode child = parent.SelectSingleNode(xpath);

            if (child == null)
            {
                throw new CanNotFindNodeException(parent.Name, name);
            }
            return child;
        }

        public static XmlNodeList SelectNodesIgnoreCase(XmlNode parent, string name)
        {
            if (parent == null)
            {
                throw new CanNotFindNodeException("ParentName", name);
            }
            string xpath = string.Format("*[translate(name(), 'abcdefghijklmnopqrsquvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ') = '{0}']", name.ToUpper());
            XmlNodeList child = parent.SelectNodes(xpath);

            if (child == null || child.Count == 0)
            {
                throw new CanNotFindNodeException(parent.Name, name);
            }
            return child;
        }
        #endregion

        #region 判断XML是否含有子节点
        /// <summary>
        /// 判断XML是否含有子节点
        /// </summary>
        /// <param name="xn"></param>
        /// <returns></returns>
        public static bool HasChild(XmlNode xn)
        {
            if (xn == null) return false;
            if (xn is XmlComment) return false;
            if (xn is XmlDocument) return true;

            if (xn.ChildNodes.Count > 1) return true;

            if (xn.ChildNodes.Count == 0) return false;
            XmlNode child = xn.ChildNodes[0];
            switch (child.NodeType)
            {
                default:
                    return true;
                case XmlNodeType.CDATA:
                    return false;
                case XmlNodeType.Text:
                    return false;
                case XmlNodeType.XmlDeclaration:
                    return false;
            }
        }
        #endregion

    }
}
