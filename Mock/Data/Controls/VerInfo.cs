namespace Mock.Data
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml;
    using System.Windows.Automation;

    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    /// <summary>
    /// 表示对象库中描述版本信息结构对象
    /// </summary>
    public class VerInfo : IFormatData
    {
        /// <summary>
        /// 构造版本信息的新实例
        /// </summary>
        public VerInfo()
        {
            VerId = string.Empty;
            StartDate = string.Empty;
            EndDate = string.Empty;
            Bh = string.Empty;
        }

        /// <summary>
        /// 版本号
        /// </summary>
        [FieldProperty("VerId", false)]
        public string VerId
        {
            get;
            set;
        }

        /// <summary>
        /// 开始日期
        /// </summary>
        [FieldProperty("StartDate", false)]
        public string StartDate
        {
            get;
            set;
        }

        /// <summary>
        /// 结束日期
        /// </summary>
        [FieldProperty("EndDate", false)]
        public string EndDate
        {
            get;
            set;
        }

        /// <summary>
        /// 编号，此处等同于VerId
        /// </summary>
        [FieldProperty("Bh", false)]
        public override string Bh
        {
            get
            {
                return VerId;
            }
            set
            {
                VerId = value;
            }
        }

        /// <summary>
        /// 从格式文档中获取数据
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public override IFormatData FromXml(XmlNode doc, Dictionary<string, string> condition)
        {
            if (condition == null || condition.Count < 1)
            {
                return null;
            }
            string conditionString = DataFactory.GetXmlConditionString("VerInfo", condition);
            XmlNodeList verNodeList = doc.SelectNodes(conditionString);
            if (verNodeList == null)
            {
                throw new CanNotFindDataException(string.Format("VerInfo {0}", conditionString));
            }
            else if (verNodeList.Count > 1)
            {
                throw new NotUniqueDataException(string.Format("VerInfo {0}", conditionString));
            }

            XmlNode verNode = verNodeList[0];
            VerInfo einfo = DataFactory.XmlToObject<VerInfo>(verNode);
            return einfo;
        }

        public override void Init()
        {
        }
    }
}
