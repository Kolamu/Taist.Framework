namespace TaistControlSelecter
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Automation;
    using System.Collections.Generic;
    using System.Threading;
    using System.Diagnostics;
    using System.Xml;
    using System.Text.RegularExpressions;
    using System.Net;
    using System.IO;
    
    using mshtml;
    using SHDocVw;

    using Mock.Tools.Controls;
    using Mock.Nature.Native;
    using Timer = System.Threading.Timer;

    public partial class SelecterMainForm : Form
    {
        private Selecter selecter = null;
        private WindowInfomationForm wInfoForm = null;
        private List<string> notifyMsg = null;
        private CursorPositionForm cursorForm = null;
        private WebWindowInfomationForm webInfoForm = null;
        private static event EventHandler HiddenEvent = null;
        private static event EventHandler ShowEvent = null;
        public SelecterMainForm()
        {
            InitializeComponent();
            notifyMsg = new List<string>();
            notifyMsg.Add("我可以帮你什么？");
            notifyMsg.Add("需要我的帮助吗？");
            notifyMsg.Add("按住Ctrl键+左键单击可以选择控件哦~");
            notifyMsg.Add("右键点我可以退出哦~");
            notifyMsg.Add("按住Ctrl键+右键单击也可以退出哦~");
            notifyMsg.Add("你又来看我啦~");
            ThreadPool.RegisterWaitForSingleObject(Program.ProgramStarted, OnProgramStarted, null, -1, false);
            try
            {
                selecter = new Selecter();
                //selecter.SelectEvent += OnSelect;
                selecter.VisualEvent += OnVisual;
                selecter.HiddenEvent += OnHidden;
                selecter.MouseMoveEvent += MouseMoveEventFun;
                
                FindMsElementCompleteEvent += OnFindMsElementComplete;
                FindIeElementCompleteEvent += OnFindIeElementComplete;
                DrawEvent += OnDraw;
                CleanEvent += OnClean;
                HiddenEvent += SelecterMainForm_HiddenEvent;
                ShowEvent += SelecterMainForm_ShowEvent;
                MessageForm mf = new MessageForm();
                mf.Message = "正在录制";
                mf.Show();
                wInfoForm = new WindowInfomationForm();
                webInfoForm = new WebWindowInfomationForm();
                cursorForm = new CursorPositionForm();
                POINT p = Mouse.Position;
                cursorForm.Left = p.x;
                cursorForm.Top = p.y;
                cursorForm.Show();
                selecter.Start();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            this.notifyIcon.ShowBalloonTip(2000, "用法", "Ctrl + 左键：选择控件\nCtrl + 右键：退出", ToolTipIcon.Info);
        }

        private void SelecterMainForm_ShowEvent()
        {
            if (this.InvokeRequired)
            {
                EventHandler eh = new EventHandler(SelecterMainForm_ShowEvent);
                this.Invoke(eh);
            }
            else
            {
                this.Visible = true;
            }
        }

        private void SelecterMainForm_HiddenEvent()
        {
            if (this.InvokeRequired)
            {
                EventHandler eh = new EventHandler(SelecterMainForm_HiddenEvent);
                this.Invoke(eh);
            }
            else
            {
                this.Visible = false;
            }
        }

        private void SelecterMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            selecter.Close();
        }
        private delegate void FindElementCompleteEventHandler(object args);
        private event FindElementCompleteEventHandler FindMsElementCompleteEvent = null;
        private event FindElementCompleteEventHandler FindIeElementCompleteEvent = null;

        private delegate void DrawEventHandler(System.Windows.Rect r);
        private static event DrawEventHandler DrawEvent = null;

        private delegate void CleanEventHandler();
        private static event CleanEventHandler CleanEvent = null;
        private Timer timer = null;

        internal static void DrawEventFun(System.Windows.Rect r)
        {
            DrawEvent(r);
        }

        internal static void DrawEventFun()
        {
            //DrawEvent(rect);
            System.Windows.Rect r = new System.Windows.Rect(rect.Left, rect.Top, rect.Width, rect.Height);
            DrawEvent(r);
        }

        internal static void CleanEventFun()
        {
            CleanEvent();
        }

        private void OnVisual()
        {
            if (this.InvokeRequired)
            {
                EventHandler handler = new EventHandler(OnVisual);
                this.Invoke(handler);
            }
            else
            {
                POINT p = Mouse.Position;
                if (timer != null)
                {
                    timer.Dispose();
                }
                timer = new Timer(Find, null, 500, 100000);
                //this.Visible = true;
                //this.Left = p.x - 10;
                //this.Width = 20;
                //this.Height = 20;
                //this.Top = p.y - 10;
                //this.Visible = true;
            }
        }

        private void Find(object state)
        {
            SelectControl();
            timer.Dispose();
            timer = null;
        }

        private void OnHidden()
        {
            if (this.InvokeRequired)
            {
                EventHandler handler = new EventHandler(OnHidden);
                this.Invoke(handler);
            }
            else
            {
                //POINT p = Mouse.Position;
                //this.Left = p.x - 10;
                //this.Top = p.y - 10;
                //this.Visible = false;
                if (timer != null)
                {
                    timer.Dispose();
                }
            }
        }

        private void SelectControl()
        {
            try
            {
                //OnHidden();
                HiddenEvent();
                Thread findThread = new Thread(new ParameterizedThreadStart(FindElement));
                findThread.IsBackground = true;
                findThread.SetApartmentState(ApartmentState.STA);
                findThread.Start(Mouse.Position);

                //DrawRect();
            }
            catch (Exception ex)
            {
                Note(ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "错误");
            }
        }

        private void FindElement(object pt)
        {
            try
            {
                POINT pot = (POINT)pt;
                
                AutomationElement element = AutomationElement.FromPoint(new System.Windows.Point(pot.x, pot.y));
                string processName = Process.GetProcessById(element.Current.ProcessId).ProcessName.ToLower();
                if (processName.Contains("iexplore") || processName.Contains("java"))
                {
                    FindIeElement(element, pot);
                }
                else
                {
                    FindMsElement(element);
                }
            }
            catch (Exception ex)
            {
                Note(ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "错误");
            }
        }

        /// <summary>
        /// 提取网页窗口的信息
        /// </summary>
        /// <param name="element"></param>
        private void FindIeElement(AutomationElement element, POINT pt)
        {
            //获取网页窗口
            List<AutomationElement> infoList = new List<AutomationElement>();
            infoList.Add(element);
            AutomationElement parent = TreeWalker.ControlViewWalker.GetParent(element);
            int depth = 0;
            string name = null;
            while (parent.Current.ControlType != ControlType.Window)
            {
                string className = parent.Current.ClassName;
                if (string.Equals(className, "Internet Explorer_Server"))
                {
                    infoList.Insert(0, parent);
                    depth = (int)parent.Current.BoundingRectangle.Y;
                    name = parent.Current.Name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        break;
                    }
                }

                if (string.Equals(className, "TabWindowClass"))
                {
                    name = parent.Current.Name;
                }
                infoList.Insert(0, parent);
                parent = TreeWalker.ControlViewWalker.GetParent(parent);
            }
            infoList.Insert(0, parent);

            System.Windows.Rect r = new System.Windows.Rect();
            XmlDocument args = new XmlDocument();
            XmlElement root = args.CreateElement("Root");
            args.AppendChild(root);
            XmlElement tmp = args.CreateElement("WindowName");
            root.AppendChild(tmp);
            if (name == null)
            {
                //IE弹出窗口

                tmp.InnerText = parent.Current.Name;
                string path = "";
                long n = 0;
                for (int i = 1; i < infoList.Count; i++)
                {
                    AutomationElement tmpe = infoList[i];

                    if (!string.IsNullOrEmpty(tmpe.Current.AutomationId) && !long.TryParse(tmpe.Current.AutomationId, out n))
                    {
                        path = path + "/" + tmpe.Current.AutomationId;
                    }
                    else
                    {
                        parent = infoList[i - 1];
                        AutomationElement child = TreeWalker.ControlViewWalker.GetFirstChild(parent);
                        List<AutomationElement> childList = new List<AutomationElement>();
                        childList.Clear();
                        while (child != null)
                        {
                            childList.Insert(0, child);
                            child = TreeWalker.ControlViewWalker.GetNextSibling(child);
                        }

                        path = path + "/id:" + (childList.IndexOf(tmpe) + 1).ToString();
                        childList.Clear();
                    }
                }
                path = path.Trim('/');
                tmp = args.CreateElement("TagName");
                root.AppendChild(tmp);
                tmp.InnerText = "popwindow";
                XmlElement propNode = args.CreateElement("Prop");
                root.AppendChild(propNode);
                tmp = args.CreateElement("path");
                propNode.AppendChild(tmp);
                tmp.InnerText = path;
                tmp = args.CreateElement("body");
                root.AppendChild(tmp);
            }
            else
            {
                //获取网页的Document对象
                if (name.IndexOf(" - Internet Explorer") > 0)
                {
                    name = name.Substring(0, name.Length - 20);
                }
                tmp.InnerText = name;
                InternetExplorer ie = GetIEWindowFromProcessId(name);
                IHTMLDocument2 doc = (IHTMLDocument2)ie.Document;

                IHTMLElement obj = (IHTMLElement)doc.elementFromPoint((int)pt.x, (int)pt.y - depth);
                if (obj is IHTMLIFrameElement)
                {
                    //如果是IHTMLFrameBase2对象的实例，获取Frame内部文档的节点
                    HTMLIFrameClass iFrame = (HTMLIFrameClass)obj;
                    string s = iFrame.componentFromPoint(100,100);
                    Note(s);
                    IHTMLDocument2 frameDoc = (IHTMLDocument2)iFrame.contentWindow.document;
                    obj = frameDoc.elementFromPoint((int)pt.x, (int)pt.y - depth);
                    object o = doc.parentWindow.window.execScript("function(){return 'a';}");
                    Note(o.ToString());
                }
                
                if (obj == null)
                {
                    return;
                }
                
                IHTMLElement2 el2 = (IHTMLElement2)obj;
                string tagName = obj.tagName;

                IHTMLRect rc = el2.getBoundingClientRect();
                r.X = rc.left;
                r.Y = rc.top;
                r.Width = rc.right - rc.left;
                r.Height = rc.bottom - rc.top;

                //string[] coString = doc.cookie.Split('=');
                //Cookie cookie = new Cookie(coString[0].Trim(), coString[1].Trim(), "/", doc.domain);

                //HttpWebResponse response = Mock.Tools.Web.HttpAccess.Get(ie.LocationURL, cookie);
                //System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                System.Text.StringBuilder docString = new System.Text.StringBuilder();

                //while (!sr.EndOfStream)
                //{
                //    string tmpStr = sr.ReadLine();
                //    if (string.IsNullOrEmpty(tmpStr)) continue;
                //    docString.AppendLine(tmpStr);
                //}
                //sr.Close();

                docString.Append(doc.body.outerHTML);
                string infoString = obj.outerHTML;
                tmp = args.CreateElement("TagName");
                root.AppendChild(tmp);
                tmp.InnerText = tagName;
                Match ma = Regex.Match(infoString, "<(.*?)(\\s?.*?)>");
                string attrString = string.Empty;
                if (ma.Success)
                {
                    //tmp.InnerText = ma.Groups[1].Value;
                    attrString = " " + ma.Groups[2].Value + " ";
                }
                else
                {
                    throw new Exception("Error");
                }

                XmlElement propNode = args.CreateElement("Prop");
                root.AppendChild(propNode);
                if (!string.IsNullOrEmpty(obj.innerText))
                {
                    tmp = args.CreateElement("innertext");
                    propNode.AppendChild(tmp);
                    tmp.InnerText = obj.innerText;
                }
                List<string> tmpObjStringList = new List<string>();

                MatchCollection mc = Regex.Matches(attrString, "\\s*?(.*?)=(.*?(?'sep'['\"]?).*?\\k'sep'.*?)[\\s>]");
                attrString = string.Empty;
                foreach (Match m in mc)
                {
                    string key = m.Groups[1].Value.Trim();
                    string value = m.Groups[2].Value.Trim('\"').Trim('\'');
                    if (key.Contains(" "))
                    {
                        key = key.Substring(key.LastIndexOf(' ') + 1);
                    }
                    tmp = args.CreateElement(key);
                    propNode.AppendChild(tmp);
                    tmp.InnerText = value;
                }

                #region 不明所以
                // 2017.03.15，优化了正则表达式\\s*?(.*?)=(.*?(?'sep'['\"]?).*?\\k'sep'.*?)[\\s>]，以下代码可能是因为正则表达式
                // 无法搜索出所有的属性写的，需要进一步观察，目前测试功能正常
                //    string pattern = string.Format("<{0}\\s*?[^>]*?\\s*?{1}\\s*?=\\s*?\\\\?['\"]?{2}\\\\?['\"]?.*?>", obj.tagName, key, value);
                //    MatchCollection mpCollection = Regex.Matches(docString.ToString(), pattern, RegexOptions.IgnoreCase);

                //    if (tmpObjStringList.Count == 0)
                //    {
                //        //第一次搜索，填充值
                //        foreach (Match mp in mpCollection)
                //        {
                //            tmpObjStringList.Add(mp.Value);
                //        }
                //    }
                //    else
                //    {
                //        //非第一次搜索，删除没有的值
                //        if (mpCollection.Count > 0)
                //        {
                //            List<string> tmpList = new List<string>();
                //            foreach (Match mp in mpCollection)
                //            {
                //                if (tmpObjStringList.Contains(mp.Value))
                //                {
                //                    tmpList.Add(mp.Value);
                //                }
                //            }
                //            tmpObjStringList.Clear();
                //            tmpObjStringList.AddRange(tmpList);
                //            if (tmpObjStringList.Count == 1) break;
                //        }
                //    }
                //}

                //if (tmpObjStringList.Count == 1)
                //{
                //    attrString = " " + tmpObjStringList[0].Trim('>').Trim('<') + " ";
                //}
                //else
                //{
                //    throw new Exception("获取属性失败");
                //}

                //mc = Regex.Matches(attrString, "\\s*?(.*?)=(.*?(?'sep'['\"]?).*?\\k'sep'.*?)\\s+?");
                //foreach (Match m in mc)
                //{
                //    string key = m.Groups[1].Value.Trim();
                //    string value = m.Groups[2].Value.Trim('\"').Trim('\'');
                //    if (key.Contains(" "))
                //    {
                //        key = key.Substring(key.LastIndexOf(' ') + 1);
                //    }
                //    tmp = args.CreateElement(key);
                //    propNode.AppendChild(tmp);
                //    tmp.InnerText = value;
                //}
                #endregion

                tmp = args.CreateElement("body");
                root.AppendChild(tmp);

                //Match bodyMatch = Regex.Match(docString.ToString(), "<BODY.*?>(.*)</BODY>", RegexOptions.IgnoreCase);
                //if (bodyMatch.Success)
                //{
                //    tmp.InnerText = bodyMatch.Groups[1].Value;
                //}
                tmp.InnerText = docString.ToString();
                
            }
            DrawEventFun(r);
            FindIeElementCompleteEvent(args);
        }

        private void OnFindIeElementComplete(object args)
        {
            XmlDocument doc = (XmlDocument)args;
            if (this.InvokeRequired)
            {
                FindElementCompleteEventHandler findHandler = new FindElementCompleteEventHandler(OnFindIeElementComplete);
                this.Invoke(findHandler, new object[] { doc });
            }
            else
            {
                if (wInfoForm.Visible) wInfoForm.Visible = false;
                this.Show();
                this.Invalidate();
                webInfoForm.ElementList = doc;
                webInfoForm.Show();
            }
        }

        /// <summary>
        /// 根据窗口名获取窗口IHTMLDocument2对象
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private InternetExplorer GetIEWindowFromProcessId(string name)
        {
            InternetExplorer ieWnd = null;
            ShellWindowsClass shellWindows = new SHDocVw.ShellWindowsClass();
            IHTMLDocument2 doc = null;
            foreach (SHDocVw.InternetExplorer ie in shellWindows)
            {
                if (ie.FullName.ToLower().IndexOf("iexplore.exe") >= 0)
                {
                    doc = (IHTMLDocument2)ie.Document;
                    if (string.Equals(doc.title, name) && !ie.Busy)
                    {
                        ieWnd = ie;
                        break;
                    }
                }
            }
            shellWindows = null;
            doc = null;
            return ieWnd;
        }

        /// <summary>
        /// 提取桌面窗口的信息
        /// </summary>
        /// <param name="element"></param>
        private void FindMsElement(AutomationElement element)
        {
            List<AutomationElement> pathList = new List<AutomationElement>();
            pathList.Add(element);
            if (element.Current.ControlType != ControlType.Window && element != AutomationElement.RootElement)
            {
                AutomationElement parent = TreeWalker.ControlViewWalker.GetParent(element);
                while (parent != AutomationElement.RootElement && parent.Current.ControlType != ControlType.Window)
                {
                    pathList.Insert(0, parent);
                    parent = TreeWalker.ControlViewWalker.GetParent(parent);
                }
                pathList.Insert(0, parent);
            }
            DrawEvent(element.Current.BoundingRectangle);
            FindMsElementCompleteEvent(pathList);
        }

        private void OnFindMsElementComplete(object args)
        {
            List<AutomationElement> elementList = (List<AutomationElement>)args;
            if (this.InvokeRequired)
            {
                FindElementCompleteEventHandler findHandler = new FindElementCompleteEventHandler(OnFindMsElementComplete);
                this.Invoke(findHandler, new object[] { elementList });
            }
            else
            {
                if (webInfoForm.Visible) webInfoForm.Visible = false;
                this.Show();
                wInfoForm.ElementList = elementList;
                wInfoForm.Show();
            }
        }

        private void OnDraw(System.Windows.Rect r)
        {
            if (this.InvokeRequired)
            {
                DrawEventHandler drawEvent = new DrawEventHandler(OnDraw);
                this.Invoke(drawEvent, new object[] { r });
            }
            else
            {
                Rectangle rt = new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
                rect = this.RectangleToClient(rt);
                DrawRect();
            }
        }

        private void OnClean()
        {
            if (this.InvokeRequired)
            {
                CleanEventHandler cleanEvent = new CleanEventHandler(OnClean);
                this.Invoke(cleanEvent, null);
            }
            else
            {
                Mock.LogManager.Debug("CleanAll");
                CleanAll();
            }
        }

        private static Rectangle rect = new Rectangle();
        private void DrawRect()
        {
            Graphics g = this.CreateGraphics();
            g.Clear(this.BackColor);
            Pen p = new Pen(Color.Red, 4);
            Mock.LogManager.DebugFormat("Draw [{0}, {1}, {2}, {3}]", rect.Left, rect.Top, rect.Width, rect.Height);
            g.DrawRectangle(p, (int)rect.Left + 5,
                               (int)rect.Top + 5,
                               (int)rect.Width,
                               (int)rect.Height);
            //g.Dispose();
        }

        private void CleanAll()
        {
            Graphics g = this.CreateGraphics();
            g.Clear(this.BackColor);
            g.Dispose();
        }

        #region log
        private void Note(string message, string path = "Selecter.log")
        {
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string strT = currentTime.ToString("u");
            string msg = string.Format("[{0,20}] {1}", strT, message);
            if (System.IO.File.Exists(path))
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(path);
                //sw.WriteLine("[" + strT + "] " + message);
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            else
            {
                System.IO.File.Delete(path);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                //sw.WriteLine("[" + strT + "] " + message);
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }
        #endregion

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Random r = new Random();
                int index = r.Next(6);
                this.notifyIcon.ShowBalloonTip(2000, "", notifyMsg[index], ToolTipIcon.Info);
            }
        }

        // 当收到第二个进程的通知时，显示窗体   
        void OnProgramStarted(object state, bool timeout)
        {
            this.notifyIcon.ShowBalloonTip(2000, "Honey", "我在这儿呢！", ToolTipIcon.Info);
        }

        private void SelecterMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon.Dispose();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 用法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Ctrl + 左键：选择控件\nCtrl + 右键：退出", "用法", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void 网页控件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebControlSelecterMainForm wbForm = new WebControlSelecterMainForm();
            wbForm.Show();
        }

        private void MouseMoveEventFun()
        {
            //cur_point_lb.Text = string.Format("{0}, {1}", x, y);
            //cur_point_lb.Left = x + 8;
            //cur_point_lb.Top = y + 20;
            if (this.InvokeRequired)
            {
                EventHandler handler = new EventHandler(MouseMoveEventFun);
                this.Invoke(handler);
            }
            else
            {
                try
                {
                    POINT p = Mouse.Position;
                    cursorForm.Left = p.x + 4;
                    cursorForm.Top = p.y + 16;
                }
                catch { }
            }
        }

        private void 导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportForm iForm = new ImportForm();
            iForm.ShowDialog(this);
        }

        private void 导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportForm eForm = new ExportForm();
            eForm.ShowDialog(this);
        }

        private void SelecterMainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SelectControl();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.Close();
            }
        }

        private void SelecterMainForm_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //Pen p = new Pen(Color.Red, 5.0f);
            //g.DrawRectangle(p, 0, 0, this.Width, this.Height);
            //g.Dispose();
        }
    }
}
