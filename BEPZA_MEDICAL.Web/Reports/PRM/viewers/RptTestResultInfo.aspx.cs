using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptTestResultInfo : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptTestResultInfo()
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
        int designationId = 0;
        int refId = 0;
        int examTypeId = 0;
        string strZoneId = string.Empty;

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try{
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
                designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
                refId = Convert.ToInt32(ddlAdvertisement.SelectedValue);
                examTypeId = Convert.ToInt32(ddlExamType.SelectedValue);

                GenerateReport(refId, designationId, examTypeId, strZoneId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int RefId, int DesignationId, int ExamTypeId, string strZoneId)
        {
            #region Processing Report Data

            rvTestResult.Reset();
            rvTestResult.ProcessingMode = ProcessingMode.Local;
            rvTestResult.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptTestResultInfo.rdlc");

            var data = (from s in base.prmContext.SP_PRM_RptTestResultInfo(RefId, DesignationId, ExamTypeId, strZoneId) select s).ToList();

            #region Search parameter

            string searchParameters = string.Empty;
            searchParameters = ddlExamType.SelectedItem.Text;
            ReportParameter p1 = new ReportParameter("ExamType", searchParameters);
            rvTestResult.LocalReport.SetParameters(new ReportParameter[] { p1});

            #endregion


            ReportDataSource dataSource = new ReportDataSource("DSTestResultInfo", data);
            rvTestResult.LocalReport.DataSources.Add(dataSource);
            this.rvTestResult.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvTestResult.DataBind();

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            if (e.ReportPath != "_ReportHeader")
            {
                //DesignationId = Convert.ToInt32(e.Parameters["DesignationId"].Values[0].ToString());
                //RefId = Convert.ToInt32(e.Parameters["RefId"].Values[0].ToString());
                //ExamTypeId = Convert.ToInt32(e.Parameters["ExamTypeId"].Values[0].ToString());
            }
            switch (e.ReportPath)
            {
                case "RptTestResultInfo_Head":
                    data = (from s in base.prmContext.SP_PRM_RptTestResultHead(refId, designationId, examTypeId, strZoneId) select s).ToList();
                    dsName = "DSTestResultHead";
                    break;

                case "RptTestResultInfo_Quota":
                    data = (from s in base.prmContext.SP_PRM_RptTestResultQuota(refId, designationId, examTypeId, strZoneId) select s).ToList();
                    dsName = "DSTestResultQuota";
                    break;

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
            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlExamType.DataSource = context.PRM_SelectionCritariaOrExamType.OrderBy(x => x.Name).ToList();
            ddlExamType.DataValueField = "Id";
            ddlExamType.DataTextField = "Name";
            ddlExamType.DataBind();
            ddlExamType.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlAdvertisement.DataSource = context.PRM_JobAdvertisementInfo.Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();
            ddlAdvertisement.DataValueField = "Id";
            ddlAdvertisement.DataTextField = "AdCode";
            ddlAdvertisement.DataBind();
            ddlAdvertisement.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }
         #endregion

        protected void rvTestResult_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}