namespace Mock.Data.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class BusinessAssemblyAttribute : Attribute
    {
        public BusinessAssemblyAttribute() { }
    }
}
