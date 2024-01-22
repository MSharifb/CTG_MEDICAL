using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SuspensionOfEmployeeViewModel:BaseViewModel
    {
        #region Ctor
        public SuspensionOfEmployeeViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.StatusList= new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.SuspensionOfEmployeeDetailList = new List<SuspensionOfEmployeeViewModel>();
            this.SuspensionOfEmployeeDeductionList = new List<SuspensionOfEmployeeDetailViewModel>();
        }
        #endregion

        #region Standard

        public int EmployeeId { get; set; }
        public int SalaryStructureId { get; set; }
        public int SalaryScaleId { get; set; }
        public int GradeId { get; set; }
        public int StepId { get; set; }
        public decimal GrossSalary { get; set; }
        public bool isConsolidated { get; set; }
        public bool IsSalaryProcessed { get; set; }
        [Display(Name = "Suspension Date")]
        [UIHint("_RequiredDate")]
        public DateTime? SuspensionDate { get; set; }
        [Display(Name = "Effective From")]
        [UIHint("_RequiredDate")]
        public DateTime? FromDate { get; set; }
        [Display(Name = "Effective To")]
        [UIHint("_RequiredDate")]
        public DateTime? ToDate { get; set; }
        [Display(Name = "Total Addition")]
        public decimal TotalAddition { get; set; }
        [Display(Name = "Total Deduction")]
        public decimal TotalDeduction { get; set; }
        [Display(Name = "Net Pay")]
        public decimal NetPay { get; set; }
        public string Status { get; set; }
        [Display(Name = "Salary Adjust on Month")]
        public string SalaryAdjustOnMonth { get; set; }
        public string Year { get; set; }
        public int ZoneInfoId { get; set; }
        #endregion

        #region Other

        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        [Display(Name = "Salary Scale Name")]
        public string SalaryScaleName { get; set; }
        [Display(Name = "Grade Name")]
        public string GradeName{get;set;}
        [Display(Name = "Step Name")]
        public int StepName { get; set; }
        [Display(Name = "Is Adjust With Salary")]
        public bool IsAdjustWithSalary { get; set; }
        [Display(Name = "Payment Date")]
        [UIHint("_Date")]
        public DateTime? AdjustOnDate { get; set; }

        public IList<SelectListItem> StatusList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> YearList { get; set; }

        public List<SuspensionOfEmployeeViewModel> SuspensionOfEmployeeDetailList { get; set; }
        public ICollection<SuspensionOfEmployeeDetailViewModel> SuspensionOfEmployeeDeductionList { get; set; }

        #endregion

        #region Salary Payments
        public int HeadId { get; set; }
        public string HeadType { get; set; }
        public string SalaryHead { get; set; }
        //[Range(0.00, 9999999999999999)]
        [RegularExpression(@"^\d+(.\d{1,2})?$")]
        public decimal Amount { get; set; }
        //[Range(0.00, 9999999999999999)]
        [RegularExpression(@"^\d+(.\d{1,2})?$")]
        public decimal ActualAmount { get; set; }
        public string AmountType { get; set; }
        public bool IsTaxable { get; set; }

        public String NotifyTo { get; set; }

        #endregion
    }
}