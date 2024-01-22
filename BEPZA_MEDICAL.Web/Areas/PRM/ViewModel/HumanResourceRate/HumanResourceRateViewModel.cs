using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.HumanResourceRate
{
    public class HumanResourceRateViewModel
    {
        public HumanResourceRateViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.ActionType = "Create";
        }

        public int Id { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [DisplayName("Remarks")]
        public string  Remarks { get; set; }

        [Required(ErrorMessage = "Human resource rate detail must be needed.")]
        public virtual ICollection<HumanResourceRateDetailViewModel> HumanResourceRateDetailList { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime? EDate { get; set; }

        public string message { get; set; }
        public bool isSuccess { get; set; }

        public string ActionType { get; set; }
    }

    public class HumanResourceRateDetailViewModel
    {
        public HumanResourceRateDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }

        public int Id { get; set; }

        public int HRRateMasterId { get; set; }

        [Required]
        [DisplayName("Job Grade")]
        public int JobGradeId { get; set; }

        [UIHint("_ReadOnly")]
        public string JobGrade { get; set; }

        [Required]
        public int ResourceLevelId { get; set; }

        [UIHint("_ReadOnly")]
        public string ResourceLevel { get; set; }


        //[Required]
        //[UIHint("_OnlyCurrency")]
        //[DisplayName("Actual Rate")]
        //[Range(0,99999999)]
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

    public class HumanResourceRateSearchViewModel
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