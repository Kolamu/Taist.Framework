namespace Mock.Data
{
    using System;
    using System.Reflection;
    internal class StringTypeProperty : TaistProperty
    {
        internal override object GetRealValue(object propertyValue)
        {
            if (propertyValue == null) return null;
            else return propertyValue.ToString();
        }
    }
}
