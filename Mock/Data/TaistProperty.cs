namespace Mock.Data
{
    using System;
    using System.Reflection;

    using Mock.Data.Attributes;
    using Mock.Data.Exception;
    
    /// <summary>
    /// 表示数据属性对象
    /// </summary>
    public abstract class TaistProperty
    {
        protected PropertyInfo reflectPropertyInfo = null;
        protected object ownerObject = null;
        private FieldPropertyAttribute propertyAttribute = null;
        internal abstract object GetRealValue(object propertyValue);

        public object Get()
        {
            return reflectPropertyInfo.GetValue(ownerObject, null);
        }

        public T Get<T>()
        {
            return (T)reflectPropertyInfo.GetValue(ownerObject, null);
        }

        public string Name
        {
            get
            {
                return reflectPropertyInfo.Name;
            }
        }

        public bool Ignore
        {
            get
            {
                if (propertyAttribute == null) return false;
                return propertyAttribute.isIgnore;
            }
        }

        public string FriendlyName
        {
            get
            {
                if (propertyAttribute == null)
                    return null;

                if (string.Equals(propertyAttribute.Name, reflectPropertyInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return propertyAttribute.Name;
            }
        }

        public void Set(object propertyValue)
        {
            reflectPropertyInfo.SetValue(ownerObject, GetRealValue(propertyValue), null);
        }

        public static TaistProperty GetDataProperty(string propertyName, object ownerObject)
        {
            if (ownerObject == null) throw new ParameterIsNullException("ownerObject");
            if (string.IsNullOrEmpty(propertyName)) throw new ParameterIsNullException("propertyName");

            Type reflectType = ownerObject.GetType();
            return GetDataProperty(reflectType.GetProperty(propertyName), ownerObject);
        }

        public static TaistProperty GetDataProperty(PropertyInfo reflectPropertyInfo, object ownerObject)
        {
            if (reflectPropertyInfo == null) throw new ParameterIsNullException("reflectPropertyInfo");

            TaistProperty dataProperty = CreateInstance(reflectPropertyInfo.PropertyType);

            dataProperty.reflectPropertyInfo = reflectPropertyInfo;
            dataProperty.ownerObject = ownerObject;
            dataProperty.propertyAttribute = null;

            object[] propertyAttributeArray = reflectPropertyInfo.GetCustomAttributes(typeof(FieldPropertyAttribute), false);
            if (propertyAttributeArray.Length > 0)
            {
                dataProperty.propertyAttribute = propertyAttributeArray[0] as FieldPropertyAttribute;
            }

            return dataProperty;
        }

        private static TaistProperty CreateInstance(Type propertyType)
        {
            if (propertyType == typeof(string) || propertyType == typeof(String))
            {
                return new StringTypeProperty();
            }

            if (propertyType == typeof(bool) || propertyType == typeof(Boolean))
            {
                return  new BooleanTypeProperty();
            }

            if (propertyType == typeof(universal))
            {
                return new UniversalTypeProperty();
            }

            if (propertyType.IsEnum)
            {
                return new EnumTypeProperty();
            }

            if (propertyType.IsPrimitive && propertyType.IsValueType)
            {
                return new PrimitiveValueTypeProperty();
            }

            return new StringTypeProperty();
        }
    }
}
