using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.OverTimeRule
{
    public class OverTimeRuleModel
    {
        #region Ctor
        public OverTimeRuleModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = "Create";
            this.SalaryHeadList = new List<SelectListItem>();

            this.FromDate = DateTime.UtcNow;
            this.ToDate = DateTime.UtcNow;
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        [Required]
        [Display(Name="Over Time Rule Name")]
        public string RuleName { get; set; }

        [Required]
        [Display(Name="From Date")]
        [UIHint("_Date")]
        public DateTime? FromDate { get; set; }

        [Required]
        [Display(Name = "To Date")]
        [UIHint("_Date")]
        public DateTime? ToDate { get; set; }

        [Required]
        public string EligibleFrom { get; set; }

        [Required]
        [Display(Name = "Overtime Start After Shift Out Time")]
        public int AfterOutTime { get; set; }

        [Required]
        [Display(Name = "Min. Unit to Calculate Overtime (Calculate overtime if duration is more than)")]
        public int MinHour { get; set; }

        [Required]
        [Display(Name = "Max. Overtime Per Day")]
        public int MaxHour { get; set; }

        [Required]
        [Display(Name="Overtime Unit")]
        //public decimal Unithour { get; set; }
        public int Unithour { get; set; }


        [Required]
        [Display(Name = "Salary Head Name")]
        public int SalaryHeadId { get; set; }
        public IList<SelectListItem> SalaryHeadList { set; get; }


        [Required]
        [Display(Name = "Overtime Rate")]
        public string Rate { get; set; }

        [Display(Name = "Night Allowance (Tk.)")]
        public Nullable<decimal> NightAllowance { get; set; }

        [Display(Name = "Max. Night Per Month")]
        public Nullable<int> MaxDays { get; set; }

        [Required]
        [Display(Name = "Eligible for Night Allowance")]
        public bool IsAllowForNight { get; set; }

        [Required]
        [Display(Name = "Revenue Stamp")]
        public Decimal RevenueStamp { get; set; }

        [Required]
        [Display(Name = "Is Impact in Pay Slip")]
        public bool IsImpactInPaySlip { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        #endregion

        #region Other
        public string BasicSalaryX { get; set; }
        public string BasicSalaryY { get; set; }
        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        #endregion
    }
}