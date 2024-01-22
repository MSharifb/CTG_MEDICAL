using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryStructure;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee
{
    public class EmployeeSalaryStructureViewModel
    {
        #region Ctor

        public EmployeeSalaryStructureViewModel()
        {
            this.EmploymnetInfo = new EmploymentInfoViewModel();
            this.SalaryStructureDetail = new List<SalaryStructureDetailsModel>();

            this.SalaryScaleList = new List<SelectListItem>();
            this.GradeList = new List<SelectListItem>();
            this.StepList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property

        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        public virtual string EmpCode { get; set; }

        //[Required]
        [DisplayName("Employee Initial")]
        [StringLength(3)]
        [UIHint("_ReadOnly")]
        public virtual string EmployeeInitial { get; set; }

        [Required]
        [MaxLength(200)]
        [UIHint("_ReadOnly")]
        [DisplayName("Full Name")]
        public virtual string FullName { get; set; }

        public virtual Nullable<System.DateTime> DateofInactive { get; set; }

        public virtual int EmployeeId { get; set; }

        [DisplayName("Job Grade")]
        [Required]
        public virtual int GradeId{ get; set; }

        [Required]
        //[UIHint("_ReadOnly")]
        public string JobGrade { get; set; }

        public IEnumerable<SelectListItem> GradeList { get; set; }
        
        [DisplayName("Salary Scale")]
        [Required]
        public virtual int SalaryScaleId { get; set; }

        [Required]
        //[UIHint("_ReadOnly")]
        public string SalaryScale { get; set; }

        public IEnumerable<SelectListItem> SalaryScaleList { get; set; }
        
        [DisplayName("Step Number")]
        [Required]
        public virtual int StepId{ get; set; }

        public IEnumerable<SelectListItem> StepList { get; set; }

        [DisplayName("Gross Salary")]
        [Required]
        public virtual decimal GrossSalary{ get; set; }

        [DisplayName("Is Consolidated Structure?")]
        public virtual bool isConsolidated{ get; set; }

        [DisplayName("Total Addition")]
        [UIHint("_ReadOnlyAmount")]
        public decimal TotalAddition { get; set; }

        [DisplayName("Total Deduction")]
        [UIHint("_ReadOnlyAmount")]
        public decimal TotalDeduction { get; set; }

        [DisplayName("Net Pay")]
        [UIHint("_ReadOnlyAmount")]
        public decimal NetPay { get; set; }

        public virtual int SalaryStructureId{ get; set; }

        [DisplayName("Original Gross Salary")]
        public virtual decimal OrgGrossSalary{ get; set; }

        #endregion

        #region Association
        public EmploymentInfoViewModel EmploymnetInfo { get; set; }
        public ICollection<SalaryStructureDetailsModel> SalaryStructureDetail { get; set; }

        #endregion

        #region Others
        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }
        public string SelectedClass{ get; set; }
        public string ErrorClass { get; set; }
        public int IsError { get; set; }
        public bool IsSalaryProcess { get; set; }
        #endregion
    }
}