using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.FixedParameters
{
    public class FixedParametersSetupModel : BaseViewModel
    {
        public FixedParametersSetupModel()
        {
            CPFContributionRateSetupModel = new CPFContributionRateSetupModel();
            //GratuityInterestRateSetupModel = new GratuityInterestRateSetupModel();
        }

        public CPFContributionRateSetupModel CPFContributionRateSetupModel { get; set; }
        //public GratuityInterestRateSetupModel GratuityInterestRateSetupModel { get; set; }
    }
}