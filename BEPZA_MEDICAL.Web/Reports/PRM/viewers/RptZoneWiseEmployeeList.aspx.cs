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

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptZoneWiseEmployeeList : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptZoneWiseEmployeeList()
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
            List<PRM_ZoneInfo> list = new List<PRM_ZoneInfo>();
            foreach (ListItem item in ddlZone.Items)
            {
                if (item.Selected)
                {
                    PRM_ZoneInfo obj = new PRM_ZoneInfo();
                    obj.Id = Convert.ToInt32(item.Value);
                    obj.ZoneName = item.Text;
                    list.Add(obj);                   
                }
            }
           
            var generationDate = txtGenerationDate.Text;

            GenerateReport(list, generationDate);
        }

        #endregion

        #region Generate Report

        public void GenerateReport(List<PRM_ZoneInfo> list, string generationDate)
        {
            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptZoneWiseEmployeeList.rdlc");

            #region Processing Report Data

            var data = (from e in base.context.vw_RptEmployeeList
                        where e.EmploymentStatus.ToLower().Equals("active")
                        select e).OrderBy(x => x.DesignationSortingOrder).ToList();
            data = data.Where(x => list.Select(n => n.Id).Contains(x.ZoneInfoId)).OrderBy(x => x.DesignationSortingOrder).ToList();
            if (generationDate.Trim() != "") data = data.Where(q => q.DateofJoining <= BEPZA_MEDICAL.Web.Utility.Common.FormatStringToDate(generationDate)).OrderBy(x => x.DesignationSortingOrder).ToList();

            #endregion

            DateTime effectiveDate;
            if (DateTime.TryParse(generationDate, out effectiveDate))
            {
                rvEmployeeInfo.LocalReport.DataSources.Add(new ReportDataSource("DSZoneWiseEmployeeListRpt", data));
                string searchParameters = "As on : " + effectiveDate.ToString("dd MMM yyyy");
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvEmployeeInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });
            }

            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "EmployeeList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);
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
            
        }

        #endregion

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}