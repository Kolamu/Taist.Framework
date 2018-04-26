namespace Mock.Nature.DataBase
{
    using System;
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
        private bool _stringType = true;
        public PrimaryKeyAttribute(bool stringType = true) 
        {
            _stringType = stringType;
        }

        public bool StringType { get { return _stringType; } set { _stringType = value; } }
    }
}
