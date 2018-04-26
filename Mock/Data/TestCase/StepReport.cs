namespace Mock.Data
{
    using System;
    using System.Collections.Generic;
    public class StepReport
    {
        private List<ReportDetail> _stepReport = new List<ReportDetail>();

        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        

        public ReportResult Result
        {
            get
            {
                foreach (ReportDetail report in _stepReport)
                {
                    if (!report.Result)
                    {
                        return ReportResult.FAILED;
                    }
                }
                return ReportResult.SUCCESS;
            }
        }

        public void Save()
        {

        }
    }
}
