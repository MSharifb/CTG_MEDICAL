using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval
{
    public class ApprovalHistoryViewModel
    {
        [DisplayName("Step : ")]
        public string ApprovalStepName { get; set; }

        [DisplayName("Approver : ")]
        public string ApproverIdAndName { get; set; }

        [DisplayName("Comments : ")]
        public string ApproverComment { get; set; }

        [DisplayName("Status : ")]
        public string ApprovalStatus { get; set; }

        public string ApproveDate { get; set; }
    }
}