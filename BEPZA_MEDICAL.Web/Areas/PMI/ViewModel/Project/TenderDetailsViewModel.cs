using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project
{
    public class TenderDetailsViewModel : BaseViewModel
    {
        public TenderDetailsViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }
        public int ProjectMasterId { get; set; }

        [DisplayName("Lot Number")]
        [Required]
        public string LotNumber { get; set; }

        [DisplayName("Identification")]
        [Required]
        public string Identification { get; set; }

        [DisplayName("Location")]

        public string Location { get; set; }

        [DisplayName("Security Money")]
        [Required]
        public decimal SecurityMoney { get; set; }

        [DisplayName("Tender Price")]
        [Required]
        public decimal? TenderPrice { get; set; }

        [DisplayName("Completion Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime CompletionDate { get; set; }

        [DisplayName("Completion Days")]
        [Required]
        public int CompletionDays { get; set; }

    }
}