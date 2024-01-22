using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.WFM.viewers
{
    public partial class RptWelfareFundConsolidateReport : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptWelfareFundConsolidateReport()
        {
            _empRepository = new PRM_GenericRepository<PRM_EmploymentInfo>(new ERP_BEPZAPRMEntities());
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

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
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

               string  strZoneId = ConvertZoneArrayListToString(arrZoneList);
               var sectionId = Convert.ToInt32(ddlSection.SelectedValue);
               var departmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
               var yearId = ddlYear.SelectedValue.ToString();
               GenerateReport(strZoneId, sectionId, departmentId, yearId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, int sectionId, int departmentId, string yearId)
        {
            #region Processing Report Data

            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/WFM/rdlc/WelfareFundConsolidateReport.rdlc");

            var data = (from s in base.wfmContext.SP_WFM_WelfareFundConsolidateReport(strZoneList, sectionId, departmentId, yearId) select s).ToList();

            #region Search parameter

            #endregion


            ReportDataSource dataSource = new ReportDataSource("DSWelfareFundConsolidateReport", data);
            rvEmployeeInfo.LocalReport.DataSources.Add(dataSource);
            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "WelfareFundConsolidateReport_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
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

            ddlSection.DataSource = context.PRM_Section.OrderBy(x => x.Name).ToList();
            ddlSection.DataValueField = "Id";
            ddlSection.DataTextField = "Name";
            ddlSection.DataBind();
            ddlSection.Items.Insert(0, new ListItem("All", "0"));

            ddlDepartment.DataSource = context.PRM_Division.OrderBy(x => x.Name).ToList();
            ddlDepartment.DataValueField = "Id";
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("All", "0"));

            ddlYear.DataSource = BEPZA_MEDICAL.Web.Utility.Common.PopulateYearList();
            ddlYear.DataValueField = "Value";
            ddlYear.DataTextField = "Text";
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, new ListItem("All", "0"));

        }

        #endregion

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}