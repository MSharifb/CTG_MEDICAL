using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmpClearanceInfoChecklistDetailsViewModel:BaseViewModel
    {
        #region Standard Property
                
        [Required]
        public int ClearanceChecklistId { get; set; }

        [Display(Name = "Clearance Status")]
        public bool Status { get; set; }

        [Display(Name = "If ‘No’, Description")]
        [UIHint("_MultiLine")]
        public string Description { get; set; }

        #endregion


        #region Other
        [Display(Name = "Name of the Checklist")]
        public string ClearanceName { get; set; }
        public string CheckStatus { get; set; }
        public int EmpClearanceFormDetailId { get; set; }     
        #endregion
    }
}