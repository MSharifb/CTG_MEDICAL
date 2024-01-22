using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.Adjustment;
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
    public class AdjustmentController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        private readonly ERP_BEPZAINVEntities _invContext;
        #endregion

        #region Ctor
        public AdjustmentController(PRMCommonSevice prmCommonService, INVCommonService invCommonService, ERP_BEPZAINVEntities invContext)
        {
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
            this._invContext = invContext;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AdjustmentSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _invCommonService.INVUnit.AdjustmentInfoRepository.GetCount(filterExpression);

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var list = _invCommonService.INVUnit.AdjustmentInfoRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrWhiteSpace(viewModel.AdjustedBy))
                {
                    var adjustedByIdList = new List<Int32>();
                    var adjustedByEmpList = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.EmpID.Contains(viewModel.AdjustedBy) || x.FullName.ToLower().Contains(viewModel.AdjustedBy.Trim().ToLower())).ToList();
                    foreach (var item in adjustedByEmpList)
                    {
                        adjustedByIdList.Add(item.Id);
                    }
                    list = list.Where(x => adjustedByIdList.Contains(x.AdjustedByEmpId)).ToList();

                }

                if (!String.IsNullOrWhiteSpace(viewModel.ApprovedBy))
                {
                    var approvedByIdList = new List<Int32>();
                    var approvedByEmpList = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.EmpID.Contains(viewModel.ApprovedBy) || x.FullName.ToLower().Contains(viewModel.ApprovedBy.Trim().ToLower())).ToList();
                    foreach (var item in approvedByEmpList)
                    {
                        approvedByIdList.Add(item.Id);
                    }
                    list = list.Where(x => approvedByIdList.Contains(x.ApprovedByEmpId)).ToList();
                }
                if (viewModel.AdjustmentDateFrom != null && viewModel.AdjustmentDateTo != null)
                {
                    list = list.Where(x => x.AdjustmentDate >= viewModel.AdjustmentDateFrom && x.AdjustmentDate <= viewModel.AdjustmentDateTo).ToList();
                }

            }

            foreach (var d in list)
            {
                var objAdjustedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == d.AdjustedByEmpId).FirstOrDefault();
                var objApprovedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == d.ApprovedByEmpId).FirstOrDefault();

                var AdjustedBy = objAdjustedBy.EmpID + " - " + objAdjustedBy.FullName;
                var ApprovedBy = objApprovedBy.EmpID + " - " + objApprovedBy.FullName;

                var AdjustmentDate = d.AdjustmentDate.ToString("dd-MMM-yyyy");

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    viewModel.AdjustmentDateFrom,
                    viewModel.AdjustmentDateTo,
                    d.AdjustmentNo,
                    AdjustmentDate,
                    AdjustedBy,
                    ApprovedBy,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            AdjustmentViewModel model = new AdjustmentViewModel();

            GenerateAdjustmentNo(model);
            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AdjustmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        model.ZoneInfoId = LoggedUserZoneInfoId;

                        var master = model.ToEntity();

                        HttpFileCollectionBase files = Request.Files;
                        master = ToAttachFile(master, files);

                        model.Attachment = master.Attachment;
                        model.FileName = master.FileName;

                        foreach (var c in model.AdjustmentItemDetail)
                        {
                            master.INV_AdjustmentItem.Add
                            (new INV_AdjustmentItem
                            {
                                ItemId = c.ItemId,
                                AdjustmentId = c.AdjustmentId,
                                Rate = c.Rate,
                                Quantity = c.Quantity,
                                TotalCost = c.TotalCost,
                                Comment = c.Comment,
                                ItemStatusId = c.ItemStatusId
                            }
                            );
                        }

                        _invCommonService.INVUnit.AdjustmentInfoRepository.Add(master);
                        _invCommonService.INVUnit.AdjustmentInfoRepository.SaveChanges();

                        model.Id = master.Id;
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
            var master = _invCommonService.INVUnit.AdjustmentInfoRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";

            if (master.INV_AdjustmentItem != null)
            {
                model.AdjustmentItemDetail = new List<AdjustmentDetailViewModel>();

                foreach (var item in master.INV_AdjustmentItem)
                {
                    var itemDetail = item.ToModel();
                    itemDetail.Item = item.INV_ItemInfo.ItemName;
                    itemDetail.Unit = item.INV_ItemInfo.INV_Unit == null ? string.Empty : item.INV_ItemInfo.INV_Unit.Name;

                    model.AdjustmentItemDetail.Add(itemDetail);

                }
            }

            var adjustedByEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.AdjustedByEmpId);
            if (adjustedByEmp != null)
            {
                model.AdjustedBy = adjustedByEmp.EmpID + " - " + adjustedByEmp.FullName;

            }

            var approvedByEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ApprovedByEmpId);
            if (approvedByEmp != null)
            {
                model.ApprovedBy = approvedByEmp.EmpID + " - " + approvedByEmp.FullName;

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
        public ActionResult Edit(AdjustmentViewModel model)
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

                        HttpFileCollectionBase files = Request.Files;
                        master = ToAttachFile(master, files);

                        if (files == null)
                        {
                            var existingObj = _invCommonService.INVUnit.AdjustmentInfoRepository.GetByID(master.Id);
                            master.Attachment = existingObj.Attachment;
                        }

                        if (model.AdjustmentItemDetail != null)
                        {
                            foreach (var detail in model.AdjustmentItemDetail)
                            {
                                var child = new INV_AdjustmentItem()
                                {
                                    Id = detail.Id,
                                    ItemId = detail.ItemId,
                                    Rate = detail.Rate,
                                    Quantity = detail.Quantity,
                                    TotalCost = detail.TotalCost,
                                    AdjustmentId = detail.AdjustmentId,
                                    Comment = detail.Comment,
                                    ItemStatusId = detail.ItemStatusId

                                };

                                // if old item then reflection will retrive old IUser & IDate
                                arrtyList.Add(child);
                            }
                        }

                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(INV_AdjustmentItem), arrtyList);
                        _invCommonService.INVUnit.AdjustmentInfoRepository.Update(master, NavigationList);
                        _invCommonService.INVUnit.AdjustmentInfoRepository.SaveChanges();

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
                _invCommonService.INVUnit.AdjustmentItemRepository.Delete(x => x.AdjustmentId == id);
                _invCommonService.INVUnit.AdjustmentItemRepository.SaveChanges();

                _invCommonService.INVUnit.AdjustmentInfoRepository.Delete(id);
                _invCommonService.INVUnit.AdjustmentInfoRepository.SaveChanges();
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
                _invCommonService.INVUnit.AdjustmentItemRepository.Delete(id);
                _invCommonService.INVUnit.AdjustmentItemRepository.SaveChanges();
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

        private void GenerateAdjustmentNo(AdjustmentViewModel model)
        {
            var maxAdjustmentNo = 1;
            var newAdjustmentNo = string.Empty;

            var zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().Where(x => x.Id == LoggedUserZoneInfoId).FirstOrDefault().ZoneCode.Trim();

            var adjustmentInfo = _invCommonService.INVUnit.AdjustmentInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x => x.AdjustmentNo);
            if (adjustmentInfo.Any())
            {
                maxAdjustmentNo = Convert.ToInt32(adjustmentInfo.FirstOrDefault().AdjustmentNo.Replace(zoneCode, ""));
                maxAdjustmentNo = maxAdjustmentNo + 1;
            }

            newAdjustmentNo = maxAdjustmentNo.ToString();

            if (newAdjustmentNo.Length < 7)
            {
                newAdjustmentNo = newAdjustmentNo.PadLeft(7, '0');
            }

            model.AdjustmentNo = string.Concat(zoneCode, newAdjustmentNo);

        }

        private string CheckingBusinessLogicValidation(AdjustmentViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                var adjustmentInfo = _invCommonService.INVUnit.AdjustmentInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();
                if (adjustmentInfo.Where(x => x.AdjustmentNo == model.AdjustmentNo && x.Id != model.Id).Any())
                {
                    GenerateAdjustmentNo(model);
                    message = model.Message = "Duplicate Adjustment#. Please try again.";
                    return message;
                }
            }

            if (model.AdjustmentItemDetail == null || model.AdjustmentItemDetail.Count() < 1)
            {
                message = model.Message = "Please add item(s) to return.";
                return message;
            }
            else
            {
                var invContext = new ERP_BEPZAINVEntities();

                var adjItems = model.AdjustmentItemDetail.GroupBy(x => x.ItemId)
                        .Select(i =>
                                new AdjustmentDetailViewModel
                                {
                                    ItemId = i.Key,
                                    Quantity = i.Sum(s => s.Quantity)
                                }).ToList();

                foreach (var item in adjItems)
                {
                    decimal closingBalance = 0;

                    var balanceInfo = invContext.fn_INV_GetItemClosingBalance(DateTime.Now, item.ItemId, LoggedUserZoneInfoId.ToString()).FirstOrDefault();
                    if (balanceInfo != null)
                    {
                        closingBalance = balanceInfo.Balance;
                        if (model.Id == 0)
                        {
                            if (item.Quantity > closingBalance)
                            {
                                message = model.Message = "Insufficient balance.";
                                return message;
                            }
                        }
                        else 
                        { 
                            var existingAdj = _invCommonService.INVUnit.AdjustmentItemRepository.GetAll().Where(x=> x.ItemId == item.ItemId && x.AdjustmentId == model.Id)
                                .GroupBy(x => x.ItemId)
                                                                .Select(i =>
                                                                        new
                                                                        {
                                                                            ItemId = i.Key,
                                                                            AdjQuantity = i.Sum(s => s.Quantity)
                                                                        }).FirstOrDefault();

                            decimal existingAdjBalance = 0;
                            if (existingAdj != null)
                                existingAdjBalance = existingAdj.AdjQuantity;

                            var tempBalance = closingBalance + existingAdjBalance;
                            if (item.Quantity > tempBalance)
                            {
                                message = model.Message = "Insufficient balance.";
                                return message;
                            }
                        }
                    }
                    else
                    {
                        message = model.Message = "Insufficient balance.";
                        return message;
                    }
                }
 
            }

            return message;
        }

        private string AddItemValidation(AdjustmentDetailViewModel model)
        {
            string message = string.Empty;
            var invContext = new ERP_BEPZAINVEntities();
            decimal closingBalance = 0;

            var balanceInfo = invContext.fn_INV_GetItemClosingBalance(DateTime.Now, model.ItemId, LoggedUserZoneInfoId.ToString()).FirstOrDefault();
            if (balanceInfo != null)
            {
                closingBalance = balanceInfo.Balance;
                if (model.AdjustmentId == 0)
                {
                    if (model.Quantity > (closingBalance - model.SumQty))
                    {
                        message = model.Message = "Insufficient balance.";
                        return message;
                    }
                }
                else
                {
                    var existingAdj = _invCommonService.INVUnit.AdjustmentItemRepository.GetAll().Where(x => x.ItemId == model.ItemId && x.AdjustmentId == model.AdjustmentId)
                        .GroupBy(x => x.ItemId)
                                                        .Select(i =>
                                                                new
                                                                {
                                                                    ItemId = i.Key,
                                                                    AdjQuantity = i.Sum(s => s.Quantity)
                                                                }).FirstOrDefault();

                    decimal existingAdjBalance = 0;
                    if (existingAdj != null)
                        existingAdjBalance = existingAdj.AdjQuantity;

                    var tempBalance = closingBalance + existingAdjBalance;
                    if (model.Quantity > (tempBalance - model.SumQty))
                    {
                        message = model.Message = "Insufficient balance.";
                        return message;
                    }
                }
            }
            else
            {
                message = model.Message = "Insufficient balance.";
                return message;
            }
                

            

            return message;
        }
        private void PopulateList(AdjustmentViewModel model)
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

            model.CategoryList = Common.PopulateDllList(_invCommonService.INVUnit.CategoryRepository.GetAll().OrderBy(x => x.Name));
            model.ColorList = Common.PopulateDllList(_invCommonService.INVUnit.ColorRepository.GetAll().OrderBy(x => x.Name));
            model.ModelList = Common.PopulateDllList(_invCommonService.INVUnit.ModelRepository.GetAll().OrderBy(x => x.Name));
            model.UnitList = Common.PopulateDllList(_invCommonService.INVUnit.UnitRepository.GetAll().OrderBy(x => x.Name));
            model.StatusList = Common.PopulateDllList(_invCommonService.INVUnit.ItemStatusRepository.GetAll().OrderBy(x => x.Name));

            //Item status default value
            model.ItemStatusId = 1;

        }

        #endregion

        public ActionResult AddDetail(AdjustmentDetailViewModel model)
        {
            var master = new AdjustmentViewModel();
            master.AdjustmentItemDetail = new List<AdjustmentDetailViewModel>();

            master.AdjustmentItemDetail.Add(model);
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
        public JsonResult GetEmployeeInfo(AdjustmentViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var AdjustedBy = string.Empty;
                    var AdjustedByEmpId = string.Empty;
                    var ApprovedBy = string.Empty;
                    var ApprovedByEmpId = string.Empty;

                    var objAdjustedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.AdjustedByEmpId);
                    var objApprovedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ApprovedByEmpId);

                    if (objAdjustedBy != null)
                    {
                        AdjustedBy = objAdjustedBy.EmpID + " - " + objAdjustedBy.FullName;
                        AdjustedByEmpId = objAdjustedBy.Id.ToString();
                    }
                    if (objApprovedBy != null)
                    {
                        ApprovedBy = objApprovedBy.EmpID + " - " + objApprovedBy.FullName;
                        ApprovedByEmpId = objApprovedBy.Id.ToString();
                    }

                    return Json(new
                    {
                        AdjustedBy = AdjustedBy,
                        AdjustedByEmpId = AdjustedByEmpId,
                        ApprovedBy = ApprovedBy,
                        ApprovedByEmpId = ApprovedByEmpId

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

        public string GetItemList(int? typeId, int? categoryId, int? modelId, int? unitId, int? colorId)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });
            var items = new List<INV_ItemInfo>();

            items = (from entity in _invCommonService.INVUnit.ItemInfoRepository.Fetch()
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

        public ActionResult DownloadFile(int adjustmentId)
        {
            try
            {
                var adjustmentInfo = _invCommonService.INVUnit.AdjustmentInfoRepository.GetByID(adjustmentId);
                string fileName = string.Empty;
                byte[] fileData = null;
                if (adjustmentInfo != null)
                {
                    if (!string.IsNullOrEmpty(adjustmentInfo.FileName))
                    {
                        fileName = adjustmentInfo.FileName;
                        fileData = adjustmentInfo.Attachment;
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

        private INV_AdjustmentInfo ToAttachFile(INV_AdjustmentInfo adjustmentInfo, HttpFileCollectionBase files)
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
                    adjustmentInfo.Attachment = fileData;
                    adjustmentInfo.FileName = file.FileName;
                }
            }

            return adjustmentInfo;
        }

        public ActionResult VoucherPosing(int id)
        {
            string url = string.Empty;
            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = "";
            string Username = "";
            string ZoneID = "";
            if (sessionUser != null)
            {
                UserID = sessionUser.UserId;
                password = sessionUser.Password;
                Username = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneID = MyAppSession.ZoneInfoId.ToString();
            }

            var obj = _invContext.INV_uspAdjustmentVoucherPosting(id).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }
    }

}