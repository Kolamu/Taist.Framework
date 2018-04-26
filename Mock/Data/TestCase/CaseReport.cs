namespace Mock.Data
{
    using System.Collections.Generic;
    public enum ReportResult
    {
        SUCCESS,
        FAILED,
        BLOCKED
    }
    public class CaseReport
    {
        private List<StepReport> _stepReport = new List<StepReport>();

        public ReportResult Result
        {
            get
            {
                if (!string.IsNullOrEmpty(RelateCaseBh))
                {
                    return ReportResult.BLOCKED;
                }
                foreach (StepReport report in _stepReport)
                {
                    if (report.Result == ReportResult.FAILED)
                    {
                        return ReportResult.FAILED;
                    }
                }
                return ReportResult.SUCCESS;
            }
        }

        public StepReport ExecutingStep
        {
            get
            {
                return currentStep;
            }
        }

        private StepReport currentStep = null;
        public void SetStep(StepReport report)
        {
            _stepReport.Add(report);
            currentStep = report;
        }

        public string RelateCaseBh { get; set; }

        public void Clear()
        {
            _stepReport.Clear();
        }

        public void Save()
        {

        }
    }
}
