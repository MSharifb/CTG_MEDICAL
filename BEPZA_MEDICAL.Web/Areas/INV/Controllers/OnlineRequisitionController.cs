﻿using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;
using System.Data.SqlClient;
using System.Collections;
using BEPZA_MEDICAL.Web.Resources;
using System.Web.Script.Serialization;
using BEPZA_MEDICAL.DAL.PRM;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class OnlineRequisitionController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        private readonly EmployeeService _empService;
        #endregion

        #region Ctor
        public OnlineRequisitionController(PRMCommonSevice prmCommonService,EmployeeService empService, INVCommonService invCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._empService = empService;
            this._invCommonService = invCommonService;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            RequisitionInfoViewModel model = new RequisitionInfoViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, RequisitionInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from ReqInfo in _invCommonService.INVUnit.RequisitionInfoRepository.GetAll()
                        join AS in _prmCommonService.PRMUnit.ApprovalStatusRepository.GetAll() on ReqInfo.ApprovalStatusId equals AS.Id
                        where (ReqInfo.ZoneInfoId == LoggedUserZoneInfoId && ReqInfo.IsOnline == true)
                        select new RequisitionInfoViewModel()
                        {
                            Id = ReqInfo.Id,
                            ToEmpId = ReqInfo.PRM_EmploymentInfo.EmpID,
                            IssuedTo = ReqInfo.PRM_EmploymentInfo.EmpID + " - " + ReqInfo.PRM_EmploymentInfo.FullName,
                            Comment = ReqInfo.Comment,
                            IndentDate = ReqInfo.IndentDate,
                            IndentNo = ReqInfo.IndentNo,
                            TotalQuantity = ReqInfo.TotalQuantity.Value,
                            Status = AS.StatusName,
                            FullName = ReqInfo.PRM_EmploymentInfo.FullName,
                            IsProcessed = ReqInfo.IsProcessed
                        }).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrWhiteSpace(viewModel.ToEmpId))
                {
                    list = list.Where(x => x.EmpID.Contains(viewModel.ToEmpId) || x.FullName.ToLower().Contains(viewModel.ToEmpId.Trim().ToLower())).ToList();
                }
                if (viewModel.IndentDateFrom != null && viewModel.IndentDateTo != null)
                {
                    list = list.Where(x => x.IndentDate >= viewModel.IndentDateFrom && x.IndentDate <= viewModel.IndentDateTo).ToList();
                }
                if (!String.IsNullOrWhiteSpace(viewModel.IndentNo))
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.IndentDateFrom,
                    d.IndentDateTo,
                    d.EmpID,
                    d.IndentNo,
                    Convert.ToDateTime(d.IndentDate).ToString(DateAndTime.GlobalDateFormat),
                    d.IssuedTo,
                    d.TotalQuantity,
                    d.Comment,
                    d.Status,
                    d.IsProcessed,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            RequisitionInfoViewModel model = new RequisitionInfoViewModel();
            model.strMode = "create";
            int empInfoId = 0;
            model.EmpID = User.Identity.Name;
            model.ByEmpId = model.EmpID;
            model.IssuedToEmpId = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.EmpID == model.EmpID).Select(x => x.Id).FirstOrDefault();
            model.IssuedByEmpId = model.IssuedToEmpId;
            var ToEmployee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(e => e.EmpID == model.EmpID).ToList();
            if (ToEmployee != null)
            {
                foreach (var d in ToEmployee)
                {
                    model.ToEmpId = d.EmpID + " - " + d.FullName;
                    model.ByEmpId = d.EmpID + " - " + d.FullName;
                    model.Designation = d.PRM_Designation.Name;
                    model.Department = d.DivisionId == null ? string.Empty : d.PRM_Division.Name;
                    model.JoiningDate = d.DateofJoining.ToShortDateString();
                    model.ConfirmationDate = Convert.ToDateTime(d.DateofConfirmation).ToShortDateString();
                    empInfoId = d.Id;
                    model.IsPhotoExist = _prmCommonService.PRMUnit.EmployeePhotoGraphRepository.GetAll().Where(x => x.EmployeeId == d.Id).Select(x => x.IsPhoto).FirstOrDefault();
                }  
                //model.EmpImg = GetEmployeePhoto(Convert.ToInt32(model.IssuedToEmpId));
            }
            //if (empInfoId != 0)
            //{
            //    model.EmpImg = _prmCommonService.PRMUnit.EmployeePhotoGraphRepository.GetAll().Where(p => p.EmployeeId == empInfoId).Select(p => p.PhotoSignature).FirstOrDefault();
            //}
            #region Approval Flow Info
            var apvInfo = _prmCommonService.PRMUnit.ApprovalFlowConfigurationRepository.GetAll().Where(x => x.APV_ApprovalProcess.ProcessNameEnum.Contains("INVReq")).FirstOrDefault();
            if (apvInfo != null)
            {
                model.IsConfigurableApprovalFlow = apvInfo.IsConfigurableApprovalFlow;
            }
            #endregion

            GenerateIndentNo(model);
            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RequisitionInfoViewModel model)
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
                        model.IsOnline = true;
                        var master = model.ToEntity();
                        master.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Submit")).FirstOrDefault().Id;

                        foreach (var c in model.RequisitionDetail)
                        {
                            master.INV_RequisitionItem.Add
                            (new INV_RequisitionItem
                            {
                                ItemId = c.ItemId,
                                Quantity = c.Quantity,
                                RequisitionId = c.RequisitionId,
                                Comment = c.Comment
                            }
                            );
                        }

                        _invCommonService.INVUnit.RequisitionInfoRepository.Add(master);
                        _invCommonService.INVUnit.RequisitionInfoRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        const int isOnlineApplication = 1;
                        _prmCommonService.PRMUnit.FunctionRepository.InitializeApprovalProcess("INVReq", model.EmpID, master.Id, isOnlineApplication, model.ForwardToEmpId, master.IUser);
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
            var master = _invCommonService.INVUnit.RequisitionInfoRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";
            //int issuedId = _invCommonService.INVUnit.IssueInfoRepository.GetAll().Where(x => x.IndentId == id).Select(x => x.Id).FirstOrDefault();
            //if(issuedId>0)
            //{
            //    RequisitionInfoViewModel viewmodel = new RequisitionInfoViewModel();
            //    viewmodel.IsError = 1;
            //    viewmodel.errClass = "failed";
            //    viewmodel.ErrMsg = "Cannot be updated. This requisition is already been issued.";
            //    return View("Index", viewmodel);
            //}
            
            if (master.INV_RequisitionItem != null)
            {
                model.RequisitionDetail = new List<RequisitionDetailViewModel>();

                foreach (var item in master.INV_RequisitionItem)
                {
                    var itemDetail = item.ToModel();
                    itemDetail.Item = item.INV_ItemInfo.ItemName;
                    itemDetail.Unit = item.INV_ItemInfo.INV_Unit == null ? string.Empty : item.INV_ItemInfo.INV_Unit.Name;

                    model.RequisitionDetail.Add(itemDetail);
                }
            }
            var ToEmployee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.IssuedToEmpId);
            if (ToEmployee != null)
            {
                model.IssuedByEmpId = model.IssuedToEmpId;
                model.EmpID = ToEmployee.EmpID;
                model.ToEmpId = ToEmployee.EmpID + " - " + ToEmployee.FullName;
                model.ByEmpId = ToEmployee.EmpID + " - " + ToEmployee.FullName;
                model.Designation = ToEmployee.PRM_Designation.Name;
                model.Department = ToEmployee.DivisionId == null ? string.Empty : ToEmployee.PRM_Division.Name ;
                model.JoiningDate = ToEmployee.DateofJoining.ToShortDateString();
                model.ConfirmationDate = Convert.ToDateTime(ToEmployee.DateofConfirmation).ToShortDateString();
                //model.EmpImg = ToEmployee.PRM_EmpPhoto.;
                model.IsPhotoExist = _prmCommonService.PRMUnit.EmployeePhotoGraphRepository.GetAll().Where(x => x.EmployeeId == ToEmployee.Id).Select(x => x.IsPhoto).FirstOrDefault();
                model.Status = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(s=>s.Id == model.ApprovalStatusId).Select(x => x.StatusName).FirstOrDefault();
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
        public ActionResult Edit(RequisitionInfoViewModel model)
        {
            var checkoutBusinessLogic = string.Empty;
            int issuedId = _invCommonService.INVUnit.IssueInfoRepository.GetAll().Where(x => x.IndentId == model.Id).Select(x => x.Id).FirstOrDefault();
            if (issuedId > 0)
            {
                RequisitionInfoViewModel viewmodel = new RequisitionInfoViewModel();
                viewmodel.IsError = 1;
                viewmodel.errClass = "failed";
                viewmodel.ErrMsg = "Update failed. This requisition has already been issued.";
                return View("Index", viewmodel);
            }
            try
            {
                checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;
                        model.IsOnline = true;
                        var master = model.ToEntity();
                        ArrayList arrtyList = new ArrayList();
                        decimal qtyCounter = 0;
                        if (model.RequisitionDetail != null)
                        {
                            foreach (var detail in model.RequisitionDetail)
                            {
                                var child = new INV_RequisitionItem()
                                {
                                    Id = detail.Id,
                                    RequisitionId = master.Id,
                                    ItemId = detail.ItemId,
                                    Quantity = detail.Quantity,
                                    Comment = detail.Comment
                                };
                                qtyCounter += detail.Quantity;
                                // if old item then reflection will retrive old IUser & IDate
                                arrtyList.Add(child);
                            }
                        }
                        master.TotalQuantity = qtyCounter;
                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(INV_RequisitionItem), arrtyList);
                        master.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Submit")).FirstOrDefault().Id;

                        _invCommonService.INVUnit.RequisitionInfoRepository.Update(master, NavigationList);
                        _invCommonService.INVUnit.RequisitionInfoRepository.SaveChanges();

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
                int issuedId = _invCommonService.INVUnit.IssueInfoRepository.GetAll().Where(x => x.IndentId == id).Select(x=>x.Id).FirstOrDefault();
                if(issuedId==0 || issuedId == null)
                 {
                  _invCommonService.INVUnit.RequisitionItemRepository.Delete(x => x.RequisitionId == id);
                  _invCommonService.INVUnit.RequisitionItemRepository.SaveChanges();
                  _invCommonService.INVUnit.RequisitionInfoRepository.Delete(id);
                  _invCommonService.INVUnit.RequisitionInfoRepository.SaveChanges();
                  result = true;
                  errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                 }
                else
                {
                    result = false;
                    errMsg = "Cannot be deleted! This requisition is already been issued.";
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
                int requisitionId = _invCommonService.INVUnit.RequisitionItemRepository.GetAll().Where(x => x.Id == id).Select(x => x.RequisitionId).FirstOrDefault();
                int issuedId = _invCommonService.INVUnit.IssueInfoRepository.GetAll().Where(x => x.IndentId == requisitionId).Select(x => x.Id).FirstOrDefault();
                int itemId = _invCommonService.INVUnit.RequisitionItemRepository.GetAll().Where(x=>x.Id == id).Select(x=>x.ItemId).FirstOrDefault();
                int issuedItemId = _invCommonService.INVUnit.IssueItemRepository.GetAll().Where(x => x.IssueId == issuedId && x.ItemId == itemId).Select(x=>x.Id).FirstOrDefault();
                if (issuedItemId == 0 || issuedItemId == null)
                {
                    _invCommonService.INVUnit.RequisitionItemRepository.Delete(id);
                    _invCommonService.INVUnit.RequisitionItemRepository.SaveChanges();
                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
                else
                {
                    result = false;
                    errMsg = "Cannot be deleted! This item is already been issued.";
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

        #endregion

        #region Private Method
        [NoCache]
        public void GenerateIndentNo(RequisitionInfoViewModel model)
        {
            string newIndentNo = string.Empty;
            var zoneId = LoggedUserZoneInfoId.ToString();
            var maxId = 1;
            var zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().Where(x => x.Id == LoggedUserZoneInfoId).FirstOrDefault().ZoneCode.Trim();
            var RequisitionInfo = _invCommonService.INVUnit.RequisitionInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x => x.IndentNo);
            if (RequisitionInfo.Any())
            {
                maxId = Convert.ToInt32(RequisitionInfo.FirstOrDefault().IndentNo.Replace(zoneCode, ""));
                maxId += 1;
            }
            newIndentNo = maxId.ToString();
            if (newIndentNo.ToString().Length < 7)
            {
                newIndentNo = newIndentNo.PadLeft(7, '0');
            }

            model.IndentNo = string.Concat(zoneCode, newIndentNo);

        }

        private string CheckingBusinessLogicValidation(RequisitionInfoViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                var requsitionInfo = _invCommonService.INVUnit.RequisitionInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();
                if (requsitionInfo.Where(x => x.IndentNo == model.IndentNo && x.Id != model.Id && x.IsOnline == true).Any())
                {
                    GenerateIndentNo(model);
                    message = model.Message = "Duplicate Indent #. Please try again.";
                    return message;
                }
            }

            if (model.RequisitionDetail != null && model.RequisitionDetail.Count() > 0)
            {
                foreach (var item in model.RequisitionDetail)
                {
                    model.TotalQuantity += item.Quantity;
                }
            }
            else
            {
                message = model.Message = "Please add item(s) for Requisition.";
                return message;
            }

            return message;
        }
        private void PopulateList(RequisitionInfoViewModel model)
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

            var employeeList = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(model.EmpID, "INVReq");

            model.ForwardToList = Common.PopulateEmployeeDDL(employeeList);

        }

        #endregion

        #region Others
        public PartialViewResult AddDetail(RequisitionDetailViewModel model)
        {
            var master = new RequisitionInfoViewModel();
            master.RequisitionDetail = new List<RequisitionDetailViewModel>();
            master.RequisitionDetail.Add(model);

            return PartialView("_Details", master);
        }

        [NoCache]
        public JsonResult GetEmployeeInfoTemp(int empId)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID + " - " + obj.FullName,
                Designation = obj.PRM_Designation.Name,
                EmployeeName = obj.FullName,
                DateofJoining = obj.DateofJoining.ToString("yyyy-MM-dd"),
                IsContractual = obj.IsContractual,
                PreviousEmploymentStatusId = obj.EmploymentStatusId,
                PreviousEmploymentStatus = obj.PRM_EmploymentType.Name,
                MetterNo = string.Empty,
                DepartmentName = obj.PRM_Division.Name
            });
        }

        public FileContentResult GetEmployeePhoto(int? id, bool isPhoto)
        {
            PRM_EmpPhoto toDisplay = null;
            if (id != null && id != 0 && isPhoto != null)
            {
                toDisplay = _empService.GetEmployeePhoto((int)id, (bool)isPhoto);
            }
            if (toDisplay != null)
            {
                return File(toDisplay.PhotoSignature, "image/jpeg");
            }
            else
            {
                return null;
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

        #endregion
    }
}