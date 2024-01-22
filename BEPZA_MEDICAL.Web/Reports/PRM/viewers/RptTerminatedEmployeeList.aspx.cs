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
    public partial class RptTerminatedEmployeeList : ReportBase
    {
       
        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
              
            }
        }

        #endregion

        #region Button Event

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            var seperationDateFrom = Convert.ToDateTime(txtSeperationDateFrom.Text);
            var seperationDateTo = Convert.ToDateTime(txtSeperationDateTo.Text);

            GenerateEmployeeInfoReport(seperationDateFrom, seperationDateTo);
        }

        #endregion

        #region Generate Report

        public void GenerateEmployeeInfoReport(DateTime seperationDateFrom, DateTime seperationDateTo)
        {
            rvTerminatedEmployeeList.Reset();
            rvTerminatedEmployeeList.ProcessingMode = ProcessingMode.Local;
            rvTerminatedEmployeeList.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptTerminatedEmployeeList.rdlc");


            #region Processing Report Data

            var company = (from c in base.context.vwCompanyInformations
                           select c).ToList();

            var data = (from s in base.context.SP_PRM_RptTerminatedEmployeeList(seperationDateFrom, seperationDateTo) select s).ToList();


            #endregion


            #region Search parameter
            string searchParameters = string.Empty;
            string searchParameters1 = string.Empty;

            //       
            if (seperationDateFrom != null && seperationDateTo != null)
            {
                string fromDate = seperationDateFrom.ToString("dd-MMM-yy");
                searchParameters = fromDate;

                string toDate = seperationDateTo.ToString("dd-MMM-yy");
                searchParameters1 = toDate;
            }

            ReportParameter p1 = new ReportParameter("FromDate", searchParameters);
            ReportParameter p2 = new ReportParameter("ToDate", searchParameters1);
            rvTerminatedEmployeeList.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSTerminatedEmployeeList", data);
            rvTerminatedEmployeeList.LocalReport.DataSources.Add(dataSource);
            this.rvTerminatedEmployeeList.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvTerminatedEmployeeList.DataBind();

            //ExportToPDF
            String newFileName = "TerminatedEmployeeList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvTerminatedEmployeeList, newFileName, fs);
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

     
        protected void rvTerminatedEmployeeList_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
     
                
    }
}
