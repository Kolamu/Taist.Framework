namespace Mock.Data
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Threading;
    using System.Collections.Generic;

    using Mock.Data.Attributes;
    using Mock.Data.Exception;
    public enum ReportResult
    {
        SUCCESS,
        FAILED,
        BLOCKED,
        WATING,
        RUNNING
    }
    public class CaseReport
    {
        private TestCase _testCase = null;
        private cse _caseReport = null;
        private List<StepReport> _stepReport = new List<StepReport>();
        private XmlDocument _caseDocument;
        private string path = null;
        
        public CaseReport(TestCase testCase, int index, XmlNode node)
        {
            if (testCase == null) throw new InvalidParamValueException("TestCase is null");
            _testCase = testCase;
            _caseReport = new cse();
            if (node == null)
            {
                _caseReport.Node = XmlFactory.LoadXml("<case></case>").DocumentElement;
            }
            else
            {
                _caseReport.Node = node;
            }
            _caseReport.bh = index.ToString();
            _caseReport.cbh = _testCase.Bh;
            _caseReport.id = _testCase.CaseId;
            _caseReport.name = _testCase.CaseName;
            _caseReport.summary = _testCase.Description;
            Result = ReportResult.WATING;
            path = Path.Combine(Config.WorkingDirectory, "Report", "info" + index + ".xml");
            InitStep();
        }

        private ReportResult _result = ReportResult.WATING;
        public ReportResult Result
        {
            get
            {
                return _result;
            }
            private set
            {
                _result = value;
                switch (_result)
                {
                    case ReportResult.RUNNING:
                        {
                            _caseReport.state = "I";
                            break;
                        }
                    case ReportResult.BLOCKED:
                        {
                            _caseReport.state = "B";
                            break;
                        }
                    case ReportResult.FAILED:
                        {
                            _caseReport.state = "F";
                            break;
                        }
                    case ReportResult.SUCCESS:
                        {
                            _caseReport.state = "S";
                            break;
                        }
                    case ReportResult.WATING:
                        {
                            _caseReport.state = "W";
                            break;
                        }
                }
            }
        }

        internal XmlNode Node
        {
            get
            {
                return _caseReport.Node;
            }
            set
            {
                _caseReport.Node = value;
            }
        }

        public string Bh
        {
            get
            {
                return _testCase.Bh;
            }
        }

        public void Fill()
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    Result = ReportResult.RUNNING;
                    Run();
                }
                catch (System.Exception ex)
                {
                    LogManager.Error(ex);
                    CaseManager.Report("用例异常", ex);
                }
            });
            string uid = CaseManager.CreateIdentification(t.ManagedThreadId);
            t.IsBackground = true;
            t.Start();
            t.Join();

            CaseManager.RemoveIdentification(uid);
            BakCache(uid);
            BakLog(uid);
        }

        internal void Break(string message)
        {
            foreach (StepReport step in _stepReport)
            {
                step.Break(message);
            }
        }

        public void Save()
        {
            _caseDocument.Save(path);
            foreach (StepReport step in _stepReport)
            {
                if (step.Result == ReportResult.FAILED)
                {
                    step.Save();
                    break;
                }
            }
        }

        private void BakCache(string uid)
        {
            string bakPath = Path.Combine(Config.WorkingDirectory, "Temp\\Bak", Bh + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml");
            if (!Directory.Exists(Path.GetDirectoryName(bakPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(bakPath));
            }

            string cachePath = Path.Combine(Config.WorkingDirectory, "Temp", uid + ".temp");
            if (File.Exists(cachePath))
            {
                File.Move(cachePath, bakPath);
                new Thread(() =>
                {
                    Robot.Recess(500);
                    File.Delete(cachePath);
                }).Start();
            }
        }

        private void BakLog(string uid)
        {
            string bakPath = Path.Combine(Config.WorkingDirectory, "Log", Bh + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            if (!Directory.Exists(Path.GetDirectoryName(bakPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(bakPath));
            }

            string cachePath = Path.Combine(Config.WorkingDirectory, uid + ".log");
            if (File.Exists(cachePath))
            {
                File.Copy(cachePath, bakPath);
                new Thread(() =>
                    {
                        Robot.Recess(500);
                        File.Delete(cachePath);
                    }).Start();
            }
        }


        private void InitStep()
        {
            _caseDocument = XmlFactory.LoadXml(XmlFactory.XmlRootString.Replace("Data", "data"));
            XmlNode dataNode = _caseDocument.SelectSingleNode("//data");
            int repeat = 1;
            if (!int.TryParse(_testCase.Repeat, out repeat))
            {
                repeat = 1;
            }

            for (int i = 0; i < repeat; i++)
            {
                string nextStepId = null;
                for (int j = 0; j < _testCase.Steps.Count; j++)
                {
                    Step step = _testCase.Steps[j];

                    XmlElement busNode = _caseDocument.CreateElement("bus");
                    dataNode.AppendChild(busNode);
                    StepReport report = new StepReport(step, i * _testCase.Steps.Count + j, _caseReport.bh, busNode);
                    
                    if (string.IsNullOrEmpty(step.JumpOnFailed))
                    {
                        if (string.Equals(nextStepId, step.Id))
                        {
                            report.Next = null;
                        }
                        else
                        {
                            report.Next = nextStepId;
                        }
                    }
                    else
                    {
                        nextStepId = step.JumpOnFailed;
                        report.Next = nextStepId;
                    }

                    if (repeat > 1)
                    {
                        report.Name = string.Format("{0} {1}/{2}", report.Name, i + 1, repeat);
                    }
                    _stepReport.Add(report);
                }
            }
        }

        private void InitCase()
        {

        }

        private void Run()
        {
            LogManager.Message(string.Format("开始测试用例 [{0}] ...", Bh));
            Result = ReportResult.RUNNING;
            _caseReport.tdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _testCase.Start();

            string stepId = null;
            foreach (StepReport step in _stepReport)
            {
                step.Fill();
                if (step.Result == ReportResult.FAILED)
                {
                    if (_testCase.BreakOnFailed)
                    {
                        TestCasePool.Break(_testCase.Bh + " failed and it's BreakOnFailed is true");
                    }
                    Result = ReportResult.FAILED;
                    stepId = step.Next;
                    break;
                }
            }

            if (Result == ReportResult.RUNNING)
            {
                Result = ReportResult.SUCCESS;
            }
            _testCase.Stop(stepId);
        }

        private class cse
        {
            private XmlNode bhNode = null;
            private XmlNode cbhNode = null;
            private XmlNode idNode = null;
            private XmlNode nameNode = null;
            private XmlNode summaryNode = null;
            private XmlNode stateNode = null;
            private XmlNode tdateNode = null;
            private XmlNode machineNode = null;

            private XmlNode caseNode = null;
            public XmlNode Node
            {
                get
                {
                    return caseNode;
                }
                set
                {
                    caseNode = value;
                    InitNode();
                }
            }

            private void InitNode()
            {
                bhNode = caseNode.SelectSingleNode("bh");
                if (bhNode == null)
                {
                    bhNode = caseNode.OwnerDocument.CreateElement("bh");
                    caseNode.AppendChild(bhNode);
                }

                cbhNode = caseNode.SelectSingleNode("cbh");
                if (cbhNode == null)
                {
                    cbhNode = caseNode.OwnerDocument.CreateElement("cbh");
                    caseNode.AppendChild(cbhNode);
                }

                idNode = caseNode.SelectSingleNode("id");
                if (idNode == null)
                {
                    idNode = caseNode.OwnerDocument.CreateElement("id");
                    caseNode.AppendChild(idNode);
                }

                nameNode = caseNode.SelectSingleNode("name");
                if (nameNode == null)
                {
                    nameNode = caseNode.OwnerDocument.CreateElement("name");
                    caseNode.AppendChild(nameNode);
                }

                summaryNode = caseNode.SelectSingleNode("summary");
                if (summaryNode == null)
                {
                    summaryNode = caseNode.OwnerDocument.CreateElement("summary");
                    caseNode.AppendChild(summaryNode);
                }

                stateNode = caseNode.SelectSingleNode("state");
                if (stateNode == null)
                {
                    stateNode = caseNode.OwnerDocument.CreateElement("state");
                    caseNode.AppendChild(stateNode);
                    stateNode.InnerText = "W";
                }

                tdateNode = caseNode.SelectSingleNode("tdate");
                if (tdateNode == null)
                {
                    tdateNode = caseNode.OwnerDocument.CreateElement("tdate");
                    caseNode.AppendChild(tdateNode);
                    tdateNode.InnerText = "";
                }

                machineNode = caseNode.SelectSingleNode("machine");
                if (machineNode == null)
                {
                    machineNode = caseNode.OwnerDocument.CreateElement("machine");
                    caseNode.AppendChild(machineNode);
                    machineNode.InnerText = "";
                }
            }

            public string bh { get { return bhNode.InnerText; } set { bhNode.InnerText = value; } }
            public string cbh { get { return cbhNode.InnerText; } set { cbhNode.InnerText = value; } }
            public string id { get { return idNode.InnerText; } set { idNode.InnerText = value; } }
            public string name { get { return nameNode.InnerText; } set { nameNode.InnerText = value; } }
            public string summary { get { return summaryNode.InnerText; } set { summaryNode.InnerText = value; } }
            public string state { get { return stateNode.InnerText; } set { stateNode.InnerText = value; } }
            public string tdate { get { return tdateNode.InnerText; } set { tdateNode.InnerText = value; } }
            public string machine { get { return machineNode.InnerText; } set { machineNode.InnerText = value; } }
        }
    }
}
