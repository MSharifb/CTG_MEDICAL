using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class JobRequisitionInfoSummaryViewModel : BaseViewModel
    {
        #region Ctor
        public JobRequisitionInfoSummaryViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.IsSubmit = false;
            this.FinancialYearList = new List<SelectListItem>();
            this.RequsitionInformationList = new List<RequisitionInfoSummaryDetail>();
            this.JobRequisitionInfoSummary = new List<JobRequisitionInfoSummaryViewModel>();
        }
        #endregion

        #region Standard Property

        [Display(Name = "Reference No.")]
        public string ReferenceNo { get; set; }
        [Display(Name = "Reference Date")]
        [UIHint("_RequiredDate")]
        public DateTime? ReferenceDate { get; set; }
        public int PreparedById { get; set; }
        [Display(Name = "Financial Year")]
        public int FinancialYearId { get; set; }
        public string FinancialYear { get; set; }
        public IList<SelectListItem> FinancialYearList { get; set; }
        public string Comments { get; set; }
        public bool IsSubmit { get; set; }
        public int ZoneInfoId { get; set; }
        #endregion

        #region Other
        public string Status { get; set; }
        [Display(Name = "Prepared By")]
        public string PreparedBy { get; set; }
        [Display(Name = "Status Designation")]
        public string Designation { get; set; }

        [Display(Name = "Total Sanctioned Post")]
        public int? TotalSanctionedPost { get; set; }
        [Display(Name = "Number Of Post for Direct Recruitment")]
        public int? DiRecSancPost { get; set; }
        [Display(Name = "Number of Post for Promotion")]
        public int? PromSancPost { get; set; }

        [Display(Name = "Total Filled Up Post")]
        public int? TotalFilledUpPost { get; set; }
        [Display(Name = "Number of Filled Up Post By Direct Recruitment")]
        public int? DiRecFillPost { get; set; }
        [Display(Name = "Number of Filled Up Post By Promotion")]
        public int? PromFillPost { get; set; }

        [Display(Name = "Total Vacant Post")]
        public int? TotalVacantPost { get; set; }
        [Display(Name = "Number of Vacant Post For Direct Recruitment")]
        public int? DiRecVacPost { get; set; }
        [Display(Name = "Number of  Vacant Post For Direct Promotion")]
        public int? PromVacPost { get; set; }

        public int DesignationId { get; set; }
        public int NumberOfRequiredPost { get; set; }
        public string PayScale { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public bool IsChecked { get; set; }
        public bool IsCheckedFinal { get; set; }
        public int RequisitionInfoDetailId { get; set; }
        public int RecommendPost { get; set; }

        public int RequisionId { get; set; }
        public string RequisitionNo { get; set; }
        public List<RequisitionInfoSummaryDetail> RequsitionInformationList { get; set; }
        public List<JobRequisitionInfoSummaryViewModel> JobRequisitionInfoSummary { get; set; }
        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion
    }
    public class RequisitionInfoSummaryDetail 
    {
        public int DesignationId { get; set; }
        public int NumberOfRequiredPost { get; set; }
        public string PayScale { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public bool IsChecked { get; set; }
        public int RequisitionInfoDetailId { get; set; }
        public int NumOfRecommendedPost { get; set; }

        public int RequisionId { get; set; }
        public string RequisitionNo { get; set; }
        public string ReqPreparedBy { get; set; }
        public string Designation { get; set; }
        public string SubmissionDate { get; set; }
        public bool IsCheckedFinal { get; set; }
    }
}