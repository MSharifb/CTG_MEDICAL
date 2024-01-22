using System;
using System.Web.UI;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class ReportMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterStartupManager();
        }

        private void RegisterStartupManager()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DateImageShow", "DateImageShow();", true);
        }
    }
}