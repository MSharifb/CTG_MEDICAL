using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class AutoReminderViewModel : BaseViewModel
    {
        public AutoReminderViewModel()
        {
            ReminderTypeList = new List<SelectListItem>();

            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        [Required]
        public int Id { get; set; }

        [DisplayName("Reminder Type")]
        [Required]
        public int ReminderTypeId { set; get; }

        [DisplayName("Reminder After")]
        [Required]
        [Range(1, 999)]
        public decimal? ReminderAfter { get; set; }

        [DisplayName("Remarks")]
        [MaxLength(200)]
        public string Remarks { get; set; }

        // Others

        public IList<SelectListItem> ReminderTypeList { set; get; }

        //
    }
}