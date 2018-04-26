namespace Mock.Data
{
    using System;
    using System.Reflection;

    using Mock.Data.Exception;
    internal class PrimitiveValueTypeProperty : TaistProperty
    {
        internal override object GetRealValue(object propertyValue)
        {
            if (propertyValue == null)
                throw new PropertyIsNullException("propertyValue");
            else if (string.IsNullOrEmpty(propertyValue.ToString()))
                throw new PropertyIsNullException("propertyValue");
            else
                return Convert.ChangeType(propertyValue, reflectPropertyInfo.PropertyType);
        }
    }
}
