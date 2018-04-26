
namespace Mock.Tools.Controls
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Web.Services.Description;
    using System.Xml.Serialization;

    using Mock.Tools.Exception;
    public abstract class WebServiceObject
    {
        private Type objType = null;
        private object obj = null;

        protected WebServiceObject(string url)
        {
            if (url == null)
            {
                throw new WebServiceUrlFormatErrorException(url);
            }

            
            string serviceName = "";
            if (!url.ToLower().EndsWith("?wsdl"))
            {
                url = url + "?wsdl";
            }

            try
            {
                // 1. 使用 WebClient 下载 WSDL 信息。
                WebClient web = new WebClient();
                Stream stream = web.OpenRead(url);

                // 2. 创建和格式化 WSDL 文档。
                ServiceDescription description = ServiceDescription.Read(stream);
                if (description.Services.Count != 1)
                {
                    throw new MultiControlException("Webservice " + url);
                }
                serviceName = description.Services[0].Name;
                // 3. 创建客户端代理代理类。
                ServiceDescriptionImporter importer = new ServiceDescriptionImporter();

                importer.ProtocolName = "Soap"; // 指定访问协议。
                importer.Style = ServiceDescriptionImportStyle.Client; // 生成客户端代理。
                importer.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateNewAsync;

                importer.AddServiceDescription(description, null, null); // 添加 WSDL 文档。

                // 4. 使用 CodeDom 编译客户端代理类。
                CodeNamespace nmspace = new CodeNamespace(); // 为代理类添加命名空间，缺省为全局空间。
                CodeCompileUnit unit = new CodeCompileUnit();
                unit.Namespaces.Add(nmspace);

                ServiceDescriptionImportWarnings warning = importer.Import(nmspace, unit);
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                CompilerParameters parameter = new CompilerParameters();
                parameter.GenerateExecutable = false;
                parameter.GenerateInMemory = true;
                parameter.ReferencedAssemblies.Add("System.dll");
                parameter.ReferencedAssemblies.Add("System.XML.dll");
                parameter.ReferencedAssemblies.Add("System.Web.Services.dll");
                parameter.ReferencedAssemblies.Add("System.Data.dll");

                CompilerResults result = provider.CompileAssemblyFromDom(parameter, unit);

                // 5. 使用 Reflection 调用 WebService。
                if (!result.Errors.HasErrors)
                {
                    Assembly asm = result.CompiledAssembly;
                    objType = asm.GetType(serviceName); // 如果在前面为代理类添加了命名空间，此处需要将命名空间添加到类型前面。
                    obj = Activator.CreateInstance(objType);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWebServiceRequestException(string.Format("Create WebService Instance occur exception {0}", ex.Message));
            }
        }


        #region 调用函数
        /// <summary>
        /// 调用COM接口导出的方法
        /// </summary>
        /// <param name="fName">方法名称</param>
        /// <param name="args">参数数组</param>
        /// <returns></returns>
        protected object CallFunction(string fName, params object[] args)
        {
            if (obj == null)
            {
                throw new InvalidWebServiceRequestException("WebService instance has been disposed!");
            }
            try
            {
                MethodInfo mi = objType.GetMethod(fName);
                object ret = mi.Invoke(obj, args);
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                if (ex.InnerException != null)
                {
                    LogManager.Error(ex.InnerException);
                }
                throw new InvalidWebServiceRequestException(string.Format("Call {0} function occur error, {1}", fName, ex.Message));
            }
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
            if (obj == null)
            {
                throw new InvalidWebServiceRequestException("WebService instance has been disposed!");
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
        protected void setProperty(string pName, object pValue)
        {
            if (obj == null)
            {
                throw new InvalidWebServiceRequestException("WebService instance has been disposed!");
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

        #region 释放对象
        /// <summary>
        /// 释放COM对象
        /// </summary>
        public void Dispose()
        {
            objType = null;
            obj = null;
            GC.Collect();
        }
        #endregion
    }
}
