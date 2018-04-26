namespace Mock.Data
{
    using System;
    using System.Xml;
    using System.Collections.Generic;

    public class TestDataList<T> : List<T>
    {
        public TestDataList() { }
        public TestDataList(string listData)
        {
            if (string.IsNullOrEmpty(listData))
            {
                return;
            }
            Type t = typeof(T);
            if (t.IsPrimitive)
            {
                List<string> bhlist = DataFactory.ParseBH(listData);
                this.AddRange(bhlist.ConvertAll(x => (T)Convert.ChangeType(x, t)));
            }
            else
            {
                try
                {

                    XmlDocument doc = XmlFactory.LoadXml(listData);
                    XmlNodeList bhNodeList = doc.SelectNodes("//Bh");
                    foreach (XmlNode bhNode in bhNodeList)
                    {
                        T obj = DataFactory.GetData<T>(bhNode.ParentNode);
                        Add(obj);
                    }
                }
                catch
                {
                    List<string> bhList = DataFactory.ParseBH(listData);
                    foreach (string bh in bhList)
                    {
                        T obj = TestCasePool.GetData<T>(bh);
                        Add(obj);
                    }
                }
            }
        }

        public static implicit operator TestDataList<T>(string listData)
        {   
            TestDataList<T> dataList = new TestDataList<T>(listData);
            return dataList;
        }
    }
}
