namespace Mock.Tools.Web
{
    using System;
    using System.Net;
    using System.Web;
    using System.Text;
    using System.Net.Sockets;
    using System.Threading;
    using System.Collections.Generic;
    using System.Diagnostics;
    public class RawHttpRequest
    {
        private Dictionary<string, string> _headers = new Dictionary<string,string>();
        public RawHttpRequest(string host, int port)
        {
            _host = host;
            _port = port;
            _headers["Accept"] = "text/html, application/xhtml+xml, */*";
            _headers["Accept-Language"] = "zh-CN";
            _headers["Content-Type"] = "application/x-www-form-urlencoded";
            _headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
            _headers["Accept-Encoding"] = "gzip, deflate";
            _headers["Connection"] = "Keep-Alive";
            _headers["Content-Length"] = "9";
            _headers["DNT"] = "1";
            _headers["Host"] = string.Format("{0}:{1}", host, port);
            _headers["Pragma"] = "no-cache";
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
            set
            {
                if (_headers.ContainsKey(name))
                {
                    _headers[name] = value;
                }
                else
                {
                    _headers.Add(name, value);
                }
            }
        }

        public void ClearHeader()
        {
            _headers.Clear();
            _headers["Content-Length"] = "9";
            _headers["Host"] = string.Format("{0}:{1}", _host, _port);
        }

        private string _host = null;
        private int _port = 80;
        private string _url = null;

        public string URL
        {
            get
            {
                return _url;
            }
            set
            {
                if (value.StartsWith("http:"))
                {
                    _url = value;
                }
                else
                {
                    _url = string.Format("/{0}", value.Trim('/'));
                }
            }
        }

        private TimeSpan _requestTime = default(TimeSpan);
        public TimeSpan RequestTime
        {
            get
            {
                return _requestTime;
            }
        }

        public RawHttpResponse Post(string url, string param)
        {
            HttpParam.Clear();
            HttpParam.Set(param);
            
            return doRequest(url, "POST", HttpParam.Value);
        }

        public RawHttpResponse Post(string url, List<HUnit> param)
        {
            return doRequest(url, "POST", param);
        }

        public RawHttpResponse Get(string url, List<HUnit> param)
        {
            return doRequest(url, "GET", param);
        }

        private RawHttpResponse doRequest(string url, string method, List<HUnit> param, Encoding encoding = null)
        {
            _requestTime = default(TimeSpan);
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            URL = url;
            HttpAccess.CheckFilter(url, encoding, param, null, HttpDataType.RAW);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse(_host), _port));

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{1} {0} HTTP/1.1\r\n", _url, method);
            
            string paramString = "";
            if (param != null && param.Count > 0)
            {
                if (param.Count == 1)
                {
                    if (string.IsNullOrEmpty(param[0].Key))
                    {
                        paramString = param[0].Value;
                    }
                    else
                    {
                        paramString = string.Format("{0}={1}", param[0].Key, param[0].Value);
                    }
                }
                else
                {
                    paramString = string.Join("&", param.ConvertAll(u => string.Format("{0}={1}", u.Key, u.Value)));
                }
                this["Content-Length"] = paramString.Length.ToString();
            }

            foreach (KeyValuePair<string, string> kv in _headers)
            {
                sb.AppendFormat("{0}:{1}\r\n", kv.Key, kv.Value);
            }
            sb.Append("\r\n");
            sb.Append(paramString);
            //Robot.Note(sb.ToString());
            Stopwatch watch = new Stopwatch();
            watch.Start();
            socket.Send(encoding.GetBytes(sb.ToString()));
            byte[] buf = new byte[1024];

            int n = socket.Receive(buf);
            watch.Stop();
            byte[] data = new byte[n];
            Array.Copy(buf, data, n);
            while (n > 0)
            {
                socket.ReceiveTimeout = 1000;
                socket.Blocking = true;
                try
                {
                    n = socket.Receive(buf);
                }
                catch
                {
                    break;
                }
                byte[] tmp = data;
                data = new byte[tmp.Length + n];
                Array.Copy(tmp, data, tmp.Length);
                Array.Copy(buf, 0, data, tmp.Length, n);
            }
            _requestTime = watch.Elapsed;
            socket.Close();
            RawHttpResponse response = new RawHttpResponse(data);

            return response;
        }
    }
}
