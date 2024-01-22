using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptDesignationWiseManpowerSummary : ReportBase
    {

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

            string strZoneId = ConvertZoneArrayListToString(arrZoneList);
            var designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
            //var fromDate = Convert.ToDateTime(txtFromDate.Text);
            //var toDate = Convert.ToDateTime(txtToDate.Text);          
            GenerateReport(strZoneId, designationId);
        }

        #endregion

        #region Generate Report
        private void GenerateReport(string ZoneInfoIds,int designationId)
        {
            rvDesignationWiseManpower.Reset();
            rvDesignationWiseManpower.ProcessingMode = ProcessingMode.Local;
            rvDesignationWiseManpower.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptDesignationWiseManpowerSummary.rdlc");

            #region Processing Report Data

            var data = (from s in base.context.SP_PRM_RptDesignationWiseManpowerInfo(ZoneInfoIds, designationId) select s).OrderBy(o => o.SortingOrder).ToList();
            //data = data.Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).ToList();
           // data = data.Where(zoneList.Contains.va);
            #endregion

            #region Search parameter
            //string searchParameters = string.Empty;
            //string searchParameters1 = string.Empty;
            //string searchParameters2 = string.Empty;

            //if (fromDate != null && toDate != null)
            //{
            //    searchParameters = fromDate.ToString("dd-MMM-yy");
            //    searchParameters1 = toDate.ToString("dd-MMM-yy");
            //}

            //searchParameters2 = ddlZone.SelectedItem.Text;
            //if (ZoneInfoId != 0)
            //{
            //    searchParameters2 = ddlZone.SelectedItem.Text;
            //}

            //ReportParameter p1 = new ReportParameter("FromDate", searchParameters);
            //ReportParameter p2 = new ReportParameter("ToDate", searchParameters1);
            //ReportParameter p3 = new ReportParameter("Department", searchParameters2);

            //rvDeptWiseManpower.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3 });

            #endregion

            rvDesignationWiseManpower.LocalReport.DataSources.Add(new ReportDataSource("DSDesignationWiseManpowerSummary", data));
            this.rvDesignationWiseManpower.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvDesignationWiseManpower.DataBind();

            //ExportToPDF
            String newFileName = "DesignationWiseManpowerList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvDesignationWiseManpower, newFileName, fs);
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

            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.SortingOrder).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));
        }

        #endregion


        protected void rvDesignationWiseManpower_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}