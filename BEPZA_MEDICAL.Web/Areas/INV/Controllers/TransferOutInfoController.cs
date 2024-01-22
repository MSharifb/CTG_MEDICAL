using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class TransferOutInfoController : BaseController
    {
        //
        // GET: /INV/TransferOutInfo/
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public TransferOutInfoController(PRMCommonSevice prmCommonService, INVCommonService invCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, TransferOutViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<TransferOutViewModel> list = (from TON in _invCommonService.INVUnit.TransferOutInfoRepository.GetAll()
                                               join Emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on TON.TransferredByEmpId equals Emp.Id
                                               join Zone in _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll() on TON.TransferredToZoneId equals Zone.Id
                                               select new TransferOutViewModel()
                                               {
                                                   TransferredByEmpId = Emp.Id,
                                                   TransferredByEmp = Emp.EmpID + "-" + Emp.FullName,
                                                   EmpId = Emp.EmpID,
                                                   FullName = Emp.FullName,
                                                   TransferredToZone = Zone.ZoneName,
                                                   TransferredToZoneId = Zone.Id,
                                                   TransferDate = TON.TransferDate,
                                                   ChallanDate = TON.ChallanDate,
                                                   Comment = TON.Comment,
                                                   Id = TON.Id,
                                                   ZoneInfoId = TON.ZoneInfoId,
                                                   TransferNo = TON.TransferNo,
                                                   ChallanNo = TON.ChallanNo

                                               }).OrderBy(x => x.TransferDate).ToList();

            totalRecords = list == null ? 0 : list.Count;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

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
                if (!String.IsNullOrWhiteSpace(viewModel.EmpId))
                {
                    list = list.Where(x => x.EmpId.Contains(viewModel.EmpId) || x.FullName.ToLower().Contains(viewModel.EmpId.Trim().ToLower())).ToList();
                }
                if (!String.IsNullOrWhiteSpace(viewModel.TransferredToZone))
                {
                    list = list.Where(x => x.TransferredToZone.Trim().ToLower().Contains(viewModel.TransferredToZone.Trim().ToLower())).ToList();
                }
            }

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            foreach (var d in list)
            {
                var TransferDate = d.TransferDate.ToString("dd-MMM-yyyy");
                var ChallanDate = d.ChallanDate != null ? Convert.ToDateTime(d.ChallanDate).ToString("dd-MMM-yyyy") : string.Empty;
                var Comment = d.Comment;

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    viewModel.TransferDateFrom,
                    viewModel.TransferDateTo,
                    viewModel.ChallanDateFrom,
                    viewModel.ChallanDateTo,
                    d.EmpId,
                    d.TransferredToZone,
                    d.TransferNo,
                    d.ChallanNo,
                    TransferDate,
                    ChallanDate,
                    d.TransferredToZone,
                    d.TransferredByEmp,
                    Comment,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            TransferOutViewModel model = new TransferOutViewModel();
            model.strMode = "create";
            model.TransferDate = DateTime.Now;
            GenerateTransferNo(model);
            PopulateList(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(TransferOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        if (model.TransferOutDetail.Count > 0)
                        {
                            model.IUser = User.Identity.Name;
                            model.IDate = DateTime.Now;
                            model.ZoneInfoId = LoggedUserZoneInfoId;

                            var master = model.ToEntity();

                            HttpFileCollectionBase files = Request.Files;
                            master = ToAttachFile(master, files);

                            model.Attachment = master.Attachment;
                            model.FileName = master.FileName;

                            foreach (var c in model.TransferOutDetail)
                            {
                                master.INV_TransferOutItem.Add
                                    (new INV_TransferOutItem
                                    {
                                        ItemId = c.ItemId,
                                        TransferOutId = c.TransferOutId,
                                        Quantity = c.Quantity,
                                        PurchaseId = null,
                                        Comment = c.Comment
                                    });
                            }
                            _invCommonService.INVUnit.TransferOutInfoRepository.Add(master);
                            _invCommonService.INVUnit.TransferOutInfoRepository.SaveChanges();

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
            var master = _invCommonService.INVUnit.TransferOutInfoRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";

            if (master.INV_TransferOutItem != null)
            {
                model.TransferOutDetail = new List<TransferOutDetailViewModel>();

                foreach (var item in master.INV_TransferOutItem)
                {
                    var itemDetail = item.ToModel();
                    itemDetail.Item = item.INV_ItemInfo.ItemName;
                    itemDetail.Unit = item.INV_ItemInfo.INV_Unit == null ? string.Empty : item.INV_ItemInfo.INV_Unit.Name;

                    model.TransferOutDetail.Add(itemDetail);

                }
            }

            var employee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.TransferredByEmpId);
            if (employee != null)
            {
                model.TransferredByEmp = employee.EmpID + " - " + employee.FullName;
                model.TransferredByEmpId = employee.Id;
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
        public ActionResult Edit(TransferOutViewModel model)
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
                            var existingObj = _invCommonService.INVUnit.TransferOutInfoRepository.GetByID(master.Id);
                            master.Attachment = existingObj.Attachment;
                        }

                        if (model.TransferOutDetail != null)
                        {
                            foreach (var detail in model.TransferOutDetail)
                            {
                                //if(detail.PurchaseId==0)
                                //{

                                //}
                                var child = new INV_TransferOutItem()
                                {
                                    ItemId = detail.ItemId,
                                    TransferOutId = master.Id,
                                    Quantity = detail.Quantity,
                                    PurchaseId = detail.PurchaseId,
                                    Comment = detail.Comment

                                };

                                // if old item then reflection will retrive old IUser & IDate
                                arrtyList.Add(child);
                            }
                        }

                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(INV_TransferOutItem), arrtyList);
                        _invCommonService.INVUnit.TransferOutInfoRepository.Update(master, NavigationList);
                        _invCommonService.INVUnit.TransferOutInfoRepository.SaveChanges();

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
                _invCommonService.INVUnit.TransferOutItemRepository.Delete(x => x.TransferOutId == id);
                _invCommonService.INVUnit.TransferOutItemRepository.SaveChanges();
                _invCommonService.INVUnit.TransferOutInfoRepository.Delete(id);
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

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _invCommonService.INVUnit.TransferOutItemRepository.Delete(id);
                _invCommonService.INVUnit.TransferOutItemRepository.SaveChanges();
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

        private string CheckingBusinessLogicValidation(TransferOutViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                var transferInfo = _invCommonService.INVUnit.TransferOutInfoRepository.GetAll().ToList();
                if (transferInfo.Where(x => x.TransferNo == model.TransferNo && x.Id != model.Id).Any())
                {
                    GenerateTransferNo(model);
                    message = model.Message = "Duplicate Transfer No#.";
                    return message;
                }
                if (model.TransferOutDetail == null || model.TransferOutDetail.Count() < 1)
                {
                    message = model.Message = "Please add item(s) for transfer.";
                    return message;
                }
                else
                {
                    var zoneId = LoggedUserZoneInfoId;
                    var _invContext = new ERP_BEPZAINVEntities();
                    DateTime date = DateTime.Now;

                    if (zoneId > 0)
                    {
                        var outItems = model.TransferOutDetail.GroupBy(x => new { x.ItemId, x.StrMode, x.TransferredToZoneId, x.SumQty })
                                .Select(i =>
                                        new TransferOutDetailViewModel
                                        {
                                            ItemId = i.Key.ItemId,
                                            StrMode = i.Key.StrMode,
                                            SumQty = i.Key.SumQty,
                                            TransferredToZoneId = i.Key.TransferredToZoneId,
                                            Quantity = i.Sum(s => s.Quantity)
                                        }).ToList();
                        foreach (var item in outItems)
                        {
                            var balanceInfo = _invContext.fn_INV_GetItemClosingBalance(date, item.ItemId, zoneId.ToString()).FirstOrDefault();
                            if (model.strMode == "create")
                            {
                                if (balanceInfo != null)
                                {
                                    if (balanceInfo.Balance > 0)
                                    {
                                        if (item.Quantity > (balanceInfo.Balance - item.SumQty))
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
                                    if (balanceInfo.Balance > 0)
                                    {
                                        var existingItem = _invCommonService.INVUnit.TransferOutItemRepository.GetAll().Where(x => x.ItemId == item.ItemId && x.TransferOutId == model.Id).GroupBy(x => x.ItemId)
                                                                .Select(i =>
                                                                        new
                                                                        {
                                                                            ItemId = i.Key,
                                                                            Quantity = i.Sum(s => s.Quantity)
                                                                        }).FirstOrDefault();
                                        decimal existingItemBalance = 0;
                                        if (existingItem != null)
                                            existingItemBalance = existingItem.Quantity;
                                        var tempBalance = balanceInfo.Balance + existingItemBalance;

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
                                else
                                {
                                    message = "Item is unavailable";
                                    return message;
                                }
                            }
                        }

                        var itemSelectedMRR = model.TransferOutDetail.Where(x => x.PurchaseId != null && x.PurchaseId > 0).GroupBy(x => new { x.ItemId, x.PurchaseId, x.StrMode, x.TransferredToZoneId, x.SumQty })
                                .Select(i =>
                                        new TransferOutDetailViewModel
                                        {
                                            ItemId = i.Key.ItemId,
                                            PurchaseId = i.Key.PurchaseId,
                                            StrMode = i.Key.StrMode,
                                            SumQty = i.Key.SumQty,
                                            TransferredToZoneId = i.Key.TransferredToZoneId,
                                            Quantity = i.Sum(s => s.Quantity)
                                        }).ToList();
                        foreach (var item in itemSelectedMRR)
                        {
                            var mrrBalance = _invContext.fn_INV_GetItemBalanceByMRR(date, item.ItemId, item.PurchaseId).FirstOrDefault();
                            if (mrrBalance != null)
                            {
                                if (model.strMode == "create")
                                {
                                    if (item.Quantity > mrrBalance.Balance)
                                    {
                                        message = model.Message = "Quantity must be less than or equal to purchase quantity of selected MRR#.";
                                        return message;
                                    }
                                    
                                }
                                else if (model.strMode == "Edit")
                                {
                                    var existingItem = _invCommonService.INVUnit.TransferOutItemRepository.GetAll().Where(x => x.ItemId == item.ItemId && x.TransferOutId == model.Id && x.PurchaseId == item.PurchaseId).GroupBy(x => x.ItemId)
                                                                    .Select(i =>
                                                                            new
                                                                            {
                                                                                ItemId = i.Key,
                                                                                Quantity = i.Sum(s => s.Quantity)
                                                                            }).FirstOrDefault();

                                    decimal existingItemBalance = 0;
                                    if (existingItem != null)
                                        existingItemBalance = existingItem.Quantity;

                                    var tempBalance = mrrBalance.Balance + existingItemBalance;
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
                        message = "Please select zone where to transfer item(s)";
                        return message;
                    }
                }
            }

            return message;
        }

        #region old code
        //private string CheckingBusinessLogicValidation(TransferOutViewModel model)
        //{
        //    string message = string.Empty;

        //    if (model != null)
        //    {
        //        var transferInfo = _invCommonService.INVUnit.TransferOutInfoRepository.GetAll().ToList();
        //        if (transferInfo.Where(x => x.TransferNo == model.TransferNo && x.Id != model.Id).Any())
        //        {
        //            GenerateTransferNo(model);
        //            message = model.Message = "Duplicate Transfer No#.";
        //            return message;
        //        }
        //        if (model.TransferOutDetail == null || model.TransferOutDetail.Count() < 1)
        //        {
        //            message = model.Message = "Please add item(s) for transfer.";
        //        }
        //        else
        //        {
        //            var zoneId = LoggedUserZoneInfoId;
        //            var _invContext = new ERP_BEPZAINVEntities();
        //            DateTime date = DateTime.Now;

        //            var outItems = model.TransferOutDetail.GroupBy(x => new { x.ItemId, x.PurchaseId, x.StrMode, x.TransferredToZoneId, x.SumQty })
        //                    .Select(i =>
        //                            new TransferOutDetailViewModel
        //                            {
        //                                ItemId = i.Key.ItemId,
        //                                PurchaseId = i.Key.PurchaseId,
        //                                StrMode = i.Key.StrMode,
        //                                SumQty = i.Key.SumQty,
        //                                TransferredToZoneId = i.Key.TransferredToZoneId,
        //                                Quantity = i.Sum(s => s.Quantity)
        //                            }).ToList();
        //            foreach (var item in outItems)
        //            {
        //                //if (zoneId == 0 || zoneId == null)
        //                //{
        //                //    zoneId = item.;
        //                //}

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
        //                                decimal prevQnty = _invCommonService.INVUnit.TransferOutItemRepository.GetAll().Where(x => x.TransferOutId == item.TransferOutId).Select(x => x.Quantity).FirstOrDefault();
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
        //                else
        //                {
        //                    message = "Please select zone where to transfer item(s)";
        //                }
        //            }
        //        }
        //    }

        //    return message;
        //}
        #endregion

        private void PopulateList(TransferOutViewModel model)
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

            model.TransferredToZoneList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x=>x.SortOrder).ToList()
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
            model.MRRList = _invCommonService.INVUnit.PurchaseInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.MRR).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.MRR,
                    Value = y.Id.ToString()
                }).ToList();

        }

        #endregion

        #region others
        public ActionResult AddDetail(TransferOutDetailViewModel model)
        {
            var master = new TransferOutViewModel();
            master.TransferOutDetail = new List<TransferOutDetailViewModel>();
            master.TransferOutDetail.Add(model);
            master.strMode = model.StrMode;
            master.Id = model.TransferOutId;
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
        public JsonResult GetEmployeeInfo(TransferOutViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.TransferredByEmpId);
                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmpId = obj.EmpID + " - " + obj.FullName,
                            TransferredByEmpId = obj.Id

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

        public string GetItemList(int? typeId, int? categoryId, int? modelId, int? unitId, int? colorId, int? purchaseId)
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
                var purchasedidList = _invCommonService.INVUnit.PurchaseItemRepository.Fetch().Where(x => x.PurchaseId == purchaseId).ToList();
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
        public void GenerateTransferNo(TransferOutViewModel model)
        {

            string newTransferNo = string.Empty;

            var zoneId = LoggedUserZoneInfoId.ToString();
            var maxId = 1;
            var zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().Where(x => x.Id == LoggedUserZoneInfoId).FirstOrDefault().ZoneCode.Trim();
            var transferInfo = _invCommonService.INVUnit.TransferOutInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x => x.TransferNo);
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

        public ActionResult ZoneListView()
        {
            var list = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ZoneName,
                    Value = y.Id.ToString()
                }).ToList();
            return PartialView("Select", list);
        }

        public ActionResult DownloadFile(int transferOutId)
        {
            try
            {
                var transferOutInfo = _invCommonService.INVUnit.TransferOutInfoRepository.GetByID(transferOutId);
                string fileName = string.Empty;
                byte[] fileData = null;
                if (transferOutInfo != null)
                {
                    if (!string.IsNullOrEmpty(transferOutInfo.FileName))
                    {
                        fileName = transferOutInfo.FileName;
                        fileData = transferOutInfo.Attachment;
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

        private INV_TransferOutInfo ToAttachFile(INV_TransferOutInfo transferOutInfo, HttpFileCollectionBase files)
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
                    transferOutInfo.Attachment = fileData;
                    transferOutInfo.FileName = file.FileName;
                }
            }

            return transferOutInfo;
        }

        #endregion
    }
}