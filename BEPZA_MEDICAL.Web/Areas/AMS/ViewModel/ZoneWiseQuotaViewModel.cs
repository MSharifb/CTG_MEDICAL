using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class ZoneWiseQuotaViewModel : BaseViewModel
    {
        public ZoneWiseQuotaViewModel()
        {
            CategoryList = new List<SelectListItem>();
            ZoneList = new List<SelectListItem>();
          
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        [Required]
        public int Id { get; set; }

        [DisplayName("Zone Name")]
        [Required]
        public int ZoneInfoId { set; get; }

        [DisplayName("Ansar Category")]
        [Required]
        public int CategoryId { set; get; }

        [DisplayName("Quota")]
        [Required]
        [Range(1, 10000)]
        public int? Quota { get; set; }

        // Others

        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ZoneList { set; get; }

        //
    }
}