using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mock.Nature.DataBase
{
    public class ConnectionString
    {
        public string DatabaseName { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
