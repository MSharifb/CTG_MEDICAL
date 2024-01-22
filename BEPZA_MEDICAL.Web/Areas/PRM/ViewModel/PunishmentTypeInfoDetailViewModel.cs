using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PunishmentTypeInfoDetailViewModel : BaseViewModel
    {
        #region Ctor
        public PunishmentTypeInfoDetailViewModel()
        {
            this.PunishmentRestrictionList = new List<SelectListItem>();
        }
        #endregion


        #region Standard Properties
        [Required]
        public int PunishmentTypeInfoId { get; set; }

        [Required]
        public int PunishmentRestrictionId { get; set; }
        public string RestrictionName { get; set; }
        public int? Days { get; set; }
        #endregion

        #region Other
        public IList<SelectListItem> PunishmentRestrictionList { get; set; }
        #endregion
    }
}