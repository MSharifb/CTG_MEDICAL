using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow
{
    public class ApprovalFlowDetailViewModel : BaseViewModel
    {
        public ApprovalFlowDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            this.StepTypeList = new List<SelectListItem>();
            this.StepSequenceList = new List<SelectListItem>();

            this.DetailList = new List<ApprovalFlowDetailViewModel>();
        }

        #region Std Prop

        public int ApprovalFlowMasterId { get; set; }

        [DisplayName("Step Name")]
        public string StepName { get; set; }

        [DisplayName("Step Type")]
        public int ApprovalStepTypeId { get; set; }

        [DisplayName("Step Sequence")]
        public string StepSequence { get; set; }

        [DisplayName("Notification Message")]
        public string NotificationMessage { get; set; }

        #endregion

        #region Other Prop

        public IList<SelectListItem> StepTypeList { get; set; }

        public IList<SelectListItem> StepSequenceList { get; set; }

        public string StepTypeName { get; set; }

        public List<ApprovalFlowDetailViewModel> DetailList { get; set; }

        #endregion
    }
}