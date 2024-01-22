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
    public partial class RptProcurementPlan : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptProcurementPlan()
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
        int nameOfWorksId = 0;
        string strZoneId = string.Empty;
        DateTime? fromDate = null;
        DateTime? toDate = null;
        int statusId;


        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;
                statusId = 0;

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
                financialYearId = Convert.ToInt32(ddlFinancialYear.SelectedValue);
                statusId = Convert.ToInt32(ddlStatus.SelectedValue);

                if (dtFromDate.Text != "")
                {
                    fromDate = Convert.ToDateTime(dtFromDate.Text);
                }
                if (dtToDate.Text != "")
                {
                    toDate = Convert.ToDateTime(dtToDate.Text);
                }

                if (ddlNameOfWorks.SelectedIndex > 0)
                {
                    nameOfWorksId = Convert.ToInt32(ddlNameOfWorks.SelectedValue);
                }
                else if (ddlProcuringEntityName.SelectedIndex > 0)
                {
                    nameOfWorksId = Convert.ToInt32(ddlProcuringEntityName.SelectedValue);
                }
                else if (ddlProcuringEntityCode.SelectedIndex > 0)
                {
                    nameOfWorksId = Convert.ToInt32(ddlProcuringEntityCode.SelectedValue);
                }

                GenerateReport(financialYearId, strZoneId, fromDate, toDate, nameOfWorksId, statusId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvProcurementPlan.Reset();
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
        public void GenerateReport(int financialYearId, string strZoneId, DateTime? fromDate, DateTime? toDate, int nameOfWorksId, int statusId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvProcurementPlan.Reset();
            rvProcurementPlan.ProcessingMode = ProcessingMode.Local;
            rvProcurementPlan.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptProcurementPlan.rdlc");

            var data = pmiContext.sp_PMI_RptProcurementPlan(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, statusId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Procurement Plan";
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvProcurementPlan.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsProcurementPlan", data);
                rvProcurementPlan.LocalReport.DataSources.Add(dataSource);
                this.rvProcurementPlan.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvProcurementPlan.Reset();
            }
            rvProcurementPlan.DataBind();

            //ExportToPDF
            String newFileName = "ProcurementPlan_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvProcurementPlan, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                dynamic data = null;
                var ProjectMasterId = 0;
                var dsName = string.Empty;
                if (e.ReportPath != "_ReportHeader")
                {
                    ProjectMasterId = Convert.ToInt32(e.Parameters["ProjectMasterId"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["ProjectMasterId"].Values[0]));
                }

                switch (e.ReportPath)
                {
                    case "SubRptProcurementPlanDetails":
                        var procurementDetails = pmiContext.sp_PMI_RptProcurementPlanDetails(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, statusId, numErrorCode, strErrorMsg).ToList();
                        data = procurementDetails.Where(x => x.ProjectMasterId == ProjectMasterId).ToList();
                        dsName = "dsProcurementPlanDetails";
                        break;

                    case "SubRptProcurementPlanHead":
                        data = pmiContext.sp_PMI_RptProcurementPlan(financialYearId, fromDate, toDate, nameOfWorksId, strZoneId, statusId, numErrorCode, strErrorMsg).Where(x => x.Id == ProjectMasterId).ToList();
                        dsName = "dsProcurementPlan";
                        break;

                    case "SubRptProcurementPlanFund":
                        var procurementFund = pmiContext.vwPMIProcurementPlanFundRpt.ToList();
                        data = procurementFund.Where(x => x.ProjectMasterId == ProjectMasterId).ToList();
                        dsName = "dsProcurementPlanFund";
                        break;

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
            var projectMaster = pmiContext.PMI_ProjectMaster.ToList();

            ddlFinancialYear.DataSource = pmiContext.acc_Accounting_Period_Information.OrderByDescending(x => x.yearName).ToList();
            ddlFinancialYear.DataValueField = "Id";
            ddlFinancialYear.DataTextField = "yearName";
            ddlFinancialYear.DataBind();
            ddlFinancialYear.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlNameOfWorks.DataSource = projectMaster.OrderBy(x => x.NameOfWorks).ToList();
            ddlNameOfWorks.DataValueField = "Id";
            ddlNameOfWorks.DataTextField = "NameOfWorks";
            ddlNameOfWorks.DataBind();
            ddlNameOfWorks.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlProcuringEntityName.DataSource = projectMaster.OrderBy(x => x.ProcuringEntryName).ToList();
            ddlProcuringEntityName.DataValueField = "Id";
            ddlProcuringEntityName.DataTextField = "ProcuringEntryName";
            ddlProcuringEntityName.DataBind();
            ddlProcuringEntityName.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlProcuringEntityCode.DataSource = projectMaster.OrderBy(x => x.ProcuringEntryCode).ToList();
            ddlProcuringEntityCode.DataValueField = "Id";
            ddlProcuringEntityCode.DataTextField = "ProcuringEntryCode";
            ddlProcuringEntityCode.DataBind();
            ddlProcuringEntityCode.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlStatus.DataSource = pmiContext.vwPMIStatusInformation.Where(x => x.ApplicableFor == "Budget").ToList();
            ddlStatus.DataValueField = "Id";
            ddlStatus.DataTextField = "Name";
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("[Select One]", "0"));
        }
        #endregion

        protected void rvProcurementPlan_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}