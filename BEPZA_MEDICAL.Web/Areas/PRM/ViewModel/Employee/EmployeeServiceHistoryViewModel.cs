using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee
{
    public class EmployeeServiceHistoryViewModel : BaseViewModel
    {
        public EmployeeServiceHistoryViewModel()
        {
            this.FromDivisionList = new List<SelectListItem>();
            this.ToDivisionList = new List<SelectListItem>();
            this.FromDisciplineList = new List<SelectListItem>();
            this.ToDisciplineList = new List<SelectListItem>();
            this.FromEmploymentTypeList = new List<SelectListItem>();
            this.ToEmploymentTypeList = new List<SelectListItem>();
            this.FromEmploymentProcessList = new List<SelectListItem>();
            this.FromDesignationList = new List<SelectListItem>();
            this.ToDesignationList = new List<SelectListItem>();
            this.FromSalaryScaleList = new List<SelectListItem>();
            this.ToSalaryScaleList = new List<SelectListItem>();
            this.FromGradeList = new List<SelectListItem>();
            this.ToGradeList = new List<SelectListItem>();
            this.FromStepList = new List<SelectListItem>();
            this.ToStepList = new List<SelectListItem>();
            this.FromZoneInfoList = new List<SelectListItem>();
            this.ToZoneInfoList = new List<SelectListItem>();
            this.ToEmploymentProcessList = new List<SelectListItem>();
            this.TypeList = new List<SelectListItem>();
        }
        [Required]
        [DisplayName("Type")]
        public string Type { get; set; }
        public IList<SelectListItem> TypeList { get; set; }

        [Required]
        [DisplayName("Employee ID")]
        public string EmpId { get; set; }
        public int EmployeeId { get; set; }

        [DisplayName("Order number")]
        public string OrderNo { set; get; }

        [DisplayName("Order Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? OrderDate { get; set; }

        [DisplayName("Effective Date")]
        [UIHint("_Date")]
        public DateTime? EffectiveDate { get; set; }

        [DisplayName("From Department")]
        public int? FromDivisionId { set; get; }

        [DisplayName("FromDivisionName")]
        public string FromDivisionName { set; get; }
        public IList<SelectListItem> FromDivisionList { set; get; }

        [DisplayName("To Department")]
        public int? ToDivisionId { set; get; }
        public IList<SelectListItem> ToDivisionList { set; get; }

        [DisplayName("From Office")]
        public int? FromDisciplineId { get; set; }
        public IList<SelectListItem> FromDisciplineList { set; get; }

        [DisplayName("To Office")]
        public int? ToDisciplineId { set; get; }
        public IList<SelectListItem> ToDisciplineList { set; get; }

        [DisplayName("From Employment Type")]
        public int? FromEmploymentTypeId { set; get; }

        public string FromEmploymentType { set; get; }
        public IList<SelectListItem> FromEmploymentTypeList { set; get; }

        [DisplayName("To Employment Type")]
        public int? ToEmploymentTypeId { set; get; }

        public string ToEmploymentType { get; set; }
        public IList<SelectListItem> ToEmploymentTypeList { set; get; }

        [DisplayName("From Employment Process")]
        public int? FromEmploymentProcessId { set; get; }
        public string FromEmploymentProcessName { set; get; }
        public IList<SelectListItem> FromEmploymentProcessList { set; get; }

        [DisplayName("To Employment Process")]
        public int? ToEmploymentProcessId { get; set; }
        public IList<SelectListItem> ToEmploymentProcessList { set; get; }

        [DisplayName("From Designation")]
        public int? FromDesignationId { set; get; }
        public IList<SelectListItem> FromDesignationList { set; get; }

        [DisplayName("To Designation")]
        public int? ToDesignationId { set; get; }
        public string ToDesignation { get; set; }
        public IList<SelectListItem> ToDesignationList { set; get; }

        [DisplayName("From Salary Scale")]
        public int? FromSalaryScaleId { set; get; }
        public IList<SelectListItem> FromSalaryScaleList { set; get; }

        [DisplayName("To Salary Scale")]
        public int? ToSalaryScaleId { get; set; }
        public string ToSalaryScale { get; set; }
        public IList<SelectListItem> ToSalaryScaleList { set; get; }

        [DisplayName("From Grade")]
        public int? FromGradeId { set; get; }
        public IList<SelectListItem> FromGradeList { set; get; }

        [DisplayName("To Grade")]
        public int? ToGradeId { set; get; }
        public IList<SelectListItem> ToGradeList { set; get; }

        [DisplayName("From Step")]
        public int? FromStepId { get; set; }
        public IList<SelectListItem> FromStepList { set; get; }

        [DisplayName("To Step")]
        public int? ToStepId { set; get; }
        public IList<SelectListItem> ToStepList { set; get; }

        [DisplayName("From Zone/Executive Office")]
        public int? FromZoneInfoId { set; get; }
        [DisplayName("From Zone/Executive Office")]
        public string FromZoneName { get; set; }
        public IList<SelectListItem> FromZoneInfoList { set; get; }

        [DisplayName("To Zone/Executive Office")]
        public int? ToZoneInfoId { get; set; }
        public IList<SelectListItem> ToZoneInfoList { set; get; }


        #region Others
        public string Designation { get; set; }
        public string EmployeeName { get; set; }
        #endregion
    }
}