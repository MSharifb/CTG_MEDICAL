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
    public class TransferInInfoController : BaseController
    {
        //
        // GET: /INV/TransferInInfo/
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public TransferInInfoController(PRMCommonSevice prmCommonService, INVCommonService invCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
        }
        #endregion

        #region Actions

        public ActionResult TransferSearch()
        {
            var model = new TransferInViewModel();

            return View("TransferSearch", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetTransferList(JqGridRequest request, TransferInViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = new List<sp_INV_PendingTransferList_Result>();

            using (var invContext = new ERP_BEPZAINVEntities())
            {
                list = invContext.sp_INV_PendingTransferList(LoggedUserZoneInfoId, 0).DistinctBy(x => x.TransferNo).ToList();
            }

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.TransferNo))
                {
                    list = list.Where(x => x.TransferNo.Contains(viewModel.TransferNo)).ToList();
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
                int Id = 0;

                Id = d.TransferOutId;

                response.Records.Add(new JqGridRecord(Convert.ToString(Id), new List<object>()
                {
                    Id,
                    d.TransferNo,
                    d.TransferDate.ToString(DateAndTime.GlobalDateFormat),
                    d.ZoneName
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, TransferInViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<TransferInViewModel> list = (from TIN in _invCommonService.INVUnit.TransferInInfoRepository.GetAll()
                                              join Emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on TIN.ReceivedById equals Emp.Id
                                              join Zone in _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll() on TIN.ReceivedFromId equals Zone.Id
                                              select new TransferInViewModel()
                                              {
                                                  ReceivedByEmpId = Emp.EmpID,
                                                  ReceivedBy = Emp.EmpID + "-" + Emp.FullName,
                                                  FullName = Emp.FullName,
                                                  ReceivedFromZone = Zone.ZoneName,
                                                  ReceivedFromId = Zone.Id,
                                                  TransferDate = TIN.TransferDate,
                                                  ChallanDate = TIN.ChallanDate,
                                                  Comment = TIN.Comment,
                                                  Id = TIN.Id,
                                                  ZoneInfoId = TIN.ZoneInfoId,
                                                  TransferNo = TIN.TransferNo,
                                                  ChallanNo = TIN.ChallanNo

                                              }).OrderBy(x => x.TransferDate).ToList();

            totalRecords = list == null ? 0 : list.Count;

            if (request.Searching)
            {
                if (viewModel.TransferDateFrom != null && viewModel.TransferDateTo != null)
                {
                    list = list.Where(x => x.TransferDate >= viewModel.TransferDateFrom && x.TransferDate <= viewModel.TransferDateTo).ToList();
                }

                if (viewModel.ChallanDateFrom != null && viewModel.ChallanDateTo != null)
                {
                    list = list.Where(x => x.ChallanDate >= viewModel.ChallanDateFrom && x.ChallanDate <= viewModel.ChallanDateTo).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.TransferNo))
                {
                    list = list.Where(x => x.TransferNo.Trim().ToLower().Contains(viewModel.TransferNo.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.ChallanNo))
                {
                    list = list.Where(x => x.ChallanNo.Trim().ToLower().Contains(viewModel.ChallanNo.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.ReceivedByEmpId))
                {
                    list = list.Where(x => x.ReceivedByEmpId.Contains(viewModel.ReceivedByEmpId) || x.FullName.ToLower().Contains(viewModel.ReceivedByEmpId.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.ReceivedFromZone))
                {
                    list = list.Where(x => x.ReceivedFromZone.Trim().ToLower().Contains(viewModel.ReceivedFromZone.Trim().ToLower())).ToList();
                }
            }
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            foreach (var d in list)
            {
                var TransferDate = d.TransferDate.ToString("dd-MMM-yyyy");
                var ChallanDate = d.ChallanDate != null ? Convert.ToDateTime(d.ChallanDate).ToString("dd-MMM-yyyy") : string.Empty;
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    viewModel.TransferDateFrom,
                    viewModel.TransferDateTo,
                    viewModel.ChallanDateFrom,
                    viewModel.ChallanDateTo,
                    d.ReceivedByEmpId,
                    d.ReceivedFromZone,
                    d.TransferNo,
                    d.ChallanNo,
                    TransferDate,
                    ChallanDate,
                    d.ReceivedFromZone,
                    d.ReceivedBy,
                    d.Comment,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            TransferInViewModel model = new TransferInViewModel();
            model.strMode = "create";
            model.TransferDate = DateTime.Now;
            GenerateTransferNo(model);
            PopulateList(model);
            return View(model);
        }

        public ActionResult PopulateCreate(int id)
        {
            TransferInViewModel model = new TransferInViewModel();

            GenerateTransferNo(model);

            var list = new List<sp_INV_PendingTransferList_Result>();

            using (var invContext = new ERP_BEPZAINVEntities())
            {
                list = invContext.sp_INV_PendingTransferList(LoggedUserZoneInfoId, id).ToList();
            }

            if (list.Count() > 0)
            {
                var transferInfo = list.FirstOrDefault();
                var transferItemList = _invCommonService.INVUnit.TransferOutItemRepository.GetAll().Where(x => x.TransferOutId == id).ToList();

                model.TransferOutId = transferInfo.TransferOutId;
                model.ReceivedFromId = transferInfo.ZoneInfoId;
                model.ReceivedFromZone = transferInfo.ZoneName;
                model.TransferDate = DateTime.Now;

                foreach (var item in list)
                {
                    var itemDetail = new TransferInDetailViewModel();
                    var transferItem = transferItemList.Where(x => x.ItemId == item.ItemId).FirstOrDefault();

                    itemDetail.ItemId = item.ItemId;
                    itemDetail.Quantity = Convert.ToDecimal(item.Quantity);
                    itemDetail.Item = transferItem.INV_ItemInfo.ItemName;
                    itemDetail.Unit = transferItem.INV_ItemInfo.INV_Unit == null ? string.Empty : transferItem.INV_ItemInfo.INV_Unit.Name;

                    model.TransferInDetail.Add(itemDetail);
                }
            }

            PopulateList(model);
            model.strMode = "create";
            model.ActionType = @"Create";
            return View("Create", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(TransferInViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(model);
                    if (model.ActionType == "Create")
                    {
                        model.Id = 0;
                    }

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        if (model.TransferInDetail.Count > 0)
                        {
                            model.IUser = User.Identity.Name;
                            model.IDate = DateTime.Now;
                            model.ZoneInfoId = LoggedUserZoneInfoId;

                            var master = model.ToEntity();

                            HttpFileCollectionBase files = Request.Files;
                            master = ToAttachFile(master, files);

                            model.Attachment = master.Attachment;
                            model.FileName = master.FileName;

                            foreach (var c in model.TransferInDetail)
                            {
                                master.INV_TransferInItem.Add
                                    (new INV_TransferInItem
                                    {
                                        ItemId = c.ItemId,
                                        TransferInId = c.TransferInId,
                                        Quantity = c.Quantity,
                                        PurchaseId = null,
                                        Comment = c.Comment
                                    });
                            }
                            _invCommonService.INVUnit.TransferInInfoRepository.Add(master);
                            _invCommonService.INVUnit.TransferInInfoRepository.SaveChanges();

                            model.Id = master.Id;
                            model.IsError = 0;
                            model.errClass = "success";
                            model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        }
                        else
                        {
                            model.IsError = 1;
                            model.errClass = "failed";
                            model.ErrMsg = "Please add atleast one item.";
                        }
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
            var master = _invCommonService.INVUnit.TransferInInfoRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";

            if (master.INV_TransferInItem != null)
            {
                model.TransferInDetail = new List<TransferInDetailViewModel>();

                foreach (var item in master.INV_TransferInItem)
                {
                    var itemDetail = item.ToModel();
                    itemDetail.Item = item.INV_ItemInfo.ItemName;
                    itemDetail.Unit = item.INV_ItemInfo.INV_Unit == null ? string.Empty : item.INV_ItemInfo.INV_Unit.Name;

                    model.TransferInDetail.Add(itemDetail);

                }
            }

            var employee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivedById);
            if (employee != null)
            {
                model.ReceivedBy = employee.EmpID + " - " + employee.FullName;
                model.ReceivedById = employee.Id;
                model.EmpId = employee.EmpID + " - " + employee.FullName;

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
        public ActionResult Edit(TransferInViewModel model)
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
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        var master = model.ToEntity();
                        ArrayList arrtyList = new ArrayList();

                        HttpFileCollectionBase files = Request.Files;
                        master = ToAttachFile(master, files);

                        if (files == null)
                        {
                            var existingObj = _invCommonService.INVUnit.TransferInInfoRepository.GetByID(master.Id);
                            master.Attachment = existingObj.Attachment;
                        }

                        if (model.TransferInDetail != null)
                        {
                            foreach (var detail in model.TransferInDetail)
                            {
                                var child = new INV_TransferInItem()
                                {
                                    ItemId = detail.ItemId,
                                    TransferInId = detail.TransferInId,
                                    Quantity = detail.Quantity,
                                    PurchaseId = detail.PurchaseId,
                                    Comment = detail.Comment

                                };
                                // if old item then reflection will retrive old IUser & IDate
                                arrtyList.Add(child);
                            }
                        }

                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(INV_TransferInItem), arrtyList);
                        _invCommonService.INVUnit.TransferInInfoRepository.Update(master, NavigationList);
                        _invCommonService.INVUnit.TransferInInfoRepository.SaveChanges();

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
                _invCommonService.INVUnit.TransferInItemRepository.Delete(x => x.TransferInId == id);
                _invCommonService.INVUnit.TransferInItemRepository.SaveChanges();
                _invCommonService.INVUnit.TransferInInfoRepository.Delete(id);
                _invCommonService.INVUnit.TransferInInfoRepository.SaveChanges();
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
                _invCommonService.INVUnit.TransferInItemRepository.Delete(id);
                _invCommonService.INVUnit.TransferInItemRepository.SaveChanges();
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
                var entity = _invCommonService.INVUnit.TransferOutInfoRepository.GetByID(id);
                entity.IsProcessed = true;

                _invCommonService.INVUnit.TransferOutInfoRepository.Update(entity);
                _invCommonService.INVUnit.TransferOutInfoRepository.SaveChanges();

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

        private string CheckingBusinessLogicValidation(TransferInViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                var transferInfo = _invCommonService.INVUnit.TransferInInfoRepository.GetAll().ToList();
                if (transferInfo.Where(x => x.TransferNo == model.TransferNo && x.Id != model.Id).Any())
                {
                    GenerateTransferNo(model);
                    message = model.Message = "Duplicate Transfer No#.";
                    return message;
                }
                if (model.TransferInDetail == null || model.TransferInDetail.Count() < 1)
                {
                    message = model.Message = "Please add item(s) for transfer.";
                    return message;
                }
                else
                {
                    var zoneId = model.ReceivedFromId;
                    if (zoneId > 0)
                    {
                        var _invContext = new ERP_BEPZAINVEntities();

                        var pendingList = new List<sp_INV_PendingTransferList_Result>();
                        pendingList = _invContext.sp_INV_PendingTransferList(LoggedUserZoneInfoId, 0).Where(x => x.ZoneInfoId == zoneId).ToList();
                        
                        DateTime date = DateTime.Now;
                        decimal pendingInBalance = 0;

                        var inItems = model.TransferInDetail.GroupBy(x => new { x.ItemId, x.StrMode, x.ReceivedFromId, x.SumQty })
                                .Select(i =>
                                        new TransferInDetailViewModel
                                        {
                                            ItemId = i.Key.ItemId,
                                            StrMode = i.Key.StrMode,
                                            ReceivedFromId = i.Key.ReceivedFromId,
                                            SumQty = i.Key.SumQty,
                                            Quantity = i.Sum(s => s.Quantity)
                                        }).ToList();
                        foreach (var item in inItems)
                        {
                            var pendingInItem = pendingList.Where(x => x.ItemId == item.ItemId).FirstOrDefault();
                            if (pendingInItem != null)
                                pendingInBalance = pendingInItem.Quantity?? 0;

                            var balanceInfo = _invContext.fn_INV_GetItemClosingBalance(date, item.ItemId, zoneId.ToString()).FirstOrDefault();
                            if (model.strMode == "create")
                            {
                                if (balanceInfo != null)
                                {

                                    if (item.Quantity > (balanceInfo.Balance + pendingInBalance - item.SumQty))
                                    {
                                        message = "Insufficient balance.";
                                        return message;
                                    }
                                    
                                }
                                else
                                {
                                    message = "Item is unavailable";
                                    return message;
                                }
                            }
                            else if (model.strMode == "Edit")
                            {
                                if (balanceInfo != null)
                                {
                                    var existingItem = _invCommonService.INVUnit.TransferInItemRepository.GetAll().Where(x => x.ItemId == item.ItemId && x.TransferInId == model.Id).GroupBy(x => x.ItemId)
                                                            .Select(i =>
                                                                    new
                                                                    {
                                                                        ItemId = i.Key,
                                                                        Quantity = i.Sum(s => s.Quantity)
                                                                    }).FirstOrDefault();
                                    decimal existingItemBalance = 0;
                                    if (existingItem != null)
                                        existingItemBalance = existingItem.Quantity;
                                    var tempBalance = balanceInfo.Balance + existingItemBalance + pendingInBalance;

                                    if (item.Quantity > (tempBalance - item.SumQty))
                                    {
                                        message = "Insufficient balance.";
                                        return message;
                                    }
                                    
                                }
                                else
                                {
                                    message = "Item is unavailable";
                                    return message;
                                }
                            }
                        }

                        var itemSelectedMRR = model.TransferInDetail.Where(x => x.PurchaseId != null && x.PurchaseId > 0).GroupBy(x => new { x.ItemId, x.PurchaseId, x.StrMode, x.ReceivedFromId, x.SumQty })
                                .Select(i =>
                                        new TransferInDetailViewModel
                                        {
                                            ItemId = i.Key.ItemId,
                                            PurchaseId = i.Key.PurchaseId,
                                            StrMode = i.Key.StrMode,
                                            SumQty = i.Key.SumQty,
                                            ReceivedFromId = i.Key.ReceivedFromId,
                                            Quantity = i.Sum(s => s.Quantity)
                                        }).ToList();
                        foreach (var item in itemSelectedMRR)
                        {
                            pendingInBalance = 0;
                            var pendingInItem = pendingList.Where(x => x.ItemId == item.ItemId).FirstOrDefault();
                            if (pendingInItem != null)
                                pendingInBalance = pendingInItem.Quantity ?? 0;

                            var mrrBalance = _invContext.fn_INV_GetItemBalanceByMRR(date, item.ItemId, item.PurchaseId).FirstOrDefault();
                            if (mrrBalance != null)
                            {
                                if (model.strMode == "create")
                                {
                                    if (item.Quantity > mrrBalance.Balance + pendingInBalance)
                                    {
                                        message = model.Message = "Quantity must be less than or equal to purchase quantity of selected MRR#.";
                                        return message;
                                    }

                                }
                                else if (model.strMode == "Edit")
                                {
                                    var existingItem = _invCommonService.INVUnit.TransferInItemRepository.GetAll().Where(x => x.ItemId == item.ItemId && x.TransferInId == model.Id && x.PurchaseId == item.PurchaseId).GroupBy(x => x.ItemId)
                                                                    .Select(i =>
                                                                            new
                                                                            {
                                                                                ItemId = i.Key,
                                                                                Quantity = i.Sum(s => s.Quantity)
                                                                            }).FirstOrDefault();

                                    decimal existingItemBalance = 0;
                                    if (existingItem != null)
                                        existingItemBalance = existingItem.Quantity;

                                    var tempBalance = mrrBalance.Balance + existingItemBalance + pendingInBalance;
                                    if (item.Quantity > tempBalance)
                                    {
                                        message = model.Message = "Quantity must be less than or equal to purchase quantity of selected MRR#.";
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
                    else
                    {
                        message = "Please select zone.";
                    }
                    
                }
            }

            return message;
        }

        #region old code
        //private string CheckingBusinessLogicValidation(TransferInViewModel model)
        //{
        //    string message = string.Empty;

        //    if (model != null)
        //    {
        //        var transferInfo = _invCommonService.INVUnit.TransferInInfoRepository.GetAll().ToList();
        //        if (transferInfo.Where(x => x.TransferNo == model.TransferNo && x.Id != model.Id).Any())
        //        {
        //            GenerateTransferNo(model);
        //            message = model.Message = "Duplicate Transfer No#.";
        //            return message;
        //        }
        //        if (model.TransferInDetail == null || model.TransferInDetail.Count() < 1)
        //        {
        //            message = model.Message = "Please add item(s) for transfer.";
        //        }
        //        else
        //        {
        //            var zoneId = model.ReceivedFromId;
        //            var _invContext = new ERP_BEPZAINVEntities();
        //            DateTime date = DateTime.Now;
        //            var inItems = model.TransferInDetail.GroupBy(x => new { x.ItemId, x.PurchaseId, x.StrMode, x.ReceivedFromId, x.SumQty })
        //                    .Select(i =>
        //                            new TransferInDetailViewModel
        //                            {
        //                                ItemId = i.Key.ItemId,
        //                                PurchaseId = i.Key.PurchaseId,
        //                                StrMode = i.Key.StrMode,
        //                                ReceivedFromId = i.Key.ReceivedFromId,
        //                                SumQty = i.Key.SumQty,
        //                                Quantity = i.Sum(s => s.Quantity)
        //                            }).ToList();
        //            foreach (var item in inItems)
        //            {
        //                if (zoneId == 0 || zoneId == null)
        //                {
        //                    zoneId = item.ReceivedFromId;
        //                }

        //                if (zoneId > 0)
        //                {
        //                    var balanceInfo = _invContext.fn_INV_GetItemClosingBalance(date, item.ItemId, zoneId.ToString()).FirstOrDefault();
        //                    if (item.StrMode == "create")
        //                    {
        //                        if (balanceInfo != null)
        //                        {
        //                            if (balanceInfo.Balance > 0)
        //                            {
        //                                if (item.Quantity > (balanceInfo.Balance - item.SumQty))
        //                                {
        //                                    message = "Insufficient balance.";
        //                                }
        //                                if (item.PurchaseId > 0)
        //                                {
        //                                    var mrrBalance = _invContext.fn_INV_GetItemBalanceByMRR(date, item.ItemId, item.PurchaseId).FirstOrDefault();
        //                                    var tempBalance = balanceInfo.Balance - item.SumQty;
        //                                    if (item.Quantity > mrrBalance.Balance || item.Quantity > tempBalance)
        //                                    {
        //                                        message = "Insufficient balance for this purchase.";
        //                                    }

        //                                }
        //                            }
        //                            else
        //                            {
        //                                message = "Item is unavailable";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            message = "Item is unavailable";
        //                        }
        //                    }
        //                    else if (item.StrMode == "Edit")
        //                    {
        //                        if (balanceInfo != null)
        //                        {
        //                            if (balanceInfo.Balance > 0)
        //                            {
        //                                decimal prevQnty = _invCommonService.INVUnit.TransferInItemRepository.GetAll().Where(x => x.TransferInId == item.TransferInId).Select(x => x.Quantity).FirstOrDefault();
        //                                if (item.Quantity > ((balanceInfo.Balance - prevQnty) - item.SumQty))
        //                                {
        //                                    message = "Insufficient balance.";
        //                                }
        //                                if (item.PurchaseId > 0)
        //                                {
        //                                    var mrrBalance = _invContext.fn_INV_GetItemBalanceByMRR(date, item.ItemId, item.PurchaseId).FirstOrDefault();
        //                                    var tempBalance = balanceInfo.Balance - item.SumQty;
        //                                    if (item.Quantity > (mrrBalance.Balance - prevQnty) || item.Quantity > (tempBalance - prevQnty))
        //                                    {
        //                                        message = "Insufficient balance for this purchase.";
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                message = "Item is unavailable";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            message = "Item is unavailable";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return message;
        //}

        //private string CheckItemValidation(TransferInViewModel model)
        //{
        //    string msg = string.Empty;
        //    var master = new INV_TransferInItem();
        //    master = model.ToEntity();
        //    var _invContext = new ERP_BEPZAINVEntities();
        //    DateTime date = DateTime.Now;
        //    var zoneId = model.ReceivedFromId;
        //    var balance = _invContext.fn_INV_GetItemClosingBalance(date, model.ItemId, zoneId.ToString()).FirstOrDefault().Balance;
        //    if (model.strMode == "create")
        //    {
        //        if (balance != 0)
        //        {
        //            if (model.Quantity > balance)
        //            {
        //                msg = "Available balance for this item is " + balance;
        //            }
        //            if (model.PurchaseId > 0)
        //            {
        //                var mrrBalance = _invContext.fn_INV_GetItemBalanceByMRR(date, model.ItemId, model.PurchaseId).FirstOrDefault().Balance;
        //                if (model.Quantity > mrrBalance)
        //                {
        //                    msg = "Available balance for this purchase is" + mrrBalance;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            msg = master.INV_ItemInfo.ItemName + " is unavailable";
        //        }
        //    }
        //    else if(model.StrMode == "Edit")
        //    {
        //        decimal prevQnty = _invCommonService.INVUnit.TransferInItemRepository.GetAll().Where(x => x.TransferInId == model.TransferInId).Select(x => x.Quantity).FirstOrDefault();
        //         if (model.Quantity > (balance-prevQnty))
        //            {
        //                msg = "Available balance for this item is " + (balance - prevQnty);
        //            }
        //            if (model.PurchaseId > 0)
        //            {
        //                var mrrBalance = _invContext.fn_INV_GetItemBalanceByMRR(date, model.ItemId, model.PurchaseId).FirstOrDefault().Balance;
        //                if (model.Quantity > (mrrBalance-prevQnty))
        //                {
        //                    msg = "Available balance for this purchase is" + (mrrBalance - prevQnty);
        //                }
        //            }
        //        else
        //        {
        //            msg = master.INV_ItemInfo.ItemName + " is unavailable";
        //        }
        //    }

        //    return msg;
        //}
        #endregion

        private void PopulateList(TransferInViewModel model)
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

            model.ReceivedFromList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x=>x.SortOrder).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ZoneName,
                    Value = y.Id.ToString()
                }).ToList();

            model.CategoryList = Common.PopulateDllList(_invCommonService.INVUnit.CategoryRepository.GetAll().OrderBy(x => x.Name));
            model.ColorList = Common.PopulateDllList(_invCommonService.INVUnit.ColorRepository.GetAll().OrderBy(x => x.Name));
            model.ModelList = Common.PopulateDllList(_invCommonService.INVUnit.ModelRepository.GetAll().OrderBy(x => x.Name));
            model.UnitList = Common.PopulateDllList(_invCommonService.INVUnit.UnitRepository.GetAll().OrderBy(x => x.Name));

        }
        public JsonResult LoadMRR(int receivedFromId)
        {
            var list = _invCommonService.INVUnit.PurchaseInfoRepository.GetAll().Where(x => x.ZoneInfoId == receivedFromId)
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.MRR,
                    Value = y.Id.ToString()
                }).ToList();

            return Json(new { Result = list });
        }

        //public JsonResult CheckQuantity(int itemId, int? purchaseIsd, decimal quantity)
        //{


        //}

        #endregion

        #region others
        public ActionResult AddDetail(TransferInDetailViewModel model)
        {
            var master = new TransferInViewModel();
            master.TransferInDetail = new List<TransferInDetailViewModel>();
            master.TransferInDetail.Add(model);
            master.ReceivedFromId = model.ReceivedFromId;
            master.strMode = model.StrMode;
            master.Id = model.TransferInId;
            model.ErrMessage = CheckingBusinessLogicValidation(master);
            if (model.ErrMessage != "")
            {
                return Json(new
                {
                    Success = true,
                    Message = model.ErrMessage
                }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_Details", master);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(TransferInViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivedById);
                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmpId = obj.EmpID + " - " + obj.FullName,
                            ReceivedById = obj.Id

                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
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

        public string GetItemList(int? typeId, int? categoryId, int? modelId, int? unitId, int? colorId, int? purchaseId, int? receivedFromZoneId)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });
            var items = (from entity in _invCommonService.INVUnit.ItemInfoRepository.Fetch()
                         select entity).OrderBy(o => o.ItemName).ToList();

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
            if (purchaseId != null)
            {
                var purchasedidList = _invCommonService.INVUnit.PurchaseItemRepository.Fetch().Where(x => x.PurchaseId == purchaseId && x.INV_PurchaseInfo.ZoneInfoId == receivedFromZoneId).ToList();
                items = items.Where(item => purchasedidList.Any(id => id.ItemId.Equals(item.Id))).ToList();

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

        [NoCache]
        public void GenerateTransferNo(TransferInViewModel model)
        {
            string newTransferNo = string.Empty;

            var zoneId = LoggedUserZoneInfoId.ToString();
            var maxId = 1;
            var zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().Where(x => x.Id == LoggedUserZoneInfoId).FirstOrDefault().ZoneCode.Trim();
            var transferInfo = _invCommonService.INVUnit.TransferInInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x => x.TransferNo);
            if (transferInfo.Any())
            {
                maxId = Convert.ToInt32(transferInfo.FirstOrDefault().TransferNo.Replace(zoneCode, ""));
                maxId += 1;
            }
            newTransferNo = maxId.ToString();
            if (newTransferNo.ToString().Length < 7)
            {
                newTransferNo = newTransferNo.PadLeft(7, '0');
            }

            model.TransferNo = string.Concat(zoneCode, newTransferNo);

        }

        //public ActionResult ZoneListView()
        //{
        //    var list = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll()
        //        .Select(y =>
        //        new SelectListItem()
        //        {
        //            Text = y.ZoneName,
        //            Value = y.Id.ToString()
        //        }).ToList();
        //    return PartialView("Select", list);
        //}

        public ActionResult DownloadFile(int transferInId)
        {
            try
            {
                var transferInInfo = _invCommonService.INVUnit.TransferInInfoRepository.GetByID(transferInId);
                string fileName = string.Empty;
                byte[] fileData = null;
                if (transferInInfo != null)
                {
                    if (!string.IsNullOrEmpty(transferInInfo.FileName))
                    {
                        fileName = transferInInfo.FileName;
                        fileData = transferInInfo.Attachment;
                        string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                        string contentType = MimeMapping.GetMimeMapping(filePath);
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = fileName,
                            Inline = true,
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());

                        return File(fileData, contentType);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private INV_TransferInInfo ToAttachFile(INV_TransferInInfo transferInInfo, HttpFileCollectionBase files)
        {
            foreach (string fileTagName in files)
            {
                HttpPostedFileBase file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.

                    int size = file.ContentLength;
                    string name = file.FileName;
                    int position = name.LastIndexOf("\\");
                    name = name.Substring(position + 1);
                    string contentType = file.ContentType;
                    byte[] fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);
                    transferInInfo.Attachment = fileData;
                    transferInInfo.FileName = file.FileName;
                }
            }

            return transferInInfo;
        }

        #endregion
    }
}