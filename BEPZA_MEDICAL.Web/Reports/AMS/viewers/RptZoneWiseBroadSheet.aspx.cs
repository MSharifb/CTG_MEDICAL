using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.AMS.viewers
{
    public partial class RptZoneWiseBroadSheet : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptZoneWiseBroadSheet()
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
                rvZoneWiseSecurityPersonnelList.LocalReport.EnableHyperlinks = true;
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

                int designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
                int districtId = Convert.ToInt32(ddlDistrict.SelectedValue);
                decimal from = 0;
                if (txtFrom.Text != "" && txtFrom.Text != string.Empty)
                {
                    from = Convert.ToDecimal(txtFrom.Text);
                }
                decimal to = 0;
                if (txtTo.Text != "" && txtTo.Text != string.Empty)
                {
                    to = Convert.ToDecimal(txtTo.Text);
                }
                int punishmentId = Convert.ToInt32(ddlPunishment.SelectedValue);
                int rewardId = Convert.ToInt32(ddlReward.SelectedValue);
                int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue);


                GenerateReport(strZoneId, statusId, designationId, districtId, from, to, punishmentId, rewardId, employeeId);

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
        public void GenerateReport(string strZoneId, bool statusId, int designationId, int districtId, decimal from, decimal to, int punishmentId, int rewardId, int employeeId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvZoneWiseSecurityPersonnelList.Reset();
            rvZoneWiseSecurityPersonnelList.LocalReport.EnableHyperlinks = true;
            rvZoneWiseSecurityPersonnelList.ProcessingMode = ProcessingMode.Local;
            rvZoneWiseSecurityPersonnelList.LocalReport.ReportPath = Server.MapPath("~/Reports/AMS/rdlc/RptZoneWiseBroadSheet.rdlc");

            var data = amsContext.sp_SMS_RptZoneWiseBroadSheet(strZoneId, statusId, designationId,districtId,punishmentId, rewardId, from, to, employeeId, numErrorCode, strErrorMsg).ToList();

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

                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

                ReportParameter p1 = new ReportParameter("param", searchParameters);
                ReportParameter p2 = new ReportParameter("param1", baseUrl);

                rvZoneWiseSecurityPersonnelList.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsZoneWiseBroadSheet", data);
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
            String newFileName = "ZoneWiseBroadSheet_" + Guid.NewGuid() + ".pdf";
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

            HashSet<int> empIDs = new HashSet<int>(amsContext.SMS_SecurityInfo.Select(x=>x.EmployeeId).ToList());

            var EmpList =prmContext.PRM_EmploymentInfo.Where(s => empIDs.Contains(s.Id)).ToList();

            HashSet<int> EmpDesiList = new HashSet<int>(EmpList.Select(x=>x.DesignationId).ToList());

            ddlDesignation.DataSource = prmContext.PRM_Designation.Where(x=> EmpDesiList.Contains(x.Id)).OrderBy(s => s.Name).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));


            ddlDistrict.DataSource = prmContext.PRM_District.OrderBy(x => x.DistrictName).ToList();
            ddlDistrict.DataValueField = "Id";
            ddlDistrict.DataTextField = "DistrictName";
            ddlDistrict.DataBind();
            ddlDistrict.Items.Insert(0, new ListItem("All", "0"));

            ddlPunishment.DataSource = amsContext.SMS_SecurityServiceHistory.Where(s=>s.DisciplineRecord != null && s.DisciplineRecord!=string.Empty).OrderBy(x => x.DisciplineRecord).ToList();
            ddlPunishment.DataValueField = "Id";
            ddlPunishment.DataTextField = "DisciplineRecord";
            ddlPunishment.DataBind();
            ddlPunishment.Items.Insert(0, new ListItem("All", "0"));

            ddlReward.DataSource = amsContext.SMS_SecurityServiceHistory.Where(s => s.Award != null && s.Award != string.Empty).OrderBy(x => x.Award).ToList();
            ddlReward.DataValueField = "Id";
            ddlReward.DataTextField = "Award";
            ddlReward.DataBind();
            ddlReward.Items.Insert(0, new ListItem("All", "0"));

            ddlStatus.Items.Insert(0, new ListItem("Active", "true"));
            ddlStatus.Items.Insert(1, new ListItem("Inactive", "false"));
            ddlStatus.Items.FindByText("Active").Selected = true;

            ddlEmployee.DataSource = amsContext.SMS_SecurityInfo
                                    .Select(q => new
                                    {
                                        EmpID = q.PRM_EmploymentInfo.Id,
                                        DisplayText = q.PRM_EmploymentInfo.FullName + " [" + q.PRM_EmploymentInfo.EmpID + "]"
                                    }).ToList()
                                    .OrderBy(x => x.DisplayText);

            ddlEmployee.DataValueField = "EmpID";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", "0"));
        }
        #endregion

        protected void rvZoneWiseSecurityPersonnelList_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}