namespace Mock.Data
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Xml;
    using System.Threading;
    using Mock.Data.Exception;

    /// <summary>
    /// 表示测试用例集对象
    /// </summary>
    public class TestCasePool
    {
        #region Storage
        private static List<TestCase> _testCaseList = null;
        //public static readonly string ID = Guid.NewGuid().ToString("N").ToUpper();
        //private static List<string> caseHeadList = null;
        //private static List<string> infoHeadList = null;
        private static Dictionary<string, XmlNode> dataInfo = null;
        private static Dictionary<string, XmlNode> caseInfo = null;
        private static Dictionary<string, Dictionary<string, XmlNode>> checkInfo = null;
        private static Dictionary<string, string> checkBhInfo = null;

        private static universal normalTest = true;

        #endregion

        static TestCasePool()
        {
            InitData();
            InitCase();
            InitCheck();

            _testCaseList = new List<TestCase>();
        }

        ~TestCasePool()
        {
            Environment.Exit(0);
        }

        #region Properties
        public static List<string> CaseList
        {
            get
            {
                List<string> bhList = new List<string>();
                if (caseInfo != null)
                {
                    foreach(string bh in caseInfo.Keys)
                    {
                        bhList.Add(bh);
                    }
                }
                return bhList;
            }
        }

        public static List<string> DataList
        {
            get
            {
                List<string> bhList = new List<string>();
                if (dataInfo != null)
                {
                    foreach (string bh in dataInfo.Keys)
                    {
                        bhList.Add(bh);
                    }
                }
                return bhList;
            }
        }

        public static List<string> CheckList
        {
            get
            {
                List<string> bhList = new List<string>();
                if (checkInfo != null)
                {
                    if (checkInfo.ContainsKey(Config.TargetProjectName.ToLower()))
                    {
                        Dictionary<string, XmlNode> ckInfo = checkInfo[Config.TargetProjectName.ToLower()];
                        foreach (string bh in ckInfo.Keys)
                        {
                            bhList.Add(bh);
                        }
                    }
                }
                return bhList;
            }
        }
        #endregion

        /// <summary>
        /// 获取测试数据节点
        /// </summary>
        /// <param name="bh"></param>
        /// <returns></returns>
        public static XmlNode GetDataNode(string bh)
        {
            if (bh == null) throw new InvalidDataBhException("Bh is null");
            bh = bh.Trim();
            if (dataInfo == null) throw new CanNotFindDataException(bh);
            if (dataInfo.ContainsKey(bh))
            {
                return dataInfo[bh];
            }
            else
            {
                throw new CanNotFindDataException(bh);
            }
        }

        /// <summary>
        /// 获取测试用例节点
        /// </summary>
        /// <param name="bh"></param>
        /// <returns></returns>
        public static XmlNode GetCaseNode(string bh)
        {
            if (bh == null) throw new InvalidDataBhException("Bh is null");
            bh = bh.Trim();
            if (caseInfo == null) throw new CanNotFindDataException(bh);
            if (caseInfo.ContainsKey(bh))
            {
                return caseInfo[bh];
            }
            else
            {
                throw new CanNotFindDataException(bh);
            }
        }

        /// <summary>
        /// 获取测试验证点节点
        /// </summary>
        /// <param name="bh"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static XmlNode GetCheckNode(string bh, string target = null)
        {
            if (bh == null) throw new InvalidDataBhException("Bh is null");
            bh = bh.Trim();
            if (target == null)
                target = Config.TargetProjectName;

            if (checkInfo == null)
                throw new CanNotFindDataException(bh);

            if (!checkInfo.ContainsKey(target.ToLower()))
                throw new CanNotFindDataException(bh);

            if (checkInfo[target.ToLower()] == null)
                throw new CanNotFindDataException(bh);

            if (!checkInfo[target.ToLower()].ContainsKey(bh))
                throw new CanNotFindDataException(bh);

            return checkInfo[target.ToLower()][bh];
        }
        

        /// <summary>
        /// 退出当前测试
        /// </summary>
        /// <param name="message">推出原因</param>
        public static void Break(string message)
        {
            normalTest = message;
        }

        /// <summary>
        /// 获取指定编号的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="bh">数据编号</param>
        /// <returns></returns>
        public static T GetData<T>(string bh)
        {
            if (bh == null) throw new InvalidDataBhException("Bh is null");
            bh = bh.Trim();
            if (typeof(T) == typeof(CheckPoint))
            {
                LogManager.Error("Please use GetCheckPoint function instead of this");
                throw new CanNotFindDataException(bh);
            }
            else if (typeof(T) == typeof(TestCase))
            {
                LogManager.Error("Please use GetTestCase function instead of this");
                throw new CanNotFindDataException(bh);
            }
            else
            {
                if (string.IsNullOrEmpty(bh))
                {
                    throw new CanNotFindDataException("null");
                }
                if (!dataInfo.ContainsKey(bh))
                {
                    throw new CanNotFindDataException(bh);
                }
                return DataFactory.GetData<T>(dataInfo[bh]);
            }
        }

        /// <summary>
        /// 获取指定编号的数据
        /// </summary>
        /// <param name="bh">数据编号</param>
        /// <returns></returns>
        public static object GetData(string bh)
        {
            if (bh == null) throw new InvalidDataBhException("Bh is null");
            bh = bh.Trim();
            if (string.IsNullOrEmpty(bh))
            {
                throw new InvalidParamValueException("Bh is empty");
            }

            if (!dataInfo.ContainsKey(bh))
            {
                throw new CanNotFindDataException(bh);
            }
            return DataFactory.GetData(dataInfo[bh]);
        }

        /// <summary>
        /// 获取用例数据
        /// </summary>
        /// <param name="bh">用例编号</param>
        /// <returns></returns>
        public static TestCase GetTestCase(string bh)
        {
            if (bh == null) throw new InvalidDataBhException("Bh is null");
            bh = bh.Trim();
            if (caseInfo.ContainsKey(bh))
            {
                return DataFactory.GetData<TestCase>(caseInfo[bh]);
            }
            else
            {
                throw new CanNotFindDataException(bh);
            }
        }

        /// <summary>
        /// 获取验证点数据
        /// </summary>
        /// <param name="bh">数据编号</param>
        /// <param name="target">目标平台</param>
        /// <returns></returns>
        public static CheckPoint GetCheckPoint(string bh, string target = null)
        {
            if (bh == null) throw new InvalidDataBhException("Bh is null");
            bh = bh.Trim();
            if (!checkBhInfo.ContainsKey(bh))
            {
                throw new CanNotFindDataException(bh);
            }
            if (target == null)
            {
                target = Config.TargetProjectName;
            }

            if (target == null)
            {
                throw new NotSetAffirmativelySettingItemException("TargetProject");
            }

            target = target.ToLower();

            if (checkInfo.ContainsKey(target))
            {
                Dictionary<string, XmlNode> checkData = checkInfo[target];
                if (checkData.ContainsKey(bh))
                {
                    return DataFactory.GetData<CheckPoint>(checkData[bh]);
                }
            }

            string[] targetArray = checkBhInfo[bh].Split(',');

            if (targetArray.Length == 1)
            {
                string tString = targetArray[0];
                Dictionary<string, XmlNode> checkData = checkInfo[tString];
                XmlNode xn = checkData[bh];

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xn.OuterXml);
                try
                {
                    xn = XmlFactory.SelectSingleNodeIgnoreCase(doc.DocumentElement, target);
                    XmlDocument xd = new XmlDocument();
                    xd.LoadXml(doc.OuterXml);
                    XmlNode checkPointNode = xd.SelectSingleNode("//CheckPoint");
                    if (checkPointNode == null)
                    {
                        LogManager.Debug("checkPointNode is null");
                        throw new CanNotFindDataException(bh);
                    }
                    checkPointNode.RemoveAll();
                    foreach (XmlNode xnc in doc.DocumentElement.ChildNodes)
                    {
                        if (XmlFactory.HasChild(xnc))
                        {
                            continue;
                        }

                        XmlNode childNode = xd.ImportNode(xnc, true);
                        checkPointNode.AppendChild(childNode);
                    }
                    
                    foreach (XmlNode x in xn.ChildNodes)
                    {
                        XmlNode childNode = xd.ImportNode(x, true);
                        checkPointNode.AppendChild(childNode);
                    }
                    LogManager.DebugFormat("Get {0}", tString);
                    return DataFactory.GetData<CheckPoint>(xd);
                }
                catch
                {
                    LogManager.DebugFormat("Get {0}", tString);
                    return DataFactory.GetData<CheckPoint>(xn);
                }
            }
            else
            {
                foreach (string tString in targetArray)
                {
                    Dictionary<string, XmlNode> checkData = checkInfo[tString];
                    XmlNode xn = checkData[bh];
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xn.OuterXml);
                    try
                    {
                        //XmlDocument doc = new XmlDocument();
                        //doc.LoadXml(xn.OuterXml);
                        //xn = DataFactory.SelectSingleNodeIgnoreCase(doc.DocumentElement, target);
                        //foreach (XmlNode x in xn.ChildNodes)
                        //{
                        //    doc.DocumentElement.AppendChild(x);
                        //}
                        //doc.DocumentElement.RemoveChild(xn);
                        //LogManager.Debug("Get {0}", tString);
                        //return DataFactory.GetData<CheckPoint>(doc);

                        xn = XmlFactory.SelectSingleNodeIgnoreCase(doc.DocumentElement, target);
                        XmlDocument xd = new XmlDocument();
                        xd.LoadXml(doc.OuterXml);
                        XmlNode checkPointNode = xd.SelectSingleNode("//CheckPoint");
                        if (checkPointNode == null)
                        {
                            LogManager.Debug("checkPointNode is null");
                            throw new CanNotFindDataException(bh);
                        }
                        checkPointNode.RemoveAll();
                        foreach (XmlNode xnc in doc.DocumentElement.ChildNodes)
                        {
                            if (XmlFactory.HasChild(xnc))
                            {
                                continue;
                            }

                            XmlNode childNode = xd.ImportNode(xnc, true);
                            checkPointNode.AppendChild(childNode);
                        }

                        foreach (XmlNode x in xn.ChildNodes)
                        {
                            XmlNode childNode = xd.ImportNode(x, true);
                            checkPointNode.AppendChild(childNode);
                        }
                        LogManager.DebugFormat("Get {0}", tString);
                        return DataFactory.GetData<CheckPoint>(xd);
                    }
                    catch { }
                }
            }
            throw new CanNotFindDataException(bh);
        }

        /// <summary>
        /// 执行用例前的初始化信息
        /// </summary>
        /// <param name="filePath">选取的用例编号数据</param>
        public static void Initilize(string filePath)
        {
            InitCaseList(filePath);
        }
        
        /// <summary>
        /// 清除所有测试用例
        /// </summary>
        public static void Clear()
        {
            if (_testCaseList == null)
            {
                return;
            }
            _testCaseList.Clear();
        }

        /// <summary>
        /// 使用代码添加测试用例
        /// </summary>
        /// <param name="testCase"></param>
        public static void AddTestCase(TestCase testCase)
        {
            if (_testCaseList == null)
            {
                _testCaseList = new List<TestCase>();
            }
            _testCaseList.Add(testCase);
        }

        public static void AddTestCase(string caseBh, bool throwException = false)
        {
            try
            {
                TestCase testCase = GetTestCase(caseBh);
                if (testCase == null)
                {
                    throw new CanNotFindDataException(caseBh);
                }
                if (_testCaseList == null)
                {
                    _testCaseList = new List<TestCase>();
                }

                _testCaseList.Add(testCase);
            }
            catch(System.Exception ex)
            {
                if (throwException) throw ex;
                LogManager.DebugFormat("Add testcase[{0}] failed, {1} \n {2}", caseBh, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 执行所有测试用例，生成测试结果
        /// </summary>
        public static void Execute()
        {
            InitBaseDirectory();
            XmlDocument caseDocument = XmlFactory.LoadXml(XmlFactory.XmlRootString.Replace("Data", "data"));
            string reportPath = Path.Combine(Config.WorkingDirectory, "Report", "case.xml");
            int index = 0;
            XmlNode dataNode = caseDocument.SelectSingleNode("//data");
            XmlNode testDateNode = caseDocument.CreateElement("startTestDate");
            dataNode.AppendChild(testDateNode);
            testDateNode.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            testDateNode = caseDocument.CreateElement("endTestDate");
            dataNode.AppendChild(testDateNode);
            testDateNode.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            AutoResetEvent reset = new AutoResetEvent(false);
            bool complete = false;
            new Thread(() =>
                {
                    while (!complete)
                    {
                        reset.Reset();
                        reset.WaitOne();
                        testDateNode.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        caseDocument.Save(reportPath);
                    }
                }).Start();

            Timer caseTimer = new Timer(state => reset.Set(), null, 0, 2000);

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(reportPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
                }
                Mock.Tools.Controls.WWindow.Listen();

                List<CaseReport> reportList = new List<CaseReport>();
                
                foreach (TestCase tc in _testCaseList)
                {
                    XmlElement caseNode = caseDocument.CreateElement("case");
                    dataNode.AppendChild(caseNode);
                    try
                    {
                        CaseReport report = new CaseReport(tc, index++, caseNode);
                        reportList.Add(report);
                    }
                    catch (InvalidParamValueException)
                    {
                        continue;
                    }
                }

                Mock.Tools.Tasks.TaistTaskCollection caseTasks = new Tools.Tasks.TaistTaskCollection();
                caseTasks.MaxRunCount = Config.ConcurrencyCount;
                foreach (CaseReport report in reportList)
                {
                    caseTasks.Add(() =>
                        {
                            if (!normalTest)
                            {
                                report.Break(normalTest);
                                LogManager.Message(normalTest);
                                caseTasks.Close();
                                return;
                            }
                            PreExecute();

                            report.Fill();
                            if (report.Result == ReportResult.FAILED)
                            {
                                report.Save();
                                SaveFailedCase(report.Bh);
                            }
                        });
                }

                caseTasks.Run(Tools.Tasks.TaistTaskType.CONCURRENCY);
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex);
                throw;
            }
            finally
            {
                complete = true;
                reset.Set();

                //2018.04.03 韩志强修改
                //测试结束更新报告需要等待一定时间后再结束定时器
                //否则会导致报告生成不完全
                Robot.Recess(1000);
                caseTimer.Dispose();
            }
        }

        /// <summary>
        /// 设置明细信息
        /// </summary>
        /// <param name="detail">明细信息，这通常为TestCasePool.SetDetailColumnName所设置的结构数据</param>
        public static void SetReportDetail(ReportDetail detail)
        {
            CaseManager.SetReportDetail(detail);
        }

        /// <summary>
        /// 设置明细信息内容中包含的项目名称
        /// </summary>
        /// <param name="displayName">明细信息显示名称</param>
        /// <param name="tagName">明细信息简化名称（这通常是明细信息生成XML时的标签名称）</param>
        /// <param name="state">指示该列内容是状态标志</param>
        public static void SetDetailColumnName(string displayName, string tagName, bool state = false)
        {
            CaseManager.SetDetailColumnName(displayName, tagName, state);
        }

        #region 内部方法
        ///// <summary>
        ///// 设置新的CheckPoint
        ///// </summary>
        ///// <param name="bh"></param>
        ///// <param name="node"></param>
        //internal static void SetCheckPoint(string bh, XmlNode node)
        //{
        //    if (newCheckInfo == null)
        //    {
        //        newCheckInfo = new Dictionary<string, XmlNode>();
        //    }

        //    if (newCheckInfo.ContainsKey(bh))
        //    {
        //        //throw new NotUniqueDataException(bh);
        //        newCheckInfo[bh] = node;
        //    }
        //    else
        //    {
        //        newCheckInfo.Add(bh, node);
        //    }
        //}

        ///// <summary>
        ///// 将修改后的CheckPoint保存到Data\NewCheck文件夹下
        ///// </summary>
        //private static void SaveCheckPoint()
        //{
        //    LogManager.Message("SaveCheckPoint");
        //    if (newCheckInfo == null)
        //    {
        //        LogManager.Message("New check infomation is null");
        //        return;
        //    }
        //    string checkPath = Path.Combine(Config.WorkingDirectory, "Data\\CheckData", Config.TargetProjectName);
        //    string newPath = Path.Combine(Config.WorkingDirectory, "Data\\NewCheck", Config.TargetProjectName);
        //    if (Directory.Exists(newPath))
        //    {
        //        DataFactory.DeleteDirecotry(newPath);
        //    }
        //    Directory.CreateDirectory(newPath);

        //    List<string> fileList = DataFactory.GetAllFileNames(checkPath, ".xml");
        //    foreach (string fileName in fileList)
        //    {
        //        string newFileName = Path.Combine(newPath, fileName.Substring(checkPath.Length + 1));
        //        string dirName = Path.GetDirectoryName(newFileName);
        //        if (!Directory.Exists(dirName))
        //        {
        //            Directory.CreateDirectory(dirName);
        //        }
        //        XmlDocument doc = new XmlDocument();
        //        try
        //        {
        //            doc.Load(fileName);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            LogManager.Error(string.Format("{0} {1}", fileName, ex.Message));
        //            continue;
        //        }
        //        XmlNodeList xnl = doc.SelectNodes("//Bh");
        //        if (xnl == null)
        //        {
        //            LogManager.Error(string.Format("{0} not contains Bh node", fileName));
        //            continue;
        //        }
        //        foreach (XmlNode xn in xnl)
        //        {
        //            string bh = xn.InnerText;
        //            if (newCheckInfo.ContainsKey(bh))
        //            {
        //                XmlNode parent = xn.ParentNode;
        //                XmlNode newNode = doc.ImportNode(newCheckInfo[bh], true);
        //                parent.ParentNode.ReplaceChild(newNode, parent);
        //            }
        //        }

        //        doc.Save(newFileName);
        //    }
        //}


        #endregion

        #region 私有方法

        #region 初始化要执行的用例列表
        private static void InitCaseList(string filePath)
        {
            _testCaseList.Clear();

            List<string> exceptList = new List<string>();
            do
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    break;
                }

                if (!File.Exists(filePath))
                {
                    LogManager.Warning(string.Format("Can not find file [{0}], it will run all the testcase", Path.GetFullPath(filePath)));
                    break;
                }

                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(filePath);
                }
                catch
                {
                    LogManager.DebugFormat("{0} is not a valid xml file.", filePath);
                    return;
                }
                
                XmlNodeList exceptNodeList = doc.SelectNodes("//Except");
                {
                    if (exceptNodeList.Count > 0)
                    {
                        foreach (XmlNode node in exceptNodeList)
                        {
                            List<string> bhList = DataFactory.ParseBH(node.InnerText);
                            foreach (string bh in bhList)
                            {
                                if (!exceptList.Contains(bh))
                                {
                                    exceptList.Add(bh);
                                }
                            }
                        }
                    }
                }

                XmlNodeList nodeList = doc.SelectNodes("//Bh");
                if (nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        if (caseInfo.ContainsKey(node.InnerText))
                        {
                            AddTestCase(node.InnerText);
                        }
                        else
                        {
                            List<string> bhList = DataFactory.ParseBH(node.InnerText);
                            foreach (string bh in bhList)
                            {
                                if (exceptList.Contains(bh))
                                {
                                    continue;
                                }

                                if (!caseInfo.ContainsKey(bh))
                                {
                                    LogManager.Warning(string.Format("未找到编号为[{0}]的测试用例", bh));
                                    continue;
                                }
                                AddTestCase(bh);
                            }
                        }
                    }
                    return;
                }
            }
            while (false);
            InitAllCase(exceptList);
        }

        private static void InitAllCase(List<string> exceptList)
        {
            foreach (string caseBh in caseInfo.Keys)
            {
                if (exceptList.Contains(caseBh)) continue;
                AddTestCase(caseBh);
            }
        }
        #endregion

        #region 加载所有测试用例信息
        /// <summary>
        /// 加载所有测试用例信息
        /// </summary>
        private static void InitCase()
        {
            string casePath = Path.Combine(Config.WorkingDirectory, "Data\\TestCase");
            if (!Directory.Exists(casePath))
            {
                Directory.CreateDirectory(casePath);
            }
            caseInfo = new Dictionary<string, XmlNode>();

            List<string> fileNameList = FileFactory.GetAllFileNames(casePath, ".xml");
            foreach (string fileName in fileNameList)
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(fileName);
                }
                catch (System.Exception ex)
                {
                    LogManager.Error(string.Format("{0} {1}", fileName, ex.Message));
                    continue;
                }
                XmlNodeList xnl = doc.SelectNodes("//Bh");
                foreach (XmlNode xn in xnl)
                {
                    try
                    {
                        caseInfo.Add(xn.InnerText, xn.ParentNode);
                    }
                    catch
                    {
                        LogManager.Warning(string.Format("{1}中编号为[{0}]的用例重复", xn.InnerText, fileName));
                    }
                }
            }
        }
        #endregion

        #region 初始化缓存
        private static readonly object tempLock = new object();
        private static bool delTmp = false;
        /// <summary>
        /// 初始化缓存文件
        /// </summary>
        private static void InitTemp()
        {
            string tmpPath = null;
            try
            {
                tmpPath = Path.Combine(Config.WorkingDirectory, "Temp", CaseManager.GetIdentification() + ".temp");
            }
            catch
            {
                tmpPath = Path.Combine(Config.WorkingDirectory, "Temp\\taist.temp");
            }
            if (!Directory.Exists(Path.GetDirectoryName(tmpPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
            }

            if (File.Exists(tmpPath))
            {
                if (delTmp)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml("<?xml version=\"1.0\" encoding=\"gbk\"?><Data></Data>");
                    lock (tempLock)
                    {
                        doc.Save(tmpPath);
                    }
                }
                else
                {
                    delTmp = true;
                }
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<?xml version=\"1.0\" encoding=\"gbk\"?><Data></Data>");
                lock (tempLock)
                {
                    doc.Save(tmpPath);
                }
            }
        }
        #endregion

        #region 初始化报告
        private static void InitBaseDirectory()
        {
            FileFactory.ClearDirectory(Path.Combine(Config.WorkingDirectory, "Report"));
            FileFactory.ClearDirectory(Path.Combine(Config.WorkingDirectory, "Temp\\Bak"));
            FileFactory.ClearDirectory(Path.Combine(Config.WorkingDirectory, "Log"));
            string tmpPath = Path.Combine(Config.WorkingDirectory, "ErrorCase.xml");
            if(File.Exists(tmpPath))
            {
                File.Delete(tmpPath);
            }
        }
        #endregion

        #region 初始化执行数据
        /// <summary>
        /// 加载测试数据信息
        /// </summary>
        private static void InitData()
        {
            string dataPath = Path.Combine(Config.WorkingDirectory, "Data\\CaseData");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            dataInfo = new Dictionary<string, XmlNode>();

            List<string> fileNameList = FileFactory.GetAllFileNames(dataPath, ".xml");
            foreach (string fileName in fileNameList)
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(fileName);
                }
                catch (System.Exception ex)
                {
                    LogManager.Error(string.Format("{0} {1}", fileName, ex.Message));
                    continue;
                }
                XmlNodeList xnl = doc.SelectNodes("//Bh");
                foreach (XmlNode xn in xnl)
                {
                    try
                    {
                        dataInfo.Add(xn.InnerText, xn.ParentNode);
                    }
                    catch
                    {
                        LogManager.Warning(string.Format("{1}中编号为[{0}]的数据重复", xn.InnerText, fileName));
                    }
                }
            }
        }
        #endregion

        #region 初始化验证点数据
        /// <summary>
        /// 加载预期结果数据信息
        /// </summary>
        private static void InitCheck()
        {
            string dataPath = Path.Combine(Config.WorkingDirectory, "Data\\CheckData");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            checkInfo = new Dictionary<string, Dictionary<string, XmlNode>>();
            checkBhInfo = new Dictionary<string, string>();

            string[] targetPathArray = Directory.GetDirectories(dataPath);

            foreach (string target in targetPathArray)
            {
                List<string> fileNameList = FileFactory.GetAllFileNames(target, ".xml");
                Dictionary<string, XmlNode> checkData = new Dictionary<string, XmlNode>();
                string name = Path.GetFileNameWithoutExtension(target).ToLower();
                checkInfo.Add(name, checkData);
                foreach (string fileName in fileNameList)
                {
                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.Load(fileName);
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.Error(string.Format("{0} {1}", fileName, ex.Message));
                        continue;
                    }
                    XmlNodeList xnl = doc.SelectNodes("//Bh");
                    foreach (XmlNode xn in xnl)
                    {
                        if (checkData.ContainsKey(xn.InnerText))
                        {
                            LogManager.Warning(string.Format("{1}中编号为[{0}]的验证点重复", xn.InnerText, fileName));
                        }
                        else
                        {
                            checkData.Add(xn.InnerText, xn.ParentNode);
                            if (checkBhInfo.ContainsKey(xn.InnerText))
                            {
                                checkBhInfo[xn.InnerText] = checkBhInfo[xn.InnerText] + "," + name;
                            }
                            else
                            {
                                checkBhInfo.Add(xn.InnerText, name);
                            }
                        }
                    }
                }
            }

            string[] otherDataArray = Directory.GetFiles(dataPath);

            Dictionary<string, XmlNode> ocheckData = new Dictionary<string, XmlNode>();
            string oname = "OTHERS";
            checkInfo.Add(oname, ocheckData);
            foreach (string target in otherDataArray)
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(target);
                }
                catch (System.Exception ex)
                {
                    LogManager.Error(string.Format("{0} {1}", target, ex.Message));
                    continue;
                }
                XmlNodeList xnl = doc.SelectNodes("//Bh");
                foreach (XmlNode xn in xnl)
                {
                    if (ocheckData.ContainsKey(xn.InnerText))
                    {
                        LogManager.Warning(string.Format("{1}中编号为[{0}]的验证点重复", xn.InnerText, target));
                    }
                    else
                    {
                        ocheckData.Add(xn.InnerText, xn.ParentNode);
                        if (checkBhInfo.ContainsKey(xn.InnerText))
                        {
                            checkBhInfo[xn.InnerText] = checkBhInfo[xn.InnerText] + "," + oname;
                        }
                        else
                        {
                            checkBhInfo.Add(xn.InnerText, oname);
                        }
                    }
                }
            }
        }
        #endregion

        #region 用例执行前要执行的操作
        private static void PreExecute()
        {
            InitTemp();
        }
        #endregion

        #region 保存失败用例的编号
        private static readonly object failedCaseLock = new object();
        private static void SaveFailedCase(string bh)
        {
            lock (failedCaseLock)
            {
                string filePath = Path.Combine(Config.WorkingDirectory, "ErrorCase.xml");
                XmlDocument doc = new XmlDocument();
                if (!File.Exists(filePath))
                {
                    string dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    doc.LoadXml("<?xml version=\"1.0\" encoding=\"gbk\"?><CasePool></CasePool>");
                }
                else
                {
                    doc.Load(filePath);
                }

                XmlNode parent = doc.SelectSingleNode("//CasePool");
                XmlNode bhNode = parent.SelectSingleNode("Bh");
                if (bhNode == null)
                {
                    bhNode = doc.CreateElement("Bh");
                    parent.AppendChild(bhNode);
                }

                if (string.IsNullOrEmpty(bhNode.InnerText))
                {
                    bhNode.InnerText = bh;
                }
                else
                {
                    bhNode.InnerText = string.Format("{0},{1}", bhNode.InnerText, bh);
                }

                doc.Save(filePath);
            }
        }
        #endregion

        #endregion
    }
}
