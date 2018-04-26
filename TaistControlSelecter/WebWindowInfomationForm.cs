using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Xml;
using System.Windows.Automation;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

using Mock;
using Mock.Data;
using Mock.Tools.Controls;

namespace TaistControlSelecter
{
    public partial class WebWindowInfomationForm : Form
    {
        private readonly string DataFileName = "Controls.dll";
        private string Path = string.Empty;
        private WebInfo wInfo = null;
        private bool findWindow = false;
        private bool findControl = false;
        private bool editControl = false;
        private string tagName = string.Empty;

        #region 构造方法
        public WebWindowInfomationForm()
        {
            InitializeComponent();

            this.Left = 5;
            this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            string tmpPath = Assembly.GetExecutingAssembly().Location;

            tmpPath = tmpPath.Substring(0, tmpPath.LastIndexOf('\\')) + "\\Lib\\";
            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }
            Path = tmpPath + DataFileName;

            if (!File.Exists(Path))
            {
                FileStream fs = File.Create(Path);
                fs.Close();
                fs.Dispose();
            }

            this.WindowName_textBox.GotFocus += OnTextBox_GotFocus;
            this.WindowName_textBox.LostFocus += OnTextBox_LostFocus;

            this.ElementName_textBox.GotFocus += OnTextBox_GotFocus;
            this.ElementName_textBox.LostFocus += OnTextBox_LostFocus;
        }
        #endregion

