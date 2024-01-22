using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptShortListedApplicant : ReportBase
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

            var refNoId = Convert.ToInt32(ddlRefNo.SelectedValue);
            var designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
            GenerateReport(refNoId, designationId,strZoneId);
        }
        #endregion

        #region Generate Report
        private void GenerateReport(int refNoId, int designationId, string ZoneInfoIds)
        {
            rvShortListedApplicant.Reset();
            rvShortListedApplicant.ProcessingMode = ProcessingMode.Local;
            rvShortListedApplicant.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptShortListedApplicant.rdlc");

            #region Processing Report Data
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var data = (from s in base.context.SP_PRM_RptShortListedApplicant(refNoId, designationId, ZoneInfoIds, numErrorCode, strErrorMsg) select s).OrderBy(o => o.ApplicationDate).ToList();
            data.ToList();
            #endregion

            #region Search parameter
            string searchParameters = string.Empty;
            string searchParameters1 = string.Empty;
            if (refNoId != 0)
            {
                string strRefNo = ddlRefNo.SelectedItem.Text;
                searchParameters = "Reference No.  : " + strRefNo;
            }

            if (designationId > 0)
            {
                string ddlDesig = ddlDesignation.SelectedItem.Text;
                searchParameters1 = "Position Name  : " + ddlDesig;
            }
            else
            {
                searchParameters1 = "Position Name  : All";
            }

            ReportParameter p1 = new ReportParameter("RefNo", searchParameters);
            ReportParameter p2 = new ReportParameter("DesignationName", searchParameters1);
            rvShortListedApplicant.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            #endregion

            rvShortListedApplicant.LocalReport.DataSources.Add(new ReportDataSource("DsShortListedApplicant", data));
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
            ddlRefNo.DataSource = context.PRM_ApplicantInterviewCardIssue.Where(q=>q.PRM_JobAdvertisementInfo.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.ReferenceNo).ToList();
            ddlRefNo.DataValueField = "Id";
            ddlRefNo.DataTextField = "ReferenceNo";
            ddlRefNo.DataBind();
            ddlRefNo.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }

        #endregion


        protected void rvShortListedApplicant_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        //[WebMethod]
        //public static string GetDesignationById(int Id)
        //{
        //    ReportBase dbContext = new ReportBase();
        //    var items = (from IvCardIss in dbContext.context.PRM_ApplicantInterviewCardIssue
        //                 join designation in dbContext.context.PRM_Designation on IvCardIss.DesignationId equals designation.Id
        //                 where IvCardIss.Id == Id
        //                 select new
        //                 {
        //                     Id = designation.Id,
        //                     Name = designation.Name
        //                 }).ToList();

        //    return new JavaScriptSerializer().Serialize(items);
        //}

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