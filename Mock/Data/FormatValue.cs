namespace Mock.Data
{
    using System;
    using System.Xml;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    internal sealed class FormatValue
    {
        private Dictionary<string, string> semanticValue = new Dictionary<string, string>();
        public string KeyName { get; set; }
        public string FormatType { get; set; }
        public string Pattern { get; set; }
        public int FormatLength { get; set; }
        public char DefaultChar { get; set; }
        public object Value { get; set; }
        public string FormatedValue
        {
            get
            {
                switch (FormatType.ToLower())
                {
                    default:
                        {
                            if (Value == null)
                            {
                                return null;
                            }
                            else
                            {
                                return Value.ToString();
                            }
                        }
                    case "pattern":
                        {
                            return PatternValue;
                        }
                    case "semantic":
                        {
                            return SemanticValue;
                        }
                    case "leftfilling":
                        {
                            return LeftFillingValue;
                        }
                    case "rightfilling":
                        {
                            return RightFillingValue;
                        }
                }
            }
        }

        public void setProperty(XmlNode xn)
        {
            if (xn == null) return;
            if (semanticValue.ContainsKey(xn.Name.ToLower()))
            {
                semanticValue[xn.Name.ToLower()] = xn.InnerText;
            }
            else
            {
                semanticValue.Add(xn.Name.ToLower(), xn.InnerText);
            }
        }

        private string PatternValue
        {
            get
            {
                universal uv = new universal(Value);
                return uv.ToString(Pattern);
            }
        }

        private string SemanticValue
        {
            get
            {
                if (Value == null)
                {
                    return null;
                }

                if (semanticValue.ContainsKey(Value.ToString().ToLower()))
                {
                    return semanticValue[Value.ToString().ToLower()];
                }

                if (semanticValue.ContainsKey(EncryptSemanticKey))
                {
                    return semanticValue[EncryptSemanticKey];
                }
                return null;
            }
        }

        private string EncryptSemanticKey
        {
            get
            {
                if (Value == null) return null;

                byte[] keyArray = EncryptFactory.Md5(Value.ToString());
                return ("B" + BitConverter.ToString(keyArray).Replace("-", "")).ToLower();
            }
        }

        private string LeftFillingValue
        {
            get
            {
                if(Value == null) return null;
                string value = Value.ToString();
                char[] valueArray = new char[FormatLength];
                for (int i = 0; i < FormatLength; i++)
                {
                    if (i < FormatLength - value.Length)
                    {
                        if (Pattern != null && i < Pattern.Length)
                        {
                            valueArray[i] = Pattern[i];
                        }
                        else
                        {
                            valueArray[i] = DefaultChar;
                        }
                    }
                    else
                    {
                        valueArray[i] = value[i - FormatLength + value.Length];
                    }
                }
                return new string(valueArray);
            }
        }

        private string RightFillingValue
        {
            get
            {
                if (Value == null) return null;
                string value = Value.ToString();
                char[] valueArray = new char[FormatLength];
                for (int i = 0; i < FormatLength; i++)
                {
                    if (i < value.Length)
                    {
                        valueArray[i] = value[i];
                    }
                    else
                    {
                        if (Pattern == null || i < FormatLength - Pattern.Length)
                        {
                            valueArray[i] = DefaultChar;
                        }
                        else
                        {
                            valueArray[i] = Pattern[i - FormatLength + Pattern.Length];
                        }
                    }
                }
                return new string(valueArray);
            }
        }
    }

}
