using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.AMS;
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

namespace BEPZA_MEDICAL.Web.Reports.AMS.viewers
{
    public partial class RptCategoryWiseAnsarList : ReportBase
    {
        #region Fields

        bool checkStatus;
        string categoryName = string.Empty;

        #endregion

        #region Ctor
        public RptCategoryWiseAnsarList()
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

        string strZoneId = string.Empty;
        int statusId = 0;
        int categoryId = 0;
        
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;

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

                statusId = Convert.ToInt32(ddlStatus.SelectedValue);
                
                categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                categoryName = ddlCategory.SelectedItem.Text;
                
                GenerateReport(strZoneId, statusId, categoryId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvCategoryWiseAnsarList.Reset();
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
        public void GenerateReport(string strZoneId, int statusId, int categoryId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvCategoryWiseAnsarList.Reset();
            rvCategoryWiseAnsarList.ProcessingMode = ProcessingMode.Local;
            rvCategoryWiseAnsarList.LocalReport.ReportPath = Server.MapPath("~/Reports/AMS/rdlc/RptCategoryWiseAnsarList.rdlc");

            var data = amsContext.sp_AMS_RptCategoryWiseAnsarList(strZoneId, categoryId, statusId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = string.Concat("Ansar Category : " , categoryName);
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvCategoryWiseAnsarList.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsCategoryWiseAnsarList", data);
                rvCategoryWiseAnsarList.LocalReport.DataSources.Add(dataSource);
                this.rvCategoryWiseAnsarList.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvCategoryWiseAnsarList.Reset();
            }
            rvCategoryWiseAnsarList.DataBind();

            //ExportToPDF
            String newFileName = "CategoryWiseAnsarList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvCategoryWiseAnsarList, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
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
        #endregion

        #region User Methods
        private void PopulateDropdownList()
        {
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlStatus.DataSource = amsContext.AMS_EmpStatus.OrderBy(x => x.SortOrder).ToList();
            ddlStatus.DataValueField = "Id";
            ddlStatus.DataTextField = "Name";
            ddlStatus.DataBind();
            ddlStatus.Items.FindByText("Active").Selected = true;
            //ddlStatus.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlCategory.DataSource = amsContext.AMS_Category.OrderBy(x => x.SortOrder).ToList();
            ddlCategory.DataValueField = "Id";
            ddlCategory.DataTextField = "Name";
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem("[Select One]", "0"));
        }
        #endregion

        protected void rvCategoryWiseAnsarList_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}