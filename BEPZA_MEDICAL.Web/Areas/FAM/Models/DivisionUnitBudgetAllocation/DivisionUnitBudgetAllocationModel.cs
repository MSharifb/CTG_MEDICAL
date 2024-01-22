using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitBudgetAllocation
{
    public class DivisionUnitBudgetAllocationModel
    {
        #region Ctor
        public DivisionUnitBudgetAllocationModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.DivisionUnitList = new List<SelectListItem>();
            this.FinancialYearList = new List<SelectListItem>();
            this.BudgetAllocationList = new List<BudgetAllocation>();
            this.Mode = "Create";
        }
        #endregion

        #region Standard Property
        public int Id {get; set;}

        [DisplayName("Financial Year")]
        [Required]
        public int FinancialYearId {get;set;}
        public string FinancialYearName { get; set; }

        [DisplayName("Division/Unit")]
        [Required]
        public int DivisionUnitId{get;set;}
        public string DivisionUnitName { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public IList<SelectListItem> FinancialYearList{get; set;}
        public IList<SelectListItem> DivisionUnitList {get; set;}
        #endregion

        #region Other
        public string Mode { get; set; }
        public ICollection<BudgetAllocation> BudgetAllocationList { get; set; }
        #endregion
    }

    
    public class BudgetAllocation
    {
        #region Ctor
        public BudgetAllocation()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }
        #endregion

        #region Standard Property
        public int Id {get; set;}

        public int DivisionUnitBudgetId {get; set;}

        public int AccountHeadId {get; set;}
        [UIHint("_ReadOnly")]
        public string AccountHeadName { get; set; }
        public string AccountHeadType { get; set; }
        [UIHint("_OnlyCurrency")]
        [Range(0, 9999999999, ErrorMessage = "Amount should be between 0 and 9999999999")]
        public decimal Amount {get; set;}
        [UIHint("_OnlyCurrency")]
        public decimal RevisedAmount { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }
        #endregion
    }
}