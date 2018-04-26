namespace Mock.Tools.Controls
{
    using System;
    using System.Xml;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System.Windows.Automation;

    using mshtml;
    using SHDocVw;

    using Mock.Data;
    using Mock.Tools.Exception;

    using Mock.Tools.Web;
    internal abstract class WebObject
    {
        #region 变量
        private static object htmlDoc = null;
        protected IHTMLElement element = null;
        protected string _windowName = null;
        protected string _elementName = null;
        protected static string _CurrentWindowName = null;
        #endregion

        #region 控件方法
        
        protected void GetElement(string windowName)
        {
            LogManager.Debug(string.Format("GetWindow {0}", windowName));
            WebInfo info = GetWindowByFriendlyName(windowName);
            GetInternetExploreWindow(info.Name);
            if (RobotContext.IsWarningWindowExist)
            {
                throw new WarningWindowExistException();
            }
        }

        protected void GetElement(string windowName, string elementName)
        {
            LogManager.Debug(string.Format("GetElement {0} from {1}", elementName, windowName));
            //try
            //{
            //    WWindow.FindWindow("主窗口", 1);
            //}
            //catch (TaistException ex)
            //{
            //    if (ex is WarningWindowExistException) throw ex;
            //}
            WWindow.Listen();
            if (RobotContext.IsWarningWindowExist)
            {
                throw new WarningWindowExistException();
            }
            WebInfo info = GetWindowByFriendlyName(windowName);
            GetInternetExploreWindow(info.Name);
            WebElementInfo einfo = GetElementByFriendlyName(windowName, elementName);
            if (einfo.Attributes == null)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("innertext", elementName);
                FindElement(einfo.TagName, param);
            }
            else
            {
                FindElement(einfo.TagName, einfo.Attributes);
            }
        }

        protected void GetElement(string windowName, string elementName, string itemName)
        {
            WWindow.Listen();
            if (RobotContext.IsWarningWindowExist)
            {
                throw new WarningWindowExistException();
            }
            WebInfo info = GetWindowByFriendlyName(windowName);
            GetInternetExploreWindow(info.Name);
            WebElementInfo einfo = GetElementByFriendlyName(windowName, elementName);
            
            einfo.setProperty("innerText", itemName);
            FindElement(einfo.TagName, einfo.Attributes);
        }

        protected byte[] Post(string url, Dictionary<string, string> param)
        {
            IHTMLDocument2 doc2 = (IHTMLDocument2)htmlDoc;
            
            //HttpAccessObject hao = new HttpAccessObject(doc2.location.host, doc2.location.port, doc2.cookie);
            return null;
        }
        #endregion

        #region 私有方法
        private void GetInternetExploreWindow(string windowName)
        {
            IHTMLDocument2 ieDoc = null;
            htmlDoc = null;
            InternetExplorer ieWnd = null;
            LogManager.Debug(windowName);
            try
            {
                ShellWindows shellWindows = new SHDocVw.ShellWindows();
                foreach (SHDocVw.InternetExplorer ie in shellWindows)
                {
                    //if it is ie Window
                    LogManager.Debug(ie.FullName);
                    if (ie.FullName.ToUpper().IndexOf("IEXPLORE.EXE") > 0)
                    {
                        //get the document displayed
                        ieDoc = (IHTMLDocument2)ie.Document;
                        LogManager.Debug(ieDoc.title);
                        if (ieDoc.title.ToUpper().IndexOf(windowName) >= 0)
                        {
                            for (int i = 0; i < 100; i++)
                            {
                                if (!ie.Busy)
                                {
                                    break;
                                }
                                Robot.Recess(100);
                            }
                            RobotContext.InternetExploreWindow = AutomationElement.FromHandle((IntPtr)ie.HWND);

                            //if (!RobotContext.IsWebObjectInitilized)
                            //{
                           //    if (p != null)
                            //    {
                            //        RobotContext.IsWebObjectInitilized = true;
                            //        RobotContext.InternetExploreDepth = (int)p.Current.BoundingRectangle.Y;
                            //    }
                            //}
                            ieWnd = ie;

                            htmlDoc = ie.Document;
                            break;
                        }
                    }
                }
                shellWindows = null;
            }
            catch(Exception ex)
            {
                LogManager.ErrorOnlyPrint(ex);
                htmlDoc = null;
            }

            if (htmlDoc == null)
            {
                throw new NullControlException(_windowName);
            }
            Robot.ExecuteWithTimeOut(() =>
                {
                    while (ieWnd.Busy)
                    {
                        Robot.Recess(100);
                    }
                }, 60000, false);
            if (ieWnd.Busy)
            {
                throw new WindowIsInactiveException(windowName);
            }
        }

        private void FindElement(string tagName, Dictionary<string, string> condition)
        {
            IHTMLDocument3 doc = (IHTMLDocument3)htmlDoc;
            
            IHTMLElementCollection elementCollection = doc.getElementsByTagName(tagName);
            bool find = true;
            foreach (IHTMLElement tmp in elementCollection)
            {
                find = true;
                
                foreach (string attrName in condition.Keys)
                {
                    if (string.Equals(attrName.ToLower(), "innertext"))
                    {
                        if (tmp.innerText == null)
                        {
                            find = false;
                            break;
                        }
                        if (!string.Equals(tmp.innerText.Trim().ToLower(), condition[attrName].Trim().ToLower()))
                        {
                            find = false;
                            break;
                        }
                    }
                    else if (string.Equals(attrName.ToLower(), "src"))
                    {
                        if (tmp.getAttribute(attrName) == null)
                        {
                            find = false;
                            break;
                        }
                        if (tmp.getAttribute(attrName).ToString().ToLower().IndexOf(condition[attrName].Trim().ToLower()) < 0)
                        //if (!string.Equals(tmp.getAttribute(attrName).ToString().ToLower(), condition[attrName].Trim().ToLower()))
                        {
                            find = false;
                            break;
                        }
                    }
                    else if (string.Equals(attrName, "class", StringComparison.OrdinalIgnoreCase))
                    {
                        if (tmp.className == null)
                        {
                            find = false;
                            break;
                        }
                        if (!string.Equals(tmp.className, condition[attrName].Trim().ToLower()))
                        {
                            find = false;
                            break;
                        }
                    }
                    else
                    {
                        if (tmp.getAttribute(attrName) == null)
                        {
                            find = false;
                            break;
                        }
                        //if (tmp.getAttribute(attrName).ToString().ToLower().IndexOf(condition[attrName].Trim().ToLower()) < 0)
                        if (!string.Equals(tmp.getAttribute(attrName).ToString().Trim().ToLower(), condition[attrName].Trim().ToLower()))
                        {
                            find = false;
                            break;
                        }

                    }
                }
                if (find)
                {
                    element = tmp;
                    break;
                }
            }

            if (!find)
            {
                throw new NullControlException(_windowName, _elementName);
            }
        }

        private XmlDocument GetXml(string name)
        {
            XmlDocument doc = new XmlDocument();
            //ControlLibraryObject controlLibrary = new ControlLibraryObject("Control.dll");
            string xml = string.Empty;
            if (System.IO.File.Exists("Controls.dll"))
            {
                xml = DataFactory.ReadLibrary("Controls.dll", name);
            }
            else
            {
                xml = DataFactory.ReadLibrary(System.IO.Path.Combine(Config.WorkingDirectory, "Lib\\Controls.dll"), name);
            }
            doc.LoadXml(xml);
            return doc;
        }

        private WebInfo GetWindowByFriendlyName(string windowName)
        {
            XmlDocument doc = GetXml("WebObject");
            XmlNodeList windowList = doc.SelectNodes(string.Format("//WebInfo[@FriendlyName='{0}']", windowName));
            if (windowList == null || windowList.Count < 1)
            {
                throw new CanNotFindNodeException("Controls", windowName);
            }

            XmlNode node = windowList[0];

            //foreach (XmlNode window in windowList)
            //{
            //    string[] Vers = window.Attributes["Version"].Value.Split(',');
            //    if (Vers.Contains(Robot.GetSoftwareVersion().VerId))
            //    {
            //        node = window;
            //        break;
            //    }
            //}

            //if (node == null)
            //{
            //    throw new CanNotFindNodeException("Controls", windowName);
            //}

            XmlDocument winDoc = new XmlDocument();
            winDoc.LoadXml(node.OuterXml);
            WebInfo info = DataFactory.GetData<WebInfo>(winDoc, windowName);
            
            return info;
        }

        private WebElementInfo GetElementByFriendlyName(string windowName, string elementName)
        {
            WebInfo winfo = GetWindowByFriendlyName(windowName);
            XmlDocument winDoc = new XmlDocument();
            winDoc.LoadXml(winfo.ToXml());
            XmlNode node = winDoc.DocumentElement;
            node = node.SelectSingleNode(string.Format("//WebElementInfo[@FriendlyName='{0}']", elementName));
            if (node == null)
            {
                throw new CanNotFindNodeException("Controls", string.Format("{0}/{1}", windowName, elementName));
            }
            WebElementInfo info = DataFactory.XmlToObject<WebElementInfo>(node);
            return info;
        }
        #endregion
    }
}
