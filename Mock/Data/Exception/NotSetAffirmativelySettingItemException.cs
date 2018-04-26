using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Data.Exception
{
    /// <summary>
    /// 未设置必选项参数异常
    /// </summary>
    public class NotSetAffirmativelySettingItemException : TaistException
    {
        /// <summary>
        /// 构造未设置必选项参数异常的新实例
        /// </summary>
        /// <param name="itemName"></param>
        public NotSetAffirmativelySettingItemException(string itemName) : base(string.Format("you had not set the affirmatively setting item named {0}.", itemName)) { }
    }
}
