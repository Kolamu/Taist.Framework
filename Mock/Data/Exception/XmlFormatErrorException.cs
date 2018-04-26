using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Data.Exception
{
    public class XmlFormatErrorException : TaistException
    {
        public XmlFormatErrorException(string content) : base(string.Format("Xml format error : {0}", content)) { }
    }
}
