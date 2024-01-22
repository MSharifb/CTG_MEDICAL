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
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Configuration;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.IO;

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptBudgetSummary : ReportBase
    {
        #region Fields

        bool checkStatus;
        public static List<BudgetSummaryParamViewModel> ReportParamList = ReportBase.BudgetSummaryParamList;

        public string connString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;



        #endregion

        #region Ctor
        public RptBudgetSummary()
        {
            //
        }
        #endregion

        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                dvControls.Visible = ReportBase.BudgetSummaryParamList != null && ReportBase.BudgetSummaryParamList.Count > 0 ? false : true;
                if (ReportBase.BudgetSummaryParamList != null && ReportBase.BudgetSummaryParamList.Count > 0)
                {
                    GenerateReport(strZoneId, budgetHeadId);
                }
                else
                {
                    SetInitialRow();
                }

                PopulateDropdownList();
            }
            dvControls.Visible = ReportBase.BudgetSummaryParamList != null && ReportBase.BudgetSummaryParamList.Count > 0 ? false : true;

        }

        #endregion

        #region Button Event

        private void FillFinancialYearList(DropDownList ddl)
        {
            var fyList = pmiContext.acc_Accounting_Period_Information.OrderByDescending(x => x.yearName).ToList();
            ddl.DataSource = fyList;
            ddl.DataValueField = "Id";
            ddl.DataTextField = "yearName";
            ddl.DataBind();
        }

        private void FillProjectStatus(DropDownList ddl)
        {
            var fyList = pmiContext.PMI_ProjectStatus.ToList();
            ddl.DataSource = fyList;
            ddl.DataValueField = "Id";
            ddl.DataTextField = "Name";
            ddl.DataBind();
        }

        private void SetInitialRow()
        {
            DataTable dt = new DataTable();
            DataRow dr = null;

            dt.Columns.Add(new System.Data.DataColumn("Column1", typeof(string)));//for DropDownList selected item 
            dt.Columns.Add(new System.Data.DataColumn("Column2", typeof(string)));//for DropDownList selected item 

            dr = dt.NewRow();
            dt.Rows.Add(dr);

            ViewState["CurrentTable"] = dt;

            Gridview1.DataSource = dt;
            Gridview1.DataBind();

            DropDownList ddl1 = (DropDownList)Gridview1.Rows[0].Cells[0].FindControl("DropDownList1");
            DropDownList ddl2 = (DropDownList)Gridview1.Rows[0].Cells[1].FindControl("DropDownList2");
            FillFinancialYearList(ddl1);
            FillProjectStatus(ddl2);
        }

        private void AddNewRowToGrid()
        {

            if (ViewState["CurrentTable"] != null)
            {

                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                DataRow drCurrentRow = null;

                if (dtCurrentTable.Rows.Count > 0)
                {
                    drCurrentRow = dtCurrentTable.NewRow();
                    dtCurrentTable.Rows.Add(drCurrentRow);

                    ViewState["CurrentTable"] = dtCurrentTable;

                    for (int i = 0; i < dtCurrentTable.Rows.Count - 1; i++)
                    {
                        DropDownList ddl1 = (DropDownList)Gridview1.Rows[i].Cells[0].FindControl("DropDownList1");
                        DropDownList ddl2 = (DropDownList)Gridview1.Rows[i].Cells[1].FindControl("DropDownList2");

                        dtCurrentTable.Rows[i]["Column1"] = ddl1.SelectedItem.Text;
                        dtCurrentTable.Rows[i]["Column2"] = ddl2.SelectedItem.Text;

                    }

                    Gridview1.DataSource = dtCurrentTable;
                    Gridview1.DataBind();
                }
            }
            else
            {
                Response.Write("ViewState is null");

            }
            SetPreviousData();
        }

        private void SetPreviousData()
        {

            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {

                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DropDownList ddl1 = (DropDownList)Gridview1.Rows[rowIndex].Cells[0].FindControl("DropDownList1");
                        DropDownList ddl2 = (DropDownList)Gridview1.Rows[rowIndex].Cells[1].FindControl("DropDownList2");

                        FillFinancialYearList(ddl1);
                        FillProjectStatus(ddl2);

                        if (i < dt.Rows.Count - 1)
                        {

                            ddl1.ClearSelection();
                            if (ddl1.Items.FindByText(dt.Rows[i]["Column1"].ToString()) != null)
                            {
                                ddl1.Items.FindByText(dt.Rows[i]["Column1"].ToString()).Selected = true;
                            }


                            ddl2.ClearSelection();
                            if (ddl2.Items.FindByText(dt.Rows[i]["Column2"].ToString()) != null)
                            {
                                ddl2.Items.FindByText(dt.Rows[i]["Column2"].ToString()).Selected = true;
                            }

                        }

                        rowIndex++;
                    }
                }
            }
        }
        string strZoneId = string.Empty;
        int budgetHeadId = 0;
        bool IsMultipleZone = false;
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;

                List<int> zoneList = new List<int>();
                int[] arrZoneList = new int[] { };
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                if (zoneList.Count > 1)
                    IsMultipleZone = true;

                arrZoneList = zoneList.ToArray();

                strZoneId = ConvertZoneArrayListToString(arrZoneList);

                budgetHeadId = Convert.ToInt32(ddlBudgetHead.SelectedValue);

                GenerateReport(strZoneId, budgetHeadId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvBudgetSummary.Reset();
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
        public void GenerateReport(string strZoneId, int budgetHeadId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            int rowIndex = 0;

            var paramList = new List<BudgetSummaryParamViewModel>();

            if (ViewState["CurrentTable"] != null)
            {

                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                if (dtCurrentTable.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    {
                        DropDownList ddl1 = (DropDownList)Gridview1.Rows[rowIndex].Cells[0].FindControl("DropDownList1");
                        DropDownList ddl2 = (DropDownList)Gridview1.Rows[rowIndex].Cells[1].FindControl("DropDownList2");

                        var param = new BudgetSummaryParamViewModel();
                        int fyId = 0, statusId = 0;
                        int.TryParse(ddl1.SelectedItem.Value, out fyId);
                        int.TryParse(ddl2.SelectedItem.Value, out statusId);

                        param.FinancialYearId = fyId;
                        param.ApprovalTypeId = statusId;
                        paramList.Add(param);
                        rowIndex++;
                    }

                }
            }

            #region Processing Report Data

            rvBudgetSummary.Reset();
            rvBudgetSummary.ProcessingMode = ProcessingMode.Local;
            int projectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            if(projectForId == 2)
            {
                if (IsMultipleZone)
                {
                    rvBudgetSummary.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptBudgetSummary_byZone.rdlc");
                }
                else
                {
                    rvBudgetSummary.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptBudgetSummary.rdlc");
                }
            }
            else
            {
                if (IsMultipleZone)
                {
                    rvBudgetSummary.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptBudgetSummary_Civil_byZone.rdlc");
                }
                else
                {
                    rvBudgetSummary.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptBudgetSummary_Civil.rdlc");
                }
            }

            DataTable dataTable = ExecuteProcedure(connString, paramList, strZoneId, budgetHeadId);

            var data = dataTable.AsEnumerable().Select(row => new
            {
                NameOfWorks = row.Field<string>("NameOfWorks"),
                BudgetAmount = row.Field<decimal>("BudgetAmount"),
                FinancialYearName = row.Field<string>("FinancialYearName"),
                Status = row.Field<string>("Status"),
                EstematedCost = row.Field<decimal>("EstematedCost"),
                PaidAmount = row.Field<decimal>("PaidAmount"),
                Liability = row.Field<decimal>("Liability"),
                Remarks = row.Field<string>("Remarks"),
                SerialNo = row.Field<int>("SerialNo"),
                BudgetHeadName = row.Field<string>("BudgetHeadName"),
                BudgetSubHead = row.Field<string>("BudgetSubHead"),
                BudgetDetailId = row.Field<int>("BudgetDetailId"),
                BudgetHeadSortOrder = row.Field<int>("BudgetHeadSortOrder"),
                BudgetSubHeadSortOrder = row.Field<int>("BudgetSubHeadSortOrder"),
                Subledger = row.Field<string>("Subledger"),
                SubledgerSortOrder = row.Field<int>("SubledgerSortOrder"),
                ZoneName = row.Field<string>("ZoneName"),
                ZoneSortingOrder = row.Field<int>("ZoneSortingOrder")

            });


            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = "Development/Non-Development Budget";
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                ReportParameter p2 = new ReportParameter("param1", searchParameters);

                rvBudgetSummary.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

                #endregion
                //if (!IsMultipleZone)
                //{
                    var Id = data.Select(x => x.BudgetDetailId).FirstOrDefault();
                    var budgetId = pmiContext.PMI_BudgetDetails.Where(s => s.Id == Id).Select(x => x.BudgetMasterId).FirstOrDefault();
                    var signature = pmiContext.SP_PMI_Signature(budgetId, "BGT").ToList();
                //}

                ReportDataSource dataSource = new ReportDataSource("dsBudgetSummary", data);
                ReportDataSource dataSource1 = new ReportDataSource("dsSignature", signature);

                rvBudgetSummary.LocalReport.DataSources.Add(dataSource);
                rvBudgetSummary.LocalReport.DataSources.Add(dataSource1);

                this.rvBudgetSummary.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvBudgetSummary.Reset();
            }
            rvBudgetSummary.DataBind();

            //ExportToPDF
            String newFileName = "BudgetSummary_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvBudgetSummary, newFileName, fs);

            #endregion
        }

        public void DatasetInsert(DataTable dt)
        {
            SqlConnection con = new SqlConnection(connString);
            con.Open();
            SqlCommand cmd = new SqlCommand("Usp_Vinsert", con);
            cmd.Parameters.AddWithValue("@UserDefinTable", dt);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private DataTable ExecuteProcedure(string connectionString, List<BudgetSummaryParamViewModel> paramList, string strZoneId, int budgetHeadId)
        {
            int projectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    try
                    {
                        using (var da = new SqlDataAdapter(command))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "dbo.sp_PMI_RptBudgetSummary";

                            SqlParameter parameter = new SqlParameter();
                            parameter.ParameterName = "@param";
                            parameter.Value = CreateDataTable(paramList);
                            parameter.SqlDbType = SqlDbType.Structured;
                            parameter.TypeName = "dbo.PMI_YearlyStatus";

                            SqlParameter parameter2 = new SqlParameter();
                            parameter2.ParameterName = "@zoneList";
                            parameter2.Value = strZoneId;
                            parameter2.SqlDbType = SqlDbType.VarChar;

                            SqlParameter parameter3 = new SqlParameter();
                            parameter3.ParameterName = "@ProjectForId";
                            parameter3.Value = projectForId;
                            parameter3.SqlDbType = SqlDbType.Int;

                            SqlParameter parameter4 = new SqlParameter();
                            parameter4.ParameterName = "@BudgetHeadId";
                            parameter4.Value = budgetHeadId;
                            parameter4.SqlDbType = SqlDbType.Int;

                            command.Parameters.Add(parameter);
                            command.Parameters.Add(parameter2);
                            command.Parameters.Add(parameter3);
                            command.Parameters.Add(parameter4);

                            da.Fill(table);
                        }
                    }
                    catch (Exception ex)
                    {
                        //
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }

            return table;
        }

        private static DataTable CreateDataTable(List<BudgetSummaryParamViewModel> paramList)
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("FinancialYearId");
            dt.Columns.Add("BudgetStatusId");

            foreach (var item in paramList)
            {
                DataRow r = dt.NewRow();
                r["FinancialYearId"] = item.FinancialYearId;
                r["BudgetStatusId"] = item.ApprovalTypeId;
                dt.Rows.Add(r);
            }

            return dt;
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                dynamic data = null;
                var dsName = string.Empty;
                switch (e.ReportPath)
                {
                    case "_LSRreportHeader":
                        data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                                select c).ToList();
                        dsName = "DSCompanyInfo";
                        break;

                    default:
                        break;
                }
                e.DataSources.Add(new ReportDataSource(dsName, data));
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region User Methods
        private void PopulateDropdownList()
        {
            //ddlFinancialYear.DataSource = famContext.FAM_FinancialYearInformation.OrderByDescending(x => x.FinancialYearName).ToList();
            //ddlFinancialYear.DataValueField = "Id";
            //ddlFinancialYear.DataTextField = "FinancialYearName";
            //ddlFinancialYear.DataBind();
            //ddlFinancialYear.Items.Insert(0, new ListItem("[Select One]", "0"));

            //ddlApprovalType.DataSource = pmiContext.vwPMIStatusInformation.Where(x => x.ApplicableFor == "Budget").ToList();
            //ddlApprovalType.DataValueField = "Id";
            //ddlApprovalType.DataTextField = "Name";
            //ddlApprovalType.DataBind();
            //ddlApprovalType.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlBudgetHead.DataSource = pmiContext.vwPMIBudgetHead.Where(x=>x.BudgetHead != string.Empty && x.BudgetHead != null).ToList();
            ddlBudgetHead.DataValueField = "Id";
            ddlBudgetHead.DataTextField = "BudgetHead";
            ddlBudgetHead.DataBind();
            ddlBudgetHead.Items.Insert(0, new ListItem("All", "0"));        
        }
        #endregion

        protected void rvBudgetSummary_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        protected void ButtonAdd_Click(object sender, EventArgs e)
        {
            AddNewRowToGrid();
        }

        protected void Gridview1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                LinkButton lb = (LinkButton)e.Row.FindControl("LinkButton1");
                if (lb != null)
                {
                    if (dt.Rows.Count > 1)
                    {
                        if (e.Row.RowIndex == dt.Rows.Count - 1)
                        {
                            lb.Visible = false;
                        }
                    }
                    else
                    {
                        lb.Visible = false;
                    }
                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            LinkButton lb = (LinkButton)sender;
            GridViewRow gvRow = (GridViewRow)lb.NamingContainer;
            int rowID = gvRow.RowIndex;
            if (ViewState["CurrentTable"] != null)
            {

                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 1)
                {
                    if (gvRow.RowIndex < dt.Rows.Count - 1)
                    {
                        dt.Rows.Remove(dt.Rows[rowID]);
                        ResetRowID(dt);
                    }
                }

                ViewState["CurrentTable"] = dt;
                Gridview1.DataSource = dt;
                Gridview1.DataBind();
            }

            SetPreviousData();
        }

        private void ResetRowID(DataTable dt)
        {
            int rowNumber = 1;
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    row[0] = rowNumber;
                    rowNumber++;
                }
            }
        }

    }
}