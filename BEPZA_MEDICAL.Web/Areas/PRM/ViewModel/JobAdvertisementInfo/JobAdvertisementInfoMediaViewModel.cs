using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.JobAdvertisementInfo
{
    public class JobAdvertisementInfoMediaViewModel : BaseViewModel
    {
        [Required]
        [DisplayName("Advertisement Media")]
        public int AdvertisementMediaId { get; set; }
        public string AdvertisementMediaName { get; set; }
        public IList<SelectListItem> AdvertisementMediaList { get; set; }
        [Required]
        [DisplayName("Advertisement Date")]
        [UIHint("_Date")]
        public DateTime AdvertisementDate { get; set; }
        public string Notes { get; set; }
        [DisplayName("Advertisement Expire Date")]
        [UIHint("_Date")]
        public DateTime? AdvertisementExpDate { get; set; }

    }
}