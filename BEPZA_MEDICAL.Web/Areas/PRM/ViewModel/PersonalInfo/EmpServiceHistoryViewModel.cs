using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmpServiceHistoryViewModel : BaseViewModel
    {
        #region Ctor
        public EmpServiceHistoryViewModel()
        {
            this.DesignationList = new List<SelectListItem>();         
            this.SalaryScaleIdList = new List<SelectListItem>();
            this.JobGradeList = new List<SelectListItem>();
            this.StaffCategoryList = new List<SelectListItem>();
            this.EmployeeClassList = new List<SelectListItem>();
            this.EmploymentTypeList = new List<SelectListItem>();
            this.EmploymentProcessList = new List<SelectListItem>();

            this.EmpTop = new EmpTop();

        }
        #endregion

        #region Standard Property
        
        public int EmployeeId { set; get; }

        [DisplayName("Designation")]
        [Required]
        public int DesignationId { set; get; }
        public IList<SelectListItem> DesignationList { set; get; }

        public int OfficeId { get; set; }
        public int? DepartmentId { get; set;}
        public int? SectionId { get; set; }

        [DisplayName("Date of Appoinment")]
        [UIHint("_Date")]
        public DateTime? OrderDate { set; get; }


        [DisplayName("Joining Date/Effective Date")]
        [Required]
        [UIHint("_RequiredDate")]
        public DateTime? EffectiveDate { set; get; }
       
        [DisplayName("Employee Type")]
        public int? StaffCategoryId { get; set; }
        public IList<SelectListItem> StaffCategoryList { set; get; }

        [DisplayName("Salary Scale")]
        public int? SalaryScaleId { get; set; }
        public IList<SelectListItem> SalaryScaleIdList { set; get; }
       

        [DisplayName("Grade")]    
        public int? JobGradeId { get; set; }
        public IList<SelectListItem> JobGradeList { set; get; }

        [DisplayName("Pay Scale")]
        [UIHint("_ReadOnly")]
        public string SalaryScaleName { get; set; }

        [DisplayName("Employee Class")]
        public int? EmployeeClassId { get; set; }
        public IList<SelectListItem> EmployeeClassList { set; get; }

        [DisplayName("Employee Category")]
        public int? EmploymentTypeId { get; set; }
        public IList<SelectListItem> EmploymentTypeList { set; get; }

        [DisplayName("Employee Process")]
        public int? EmploymentProcessId { get; set; }
        public IList<SelectListItem> EmploymentProcessList { set; get; }
        
        public int OrganogramLevelId { get; set; }

        [UIHint("_MultiLine")]
        public string Remarks { set; get; }

        [DisplayName("Duration(Year)")]
        [Range(1, 100)]
        public decimal? Duration { get; set;}

        #endregion

        #region Others      
        public EmpTop EmpTop { get; set; }

        [DisplayName("Organogram Level Name")]
        [UIHint("_ReadOnly")]
        [Required]
        public string OrganogramLevelName { get; set; }     
        #endregion

    }
}