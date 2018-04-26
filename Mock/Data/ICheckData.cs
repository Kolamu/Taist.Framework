namespace Mock.Data
{
    using System.Xml;
    public interface ICheckData
    {
        universal Check(Cache cache);
        void Writeback(XmlNode node);
    }
}
