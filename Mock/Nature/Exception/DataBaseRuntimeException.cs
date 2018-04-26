using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Nature.Exception
{
    /// <summary>
    /// Exception occured when running something in data base error appear.
    /// </summary>
    public class DataBaseRuntimeException : TaistException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public DataBaseRuntimeException(string message) : base(string.Format("DatabaseRuntimeException : {0}", message)) { }
    }
}
