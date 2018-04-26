namespace Mock.Tools.Exception
{
    using System;

    /// <summary>
    /// 未知异常
    /// </summary>
    public class CanNotGetPropertyException : TaistException
    {
        /// <summary>
        /// 构造未知异常的新实例
        /// </summary>
        /// <param name="propertyName"></param>
        public CanNotGetPropertyException(string propertyName) : base(string.Format("Can not get property named {0}", propertyName)) { }
    }
}
