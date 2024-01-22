using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ProgressReport
{
    public class ProgressReportMasterViewModel:BaseViewModel
    {
        public ProgressReportMasterViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
            YearWiseBilledList = new List<YearWiseBilledViewModel>();
            ProgressReportDetailList = new List<ProgressReportDetailsViewModel>();
            FinancialYearList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            this.APPList = new List<SelectListItem>();
            this.TempAttachmentDetail = new List<ProgressReportMasterViewModel>();
            ApproverList = new List<ApprovalFlowViewModel>();
        }

        #region Standard Property
        [DisplayName("APP")]
        public int AnnualProcurementPlanMasterId { get; set; }

        [DisplayName("Report Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime ReportDate { get; set; }

        public int? ZoneInfoId { get; set; }

        public int ProjectForId { get; set; }

        [DisplayName("Financial Year")]
        public int FinancialYearId { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }
        public bool IsConfirm { get; set; }
        #endregion

        #region List
        public IList<SelectListItem> APPList { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }

        public IList<SelectListItem> StatusList { get; set; }

        public List<ApprovalFlowViewModel> ApproverList { get; set; }

        #endregion

        #region Others
        public DateTime ReportFromDate { get; set; }
        public DateTime ReportToDate { get; set; }

        public int ProgressReportDetailsId { get; set; }
        public string NameOfWorks { get; set; }
        public string NameOfContractor { get; set; }
        public string DateOfNOA { get; set; }
        public List<YearWiseBilledViewModel> YearWiseBilledList { get; set; }
        public List<ProgressReportDetailsViewModel> ProgressReportDetailList { get; set; }
        #endregion

        #region Attachment List

        public int? ProgressReportId { get; set; }
        [DisplayName("Name of Work")]
        public string NameofWork { get; set; }
        [DisplayName("Previous Date")]
        [UIHint("_Date")]
        public DateTime PreviousDate { get; set; }
        [DisplayName("Current Date")]
        [UIHint("_Date")]
        public DateTime CurrentDate { get; set; }

        public byte[] PreviousAttachment { set; get; }
        public byte[] CurrentAttachment { set; get; }

        public HttpPostedFileBase PreviousFile { get; set; }
        public HttpPostedFileBase CurrentFile { get; set; }

        public string PreviousFileName { get; set; }
        public string CurrentFileName { get; set; }

        public string PreviousFilePath { get; set; }
        public string CurrentFilePath { get; set; }

        public int? FileSize { get; set; }
        public IList<ProgressReportMasterViewModel> TempAttachmentDetail { get; set; }
        #endregion
    }
    public class YearWiseBilledViewModel
    {
        public YearWiseBilledViewModel()
        {
            FinancialYearList = new List<SelectListItem>();
        }
        public int? FinancialYearId { get; set; }


        public IList<SelectListItem> FinancialYearList { get; set; }

        public string FinancialYearName { get; set; }

        public decimal BilledAmount { get; set; }
    }

}