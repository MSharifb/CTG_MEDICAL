using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class OfficerPerformanceofWorkViewModel
    {
        #region Ctor
        public OfficerPerformanceofWorkViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            this.MarkList = new List<OfficerPerformanceofWorkViewModel>();
            this.AttributeList = new List<OfficerPerformanceofWorkViewModel>();
            this.AttributeDetailList = new List<OfficerPerformanceofWorkDetailViewModel>();
        }

        #endregion

        #region Standard

        public int OfficerInfoId { get; set; }
        public int EmployeeId { get; set; }
        public decimal? TotalObtainMarks { get; set; }

        public int ReportingOfficerId { get; set; }
        [Display(Name = "Report Initiating Officer")]
        public string NameOfReportingOfficer { get; set; }
        public string Designation { get; set; }


        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public decimal? SerialNumber { get; set; }

        public bool ChkFour { get; set; }
        public bool ChkThree { get; set; }
        public bool CkhTwo { get; set; }
        public bool ChkOne { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

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
        public decimal? PersonalCharacterMarks { get; set; }

        public IList<OfficerPerformanceofWorkViewModel> MarkList { get; set; }
        public IList<OfficerPerformanceofWorkViewModel> AttributeList { get; set; }
        public IList<OfficerPerformanceofWorkDetailViewModel> AttributeDetailList { get; set; }
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

        #region Rank
        public string RankName { get; set; }
        public decimal? FromMark { get; set; }
        public decimal? ToMark { get; set; }
        #endregion
    }
}