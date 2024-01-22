using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class BankWiseOfferRateViewModel:BaseViewModel
    {
        public BankWiseOfferRateViewModel()
        {
            this.BankList = new List<SelectListItem>();
            this.BranchList = new List<SelectListItem>();
            this.BankWiseOfferRateList = new List<BankWiseOfferRateViewModel>();
            this.TypeList = new List<SelectListItem>();
        }
        public int BankId { get; set; }
        public int? BranchId { get; set; }
        public decimal OfferRate { get; set;}
        public decimal Duration { get; set; }
        public string Type { get; set; }
        public string Remarks { get; set; }
        public int FDRTypeId { get; set; }

        public List<BankWiseOfferRateViewModel> BankWiseOfferRateList { get; set; }

        public IList<SelectListItem> BankList { get; set; }

        public IList<SelectListItem> BranchList { get; set; }

        public IList<SelectListItem> TypeList { get; set; }

    }
}