using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PunishmentTypeInfoViewModel : BaseViewModel
    {
        #region Ctor
        public PunishmentTypeInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.DisciplinaryActionTypeList = new List<SelectListItem>();
            this.PunishmentTypeInfoDetail = new List<PunishmentTypeInfoDetailViewModel>();
            this.PunishmentRestrictionList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Disciplinary Action Type")]
        public int DisciplinaryActionTypeId { get; set; }

        [Required]
        [DisplayName("Punishment Name")]
        public string PunishmentName { get; set; }

        [UIHint("_MultiLine")]
        public string Remarks { get; set; }

        #endregion

        #region Others
        public string DisciplinaryActionTypeName { get; set; }
        public IList<SelectListItem> DisciplinaryActionTypeList { set; get; }

        public IList<SelectListItem> PunishmentRestrictionList { get; set; }

        public IList<PunishmentTypeInfoDetailViewModel> PunishmentTypeInfoDetail { get; set; }

        [DisplayName("Punishment Type")]
        public string RestrictionName { get; set; }
        public int? Days { get; set; }

        #endregion
    }
}