using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation
{
    public class EstimationZoneListViewModel : BaseViewModel
    {
        public EstimationZoneListViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now.Date;
        }

        [Required]
        public int ZoneId { get; set; }

        public string ZoneName { get; set; }

        public bool IsSelected { get; set; }

        public int EstimationMasterId { get; set; }
    }
}