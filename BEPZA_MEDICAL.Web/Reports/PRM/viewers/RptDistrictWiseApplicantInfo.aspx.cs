using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptDistrictWiseApplicantInfo : ReportBase
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
            var jobAdvertisementInfoId = Convert.ToInt32(ddladvertisementCode.SelectedValue);
            var designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
            var districtId = Convert.ToInt32(ddlDistrict.SelectedValue);

            GenerateReport(jobAdvertisementInfoId, designationId, districtId);
        }
        #endregion

        #region Generate Report
        private void GenerateReport(int jobAdvertisementInfoId, int designationId, int districtId)
        {
            rvApplicant.Reset();
            rvApplicant.ProcessingMode = ProcessingMode.Local;
            rvApplicant.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptDistrictWiseApplicantInfo.rdlc");

            #region Processing Report Data
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var data = (from s in base.context.EREC_uspReportListofCandidates(jobAdvertisementInfoId, designationId, null, null, 0, null, null, null, districtId, null, 0, 0, null, null, 0, numErrorCode, strErrorMsg) select s).ToList();
            data.ToList();

            #endregion

            #region Search parameter

            string searchParameters1 = string.Empty;

            if (designationId > 0)
            {
                string ddlDesig = ddlDesignation.SelectedItem.Text;
                searchParameters1 = "Position Name  : " + ddlDesig;
            }
            else
            {
                searchParameters1 = "Position Name  : All";
            }

            ReportParameter p2 = new ReportParameter("PostName", searchParameters1);
            rvApplicant.LocalReport.SetParameters(new ReportParameter[] { p2 });

            #endregion

            rvApplicant.LocalReport.DataSources.Add(new ReportDataSource("DsListofApplicants", data));
            this.rvApplicant.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvApplicant.DataBind();
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
            ddladvertisementCode.DataSource = context.PRM_JobAdvertisementInfo.Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            ddladvertisementCode.DataValueField = "Id";
            ddladvertisementCode.DataTextField = "AdCode";
            ddladvertisementCode.DataBind();
            ddladvertisementCode.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("[ALL]", "0"));

            ddlDistrict.DataSource = context.EREC_tblDistrict.OrderBy(x => x.Name).ToList();
            ddlDistrict.DataValueField = "intPK";
            ddlDistrict.DataTextField = "Name";
            ddlDistrict.DataBind();
            ddlDistrict.Items.Insert(0, new ListItem("[ALL]", "0"));

        }

        #endregion


        protected void rvApplicant_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
   }
}