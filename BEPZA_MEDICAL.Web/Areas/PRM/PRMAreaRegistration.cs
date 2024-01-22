using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.PRM
{
    public class PRMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PRM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            string nameSpace = AppConstant.ProjectName + ".Areas." + this.AreaName + "." + "Controllers";
            context.MapRoute(
                "PRM_default",
                "PRM/{controller}/{action}/{id}",
                new {  action = "Index", id = UrlParameter.Optional },
                new[] { nameSpace } 
                
            );
        }
    }
}
