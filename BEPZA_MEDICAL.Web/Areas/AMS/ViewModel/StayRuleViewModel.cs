using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class StayRuleViewModel : BaseViewModel
    {
        public StayRuleViewModel()
        {
            CategoryList = new List<SelectListItem>();
          
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        [Required]
        public int Id { get; set; }

        [DisplayName("Ansar Category")]
        [Required]
        public int CategoryId { set; get; }

        [DisplayName("Maximum Stay")]
        [Required]
        [Range(1, 999)]
        public decimal? MaximumStay { get; set; }

        [DisplayName("Remarks")]
        [MaxLength(200)]
        public string Remarks { get; set; }

        // Others

        public IList<SelectListItem> CategoryList { set; get; }

        //
    }
}