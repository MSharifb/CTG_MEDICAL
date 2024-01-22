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
    public partial class RptFixedAssetRegistration : ReportBase
    {
        bool status = false;
        protected void Page_Load(object sender, EventArgs e)
        {
           // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "LoadADCAutocmplete", "LoadADCAutocmplete();", true);

            if (!IsPostBack)
            {
                PopulateLocationDDL();
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

              
                int assetId = Convert.ToInt32(ddlAssetList.SelectedValue);
                int CategoryId = Convert.ToInt32(ddlCategory.SelectedValue.ToString());
                int SubCategoryId = Convert.ToInt32(ddlSubCategory.SelectedValue.ToString());               
               
                //if (txtAsset.Text.Trim() != string.Empty)
                //{
                //    assetId = farContext.FAR_FixedAsset.Where(q => q.AssetCode == txtAsset.Text).FirstOrDefault().Id;
                //}
                DateTime startDate, endDate;
                if (txtStartDate.Text.Trim() != string.Empty && txtEndDate.Text.Trim() != string.Empty)
                {
                    startDate = Convert.ToDateTime(txtStartDate.Text.Trim());
                    endDate = Convert.ToDateTime(txtEndDate.Text.Trim());

                    GenerateReport(strZoneId,strZoneNames, assetId, CategoryId, SubCategoryId, startDate, endDate);

                    lblMsg.Text = string.Empty;
                    if (status == true)
                    {
                        lblMsg.Text = BEPZA_MEDICAL.Web.Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                        lblMsg.ForeColor = System.Drawing.Color.Red;
                    }
                    status = false;
                }
                else
                {
                    lblMsg.Text = string.Empty;
                    lblMsg.Text = BEPZA_MEDICAL.Web.Utility.Common.GetCommomMessage(CommonMessage.MandatoryInputFailed);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvFixedAssetRegistration.Reset();
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void GenerateReport(string zoneList,string strZoneNames, int assetId, int CategoryId, int SubCategoryId, DateTime startDate, DateTime endDate)
        {
            
            lblMsg.Text = string.Empty;

            rvFixedAssetRegistration.Reset();
            rvFixedAssetRegistration.ProcessingMode = ProcessingMode.Local;
            rvFixedAssetRegistration.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptFixedAssetRegistration.rdlc");

            var data = farContext.SP_FAR_FixedAssetRegisterRpt(zoneList,assetId, CategoryId, SubCategoryId, startDate, endDate).ToList();
            if (data.Count() > 0)
            {
                rvFixedAssetRegistration.LocalReport.DataSources.Add(new ReportDataSource("dsFixedAssetRegistration", data));

                string period = startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy");
                ReportParameter p1 = new ReportParameter("paramZoneName", strZoneNames);
                ReportParameter p2 = new ReportParameter("pramReportPeriod", period);
                ReportParameter p3 = new ReportParameter("fromDate", startDate.ToString("dd-MMM-yyyy"));
                ReportParameter p4 = new ReportParameter("toDate", endDate.ToString("dd-MMM-yyyy"));
                rvFixedAssetRegistration.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4 });
                
            }
            else
            {
                status = true;
                rvFixedAssetRegistration.Reset();
            }
            rvFixedAssetRegistration.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFixedAssetRegistration.DataBind();

            //ExportToPDF
            String newFileName = "FixedAssetRegistration_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFixedAssetRegistration, newFileName, fs);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        protected void rvFixedAssetRegistration_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region DDL Population-------------

        private void PopulateLocationDDL()
        {
            ddlAssetList.DataSource = farContext.FAR_FixedAsset.Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.AssetCode).Select(q => new { Id = q.Id, Text = q.AssetName + " [" + q.AssetCode + "]" }).ToList();
            ddlAssetList.DataTextField = "Text";
            ddlAssetList.DataValueField = "Id";
            ddlAssetList.DataBind();
            ddlAssetList.Items.Insert(0, new ListItem("All", "0"));


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

            ddlSubCategory.Items.Insert(0, new ListItem("All", "0"));
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int CategoryId = Convert.ToInt32(ddlCategory.SelectedValue.ToString());

            ddlSubCategory.Items.Clear();

            ddlSubCategory.DataSource = farContext.FAR_SubCategory.Where(t => t.CategoryId == CategoryId).ToList();
            ddlSubCategory.DataTextField = "SubCategoryName";
            ddlSubCategory.DataValueField = "Id";
            ddlSubCategory.DataBind();

            ddlSubCategory.Items.Insert(0, new ListItem("All", "0"));
        }

        #endregion
    }
}