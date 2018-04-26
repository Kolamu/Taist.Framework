namespace Mock.Data
{
    using System.Collections.Generic;
    using System;
    using System.Xml;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    using Mock.Data.Attributes;
    using Mock.Data.Exception;
    /// <summary>
    /// 表示测试用例对象
    /// </summary>
    public class TestCase : IFormatData
    {
        private List<Step> _steps = new List<Step>();
        private Dictionary<string, string> configValue = null;

        public TestCase()
        {
            Bh = "Default";
            CaseName = "CaseTemplete";
            Description = "This is a templete of case object";
            CaseId = "Default";
            _steps = null;
            BreakOnFailed = false;
            configValue = new Dictionary<string, string>();
        }

        [FieldProperty("CaseName", false)]
        public string CaseName { get; set; }

        [FieldProperty("Description", false)]
        public string Description { get; set; }

        [FieldProperty("CaseId", false)]
        public string CaseId { get; set; }

        [FieldProperty("Repeat", true)]
        public string Repeat { get; set; }

        [FieldProperty("BreakOnFailed", true)]
        public bool BreakOnFailed { get; set; }

        public override IFormatData FromXml(XmlNode doc, Dictionary<string, string> conditions)
        {
            string conditionString = string.Empty;
            if (conditions == null || conditions.Count < 1)
            {
                conditionString = "//TestCase";
            }
            else
            {
                List<string> keys = conditions.Keys.ToList<string>();
                foreach (string key in keys)
                {
                    if (string.IsNullOrEmpty(conditionString))
                    {
                        conditionString = string.Format("//TestCase[{0}='{1}']", key, conditions[key]);
                    }
                    else
                    {
                        conditionString = string.Format("{0} and {1}='{2}']", conditionString.TrimEnd(']'), key, conditions[key]);
                    }
                }
            }

            XmlNode caseNode = doc.SelectSingleNode(conditionString);
            if (caseNode == null)
            {
                throw new CanNotFindDataException(conditionString);
            }
            TestCase caseObject = DataFactory.XmlToObject<TestCase>(caseNode);

            caseObject.configValue = new Dictionary<string, string>();
            if(caseNode.Attributes != null && caseNode.Attributes.Count > 0)
            {
                foreach (XmlAttribute attr in caseNode.Attributes)
                {
                    if (typeof(TestCase).GetProperty(attr.Name) == null)
                    {
                        caseObject.configValue.Add(attr.Name, attr.Value);
                    }
                }
            }

            XmlNodeList stepNodeList = caseNode.SelectNodes("Step");
            caseObject.Steps = new List<Step>();
            foreach (XmlNode stepNode in stepNodeList)
            {
                Step step = DataFactory.GetData<Step>(stepNode);
                caseObject.Steps.Add(step);
            }
            return (IFormatData)caseObject;
        }

        public override string ToXml()
        {
            string xml = string.Empty;
            return xml;
        }

        [FieldProperty(isIgnore=true)]
        public List<Step> Steps
        {
            get
            {
                return _steps;
            }
            set
            {
                _steps = value;
            }
        }

        public override void Init()
        {
        }

        private bool minExecute = false;
        internal void Start()
        {
            //Config.resetCaseConfig();
            foreach (KeyValuePair<string, string> kv in configValue)
            {
                Config.setCaseConfig(kv.Key, kv.Value);
            }
            if (_steps == null || _steps.Count == 0)
            {
                LogManager.DebugFormat("TestCase [{0}] not contains steps", Bh);
                CaseManager.Report("初始化", string.Format("TestCase [{0}] not contains steps", Bh));
            }

            minExecute = false;

            new Thread(() =>
                {
                    try
                    {
                        Robot.ExecuteWithTimeOut(() =>
                        {
                            Robot.Recess(Config.MinExecutionTime * 1000);
                        });
                    }
                    catch { }
                    finally
                    {
                        minExecute = true;
                    }
                }).Start();
        }

        internal void Stop(string stepId)
        {
            while (!minExecute)
            {
                Robot.Recess(10);
            }

            Jump(stepId);
        }

        internal void Jump(string stepId)
        {

            if (string.IsNullOrEmpty(stepId))
            {
                return;
            }

            Step step = null;
            foreach (Step tmp in _steps)
            {
                if (string.Equals(tmp.Id, stepId))
                {
                    step = tmp;
                    break;
                }
            }

            if (step == null)
            {
                return;
            }

            StepReport report = new StepReport(step, -1, "jmpf", null);
            for (int i = 0; i < Config.RedoCount; i++)
            {
                report.Fill();
                if (report.Result == ReportResult.SUCCESS)
                {
                    break;
                }
            }
            Jump(step.JumpOnFailed);
        }

        internal void Invoke(Step parentStep)
        {
            //Config.resetCaseConfig();
            foreach (KeyValuePair<string, string> kv in configValue)
            {
                Config.setCaseConfig(kv.Key, kv.Value);
            }
            if (_steps == null || _steps.Count == 0)
            {
                LogManager.DebugFormat("TestCase [{0}] not contains steps", Bh);
                CaseManager.Report("初始化", string.Format("TestCase [{0}] not contains steps", Bh));
            }

            foreach (Step step in _steps)
            {
                step.ParentStep = parentStep;
                step.Execute();

                if (!parentStep.ExecuteResult)
                {
                    Jump(step.JumpOnFailed);
                    break;
                }
                else
                {
                    CaseManager.ClearReportDetail();
                }
            }
        }
    }
}
