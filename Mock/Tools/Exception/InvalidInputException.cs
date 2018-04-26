using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Tools.Exception
{
    /// <summary>
    /// 无效输入异常
    /// </summary>
    public class InvalidInputException : TaistException
    {
        /// <summary>
        /// 构造无效输入异常的新实例
        /// </summary>
        /// <param name="message"></param>
        public InvalidInputException(string message) : base(string.Format("The input named {0} is invalid", message)) { }
    }
}
