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
    public partial class RptFinallyApprovedApplicant : ReportBase
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

            var refNoId = Convert.ToInt32(ddlAdvertisement.SelectedValue);
            var designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
            GenerateReport(refNoId, designationId, strZoneId);
        }
        #endregion

        #region Generate Report
        private void GenerateReport(int refNoId, int designationId, string strZoneId)
        {
            rvFinallyApprovedApplicant.Reset();
            rvFinallyApprovedApplicant.ProcessingMode = ProcessingMode.Local;
            rvFinallyApprovedApplicant.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptFinallyApprovedApplicant.rdlc");

            #region Processing Report Data

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var data = (from s in base.context.SP_PRM_RptFinallyApprovedApplicant(refNoId, designationId, strZoneId, numErrorCode, strErrorMsg) select s).ToList();
            data.ToList();
            #endregion

            #region Search parameter
            string searchParameters = string.Empty;
            string searchParameters1 = string.Empty;
            if (refNoId != 0)
            {
                string strRefNo = ddlAdvertisement.SelectedItem.Text;
                searchParameters = "Advertisement Code  : " + strRefNo;
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
            rvFinallyApprovedApplicant.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            #endregion

            rvFinallyApprovedApplicant.LocalReport.DataSources.Add(new ReportDataSource("DSFinallyApprovedApplicantRpt", data));
            this.rvFinallyApprovedApplicant.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFinallyApprovedApplicant.DataBind();
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
            ddlAdvertisement.DataSource = context.PRM_JobAdvertisementInfo.Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();
            ddlAdvertisement.DataValueField = "Id";
            ddlAdvertisement.DataTextField = "AdCode";
            ddlAdvertisement.DataBind();
            ddlAdvertisement.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }

        #endregion

        protected void rvFinallyApprovedApplicant_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        [WebMethod]
        public static ArrayList FetchDesignation(int Id)
        {
            ReportBase dbContext = new ReportBase();
            ArrayList list = new ArrayList();

            var items = (from jobAd in dbContext.context.PRM_JobAdvertisementInfo
                         join jobAdDtl in dbContext.context.PRM_JobAdvertisementInfoDetailRequisition on jobAd.Id equals jobAdDtl.JobAdvertisementInfoId
                         join designation in dbContext.context.PRM_Designation on jobAdDtl.DesignationId equals designation.Id
                         where jobAd.Id == Id
                         select new
                         {
                             Id = designation.Id,
                             Name = designation.Name
                         }).Concat(from jobAd in dbContext.context.PRM_JobAdvertisementInfo
                                   join jobAdDtl in dbContext.context.PRM_JobAdvertisementPostDetail on jobAd.Id equals jobAdDtl.JobAdvertisementInfoId
                                   join designation in dbContext.context.PRM_Designation on jobAdDtl.DesignationId equals designation.Id
                                   where jobAd.Id == Id
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