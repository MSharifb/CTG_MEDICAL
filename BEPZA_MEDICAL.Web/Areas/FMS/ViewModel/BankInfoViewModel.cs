using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class BankInfoViewModel : BaseViewModel
    {
        #region Ctor
        public BankInfoViewModel()
        {
            this.BankInfoBranchList = new List<BankInfoBranchDetailViewModel>();
            this.CountryList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Properties
        //[Required]
        //public int ZoneInfoId { get; set; }

        [Required]
        [DisplayName("Bank Name")]

        public string BankName { get; set; }

        [DisplayName("Code")]
        public string BankCode { get; set; }

        [Required]
        [DisplayName("Bank Type")]
        public string BankType { get; set; }

        #endregion

        #region Others
        public string ShowRecord { get; set; }
        public IList<BankInfoBranchDetailViewModel> BankInfoBranchList { get; set; }
        #endregion

        #region Standard Properties
        public int? BankInfoId { get; set; }

        [DisplayName("Branch Name")]
        public string BranchName { get; set; }

        [DisplayName("Address")]
        public string BranchAddress { get; set; }

        [DisplayName("SWIFT Code")]
        public string SWIFTCode { get; set; }

        [DisplayName("Contact No.")]
        public string BranchContactNo { get; set; }

        [DisplayName("Email")]
        public string BranchEmail { get; set; }

        [DisplayName("Country")]
        public int? CountryId { get; set; }
        public string CountryName { get; set; }    
        public IList<SelectListItem> CountryList { get; set; }

        public int DetailId { get; set; }
        #endregion
    }
}