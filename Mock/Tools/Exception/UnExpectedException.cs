namespace Mock.Tools.Exception
{
    using System;

    /// <summary>
    /// 未知异常
    /// </summary>
    public class UnExpectedException : TaistException
    {
        /// <summary>
        /// 构造未知异常的新实例
        /// </summary>
        /// <param name="ex"></param>
        public UnExpectedException(Exception ex) : base(string.Format("UnExpectedException : {0}", ex.Message)) { }
    }
}
