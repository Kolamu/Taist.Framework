namespace Mock.Data.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParamAttribute : Attribute
    {
        public ParamAttribute() { }

        private string _name = string.Empty;
        private int _id = 0;
        private object _default = null;
        private string _description = string.Empty;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public object Default
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
    }
}
