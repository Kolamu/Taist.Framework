namespace Runner
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;
    using System.Reflection;

    using Mock;
    using Mock.Data;
    internal class Functions
    {
        private TestService testService = new TestService();
        private string StartTest(Dictionary<string, string> args)
        {
            try
            {
                if (testService.Busy)
                {
                    throw new Exception("当前系统正在执行测试用例");
                }

                if (args.ContainsKey("bhs"))
                {
                    string bhs = args["bhs"];
                    if (!string.IsNullOrEmpty(bhs))
                    {
                        List<string> bhArray = DataFactory.ParseBH(bhs.Replace('\n', ','));
                        XmlDocument doc = XmlFactory.LoadXml(XmlFactory.XmlRootString);
                        XmlNode data = doc.SelectSingleNode("//Data");
                        foreach (string bh in bhArray)
                        {
                            if (string.IsNullOrEmpty(bh)) continue;
                            XmlElement bhNode = doc.CreateElement("Bh");
                            bhNode.InnerText = bh;
                            data.AppendChild(bhNode);
                        }
                        doc.Save(Path.Combine(Mock.Config.WorkingDirectory, "TestCasePool.xml"));
                    }
                }
                testService.Start();
                return null;
            }
            catch (Exception ex)
            {
                return ReportException(ex, "开始测试异常");
            }
        }

        private void StopTest()
        {
            testService.Stop();
        }

        private string ViewSchedule()
        {
            try
            {
                string path = Path.Combine(Config.WorkingDirectory, "Report\\case.xml");
                XmlDocument doc = XmlFactory.LoadXml(path);
                XmlNodeList xn = doc.SelectNodes("//case");
                int total = xn.Count;
                xn = doc.SelectNodes("//case[state='S']");
                int success = xn.Count;
                xn = doc.SelectNodes("//case[state='F']");
                int fail = xn.Count;
                xn = doc.SelectNodes("//startTestDate");
                DateTime startTime = DateTime.Parse(xn[0].InnerText);
                xn = doc.SelectNodes("//endTestDate");
                string endString = xn[0].InnerText;
                TimeSpan costTime;
                if (string.IsNullOrEmpty(endString))
                {
                    costTime = DateTime.Now - startTime;
                }
                else
                {
                    costTime = DateTime.Parse(endString) - startTime;
                }
                string message = Properties.Resources.SchedulePage.Replace("[total]", total.ToString());
                message = message.Replace("[runned]", string.Format("{0}", success + fail));
                message = message.Replace("[pending]", string.Format("{0}", total - success - fail));
                message = message.Replace("[success]", string.Format("{0}", success));
                message = message.Replace("[fail]", string.Format("{0}", fail));
                message = message.Replace("[curpercent]", string.Format("{0:F2}%", (success + fail) / total * 100));
                if (success + fail == 0)
                {
                    message = message.Replace("[sucesspercent]", "0.00%");
                }
                else
                {
                    message = message.Replace("[sucesspercent]", string.Format("{0:F2}%", success / (success + fail) * 100));
                }
                message = message.Replace("[start]", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
                message = message.Replace("[end]", string.Format("{0}天{1}时{2}分{3}秒", costTime.Days, costTime.Hours, costTime.Minutes, costTime.Seconds));

                return message;
            }
            catch (Exception ex)
            {
                return ReportException(ex, "查看进度异常");
            }
        }

        private string ViewReport()
        {
            try
            {
                string report = null;
                using (StreamReader sr = new StreamReader(File.Open("TaistReport.html", FileMode.Open)))
                {
                    report = sr.ReadToEnd();
                    sr.Close();
                }
                return report;
            }
            catch (Exception ex)
            {
                return ReportException(ex, "查看报告异常");
            }
        }

        private string ViewCache()
        {
            string path = Path.Combine(Config.WorkingDirectory, "Temp\\taist.temp");
            return ReportFile(path, "自动化测试缓存");
        }

        private string ViewLog()
        {
            string path = Path.Combine(Config.WorkingDirectory, "Robot.log");
            return ReportFile(path, "自动化测试报告");
        }

        private string DeleteLog()
        {
            try
            {
                string path = Path.Combine(Config.WorkingDirectory, "Robot.log");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return null;
            }
            catch (Exception ex)
            {
                return ReportException(ex, "删除日志异常");
            }
        }

        private string ReportMessage(string message, string title)
        {
            message = Properties.Resources.MessagePage.Replace("[message]", message.Replace("\n", "<br>"));
            message = message.Replace("[title]", title);
            return message;
        }

        private string ReportFile(string path, string title)
        {
            try
            {
                string logString = "";
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(File.Open(path, FileMode.Open), System.Text.Encoding.Default))
                    {
                        logString = sr.ReadToEnd();
                        sr.Close();
                    }
                }

                logString = Properties.Resources.MessagePage.Replace("[message]", logString.Replace("\n", "<br>"));
                logString = logString.Replace("[title]", title);
                return logString;
            }
            catch (Exception ex)
            {
                return ReportException(ex, title + "异常");
            }
        }

        private string ReportException(Exception ex, string title)
        {
            string logString = string.Format("{0}<br>{1}", ex.Message, ex.StackTrace);
            return ReportMessage(logString, title);
        }

        private string Manage()
        {
            return Properties.Resources.MainPage;
        }

        internal string Invoke(string method, Dictionary<string, string> args = null)
        {
            try
            {
                if (method == null)
                {
                    return "未知的执行方法";
                }

                method = method.TrimStart('/');

                Type t = this.GetType();
                MethodInfo mi = t.GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Public);
                if (mi == null)
                {
                    return "请求的方法未找到";
                }

                object obj = null;
                try
                {
                    if (args == null || args.Count == 0)
                    {
                        obj = mi.Invoke(this, null);
                    }
                    else
                    {
                        obj = mi.Invoke(this, new object[] { args });
                    }
                }
                catch (Exception ex)
                {
                    return ex.InnerException.Message;
                }

                if (obj == null)
                {
                    return null;
                }
                else
                {
                    return obj.ToString();
                }
            }
            catch (Exception ex)
            {
                return ReportException(ex, "访问异常");
            }
        }
    }
}
