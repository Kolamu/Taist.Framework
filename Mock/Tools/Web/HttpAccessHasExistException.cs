using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Tools.Web
{
    public class HttpAccessHasExistException : TaistException
    {
        public HttpAccessHasExistException(string name) : base(string.Format("HttpAccess named {0} has exist", name)) { }
    }
}
