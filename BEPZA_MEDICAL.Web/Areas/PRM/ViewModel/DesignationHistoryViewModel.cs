using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class DesignationHistoryViewModel : BaseViewModel
    {

        #region Ctor
        public DesignationHistoryViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.OldSalaryScaleList = new List<SelectListItem>();
            this.NewSalaryScaleList = new List<SelectListItem>();
            this.OldJobGradeList = new List<SelectListItem>();
            this.NewJobGradeList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.DesignationHistoryList = new List<DesignationHistoryViewModel>();
        }

        #endregion

        #region Standard Property     
        [UIHint("_RequiredDate")]
        [Display(Name = "Effective Date")]
        public DateTime? EffectiveDate { get; set; }

        [Required]
        [Display(Name = "Old Salary Scale")]
        public int OldSalaryScaleId { get; set; }

        public int? OldJobGradeId { get; set; }

        [Required]
        [Display(Name = "New Salary Scale")]
        public int? NewSalaryScaleId { get; set; }

        public int? NewJobGradeId { get; set; }

        public int? DesignationId { get; set; }

        #endregion

        #region Other

        public string ShowRecord { get; set; }
        public int? DesignationHistoryId { get; set; }

        public string OldSalaryScaleName { get; set; }
        public string NewSalaryScaleName { get; set; }
        public string OldJobGradeName { get; set; }
        public string NewJobGradeName { get; set; }
        public string Designation { get; set; }
        public int? DesignationSortOrder { get; set; }
        public IList<SelectListItem> OldSalaryScaleList { set; get; }
        public IList<SelectListItem> NewSalaryScaleList { set; get; }

        public IList<SelectListItem> OldJobGradeList { set; get; }
        public IList<SelectListItem> NewJobGradeList { set; get; }
        public IList<SelectListItem> DesignationList { set; get; }
        public IList<DesignationHistoryViewModel> DesignationHistoryList { get; set; }



        #endregion
    }
}