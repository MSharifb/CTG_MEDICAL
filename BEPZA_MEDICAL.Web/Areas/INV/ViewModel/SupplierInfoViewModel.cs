using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class SupplierInfoViewModel : BaseViewModel
    {
        [DisplayName("Supplier")][Required]
        public string SupplierName { get; set; }
        public string Address { get; set; }
        [DisplayName("Phone")]
        public string PhoneNo { get; set; }
        [DisplayName("Fax")]
        public string FaxNo { get; set; }
        public string Email { get; set; }
        [DisplayName("Trade License Number")]
        public string TradeLicenseNo { get; set; }
        public string TIN { get; set; }
        [DisplayName("VAT Registration Number")]
        public string VATRegNo { get; set; }
        [DisplayName("Contact Person")]
        public string ContactPersonName { get; set; }
        [DisplayName("Contact Person(Phone)")]
        public string ContactPersonNo { get; set; }
        public string Remarks { get; set; }
    }
}