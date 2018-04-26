using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class DataAssemblyAttribute : Attribute
    {
        public DataAssemblyAttribute() { }
    }
}
