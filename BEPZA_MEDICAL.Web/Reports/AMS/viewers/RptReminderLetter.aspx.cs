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
    public partial class RptReminderLetter : ReportBase
    {
        #region Fields

        bool checkStatus;
        string reminderTypeName = string.Empty;

        #endregion

        #region Ctor
        public RptReminderLetter()
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
        int reminderTypeId = 0;
        DateTime dateFrom;
        DateTime dateTo;
        
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

                reminderTypeId = Convert.ToInt32(ddlReminderType.SelectedValue);
                reminderTypeName = ddlReminderType.SelectedItem.Text;

                dateFrom = Convert.ToDateTime(dpDateFrom.Text);
                dateTo = Convert.ToDateTime(dpDateTo.Text);

                GenerateReport(strZoneId, reminderTypeId, dateFrom, dateTo);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvReminderLetter.Reset();
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
        public void GenerateReport(string strZoneId, int reminderTypeId, DateTime dateFrom, DateTime dateTo)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvReminderLetter.Reset();
            rvReminderLetter.ProcessingMode = ProcessingMode.Local;
            rvReminderLetter.LocalReport.ReportPath = Server.MapPath("~/Reports/AMS/rdlc/RptReminderLetter.rdlc");

            var data = amsContext.sp_AMS_RptReminderLetter(strZoneId, reminderTypeId, dateFrom, dateTo, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = string.Concat("Reminder Type : ", reminderTypeName);
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvReminderLetter.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsReminderLetter", data);
                rvReminderLetter.LocalReport.DataSources.Add(dataSource);
                this.rvReminderLetter.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvReminderLetter.Reset();
            }
            rvReminderLetter.DataBind();

            //ExportToPDF
            String newFileName = "ReminderLetter_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvReminderLetter, newFileName, fs);

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

            ddlReminderType.DataSource = amsContext.AMS_ReminderType.OrderBy(x => x.SortOrder).ToList();
            ddlReminderType.DataValueField = "Id";
            ddlReminderType.DataTextField = "Name";
            ddlReminderType.DataBind();
            ddlReminderType.Items.Insert(0, new ListItem("[Select One]", "0"));
        }
        #endregion

        protected void rvReminderLetter_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}