namespace Mock.Data
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;

    using Mock.Data.Attributes;
    using Mock.Data.Exception;
    public class SubKeyword
    {
        private string _name = null;
        private object obj = null;
        private MethodInfo mi = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classObj"></param>
        /// <param name="mi"></param>
        public SubKeyword(string name, object classObj, MethodInfo mi)
        {
            _name = name;
            obj = classObj;
            this.mi = mi;
        }

        /// <summary>
        /// 执行该关键字
        /// </summary>
        /// <param name="bhs"></param>
        /// <param name="paramDic"></param>
        public void Invoke(string bhs, Dictionary<string, string> paramDic)
        {
            if (mi == null)
            {
                throw new KeywordInvokeErrorException("Method infomation is null");
            }
            ParameterInfo[] piArray = mi.GetParameters();
            if (piArray.Length == 0)
            {
                mi.Invoke(obj, null);
            }
            else if (piArray.Length == 1)
            {
                ParameterInfo pi = piArray[0];
                Type pType = pi.ParameterType;
                if (pType.IsGenericType)
                {
                    Type paramType = typeof(DataFactory).Assembly.GetType("TestDataList");
                    paramType.MakeGenericType(pType.GetGenericArguments());
                    object arg = Activator.CreateInstance(paramType);
                    arg = bhs;
                    mi.Invoke(obj, new object[] { arg });
                }
                else
                {
                    ParamInvoke(paramDic);
                }
            }
            else
            {
                ParamInvoke(paramDic);
            }
        }

        private void ParamInvoke(Dictionary<string, string> param)
        {
            object[] paramAttributesObjs = mi.GetCustomAttributes(typeof(ParamAttribute), false);
            object[] paramObjs = new object[paramAttributesObjs.Length];
            foreach (object o in paramAttributesObjs)
            {
                ParamAttribute pa = o as ParamAttribute;

                if (param != null && param.ContainsKey(pa.Name.ToLower()))
                {
                    paramObjs[pa.Id - 1] = param[pa.Name.ToLower()];
                }
                else
                {
                    paramObjs[pa.Id - 1] = pa.Default;
                }
            }

            mi.Invoke(obj, paramObjs);
        }
    }
}
