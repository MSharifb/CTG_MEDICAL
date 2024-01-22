using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class DynamicQueryAnalyzerController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly EmployeeService _empService;
        #endregion

        #region Ctor

        public DynamicQueryAnalyzerController(PRMCommonSevice pPRMCommonService, EmployeeService empService)
        {
            this._prmCommonService = pPRMCommonService;
            this._empService = empService;

        }

        #endregion

        public ActionResult Index()
        {
            var model = InitializeModel();
            return View(model);
        }

        private QueryAnalyzerViewModel InitializeModel()
        {
            QueryAnalyzerViewModel model = new QueryAnalyzerViewModel();
            model.ReportTitle = "Employee Information";
            model.ReportDate = DateTime.Now;

            model.QueryAnalyzerTableList = _prmCommonService.PRMUnit.QueryAnalyzerTableRepository.GetAll().OrderBy(q => q.SortOrder).ToList();
            model.QueryAnalyzerTableItemsList = _prmCommonService.PRMUnit.QueryAnalyzerTableItemsRepository.GetAll().OrderBy(q => q.SortOrder).ToList();
            model.ZoneListByUser = Common.PopulateDdlZoneList(MyAppSession.SelectedZoneList);
            model.ZoneListByUserId = LoggedUserZoneInfoId;
            return model;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Type)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var empTypeId = _empService.PRMUnit.EmploymentTypeRepository.GetAll().Where(q => q.Name == BEPZA_MEDICAL.Utility.PRMEnum.EmploymentType.Permanent.ToString()).FirstOrDefault().Id;

            if (Type == 1)
                filterExpression = "EmpTypeId !=" + empTypeId + "";
            else if (Type == 3 || Type == 4)
                filterExpression = "EmpTypeId =" + empTypeId + "";

            var list = _empService.GetPaged(
                filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                viewModel.EmployeeStatus,
                1,
                LoggedUserZoneInfoId,
                out totalRecords
                );

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.EmpName,
                    item.ID,
                    item.EmpId,
                    item.DesigName,
                    item.EmpTypeName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,

                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult EmployeeSearch(string Type)
        {
            var model = new EmployeeSearchViewModel();
            if (Type == "Active") /*RH#02*/
            {
                model.EmployeeStatus = 1;
            }
            else if (Type == "Inactive") /*RH#02*/
            {
                model.EmployeeStatus = 2;
            }
            else
            {
                model.EmployeeStatus = -1;
            }

            return View("EmployeeSearch", model);
        }

        [HttpPost]
        public ActionResult SelectionCriteria(QueryAnalyzerViewModel models)
        {
            if (models.QueryAnalyzerItemsList == null)
                models.QueryAnalyzerItemsList = new List<QueryAnalyzerItemsViewModel>();
            if (models.AddRemove == true)
            {
                QueryAnalyzerItemsViewModel items = new QueryAnalyzerItemsViewModel();
                items.CategoryColumn = models.Column;
                items.CategoryColumnName = models.ColumnName;
                items.CategoryTable = models.Table;
                items.CategoryTableName = models.TableName;

                models.QueryAnalyzerItemsList.Add(items);
            }
            else
            {
                QueryAnalyzerItemsViewModel items = models.QueryAnalyzerItemsList.ToList().Find(
                    c =>
                    c.CategoryTable == models.Table && c.CategoryTableName == models.TableName &&
                    c.CategoryColumn == models.Column && c.CategoryColumnName == models.ColumnName);

                int index = -1;
                for (int i = 0; i < models.QueryAnalyzerItemsList.Count; i++)
                {
                    if (models.QueryAnalyzerItemsList[i] == items)
                    {
                        index = i;
                    }
                }

                if (index > -1)
                {
                    models.QueryAnalyzerItemsList.Remove(models.QueryAnalyzerItemsList[index]);
                }

            }
            ModelState.Clear();

            return PartialView("_Selection", models);
        }

        [HttpPost]
        public ActionResult SelectionLogic(QueryAnalyzerViewModel models)
        {

            if (models.QueryAnalyzerLogicList == null)
                models.QueryAnalyzerLogicList = new List<QueryAnalyzerLogicViewModel>();
            if (models.AddRemove == true)
            {
                QueryAnalyzerLogicViewModel item = new QueryAnalyzerLogicViewModel();
                item.Item = models.Column;
                item.ItemName = models.ColumnName;
                item.TableName = models.Table;

                models.QueryAnalyzerLogicList.Add(item);
            }
            else
            {
                QueryAnalyzerLogicViewModel item = models.QueryAnalyzerLogicList.ToList().Find(c => c.Item == models.Column && c.ItemName == models.ColumnName && c.TableName == models.Table);
                models.QueryAnalyzerLogicList.Remove(item);
                ModelState.Clear();

            }

            ModelState.Clear();
            return PartialView("_Logic", models);
        }

        [HttpPost]
        public ActionResult ClearAll(QueryAnalyzerViewModel model)
        {
            model = InitializeModel();
            ModelState.Clear();
            return PartialView("_Details", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetReportData(QueryAnalyzerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            model = ConstructQuery(model);
            ModelState.Clear();
            return View("Preview", model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ProcessReportData(QueryAnalyzerViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Index");
            //}
            model = ConstructQuery(model);
            ModelState.Clear();
            return View("Preview", model);
        }

        //[NonAction]
        private QueryAnalyzerViewModel ConstructQuery(QueryAnalyzerViewModel model)
        {
            string query = string.Empty;
            string table = string.Empty;
            string column = string.Empty;
            string whereCluase = string.Empty;
            string previousTable = string.Empty;
            string sortBy = string.Empty;
            string sortColumn = string.Empty;
            string firstColumn = string.Empty;
            string strSQL = string.Empty;

            DateTime dtScodeStart = new DateTime();
            DateTime dtScopeEnd = new DateTime();


            string StrEmpStatus = model.EmploymentStatus;
            int employmentStatusId = -1;
            if (StrEmpStatus != "All" && StrEmpStatus != null)
            {
                employmentStatusId = _prmCommonService.PRMUnit.EmploymentStatusRepository.GetAll().Where(q => q.Name == StrEmpStatus).FirstOrDefault().Id;
            }


            model.PageSize = 100000;
            int currentPageIndex = model.Page.HasValue ? model.Page.Value - 1 : 0;
            int startIndex = currentPageIndex * model.PageSize + 1;
            int maxrows = model.PageSize;
            model.CurrentPageIndex = model.Page.HasValue ? model.Page.Value : 1;
            if (model.QueryAnalyzerItemsList != null)
            {
                foreach (var item in model.QueryAnalyzerItemsList)
                {

                    if (!table.Contains(item.CategoryTable))
                    {
                        if (table != "")
                        {
                            table += " left outer join " + item.CategoryTable;

                            if (item.CategoryTable.ToLower().Contains("prm_employmentInfo"))
                            {
                                table += " on prm_employmentInfo.Id = " + previousTable + ".EmployeeId";
                            }
                            else
                            {
                                if (table.ToLower().Contains("prm_employmentInfo"))
                                {
                                    table += " on prm_employmentInfo.Id = " + item.CategoryTable + ".EmployeeId";
                                }
                                else
                                {
                                    table += " on " + previousTable + ".EmployeeId = " + item.CategoryTable + ".EmployeeId";
                                }
                            }
                            //table += ","+ item.CategoryTable ;
                        }
                        else
                        {
                            table = item.CategoryTable;
                        }
                        previousTable = item.CategoryTable;

                    }

                    if (column != "")
                    {
                        string type = model.GetColumnType(item.CategoryTable, item.CategoryColumn);

                        if (type == "date")
                        {
                            column += "," + "convert(varchar(10)," + item.CategoryTable + "." + item.CategoryColumn + ",105)" + " as [" + item.CategoryColumnName + "]";
                        }
                        else
                        {
                            column += "," + item.CategoryTable + "." + item.CategoryColumn + " as [" + item.CategoryColumnName + "]";
                        }
                    }
                    else
                    {
                        string type = model.GetColumnType(item.CategoryTable, item.CategoryColumn);
                        if (type == "date")
                        {
                            column = "" + "convert(varchar(10)," + item.CategoryTable + "." + item.CategoryColumn + ",105)" + " as [" + item.CategoryColumnName + "]";
                        }
                        else
                            column = "" + item.CategoryTable + "." + item.CategoryColumn + " as [" + item.CategoryColumnName + "]";

                        firstColumn = "[" + item.CategoryColumnName + "]";
                    }
                }

                query += " select distinct " + column + " from " + table + " where 0 = 0 ";
                strSQL = " from " + table + " where 0 = 0 ";
            }

            if (model.QueryAnalyzerLogicList != null)
            {
                string strStratDate = string.Empty;
                string strScopeEnd = string.Empty;
                foreach (var item in model.QueryAnalyzerLogicList)
                {
                    string type = model.GetColumnType(item.TableName, item.Item);
                    if (type == "date")
                    {
                        dtScodeStart = Common.FormatDateforSQL(item.ScodeStart);
                        strStratDate = dtScodeStart.ToString("yyyy-MM-dd");
                    }
                    if (type == "date" && item.ScopeLogic != "=")
                    {
                        dtScopeEnd = Common.FormatDateforSQL(item.ScopeEnd);
                        strScopeEnd = dtScopeEnd.ToString("yyyy-MM-dd");
                    }
                    if (type == "date" && item.ScopeLogic != "Between")
                    {
                        dtScopeEnd = Common.FormatDateforSQL(item.ScopeEnd);
                        strScopeEnd = dtScopeEnd.ToString("yyyy-MM-dd");
                    }
                    if (type == "date" && item.ScopeLogic != "Or")
                    {
                        dtScopeEnd = Common.FormatDateforSQL(item.ScopeEnd);
                        strScopeEnd = dtScopeEnd.ToString("yyyy-MM-dd");
                    }


                    ///implement logic.
                    //  if (item.ScopeLogic == "AND")
                    //  {
                    //      if (type == "date")
                    //      {
                    //          whereCluase += " and (" + item.TableName + "." + item.Item + " " + " =  '" + strStratDate + "' " + item.ScopeLogic + "  " + item.TableName + "." + item.Item + " =  " + " '" + strScopeEnd + "' )";
                    //      }
                    //      else
                    //          whereCluase += " and (" + item.TableName + "." + item.Item + " " + " =  '" + item.ScodeStart + "' " + item.ScopeLogic + "  " + item.TableName + "." + item.Item + " =  " + " '" + item.ScopeEnd + "' )";
                    //  }
                    //else
                    if (item.ScopeLogic == "Or")
                    {
                        if (type == "date")
                        {
                            whereCluase += " and (" + item.TableName + "." + item.Item + " " + " =  '" + strStratDate + "' " + item.ScopeLogic + "  " + item.TableName + "." + item.Item + " =  " + " '" + strScopeEnd + "' )";
                        }
                        else
                            whereCluase += " and (" + item.TableName + "." + item.Item + " " + " =  '" + item.ScodeStart + "' " + item.ScopeLogic + "  " + item.TableName + "." + item.Item + " =  " + " '" + item.ScopeEnd + "' )";
                    }
                    else if (item.ScopeLogic == "Between")
                    {
                        if (type == "date")
                        {
                            whereCluase += " and " + item.TableName + "." + item.Item + "" + " " + item.ScopeLogic + " '" + strStratDate + "" + "'  and " + " '" + strScopeEnd + "" + "' ";
                        }
                        else
                            whereCluase += " and " + item.TableName + "." + item.Item + " " + item.ScopeLogic + " '" + item.ScodeStart + "'  and '" + item.ScopeEnd + "' ";
                    }
                    else
                    {
                        if (type == "date")
                            whereCluase += " and " + item.TableName + "." + item.Item + " " + item.ScopeLogic + " '" + strStratDate + "'";
                        else
                            whereCluase += " and " + item.TableName + "." + item.Item + " " + item.ScopeLogic + " '" + item.ScodeStart + "'";
                    }


                    if (item.SortType != null)
                        if (item.SortType.Length > 0)
                        {
                            if (item.SortType == "Ascending")
                                sortBy = (item.SortType == "Ascending") ? " Asc " : " Asc ";
                            else
                                sortBy = " Desc ";
                            if (type == "date")
                                sortColumn += "convert(datetime,[" + item.ItemName + "],126),";
                            else
                                sortColumn += "[" + item.ItemName + "],";
                        }
                }
            }
            if (model.ZoneListByUserId != null)
            {
                whereCluase += " and ISNULL(PRM_VIEW_EmploymentInfo.ZoneInfoId,0) = " + model.ZoneListByUserId;
            }
            else
            {
                whereCluase += " and PRM_VIEW_EmploymentInfo.ZoneInfoId IS NOT NULL";
            }

            if (model.OrganogramLevelId != 0)
            {
                whereCluase += " and isnull(PRM_VIEW_EmploymentInfo.OrganogramLevelId,0) = " + model.OrganogramLevelId;
            }

            if (StrEmpStatus != "All" && StrEmpStatus != null)
            {
                whereCluase += " and isnull(PRM_VIEW_EmploymentInfo.EmploymentStatusId,0) = " + employmentStatusId;
            }

            if (model.EmployeeId != 0)
            {
                whereCluase += "and isnull(PRM_VIEW_EmploymentInfo.EmployeeId,0) = " + model.EmployeeId;
            }

            query += " " + whereCluase;
            strSQL += " " + whereCluase;

            if (sortBy.Trim().Length < 1)
            {
                sortColumn = firstColumn;
                sortBy = " Asc ";
            }

            //string strSQL = "WITH tmpEmp AS (SELECT ROW_NUMBER() OVER(Order by " + firstColumn + " desc ) AS ROWID, * FROM(" + query + " ) as tmp )  SELECT * FROM tmpEmp WHERE ROWID BETWEEN " + startIndex + " and "+maxrows;
            int totalRows;
            if (sortColumn.Contains(","))
                sortColumn = sortColumn.Substring(0, sortColumn.Length - 1);

            if (startIndex > 1)
            {
                sortColumn = Session["sortColumn"] == null ? "" : Session["sortColumn"].ToString();
                sortBy = Session["sortBy"] == null ? "" : Session["sortBy"].ToString();
            }

            model.DtReport = DynamicQueryAnalyzerRepository.GetQueryData(query, strSQL, sortColumn, sortBy, startIndex, maxrows, out totalRows);

            Session["sortColumn"] = sortColumn;
            Session["sortBy"] = sortBy;

            makeTextForExcelExport(model, model.DtReport);
            foreach (System.Data.DataColumn dc in model.DtReport.Columns)
            {
                if (dc.Caption == "ROWID")
                {
                    dc.Caption = "Sl No";
                    dc.ColumnName = "Sl No";
                }
                if (dc.DataType.Name == "Decimal")
                {
                    foreach (DataRow dr in model.DtReport.Rows)
                    {
                        if (dr[dc.ColumnName] != DBNull.Value)
                        {
                            dr[dc.ColumnName] = Math.Round(Convert.ToDecimal(dr[dc.ColumnName]), 2);
                        }
                    }
                }

            }

            model.TotalRecordCount = totalRows;
            return model;
        }


        private void makeTextForExcelExport(QueryAnalyzerViewModel model, DataTable dt)
        {
            StringBuilder contentBuilder = new StringBuilder();
            contentBuilder.Append("<div style='padding: 2px;width: 100%;float: left;font-size: 12px;text-decoration: none;font-weight: bold;Text-align:center;'>Bangladesh Export Processing Zones Authority</div>");
            contentBuilder.Append("<div style='padding: 2px;width: 100%;float: left;font-size: 12px;text-decoration: none;font-weight: bold;Text-align:center;'>BEPZA COMPLEX, HOUSE: 19/D, ROAD: 6, DHANMONDI R/A</div>");
            contentBuilder.Append("<div style='padding: 2px;width: 100%;float: left;font-size: 12px;text-decoration: none;font-weight: bold;Text-align:center;'>DHAKA, BANGLADESH.</div>");
            contentBuilder.Append("<div class='border'></div>");
            contentBuilder.Append("<div class='border'><b>Report Title:</b> " + model.ReportTitle + "</div>");
            contentBuilder.Append("<div class='border'><b>Report Date:</b> " + model.ReportDate.ToString("dd-MM-yyyy") + "</div>");
            contentBuilder.Append("<div class='border'></div>");
            contentBuilder.Append("<div style='padding:2px 5px 5px'></div>");
            contentBuilder.Append("<table id='tblHead' style='width: 100%;' border='1'>");
            foreach (System.Data.DataColumn dc in dt.Columns)
            {
                if (dc.Caption == "ROWID")
                {
                    dc.Caption = "Sl No";
                    dc.ColumnName = "Sl No";
                }

                contentBuilder.Append("<th>");
                contentBuilder.Append("" + dc.ColumnName + "</th>");

            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                contentBuilder.Append("<tr>");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //contentBuilder.Append("<td align='left'>");
                    contentBuilder.Append("<td style = mso-number-format:\\@>");
                    contentBuilder.Append("" + dt.Rows[i][j].ToString() + "</td>");
                }
                contentBuilder.Append("</tr>");
            }

            contentBuilder.Append("</table>");
            model.strHtml = contentBuilder.ToString();

        }


        #region Oraganogram Level Tree

        public ActionResult OrganogramLevelTreeSearchList(int zoneId)
        {
            QueryAnalyzerViewModel model = new QueryAnalyzerViewModel();
            if (zoneId < 0)
            {
                return View("Index", model);
            }
            ViewBag.ZoneInfoId = zoneId;
            return PartialView("_ZoneWiseOrganogramLevelTree");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetOrganogramTreeData(int zoneId)
        {
            var nodes = new List<JsTreeModel>();
            var parentNodes = _prmCommonService.PRMUnit.OrganogramLevelRepository.GetAll().Where(q => q.ZoneInfoId == null || q.ZoneInfoId == zoneId).ToList();
            var parentNode = parentNodes.Where(x => x.ParentId == 0).FirstOrDefault();
            nodes.Add(new JsTreeModel() { id = parentNode.Id.ToString(), parent = "#", text = parentNode.LevelName });
            var childs = _prmCommonService.PRMUnit.OrganogramLevelRepository.Get(q => q.ParentId == parentNode.Id && q.ZoneInfoId == zoneId).ToList();
            foreach (var item in childs)
            {
                nodes.Add(new JsTreeModel() { id = item.Id.ToString(), parent = item.ParentId.ToString(), text = GenerateOrganogramLevelNodeText(item).ToString() });
                AddOrganogramLevelChild(nodes, item);
            }
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        private void AddOrganogramLevelChild(List<JsTreeModel> nodes, PRM_OrganogramLevel item)
        {
            var childs = _prmCommonService.PRMUnit.OrganogramLevelRepository.Get(t => t.ParentId == item.Id).DefaultIfEmpty().OfType<PRM_OrganogramLevel>().ToList();

            if (childs != null && childs.Count > 0)
            {
                foreach (var anChild in childs)
                {
                    nodes.Add(new JsTreeModel() { id = anChild.Id.ToString(), parent = anChild.ParentId.ToString(), text = GenerateOrganogramLevelNodeText(anChild).ToString() });
                    AddOrganogramLevelChild(nodes, anChild);
                }
            }
        }

        private static StringBuilder GenerateOrganogramLevelNodeText(PRM_OrganogramLevel parentNode)
        {
            StringBuilder lvlName = new StringBuilder();
            lvlName.Append(parentNode.LevelName);

            if (parentNode.PRM_OrganogramType != null)
            {
                lvlName.Append(" [");
                lvlName.Append(parentNode.PRM_OrganogramType.Name);
                lvlName.Append("]");
            }
            return lvlName;
        }

        public void PopulateOrganogramLevelTree(PRM_OrganogramLevel parentNode, JsTreeNode jsTNode, List<PRM_OrganogramLevel> nodes)
        {
            StringBuilder nodeText = new StringBuilder();
            jsTNode.children = new List<JsTreeNode>();
            foreach (var dr in nodes)
            {
                if (dr != null)
                {
                    if (dr.ParentId == parentNode.Id)
                    {
                        JsTreeNode cnode = new JsTreeNode();
                        cnode.attr = new Attributes();
                        cnode.attr.id = Convert.ToString(dr.Id);
                        cnode.attr.rel = "folder" + dr.Id;
                        cnode.data = new Data();
                        nodeText = GenerateOrganogramLevelNodeText(dr);
                        cnode.data.title = Convert.ToString(nodeText);

                        cnode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";

                        jsTNode.children.Add(cnode);
                        PopulateOrganogramLevelTree(dr, cnode, nodes);
                    }
                }
            }
        }

        #endregion
    }
}