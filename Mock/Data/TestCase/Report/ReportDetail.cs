namespace Mock.Data
{
    using System.Xml;
    using System.Collections.Generic;

    public class ReportDetail
    {
        private Dictionary<string, string> _detailData = null;
        private string _res = "";

        /// <summary>nz
        /// 执行结果
        /// </summary>
        public bool Result
        {
            get
            {
                try
                {
                    return bool.Parse(_res);
                }
                catch
                {
                    throw new Mock.Data.Exception.NotSetAffirmativelySettingItemException("Result");
                }
            }
        }

        /// <summary>
        /// 设置执行结果
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(bool result)
        {
            _res = result.ToString();
        }

        /// <summary>
        /// 设置结果信息
        /// </summary>
        /// <param name="detailName">报告明细的名称，这通常为TestCasePool.SetDetailColumnName中的tagName参数</param>
        /// <param name="detailValue">报告的明细信息</param>
        public void Set(string detailName, string detailValue)
        {
            if (_detailData == null)
            {
                _detailData = new Dictionary<string, string>();
            }
            if (_detailData.ContainsKey(detailName))
            {
                _detailData[detailName] = detailValue;
            }
            else
            {
                _detailData.Add(detailName, detailValue);
            }
        }

        /// <summary>
        /// 设置结果信息
        /// </summary>
        /// <param name="detailName">报告明细的名称，这通常为TestCasePool.SetDetailColumnName中的tagName参数</param>
        /// <param name="detailValue">报告的明细信息</param>
        public void Set(string detailName, ReportResult detailValue)
        {
            if (_detailData == null)
            {
                _detailData = new Dictionary<string, string>();
            }
            if (_detailData.ContainsKey(detailName))
            {
                _detailData[detailName] = GetReportResultString(detailValue);
            }
            else
            {
                _detailData.Add(detailName, GetReportResultString(detailValue));
            }
        }

        private string GetReportResultString(ReportResult result)
        {
            switch (result)
            {
                case ReportResult.BLOCKED:
                    {
                        return "B";
                    }
                case ReportResult.FAILED:
                    {
                        return "F";
                    }
                case ReportResult.SUCCESS:
                    {
                        return "S";
                    }
                case ReportResult.WATING:
                    {
                        return "W";
                    }
                case ReportResult.RUNNING:
                    {
                        return "I";
                    }
                default:
                    {
                        return result.ToString();
                    }
            }
        }

        /// <summary>
        /// 获取明细信息
        /// </summary>
        /// <param name="detailName">报告明细的名称，这通常为TestCasePool.SetDetailColumnName中的tagName参数</param>
        /// <returns>detailValue</returns>
        internal string Get(string detailName)
        {
            if (_detailData == null || !_detailData.ContainsKey(detailName))
            {
                return string.Empty;
            }
            else
            {
                return _detailData[detailName];
            }
        }

        public override string ToString()
        {
            XmlNode xn = XmlFactory.LoadXml("<cz></cz>").DocumentElement;
            foreach(KeyValuePair<string,string> kv in _detailData)
            {
                XmlNode x = xn.OwnerDocument.CreateElement(kv.Key);
                x.InnerText = kv.Value;
                xn.AppendChild(x);
            }

            return xn.OuterXml;
        }

        public Dictionary<string, string> getProperty()
        {
            return _detailData;
        }
    }
}
