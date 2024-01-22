using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class JobGradeViewModel : BaseViewModel
    {
        public JobGradeViewModel()
        {
            GradeStep = new List<GradeStepViewModel>();
        }

        [DisplayName("Grade Name")]
        [Required(AllowEmptyStrings = false)]
        public string GradeName { set; get; }

        [DisplayName("Grade Code")]
        public string GradeCode { set; get; }

        [DisplayName("Number of Steps")]
        [UIHint("_OnlyInteger")]
        [Range(0, 20)]
        [Required]
        public int NumberOfSteps { set; get; }

        [DisplayName("Initial Basic")]
        [UIHint("_OnlyCurrency")]
        [Required]
        public decimal InitialBasic { set; get; }

        [DisplayName("Last Basic")]
        [UIHint("_OnlyCurrency")]
        [Required]
        public decimal LastBasic { set; get; }

        [DataType(DataType.Currency)]
        [DisplayName("Yearly Increment")]
        [UIHint("_OnlyCurrency")]
        //[Required]
        public decimal? YearlyIncrement { set; get; }

        [DisplayName("Effective Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime DateOfEffective { set; get; }

        public bool? IsConsolidated { get; set; }

        #region Others

        [Required(ErrorMessage = "Provide at least one step name.")]       
        public IList<GradeStepViewModel> GradeStep { get; set; }
        #endregion

       
        #region New

        [Required]
        public int SalaryScaleId { get; set; }

        [DisplayName("Pay Scale Name")]
        public string PayScale { get; set; }

        #endregion
    }
}