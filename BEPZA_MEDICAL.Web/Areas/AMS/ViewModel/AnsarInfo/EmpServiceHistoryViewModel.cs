using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class EmpServiceHistoryViewModel : BaseViewModel
    {
        #region Ctor
        public EmpServiceHistoryViewModel()
        {
            this.ZoneList = new List<SelectListItem>();
            this.PeriodFrom = DateTime.Now;
            this.PeriodTo = DateTime.Now;
            this.EmpTop = new EmpTop();

        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        public int EmployeeId { set; get; }

        [DisplayName("Zone")]
        [Required]
        public int? ZoneId { get; set; }
        public IList<SelectListItem> ZoneList { set; get; }

        [DisplayName("Period From")]
        [UIHint("_Date")]
        [Required]
        public virtual System.DateTime PeriodFrom { get; set; }

        [DisplayName("Period To")]
        [UIHint("_Date")]
        //[Required]
        public virtual System.DateTime? PeriodTo { get; set; }
       
        [DisplayName("Disciplinary Record (Award)")]
        [UIHint("_MultiLine")]
        public string RemarkableWork { set; get; }

        [DisplayName("Disciplinary Record (Punishment)")]
        [UIHint("_MultiLine")]
        public string DisciplinaryRecord { set; get; }

        [DisplayName("Remarks")]
        [UIHint("_MultiLine")]
        public string Comment { set; get; }

        [DisplayName("Duration(Year)")]
        [Range(0, 100)]
        [UIHint("_readonly")]
        public decimal? Duration { get; set; }

        #endregion

        #region Others      
        public EmpTop EmpTop { get; set; }
        public virtual Nullable<System.DateTime> InactiveDate { get; set; }

        public string StrDuration { get; set; }
  
        #endregion

    }
}