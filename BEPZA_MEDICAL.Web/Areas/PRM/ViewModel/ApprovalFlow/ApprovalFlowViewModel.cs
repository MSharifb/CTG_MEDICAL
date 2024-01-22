using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow
{
    public class ApprovalFlowViewModel : BaseViewModel
    {
        public ApprovalFlowViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            this.ApprovalFlowDetailList = new List<ApprovalFlowDetailViewModel>();
            this.ApprovalFlowDetail = new ApprovalFlowDetailViewModel();

            this.ZoneList = new List<SelectListItem>();
            this.ApprovalGroupList = new List<SelectListItem>();
            this.ApprovalProcessList = new List<SelectListItem>();

            
        }

        #region Std Prop

        [DisplayName("Zone / Executive Office")]
        [Required]
        public int ZoneId { get; set; }

        [DisplayName("Approver Group")]
        [Required]
        public int ApprovalGroupId { get; set; }

        [DisplayName("Flow Name")]
        [Required]
        public string ApprovalFlowName { get; set; }

        [DisplayName("Approval Process")]
        [Required]
        public int ApprovalProcesssId { get; set; }

        #endregion

        #region Other Prop

        public ApprovalFlowDetailViewModel ApprovalFlowDetail { get; set; }

        public IList<ApprovalFlowDetailViewModel> ApprovalFlowDetailList { get; set; }

        public IList<SelectListItem> ZoneList { get; set; }

        public IList<SelectListItem> ApprovalGroupList { get; set; }

        public IList<SelectListItem> ApprovalProcessList { get; set; }

        public string ApprovalProcessName { get; set; }

        public string ApproverGroupName { get; set; }

        #endregion

    }
}