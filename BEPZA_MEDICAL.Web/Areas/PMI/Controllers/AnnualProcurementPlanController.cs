using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.AnnualProcurementPlan;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class AnnualProcurementPlanController : BaseController
    {
        #region Declaration

        private readonly PMICommonService _pmiCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Ctrl
        public AnnualProcurementPlanController(PMICommonService pmiCommonServices, PRMCommonSevice prmCommonService)
        {
            _pmiCommonService = pmiCommonServices;
            _prmCommonService = prmCommonService;
        }
        #endregion

        // GET: PMI/AnnualProcurementPlan

        #region Action Result
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AnnualProcurementPlanMasterViewModel model)
        {
            string filterExpression = String.Empty;

            int totalRecords = 0;
            int loggedUserZoneId = LoggedUserZoneInfoId;
            int ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            List<AnnualProcurementPlanMasterViewModel> list = (from x in _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetAll()
                                                               //join y in _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.GetAll() on x.Id equals y.AnnualProcurementPlanMasterId
                                                               join acc in _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll() on x.FinancialYearId equals acc.id
                                                               //join accHead in _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll() on y.APPHeadId equals accHead.Id
                                                               where(x.ZoneInfoId==loggedUserZoneId && x.ProjectForId == ProjectForId)
                                                               select new AnnualProcurementPlanMasterViewModel
                                                                {
                                                                   Id = x.Id,
                                                                   FinancialYearId = x.FinancialYearId,
                                                                   FinancialYearName = acc.yearName,
                                                                   APPStatusId = x.APPStatusId
                                                               }).DefaultIfEmpty().OfType<AnnualProcurementPlanMasterViewModel>().ToList();

            #region Sorting

            if (request.SortingName == "FinancialYearName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FinancialYearName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FinancialYearName).ToList();
                }
            }

            #endregion

            #region Search

            if (request.Searching)
            {
                if (model.FinancialYearId != 0)
                {
                    list = list.Where(d => d.FinancialYearId == model.FinancialYearId).ToList();

                }

                if (model.APPStatusId != 0)
                {
                    list = list.Where(d => d.APPStatusId == model.APPStatusId).ToList();

                }
            }

            #endregion

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
                    d.FinancialYearId,
                    d.FinancialYearName,
                    d.APPStatusId,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new AnnualProcurementPlanMasterViewModel();
            PopulateDropdown(model);

            var apvModel = new ApprovalFlowViewModel();
            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
            apvModel.DesignationList = Common.PopulateDllList(designationList);

            var empList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == HttpContext.User.Identity.Name)
             .Select(x => new { Id = x.Id, Name = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();
            apvModel.EmployeeList = Common.PopulateDllList(empList);
            model.ApproverList.Add(apvModel);

            model.Agency = "BEPZA";
            model.strMode = "Create";
            model.ActionType = "Create";
            model.ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            model.ZoneInfoId = LoggedUserZoneInfoId;
            var approvalStatusId = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => x.Name.Contains("Draft")).Select(s => s.Id).FirstOrDefault();
            model.ApprovalStatusId = approvalStatusId;
            model.IsConfirm = false;

            //GenerateBudgetFirstRow(model, null, null, string.Empty);
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(AnnualProcurementPlanMasterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var masterObj = model.ToEntity();
                    masterObj.IUser = HttpContext.User.Identity.Name;
                    masterObj.IDate = DateTime.Now;
                    masterObj.ZoneInfoId = LoggedUserZoneInfoId;

                    var budgetAllHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll();

                    foreach (var item in model.AnnualProcurementPlanDetailList)
                    {
                        var detailObj = item.ToEntity();
                        detailObj.APPHeadId = item.TempHeadId;
                        detailObj.BudgetDetailsId = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(x=>x.NameOfWorks.Contains(item.DescritionOfAPP)).Select(s=>s.Id).FirstOrDefault();
                        detailObj.PMI_AnnualProcurementPlanMaster = masterObj;
                        _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Add(detailObj);
                    }
                    _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.Add(masterObj);
                    _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.SaveChanges();

                    SaveApprovalFlow(masterObj.Id, model.ApprovalStatusId, model.ApproverList);

                    model = masterObj.ToModel();

                }
                else
                {
                    PopulateDropdown(model);
                    var apvModel = new ApprovalFlowViewModel();
                    var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
                    apvModel.DesignationList = Common.PopulateDllList(designationList);
                    model.ApproverList.Add(apvModel);
                    return View("CreateOrEdit", model);
                }

            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                PopulateDropdown(model);
                var apvModel = new ApprovalFlowViewModel();
                var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
                apvModel.DesignationList = Common.PopulateDllList(designationList);
                model.ApproverList.Add(apvModel);
                return View("CreateOrEdit", model);
            }

            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
        }

        public ActionResult Edit(int id)
        {
            var model = new AnnualProcurementPlanMasterViewModel();
            try
            {
                var budgetStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
                var approveStatusId = budgetStatusList.Where(q => q.Name.Contains("Appr")).FirstOrDefault().Id;

                var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();

                var master = _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetByID(id);
                model = master.ToModel();

                var details = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Get(q => q.AnnualProcurementPlanMasterId == id).DefaultIfEmpty().ToList();

                var budgetHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll().ToList();

                var distinctHeads = details.DistinctBy(q => q.APPHeadId).DefaultIfEmpty();
                foreach (var item in distinctHeads)
                {
                    var detailHead = new BudgetDetailsHeadViewModel();
                    var headList = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
                    detailHead.BudgetHeadList = Common.PopulateBudgetHeadDDL(headList);
                    detailHead.APPHeadId = item.APPHeadId;
                    model.BudgetDetailHeadList.Add(detailHead);
                }

                if (details != null && details.Count > 0)
                {
                    foreach (var item in details)
                    {

                        var detailModel = item.ToModel();
                        detailModel.Id = item.Id;
                        detailModel.TempHeadId = item.APPHeadId;
                        detailModel.ConstructionTypeList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ConstructionTypeRepository.GetAll().ToList());
                        var budgetHeads = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
                        detailModel.BudgetHeadList = Common.PopulateBudgetHeadDDL(budgetHeads);
                        detailModel.ProcurementTypeList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ProcurementMethodRepository.GetAll().DefaultIfEmpty().ToList());
                        detailModel.SourceOfFundList = Common.PopulateDllList(_pmiCommonService.PMIUnit.SourceOfFundRepository.GetAll().DefaultIfEmpty().ToList());
                        detailModel.ApprovingAuthorityList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ApprovalAuthorityRepository.GetAll().DefaultIfEmpty().ToList());


                        var nameOfWorkList = _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll().ToList();
                        var list = new List<SelectListItem>();
                        foreach (var item2 in nameOfWorkList)
                        {
                            list.Add(new SelectListItem()
                            {
                                Text = item2.NameOfWorks,
                                Value = item2.NameOfWorks
                            });
                        }
                        detailModel.DescritionOfAPPList = list;

                        model.AnnualProcurementPlanDetailList.Add(detailModel);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            PopulateDropdown(model);

            #region Approval Flow
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(model.ApprovalStatusId);
            var approverList = (from x in _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.GetAll()
                                join y in _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.GetAll() on x.Id equals y.ApprovalFlowMasterId
                                join appStatus in _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll() on y.StatusId equals appStatus.Id
                                join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on y.EmployeeId equals emp.Id
                                where (x.BudgetProjectAPPId == id && x.PMI_ApprovalSection.Enum.Contains("APP"))
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
            if(approverList.Count == 0)
            {
                var apvModel = new ApprovalFlowViewModel();
                var empList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == HttpContext.User.Identity.Name)
                .Select(x => new { Id = x.Id, Name = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();
                apvModel.EmployeeList = Common.PopulateDllList(empList);
                model.ApproverList.Add(apvModel);

            }
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
            #endregion

            model.strMode = "Update";
            model.ActionType = "Edit";

            model.ApprovalStatus = approverStatus == null ? string.Empty : approverStatus.Name;
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(AnnualProcurementPlanMasterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var masterObj = model.ToEntity();
                    masterObj.EDate = DateTime.Now;
                    masterObj.EUser = HttpContext.User.Identity.Name;
                    masterObj.ZoneInfoId = LoggedUserZoneInfoId;

                    var budgetAllHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll();

                    var existingProjects = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Get(q => q.AnnualProcurementPlanMasterId == masterObj.Id).DefaultIfEmpty().ToList();
                    var existingProjectInModel = model.AnnualProcurementPlanDetailList.Where(q => q.Id > 0).DefaultIfEmpty().ToList();
                    var deletedProjects = (from existing in existingProjects
                                           where !(existingProjectInModel.Any(dt => dt.Id == existing.Id))
                                           select existing).DefaultIfEmpty().ToList();
                    foreach (var item in deletedProjects)
                    {
                        if(item!=null)
                        _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Delete(item);
                    }

                    foreach (var item in model.AnnualProcurementPlanDetailList)
                    {
                        var detailObj = item.ToEntity();
                        detailObj.AnnualProcurementPlanMasterId = masterObj.Id;
                        detailObj.APPHeadId = item.TempHeadId;
                        detailObj.BudgetDetailsId = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(x => x.NameOfWorks.Contains(item.DescritionOfAPP)).Select(s=>s.Id).FirstOrDefault();

                        if (detailObj.Id > 0)
                        {
                            _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Update(detailObj);
                        }
                        else
                        {
                            _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Add(detailObj);
                            _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.SaveChanges();
                        }
                    }
                    _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.Update(masterObj);
                    _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.SaveChanges();

                    SaveApprovalFlow(masterObj.Id, model.ApprovalStatusId, model.ApproverList);

                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                var isConfirm = _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetByID(id).IsConfirm;
                if (isConfirm)
                {
                    result = false;
                    errMsg = "Sorry! APP is confirmed.";
                }
                else
                {
                    var appDetail = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Get(x => x.AnnualProcurementPlanMasterId == id).ToList();
                    foreach (var dtl in appDetail)
                    {
                        _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Delete(dtl.Id);
                    }
                    var appZoneList = _pmiCommonService.PMIUnit.AppZonesRepository.Get(t => t.APPMasterId == id);
                    foreach (var item in appZoneList)
                    {
                        _pmiCommonService.PMIUnit.AppZonesRepository.Delete(item.Id);

                    }
                    _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.Delete(id);

                    _pmiCommonService.PMIUnit.AppZonesRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.SaveChanges();

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
        #endregion

        #region Approval Flow

        public void SaveApprovalFlow(int BudgetProjectAPPId, int? approverStatusId, List<ApprovalFlowViewModel> approverList)
        {
            PMI_ApprovalFlowMaster ApprovalFlowMaster = new PMI_ApprovalFlowMaster();
            var approverStatus = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetByID(approverStatusId);
            var tempPeriod = _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Get(x => x.BudgetProjectAPPId == BudgetProjectAPPId && x.PMI_ApprovalSection.Enum.Contains("APP")).FirstOrDefault();
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
                    if (item.EmployeeId != null && item.EmployeeId != 0)
                    {
                        PMI_ApprovalFlowDetails ApprovalFlowDetails = new PMI_ApprovalFlowDetails();

                        ApprovalFlowDetails.ApprovalFlowMasterId = ApprovalFlowDetails.Id;
                        ApprovalFlowDetails.EmployeeId = (int)item.EmployeeId;
                        ApprovalFlowDetails.StatusId = Convert.ToInt32(approverStatusId);
                        ApprovalFlowDetails.Remarks = string.Empty;
                        ApprovalFlowDetails.IUser = HttpContext.User.Identity.Name;
                        ApprovalFlowDetails.IDate = DateTime.Now;
                        _pmiCommonService.PMIUnit.ApprovalFlowDetailsRepository.Add(ApprovalFlowDetails);
                    }
                }

                ApprovalFlowMaster.BudgetProjectAPPId = BudgetProjectAPPId;
                ApprovalFlowMaster.ApprovalSectionId = _pmiCommonService.PMIUnit.ApprovalSectionRepository.GetAll().Where(x => x.Enum == "APP").Select(s => s.Id).FirstOrDefault();
                ApprovalFlowMaster.CreateDate = DateTime.Now;
                ApprovalFlowMaster.IUser = HttpContext.User.Identity.Name;
                ApprovalFlowMaster.IDate = DateTime.Now;

                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.Add(ApprovalFlowMaster);
                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.SaveChanges();
            }
        }

        #endregion

        #region DDL & Others
        private void PopulateDropdown(AnnualProcurementPlanMasterViewModel model)
        {
            var ministryList = _pmiCommonService.PMIUnit.MinistryRepository.GetAll().DefaultIfEmpty().ToList();
            model.DivisionOrMinistryList = Common.PopulateDllList(ministryList);

            var approvalAuthorityList = _pmiCommonService.PMIUnit.ApprovalAuthorityRepository.GetAll().DefaultIfEmpty().ToList();
            model.ApprovalAuthorityList = Common.PopulateDllList(approvalAuthorityList);

            var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll().DefaultIfEmpty().ToList();
            model.ProjectStatusList = Common.PopulateDllList(projectStatusList);

            var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().ToList();
            model.DesignationList = Common.PopulateDllList(designationList);

            var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => x.Name.Contains("Submit") || x.Name.Contains("Draft")).ToList();
            model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);

            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();
            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);

            var statusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
            model.APPStatusList = Common.PopulateDllList(statusList);

            model.BudgetStatusList = Common.PopulateDllList(projectStatusList);


            #region Budget
            var budgetList = (from B in _pmiCommonService.PMIUnit.BudgetMasterRepository.GetAll()
                              join Z in _pmiCommonService.PMIUnit.BudgetZoneRepository.GetAll() on B.Id equals Z.BudgetMasterId
                              where (Z.ZoneOrProjectId == LoggedUserZoneInfoId)
                              select (B)).DistinctBy(s=>s.Id).ToList();
                            

            var list = new List<SelectListItem>();
            foreach (var item in budgetList)
            {
                list.Add(new SelectListItem()
                {
                    Text = string.Concat("Tracking Number: ", item.Id ," ", item.PMI_ProcurementType.Name," ", item.PMI_SourceOfFund.Name),
                    Value = item.Id.ToString()
                });
            }
            model.BudgetList = list;
            #endregion
        }
        public ActionResult GetFinancialYear()
        {
            var financialYearList = Common.PopulateAccountingPeriodDdl(_pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll());
            return PartialView("_Select", financialYearList);
        }

        public ActionResult GetBudgetStatus()
        {
            var statusList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll());
            return PartialView("_Select", statusList);
        }

        [HttpPost]
        public ActionResult AddNewBudgetHead(AnnualProcurementPlanMasterViewModel model, int? noOfFinancialYear)
        {
            GenerateBudgetFirstRow(model, null, noOfFinancialYear, string.Empty);
            return PartialView("_BudgetDetailHeads", model);
        }
        [HttpPost]
        public ActionResult AddNewProject(AnnualProcurementPlanMasterViewModel model, int? budgetHeadId, int? noOfFinancialYear, string id)
        {
            GenerateBudgetFirstRow(model, budgetHeadId, noOfFinancialYear, id);
            return PartialView("_BudgetDetail", model.AnnualProcurementPlanDetailList);
        }

        private AnnualProcurementPlanMasterViewModel GenerateBudgetFirstRow(AnnualProcurementPlanMasterViewModel model, int? budgetHeadId, int? noOfFinancialYear, string id)
        {
            AnnualProcurementPlanDetailViewModel anDetailItem = new AnnualProcurementPlanDetailViewModel();
            var budgetHeads = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll().DefaultIfEmpty().ToList();
            var budgetHeadList = budgetHeads.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
            if (budgetHeadList.Count > 0)
            {
                anDetailItem.BudgetHeadList = Common.PopulateBudgetHeadDDL(budgetHeadList);
            }
            var constructionTypeList = _pmiCommonService.PMIUnit.ConstructionTypeRepository.GetAll().ToList();
            anDetailItem.TempHeadId = budgetHeadId ==null?0 : budgetHeadId.Value;
            anDetailItem.ConstructionTypeList = Common.PopulateDllList(constructionTypeList);

            anDetailItem.ProcurementTypeList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ProcurementMethodRepository.GetAll().DefaultIfEmpty().ToList());
            anDetailItem.SourceOfFundList = Common.PopulateDllList(_pmiCommonService.PMIUnit.SourceOfFundRepository.GetAll().DefaultIfEmpty().ToList());
            anDetailItem.ApprovingAuthorityList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ApprovalAuthorityRepository.GetAll().DefaultIfEmpty().ToList());

            #region name Of Work List
            var nameOfWorkList = _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll().ToList();
            var list = new List<SelectListItem>();
            foreach (var item in nameOfWorkList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.NameOfWorks,
                    Value = item.NameOfWorks
                });
            }
            anDetailItem.DescritionOfAPPList = list;
            #endregion

            anDetailItem.strMode = "NewAddClick";
            model.AnnualProcurementPlanDetailList.Add(anDetailItem);

            var detailHead = new BudgetDetailsHeadViewModel();
            var headList = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
            detailHead.BudgetHeadList = Common.PopulateBudgetHeadDDL(headList);
            model.BudgetDetailHeadList.Add(detailHead);
            
            return model;
        }


        [HttpPost]
        public ActionResult AddNewAppItemFromBudget(AnnualProcurementPlanMasterViewModel model, int financialYearId, int statusId)
        {
            GenerateAppRowFromBudget(model, financialYearId, statusId);
            return PartialView("_BudgetDetailHeads", model);
        }

        private AnnualProcurementPlanMasterViewModel GenerateAppRowFromBudget(AnnualProcurementPlanMasterViewModel model, int financialYearId, int statusId)
        {

            int projectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            var budgetList = (from B in _pmiCommonService.PMIUnit.BudgetMasterRepository.GetAll()
                              join BD in _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll() on B.Id equals BD.BudgetMasterId
                              join F in _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.GetAll() on BD.Id equals F.BudgetDetailsId
                              where (B.ZoneInfoId == LoggedUserZoneInfoId && B.ProjectForId == projectForId)
                              && (financialYearId == 0 || F.FinancialYearId == financialYearId)
                              && (statusId == 0 || F.BudgetStatusId == statusId)
                              select (B)).DistinctBy(s => s.Id).ToList();

            var budgetIdList = budgetList.Select(x => x.Id).ToList();

            var budgetHeadList = _pmiCommonService.PMIUnit.BudgetHeadViewRepository.GetAll().ToList();
            var details = _pmiCommonService.PMIUnit.BudgetDetailsRepository.Get(q => budgetIdList.Contains(q.BudgetMasterId)).DefaultIfEmpty().ToList();

            var distinctHeads = details.DistinctBy(q => q.BudgetHeadId).DefaultIfEmpty();
            foreach (var item in distinctHeads)
            {
                var detailHead = new BudgetDetailsHeadViewModel();
                var headList = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
                detailHead.BudgetHeadList = Common.PopulateBudgetHeadDDL(headList);
                detailHead.APPHeadId = item.BudgetHeadId;
                model.BudgetDetailHeadList.Add(detailHead);
            }

            var constructionTypeList = _pmiCommonService.PMIUnit.ConstructionTypeRepository.GetAll().ToList();

            #region name Of Work List

             if (details != null && details.Count > 0)
                {
                    foreach (var item in details)
                    {
                    var anDetailItem = new AnnualProcurementPlanDetailViewModel();

                    anDetailItem.APPHeadId = item.BudgetHeadId;
                    anDetailItem.TempHeadId = item.BudgetHeadId;
                    anDetailItem.DescritionOfAPP = item.NameOfWorks;
                    anDetailItem.ConstructionTypeId = item.ConstructionTypeId;
                    anDetailItem.LotNo = item.SerialNo;
                    anDetailItem.PackageNo = item.WorkIdentificationNumber;
                    anDetailItem.SourceOfFundId = item.PMI_BudgetMaster.SourceOfFundId;
                    anDetailItem.EstdCost = item.BudgetAmount;
                    anDetailItem.BudgetDetailsId = item.Id;

                    anDetailItem.ConstructionTypeList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ConstructionTypeRepository.GetAll().ToList());
                    var budgetHeads = budgetHeadList.Where(q => q.ParentId == null).DefaultIfEmpty().ToList();
                    anDetailItem.BudgetHeadList = Common.PopulateBudgetHeadDDL(budgetHeads);
                    anDetailItem.ProcurementTypeList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ProcurementMethodRepository.GetAll().DefaultIfEmpty().ToList());
                    anDetailItem.SourceOfFundList = Common.PopulateDllList(_pmiCommonService.PMIUnit.SourceOfFundRepository.GetAll().DefaultIfEmpty().ToList());
                    anDetailItem.ApprovingAuthorityList = Common.PopulateDllList(_pmiCommonService.PMIUnit.ApprovalAuthorityRepository.GetAll().DefaultIfEmpty().ToList());


                        var nameOfWorkList = _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll().ToList();
                        var list = new List<SelectListItem>();
                        foreach (var item2 in nameOfWorkList)
                        {
                            list.Add(new SelectListItem()
                            {
                                Text = item2.NameOfWorks,
                                Value = item2.NameOfWorks
                            });
                        }
                        anDetailItem.DescritionOfAPPList = list;

                        model.AnnualProcurementPlanDetailList.Add(anDetailItem);

                    }
                }

            #endregion

            return model;
        }
        [HttpPost]
        public JsonResult GetBudgetZones(int budgetId)
        {
            var zoneList = _pmiCommonService.PMIUnit.BudgetZoneRepository.Get(t => t.BudgetMasterId == budgetId).Select(x => x.ZoneOrProjectId).ToList();
            return Json(zoneList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetBudgetListByFinStatus(int? financialYearId, int? statusId)
        {
            int projectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            var budgetList = (from B in _pmiCommonService.PMIUnit.BudgetMasterRepository.GetAll()
                              join BD in _pmiCommonService.PMIUnit.BudgetDetailsRepository.GetAll() on B.Id equals BD.BudgetMasterId
                              join F in _pmiCommonService.PMIUnit.BudgetDetailsYearlyCostRepository.GetAll() on BD.Id equals F.BudgetDetailsId
                              where (B.ZoneInfoId == LoggedUserZoneInfoId && B.ProjectForId == projectForId) 
                              && (financialYearId == null || financialYearId == 0 || F.FinancialYearId == financialYearId)
                              && (statusId == null || statusId == 0 || F.BudgetStatusId == statusId)
                              select (B)).DistinctBy(s => s.Id).ToList();


            var list = new List<SelectListItem>();
            foreach (var item in budgetList)
            {
                list.Add(new SelectListItem()
                {
                    Text = string.Concat("Tracking Number: ", item.Id, " ", item.PMI_ProcurementType.Name, " ", item.PMI_SourceOfFund.Name),
                    Value = item.Id.ToString()
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
    }
        #endregion

        public ActionResult Confirm(int id)
        {
            var masterData = _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetByID(id);
            masterData.IsConfirm = true;
            _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.Update(masterData);
            _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.SaveChanges();
            return RedirectToAction("Edit", new { id = id, type = "success" });
        }

    }
}