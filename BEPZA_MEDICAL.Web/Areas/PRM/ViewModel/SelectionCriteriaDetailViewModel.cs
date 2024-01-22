using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SelectionCriteriaDetailViewModel : BaseViewModel
    {
        #region Ctor
        public SelectionCriteriaDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.SelectionCriteriaOrExamList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        [Required]
        public int SelectionCriteriaOrExamTypeId { get; set; }
        public IList<SelectListItem> SelectionCriteriaOrExamList { get; set; }
        public string SelectionCriteriaOrExamName { get; set; }
        [Required]
        [Display(Name = "Full Mark")]
        public decimal? FullMark { get; set; }
        [Required]
        [Display(Name = "Pass Mark")]
        public decimal? PassMark { get; set; }
        public string Remarks { get; set; }
        public bool IsLastExam { get; set; }
        #endregion
    }
}