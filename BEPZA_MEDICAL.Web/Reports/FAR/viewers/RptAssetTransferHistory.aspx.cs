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
    public partial class RptAssetTransferHistory :ReportBase
    {
        bool status = false;
        DateTime startDate, endDate;
        int toLocationId = default(int);
        int fixedAssetId = default(int);
        int toBeneficiaryEmloyee = default(int);
        int subCategoryId = default(int);

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
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                string strZoneId = ConvertZoneArrayListToString(zoneList.ToArray());
                string EmpId = ddlEmployeeList.SelectedValue.ToString();
                toLocationId = Convert.ToInt32(ddlToLocation.SelectedValue.ToString());
                var assetList = farContext.FAR_FixedAsset.Where(q => q.AssetCode == txtAssetCode.Text).ToList();
                if (assetList.Count > 0)
                {
                    fixedAssetId = Convert.ToInt32(assetList.Where(q => q.AssetCode == txtAssetCode.Text).SingleOrDefault().Id);
                }
                var empList = context.PRM_EmploymentInfo.Where(q => q.EmpID == EmpId).ToList();

                if (empList.Count > 0)
                {
                    toBeneficiaryEmloyee = Convert.ToInt32(empList.Where(q => q.EmpID == EmpId).SingleOrDefault().Id);
                }

                subCategoryId = Convert.ToInt32(ddlSubCategory.SelectedValue.ToString());

                startDate = txtStartDate.Text.Trim() != string.Empty ? Convert.ToDateTime(txtStartDate.Text.Trim()) : Convert.ToDateTime("1900-01-01");
                endDate = txtEndDate.Text.Trim() != string.Empty ? Convert.ToDateTime(txtEndDate.Text.Trim()) : Convert.ToDateTime(DateTime.MaxValue);

                GenerateReport(strZoneId,fixedAssetId, subCategoryId, toLocationId, toBeneficiaryEmloyee, startDate, endDate);

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

        private void GenerateReport(string strZoneId,int fixedAssetId, int subCategoryId, int toLocationId, int toBeneficiaryEmployeeId, DateTime startDate, DateTime endDate)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            lblMsg.Text = string.Empty;

            rvAssetTransferHistory.Reset();
            rvAssetTransferHistory.ProcessingMode = ProcessingMode.Local;
            rvAssetTransferHistory.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptAssetTransferHistory.rdlc");

            var data = farContext.SP_FAR_AssetTransferHistoryRpt(strZoneId, fixedAssetId, subCategoryId, toLocationId, toBeneficiaryEmployeeId, startDate, endDate).ToList();

            if (data.Count() > 0)
            {
                data = data.OrderBy(o => o.AssetCode).ToList();
                rvAssetTransferHistory.LocalReport.DataSources.Add(new ReportDataSource("dsAssetTransferHistory", data));

                string searchParameters = "";

                if (txtStartDate.Text.Trim() != string.Empty && txtEndDate.Text.Trim() != string.Empty)
                {
                    searchParameters = "For the Period from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy");
                }

                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvAssetTransferHistory.LocalReport.SetParameters(new ReportParameter[] { p1 });
            }
            else
            {
                status = true;
                rvAssetTransferHistory.Reset();
            }
            rvAssetTransferHistory.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvAssetTransferHistory.DataBind();

            //ExportToPDF
            String newFileName = "AssetTransferHistory_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvAssetTransferHistory, newFileName, fs);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        protected void rvAssetTransferHistory_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region DDL Population-------------

        private void PopulateLocationDDL()
        {
            ddlEmployeeList.DataSource = prmContext.PRM_EmploymentInfo.Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            ddlEmployeeList.DataTextField = "FullName";
            ddlEmployeeList.DataValueField = "EmpID";
            ddlEmployeeList.DataBind();
            ddlEmployeeList.Items.Insert(0, new ListItem("All", ""));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;  

            ddlToLocation.DataSource = farContext.FAR_Location.ToList();
            ddlToLocation.DataTextField = "Name";
            ddlToLocation.DataValueField = "Id";
            ddlToLocation.DataBind();
            ddlToLocation.Items.Insert(0, new ListItem("All", "0"));

            ddlSubCategory.DataSource = farContext.FAR_SubCategory.ToList();
            ddlSubCategory.DataTextField = "SubCategoryName";
            ddlSubCategory.DataValueField = "Id";
            ddlSubCategory.DataBind();
            ddlSubCategory.Items.Insert(0, new ListItem("All", "0"));
        }

        #endregion
    }
}