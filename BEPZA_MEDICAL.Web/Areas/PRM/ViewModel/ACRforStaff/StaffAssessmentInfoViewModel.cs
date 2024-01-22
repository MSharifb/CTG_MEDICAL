using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforStaff
{
    public class StaffAssessmentInfoViewModel
    {
        #region Ctor
        public StaffAssessmentInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            this.AttributeDetailList = new List<StaffAssessmentInfoDetailViewModel>();
        }
        #endregion

        #region Standerd Property

        public int StaffInfoId { get; set; }
        public int EmployeeId { get; set; }
        public string OverAllAssessment { get; set; }
        public string QualificationForPromotion { get; set; }
        public int ReportingOfficerId { get; set; }
        [Display(Name = "Date")]
        [UIHint("_Date")]
        public DateTime? ReportingDate { get; set; }
        [Display(Name = "General Remarks")]
        public string GeneralRemarks { get; set; }
        public int? ReviewingOfficerId { get; set; }
        [Display(Name = "Date")]
        [UIHint("_Date")]
        public DateTime? ReviewingDate { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        //reporting officer
        [Display(Name = "Assessment Employee")]
        public string NameOfReportingOfficer { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }

        //Reviewing Officer
        [Display(Name = "Report Initiating Officer")]
        public string NameOfReviewingOfficer { get; set; }
        [Display(Name = "Designation")]
        public string ReDesignation { get; set; }
        [Display(Name = "Department")]
        public string ReDepartment { get; set; }

        public int ApproverId { get; set; }

        [DisplayName("Forward To")]
        [Required]
        public int NextApproverId { get; set; }

        public int ApprovalStatusId { get; set; }

        [DisplayName("Comments")]
        public string ApproverComments { get; set; }

        public IList<SelectListItem> ApproverList { get; set; }

        #endregion

        #region Officer's Info

        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Designation")]
        public string EmployeeDesignation { get; set; }
        [Display(Name = "Department")]
        public string EmployeeDepartment { get; set; }
        [Display(Name = "Annual Confidential Report Date")]
        public string ACRDate { get; set; }
        [Display(Name = "ACR Period From")]
        public string ACRPeriodFrom { get; set; }
        [Display(Name = "ACR Period To")]
        public string ACRPeriodTo { get; set; }
        [Display(Name = "Seniority Serial Number")]
        public string SeniorityNumber { get; set; }

        #endregion

        #region Other

        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public string SelectedClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ErrorClass { get; set; }
        public int IsError { get; set; }
        public bool isExist { get; set; }
        public bool IsPhotoExist { get; set; }

        public int AttributeId { get; set; }

        public bool Excellent { get; set; }
        public bool Good { get; set; }
        public bool Standard { get; set; }
        public bool BelowStnd { get; set; }
        public bool NotExpt { get; set; }

        public bool A { get; set; }
        public bool B { get; set; }
        public bool C { get; set; }
        public bool D { get; set; }

        public IList<StaffAssessmentInfoDetailViewModel> AttributeDetailList { get; set; }

        public string ApprovalActionName { get; set; }

        #endregion

    }
}