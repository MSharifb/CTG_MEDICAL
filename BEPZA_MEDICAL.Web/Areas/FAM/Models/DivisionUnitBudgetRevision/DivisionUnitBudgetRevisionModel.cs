using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitBudgetRevision
{
    public class DivisionUnitBudgetRevisionModel
    {
        #region Ctor
        public DivisionUnitBudgetRevisionModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.DivisionUnitList = new List<SelectListItem>();
            this.FinancialYearList = new List<SelectListItem>();
            this.DivisionUnitBudgetRevisionList = new List<DivisionUnitBudgetRevision>();

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        [DisplayName("Financial Year")]
        [Required]
        public int FinancialYearId { get; set; }
        public string FinancialYearName { get; set; }

        [DisplayName("Division/Unit")]
        [Required]
        public int DivisionUnitId { get; set; }
        public string DivisionUnitName { get; set; }

        [DisplayName("Revision No")]
        [Required]
        [UIHint("_OnlyInteger")]
        public int RevisionNo { get; set; }

        [DisplayName("Remarks")]
        [UIHint("_MultiLine")]
        //[MaxLength(500, ErrorMessage="The field Remarks must be string with a maximum length of 500")] 
        [MaxLength(500)]
        public string Remarks { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public IList<SelectListItem> DivisionUnitList { get; set; }
        public IList<SelectListItem> FinancialYearList { get; set; }
        
        #endregion

        #region Other
        public string Mode { get; set; }
        public ICollection<DivisionUnitBudgetRevision> DivisionUnitBudgetRevisionList { get; set; }
        #endregion
    }



    public class DivisionUnitBudgetRevision
    {
        #region Ctor
        public DivisionUnitBudgetRevision()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }
        #endregion

        #region Standard Property
        public int Id { get; set; }

        public int DivisionUnitBudgetRevisionId { get; set; }

        public int RevisionAccountHeadId { get; set; }
        [UIHint("_ReadOnly")]
        public string AccountHeadName { get; set; }
        public string AccountHeadType { get; set; }

        [UIHint("_ReadOnly")]
        public decimal PreviousBudget { get; set; }

        [UIHint("_OnlyCurrency")]
        [Range(0, 9999999999, ErrorMessage = "Amount should be between 0 and 9999999999")]
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