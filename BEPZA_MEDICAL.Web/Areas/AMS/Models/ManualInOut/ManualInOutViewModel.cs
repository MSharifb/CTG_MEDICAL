using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.ManualInOut
{
    public class ManualInOutViewModel
    {
        #region Ctor
        public ManualInOutViewModel()
        {
            this.DepartmentList = new List<SelectListItem>();
            this.ShiftNameList = new List<SelectListItem>();
            //this.AttendanceDate = DateTime.UtcNow;
            this.IsDevice = false;

            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
            this.Employee = new List<EmployeeAttendance>();
            this.ManualInOut = new List<ManualInOutViewModel>();
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        [Display(Name = "Attendance Date")]
        [UIHint("_RequiredDate")]
        //public DateTime AttendanceDate { get; set; }
        public Nullable<System.DateTime> AttendanceDate { get; set; }


        [Required]
        public int EmployeeId { get; set; }

        [Display(Name = "In Time")]
        public Nullable<System.TimeSpan> InTime { get; set; }

        [Display(Name = "Out Time")]
        public Nullable<System.TimeSpan> OutTime { get; set; }

        [Required]
        [Display(Name = "Shift Name")]
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public IList<SelectListItem> ShiftNameList { get; set; }

        public string Reason { get; set; }
        public bool IsDevice { get; set; }
        public Nullable<int> DeviceNo { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; } 
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        [Display(Name="Department Name")]
        public Nullable<int> DepartmentId { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }

        [Display(Name="Shift In Time")]
        public System.TimeSpan ShiftInTime { get; set; }
        [Display(Name="Shift Out Time")]
        public System.TimeSpan ShiftOutTime { get; set; }
        public List<ManualInOutViewModel> ManualInOut { get; set; }

        #endregion

        #region Other
        public bool IsOSD { get; set; }
        public string CardId { get; set; }

        [Display(Name = "I.C. No.")]
        public string EmpId { get; set; }

        public bool IsMark { get; set; }

        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }

        public string CardNo { get; set; }

        public string EntryType { get; set; }
        public string ActionType { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public List<EmployeeAttendance> Employee { get; set; }
        #endregion
    }
    public class EmployeeAttendance
    {
        public string EmpId { get; set; }
        public bool IsMark { get; set; }
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
        public string EmpInitial { get; set; }
        public string EmployeeName { get; set; }
        public string CardNo { get; set; }
        public string DesignationName { get; set; }
        public System.TimeSpan InTime { get; set; }
        public System.TimeSpan OutTime { get; set; }
        public string Reason { get; set; }
        public bool IsChecked { get; set; }
    }
}