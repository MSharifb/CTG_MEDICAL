using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation;
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
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class ProjectEstimationController : BaseController
    {
        //
        // GET: /PMI/ProjectEstimation/
        #region Fields
        private readonly PMICommonService _pmiCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        //public int LoggedUserZoneInfoId
        //{
        //    get { return MyAppSession.ZoneInfoId; }
        //}

        #endregion

        #region Ctor
        public ProjectEstimationController(PMICommonService pmiCommonService, PRMCommonSevice prmCommonService)
        {
            this._pmiCommonService = pmiCommonService;
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EstimationListViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var loggedZoneId = LoggedUserZoneInfoId;

            List<EstimationListViewModel> list = (from x in _pmiCommonService.PMIUnit.EstimationViewRepository.Get(q => q.ZoneId == loggedZoneId).DefaultIfEmpty().OfType<vwPMIEstimation>().ToList()
                                                  select new EstimationListViewModel
                                                  {
                                                      Id = x.Id,
                                                      EstimationDetailId = x.EstimationDetailId,
                                                      ProjectId = x.ProjectId,
                                                      ProjectName = x.ProjectName,
                                                      EstimationItemId = x.EstimationItemId,
                                                      ItemName = x.ItemName,
                                                      ZoneCode = x.ZoneCode,
                                                      TotalAmount = x.TotalAmount,
                                                      EstimationDate = x.EstimationDate,
                                                      EstimationDateTo = x.EstimationDate,
                                                      BudgetAmount = x.EstimatedCost,
                                                      ZoneId = x.ZoneId,
                                                      Quantity = x.Quantity,
                                                      UnitPrice = x.UnitPrice,
                                                  }).DefaultIfEmpty().OfType<EstimationListViewModel>().ToList();

            list = list.OrderBy(q => q.Id).ThenBy(q => q.EstimationDetailId).ToList();

            totalRecords = list == null ? 0 : list.Count;

            if (request.Searching)
            {
                if (viewModel.ProjectId > 0)
                {
                    list = list.Where(x => x.ProjectId == viewModel.ProjectId).ToList();
                }


                if (viewModel.ItemName != "0")
                {
                    list = list.Where(t => t.ItemName == viewModel.ItemName).ToList();
                }


                if (viewModel.ZoneCode != "0")
                {
                    list = list.Where(x => x.ZoneCode.Contains(viewModel.ZoneCode)).ToList();
                }

                if (viewModel.EstimationDate != DateTime.MinValue && viewModel.EstimationDateTo != DateTime.MinValue)
                {
                    list = list.Where(t => t.EstimationDate >= viewModel.EstimationDate && t.EstimationDateTo <= viewModel.EstimationDateTo).ToList();
                }
            }

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EstimationDetailId,
                    d.ProjectId,
                    d.ProjectName,
                    d.EstimationItemId,
                    d.ItemName,
                    d.Quantity,
                    d.UnitPrice,
                    d.TotalAmount,
                    d.EstimationDate.ToString("dd/MM/yyyy"),
                    d.EstimationDateTo.ToString("dd/MM/yyyy"),
                    d.ZoneCode,
                    d.BudgetAmount,
                    "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        private string GetAccessableZones()
        {
            var zoneInfoSession = MyAppSession.ZoneInfoId;
            int LoggedUserZoneInfoId = 0;
            int.TryParse(zoneInfoSession.ToString(), out LoggedUserZoneInfoId);
            CustomMembershipProvider _provider = new CustomMembershipProvider();
            string userName = HttpContext.User.Identity.Name;
            var assignedZones = _provider.GetZoneList(userName, LoggedUserZoneInfoId);
            var accessableZone = (from zone in _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll()
                                  join aZ in assignedZones on zone.Id equals aZ.ZoneId
                                  where zone.Id == LoggedUserZoneInfoId
                                  select zone.ZoneCode).Distinct().FirstOrDefault();
            //var zones = string.Join(",", accessableZone);
            return accessableZone.ToString();
        }

        public ActionResult Create()
        {
            ProjectEstimationViewModel model = new ProjectEstimationViewModel();
            model.strMode = "Create";
            GenerateEstimationHead(model, 0);
            PopulateList(model);
            model.EstimationDate = DateTime.Now;

            var apvModel = new ApprovalFlowViewModel();
            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
            apvModel.DesignationList = Common.PopulateDllList(designationList);

            model.ApproverList.Add(apvModel); 


            return View("Create", model);
        }

        private void BindEstimationZoneInfo(ProjectEstimationViewModel model, List<PMI_BudgetZoneOrProject> selectedZones)
        {
            var zoneViewList = new List<EstimationZoneListViewModel>();
            var zoneList = (from zone in _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().DefaultIfEmpty().OfType<BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo>().ToList()
                            select zone).ToList();
            foreach (var item in zoneList)
            {
                var anItem = new EstimationZoneListViewModel();
                anItem.ZoneId = item.Id;
                anItem.ZoneName = item.ZoneName;
                if (selectedZones != null)
                {

                    anItem.IsSelected = (selectedZones.Where(t => t.ZoneOrProjectId == item.Id).SingleOrDefault()) == null ? false : true;
                }
                else
                {
                    anItem.IsSelected = false;
                }
                zoneViewList.Add(anItem);
            }
            model.ZoneList = zoneViewList;
        }


        private ProjectEstimationViewModel GenerateEstimationHead(ProjectEstimationViewModel model, int headCount)
        {
            var anEstimationDetail = new ProjectEstimationDetailsViewModel();
            var headDescriptionViewModel = new EstimationHeadDesciptionViewModel();

            var headList = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll();
            headDescriptionViewModel.HeadList = Common.PopulateEstimationHead(headList);

            model.ProjectEstimationDetails.Add(anEstimationDetail);
            var anItem = AddItemFields(model.DetailViewModel, headCount);
            model.EstimationItemList.Add(anItem);
            model.EstimationHeadList = Common.PopulateEstimationHead(headList);

            model.EstimationHeadDescriptions.Add(headDescriptionViewModel);
            return model;
        }

        public ActionResult AddNewEstimationHead(ProjectEstimationViewModel model, string previousHeadCount)
        {
            int prevHeadCount = 0;
            int.TryParse(previousHeadCount, out prevHeadCount);
            GenerateEstimationHead(model, prevHeadCount);
            return PartialView("_PartialHeadDetail", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(ProjectEstimationViewModel model)
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
                        var master = model.ToEntity();

                        var itemList = new List<PMI_EstimationDetails>();

                        foreach (var item in model.ZoneList)
                        {
                            if (item.IsSelected)
                            {
                                var obj = item.ToEntity();
                                obj.EstimationMasterId = master.Id;
                                _pmiCommonService.PMIUnit.EstimationZoneRepository.Add(obj);
                            }
                        }

                        foreach (var item in model.EstimationItemList)
                        {
                            var obj = item.ToEntity();
                            obj.MasterId = master.Id;
                            itemList.Add(obj);
                        }

                        foreach (var item in itemList)
                        {
                            _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Add(item);
                        }

                        foreach (var item in model.EstimationHeadDescriptions)
                        {
                            var obj = item.ToEntity();
                            obj.MasterId = master.Id;
                            obj.PMI_EstimationMaster = master;
                            _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.Add(obj);
                        }



                        _pmiCommonService.PMIUnit.ProjectEstimationRepository.Add(master);
                        _pmiCommonService.PMIUnit.ProjectEstimationRepository.SaveChanges();
                        _pmiCommonService.PMIUnit.EstimationZoneRepository.SaveChanges();
                        _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.SaveChanges();
                        _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.SaveChanges();

                        SaveApprovalFlow(master.Id, model.ApprovalStatusId, model.ApproverList);

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model = master.ToModel();
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
            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
        }


        private ProjectEstimationViewModel PrepareModel(ProjectEstimationViewModel model)
        {
            PopulateList(model);
            BindEstimationZoneInfo(model, null);
            var unitList = _pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll();
            foreach (var item in model.EstimationItemList)
            {
                item.UnitList = Common.PopulateDllList(unitList);
                item.EstimationItemList = Common.PopulateEstimationItemList(_pmiCommonService.PMIUnit.EstimationSetupRepository.Get(t => t.EstimationHeadId == item.EstimationHeadId));
            }

            return model;
        }

        public ActionResult Edit(int id)
        {
            var master = _pmiCommonService.PMIUnit.ProjectEstimationRepository.Get(t => t.Id == id).FirstOrDefault();
            var model = master.ToModel();
            GetEstimatedItemInformation(model);

            #region Approval Flow
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(model.ApprovalStatusId==null ? 0: model.ApprovalStatusId);
            var approverList = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.GetAll()
                                join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                join appStatus in _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll() on y.StatusId equals appStatus.Id
                                join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on y.EmployeeId equals emp.Id
                                where (x.BudgetProjectAPPId == id && x.PMI_ApprovalSection.Enum.Contains("ETN"))
                                select new ApprovalFlowViewModel
                                {
                                    Id = x.Id,
                                    EmployeeName = emp.FullName,
                                    EmpId = emp.EmpID,
                                    EmployeeId = emp.Id,
                                    DesignationId = emp.DesignationId,
                                    Remarks = y.Remarks,
                                    DesignationName = emp.PRM_Designation.Name,
                                    DepartmentName = emp.PRM_Division == null ? string.Empty : emp.PRM_Division.Name,
                                    Status = appStatus.Name.Contains("Submit") ? "Pending" : appStatus.Name
                                }).ToList();
            if (approverStatus.Name.Contains("Draft"))
            {
                foreach (var item in approverList)
                {
                    ApprovalFlowViewModel apModel = new ApprovalFlowViewModel();
                    apModel.Id = item.Id;
                    apModel.EmployeeId = item.EmployeeId;
                    apModel.DesignationId = item.DesignationId;
                    apModel.EmployeeList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.Id == item.EmployeeId)
                        .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();

                    apModel.DesignationList = Common.PopulateDllList(_prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == item.DesignationId).ToList());


                    model.ApproverList.Add(apModel);
                }

            }
            else
            {
                model.ApproverList = approverList;
            }
            model.ApprovalStatus = approverStatus == null ? string.Empty : approverStatus.Name;
            #endregion

            return View("Create", model);
        }

        private void GetEstimatedItemInformation(ProjectEstimationViewModel model)
        {
            var detailList = _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Get(t => t.MasterId == model.Id).DefaultIfEmpty().OfType<PMI_EstimationDetails>().ToList();
            var estimationItemList = _pmiCommonService.PMIUnit.EstimationSetupRepository.GetAll();
            var unitList = _pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll();
            var headList = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll();

            var projectWiseDescription = _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.Get(q => q.MasterId == model.Id).DefaultIfEmpty().OfType<PMI_EstimationHeadDescription>().ToList();

            PopulateList(model);

            var estimationHeadIds = detailList.DistinctBy(q => q.EstimationHeadId).Select(q => q.EstimationHeadId);
            if (estimationHeadIds != null && estimationHeadIds.Count() > 0)
            {
                foreach (var heads in estimationHeadIds)
                {
                    var anEstimationDetail = new ProjectEstimationDetailsViewModel();

                    anEstimationDetail.EstimationHeadId = heads;

                    foreach (var item in detailList.Where(q => q.EstimationHeadId == heads).ToList())
                    {
                        var estimationItems = estimationItemList.Where(q => q.EstimationHeadId == item.EstimationHeadId).DefaultIfEmpty().ToList();
                        var detailModel = item.ToModel();
                        detailModel.EstimationItemList = Common.PopulateEstimationItemList(estimationItems);
                        detailModel.UnitList = Common.PopulateDllList(unitList);
                        //detailModel.HeadList = Common.PopulateEstimationHead(headList);
                        model.EstimationItemList.Add(detailModel);
                    }

                    model.ProjectEstimationDetails.Add(anEstimationDetail);

                    var headDescriptionViewModel = new EstimationHeadDesciptionViewModel();
                    headDescriptionViewModel.HeadList = Common.PopulateEstimationHead(headList);
                    headDescriptionViewModel.EstimationHeadId = heads;
                    var description = string.Empty;
                    var projectDescObj = projectWiseDescription.Where(q => q.EstimationHeadId == heads).FirstOrDefault();
                    if (projectDescObj != null)
                    {
                        description = projectDescObj.HeadDescription;
                    }
                    else
                    {
                        description = headList.Where(q => q.Id == heads).FirstOrDefault().Description;
                    }
                    headDescriptionViewModel.HeadDescription = description;
                    model.EstimationHeadDescriptions.Add(headDescriptionViewModel);
                }
            }

            decimal budgetAmount = 0;
            var budgetInfo = (from cost in _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.GetAll()
                              join bd in _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll() on cost.BudgetDetailsId equals bd.Id
                              join prj in _pmiCommonService.PMIUnit.ProjectMasterRepository.GetAll() on bd.Id equals prj.APPDetailsId
                              join sts in _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll() on cost.BudgetStatusId equals sts.Id
                              where sts.Name.Contains("Approv") && prj.Id == model.ProjectId
                              select cost
                                  ).FirstOrDefault();
            if (budgetInfo != null)
            {
                decimal.TryParse(budgetInfo.EstematedCost.ToString(), out budgetAmount);
            }
            budgetAmount = budgetAmount * 100000;
            model.BudgetAmount = budgetAmount;

            decimal totalAmount = 0;
            if (detailList != null)
            {
                totalAmount = detailList != null ? detailList.Sum(t => t.TotalAmount) : 0;
            }
            model.TotalAmount = totalAmount;
            model.StatusName = _pmiCommonService.PMIUnit.ProjectStatusRepository.Get(q => q.Id == model.EstimationStatusId).FirstOrDefault().Name;
            model.strMode = "Update";
            model.ActionType = "Update";
        }

        [HttpPost]
        public ActionResult Update(ProjectEstimationViewModel model)
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

                        var existingItems = _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Get(q => q.MasterId == master.Id).ToList();
                        var deletedItems = (from existing in existingItems
                                            where !(model.EstimationItemList.Any(dt => dt.Id == existing.Id))
                                            select existing).DefaultIfEmpty().OfType<PMI_EstimationDetails>().ToList();

                        foreach (var item in deletedItems)
                        {
                            _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Delete(item.Id);
                        }

                        var existingDescriptions = _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.Get(q => q.MasterId == model.Id).DefaultIfEmpty().OfType<PMI_EstimationHeadDescription>().ToList();
                        var deletedDescriptions = (from existing in existingDescriptions
                                                   where !(model.EstimationHeadDescriptions.Any(dt => dt.Id == existing.Id))
                                                   select existing).DefaultIfEmpty().OfType<PMI_EstimationHeadDescription>().ToList();

                        foreach (var item in deletedDescriptions)
                        {
                            _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.Delete(item.Id);
                        }

                        foreach (var item in model.EstimationItemList)
                        {
                            var itemObj = item.ToEntity();
                            itemObj.EUser = model.EUser;
                            itemObj.EDate = model.EDate;
                            itemObj.MasterId = model.Id;
                            if (itemObj.Id > 0)
                            {
                                _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Update(itemObj);
                            }
                            else
                            {
                                _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Add(itemObj);
                            }
                        }

                        foreach (var item in model.EstimationHeadDescriptions)
                        {
                            var obj = item.ToEntity();
                            obj.MasterId = master.Id;
                            if (obj.Id > 0)
                            {
                                obj.EUser = HttpContext.User.Identity.Name;
                                obj.EDate = DateTime.Now;
                                _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.Update(obj);
                            }
                            else
                            {
                                _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.Add(obj);
                            }
                        }

                        _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.SaveChanges();
                        _pmiCommonService.PMIUnit.ProjectEstimationRepository.Update(master);
                        _pmiCommonService.PMIUnit.ProjectEstimationRepository.SaveChanges();
                        _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.SaveChanges();

                        SaveApprovalFlow(master.Id, model.ApprovalStatusId, model.ApproverList);

                        model.IsError = 0;
                        model.errClass = "success";
                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
            model.ErrMsg = model.Message;
            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            bool isApproved = false;

            try
            {
                var obj = _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.GetByID(id);
                if (obj != null)
                {
                    var master = _pmiCommonService.PMIUnit.ProjectEstimationRepository.GetByID(obj.MasterId);
                    if (master != null)
                    {
                        string status = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetByID(master.EstimationStatusId).Name;
                        if (status.ToLower().Contains("appr") == true)
                        {
                            result = false;
                            errMsg = "Approved Estimation can't be delete.";
                            isApproved = true;
                        }
                    }
                }

                if (!isApproved)
                {
                    _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Delete(id);
                    _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.SaveChanges();
                    var detailsObj = _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.GetByID(id);
                    if (detailsObj != null)
                    {
                        var estimationDetailsList = _pmiCommonService.PMIUnit.ProjectEstimationDetailsRepository.Get(t => t.MasterId == detailsObj.MasterId && t.Id != id).DefaultIfEmpty().OfType<PMI_EstimationDetails>().ToList();
                        if (estimationDetailsList == null)
                        {
                            _pmiCommonService.PMIUnit.ProjectEstimationRepository.Delete(detailsObj.MasterId);
                            _pmiCommonService.PMIUnit.ProjectZoneRepository.SaveChanges();
                            _pmiCommonService.PMIUnit.ProjectEstimationRepository.SaveChanges();
                        }

                    }

                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEstimationItemByHeadId(int estimationHeadId)
        {
            var itemList = (from item in _pmiCommonService.PMIUnit.EstimationSetupRepository.Get(t => t.EstimationHeadId == estimationHeadId).ToList()
                            join u in _pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll().ToList() on item.UnitId equals u.Id
                            select new EstimationItemInfo
                            {
                                EstimationHeadId = item.EstimationHeadId,
                                EstimationItemId = item.Id,
                                ItemName = string.IsNullOrWhiteSpace(item.ItemCode) ? item.ItemName : item.ItemCode + " : " + item.ItemName,
                                UnitId = item.UnitId,
                                UnitName = u.Name,
                                UnitPrice = item.UnitPrice,
                            }).ToList();

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetItemInformation(int itemId)
        {
            var itemInfo = _pmiCommonService.PMIUnit.EstimationSetupRepository.GetByID(itemId);
            var result = new PMI_EstimationSetup();
            if (itemInfo != null)
            {
                result.UnitId = itemInfo.UnitId;
                result.UnitPrice = itemInfo.UnitPrice;
                result.Id = itemInfo.Id;
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        #endregion

        #region Approval Flow

        public void SaveApprovalFlow(int BudgetProjectAPPId, int? approverStatusId, List<ApprovalFlowViewModel> approverList)
        {
            PMI_ApprovalFlowMaster ApprovalFlowMaster = new PMI_ApprovalFlowMaster();
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(approverStatusId);
            var tempPeriod = _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == BudgetProjectAPPId && x.PMI_ApprovalSection.Enum.Contains("ETN")).FirstOrDefault();
            if (tempPeriod != null && approverList.Count > 0)
            {
                List<Type> allTypes = new List<Type> { typeof(PMI_ApprovalFlowDetails) };
                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Delete(tempPeriod.Id, allTypes);
                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.SaveChanges();
            }

            if (approverList.Count > 0)
            {

                foreach (var item in approverList)
                {
                    PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();

                    ApprovalFlowDetails.ApprovalFlowMasterId = ApprovalFlowDetails.Id;
                    ApprovalFlowDetails.EmployeeId = (int)item.EmployeeId;
                    ApprovalFlowDetails.StatusId = approverStatusId.Value;
                    ApprovalFlowDetails.Remarks = string.Empty;
                    ApprovalFlowDetails.IUser = HttpContext.User.Identity.Name;
                    ApprovalFlowDetails.IDate = DateTime.Now;

                    _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Add(ApprovalFlowDetails);
                }

                ApprovalFlowMaster.BudgetProjectAPPId = BudgetProjectAPPId;
                ApprovalFlowMaster.ApprovalSectionId = _pmiCommonService.PMIUnit.ApprovalSectionRepository.GetAll().Where(x => x.Enum == "ETN").Select(s => s.Id).FirstOrDefault();
                ApprovalFlowMaster.CreateDate = DateTime.Now;
                ApprovalFlowMaster.IUser = HttpContext.User.Identity.Name;
                ApprovalFlowMaster.IDate = DateTime.Now;

                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Add(ApprovalFlowMaster);
                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.SaveChanges();
            }
        }

        #endregion

        #region Private Method



        private string CheckingBusinessLogicValidation(ProjectEstimationViewModel model)
        {
            string message = string.Empty;
            var obj = _pmiCommonService.PMIUnit.ProjectEstimationRepository.Get(q => q.Id == model.Id).FirstOrDefault();
            if (obj != null)
            {
                string statusName = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetByID(obj.EstimationStatusId).Name;
                if (statusName.ToLower().Contains("appr") == true)
                {
                    message = "Estimation is already approved.";
                }
            }
            return message;
        }

        private void PopulateList(ProjectEstimationViewModel model)
        {
            var estimationHeadList = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll();
            model.EstimationHeadList = Common.PopulateEstimationHead(estimationHeadList);
            decimal totalAmount = 0;
            var a = GetProjectInformation(model.ProjectId);
            decimal.TryParse(a.Data.ToString(), out totalAmount);
            model.TotalAmount = totalAmount;
            var fyList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();
            int currentFyId = 0;
            if (fyList != null)
            {
                var currentFy = fyList.Where(q => q.isActive == true).LastOrDefault();
                if (currentFy != null)
                {
                    int.TryParse(currentFy.id.ToString(), out currentFyId);
                }
                else
                {
                    currentFyId = fyList.Max(q => q.id);
                }

            }
            //var projectList = _pmiCommonService.PMIUnit.ProjectViewRepository.Get(q => q.ZoneId == LoggedUserZoneInfoId && q.FinancialYearId == currentFyId).DefaultIfEmpty().OfType<vwPMIProjectSummary>().ToList();
            var projectList = _pmiCommonService.PMIUnit.ProjectViewRepository.Get(q => q.ZoneId == LoggedUserZoneInfoId).DefaultIfEmpty().OfType<vwPMIProjectSummary>().ToList();
            projectList = projectList.Where(q => q.ProjectMasterId != 0).DefaultIfEmpty().OfType<vwPMIProjectSummary>().ToList();
            model.ProjectList = projectList
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.DescritionOfAPP,
                    Value = y.ProjectMasterId.ToString()
                }).ToList();

            var statusList = _pmiCommonService.PMIUnit.StatusInformationRepository.Get(t => t.ApplicableFor == "Estimation").ToList();
            model.EstimationStatusList = Common.PopulateDllList(statusList);


            var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => x.Name.Contains("Submit") || x.Name.Contains("Draft")).ToList();
            model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);
        }

        #endregion

        #region others


        public JsonResult GetBudgetAmountOf(int projectId)
        {
            decimal budgetAmount = 0;
            var budgetInfo = (from cost in _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.GetAll()
                              join bd in _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll() on cost.BudgetDetailsId equals bd.Id
                              join prj in _pmiCommonService.PMIUnit.ProjectMasterRepository.GetAll() on bd.Id equals prj.APPDetailsId
                              join sts in _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll() on cost.BudgetStatusId equals sts.Id
                              where sts.Name.Contains("Approv") && prj.Id == projectId
                              select cost
                                  ).FirstOrDefault();
            if (budgetInfo != null)
            {
                decimal.TryParse(budgetInfo.EstematedCost.ToString(), out budgetAmount);
            }
            budgetAmount = budgetAmount * 100000;
            var data = new
            {
                BudgetAmount = budgetAmount
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectInformation(int projectId)
        {
            var master = _pmiCommonService.PMIUnit.ProjectEstimationRepository.Get(t => t.ProjectId == projectId).FirstOrDefault();
            int masterId = 0;
            if (master != null)
            {
                masterId = master.Id;
            }
            var data = new
            {
                MasterId = masterId
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult GetHeadDescription(int EstimationHead, int estimationMasterId)
        {
            string description = string.Empty;
            if (estimationMasterId > 0)
            {
                var modifiedHeadInfo = _pmiCommonService.PMIUnit.EstimationHeadDescriptionRepository.Get(q => q.MasterId == estimationMasterId && q.EstimationHeadId == EstimationHead).DefaultIfEmpty().OfType<PMI_EstimationHeadDescription>().ToList();
                if (modifiedHeadInfo != null)
                {
                    description = modifiedHeadInfo.Select(q => q.HeadDescription).LastOrDefault();
                }
            }
            else
            {
                var master = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetByID(EstimationHead);
                if (master != null)
                {
                    var model = master.ToModel();
                    description = model.Description;
                }
            }
            var data = new
            {
                Description = description
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult LoadItemDescription(int itemId)
        {
            string description = string.Empty;
            var model = new EstimationSetupViewModel();
            return PartialView("_PartialItemDetail", model);
        }

        public ProjectEstimationDetailsViewModel AddItemFields(ProjectEstimationDetailsViewModel model, int headCount)
        {
            var unitList = _pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll();
            model.UnitList = Common.PopulateDllList(unitList);
            var newSlNo = headCount + Convert.ToDouble("1.1");
            model.SerialNo = newSlNo.ToString("F1");
            return model;
        }

        [HttpPost]
        public ActionResult AddNewItem(ProjectEstimationDetailsViewModel model, int estimationHeadId, string currentSlNo)
        {
            AddItemFields(model, 0);
            double slNo = 0;
            double.TryParse(currentSlNo, out slNo);
            double newSlNo = slNo + 0.1;
            model.SerialNo = newSlNo.ToString("F1");
            if (estimationHeadId > 0)
            {
                var itemList = _pmiCommonService.PMIUnit.EstimationSetupRepository.Get(t => t.EstimationHeadId == estimationHeadId).ToList();
                model.EstimationItemList = Common.PopulateEstimationItemList(itemList);
            }
            model.EstimationHeadId = estimationHeadId;
            return PartialView("_PartialItemDetail", model);

        }

        [HttpPost]
        public ActionResult AddHeadFields(ProjectEstimationDetailsViewModel model, int increment, int tblId)
        {
            //model.HeadList = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll().ToList()
            //    .Select(y =>
            //    new SelectListItem()
            //    {
            //        Text = y.HeadName,
            //        Value = y.Id.ToString()
            //    }).ToList();
            return PartialView("_PartialHeadDetail", model);
        }

        public ActionResult GetProjectList()
        {
            var list = _pmiCommonService.PMIUnit.ProjectMasterRepository.GetAll().ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.NameOfWorks,
                    Value = y.Id.ToString()
                }).ToList();
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult GetZoneList()
        {
            var list = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ZoneName,
                    Value = y.ZoneCode.ToString()
                }).ToList();
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult GetItemList()
        {
            var list = _pmiCommonService.PMIUnit.EstimationSetupRepository.GetAll()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemName,
                    Value = y.ItemName.ToString()
                }).ToList();
            return PartialView("Select", list);
        }

        #endregion
    }
}