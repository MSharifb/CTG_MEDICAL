using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV
{
    public class PMIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PMI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            string nameSpace = AppConstant.ProjectName + ".Areas." + this.AreaName + "." + "Controllers";

            context.MapRoute(
                "PMI_default",
                "PMI/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { nameSpace }
            );
        }
    }
}