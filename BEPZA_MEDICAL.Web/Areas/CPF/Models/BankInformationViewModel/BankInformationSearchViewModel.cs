using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.BankInformationViewModel
{
    public class BankInformationSearchViewModel
    {
 #region Ctor

        public BankInformationSearchViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;           
        }

        #endregion

        #region Standard Property


        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Bank Name")]
        public int? BankId { set; get; }
        
        [Required]
        [DisplayName("Bank Branch Name")]
        public int? BranchId { set; get; }
       

        [Required]
        [DisplayName("Account No")]
        public string AccountNo { set; get; }

        [Required]
        [UIHint("_OnlyNumber")]
        [Range(1, 999999999999999999)]
        public decimal OpeningBalance { set; get; }
        
        public string BankName { get; set; }

        [DisplayName("Branch Name")]
        [UIHint("_ReadOnly")]
        public string BranchName { get; set; }


        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        #endregion

       
    }
}