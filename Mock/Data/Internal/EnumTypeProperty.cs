namespace Mock.Data
{
    using System;
    using System.Reflection;

    using Mock.Data.Exception;
    internal class EnumTypeProperty : TaistProperty
    {
        internal override object GetRealValue(object propertyValue)
        {
            if (propertyValue == null)
                throw new PropertyIsNullException("propertyValue");
            else if (string.IsNullOrEmpty(propertyValue.ToString()))
                throw new PropertyIsNullException("propertyValue");
            else
                return Enum.Parse(reflectPropertyInfo.PropertyType, propertyValue.ToString());
        }
    }
}
