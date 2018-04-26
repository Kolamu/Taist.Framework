namespace Taist.Framework.Test
{
    using System.Xml;
    using Mock.Data;
    using Mock.Data.Exception;
    using Mock.Data.Attributes;
    [DataClass]
    public class TaistData : IFormatData
    {
        public bool State { get; set; }

        public override void Init()
        {
            if (!string.IsNullOrEmpty(RelateDataBh))
            {
                Cache cache = new Cache();
                cache.Bh = RelateDataBh;
                cache.Get();
                SetDefault(State, cache["State"]);
            }
        }

        public override IFormatData FromXml(System.Xml.XmlNode doc, System.Collections.Generic.Dictionary<string, string> condition)
        {
            string conditionString = DataFactory.GetXmlConditionString("TaistData", condition);
            if(doc == null) throw new CanNotFindDataException(conditionString);
            XmlNode node = doc.SelectSingleNode(conditionString);
            if (node == null)throw new CanNotFindDataException(conditionString);
            return DataFactory.XmlToObject<TaistData>(node);
        }

        public override string ToXml()
        {
            return DataFactory.ObjectToXml(this, false);
        }
    }
}
