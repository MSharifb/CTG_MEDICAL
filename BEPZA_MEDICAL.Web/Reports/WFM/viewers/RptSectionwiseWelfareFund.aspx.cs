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
    public partial class RptSectionwiseWelfareFund : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptSectionwiseWelfareFund()
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
               var empId = ddlEmployee.SelectedValue;
               var year = ddlYear.SelectedValue.ToString();
               GenerateReport(strZoneId, sectionId, empId, year);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneList, int sectionId, string empId, string year)
        {
            #region Processing Report Data

            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/WFM/rdlc/SectionwiseWelfareFund.rdlc");

            var data = (from s in base.wfmContext.SP_WFM_SectionWiseWelfareFund(strZoneList, sectionId, empId, year) select s).ToList();

            #region Search parameter

            #endregion


            ReportDataSource dataSource = new ReportDataSource("DSSectionwiseWelfareFund", data);
            rvEmployeeInfo.LocalReport.DataSources.Add(dataSource);
            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "SectionWiseWelfareFund_" + Guid.NewGuid() + ".pdf";
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

            ddlSection.DataSource = context.PRM_Section.Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            ddlSection.DataValueField = "Id";
            ddlSection.DataTextField = "Name";
            ddlSection.DataBind();
            ddlSection.Items.Insert(0, new ListItem("All", "0"));

            ddlYear.DataSource = BEPZA_MEDICAL.Web.Utility.Common.PopulateYearList();
            ddlYear.DataValueField = "Value";
            ddlYear.DataTextField = "Text";
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, new ListItem("All", "0"));

            ddlEmployee.DataSource = context.PRM_EmploymentInfo.Select(q => new { ZoneInfoId = q.ZoneInfoId, EmpID = q.EmpID, DisplayText = q.FullName + " [" + q.EmpID + " ]" }).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.DisplayText).ToList();
            ddlEmployee.DataValueField = "EmpID";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", ""));

        }

        #endregion

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}