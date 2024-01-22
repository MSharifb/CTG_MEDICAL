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
    public partial class RptFinancialBudget : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptFinancialBudget()
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
        string strBudgetType = string.Empty;


        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;

                financialYearId = Convert.ToInt32(ddlFinancialYear.SelectedValue);
                statusId = Convert.ToInt32(ddlStatus.SelectedValue);
                strBudgetType = ddlBudgetType.SelectedValue;

                GenerateReport(financialYearId, statusId, strBudgetType);

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
        public void GenerateReport(int financialYearId, int statusId, string strBudgetType)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvFinancialBudget.Reset();
            rvFinancialBudget.ProcessingMode = ProcessingMode.Local;
            rvFinancialBudget.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptFinancialBudget.rdlc");

            var data = pmiContext.sp_PMI_RptFinancialBudget(financialYearId, statusId, strBudgetType, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Development/Non-Development Budget";
                if (!string.IsNullOrEmpty(strBudgetType))
                {
                    searchParameters = strBudgetType + " Budget";
                }
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                searchParameters = "Financial Year : " + ddlFinancialYear.SelectedItem.Text.ToString();
                ReportParameter p2 = new ReportParameter("param1", searchParameters);

                rvFinancialBudget.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsFinancialBudget", data);
                rvFinancialBudget.LocalReport.DataSources.Add(dataSource);
                this.rvFinancialBudget.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvFinancialBudget.Reset();
            }
            rvFinancialBudget.DataBind();

            //ExportToPDF
            String newFileName = "FinancialBudget_" + Guid.NewGuid() + ".pdf";
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

            ddlBudgetType.DataSource = pmiContext.PMI_BudgetType.ToList();
            ddlBudgetType.DataValueField = "TypeName";
            ddlBudgetType.DataTextField = "TypeName";
            ddlBudgetType.DataBind();
            //ddlBudgetType.Items.Insert(0, new ListItem("[Select One]", "0"));
            ddlBudgetType.Items.Insert(0, new ListItem("[All]", ""));

        }
        #endregion

        protected void rvFinancialBudget_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}