        #region 保存信息
        private void Submit_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.Equals("body",tagName))
                {
                    string windowName = WindowName_textBox.Text.Trim();
                    if (string.IsNullOrEmpty(windowName) || windowName.Equals("请输入窗口别名"))
                    {
                        MessageBox.Show(this, "请输入窗口别名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    string windowName = WindowName_textBox.Text.Trim();
                    string elementName = ElementName_textBox.Text.Trim();
                    if (string.IsNullOrEmpty(windowName) || windowName.Equals("请输入窗口别名"))
                    {
                        MessageBox.Show(this, "请输入窗口别名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(elementName) || elementName.Equals("请输入控件别名"))
                    {
                        MessageBox.Show(this, "请输入控件别名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (SaveControl(findWindow, findControl)) this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SelecterMainForm.CleanEventFun();
        }
        #endregion

        #region 设置控件列表
        private XmlDocument _elementList = null;
        public XmlDocument ElementList
        {
            set
            {
                _elementList = value;
                tagName = _elementList.SelectSingleNode("//TagName").InnerText.ToLower();
                RefreshForm();
            }
            get
            {
                return _elementList;
            }
        }
        #endregion

        #region 窗口动画
        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);
        /// <summary>
        /// 自左向右显示窗口
        /// </summary>
        private const Int32 AW_HOR_POSITIVE = 0x00000001;
        /// <summary>
        /// 自右向左
        /// </summary>
        private const Int32 AW_HOR_NEGATIVE = 0x00000002;
        /// <summary>
        /// 自顶向下
        /// </summary>
        private const Int32 AW_VER_POSITIVE = 0x00000004;
        /// <summary>
        /// 自下向上
        /// </summary>
        private const Int32 AW_VER_NEGATIVE = 0x00000008;
        /// <summary>
        /// 与AW_HIDE效果配合使用则效果为窗口几层重叠， 单独使用窗口向外扩展
        /// </summary>
        private const Int32 AW_CENTER = 0x00000010;
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        private const Int32 AW_HIDE = 0x00010000;
        /// <summary>
        /// 激活窗口
        /// </summary>
        private const Int32 AW_ACTIVATE = 0x00020000;
        /// <summary>
        /// 使用滑动类型
        /// </summary>
        private const Int32 AW_SLIDE = 0x00040000;
        /// <summary>
        /// 使用淡入效果
        /// </summary>
        private const Int32 AW_BLEND = 0x00080000;
        #endregion

        #region 显示隐藏事件
        private void WindowInfomationForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                RefreshForm();
                AnimateWindow(this.Handle, 100, AW_SLIDE + AW_HOR_POSITIVE + AW_ACTIVATE);
            }
            else
            {
                AnimateWindow(this.Handle, 100, AW_SLIDE + AW_HOR_NEGATIVE + AW_HIDE);
            }
        }
        #endregion

        #region 查询窗口
        private void FindWindowFromLibrary()
        {
            findWindow = false;
            findControl = false;
            wInfo = null;

            string winInfoString = DataFactory.ReadLibrary(Path, "WebObject");
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrEmpty(winInfoString))
            {
                return;
            }
            doc.LoadXml(winInfoString);
            WebInfo winfo;
            try
            {
                winfo = GetInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            XmlNodeList winInfoNodeList;
            winInfoNodeList = doc.SelectNodes(string.Format("//WebInfo[Name='{0}']", winfo.Name));

            if (winInfoNodeList.Count > 0)
            {
                if (string.Equals(tagName, "popwindow"))
                {
                    if (winInfoNodeList.Count > 1)
                    {
                        throw new Exception("录制了多个弹出窗口");
                    }
                    findWindow = true;
                    WebInfo wi = DataFactory.GetData<WebInfo>(winInfoNodeList[0], winInfoNodeList[0].Attributes["FriendlyName"].Value);

                    wInfo = new WebInfo();
                    //wInfo.Bh = wi.Bh;
                    wInfo.FriendlyName = wi.FriendlyName;
                    wInfo.Description = wi.Description;
                    wInfo.id = wi.id;
                    wInfo.Name = wi.Name;
                    wInfo.Version = wi.Version;
                    WebElementInfo newWebElementInfo = winfo.GetElementInfo()[0];
                    List<WebElementInfo> eInfoList = wi.GetElementInfo();

                    if (eInfoList == null)
                    {
                        findControl = false;
                    }
                    else
                    {
                        Dictionary<string, string> attrnew = newWebElementInfo.Attributes;
                        foreach (WebElementInfo ei in eInfoList)
                        {
                            Dictionary<string, string> attrold = ei.Attributes;

                            if (CompareDictionary(attrold, attrnew))
                            {
                                findControl = true;
                                wInfo.AddElementInfo(ei);
                            }
                        }
                    }
                }
                else
                {
                    //if (winInfoNodeList.Count > 1)
                    //{
                    //    throw new Exception("对象库异常，该名称的窗口出现在多个节点");
                    //}
                    //else
                    //{
                        foreach (XmlNode tmp in winInfoNodeList)
                        {
                            XmlDocument tmpDoc = new XmlDocument();
                            tmpDoc.LoadXml(tmp.OuterXml);
                            string friendlyName = tmp.Attributes["FriendlyName"].Value;
                            WebInfo wi = DataFactory.GetData<WebInfo>(tmpDoc, friendlyName);
                            bool flag = true;

                            XmlNode body = _elementList.SelectSingleNode("//body");
                            string bodyString = body.InnerText;
                            string pattern = null;

                            foreach (WebElementInfo wei in wi.ElementInfo)
                            {
                                flag = true;

                                Dictionary<string, string> pro = wei.getProperty();
                                if (pro == null)
                                {
                                    flag = false;
                                    break;
                                }
                                foreach (string key in pro.Keys)
                                {
                                    pattern = string.Format("<{0}\\s*?[^>]*?\\s*?{1}\\s*?=\\s*?\\\\?['\"]?{2}\\\\?['\"]?.*?>", wei.TagName, key, pro[key]);
                                    Match m = Regex.Match(bodyString, pattern, RegexOptions.IgnoreCase);
                                    if (!m.Success)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag) break;
                            }
                            if (flag)
                            {
                                wInfo = new WebInfo();
                                wInfo.Version = wi.Version;
                                wInfo.Name = wi.Name;
                                wInfo.id = wi.id;
                                wInfo.FriendlyName = wi.FriendlyName;
                                if (!string.Equals("body", tagName))
                                {
                                    List<WebElementInfo> eInfoList = wi.GetElementInfo();
                                    if (eInfoList == null || eInfoList.Count == 0)
                                    {
                                        findControl = false;
                                    }
                                    else
                                    {
                                        WebElementInfo newElementInfo = winfo.GetElementInfo()[0];
                                        List<WebElementInfo> findedeInfoList = new List<WebElementInfo>();
                                        foreach (WebElementInfo ei in eInfoList)
                                        {
                                            Dictionary<string, string> propertyInfo = ei.getProperty();
                                            if (propertyInfo == null || propertyInfo.Count == 0) continue;
                                            Dictionary<string, string> checkPropertyInfo = winfo.ElementInfo[0].getProperty();
                                            flag = true;
                                            foreach (string key in propertyInfo.Keys)
                                            {
                                                if (!checkPropertyInfo.ContainsKey(key) || !string.Equals(checkPropertyInfo[key].ToLower().Trim(), propertyInfo[key].ToLower().Trim()))
                                                {
                                                    flag = false;
                                                    break;
                                                }
                                            }
                                            if (flag)
                                            {
                                                findedeInfoList.Add(ei);
                                            }
                                        }

                                        if (findedeInfoList.Count > 1)
                                        {
                                            throw new Exception("录制了两个相同的控件？！！\n基本不会出现吧！");
                                        }
                                        else if (findedeInfoList.Count == 1)
                                        {
                                            wInfo.AddElementInfo(findedeInfoList[0]);
                                            findControl = true;
                                        }
                                        else
                                        {
                                            findControl = false;
                                        }
                                    }
                                }
                                findWindow = true;
                                break;
                        //    }
                        }
                    }
                }
            }
        }
        #endregion

        private bool CompareDictionary(Dictionary<string, string> dic1, Dictionary<string, string> dic2)
        {
            if (dic1 == null)
            {
                if (dic2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (dic2 == null)
                {
                    return false;
                }
                else
                {
                    if (dic1.Count == dic2.Count)
                    {
                        foreach (KeyValuePair<string, string> kv in dic1)
                        {
                            string key = kv.Key;
                            string value = kv.Value;
                            if (dic2.ContainsKey(key))
                            {
                                return string.Equals(dic2[key], value);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        #region 保存窗口
        private bool SaveControl(bool fWindow, bool fControl)
        {
            string windowName = WindowName_textBox.Text.Trim();
            string elementName = ElementName_textBox.Text.Trim();
            WebInfo winfo = GetInfo();
            winfo.FriendlyName = windowName;

            string winInfoString = DataFactory.ReadLibrary(Path, "WebObject");
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrEmpty(winInfoString))
            {
                doc.LoadXml(InitializationString.WebObject);
            }
            else
            {
                doc.LoadXml(winInfoString);
            }
            if (fWindow)
            {
                XmlNode windowNode = doc.SelectSingleNode(string.Format("//WebInfo[@id='{0}']", wInfo.id));
                if (windowNode == null)
                {
                    MessageBox.Show(this, "根据id没找到窗口", "错啦", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (string.Equals("body", tagName))
                {
                    //保存的只有窗口信息
                    string oldFriendlyName = windowNode.Attributes["FriendlyName"].Value;
                    if (!oldFriendlyName.Equals(winfo.FriendlyName))
                    {
                        XmlNodeList xnl = doc.SelectNodes(string.Format("//WebInfo[@FriendlyName='{0}']", oldFriendlyName));
                        foreach (XmlNode xn in xnl)
                        {
                            XmlAttribute xa = xn.Attributes["FriendlyName"];
                            xa.Value = winfo.FriendlyName;
                            XmlNode node = xn.SelectSingleNode("Bh");
                            node.InnerText = winfo.FriendlyName;
                        }
                    }
                    return true;
                }
                else
                {
                    //保存窗口与控件信息
                    WebElementInfo ei = winfo.GetElementInfo()[0];
                    List<string> propertyNameList = new List<string>();
                    foreach (DataGridViewRow row in dataGridView_info.Rows)
                    {
                        if ((bool)row.Cells[0].Value)
                        {
                            propertyNameList.Add(row.Cells[1].Value.ToString());
                        }
                    }
                    Dictionary<string,string> pDic = ei.getProperty();
                    Dictionary<string, string> cpDic = new Dictionary<string, string>(pDic);
                    foreach (string key in cpDic.Keys)
                    {
                        if (!propertyNameList.Contains(key))
                        {
                            pDic.Remove(key);
                        }
                    }
                    cpDic = null;
                    ei.FriendlyName = elementName;
                    if (fControl)
                    {
                        //控件在库中已经存在
                        XmlNode elementNode = windowNode.SelectSingleNode(string.Format("WebElementInfo[@elementId='{0}']", wInfo.ElementInfo[0].elementId));
                        WebElementInfo oldElement = DataFactory.XmlToObject<WebElementInfo>(elementNode);

                        bool flag = true;
                        Dictionary<string, string> oldProperty = oldElement.getProperty();
                        Dictionary<string, string> newProperty = ei.getProperty();
                        foreach (string key in oldProperty.Keys)
                        {
                            if (!newProperty.ContainsKey(key) || string.Equals(newProperty[key], oldProperty[key]))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            //录制的控件与现有控件完全一致
                            if (oldElement.FriendlyName != ei.FriendlyName)
                            {
                                //修改FriendlyName
                                XmlAttribute fName = elementNode.Attributes["FriendlyName"];
                                fName.Value = ei.FriendlyName;
                            }
                        }
                        else
                        {
                            //录制的控件与现有控件不同
                            winfo.id = (doc.DocumentElement.ChildNodes.Count + 1).ToString();
                            ei.elementId = wInfo.ElementInfo[0].elementId;

                            if (string.Equals(oldElement.FriendlyName, ei.FriendlyName))
                            {
                                //录制控件与现有控件别名相同
                                //修改控件信息
                                XmlDocument tmpDoc = new XmlDocument();
                                tmpDoc.LoadXml(DataFactory.ObjectToXml(ei));
                                XmlNode neweNode = doc.ImportNode(tmpDoc.DocumentElement, true);
                                windowNode = windowNode.ReplaceChild(neweNode, elementNode);
                            }
                            else
                            {
                                //录制控件与现有控件别名不同
                                //新添加控件
                                XmlNodeList countList = windowNode.SelectNodes("WebElementInfo");
                                ei.elementId = (countList.Count + 1).ToString();
                                XmlDocument tmpDocument = new XmlDocument();
                                tmpDocument.LoadXml(DataFactory.ObjectToXml(ei));
                                XmlNode tmpNode = doc.ImportNode(tmpDocument.DocumentElement, true);
                                windowNode.AppendChild(tmpNode);
                            }
                        }
                    }
                    else
                    {
                        //当前库中不存在控件
                        XmlNode elementNode = windowNode.SelectSingleNode(string.Format("WebElementInfo[@FriendlyName='{0}']", ei.FriendlyName));
                        if (elementNode != null)
                        {
                            //当前库中存在与录制控件别名相同的控件
                            DialogResult dr = MessageBox.Show(this, "窗口中已经有该别名的控件，是否更新？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == System.Windows.Forms.DialogResult.Yes)
                            {
                                //更新新控件
                                ei.elementId = elementNode.Attributes["elementId"].Value;
                                XmlDocument tmpDocument = new XmlDocument();
                                tmpDocument.LoadXml(DataFactory.ObjectToXml(ei));
                                XmlNode tmpNode = doc.ImportNode(tmpDocument.DocumentElement, true);
                                windowNode.ReplaceChild(tmpNode, elementNode);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            //当前库中不存在与录制控件别名相同的控件
                            //添加新控件
                            XmlNodeList countList = windowNode.SelectNodes("WebElementInfo");
                            ei.elementId = (countList.Count + 1).ToString();
                            XmlDocument tmpDocument = new XmlDocument();
                            tmpDocument.LoadXml(DataFactory.ObjectToXml(ei));
                            XmlNode tmpNode = doc.ImportNode(tmpDocument.DocumentElement, true);
                            windowNode.AppendChild(tmpNode);
                        }
                    }

                    string oldFriendlyName = windowNode.Attributes["FriendlyName"].Value;
                    if (!string.Equals(oldFriendlyName, winfo.FriendlyName))
                    {
                        //窗口别名与库窗口别名不一致
                        //修改所有该别名的窗口
                        XmlNodeList xnl = doc.SelectNodes(string.Format("//WebInfo[@FriendlyName='{0}']", oldFriendlyName));
                        foreach (XmlNode xn in xnl)
                        {
                            XmlAttribute xa = xn.Attributes["FriendlyName"];
                            xa.Value = winfo.FriendlyName;
                            XmlNode node = xn.SelectSingleNode("Bh");
                            node.InnerText = winfo.FriendlyName;
                        }
                    }
                }
            }
            else
            {
                //未找到窗口
                //添加窗口
                XmlNodeList windowNodeList = doc.SelectNodes(string.Format("//WebInfo[@FriendlyName='{0}']", winfo.FriendlyName));
                if (windowNodeList != null && windowNodeList.Count > 0)
                {
                    MessageBox.Show(this, "该窗口别名已经有窗口使用，请重新输入窗口别名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    winfo.id = (doc.DocumentElement.ChildNodes.Count + 1).ToString();
                    if (!string.Equals("body", tagName))
                    {
                        WebElementInfo ei = winfo.GetElementInfo()[0];
                        List<string> propertyNameList = new List<string>();
                        foreach (DataGridViewRow row in dataGridView_info.Rows)
                        {
                            if ((bool)row.Cells[0].Value)
                            {
                                propertyNameList.Add(row.Cells[1].Value.ToString());
                            }
                        }
                        Dictionary<string, string> pDic = ei.getProperty();
                        Dictionary<string, string> cpDic = new Dictionary<string, string>(pDic);
                        foreach (string key in cpDic.Keys)
                        {
                            if (!propertyNameList.Contains(key))
                            {
                                pDic.Remove(key);
                            }
                        }
                        cpDic = null;
                        ei.FriendlyName = elementName;
                        ei.elementId = "1";
                    }
                    string windowInfoString = winfo.ToXml();
                    XmlDocument winfoDoc = new XmlDocument();
                    winfoDoc.LoadXml(windowInfoString);
                    XmlNode xe = doc.ImportNode(winfoDoc.DocumentElement, true);
                    doc.DocumentElement.AppendChild(xe);
                }
            }
            doc.Save(string.Format("{0}.xml", "WebObject"));
            DataFactory.WriteLibrary(Path, "WebObject", doc.OuterXml);
            return true;
        }
        #endregion

        #region 窗口信息
        private WebInfo GetInfo()
        {
            WebInfo winfo = new WebInfo();
            winfo.Version = "1";
            winfo.Name = _elementList.SelectSingleNode("//WindowName").InnerText;
            WebElementInfo einfo = new WebElementInfo();
            einfo.Inherit = false;
            einfo.TagName = _elementList.SelectSingleNode("//TagName").InnerText;
            XmlNode propertyNode = _elementList.SelectSingleNode("//Prop");
            foreach(XmlNode node in propertyNode.ChildNodes)
            {
                einfo.setProperty(node.Name, node.InnerText);
            }
            winfo.AddElementInfo(einfo);
            return winfo;
        }
        #endregion

        #region 添加版本
        private void AddVersion_Btn_Click(object sender, EventArgs e)
        {
        //    try
        //    {
        //        AddVersion();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        }

        //private void AddVersion()
        //{
        //    DateTime date = Robot.GetFileTime();
        //    string VersionXmlString = DataFactory.ReadLibrary(Path, "Version");

        //    XmlDocument doc = new XmlDocument();
        //    if (string.IsNullOrEmpty(VersionXmlString))
        //    {
        //        doc.LoadXml(InitializationString.Version);
        //    }
        //    else
        //    {
        //        doc.LoadXml(VersionXmlString);
        //    }

        //    XmlNode pNode = doc.SelectSingleNode(string.Format("//{0}", "WebObject"));

        //    if (pNode == null)
        //    {
        //        XmlElement pElement = doc.CreateElement("WebObject");
        //        Version_textBox.Text = "1";
        //        VerInfo newVerInfo = new VerInfo();
        //        newVerInfo.VerId = "1";
        //        newVerInfo.StartDate = "19000101";
        //        newVerInfo.EndDate = "99991231";
        //        XmlDocument newVerDocument = new XmlDocument();
        //        newVerDocument.LoadXml(newVerInfo.ToXml());
        //        XmlNode newVerNode = doc.ImportNode(newVerDocument.DocumentElement, true);
        //        pElement.AppendChild(newVerNode);
        //        doc.DocumentElement.AppendChild(pElement);
        //        DataFactory.WriteLibrary(Path, "Version", doc.OuterXml);
        //    }
        //    else
        //    {
        //        XmlNode checkNode = pNode.SelectSingleNode(string.Format("VerInfo[StartDate = {0} or EndDate = {0}]", date.ToString("yyyyMMdd")));
        //        if (checkNode == null)
        //        {
        //            XmlNodeList verList = pNode.SelectNodes(string.Format("VerInfo", ProcessName));
        //            Version_textBox.Text = (verList.Count + 1).ToString();
        //            VerInfo newVerInfo = new VerInfo();
        //            newVerInfo.VerId = (verList.Count + 1).ToString();
        //            string nextVer = null;
        //            if (verList.Count > 0)
        //            {
        //                //有历史版本
        //                //根据日期拆分历史版本
        //                string tmpList = string.Empty;
        //                XmlNode modifyNode = pNode.SelectSingleNode(string.Format("VerInfo[StartDate < {0} and EndDate > {0}]", date.ToString("yyyyMMdd")));

        //                VerInfo oldVerInfo = DataFactory.XmlToObject<VerInfo>(modifyNode);
        //                if (string.Equals(oldVerInfo.EndDate, "99991231"))
        //                {
        //                    //要拆分的版本为最新的版本
        //                    //将当前软件日期设为版本起始日期
        //                    newVerInfo.StartDate = date.ToString("yyyyMMdd");
        //                    newVerInfo.EndDate = oldVerInfo.EndDate;
        //                    oldVerInfo.EndDate = date.AddDays(-1).ToString("yyyyMMdd");
        //                }
        //                else
        //                {
        //                    //要拆分的版本为历史发布版本
        //                    //将当前软件日期设为版本终止日期
        //                    newVerInfo.StartDate = oldVerInfo.StartDate;
        //                    newVerInfo.EndDate = date.ToString("yyyyMMdd");
        //                    oldVerInfo.StartDate = date.AddDays(1).ToString("yyyyMMdd");
        //                }
        //                nextVer = oldVerInfo.VerId.ToString();
        //                XmlDocument oldVerDocument = new XmlDocument();
        //                oldVerDocument.LoadXml(oldVerInfo.ToXml());
        //                XmlNode oldNode = doc.ImportNode(oldVerDocument.DocumentElement, true);
        //                pNode.ReplaceChild(oldNode, modifyNode);
        //            }
        //            else
        //            {
        //                //无历史版本
        //                //添加第一个版本
        //                newVerInfo.StartDate = "19000101";
        //                newVerInfo.EndDate = "99991231";
        //                nextVer = "1";
        //            }

        //            XmlDocument newVerDocument = new XmlDocument();
        //            newVerDocument.LoadXml(newVerInfo.ToXml());
        //            XmlNode newVerNode = doc.ImportNode(newVerDocument.DocumentElement, true);
        //            pNode.AppendChild(newVerNode);
        //            DataFactory.WriteLibrary(Path, "Version", doc.OuterXml);
        //            ChangeWinObject(nextVer, newVerInfo.VerId);
        //            ChangeControlName(nextVer, newVerInfo.VerId);
        //        }
        //        else
        //        {
        //            checkNode = checkNode.SelectSingleNode("VerId");
        //            Version_textBox.Text = checkNode.InnerText.Trim();
        //        }
        //    }
        //}
        #endregion

        #region ChangeWinObject
        private void ChangeWinObject(string nextVer, string verId)
        {
            string WinObjectString = DataFactory.ReadLibrary(Path, "WebObject");
            if (!string.IsNullOrEmpty(WinObjectString))
            {
                XmlDocument WinObjectDocument = new XmlDocument();
                WinObjectDocument.LoadXml(WinObjectString);
                XmlNodeList windowNodeList = WinObjectDocument.SelectNodes("//WinInfo");
                foreach (XmlNode winInfoNode in windowNodeList)
                {
                    XmlAttribute attr = winInfoNode.Attributes["Version"];
                    if (string.IsNullOrEmpty(attr.Value.Trim())) continue;
                    string[] verArray = attr.Value.Split(',');
                    if (verArray.Contains(nextVer))
                    {
                        attr.Value = attr.Value + "," + verId;
                    }
                }
                DataFactory.WriteLibrary(Path, "WebObject", WinObjectDocument.OuterXml);
            }
        }
        #endregion

        #region ChangeControlName
        private void ChangeControlName(string nextVer, string verId)
        {
            string ControlNameString = DataFactory.ReadLibrary(Path, "ControlName");
            if (!string.IsNullOrEmpty(ControlNameString))
            {
                XmlDocument ControlNameDocument = new XmlDocument();
                ControlNameDocument.LoadXml(ControlNameString);
                XmlNodeList ControlNameNodeList = ControlNameDocument.SelectNodes("//ControlName");
                foreach (XmlNode ControlNameNode in ControlNameNodeList)
                {
                    XmlAttribute attr = ControlNameNode.Attributes["Version"];
                    if (string.IsNullOrEmpty(attr.Value.Trim())) continue;
                    string[] verArray = attr.Value.Split(',');
                    if (verArray.Contains(nextVer))
                    {
                        attr.Value = attr.Value + "," + verId;
                    }
                }
                DataFactory.WriteLibrary(Path, "ControlName", ControlNameDocument.OuterXml);
            }
        }
        #endregion

        #region 刷新页面显示内容
        private void RefreshForm()
        {
            try
            {
                editControl = false;
                Version_textBox.Text = "1";
                FindWindowFromLibrary();
                if (string.Equals("body", tagName))
                {
                    if (wInfo != null)
                    {
                        WindowName_textBox.ForeColor = System.Drawing.Color.Black;
                        WindowName_textBox.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular);
                        WindowName_textBox.Text = wInfo.FriendlyName.Trim();
                        WindowName_textBox.ReadOnly = true;
                    }
                    else
                    {
                        WindowName_textBox.ForeColor = System.Drawing.Color.Gray;
                        WindowName_textBox.Font = new System.Drawing.Font("微软雅黑", 8.5f, System.Drawing.FontStyle.Italic);
                        WindowName_textBox.Text = "请输入窗口别名";
                        WindowName_textBox.ReadOnly = false;
                    }
                    ElementName_textBox.Visible = false;
                    label9.Visible = false;
                }
                else
                {
                    ElementName_textBox.Visible = true;
                    label9.Visible = true;
                    if (wInfo != null)
                    {
                        WindowName_textBox.ForeColor = System.Drawing.Color.Black;
                        WindowName_textBox.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular);
                        WindowName_textBox.Text = wInfo.FriendlyName.Trim();
                        WindowName_textBox.ReadOnly = true;

                        List<WebElementInfo> eList = wInfo.GetElementInfo();
                        if (eList == null || eList.Count == 0)
                        {
                            ElementName_textBox.ForeColor = System.Drawing.Color.Gray;
                            ElementName_textBox.Font = new System.Drawing.Font("微软雅黑", 8.5f, System.Drawing.FontStyle.Italic);
                            ElementName_textBox.Text = "请输入控件别名";
                            ElementName_textBox.ReadOnly = false;
                        }
                        else
                        {
                            ElementName_textBox.ForeColor = System.Drawing.Color.Black;
                            ElementName_textBox.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular);
                            ElementName_textBox.Text = wInfo.GetElementInfo()[0].FriendlyName.Trim();
                            ElementName_textBox.ReadOnly = true;
                        }
                    }
                    else
                    {
                        WindowName_textBox.ForeColor = System.Drawing.Color.Gray;
                        WindowName_textBox.Font = new System.Drawing.Font("微软雅黑", 8.5f, System.Drawing.FontStyle.Italic);
                        WindowName_textBox.Text = "请输入窗口别名";
                        WindowName_textBox.ReadOnly = false;

                        ElementName_textBox.ForeColor = System.Drawing.Color.Gray;
                        ElementName_textBox.Font = new System.Drawing.Font("微软雅黑", 8.5f, System.Drawing.FontStyle.Italic);
                        ElementName_textBox.Text = "请输入控件别名";
                        ElementName_textBox.ReadOnly = false;
                    }
                }

                dataGridView_info.Rows.Clear();
                int index = 0;
                XmlNode prop = _elementList.SelectSingleNode("//Prop");
                foreach (XmlNode node in prop.ChildNodes)
                {
                    index = dataGridView_info.Rows.Add();
                    DataGridViewRow row = dataGridView_info.Rows[index];
                    row.Cells[0].Value = false;
                    row.Cells[1].Value = node.Name;
                    row.Cells[2].Value = node.InnerText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 获得焦点
        private void OnTextBox_GotFocus(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = System.Drawing.Color.Black;
            tmp.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular);
            if (tmp.Text.Contains("请输入"))
            {
                tmp.Text = "";
            }
        }
        #endregion

        #region 失去焦点
        private void OnTextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            if (string.IsNullOrEmpty(tmp.Text))
            {
                tmp.ForeColor = System.Drawing.Color.Gray;
                tmp.Font = new System.Drawing.Font("微软雅黑", 8.5f, System.Drawing.FontStyle.Italic);
                if (tmp.Name == "WindowName_textBox")
                {
                    tmp.Text = "请输入窗口别名";
                }
                else if (tmp.Name == "ElementName_textBox")
                {
                    tmp.Text = "请输入控件别名";
                }
            }
            else
            {
                if (editControl)
                {
                    tmp.ReadOnly = true;
                }
            }
        }
        #endregion

        private void Cancel_Btn_Click(object sender, EventArgs e)
        {
            SelecterMainForm.CleanEventFun();
            this.Hide();
        }

        private void Name_textBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            VerInfo vInfo = Robot.GetSoftwareVersion();
            DateTime fileTime = Robot.GetFileTime();
            
            if (vInfo.EndDate == "99991231" || vInfo.EndDate == fileTime.ToString("yyyyMMdd"))
            {
                TextBox tb = sender as TextBox;
                tb.ReadOnly = false;
                editControl = true;
            }
        }

        private void WindowAndElementNameText_MouseEnter(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.ReadOnly)
            {
                toolTip.SetToolTip((Control)sender, "双击编辑");
            }
        }

        private void AddVersion_Btn_MouseEnter(object sender, EventArgs e)
        {
            toolTip.SetToolTip((Control)sender, "添加一个新版本");
        }

        private void Manual_Btn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this, "手工修改对象库功能，正在开发...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML文件|*.xml";
            DialogResult dr = ofd.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ofd.FileName);
                DataFactory.WriteLibrary(Path, "WebObject", doc.OuterXml);
            }
        }

        private void WebWindowInfomationForm_Load(object sender, EventArgs e)
        {
            SelecterMainForm.DrawEventFun();
        }
    }
}
