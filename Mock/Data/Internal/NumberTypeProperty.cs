namespace Mock.Data
{
    using System;
    using System.Reflection;
    internal class NumberTypeProperty : TaistProperty
    {
        internal override object GetRealValue(object propertyValue)
        {
            if (propertyValue == null)
                return 0;
            else if (string.IsNullOrEmpty(propertyValue.ToString()))
                return 0;
            else
                return Convert.ChangeType(propertyValue, reflectPropertyInfo.PropertyType);
        }
    }
}
