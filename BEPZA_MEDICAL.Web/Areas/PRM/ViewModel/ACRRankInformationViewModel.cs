using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ACRRankInformationViewModel:BaseViewModel
    {
        #region Ctor
        public ACRRankInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
        }
        #endregion

        #region Standard Property

        [Required]
        [Display(Name = "Rank Name")]
        public string RankName { get; set; }
        [Display(Name = "From Mark")]
        public decimal? FromMark { get; set; }
        [Display(Name = "To Mark")]
        public decimal? ToMark { get; set; }

        #endregion

    }
}