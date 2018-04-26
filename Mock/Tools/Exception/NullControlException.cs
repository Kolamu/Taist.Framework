namespace Mock.Tools.Exception
{
    /// <summary>
    /// 控件为空异常
    /// </summary>
    public class NullControlException : TaistException
    {
        /// <summary>
        /// 构造控件为空异常的新实例
        /// </summary>
        /// <param name="controlName"></param>
        public NullControlException(string controlName) : base(string.Format("There is no Control named {0}", controlName)) { }
        
        /// <summary>
        /// 构造控件为空异常的新实例
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="controlName"></param>
        public NullControlException(string windowName, string controlName) : base(string.Format("There is no Control named {0} in {1} window", controlName, windowName)) { }
        
        /// <summary>
        /// 构造控件为空异常的新实例
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="elementName"></param>
        /// <param name="controlName"></param>
        public NullControlException(string windowName,string elementName, string controlName) : base(string.Format("There is no Control named {0} in {1} element in {2} window", controlName, elementName, windowName)) { }
    }
}
