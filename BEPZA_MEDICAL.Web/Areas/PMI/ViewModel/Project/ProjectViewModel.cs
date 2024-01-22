using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project
{
    public class ProjectViewModel : BaseViewModel
    {
        #region Constructor

        public ProjectViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            //this.MinistryList = new List<SelectListItem>();
            this.AgencyList = new List<SelectListItem>();

            this.ConstructionTypeList = new List<SelectListItem>();
            this.ConstructionCategoryList = new List<SelectListItem>();

            this.ProjectStatusList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.PaymentStatusList = new List<SelectListItem>();
            this.FinancialYearList = new List<SelectListItem>();
            this.ApprovalStatusList = new List<SelectListItem>();

            this.TenderDetailsViewModel = new TenderDetailsViewModel();
            this.TenderDetailsViewModelList = new List<TenderDetailsViewModel>();

            this.ProjectDetailsViewModel = new ProjectDetailsViewModel();
            this.ProjectDetailList = new List<ProjectDetailsViewModel>();
            this.ProjectTimeExtensionList = new List<ProjectTimeExtensionViewModel>();
            this.ApproverList = new List<ApprovalFlowViewModel>();
        }

        #endregion

        #region Standard Prop

        public int APPMasterId { get; set; }

        [DisplayName("Financial Year")]
        public int FinancialYearId { get; set; }

        public int APPDetailsId { get; set; }

        [DisplayName("Ministry")]
        [Required]
        [UIHint("_ReadOnly")]
        public int MinistryId { get; set; }

        [DisplayName("Agency")]
        [Required]
        public int AgencyId { get; set; }

        [DisplayName("Budget Approval Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime BudgetApprovalDate { get; set; }

        [DisplayName("Works/Goods/Service")]
        [Required]
        [UIHint("_ReadOnly")]
        public string NameOfWorks { get; set; }

        [DisplayName("Zone")]
        public string ProjectZones { get; set; }

        [DisplayName("Procuring Entity Name")]
        public string ProcuringEntryName { get; set; }

        [DisplayName("Procuring Entity Code")]
        public string ProcuringEntryCode { get; set; }

        [DisplayName("Invitation Ref. No.")]
        public string InvitationRefNo { get; set; }

        [DisplayName("Type")]
        public int ConstructionTypeId { get; set; }

        [DisplayName("Category")]
        public int? ConstructionCategoryId { get; set; }

        [DisplayName("Development Partner")]
        public string DevelopmentPartner { get; set; }

        [DisplayName("Project Code")]
        public string ProjectCode { get; set; }


        [DisplayName("Administrative Approval Date")]
        [UIHint("_Date")]
        public DateTime AdministrativeApprovalDate { get; set; }

        [DisplayName("Tender Evaluation Date")]
        [UIHint("_Date")]
        public DateTime TenderEvalDate { get; set; }

        [DisplayName("Tender Issue Date")]
        [UIHint("_Date")]
        public DateTime TenderIssueDate { get; set; }

        [DisplayName("Tender Publication Date")]
        [UIHint("_Date")]
        public DateTime? TenderPubDate { get; set; }

        [DisplayName("Tender Last Selling Date")]
        [UIHint("_Date")]
        public DateTime TenderLastSellingDate { get; set; }

        [DisplayName("Tender Closing Date")]
        [UIHint("_Date")]
        public DateTime TenderClosingDate { get; set; }

        [DisplayName("Tender Closing Time")]
        public string TenderClosingTime { get; set; }


        [DisplayName("Tender Opening Date")]
        [UIHint("_Date")]

        public DateTime TenderOpeningDate { get; set; }

        [DisplayName("Tender Opening Time")]
        public string TenderOpeningTime { get; set; }

        [DisplayName("Contract Signing Date")]
        [UIHint("_Date")]
        public DateTime? ContractSignDate { get; set; }

        [DisplayName("Completion Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime? CompletionDate { get; set; }

        [DisplayName("Site Handover Date")]
        [UIHint("_Date")]
        public DateTime? SiteHandoverDate { get; set; }

        [DisplayName("Completion Time (Days)")]
        public int? TimeExtensionInDays { get; set; }

        [DisplayName("Award Issue Date")]
        [UIHint("_Date")]
        public DateTime? AwardIssueDate { get; set; }

        [DisplayName("Created Date")]
        [UIHint("_Date")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Approval Authority")]
        public int ApprovalAuthorityId { get; set; }

        [DisplayName("Project Status")]
        public int ProjectStatusId { get; set; }

        [DisplayName("Selling Tender Description")]
        public string SellingTenderDescription { get; set; }

        [DisplayName("Receiving Tender Description")]
        public string ReceivingTenderDescription { get; set; }

        [DisplayName("Opening Tender Description")]
        public string OpeningTenderDescription { get; set; }

        [DisplayName("Pre Tender Meeting Place")]
        public string PreTenderMeetingPlace { get; set; }

        [DisplayName("Eligibility of Tenderer")]
        public string EligibilityOfTenderer { get; set; }

        [DisplayName("Brief Description")]
        public string BriefDescription { get; set; }

        [DisplayName("Brief Description of Goods / Works / Services")]
        public string BriefDescriptionOfRelated { get; set; }

        [DisplayName("Services")]
        public string Services { get; set; }

        [DisplayName("Name of Inviting Tender")]
        public string NameOfInvitingTender { get; set; }

        [DisplayName("Designation")]
        public int? DesignationId { get; set; }

        [DisplayName("Procurement Address")]
        public string ProcureAddress { get; set; }

        [DisplayName("Phone")]
        public string PhoneNumber { get; set; }

        [DisplayName("Email")]
        public string EmailAddress { get; set; }

        [DisplayName("Fax")]
        public string FaxNumber { get; set; }

        [DisplayName("Notice")]
        public string Notice { get; set; }

        [DisplayName("Reefering")]
        [UIHint("_Date")]
        public DateTime? SecurityMoneyReefingDate { get; set; }

        [DisplayName("Liquidate Damage")]
        public string LiquidatedDamage { get; set; }

        [DisplayName("Payment Status")]
        public int? PaymentStatusId { get; set; }

        [DisplayName("Last Payment Date")]
        [UIHint("_Date")]
        public DateTime? LastPaymentDate { get; set; }

        [DisplayName("Final Payment Date")]
        [UIHint("_Date")]
        public DateTime? FinalPaymentDate { get; set; }
        public HttpPostedFileBase File { get; set; }

        [DisplayName("Drawing Attachment")]
        public byte[] DrawingAttachment { get; set; }

        //

        public string FileName { get; set; }
        public string FilePath { get; set; }

        //

        [DisplayName("Drawing Approval Date")]
        [UIHint("_Date")]
        public DateTime DrawingApprovalDate { get; set; }

        [DisplayName("Contractor Name")]
        public string ContractorName { get; set; }

        [DisplayName("Date Of Issuing NOA")]
        [UIHint("_Date")]
        [Required]
        public DateTime? DateOfIssuingNOA { get; set; }

        [DisplayName("Date Of PG Submission")]
        [UIHint("_Date")]
        public DateTime? DateOfPGSubmission { get; set; }

        [DisplayName("Date Of PG Expire")]
        [UIHint("_Date")]
        public DateTime? DateOfPGExpire { get; set; }

        [DisplayName("Date Of Commencement")]
        [UIHint("_Date")]
        [Required]
        public DateTime? DateOfCommencement { get; set; }

        [DisplayName("Physical Progress")]
        public Decimal? PhysicalProgress { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        public string ApprovalStatus { get; set; }
        [DisplayName("submission status")]
        public int? ApprovalStatusId { get; set; }
        public int? ApprovalSelectionId { get; set; }
        public IList<SelectListItem> ApprovalStatusList { get; set; }

        #endregion

        #region Other Prop

        public IList<SelectListItem> MinistryList { get; set; }

        public IList<SelectListItem> AgencyList { get; set; }

        public IList<SelectListItem> ConstructionTypeList { get; set; }

        public IList<SelectListItem> ConstructionCategoryList { get; set; }

        public IList<SelectListItem> ProjectStatusList { get; set; }

        public IList<SelectListItem> DesignationList { get; set; }

        public IList<SelectListItem> PaymentStatusList { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }

        public TenderDetailsViewModel TenderDetailsViewModel { get; set; }

        public List<TenderDetailsViewModel> TenderDetailsViewModelList { get; set; }

        [DisplayName("Ministry")]
        [UIHint("_ReadOnly")]
        public string MinistryName { get; set; }

        public string AgencyName { get; set; }

        public string ProjectImplementationAuthority { get; set; }

        public decimal TotalCostOfProject { get; set; }

        public string FinancialYearName { get; set; }

        public List<ProjectDetailsViewModel> ProjectDetailList { get; set; }

        public ProjectDetailsViewModel ProjectDetailsViewModel { get; set; }

        public List<ProjectTimeExtensionViewModel> ProjectTimeExtensionList { get; set; }
        public string ProjectStatusName { get; set; }

        [DisplayName("Type")]
        [UIHint("_ReadOnly")]
        public string ConstructionTypeName { get; set; }

        public int? ZoneId { get; set; }

        public string ZoneCode { get; set; }

        public List<ApprovalFlowViewModel> ApproverList { get; set; }

        #endregion

    }
}