namespace Mock.Data
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;

    using Mock.Data.Attributes;
    using Mock.Data.Exception;
    public class StepReport
    {
        public string Name
        {
            get
            {
                return _bus.name;
            }
            set
            {
                _bus.name = value;
            }
        }

        public string Next
        {
            get;
            set;
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
                            _bus.state = "I";
                            _step.ExecuteResult = true;
                            break;
                        }
                    case ReportResult.BLOCKED:
                        {
                            _bus.state = "B";
                            break;
                        }
                    case ReportResult.FAILED:
                        {
                            _bus.state = "F";
                            _step.ExecuteResult = false;
                            break;
                        }
                    case ReportResult.SUCCESS:
                        {
                            _bus.state = "S";
                            _step.ExecuteResult = true;
                            break;
                        }
                    case ReportResult.WATING:
                        {
                            _bus.state = "W";
                            _step.ExecuteResult = true;
                            break;
                        }
                }
            }
        }

        public void Fill()
        {
            _bus.startDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                Result = ReportResult.RUNNING;
                _id = CaseManager.GetIdentification();
                CaseManager.SetCaseReportDetailHandler(_id, AddDetail);
                CaseManager.SetCaseReportHeaderHandler(_id, AddDetailHeader);
                CaseManager.SetCaseReportClearHandler(_id, Clear);
                _step.Execute();
                if (Result == ReportResult.RUNNING)
                {
                    Result = ReportResult.SUCCESS;
                }
            }
            finally
            {
                _bus.endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        internal void Break(string message)
        {
            ReportDetail rd = new ReportDetail();
            rd.SetResult(false);
            rd.Set("name", _step.Name);
            rd.Set("state", ReportResult.FAILED);
            rd.Set("info", message);
            
            AddDetailHeader("名称", "name", false);
            AddDetailHeader("状态", "state", true);
            AddDetailHeader("错误信息", "info", false);

            AddDetail(rd);
        }

        public void Save()
        {
            SaveHead();
            SaveDetail();
        }

        private void SaveHead()
        {
            XmlDocument headDoc = XmlFactory.LoadXml(XmlFactory.XmlRootString.Replace("Data", "data"));
            XmlNode dataNode = headDoc.SelectSingleNode("//data");
            foreach(head h in _detailHeaderList)
            {
                XmlNode hNode = XmlFactory.LoadXml(DataFactory.ObjectToXml(h, false)).DocumentElement;
                XmlNode nNode = headDoc.ImportNode(hNode, true);
                dataNode.AppendChild(nNode);
            }
            headDoc.Save(Path.Combine(Config.WorkingDirectory, "Report", "head" + _caseIndex + "_" + _bus.bh + ".xml"));
        }

        private void SaveDetail()
        {
            XmlDocument headDoc = XmlFactory.LoadXml(XmlFactory.XmlRootString.Replace("Data", "data"));
            XmlNode dataNode = headDoc.SelectSingleNode("//data");
            foreach (ReportDetail d in _detailList)
            {
                XmlNode hNode = XmlFactory.LoadXml(DataFactory.ObjectToXml(d, false)).DocumentElement;
                XmlNode nNode = headDoc.ImportNode(hNode, true);
                dataNode.AppendChild(nNode);
            }
            headDoc.Save(Path.Combine(Config.WorkingDirectory, "Report", "detail" + _caseIndex + "_" + _bus.bh + ".xml"));
        }

        private void AddDetail(ReportDetail detail)
        {
            _detailList.Add(detail);
            if (!detail.Result)
            {
                Result = ReportResult.FAILED;
            }
        }

        private void AddDetailHeader(string displayName, string tagName, bool state)
        {
            if (_tagList.Contains(tagName))
            {
                LogManager.Warning("The tag named " + tagName +" has been added");
                return;
                //throw new NotUniqueDataException(tagName);
            }

            _tagList.Add(tagName);
            if (_displayList.Contains(displayName))
            {
                foreach (head header in _detailHeaderList)
                {
                    if (string.Equals(displayName, header.name))
                    {
                        header.tag = tagName;
                        header.isState = state;
                        break;
                    }
                }
            }
            else
            {
                head dh = new head();
                dh.name = displayName;
                dh.tag = tagName;
                dh.isState = state;
                _detailHeaderList.Add(dh);
                _displayList.Add(displayName);
            }
        }

        private void Clear()
        {
            _displayList.Clear();
            _tagList.Clear();
            _detailList.Clear();
            _detailHeaderList.Clear();
        }

        private string _id = null;
        private List<ReportDetail> _detailList = new List<ReportDetail>();
        private List<head> _detailHeaderList = new List<head>();
        private List<string> _displayList = new List<string>();
        private List<string> _tagList = new List<string>();
        private Step _step = null;
        private bus _bus = null;
        private string _caseIndex = null;

        public StepReport(Step step, int index, string caseIndex, XmlNode node)
        {
            _step = step;
            this._caseIndex = caseIndex;
            _bus = new bus();
            if (node == null)
            {
                node = XmlFactory.LoadXml("<bus></bus>").DocumentElement;
            }
            _bus.Node = node;
            _bus.bh = index.ToString();
            _bus.name = _step.Name;
            Result = ReportResult.WATING;
        }

        private class head
        {
            public string name { get; set; }
            public string tag { get; set; }
            public bool isState { get; set; }
        }

        private class bus
        {
            private XmlNode busNode = null;

            private XmlNode bhNode = null;
            private XmlNode nameNode = null;
            private XmlNode stateNode = null;
            private XmlNode startDateNode = null;
            private XmlNode endDateNode = null;

            internal XmlNode Node
            {
                get
                {
                    return busNode;
                }
                set
                {
                    busNode = value;
                    InitNode();
                }
            }

            private void InitNode()
            {
                bhNode = busNode.SelectSingleNode("bh");
                if (bhNode == null)
                {
                    bhNode = busNode.OwnerDocument.CreateElement("bh");
                }

                nameNode = busNode.SelectSingleNode("name");
                if (nameNode == null)
                {
                    nameNode = busNode.OwnerDocument.CreateElement("name");
                    busNode.AppendChild(nameNode);
                }

                stateNode = busNode.SelectSingleNode("state");
                if (stateNode == null)
                {
                    stateNode = busNode.OwnerDocument.CreateElement("state");
                    busNode.AppendChild(stateNode);
                    stateNode.InnerText = "W";
                }

                startDateNode = busNode.SelectSingleNode("startDate");
                if (startDateNode == null)
                {
                    startDateNode = busNode.OwnerDocument.CreateElement("startDate");
                    busNode.AppendChild(startDateNode);
                    startDateNode.InnerText = "";
                }

                endDateNode = busNode.SelectSingleNode("endDate");
                if (endDateNode == null)
                {
                    endDateNode = busNode.OwnerDocument.CreateElement("endDate");
                    busNode.AppendChild(endDateNode);
                    endDateNode.InnerText = "";
                }
            }

            public string bh { get { return bhNode.InnerText; } set { bhNode.InnerText = value; } }
            public string name { get { return nameNode.InnerText; } set { nameNode.InnerText = value; } }
            public string state { get { return stateNode.InnerText; } set { stateNode.InnerText = value; } }
            public string startDate { get { return startDateNode.InnerText; } set { startDateNode.InnerText = value; } }
            public string endDate { get { return endDateNode.InnerText; } set { endDateNode.InnerText = value; } }

        }
    }
}
