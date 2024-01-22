using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.Withdraw
{
    public class WithdrawViewModel
    {

        #region Ctor
        public WithdrawViewModel()
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
        public int Id { get; set; }

        [Required]
        [DisplayName("Membership ID")]
        [UIHint("_ReadOnlyInteger")]
        public int MembershipId { get; set; }

        [Required]
        public int PeriodId { set; get; }

        [Required]
        [DisplayName("Request Date")]
        [UIHint("_Date")]
        public DateTime? RequestDate { set; get; }

        [Required]
        [DisplayName("Withdraw Date ")]
        [UIHint("_Date")]
        public DateTime? WithdrawDate { set; get; }

        [Required]
        [DisplayName("Withdraw No.")]
        [UIHint("_ReadOnly")]
        public string WithdrawNo { get; set; }

        [Required]
        [DisplayName("Employee Portion Balance")]
        [UIHint("_ReadOnlyAmount")]
        [Range(1, 999999999999, ErrorMessage = "Employee Portion Balance must be greater than zero.")]
        public decimal EmpPortionBalance { set; get; }

        [Required]
        [DisplayName("Withdraw Amount")]
        [UIHint("_OnlyCurrency")]
        [Range(1, 999999999999, ErrorMessage = "Withdraw Amount must be greater than zero.")]
        public decimal WithdrawAmount { set; get; }

        [Required]
        [DisplayName("Already Withdrawn Amount")]
        [UIHint("_ReadOnlyAmount")]
        [Range(0, 999999999999, ErrorMessage = "Already Withdrawn Amount must be greater than zero.")]
        public decimal AlreadyWithdrawnAmount { set; get; }

        [Required]
        [DisplayName("Reason")]
        public string Reason { get; set; }

        # region View Property

        [Required]
        [DisplayName("Employee ID")]
        public string EmployeeCode { get; set; }

        [Required]
        [DisplayName("Membership ID")]
        [UIHint("_ReadOnly")]
        public string MembershipCode { get; set; }

        //[Required]
        //[DisplayName("Employee Initial")]
        //[UIHint("_ReadOnly")]
        //public string EmployeeInitial { get; set; }

        [Required]
        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }      

        [DisplayName("CPF Period")]
        [UIHint("_ReadOnly")]
        public string CpfPeriod { set; get; }
        public IList<SelectListItem> CPFPeriodList { set; get; }

       
        [DisplayName("Withdraw to Date ")]
        [UIHint("_Date")]
        public DateTime? WithdrawToDate { set; get; }
        
        [DisplayName("Membership Date ")]
        [UIHint("_Date")]
        public DateTime? MembershipDate { set; get; }

        public decimal EmpContibutionInPeriod { set; get; }


        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public string strMessage { set; get; }
        public string errClass { set; get; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        #endregion

        #endregion

        #region Other
        public String Mode { get; set; }
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