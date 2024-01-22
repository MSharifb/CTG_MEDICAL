using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class InformationForAuthority
    {
        #region Standard

        public int OfficerInfoId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "Receive Date of Filled up Form")]
        [UIHint("_Date")]
        public DateTime? ReceiveDate { get; set; }
        [Display(Name="Reason of Delay")]
        public string ReasonOfDelay { get; set; }
        [Display(Name = "Activities on Application (If Any)")]
        public string ActivitiesOnApplication { get; set; }
        public int ReceivingOfficerId { get; set; }
        [Display(Name = "Date")]
        [UIHint("_Date")]
        public DateTime? ReceivingDate { get; set; }
        [Display(Name = "Receiving Officer")]
        public string NameOfReportingOfficer { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }

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