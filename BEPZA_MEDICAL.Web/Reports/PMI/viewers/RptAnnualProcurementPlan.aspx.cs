using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptAnnualProcurementPlan : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptAnnualProcurementPlan()
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
                int Id = 0;
                if (Request.QueryString["param1"] != null)
                {
                    int.TryParse(Request.QueryString["param1"], out Id);
                }
                if (Id > 0)
                {
                    ShowReport();
                }
            }

        }

        #endregion

        #region Button Event

        int financialYearId = 0;
        int statusId = 0;
        string strBudgetType = string.Empty;
        string strZoneId = string.Empty;

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;

                financialYearId = Convert.ToInt32(ddlFinancialYear.SelectedValue);
                strBudgetType = ddlBudgetType.SelectedValue;

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


                GenerateReport(0, financialYearId, statusId, strZoneId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvAPP.Reset();
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
        public void GenerateReport(int? id, int? financialYearId, int? statusId, string strZoneId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            int projectForId = Convert.ToInt32(Session["ProjectSectionId"]);

            #region Processing Report Data

            rvAPP.Reset();
            rvAPP.ProcessingMode = ProcessingMode.Local;
            rvAPP.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptAnnualProcurementPlan.rdlc");

            var data = pmiContext.sp_PMI_RptAnnualProcurementPlan( id, financialYearId, statusId, strZoneId, projectForId, numErrorCode, strErrorMsg).ToList();

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
                //searchParameters = "Financial Year : " + ddlFinancialYear.SelectedItem.Text.ToString();
                //ReportParameter p2 = new ReportParameter("param1", searchParameters);

                //rvAPP.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

                #endregion
                var Id = data.Select(x => x.MasterId).FirstOrDefault();
                //var budgetId = pmiContext.PMI_BudgetDetails.Where(s => s.Id == Id).Select(x => x.BudgetMasterId).FirstOrDefault();
                var signature = pmiContext.SP_PMI_Signature(Id, "APP").ToList();


                ReportDataSource dataSource = new ReportDataSource("dsAPP", data);
                ReportDataSource dataSource1 = new ReportDataSource("dsSignature", signature);

                rvAPP.LocalReport.DataSources.Add(dataSource);
                rvAPP.LocalReport.DataSources.Add(dataSource1);

                this.rvAPP.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvAPP.Reset();
            }
            rvAPP.DataBind();

            //ExportToPDF
            String newFileName = "APP_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvAPP, newFileName, fs);

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

            ddlBudgetType.DataSource = pmiContext.PMI_ProjectStatus.ToList();
            ddlBudgetType.DataValueField = "Id";
            ddlBudgetType.DataTextField = "Name";
            ddlBudgetType.DataBind();
            ddlBudgetType.Items.Insert(0, new ListItem("[All]", ""));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }
        #endregion

        protected void rvAPP_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        private void ShowReport()
        {
            int Id = 0;
            if (Request.QueryString["param1"] != null)
            {
                int.TryParse(Request.QueryString["param1"], out Id);
            }
            GenerateReport(Id, null, null, LoggedUserZoneInfoId.ToString());
        }

    }
}