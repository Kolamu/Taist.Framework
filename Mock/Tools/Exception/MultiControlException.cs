namespace Mock.Tools.Exception
{
    /// <summary>
    /// 多个控件同时存在时触发此异常
    /// </summary>
    public class MultiControlException : TaistException
    {
        /// <summary>
        /// 构造多个控件同时存在异常的新实例
        /// </summary>
        /// <param name="controlName"></param>
        public MultiControlException(string controlName) : base(string.Format("Control named {0} has more than one", controlName)) { }
        
        /// <summary>
        /// 构造多个控件同时存在异常的新实例
        /// </summary>
        /// <param name="parentName"></param>
        /// <param name="controlName"></param>
        public MultiControlException(string parentName, string controlName) : base(string.Format("Control named {0} has more than one in {1} window", controlName, parentName)) { }

        /// <summary>
        /// 构造多个控件同时存在异常的新实例
        /// </summary>
        /// <param name="parentName"></param>
        /// <param name="controlName"></param>
        /// <param name="itemName"></param>
        public MultiControlException(string parentName, string controlName, string itemName) : base(string.Format("Control named {0} has more than one in {1} element in {2} window", controlName, parentName, itemName)) { }
    }
}
