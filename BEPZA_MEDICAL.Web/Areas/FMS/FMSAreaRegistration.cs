
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS
{
    public class FMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "FMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "FMS_default",
                "FMS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }

    }
}