using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM
{
    public class WFMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WFM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WFM_default",
                "WFM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }

    }
}