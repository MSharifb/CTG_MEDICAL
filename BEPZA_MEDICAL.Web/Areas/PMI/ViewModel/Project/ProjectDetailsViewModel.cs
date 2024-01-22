using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project
{
    public class ProjectDetailsViewModel : LongBaseViewModel
    {

        public ProjectDetailsViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ProjectDetailsList = new List<ProjectDetailsViewModel>();
            this.ProcurementMethodList = new List<SelectListItem>();
            this.ProcurementTypeList = new List<SelectListItem>();
            this.SourceOfFundList = new List<SelectListItem>();
            this.ApprovalAuthorityList = new List<SelectListItem>();
        }
        [DisplayName("Procurement Method")]
        [Required]
        public int ProcurementMethodId { get; set; }

        [DisplayName("Procurement Type")]
        [Required]
        public int ProcurementTypeId { get; set; }

        [DisplayName("Source of Fund")]
        [Required]
        public int SourceOfFundId { get; set; }

        [DisplayName("Approval Authority")]
        [Required]
        public int ApprovalAuthorityId { get; set; }

        [DisplayName("Package No")]
        [Required]
        public string PackageNo { get; set; }

        [DisplayName("Package Name")]
        [Required]
        public string PackageName { get; set; }

        [DisplayName("Unit")]
        public string Unit { get; set; }

        [DisplayName("Quantity")]
        [Required]
        public int Quantity { get; set; }

        [DisplayName("Estimated Cost (In Lac)")]
        public decimal EstimatedCost { get; set; }

        public List<ProjectDetailsViewModel> ProjectDetailsList { get; set; }

        public string ProcurementType { get; set; }

        public string ProcurementMethod { get; set; }

        public string SourceOfFund { get; set; }

        public string ApprovalAuthority { get; set; }

        public IList<SelectListItem> ProcurementMethodList { get; set; }

        public IList<SelectListItem> ProcurementTypeList { get; set; }

        public IList<SelectListItem> SourceOfFundList { get; set; }

        public IList<SelectListItem> ApprovalAuthorityList { get; set; }

    }
}