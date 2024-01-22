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
    public class EmployeeMappingViewModel
    {
        public EmployeeMappingViewModel()
        {
            this.ResourceTypeList = new List<SelectListItem>();
            this.ResourceCategoryList = new List<SelectListItem>();           
        }

        [Required]
        public string Type { get; set; }        
     
        [Required]
        [DisplayName("Resource Type")]
        public int ResourceTypeId { set; get; }
        public IList<SelectListItem> ResourceTypeList { set; get; }

        [DisplayName("Resource Category")]
        public int ResourceCategoryId { set; get; }
        public IList<SelectListItem> ResourceCategoryList { set; get; }

    }
}