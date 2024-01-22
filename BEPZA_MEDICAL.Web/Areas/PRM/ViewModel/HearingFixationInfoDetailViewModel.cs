using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class HearingFixationInfoDetailViewModel : BaseViewModel
    {
        #region Ctor
        public HearingFixationInfoDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #endregion
        #region Standard Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public int HearingFixationInfoId { get; set; }

        public DateTime? HearingDate { get; set; }

        public TimeSpan HearingTime { get; set; }
        public string HearingLocation { get; set; }

        public string HearingComments { get; set; }
        public string HearingStatus { get; set; }

        #endregion
    }
}