namespace Mock.Tools.Controls
{
    using System;
    using System.IO;
    using System.CodeDom;
    using Microsoft.CSharp;
    using System.CodeDom.Compiler;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Diagnostics;
    //public delegate bool ComNotInstallEventHandler();
    public abstract class ComObject : IDisposable
    {
        //private string path = null;
        private string comName = null;
        private string progid = null;
        private string clsid = null;

        private AppDomain comDomain = null;
        private TransparentAgent agent = null;

        //protected event ComNotInstallEventHandler ComNotInstall = null;

        protected ComObject(string comName, string progid, string clsid)
        {
            this.comName = comName;
            this.progid = progid;
            this.clsid = clsid;

            InstallCom();
            //comDomain = AppDomain.CreateDomain(comName);

            //comDomain = AppDomain.CreateDomain(comName, AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
            agent = new TransparentAgent();//(TransparentAgent)comDomain.CreateInstance("Mock", "Mock.Tools.Controls.TransparentAgent", true, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null, null).Unwrap();
            agent.Create(progid, clsid, comName);


            //try
            //{
            //    RegistryKey root = Registry.ClassesRoot;

            //    if (Environment.Is64BitOperatingSystem)
            //    {
            //        RegistryKey type = root.OpenSubKey(@"Wow6432Node\CLSID\{7126812F-8D2A-11D6-9C69-00E04C103A76}\InprocServer32");
            //        path = System.IO.Path.GetDirectoryName((string)type.GetValue(null));
            //    }
            //    else
            //    {
            //        RegistryKey type = root.OpenSubKey(@"CLSID\{7126812F-8D2A-11D6-9C69-00E04C103A76}\InprocServer32");
            //        path = System.IO.Path.GetDirectoryName((string)type.GetValue(null));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log(ex.Message + "\n" + ex.StackTrace, NoteType.EXCEPTION);
            //}
        }

        private int TryCreate()
        {
            CodeDomProvider csharpCodeProvider = CodeDomProvider.CreateProvider("CSharp");

            //ICodeCompiler compiler = csharpCodeProvider.CreateCompiler();
            CSharpCodeProvider compiler = new CSharpCodeProvider();
            CompilerParameters param = new CompilerParameters();
            param.ReferencedAssemblies.Add("System.dll");
            param.GenerateExecutable = true;
            param.GenerateInMemory = false;
            string exePath = Path.Combine(Path.GetTempPath(), "tc.exe");
            param.OutputAssembly = exePath;

            CompilerResults result = compiler.CompileAssemblyFromSource(param, Code);

            if (result.Errors.HasErrors)
            {
                return 9;
            }

            Process p = new Process();
            p.StartInfo.FileName = exePath;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            int code = p.ExitCode;
            File.Delete(exePath);
            return code;
        }

