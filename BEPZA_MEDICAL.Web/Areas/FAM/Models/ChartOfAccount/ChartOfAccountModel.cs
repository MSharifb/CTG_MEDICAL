using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.ChartOfAccount
{
    public class ChartOfAccountModel
    {
        #region Ctor
        public ChartOfAccountModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = "Create";
            this.IsActive = true;
            this.CompanyId = 1;

            this.AccountHeadTypeList = new List<SelectListItem>();
            this.CashBankTypeList = new List<SelectListItem>();
            this.ParentHeadCodeList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        [DisplayName("Account Head Code")]
        [Required]
        //[UIHint("_OnlyCurrency")]
        public string AccountHeadCode { get; set; }

        [DisplayName("Account Head Name")]
        [Required]
        public string AccountHeadName { get; set; }

        [DisplayName("Account Type")]
        [Required]
        public string AccountHeadType { get; set; }

        public List<SelectListItem> AccountHeadTypeList { get; set; }

        [DisplayName("Is Posting Account")]        
        public bool IsPostingAccount { get; set; }

        [DisplayName("Parent Head Name")]
        [Required]
        public string ParentHeadCode { get; set; }

        public List<SelectListItem> ParentHeadCodeList { get; set; }

        [DisplayName("Opening Balance")]
        [Required]
        public decimal OpeningBalance { get; set; }

        [DisplayName("BSPL Name")]
        public string BSPLName { get; set; }

        [DisplayName("BSPL Serial")]
        public int BSPLSerial { get; set; }

        [DisplayName("Is Budget Head")]        
        public bool IsBudgetHead { get; set; }

        [DisplayName("Cash/Bank Type")]
        [Required]
        public string CashBankType { get; set; }

        public List<SelectListItem> CashBankTypeList { get; set; }

        [DisplayName("Is Bank")]        
        public bool IsBank { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [DisplayName("Is Active")]        
        public bool IsActive { get; set; }

        public int CompanyId { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }
        #endregion

        #region Others
        public string Mode { get; set; }
        #endregion
    }
}