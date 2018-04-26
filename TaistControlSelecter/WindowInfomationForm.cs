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

using Mock;
using Mock.Data;
using Mock.Tools.Controls;

namespace TaistControlSelecter
{
    public partial class WindowInfomationForm : Form
    {
        private readonly string DataFileName = "Controls.dll";
        private string Path = string.Empty;
        private WinInfo wInfo = null;
        private bool findWindow = false;
        private bool findControl = false;
        private bool editControl = false;
        private bool addVersion = false;
        private string ProcessName = string.Empty;

        #region 构造方法
        public WindowInfomationForm()
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
                if (_elementList.Count == 1)
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
                if (SaveControl(_elementList, findWindow, findControl)) this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SelecterMainForm.CleanEventFun();
        }
        #endregion

        #region 设置控件列表
        private List<AutomationElement> _elementList = null;
        public List<AutomationElement> ElementList
        {
            set
            {
                _elementList = value;
                ProcessName = Process.GetProcessById(_elementList[0].Current.ProcessId).ProcessName.ToUpper();
                Robot.SwitchTestSoftware(ProcessName);
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
            string verId = "1";
            try
            {
                verId = Robot.GetSoftwareVersion().VerId;
            }
            catch { }
            findWindow = false;
            findControl = false;
            wInfo = null;

            string winInfoString = DataFactory.ReadLibrary(Path, ProcessName);
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrEmpty(winInfoString))
            {
                return;
            }
            doc.LoadXml(winInfoString);
            WinInfo winfo;
            try
            {
                winfo = GetInfo(_elementList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            XmlNodeList winInfoNodeList;
            if (string.IsNullOrEmpty(winfo.AutomationId))
            {
                winInfoNodeList = doc.SelectNodes(string.Format("//WinInfo[AutomationId='{0}' and Name='{1}']", winfo.AutomationId, winfo.Name));
            }
            else
            {
                winInfoNodeList = doc.SelectNodes(string.Format("//WinInfo[AutomationId='{0}' and Name='{1}']", winfo.AutomationId, winfo.Name));
                if (winInfoNodeList == null || winInfoNodeList.Count == 0)
                {
                    winInfoNodeList = doc.SelectNodes(string.Format("//WinInfo[AutomationId='{0}']", winfo.AutomationId));
                }
            }
            if (winInfoNodeList.Count > 0)
            {
                foreach (XmlNode tmp in winInfoNodeList)
                {
                    string []verArray = tmp.Attributes["Version"].Value.Split(',');
                    List<string> verList = new List<string>();
                    foreach (string ver in verArray)
                    {
                        if (string.IsNullOrEmpty(ver.Trim()))
                        {
                            continue;
                        }
                        verList.Add(ver.Trim());
                    }

                    if (verList.Contains(verId))
                    {
                        XmlDocument tmpDoc = new XmlDocument();
                        tmpDoc.LoadXml(tmp.OuterXml);
                        WinInfo wi = DataFactory.GetData<WinInfo>(tmpDoc, tmp.Attributes["FriendlyName"].Value);
                        wInfo = new WinInfo();
                        wInfo.FriendlyName = wi.FriendlyName;
                        wInfo.Version = wi.Version;
                        wInfo.Name = wi.Name;
                        wInfo.Type = wi.Type;
                        wInfo.id = wi.id;
                        wInfo.AutomationId = wi.AutomationId;

                        if (_elementList.Count > 1)
                        {
                            List<ElementInfo> eInfoList = wi.GetElementInfo();
                            if (eInfoList == null || eInfoList.Count == 0)
                            {
                                findControl = false;
                            }
                            else
                            {
                                ElementInfo newElementInfo = winfo.GetElementInfo()[0];
                                List<ElementInfo> findedeInfoList = new List<ElementInfo>();
                                foreach (ElementInfo ei in eInfoList)
                                {
                                    if (string.Equals(ei.AutomationId, newElementInfo.AutomationId)
                                        && string.Equals(ei.RelativePath, newElementInfo.RelativePath)
                                        && string.Equals(ei.RelativePosition, newElementInfo.RelativePosition))
                                    {
                                        findedeInfoList.Add(ei);
                                    }
                                }

                                if (findedeInfoList.Count > 1)
                                {
                                    foreach (ElementInfo ei in findedeInfoList)
                                    {
                                        if (string.Equals(ei.Name, newElementInfo.Name))
                                        {
                                            wInfo.AddElementInfo(ei);
                                            findControl = true;
                                            break;
                                        }
                                    }
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
                    }
                }
            }
        }
        #endregion

        #region 保存窗口
        private bool SaveControl(List<AutomationElement> infoList, bool fWindow, bool fControl)
        {
            if (infoList == null || infoList.Count == 0)
            {
                return true;
            }
            if (addVersion)
            {
                AddVersion();
            }
            VerInfo verInfo = Robot.GetSoftwareVersion();
            string windowName = WindowName_textBox.Text.Trim();
            string elementName = ElementName_textBox.Text.Trim();
            WinInfo winfo = GetInfo(infoList);
            winfo.FriendlyName = windowName;

            string winInfoString = DataFactory.ReadLibrary(Path, ProcessName);
            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrEmpty(winInfoString))
            {
                doc.LoadXml(InitializationString.WinObject);
            }
            else
            {
                doc.LoadXml(winInfoString);
            }
            if (fWindow)
            {
                XmlNode windowNode = doc.SelectSingleNode(string.Format("//WinInfo[@id='{0}']", wInfo.id));
                if (windowNode == null)
                {
                    MessageBox.Show(this, "根据id没找到窗口", "错啦", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (infoList.Count == 1)
                {
                    //保存的只有窗口信息
                    string oldFriendlyName = windowNode.Attributes["FriendlyName"].Value;
                    if (!string.Equals(oldFriendlyName, winfo.FriendlyName))
                    {
                        XmlNodeList xnl = doc.SelectNodes(string.Format("//WinInfo[@FriendlyName='{0}']", oldFriendlyName));
                        foreach (XmlNode xn in xnl)
                        {
                            XmlAttribute xa = xn.Attributes["FriendlyName"];
                            xa.Value = winfo.FriendlyName;
                            XmlNode node = xn.SelectSingleNode("Bh");
                            node.InnerText = winfo.FriendlyName;
                        }
                        Robot.Note(string.Format("Only Window old {0} new {1} count {2}", oldFriendlyName, windowName, xnl.Count));
                    }
                }
                else
                {
                    //保存窗口与控件信息
                    ElementInfo ei = winfo.GetElementInfo()[0];
                    ei.FriendlyName = elementName;
                    if (fControl)
                    {
                        //控件在库中已经存在
                        XmlNode elementNode = windowNode.SelectSingleNode(string.Format("ElementInfo[@id='{0}']", wInfo.ElementInfo[0].id));
                        ElementInfo oldElement = DataFactory.XmlToObject<ElementInfo>(elementNode);

                        if (string.Equals(oldElement.Name, ei.Name)
                        && string.Equals(oldElement.AutomationId, ei.AutomationId)
                        && string.Equals(oldElement.RelativePath, ei.RelativePath)
                        && string.Equals(oldElement.RelativePosition, ei.RelativePosition))
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
                            ei.id = wInfo.ElementInfo[0].id;
                            XmlAttribute VerAttr = windowNode.Attributes["Version"];
                            string verString = VerAttr.Value.Trim().Trim(',');
                            if (!string.Equals(verString, winfo.Version))
                            {
                                //现有控件版本中含有其他的软件版本
                                verString = verString.Replace(verInfo.VerId, "");
                                verString = verString.Replace(",,", ",");
                                VerAttr.Value = verString;
                                if (string.Equals(oldElement.FriendlyName, ei.FriendlyName))
                                {
                                    //录制控件与现有控件别名相同
                                    //将该版本分离出一个新的版本
                                    XmlDocument winfoDoc = new XmlDocument();
                                    winfoDoc.LoadXml(windowNode.OuterXml);
                                    XmlNode newwinNode = winfoDoc.SelectSingleNode("//WinInfo");
                                    XmlAttribute idAttr = newwinNode.Attributes["id"];
                                    idAttr.Value = winfo.id;
                                    XmlAttribute vAttr = newwinNode.Attributes["Version"];
                                    vAttr.Value = winfo.Version;

                                    XmlNodeList eNodeList = newwinNode.SelectNodes("//ElementInfo");
                                    foreach (XmlNode n in eNodeList)
                                    {
                                        XmlAttribute xaInherit = n.Attributes["Inherit"];
                                        if (xaInherit == null)
                                        {
                                            xaInherit = winfoDoc.CreateAttribute("Inherit");
                                            n.Attributes.Append(xaInherit);
                                        }
                                        xaInherit.Value = "true";
                                    }

                                    XmlNode eNode = newwinNode.SelectSingleNode(string.Format("ElementInfo[@id='{0}']", wInfo.ElementInfo[0].id));

                                    XmlDocument tmpDoc = new XmlDocument();
                                    tmpDoc.LoadXml(DataFactory.ObjectToXml(ei));
                                    XmlNode neweNode = winfoDoc.ImportNode(tmpDoc.DocumentElement, true);
                                    newwinNode = newwinNode.ReplaceChild(neweNode, eNode);
                                    XmlNode xe = doc.ImportNode(winfoDoc.DocumentElement, true);
                                    doc.DocumentElement.AppendChild(xe);
                                }
                                else
                                {
                                    //录制控件与现有控件别名不同
                                    //新添加控件
                                    XmlNodeList countList = windowNode.SelectNodes("ElementInfo");
                                    ei.id = (countList.Count + 1).ToString();
                                    XmlDocument tmpDocument = new XmlDocument();
                                    tmpDocument.LoadXml(DataFactory.ObjectToXml(ei));
                                    XmlNode tmpNode = doc.ImportNode(tmpDocument.DocumentElement, true);
                                    windowNode.AppendChild(tmpNode);
                                }
                            }
                            else
                            {
                                //现有控件的版本中不含其他软件版本
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
                                    XmlNodeList countList = windowNode.SelectNodes("ElementInfo");
                                    ei.id = (countList.Count + 1).ToString();
                                    XmlDocument tmpDocument = new XmlDocument();
                                    tmpDocument.LoadXml(DataFactory.ObjectToXml(ei));
                                    XmlNode tmpNode = doc.ImportNode(tmpDocument.DocumentElement, true);
                                    windowNode.AppendChild(tmpNode);
                                }
                            }
                        }
                    }
                    else
                    {
                        //当前库中不存在控件
                        XmlNode elementNode = windowNode.SelectSingleNode(string.Format("ElementInfo[@FriendlyName='{0}']", ei.FriendlyName));
                        if (elementNode != null)
                        {
                            //当前库中存在与录制控件别名相同的控件
                            ElementInfo oldElement = DataFactory.XmlToObject<ElementInfo>(elementNode);
                            XmlAttribute VerAttr = windowNode.Attributes["Version"];
                            string verString = VerAttr.Value.Trim().Trim(',');
                            if (string.Equals(oldElement.AutomationId, ei.AutomationId))
                            {
                                //控件AutomationId与已经存在的控件id相同
                                winfo.id = (doc.DocumentElement.ChildNodes.Count + 1).ToString();
                                ei.id = elementNode.Attributes["id"].Value.Trim();
                                
                                if (!string.Equals(verString, winfo.Version))
                                {
                                    //现有控件版本中含有其他的软件版本
                                    //将该版本分离出一个新的版本
                                    verString = verString.Replace(verInfo.VerId, "");
                                    verString = verString.Replace(",,", ",");
                                    VerAttr.Value = verString.Trim(',');
                                    XmlDocument winfoDoc = new XmlDocument();
                                    winfoDoc.LoadXml(windowNode.OuterXml);
                                    XmlNode newwinNode = winfoDoc.SelectSingleNode("//WinInfo");
                                    XmlAttribute idAttr = newwinNode.Attributes["id"];
                                    idAttr.Value = winfo.id;
                                    XmlAttribute vAttr = newwinNode.Attributes["Version"];
                                    vAttr.Value = winfo.Version;
                                    XmlNodeList eNodeList = newwinNode.SelectNodes("//ElementInfo");
                                    foreach (XmlNode n in eNodeList)
                                    {
                                        XmlAttribute xaInherit = n.Attributes["Inherit"];
                                        if (xaInherit == null)
                                        {
                                            xaInherit = winfoDoc.CreateAttribute("Inherit");
                                            n.Attributes.Append(xaInherit);
                                        }
                                        xaInherit.Value = "true";
                                    }

                                    XmlNode eNode = newwinNode.SelectSingleNode(string.Format("ElementInfo[@id='{0}']", ei.id));

                                    XmlDocument tmpDoc = new XmlDocument();
                                    ei.Inherit = false;
                                    tmpDoc.LoadXml(DataFactory.ObjectToXml(ei));
                                    XmlNode neweNode = winfoDoc.ImportNode(tmpDoc.DocumentElement, true);
                                    newwinNode = newwinNode.ReplaceChild(neweNode, eNode);
                                    XmlNode xe = doc.ImportNode(winfoDoc.DocumentElement, true);
                                    doc.DocumentElement.AppendChild(xe);
                                }
                                else
                                {
                                    //现有控件的版本中不含其他软件版本
                                    //修改控件信息
                                    XmlDocument tmpDoc = new XmlDocument();
                                    tmpDoc.LoadXml(DataFactory.ObjectToXml(ei));
                                    XmlNode neweNode = doc.ImportNode(tmpDoc.DocumentElement, true);
                                    windowNode = windowNode.ReplaceChild(neweNode, elementNode);
                                }
                            }
                            else
                            {
                                //控件AutomationId与已经存在的控件id不同
                                XmlNode sameAidNode = windowNode.SelectSingleNode(string.Format("ElementInfo[AutomationId='{0}']", ei.AutomationId));
                                if (sameAidNode == null)
                                {
                                    if (!string.Equals(verString, winfo.Version))
                                    {
                                        //现有控件版本中含有其他的软件版本
                                        //将该版本分离出一个新的版本
                                        verString = verString.Replace(verInfo.VerId, "");
                                        verString = verString.Replace(",,", ",");
                                        VerAttr.Value = verString.Trim(',');
                                        winfo.id = (doc.DocumentElement.ChildNodes.Count + 1).ToString();
                                        ei.id = elementNode.Attributes["id"].Value.Trim();
                                        XmlDocument winfoDoc = new XmlDocument();
                                        winfoDoc.LoadXml(windowNode.OuterXml);
                                        XmlNode newwinNode = winfoDoc.SelectSingleNode("//WinInfo");
                                        XmlAttribute idAttr = newwinNode.Attributes["id"];
                                        idAttr.Value = winfo.id;
                                        XmlAttribute vAttr = newwinNode.Attributes["Version"];
                                        vAttr.Value = winfo.Version;

                                        XmlNodeList eNodeList = newwinNode.SelectNodes("//ElementInfo");
                                        foreach (XmlNode n in eNodeList)
                                        {
                                            XmlAttribute xaInherit = n.Attributes["Inherit"];
                                            if (xaInherit == null)
                                            {
                                                xaInherit = winfoDoc.CreateAttribute("Inherit");
                                                n.Attributes.Append(xaInherit);
                                            }
                                            xaInherit.Value = "true";
                                        }

                                        XmlNode eNode = newwinNode.SelectSingleNode(string.Format("ElementInfo[@id='{0}']", oldElement.id));

                                        XmlDocument tmpDoc = new XmlDocument();
                                        tmpDoc.LoadXml(DataFactory.ObjectToXml(ei));
                                        XmlNode neweNode = winfoDoc.ImportNode(tmpDoc.DocumentElement, true);
                                        newwinNode = newwinNode.ReplaceChild(neweNode, eNode);
                                        XmlNode xe = doc.ImportNode(winfoDoc.DocumentElement, true);
                                        doc.DocumentElement.AppendChild(xe);
                                    }
                                    else
                                    {
                                        //现有控件的版本中不含其他软件版本
                                        if (oldElement.Inherit)
                                        {
                                            //控件信息是继承下来的控件
                                            //不存在与当前录制控件相同AutomationId相同的控件
                                            //修改控件信息
                                            XmlDocument tmpDoc = new XmlDocument();
                                            tmpDoc.LoadXml(DataFactory.ObjectToXml(ei));
                                            XmlNode neweNode = doc.ImportNode(tmpDoc.DocumentElement, true);
                                            windowNode = windowNode.ReplaceChild(neweNode, elementNode);
                                        }
                                        else
                                        {
                                            //控件信息是录制的
                                            MessageBox.Show(this, "窗口中已经有该别名的控件，请重新输入控件别名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    //存在与当前录制控件相同AutomationId相同的控件
                                    //错误操作，给出提示
                                    MessageBox.Show(this, string.Format("窗口中已经有该别名的控件，请重新输入控件别名\n\n该控件的别名可能为[{0}]", sameAidNode.Attributes["FriendlyName"].Value), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            //当前库中不存在与录制控件别名相同的控件
                            //添加新控件
                            XmlNodeList countList = windowNode.SelectNodes("ElementInfo");
                            ei.id = (countList.Count + 1).ToString();
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
                        XmlNodeList xnl = doc.SelectNodes(string.Format("//WinInfo[@FriendlyName='{0}']", oldFriendlyName));
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
                XmlNodeList windowNodeList = doc.SelectNodes(string.Format("//WinInfo[@FriendlyName='{0}']", winfo.FriendlyName));
                if (windowNodeList != null && windowNodeList.Count > 0)
                {
                    //MessageBox.Show(this, "该窗口别名已经有窗口使用，请重新输入窗口别名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return false;
                    XmlNode windowNode = null;
                    foreach (XmlNode node in windowNodeList)
                    {
                        if (node.Attributes["Version"].Value.Contains(winfo.Version))
                        {
                            windowNode = node;
                            break;
                        }
                    }

                    if (windowNode == null)
                    {
                        MessageBox.Show(this, "基本不会出现", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        if (string.Equals(windowNode.Attributes["Version"].Value, winfo.Version))
                        {
                            MessageBox.Show(this, "该窗口别名已经有窗口使用，请重新输入窗口别名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        else
                        {
                            XmlAttribute VerAttr = windowNode.Attributes["Version"];
                            string verString = VerAttr.Value.Trim().Trim(',');
                            verString = verString.Replace(verInfo.VerId, "");
                            verString = verString.Replace(",,", ",");
                            VerAttr.Value = verString.Trim(',');
                            winfo.id = (doc.DocumentElement.ChildNodes.Count + 1).ToString();
                            if (infoList.Count > 1)
                            {
                                ElementInfo ei = winfo.GetElementInfo()[0];
                                ei.FriendlyName = elementName;
                                ei.id = "1";
                            }
                            string windowInfoString = winfo.ToXml();
                            XmlDocument winfoDoc = new XmlDocument();
                            winfoDoc.LoadXml(windowInfoString);
                            XmlNode xe = doc.ImportNode(winfoDoc.DocumentElement, true);
                            doc.DocumentElement.AppendChild(xe);
                        }
                    }
                }
                else
                {
                    XmlDocument verDoc = new XmlDocument();
                    verDoc.LoadXml(DataFactory.ReadLibrary(Path, "Version"));
                    XmlNodeList verNodeList = verDoc.SelectNodes(string.Format("//{0}/VerInfo/VerId", ProcessName));
                    string ver = string.Empty;
                    foreach (XmlNode verNode in verNodeList)
                    {
                        if (!string.IsNullOrEmpty(verNode.InnerText))
                        {
                            ver = ver + "," + verNode.InnerText;
                        }
                    }
                    ver = ver.Trim(',');
                    winfo.id = (doc.DocumentElement.ChildNodes.Count + 1).ToString();
                    winfo.Version = ver;
                    if (infoList.Count > 1)
                    {
                        ElementInfo ei = winfo.GetElementInfo()[0];
                        ei.FriendlyName = elementName;
                        ei.id = "1";
                    }
                    string windowInfoString = winfo.ToXml();
                    XmlDocument winfoDoc = new XmlDocument();
                    winfoDoc.LoadXml(windowInfoString);
                    XmlNode xe = doc.ImportNode(winfoDoc.DocumentElement, true);
                    doc.DocumentElement.AppendChild(xe);
                }
            }
            doc.Save(string.Format("{0}.xml", ProcessName));
            DataFactory.WriteLibrary(Path, ProcessName, doc.OuterXml);
            return true;
        }
        #endregion

        #region 窗口信息
        private WinInfo GetInfo(List<AutomationElement> infoList)
        {
            if (infoList == null || infoList.Count == 0)
            {
                throw new Exception("GetInfo: infoList is null or empty.");
            }
            AutomationElement windowElement = infoList[0];
            WinInfo winfo = new WinInfo();
            winfo.Version = Robot.GetSoftwareVersion().VerId;
            winfo.AutomationId = windowElement.Current.AutomationId;
            winfo.Name = windowElement.Current.Name;
            winfo.Type = windowElement.Current.ControlType.ProgrammaticName.Split('.')[1];
            if (infoList.Count > 1)
            {
                AutomationElement elementElement = infoList[infoList.Count - 1];
                ElementInfo einfo = new ElementInfo();
                string eAid = elementElement.Current.AutomationId;
                long n = -1;
                if (!string.IsNullOrEmpty(eAid) && !long.TryParse(eAid, out n))
                {
                    einfo.AutomationId = eAid;
                }
                else
                {
                    AutomationElement parent = infoList[infoList.Count - 2];
                    AutomationElement child = TreeWalker.ControlViewWalker.GetFirstChild(parent);
                    List<AutomationElement> childList = new List<AutomationElement>();
                    childList.Clear();
                    while (child != null)
                    {
                        childList.Insert(0, child);
                        child = TreeWalker.ControlViewWalker.GetNextSibling(child);
                    }
                    einfo.RelativePosition = (childList.IndexOf(elementElement) + 1).ToString();
                    childList.Clear();
                }
                einfo.Name = elementElement.Current.Name;
                einfo.Type = elementElement.Current.ControlType.ProgrammaticName.Split('.')[1];
                einfo.RelativePath = string.Empty;

                for (int i = 1; i < infoList.Count - 1; i++)
                {
                    AutomationElement tmp = infoList[i];

                    if (tmp.Current.ControlType == ControlType.TabItem)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(tmp.Current.AutomationId) && !long.TryParse(tmp.Current.AutomationId, out n))
                    {
                        einfo.RelativePath = einfo.RelativePath + "/" + tmp.Current.AutomationId;
                    }
                    else
                    {
                        AutomationElement parent = infoList[i - 1];
                        AutomationElement child = TreeWalker.ControlViewWalker.GetFirstChild(parent);
                        List<AutomationElement> childList = new List<AutomationElement>();
                        childList.Clear();
                        while (child != null)
                        {
                            childList.Insert(0, child);
                            child = TreeWalker.ControlViewWalker.GetNextSibling(child);
                        }

                        einfo.RelativePath = einfo.RelativePath + "/id:" + (childList.IndexOf(tmp) + 1).ToString();
                        childList.Clear();
                    }
                }
                einfo.RelativePath = einfo.RelativePath.Trim('/');
                einfo.Inherit = false;
                winfo.AddElementInfo(einfo);
            }
            return winfo;
        }
        #endregion

        #region 选择控件事件
        private void element_List_SelectedIndexChanged(object sender, EventArgs e)
        {

            int index = element_List.SelectedIndex;
            if (index >= 0)
            {
                AutomationElement tmpElement = ElementList[index];
                try
                {
                    SelecterMainForm.DrawEventFun(tmpElement.Current.BoundingRectangle);
                }
                catch
                {
                    MessageBox.Show(this, "控件已经关闭", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    SelecterMainForm.CleanEventFun();
                    return;
                }
                Name_textBox.Text = tmpElement.Current.Name;
                Type_textBox.Text = tmpElement.Current.ControlType.ProgrammaticName.Split('.')[1];
                Aid_textBox.Text = tmpElement.Current.AutomationId;
                bRect_textBox.Text = tmpElement.Current.BoundingRectangle.ToString();
                Handle_textBox.Text = tmpElement.Current.NativeWindowHandle.ToString();

                AutomationPattern[] patterns = tmpElement.GetSupportedPatterns();
                p1_textBox.Text = "";
                p2_textBox.Text = "";
                p3_textBox.Text = "";
                p4_textBox.Text = "";

                if (patterns.Length > 0)
                {
                    string name = patterns[0].ProgrammaticName;
                    p1_textBox.Text = name.Substring(0, name.Length - 19);
                }
                else
                {
                    return;
                }
                if (patterns.Length > 1)
                {
                    string name = patterns[1].ProgrammaticName;
                    p2_textBox.Text = name.Substring(0, name.Length - 19);
                }
                else
                {
                    return;
                }
                if (patterns.Length > 2)
                {
                    string name = patterns[2].ProgrammaticName;
                    p3_textBox.Text = name.Substring(0, name.Length - 19);
                }
                else
                {
                    return;
                }
                if (patterns.Length > 3)
                {
                    string name = patterns[3].ProgrammaticName;
                    p4_textBox.Text = name.Substring(0, name.Length - 19);
                }
                else
                {
                    return;
                }
            }
        }
        #endregion

        #region 键盘按键消息处理事件
        private void element_List_KeyUp(object sender, KeyEventArgs e)
        {
            int index = element_List.SelectedIndex;
            if (e.KeyCode == Keys.Delete && index >= 0)
            {
                DialogResult dr = MessageBox.Show(this, "确定要删除吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    if (index == 0)
                    {
                        MessageBox.Show(this, "至少要保留一个控件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (index == _elementList.Count - 1 && findControl)
                    {
                        ControlInfo ci = ControlInfo.getInstance(Path);
                        XmlDocument doc = ci[ProcessName];
                        
                        //doc.LoadXml(DataFactory.ReadLibrary(Path, "WinObject"));
                        XmlNode windowNode = doc.SelectSingleNode(string.Format("//WinInfo[@id='{0}']", wInfo.id));
                        XmlNode elementNode = windowNode.SelectSingleNode(string.Format("ElementInfo[@id='{0}']", wInfo.ElementInfo[0].id));
                        windowNode.RemoveChild(elementNode);
                        DataFactory.WriteLibrary(Path, ProcessName, doc.OuterXml);
                    }
                    //MessageBox.Show(string.Format("{0}/{1}", index, element_List.Items.Count));
                    int count = element_List.Items.Count;
                    for (int i = index; i < count; i++)
                    {
                        element_List.Items.RemoveAt(index);
                        _elementList.RemoveAt(index);
                    }
                    element_List.SelectedIndex = index - 1;
                    RefreshForm();
                }
            }
        }
        #endregion

        #region 添加版本
        private void AddVersion_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                AddVersion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddVersion()
        {
            DateTime date = Robot.GetFileTime();
            string VersionXmlString = DataFactory.ReadLibrary(Path, "Version");

            XmlDocument doc = new XmlDocument();
            if (string.IsNullOrEmpty(VersionXmlString))
            {
                doc.LoadXml(InitializationString.Version);
            }
            else
            {
                doc.LoadXml(VersionXmlString);
            }

            XmlNode pNode = doc.SelectSingleNode(string.Format("//{0}", ProcessName));

            if (pNode == null)
            {
                XmlElement pElement = doc.CreateElement(ProcessName);
                Version_textBox.Text = "1";
                VerInfo newVerInfo = new VerInfo();
                newVerInfo.VerId = "1";
                newVerInfo.StartDate = "19000101";
                newVerInfo.EndDate = "99991231";
                XmlDocument newVerDocument = new XmlDocument();
                newVerDocument.LoadXml(newVerInfo.ToXml());
                XmlNode newVerNode = doc.ImportNode(newVerDocument.DocumentElement, true);
                pElement.AppendChild(newVerNode);
                doc.DocumentElement.AppendChild(pElement);
                DataFactory.WriteLibrary(Path, "Version", doc.OuterXml);
            }
            else
            {
                XmlNode checkNode = pNode.SelectSingleNode(string.Format("VerInfo[StartDate = {0} or EndDate = {0}]", date.ToString("yyyyMMdd")));
                if (checkNode == null)
                {
                    XmlNodeList verList = pNode.SelectNodes(string.Format("VerInfo", ProcessName));
                    Version_textBox.Text = (verList.Count + 1).ToString();
                    VerInfo newVerInfo = new VerInfo();
                    newVerInfo.VerId = (verList.Count + 1).ToString();
                    string nextVer = null;
                    if (verList.Count > 0)
                    {
                        //有历史版本
                        //根据日期拆分历史版本
                        string tmpList = string.Empty;
                        XmlNode modifyNode = pNode.SelectSingleNode(string.Format("VerInfo[StartDate < {0} and EndDate > {0}]", date.ToString("yyyyMMdd")));

                        VerInfo oldVerInfo = DataFactory.XmlToObject<VerInfo>(modifyNode);
                        if (string.Equals(oldVerInfo.EndDate, "99991231"))
                        {
                            //要拆分的版本为最新的版本
                            //将当前软件日期设为版本起始日期
                            newVerInfo.StartDate = date.ToString("yyyyMMdd");
                            newVerInfo.EndDate = oldVerInfo.EndDate;
                            oldVerInfo.EndDate = date.AddDays(-1).ToString("yyyyMMdd");
                        }
                        else
                        {
                            //要拆分的版本为历史发布版本
                            //将当前软件日期设为版本终止日期
                            newVerInfo.StartDate = oldVerInfo.StartDate;
                            newVerInfo.EndDate = date.ToString("yyyyMMdd");
                            oldVerInfo.StartDate = date.AddDays(1).ToString("yyyyMMdd");
                        }
                        nextVer = oldVerInfo.VerId.ToString();
                        XmlDocument oldVerDocument = new XmlDocument();
                        oldVerDocument.LoadXml(oldVerInfo.ToXml());
                        XmlNode oldNode = doc.ImportNode(oldVerDocument.DocumentElement, true);
                        pNode.ReplaceChild(oldNode, modifyNode);
                    }
                    else
                    {
                        //无历史版本
                        //添加第一个版本
                        newVerInfo.StartDate = "19000101";
                        newVerInfo.EndDate = "99991231";
                        nextVer = "1";
                    }

                    XmlDocument newVerDocument = new XmlDocument();
                    newVerDocument.LoadXml(newVerInfo.ToXml());
                    XmlNode newVerNode = doc.ImportNode(newVerDocument.DocumentElement, true);
                    pNode.AppendChild(newVerNode);
                    DataFactory.WriteLibrary(Path, "Version", doc.OuterXml);
                    ChangeWinObject(nextVer, newVerInfo.VerId);
                    ChangeControlName(nextVer, newVerInfo.VerId);
                }
                else
                {
                    checkNode = checkNode.SelectSingleNode("VerId");
                    Version_textBox.Text = checkNode.InnerText.Trim();
                }
            }
        }
        #endregion

        #region ChangeWinObject
        private void ChangeWinObject(string nextVer, string verId)
        {
            string WinObjectString = DataFactory.ReadLibrary(Path, ProcessName);
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
                DataFactory.WriteLibrary(Path, ProcessName, WinObjectDocument.OuterXml);
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
                this.element_List.Items.Clear();
                foreach (AutomationElement tmp in _elementList)
                {
                    if (tmp == AutomationElement.RootElement)
                    {
                        element_List.Items.Add("Desktop");
                    }
                    else
                    {
                        element_List.Items.Add(tmp.Current.ControlType.ProgrammaticName.Split('.')[1]);
                    }
                }
                element_List.SelectedIndex = -1;
                element_List.SelectedIndex = _elementList.Count - 1;
                try
                {
                    addVersion = false;
                    Version_textBox.Text = Robot.GetSoftwareVersion().VerId;
                }
                catch
                {
                    //AddVersion();
                    addVersion = true;
                    Version_textBox.Text = "1";
                }
                FindWindowFromLibrary();
                if (_elementList.Count == 1)
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

                        List<ElementInfo> eList = wInfo.GetElementInfo();
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
                XmlNode desktop = doc.SelectSingleNode("Desktop");
                int index = 1;
                foreach (XmlNode xn in desktop.ChildNodes)
                {
                    xn.Attributes["id"].Value = index++.ToString();
                }
                DataFactory.WriteLibrary(Path, ProcessName, doc.OuterXml);
            }
        }

        //private void ignoreName_chkBox_CheckedChanged(object sender, EventArgs e)
        //{
        //    ignoreName = ignoreName_chkBox.Checked;
        //    Name_textBox.Visible = !ignoreName_chkBox.Checked;
        //    RefreshForm();
        //}
    }
}
