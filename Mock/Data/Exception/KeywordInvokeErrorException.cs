using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Data.Exception
{
    public class KeywordInvokeErrorException : TaistException
    {
        public KeywordInvokeErrorException(string content) : base(string.Format("Keyword invoke error : {0}", content)) { }
    }
}
