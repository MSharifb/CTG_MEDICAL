using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation
{
    public class EstimationHeadViewModel : BaseViewModel
    {
        [DisplayName("Code")]
        [Required]
        public string ItemCode { get; set; }

        [Required]
        [DisplayName("Estimation Head")]
        public string HeadName { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}