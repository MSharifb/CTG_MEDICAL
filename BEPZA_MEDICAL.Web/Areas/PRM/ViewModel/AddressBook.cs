using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.PRM.ViewModel
{
    public class AddressBookViewModel : BaseViewModel
    {
        [Required]
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNo { get; set; }

        [Required]
        [DisplayName("Address")]
        public string Address { get; set; }

        [EmailAddress (ErrorMessage="Type a valid Email Address")]
        [Required]
        public string Email { get; set; }

        public int? DivisionId { get; set; }

        public IList<SelectListItem> DivisionList { get; set; }

        public AddressBookViewModel()
        {
            DivisionList = new List<SelectListItem>();
        }

    }
}