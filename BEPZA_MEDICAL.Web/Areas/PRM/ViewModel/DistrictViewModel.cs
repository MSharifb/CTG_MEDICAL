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
    public class DistrictViewModel
    {
        public DistrictViewModel()
        {
            this.CountryList = new List<SelectListItem>();
            this.DivisionList = new List<SelectListItem>();           
        }

       
        public int Id { get; set; }

        [Required]
        [DisplayName("District")]
        public string DistrictName { set; get; } 
     
        [Required]
        [DisplayName("Division")]
        public int DivisionId { set; get; }
        public IList<SelectListItem> DivisionList { set; get; }

         [Required]
        [DisplayName("Country")]
        public int CountryId { set; get; }
        public IList<SelectListItem> CountryList { set; get; }

    }
}