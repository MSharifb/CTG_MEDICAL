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
    public partial class UserLoginHistory : ReportBase
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

                DateTime? formDate = null;

                DateTime? toDate = null;

                if(txtFromDate.Text!=string.Empty)
                {
                    formDate = Convert.ToDateTime(txtFromDate.Text);
                }

                if (txtToDate.Text != string.Empty)
                {
                    toDate = Convert.ToDateTime(txtToDate.Text);
                }

                var employeeId = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());


                GenerateReport(strZoneId, employeeId, formDate, toDate);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string zoneList, int employeeId, DateTime? fromDate, DateTime? toDate)
        {
            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/UserLoginHistory.rdlc");

            #region Processing Report Data

            var data = (from s in base.prmContext.SP_PRM_UserLoginHistory(zoneList, employeeId, fromDate, toDate) select s).ToList();

            #endregion
            rvEmployeeInfo.LocalReport.DataSources.Add(new ReportDataSource("dsUser", data));

            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "EmployeeList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);
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
            
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;


            HashSet<int> zoneIDs = new HashSet<int>(MyAppSession.SelectedZoneList.Select(s => s.Id));

            ddlEmployee.DataSource = context.PRM_EmploymentInfo.Select(q => new
            {
                ZoneInfoId = (int) q.SalaryWithdrawFromZoneId,
                EmpID = q.EmpID,
                Id = q.Id,
                DisplayText = q.FullName + " [" + q.EmpID + " ]"
            })
              .Where(x => zoneIDs.Contains(x.ZoneInfoId))
              .OrderBy(x => x.DisplayText)
              .ToList();
            ddlEmployee.DataValueField = "Id";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", "0"));
        }

        #endregion

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}