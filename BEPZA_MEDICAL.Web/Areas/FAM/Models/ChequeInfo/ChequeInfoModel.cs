using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.ChequeInfo
{
    public class ChequeInfoModel
    {
        #region Ctor
        public ChequeInfoModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = "Create";

            this.ChequeInfoDetails = new List<ChequeInfoDetails>();
            this.BankList = new List<SelectListItem>();
            this.AccHeadList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        [DisplayName("Account Number")]
        [Required]
        public int BankAccountId { get; set; }

        public IList<SelectListItem> AccHeadList { get; set; }

        [DisplayName("Cheque Book Number")]
        [Required]
        public string ChequeBookNumber { get; set; }

        [DisplayName("Cheque Initial Part")]
        [Required]
        public string ChequeInitialPart { get; set; }

        [DisplayName("Cheque Starting Number")]
        [Required]
        public int ChequeStartingNumber { get; set; }

        [DisplayName("Number of Leaf")]
        [Required]
        public int NumberOfLeaf { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }
        public ICollection<ChequeInfoDetails> ChequeInfoDetails { get; set; }

        [DisplayName("Bank Name")]
        [Required]
        public int BankId { get; set; }
        public IList<SelectListItem> BankList { get; set; }

        public string BankName { get; set; }
        #endregion
    }

    public class ChequeInfoDetails
    {
        #region Ctor
        public ChequeInfoDetails()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;            
        }
        #endregion

        #region Standard Propery

        public int Id { get; set; }
        public int ChequeBookId { get; set; }
        public string ChequeNumber { get; set; }
        public string ChequeStatus { get; set; }
        public decimal ChequeAmount { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        #endregion
    }

    public class ChequeInfoSearch
    {

    }
}