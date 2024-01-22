using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAM
{
    public class FAMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "FAM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "FAM_default",
                "FAM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
