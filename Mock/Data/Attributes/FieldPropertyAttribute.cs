namespace Mock.Data.Attributes
{
    using System;
    /// <summary>
    /// 标记类属性的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple= false)]
    public class FieldPropertyAttribute : Attribute
    {
        private string _propertyName = null;
        private bool _attributeProperty = false;
        private bool _ignore = false;

        /// <summary>
        /// 构造标记类属性特性的新实例
        /// </summary>
        public FieldPropertyAttribute() { }

        /// <summary>
        /// 构造标记类属性特性的新实例
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="isAttribute"></param>
        public FieldPropertyAttribute(string pName, bool isAttribute)
        {
            _propertyName = pName;
            _attributeProperty = isAttribute;
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        /// <summary>
        /// 属性是否标记为特性
        /// </summary>
        public bool isAttribute
        {
            get
            {
                return _attributeProperty;
            }
            set
            {
                _attributeProperty = value;
            }
        }

        /// <summary>
        /// 属性是否输出
        /// </summary>
        public bool isIgnore
        {
            get
            {
                return _ignore;
            }
            set
            {
                _ignore = value;
            }
        }
    }
}
