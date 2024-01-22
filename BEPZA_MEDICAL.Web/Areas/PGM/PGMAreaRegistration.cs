using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PGM
{
    public class PGMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PGM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PGM_default",
                "PGM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
