using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.ShiftAssigment
{

    public partial class ShiftAssignmentViewModel : BaseViewModel
    {
        public ShiftAssignmentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
            this.ShiftAssignment = new List<ShiftAssignmentViewModel>();

            this.Employee = new List<EmployeeVm>();
        }

        #region Standard Property
        [Required]
        [DisplayName("I.C. No.")]
        public int EmployeeId { get; set; }
        [Required]
        [DisplayName("Department Name")]
        public int DepartmentId { get; set; }
        [Required]
        [DisplayName("Shift Group")]
        public Nullable<int> RosterId { get; set; }
        [Required]
        [DisplayName("Shift Name")]
        public int ShiftId { get; set; }
        [Required]
        [DisplayName("Rest Day")]
        public string RestDay { get; set; }
        [Required]
        [DisplayName("Apply Type")]
        public int ApplyType { get; set; }
        [Required]
        [DisplayName("Shift Type")]
        public int ShiftType { get; set; }
        [Required]
        [DisplayName("Effective From Date")]
         [UIHint("_Date")] 
        public DateTime FromDate { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Effective To Date")]
        public DateTime ToDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsResterDayActive { get; set; }

        public List<ShiftAssignmentViewModel> ShiftAssignment { get; set; }

        #endregion

        #region Others
        public string EmpInitial { get; set; }
        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }

        public string GradeName { get; set; }
        [DisplayName("Grade")]
        public int GradeId { get; set; } 
        public string Mode { get; set; }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public IList<SelectListItem> Department { get; set; }
        public IList<SelectListItem> Grade { get; set; }
        public IList<SelectListItem> ShiftGroup { get; set; }
        public IList<SelectListItem> Shift { get; set; }
        public IList<SelectListItem> RestDayList { get; set; }

        public IList<SelectListItem> ApplyTypeList { get; set; }

        public IList<SelectListItem> ShiftTypeList { get; set; }
        public List<EmployeeVm> Employee { get; set; }
        #endregion

    }

    public class EmployeeVm
    {
        public int EmployeeId { get; set; }

        public string ICNo { get; set; }
        public string EmpInitial { get;set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public bool IsChecked { get; set; }
    }
  
}
