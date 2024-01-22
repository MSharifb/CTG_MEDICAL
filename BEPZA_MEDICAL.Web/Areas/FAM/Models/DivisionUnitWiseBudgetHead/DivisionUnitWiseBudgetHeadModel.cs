using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitWiseBudgetHead
{
    public class DivisionUnitWiseBudgetHeadModel
    {
        #region Ctor
        public DivisionUnitWiseBudgetHeadModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.DivisionUnitList = new List<SelectListItem>();
            this.Mode = "Create";
            this.BudgetHeadAllocationList = new List<BudgetHeadAllocation>();
        }
        #endregion


        #region Standard Property
        //public int Id { get; set; }

        [DisplayName("Division/Unit")]
        [Required]
        public int DivisionUnitId { get; set; }
        public IList<SelectListItem> DivisionUnitList { get; set; }
        public string DivisionUnitName { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }
        #endregion


        #region Other
        public string Mode { get; set; }
        public ICollection<BudgetHeadAllocation> BudgetHeadAllocationList { get; set; }
        #endregion

        #region Inner class
        
        #endregion
    }

    public class BudgetHeadAllocation
    {
        public BudgetHeadAllocation()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.DivisionUnitList = new List<SelectListItem>();
        }
        [Required]
        public int Id { get; set; }
        public int DivisionUnitId { get; set; }
        public int AccountHeadId { get; set; }
        public string AccountHeadName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsUsed { get; set; }
        public string AccountHeadType { get; set; }
        public IList<SelectListItem> DivisionUnitList { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

    }
}