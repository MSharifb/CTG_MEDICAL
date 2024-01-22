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

namespace BEPZA_MEDICAL.Web.Reports.CPF.viewers
{
    public partial class CPFYearlyStatement : ReportBase
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
            try
            {
                int financialYearId = Convert.ToInt32(ddlFinancialYear.SelectedValue);
                int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue);
                var yearName = ddlFinancialYear.SelectedItem.Text;
                GenerateReport(employeeId, financialYearId, yearName);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(int employeeId, int financialYearId, string yearName)
        {
            #region Processing Report Data

            rvInfo.Reset();
            rvInfo.ProcessingMode = ProcessingMode.Local;

            dynamic data = ""; 
            if (employeeId == 0)
            {
                data = (from s in base.cpfContext.CPF_SP_RptYearlyPFStatement(financialYearId) select s).ToList();
                rvInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/CPF/rdlc/YearlyPFStatementAll.rdlc");
            }
            else
            {
                data = (from s in base.cpfContext.CPF_SP_RptIndividualYearlyPFStatement(employeeId, financialYearId) select s).ToList();
                rvInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/CPF/rdlc/YearlyPFStatementIndividual.rdlc");
            }
            #region Search parameter
            string searchParameters = "For the period of : " + yearName;
            ReportParameter p1 = new ReportParameter("param", searchParameters);
            rvInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });
            #endregion
            var reportHeader = base.GetZoneInfoForReportHeader();

            ReportDataSource dataSource = new ReportDataSource("DataSet1", data);
            rvInfo.LocalReport.DataSources.Add(dataSource);
            rvInfo.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));

            this.rvInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvInfo.DataBind();

            //ExportToPDF
            String newFileName = "YearlyPFStatement_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvInfo, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            switch (e.ReportPath)
            {
                case "_ReportHeader":
                    data = null;
                    dsName = "DSCompanyInfo";
                    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;

                default:
                    break;
            }
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        #endregion

        #region User Methods

        private void PopulateDropdownList()
        {
            ddlEmployee.DataSource = context
            .PRM_EmploymentInfo
            .Select(q => new
            {
                ZoneInfoId = q.ZoneInfoId,
                EmpID = q.Id,
                DisplayText = q.FullName + " [" + q.EmpID + "]"
            }).ToList()
            .OrderBy(x => x.DisplayText);
            ddlEmployee.DataValueField = "EmpID";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", "0"));


            ddlFinancialYear.DataSource = pmiContext.acc_Accounting_Period_Information.ToList();
            ddlFinancialYear.DataValueField = "id";
            ddlFinancialYear.DataTextField = "yearName";
            ddlFinancialYear.DataBind();
            ddlFinancialYear.Items.Insert(0, new ListItem("All", "0"));
        }

        #endregion

        protected void rvInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}