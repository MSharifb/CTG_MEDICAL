using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.BankInformationViewModel
{
    public class BankInformationViewModel
    {  
        
        #region Ctor
        public BankInformationViewModel()
        {
            this.BankList = new List<SelectListItem>();
            this.BankBranchList = new List<SelectListItem>();           
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = "Create";
            //this.IsActive = true;
        }
        #endregion

        #region Standard Property      
       
        
        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Bank Name")]
        public int? BankId { set; get; }
        public IList<SelectListItem> BankList { set; get; }

        [Required]
        [DisplayName("Bank Branch Name")]
        public int? BranchId { set; get; }
        public IList<SelectListItem> BankBranchList { set; get; }

        [Required]
        [DisplayName("Account No")]
        public string AccountNo { set; get; }

        [Required]
        [DisplayName("Opening Balance")]  
        [UIHint("_OnlyNumber")]
        [Range(1, 999999999999, ErrorMessage = "Opening Balance must be greater than zero ")]       
        public decimal OpeningBalance { set; get; }  
         
        [DisplayName("Remarks")]        
        public string Remarks { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        public string strMessage { set; get; }
        public string errClass { set; get; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }  
        #endregion

        #region Other
        public String Mode { get; set; }
       
        #endregion
    }
}