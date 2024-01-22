using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ResourceCategoryViewModel
    {
        public ResourceCategoryViewModel()
        {
            this.ResourceTypeList = new List<SelectListItem>();
            this.UOMTypeList = new List<SelectListItem>();
        }

        [Required]
        public int Id { get; set; }

        [DisplayName("Resoruce Type")]
        [Required]
        public int ResourceTypeId { set; get; }
        public IList<SelectListItem> ResourceTypeList { set; get; }

        [DisplayName("Resource Category")]
        [Required(AllowEmptyStrings = false)]
        public string ResourceCategory { set; get; }

        [DisplayName("Unit of Measurement")]
        [Required]
        public int UOMId { set; get; }
        public IList<SelectListItem> UOMTypeList { set; get; }

        [DisplayName("Actual Rate")]
        [UIHint("_OnlyCurrency")]
        [Required]     
        public decimal ActualRate { set; get; }

        [DisplayName("Budget Rate")]
        [UIHint("_OnlyCurrency")]
        [Required]        
        public decimal BudgetRate { set; get; }

        [DisplayName("Effective Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime EffectiveDate { set; get; }

        [UIHint("_MultiLine")]
        public string Remarks { set; get; }

        public int IsError { set; get; }
        public string ErrMsg { set; get; }

    }

}