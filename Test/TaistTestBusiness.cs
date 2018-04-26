using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mock.Data;
using Mock.Data.Attributes;
namespace Taist.Framework.Test
{
    [BusinessClass]
    public class TaistTestBusiness
    {
        [BusinessMethod(Keywords="FTEST")]
        public static void TestFramework(TestDataList<TaistData> datalist)
        {
            TestCasePool.SetDetailColumnName("名称", "name");
            TestCasePool.SetDetailColumnName("状态", "state", true);
            TestCasePool.SetDetailColumnName("结果", "msg");
            foreach (TaistData data in datalist)
            {
                ReportDetail rd = new ReportDetail();
                rd.Set("name", "结果");
                if (data.State)
                {
                    rd.SetResult(true);
                    rd.Set("state", "S");
                    rd.Set("msg", "State设置为true");
                }
                else
                {
                    rd.SetResult(false);
                    rd.Set("state", "F");
                    rd.Set("msg", "State设置为false");
                }

                TestCasePool.SetReportDetail(rd);

                Cache cache = new Cache();
                cache.Bh = data.Bh;
                cache["State"] = data.State.ToString();
                cache.Save();
            }
        }
    }
}
