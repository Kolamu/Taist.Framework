using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Tools.Exception
{
    /// <summary>
    /// 表示存在警告窗口时操作非警告窗口时触发此异常
    /// </summary>
    public class WarningWindowExistException : TaistException
    {
        /// <summary>
        /// 构造存在警告窗口异常的新实例
        /// </summary>
        public WarningWindowExistException() : base("Some warning window has exist, please deal with them first!") { }
    }
}
