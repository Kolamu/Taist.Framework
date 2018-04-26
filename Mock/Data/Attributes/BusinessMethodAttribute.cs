namespace Mock.Data.Attributes
{
    using System;
    using Mock.Data.Exception;

    /// <summary>
    /// 表示业务方法特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BusinessMethodAttribute : Attribute
    {
        private string _businessName = null;
        private string _keywords = "";
        private string _description = "";
        private string _mode = "";
        private string _target = "";
        private bool _default = false;

        /// <summary>
        /// 业务名称
        /// </summary>
        public string BusinessName
        {
            get
            {
                return _businessName;
            }
            set
            {
                _businessName = value;
            }
        }

        /// <summary>
        /// 方法的关键字
        /// </summary>
        public string Keywords
        {
            get
            {
                return _keywords;
            }
            set
            {
                _keywords = value;
            }
        }

        /// <summary>
        /// 方法二级关键字
        /// </summary>
        public string SubKeyword
        {
            get
            {
                if (string.IsNullOrEmpty(_mode))
                {
                    return _target;
                }
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        /// <summary>
        /// 方法二级关键字
        /// </summary>
        public string TargetPorject
        {
            get
            {
                if (string.IsNullOrEmpty(_target))
                {
                    return _mode;
                }
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// 关键字的默认调用方法
        /// </summary>
        public bool Default
        {
            get
            {
                return _default;
            }
            set
            {
                _default = value;
            }
        }

        public static implicit operator string(BusinessMethodAttribute bma)
        {
            return string.Format("{0},{1},{2}", bma.Keywords, bma.SubKeyword, bma.TargetPorject);
        }
    }
}
