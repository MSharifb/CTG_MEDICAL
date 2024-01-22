using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptEmployeeClassWiseManpowerInfo : ReportBase
    {
        #region Page Event
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        #endregion


        #region Button Event
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            var asOnDate = Convert.ToDateTime(txtGenerationDate.Text);
            var employeeClassId = Convert.ToInt32(ddlEmployeeClass.SelectedValue);
            GenerateReport(asOnDate, employeeClassId);
        }
        #endregion

        #region Generate Report
        private void GenerateReport(DateTime asOnDate, int ClassId)
        {
            rvOrgWiseManpower.Reset();
            rvOrgWiseManpower.ProcessingMode = ProcessingMode.Local;
            rvOrgWiseManpower.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptEmployeeClassWiseManpowerInfo.rdlc");

            #region Processing Report Data
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
        
            var data = (from s in base.context.SP_PRM_RptEmployeeClassWiseManpowerInfo(asOnDate,ClassId) select s).ToList();
            #endregion

            #region Search parameter
            string searchParameters = string.Empty;
            //DateTime AsOnDate;         
            if (asOnDate != null)
            {
                string strAsOnDate = asOnDate.ToString("dd-MMM-yy");
                searchParameters = strAsOnDate;
            }

            ReportParameter p1 = new ReportParameter("AsOnDate", searchParameters);          
            rvOrgWiseManpower.LocalReport.SetParameters(new ReportParameter[] { p1 });

            #endregion

            rvOrgWiseManpower.LocalReport.DataSources.Add(new ReportDataSource("DSEmployeeClassWiseManpowerInfo", data));
            this.rvOrgWiseManpower.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvOrgWiseManpower.DataBind();

            //ExportToPDF
            String newFileName = "EmployeeClassWiseManpowerInfo_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvOrgWiseManpower, newFileName, fs);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            //dynamic data = null;
            //var dsName = "DSCompanyInfo";
            //data = (from c in base.context.vwCompanyInformations select c).ToList();
            //e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        #endregion


        #region User Methods

        private void PopulateDropdownList()
        {
            ddlEmployeeClass.DataSource = context.PRM_EmployeeClass.OrderBy(x => x.SortOrder).ToList();
            ddlEmployeeClass.DataValueField = "Id";
            ddlEmployeeClass.DataTextField = "Name";
            ddlEmployeeClass.DataBind();
            ddlEmployeeClass.Items.Insert(0, new ListItem("All", "0"));

        }

        #endregion

        protected void rvOrgWiseManpower_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}