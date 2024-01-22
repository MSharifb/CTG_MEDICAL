using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptSuspensionEmployee__ : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptSuspensionEmployee__()
        {
            _empRepository = new PRM_GenericRepository<PRM_EmploymentInfo>(new ERP_BEPZAPRMEntities());
        }
        #endregion

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
        string fromDate = string.Empty;
        string toDate = string.Empty;
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> zoneList = new List<int>();
                int[] arrZoneList = new int[] { };
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                arrZoneList = zoneList.ToArray();

                string strZoneId = ConvertZoneArrayListToString(arrZoneList);

                fromDate = txtFromDate.Text;
                toDate = txtToDate.Text;
                GenerateReport(fromDate, toDate, strZoneId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(string fromDate, string toDate, string strZoneId)
        {
            #region Processing Report Data

            rvTestResult.Reset();
            rvTestResult.ProcessingMode = ProcessingMode.Local;
            rvTestResult.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptSuspensionEmployee.rdlc");

            var data = (from s in base.prmContext.SP_PRM_RptSuspensionEmployees(fromDate, toDate, strZoneId) select s).ToList();

            #region Search parameter


            string searchParameters1 = string.Empty;
            string searchParameters2 = string.Empty;
            searchParameters1 =(Convert.ToDateTime(fromDate)).ToString("dd-MMM-yy");
            searchParameters2 =(Convert.ToDateTime(toDate)).ToString("dd-MMM-yy");

            ReportParameter p1 = new ReportParameter("FromDate", searchParameters1);
            ReportParameter p2 = new ReportParameter("ToDate", searchParameters2);

            rvTestResult.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSSuspensionEmployee", data);
            rvTestResult.LocalReport.DataSources.Add(dataSource);
            this.rvTestResult.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvTestResult.DataBind();

            //ExportToPDF
            String newFileName = "SuspensionEmployeeList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvTestResult, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            var empId = string.Empty;
            switch (e.ReportPath)
            {
                case "_SuspensionReportHeader":
                    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                    dsName = "DSCompanyInfo";
                    break;

                default:
                    break;
            }
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }
        #endregion

        protected void rvTestResult_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region User Methods
        private void PopulateDropdownList()
        {
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }
        #endregion


    }
}