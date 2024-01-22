using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptDesignation : ReportBase
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
            var designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
            //var jobGradeId = Convert.ToInt32(ddlJobGrade.SelectedValue);
            //GenerateReport(designationId, jobGradeId);
            GenerateReport(designationId);
        }
        #endregion

        #region Generate Report
        private void GenerateReport(int designationId)
        {
            rvDesignation.Reset();
            rvDesignation.ProcessingMode = ProcessingMode.Local;
            rvDesignation.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptDesignation.rdlc");

            #region Processing Report Data

            var data = (from d in base.context.vwPRMDesignationRpts select d).OrderBy(o => o.GradeId).ToList();

            if (designationId > 0) data = data.Where(q => q.Id == designationId).OrderBy(o => o.GradeId).ToList();
         //   if (jobGradeId > 0) data = data.Where(q => q.GradeId == jobGradeId).ToList();


            #endregion

            rvDesignation.LocalReport.DataSources.Add(new ReportDataSource("DSDesignation", data));
            this.rvDesignation.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvDesignation.DataBind();

            //ExportToPDF
            String newFileName = "DesignationList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvDesignation, newFileName, fs);
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
            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.SortingOrder).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));

        }

        #endregion


        protected void rvDesignation_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        //[WebMethod]
        //public static string GetJobGradeByDesignaitonId(int id)
        //{
        //    ReportBase dbContext = new ReportBase();         
        //    var items = (from de in dbContext.context.PRM_Designation
        //                 join JG in dbContext.context.PRM_JobGrade on de.GradeId equals JG.Id
        //                 where de.Id == id
        //                 select new
        //                 {
        //                     GradeId = de.GradeId,
        //                     GradeName = JG.GradeName
        //                 }).FirstOrDefault();

        //    return new JavaScriptSerializer().Serialize(items);
        //}
    }
}