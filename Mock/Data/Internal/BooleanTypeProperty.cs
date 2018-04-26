namespace Mock.Data
{
    using System;
    using System.Reflection;
    internal class BooleanTypeProperty : TaistProperty
    {
        internal override object GetRealValue(object propertyValue)
        {
            if (propertyValue == null)
                return false;
            else if (string.IsNullOrEmpty(propertyValue.ToString()))
                return false;
            else if (string.Equals(propertyValue.ToString(), "0"))
                return false;
            else if (string.Equals(propertyValue.ToString(), "1"))
                return true;
            else
                return bool.Parse(propertyValue.ToString());
        }
    }
}
