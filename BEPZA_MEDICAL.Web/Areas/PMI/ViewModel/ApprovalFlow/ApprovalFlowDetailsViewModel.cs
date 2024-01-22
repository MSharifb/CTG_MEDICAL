using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow
{
    public class ApprovalFlowDetailsViewModel : BaseViewModel
    {
        public ApprovalFlowDetailsViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        #region Standard Property

        public int ApprovalFlowMasterId { get; set; }
        public int EmployeeId { get; set; }
        public int StatusId { get; set; }
        public string Remarks { get; set; }

        #endregion

        #region Others

        #endregion

    }
}