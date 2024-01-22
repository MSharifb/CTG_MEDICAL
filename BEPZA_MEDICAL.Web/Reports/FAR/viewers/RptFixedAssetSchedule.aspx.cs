using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.FAR.viewers
{
    public partial class RptFixedAssetSchedule : ReportBase
    {
        bool status = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateDDL();
            }
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> zoneList = new List<int>();
                List<string> zoneNames = new List<string>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                        zoneNames.Add(item.Text.ToString());
                    }
                }

                string strZoneId = string.Join(",", zoneList.ToArray());
                string strZoneNames = string.Join(", ", zoneNames.ToArray());
                string Refurbishment = rdbRefurbishment.SelectedValue.ToString();
                int reportType = Convert.ToInt32(rdRptType.SelectedValue);
                int itemTypeid = Convert.ToInt32(ddlItemType.SelectedValue);
                int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                int financialYearId = Convert.ToInt32(ddlFinancialYear.SelectedValue);
                
                DateTime startDate=DateTime.Now;
                DateTime endDate = DateTime.Now;
                //startDate = txtStartDate.Text.Trim() != string.Empty ? Convert.ToDateTime(txtStartDate.Text.Trim()) : Convert.ToDateTime("01-01-1900");
                //endDate = txtEndDate.Text.Trim() != string.Empty ? Convert.ToDateTime(txtEndDate.Text.Trim()) : Convert.ToDateTime(DateTime.MaxValue);

                var objAcc = farContext.acc_Accounting_Period_Information.ToList().Where(q => q.id == financialYearId).FirstOrDefault();
                if (objAcc != null)
                {
                    startDate = objAcc.periodStartDate;
                    endDate = objAcc.periodEndDate;
                }

                GenerateReport(strZoneId, strZoneNames, Refurbishment, itemTypeid, categoryId, reportType, startDate, endDate);
                lblMsg.Text = string.Empty;
                if (status == true)
                {
                    lblMsg.Text = BEPZA_MEDICAL.Web.Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
                status = false;
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message.ToString();
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void GenerateReport(string zoneList, string strZoneNames, string rdbRefurbishment, int itemTypeid, int categoryId, int reportType, DateTime startDate, DateTime endDate)
        {

            lblMsg.Text = string.Empty;
            rvFixedAssetSchedule.Reset();
            rvFixedAssetSchedule.ProcessingMode = ProcessingMode.Local;

            var data = farContext.SP_FAR_FixedAssetScheduleRpt(zoneList, itemTypeid, categoryId, startDate, endDate, rdbRefurbishment).ToList();
            if (reportType == 0)
            {
                rvFixedAssetSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptItemTypeWiseFixedAssetSchedule.rdlc");

            }
            else if (reportType == 1)
            {
                rvFixedAssetSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptGroupWiseFixedAssetSchedule.rdlc");
            }
            else if (reportType == 2)
            {
                rvFixedAssetSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptZoneWiseFixedAssetSchedule.rdlc");
            }
            else
            {
                rvFixedAssetSchedule.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptFixedAssetSchedule.rdlc");
            }

            string refurbishmentName = "All Assets";
            if (rdbRefurbishment == "")
            {
                refurbishmentName = "Assets without Refurbishment";
            }
            else if (rdbRefurbishment == "R")
            {
                refurbishmentName = "Only Refurbishment";
            }

            if (data.Count() > 0)
            {
                rvFixedAssetSchedule.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                string period = startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy");
                ReportParameter p1 = new ReportParameter("paramZoneName", strZoneNames);
                ReportParameter p2 = new ReportParameter("pramReportPeriod", period);
                ReportParameter p3 = new ReportParameter("fromDate", startDate.ToString("dd-MMM-yyyy"));
                ReportParameter p4 = new ReportParameter("toDate", endDate.ToString("dd-MMM-yyyy"));
                ReportParameter p5 = new ReportParameter("paramRefurbishment", refurbishmentName);
                rvFixedAssetSchedule.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5 });
            }
            else
            {
                status = true;
                rvFixedAssetSchedule.Reset();
            }
            rvFixedAssetSchedule.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFixedAssetSchedule.DataBind();

            //ExportToPDF
            String newFileName = "FixedAssetSchedule_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFixedAssetSchedule, newFileName, fs);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        protected void rvFixedAssetSchedule_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region DDL Population-------------

        private void PopulateDDL()
        {
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlCategory.DataSource = farContext.FAR_Catagory.ToList();
            ddlCategory.DataTextField = "CategoryName";
            ddlCategory.DataValueField = "Id";
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem("All", "0"));

            var list = invContext.INV_ItemType.Where(q => q.ParentId != null).ToList();
            ddlItemType.DataSource = list;
            ddlItemType.DataTextField = "ItemTypeName";
            ddlItemType.DataValueField = "Id";
            ddlItemType.DataBind();
            ddlItemType.Items.Insert(0, new ListItem("All", "0"));


            ddlFinancialYear.DataSource = farContext.acc_Accounting_Period_Information.ToList();
            ddlFinancialYear.DataTextField = "yearName";
            ddlFinancialYear.DataValueField = "Id";
            ddlFinancialYear.DataBind();
            ddlFinancialYear.Items.Insert(0, new ListItem("Select One", "0"));
        }

        #endregion
    }
}