using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_BEPZA.Web.Areas.PMI.ViewModel.Budget
{
    public class BugdetZoneListViewModel : BaseViewModel
    {
        public BugdetZoneListViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now.Date;
        }

        [Required]
        public int ZoneId { get; set; }

        public string ZoneName { get; set; }

        public bool IsSelected { get; set; }
    }
}