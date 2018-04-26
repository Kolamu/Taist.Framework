namespace Runner
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Text;
    using System.Net.Sockets;
    using System.Threading;
    using System.Collections.Generic;

    internal class ListenService
    {
        private int port = 9101;
        TcpListener listener = null;
        private bool isActive = false;
        private Functions fun = null;

        internal ListenService() { }

        ~ListenService()
        {
            Stop();
        }

        internal void Start()
        {
            isActive = true;
            fun = new Functions();
            new Thread(() =>
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                while (isActive)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Log.Debug("New client " + client.Client.RemoteEndPoint.ToString());
                    Thread clientThread = new Thread(ProcessClient);
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
            }).Start();
        }

        internal void ProcessClient(object state)
        {
            TcpClient client = state as TcpClient;
            string url = "";
            string method = "";
            string version = "";
            Dictionary<string, string> header = new Dictionary<string, string>();
            using (BufferedStream stream = new BufferedStream(client.GetStream()))
            {
                try
                {
                    string tmp = StreamReadLine(stream);
                    string[] baseInfo = tmp.Split(' ');
                    if (baseInfo.Length != 3)
                    {
                        Console.WriteLine("error http request " + tmp);
                        return;
                    }

                    method = baseInfo[0].ToUpper();
                    url = baseInfo[1];
                    version = baseInfo[2];
                    Console.WriteLine(url);
                    while (!string.IsNullOrEmpty(tmp))
                    {
                        tmp = StreamReadLine(stream);
                        Console.WriteLine(tmp);
                        string[] headerArray = tmp.Split(':');
                        if (headerArray.Length == 1)
                        {
                            header.Add(headerArray[0], "");
                        }
                        else
                        {
                            header.Add(headerArray[0], headerArray[1]);
                        }
                    }
                }
                catch { }
                using (StreamWriter sw = new StreamWriter(new BufferedStream(client.GetStream()), Encoding.GetEncoding("GBK")))
                {
                    switch (method)
                    {
                        case "GET":
                            {
                                doGet(url, sw);
                                break;
                            }
                        case "POST":
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    byte[] bArray = new byte[256];
                                    int contentLength = int.Parse(header["Content-Length"]);
                                    Console.WriteLine("Content-Length {0}", contentLength);
                                    int n = 0;
                                    int offset = 0;
                                    while (offset < contentLength)
                                    {
                                        n = stream.Read(bArray, 0, Math.Min(contentLength, 256));
                                        ms.Write(bArray, offset, n);
                                        offset += n;
                                    }

                                    string paramString = "";
                                    ms.Seek(0, SeekOrigin.Begin);
                                    using (StreamReader sr = new StreamReader(ms))
                                    {
                                        paramString = sr.ReadToEnd();
                                        sr.Close();
                                    }
                                    Dictionary<string, string> param = new Dictionary<string, string>();
                                    string[] kvArray = HttpUtility.UrlDecode(paramString, Encoding.GetEncoding("GBK")).Split('&');
                                    foreach (string kvString in kvArray)
                                    {
                                        string[] kv = kvString.Split('=');
                                        if (string.IsNullOrEmpty(kv[0]))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            if (kv.Length == 1)
                                            {
                                                param.Add(kv[0], null);
                                            }
                                            else
                                            {
                                                param.Add(kv[0], kv[1]);
                                            }
                                        }
                                    }
                                    doPost(url, sw, param);
                                    ms.Close();
                                }
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("error http method " + method);
                                break;
                            }
                    }
                    sw.Close();
                }
            }
        }

        internal string StreamReadLine(BufferedStream stream)
        {
            string s = "";
            while (true)
            {
                int b = stream.ReadByte();
                if (b == '\r') continue;
                if (b == '\n') break;
                if (b == -1) continue;
                s += Convert.ToChar(b);
            }
            return s;
        }

        internal void Stop()
        {
            isActive = false;
            try
            {
                TcpClient client = new TcpClient();
                client.Connect("localhost", port);
                client.Close();
            }
            catch { }
            if (listener != null)
            {
                listener.Stop();
            }
        }

        private void doPost(string url, StreamWriter sw, Dictionary<string, string> data)
        {
            string ret = fun.Invoke(url, data);
            if (string.IsNullOrEmpty(ret))
            {
                ReturnSuccessPage(sw, Properties.Resources.MainPage);
            }
            else
            {
                ReturnSuccessPage(sw, ret);
            }
        }

        private void doGet(string url, StreamWriter sw)
        {
            sw.WriteLine("HTTP/1.0 200 OK");
            sw.WriteLine("Content-Type: text/html");
            sw.WriteLine("Connection: close");
            sw.WriteLine("");

            sw.WriteLine(Properties.Resources.MainPage);
        }

        private void ReturnSuccessPage(StreamWriter sw, string pageVal)
        {
            sw.WriteLine("HTTP/1.0 200 OK");
            sw.WriteLine("Content-Type: text/html");
            sw.WriteLine("Connection: close");
            sw.WriteLine("");

            sw.WriteLine(pageVal);
        }

        public string GetData(string bh, string name)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<body>");
            sb.AppendLine(string.Format("<h1>{0}</h1>", bh));
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }
}
