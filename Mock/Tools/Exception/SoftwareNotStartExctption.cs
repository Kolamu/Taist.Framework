namespace Mock.Tools.Exception
{
    /// <summary>
    /// 软件未启动异常
    /// </summary>
    public class SoftwareNotStartExctption : TaistException
    {
        /// <summary>
        /// 构造软件未启动异常的新实例
        /// </summary>
        /// <param name="name"></param>
        public SoftwareNotStartExctption(string name) : base(string.Format("The software named {0} has not open", name)) { }
    }
}
