using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.ForfeitedRuleViewModel
{
    
     public class ForfeitedRuleViewModel
    {  
        
        #region Ctor
        public ForfeitedRuleViewModel()
        {
                    
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
        [DisplayName("From Service Length")]       
        [UIHint("_OnlyNumber")]
        [Range(0, 99999999, ErrorMessage = "From Service Length must be greater than zero.")]
        public decimal FromServiceLength { set; get; }
      

        [Required]
        [DisplayName("To Service Length")]
        [UIHint("_OnlyNumber")]
        [Range(1, 99999999, ErrorMessage = "To Service Length must be greater than zero.")]
        public decimal ToServiceLength { set; get; }
       

        [Required]
        [DisplayName("Forfeited Rate")]
        [UIHint("_OnlyNumber")]
        [Range(0, 99999999, ErrorMessage = "Forfeited Rate must be greater than zero.")]
        public decimal ForfeitedRate { set; get; }


       
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
        public string strRuleName { set; get; }
        #endregion

        #region Other
        public String Mode { get; set; }
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            //if (!String.IsNullOrWhiteSpace(FromServiceLength))
            //    filterExpressionBuilder.Append(String.Format("Name like {0} AND ", FromServiceLength));
            //if (!String.IsNullOrWhiteSpace(ID))
            //    filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));
            //if (SortOrder > 0)
            //    filterExpressionBuilder.Append(String.Format("Rank = {0} AND ", SortOrder));


            //if (filterExpressionBuilder.Length > 0)
            //    filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }

       
        #endregion
    }
}