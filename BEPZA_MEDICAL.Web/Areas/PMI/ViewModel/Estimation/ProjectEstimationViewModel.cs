using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation
{
    public class ProjectEstimationViewModel : BaseViewModel
    {
        #region ctor

        public ProjectEstimationViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            ProjectList = new List<SelectListItem>();
            ZoneList = new List<EstimationZoneListViewModel>();
            ProjectEstimationDetails = new List<ProjectEstimationDetailsViewModel>();
            EstimationItemList = new List<ProjectEstimationDetailsViewModel>();
            DetailViewModel = new ProjectEstimationDetailsViewModel();
            EstimationHeadList = new List<SelectListItem>();
            EstimationStatusList = new List<SelectListItem>();

            EstimationHeadDescriptions = new List<EstimationHeadDesciptionViewModel>();
            ApproverList = new List<ApprovalFlowViewModel>();
            ApprovalStatusList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property
        [Required]
        [DisplayName("Name of Project")]
        public int ProjectId { get; set; }

        [DisplayName("Name of Project")]
        public string ProjectName { get; set; }

        [Required]
        [DisplayName("Estimation Date")]
        [UIHint("_Date")]
        public DateTime? EstimationDate { get; set; }

        [DisplayName("Status")]
        public int EstimationStatusId { get; set; }

        [DisplayName("Zone")]
        public int ZoneId { get; set; }

        [DisplayName("Zone")]
        public string ZoneName { get; set; }

        [DisplayName("Total Estimated Amount(TK)")]
        [UIHint("_readOnly")]
        public decimal? TotalAmount { get; set; }
        #endregion

        #region Others

        [DisplayName("Estimation Head")]
        public int EstimationHeadId { get; set; }

        [DisplayName("Description")]
        [UIHint("_readOnly")]
        public string Description { get; set; }
        public IList<SelectListItem> ProjectList { get; set; }
        public List<EstimationZoneListViewModel> ZoneList { get; set; }

        public IList<SelectListItem> EstimationStatusList { get; set; }
        public List<ProjectEstimationDetailsViewModel> ProjectEstimationDetails { get; set; }

        public IList<SelectListItem> EstimationHeadList { get; set; }

        public ProjectEstimationDetailsViewModel DetailViewModel { get; set; }

        public List<ProjectEstimationDetailsViewModel> EstimationItemList { get; set; }

        public List<EstimationHeadDesciptionViewModel> EstimationHeadDescriptions { get; set; }

        public string ItemName { get; set; }

        [DisplayName("Budget Amount")]
        public decimal? BudgetAmount { get; set; }

        public string StatusName { get; set; }

        public List<ApprovalFlowViewModel> ApproverList { get; set; }

        public string Remarks { get; set; }
        public string ApprovalStatus { get; set; }
        [DisplayName("submission status")]
        public int? ApprovalStatusId { get; set; }
        public int? ApprovalSelectionId { get; set; }
        public IList<SelectListItem> ApprovalStatusList { get; set; }

        #endregion
    }

    public class EstimationHeadDesciptionViewModel : BaseViewModel
    {
        public EstimationHeadDesciptionViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            HeadList = new List<SelectListItem>();
        }

        public int MasterId { get; set; }

        [DisplayName("Head")]
        public int EstimationHeadId { get; set; }

        [DisplayName("Description")]
        public string HeadDescription { get; set; }

        public IList<SelectListItem> HeadList { get; set; }

        public string HeadName { get; set; }

    }
}