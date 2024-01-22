using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class BankBranchViewModel : BaseViewModel
    {
        public BankBranchViewModel()
        {
            this.BankId = 0;
            this.Name =
            this.Address =
            this.Remarks ="";
        }

        //public int Id { get; set; }
        public int BankId { get; set; }

        [UIHint("_ReadOnly")]
        [DisplayName("Branch Name")]
        public string Name { get; set; }
        [DisplayName("Branch Name (In Bengali)")]
        public string NameInBengali { get; set; }

        [UIHint("_ReadOnly")]
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Address (In Bengali)")]
        public string AddressInBengali { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }
    }
}
