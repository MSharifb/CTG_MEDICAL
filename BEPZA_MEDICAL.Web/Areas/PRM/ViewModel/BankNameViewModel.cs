using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class BankNameViewModel : BaseViewModel
    {
        #region Ctor
        public BankNameViewModel()
        {            
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            BankBranches = new List<BankBranchViewModel>();
        }
        #endregion

        #region Standard Properties

        [DisplayName("Bank Name")]
        public string Name { get; set; }
        [DisplayName("Bank Name (In Bengali)")]
        public string NameInBengali { get; set; }
        [DisplayName("Remarks")]
        [MaxLength(150, ErrorMessage = "Remarks cannot be longer than 150 characters.")]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "Please Provide at least one Branch")]
        public virtual IList<BankBranchViewModel> BankBranches { get; set; }

        // Details fields
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Brach Name (In Bengali)")]
        public string BranchNameInBengali { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Address (In Bengali)")]
        public string AddressInBengali { get; set; }
        [Required]
        public int ZoneInfoId { get; set; }
        #endregion


    }

}

