using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow
{
    [Serializable]
    public class ApproverInformationDetailsViewModel : BaseViewModel
    {
        public ApproverInformationDetailsViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            //this.ApproverInfoDetailList = new List<ApproverInformationDetailsViewModel>();
        }

        #region Std Prop
        [DisplayName("Step Name")]
        public int? ApprovalFlowDetailId { get; set; }

        [DisplayName("Recommender / Approver Type")]
        public int? ApproverTypeId { get; set; }

        [DisplayName("Approve By")]
        public int? AuthorTypeId { get; set; }

        public int? ApproverId { get; set; }

        public int? LevelId { get; set; }

        #endregion

        #region Other Prop

        [DisplayName("Flow Name")]
        public string ApprovalFlowName { get; set; }

        [DisplayName("Approval Process")]
        public string ApprovalProcessName { get; set; }


        [DisplayName("Employee Id")]
        public int? EmployeeId { get; set; }

        [DisplayName("Employee Id")]
        public string EmpId { get; set; }

        [DisplayName("Name")]
        public string EmployeeName { get; set; }

        [DisplayName("Designation")]
        public string Designation { get; set; }

        [DisplayName("Zone")]
        public int? ZoneId { get; set; }

        public string HeadOfLevelType { get; set; }

        public string OrganogramLevelAndPost { get; set; }

        public string ApproverTypeName { get; set; }

        public string SelectedLevelId { get; set; }

        //public List<ApproverInformationDetailsViewModel> ApproverInfoDetailList { get; set; }

        [DisplayName("Minimum Amount")]
        public decimal? MinAmount { get; set; }

        [DisplayName("Maximum Amount")]
        public decimal? MaxAmount { get; set; }

        #endregion

    }
}