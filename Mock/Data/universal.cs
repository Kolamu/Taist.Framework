namespace Mock.Data
{
    using System;
    using System.Reflection;
    /// <summary>
    /// 通用数据类型
    /// </summary>
    public class universal
    {
        public universal(object value)
        {
            _obj = value;
            if (value == null)
            {
                _val = null;
            }
            else
            {
                _val = value.ToString();
            }
        }

        public bool IsNumeric
        {
            get
            {
                decimal result = 0;
                return decimal.TryParse(_val, out result);
            }
        }

        private string _val = "";
        private object _obj = null;
        public object ObjValue
        {
            set
            {
                _obj = value;
                if (value == null)
                {
                    _val = null;
                }
                else
                {
                    _val = _obj.ToString();
                }
            }
            get
            {
                return _obj;
            }
        }

        public bool BoolValue
        {
            get
            {
                try
                {
                    return bool.Parse(_val);
                }
                catch
                {
                    if (string.IsNullOrEmpty(_val))
                    {
                        return true;
                    }
                    try
                    {
                        return DoubleValue > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public string StringValue
        {
            get
            {
                return _val;
            }
        }

        public byte ByteValue
        {
            get
            {
                try
                {
                    return byte.Parse(_val);
                }
                catch
                {
                    if (DoubleValue > byte.MaxValue)
                    {
                        throw new InvalidCastException();
                    }
                    return (byte)DoubleValue; ;
                }
            }
        }

        public short ShortValue
        {
            get
            {
                try
                {
                    return short.Parse(_val);
                }
                catch
                {
                    if (DoubleValue > short.MaxValue)
                    {
                        throw new InvalidCastException();
                    }
                    return (short)DoubleValue; ;
                }
            }
        }


        public int IntValue
        {
            get
            {
                try
                {
                    return int.Parse(_val);
                }
                catch
                {
                    if (DoubleValue > int.MaxValue)
                    {
                        throw new InvalidCastException();
                    }
                    return (int)DoubleValue; ;
                }
            }
        }

        public long LongValue
        {
            get
            {
                try
                {
                    return long.Parse(_val);
                }
                catch
                {
                    if (DoubleValue > long.MaxValue)
                    {
                        throw new InvalidCastException();
                    }
                    return (long)DoubleValue; ;
                }
            }
        }

        public float FloatValue
        {
            get
            {
                try
                {
                    return float.Parse(_val);
                }
                catch
                {
                    if (DoubleValue > float.MaxValue)
                    {
                        throw new InvalidCastException();
                    }
                    return (float)DoubleValue;
                }
            }
        }

        public double DoubleValue
        {
            get
            {
                if (string.IsNullOrEmpty(_val))
                {
                    return 0;
                }
                try
                {
                    return double.Parse(_val.Trim());
                }
                catch
                {
                    if (_val.StartsWith("."))
                    {
                        try
                        {
                            return double.Parse(string.Format("0{0}", _val));
                        }
                        catch
                        {
                            throw new InvalidCastException();
                        }
                    }
                    if (string.Equals(_val.ToLower(), "true"))
                    {
                        return 1;
                    }
                    else if (string.Equals(_val.ToLower(), "false"))
                    {
                        return 0;
                    }
                    else
                    {
                        throw new InvalidCastException();
                    }
                }
            }
        }

        public override string ToString()
        {
            return _val;
        }

        public string ToString(string format)
        {
            universal u;
            if (_obj == null)
            {
                u = new universal("");
            }
            else
            {
                u = new universal(_obj);
            }

            try
            {
                if (format.ToUpper().StartsWith("F"))
                {
                    return u.DoubleValue.ToString(format);
                }
                else if (format.ToUpper().StartsWith("D"))
                {
                    return u.LongValue.ToString(format);
                }
            }
            catch { }
            Type[] tArray = { typeof(string) };
            Type objType = u._obj.GetType();
            MethodInfo mi = objType.GetMethod("ToString", tArray);
            if (mi == null)
            {
                return u._obj.ToString();
            }
            else
            {
                try
                {
                    return (string)mi.Invoke(u._obj, new object[] { format });
                }
                catch
                {
                    return u._obj.ToString();
                }
            }
        }

        public static universal operator -(universal value)
        {
            universal u = new universal("-" + value.StringValue);
            return u;
        }

        #region bool
        public static implicit operator bool(universal value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                try
                {
                    return value.BoolValue;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static implicit operator universal(bool value)
        {
            return new universal(value);
        }
        #endregion

        #region string
        public static implicit operator string(universal value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return value.ToString();
            }
        }

        public static implicit operator universal(string value)
        {
            return new universal(value);
        }
        #endregion

        #region byte
        public static implicit operator byte(universal value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value.ByteValue;
            }
        }

        public static implicit operator universal(byte value)
        {
            return new universal(value);
        }
        #endregion

        #region short
        public static implicit operator short(universal value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value.ShortValue;
            }
        }

        public static implicit operator universal(short value)
        {
            return new universal(value);
        }
        #endregion

        #region int
        public static implicit operator int(universal value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value.IntValue;
            }
        }

        public static implicit operator universal(int value)
        {
            return new universal(value);
        }
        #endregion

        #region long
        public static implicit operator long(universal value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value.LongValue;
            }
        }

        public static implicit operator universal(long value)
        {
            return new universal(value);
        }
        #endregion

        #region double
        public static implicit operator double(universal value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value.DoubleValue;
            }
        }

        public static implicit operator universal(double value)
        {
            return new universal(value);
        }
        #endregion

        #region float
        public static implicit operator float(universal value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value.FloatValue;
            }
        }

        public static implicit operator universal(float value)
        {
            return new universal(value);
        }
        #endregion

        #region +
        public static universal operator +(universal un1, universal un2)
        {
            try
            {
                double d1 = un1;
                double d2 = un2;
                return d1 + d2;
            }
            catch
            {
                if (string.IsNullOrEmpty(un1))
                {
                    return un2;
                }

                if (string.IsNullOrEmpty(un2))
                {
                    return un1;
                }
                
                return un1.StringValue + un2.StringValue;
            }
        }
        #endregion

        #region -
        public static universal operator -(universal un1, universal un2)
        {
            try
            {
                double d1 = un1;
                double d2 = un2;
                return d1 - d2;
            }
            catch
            {
                if (string.IsNullOrEmpty(un1) || string.IsNullOrEmpty(un2))
                {
                    return un1;
                }

                int index = un1.StringValue.IndexOf(un2.StringValue);
                if (index >= 0)
                {
                    return un1.StringValue.Remove(index, un2.StringValue.Length);
                }
                else
                {
                    return un1;
                }
            }
        }
        #endregion

        #region *
        public static universal operator *(universal un1, universal un2)
        {
            try
            {
                double d1 = un1;
                double d2 = un2;
                return d1 * d2;
            }
            catch
            {
                if (un2.IsNumeric)
                {
                    System.Text.StringBuilder ret = new System.Text.StringBuilder();
                    for (int i = 0; i < un2; i++)
                    {
                        ret.Append(un1.StringValue);
                    }
                    return new universal(ret.ToString());
                }
                else
                {
                    return un1 + un2;
                }
            }
        }
        #endregion

        #region /
        public static universal operator /(universal un1, universal un2)
        {
            try
            {
                double d1 = un1;
                double d2 = un2;
                return d1 / d2;
            }
            catch
            {
                return un1;
            }
        }
        #endregion

    }
}
