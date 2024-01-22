using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Objects.DataClasses;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ProfessionalLicenseInfoViewModel
    {
        public ProfessionalLicenseInfoViewModel()
        {
            this.LicensTypeList = new List<SelectListItem>();
            this.CountryList = new List<SelectListItem>();
            this.LicenseCategoryList = new List<SelectListItem>();

            this.EmpTop = new EmpTop();

            //this.PermitDate = DateTime.Now;
            //this.ExpireDate = DateTime.Now;
        }

        #region Standard Property

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [Required]
        [DisplayName("License/Membership Type")]
        public int LicensTypeId { get; set; }
        [Required]
        [MaxLength(20)]
        [DisplayName("License/Membership No.")]
        public string LicenseNo { get; set; }
        [DisplayName("Permit Date")]
        [UIHint("_Date")]

        public System.DateTime? PermitDate { get; set; }
        [DisplayName("Expire Date")]
        [UIHint("_Date")]

        public System.DateTime? ExpireDate { get; set; }
        [Required]
        [MaxLength(20)]
        [DisplayName("Issuing Institute")]
        public string IssuingInstitute { get; set; }
        [Required]
        [DisplayName("Issuing Country")]
        public int IssuingCountryId { get; set; }

        [DisplayName("License Category")]
        public int? LicenseCategoryId { get; set; }

        [MaxLength(200)]
        [DisplayName("Issuing Address")]
        [UIHint("_MultiLineBig")]
        public string IssuingAddress { get; set; }

        #endregion

        public IList<SelectListItem> LicensTypeList { get; set; }
        public IList<SelectListItem> CountryList { get; set; }
        public IList<SelectListItem> LicenseCategoryList { get; set; }
        public EmpTop EmpTop { get; set; }        

        public string strMode { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public string SideBarClassName { get; set; }
    }
}