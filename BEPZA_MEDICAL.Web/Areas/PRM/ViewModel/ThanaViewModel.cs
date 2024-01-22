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
    public class ThanaViewModel
    {
        public ThanaViewModel()
        {
            this.CountryList = new List<SelectListItem>();
            this.DivisionList = new List<SelectListItem>();
            this.DistrictList = new List<SelectListItem>();            
        }
        [Required]
        public int Id { get; set; }
        [Required]
        [DisplayName("Upazila")]
        public string ThanaName { set; get; }      
        
        [Required]
        [DisplayName("District")]
        public int DistrictId { set; get; }
        public IList<SelectListItem> DistrictList { set; get; }

        [DisplayName("Division")]
        public int DivisionId { set; get; }
        public IList<SelectListItem> DivisionList { set; get; }

        [DisplayName("Country")]
        public int CountryId { set; get; }
        public IList<SelectListItem> CountryList { set; get; }

        public int IsError { get; set; }
        public string ErrMsg { get; set; }
    }
}