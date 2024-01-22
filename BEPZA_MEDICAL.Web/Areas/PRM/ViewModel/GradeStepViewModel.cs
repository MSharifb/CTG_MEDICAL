using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class GradeStepViewModel : BaseViewModel
    {
        public int JobGradeId { get; set; }

        [Required]
       // [Range(-5, 30, ErrorMessage = "Please enter valid integer Number (Range -5 to 30)")]
        public int StepName { get; set; }

        [Required]
        //[Range(0, 80000, ErrorMessage = "Please enter valid integer Number (Range 0 to 80000)")]
       // [UIHint("_OnlyCurrency")]
        public decimal StepAmount { get; set; }
       
    }
}