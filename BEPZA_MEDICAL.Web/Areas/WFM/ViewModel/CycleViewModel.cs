using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class CycleViewModel : BaseViewModel
    {
        #region Ctor
        public CycleViewModel()
        {
            this.FromMonthList = new List<SelectListItem>();
            this.ToMonthList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Properties

        [Required]
        [DisplayName("Cycle Name")]

        public string CycleName { get; set; }

        [Required]
        [DisplayName("From Month")]
        public string FromMonth { get; set; }

        [Required]
        [DisplayName("To Month")]
        public string ToMonth { get; set; }

        #endregion

        #region Others

        public IList<SelectListItem> FromMonthList { get; set; }
        public IList<SelectListItem> ToMonthList { get; set; }

        public int? StartMonthId { get; set; }

        public int? EndMonthId { get; set; }

        #endregion
    }
}