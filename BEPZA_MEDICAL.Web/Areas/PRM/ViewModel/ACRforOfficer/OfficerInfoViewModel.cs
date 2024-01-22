using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class OfficerInfoViewModel
    {
        #region Ctor
        public OfficerInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #endregion

        #region Standerd Property
        public virtual int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Annual Confidential Report Date")]
        [UIHint("_Date")]
        public DateTime? ACRDate { get; set; }

        [Required]
        [Display(Name = "ACR Period From")]
        [UIHint("_Date")]
        public DateTime? ACRPeriodFrom { get; set; }

        [Required]
        [Display(Name = "ACR Period To")]
        [UIHint("_Date")]
        public DateTime? ACRPeriodTo { get; set; }
        public int ZoneInfoId { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        public string EmployeeDesignation { get; set; }

        public string EmployeeDepartment { get; set; }

        [Display(Name = "Seniority Serial Number")]
        public string SeniorityNumber { get; set; }

        #region Reporting Officer
        public int ReportingOfficerId { get; set; }
        [Display(Name = "Officer Reported Under (ORU)")]
        public string NameOfReportingOfficer { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Date")]
        [UIHint("_Date")]
        public DateTime? ReportingDate { get; set; }
        #endregion

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        public string DateofJoining { get; set; }
        public string PRLDate { get; set; }

        public int ApproverId { get; set; }

        [DisplayName("Forward To")]
        [Required]
        public int NextApproverId { get; set; }

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

        public IList<SelectListItem> ApproverList { get; set; }
        public bool ReportingDateEditable { get; set; }

        #endregion
    }
}