using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ZoneInfoViewModel : BaseViewModel
    {
        #region Ctor
        public ZoneInfoViewModel()
        {
            this.OrganogramCategoryTypeList = new List<SelectListItem>();
        }
        #endregion

        #region Standard

        [Required]
        [DisplayName("Zone Name")]
        public string ZoneName { get; set; }

        [DisplayName("Zone Name (In Bengali)")]
        public string ZoneNameInBengali { get; set; }

        [Required]
        [DisplayName("Zone Code")]
        public string ZoneCode { get; set; }

        [Required]
        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        [Required]
        [DisplayName("Category Type")]
        public int OrganogramCategoryTypeId { get; set; }

        [Required]
        [DisplayName("Zone Address")]
        public string ZoneAddress { get; set; }

        [DisplayName("Zone Address (In Bengali)")]
        public string ZoneAddressInBengali { get; set; }

        [DisplayName("Short Code for Voucher No.")]
        public string Prefix { get; set; }

        [DisplayName("Is Head Office")]
        public bool IsHeadOffice { get; set; }

        [DisplayName("Zone Code for Billing System"), Required]
        public string ZoneCodeForBillingSystem { get; set; }
        #endregion

        #region Others

        public IList<SelectListItem> OrganogramCategoryTypeList { set; get; }

        #endregion
    }
}