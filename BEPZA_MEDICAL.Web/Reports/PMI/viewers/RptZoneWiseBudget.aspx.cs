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

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptZoneWiseBudget : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptZoneWiseBudget()
        {
            //
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

                financialYearId = Convert.ToInt32(ddlFinancialYear.SelectedValue);
                statusId = Convert.ToInt32(ddlStatus.SelectedValue);

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

                GenerateReport(financialYearId, statusId, strZoneId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvZoneWiseBudget.Reset();
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
        public void GenerateReport(int financialYearId, int statusId, string strZoneId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            int projectForId = Convert.ToInt32(Session["ProjectSectionId"]);

            #region Processing Report Data

            rvZoneWiseBudget.Reset();
            rvZoneWiseBudget.ProcessingMode = ProcessingMode.Local;
            rvZoneWiseBudget.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptZoneWiseBudget.rdlc");

            var data = pmiContext.sp_PMI_RptZoneWiseBudget(financialYearId, strZoneId, statusId, projectForId, numErrorCode, strErrorMsg).ToList();
            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;

                searchParameters = string.Concat(ddlStatus.SelectedItem.Text.ToString(), " Budget For The Year ", ddlFinancialYear.SelectedItem.Text.ToString());

                ReportParameter p1 = new ReportParameter("param", searchParameters);

                ReportParameter p2 = new ReportParameter("param1", searchParameters);

                rvZoneWiseBudget.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsZoneWiseBudget", data);

                rvZoneWiseBudget.LocalReport.DataSources.Add(dataSource);
                this.rvZoneWiseBudget.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvZoneWiseBudget.Reset();
            }
            rvZoneWiseBudget.DataBind();

            //ExportToPDF
            String newFileName = "ZoneWiseBudget_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvZoneWiseBudget, newFileName, fs);

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
                    case "_LSRreportHeader":
                        data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                                select c).ToList();
                        dsName = "DSCompanyInfo";
                        break;

                    default:
                        break;
                }
                e.DataSources.Add(new ReportDataSource(dsName, data));
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region User Methods
        private void PopulateDropdownList()
        {
            ddlFinancialYear.DataSource = pmiContext.acc_Accounting_Period_Information.OrderByDescending(x => x.yearName).ToList();
            ddlFinancialYear.DataValueField = "Id";
            ddlFinancialYear.DataTextField = "yearName";
            ddlFinancialYear.DataBind();
            ddlFinancialYear.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlStatus.DataSource = pmiContext.vwPMIStatusInformation.Where(x => x.ApplicableFor == "Budget").ToList();
            ddlStatus.DataValueField = "Id";
            ddlStatus.DataTextField = "Name";
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }
        #endregion

        protected void rvZoneWiseBudget_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}