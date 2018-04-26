using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Data.Exception
{
    public class PropertyAlisaHasExistException : TaistException
    {
        public PropertyAlisaHasExistException(string alisaName, string pNameCurrent, string pNameExist) : base(string.Format("Property named {1}'s alisa named {0} has been used by another property named {2}", alisaName, pNameCurrent, pNameExist)) { }
    }
}
