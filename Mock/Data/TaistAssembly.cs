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
    public sealed class TaistAssembly
    {
        private Assembly reflectAssembly;

        private Dictionary<string, Type> findTypeByName = new Dictionary<string, Type>();
        private Dictionary<string, Type> findTypeByNameIgnoreCase = new Dictionary<string, Type>();

        public TaistAssembly(Assembly reflectAssembly)
        {
            this.reflectAssembly = reflectAssembly;
        }

        //public DataType GetPrivateType(string typeName)
        //{
        //    return new DataType(reflectAssembly.GetType(typeName));
        //}
        
        //public DataType GetType(string propertyName)
        //{
        //    if(findPropertyByName.ContainsKey(propertyName))
        //        return DataProperty.GetDataProperty(findPropertyByName[propertyName], Instance);
        //    return GetPropertyIgnoreCase(propertyName);
        //}

        //public DataProperty GetPropertyIgnoreCase(string propertyName)
        //{
        //    if (findPropertyByNameIgnoreCase.ContainsKey(propertyName.ToLower()))
        //        return DataProperty.GetDataProperty(findPropertyByNameIgnoreCase[propertyName.ToLower()], Instance);
        //    throw new CanNotFindPropertyException(reflectDataType.Name, propertyName);
        //}

        //private void InitilizeFriendlyName()
        //{
        //    Type[] allTypeArray = reflectAssembly.GetTypes();
        //    foreach (Type reflectType in allTypeArray)
        //    {
        //        DataType dataType = new DataType(reflectType);
        //        if(property.Ignore) continue;
        //        AddType(dataType.FriendlyName, propertyInfo);
        //        AddType(propertyInfo.Name, propertyInfo);
        //    }
        //}

        private void AddType(string typeName, Type reflectType)
        {
            if (string.IsNullOrEmpty(typeName))
                return;

            if (findTypeByName.ContainsKey(typeName))
                throw new PropertyAlisaHasExistException(typeName, reflectType.Name, findTypeByName[typeName].Name);

            if (findTypeByNameIgnoreCase.ContainsKey(typeName.ToLower()))
                throw new PropertyAlisaHasExistException(typeName.ToLower(), reflectType.Name, findTypeByNameIgnoreCase[typeName.ToLower()].Name);

            findTypeByName.Add(typeName, reflectType);
            findTypeByNameIgnoreCase.Add(typeName.ToLower(), reflectType);
        }
    }
}
