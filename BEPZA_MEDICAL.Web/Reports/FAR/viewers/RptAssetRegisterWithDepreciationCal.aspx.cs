using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.FAR.viewers
{
    public partial class RptAssetRegisterWithDepreciationCal : ReportBase
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
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                string strZoneId = string.Join(",", zoneList.ToArray());

                int finYearId = Convert.ToInt32(ddlYear.SelectedValue);             
                var option = Convert.ToInt32(rdRptType.SelectedValue);
                int assetCategoryId = Convert.ToInt32(ddlAssetCategory.SelectedValue);

                GenerateReport(strZoneId, finYearId, option, assetCategoryId);

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
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void GenerateReport(string strZoneId, int finYearId, int? option, int assetCategory)
        {
            lblMsg.Text = string.Empty;

            rvAssetRegisterWithDepreciationCal.Reset();
            rvAssetRegisterWithDepreciationCal.ProcessingMode = ProcessingMode.Local;

            if (option == 1) //Summary
            {
                rvAssetRegisterWithDepreciationCal.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptAssetRegisterWithDepreciationCal.rdlc");
            }
            else
            {
                rvAssetRegisterWithDepreciationCal.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptAssetRegisterWithDepreciationCalDetail.rdlc");
            }

            var data = farContext.SP_FAR_AssetRegisterWithDepreciationCalRpt(strZoneId, finYearId, option, assetCategory).ToList();

            if (data.Count() > 0)
            {
                data = data.OrderBy(o => o.AssetCode).ToList();

                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;

                var objAcc = farContext.acc_Accounting_Period_Information.ToList().Where(q => q.id == finYearId).FirstOrDefault();
                if (objAcc != null)
                {
                    startDate = objAcc.periodStartDate;
                    endDate = objAcc.periodEndDate;
                }

                rvAssetRegisterWithDepreciationCal.LocalReport.DataSources.Add(new ReportDataSource("dsAssetRegisterWithDepreciationCal", data));

                string searchParameters = "As on " + endDate.ToString("yyyy-MM-dd");
                string preMonthYr = startDate.ToString("yyyy-MM-dd");
                string FY = ddlYear.SelectedItem.ToString();
                string curMonthYr = endDate.ToString("yyyy-MM-dd");
                string assetType = ddlAssetCategory.SelectedValue.ToString() == "0" ? "All" : ddlAssetCategory.SelectedItem.Text;

                ReportParameter p1 = new ReportParameter("param", searchParameters);
                ReportParameter p2 = new ReportParameter("preMonthYr", preMonthYr);
                ReportParameter p3 = new ReportParameter("FY", FY);
                ReportParameter p4 = new ReportParameter("curMonthYr", curMonthYr);
                ReportParameter p5 = new ReportParameter("assetType", assetType);

                rvAssetRegisterWithDepreciationCal.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5 });
            }
            else
            {
                status = true;
                rvAssetRegisterWithDepreciationCal.Reset();
            }
            rvAssetRegisterWithDepreciationCal.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvAssetRegisterWithDepreciationCal.DataBind();

            //ExportToPDF
            String newFileName = "AssetRegisterWithDepriciationCal_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvAssetRegisterWithDepreciationCal, newFileName, fs);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        protected void rvAssetRegisterWithDepreciationCal_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region Method-------------

        private void PopulateDDL()
        {
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlYear.DataSource = farContext.acc_Accounting_Period_Information.OrderByDescending(q => q.periodEndDate).ToList();
            ddlYear.DataTextField = "yearName";
            ddlYear.DataValueField = "id";
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, new ListItem("Select One", "0"));

            ddlAssetCategory.DataSource = farContext.FAR_Catagory.ToList();
            ddlAssetCategory.DataTextField = "CategoryName";
            ddlAssetCategory.DataValueField = "Id";
            ddlAssetCategory.DataBind();
            ddlAssetCategory.Items.Insert(0, new ListItem("All", "0"));
        }
        #endregion
    }
}