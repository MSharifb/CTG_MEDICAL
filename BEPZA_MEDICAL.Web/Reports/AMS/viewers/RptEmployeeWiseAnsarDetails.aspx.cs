using ERP_BEPZA.DAL.PRM;
using ERP_BEPZA.DAL.AMS;
using ERP_BEPZA.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity.Core.Objects;

namespace ERP_BEPZA.Web.Reports.AMS.viewers
{
    public partial class RptEmployeeWiseAnsarDetails : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptEmployeeWiseAnsarDetails()
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
        int employeeId = 0;


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

                if (!String.IsNullOrEmpty(hdnEmployeeId.Value))
                {
                    var empId = hdnEmployeeId.Value;
                    employeeId = (from p in amsContext.AMS_AnsarEmpInfo
                                  where p.BEPZAId.Trim().ToUpper() == empId.Trim().ToUpper()
                                  select p.Id).FirstOrDefault();
                }
                else
                    employeeId = 0;

                GenerateReport(strZoneId, employeeId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvEmployeeWiseAnsarDetails.Reset();
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
        public void GenerateReport(string strZoneId, int employeeId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvEmployeeWiseAnsarDetails.Reset();
            rvEmployeeWiseAnsarDetails.ProcessingMode = ProcessingMode.Local;
            rvEmployeeWiseAnsarDetails.LocalReport.ReportPath = Server.MapPath("~/Reports/AMS/rdlc/RptEmployeeWiseAnsarDetails.rdlc");

            var data = amsContext.sp_AMS_RptEmployeeWiseAnsarDetails(employeeId, strZoneId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Employee Name: " + hdnEmployeeName.Value;
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvEmployeeWiseAnsarDetails.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsEmployeeWiseAnsarDetails", data);
                rvEmployeeWiseAnsarDetails.LocalReport.DataSources.Add(dataSource);
                this.rvEmployeeWiseAnsarDetails.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvEmployeeWiseAnsarDetails.Reset();
            }
            rvEmployeeWiseAnsarDetails.DataBind();

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {

            dynamic data = null;
            var dsName = string.Empty;
            switch (e.ReportPath)
            {
                case "_ReportHeader":
                    data = (from c in base.context.vwCompanyInformations
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

        protected void rvEmployeeWiseAnsarDetails_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}