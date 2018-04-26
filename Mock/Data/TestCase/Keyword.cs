namespace Mock.Data
{
    using System;
    using System.Reflection;
    using Mock.Data.Exception;
    using System.Collections.Generic;

    using Mock.Data;
    using Mock.Data.Attributes;
    public class Keyword
    {
        //private Dictionary<string, SubKeyword> _keyWordDictionary = null;
        private string _keywordName = null;
        private object obj = null;
        private MethodInfo mi = null;
        private TestCase _testCase = null;
        private Step _pStep = null;

        public Keyword(TestCase _testCase, Step _pStep)
        {
            this._testCase = _testCase;
            this._pStep = _pStep;
        }

        public Keyword(string name, object classObj, MethodInfo mi)
        {
            _keywordName = name;
            obj = classObj;
            this.mi = mi;
            //_keyWordDictionary = new Dictionary<string, SubKeyword>();
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
                CaseInvoke();
                return;
            }
            LogManager.DebugFormat("Start Invoke {0}", mi.Name);
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
                    if (pType.Name.StartsWith("TestDataList") || pType.Name.StartsWith("List"))
                    {
                        Type paramType = typeof(TestDataList<>);
                        paramType = paramType.MakeGenericType(pType.GetGenericArguments());
                        object arg = Activator.CreateInstance(paramType, bhs);
                        
                        mi.Invoke(obj, new object[] { arg });
                    }
                    else
                    {
                        LogManager.DebugFormat("End Invoke {0}", mi.Name);
                        throw new InvalidDataTypeException(pType.Name);
                    }
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
            LogManager.DebugFormat("End Invoke {0}", mi.Name);
        }

        private void ParamInvoke(Dictionary<string, string> param)
        {
            object[] paramAttributesObjs = mi.GetCustomAttributes(typeof(ParamAttribute), false);
            if (paramAttributesObjs.Length == 0)
            {
                mi.Invoke(obj, null);
            }
            else
            {
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

        private void CaseInvoke()
        {
            LogManager.Debug("Case keyword execute");
            if (_testCase == null)
            {
                throw new KeywordInvokeErrorException("Keyword is null");
            }

            _testCase.Invoke(_pStep);
        }
    }
}
