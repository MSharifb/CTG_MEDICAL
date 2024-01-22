using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class DepreciationViewModel : BaseViewModel
    {
        public DepreciationViewModel()
        {
            //this.YearList = new List<SelectListItem>();
            //this.MonthList = new List<SelectListItem>();
            this.FinancialYearList = new List<SelectListItem>();
        }

        [Required]
        [DisplayName("Financial Year")]
        public int FinancialYearId { get; set; }

        public DateTime FinYearStartDate { get; set; }
        public DateTime FinYearEndDate { get; set; }
       
        [DisplayName("Month")]
        public string FinYearName { get; set; }

        //[DisplayName("Year")]        
        //public string strYear { get; set; }

        [DisplayName("Process Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime ProcessDate { get; set; }

        [DisplayName("Remarks")]
        [StringLength(100, ErrorMessage = "Remarks must be within 500 characters.")]
        public string Remarks { get; set; }

        #region Other properties
        //public IList<SelectListItem> YearList { get; set; }
        //public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> FinancialYearList { get; set; }

        #endregion
    }
}