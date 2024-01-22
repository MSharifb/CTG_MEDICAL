using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class VerifytheApplicationInfoViewModel : BaseViewModel
    {
        #region Ctor
        public VerifytheApplicationInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.VerifyDate = DateTime.UtcNow;

            this.WelfareFundCategoryList = new List<SelectListItem>();
            this.ZoneList = new List<SelectListItem>();
            this.DepartmentList= new List<SelectListItem>();
            this.SectionList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.CycleList = new List<SelectListItem>();
            this.EmployeeList = new List<VerifytheApplicationInfoViewModel>();
        }
        #endregion

        #region Standard Property
        [DisplayName("Executive Office/Zone")]
        public int ApplicantZoneInfoId { get; set; }

        [DisplayName("Verify Date")]
        [UIHint("_Date")]
        public DateTime? VerifyDate { get; set; }

        [DisplayName("Cycle Name")]
        public int? CycleId { get; set; }

        [DisplayName("Year")]
        public string Year { get; set; }

        [DisplayName("Fund Category")]
        public int? WelfareFundCategoryId { get; set; }

        public decimal? AppliedAmount { get; set; }

        public int VerifiedById { get; set; }

        public string Comments { get; set; }

        public int ZoneInfoId { get; set; }

        public string ApplicationStatus { get; set; }
        #endregion

        #region Other
        [DisplayName("Employee Id")]
        public string SearchEmpId { get; set; }
        [DisplayName("Department")]
        public int? DepartmentId { get; set; }
        [DisplayName("Section")]
        public int? SectionId { get; set; }
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }


        public IList<SelectListItem> WelfareFundCategoryList { get; set; }
        public IList<SelectListItem> ZoneList { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }
        public IList<SelectListItem> SectionList { get; set; }
        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> CycleList { get; set; }
        public List<VerifytheApplicationInfoViewModel> EmployeeList { get; set; }

        [DisplayName("Name")]
        public string VerifiedByName { get; set; }
        [DisplayName("Designation")]
        public string VerifiedByDesignation { get; set; }
        [DisplayName("Department")]
        public string VerifiedByDepartment { get; set; }
        public string WelfareFundCategoryName { get; set; }
        [DisplayName("Cycle Name")]
        public string CycleName { get; set; }

        public string Subject { get; set; }
        #endregion

        #region Detail

        public string EmpId { get; set; }
        public int EmployeeId { get; set; }
        public bool Status { get; set; }
        public bool IsCheckedFinal { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string ApplieedAmount { get; set; }
        public string AppliedDate { get; set; }
        public DateTime DBAppliedDate { get; set; }

        #endregion

    }
}