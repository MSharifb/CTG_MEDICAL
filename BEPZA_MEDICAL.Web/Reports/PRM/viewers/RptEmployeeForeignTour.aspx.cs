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
    public partial class RptEmployeeForeignTour : ReportBase
    {
        #region Fields
        #endregion

        #region Ctor
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
                int employeeId =Convert.ToInt32(ddlEmployee.SelectedValue);
                string trainingTitle = ddlTrainingTitle.SelectedValue.ToString();
                string organizedBy = ddlOrganizedBy.SelectedValue.ToString();
                string fundedby = ddlFundedBy.SelectedValue.ToString();
                int countryId = Convert.ToInt32(ddlCountry.SelectedValue);
                int departmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
                DateTime? fromDate = null;
                DateTime? toDate = null;

                if (txtFromDate.Text != string.Empty)
                {
                    fromDate = Convert.ToDateTime(txtFromDate.Text);
                }
                if (txtToDate.Text != string.Empty)
                {
                    toDate = Convert.ToDateTime(txtToDate.Text);
                }

                GenerateReport(employeeId, strZoneId, trainingTitle, organizedBy, fundedby, countryId, departmentId, fromDate, toDate);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int employeeId, string strZoneId, string trainingTitle, string organizedBy, string fundedby, int countryId, int departmentId, DateTime? fromdate, DateTime? toDate)
        {
            #region Processing Report Data

            rvTestResult.Reset();
            rvTestResult.ProcessingMode = ProcessingMode.Local;
            rvTestResult.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/EmployeeForeignTourInfo.rdlc");

            var data = (from s in base.prmContext.SP_PRM_RptEmpyoleeForeignTour(employeeId, strZoneId, trainingTitle, organizedBy, fundedby, countryId, departmentId, fromdate, toDate) select s).ToList();

            #region Search parameter


            //string searchParameters1 = string.Empty;
            //string searchParameters2 = string.Empty;
            //searchParameters1 = (Convert.ToDateTime(fromDate)).ToString("dd-MMM-yy");
            //searchParameters2 = (Convert.ToDateTime(toDate)).ToString("dd-MMM-yy");

            //ReportParameter p1 = new ReportParameter("FromDate", searchParameters1);
            //ReportParameter p2 = new ReportParameter("ToDate", searchParameters2);

            //rvTestResult.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DS1", data);
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

            HashSet<int> zoneIDs = new HashSet<int>(MyAppSession.SelectedZoneList.Select(s => s.Id));

            ddlEmployee.DataSource = _pgmExecuteFunctions.GetEmployeeList().Select(q => new
            {
                ZoneInfoId = q.SalaryWithdrawFromZoneId,
                EmpID = q.EmpID,
                Id = q.Id,
                DisplayText = q.FullName + " [" + q.EmpID + "]"
            }).ToList()
                .Where(x => zoneIDs.Contains(BEPZA_MEDICAL.Web.Utility.Common.GetInteger(x.ZoneInfoId)))
                .OrderBy(x => x.DisplayText);
            ddlEmployee.DataValueField = "Id";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", "0"));

            ddlTrainingTitle.DataSource = context.PRM_EmpForeignTourInfo.DistinctBy(s => s.TitleOfTheTour).OrderBy(x => x.TitleOfTheTour).ToList();
            ddlTrainingTitle.DataValueField = "TitleOfTheTour";
            ddlTrainingTitle.DataTextField = "TitleOfTheTour";
            ddlTrainingTitle.DataBind();
            ddlTrainingTitle.Items.Insert(0, new ListItem("All", "0"));

            ddlOrganizedBy.DataSource = context.PRM_EmpForeignTourInfo.OrderBy(x => x.OrganizedBy).ToList();
            ddlOrganizedBy.DataValueField = "OrganizedBy";
            ddlOrganizedBy.DataTextField = "OrganizedBy";
            ddlOrganizedBy.DataBind();
            ddlOrganizedBy.Items.Insert(0, new ListItem("All", "0"));

            ddlFundedBy.DataSource = context.PRM_EmpForeignTourInfo.OrderBy(x => x.Financed).ToList();
            ddlFundedBy.DataValueField = "Financed";
            ddlFundedBy.DataTextField = "Financed";
            ddlFundedBy.DataBind();
            ddlFundedBy.Items.Insert(0, new ListItem("All", "0"));

            ddlCountry.DataSource = context.PRM_Country.OrderBy(x => x.Name).ToList();
            ddlCountry.DataValueField = "Id";
            ddlCountry.DataTextField = "Name";
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem("All", "0"));

            ddlDepartment.DataSource = context.PRM_Division.Where(s => s.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            ddlDepartment.DataValueField = "Id";
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("All", "0"));
        }
        #endregion
    }
}