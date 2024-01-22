using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_BEPZA.Web.Areas.AMS.Models.HolidayWorkingDay
{
    public class HolidayWorkingDayViewModel
    {
        #region Ctor
        public HolidayWorkingDayViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        [Display(Name = "Declaration Date")]
        [UIHint("_Date")]
        public DateTime? DeclarationDate { get; set; }

        [Required]
        [Display(Name = "Effective Date From")]
        [UIHint("_Date")]
        public DateTime? FromDate { get; set; }

        [Required]
        [Display(Name = "Effective Date To")]
        [UIHint("_Date")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Purpose")]
        public string Purpose { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        #endregion

        #region Other

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        #endregion
    }
}