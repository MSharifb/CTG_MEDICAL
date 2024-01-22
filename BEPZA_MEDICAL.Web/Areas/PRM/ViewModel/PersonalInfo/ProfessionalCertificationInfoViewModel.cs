using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Objects.DataClasses;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ProfessionalCertificationInfoViewModel
    {
        public ProfessionalCertificationInfoViewModel()
        {
            this.InstituteList = new List<SelectListItem>();
            this.CertificationCategoryList = new List<SelectListItem>();
            this.CountryList = new List<SelectListItem>();
            this.CertificationYearList = new List<SelectListItem>();
            this.EmpTop = new EmpTop();
        }

        #region Standard Property

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [Required]
        [DisplayName("Certification Catagory")]
        public int CertificationCatagoryId { get; set; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Certification Title")]
        public string CertificationTitle { get; set; }
        [Required]
        [DisplayName("Institute")]
        public int CertificationInstituteId { get; set; }
        [Required]
        [DisplayName("Country")]
        public int CountryId { get; set; }

        [DisplayName("Year")]
        public string CertificationYear { get; set; }

        [MaxLength(100)]
        public string Result { get; set; }

        [MaxLength(500)]
        [DisplayName("Certification Detail")]
        //[UIHint("_MultiLineBig")]
        public string CertificationDetail { get; set; }

        #endregion

        public IList<SelectListItem> CertificationCategoryList { get; set; }
        public IList<SelectListItem> InstituteList { get; set; }
        public IList<SelectListItem> CountryList { get; set; }

        public IList<SelectListItem> CertificationYearList { get; set; }
        public EmpTop EmpTop { get; set; }        

        public string strMode { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public string SideBarClassName { get; set; }
    }
}