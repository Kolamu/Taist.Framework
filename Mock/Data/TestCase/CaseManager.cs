namespace Mock.Data
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;

    using System.Threading;
    using Mock.Data.Exception;
    public class CaseManager
    {
        public static event CaseEventHandler CaseCreate;
        public static event CaseEventHandler CaseRemoved;
       
        private static Dictionary<int, string> identificationDic = null;
        private static readonly object identificationLock = new object();
        internal static string GetIdentification()
        {
            lock (identificationLock)
            {
                int id = Thread.CurrentThread.ManagedThreadId;
                if (identificationDic == null)
                {
                    throw new UnknownIdentificationException(id);
                }

                if (identificationDic.ContainsKey(id))
                {
                    return identificationDic[id];
                }

                throw new UnknownIdentificationException(id);
            }
        }

        internal static string CreateIdentification(int id)
        {
            lock (identificationLock)
            {
                if (identificationDic == null)
                {
                    identificationDic = new Dictionary<int, string>();
                }

                string identification = null;
                if (identificationDic.ContainsKey(id))
                {
                    identification = identificationDic[id];
                }
                else
                {
                    identification = Guid.NewGuid().ToString("N").ToUpper();
                    identificationDic.Add(id, identification);
                }

                if (CaseCreate != null)
                {
                    CaseCreate(identification);
                }

                return identification;
            }
        }

        internal static void SetIdentification(int id)
        {
            lock (identificationLock)
            {
                if (identificationDic == null)
                {
                    LogManager.DebugTimer("identification is null", 2);
                    return;
                }

                if (identificationDic.ContainsKey(id))
                {
                    throw new NotUniqueDataException(id.ToString());
                }
                int parentId = Thread.CurrentThread.ManagedThreadId;
                if (identificationDic.ContainsKey(parentId))
                {
                    identificationDic.Add(id, identificationDic[parentId]);
                }
            }
        }

        internal static void RemoveIdentification(int threadId)
        {
            lock (identificationLock)
            {
                if (identificationDic == null || !identificationDic.ContainsKey(threadId)) return;
                identificationDic.Remove(threadId);
            }
        }

        internal static void RemoveIdentification(string caseId)
        {
            lock (identificationLock)
            {
                if (identificationDic == null) return;
                Dictionary<int, string> tmp = new Dictionary<int, string>();
                foreach (KeyValuePair<int, string> kv in identificationDic)
                {
                    if (string.Equals(kv.Value, caseId)) continue;
                    tmp.Add(kv.Key, kv.Value);
                }
                identificationDic = tmp;

                if (_setDetailDic.ContainsKey(caseId))
                {
                    _setDetailDic.Remove(caseId);
                }

                if (_setHeaderDic.ContainsKey(caseId))
                {
                    _setHeaderDic.Remove(caseId);
                }

                if (_setClearDic.ContainsKey(caseId))
                {
                    _setClearDic.Remove(caseId);
                }

                if (CaseRemoved != null)
                {
                    CaseRemoved(caseId);
                }
            }
        }

        private static Dictionary<string, SetReportDetailHandler> _setDetailDic = new Dictionary<string, SetReportDetailHandler>();
        internal static void SetCaseReportDetailHandler(string caseId, SetReportDetailHandler handler)
        {
            lock (identificationLock)
            {
                if (_setDetailDic.ContainsKey(caseId))
                {
                    _setDetailDic[caseId] = handler;
                }
                else
                {
                    _setDetailDic.Add(caseId, handler);
                }
            }
        }

        private static Dictionary<string, SetReportHeaderHandler> _setHeaderDic = new Dictionary<string, SetReportHeaderHandler>();
        internal static void SetCaseReportHeaderHandler(string caseId, SetReportHeaderHandler handler)
        {
            lock (identificationLock)
            {
                if (_setHeaderDic.ContainsKey(caseId))
                {
                    _setHeaderDic[caseId] = handler;
                }
                else
                {
                    _setHeaderDic.Add(caseId, handler);
                }
            }
        }

        private static Dictionary<string, SetReportClearHandler> _setClearDic = new Dictionary<string, SetReportClearHandler>();
        internal static void SetCaseReportClearHandler(string caseId, SetReportClearHandler handler)
        {
            lock (identificationLock)
            {
                if (_setClearDic.ContainsKey(caseId))
                {
                    _setClearDic[caseId] = handler;
                }
                else
                {
                    _setClearDic.Add(caseId, handler);
                }
            }
        }

        /// <summary>
        /// 设置明细信息
        /// </summary>
        /// <param name="detail">明细信息，这通常为TestCasePool.SetDetailColumnName所设置的结构数据</param>
        internal static void SetReportDetail(ReportDetail detail)
        {
            string id = null;
            try
            {
                id = CaseManager.GetIdentification();
            }
            catch
            {
                return;
            }
            if (!_setDetailDic.ContainsKey(id))
            {
                throw new CanNotFindDataException("Case identification");
            }

            _setDetailDic[id](detail);
        }

        /// <summary>
        /// 设置明细信息内容中包含的项目名称
        /// </summary>
        /// <param name="displayName">明细信息显示名称</param>
        /// <param name="tagName">明细信息简化名称（这通常是明细信息生成XML时的标签名称）</param>
        /// <param name="state">指示该列内容是状态标志</param>
        internal static void SetDetailColumnName(string displayName, string tagName, bool state = false)
        {
            string id = null;
            try
            {
                id = CaseManager.GetIdentification();
            }
            catch
            {
                return;
            }

            if (!_setHeaderDic.ContainsKey(id))
            {
                throw new CanNotFindDataException("Case identification");
            }

            _setHeaderDic[id](displayName, tagName, state);
        }

        /// <summary>
        /// 清除测试报告的明细信息
        /// </summary>
        internal static void ClearReportDetail()
        {
            string id = null;
            try
            {
                id = CaseManager.GetIdentification();
            }
            catch
            {
                return;
            }

            if (!_setDetailDic.ContainsKey(id))
            {
                throw new CanNotFindDataException("Case identification");
            }

            _setClearDic[id]();
        }


        internal static void Report(string name, System.Exception ex)
        {
            while (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
            ReportDetail rd = new ReportDetail();
            rd.SetResult(false);
            rd.Set("name", name);
            rd.Set("state", "F");
            rd.Set("expInfo", ex.Message);
            LogManager.Error(ex);
            if (ex.InnerException != null)
            {
                rd.Set("innerExpInfo", ex.InnerException.Message);
                LogManager.Error(ex.InnerException);
            }
            else
            {
                rd.Set("innerExpInfo", "");
            }
            TestCasePool.SetDetailColumnName("名称", "name");
            TestCasePool.SetDetailColumnName("状态", "state", true);
            TestCasePool.SetDetailColumnName("异常", "expInfo");
            TestCasePool.SetDetailColumnName("内部异常", "innerExpInfo");
            TestCasePool.SetReportDetail(rd);
        }

        internal static void Report(string name, string message)
        {
            ReportDetail rd = new ReportDetail();
            rd.SetResult(false);
            rd.Set("name", name);
            rd.Set("state", "F");
            rd.Set("msg", message);
            TestCasePool.SetDetailColumnName("名称", "name");
            TestCasePool.SetDetailColumnName("状态", "state", true);
            TestCasePool.SetDetailColumnName("错误信息", "msg");
            TestCasePool.SetReportDetail(rd);
        }

    }

    internal delegate void SetReportClearHandler();
    internal delegate void SetReportDetailHandler(ReportDetail detail);
    internal delegate void SetReportHeaderHandler(string displayName, string tagName, bool state);

    public delegate void CaseEventHandler(string identification);

}
