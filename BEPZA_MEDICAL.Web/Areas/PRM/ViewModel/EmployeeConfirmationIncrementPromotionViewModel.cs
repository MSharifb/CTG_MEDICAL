using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeConfirmationIncrementPromotionViewModel
    {
        public EmployeeConfirmationIncrementPromotionViewModel()
        {
            this.DesignationList = new List<SelectListItem>();
            this.EmploymentTypeList = new List<SelectListItem>();
            this.GradeList = new List<SelectListItem>();
            this.StepList = new List<SelectListItem>();
            this.ZoneListByUser = new List<SelectListItem>();
            this.ToEmploymentProcessList = new List<SelectListItem>();
            this.ToRegionList = new List<SelectListItem>();
            this.ToZoneInfoList = new List<SelectListItem>();
        }

        [Required]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        [Required]
        public string EmpId { set; get; }

        //[Required]
        public int? FromDisciplineId { get; set; }

        //[Required]
        public int? ToDisciplineId { get; set; }

        [DisplayName("Department")]
        //  [Required]
        public int? FromDivisionId { set; get; }
        public string FromDivisionName { set; get; }

        // [Required]
        public int? ToDivisionId { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { set; get; }

        [DisplayName("Date of Joining")]
        [UIHint("_ReadOnlyDate")]
        //[ReadOnly(true)] 
        public DateTime? JoiningDate { set; get; }

        [DisplayName("Order Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? OrderDate { set; get; }

        [DisplayName("Effective Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? EffectiveDate { set; get; }

        public string Type { get; set; }

        //[Required]
        //[DisplayName("Job Location")]
        //public int FromJobLocationId { set; get; }
        //public string FromJobLocationName { set; get; }

        //[Required]
        //[DisplayName("To Job Location")]
        //public int ToJobLocationId { set; get; }
        //public IList<SelectListItem> ToJobLocationList { set; get; }


        [DisplayName("From Employment Process")]
        public int? FromEmploymentProcessId { set; get; }
        public string FromEmploymentProcessName { set; get; }


        [DisplayName("To Employment Process")]
        public int? ToEmploymentProcessId { set; get; }
        public IList<SelectListItem> ToEmploymentProcessList { set; get; }

        [DisplayName("Status Designation")]
        [Required]
        public int FromDesignationId { set; get; }
        public string FromDesignation { set; get; }
        public IList<SelectListItem> DesignationList { set; get; }

        [DisplayName("Status Designation")]
        [Required]
        public int ToDesignationId { set; get; }
        public string ToDesignation { get; set; }

        [DisplayName("Employment Type")]
        [Required]
        public int FromEmploymentTypeId { set; get; }
        public string FromEmploymentType { set; get; }
        public IList<SelectListItem> EmploymentTypeList { set; get; }

        [DisplayName("Employment Type")]
        [Required]
        public int ToEmploymentTypeId { set; get; }
        public string ToEmploymentType { get; set; }

        [DisplayName("Grade")]
        [Required]
        public int FromGradeId { set; get; }
        public string FromGrade { set; get; }
        public IList<SelectListItem> GradeList { set; get; }

        [DisplayName("Grade")]
        [Required]
        public int ToGradeId { set; get; }
        public string ToGrade { set; get; }

        [DisplayName("Step")]
        [Required]
        public int FromStepId { set; get; }

        public int? FromStep { set; get; }
        public IList<SelectListItem> StepList { set; get; }

        [DisplayName("Step")]
        [Required]
        public int ToStepId { set; get; }

        [DisplayName("Salary Scale")]
        [Required]
        public int? FromSalaryScaleId { get; set; }
        public string FromSalaryScale { set; get; }

        [DisplayName("Salary Scale")]
        [Required]
        public int? ToSalaryScaleId { get; set; }
        public string ToSalaryScale { get; set; }
        public IList<SelectListItem> SalaryScaleList { set; get; }

        [Description("Fill directly from controller")]
        public bool FromIsConsolidated { set; get; }

        [Description("Fill directly from controller")]
        public bool ToIsConsolidated { set; get; }
        //------------------------------------------------------

        [DisplayName("From Organogram Level")]
        [Required]
        public int FromOrganogramLevelId { set; get; }
        public string FromOrganogramLevelName { set; get; }

        [DisplayName("To Organogram Level")]
        //[Required]
        public int? ToOrganogramLevelId { set; get; }
        public string ToOrganogramLevelName { set; get; }

        public string OrganogramLevelDetail { set; get; }

        [DisplayName("Order No.")]
        public string OrderNo { set; get; }

        [DisplayName("Remarks")]
        public string Remarks { set; get; }

        [DisplayName("Add Attachment")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        //-------------------------------------------------------

        [DisplayName("Basic")]
        [Required]
        [UIHint("_ReadOnlyAmount")]
        public decimal FromBasicSalary { set; get; }

        [DisplayName("Basic")]
        [UIHint("_ReadOnlyAmount")]
        [Required]
        public decimal? ToBasicSalary { set; get; }

        [DisplayName("Gross")]
        [Required]
        [UIHint("_ReadOnlyAmount")]
        public decimal FromGrossSalary { set; get; }

        [DisplayName("Gross")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public decimal? ToGrossSalary { set; get; }

        [DisplayName("Increment")]
        [Required]
        [UIHint("_ReadOnlyAmount")]
        public decimal IncrementAmount { set; get; }
        public decimal InitialBasic { set; get; }
        public decimal YearlyIncrement { set; get; }
        // public DateTime? DateofConfirmation { set; get; }
        public string DateofConfirmation { set; get; }
        public bool IsEffective { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public string strMode { set; get; }


        //transfer info
        [DisplayName("From Zone/Executive Office")]
        public int FromZoneInfoId { get; set; }

        [DisplayName("From Zone/Executive Office")]
        public string FromZoneName { get; set; }

        [DisplayName("To Zone/Executive Office")]
        public int? ToZoneInfoId { get; set; }
        public IList<SelectListItem> ToZoneInfoList { set; get; }

        [DisplayName("Transfer Type")]
        public bool IsOutofOfficeOrDeputaion { set; get; }

        [DisplayName("Organization Name")]
        public string OrganizationName { set; get; }

        [DisplayName("Department Name")]
        public string DepartmentName { set; get; }

        [DisplayName("Section Name")]
        public string SectionName { set; get; }

        [DisplayName("Address")]
        public string Address { set; get; }

        [Required, DisplayName("From Region")]
        public int FromRegionId { get; set; }

        [DisplayName("From Region")]
        public string FromRegionName { get; set; }

        [Required, DisplayName("To Region")]
        public int ToRegionId { get; set; }
        public IList<SelectListItem> ToRegionList { set; get; }

        [NotMapped, DisplayName("Zone List By User")]
        public int ZoneListByUserId { get; set; }
        public IList<SelectListItem> ZoneListByUser { set; get; }

        public String NotifyTo { get; set; }
    }
}