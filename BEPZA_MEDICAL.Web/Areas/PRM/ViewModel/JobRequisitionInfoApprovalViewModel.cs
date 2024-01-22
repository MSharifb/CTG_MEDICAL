using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class JobRequisitionInfoApprovalViewModel : BaseViewModel
    {
        #region Ctor
        public JobRequisitionInfoApprovalViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.ReferenceNoList = new List<SelectListItem>();
            this.RequsitionApprovalDetailList = new List<JobRequisitionInfoApprovalDetailViewModel>();
            this.JobRequisitionApprovalList = new List<JobRequisitionInfoApprovalViewModel>();
        }
        #endregion

        #region Standard Property

        [Display(Name="Reference No.")]
        public int JobRequisitionInfoSummaryId { get; set; }
        public IList<SelectListItem> ReferenceNoList { get; set; }

        public int ApprovedById { get; set; }
        [Display(Name = "Approve Date")]
        [UIHint("_RequiredDate")]
        public DateTime? ApproveDate { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        #endregion

        #region Other
        [Display(Name="Reference Date")]
        public string ReferenceDate { get; set; }
        [Display(Name = "Prepared By")]
        public string PreparedBy { get; set; }
        [Display(Name = "Status Designation")]
        public string Designation { get; set; }
        public int FinancialYearId { get; set; }
        [Display(Name = "Financial Year")]
        public string FinancialYear { get; set; }
        public string ReferenceNo { get; set; }

        public int RequisitionInfoSummaryDetailId { get; set; }
        public int DesignationId { get; set; }
        public bool IsCheckedFinal { get; set; }
        public string RequisitionNo { get; set; }
        public int NumberOfRequiredPost { get; set; }
        public string PayScale { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public int? RecommendPost { get; set; }
        public int? ApprovedPost { get; set; }

        public int RequisitionSummaryId { get; set; }

        [Display(Name="Name")]
        public string ApprovedByName { get; set; }
        [Display(Name="Designation")]
        public string ApprovedByDesignation { get; set; }

        public List<JobRequisitionInfoApprovalDetailViewModel> RequsitionApprovalDetailList { get; set; }
        public List<JobRequisitionInfoApprovalViewModel> JobRequisitionApprovalList { get; set; }

        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

        #region Post
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
        #endregion
    }
}