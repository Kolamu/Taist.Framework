namespace Mock.Tools.Web
{
    using System;
    using System.Text;
    using System.Net;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    using Mock.Data;
    using Mock.Tools.Exception;

    public enum HttpDataType
    {
        JSON,
        STRING,
        DATA,
        OCTEST,
        RAW
    }

    public delegate bool HttpPostEventHandler(string url, System.Text.Encoding encoding, List<HUnit> param, string Refer, HttpDataType dataType);

    public class HttpAccess
    {
        private static Dictionary<string, HttpAccessObject> _httpDic = null;
        public static event HttpPostEventHandler PreviewPost = null;

        public static void Create(string name, string ip, string port)
        {
            if (_httpDic == null)
            {
                _httpDic = new Dictionary<string, HttpAccessObject>();
            }

            if (_httpDic.ContainsKey(name))
            {
                throw new HttpAccessHasExistException(name);
            }

            HttpAccessObject haObj = new HttpAccessObject(ip, port);
            _httpDic.Add(name, haObj);
        }

        public static void Create(string name, string ip, string port, string certPath)
        {
            if (_httpDic == null)
            {
                _httpDic = new Dictionary<string, HttpAccessObject>();
            }

            if (_httpDic.ContainsKey(name))
            {
                throw new HttpAccessHasExistException(name);
            }

            HttpAccessObject haObj = new HttpAccessObject(ip, port, HttpAccessObject.HttpType.HTTPS);
            haObj.CertPath = certPath;
            _httpDic.Add(name, haObj);
        }

        public static HttpWebResponse Post(string name, string url, List<HUnit> param = null, string Refer = null, HttpDataType dataType = HttpDataType.DATA)
        {
            if (_httpDic == null)
            {
                throw new HttpAccessNotExistException(name);
            }
            if(!_httpDic.ContainsKey(name))
            {
                throw new HttpAccessNotExistException(name);
            }

            if (PreviewPost != null)
            {
                if (!PreviewPost(url, System.Text.Encoding.Default, param, Refer, dataType))
                {
                    throw new HttpPreviewPostCanceledException();
                }
            }

            HttpAccessObject haObj = _httpDic[name];

            switch(dataType)
            {
                case HttpDataType.DATA:
                    {
                        haObj.PostData(url, param, Refer);
                        break;
                    }
                case HttpDataType.JSON:
                    {
                        if (param != null)
                        {
                            haObj.PostJson(url, param[0].Key, param[0].Value);
                        }
                        else
                        {
                            haObj.PostJson(url);
                        }
                        break;
                    }
                case HttpDataType.OCTEST:
                    {
                        if (param != null)
                        {
                            haObj.PostOctestStream(url, param[0].Value);
                        }
                        else
                        {
                            haObj.PostOctestStream(url);
                        }
                        break;
                    }
                case HttpDataType.STRING:
                    {
                        if (param != null)
                        {
                            haObj.PostString(url, param[0].Value);
                        }
                        else
                        {
                            haObj.PostString(url);
                        }
                        break;
                    }
            }

            return CheckResponseValid(haObj.Response);
        }

        public static HttpWebResponse Post(string name, string url, System.Text.Encoding encoding, List<HUnit> param = null, string Refer= null, HttpDataType dataType = HttpDataType.DATA)
        {
            if (_httpDic == null)
            {
                throw new HttpAccessNotExistException(name);
            }
            if (!_httpDic.ContainsKey(name))
            {
                throw new HttpAccessNotExistException(name);
            }

            if (PreviewPost != null)
            {
                if (!PreviewPost(url, encoding, param, Refer, dataType))
                {
                    throw new HttpPreviewPostCanceledException();
                }
            }

            HttpAccessObject haObj = _httpDic[name];

            switch (dataType)
            {
                case HttpDataType.DATA:
                    {
                        haObj.PostData(url, encoding, param, Refer);
                        break;
                    }
                case HttpDataType.JSON:
                    {
                        if (param != null)
                        {
                            haObj.PostJson(url, param[0].Key, param[0].Value);
                        }
                        else
                        {
                            haObj.PostJson(url);
                        }
                        break;
                    }
                case HttpDataType.OCTEST:
                    {
                        if (param != null)
                        {
                            haObj.PostOctestStream(url, param[0].Value);
                        }
                        else
                        {
                            haObj.PostOctestStream(url);
                        }
                        break;
                    }
                case HttpDataType.STRING:
                    {
                        if (param != null)
                        {
                            haObj.PostString(url, param[0].Value);
                        }
                        else
                        {
                            haObj.PostString(url);
                        }
                        break;
                    }
            }
            return CheckResponseValid(haObj.Response);
        }

        public static HttpWebResponse Get(string name, string url)
        {
            if (_httpDic == null)
            {
                throw new HttpAccessNotExistException(name);
            }
            if (!_httpDic.ContainsKey(name))
            {
                throw new HttpAccessNotExistException(name);
            }
            HttpAccessObject haObj = _httpDic[name];
            haObj.Get(url);
            return CheckResponseValid(haObj.Response);
        }

        public static HttpWebResponse Get(string name, string url, List<HUnit> param = null, string Refer = "")
        {
            if (_httpDic == null)
            {
                throw new HttpAccessNotExistException(name);
            }
            if (!_httpDic.ContainsKey(name))
            {
                throw new HttpAccessNotExistException(name);
            }
            HttpAccessObject haObj = _httpDic[name];
            haObj.Get(url, param, Refer);
            return CheckResponseValid(haObj.Response);
        }

        public static HttpWebResponse Get(string url, Cookie cookie = null)
        {
            HttpAccessObject haObj = new HttpAccessObject(url, cookie);
            haObj.Get();
            return CheckResponseValid(haObj.Response);
        }

        public static void Close(string name)
        {
            if (_httpDic == null)
            {
                return;
            }
            if (!_httpDic.ContainsKey(name))
            {
                return;
            }
            HttpAccessObject haObj = _httpDic[name];

            _httpDic.Remove(name);
            haObj.Dispose();
            haObj = null;
        }

        public static void SetParams(HttpWebResponse response)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
            string s = sr.ReadToEnd();
            sr.Close();

            MatchCollection mCollection = Regex.Matches(s, "<input type=\"hidden\".*?>");
            HttpParam.Clear();
            foreach (Match m in mCollection)
            {
                Match nm = Regex.Match(m.Value, "name=\"(.*?)\"");
                Match vm = Regex.Match(m.Value, "value=\"(.*?)\"");
                HttpParam.Set(nm.Groups[1].Value, vm.Groups[1].Value);
            }
        }

        public static string GetContent(string name, Encoding encoding = null)
        {
            byte[] contentBytes = GetContent(name);
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            return encoding.GetString(contentBytes);
        }

        public static byte[] GetContent(string name)
        {
            if (_httpDic == null)
            {
                throw new HttpAccessNotExistException(name);
            }
            if (!_httpDic.ContainsKey(name))
            {
                throw new HttpAccessNotExistException(name);
            }
            HttpAccessObject haObj = _httpDic[name];
            HttpWebResponse response = CheckResponseValid(haObj.Response);

            using (MemoryStream contentBuffer = new MemoryStream())
            {
                Stream stream = response.GetResponseStream();
                stream.CopyTo(contentBuffer);
                if(string.IsNullOrEmpty(response.ContentEncoding))
                    return contentBuffer.ToArray();
                else if (string.Equals(response.ContentEncoding.ToLower(), "gzip"))
                    return DataFactory.GZipDecompress(contentBuffer.ToArray());
                else if (string.Equals(response.ContentEncoding.ToLower(), "deflate"))
                    return DataFactory.DeflateDecompress(contentBuffer.ToArray());
                else
                    return contentBuffer.ToArray();
            }
        }

        internal static void CheckFilter(string url, Encoding encoding, List<HUnit> param, string Refer, HttpDataType dataType)
        {
            if (HttpAccess.PreviewPost != null)
            {
                if (!PreviewPost(url, encoding, param, Refer, dataType))
                {
                    throw new HttpPreviewPostCanceledException();
                }
            }
        }

        private static HttpWebResponse CheckResponseValid(HttpWebResponse response)
        {
            if (response == null)
            {
                throw new HttpRequestFailedException("NoResponse", "Response is null");
            }
            switch (response.StatusCode)
            {
                default:
                    {
                        throw new HttpRequestFailedException(response.StatusCode.ToString(), response.StatusDescription);
                    }
                case HttpStatusCode.OK:
                case HttpStatusCode.Found:
                case HttpStatusCode.Ambiguous:
                case HttpStatusCode.Moved:
                    {
                        return response;
                    }
            }
        }
    }
}
