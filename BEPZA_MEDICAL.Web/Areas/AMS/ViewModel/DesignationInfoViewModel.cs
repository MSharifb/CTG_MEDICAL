using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class DesignationInfoViewModel : BaseViewModel
    {
        public DesignationInfoViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        [Required]
        public int Id { get; set; }

        [DisplayName("Sort Order")]
        [Required]
        public int? SortOrder { set; get; }

        [DisplayName("Designation")]
        [MaxLength(50)]
        [Required]
        public string DesignationName { get; set; }
    }
}