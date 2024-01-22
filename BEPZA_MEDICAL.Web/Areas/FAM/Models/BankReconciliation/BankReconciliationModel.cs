using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Resources;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.BankReconciliation
{
    public class BankReconciliationModel
    {
        public BankReconciliationModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.BankList = new List<SelectListItem>();
            this.AccountList = new List<SelectListItem>();

            this.VoucherDateFrom = DateTime.UtcNow;
            this.VoucherDateTo = DateTime.UtcNow;

            this.Debits = new List<DebitOrCredit>();
            this.Credits = new List<DebitOrCredit>();
            this.Mode = "Create";
        }
        [DisplayName("Bank Name")]
        public int? BankId { get; set; }
        public IList<SelectListItem> BankList { get; set; }

        [DisplayName("Account Number")]
        public int? AccountId { get; set; }
        public IList<SelectListItem> AccountList { get; set; }

        [DisplayName("To")]
        public DateTime? VoucherDateTo { get; set; }
        [DisplayName("Voucher Date From")]
        public DateTime? VoucherDateFrom { get; set; }

        [DisplayName("Show Reconciled Information")]
        public bool IsReconciled { get; set; }

        public IList<DebitOrCredit> Debits { get; set; }
        public IList<DebitOrCredit> Credits { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        public string Mode { get; set; }
        public string strMessage { get; set; }
        public string strClass { get; set; }    
    }

    public class DebitOrCredit
    {
        public int Id { get; set; }
        public bool IsReconciled { get; set; }
        public DateTime ChequeDate { get; set; }
        public string ChequeNo { get; set; }
        public decimal Amount { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public DateTime? ClearDate { get; set; }
        public string Description { get; set; }
    }
}