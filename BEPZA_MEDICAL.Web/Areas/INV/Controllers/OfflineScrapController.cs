using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval;
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
    public class OfflineScrapController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        private readonly string ApprovalProcessEnum = @"InvScrap";
        #endregion

        #region Ctor
        public OfflineScrapController(PRMCommonSevice prmCommonService, INVCommonService invCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            ScrapViewModel model = new ScrapViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ScrapSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            //if (request.Searching)
            //{
            //    if (viewModel != null)
            //        filterExpression = viewModel.GetFilterExpression();
            //}

            //totalRecords = _invCommonService.INVUnit.ScrapInfoRepository.GetCount(filterExpression);
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var list = _invCommonService.INVUnit.ScrapInfoRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId && x.IsOnline == false).ToList();

            //totalRecords = list == null ? 0 : list.Count;
            if (request.Searching)
            {
                if (!String.IsNullOrWhiteSpace(viewModel.ByEmpID))
                {
                    list = list.Where(x => x.PRM_EmploymentInfo.EmpID.Contains(viewModel.ByEmpID) || x.PRM_EmploymentInfo.FullName.ToLower().Contains(viewModel.ByEmpID.Trim().ToLower())).ToList();
                }
                if (viewModel.ScrapDateFrom != null && viewModel.ScrapDateTo != null)
                {
                    list = list.Where(x => x.ScrapDate >= viewModel.ScrapDateFrom && x.ScrapDate <= viewModel.ScrapDateTo).ToList();
                }
                if (!String.IsNullOrWhiteSpace(viewModel.ScrapNo))
                {
                    list = list.Where(x => x.ScrapNo.Trim().ToLower().Contains(viewModel.ScrapNo.Trim().ToLower())).ToList();
                }
            }

            foreach (var d in list)
            {
                var ByEmpID = d.PRM_EmploymentInfo.EmpID;
                var IssuedBy = d.PRM_EmploymentInfo.EmpID + " - " + d.PRM_EmploymentInfo.FullName;
                var Comment = d.Comment;
                var ScrapDate = d.ScrapDate.ToString("dd-MMM-yyyy");
                var ScrapNo = d.ScrapNo;
                var TotalQuantity = d.TotalQuantity;

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    viewModel.ScrapDateFrom,
                    viewModel.ScrapDateTo,
                    ByEmpID,
                    ScrapNo,
                    ScrapDate,
                    IssuedBy,
                    TotalQuantity,
                    Comment,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ScrapViewModel model = new ScrapViewModel();
            model.strMode = "create";
            GenerateScrapNo(model);
            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ScrapViewModel model)
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
                        model.IsOnline = false;
                        var master = model.ToEntity();

                        int submitStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Submit")).FirstOrDefault().Id;

                        if (master.ApprovalStatusId == null || master.ApprovalStatusId == 0)
                        {
                            master.ApprovalStatusId = submitStatusId;
                        }

                        foreach (var c in model.ScrapDetail)
                        {
                            master.INV_ScrapItem.Add
                            (new INV_ScrapItem
                            {
                                ItemId = c.ItemId,
                                Quantity = c.Quantity,
                                ScrapId = c.ScrapId,
                                Comment = c.Comment,
                                ItemStatusId = c.ItemStatusId
                            }
                            );
                        }

                        _invCommonService.INVUnit.ScrapInfoRepository.Add(master);
                        _invCommonService.INVUnit.ScrapInfoRepository.SaveChanges();

                        _prmCommonService.PRMUnit.FunctionRepository.InitializeApprovalProcess(ApprovalProcessEnum, model.EmpID, master.Id, 0, model.ApproverId, master.IUser);

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
            var master = _invCommonService.INVUnit.ScrapInfoRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";



            if (master.INV_ScrapItem != null)
            {
                model.ScrapDetail = new List<ScrapDetailViewModel>();

                foreach (var item in master.INV_ScrapItem)
                {
                    var itemDetail = item.ToModel();
                    itemDetail.Item = item.INV_ItemInfo.ItemName;
                    itemDetail.Unit = item.INV_ItemInfo.INV_Unit == null ? string.Empty : item.INV_ItemInfo.INV_Unit.Name;

                    model.ScrapDetail.Add(itemDetail);
                }
            }

            var ByEmployee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.IssuedByEmpId);
            if (ByEmployee != null)
            {
                model.ByEmpId = ByEmployee.EmpID + " - " + ByEmployee.FullName;
            }
            var ToEmployee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.IssuedToEmpId);
            if (ToEmployee != null)
            {
                model.ToEmpId = ToEmployee.EmpID + " - " + ToEmployee.FullName;
                model.Designation = ToEmployee.PRM_Designation.Name;
                model.Department = ToEmployee.PRM_Division == null ? String.Empty : ToEmployee.PRM_Division.Name;
                model.EmpID = ToEmployee.EmpID;
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
        public ActionResult Edit(ScrapViewModel model)
        {
            var checkoutBusinessLogic = string.Empty;
            int returnId = _invCommonService.INVUnit.IssueReturnInfoRepository.GetAll().Where(x => x.ScrapId == model.Id).Select(x => x.Id).FirstOrDefault();
            if (returnId > 0)
            {
                ScrapViewModel viewmodel = new ScrapViewModel();
                viewmodel.IsError = 1;
                viewmodel.errClass = "failed";
                viewmodel.ErrMsg = "Update failed. This scrap has already been returned.";
                return View("Index", viewmodel);
            }
            try
            {
                checkoutBusinessLogic = CheckingBusinessLogicValidation(model);
                var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.IssuedToEmpId);
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;
                        model.IsOnline = false;
                        var master = model.ToEntity();
                        int submitStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Submit")).FirstOrDefault().Id;
                        int approvalStatusId = 0;
                        int.TryParse(master.ApprovalStatusId.ToString(), out approvalStatusId);
                        if (approvalStatusId <= submitStatusId)
                        {
                            if (master.ApprovalStatusId == null || master.ApprovalStatusId == 0)
                            {
                                master.ApprovalStatusId = submitStatusId;
                            }

                            ArrayList arrtyList = new ArrayList();
                            decimal qtyCounter = 0;
                            if (model.ScrapDetail != null)
                            {
                                foreach (var detail in model.ScrapDetail)
                                {
                                    var child = new INV_ScrapItem()
                                    {
                                        Id = detail.Id,
                                        ScrapId = master.Id,
                                        ItemId = detail.ItemId,
                                        Quantity = detail.Quantity,
                                        Comment = detail.Comment,
                                        ItemStatusId = detail.ItemStatusId
                                    };
                                    qtyCounter += detail.Quantity;
                                    // if old item then reflection will retrive old IUser & IDate
                                    arrtyList.Add(child);
                                }
                            }
                            master.TotalQuantity = qtyCounter;
                            Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                            NavigationList.Add(typeof(INV_ScrapItem), arrtyList);

                            _invCommonService.INVUnit.ScrapInfoRepository.Update(master, NavigationList);
                            _invCommonService.INVUnit.ScrapInfoRepository.SaveChanges();

                            _prmCommonService.PRMUnit.FunctionRepository.InitializeApprovalProcess(ApprovalProcessEnum, empInfo.EmpID, master.Id, 0, model.ApproverId, master.IUser);

                            model.IsError = 0;
                            model.errClass = "success";
                            model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                            PopulateList(model);
                            //return RedirectToAction("Index");
                            return RedirectToAction("Edit", new { id = master.Id, type = "success" });
                        }
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
                int returnId = _invCommonService.INVUnit.IssueReturnInfoRepository.GetAll().Where(x => x.ScrapId == id).Select(x => x.Id).FirstOrDefault();
                if (returnId == 0 || returnId == null)
                {
                    _invCommonService.INVUnit.ScrapItemRepository.Delete(x => x.ScrapId == id);
                    _invCommonService.INVUnit.ScrapItemRepository.SaveChanges();
                    _invCommonService.INVUnit.ScrapInfoRepository.Delete(id);
                    _invCommonService.INVUnit.ScrapInfoRepository.SaveChanges();
                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
                else
                {
                    result = false;
                    errMsg = "Delete failed! This scrap has already been returned.";
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
                int ScrapId = _invCommonService.INVUnit.ScrapItemRepository.GetAll().Where(x => x.Id == id).Select(x => x.ScrapId).FirstOrDefault();
                int returnId = _invCommonService.INVUnit.IssueReturnInfoRepository.GetAll().Where(x => x.ScrapId == ScrapId).Select(x => x.Id).FirstOrDefault();
                int itemId = _invCommonService.INVUnit.ScrapItemRepository.GetAll().Where(x => x.Id == id).Select(x => x.ItemId).FirstOrDefault();
                int returnedItemId = _invCommonService.INVUnit.IssueReturnItemRepository.GetAll().Where(x => x.IssueReturnId == returnId && x.ItemId == itemId).Select(x => x.Id).FirstOrDefault();
                if (returnedItemId == 0 || returnedItemId == null)
                {
                    _invCommonService.INVUnit.ScrapItemRepository.Delete(id);
                    _invCommonService.INVUnit.ScrapItemRepository.SaveChanges();
                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
                else
                {
                    result = false;
                    errMsg = "Cannot be deleted! This item is already been returned.";
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
        public void GenerateScrapNo(ScrapViewModel model)
        {
            string newScrapNo = string.Empty;
            var zoneId = LoggedUserZoneInfoId.ToString();
            var maxId = 1;
            var zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().Where(x => x.Id == LoggedUserZoneInfoId).FirstOrDefault().ZoneCode.Trim();
            var ScrapInfo = _invCommonService.INVUnit.ScrapInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x => x.ScrapNo);
            if (ScrapInfo.Any())
            {
                maxId = Convert.ToInt32(ScrapInfo.FirstOrDefault().ScrapNo.Replace(zoneCode, ""));
                maxId += 1;
            }
            newScrapNo = maxId.ToString();
            if (newScrapNo.ToString().Length < 7)
            {
                newScrapNo = newScrapNo.PadLeft(7, '0');
            }

            model.ScrapNo = string.Concat(zoneCode, newScrapNo);

        }

        private string CheckingBusinessLogicValidation(ScrapViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                var scrapInfo = _invCommonService.INVUnit.ScrapInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();
                if (scrapInfo.Where(x => x.ScrapNo == model.ScrapNo && x.Id != model.Id && x.IsOnline == false).Any())
                {
                    GenerateScrapNo(model);
                    message = model.Message = "Duplicate Scrap #. Please try again.";
                    return message;
                }
            }

            if (model.ScrapDetail != null && model.ScrapDetail.Count() > 0)
            {
                foreach (var item in model.ScrapDetail)
                {
                    model.TotalQuantity += item.Quantity;
                }

                var scrapItems = model.ScrapDetail.GroupBy(x => x.ItemId)
                                               .Select(i =>
                                                       new
                                                       {
                                                           ItemId = i.Key,
                                                           Quantity = i.Sum(s => s.Quantity)
                                                       }).ToList();

                foreach (var item in scrapItems)
                {
                    decimal issuedQty = 0;
                    decimal returnedQty = 0;
                    var existingIssueInfo = _invCommonService.INVUnit.IssueItemRepository.Fetch().Where(x => x.ItemId == item.ItemId && x.INV_IssueInfo.IssuedToId == model.IssuedToEmpId).GroupBy(x => x.ItemId)
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

                    var existingReturnInfo = _invCommonService.INVUnit.IssueReturnItemRepository.Fetch().Where(x => x.ItemId == item.ItemId && x.INV_IssueReturnInfo.ReceivedFromEmpId == model.IssuedToEmpId).GroupBy(x => x.ItemId)
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


                    if (item.Quantity > (issuedQty - returnedQty))
                    {
                        message = "Return quantity must be less than or equal to existing issued quantity.";
                        return message;
                    }

                }
            }
            else
            {
                message = model.Message = "Please add item(s) for Scrap.";
                return message;
            }

            return message;
        }

        private string AddItemValidation(ScrapDetailViewModel model)
        {
            string message = string.Empty;

            if (model.EmployeeId == 0)
            {
                message = "Please select employee first.";
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

            if (model.Quantity > (issuedQty - returnedQty - model.SumQty))
            {
                message = "Return quantity must be less than or equal to existing issued quantity.";
                return message;
            }

            return message;
        }
        private void PopulateList(ScrapViewModel model)
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
            if (model.Id > 0)
            {
                var approverList = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(model.EmpID, ApprovalProcessEnum);
                model.ApproverList = Common.PopulateEmployeeDDL(approverList);
                int? approvalStepId = approverList != null && approverList.Count > 0 ? approverList.DistinctBy(q => q.InitialStepId).FirstOrDefault().InitialStepId : 0;
                model.ApprovalStepId = (int)approvalStepId;
            }
            else
            {
                model.ApproverList = new List<SelectListItem>();
            }
            //Item status default value
            model.ItemStatusId = 1;
            BindApprovalHistory(model);
        }

        private void BindApprovalHistory(ScrapViewModel model)
        {
            int approvalProcessId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == ApprovalProcessEnum).FirstOrDefault().Id;
            var history = _prmCommonService.PRMUnit.ApprovalHistoryRepository.Get(q => q.ApplicationId == model.Id && q.ApprovalProcesssId == approvalProcessId).DefaultIfEmpty().OfType<vwApvApplicationWiseApprovalStatu>().ToList();
            var approvalHistory = new List<ApprovalHistoryViewModel>();

            foreach (var item in history)
            {
                var historyObj = new ApprovalHistoryViewModel
                {
                    ApprovalStepName = item.StepSequence,
                    ApproverComment = item.ApproverComments == null ? string.Empty : item.ApproverComments,
                    ApprovalStatus = item.ApprovalStatusName,
                    ApproverIdAndName = item.ApproverIdAndName,
                };
                approvalHistory.Add(historyObj);
            }

            model.ApprovalHistory = approvalHistory;
        }

        #endregion

        public ActionResult AddDetail(ScrapDetailViewModel model)
        {
            var master = new ScrapViewModel();
            master.ScrapDetail = new List<ScrapDetailViewModel>();
            master.ScrapDetail.Add(model);

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
                DepartmentName = obj.DivisionId == null ? "" : obj.PRM_Division.Name,
                EmpIdStr = obj.EmpID
            });
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

        #region Approval

        public JsonResult GetApproverInfo(string employeeId)
        {
            var employeeList = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(employeeId, "InvScrap");
            var empInfoList = new List<BEPZA_MEDICAL.DAL.PRM.PRM_EmploymentInfo>();
            foreach (var d in employeeList)
            {
                empInfoList.Add(new BEPZA_MEDICAL.DAL.PRM.PRM_EmploymentInfo
                {
                    Id = (Int32)d.Id,
                    FullName = d.FullName,
                });
            }
            return Json(empInfoList, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}