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
    public partial class RptBlacklistedAnsarList : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptBlacklistedAnsarList()
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
        DateTime? FromDt = null;
        DateTime? ToDt = null;
        

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
                //fromDate = Convert.ToDateTime(transferDtFromDate.Text);
                //toDate = Convert.ToDateTime(transferDtToDate.Text);

                if (blacklistedFromDate.Text != "")
                {
                    FromDt = Convert.ToDateTime(blacklistedFromDate.Text);
                }
                if (blacklistedToDate.Text != "")
                {
                    ToDt = Convert.ToDateTime(blacklistedToDate.Text);
                }

                GenerateReport(FromDt, ToDt, strZoneId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvBlacklistedAnsarList.Reset();
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
        public void GenerateReport(DateTime? fromDate, DateTime? toDate, string strZoneId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvBlacklistedAnsarList.Reset();
            rvBlacklistedAnsarList.ProcessingMode = ProcessingMode.Local;
            rvBlacklistedAnsarList.LocalReport.ReportPath = Server.MapPath("~/Reports/AMS/rdlc/RptBlacklistedAnsarList.rdlc");

            var data = amsContext.sp_AMS_RptBlacklistedAnsarList(strZoneId, fromDate, toDate, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                //if(FromDt != null && ToDt != null)
                //{
                //   searchParameters = "For the period from: " + FromDt.Value.ToShortDateString() + " To " + ToDt.Value.ToShortDateString();
                //}
                
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvBlacklistedAnsarList.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsBlacklistedAnsarList", data);
                rvBlacklistedAnsarList.LocalReport.DataSources.Add(dataSource);
                this.rvBlacklistedAnsarList.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvBlacklistedAnsarList.Reset();
            }
            rvBlacklistedAnsarList.DataBind();

            //ExportToPDF
            String newFileName = "BlacklistedAnsarList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvBlacklistedAnsarList, newFileName, fs);

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
        }
        #endregion

        protected void rvBlacklistedAnsarList_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}