using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class JobRequisitionInfoApprovalDetailViewModel : BaseViewModel
    {
        #region Ctor
        public JobRequisitionInfoApprovalDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

        }
        #endregion

        #region Standard Property

        public int JobRequisitionInfoApprovalId { get; set; }
        public int JobRequisitionInfoSummaryDtlId { get; set; }
        public int? ApprovedPost { get; set; }

        #endregion

        #region Other

        public bool IsChecked { get; set; }
        public int RequisionId { get; set; }
        public int RequisitionSummaryId { get; set; }
        public string RequisitionNo { get; set; }
        public string ReqPreparedBy { get; set; }
        public string Designation { get; set; }
        public string SubmissionDate { get; set; }

        #endregion
    }
}