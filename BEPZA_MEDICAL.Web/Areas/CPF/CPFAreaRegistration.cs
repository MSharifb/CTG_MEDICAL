using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.CPF
{
    public class CPFAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CPF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //string nameSpace = AppConstant.ProjectName + ".Areas." + this.AreaName + "." + "Controllers";
            //context.MapRoute(
            //    "CPF_default",
            //    "CPF/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional },
            //    new[] { nameSpace }

            //);

            context.MapRoute(
                "CPF_default",
                "CPF/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}