        private string Code
        {
            get
            {
                System.Text.StringBuilder codeBuilder = new System.Text.StringBuilder();
                codeBuilder.Append("namespace tc                                                     ");
                codeBuilder.Append("{                                                                   ");
                codeBuilder.Append("    using System;                                                   ");
                codeBuilder.Append("    using System.Reflection;                                        ");
                codeBuilder.Append("    class Program                                                   ");
                codeBuilder.Append("    {                                                               ");
                codeBuilder.Append("        static int Main(string[] args)                              ");
                codeBuilder.Append("        {                                                           ");
                codeBuilder.Append(string.Format("string progid = {0};                                  ", progid));
                codeBuilder.Append(string.Format("string clsid = {0};                                   ", clsid));
                codeBuilder.Append("                                                                    ");
                codeBuilder.Append("            Type objType = null;                                    ");
                codeBuilder.Append("                                                                    ");
                codeBuilder.Append("            if (string.IsNullOrEmpty(clsid))                        ");
                codeBuilder.Append("            {                                                       ");
                codeBuilder.Append("                if (string.IsNullOrEmpty(progid))                   ");
                codeBuilder.Append("                {                                                   ");
                codeBuilder.Append("                    return 1;                                       ");
                codeBuilder.Append("                }                                                   ");
                codeBuilder.Append("                else                                                ");
                codeBuilder.Append("                {                                                   ");
                codeBuilder.Append("                    objType = Type.GetTypeFromProgID(progid);       ");
                codeBuilder.Append("                }                                                   ");
                codeBuilder.Append("            }                                                       ");
                codeBuilder.Append("            else                                                    ");
                codeBuilder.Append("            {                                                       ");
                codeBuilder.Append("                objType = Type.GetTypeFromCLSID(Guid.Parse(clsid)); ");
                codeBuilder.Append("            }                                                       ");
                codeBuilder.Append("                                                                    ");
                codeBuilder.Append("            if (objType == null)                                    ");
                codeBuilder.Append("            {                                                       ");
                codeBuilder.Append("                return 2;                                           ");
                codeBuilder.Append("            }                                                       ");
                codeBuilder.Append("                                                                    ");
                codeBuilder.Append("            try                                                     ");
                codeBuilder.Append("            {                                                       ");
                codeBuilder.Append("                Activator.CreateInstance(objType);                  ");
                codeBuilder.Append("            }                                                       ");
                codeBuilder.Append("            catch                                                   ");
                codeBuilder.Append("            {                                                       ");
                codeBuilder.Append("                return 3;                                           ");
                codeBuilder.Append("            }                                                       ");
                codeBuilder.Append("                                                                    ");
                codeBuilder.Append("            return 0;                                               ");
                codeBuilder.Append("        }                                                           ");
                codeBuilder.Append("    }                                                               ");
                codeBuilder.Append("}                                                                   ");
                return codeBuilder.ToString();
            }
        }

        internal virtual void InstallCom() { }

        internal virtual void Close() { }

        #region 调用函数
        /// <summary>
        /// 调用COM接口导出的方法
        /// </summary>
        /// <param name="fName">方法名称</param>
        /// <param name="args">参数数组</param>
        /// <returns></returns>
        protected object CallFunction(string fName, params object[] args)
        {

            if (agent == null)
            {
                throw new Exception("COM 对象已注销，不能执行该操作！");
            }

            LogManager.DebugFormat("Start Invoke {0} Function", fName);
            object ret = agent.CallFunction(fName, args);
            LogManager.DebugFormat("End Invoke {0} Function", fName);
            return ret;
        }

        protected object CallFunction(BindingFlags flag, string fName, params object[] args)
        {
            if (agent == null)
            {
                throw new Exception("COM 对象已注销，不能执行该操作！");
            }
            LogManager.DebugFormat("Start Invoke {0} Function", fName);
            object ret = agent.CallFunction(flag, fName, args);
            LogManager.DebugFormat("End Invoke {0} Function", fName);
            return ret;
        }
        #endregion

        #region 获取属性
        /// <summary>
        /// 获取COM接口导出的属性值，该属性必须有get权限
        /// </summary>
        /// <param name="pName">属性名称</param>
        /// <returns></returns>
        protected object getProperty(string pName)
        {
            if (agent == null)
            {
                throw new Exception("COM 对象已注销，不能执行该操作！");
            }
            LogManager.Debug(string.Format("{0} getProperty {1}", this.GetType().Name, pName));
            return agent.getProperty(pName);
        }
        #endregion

        #region 设置属性
        /// <summary>
        /// 设置COM接口导出的属性，该属性必须有set权限
        /// </summary>
        /// <param name="pName">属性名称</param>
        /// <param name="pValue">属性值</param>
        protected void setProperty(string pName, object pValue)
        {
            if (agent == null)
            {
                throw new Exception("COM 对象已注销，不能执行该操作！");
            }
            LogManager.Debug(string.Format("{0} setProperty {1} = {2}", this.GetType().Name, pName, pValue));
            agent.setProperty(pName, pValue);
        }
        #endregion

        #region 释放对象
        /// <summary>
        /// 释放COM对象
        /// </summary>
        public void Dispose()
        {

            if (agent != null)
            {
                agent.Dispose();
                agent = null;
            }
            if (comDomain != null)
            {
                AppDomain.Unload(comDomain);
                comDomain = null;
            }
            GC.Collect();
        }
        #endregion

    }

