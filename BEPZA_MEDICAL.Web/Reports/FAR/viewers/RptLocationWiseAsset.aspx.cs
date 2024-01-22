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
    public partial class RptLocationWiseAsset : ReportBase
    {
        bool status = false;
        protected void Page_Load(object sender, EventArgs e)
        {            
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

                int itemTypeId = default(int);
                int CategoryId = default(int);
                int SubCategoryId = default(int);
                int locationId = default(int);
                //int AssetConditionId = default(int);
                DateTime startDate, endDate;
              
              
                itemTypeId = Convert.ToInt32(ddlItemType.SelectedValue);
                CategoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                SubCategoryId = Convert.ToInt32(ddlSubCategory.SelectedValue);
                locationId = Convert.ToInt32(ddlLocation.SelectedValue);
                //AssetConditionId = Convert.ToInt32(ddlAssetCondition.SelectedValue);
                string BeneficiaryEmpId = ddlEmployeeList.SelectedValue.ToString();
                startDate = txtStartDate.Text.Trim() != string.Empty ? Convert.ToDateTime(txtStartDate.Text.Trim()) : Convert.ToDateTime("1900-01-01");
                endDate = txtEndDate.Text.Trim() != string.Empty ? Convert.ToDateTime(txtEndDate.Text.Trim()) : Convert.ToDateTime(DateTime.MaxValue);
                GenerateReport(strZoneId,strZoneNames,itemTypeId, CategoryId, SubCategoryId, locationId, startDate, endDate,BeneficiaryEmpId);
              
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

        private void GenerateReport(string zoneList,string strZoneNames,int itemTypeId, int CategoryId, int SubCategoryId, int locationId, DateTime startDate, DateTime endDate, String BeneficiaryEmpId)
        {
        
            lblMsg.Text = string.Empty;

            rvLocationWiseAsset.Reset();
            rvLocationWiseAsset.ProcessingMode = ProcessingMode.Local;

            if (rbl.SelectedValue == "1")
            {
                rvLocationWiseAsset.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptLocationWiseAsset.rdlc");
            }
            else
            {
                rvLocationWiseAsset.LocalReport.ReportPath = Server.MapPath("~/Reports/FAR/rdlc/RptAssetList.rdlc");
            }

            var data = farContext.SP_FAR_LocationWiseAssetListRpt(zoneList, itemTypeId, CategoryId, SubCategoryId, locationId, startDate, endDate, BeneficiaryEmpId).ToList();

            if (data.Count() > 0)
            {   
                data = data.OrderBy(o => o.AssetCode).ToList();
                rvLocationWiseAsset.LocalReport.DataSources.Add(new ReportDataSource("dsLocationWiseAsset", data));

                string searchParameters = startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy");
                ReportParameter p1 = new ReportParameter("paramZoneName", strZoneNames);
                ReportParameter p2 = new ReportParameter("pramReportPeriod", searchParameters);
               
                rvLocationWiseAsset.LocalReport.SetParameters(new ReportParameter[] { p1,p2 });
            }
            else
            {
                status = true;
                rvLocationWiseAsset.Reset();
            }
            rvLocationWiseAsset.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvLocationWiseAsset.DataBind();

            //ExportToPDF
            String newFileName = "LocationWiseAsset_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvLocationWiseAsset, newFileName, fs);
        }
       

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        protected void rvLocationWiseAsset_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region DDL Population-------------

        private void PopulateLocationDDL()
        {

            ddlEmployeeList.DataSource = context.PRM_EmploymentInfo.Select(q => new { ZoneInfoId = q.ZoneInfoId, EmpID = q.EmpID, DisplayText = q.FullName + " [" + q.EmpID + "]" }).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.DisplayText).ToList();
            ddlEmployeeList.DataValueField = "EmpID";
            ddlEmployeeList.DataTextField = "DisplayText";
            ddlEmployeeList.DataBind();
            ddlEmployeeList.Items.Insert(0, new ListItem("All", ""));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlLocation.DataSource = farContext.FAR_Location.ToList();
            ddlLocation.DataTextField = "Name";
            ddlLocation.DataValueField = "Id";
            ddlLocation.DataBind();
            ddlLocation.Items.Insert(0, new ListItem("All", "0"));

            ddlCategory.DataSource = farContext.FAR_Catagory.ToList();
            ddlCategory.DataTextField = "CategoryName";
            ddlCategory.DataValueField = "Id";
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem("All", "0"));

            ddlSubCategory.Items.Insert(0, new ListItem("All", "0"));

            // Asset Condition
            //ddlAssetCondition.DataSource = farContext.FAR_AssetCondition.ToList();
            //ddlAssetCondition.DataTextField = "Name";
            //ddlAssetCondition.DataValueField = "Id";
            //ddlAssetCondition.DataBind();
            //ddlAssetCondition.Items.Insert(0, new ListItem("All", "0"));

            var list = invContext.INV_ItemType.Where(q => q.ParentId != null).ToList();
            ddlItemType.DataSource = list;
            ddlItemType.DataTextField = "ItemTypeName";
            ddlItemType.DataValueField = "Id";
            ddlItemType.DataBind();
            ddlItemType.Items.Insert(0, new ListItem("All", "0"));
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