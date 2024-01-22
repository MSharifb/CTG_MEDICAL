using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class OfficeEquipmentFurnitureInfoViewModel:BaseViewModel
    {
        #region Ctor
        public OfficeEquipmentFurnitureInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
        }
        #endregion

        #region Standard
        [Required]
        public string Name { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        [AllowHtml]
        [UIHint("_CKEditor")]
        public string Description { get; set; }
        public string Remarks { get; set; }
        #endregion
    }
}