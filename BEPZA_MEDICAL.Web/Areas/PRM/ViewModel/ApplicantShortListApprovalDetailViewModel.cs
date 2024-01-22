using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ApplicantShortListApprovalDetailViewModel : BaseViewModel
    {
        #region Standard Property

        [Required]
        public int ApplicantShortListApprovalId { get; set; }

        [Required]
        public int ApplicantInfoId { get; set; }

        #endregion

        #region Other

        public bool IsChecked { get; set; }

        public int? DesignationId { get; set; }

        [Display(Name = "Short Listed By")]
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        public string DateOfBirth { get; set; }

        public string NID { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? NoOfPost { get; set; }

        #endregion
    }
}