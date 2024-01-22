using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class ReviewingOfficerComments
    {
        #region Standard

        public int OfficerInfoId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "General Remarks")]
        public string GeneralRemarks { get; set; }
        [Display(Name = "Total Marks based on Overall Assessment")]
        public decimal? TotalMarks { get; set; }
        public int ReviewingOfficerId { get; set; }
        [Display(Name="Date")]
        [UIHint("_Date")]
        public DateTime? ReviewingDate { get; set; }
        [Display(Name = "Counter Signing Officer (CSO)")]
        public string NameOfReportingOfficer { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }

        public int ApproverId { get; set; }

        [DisplayName("Forward To")]
        [Required]
        public int NextApproverId { get; set; }

        public int ApprovalStatusId { get; set; }

        [DisplayName("Comments")]
        public string ApproverComments { get; set; }

        public IList<SelectListItem> ApproverList { get; set; }		

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

    }
}