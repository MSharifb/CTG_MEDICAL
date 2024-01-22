using BEPZA_MEDICAL.Domain.PGM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.OpeningBalance
{
    public class OpeningBalanceViewModel : BaseViewModel
    {
        #region Ctor
        public OpeningBalanceViewModel()
        {
            this.CPFPeriodList = new List<SelectListItem>();
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = "Create";
        }
        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Employee Id")]
        public int EmployeeId { get; set; }

        [Required]
        [DisplayName("Employee Id")]
        public string EmployeeCode { get; set; }

        [DisplayName("Employee Initial")]
        [UIHint("_ReadOnly")]
        public string EmployeeInitial { get; set; }

        [Required]
        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        [Required]
        [DisplayName("PF ID")]
        [UIHint("_ReadOnlyInteger")]
        public int MembershipId { get; set; }

        [Required]
        [DisplayName("Membership Id")]
        [UIHint("_ReadOnly")]
        public string MembershipCode { get; set; }

        [DisplayName("CPF Period")]
        [UIHint("_ReadOnly")]
        public string CpfPeriod { set; get; }
        public IList<SelectListItem> CPFPeriodList { set; get; }

        [Required]
        public int PeriodId { set; get; }

        [DisplayName("Designation")]
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        [DisplayName("Joining Date")]
        [UIHint("_ReadOnlyDate")]
        public DateTime JoiningDate { get; set; }

        [DisplayName("Date of Membership")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? DateOfMembership { get; set; }

        [Required]
        [DisplayName("Opening Date")]
        [UIHint("_Date")]
        public DateTime? DateOfOpening { set; get; }

        [Required]
        //[UIHint("_OnlyNumber")]
        [Range(0, 999999999999)]
        public decimal PrevYearEmpCoreContribution { set; get; }

        [Required]
        [DisplayName("Core Contribution")]
        //[UIHint("_OnlyNumber")]
        [Range(-999999999999, 999999999999)]
        public decimal EmpCoreContribution { set; get; }


        [Required]
        //[UIHint("_OnlyNumber")]
        [Range(0, 999999999999)]
        public decimal PrevYearEmpProfit { set; get; }

        [Required]
        [DisplayName("Share of Interst")]
        //[UIHint("_OnlyNumber")]
        [Range(-999999999999, 999999999999)]
        public decimal EmpProfit { set; get; }

        [Required]
        [DisplayName("Total")]
        [UIHint("_ReadOnlyAmount")]
        [Range(-999999999999, 999999999999)]
        public decimal EmpTotalContrib { set; get; }

        [Required]
        [DisplayName("Total")]
        [UIHint("_ReadOnlyAmount")]
        [Range(-999999999999, 999999999999)]
        public decimal EmpTotalProfit { set; get; }

        [Required]
        //[UIHint("_OnlyNumber")]
        [Range(0, 999999999999)]
        public decimal PrevYearComCoreContribution { set; get; }

        [Required]
        [DisplayName("Core Contribution")]
        //[UIHint("_OnlyNumber")]
        [Range(-999999999999, 999999999999)]
        public decimal ComCoreContribution { set; get; }

        [Required]
        //[UIHint("_OnlyNumber")]
        [Range(0, 999999999999)]
        public decimal PrevYearComProfit { set; get; }

        [Required]
        [DisplayName("Share of Interst")]
        //[UIHint("_OnlyNumber")]
        [Range(-999999999999, 999999999999)]
        public decimal ComProfit { set; get; }

        [Required]
        [DisplayName("Total")]
        [UIHint("_ReadOnlyAmount")]
        [Range(-999999999999, 999999999999)]
        public decimal ComTotalContrib { set; get; }

        [Required]
        [DisplayName("Total")]
        [UIHint("_ReadOnlyAmount")]
        [Range(-999999999999, 999999999999)]
        public decimal ComTotalInterest { set; get; }

        public string MembershipIDName { set; get; }

        [DisplayName("Permanent Date")]
        [UIHint("_Date")]
        public DateTime? PermanentDate { set; get; }

        [DisplayName("Permanent Date To")]
        [UIHint("_Date")]
        public DateTime? PermanentDateTo { set; get; }

        //public string IUser { get; set; }
        //public string EUser { get; set; }
        //public DateTime IDate { get; set; }
        //public DateTime EDate { get; set; }

        public string strMessage { set; get; }
        //public string errClass { set; get; }
        //public int IsError { set; get; }
        //public string ErrMsg { set; get; }
        #endregion

        #region Other
        public String Mode { get; set; }

        [CustomDisplay("CustomDisplay")]
        public String OpeningBalanceCompany { get; set; }

        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            //if (ID.HasValue)
            //    filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));      


            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }

        #endregion
    }
}
