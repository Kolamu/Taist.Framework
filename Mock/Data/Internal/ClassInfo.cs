namespace Mock.Data.TaistDataCenter
{
    using System;
    using System.Reflection;
    using System.Xml;
    using System.IO;

    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    public class ClassInfo
    {
        private string conditionString
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    throw new PropertyIsNullException("Name");
                }

                return string.Format("//ClassInfo[Name='{0}']", _name);
            }
        }

        private string _name = null;
        [FieldProperty("", false)]
        public string Name { get { return _name; } set { _name = value; } }

        private string _class = null;
        [FieldProperty("", false)]
        public string ClassName { get { return _class; } set { _class = value; } }

        private Assembly _ass = null;
        [FieldProperty("", false)]
        public Assembly Assembly
        {
            get
            {
                return _ass;
            }
            internal set
            {
                _ass = value;
            }
        }

        private Type type = null;
        public Type Type
        {
            get
            {
                if (type == null)
                {
                    type = Assembly.GetType(_class);
                }
                return type;
            }
        }
    }
}
