using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_BEPZA.Web.Areas.AMS.Models.OutStationDuty
{
    public class OutStationDuty
    {
        #region Ctor
        public OutStationDuty()
        {
            this.FromDate = DateTime.UtcNow;
            this.ToDate = DateTime.UtcNow;

            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        [Required]
        public int? EmployeeId { get; set; }

        [Required]
        [Display(Name = "From Date")]
        [UIHint("_Date")]
        public DateTime FromDate { get; set; }

        [Required]
        [Display(Name = "To Date")]
        [UIHint("_Date")]
        public DateTime ToDate { get; set; }

        [Required]
        [Display(Name = "No. of Days")]
        public int TotalDays { get; set; }

        [Required]
        public string Purpose { get; set; }

        public string Destination { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Department Name")]
        public string DepartmentName { get; set;}

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        #endregion

        #region Other
        [Display(Name = "I.C No.")]
        public string EmpId { get; set; }
        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        #endregion
    }
}