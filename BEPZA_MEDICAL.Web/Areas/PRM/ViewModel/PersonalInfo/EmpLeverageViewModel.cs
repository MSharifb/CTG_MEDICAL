using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmpLeverageViewModel : BaseViewModel
    {
        public EmpLeverageViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            EmpTop = new EmpTop();
        }

        public int EmployeeId { get; set; }

        [Required]
        [DisplayName("Name of the Item")]
        public string ItemName { get; set; }

        [DisplayName("Item Description")]
        public string ItemDescription { get; set; }

        [DisplayName("Item Quantity")]
        public int ItemQnty { get; set; }

        [UIHint("_Date")]
        [DisplayName("Issue Date")]
        public DateTime? IssueDate { get; set; }

        [UIHint("_Date")]
        [DisplayName("Duration From Date")]
        public DateTime? DurationFromDate { get; set; }

        [UIHint("_Date")]
        [DisplayName("Duration To Date")]
        public DateTime? DurationToDate { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        #region Other

        public EmpTop EmpTop { get; set; }

        #endregion
    }
}
