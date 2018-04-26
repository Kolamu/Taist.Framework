namespace Mock.Nature.DataBase
{
    /// <summary>
    /// the key info of the table
    /// </summary>
    public class TableKeyInfo
    {
        /// <summary>
        /// table key name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// table key data type
        /// </summary>
        private string _type = "STRING";
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                switch (value)
                {
                    case "INT":
                    case "INT2":
                    case "INT4":
                    case "INT8":
                    case "INT16":
                    case "INT32":
                    case "INT64":
                    case "LONG":
                    case "BYTE":
                    case "BIT":
                    case "FLOAT":
                    case "FLOAT4":
                    case "FLOAT8":
                    case "DOUBLE":
                    case "SERIAL":
                    case "SERIAL4":
                    case "SERIAL8":
                    case "VARBIT":
                    case "SMALLINT":
                    case "INTEGER":
                        {
                            _type = value;
                            break;
                        }
                    case "DATE":
                    case "TIME":
                    case "DATETIME":
                    case "TIMETZ":
                    case "TIMESTAMPTZ":
                        {
                            _type = "DATE";
                            break;
                        }
                }
            }
        }

    }
}
