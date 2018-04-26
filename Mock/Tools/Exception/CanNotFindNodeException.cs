namespace Mock.Tools.Exception
{
    /// <summary>
    /// 无法找到节点时触发此异常
    /// </summary>
    public class CanNotFindNodeException : TaistException
    {
        /// <summary>
        /// 构造无法找到节点异常的新实例
        /// </summary>
        /// <param name="xmlName"></param>
        /// <param name="nodeName"></param>
        public CanNotFindNodeException(string xmlName, string nodeName) : base(string.Format("Can not find node {0} from {1}", nodeName, xmlName)) { }
        
        /// <summary>
        /// 构造无法找到节点异常的新实例
        /// </summary>
        /// <param name="xmlName"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeId"></param>
        public CanNotFindNodeException(string xmlName, string nodeName, int nodeId) : base(string.Format("Can not find node {0} from {2} where element id is {1}", nodeName, nodeId, xmlName)) { }
    }
}
