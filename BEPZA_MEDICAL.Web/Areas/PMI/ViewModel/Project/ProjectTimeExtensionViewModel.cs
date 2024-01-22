using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project
{
    [Serializable]
    public class ProjectTimeExtensionViewModel : BaseViewModel
    {
        public ProjectTimeExtensionViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        public int ProjectId { get; set; }

        [UIHint("_Date")]
        public DateTime? ExtendedDate { get; set; }

        [Required]
        public int ExtendedDays { get; set; }

        public DateTime? ExpectedCompletionDate { get; set; }


    }
}