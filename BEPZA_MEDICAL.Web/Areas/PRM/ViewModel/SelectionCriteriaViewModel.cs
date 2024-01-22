using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SelectionCriteriaViewModel : BaseViewModel
    {
        #region Ctor
        public SelectionCriteriaViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.AdvetisementCodeList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.SelectionCriteriaOrExamList = new List<SelectListItem>();
            this.SelectionCriteriaDetailList= new List<SelectionCriteriaDetailViewModel>();
        }
        #endregion

        #region Standard Property
        [Display(Name="Advertisement Code")]
        [Required]
        public int JobAdvertisementInfoId { get; set; }
        public IList<SelectListItem> AdvetisementCodeList { get; set; }
        public string AdvertisementName { get; set; }
        [Display(Name="Job Post Name")]
        public int? DesignationId { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }
        public string Designation { get; set; }
        [Display(Name="Same Criteria for All Job Post")]
        public bool IsSameCriteria { get; set; }
        [Required]
        public int ZoneInfoId { get; set; }
        #endregion

        #region Other

        [Display(Name="Selection Criteria/Exam. Type")]
        public int? SelectionCriteriaOrExamTypeId { get; set; }
        public IList<SelectListItem> SelectionCriteriaOrExamList { get; set; }
        [Display(Name="Full Mark")]
        public decimal? FullMark { get; set; }
        [Display(Name="Pass Mark")]
        public decimal? PassMark { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Last Criteria/Exam")]
        public bool IsLastExam { get; set; }
        public IList<SelectionCriteriaDetailViewModel> SelectionCriteriaDetailList { get; set; }

        #endregion
    }
}