using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.SWM
{
    public class SWMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SWM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SWM_default",
                "SWM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "BEPZA_MEDICAL.Web.Areas.SWM.Controllers" }
            );
        }
    }
}