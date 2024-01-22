using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.SMS.ViewModel
{
    public class SecurityServiceHistoryViewModel : BaseViewModel
    {
        #region Ctor
        public SecurityServiceHistoryViewModel()
        {
            this.ZoneList = new List<SelectListItem>();
            this.PeriodFrom = DateTime.Now;
            this.PeriodTo = DateTime.Now;
            this.EmpTop = new BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.EmpTop();

        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        public int EmployeeId { set; get; }

        public int SecurityInfoId { get; set; }

        [DisplayName("Zone")]
        [Required]
        public int? WorkZoneId { get; set; }
        public IList<SelectListItem> ZoneList { set; get; }

        [DisplayName("Period From")]
        [UIHint("_Date")]
        [Required]
        public virtual System.DateTime PeriodFrom { get; set; }

        [DisplayName("Period To")]
        [UIHint("_Date")]
        public virtual System.DateTime? PeriodTo { get; set; }

        [DisplayName("Disciplinary Record (Punishment)")]
        [UIHint("_MultiLine")]
        public string DisciplineRecord { set; get; }

        [DisplayName("Disciplinary Record (Award)")]
        [UIHint("_MultiLine")]
        public string Award { set; get; }

        [DisplayName("Remarks")]
        [UIHint("_MultiLine")]
        public string Remarks { set; get; }

        [DisplayName("Duration(Year)")]
        [Range(0, 100)]
        [UIHint("_readonly")]
        public decimal? Duration { get; set; }

        [DisplayName("Is Intelligence?")]
        [Required]
        public Boolean Is_Intelligence { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        #endregion

        #region Others  
    
        public string StrDuration { get; set; }
        public BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.EmpTop EmpTop { get; set; }
        //public virtual Nullable<System.DateTime> InactiveDate { get; set; }
  
        #endregion

    }
}