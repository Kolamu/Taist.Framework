namespace Mock.Data
{
    using System;
    using System.Reflection;

    using Mock.Data.Exception;
    internal class UniversalTypeProperty : TaistProperty
    {
        internal override object GetRealValue(object propertyValue)
        {
            return new universal(propertyValue);
        }
    }
}
