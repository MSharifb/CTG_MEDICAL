using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class OfflineWelfareFundApplicationInformationViewModel: BaseViewModel
    {
        #region Ctor
        public OfflineWelfareFundApplicationInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.ApplicationDate = DateTime.UtcNow;

            this.WelfareFundCategoryList = new List<SelectListItem>();
            this.BankNameList = new List<SelectListItem>();
            this.BranchNameList = new List<SelectListItem>();
            this.RelationList = new List<SelectListItem>();
            this.ReasonList = new List<SelectListItem>();
            this.HistoryList = new List<OfflineWelfareFundApplicationInformationViewModel>();

            ApplicationStatusList = new List<SelectListItem>();
            SignatoryList = new List<SelectListItem>();

        }
        #endregion

        #region Standard Property

        public string ApplicantStatus { get; set; }
        [DisplayName("Applicant Name")]
        public string ApplicantName { get; set; }
        [DisplayName("Relation")]
        public int? RelationId { get; set; }
        public string Address { get; set; }
        [DisplayName("Bank Name")]
        public int? BankNameId { get; set; }
        [DisplayName("Branch Name")]
        public int? BranchNameId { get; set; }
        [DisplayName("Bank Account No.")]
        public string BankAccountNo { get; set; }
        [DisplayName("Mobile No.")]
        public string MobileNo { get; set; }
        public string NID { get; set; }

        [DisplayName("Employee ID")]
        public int EmployeeId { get; set; }
        [DisplayName("Application Date")]
        [UIHint("_Date")]
        public DateTime? ApplicationDate { get; set; }

        [DisplayName("Ref. No")]
        public string RefNo { get; set; }

        [DisplayName("Application No.")]
        public string ApplicationNo { get; set; }

        [DisplayName("Fund Category")]
        public int WelfareFundCategoryId { get; set; }
        [DisplayName("Applied Amount")]
        public decimal AppliedAmount { get; set; }

        [DisplayName("Reason")]
        [MaxLength(250, ErrorMessage = "Maximum length 250.")]
        public string Reason { get; set; }
        [DisplayName("To")]
        public string AppTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        [DisplayName("Forward To")]
        public int SignatoryId { get; set; }

        public string Comments { get; set; }

        public int ZoneInfoId { get; set; }

        [DisplayName("Application Status")]
        public int ApplicationStatusId { get; set; }

        public int CycleId { get; set; }
        public bool IsConfigurableApprovalFlow { get; set; }
        #endregion

        #region Other
        [Display(Name = "Employee ID")]
        public string EmpID { get; set; }
        [Display(Name = "Department")]
        public string  Department { get; set; }
        public string Name { get; set; }
        [Display(Name = "Joining Date")]
        public string JoiningDate { get; set; }
        public string Designation { get; set; }
        [Display(Name = "Confirmation Date")]
        public string ConfirmationDate { get; set; }
        [Display(Name = "Service Duration")]
        public string ServiceDuration { get; set; }

        public IList<SelectListItem> WelfareFundCategoryList { get; set; }
        public IList<SelectListItem> BankNameList { get; set; }
        public IList<SelectListItem> BranchNameList { get; set; }
        public IList<SelectListItem> RelationList { get; set; }
        public IList<SelectListItem> ReasonList { get; set; }
        public IList<OfflineWelfareFundApplicationInformationViewModel> AttachmentFilesList { get; set; }
        public IList<OfflineWelfareFundApplicationInformationViewModel> HistoryList { get; set; }
        [DisplayName("Employee Id")]
        public string SignatoryEmpId { get; set; }
        [DisplayName("Name")]
        public string SignatoryEmpName { get; set; }
        [DisplayName("Designation")]
        public string SignatoryEmpDesignation { get; set; }
        [DisplayName("Phone")]
        public string SignatoryEmpPhone { get; set; }
        [DisplayName("Email")]
        public string SignatoryEmpEmail { get; set; }
        public string WelfareFundCategoryName { get; set; }
        public DateTime? ApplicationFromDate { get; set; }
        public DateTime? ApplicationToDate { get; set; }
        public decimal? MaxAmount { get; set; }

        public decimal? SuggestAmount { get; set; }

        public IList<SelectListItem> ApplicationStatusList { get; set; }

        public IList<SelectListItem> SignatoryList { get; set; }

        public string ApplicationStatusName { get; set; }

        public List<ApprovalHistoryViewModel> ApprovalHistory { get; set; }


        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public string Title { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

        #region Signature

        [DisplayName("Signature")]
        public byte[] SignatureAttachment { set; get; }
        public HttpPostedFileBase SignatureFile { get; set; }
        public string SignatureFileName { get; set; }
        public string SignatureFilePath { get; set; }

        #endregion
    }
}