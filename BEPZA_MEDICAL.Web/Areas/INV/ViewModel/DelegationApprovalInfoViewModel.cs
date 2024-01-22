using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class DelegationApprovalInfoViewModel : BaseViewModel
    {
        public DelegationApprovalInfoViewModel()
        {
            ItemTypeList = new List<SelectListItem>();
            DesignationList = new List<SelectListItem>();
            DepartmentList = new List<SelectListItem>();
            
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        [Required]
        public int Id { get; set; }

        [Display(Name = "Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime? ApprovalDate { get; set; }

        [DisplayName("Approval Limit")]
        [Required]
        [Range(0, 9999999999999999)]
        public decimal? ApprovalLimit { get; set; }

        [DisplayName("Item Type")]
        [Required]
        public int? ItemTypeId { set; get; }

        [DisplayName("Department")]
        [Required]
        public int? DepartmentId { set; get; }

        [DisplayName("Designation")]
        [Required]
        public int? DesignationId { set; get; }

        [DisplayName("Remarks")]
        [MaxLength(200)]
        public string Remarks { get; set; }

        public int ZoneInfoId { get; set; }


        // Others

        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> DesignationList { set; get; }
        public IList<SelectListItem> DepartmentList { set; get; }

        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }

        public DateTime? ApprovalDateFrom { get; set; }
        public DateTime? ApprovalDateTo { get; set; }


        //
    }
}