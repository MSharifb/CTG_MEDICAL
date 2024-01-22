using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PracticeViewModel : BaseViewModel
    {
        
        #region Standard Property
        [Required]
        [Display(Name = "Practice no")]
        public int PracticeId { get; set; }
        [Display(Name = "Person Name")]
        public string PracticeName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        #endregion

          }
}