using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BOM_MPA.Web.Areas.PRM.ViewModel
{
    public class TimeScaleInfoViewModel
    {

        #region Ctor
        public TimeScaleInfoViewModel()
        {
            //this.FromDate = DateTime.UtcNow;
            //this.ToDate = DateTime.UtcNow;

            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        [Required]
        [Display(Name = "I.C No.")]
        public int EmployeeId { get; set; }

        [Required]
        public string ScaleTime { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        [UIHint("_Date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [UIHint("_Date")]
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }


        public string RefNo { get; set; }

        [Required]
        [Display(Name = "Pay Scale Name")]
        public int CurrentJobGradeId { get; set; }
        public IList<SelectListItem> CurrentJobGradeList { set; get; }

        [Required]
        [Display(Name = "Increment No")]
        public decimal CurrentIncrementNo { get; set; }

        [Required]
        [Display(Name = "Pay Scale Name")]
        public int NewJobGradeId { get; set; }
        public IList<SelectListItem> NewJobGradeList { set; get; }

        [Required]
        [Display(Name = "Increment No")]
        public decimal NewIncrementNo { get; set; }


        public string Remarks { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        #endregion


        #region Other

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        [UIHint("_Date")]

        [Display(Name = "Joining Date")]
        public DateTime DateofJoining { get; set; }
        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        #endregion

    }
}