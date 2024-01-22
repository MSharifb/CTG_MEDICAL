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
    public class ResourceInfoViewModel
    {
        public ResourceInfoViewModel()
        {
            this.ResourceTypeList = new List<SelectListItem>();
            this.ResourceCategoryList = new List<SelectListItem>();
        }

        [Required]        
        public int Id { get; set; }

        public int EmployeeId { set; get; }

        [DisplayName("Resource Type")]
        [Required]
        public int ResourceTypeId { set; get; }
        public IList<SelectListItem> ResourceTypeList { set; get; }

        [DisplayName("Resource Category")]
        [Required]
        public int ResourceCategoryId { set; get; }
        public IList<SelectListItem> ResourceCategoryList { set; get; }

        [DisplayName("Employee ID")]
        //[UIHint("_ShortText")]        
        public string EmpId { set; get; }        

        [DisplayName("Employee Initial")]
        [ReadOnly(true)]      
        public virtual string EmployeeInitial{ set; get; }
        
        [DisplayName("Designation")]
        [ReadOnly(true)]
        public virtual string Designation { set; get; }

        [DisplayName("Division/Unit")]
        [ReadOnly(true)]
        public virtual string Division { set; get; }        

        [DisplayName("Resource Name")]
        [Required(AllowEmptyStrings = false)]
        public string ResourceName { set; get; }

        [DisplayName("Measurement Unit")]
        [ReadOnly(true)]
        public string UOM { set; get; }

        [DisplayName("Actual Rate")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public decimal? ActualRate { set; get; }

        [DisplayName("Budget Rate")]
        [Required]
        [UIHint("_OnlyCurrency")]        
        public decimal? BudgetRate { set; get; }        

        [DisplayName("Effective Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? EffectiveDate { set; get; }

        [DisplayName("External Resource")]
        public bool IsExternal { set; get; }

        [DisplayName("Active")]
        public bool IsActive { set; get; }

        [UIHint("_MultiLine")]
        public string Remarks { set; get; }

        [DisplayName("Human Resource")]
        public bool IsHumanResource { set; get; }

        public int IsError { set; get; }
        public string ErrMsg { set; get; }
    }
  
}