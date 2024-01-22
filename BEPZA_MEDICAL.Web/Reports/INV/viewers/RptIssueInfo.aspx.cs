using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.INV;
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

namespace BEPZA_MEDICAL.Web.Reports.INV.viewers
{
    public partial class RptIssueInfo : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptIssueInfo()
        {
            //
        }
        #endregion

        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "LoadEmployeeAutocmplete", "LoadEmployeeAutocmplete();", true);
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }

        }

        #endregion

        #region Button Event

        string strZoneId = string.Empty;
        DateTime issueDateFrom;
        DateTime issueDateTo;

        DateTime? indentDateFrom = null;
        DateTime? indentDateTo = null;
        string issueNo = string.Empty;
        int indentId = 0;
        int issuedToId = 0;
        int itemTypeId = 0;
        int categoryId = 0;
        int colorId = 0;
        int modelId = 0;
        int itemId = 0;


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
                issueDateFrom = Convert.ToDateTime(dpIssueDateFrom.Text);
                issueDateTo = Convert.ToDateTime(dpIssueDateTo.Text);

                issueNo = tbIssueNo.Text.Trim();
                if (dpIndentDateFrom.Text != "")
                {
                    indentDateFrom = Convert.ToDateTime(dpIndentDateFrom.Text);
                }
                if (dpIndentDateTo.Text != "")
                {
                    indentDateTo = Convert.ToDateTime(dpIndentDateTo.Text);
                }
                if (ddlIndent.SelectedIndex > 0)
                {
                    indentId = Convert.ToInt32(ddlIndent.SelectedValue);
                }
                if (!String.IsNullOrEmpty(hdnEmployeeId.Value))
                {
                    var empId = hdnEmployeeId.Value;
                    issuedToId = (from p in context.PRM_EmploymentInfo
                                  where p.EmpID.Trim().ToUpper() == empId.Trim().ToUpper()
                                  select p.Id).FirstOrDefault();
                }
                else
                    issuedToId = 0;

                if (ddlItemType.SelectedIndex > 0)
                {
                    itemTypeId = Convert.ToInt32(ddlItemType.SelectedValue);
                }
                if (ddlCategory.SelectedIndex > 0)
                {
                    categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                }
                if (ddlColor.SelectedIndex > 0)
                {
                    colorId = Convert.ToInt32(ddlColor.SelectedValue);
                }
                if (ddlModel.SelectedIndex > 0)
                {
                    modelId = Convert.ToInt32(ddlModel.SelectedValue);
                }
                if (ddlItem.SelectedIndex > 0)
                {
                    itemId = Convert.ToInt32(ddlItem.SelectedValue);
                }

                GenerateReport(strZoneId, issueDateFrom, issueDateTo, indentDateFrom, indentDateTo, issuedToId, issueNo, indentId, itemTypeId, categoryId, colorId, modelId, itemId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvIssueInfo.Reset();
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
        public void GenerateReport(string strZoneId, DateTime issueDateFrom, DateTime issueDateTo, DateTime? indentDateFrom, DateTime? indentDateTo, int issuedToId, string issueNo, int indentId, int itemTypeId, int categoryId, int colorId, int modelId, int itemId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvIssueInfo.Reset();
            rvIssueInfo.ProcessingMode = ProcessingMode.Local;
            rvIssueInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/INV/rdlc/RptIssueInfo.rdlc");

            var data = invContext.sp_INV_RptIssueInfo(strZoneId, issueDateFrom, issueDateTo, indentDateFrom, indentDateTo, issuedToId, issueNo, indentId, itemTypeId, categoryId, modelId, colorId, itemId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Period From : " + issueDateFrom.ToString("dd-MMM-yyyy") + " To " + issueDateTo.ToString("dd-MMM-yyyy");
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvIssueInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsIssueInfo", data);
                rvIssueInfo.LocalReport.DataSources.Add(dataSource);
                this.rvIssueInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvIssueInfo.Reset();
            }
            rvIssueInfo.DataBind();

            //ExportToPDF
            String newFileName = "IssueInfo_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvIssueInfo, newFileName, fs);

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
            ddlIndent.DataSource = invContext.INV_RequisitionInfo.OrderBy(x => x.IndentNo).ToList();
            ddlIndent.DataValueField = "Id";
            ddlIndent.DataTextField = "IndentNo";
            ddlIndent.DataBind();
            ddlIndent.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlItemType.DataSource = invContext.INV_ItemType.OrderBy(x => x.ItemTypeName).ToList();
            ddlItemType.DataValueField = "Id";
            ddlItemType.DataTextField = "ItemTypeName";
            ddlItemType.DataBind();
            ddlItemType.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlCategory.DataSource = invContext.INV_Category.OrderBy(x => x.Name).ToList();
            ddlCategory.DataValueField = "Id";
            ddlCategory.DataTextField = "Name";
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlColor.DataSource = invContext.INV_Color.OrderBy(x => x.Name).ToList();
            ddlColor.DataValueField = "Id";
            ddlColor.DataTextField = "Name";
            ddlColor.DataBind();
            ddlColor.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlModel.DataSource = invContext.INV_Model.OrderBy(x => x.Name).ToList();
            ddlModel.DataValueField = "Id";
            ddlModel.DataTextField = "Name";
            ddlModel.DataBind();
            ddlModel.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlItem.DataSource = invContext.INV_ItemInfo.OrderBy(x => x.ItemName).ToList();
            ddlItem.DataValueField = "Id";
            ddlItem.DataTextField = "ItemName";
            ddlItem.DataBind();
            ddlItem.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }
        #endregion

        protected void rvIssueInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}