using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class StatusDesignationInfoViewModel : BaseViewModel
    {

        #region Ctor
        public StatusDesignationInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #endregion       

        #region Standard Property

        [Required]
        public int ZoneInfoId { get; set; }

        [Required]
        [DisplayName("Designation")]
        public string StatusDesignationName { get; set; }

        [UIHint("_MultiLine")]
        public string Remarks { get; set; }

        [Required]
        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        #endregion

    }
}