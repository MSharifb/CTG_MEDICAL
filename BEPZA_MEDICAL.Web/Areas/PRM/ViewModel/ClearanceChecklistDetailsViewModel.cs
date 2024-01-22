using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ClearanceChecklistDetailsViewModel : BaseViewModel
    {

        #region Standard Property

        [Required]
        [Display(Name = "Checklist Name")]
        public string Name { get; set; }
        #endregion

        #region Other
        public int ClearanceChecklistId { get; set; }

        #endregion


    }
}