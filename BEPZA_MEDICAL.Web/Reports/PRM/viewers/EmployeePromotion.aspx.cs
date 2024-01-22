using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.ReportingServices;
using Microsoft.Reporting.WebForms;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.DAL.PRM;
using System.IO;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class EmployeePromotion : ReportBase
    {
        #region Ctor
        public EmployeePromotion()
        {
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

            DateTime? fromDate=null;
            DateTime? toDate=null;

            if (dtPromotionFromDate.Text != "")
            {
                fromDate = Convert.ToDateTime(dtPromotionFromDate.Text);
            }
            if (dtPromotionToDate.Text != "")
            {
                toDate = Convert.ToDateTime(dtPromotionToDate.Text);
            }
            var divisionId = Convert.ToInt32(ddlDivision.SelectedValue);
            var salaryScaleId = Convert.ToInt32(ddlSalaryScaleName.SelectedValue);
            var titleFromId = Convert.ToInt32(ddlJobTitleFrom.SelectedValue);
            var titleToId = Convert.ToInt32(ddlJobTitleTo.SelectedValue);
            var jobGradeFromId = Convert.ToInt32(ddlJobGradeFrom.SelectedValue);
            var jobGradeToId = Convert.ToInt32(ddlJobGradeTo.SelectedValue);
            GenerateReport(list,fromDate, toDate, divisionId, salaryScaleId, titleFromId, titleToId, jobGradeFromId, jobGradeToId);
        }

        #endregion

        #region Generate Report
        public void GenerateReport(List<PRM_ZoneInfo> list, DateTime? fromDate, DateTime? toDate, int divisionId, int salaryScaleId, int TitleFromId, int TitleToId, int JobGradeFromId, int JobGradeToId)
        {
            rvEmployeePromotion.Reset();
            rvEmployeePromotion.ProcessingMode = ProcessingMode.Local;
            rvEmployeePromotion.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/EmployeePromotion.rdlc");
            
            #region Processing Report Data

              var data = (from e in base.context.vwPromotedEmployees
                          select e).OrderBy(o => o.PromotionDate).ToList();
                  data = data.Where(x => list.Select(n => n.Id).Contains(x.ZoneInfoId)).ToList();

            if (data != null)
            {
                if (fromDate != null && toDate != null)
                {
                    data = data.Where(q => q.PromotionDate>=fromDate  && q.PromotionDate<=toDate && Convert.ToDateTime(q.PromotionDate).Year> 2014).ToList();       
                }
                if (divisionId > 0) data = data.Where(q => q.DivisionId == divisionId).ToList();
                if (salaryScaleId > 0) data = data.Where(q => q.ToSalaryScaleId == salaryScaleId).ToList();
                if (TitleFromId > 0) data = data.Where(q => q.FromJobTitleId == TitleFromId).ToList();
                if (TitleToId > 0) data = data.Where(q => q.ToJobTitleId == TitleToId).ToList();
                if (JobGradeFromId > 0) data = data.Where(q => q.FromJobGradeId == JobGradeFromId).ToList();
                if (JobGradeToId > 0) data = data.Where(q => q.ToJobGradeId == JobGradeToId).ToList();
            }  
            #endregion

            ReportDataSource dataSource = new ReportDataSource("EmployeePromotion", data);
            rvEmployeePromotion.LocalReport.DataSources.Add(dataSource);
            string searchParameters = "";
            if (fromDate != null && toDate != null)
            {
                searchParameters = "Promotion Date: " + Convert.ToDateTime(fromDate).ToString("dd MMM yyyy") + " to " + Convert.ToDateTime(toDate).ToString("dd MMM yyyy");
            }
            ReportParameter p1 = new ReportParameter("param", searchParameters);
            rvEmployeePromotion.LocalReport.SetParameters(new ReportParameter[] { p1 });
            this.rvEmployeePromotion.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

            rvEmployeePromotion.DataBind();

            //ExportToPDF
            String newFileName = "EmployeePromotionList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeePromotion, newFileName, fs);
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
            ddlDivision.DataSource = context.PRM_Division.OrderBy(x => x.Name).ToList();
            ddlDivision.DataValueField = "Id";
            ddlDivision.DataTextField = "Name";            
            ddlDivision.DataBind();
            ddlDivision.Items.Insert(0,new ListItem("All", "0"));

            ddlSalaryScaleName.DataSource = context.PRM_SalaryScale.OrderByDescending(x => x.Id).ToList();
            ddlSalaryScaleName.DataValueField = "Id";
            ddlSalaryScaleName.DataTextField = "SalaryScaleName";
            ddlSalaryScaleName.DataBind();

            ddlJobTitleFrom.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlJobTitleFrom.DataValueField = "Id";
            ddlJobTitleFrom.DataTextField = "Name";
            ddlJobTitleFrom.DataBind();
            ddlJobTitleFrom.Items.Insert(0, new ListItem("All", "0"));

            ddlJobTitleTo.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlJobTitleTo.DataValueField = "Id";
            ddlJobTitleTo.DataTextField = "Name";
            ddlJobTitleTo.DataBind();
            ddlJobTitleTo.Items.Insert(0, new ListItem("All", "0"));

            var salaryScaleId = Convert.ToInt32(ddlSalaryScaleName.SelectedValue);

            ddlJobGradeFrom.DataSource = context.PRM_JobGrade.Where(x => x.SalaryScaleId==salaryScaleId).OrderBy(x => x.GradeName).ToList();
            ddlJobGradeFrom.DataValueField = "Id";
            ddlJobGradeFrom.DataTextField = "GradeName";
            ddlJobGradeFrom.DataBind();
            ddlJobGradeFrom.Items.Insert(0, new ListItem("All", "0"));

            ddlJobGradeTo.DataSource = context.PRM_JobGrade.Where(x => x.SalaryScaleId==salaryScaleId).OrderBy(x => x.GradeName).ToList();
            ddlJobGradeTo.DataValueField = "Id";
            ddlJobGradeTo.DataTextField = "GradeName";
            ddlJobGradeTo.DataBind();
            ddlJobGradeTo.Items.Insert(0, new ListItem("All", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }
        #endregion

        protected void rvEmployeePromotion_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}
