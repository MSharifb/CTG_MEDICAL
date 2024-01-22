using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.OverTimeRuleAssignment
{
    public class OverTimeRuleAssignmentViewModel
    {

        #region Ctor
        public OverTimeRuleAssignmentViewModel()
        {
            this.RuleNameList = new List<SelectListItem>();
            this.Departmentlist = new List<SelectListItem>();
            this.CategoryList = new List<SelectListItem>();
            this.GradeList = new List<SelectListItem>();
            this.EmploymentTypeList = new List<SelectListItem>();
            this.EmployeeClassList = new List<SelectListItem>();

            this.IsActive = true;

            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        [DisplayName("Overtime Rule Name")]
        public int OverTimeId { get; set; }

        [DisplayName("Department Name")]
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public IList<SelectListItem> Departmentlist { set; get; }

        [DisplayName("Grade")]
        public Nullable<int> GradeId { get; set; }
        public IList<SelectListItem> GradeList { get; set; }
     
        public int? EmployeeId { get; set; }

        [DisplayName("Employee Category")]
        public Nullable<int> StaffCategoryId { get; set; }
        public IList<SelectListItem> CategoryList { get; set; }

        [DisplayName("Employee Type")]
        public Nullable<int> EmploymentTypeId { get; set; }
        public string EmploymentTypeName { get; set; }
        public IList<SelectListItem> EmploymentTypeList { get; set; }

        [DisplayName("Employee Class")]
        public Nullable<int> EmployeeClassId { get; set; }
        public string EmployeeClassName { get; set; }
        public IList<SelectListItem> EmployeeClassList { get; set; }

        public int TrLevel { get; set; }
        public bool IsActive { get; set; }
        [DisplayName("Information Type")]
        public string RuleName { get; set; }
        public IList<SelectListItem> RuleNameList { get; set; }

        #endregion

        #region Temporary Property
        [DisplayName("Information Type")]
        public string InfoType { get; set; }
        public int? DepId { get; set; }
        public int? GrdId { get; set; }
        public int? StaffCatgId { get; set; }
        public int? EmpTypeId { get; set; }
        public int? EmpClassId { get; set; }
        [DisplayName("I.C. No.")]
        public string EmpId { get; set; }
        public IList<SelectListItem> EmpIdList { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        [DisplayName("Designation")]
        public string Designation { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string GradeName { get; set; }

        public string StaffCategoryName { get; set; }
        #endregion

        #region Other

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        #endregion
    }
}