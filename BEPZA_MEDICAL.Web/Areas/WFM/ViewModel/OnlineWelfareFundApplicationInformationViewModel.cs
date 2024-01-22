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
    public class OnlineWelfareFundApplicationInformationViewModel : BaseViewModel
    {
        #region Ctor
        public OnlineWelfareFundApplicationInformationViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
            strMode = "Create";
            ApplicationDate = DateTime.UtcNow;

            WelfareFundCategoryList = new List<SelectListItem>();
            ReasonList = new List<SelectListItem>();
            ApplicationStatusList = new List<SelectListItem>();
            SignatoryList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
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

        #endregion

        #region Other

        [Display(Name = "Employee ID")]
        public string EmpID { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        public string Name { get; set; }
        [Display(Name = "Joining Date")]
        public string JoiningDate { get; set; }
        public string Designation { get; set; }
        [Display(Name = "Confirmation Date")]
        public string ConfirmationDate { get; set; }
        [Display(Name = "Service Duration")]
        public string ServiceDuration { get; set; }

        public IList<SelectListItem> WelfareFundCategoryList { get; set; }
        public IList<SelectListItem> ReasonList { get; set; }
        public IList<OnlineWelfareFundApplicationInformationViewModel> AttachmentFilesList { get; set; }

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
        public bool IsConfigurableApprovalFlow { get; set; }
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