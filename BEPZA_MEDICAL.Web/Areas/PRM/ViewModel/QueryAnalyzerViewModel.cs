using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using BEPZA_MEDICAL.Domain.PRM;
using System.ComponentModel.DataAnnotations.Schema;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class QueryAnalyzerViewModel
    {
        //private QueryAnalyzerService _queryAnalyzerService;
        #region Ctor
        public QueryAnalyzerViewModel()
        {
            this.QueryAnalyzerTableList = new List<PRM_QueryAnalyzerTable>();
            this.QueryAnalyzerTableItemsList = new List<PRM_QueryAnalyzerTableItems>();
            this.QueryAnalyzerItemsList = new List<QueryAnalyzerItemsViewModel>();
            this.QueryAnalyzerLogicList = new List<QueryAnalyzerLogicViewModel>();
            this.ZoneListByUser = new List<SelectListItem>();
            this.ScopeLogicList = Common.PopulateScopeLogic();
            this.SortTypeList = Common.PopulateSortType();
            //this._queryAnalyzerService = new QueryAnalyzerService();
            this.PageSize = 10;
            this.NumericPageCount = 5;
        }
        #endregion


        [NotMapped, DisplayName("Zone List By User")]
        public int? ZoneListByUserId { get; set; }
        public IList<SelectListItem> ZoneListByUser { set; get; }

        #region Report Info

        [DisplayName("Report Title")]
        public string ReportTitle { get; set; }

        [DisplayName("Report Date")]
        [UIHint("_Date")]
        public DateTime ReportDate { get; set; }

        #endregion

        #region Organogram Info
        public int OrganogramLevelId { get; set; }
        public string OrganogramLevelName { get; set; }

        public string LevelName { get; set; }
        #endregion

        #region Employee Info
        public string EmploymentStatus { get; set; }
        public bool IsIndividual { get; set; }
        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        public string EmpId { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }


        #endregion

        public string QueryAnalyzerTable { get; set; }
        public IList<PRM_QueryAnalyzerTable> QueryAnalyzerTableList { get; set; }

        public string QueryAnalyzerTableItems { get; set; }
        public IList<PRM_QueryAnalyzerTableItems> QueryAnalyzerTableItemsList { get; set; }

        public string TableName { get; set; }
        public string Table { get; set; }
        public string ColumnName { get; set; }
        public string Column { get; set; }
        public bool AddRemove { get; set; }

        #region QueryAnalyzer Items
        public List<QueryAnalyzerItemsViewModel> QueryAnalyzerItemsList { get; set; }
        #endregion

        #region QueryAnalyzer Logic
        public IList<QueryAnalyzerLogicViewModel> QueryAnalyzerLogicList { get; set; }
        #endregion

        #region Scope Logic
        public string ScopeLogic { get; set; }
        public IList<SelectListItem> ScopeLogicList { get; set; }

        #endregion

        #region Sort Type
        public IList<SelectListItem> SortTypeList { get; set; }

        #endregion
       
        
        public SelectList GetColumnData(string table, string column, string selectedValue)
        {

            SelectList list;
            List<SelectListItem> itemList = new List<SelectListItem>();

            string query = "Select distinct " + column + " from " + table;
            DataTable dt = DynamicQueryAnalyzerRepository.GetColumnData(query);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                SelectListItem item = new SelectListItem();
                if (table == "HRM_ViewLanguage" && column == "isNative")
                {
                    item.Text = (dt.Rows[i][0].ToString().ToLower() == "true") ? "Yes" : "No";
                    item.Value = (dt.Rows[i][0].ToString().ToLower() == "true") ? "1" : "0";
                    item.Selected = (selectedValue == dt.Rows[i][0].ToString()) ? true : false;
                    itemList.Add(item);
                }
                else
                {
                    item.Text = dt.Rows[i][0].ToString();
                    item.Value = dt.Rows[i][0].ToString();
                    item.Selected = (selectedValue == dt.Rows[i][0].ToString()) ? true : false;
                    itemList.Add(item);
                }
            }
            list = new SelectList(itemList, "Value", "Text", selectedValue);
            return list;

        }

        public string GetColumnType(string table, string column)
        {
            string returnString = "";
            try
            {
                string query = "Select distinct " + column + " from " + table;
                DataTable dt = DynamicQueryAnalyzerRepository.GetColumnData(query);
                foreach (System.Data.DataColumn dataColumn in dt.Columns)
                {
                    if (dataColumn.DataType == typeof(System.String))
                    {
                        returnString = "ddl";
                    }

                    else if (dataColumn.DataType == typeof(System.DateTime))
                    {
                        returnString = "date";
                    }

                    else if (dataColumn.DataType == typeof(System.Int32) || dataColumn.DataType == typeof(System.Decimal) || dataColumn.DataType == typeof(System.Double))
                    {
                        returnString = "txtbox";
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }

            return returnString;

        }

        public DataTable DtReport { get; set; }
        public string strHtml { get; set; }


        #region Pagination
        public int? Page { get; set; }
        public int? CurrentPage { get; set; }

        // Sorting-related properties
        public string SortBy { get; set; }
        public bool SortAscending { get; set; }
        public string SortExpression
        {
            get { return this.SortAscending ? this.SortBy + " asc" : this.SortBy + " desc"; }
        }
        public string SortDirection
        {
            get { return this.SortAscending ? " asc" : " desc"; }
        }

        // Paging-related properties
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
        public int PageCount
        {
            get { return (int)Math.Ceiling((decimal)this.TotalRecordCount / this.PageSize); }
        }
        public int NumericPageCount { get; set; }

        public string LoaderImage { get; set; }

        // common
        public bool? ascending { get; set; }
        public string Message { get; set; }
        public bool IsSuccessFailure { get; set; }
        public bool isViewable { get; set; }
        public bool IsUpdate { get; set; }
        #endregion
        
    }

}