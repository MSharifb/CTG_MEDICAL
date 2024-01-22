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
    public partial class RptSecurityPersonnelDetails : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptSecurityPersonnelDetails()
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
                //
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

                var zoneList = GetZoneDDL().Select(x => x.Value);

                strZoneId = string.Join(",", zoneList);

                if (!String.IsNullOrEmpty(hdnEmployeeId.Value))
                {
                    var empId = hdnEmployeeId.Value;
                    employeeId = (from p in prmContext.PRM_EmploymentInfo
                                  where p.EmpID.Trim().ToUpper() == empId.Trim().ToUpper()
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
                    rvSecurityPersonnelDetails.Reset();
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

            rvSecurityPersonnelDetails.Reset();
            rvSecurityPersonnelDetails.ProcessingMode = ProcessingMode.Local;
            rvSecurityPersonnelDetails.LocalReport.ReportPath = Server.MapPath("~/Reports/AMS/rdlc/RptSecurityPersonnelDetails.rdlc");

            var data = amsContext.sp_SMS_RptSecurityPersonnelDetails(employeeId, strZoneId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Name: " + hdnEmployeeName.Value;
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvSecurityPersonnelDetails.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsSecurityPersonnelDetails", data);
                rvSecurityPersonnelDetails.LocalReport.DataSources.Add(dataSource);
                this.rvSecurityPersonnelDetails.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvSecurityPersonnelDetails.Reset();
            }
            rvSecurityPersonnelDetails.DataBind();

            //ExportToPDF
            String newFileName = "SecurityPersonnelDetails_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvSecurityPersonnelDetails, newFileName, fs);

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

        protected void rvSecurityPersonnelDetails_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}