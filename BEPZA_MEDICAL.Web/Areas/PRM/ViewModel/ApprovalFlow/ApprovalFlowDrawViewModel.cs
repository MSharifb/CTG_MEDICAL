using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow
{
    public class ApprovalFlowDrawViewModel
    {
        public ApprovalFlowDrawViewModel()
        {
        }

        public int StepId { get; set; }

        public string ApprovalProcessName { get; set; }

        public string FlowName { get; set; }

        public string StepName { get; set; }

        public string ApproverIdAndName { get; set; }

        public int InitialStepId { get; set; }

    }
}