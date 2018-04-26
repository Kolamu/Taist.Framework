namespace Mock.Data
{
    using System;
    using System.Reflection;

    using Mock.Data.Attributes;
    using Mock.Data.Exception;
    internal class DataType : TaistType
    {
        public override string FriendlyName
        {
            get
            {
                object[] typeAttributeArray = reflectDataType.GetCustomAttributes(typeof(DataClassAttribute), false);
                if (typeAttributeArray.Length > 0)
                {
                    DataClassAttribute classAttribute = typeAttributeArray[0] as DataClassAttribute;
                    if (string.IsNullOrEmpty(classAttribute.Name))
                    {
                        return Name;
                    }
                    return classAttribute.Name;
                }
                else
                {
                    return Name;
                }
            }
        }
    }
}
