using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity.Core.Objects;
using System.Collections;
using System.IO;

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptWorkWiseBudget : ReportBase
    {
        #region Fields

        bool checkStatus;
        private static List<vwPMIBudgetHead> _budgetHeadList = new List<vwPMIBudgetHead>();

        #endregion

        #region Ctor
        public RptWorkWiseBudget()
        {
            //
        }
        #endregion

        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                _budgetHeadList = pmiContext.vwPMIBudgetHead.ToList();
                PopulateDropdownList();
            }

        }

        #endregion

        #region Button Event

        int zoneId;
        string strBudgetHeadId;
        string strBudgetSubHeadId;
        string strFinancialYearId;

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;
                strBudgetHeadId = string.Empty;
                strBudgetSubHeadId = string.Empty;
                strFinancialYearId = string.Empty;
                zoneId = 0;

                foreach (ListItem item in ddlBudgetHead.Items)
                {
                    if (item.Selected)
                    {
                        if (!string.IsNullOrEmpty(strBudgetHeadId))
                        {
                            strBudgetHeadId = string.Concat(strBudgetHeadId, ",");
                        }
                        strBudgetHeadId = string.Concat(strBudgetHeadId, item.Value.ToString());

                    }
                }

                foreach (ListItem item in ddlBudgetSubHead.Items)
                {
                    if (item.Selected)
                    {
                        if (!string.IsNullOrEmpty(strBudgetSubHeadId))
                        {
                            strBudgetSubHeadId = string.Concat(strBudgetSubHeadId, ",");
                        }
                        strBudgetSubHeadId = string.Concat(strBudgetSubHeadId, item.Value.ToString());
                    }
                }

                foreach (ListItem item in ddlFinancialYear.Items)
                {
                    if (item.Selected)
                    {
                        if (!string.IsNullOrEmpty(strFinancialYearId))
                        {
                            strFinancialYearId = string.Concat(strFinancialYearId, ",");
                        }
                        strFinancialYearId = string.Concat(strFinancialYearId, item.Value.ToString());
                    }
                }
                zoneId = Convert.ToInt32(ddlBudgetZoneInformation.SelectedValue);

                GenerateReport(zoneId, strBudgetHeadId, strBudgetSubHeadId, strFinancialYearId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvWorkWiseBudget.Reset();
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int zoneId, string strBudgetHeadId, string strBudgetSubHeadId, string strFinancialYearId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data
            string zoneName = prmContext.PRM_ZoneInfo.Where(q => q.Id == zoneId).Select(q => q.ZoneName).FirstOrDefault();

            rvWorkWiseBudget.Reset();
            rvWorkWiseBudget.ProcessingMode = ProcessingMode.Local;

            rvWorkWiseBudget.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptWorkWiseBudget.rdlc");

            //var fy = hdnFinancialYear.Value;

            var data = pmiContext.sp_PMI_RptWorkWiseBudget(zoneId, strBudgetHeadId, strBudgetSubHeadId, strFinancialYearId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "...";
                //searchParameters = "Budget";// \n " + zoneName;
                ReportParameter p1 = new ReportParameter("param", searchParameters);

                rvWorkWiseBudget.LocalReport.SetParameters(new ReportParameter[] { p1 });

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsWorkWiseBudget", data);
                rvWorkWiseBudget.LocalReport.DataSources.Add(dataSource);
                this.rvWorkWiseBudget.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvWorkWiseBudget.Reset();
            }
            rvWorkWiseBudget.DataBind();

            //ExportToPDF
            String newFileName = "WorkWiseBudget_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvWorkWiseBudget, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                dynamic data = null;
                var dsName = string.Empty;
                switch (e.ReportPath)
                {
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
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region User Methods
        private void PopulateDropdownList()
        {
            var budgetHeadList = pmiContext.vwPMIBudgetHead.ToList();
            var parentHeadList = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().OfType<vwPMIBudgetHead>().ToList();
            var budgetSubHeadList = budgetHeadList.Where(q => q.ParentId != null).DefaultIfEmpty().OfType<vwPMIBudgetHead>().ToList();

            ddlBudgetZoneInformation.DataSource = GetZoneDDL();
            ddlBudgetZoneInformation.DataValueField = "Value";
            ddlBudgetZoneInformation.DataTextField = "Text";
            ddlBudgetZoneInformation.DataBind();
            ddlBudgetZoneInformation.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlBudgetHead.DataSource = parentHeadList;
            ddlBudgetHead.DataValueField = "Id";
            ddlBudgetHead.DataTextField = "BudgetHead";
            ddlBudgetHead.DataBind();
            //ddlBudgetHead.Items.Insert(0, new ListItem("[Select One]", "0"));


            //ddlStatus.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlFinancialYear.DataSource =  pmiContext.acc_Accounting_Period_Information.OrderByDescending(x => x.yearName).ToList();
            ddlFinancialYear.DataValueField = "Id";
            ddlFinancialYear.DataTextField = "yearName";
            ddlFinancialYear.DataBind();


        }
        #endregion

        protected void rvWorkWiseBudget_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }


        [System.Web.Services.WebMethod]
        public static ArrayList PopulateSubHeads(string headIds)
        {
            ArrayList list = new ArrayList();

            var budgetSubHeadList = _budgetHeadList.Where(q => q.ParentId != null).ToList();
            foreach (var item in budgetSubHeadList)
            {
                list.Add(new ListItem { Text = item.BudgetSubHead, Value = item.Id.ToString() });
            }
            return list;
        }


        protected void ddlBudgetHead_SelectedIndexChanged(object sender, EventArgs e)
        {
            var budgetHeadList = pmiContext.vwPMIBudgetHead.ToList();


            string budgetHeadId = string.Empty;
            foreach (ListItem item in ddlBudgetHead.Items)
            {
                if (item.Selected)
                {
                    if (!string.IsNullOrEmpty(budgetHeadId))
                    {
                        budgetHeadId = string.Concat(budgetHeadId, ",");
                    }
                    budgetHeadId = string.Concat(budgetHeadId, item.Value.ToString());
                }
            }
            var result = new List<int>();
            if (string.IsNullOrWhiteSpace(budgetHeadId))
            {
                result.Add(0);
            }
            else
            {
                result = budgetHeadId.Split(',').Select(x => int.Parse(x)).ToList();
            }

            var budgetSubHeadList = (from x in budgetHeadList
                                     join r in result on x.ParentId equals r
                                     select x).DefaultIfEmpty().OfType<vwPMIBudgetHead>().ToList();

            //var headIds = hdnBudgetHead.Value;
            ddlBudgetSubHead.DataSource = budgetSubHeadList;
            ddlBudgetSubHead.DataValueField = "Id";
            ddlBudgetSubHead.DataTextField = "BudgetSubHead";
            ddlBudgetSubHead.DataBind();

        }

    }
}