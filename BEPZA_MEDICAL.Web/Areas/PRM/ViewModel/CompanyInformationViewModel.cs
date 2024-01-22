using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class CompanyInformationViewModel
    {
        public int? Id { get; set; }
        [Required]
        [DisplayName("Name of the Authority")]
        public string CompanyName { get; set; }
        [DisplayName("Company Name (In Bengali)")]
        public string CompanyNameInBengali { get; set; }
        [Required]
        [UIHint("_MultiLine")]
        public string Address { get; set; }
        [DisplayName("Address (In Bengali)")]
        public string AddressInBengali { get; set; }
        [DisplayName("Web Address")]
        public string WebURL { get; set; }
        public string City { get; set; }
        [DisplayName("City (In Bengali)")]
        public string CityInBengali { get; set; }
        public string Country { get; set; }
        [DisplayName("Country (In Bengali)")]
        public string CountryInBengali { get; set; }

        [Required]
        [DisplayName("Phone No")]
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }

        [DisplayName("Fax")]
        public string FaxNo { get; set; }

        [DisplayName("E-Mail")]
        [Email]
        public string EmailAddress { get; set; }

        [Required]
        [DisplayName("Logo")]
        public byte[] CompanyLogo { get; set; }
        public bool HasPhoto { get; set; }
        //public bool HasData { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
       
        public string btnText { get; set; }
        public string strMode { get; set; }
        public string errClass { get; set; }
        public string ErrMsg { get; set; }


    }
}