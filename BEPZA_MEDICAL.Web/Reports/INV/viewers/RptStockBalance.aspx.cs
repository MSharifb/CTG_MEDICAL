using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.PMI;
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
using BEPZA_MEDICAL.Domain.INV;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.INV;
using System.IO;

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptStockBalance : ReportBase
    {
       #region Fields
        private readonly INVCommonService _invCommonService;
        bool checkStatus;

        #endregion

        #region Ctor
        public RptStockBalance()
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

        DateTime fromDt;
        DateTime toDt;
        string strZoneId = string.Empty;
        int itemId = 0;
        int categoryId = 0;
        int typeId = 0;
        int colorId = 0;
        int modelId = 0;
        int classId = 0;
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
                fromDt = Convert.ToDateTime(fromDate.Text);
                toDt = Convert.ToDateTime(toDate.Text);

                if (ddlItem.SelectedIndex > 0)
                {
                    itemId = Convert.ToInt32(ddlItem.SelectedValue);
                }
                if (ddlItemCategory.SelectedIndex > 0)
                {
                    categoryId = Convert.ToInt32(ddlItemCategory.SelectedValue);
                }
                if (ddlItemType.SelectedIndex > 0)
                {
                    typeId = Convert.ToInt32(ddlItemType.SelectedValue);
                }
                if (ddlItemColor.SelectedIndex > 0)
                {
                    colorId = Convert.ToInt32(ddlItemColor.SelectedValue);
                }
                if (ddlItemModel.SelectedIndex > 0)
                {
                    modelId = Convert.ToInt32(ddlItemModel.SelectedValue);
                }
                if (ddlItemClass.SelectedIndex > 0)
                {
                    classId = Convert.ToInt32(ddlItemClass.SelectedValue);
                }
                GenerateReport(fromDt, toDt, itemId, categoryId, typeId, colorId, modelId, strZoneId, classId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvStockBalance.Reset();
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
        public void GenerateReport(DateTime fromDate, DateTime toDate, int itemId, int categoryId, int typeId, int colorId, int modelId, string strZoneId, int classId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvStockBalance.Reset();
            rvStockBalance.ProcessingMode = ProcessingMode.Local;
            rvStockBalance.LocalReport.ReportPath = Server.MapPath("~/Reports/INV/rdlc/RptStockBalance.rdlc");

            var data = invContext.sp_INV_RptStockBalance(strZoneId, fromDate, toDate, typeId, categoryId, modelId, colorId, itemId, numErrorCode, strErrorMsg).ToList();

            if(chkZeroItem.Checked)
            {
                var item = invContext.INV_ItemInfo.ToList();
                foreach (var d in data)
                {
                    item.RemoveAll(p => p.Id == d.ItemId);
                }

                IList<sp_INV_RptStockBalance_Result> newList = new List<sp_INV_RptStockBalance_Result>();
                foreach (var i in item)
                {
                    sp_INV_RptStockBalance_Result stockItem = new sp_INV_RptStockBalance_Result();
                    stockItem.ItemId = i.Id;
                    stockItem.ItemName = i.ItemName;
                    stockItem.TransactionType = "Purchase";
                    stockItem.Quantity = 0;
                    stockItem.UnitName = i.INV_Unit.Name;
                    stockItem.OpeningBalance = 0;
                    stockItem.ClosingBalance = 0;
                    stockItem.SortOrder = 0;
                    stockItem.IsPeriodicAsset = Convert.ToBoolean(i.IsPeriodicAsset);
                    stockItem.IsCondemnableAsset = Convert.ToBoolean(i.IsCondemnableAsset);
                    stockItem.HasQuota = Convert.ToBoolean(i.HasQuota);
                    stockItem.AssetGroupId = i.AssetGroupId;

                    newList.Add(stockItem);
                }
               
                data.AddRange(newList);
            }
           if(chkPeriodic.Checked)
           {
             data = data.Where(d => d.IsPeriodicAsset == true).ToList();
           }
           if (chkCondemnable.Checked)
           {
               data = data.Where(d => d.IsCondemnableAsset == true).ToList();
           }
           if (chkHasQuota.Checked)
           {
               data = data.Where(d => d.HasQuota == true).ToList();
           }
           if (classId > 0)
           {
               data = data.Where(d => d.AssetGroupId == classId).ToList();
           }
            //.OrderBy(d => new {d.ItemName,d.SortOrder })
            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "For the period from: "+fromDate.ToShortDateString() + " To " + toDate.ToShortDateString();
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvStockBalance.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsStockBalance", data);
                rvStockBalance.LocalReport.DataSources.Add(dataSource);
                this.rvStockBalance.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvStockBalance.Reset();
            }
            rvStockBalance.DataBind();

            //ExportToPDF
            String newFileName = "StockBalance_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvStockBalance, newFileName, fs);

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
            //var transferInInfo = invContext.INV_TransferInInfo.ToList();

            ddlItem.DataSource = invContext.INV_ItemInfo.OrderByDescending(x => x.ItemName).ToList();
            ddlItem.DataValueField = "Id";
            ddlItem.DataTextField = "ItemName";
            ddlItem.DataBind();
            ddlItem.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlItemCategory.DataSource = invContext.INV_Category.OrderBy(x => x.Name).ToList();
            ddlItemCategory.DataValueField = "Id";
            ddlItemCategory.DataTextField = "Name";
            ddlItemCategory.DataBind();
            ddlItemCategory.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlItemType.DataSource = invContext.INV_ItemType.OrderBy(x => x.ItemTypeName).ToList();
            ddlItemType.DataValueField = "Id";
            ddlItemType.DataTextField = "ItemTypeName";
            ddlItemType.DataBind();
            ddlItemType.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlItemColor.DataSource = invContext.INV_Color.OrderBy(x => x.Name).ToList();
            ddlItemColor.DataValueField = "Id";
            ddlItemColor.DataTextField = "Name";
            ddlItemColor.DataBind();
            ddlItemColor.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlItemModel.DataSource = invContext.INV_Model.OrderBy(x => x.Name).ToList();
            ddlItemModel.DataValueField = "Id";
            ddlItemModel.DataTextField = "Name";
            ddlItemModel.DataBind();
            ddlItemModel.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlItemClass.Items.Add(new ListItem(text: "[Select One]", value: "0"));
            ddlItemClass.Items.Add(new ListItem(text: "Fixed Asset", value: "1"));
            ddlItemClass.Items.Add(new ListItem(text: "Consumable", value: "2"));
            ddlItemClass.Items.Add(new ListItem(text: "Non-Consumable", value: "3"));
            ddlItemClass.Items.FindByValue("0").Selected = true;
        }
        #endregion

        protected void rvStockBalance_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}