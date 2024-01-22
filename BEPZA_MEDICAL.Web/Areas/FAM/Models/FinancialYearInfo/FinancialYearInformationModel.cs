using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.FinancialYearInfo
{
    public class FinancialYearInformationModel
    {
        #region Ctor
        public FinancialYearInformationModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.FinancialYearStartDate = DateTime.Now;
            this.FinancialYearEndDate = DateTime.Now;

            this.Mode = "Create";
            this.IsActive = true;
        }
        #endregion


        #region Standard Property
        
        public int Id { get; set; }
        
        [DisplayName("Financial Year Name")]
        [Required]
        [StringLength(100)]
        public string FinancialYearName { get; set; }

        [DisplayName("Financial Year Start Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime FinancialYearStartDate { get; set; }

        [DisplayName("Financial Year End Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime FinancialYearEndDate { get; set; }

        [DisplayName("Financial Year Voucher Format")]
        [Required]
        [StringLength(7)]
        public string FinancialYearVoucherFormat { get; set; }

        [DisplayName("Is Close")]
        public bool IsClose { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public int CompanyId { get; set; }
        #endregion


        #region Other
        public String Mode { get; set; }
        #endregion
    }
}