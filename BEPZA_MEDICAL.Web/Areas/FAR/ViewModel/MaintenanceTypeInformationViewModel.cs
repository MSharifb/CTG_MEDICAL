using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class MaintenanceTypeInformationViewModel:BaseViewModel
    {
        #region Ctor
        public MaintenanceTypeInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.RedoTypeList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Properties

        [Required]
        [DisplayName("Maintenance Type")]
        public string MaintenanceType { get; set; }

        [DisplayName("Periodically repeating process")]
        public bool IsPeriodicallyRepeating { get; set; }

        [DisplayName("Redo after")]
        public int RedoAfter { get; set; }

        public string RedoType { get; set; }

        public string Remarks { get; set; }
        #endregion

        #region Others
        public IList<SelectListItem> RedoTypeList { get; set; }
        #endregion
    }
}