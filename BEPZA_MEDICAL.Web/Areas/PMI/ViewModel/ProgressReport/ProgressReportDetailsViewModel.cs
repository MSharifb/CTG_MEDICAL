using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ProgressReport
{
    public class ProgressReportDetailsViewModel:BaseViewModel
    {
        public ProgressReportDetailsViewModel()
        {
            this.YearlyBilledList = new List<ProgressReportDetailYearlyBilledViewModel>();
            this.WorkStatusList = new List<SelectListItem>();
        }

        public string SerialNo { get; set; }
        public int ProgressReportMasterId { get; set; }
        public string NameOfWorks { get; set; }
        public string NameOfContractor { get; set; }
        public decimal? EstimatedAmount { get; set; }
        public decimal? AcceptedAmount { get; set; }
        [UIHint("_Date")]
        public DateTime? DateOfNOA { get; set; }
        [UIHint("_Date")]
        public DateTime? ExpireDateOfBG { get; set; }
        [UIHint("_Date")]
        public DateTime? DateOfContractAgreement { get; set; }
        [UIHint("_Date")]
        public DateTime? DateOfSiteHandingOver { get; set; }
        [UIHint("_Date")]
        public DateTime? DateOfCommencement { get; set; }
        public decimal? CompletionTime { get; set; }
        [UIHint("_Date")]
        public DateTime? DateOfCompletion { get; set; }
        public decimal? TimeElapsed { get; set; }
        public decimal? PhysicalProgress { get; set; }
        public int? WrokStatusId { get; set; }
        public string Remarks { get; set; }

        public List<ProgressReportDetailYearlyBilledViewModel> YearlyBilledList { get; set; }
        public IList<SelectListItem> WorkStatusList { get; set; }
    }
}