namespace Mock.Data.Attributes
{
    using System;
    /// <summary>
    /// 标记数据类的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple= false)]
    public class DataClassAttribute : Attribute
    {
        private string _className = null;

        /// <summary>
        /// 构造标记数据类特性的新实例
        /// </summary>
        public DataClassAttribute() { }

        /// <summary>
        /// 构造标记数据类特性的新实例
        /// </summary>
        /// <param name="pName"></param>
        public DataClassAttribute(string pName)
        {
            _className = pName;
        }

        /// <summary>
        /// 数据类名称
        /// </summary>
        public string Name
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }
    }
}
