using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ACRAttributesInformationDetailViewModel : BaseViewModel
    {
        #region Ctor
        public ACRAttributesInformationDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
        }
        #endregion

        #region Standard Property

        [Required]
        [Display(Name = "Attribute Name")]
        public string AttributeName { get; set; }
        [Display(Name = "Full Mark")]
        public decimal? FullMark { get; set; }
        [Display(Name = "Serial Number")]
        public decimal? SerialNumber { get; set; }

        #endregion

    }
}