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
    public partial class RptOrganizationWiseSecurityPersonnelList : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptOrganizationWiseSecurityPersonnelList()
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
        int orgId;
        
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;
                orgId = 0;

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

                if(ddlOrg.SelectedIndex > 0)
                {
                    orgId = Convert.ToInt32(ddlOrg.SelectedValue);
                }

                GenerateReport(strZoneId, statusId, orgId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvOrganizationWiseSecurityPersonnelList.Reset();
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
        public void GenerateReport(string strZoneId, bool statusId, int orgId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvOrganizationWiseSecurityPersonnelList.Reset();
            rvOrganizationWiseSecurityPersonnelList.ProcessingMode = ProcessingMode.Local;
            rvOrganizationWiseSecurityPersonnelList.LocalReport.ReportPath = Server.MapPath("~/Reports/AMS/rdlc/RptOrganizationWiseSecurityPersonnelList.rdlc");

            var data = amsContext.sp_SMS_RptOrganizationWiseSecurityPersonnelList(strZoneId, statusId, orgId, numErrorCode, strErrorMsg).ToList();

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

                if(ddlOrg.SelectedIndex > 0)
                {
                    var strOrg = string.Concat("Organization : ", ddlOrg.SelectedItem.Text);
                    searchParameters += strOrg;
                }

                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvOrganizationWiseSecurityPersonnelList.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsOrganizationWiseSecurityPersonnelList", data);
                rvOrganizationWiseSecurityPersonnelList.LocalReport.DataSources.Add(dataSource);
                this.rvOrganizationWiseSecurityPersonnelList.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvOrganizationWiseSecurityPersonnelList.Reset();
            }
            rvOrganizationWiseSecurityPersonnelList.DataBind();

            //ExportToPDF
            String newFileName = "OrganizationWiseSecuritylist_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvOrganizationWiseSecurityPersonnelList, newFileName, fs);

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

            ddlOrg.DataSource = amsContext.SMS_Organization.OrderBy(x => x.SortOrder).ToList();
            ddlOrg.DataValueField = "Id";
            ddlOrg.DataTextField = "Name";
            ddlOrg.DataBind();
            ddlOrg.Items.Insert(0, new ListItem("[Select One]", "0"));

        }
        #endregion

        protected void rvOrganizationWiseSecurityPersonnelList_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}