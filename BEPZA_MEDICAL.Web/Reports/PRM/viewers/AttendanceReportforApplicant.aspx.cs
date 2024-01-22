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
    public partial class Attendance_Report_for_Applicant : ReportBase
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

            GenerateReport(jobAdvertisementInfoId, designationId);
        }
        #endregion

        #region Generate Report
        private void GenerateReport(int jobAdvertisementInfoId, int designationId)
        {
            rvShortListedApplicant.Reset();
            rvShortListedApplicant.ProcessingMode = ProcessingMode.Local;
            rvShortListedApplicant.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/AttendanceReportforApplicant.rdlc");

            #region Processing Report Data
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));


            var data = (from s in base.context.EREC_uspReportAttendanceInformation(jobAdvertisementInfoId, designationId, numErrorCode, strErrorMsg) select s).ToList();
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
            rvShortListedApplicant.LocalReport.SetParameters(new ReportParameter[] { p2 });

            #endregion

            rvShortListedApplicant.LocalReport.DataSources.Add(new ReportDataSource("DsAttendanceInformation", data));
            this.rvShortListedApplicant.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvShortListedApplicant.DataBind();
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
            ddladvertisementCode.DataSource = context.PRM_JobAdvertisementInfo.Where(q => q.ZoneInfoId== LoggedUserZoneInfoId).ToList();
            ddladvertisementCode.DataValueField = "Id";
            ddladvertisementCode.DataTextField = "AdCode";
            ddladvertisementCode.DataBind();
            ddladvertisementCode.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("[ALL]", "0"));
        }

        #endregion


        protected void rvShortListedApplicant_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        [WebMethod]
        public static ArrayList FetchDesignation(int Id)
        {
            ReportBase dbContext = new ReportBase();
            ArrayList list = new ArrayList();

            var items = (from IvCardIss in dbContext.context.PRM_ApplicantInterviewCardIssue
                         join designation in dbContext.context.PRM_Designation on IvCardIss.DesignationId equals designation.Id
                         where IvCardIss.Id == Id
                         select new
                         {
                             Id = designation.Id,
                             Name = designation.Name
                         }).ToList();
            foreach (var item in items)
            {
                list.Add(item);
            }
            return list;
        }
    }
}