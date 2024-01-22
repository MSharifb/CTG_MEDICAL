using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ApprovalFlowConfigurationViewModel : BaseViewModel
    {
        public ApprovalFlowConfigurationViewModel()
        {
            this.ApprovalProcessList = new List<SelectListItem>();
        }

        [DisplayName("Approval Process")]
        [Required]
        public int ApprovalProcesssId { get; set; }

        public bool IsConfigurableApprovalFlow { get; set; }

        public bool IsManualApprovalFlow { get; set; }

        public IList<SelectListItem> ApprovalProcessList { get; set; }

        public string ApprovalProcessName { get; set; }

    }
}