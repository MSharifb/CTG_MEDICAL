using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.HumanResourceRateAssign
{
    public class HumanResourceRateAssignModel
    {
        public HumanResourceRateAssignModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.EmploymentTypeList = new List<SelectListItem>();    

        }

        public int Id { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [DisplayName("Multiply Factor")]
        [Range(0, 999.99)]
        [UIHint("_OnlySixLengthDecimalNumber")]
        public decimal MultiplicationFactor { get; set; }

        [Required]
        [DisplayName("Employment Type")]
        public int EmploymentTypeMasterId { get; set; } 

        public List<SelectListItem> EmploymentTypeList { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "Human resource rate assign detail must be needed.")]
        public virtual ICollection<HumanResourceRateAssignDetailModel> HumanResourceRateAssignDetailList { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime? EDate { get; set; }

        public string message { get; set; }
        public bool isSuccess { get; set; }

        public string ActionType { get; set; }

    }

    public class HumanResourceRateAssignDetailModel
    {
        public HumanResourceRateAssignDetailModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }

        public int Id { get; set; }

        public int HRRateAssignMasterId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [UIHint("_ReadOnly")]
        public string EmployeeInitial { get; set; }

        [Required]
        public int ResourceLevelId { get; set; }

        [UIHint("_ReadOnly")]
        public string ResourceLevel { get; set; }

        public int EmploymentTypeId { get; set; }

        [Required]
        [UIHint("_OnlyCurrency")]
        [DisplayName("Actual Rate")]
        [Range(0, 99999999)]
        public decimal ActualRate { get; set; }

        [Required]
        [UIHint("_OnlyCurrency")]
        [DisplayName("Budget Rate")]
        [Range(0, 99999999)]
        public decimal BudgetRate { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime? EDate { get; set; }

    }

    public class HumanResourceRateAssignSearchViewModel
    {

        public int? ID { get; set; }

        public DateTime EffectiveDate { get; set; }


        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("Id = {0} AND ", ID));

            //if (EffectiveDate != System.DateTime.MinValue)
            //    filterExpressionBuilder.Append(String.Format("EffectiveDate = {0} AND ", EffectiveDate));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            return filterExpressionBuilder.ToString();
        }
        #endregion
    }


}