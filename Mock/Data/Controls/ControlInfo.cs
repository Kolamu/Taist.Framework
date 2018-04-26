namespace Mock.Data
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;

    using Mock.Data.Exception;
    public class ControlInfo
    {
        private string filePath = null;
        private List<string> _nameList = null;
        private Dictionary<string, XmlDocument> docDic = new Dictionary<string, XmlDocument>();
        private object libLock = new object();
        private static ControlInfo controlInfo = null;

        private ControlInfo() : this(Path.Combine(Config.WorkingDirectory, "Lib\\Controls.dll")) { }

        private ControlInfo(string filePath)
        {
            this.filePath = filePath;
            if (!File.Exists(filePath))
            {
                throw new CanNotFindFileException(filePath);
            }
            _nameList = new List<string>();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int n = int.Parse(DataFactory.Base64Decrypt(br.ReadString()));
                    for (int i = 0; i < n; i++)
                    {
                        string tmp = DataFactory.Base64Decrypt(br.ReadString());
                        _nameList.Add(tmp);
                    }
                    br.Close();
                }
                fs.Close();
            }
        }

        public static ControlInfo getInstance(string filePath = null)
        {
            if (controlInfo == null)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    controlInfo = new ControlInfo();
                }
                else
                {
                    controlInfo = new ControlInfo(filePath);
                }
            }
            return controlInfo;
        }

        public List<string> SectionNameList
        {
            get
            {
                return _nameList;
            }
        }

        public XmlDocument this[string name]
        {
            get
            {
                if (docDic.ContainsKey(name))
                {
                    return docDic[name];
                }
                if (!_nameList.Contains(name))
                {
                    return null;
                }
                XmlDocument doc = null;
                lock (libLock)
                {
                    for (int j = 0; j < Config.RedoCount; j++)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                using (BinaryReader br = new BinaryReader(fs))
                                {
                                    int n = _nameList.Count + 1 + _nameList.IndexOf(name);
                                    for (int i = 0; i < n; i++)
                                    {
                                        br.ReadString();
                                    }
                                    doc = XmlFactory.LoadXml(DataFactory.Base64Decrypt(br.ReadString()));
                                    br.Close();
                                }
                                fs.Close();
                            }
                            docDic.Add(name, doc);
                            break;
                        }
                        catch
                        {
                            Robot.Recess(100);
                        }
                    }
                }
                return doc;
            }
        }

    }
}
