namespace Mock.Data
{
    using System;
    using System.Xml;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;

    using Mock.Data.Attributes;
    using Mock.Data.Exception;
    
    /// <summary>
    /// 表示数据属性对象
    /// </summary>
    public abstract class TaistType
    {
        private static readonly object DEFAULT_OWNER = null;

        protected Type reflectDataType;
        protected object ownerObject;

        private Dictionary<string, PropertyInfo> findPropertyByName = new Dictionary<string, PropertyInfo>();
        private Dictionary<string, PropertyInfo> findPropertyByNameIgnoreCase = new Dictionary<string, PropertyInfo>();

        public object Instance
        {
            get
            {
                if (ownerObject == TaistType.DEFAULT_OWNER)
                {
                    ownerObject = DataFactory.CreateInstance(reflectDataType);
                }
                return ownerObject;
            }
        }

        public virtual string FriendlyName
        {
            get
            {
                return Name;
            }
        }

        public string Name
        {
            get
            {
                return reflectDataType.Name;
            }
        }

        public List<string> InterfaceName
        {
            get
            {
                return new List<Type>(reflectDataType.GetInterfaces()).ConvertAll(x => x.Name);
            }
        }

        public TaistProperty GetPrivateProperty(string propertyName)
        {
            return TaistProperty.GetDataProperty(reflectDataType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic), Instance);
        }

        public TaistProperty GetProperty(string propertyName)
        {
            if(findPropertyByName.ContainsKey(propertyName))
                return TaistProperty.GetDataProperty(findPropertyByName[propertyName], Instance);
            return GetPropertyIgnoreCase(propertyName);
        }

        public TaistProperty GetPropertyIgnoreCase(string propertyName)
        {
            if (findPropertyByNameIgnoreCase.ContainsKey(propertyName.ToLower()))
                return TaistProperty.GetDataProperty(findPropertyByNameIgnoreCase[propertyName.ToLower()], Instance);
            throw new CanNotFindPropertyException(reflectDataType.Name, propertyName);
        }

        public void CallClassSetPropertyFunction(XmlNode propertyValue)
        {
            Type[] paramTypeArray = { typeof(XmlNode) };
            MethodInfo setPropertyMethodInfo = reflectDataType.GetMethod("setProperty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase, null, paramTypeArray, null);

            if (setPropertyMethodInfo == null)
            {
                return;
            }
            setPropertyMethodInfo.Invoke(Instance, new object[] { propertyValue });
        }

        public bool ExtendsFrom(Type BaseType)
        {
            Type baseType = reflectDataType.BaseType;
            while (baseType != null)
            {
                if (baseType == BaseType)
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }

            Type[] interfaceTypes = reflectDataType.GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                if (interfaceType == BaseType)
                {
                    return true;
                }
            }

            return false;
        }

        public object CreateInstance()
        {
            ownerObject = Activator.CreateInstance(reflectDataType);
            return ownerObject;
        }

        private void Initilize()
        {
            FindVisuableProperty();
        }

        private void FindVisuableProperty()
        {
            PropertyInfo[] allPropertyInfoArray = reflectDataType.GetProperties();
            foreach (PropertyInfo propertyInfo in allPropertyInfoArray)
            {
                TaistProperty property = TaistProperty.GetDataProperty(propertyInfo, TaistType.DEFAULT_OWNER);
                if (property.Ignore) continue;
                AddPropertyInfo(property.FriendlyName, propertyInfo);
                AddPropertyInfo(propertyInfo.Name, propertyInfo);
            }
        }

        private void AddPropertyInfo(string propertyName, PropertyInfo propertyInfo)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            if (findPropertyByName.ContainsKey(propertyName))
                throw new PropertyAlisaHasExistException(propertyName, propertyInfo.Name, findPropertyByName[propertyName].Name);

            if (findPropertyByNameIgnoreCase.ContainsKey(propertyName.ToLower()))
                throw new PropertyAlisaHasExistException(propertyName.ToLower(), propertyInfo.Name, findPropertyByNameIgnoreCase[propertyName.ToLower()].Name);
            
            findPropertyByName.Add(propertyName, propertyInfo);
            findPropertyByNameIgnoreCase.Add(propertyName.ToLower(), propertyInfo);
        }

        public static TaistType GetDataType(object ownerObject)
        {
            if (ownerObject == null) throw new InvalidParamValueException("ownerObject is null");
            TaistType dataType = GetDataType(ownerObject.GetType());
            dataType.ownerObject = ownerObject;
            return dataType;
        }

        public static TaistType GetDataType(Type reflectDataType)
        {
            if (reflectDataType == null) throw new InvalidParamValueException("reflectDataType is null");
            TaistType dataType;
            if(HasAttribute(reflectDataType, typeof(DataClassAttribute)))
            {
                dataType = new DataType();
            }

            else if (HasAttribute(reflectDataType, typeof(BusinessClassAttribute)))
            {
                dataType = new BusinessType();
            }
            else
            {
                dataType = new UnknownType();
            }

            dataType.reflectDataType = reflectDataType;
            dataType.ownerObject = DEFAULT_OWNER;
            dataType.Initilize();

            return dataType;
        }

        private static bool HasAttribute(Type reflectDataType, Type attributeType)
        {
            object[] typeAttributeArray = reflectDataType.GetCustomAttributes(attributeType, false);
            return typeAttributeArray.Length > 0;
        }
    }
}
