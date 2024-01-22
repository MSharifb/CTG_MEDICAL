using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Models.BankInfo
{
    public class BankInformationModel
    {
        public BankInformationModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.BankInfoChilds = new List<BankInfoChild>();

            this.BankList = new List<SelectListItem>();
            this.BranchList = new List<SelectListItem>();
            this.AccHeadList = new List<SelectListItem>();

            this.Mode = "Create";
        }

        #region Standard Property

        public int Id { get; set; }

        [DisplayName("Bank Name")]
        [Required]
        public int BankId { get; set; }       

        public List<SelectListItem> BankList { get; set; }

        public string BankName { get; set; }

        [DisplayName("Branch Name")]
        [Required]
        public int BranchId { get; set; }

        public List<SelectListItem> BranchList { get; set; }

        [DisplayName("SWIFT Code")]
        public string SWIFTCode { get; set; }

        [DisplayName("Bank Address")]
        public string Address { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }
        
        #endregion

        public ICollection<BankInfoChild> BankInfoChilds { get; set; }

        public string Mode { get; set; }

        public int? AccHeadId { get; set; }        
        public List<SelectListItem> AccHeadList { get; set; }

        #region Methods
        public string GetFilterExpression(string bankName)
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            
            if (!String.IsNullOrWhiteSpace(bankName))
                filterExpressionBuilder.Append(String.Format("GradeName like {0} AND ", bankName));
                        
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }

    public class BankInfoChild
    {
        public BankInfoChild()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }

        #region Standard Property

        public int Id { get; set; }

        public int BankBranchMapId { get; set; }
        

        [DisplayName("Account Head")]        
        public int AccountHeadId { get; set; }

        [DisplayName("Bank Account Number")]
        public string BankAccountNo { get; set; }
        
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }
        
        #endregion
    }

}