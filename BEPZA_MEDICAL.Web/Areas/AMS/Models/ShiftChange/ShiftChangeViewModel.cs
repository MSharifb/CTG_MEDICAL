using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.ShiftChange
{
    public class ShiftChangeViewModel : BaseViewModel
    {
        public ShiftChangeViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
        }


        #region Standard Property

        [Required]
        [UIHint("_Date")]
        [DisplayName("Effective Date")]
        public System.DateTime EffectiveDate { get; set; }
       
        [Required]
        [DisplayName("Department Name")]
        public int? DepartmentId { get; set; }
        
        [DisplayName("Group")]
        public Nullable<int> RosterId { get; set; }
        [Required]
        [DisplayName("Shift")]
        public int ShiftId { get; set; }
        [Required]
        [DisplayName("Employee Name")]
        public int EmployeeId { get; set; }
        public bool IsRestDay { get; set; }
        public Nullable<int> ApplyType { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        #endregion

        #region Others
        public string RestDay { get; set; }
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }

        public string GradeName { get; set; }
         [DisplayName("Grade")]
        public int GradeId { get; set; }
        public string ShiftName { get; set; }
        public bool IsMark { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string Mode { get; set; }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public IList<SelectListItem> Department { get; set; }
        public IList<SelectListItem> Grade { get; set; }
        public IList<SelectListItem> ShiftGroup { get; set; }
        public IList<SelectListItem> Shift { get; set; }
        public IList<SelectListItem> ApplyTypeList { get; set; }

        #endregion

    }
}