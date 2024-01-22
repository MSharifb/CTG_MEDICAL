using BEPZA_MEDICAL.Domain.PGM;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.FixedParameters
{
    public class CPFContributionRateSetupModel : BaseViewModel
    {
        

        public CPFContributionRateSetupModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
            
        }

        #region Standard Property

        [DisplayName("Own Contribution Rate")]
        [Required]
        public decimal? OwnContributionRate { get; set; }
        
        /// <summary>
        /// Get custom display text from CustomAttributeModel class.
        /// </summary>
        [CustomDisplay("[CustomText]")]
        [Required]
        public decimal? OfficeContributionRate { get; set; }

        #endregion
    }
}