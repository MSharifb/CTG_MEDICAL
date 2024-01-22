using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class BankInfoBranchDetailViewModel
    {
        #region Ctor
        public BankInfoBranchDetailViewModel()
        {

        }

        #endregion

        #region Standard Properties
        [Required]
        public int Id { get; set; }

        [Required]
        public int BankInfoId { get; set; }

        //  [Required]
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

        // [Required]
        [DisplayName("Country")]
        public int CountryId { get; set; }

        #endregion

        #region Others
        public string CountryName { get; set; }
        public string IUser { get; set; }
        public Nullable<System.DateTime> IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        #endregion
    }
}