    internal class TransparentAgent : MarshalByRefObject, IDisposable
    {
        private const BindingFlags bfi = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;

        private Type objType = null;
        private object obj = null;
        internal TransparentAgent() { }
        public void Create(string progid, string clsid, string comName)
        {
            if (string.IsNullOrEmpty(clsid))
            {
                if (string.IsNullOrEmpty(progid))
                {
                    throw new InvalidOperationException("progid与clsid不能同时为空");
                }
                else
                {
                    objType = Type.GetTypeFromProgID(progid);
                }
            }
            else
            {
                objType = Type.GetTypeFromCLSID(Guid.Parse(clsid));
            }

            if (objType == null)
            {
                throw new Exception(string.Format("未安装{0}控件", comName));
            }

            try
            {
                obj = Activator.CreateInstance(objType);
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new Exception(string.Format("未安装{0}控件", comName));
            }
        }


        #region 调用函数
        /// <summary>
        /// 调用COM接口导出的方法
        /// </summary>
        /// <param name="fName">方法名称</param>
        /// <param name="args">参数数组</param>
        /// <returns></returns>
        public object CallFunction(string fName, params object[] args)
        {
            if (obj == null)
            {
                throw new Exception("COM 对象已注销，不能执行该操作！");
            }

            try
            {
                //string currPath = System.IO.Directory.GetCurrentDirectory();
                //if (!string.IsNullOrEmpty(path))
                //{
                //    System.IO.Directory.SetCurrentDirectory(path);
                //}
                object ret = objType.InvokeMember(fName, BindingFlags.InvokeMethod | BindingFlags.Instance, null, obj, args);
                //System.IO.Directory.SetCurrentDirectory(currPath);
                LogManager.DebugFormat("End Invoke {0} Function", fName);
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                if (ex.InnerException != null)
                {
                    LogManager.Error(ex.InnerException);
                }
                throw new Exception(string.Format("Call {0} function occur error.", fName), ex);
            }
        }

        public object CallFunction(BindingFlags flag, string fName, params object[] args)
        {
            if (obj == null)
            {
                throw new Exception("COM 对象已注销，不能执行该操作！");
            }
            try
            {
                //string currPath = System.IO.Directory.GetCurrentDirectory();
                //if (!string.IsNullOrEmpty(path))
                //{
                //    System.IO.Directory.SetCurrentDirectory(path);
                //}
                LogManager.DebugFormat("Start Invoke {0} Function", fName);
                object ret = objType.InvokeMember(fName, flag, null, obj, args);
                LogManager.DebugFormat("End Invoke {0} Function", fName);
                //System.IO.Directory.SetCurrentDirectory(currPath);
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                if (ex.InnerException != null)
                {
                    LogManager.Error(ex.InnerException);
                }
                throw new Exception(string.Format("Call {0} function occur error.", fName), ex);
            }
        }
        #endregion

        #region 获取属性
        /// <summary>
        /// 获取COM接口导出的属性值，该属性必须有get权限
        /// </summary>
        /// <param name="pName">属性名称</param>
        /// <returns></returns>
        public object getProperty(string pName)
        {
            if (obj == null)
            {
                throw new Exception("COM 对象已注销，不能执行该操作！");
            }
            try
            {
                return objType.InvokeMember(pName, BindingFlags.GetProperty, null, obj, null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Get {0} property occur exception", pName), ex);
            }
        }
        #endregion

        #region 设置属性
        /// <summary>
        /// 设置COM接口导出的属性，该属性必须有set权限
        /// </summary>
        /// <param name="pName">属性名称</param>
        /// <param name="pValue">属性值</param>
        public void setProperty(string pName, object pValue)
        {
            if (obj == null)
            {
                throw new Exception("COM 对象已注销，不能执行该操作！");
            }
            try
            {
                objType.InvokeMember(pName, BindingFlags.SetProperty, null, obj, new object[] { pValue });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Set {0} property occur exception", pName), ex);
            }
        }
        #endregion

        public void Dispose()
        {
            objType = null;
            obj = null;
            GC.Collect();
        }
    }
}
