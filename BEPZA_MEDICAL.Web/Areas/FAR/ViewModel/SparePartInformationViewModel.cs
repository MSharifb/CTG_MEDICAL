using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class SparePartInformationViewModel:BaseViewModel
    {
       #region Ctor
        public SparePartInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
        }
        #endregion

       #region Standard Properties
        [Required]
        [DisplayName("Spare Part Name")]
        public string SparePart { get; set; }

        public string Code { get; set; }

        public string Remarks { get; set; }
       #endregion

       #region Other
        #endregion
    }
}