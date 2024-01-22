using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ProgressReport
{
    public class ProgressReportDetailYearlyBilledViewModel
    {
        public int Id { get; set; }
        public int ProgressReportDetailsId { get; set; }
        public int FinancialYearId { get; set; }
        public decimal? BilledAmount { get; set; }

        #region Others
        public string PreviousFieldId { get; set; }
        #endregion
    }
}