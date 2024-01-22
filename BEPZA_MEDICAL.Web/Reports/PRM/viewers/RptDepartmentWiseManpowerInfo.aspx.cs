using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptDepartmentWiseManpowerInfo : ReportBase
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
            var fromDate = Convert.ToDateTime(txtFromDate.Text);
            var toDate = Convert.ToDateTime(txtToDate.Text);
            var divisionId = Convert.ToInt32(ddlDivision.SelectedValue);
            GenerateReport(fromDate, toDate, divisionId);
        }
        #endregion

        #region Generate Report
        private void GenerateReport(DateTime fromDate, DateTime toDate, int divisionId)
        {
            rvDeptWiseManpower.Reset();
            rvDeptWiseManpower.ProcessingMode = ProcessingMode.Local;
            rvDeptWiseManpower.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptDepartmentWiseManpowerInfo.rdlc");

            #region Processing Report Data
         
            var data = (from s in base.context.SP_PRM_RptDepartmentWiseManpowerInfo(fromDate,toDate, divisionId) select s).ToList();
            #endregion

            #region Search parameter
            string searchParameters = string.Empty;
            string searchParameters1 = string.Empty;
            string searchParameters2 = string.Empty;

            if (fromDate != null && toDate != null)
            {
                searchParameters = fromDate.ToString("dd-MMM-yy");
                searchParameters1 = toDate.ToString("dd-MMM-yy");
            }

            searchParameters2 = ddlDivision.SelectedItem.Text;
            //if (divisionId != 0)
            //{
            //    searchParameters2 = ddlDivision.SelectedItem.Text;
            //}

            ReportParameter p1 = new ReportParameter("FromDate", searchParameters);
            ReportParameter p2 = new ReportParameter("ToDate", searchParameters1);
            ReportParameter p3 = new ReportParameter("Department", searchParameters2);

            rvDeptWiseManpower.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3 });

            #endregion

            rvDeptWiseManpower.LocalReport.DataSources.Add(new ReportDataSource("DSDepartmentWiseManpowerInfo", data));
            this.rvDeptWiseManpower.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvDeptWiseManpower.DataBind();
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        #endregion


        #region User Methods

        private void PopulateDropdownList()
        {
            ddlDivision.DataSource = context.PRM_Division.Where(x=>x.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            ddlDivision.DataValueField = "Id";
            ddlDivision.DataTextField = "Name";
            ddlDivision.DataBind();
            ddlDivision.Items.Insert(0, new ListItem("All", "0"));

        }

        #endregion

        protected void rvDeptWiseManpower_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}