namespace Mock
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;

    using Mock.Data;
    using Mock.Data.Exception;
    using Mock.Tools.Exception;
    /// <summary>
    /// 表示测试配置信息
    /// </summary>
    public sealed class Config
    {
        private static string configPath = null;
        static Config()
        {
            try
            {
                string dirPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                configPath = Path.Combine(dirPath, "taist.config");
                if (!File.Exists(configPath))
                {
                    workingDirectory = dirPath;
                    return;
                }
                
                originConfig = XmlFactory.LoadXml(configPath);

                XmlNode node = originConfig.SelectSingleNode("//WorkingDirectory");
                if (node == null)
                {
                    workingDirectory = dirPath;
                }
                else
                {
                    workingDirectory = node.InnerText;
                }

                OnCaseCreate("default");

                CaseManager.CaseCreate += OnCaseCreate;
                CaseManager.CaseRemoved += OnCaseRemoved;
            }
            catch (Exception ex)
            {
                LogManager.Atom("Config initilize error: {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private static string workingDirectory = null;
        /// <summary>
        /// 主程序所在文件夹绝对路径
        /// </summary>
        public static string WorkingDirectory
        {
            get
            {
                return workingDirectory;
            }
        }

        /// <summary>
        /// 修复验证数据
        /// </summary>
        public static bool RepairCheckPoint
        {
            get
            {
                try
                {
                    return bool.Parse(getConfigValue("RepairCheckPoint"));
                }
                catch (CanNotFindNodeException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 并发执行数量
        /// </summary>
        public static int ConcurrencyCount
        {
            get
            {
                try
                {
                    return int.Parse(getConfigValue("ConcurrencyCount"));
                }
                catch (CanNotFindNodeException)
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// 软件最大内存占用
        /// </summary>
        public static int MaxMemoryUsage
        {
            get
            {
                try
                {
                    string val = getConfigValue("MaxMemoryUsage").ToUpper();
                    try
                    {
                        return int.Parse(val) / 1024;
                    }
                    catch
                    {
                        if (val.EndsWith("B"))
                        {
                            val = val.Substring(0, val.Length - 1);
                        }
                        if (val.EndsWith("K"))
                        {
                            val = val.Substring(0, val.Length - 1);
                            return int.Parse(val);
                        }
                        if (val.EndsWith("M"))
                        {
                            val = val.Substring(0, val.Length - 1);
                            return int.Parse(val) * 1024;
                        }
                        if (val.EndsWith("G"))
                        {
                            val = val.Substring(0, val.Length - 1);
                            return int.Parse(val) * 1024 * 1024;
                        }
                        if (val.EndsWith("T"))
                        {
                            val = val.Substring(0, val.Length - 1);
                            return int.Parse(val) * 1024 * 1024 * 1024;
                        }
                        return int.Parse(val) / 1024;
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch
                {
                    return 1048576;
                }
            }
        }

        /// <summary>
        /// 表示是否当前状态是否为debug状态
        /// </summary>
        public static bool Debug
        {
            get
            {
                try
                {
                    return bool.Parse(getConfigValue("Debug"));
                }
                catch (CanNotFindNodeException)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 测试目标名称
        /// </summary>
        public static string TargetProjectName
        {
            get
            {
                try
                {
                    return getConfigValue("TargetProjectName");
                }
                catch (CanNotFindNodeException)
                {
                    return "单机版组件接口";
                }
            }
        }

        /// <summary>
        /// 要测试的开票软件版本的日期
        /// </summary>
        public static DateTime SoftwareVersionDate
        {
            get
            {
                return DateTime.ParseExact(getConfigValue("SoftwareVersionDate"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                setConfigValue("SoftwareVersionDate", value.ToString("yyyyMMdd"));
            }
        }

        /// <summary>
        /// 超时时间，通常设置为本次测试最大的等待时间
        /// </summary>
        public static int TimeOut
        {
            get
            {
                try
                {
                    return int.Parse(getConfigValue("TimeOut")) * 1000;
                }
                catch (CanNotFindNodeException)
                {
                    return 3000;
                }
            }
        }

        /// <summary>
        /// 单次操作最大失败次数
        /// </summary>
        public static int RedoCount
        {
            get
            {
                try
                {
                    return int.Parse(getConfigValue("RedoCount"));
                }
                catch (CanNotFindNodeException)
                {
                    return 2;
                }
            }
        }

        /// <summary>
        /// 一条用例的最短执行时间
        /// </summary>
        public static double MinExecutionTime
        {
            get
            {
                try
                {
                    return double.Parse(getConfigValue("MinExecutionTime")) * 1000;
                }
                catch (CanNotFindNodeException)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 获取测试软件的进程名称
        /// </summary>
        public static string SoftwareProcessName
        {
            get
            {
                return getConfigValue("SoftwareProcessName").ToUpper();
            }
            set
            {
                setConfigValue("SoftwareProcessName", value);
            }
        }

        public static string User
        {
            get
            {
                return getConfigValue("User");
            }
        }

        public static string Password
        {
            get
            {
                return getConfigValue("Password");
            }
        }

        /// <summary>
        /// 获取报警窗口的AutomaitionId属性
        /// </summary>
        public static List<string> WarningWindowList
        {
            get
            {
                string idsString = getConfigValue("WarningWindowList");
                string []idsArray = idsString.Split(',');
                List<string> idsList = new List<string>();
                foreach (string tmp in idsArray)
                {
                    if (tmp != "" && tmp != null)
                    {
                        idsList.Add(tmp);
                    }
                }
                return idsList;
            }
        }

        /// <summary>
        /// 获取虚拟打印的图片保存位置
        /// </summary>
        public static string PrintImageDirectory
        {
            get
            {
                return getConfigValue("PrintImageDirectory");
            }
        }

        /// <summary>
        /// 修改时钟服务器时间
        /// </summary>
        public static string TaistServer
        {
            get
            {
                try
                {
                    return getConfigValue("TaistServer");
                }
                catch (CanNotFindNodeException)
                {
                    return "192.168.25.181";
                }
            }
        }

        /// <summary>
        /// 对象库路径
        /// </summary>
        public static string ControlLibraryPath
        {
            get
            {
                try
                {
                    return getConfigValue("ControlLibraryPath");
                }
                catch (CanNotFindNodeException)
                {
                    return Path.Combine(Config.WorkingDirectory, "Lib\\Controls.dll");
                }
            }
        }

        /// <summary>
        /// 获取配置项的值
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <returns>配置值</returns>
        public static string getConfigValue(string name)
        {
            XmlDocument config = originConfig;
            try
            {
                string id = Mock.Data.CaseManager.GetIdentification();
                config = stepConfig[id];
            }
            catch
            {
                config = originConfig;
            }

            if (config == null)
            {
                try
                {
                    new XmlDocument().Load(configPath);
                }
                catch (Exception ex)
                {
                    throw new XmlFormatErrorException(string.Format("{0} -> please check file named taist.config", ex.Message));
                }
                throw new XmlFormatErrorException("please check file named taist.config");
            }

            XmlNode node = config.SelectSingleNode(string.Format("//{0}", name));
            if (node == null)
            {
                throw new CanNotFindNodeException("taist.config", name);
            }
            return node.InnerText;
        }

        //private static readonly object clock = new object();
        //private static readonly object slock = new object();
        //private static readonly object ulock = new object();

        private static Dictionary<string, XmlDocument> caseConfig = new Dictionary<string,XmlDocument>();
        private static Dictionary<string, XmlDocument> stepConfig = new Dictionary<string,XmlDocument>();
        private static XmlDocument originConfig = null;
        //private static Dictionary<string, XmlDocument> currentConfig = new Dictionary<string,XmlDocument>();

        private static void OnCaseRemoved(string identification)
        {
            caseConfig.Remove(identification);
            stepConfig.Remove(identification);
            //currentConfig.Remove(identification);
        }

        private static void OnCaseCreate(string identification)
        {
            if (originConfig == null)
            {
                caseConfig.Add(identification, null);
                stepConfig.Add(identification, null);
                //currentConfig.Add(identification, null);
            }
            else
            {
                caseConfig.Add(identification, XmlFactory.LoadXml(originConfig.OuterXml));
                stepConfig.Add(identification, XmlFactory.LoadXml(originConfig.OuterXml));
                //currentConfig.Add(identification, DataFactory.LoadXml(originConfig.OuterXml));
            }
        }

        internal static void resetStepConfig()
        {
            if (caseConfig[CaseId] == null)
            {
                stepConfig[CaseId] = null;
            }
            else
            {
                stepConfig[CaseId] = XmlFactory.LoadXml(caseConfig[CaseId].OuterXml);
            }
        }

        internal static void setCaseConfig(string name, string value)
        {
            if (caseConfig[CaseId] == null)
            {
                throw new CanNotFindNodeException("taist.config", name);
            }
            XmlNode node = caseConfig[CaseId].SelectSingleNode(string.Format("//{0}", name));
            if (node == null)
            {
                throw new CanNotFindNodeException("taist.config", name);
            }
            node.InnerText = value;
        }

        internal static void setStepConfig(string name, string value)
        {
            if (stepConfig[CaseId] == null)
            {
                throw new CanNotFindNodeException("taist.config", name);
            }
            XmlNode node = stepConfig[CaseId].SelectSingleNode(string.Format("//{0}", name));
            if (node == null)
            {
                throw new CanNotFindNodeException("taist.config", name);
            }
            node.InnerText = value;
        }

        /// <summary>
        /// 设置配置项的值
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <param name="value">配置项的值</param>
        public static void setConfigValue(string name, string value)
        {
            XmlDocument config = originConfig;
            try
            {
                string id = Mock.Data.CaseManager.GetIdentification();
                config = stepConfig[id];
            }
            catch
            {
                config = originConfig;
            }

            if (originConfig == null)
            {
                throw new CanNotFindNodeException("taist.config", name);
            }
            XmlNode node = originConfig.SelectSingleNode(string.Format("//{0}", name));
            if (node == null)
            {
                throw new CanNotFindNodeException("taist.config", name);
            }

            node.InnerText = value;
            //originConfig.Save(configPath);
            if (config == null)
            {
                throw new CanNotFindNodeException("taist.config", name);
            }
            node = config.SelectSingleNode(string.Format("//{0}", name));
            node.InnerText = value;
            node = null;
        }

        private static string CaseId
        {
            get
            {
                try
                {
                    return Mock.Data.CaseManager.GetIdentification();
                }
                catch
                {
                    return "default";
                }
            }
        }
    }
}
