namespace Mock.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Reflection;
    
    using Mock.Data.TaistDataCenter;
    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    public class Step : IFormatData
    {
        public Step()
        {
            _params = new Dictionary<string, string>();
            _lparams = new Dictionary<string, string>();
            configValue = new Dictionary<string, string>();
            this.UserDataName = "StepFilter.xml";
        }

        private string _name = null;
        private string _keyword = null;
        private string _subKeyword = null;
        private string _targetProject = null;
        private Dictionary<string, string> _params = null;
        private Dictionary<string, string> _lparams = null;
        private Dictionary<string, string> configValue = null;
        private string _data = null;

        public string DataList
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public string TargetDataList
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string TargetProject
        {
            get
            {
                return _targetProject;
            }
            set
            {
                _targetProject = value;
            }
        }

        public string TargetProjectName
        {
            get
            {
                return TargetProject;
            }
            set
            {
                TargetProject = value;
            }
        }

        [FieldProperty("Keyword", false)]
        public string Keyword
        {
            get
            {
                return _keyword;
            }
            set
            {
                _keyword = value;
            }
        }

        [FieldProperty("SubKeyword", false)]
        public string SubKeyword
        {
            get
            {
                return _subKeyword;
            }
            set
            {
                _subKeyword = value;
            }
        }

        [FieldProperty(Name="id", isAttribute = true)]
        public string Id { get; set; }

        [FieldProperty(Name = "jmpf", isAttribute = true)]
        public string JumpOnFailed { get; set; }

        private Dictionary<string, string> Params
        {
            get
            {
                return _params;
            }
        }

        private Dictionary<string, string> lParams
        {
            get
            {
                return _lparams;
            }
        }

        internal bool ExecuteResult { get; set; }

        internal Step ParentStep { get; set; }

        private string _ofKeyword = null;
        private string FilterKeyword
        {
            get
            {
                try
                {
                    return this.getUserConfig("Keyword");
                }
                catch(System.Exception)
                {
                    return "";
                }
            }
        }

        private string _ofSubKeyword = null;
        private string FilterSubKeyword
        {
            get
            {
                try
                {
                    return this.getUserConfig("SubKeyword");
                }
                catch (System.Exception)
                {
                    return "";
                }
            }
        }

        private string _ofTargetProject = null;
        private string FilterTargetProject
        {
            get
            {
                try
                {
                    return this.getUserConfig("TargetProject");
                }
                catch (System.Exception)
                {
                    return "";
                }
            }
        }

        private int FilterMaxRepeatCount
        {
            get
            {
                try
                {
                    return int.Parse(this.getUserConfig("MaxRepeatCount"));
                }
                catch (System.Exception)
                {
                    return 0;
                }
            }
        }

        public string getParam(string key)
        {
            if (_params == null || !_params.ContainsKey(key))
            {
                return null;
            }
            else
            {
                return _params[key];
            }
        }

        public void setProperty(System.Xml.XmlNode node)
        {
            if (_params == null)
            {
                _params = new Dictionary<string, string>();
                _lparams = new Dictionary<string, string>();
            }

            string key = string.Empty;
            string value = string.Empty;
            if (string.Equals(node.Name, "param", StringComparison.OrdinalIgnoreCase))
            {
                XmlAttribute nameAttr = node.Attributes["name"];
                if (nameAttr == null)
                {
                    throw new CanNotFindPropertyException("param", "name");
                }
                key = node.Attributes["name"].Value;
                value = node.InnerXml;
            }
            else
            {
                key = node.Name;
                value = node.InnerXml;
                LogManager.Warning(string.Format("{0} is neither a Property nor a param", key));
            }

            if (_lparams.ContainsKey(key.ToLower()))
            {
                _lparams[key.ToLower()] = value;
                foreach(string k in _params.Keys)
                {
                    if(string.Equals(k.ToLower(), key))
                    {
                        _params[k] = value;
                        break;
                    }
                }
            }
            else
            {
                _params.Add(key, value);
                _lparams.Add(key.ToLower(), value);
            }
        }

        public Dictionary<string, string> getProperty()
        {
            return _params;
        }

        public void Execute()
        {
            try
            {
                Config.resetStepConfig();
                foreach (KeyValuePair<string, string> kv in configValue)
                {
                    Config.setStepConfig(kv.Key, kv.Value);
                }

                //2017.09.13 增加开票软件内存溢出处理
                LogManager.Message("当前开票软件占用内存 ： {0}, 最大值 ： {1}", Mock.Tools.Controls.RobotContext.CurrentProcessMemory.ToString(), Config.MaxMemoryUsage);
                if (Mock.Tools.Controls.RobotContext.CurrentProcessMemory > Config.MaxMemoryUsage)
                {
                    LogManager.Debug("被测软件内存占用过高！");
                    Robot.CloseSoftware();
                }

                switch (Keyword.ToUpper())
                {
                    case "CHECK":
                        {
                            Check();
                            break;
                        }
                    default:
                        {
                            Keyword keyword = GetKeyword();

                            if (CheckFilter())
                            {
                                for (int i = 0; i < FilterMaxRepeatCount + 1; i++)
                                {
                                    try
                                    {
                                        if (ParentStep != null)
                                        {
                                            ParentStep.ExecuteResult = true;
                                            LogManager.DebugFormat("execute : {0}, pexec: {1}", ExecuteResult, ParentStep.ExecuteResult);
                                        }

                                        keyword.Invoke(DataList, _lparams);
                                        if (ParentStep != null)
                                        {
                                            LogManager.DebugFormat("execute : {0}, pexec: {1}", ExecuteResult, ParentStep.ExecuteResult);
                                        }

                                        if (ExecuteResult || (ParentStep != null && ParentStep.ExecuteResult))
                                        {
                                            break;
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        SaveExceptionReport(ex);
                                    }
                                }
                            }
                            else
                            {
                                keyword.Invoke(DataList, _lparams);
                            }
                            break;
                        }
                }
                //keyword[SubKeyword].Invoke(DataList, _lparams);
            }
            catch (System.Exception ex)
            {
                SaveExceptionReport(ex);
            }
        }

        private Keyword GetKeyword()
        {
            BusinessMethodAttribute bma = new BusinessMethodAttribute();
            bma.Keywords = Keyword;
            bma.SubKeyword = SubKeyword;
            bma.TargetPorject = TargetProject;
            try
            {
                return DataFactory.GetKeyword(bma);
            }
            catch
            {
                try
                {
                    return DataFactory.GetDefaultKeyword(bma.Keywords);
                }
                catch
                {
                    try
                    {
                        return DataFactory.GetFirstKeyword(bma.Keywords);
                    }
                    catch
                    {
                        return new Data.Keyword(TestCasePool.GetTestCase(bma.Keywords), this);
                    }
                }
            }
        }

        private bool CheckFilter()
        {
            universal flag = DataFactory.Compare(FilterKeyword, _ofKeyword);
            if(!flag)
            {
                LogManager.DebugFormat("Keyword not equal : ", flag.ToString());
                return false;
            }

            flag = DataFactory.Compare(FilterSubKeyword, _ofSubKeyword);
            if (!flag)
            {
                LogManager.DebugFormat("SubKeyword not equal : ", flag.ToString());
                return false;
            }
            flag = DataFactory.Compare(FilterTargetProject, _ofTargetProject);
            if (!flag)
            {
                LogManager.DebugFormat("TargetProject not equal : ", flag.ToString());
                return false;
            }

            LogManager.Debug("=========================Filter Check Success");
            return true;
        }

        private void SaveExceptionReport(System.Exception ex)
        {
            while (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
            ReportDetail rd = new ReportDetail();
            rd.SetResult(false);
            rd.Set("name", this.Name);
            rd.Set("state", "F");
            LogManager.Error(string.Format("Execute [{1}, {0}] exception!", this.Keyword, this.Name));
            rd.Set("expInfo", ex.Message);
            LogManager.Error(ex);
            if (ex.InnerException != null)
            {
                rd.Set("innerExpInfo", ex.InnerException.Message);
                LogManager.Error(ex.InnerException);
            }
            else
            {
                rd.Set("innerExpInfo", "");
            }
            TestCasePool.SetDetailColumnName("名称", "name");
            TestCasePool.SetDetailColumnName("状态", "state", true);
            TestCasePool.SetDetailColumnName("异常", "expInfo");
            TestCasePool.SetDetailColumnName("内部异常", "innerExpInfo");
            TestCasePool.SetReportDetail(rd);
        }

        private void Check()
        {
            List<string> bhlist = DataFactory.ParseBH(DataList);
            List<string> targetlist = DataFactory.ParseBH(TargetDataList);
            TestCasePool.SetDetailColumnName("名称", "name");
            TestCasePool.SetDetailColumnName("状态", "state", true);
            TestCasePool.SetDetailColumnName("详细信息", "msg");
            foreach (string bh in bhlist)
            {
                CheckPoint cp = TestCasePool.GetCheckPoint(bh, TargetProject);
                if (cp == null)
                {
                    LogManager.Warning(string.Format("未找到编号{0}对应数据", bh));
                    continue;
                }
                if (targetlist.Count > 0)
                {
                    foreach (string target in targetlist)
                    {
                        cp.DataBh = target;
                        cp.Check();
                    }
                }
                else
                {
                    cp.Check();
                }
            }
        }

        #region IFormatData
        public override IFormatData FromXml(XmlNode doc, Dictionary<string, string> conditions)
        {
            string conditionString = string.Empty;
            if (conditions == null || conditions.Count < 1)
            {
                conditionString = "//Step";
            }
            else
            {
                List<string> keys = conditions.Keys.ToList<string>();
                foreach (string key in keys)
                {
                    if (string.IsNullOrEmpty(conditionString))
                    {
                        conditionString = string.Format("//Step[{0}='{1}']", key, conditions[key]);
                    }
                    else
                    {
                        conditionString = string.Format("{0} and {1}='{2}']", conditionString.TrimEnd(']'), key, conditions[key]);
                    }
                }
            }


            Type objType = typeof(Step);

            XmlNode stepNode = doc.SelectSingleNode(conditionString);
            if (stepNode == null)
            {
                return null;
            }

            Step stepObject = DataFactory.XmlToObject<Step>(stepNode);

            if (stepNode.Attributes != null && stepNode.Attributes.Count > 0)
            {
                foreach (XmlAttribute attr in stepNode.Attributes)
                {
                    if (string.Equals(attr.Name, "id", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (string.Equals(attr.Name, "jmpf", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (typeof(Step).GetProperty(attr.Name) == null)
                    {
                        stepObject.configValue.Add(attr.Name, attr.Value);
                    }
                }
            }

            return (IFormatData)stepObject;
        }

        public override string ToXml()
        {
            string xml = string.Empty;
            return xml;
        }

        public override void Init()
        {
            _ofKeyword = _keyword == null ? "" : _keyword;
            _ofSubKeyword = _subKeyword == null ? "" : _subKeyword;
            _ofTargetProject = _targetProject == null ? "" : _targetProject;

            if (_lparams == null)
            {
                _params = new Dictionary<string, string>();
                _lparams = new Dictionary<string, string>();
            }

            if (_lparams.ContainsKey("databhs"))
            {
                _data = SetDefault(_data, _lparams["databhs"]);
            }
            else
            {
                _params.Add("DATABHS", _data);
                _lparams.Add("databhs", _data);
            }

            if (_lparams.ContainsKey("mode"))
            {
                _targetProject = SetDefault(_targetProject, _lparams["mode"]);
                _subKeyword = SetDefault(_subKeyword, _lparams["mode"]);
            }
            else
            {
                if (string.IsNullOrEmpty(_targetProject))
                {
                    _targetProject = Config.TargetProjectName;
                    _params.Add("MODE", _subKeyword);
                    _lparams.Add("mode", _subKeyword);
                }
                else
                {
                    _params.Add("MODE", _targetProject);
                    _lparams.Add("mode", _targetProject);
                }
            }
        }
        #endregion
    }
}
