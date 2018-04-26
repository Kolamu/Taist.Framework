namespace Mock.Data
{
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Collections.Generic;

    using Mock.Data.Exception;
    public class DataDesigner
    {
        public static void XmlToClass(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new CanNotFindFileException(path);
            }

            string[] fileList = Directory.GetFiles(path);
            foreach (string fileName in fileList)
            {
                CreateClass(fileName);
            }
        }

        private static void CreateClass(string fileName)
        {
            XmlDocument doc = XmlFactory.LoadXml(fileName);

            XmlNode root = doc.DocumentElement;

            string className = root.Name;//Path.GetFileNameWithoutExtension(fileName);
            string classPath = Path.Combine(Config.WorkingDirectory, "Class", className + ".cs");
            string convertPath = Path.Combine(Config.WorkingDirectory, "Class", className + "Convert.xml");
            if (File.Exists(classPath))
            {
                File.Delete(classPath);
            }
            FileFactory.CreateFile(classPath);
            StringBuilder classContent = new StringBuilder();

            #region 类体
            classContent.AppendLine("namespace DataStore");
            classContent.AppendLine("{");

            classContent.AppendLine("\tusing System.Xml;");
            classContent.AppendLine("\tusing System.Collections.Generic;");
            classContent.AppendLine();
            classContent.AppendLine("\tusing Mock;");
            classContent.AppendLine("\tusing Mock.Data;");
            classContent.AppendLine("\tusing Mock.Data.Exception;");
            classContent.AppendLine("\tusing Mock.Data.Attributes;");

            classContent.AppendLine("\t/// <summary>");
            classContent.AppendLine(string.Format("\t/// {0}", className));
            classContent.AppendLine("\t/// </summary>");
            classContent.AppendLine(string.Format("\tpublic class {0} : IFormatData", className));
            classContent.AppendLine("\t{");

            classContent.AppendLine("\t\t#region Parent Functions");
            classContent.AppendLine("\t\tpublic override IFormatData FromXml(XmlNode doc, Dictionary<string, string> conditions)");
            classContent.AppendLine("\t\t{");
            classContent.AppendLine("\t\t\t//Do Xml To Object");
            classContent.AppendLine("\t\t\t\treturn this;");
            classContent.AppendLine("\t\t}");

            classContent.AppendLine("\t\tpublic override string ToXml()");
            classContent.AppendLine("\t\t{");
            classContent.AppendLine("\t\t\t//Do Object To Xml");
            classContent.AppendLine("\t\t\t\treturn this;");
            classContent.AppendLine("\t\t}");

            classContent.AppendLine("\t\tpublic override void Init()");
            classContent.AppendLine("\t\t{");
            classContent.AppendLine("\t\t\t//Do Init");
            classContent.AppendLine("\t\t\tbase.Init();");
            classContent.AppendLine("\t\t}");
            classContent.AppendLine("\t\t#endregion");
            #endregion

            #region 转换文件结构
            XmlDocument convertDoc = XmlFactory.LoadXml(string.Format("<?xml version=\"1.0\" encoding=\"GBK\" ?><ConvertInformation name=\"\" type=\"Class\"><Type name1=\"{0}\" name2=\"\"/></ConvertInformation>", className));
            XmlNode infoNode = convertDoc.SelectSingleNode("//ConvertInformation");
            #endregion

            foreach (XmlNode xn in root.ChildNodes)
            {
                if (xn is XmlComment) continue;
                string s = xn.Name;

                #region 生成属性
                classContent.AppendLine(string.Format("\t\tprivate string _{0} = null;", s.ToLower()));
                classContent.AppendLine("\t\t/// <summary>");
                classContent.AppendLine(string.Format("\t\t/// {0}", s.ToUpper()));
                classContent.AppendLine("\t\t/// </summary>");
                classContent.AppendLine(string.Format("\t\tpublic string {0}", s));
                classContent.AppendLine("\t\t{");
                classContent.AppendLine("\t\t\tget");
                classContent.AppendLine("\t\t\t{");
                classContent.AppendLine(string.Format("\t\t\t\treturn _{0};", s.ToLower()));
                classContent.AppendLine("\t\t\t}");
                classContent.AppendLine("\t\t\tset");
                classContent.AppendLine("\t\t\t{");
                classContent.AppendLine(string.Format("\t\t\t\t_{0} = value;", s.ToLower()));
                classContent.AppendLine("\t\t\t}");
                classContent.AppendLine("\t\t}");
                classContent.AppendLine("");
                #endregion

                #region 生成转换文件
                XmlNode propertyNode = convertDoc.CreateElement("Property");
                propertyNode.InnerXml = null;
                infoNode.AppendChild(propertyNode);
                XmlAttribute xa = convertDoc.CreateAttribute("name1");
                xa.Value = "";
                propertyNode.Attributes.Append(xa);
                xa = convertDoc.CreateAttribute("name2");
                xa.Value = s;
                propertyNode.Attributes.Append(xa);
                #endregion
            }

            classContent.AppendLine("\t}");
            classContent.AppendLine("}");

            using (StreamWriter sw = new StreamWriter(classPath, false, Encoding.Default))
            {
                sw.Write(classContent.ToString());
                sw.Flush();
                sw.Close();
            }
            convertDoc.Save(convertPath);
        }
    }
}
