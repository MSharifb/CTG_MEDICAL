using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.IssueReturn;
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
    public class IssueReturnController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public IssueReturnController(PRMCommonSevice prmCommonService, INVCommonService invCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
        }
        #endregion

        #region Actions

        public ActionResult ScrapSearch()
        {
            var model = new IssueReturnSearchViewModel();

            return View("ScrapSearch", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetScrapList(JqGridRequest request, IssueReturnSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = new List<sp_INV_PendingScrapList_Result>();

            using (var invContext = new ERP_BEPZAINVEntities())
            {
                list = invContext.sp_INV_PendingScrapList(LoggedUserZoneInfoId, 0).DistinctBy(x => x.ScrapNo).ToList();
            }

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.ScrapNo))
                {
                    list = list.Where(x => x.ScrapNo.Contains(viewModel.ScrapNo)).ToList();
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
                Id = d.ScrapId;

                response.Records.Add(new JqGridRecord(Convert.ToString(Id), new List<object>()
                {
                    Id,
                    d.ScrapNo,
                    d.ScrapDate.ToString(DateAndTime.GlobalDateFormat),
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
        public ActionResult GetList(JqGridRequest request, IssueReturnSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _invCommonService.INVUnit.IssueReturnInfoRepository.GetCount(filterExpression);

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var list = _invCommonService.INVUnit.IssueReturnInfoRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrWhiteSpace(viewModel.ReceivedFrom))
                {
                    var receivedFromIdList = new List<Int32>();
                    var receivedFromEmpList = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.EmpID.Contains(viewModel.ReceivedFrom) || x.FullName.ToLower().Contains(viewModel.ReceivedFrom.Trim().ToLower())).ToList();
                    foreach (var item in receivedFromEmpList)
                    {
                        receivedFromIdList.Add(item.Id);
                    }
                    list = list.Where(x => receivedFromIdList.Contains(x.ReceivedFromEmpId)).ToList();

                }

                if (!String.IsNullOrWhiteSpace(viewModel.ReceivedBy))
                {
                    var receivedByIdList = new List<Int32>();
                    var receivedByEmpList = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.EmpID.Contains(viewModel.ReceivedBy) || x.FullName.ToLower().Contains(viewModel.ReceivedBy.Trim().ToLower())).ToList();
                    foreach (var item in receivedByEmpList)
                    {
                        receivedByIdList.Add(item.Id);
                    }
                    list = list.Where(x => receivedByIdList.Contains(x.ReceivedByEmpId)).ToList();
                }
                if (viewModel.ReturnDateFrom != null && viewModel.ReturnDateTo != null)
                {
                    list = list.Where(x => x.ReturnDate >= viewModel.ReturnDateFrom && x.ReturnDate <= viewModel.ReturnDateTo).ToList();
                }

            }

            foreach (var d in list)
            {
                var objReceivedFrom = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == d.ReceivedFromEmpId).FirstOrDefault();
                var objReceivedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == d.ReceivedByEmpId).FirstOrDefault();

                var ReceivedFrom = objReceivedFrom.EmpID + " - " + objReceivedFrom.FullName;
                var ReceivedBy = objReceivedBy.EmpID + " - " + objReceivedBy.FullName;

                var ReturnDate = d.ReturnDate.ToString("dd-MMM-yyyy");

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    viewModel.ReturnDateFrom,
                    viewModel.ReturnDateTo,
                    d.ReturnNo,
                    ReturnDate,
                    ReceivedFrom,
                    ReceivedBy,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            IssueReturnViewModel model = new IssueReturnViewModel();

            GenerateReturnNo(model);
            PopulateList(model);
            return View(model);
        }

        public ActionResult PopulateCreate(int id)
        {
            IssueReturnViewModel model = new IssueReturnViewModel();

            GenerateReturnNo(model);

            var list = new List<sp_INV_PendingScrapList_Result>();

            using (var invContext = new ERP_BEPZAINVEntities())
            {
                list = invContext.sp_INV_PendingScrapList(LoggedUserZoneInfoId, id).ToList();
            }

            if (list.Count() > 0)
            {
                var reqInfo = list.FirstOrDefault();
                var reqItemList = _invCommonService.INVUnit.ScrapItemRepository.GetAll().Where(x => x.ScrapId == id).ToList();

                model.ScrapId = reqInfo.ScrapId;
                //model.ScrapDate = reqInfo.ScrapDate;
                model.ReceivedFromEmpId = reqInfo.IssuedToEmpId;
                model.ReceivedFrom = string.Concat(reqInfo.EmpID, " - ", reqInfo.FullName);

                foreach (var item in list)
                {
                    var returnDetail = new IssueReturnDetailViewModel();
                    var reqItem = reqItemList.Where(x => x.ItemId == item.ItemId).FirstOrDefault();

                    returnDetail.ItemId = item.ItemId;
                    returnDetail.Quantity = Convert.ToDecimal(item.Quantity);
                    //returnDetail.DemandQuantity = reqItem.Quantity;
                    returnDetail.Item = reqItem.INV_ItemInfo.ItemName;
                    returnDetail.Unit = reqItem.INV_ItemInfo.INV_Unit == null ? string.Empty : reqItem.INV_ItemInfo.INV_Unit.Name;
                    returnDetail.ItemStatusId = reqItem.ItemStatusId;

                    model.IssueReturnItemDetail.Add(returnDetail);
                }
            }

            PopulateList(model);
            model.ActionType = @"Create";
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(IssueReturnViewModel model)
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

                        foreach (var c in model.IssueReturnItemDetail)
                        {
                            master.INV_IssueReturnItem.Add
                            (new INV_IssueReturnItem
                            {
                                ItemId = c.ItemId,
                                IssueReturnId = c.IssueReturnId,
                                Quantity = c.Quantity,
                                Comment = c.Comment,
                                ItemStatusId = c.ItemStatusId
                            }
                            );
                        }

                        _invCommonService.INVUnit.IssueReturnInfoRepository.Add(master);
                        _invCommonService.INVUnit.IssueReturnInfoRepository.SaveChanges();

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
            var master = _invCommonService.INVUnit.IssueReturnInfoRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";

            if (master.INV_IssueReturnItem != null)
            {
                model.IssueReturnItemDetail = new List<IssueReturnDetailViewModel>();

                foreach (var item in master.INV_IssueReturnItem)
                {
                    var itemDetail = item.ToModel();
                    itemDetail.Item = item.INV_ItemInfo.ItemName;
                    itemDetail.Unit = item.INV_ItemInfo.INV_Unit == null ? string.Empty : item.INV_ItemInfo.INV_Unit.Name;

                    model.IssueReturnItemDetail.Add(itemDetail);

                }
            }

            var receivedByEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivedByEmpId);
            if (receivedByEmp != null)
            {
                model.ReceivedBy = receivedByEmp.EmpID + " - " + receivedByEmp.FullName;

            }

            var receivedFromEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivedFromEmpId);
            if (receivedFromEmp != null)
            {
                model.ReceivedFrom = receivedFromEmp.EmpID + " - " + receivedFromEmp.FullName;

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
        public ActionResult Edit(IssueReturnViewModel model)
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

                        if (model.IssueReturnItemDetail != null)
                        {
                            foreach (var detail in model.IssueReturnItemDetail)
                            {
                                var child = new INV_IssueReturnItem()
                                {
                                    Id = detail.Id,
                                    ItemId = detail.ItemId,
                                    Quantity = detail.Quantity,
                                    IssueReturnId = detail.IssueReturnId,
                                    Comment = detail.Comment,
                                    ItemStatusId = detail.ItemStatusId

                                };

                                // if old item then reflection will retrive old IUser & IDate
                                arrtyList.Add(child);
                            }
                        }

                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(INV_IssueReturnItem), arrtyList);
                        _invCommonService.INVUnit.IssueReturnInfoRepository.Update(master, NavigationList);
                        _invCommonService.INVUnit.IssueReturnInfoRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
                    model.Message = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.Message = ErrorMessages.UpdateFailed;
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
                _invCommonService.INVUnit.IssueReturnItemRepository.Delete(x => x.IssueReturnId == id);
                _invCommonService.INVUnit.IssueReturnItemRepository.SaveChanges();

                _invCommonService.INVUnit.IssueReturnInfoRepository.Delete(id);
                _invCommonService.INVUnit.IssueReturnInfoRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _invCommonService.INVUnit.IssueReturnItemRepository.Delete(id);
                _invCommonService.INVUnit.IssueReturnItemRepository.SaveChanges();
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
                var entity = _invCommonService.INVUnit.ScrapInfoRepository.GetByID(id);
                entity.IsProcessed = true;

                _invCommonService.INVUnit.ScrapInfoRepository.Update(entity);
                _invCommonService.INVUnit.ScrapInfoRepository.SaveChanges();

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

        private void GenerateReturnNo(IssueReturnViewModel model)
        {
            var maxReturnNo = 1;
            var newReturnNo = string.Empty;

            var zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().Where(x => x.Id == LoggedUserZoneInfoId).FirstOrDefault().ZoneCode.Trim();

            var returnInfo = _invCommonService.INVUnit.IssueReturnInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x => x.ReturnNo);
            if (returnInfo.Any())
            {
                maxReturnNo = Convert.ToInt32(returnInfo.FirstOrDefault().ReturnNo.Replace(zoneCode, ""));
                maxReturnNo = maxReturnNo + 1;
            }

            newReturnNo = maxReturnNo.ToString();

            if (newReturnNo.Length < 7)
            {
                newReturnNo = newReturnNo.PadLeft(7, '0');
            }

            model.ReturnNo = string.Concat(zoneCode, newReturnNo);

        }

        private string CheckingBusinessLogicValidation(IssueReturnViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                var returnInfo = _invCommonService.INVUnit.IssueReturnInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();
                if (returnInfo.Where(x => x.ReturnNo == model.ReturnNo && x.Id != model.Id).Any())
                {
                    GenerateReturnNo(model);
                    message = model.Message = "Duplicate Return#. Please try again.";
                    return message;
                }
            }

            if (model.IssueReturnItemDetail == null || model.IssueReturnItemDetail.Count() < 1)
            {
                message = model.Message = "Please add item(s) to return.";
                return message;
            }
            else
            {
                var returnItems = model.IssueReturnItemDetail.GroupBy(x => x.ItemId)
                                                .Select(i =>
                                                        new 
                                                        {
                                                            ItemId = i.Key,
                                                            Quantity = i.Sum(s => s.Quantity)
                                                        }).ToList();

                foreach (var item in returnItems)
                {
                    decimal issuedQty = 0;
                    decimal returnedQty = 0;
                    var existingIssueInfo = _invCommonService.INVUnit.IssueItemRepository.Fetch().Where(x => x.ItemId == item.ItemId && x.INV_IssueInfo.IssuedToId == model.ReceivedFromEmpId).GroupBy(x => x.ItemId)
                            .Select(i =>
                                new
                                {
                                    ItemId = i.Key,
                                    IssuedQuantity = i.Sum(s => s.IssueQuantity)
                                }).FirstOrDefault();

                    if (existingIssueInfo != null)
                    {
                        issuedQty = existingIssueInfo.IssuedQuantity;
                    }

                    var existingReturnInfo = _invCommonService.INVUnit.IssueReturnItemRepository.Fetch().Where(x => x.ItemId == item.ItemId && x.INV_IssueReturnInfo.ReceivedFromEmpId == model.ReceivedFromEmpId).GroupBy(x => x.ItemId)
                            .Select(i =>
                                new
                                {
                                    ItemId = i.Key,
                                    ReturnedQuantity = i.Sum(s => s.Quantity)
                                }).FirstOrDefault();

                    if (existingReturnInfo != null)
                    {
                        returnedQty = existingReturnInfo.ReturnedQuantity;
                    }

                    var existingReturnedScrap = _invCommonService.INVUnit.IssueReturnItemRepository.GetAll().Where(x => x.ItemId == item.ItemId && x.INV_IssueReturnInfo.ScrapId == model.ScrapId).GroupBy(x => x.ItemId)
                                                                .Select(i =>
                                                                        new
                                                                        {
                                                                            ItemId = i.Key,
                                                                            Quantity = i.Sum(s => s.Quantity)
                                                                        }).FirstOrDefault();
                    var scrapInfo = _invCommonService.INVUnit.ScrapItemRepository.Fetch().Where(x => x.ItemId == item.ItemId && x.ScrapId == model.ScrapId).GroupBy(x => x.ItemId)
                        .Select(i =>
                                new
                                {
                                    ItemId = i.Key,
                                    DemandQuantity = i.Sum(s => s.Quantity)
                                }).FirstOrDefault();

                    if (model.Id == 0)
                    {
                        if (item.Quantity > (issuedQty - returnedQty))
                        {
                            message = "Return quantity must be less than or equal to existing issued quantity.";
                            return message;
                        }

                        if (existingReturnedScrap != null)
                        {
                            if (item.Quantity > (scrapInfo.DemandQuantity - existingReturnedScrap.Quantity))
                            {
                                message = model.Message = "Item(s) already returned for the scrap. Return quantity must be less than or equal to rest scrap quantity.";
                                return message;
                            }
                        }
                        else
                        {
                            if (item.Quantity > scrapInfo.DemandQuantity)
                            {
                                message = model.Message = "Return quantity must be less than or equal to scrap quantity.";
                                return message;
                            }
                        }
                    }
                    else
                    {   decimal currRetBalance = 0;
                        var currentReturnInfo = _invCommonService.INVUnit.IssueReturnItemRepository.Fetch().Where(x => x.ItemId == item.ItemId && x.IssueReturnId == model.Id).GroupBy(x => x.ItemId)
                            .Select(i =>
                                new
                                {
                                    ItemId = i.Key,
                                    RetQuantity = i.Sum(s => s.Quantity)
                                }).FirstOrDefault();

                        if (currentReturnInfo != null)
                            currRetBalance = currentReturnInfo.RetQuantity;

                        if (item.Quantity > (issuedQty - (returnedQty - currRetBalance)))
                        {
                            message = "Return quantity must be less than or equal to existing issued quantity.";
                            return message;
                        }

                        if (existingReturnedScrap != null)
                        {
                            var existingReturnedScrapBalance = existingReturnedScrap.Quantity - currRetBalance;

                            if (item.Quantity > (scrapInfo.DemandQuantity - existingReturnedScrapBalance))
                            {
                                message = model.Message = "Item(s) already returned for the scrap. Return quantity must be less than or equal to rest scrap quantity.";
                                return message;
                            }
                        }
                        else
                        {
                            if (item.Quantity > scrapInfo.DemandQuantity)
                            {
                                message = model.Message = "Return quantity must be less than or equal to scrap quantity.";
                                return message;
                            }
                        }
                    }
                }
            }

            return message;
        }

        private string AddItemValidation(IssueReturnDetailViewModel model)
        {
            string message = string.Empty;

            if (model.EmployeeId == 0)
            {
                message = "Please select received from employee first.";
                return message;
            }

            if (model.ScrapId == 0)
            {
                message = "Please select Scrap# first.";
                return message;
            }
            
            decimal issuedQty = 0;
            decimal returnedQty = 0;
            var existingIssueInfo = _invCommonService.INVUnit.IssueItemRepository.Fetch().Where(x => x.ItemId == model.ItemId && x.INV_IssueInfo.IssuedToId == model.EmployeeId).GroupBy(x => x.ItemId)
                    .Select(i =>
                        new
                        {
                            ItemId = i.Key,
                            IssuedQuantity = i.Sum(s => s.IssueQuantity)
                        }).FirstOrDefault();

            if (existingIssueInfo != null)
            {
                issuedQty = existingIssueInfo.IssuedQuantity;
            }
            else
            {
                message = "Selected item is never issued to this employee.";
                return message;
            }

            var existingReturnInfo = _invCommonService.INVUnit.IssueReturnItemRepository.Fetch().Where(x => x.ItemId == model.ItemId && x.INV_IssueReturnInfo.ReceivedFromEmpId == model.EmployeeId).GroupBy(x => x.ItemId)
                    .Select(i =>
                        new
                        {
                            ItemId = i.Key,
                            ReturnedQuantity = i.Sum(s => s.Quantity)
                        }).FirstOrDefault();

            if (existingReturnInfo != null)
            {
                returnedQty = existingReturnInfo.ReturnedQuantity;
            }

            var existingReturnedScrap = _invCommonService.INVUnit.IssueReturnItemRepository.GetAll().Where(x => x.ItemId == model.ItemId && x.INV_IssueReturnInfo.ScrapId == model.ScrapId).GroupBy(x => x.ItemId)
                                                                .Select(i =>
                                                                        new
                                                                        {
                                                                            ItemId = i.Key,
                                                                            Quantity = i.Sum(s => s.Quantity)
                                                                        }).FirstOrDefault();
            var scrapInfo = _invCommonService.INVUnit.ScrapItemRepository.Fetch().Where(x => x.ItemId == model.ItemId && x.ScrapId == model.ScrapId).GroupBy(x => x.ItemId)
                .Select(i =>
                        new
                        {
                            ItemId = i.Key,
                            DemandQuantity = i.Sum(s => s.Quantity)
                        }).FirstOrDefault();

            if (model.IssueReturnId == 0)
            {
                if (model.Quantity > (issuedQty - returnedQty - model.SumQty))
                {
                    message = "Return quantity must be less than or equal to existing issued quantity.";
                    return message;
                }

                if (existingReturnedScrap != null)
                {
                    if (model.Quantity > (scrapInfo.DemandQuantity - (existingReturnedScrap.Quantity + model.SumQty)))
                    {
                        message = model.Message = "Item(s) already returned for the scrap. Return quantity must be less than or equal to rest scrap quantity.";
                        return message;
                    }
                }
                else
                {
                    if ((model.Quantity + model.SumQty) > scrapInfo.DemandQuantity)
                    {
                        message = model.Message = "Return quantity must be less than or equal to scrap quantity.";
                        return message;
                    }
                }
            }
            else
            {
                decimal currRetBalance = 0;
                var currentReturnInfo = _invCommonService.INVUnit.IssueReturnItemRepository.Fetch().Where(x => x.ItemId == model.ItemId && x.IssueReturnId == model.IssueReturnId).GroupBy(x => x.ItemId)
                    .Select(i =>
                        new
                        {
                            ItemId = i.Key,
                            RetQuantity = i.Sum(s => s.Quantity)
                        }).FirstOrDefault();

                if (currentReturnInfo != null)
                    currRetBalance = currentReturnInfo.RetQuantity;

                if (model.Quantity > (issuedQty - (returnedQty - currRetBalance + model.SumQty)))
                {
                    message = "Return quantity must be less than or equal to existing issued quantity.";
                    return message;
                }

                if (existingReturnedScrap != null)
                {
                    var existingReturnedScrapBalance = existingReturnedScrap.Quantity - currRetBalance;

                    if (model.Quantity > (scrapInfo.DemandQuantity - (existingReturnedScrapBalance + model.SumQty)))
                    {
                        message = model.Message = "Item(s) already returned for the scrap. Return quantity must be less than or equal to rest scrap quantity.";
                        return message;
                    }
                }
                else
                {
                    if ((model.Quantity + model.SumQty) > scrapInfo.DemandQuantity)
                    {
                        message = model.Message = "Return quantity must be less than or equal to scrap quantity.";
                        return message;
                    }
                }
            }
            
            

            return message;
        }
        private void PopulateList(IssueReturnViewModel model)
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

            if ( model.ScrapId > 0)
            {
                var idList = new List<string>();
                var scrapItems = _invCommonService.INVUnit.ScrapItemRepository.GetAll().Where(x => x.ScrapId == model.ScrapId);
                foreach (var scrapItem in scrapItems)
                {
                    idList.Add(scrapItem.ItemId.ToString());
                }
                model.ItemList = model.ItemList.Where(x => idList.Contains(x.Value)).ToList();
            }

            model.ScrapList = _invCommonService.INVUnit.ScrapInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x => x.ScrapNo).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ScrapNo,
                    Value = y.Id.ToString()
                }).ToList();

            model.CategoryList = Common.PopulateDllList(_invCommonService.INVUnit.CategoryRepository.GetAll().OrderBy(x => x.Name));
            model.ColorList = Common.PopulateDllList(_invCommonService.INVUnit.ColorRepository.GetAll().OrderBy(x => x.Name));
            model.ModelList = Common.PopulateDllList(_invCommonService.INVUnit.ModelRepository.GetAll().OrderBy(x => x.Name));
            model.UnitList = Common.PopulateDllList(_invCommonService.INVUnit.UnitRepository.GetAll().OrderBy(x => x.Name));
            model.StatusList = Common.PopulateDllList(_invCommonService.INVUnit.ItemStatusRepository.GetAll().OrderBy(x => x.Name));

            //Item status default value
            model.ItemStatusId = 1;

        }

        #endregion

        public ActionResult AddDetail(IssueReturnDetailViewModel model)
        {
            var master = new IssueReturnViewModel();
            master.IssueReturnItemDetail = new List<IssueReturnDetailViewModel>();
            master.IssueReturnItemDetail.Add(model);

            master.Message = AddItemValidation(model);

            if (master.Message != string.Empty)
            {
                return Json(new
                {
                    Success = true,
                    Message = master.Message
                }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_Details", master);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(IssueReturnViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var ReceivedFrom = string.Empty;
                    var ReceivedFromEmpId = string.Empty;
                    var ReceivedBy = string.Empty;
                    var ReceivedByEmpId = string.Empty;

                    var objReceivedFrom = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivedFromEmpId);
                    var objReceivedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivedByEmpId);

                    if (objReceivedFrom != null)
                    {
                        ReceivedFrom = objReceivedFrom.EmpID + " - " + objReceivedFrom.FullName;
                        ReceivedFromEmpId = objReceivedFrom.Id.ToString();
                    }
                    if (objReceivedBy != null)
                    {
                        ReceivedBy = objReceivedBy.EmpID + " - " + objReceivedBy.FullName;
                        ReceivedByEmpId = objReceivedBy.Id.ToString();
                    }

                    return Json(new
                    {
                        ReceivedFrom = ReceivedFrom,
                        ReceivedFromEmpId = ReceivedFromEmpId,
                        ReceivedBy = ReceivedBy,
                        ReceivedByEmpId = ReceivedByEmpId

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

        public JsonResult GetScrapDetails(int? Id)
        {
            //string scrapDate = string.Empty;
            string issuedToEmpId = string.Empty;
            string issuedTo = string.Empty;

            if (Id > 0)
            {
                var scrapInfo = _invCommonService.INVUnit.ScrapInfoRepository.GetByID(Id);
                //scrapDate = scrapInfo.ScrapDate.ToString("yyyy-MM-dd");
                issuedToEmpId = scrapInfo.IssuedToEmpId.ToString();

                var objIssuedToEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(scrapInfo.IssuedToEmpId);

                if (objIssuedToEmp != null)
                {
                    issuedTo = objIssuedToEmp.EmpID + " - " + objIssuedToEmp.FullName;
                }
            }

            return Json(new
            {
                //scrapDate = scrapDate,
                issuedToEmpId = issuedToEmpId,
                issuedTo = issuedTo
            }, JsonRequestBehavior.AllowGet);
        }

        public string GetItemList(int? typeId, int? categoryId, int? modelId, int? unitId, int? colorId, int? scrapId)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });
            var items = new List<INV_ItemInfo>();

            items = (from entity in _invCommonService.INVUnit.ItemInfoRepository.Fetch()
                     select entity).OrderBy(o => o.ItemName).ToList();

            if (scrapId != null)
            {
                var idList = new List<Int32>();
                var scrapItems = _invCommonService.INVUnit.ScrapItemRepository.GetAll().Where(x => x.ScrapId == scrapId);
                foreach (var scrapItem in scrapItems)
                {
                    idList.Add(scrapItem.ItemId);
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

        public string GetScrapList(int? issuedToId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "[Select One]", Value = "" });
            var scrapList = new List<INV_ScrapInfo>();

            scrapList = (from entity in _invCommonService.INVUnit.ScrapInfoRepository.Fetch()
                       select entity).OrderByDescending(o => o.ScrapNo).ToList();

            if (issuedToId != null && issuedToId > 0)
            {
                scrapList = scrapList.Where(x => x.IssuedToEmpId == issuedToId).ToList();
            }

            //if (!string.IsNullOrEmpty(scrapDate))
            //{
            //    scrapList = scrapList.Where(x => x.ScrapDate == Convert.ToDateTime(scrapDate)).ToList();
            //}

            if (scrapList != null)
            {
                foreach (var item in scrapList)
                {
                    var listItem = new SelectListItem { Text = item.ScrapNo, Value = item.Id.ToString() };
                    list.Add(listItem);
                }
            }

            return new JavaScriptSerializer().Serialize(list);
        }
    }

}