using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ClearanceInfoFromMinistryViewModel : BaseViewModel
    {
        #region Ctor
        public ClearanceInfoFromMinistryViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.ReferenceNoList = new List<SelectListItem>();
            this.RequsitionClearanceDetailList = new List<ClearanceInfoFromMinistryDetailViewModel>();
            this.JobRequisitionClearanceList = new List<ClearanceInfoFromMinistryViewModel>();

        }
        #endregion

        #region Standard Property

        [Display(Name = "Reference No.")]
        public int JobRequisitionInfoSummaryId { get; set; }
        public IList<SelectListItem> ReferenceNoList { get; set; }

        [Display(Name = "Clearance Date")]
        [UIHint("_RequiredDate")]
        public DateTime? ClearanceDate { get; set; }

        [Display(Name="Clearance Details")]
        public string ClearanceDetails { get; set; }

        [Display(Name="Ref./Order No.")]
        public string RefOrOrderNo { get; set; }

        public string Comments { get; set; }
        public string Status { get; set; }
        #endregion

        #region Other
        [Display(Name = "Reference Date")]
        [UIHint("_Date")]
        public string ReferenceDate { get; set; }
        [Display(Name = "Prepared By")]
        public string PreparedBy { get; set; }
        public string Designation { get; set; }
        public int FinancialYearId { get; set; }
        [Display(Name = "Financial Year")]
        public string FinancialYear { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? ApproveDate { get; set; }

        public int RequisitionInfoApprovalDetailId { get; set; }
        public int DesignationId { get; set; }
        public bool IsCheckedFinal { get; set; }
        public string RequisitionNo { get; set; }
        public int NumberOfRequiredPost { get; set; }
        public string PayScale { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public int? RecommendPost { get; set; }
        public int? ApprovedPost { get; set; }
        public int? ClearancePost { get; set; }

        public int RequisitionSummaryId { get; set; }


        public List<ClearanceInfoFromMinistryDetailViewModel> RequsitionClearanceDetailList { get; set; }
        public List<ClearanceInfoFromMinistryViewModel> JobRequisitionClearanceList { get; set; }

        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion
    }
}