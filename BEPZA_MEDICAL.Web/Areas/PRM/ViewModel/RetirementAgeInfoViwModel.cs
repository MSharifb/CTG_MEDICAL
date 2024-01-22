using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class RetirementAgeInfoViwModel : BaseViewModel
    {
        #region Ctor
        public RetirementAgeInfoViwModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }
        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Retirement Age")]
        public double RetirementAge { get; set; }

        [Required]
        [DisplayName("Retirement Age for Freedom Fighter")]
        public double FreedomFighterAge { get; set; }

        #endregion

        #region Other Property
        public string btnText { get; set; }

        #endregion

    }
}