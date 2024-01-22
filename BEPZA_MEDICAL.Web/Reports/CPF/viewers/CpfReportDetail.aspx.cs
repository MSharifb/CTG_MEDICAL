using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.CPF.viewers
{
    public partial class CpfReportDetail : ReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void rvCpfRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Page_Load(null, null);
        }
    }
}