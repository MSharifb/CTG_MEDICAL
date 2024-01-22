using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Data.Entity.SqlServer;
using System.Web.Services;
using System.Collections;
using System.Web.Script.Services;

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptProgressReport : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptProgressReport()
        {
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

        int financialYearId = 0;
        int statusId = 0;
        string strZoneId = string.Empty;

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;

                financialYearId = Convert.ToInt32(ddlProgressReport.SelectedValue);
                //statusId = Convert.ToInt32(ddlStatus.SelectedValue);

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

                strZoneId = ConvertZoneArrayListToString(arrZoneList);

                GenerateReport(financialYearId, strZoneId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvFinancialBudget.Reset();
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int financialYearId, string strZoneId)
        {
            int projectForId = Convert.ToInt32(Session["ProjectSectionId"]);

            #region Processing Report Data

            rvFinancialBudget.Reset();
            rvFinancialBudget.ProcessingMode = ProcessingMode.Local;
            rvFinancialBudget.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptProgressReport_M1.rdlc");
            var data = pmiContext.sp_PMI_ProgressReport_M1(financialYearId,projectForId,strZoneId).ToList();
            var data1 = pmiContext.sp_PMI_ProgressReport_Attachment(financialYearId).ToList();
            var signature = pmiContext.SP_PMI_Signature(financialYearId, "PRT").ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                //string searchParameters = string.Empty;
                //searchParameters = "Development/Non-Development Budget";
                //if (!string.IsNullOrEmpty(strBudgetType))
                //{
                //    searchParameters = strBudgetType + " Budget";
                //}
                //ReportParameter p1 = new ReportParameter("param", searchParameters);
                //rvFinancialBudget.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsProgressReport", data);
                ReportDataSource dataSource1 = new ReportDataSource("dsProgressReportAttach", data1);
                ReportDataSource dataSource2 = new ReportDataSource("dsSignature", signature);

                rvFinancialBudget.LocalReport.DataSources.Add(dataSource);
                rvFinancialBudget.LocalReport.DataSources.Add(dataSource1);
                rvFinancialBudget.LocalReport.DataSources.Add(dataSource2);

                this.rvFinancialBudget.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvFinancialBudget.Reset();
            }
            rvFinancialBudget.DataBind();

            //ExportToPDF
            String newFileName = "ProgressReport_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFinancialBudget, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                dynamic data = null;
                var dsName = string.Empty;
                switch (e.ReportPath)
                {
                    case "_ReportHeader":
                        data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                                select c).ToList();
                        dsName = "DSCompanyInfo";
                        break;

                    default:
                        break;
                }
                e.DataSources.Add(new ReportDataSource(dsName, data));
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region User Methods

        private void PopulateDropdownList()
        {
            int projectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            var items = (from pr in pmiContext.PMI_ProgressReportMaster
                         where (pr.ProjectForId == projectForId && pr.ZoneInfoId == LoggedUserZoneInfoId)
                         select new
                         {
                             Id = pr.Id,
                             Name = SqlFunctions.DateName("day", pr.ReportDate) + "-" + SqlFunctions.DateName("month", pr.ReportDate) + "-" + SqlFunctions.DateName("year", pr.ReportDate)
                         }).ToList();

            ddlProgressReport.DataSource = items;
            ddlProgressReport.DataValueField = "Id";
            ddlProgressReport.DataTextField = "Name";
            ddlProgressReport.DataBind();
            ddlProgressReport.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            tbProjectforId.Text = Convert.ToInt32(Session["ProjectSectionId"]).ToString();
        }
        #endregion

        protected void rvFinancialBudget_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        [WebMethod]
        public static ArrayList FetchProgressReport( int projectForId, string fromDate, string toDate)
        {
            ReportBase dbContext = new ReportBase();
            ArrayList list = new ArrayList();
            DateTime fDate = Convert.ToDateTime(fromDate);
            DateTime tDate = Convert.ToDateTime(toDate);
            var items = (from pr in dbContext.pmiContext.PMI_ProgressReportMaster
                         where (pr.ReportDate >= fDate && pr.ReportDate <= tDate && pr.ProjectForId == projectForId)
                         select new
                         {
                             Id = pr.Id,
                             Name = SqlFunctions.DateName("day", pr.ReportDate) + "-" + SqlFunctions.DateName("month", pr.ReportDate) + "-" + SqlFunctions.DateName("year", pr.ReportDate)
                         }).ToList();
            foreach (var item in items)
            {
                list.Add(item);
            }
            return list;
        }

    }
}