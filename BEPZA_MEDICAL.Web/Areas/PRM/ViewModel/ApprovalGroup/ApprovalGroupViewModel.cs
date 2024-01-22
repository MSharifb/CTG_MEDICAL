using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalGroup
{
    public class ApprovalGroupViewModel : BaseViewModel
    {
        public ApprovalGroupViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }
        [DisplayName("Approver Group")]
        [Required]
        public string ApprovalGroupName { get; set; }

        [DisplayName("Sort Order")]
        [Range(1, Int32.MaxValue)]
        [Required]
        public int? SortOrder { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }
    }
}