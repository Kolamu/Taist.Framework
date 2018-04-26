namespace Mock.Data.Exception
{
    /// <summary>
    /// 没有继承接口时触发此异常
    /// </summary>
    public class NotImplementingInterfaceException : TaistException
    {
        /// <summary>
        /// 构造没有继承接口异常的新实例
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="interfaceName">接口名</param>
        public NotImplementingInterfaceException(string className, string interfaceName) : base(string.Format("{0} class does not implementing {1} interface.", className, interfaceName)) { }
    }
}
