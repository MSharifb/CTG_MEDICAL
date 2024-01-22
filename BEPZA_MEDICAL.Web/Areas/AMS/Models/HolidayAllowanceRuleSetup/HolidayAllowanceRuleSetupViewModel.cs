using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_BEPZA.Web.Areas.AMS.Models.HolidayAllowanceRuleSetup
{
    public class HolidayAllowanceRuleSetupViewModel : BaseViewModel
    {
        #region Ctor
        public HolidayAllowanceRuleSetupViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

        }
        #endregion

        #region Standard Property
        [Required]
        [Display(Name="Weekend/Holiday Allowance For")]
        public string AllowanceRuleName { get; set; }
        [Required]
        [Display(Name="Effective Date From")]
        [UIHint("_Date")]
        public DateTime? EffFromDate { get; set; }
        [Required]
        [Display(Name="Effective Date To")]
        [UIHint("_Date")]
        public DateTime? EffToDate { get; set; }

        public string AllowanceType { get; set; }
        [Display(Name="Max. Weekend Per Year")]
        public int? MaxWeekendPerYear { get; set; }
        [Display(Name="Weekend Allowance Rate")]
        public decimal? Rate { get; set; } 
        #endregion

        #region Other

        #endregion
    }
}