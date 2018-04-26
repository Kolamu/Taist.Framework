namespace Mock
{
    using System;
    /// <summary>
    /// 表示测试框架异常的基类
    /// </summary>
    public class TaistException : RobotException
    {
        /// <summary>
        /// 构造异常类的新实例
        /// </summary>
        /// <param name="message"></param>
        public TaistException(string message) : base(message) { }

        /// <summary>
        /// 构造异常类的新实例
        /// </summary>
        public TaistException() : base() { }

        /// <summary>
        /// 构造异常类的新实例
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public TaistException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// 构造异常类的新实例
        /// </summary>
        /// <param name="innerException"></param>
        public TaistException(Exception innerException) : base("", innerException) { }
    }

    /// <summary>
    /// 表示测试框架异常的基类，该类更名为TaistException，为了兼容旧版而存，后续可能会移除
    /// </summary>
    public abstract class RobotException : Exception
    {
        /// <summary>
        /// 构造异常类的新实例
        /// </summary>
        /// <param name="message"></param>
        public RobotException(string message) : base(message) { }

        /// <summary>
        /// 构造异常类的新实例
        /// </summary>
        public RobotException() : base() { }

        /// <summary>
        /// 构造异常类的新实例
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RobotException(string message, Exception innerException) : base(message, innerException) { }
    }

}
