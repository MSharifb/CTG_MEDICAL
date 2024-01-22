using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class SourceofFundViewModel : BaseViewModel
    {
        #region Ctor
        public SourceofFundViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
        }
        #endregion

        #region Standard Properties
        [Required]
        [DisplayName("Source of Fund")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        [DisplayName("Remarks")]
        [MaxLength(100)]
        public string Remarks { get; set; }

        #endregion
    }
}