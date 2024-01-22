using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BEPZA_MEDICAL.DAL.FAM.CustomEntities;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.VoucherInfo
{
    public class VoucherInfoModel
    {
        #region Fields
        #endregion

        #region Ctor
        public VoucherInfoModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;

            this.Mode = "Create";            
            this.VoucherStatus = "Draft";
            this.ApproveDate = null;
            this.VoucherSearch = new List<VoucherSearch>();
            this.VoucherInfoDetailsClient = new List<VoucherInfoDetails>();
            this.VoucherTypeList = new List<SelectListItem>();
            this.PayeeList = new List<SelectListItem>();
            this.ReceiveList = new List<SelectListItem>();
            this.PaymentTypeList = new List<SelectListItem>();
            this.BankList = new List<SelectListItem>();
            this.BankAccountList = new List<SelectListItem>();
            this.NextApprovalList = new List<SelectListItem>();
            this.ApprovalPathList = new List<SelectListItem>();
            this.ReferenceTypeList = new List<SelectListItem>();

            this.VoucherDate = DateTime.UtcNow;
            this.ChequeDate = DateTime.UtcNow;

        }
        #endregion
        
        #region Standard Property

        public int Id { get; set; }

        public int FinancialYearId { get; set; } //readonly

        [DisplayName("Voucher Number")]
        [Required]
        public string VoucherNumber { get; set; }

        [DisplayName("Voucher Type")]
        [Required]
        public int VoucherTypeId { get; set; }

        public IList<SelectListItem> VoucherTypeList { get; set; }

        [DisplayName("Voucher Date")]
        [UIHint("_Date")]
        public DateTime VoucherDate { get; set; }

        public string BankOrCash { get; set; } //1= cash 0 = bank
        public bool IsCashOrBank { get; set; }

        public string ApplicableForClient { get; set; } //Staff 1, Client 2, Vendor 3
        public int? ApplicableFor { get; set; } //Staff 1, Client 2, Vendor 3

        [DisplayName("Payee")]
        public int? PayeeStaffId { get; set; }

        [DisplayName("Payee")]
        public int? PayeeClientId { get; set; }

        [DisplayName("Payee")]
        public int? PayeeVendorId { get; set; }

        [DisplayName("Received By")]
        public string ReceivedBy { get; set; }

        [DisplayName("Received From")]
        public int? ReceiveFromStaffId { get; set; }

        [DisplayName("Received From")]
        public int? ReceiveFromClientId { get; set; }

        [DisplayName("Received From")]
        public int? ReceiveFromVendorId { get; set; }

        [DisplayName("Reference Type")]
        public string ReferenceType { get; set; }

        public IList<SelectListItem> ReferenceTypeList { get; set; }

        [DisplayName("Reference Number")]
        public string ReferenceNumber { get; set; }

        [DisplayName("Payment Mode")]
        public int? PaymentTypeId { get; set; }

        public IList<SelectListItem> PaymentTypeList { get; set; }

        [DisplayName("Reference Number")]
        public string PaymentTypeReferenceNumber { get; set; }

        [DisplayName("Bank Name")]
        public int? BankId { get; set; }
        public string BankAccountNoReceive { get; set; }

        public IList<SelectListItem> BankList { get; set; }

        [DisplayName("Account Number")]
        public int? BankAccountId { get; set; }

        public IList<SelectListItem> BankAccountList { get; set; }

        [DisplayName("Cheque Date")]
        [UIHint("_Date")]
        public System.DateTime? ChequeDate { get; set; }

        [DisplayName("Narration")]
        public string Narration { get; set; }

        [DisplayName("Next Approval Path")]
        public int? CurrentApprovalNodeId { get; set; }

        public IList<SelectListItem> NextApprovalList { get; set; }

        [DisplayName("Approval Path")]
        [Required]
        public int? ApprovalPathId { get; set; }

        public IList<SelectListItem> ApprovalPathList { get; set; }

        public string ApprovalComment { get; set; }

        public string VoucherStatus { get; set; } //draft, recommandation, approved
        public int? FinalApproverId { get; set; }
        public System.DateTime? ApproveDate { get; set; } //when final approve done 

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        #endregion

        #region Others
        [DisplayName("Payee")]
        public int? PayeeId { get; set; }
        public IList<SelectListItem> PayeeList { get; set; }

        [DisplayName("Received From")]
        public int? ReceiveId { get; set; }
        public IList<SelectListItem> ReceiveList { get; set; }

        public string Mode { get; set; }
        public virtual ICollection<VoucherSearch> VoucherSearch { get; set; }
        public virtual ICollection<VoucherInfoDetails> VoucherInfoDetailsClient { get; set; }

        [DisplayName("Account Head Name")]
        public string SelectedAccountHeadName { get; set; }

        [DisplayName("Selected Head Balance")]
        public decimal SelectedHeadCurrentBalance { get; set; }

        [DisplayName("Balance will be")]
        public decimal SelectedHeadBalanceWillBe { get; set; }

        public string recommenderOrApprover { get; set; }

        public int InvoiceVoucherReferenceId { get; set; }

        [DisplayName("Invoice")]
        public decimal InvoiceVoucherAmount { get; set; }

        [DisplayName("Realized")]
        public decimal RealizationVoucherTotal { get; set; }

        [DisplayName("Is Realized")]
        public bool IsRealized { get; set; }

        #endregion
    }
    public class VoucherInfoDetails
    {
        #region Fields

        #endregion

        #region Ctor
        public VoucherInfoDetails()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;

            this.ChequeDate = DateTime.UtcNow;
            this.IsReconciled = null;
            this.ReconcileDate = null;

            this.AccountHeadList = new List<SelectListItem>();
            this.ProjectList = new List<SelectListItem>();
            this.ChequeNumberList = new List<SelectListItem>();

        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        public int VoucherId { get; set; }

        [DisplayName("Account Head Code")]
        [Required]
        public int AccountHeadId { get; set; }

        public IList<SelectListItem> AccountHeadList { get; set; }

        public IList<SelectListItem> AccountHeadNameList { get; set; }

        [DisplayName("Debit")]
        [Required]
        public decimal Debit { get; set; }

        [DisplayName("Credit")]
        [Required]
        public decimal Credit { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        public IList<SelectListItem> ProjectList { get; set; }

        [DisplayName("Cheque No")]
        public string ChequeNumber { get; set; }

        public string ChequeNumberRV { get; set; }

        public IList<SelectListItem> ChequeNumberList { get; set; }

        public System.DateTime ChequeDate { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        public bool? IsReconciled { get; set; }
        public DateTime? ReconcileDate { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        #endregion

        #region Others

        #endregion
    }    
}