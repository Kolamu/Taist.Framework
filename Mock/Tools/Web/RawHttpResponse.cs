namespace Mock.Tools.Web
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using Mock.Data.Exception;
    public class RawHttpResponse
    {
        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private byte[] _data = new byte[0];
        private int _code = 200;
        private string _status = "OK";
        
        public RawHttpResponse(byte[] data)
        {
            int index = ParseHeader(data);
            byte[] tmp = new byte[data.Length - index];
            Array.Copy(data, index, tmp, 0, data.Length - index);
            ParseData(tmp);
        }

        private int ParseHeader(byte[] data)
        {
            int index = 0;
            string s = Encoding.ASCII.GetString(data);
            string[] headerArray = s.Split('\n');
            string[] titleStringArray = headerArray[0].Trim().Split(' ');
            if (titleStringArray.Length != 3)
            {
                throw new InvalidDataTypeException(headerArray[0]);
            }
            _code = int.Parse(titleStringArray[1]);
            _status = titleStringArray[2];
            index += headerArray[0].Length + 1;

            for (int i = 1; i < headerArray.Length; i++)
            {
                index += headerArray[i].Length + 1;
                string headerString = headerArray[i].Trim();
                if (string.IsNullOrEmpty(headerString)) break;
                string[] kvString = headerString.Split(':');
                _headers.Add(kvString[0], kvString[1].Trim());
            }

            return index;
        }

        private void ParseData(byte[] data)
        {
            if (_headers.ContainsKey("Transfer-Encoding"))
            {
                if (string.Equals("chunked", _headers["Transfer-Encoding"]))
                {
                    ParseChunkedData(data);
                    return;
                }
            }
            ParseDefaultData(data);
        }

        private void ParseDefaultData(byte[] data)
        {
            _data = data;
        }

        private void ParseChunkedData(byte[] data)
        {
            int index = 0;
            _data = new byte[0];
            int len = 0;
            //bool th = true;
            bool flag = false;
            StringBuilder sb = new StringBuilder();
            while (index < data.Length)
            {
                byte b = data[index++];
                char c = (char)b;
                switch (c)
                {
                    case '\r':
                        {
                            flag = true;
                            break;
                        }
                    case '\n':
                        {
                            if (flag)
                            {
                                len = Convert.ToInt32(sb.ToString(), 16);
                                byte[] tmp = _data;
                                _data = new byte[tmp.Length + len];
                                Array.Copy(tmp, _data, tmp.Length);
                                Array.Copy(data, index, _data, tmp.Length, len);
                                index += len + 2;
                                sb.Clear();
                                flag = false;
                            }
                            break;
                        }
                    default:
                        {
                            sb.Append(c);
                            break;
                        }

                }
            }
        }

        public string this[string name]
        {
            get
            {
                if (_headers.ContainsKey(name))
                {
                    return _headers[name];
                }
                else
                {
                    return null;
                }
            }
        }

        public int Code
        {
            get
            {
                return _code;
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }
        }
    }
}
