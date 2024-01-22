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
    public partial class RptZoneWiseSecurityPersonnelList : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptZoneWiseSecurityPersonnelList()
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
        bool statusId = true;
        
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

                statusId = Convert.ToBoolean(ddlStatus.SelectedValue);

                GenerateReport(strZoneId, statusId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvZoneWiseSecurityPersonnelList.Reset();
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
        public void GenerateReport(string strZoneId, bool statusId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvZoneWiseSecurityPersonnelList.Reset();
            rvZoneWiseSecurityPersonnelList.ProcessingMode = ProcessingMode.Local;
            rvZoneWiseSecurityPersonnelList.LocalReport.ReportPath = Server.MapPath("~/Reports/AMS/rdlc/RptZoneWiseSecurityPersonnelList.rdlc");

            var data = amsContext.sp_SMS_RptZoneWiseSecurityPersonnelList(strZoneId, statusId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Status : ";
                if (statusId == true)
                {
                    searchParameters += "Active";
                }
                else 
                {
                    searchParameters += "Inactive";
                }

                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvZoneWiseSecurityPersonnelList.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsZoneWiseSecurityPersonnelList", data);
                rvZoneWiseSecurityPersonnelList.LocalReport.DataSources.Add(dataSource);
                this.rvZoneWiseSecurityPersonnelList.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvZoneWiseSecurityPersonnelList.Reset();
            }
            rvZoneWiseSecurityPersonnelList.DataBind();

            //ExportToPDF
            String newFileName = "ZoneWiseSecurityList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvZoneWiseSecurityPersonnelList, newFileName, fs);

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

            //ddlStatus.DataSource = amsContext.AMS_EmpStatus.OrderBy(x => x.SortOrder).ToList();
            //ddlStatus.DataValueField = "Id";
            //ddlStatus.DataTextField = "Name";
            //ddlStatus.DataBind();
            //ddlStatus.Items.FindByText("Active").Selected = true;
            ddlStatus.Items.Insert(0, new ListItem("Active", "true"));
            ddlStatus.Items.Insert(1, new ListItem("Inactive", "false"));
            ddlStatus.Items.FindByText("Active").Selected = true;
        }
        #endregion

        protected void rvZoneWiseSecurityPersonnelList_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}