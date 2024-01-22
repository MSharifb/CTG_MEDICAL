using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.RevisionOfCentralBudget
{
    public class RevisionOfCentralBudgetModel
    {
        #region Ctor
        public RevisionOfCentralBudgetModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.FinancialYearList = new List<SelectListItem>();
            this.CentralBudgetRevisionList = new List<CentralBudgetRevision>();
            this.ApprovalPathList = new List<SelectListItem>();
            this.NextApprovalNodeList = new List<SelectListItem>();

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        [DisplayName("Financial Year")]
        [Required]
        public int FinancialYearId { get; set; }
        public string FinancialYearName { get; set; }

        [DisplayName("Revision No")]
        [Required]
        [UIHint("_OnlyInteger")]
        public int RevisionNo { get; set; }

        [DisplayName("Remarks")]
        [UIHint("_MultiLine")]
        //[MaxLength(500, ErrorMessage="The field Remarks must be string with a maximum length of 500")] 
        [MaxLength(500)]
        public string Remarks { get; set; }

        [DisplayName("Approval Path")]
        [Required]
        public int ApprovalPathId { get; set; }
        public string ApprovalPathName { get; set; }

        [DisplayName("Next Approver")]
        public int CurrentApprovalNodeId { get; set; }
        public string ApprovalStatus { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }
        public IList<SelectListItem> ApprovalPathList { get; set; }
        public IList<SelectListItem> NextApprovalNodeList { get; set; }
        
        #endregion

        #region Other
        public string Mode { get; set; }
        public ICollection<CentralBudgetRevision> CentralBudgetRevisionList { get; set; }
        #endregion
    }

    public class CentralBudgetRevision
    {
        #region Ctor
        public CentralBudgetRevision()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        public int CentralBudgetRevisionId { get; set; }

        public int RevisionAccountHeadId { get; set; }
        [UIHint("_ReadOnly")]
        public string AccountHeadName { get; set; }
        public string AccountHeadType { get; set; }

        [UIHint("_OnlyCurrency")]
        public decimal PreviousBudget { get; set; }

        [UIHint("_OnlyCurrency")]
        public decimal RevisedBudget { get; set; }

        [StringLength(500)] 
        public string Remarks { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }
        #endregion
    }
}