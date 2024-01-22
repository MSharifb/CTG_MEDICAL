using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using BEPZA_MEDICAL.Web.Resources;
using System.Web.Script.Serialization;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class ItemIssueController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly EmployeeService _empService;
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public ItemIssueController(PRMCommonSevice prmCommonService, INVCommonService invCommonService, EmployeeService empService)
        {
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
            this._empService = empService;
        }
        #endregion

        #region Actions

        public ActionResult RequisitionSearch()
        {
            var model = new ItemIssueSearchViewModel();

            return View("RequisitionSearch",model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetRequisitionList(JqGridRequest request, ItemIssueSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = new List<sp_INV_PendingRequisitionList_Result> ();

            using (var invContext = new ERP_BEPZAINVEntities())
            {
                list = invContext.sp_INV_PendingRequisitionList(LoggedUserZoneInfoId, 0).DistinctBy(x=> x.IndentNo).ToList();
            }

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.IndentNo))
                {
                    list = list.Where(x => x.IndentNo.Contains(viewModel.IndentNo)).ToList();
                } 
            }

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                string FullName = string.Empty;
                int Id = 0;

                FullName = d.EmpID + "-" + d.FullName;
                Id = d.IndentId;

                response.Records.Add(new JqGridRecord(Convert.ToString(Id), new List<object>()
                {
                    Id,
                    d.IndentNo,
                    d.IndentDate.ToString(DateAndTime.GlobalDateFormat),
                    FullName
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ItemIssueSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _invCommonService.INVUnit.IssueInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrWhiteSpace(viewModel.IssuedTo))
                {
                    var issuedToIdList = new List<Int32>();
                    var issuedToEmpList = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.EmpID.Contains(viewModel.IssuedTo) || x.FullName.ToLower().Contains(viewModel.IssuedTo.Trim().ToLower())).ToList();
                    foreach (var item in issuedToEmpList)
                    {
                        issuedToIdList.Add(item.Id);
                    }
                    list = list.Where(x => issuedToIdList.Contains(x.IssuedToId)).ToList();
                }

                if (!String.IsNullOrWhiteSpace(viewModel.IssuedBy))
                {
                    var issuedByIdList = new List<Int32>();
                    var issuedByEmpList = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.EmpID.Contains(viewModel.IssuedBy) || x.FullName.ToLower().Contains(viewModel.IssuedBy.Trim().ToLower())).ToList();
                    foreach (var item in issuedByEmpList)
                    {
                        issuedByIdList.Add(item.Id);
                    }
                    list = list.Where(x => issuedByIdList.Contains(x.IssuedById)).ToList();
                }
                if (viewModel.IssueDateFrom != null && viewModel.IssueDateTo != null)
                {
                    list = list.Where(x => x.IssueDate >= viewModel.IssueDateFrom && x.IssueDate <= viewModel.IssueDateTo).ToList();
                }

                if (viewModel.IndentDateFrom != null && viewModel.IndentDateTo != null)
                {
                    list = list.Where(x => x.IndentDate >= viewModel.IndentDateFrom && x.IndentDate <= viewModel.IndentDateTo).ToList();
                }

            }

            totalRecords = list == null ? 0 : list.Count;
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                var objIssuedTo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == d.IssuedToId).FirstOrDefault();
                var objIssuedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == d.IssuedById).FirstOrDefault();

                var IssuedTo = objIssuedTo.EmpID + " - " + objIssuedTo.FullName;
                var IssuedBy = objIssuedBy.EmpID + " - " + objIssuedBy.FullName;

                var IssueDate = d.IssueDate.ToString("dd-MMM-yyyy");
                var IndentDate = d.IndentDate != null ? d.IndentDate.Value.ToString("dd-MMM-yyyy") : "";

                var IndentNo = d.INV_RequisitionInfo.IndentNo;

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    viewModel.IssueDateFrom,
                    viewModel.IssueDateTo,
                    viewModel.IndentDateFrom,
                    viewModel.IndentDateTo,
                    d.IssueNo,
                    IndentNo,
                    IssueDate,
                    IndentDate,
                    IssuedTo,
                    IssuedBy,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ItemIssueViewModel model = new ItemIssueViewModel();

            GenerateIssueNo(model);
            PopulateList(model);
            return View(model);
        }

        public ActionResult PopulateCreate(int id)
        {
            ItemIssueViewModel model = new ItemIssueViewModel();

            GenerateIssueNo(model);

            var list = new List<sp_INV_PendingRequisitionList_Result>();

            using (var invContext = new ERP_BEPZAINVEntities())
            {
                list = invContext.sp_INV_PendingRequisitionList(LoggedUserZoneInfoId, id).ToList();
            }

            if(list.Count() > 0)
            {
                var reqInfo = list.FirstOrDefault();
                var reqItemList = _invCommonService.INVUnit.RequisitionItemRepository.GetAll().Where(x => x.RequisitionId == id).ToList();

                model.IndentId = reqInfo.IndentId;
                model.IndentDate = reqInfo.IndentDate;
                model.IssuedToId = reqInfo.IssuedToEmpId;
                model.IssuedTo = string.Concat(reqInfo.EmpID, " - ", reqInfo.FullName);

                var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
                if (loginUser != null)
                {
                    model.IssuedById = loginUser.ID;
                    model.IssuedBy = string.Concat(loginUser.EmpId, " - ", loginUser.EmpName);
                }

                foreach(var item in list)
                {
                    var itemIssueDetail = new ItemIssueDetailViewModel();
                    var reqItem = reqItemList.Where(x => x.ItemId == item.ItemId).FirstOrDefault();

                    itemIssueDetail.ItemId = item.ItemId;
                    itemIssueDetail.IssueQuantity = Convert.ToDecimal(item.Quantity);
                    itemIssueDetail.DemandQuantity = reqItem.Quantity;
                    itemIssueDetail.Item = reqItem.INV_ItemInfo.ItemName;
                    itemIssueDetail.Unit = reqItem.INV_ItemInfo.INV_Unit == null ? string.Empty : reqItem.INV_ItemInfo.INV_Unit.Name;

                    model.IssueItemDetail.Add(itemIssueDetail);
                }
            }

            PopulateList(model);
            model.ActionType = @"Create";
            return View("Create",model);
        }

        [HttpPost]
        public ActionResult Create(ItemIssueViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ActionType == "Create")
                    {
                        model.Id = 0;
                    }
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        model.ZoneInfoId = LoggedUserZoneInfoId;

                        var master = model.ToEntity();

                        foreach (var c in model.IssueItemDetail)
                        {
                            master.INV_IssueItem.Add
                            (new INV_IssueItem
                            {
                                ItemId = c.ItemId,
                                DemandQuantity = c.DemandQuantity,
                                IssueQuantity = c.IssueQuantity,
                                PurchaseId = c.PurchaseId,
                                Comment = c.Comment
                            }
                            );
                        }

                        _invCommonService.INVUnit.IssueInfoRepository.Add(master);
                        _invCommonService.INVUnit.IssueInfoRepository.SaveChanges();

                        if(master.Id > 0)
                        {
                            INV_RequisitionInfo reqInfo = _invCommonService.INVUnit.RequisitionInfoRepository.GetByID(master.IndentId);
                            reqInfo.IsProcessed = true;
                            _invCommonService.INVUnit.RequisitionInfoRepository.Update(reqInfo);
                            _invCommonService.INVUnit.RequisitionInfoRepository.SaveChanges();
                        }

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = checkoutBusinessLogic;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                    }
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsSuccessful = false;
            }

            PopulateList(model);
            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var master = _invCommonService.INVUnit.IssueInfoRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";

            if (master.INV_IssueItem != null)
            {
                model.IssueItemDetail = new List<ItemIssueDetailViewModel>();

                foreach (var item in master.INV_IssueItem)
                {
                    var itemDetail = item.ToModel();
                    itemDetail.Item = item.INV_ItemInfo.ItemName;
                    itemDetail.Unit = item.INV_ItemInfo.INV_Unit == null ? string.Empty : item.INV_ItemInfo.INV_Unit.Name;

                    model.IssueItemDetail.Add(itemDetail);

                }
            }

            var issuedToEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.IssuedToId);
            if (issuedToEmp != null)
            {
                model.IssuedTo = issuedToEmp.EmpID + " - " + issuedToEmp.FullName;

            }

            var issuedByEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.IssuedById);
            if (issuedByEmp != null)
            {
                model.IssuedBy = issuedByEmp.EmpID + " - " + issuedByEmp.FullName;

            }

            if (type == "success")
            {
                model.IsError = 0;
                model.errClass = "success";
                model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
            }
            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ItemIssueViewModel model)
        {
            var checkoutBusinessLogic = string.Empty;

            try
            {
                checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;

                        var master = model.ToEntity();
                        ArrayList arrtyList = new ArrayList();

                        if (model.IssueItemDetail != null)
                        {
                            foreach (var detail in model.IssueItemDetail)
                            {
                                var child = new INV_IssueItem()
                                {
                                    Id = detail.Id,
                                    IssueId = detail.IssueId,
                                    ItemId = detail.ItemId,
                                    DemandQuantity = detail.DemandQuantity,
                                    IssueQuantity = detail.IssueQuantity,
                                    PurchaseId = detail.PurchaseId,
                                    Comment = detail.Comment

                                };

                                // if old item then reflection will retrive old IUser & IDate
                                arrtyList.Add(child);
                            }
                        }

                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(INV_IssueItem), arrtyList);
                        _invCommonService.INVUnit.IssueInfoRepository.Update(master, NavigationList);
                        _invCommonService.INVUnit.IssueInfoRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        //return RedirectToAction("Index");

                        return RedirectToAction("Edit", new { id = master.Id, type = "success" });
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = checkoutBusinessLogic;
                    }
                }
                else
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.errClass = "failed";
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    model.ErrMsg = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.ErrMsg = ErrorMessages.UpdateFailed;
                }
            }

            PopulateList(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _invCommonService.INVUnit.IssueItemRepository.Delete(x => x.IssueId == id);
                _invCommonService.INVUnit.IssueItemRepository.SaveChanges();

                int indentId = _invCommonService.INVUnit.IssueInfoRepository.GetByID(id).IndentId;
                _invCommonService.INVUnit.IssueInfoRepository.Delete(id);
                _invCommonService.INVUnit.IssueInfoRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);

                var checkList = _invCommonService.INVUnit.IssueInfoRepository.GetByID(id);
                if(checkList == null)
                {
                    INV_RequisitionInfo reqInfo = _invCommonService.INVUnit.RequisitionInfoRepository.GetByID(indentId);
                    reqInfo.IsProcessed = false;
                    _invCommonService.INVUnit.RequisitionInfoRepository.Update(reqInfo);
                    _invCommonService.INVUnit.RequisitionInfoRepository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _invCommonService.INVUnit.IssueItemRepository.Delete(id);
                _invCommonService.INVUnit.IssueItemRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [HttpPost]
        public JsonResult Process(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                var entity = _invCommonService.INVUnit.RequisitionInfoRepository.GetByID(id);
                entity.IsProcessed = true;

                _invCommonService.INVUnit.RequisitionInfoRepository.Update(entity);
                _invCommonService.INVUnit.RequisitionInfoRepository.SaveChanges();

                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        #endregion

        #region Private Method

        private void GenerateIssueNo(ItemIssueViewModel model)
        {
            var maxIssueNo = 1;
            var newIssueNo = string.Empty;

            var zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().Where(x => x.Id == LoggedUserZoneInfoId).FirstOrDefault().ZoneCode.Trim();

            var issueInfo = _invCommonService.INVUnit.IssueInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x => x.IssueNo);
            if (issueInfo.Any())
            {
                maxIssueNo = Convert.ToInt32(issueInfo.FirstOrDefault().IssueNo.Replace(zoneCode, ""));
                maxIssueNo = maxIssueNo + 1;
            }

            newIssueNo = maxIssueNo.ToString();

            if (newIssueNo.Length < 7)
            {
                newIssueNo = newIssueNo.PadLeft(7, '0');
            }

            model.IssueNo = string.Concat(zoneCode, newIssueNo);

        }

        private string CheckingBusinessLogicValidation(ItemIssueViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                var issueInfo = _invCommonService.INVUnit.IssueInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();
                if (issueInfo.Where(x => x.IssueNo == model.IssueNo && x.Id != model.Id).Any())
                {
                    GenerateIssueNo(model);
                    message = model.Message = "Duplicate Issue#. Please try again.";
                    return message;
                }
            }

            if (model.IssueItemDetail == null || model.IssueItemDetail.Count() < 1)
            {
                message = model.Message = "Please add item(s) to issue.";
                return message;
            }
            else
            {
                var invContext = new ERP_BEPZAINVEntities();

                var issueItems = model.IssueItemDetail.GroupBy(x => x.ItemId)
                        .Select(i =>
                                new ItemIssueDetailViewModel
                                {
                                    ItemId = i.Key,
                                    IssueQuantity = i.Sum(s => s.IssueQuantity)
                                }).ToList();

                foreach (var item in issueItems)
                {
                    decimal closingBalance = 0;

                    var balanceInfo = invContext.fn_INV_GetItemClosingBalance(DateTime.Now, item.ItemId, LoggedUserZoneInfoId.ToString()).FirstOrDefault();

                    var existingIssuedIndent = _invCommonService.INVUnit.IssueItemRepository.GetAll().Where(x => x.ItemId == item.ItemId && x.INV_IssueInfo.IndentId == model.IndentId).GroupBy(x => x.ItemId)
                                                                .Select(i =>
                                                                        new
                                                                        {
                                                                            ItemId = i.Key,
                                                                            Quantity = i.Sum(s => s.IssueQuantity)
                                                                       }).FirstOrDefault();
                    var requisitionInfo = _invCommonService.INVUnit.RequisitionItemRepository.Fetch().Where(x => x.ItemId == item.ItemId && x.RequisitionId == model.IndentId).GroupBy(x => x.ItemId)
                        .Select(i =>
                                new
                                {
                                    ItemId = i.Key,
                                    DemandQuantity = i.Sum(s => s.Quantity)
                                }).FirstOrDefault();

                    if (balanceInfo != null)
                    {
                        closingBalance = balanceInfo.Balance;
                        if (model.Id == 0)
                        {
                            if (item.IssueQuantity > closingBalance)
                            {
                                message = model.Message = "Insufficient balance.";
                                return message;
                            }

                            if (existingIssuedIndent != null)
                            {
                                if (item.IssueQuantity > (requisitionInfo.DemandQuantity - existingIssuedIndent.Quantity))
                                {
                                    message = model.Message = "Item(s) already issued for the requisition.";
                                    return message;
                                }
                            }
                            else
                            {
                                if (item.IssueQuantity > requisitionInfo.DemandQuantity)
                                {
                                    message = model.Message = "Issue quantity must be less than or equal to demand quantity.";
                                    return message;
                                }
                            }
                        }
                        else
                        {
                            var existingItem = _invCommonService.INVUnit.IssueItemRepository.GetAll().Where(x => x.ItemId == item.ItemId && x.IssueId == model.Id).GroupBy(x => x.ItemId)
                                                                .Select(i =>
                                                                        new
                                                                        {
                                                                            ItemId = i.Key,
                                                                            Quantity = i.Sum(s => s.IssueQuantity)
                                                                       }).FirstOrDefault();

                            decimal existingItemBalance = 0;
                            if (existingItem != null)
                                existingItemBalance = existingItem.Quantity;

                            var tempBalance = closingBalance + existingItemBalance;
                            if (item.IssueQuantity > tempBalance)
                            {
                                message = model.Message = "Insufficient balance.";
                                return message;
                            }

                            if (existingIssuedIndent != null)
                            {
                                var existingIssuedIndentBalance = existingIssuedIndent.Quantity - existingItemBalance;

                                if (item.IssueQuantity > (requisitionInfo.DemandQuantity - existingIssuedIndentBalance))
                                {
                                    message = model.Message = "Item(s) already issued for the requisition.";
                                    return message;
                                }
                            }
                            else
                            {
                                if (item.IssueQuantity > requisitionInfo.DemandQuantity)
                                {
                                    message = model.Message = "Issue quantity must be less than or equal to demand quantity.";
                                    return message;
                                }
                            }

                        }
                    }
                    else
                    {
                        message = model.Message = "Insufficient balance.";
                        return message;
                    }
                }

                var itemSelectedMRR = model.IssueItemDetail.Where(x => x.PurchaseId != null && x.PurchaseId > 0).GroupBy(x => new { x.PurchaseId, x.ItemId })
                        .Select(m =>
                            new ItemIssueDetailViewModel
                            {
                                PurchaseId = m.Key.PurchaseId,
                                ItemId = m.Key.ItemId,
                                IssueQuantity = m.Sum(s => s.IssueQuantity)
                            }).ToList();

                foreach (var item in itemSelectedMRR)
                {
                    var balanceInfoByMRR = invContext.fn_INV_GetItemBalanceByMRR(DateTime.Now, item.ItemId, item.PurchaseId).FirstOrDefault();
                    if (balanceInfoByMRR != null)
                    {
                        var balanceByMRR = balanceInfoByMRR.Balance;
                        if (model.Id == 0)
                        {
                            if (item.IssueQuantity > balanceByMRR)
                            {
                                message = model.Message = "Issue quantity must be less than or equal to purchase quantity of selected MRR#.";
                                return message;
                            }
                        }
                        else
                        {
                            var existingItem = _invCommonService.INVUnit.IssueItemRepository.GetAll().Where(x => x.ItemId == item.ItemId && x.IssueId == model.Id && x.PurchaseId == item.PurchaseId).GroupBy(x => x.ItemId)
                                                                .Select(i =>
                                                                        new
                                                                        {
                                                                            ItemId = i.Key,
                                                                            Quantity = i.Sum(s => s.IssueQuantity)
                                                                        }).FirstOrDefault();

                            decimal existingItemBalance = 0;
                            if (existingItem != null)
                                existingItemBalance = existingItem.Quantity;

                            var tempBalance = balanceByMRR + existingItemBalance;
                            if (item.IssueQuantity > tempBalance)
                            {
                                message = model.Message = "Issue quantity must be less than or equal to purchase quantity of selected MRR#.";
                                return message;
                            }
                        }
                       
                    }
                    else 
                    {
                        message = model.Message = "Invalid MRR# selected.";
                        return message;
                    }

                }
                
            }

            return message;
        }

        private string AddItemValidation(ItemIssueDetailViewModel model)
        {
            string message = string.Empty;
            decimal closingBalance = 0;
            var invContext = new ERP_BEPZAINVEntities();
            if (model.RequisitionId > 0)
            {
                var balanceInfo = invContext.fn_INV_GetItemClosingBalance(DateTime.Now, model.ItemId, LoggedUserZoneInfoId.ToString()).FirstOrDefault();


                var existingIssuedIndent = _invCommonService.INVUnit.IssueItemRepository.GetAll().Where(x => x.ItemId == model.ItemId && x.INV_IssueInfo.IndentId == model.RequisitionId).GroupBy(x => x.ItemId)
                                                            .Select(i =>
                                                                    new
                                                                    {
                                                                        ItemId = i.Key,
                                                                        Quantity = i.Sum(s => s.IssueQuantity)
                                                                    }).FirstOrDefault();
                var requisitionInfo = _invCommonService.INVUnit.RequisitionItemRepository.Fetch().Where(x => x.ItemId == model.ItemId && x.RequisitionId == model.RequisitionId).GroupBy(x => x.ItemId)
                    .Select(i =>
                            new
                            {
                                ItemId = i.Key,
                                DemandQuantity = i.Sum(s => s.Quantity)
                            }).FirstOrDefault();

                if (balanceInfo != null)
                {
                    closingBalance = balanceInfo.Balance;
                    if (model.IssueId == 0)
                    {
                        if (model.IssueQuantity > (closingBalance - model.SumQty))
                        {
                            message = "Insufficient balance.";
                            return message;
                        }

                        if (existingIssuedIndent != null)
                        {
                            if (model.IssueQuantity > (requisitionInfo.DemandQuantity - (existingIssuedIndent.Quantity + model.SumQty)))
                            {
                                message = "Item(s) already issued for this requisition.";
                                return message;
                            }
                        }
                        else
                        {
                            if ((model.IssueQuantity + model.SumQty) > requisitionInfo.DemandQuantity)
                            {
                                message =  "Issue quantity must be less than or equal to demand quantity.";
                                return message;
                            }
                        }

                    }
                    else
                    {
                        var existingItem = _invCommonService.INVUnit.IssueItemRepository.GetAll().Where(x => x.ItemId == model.ItemId && x.IssueId == model.IssueId).GroupBy(x => x.ItemId)
                                                            .Select(i =>
                                                                    new
                                                                    {
                                                                        ItemId = i.Key,
                                                                        Quantity = i.Sum(s => s.IssueQuantity)
                                                                    }).FirstOrDefault();

                        decimal existingItemBalance = 0;
                        if (existingItem != null)
                            existingItemBalance = existingItem.Quantity;

                        var tempBalance = closingBalance + existingItemBalance - model.SumQty;
                        if (model.IssueQuantity > tempBalance)
                        {
                            message =  "Insufficient balance.";
                            return message;
                        }

                        if (existingIssuedIndent != null)
                        {
                            var existingIssuedIndentBalance = existingIssuedIndent.Quantity - existingItemBalance;

                            if (model.IssueQuantity > (requisitionInfo.DemandQuantity - (existingIssuedIndentBalance + model.SumQty)))
                            {
                                message =  "Item(s) already issued for this requisition.";
                                return message;
                            }
                        }
                        else
                        {
                            if ((model.IssueQuantity + model.SumQty) > requisitionInfo.DemandQuantity)
                            {
                                message =  "Issue quantity must be less than or equal to demand quantity.";
                                return message;
                            }
                        }

                    }
                }
                else
                {
                    message =  "Insufficient balance.";
                    return message;
                }

                #region purchase balance Check
                //////////////////////////////
                //if (model.PurchaseId != null)
                //{
                    
                //    var balanceInfoByMRR = invContext.fn_INV_GetItemBalanceByMRR(DateTime.Now, model.ItemId, model.PurchaseId).FirstOrDefault();
                //    if (balanceInfoByMRR != null)
                //    {
                //        var balanceByMRR = balanceInfoByMRR.Balance;
                //        if (model.IssueId == 0)
                //        {
                //            if (model.IssueQuantity > (balanceByMRR - model.SumQty))
                //            {
                //                message =  "Issue quantity must be less than or equal to purchase quantity of selected MRR#.";
                //                return message;
                //            }
                //        }
                //        else
                //        {
                //            var existingItem = _invCommonService.INVUnit.IssueItemRepository.GetAll().Where(x => x.ItemId == model.ItemId && x.IssueId == model.IssueId).GroupBy(x => x.ItemId)
                //                                                .Select(i =>
                //                                                        new
                //                                                        {
                //                                                            ItemId = i.Key,
                //                                                            Quantity = i.Sum(s => s.IssueQuantity)
                //                                                        }).FirstOrDefault();

                //            decimal existingItemBalance = 0;
                //            if (existingItem != null)
                //                existingItemBalance = existingItem.Quantity;

                //            var tempBalance = balanceByMRR + existingItemBalance - model.SumQty;
                //            if (model.IssueQuantity > tempBalance)
                //            {
                //                message = "Issue quantity must be less than or equal to purchase quantity of selected MRR#.";
                //                return message;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        message = "Invalid MRR# selected.";
                //        return message;
                //    }
                //}
                ///////////////////////////////////////////////////////////////
                #endregion
            }
            else 
            {
                message =  "Please select indent first.";
                return message;
            }
            return message;
        }
        private void PopulateList(ItemIssueViewModel model)
        {
            model.ItemTypeList = _invCommonService.INVUnit.ItemTypeRepository.GetAll().Where(x => x.ParentId != null).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemTypeName,
                    Value = y.Id.ToString()
                }).ToList();

            model.ItemList = _invCommonService.INVUnit.ItemInfoRepository.GetAll().ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemName,
                    Value = y.Id.ToString()
                }).ToList();
            
            if (model.IndentId != null && model.IndentId > 0)
            {
                var idList = new List<string>();
                var indentItems = _invCommonService.INVUnit.RequisitionItemRepository.GetAll().Where(x => x.RequisitionId == model.IndentId);
                foreach (var indentItem in indentItems)
                {
                    idList.Add(indentItem.ItemId.ToString());
                }
                model.ItemList = model.ItemList.Where(x => idList.Contains(x.Value)).ToList();
            }

            if (model.PurchaseId != null && model.PurchaseId > 0)
            {
                var idList = new List<string>();
                var purchaseItems = _invCommonService.INVUnit.PurchaseItemRepository.GetAll().Where(x => x.PurchaseId == model.PurchaseId);
                foreach (var purchaseItem in purchaseItems)
                {
                    idList.Add(purchaseItem.ItemId.ToString());
                }
                model.ItemList = model.ItemList.Where(x => idList.Contains(x.Value)).ToList();
            }

            //model.IndentList = _invCommonService.INVUnit.RequisitionInfoRepository.GetAll().Where(x=> x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x=> x.IndentNo).ToList()
            //    .Select(y =>
            //    new SelectListItem()
            //    {
            //        Text = y.IndentNo,
            //        Value = y.Id.ToString()
            //    }).ToList();

            model.IndentList =(from Req in _invCommonService.INVUnit.RequisitionInfoRepository.GetAll()
                               join Ap in _prmCommonService.PRMUnit.ApprovalStatusRepository.GetAll() on Req.ApprovalStatusId equals Ap.Id
                               where (Ap.StatusName.Contains("Approved"))
                               select Req
                               ).ToList()
                               .Select(y =>
                                new SelectListItem()
                                {
                                    Text = y.IndentNo,
                                    Value = y.Id.ToString()
                                }).ToList();
                              



            model.MRRList = _invCommonService.INVUnit.PurchaseInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.MRR).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.MRR,
                    Value = y.Id.ToString()
                }).ToList();

            model.CategoryList = Common.PopulateDllList(_invCommonService.INVUnit.CategoryRepository.GetAll().OrderBy(x => x.Name));
            model.ColorList = Common.PopulateDllList(_invCommonService.INVUnit.ColorRepository.GetAll().OrderBy(x => x.Name));
            model.ModelList = Common.PopulateDllList(_invCommonService.INVUnit.ModelRepository.GetAll().OrderBy(x => x.Name));
            model.UnitList = Common.PopulateDllList(_invCommonService.INVUnit.UnitRepository.GetAll().OrderBy(x => x.Name));

        }

        #endregion

        public ActionResult AddDetail(ItemIssueDetailViewModel model)
        {
            var master = new ItemIssueViewModel();
            master.IssueItemDetail = new List<ItemIssueDetailViewModel>();

            master.IssueItemDetail.Add(model);
            master.Message = AddItemValidation(model);

            if (master.Message !=string.Empty) {
                return Json(new
                {
                    Success=true,
                    Message = master.Message
                },JsonRequestBehavior.AllowGet);
            }

            return PartialView("_Details", master);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(ItemIssueViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var IssuedTo = string.Empty;
                    var IssuedToId = string.Empty;
                    var IssuedBy = string.Empty;
                    var IssuedById = string.Empty;

                    var objIssuedTo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.IssuedToId);
                    var objIssuedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.IssuedById);

                    if (objIssuedTo != null)
                    {
                        IssuedTo = objIssuedTo.EmpID + " - " + objIssuedTo.FullName;
                        IssuedToId = objIssuedTo.Id.ToString();
                    }
                    if (objIssuedBy != null)
                    {
                        IssuedBy = objIssuedBy.EmpID + " - " + objIssuedBy.FullName;
                        IssuedById = objIssuedBy.Id.ToString();
                    }
                    
                    return Json(new
                    {
                        IssuedTo = IssuedTo,
                        IssuedToId = IssuedToId,
                        IssuedBy = IssuedBy,
                        IssuedById = IssuedById

                    });
                    
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.Message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        return Json(new { Result = false });

                    }
                    else
                    {
                        model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        return Json(new { Result = false });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Result = msg
                });
            }
        }

        public JsonResult GetIndentDetails(int? IndentId)
        {
            string indentDate = string.Empty;
            string issuedToEmpId = string.Empty;
            string issuedTo = string.Empty;

            if (IndentId > 0)
            {
                var indentInfo = _invCommonService.INVUnit.RequisitionInfoRepository.GetByID(IndentId);
                indentDate = indentInfo.IndentDate.ToString("yyyy-MM-dd");
                issuedToEmpId = indentInfo.IssuedToEmpId.ToString();

                var objIssuedToEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(indentInfo.IssuedToEmpId);

                if (objIssuedToEmp != null)
                {
                    issuedTo = objIssuedToEmp.EmpID + " - " + objIssuedToEmp.FullName;
                }
            }

             return Json(new
                {
                    indentDate = indentDate,
                    issuedToEmpId = issuedToEmpId,
                    issuedTo = issuedTo
                }, JsonRequestBehavior.AllowGet);         
        }

        public JsonResult GetDemandQuantity(int? itemId, int? indentId)
        {
            string DemandQuantity = string.Empty;

            if (itemId > 0 && indentId > 0)
            {
                var requisitionInfo = _invCommonService.INVUnit.RequisitionItemRepository.GetAll().Where(x => x.ItemId == itemId && x.RequisitionId == indentId).FirstOrDefault();
                if (requisitionInfo != null)
                {
                    DemandQuantity = requisitionInfo.Quantity.ToString();
                }

            }

            return Json(new
            {
                DemandQuantity = DemandQuantity
            }, JsonRequestBehavior.AllowGet);
        }

        public string GetItemList(int? typeId, int? categoryId, int? modelId, int? unitId, int? colorId, int? indentId, int? purchaseId)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });
            var items = new List<INV_ItemInfo>();

            items = (from entity in _invCommonService.INVUnit.ItemInfoRepository.Fetch()
                     select entity).OrderBy(o => o.ItemName).ToList();

            if (indentId != null)
            { 
                var idList = new List<Int32>();
                var indentItems = _invCommonService.INVUnit.RequisitionItemRepository.GetAll().Where(x=> x.RequisitionId == indentId);
                foreach (var indentItem in indentItems)
                {
                    idList.Add(indentItem.ItemId);
                }
                items = items.Where(x => idList.Contains(x.Id)).ToList();
            }

            if (purchaseId != null)
            { 
                var idList = new List<Int32>();
                var purchaseItems = _invCommonService.INVUnit.PurchaseItemRepository.GetAll().Where(x => x.PurchaseId == purchaseId);
                foreach (var purchaseItem in purchaseItems)
                {
                    idList.Add(purchaseItem.ItemId);
                }
                items = items.Where(x => idList.Contains(x.Id)).ToList();
            }

            if (typeId != null)
            {
                int parent = typeId ?? 0;
                var typeIdList = new List<Int32>();

                typeIdList.Add(parent);
                AddChild(typeIdList, typeId);

                items = items.Where(x => typeIdList.Contains(x.TypeId ?? 0)).ToList();
            }

            if (categoryId != null)
            {
                items = items.Where(x => x.CategoryId == categoryId).ToList();
            }

            if (modelId != null)
            {
                items = items.Where(x => x.ModelId == modelId).ToList();
            }

            if (unitId != null)
            {
                items = items.Where(x => x.UnitId == unitId).ToList();
            }

            if (colorId != null)
            {
                items = items.Where(x => x.ColorId == colorId).ToList();
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.ItemName, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }

            return new JavaScriptSerializer().Serialize(listItems);
        }

        private void AddChild(List<Int32> typeIdList, int? typeId)
        {

            var childs = _invCommonService.INVUnit.ItemTypeRepository.Get(t => t.ParentId == typeId).Select(x => x.Id).ToList();

            if (childs != null && childs.Count > 0)
            {
                foreach (var anChild in childs)
                {
                    typeIdList.Add(anChild);
                    AddChild(typeIdList, anChild);
                }
            }
        }

        public string GetIndentList(int? issuedToId, string indentDate)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "[Select One]", Value = "" });
            var indents = new List<INV_RequisitionInfo>();

            indents = (from entity in _invCommonService.INVUnit.RequisitionInfoRepository.Fetch()
                     select entity).OrderByDescending(o => o.IndentNo).ToList();

            if (issuedToId != null && issuedToId > 0)
            {
                indents = indents.Where(x => x.IssuedToEmpId == issuedToId).ToList();
            }
            if (!string.IsNullOrEmpty(indentDate))
            {
                indents = indents.Where(x => x.IndentDate == Convert.ToDateTime(indentDate)).ToList();
            }

            if (indents != null)
            {
                foreach (var item in indents)
                {
                    var listItem = new SelectListItem { Text = item.IndentNo, Value = item.Id.ToString() };
                    list.Add(listItem);
                }
            }

            return new JavaScriptSerializer().Serialize(list);
        }

        
    }

}