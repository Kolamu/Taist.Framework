namespace Mock.Tools.Web
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Text;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Collections.Generic;

    using Mock.Tools.Exception;
    internal class HttpAccessObject
    {
        private CookieContainer cookieContainer;
        private HttpWebResponse response;
        private HttpWebRequest request;
        private string ipAddress = null;
        private string port = null;
        private string baseUrl = null;
        internal HttpAccessObject(string ip, string port, HttpType httpType = HttpType.HTTP, Cookie cookie = null)
        {
            ServicePointManager.DefaultConnectionLimit = 50;
            request = null;
            response = null;
            ipAddress = ip;
            this.port = port;
            cookieContainer = new CookieContainer();
            if (cookie != null)
            {
                try
                {
                    cookieContainer.Add(cookie);
                }
                catch { }
            }

            if (httpType == HttpType.HTTP)
            {
                GetHttpBaseUrl();
            }
            else if (httpType == HttpType.HTTPS)
            {
                GetHttpsBaseUrl();
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
        }

        private void GetHttpBaseUrl()
        {
            if (string.IsNullOrEmpty(port))
            {
                baseUrl = string.Format("http://{0}/", Host);
            }
            else
            {
                baseUrl = string.Format("http://{0}:{1}/", Host, port);
            }

        }

        private void GetHttpsBaseUrl()
        {
            if (string.IsNullOrEmpty(port))
            {
                baseUrl = string.Format("https://{0}:443/", Host);
            }
            else
            {
                baseUrl = string.Format("https://{0}:{1}/", Host, port);
            }
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        internal HttpAccessObject(string url, Cookie cookie = null)
        {
            ServicePointManager.DefaultConnectionLimit = 50;
            request = null;
            response = null;
            ipAddress = null;
            port = null;
            this.baseUrl = url;
            cookieContainer = new CookieContainer();
            if (cookie != null)
            {
                try
                {
                    cookieContainer.Add(cookie);
                }
                catch { }
            }
        }

        internal string Host
        {
            get
            {
                if (string.IsNullOrEmpty(ipAddress)) return "127.0.0.1";
                return ipAddress;
            }
        }

        internal string Port
        {
            get
            {
                return port;
            }
        }

        internal HttpWebResponse Response
        {
            get
            {
                return response;
            }
        }

        internal void PostData(string url, Encoding encoding, List<HUnit> param = null, string Referer = "")
        {
            MakeRequest(url, HttpMethod.POST, Referer);
            //如果需要POST数据
            if (!(param == null || param.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (HUnit hp in param)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", hp.Key, EncodeData(hp.Value, encoding));
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", hp.Key, EncodeData(hp.Value, encoding));
                    }
                    i++;
                }
                //Console.WriteLine(buffer.ToString());
                byte[] data = encoding.GetBytes(buffer.ToString());
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Close();
                }
            }

            MakeResponse();
        }

        private string EncodeData(string data, Encoding encoding)
        {
            if (data == null) return data;
            StringBuilder sb = new StringBuilder();
            byte[] bts = encoding.GetBytes(data);
            foreach (byte b in bts)
            {
                if (Mock.Data.DataFactory.ConstantUrlCode.Contains(b))
                {
                    sb.Append((char)b);
                }
                else if (b == 32)
                {
                    if (encoding == Encoding.UTF8)
                    {
                        sb.Append("%20");
                    }
                    else
                    {
                        sb.Append("+");
                    }
                }
                else
                {
                    sb.AppendFormat("%{0}", b.ToString("X2"));
                }
            }
            return sb.ToString();
        }

        internal void PostData(string url, List<HUnit> param = null, string Referer = "")
        {
            PostData(url, Encoding.Default, param, Referer);
        }

        internal string CookieString
        {
            get
            {
                return cookieContainer.ToString();
            }
        }

        internal string CertPath { get; set; }

        //internal void PostData(string url, string cookie, List<HUnit> param, string Referer)
        //{
        //    if (request != null)
        //    {
        //        request.Abort();
        //    }

        //    url = url.TrimStart('/');
        //    request = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}:{1}/{2}", ipAddress, port, url));
            
        //    request.Credentials = CredentialCache.DefaultCredentials;
        //    request.Method = "POST";
        //    request.Timeout = Config.TimeOut;
        //    request.ReadWriteTimeout = Config.TimeOut;
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.CookieContainer = cookieContainer;
        //    request.AllowAutoRedirect = false;
        //    if (string.IsNullOrEmpty(Referer))
        //    {
        //        request.Referer = string.Format("http://{0}:{1}/{2}", ipAddress, port, url);
        //    }
        //    else
        //    {
        //        if (Referer.StartsWith("http://"))
        //        {
        //            request.Referer = Referer;
        //        }
        //        else
        //        {
        //            request.Referer = string.Format("http://{0}:{1}/{2}", ipAddress, port, Referer.TrimStart('/'));
        //        }
        //    }
        //    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)";
        //    request.KeepAlive = true;
        //    request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
        //    request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh_cn");

        //    //如果需要POST数据
        //    if (!(param == null || param.Count == 0))
        //    {
        //        StringBuilder buffer = new StringBuilder();
        //        int i = 0;
        //        foreach (HUnit hp in param)
        //        {
        //            if (i > 0)
        //            {
        //                buffer.AppendFormat("&{0}={1}", hp.Key, HttpUtility.UrlEncode(hp.Value, Encoding.Default));
        //            }
        //            else
        //            {
        //                buffer.AppendFormat("{0}={1}", hp.Key, HttpUtility.UrlEncode(hp.Value, Encoding.Default));
        //            }
        //            i++;
        //        }
        //        //Console.WriteLine(buffer.ToString());
        //        byte[] data = Encoding.Default.GetBytes(buffer.ToString());
        //        request.ContentLength = data.Length;
        //        using (Stream stream = request.GetRequestStream())
        //        {
        //            stream.Write(data, 0, data.Length);
        //            stream.Close();
        //        }
        //    }
        //    if (response != null)
        //    {
        //        response.Close();
        //    }
        //    try
        //    {
        //        response = (HttpWebResponse)request.GetResponse();
        //    }
        //    catch (WebException ex)
        //    {
        //        if (ex.Status == WebExceptionStatus.Success)
        //        {
        //            response = (HttpWebResponse)ex.Response;
        //        }
        //        else
        //        {
        //            throw new HttpRequestFailedException(ex.Status.ToString(), ex.Message);
        //        }
        //    }
        //}

        internal void PostString(string url, string param = null, string Referer = "", Encoding encoding = null)
        {
            MakeRequest(url, HttpMethod.POST, Referer);

            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            request.ContentType = "text/xml;charset=" + encoding.EncodingName;
            
            //如果需要POST数据
            if (!string.IsNullOrEmpty(param))
            {
                byte[] data = Encoding.UTF8.GetBytes(param);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    stream.Close();
                }
            }

            MakeResponse();
        }

        internal void PostOctestStream(string url, string param = null, string Referer = "")
        {
            MakeRequest(url, HttpMethod.POST, Referer);
            request.ContentType = "application/octest-stream";
            request.Accept = "text/html, image/gif, image/jpeg, *; q=.2, */*; q=.2";
            
            //如果需要POST数据
            if (!string.IsNullOrEmpty(param))
            {
                byte[] data = Encoding.UTF8.GetBytes(param);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Close();
                }
            }
            MakeResponse();
        }

        internal void PostJson(string url, string name = null, string param = null, string Referer = "")
        {
            MakeRequest(url, HttpMethod.POST, Referer);
            request.Accept = "application/json, text/javascript, */*";
            
            //如果需要POST数据
            if (!string.IsNullOrEmpty(param))
            {
                byte[] data = Encoding.UTF8.GetBytes(string.Format("{0}={1}", name, param));
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Close();
                }
            }
            
            MakeResponse();
        }

        internal void Get(string url)
        {
            Get(url, null, null);
        }

        internal void Get()
        {
            Get(baseUrl);
        }

        internal void Get(string url, List<HUnit> param, string Refer = "")
        {
            if (param != null && param.Count > 0)
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (HUnit hp in param)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", hp.Key, HttpUtility.UrlEncode(hp.Value, Encoding.Default));
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", hp.Key, HttpUtility.UrlEncode(hp.Value, Encoding.Default));
                    }
                    i++;
                }

                url = url + "?" + buffer.ToString();
            }

            MakeRequest(url, HttpMethod.GET, Refer);
            MakeResponse();
        }

        public void Dispose()
        {
            cookieContainer = null;
            request = null;
            if (response != null)
            {
                response.Close();
                response = null;
            }
        }

        private void MakeRequest(string url, HttpMethod method, string refer)
        {
            if (request != null)
            {
                request.Abort();
            }

            string realUrl = null;
            if (string.IsNullOrEmpty(CertPath))
            {
                realUrl = GetHttpUrl(url, null);
            }
            else
            {
                realUrl = GetHttpsUrl(url, null);
            }

            request = (HttpWebRequest)WebRequest.Create(realUrl);

            if (!string.IsNullOrEmpty(CertPath))
            {
                AddCert();
            }

            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = method.ToString();
            request.Timeout = Config.TimeOut;
            request.ReadWriteTimeout = Config.TimeOut;
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            if (string.IsNullOrEmpty(CertPath))
            {
                if (refer != null)
                {
                    request.Referer = GetHttpUrl(refer, realUrl);
                }
            }
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)";
            request.KeepAlive = true;
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh_cn");
            request.Accept = "text/html, application/xhtml+xml, */*";
        }

        private void AddCert()
        {
            X509Certificate Cert = X509Certificate.CreateFromCertFile(CertPath); //证书存放的绝对路径
            request.ClientCertificates.Add(Cert);
        }

        private void MakeResponse()
        {
            if (response != null)
            {
                response.Close();
            }

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Success)
                {
                    response = (HttpWebResponse)ex.Response;
                }
                else
                {
                    throw new HttpRequestFailedException(ex.Status.ToString(), ex.Message);
                }
            }
        }

        private string GetHttpUrl(string url, string defaultUrlIfNull)
        {
            if (string.IsNullOrEmpty(url))
                return GetHttpUrl(defaultUrlIfNull, baseUrl);
            
            if (url.StartsWith("http://"))
                return url;

            return string.Format("{0}{1}", baseUrl, url.TrimStart('/'));
        }

        private string GetHttpsUrl(string url, string defaultUrlIfNull)
        {
            if (string.IsNullOrEmpty(url))
                return GetHttpsUrl(defaultUrlIfNull, baseUrl);

            if (url.StartsWith("https://")) 
                return url;

            return string.Format("{0}{1}", baseUrl, url.TrimStart('/'));
        }

        private enum HttpMethod
        {
            POST,
            GET
        }

        public enum HttpType
        {
            HTTP,
            HTTPS
        }
    }
}
