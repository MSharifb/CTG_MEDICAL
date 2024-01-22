using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.RequestedApplication
{
    public class RequestedApplicationViewModel : BaseViewModel
    {
        public RequestedApplicationViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            AprovalProcessList = new List<SelectListItem>();
            ApprovalStatusList = new List<SelectListItem>();
            ApproverActionList = new List<SelectListItem>();
            //ApplicationList = new List<RequestedApplicationViewModel>();
            NextApproverList = new List<SelectListItem>();
            ApprovalStepList = new List<SelectListItem>();
        }

        public int? ModuleId { get; set; }


        [DisplayName("Process Name")]
        public int? ApprovalProcessId { get; set; }

        public int? ApplicationId { get; set; }

        public int? ApprovalStepId { get; set; }

        public int? ApproverId { get; set; }

        [DisplayName("Status")]
        public int? ApprovalStatusId { get; set; }

        public int? ApprovalActionId { get; set; }

        [DisplayName("Comment")]
        public string ApproverComments { get; set; }

        #region Other Prop

        public DateTime ApplicationDate { get; set; }

        public string ApplicantId { get; set; }

        public string ApplicantName { get; set; }

        public string Department { get; set; }

        public string Designation { get; set; }

        public string Zone { get; set; }

        public decimal? RequestedAmount { get; set; }

        public decimal? ApprovedAmount { get; set; }

        public IList<SelectListItem> AprovalProcessList { get; set; }

        public IList<SelectListItem> ApprovalStatusList { get; set; }

        public string ApprovalStatusName { get; set; }

        public IList<SelectListItem> ApproverActionList { get; set; }

        public bool IsSelected { get; set; }

        //public List<RequestedApplicationViewModel> ApplicationList { get; set; }

        public bool? IsOnlineApplication { get; set; }

        public ViewApplicationViewModel Application { get; set; }

        public List<ApprovalHistoryViewModel> ApprovalHistory { get; set; }

        public List<INVRequisitionDtlViewModel> INVRequisitionDtlList { get; set; }


        public string NextStepName { get; set; }

        public IList<SelectListItem> NextApproverList { get; set; }

        public IList<SelectListItem> ApprovalStepList { get; set; }


        [DisplayName("Start Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime EndDate { get; set; }

        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public CpfApplicationViewModel CpfApplication { get; set; }

        public List<WFMAttachmentViewModel> AttachmentFilesList { get; set; }


        public string WelfareFundCategoryName { get; set; }
        public string WelfareFundReason { get; set; }
        public string WelfareFundAppliedAmount{ get; set; }
        public string WelfareFundApprovedAmount { get; set; }
        public string WelfareFundApplicationDate { get; set; }
        public string ApplicationStatus { get; set; }
        public IList<RequestedApplicationViewModel> WRMApplicantHistoryList { get; set; }
        public decimal? SuggestAmount { get; set; }
        public bool IsConfigurableApprovalFlow { get; set; }

        [DisplayName("Employee Id")]
        public string SignatoryEmpId { get; set; }
        [DisplayName("Name")]
        public string SignatoryEmpName { get; set; }
        [DisplayName("Designation")]
        public string SignatoryEmpDesignation { get; set; }

        #endregion
    }
}