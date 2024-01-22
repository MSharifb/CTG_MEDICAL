using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.ProfitDistribution
{
    public class ProfitDistributionViewModel : BaseViewModel
    {
        public ProfitDistributionViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            EUser = IUser;
            IDate = DateTime.UtcNow;
            EDate = IDate;

            PFYearList = new List<SelectListItem>();
            PFMonthList = new List<SelectListItem>();
            DivisionList = new List<SelectListItem>();
            Mode = "Create";
        }
                
        [DisplayName("Distribution Month")]
        public string PeriodMonth { get; set; }

        [DisplayName("Distribution Year")]
        public string PeriodYear { get; set; }

        [Required]
        [UIHint("_OnlyNumber")]
        [DisplayName("Interest Rate")]
        [Range(1, 999, ErrorMessage = "Interest Rate must be greater than zero.")]
        public decimal ProfitRate { get; set; }

        [UIHint("_MultiLine")]
        [DisplayName("Remarks")]
        public string Remarks { get; set; }


        public int EmployeeId { get; set; }
        [DisplayName("Employee ID")]
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public int DivisionId { get; set; }
        public int EmploymentTypeId { get; set; }

        
        #region Other properties

        public string Mode { get; set; }

        [DisplayName("Profit Rate Type")]
        [Description("Monthly / Yearly")]
        public string ProfitRateType { get; set; }
        
        public int ProfitDistributionId { get; set; }

        [DisplayName("Membership ID")]
        public int MembershipID { get; set; }

        public IList<SelectListItem> PFYearList { get; set; }
        public IList<SelectListItem> PFMonthList { get; set; }
        public IList<SelectListItem> DivisionList { get; set; }

        #endregion
    }
}