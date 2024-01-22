using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.AssignHolidayAllowanceRule
{
    public class AssignHolidayAllowanceRuleViewModel : BaseViewModel
    {
        #region Ctor
        public AssignHolidayAllowanceRuleViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.IsActive = true;

            this.HolidayAllowanceRuleNameList = new List<SelectListItem>();
            this.Departmentlist = new List<SelectListItem>();
            this.EmployeeCategoryList = new List<SelectListItem>();
            this.EmploymentTypeList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        [DisplayName("Weekend/Holiday Allowance Rule Name")]
        public int HolidayAllowanceRuleId { get; set; }
        public string RuleName { get; set; }
        public IList<SelectListItem> HolidayAllowanceRuleNameList { get; set; }

        public int? EmployeeId { get; set; }

        [DisplayName("Department Name")]
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public IList<SelectListItem> Departmentlist { set; get; }

        [DisplayName("Employee Category")]
        public int? EmployeeCategoryId { get; set; }
        public string EmployeeCategoryName { get; set; }
        public IList<SelectListItem> EmployeeCategoryList { get; set; }

        [DisplayName("Employment Type")]
        public int? EmployeeTypeId { get; set; }
        public string EmploymentTypeName { get; set; }
        public IList<SelectListItem> EmploymentTypeList { get; set; }

        public bool IsAll { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Other
        [DisplayName("I.C. No.")]
        public string EmpId { get; set; }
        public string InfoType { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        [DisplayName("Designation")]
        public string Designation { get; set; }

        #endregion
    }
}