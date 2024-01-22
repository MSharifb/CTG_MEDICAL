using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ProgressReport;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class ProgressReportController : BaseController
    {
        #region Declaration

        private readonly PMICommonService _pmiCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Ctrl
        public ProgressReportController(PMICommonService pmiCommonServices, PRMCommonSevice prmCommonService)
        {
            _pmiCommonService = pmiCommonServices;
            _prmCommonService = prmCommonService;
        }
        #endregion

        #region Action Result
        // GET: PMI/ProgressReport
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ProgressReportMasterViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            int loggedUserZoneId = LoggedUserZoneInfoId;
            int ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            List<ProgressReportMasterViewModel> list = (from x in _pmiCommonService.PMIUnit.ProgressReportMasterRepository.GetAll()
                                                        where (x.ZoneInfoId == loggedUserZoneId && x.ProjectForId == ProjectForId)
                                                        select new ProgressReportMasterViewModel
                                                        {
                                                            Id = x.Id,
                                                            ReportDate = x.ReportDate,
                                                            ReportFromDate = x.ReportDate,
                                                            ReportToDate = x.ReportDate

                                                        }).DefaultIfEmpty().OfType<ProgressReportMasterViewModel>().ToList();

            #region Sorting

            if (request.SortingName == "ReportDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ReportDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ReportDate).ToList();
                }
            }

            #endregion

            #region Search

            if (request.Searching)
            {
                if (model.ReportFromDate != DateTime.MinValue && model.ReportToDate != DateTime.MinValue)
                {
                    list = list.Where(t => t.ReportDate >= model.ReportFromDate && t.ReportDate <= model.ReportToDate).ToList();
                }
                if (model.ReportFromDate != DateTime.MinValue)
                {
                    list = list.Where(t => t.ReportDate >= model.ReportFromDate).ToList();
                }
                if (model.ReportToDate != DateTime.MinValue)
                {
                    list = list.Where(t => t.ReportDate <= model.ReportToDate).ToList();
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
                    d.ReportDate.ToString("dd/MM/yyyy"),
                    d.ReportFromDate.ToString("dd/MM/yyyy"),
                    d.ReportToDate.ToString("dd/MM/yyyy"),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new ProgressReportMasterViewModel();
            PopulateDropdown(model);
            model.strMode = "Create";
            model.ActionType = "Create";
            model.ReportDate = DateTime.Now;
            model.CurrentDate = DateTime.Now;
            model.PreviousDate = DateTime.Now;
            model.ZoneInfoId = LoggedUserZoneInfoId;
            model.ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            model.IsConfirm = false;

            var apvModel = new ApprovalFlowViewModel();
            var empList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == HttpContext.User.Identity.Name)
             .Select(x => new { Id = x.Id, Name = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();
            apvModel.EmployeeList = Common.PopulateDllList(empList);
            model.ApproverList.Add(apvModel);


            GenerateProgressReportFirstRow(model, 0, string.Empty);

            var yearWiseBilledList = GetYearWiseBilledList();
            model.YearWiseBilledList.Add(yearWiseBilledList);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(ProgressReportMasterViewModel model)
        {
            try
            {
                var masterObj = model.ToEntity();
                masterObj.IUser = HttpContext.User.Identity.Name;
                masterObj.IDate = DateTime.Now;
                masterObj.ZoneInfoId = LoggedUserZoneInfoId;

                foreach (var item in model.ProgressReportDetailList)
                {
                    var detailObj = item.ToEntity();

                    foreach (var billed in item.YearlyBilledList)
                    {
                        var billedObj = billed.ToEntity();

                        decimal BilledAmount = 0;
                        decimal.TryParse(billed.BilledAmount.ToString(), out BilledAmount);
                        billedObj.BilledAmount = BilledAmount;

                        billedObj.ProgressReportDetailsId = detailObj.Id;
                        billedObj.PMI_ProgressReportDetails = detailObj;
                        _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Add(billedObj);
                    }

                    detailObj.PMI_ProgressReportMaster = masterObj;
                    _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Add(detailObj);
                }

                foreach (var item in model.TempAttachmentDetail)
                {
                    var progressReportAttachment = new PMI_ProgressReportAttachment();

                    progressReportAttachment.Id = item.Id;
                    progressReportAttachment.ProgressReportId = masterObj.Id;
                    progressReportAttachment.NameofWork = item.NameofWork;
                    progressReportAttachment.PreviousDate = item.PreviousDate;
                    progressReportAttachment.CurrentDate = item.CurrentDate;
                    progressReportAttachment.PreviousFileName = item.PreviousFileName;
                    progressReportAttachment.CurrentFileName = item.CurrentFileName;
                    progressReportAttachment.PreviousAttachment = item.PreviousAttachment;
                    progressReportAttachment.CurrentAttachment = item.CurrentAttachment;
                    _pmiCommonService.PMIUnit.ProgressReportAttachmentRepository.Add(progressReportAttachment);
                }
                _pmiCommonService.PMIUnit.ProgressReportMasterRepository.Add(masterObj);
                _pmiCommonService.PMIUnit.ProgressReportMasterRepository.SaveChanges();
                SaveSignatureFlow(masterObj.Id, model.ApproverList);
                model = masterObj.ToModel();
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
                return View("CreateOrEdit", model);
            }

            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
        }

        public ActionResult Edit(int id)
        {
            var model = new ProgressReportMasterViewModel();
            try
            {
                var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();

                var master = _pmiCommonService.PMIUnit.ProgressReportMasterRepository.GetByID(id);
                model = master.ToModel();

                var details = _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Get(q => q.ProgressReportMasterId == id)
                             .DefaultIfEmpty().OfType<PMI_ProgressReportDetails>().ToList();

                var workStatusList = _pmiCommonService.PMIUnit.WorkStatusRepository.GetAll().ToList();

                var attList = _pmiCommonService.PMIUnit.ProgressReportAttachmentRepository.Get(s => s.ProgressReportId == id).ToList();

                var yearWiseBilledList = new List<YearWiseBilledViewModel>();

                if (details != null && details.Count > 0)
                {
                    foreach (var item in details)
                    {

                        var detailModel = item.ToModel();
                        detailModel.WorkStatusList = Common.PopulateDllList(workStatusList);

                        var billedList = _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Get(q => q.ProgressReportDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_ProgressReportDetailsYearlyBilled>().ToList();

                        if (billedList != null && billedList.Count > 0)
                        {
                            foreach (var billed in billedList)
                            {
                                var billedModel = billed.ToModel();
                                detailModel.YearlyBilledList.Add(billedModel);
                                var addedList = yearWiseBilledList.Where(q => q.FinancialYearId == billed.FinancialYearId).DefaultIfEmpty().OfType<YearWiseBilledViewModel>().ToList();
                                if (addedList == null || addedList.Count == 0)
                                {
                                    var statusModel = new YearWiseBilledViewModel();
                                    statusModel.FinancialYearId = billed.FinancialYearId;
                                    statusModel.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);
                                    yearWiseBilledList.Add(statusModel);
                                }
                            }
                        }
                        else
                        {
                            GenerateYearlyBilled(detailModel, 0, string.Empty);
                        }
                        model.ProgressReportDetailList.Add(detailModel);
                        model.YearWiseBilledList = yearWiseBilledList;
                    }

                    if (model.YearWiseBilledList.Count == 0)
                    {
                        var yearWiseBilledListOne = GetYearWiseBilledList();
                        model.YearWiseBilledList.Add(yearWiseBilledListOne);
                    }
                }

                if (attList != null && attList.Count > 0)
                {
                    foreach (var item in attList)
                    {
                        var obj = new ProgressReportMasterViewModel
                        {
                            Id = item.Id,
                            ProgressReportId = item.ProgressReportId,
                            NameofWork = item.NameofWork,
                            PreviousDate = Convert.ToDateTime(item.PreviousDate),
                            CurrentDate = Convert.ToDateTime(item.CurrentDate),
                            PreviousFileName = item.PreviousFileName,
                            CurrentFileName = item.CurrentFileName,
                            PreviousAttachment = item.PreviousAttachment,
                            CurrentAttachment = item.CurrentAttachment
                        };
                        model.TempAttachmentDetail.Add(obj);
                    }
                }

                #region Approval Flow
                var approverList = (from x in _pmiCommonService.PMIUnit.ProgressReportSignatureRepository.GetAll()
                                    join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on x.EmployeeId equals emp.Id
                                    where (x.ProgressReportId == id)
                                    select new ApprovalFlowViewModel
                                    {
                                        Id = x.Id,
                                        EmployeeName = emp.FullName,
                                        EmpId = emp.EmpID,
                                        EmployeeId = emp.Id,
                                        DesignationId = emp.DesignationId,
                                        DesignationName = emp.PRM_Designation.Name,
                                        DepartmentName = emp.PRM_Division == null ? string.Empty : emp.PRM_Division.Name,
                                    }).ToList();
                if (approverList.Count == 0)
                {
                    var apvModel = new ApprovalFlowViewModel();
                    var empList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(x => x.EmpID == HttpContext.User.Identity.Name)
                     .Select(x => new { Id = x.Id, Name = string.Concat(x.FullName, " [", x.EmpID, "]", " [", x.PRM_Division == null ? string.Empty : x.PRM_Division.Name, "]") }).ToList();
                    apvModel.EmployeeList = Common.PopulateDllList(empList);
                    model.ApproverList.Add(apvModel);
                }
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
                #endregion

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            PopulateDropdown(model);
            model.strMode = "Update";
            model.ActionType = "Edit";

            model.CurrentDate = DateTime.Now;
            model.PreviousDate = DateTime.Now;

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(ProgressReportMasterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var masterObj = model.ToEntity();
                    var existingProjects = _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Get(q => q.ProgressReportMasterId == masterObj.Id).DefaultIfEmpty().OfType<PMI_ProgressReportDetails>().ToList();
                    var existingProjectInModel = model.ProgressReportDetailList.Where(q => q.Id > 0).DefaultIfEmpty().ToList();
                    var deletedProjects = (from existing in existingProjects
                                           where !(existingProjectInModel.Any(dt => dt.Id == existing.Id))
                                           select existing).DefaultIfEmpty().OfType<PMI_ProgressReportDetails>().ToList();
                    foreach (var item in deletedProjects)
                    {
                        var costs = _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Get(q => q.ProgressReportDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_ProgressReportDetailsYearlyBilled>().ToList();
                        foreach (var c in costs)
                        {
                            _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Delete(c);
                        }
                        _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Delete(item);
                    }

                    foreach (var item in model.ProgressReportDetailList)
                    {
                        var detailObj = item.ToEntity();
                        detailObj.ProgressReportMasterId = masterObj.Id;

                        if (detailObj.Id > 0)
                        {
                            _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Update(detailObj);
                        }
                        else
                        {
                            _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Add(detailObj);
                            _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.SaveChanges();
                        }

                        // Yearly Billed Amount Update
                        var existingBilled = _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Get(q => q.ProgressReportDetailsId == detailObj.Id).DefaultIfEmpty().OfType<PMI_ProgressReportDetailsYearlyBilled>().ToList();
                        var existingBilledModel = item.YearlyBilledList.Where(q => q.Id > 0).DefaultIfEmpty().OfType<ProgressReportDetailYearlyBilledViewModel>().ToList();
                        var deletedBilled = (from existing in existingBilled
                                             where !(existingBilledModel.Any(dt => dt.Id == existing.Id))
                                             select existing).DefaultIfEmpty().OfType<PMI_ProgressReportDetailsYearlyBilled>().ToList();

                        foreach (var deletedItem in deletedBilled)
                        {
                            _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Delete(deletedItem);
                        }

                        foreach (var billed in item.YearlyBilledList)
                        {
                            var billedObj = billed.ToEntity();
                            if (billedObj.Id > 0)
                            {
                                _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Update(billedObj);
                            }
                            else
                            {
                                billedObj.ProgressReportDetailsId = detailObj.Id;
                                _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Add(billedObj);
                            }
                        }
                    }

                    foreach (var item in model.TempAttachmentDetail)
                    {
                        var progressReportAttachment = new PMI_ProgressReportAttachment();

                        progressReportAttachment.Id = item.Id;
                        progressReportAttachment.ProgressReportId = masterObj.Id;
                        progressReportAttachment.NameofWork = item.NameofWork;
                        progressReportAttachment.PreviousDate = item.PreviousDate;
                        progressReportAttachment.CurrentDate = item.CurrentDate;
                        progressReportAttachment.PreviousFileName = item.PreviousFileName;
                        progressReportAttachment.CurrentFileName = item.CurrentFileName;
                        progressReportAttachment.PreviousAttachment = item.PreviousAttachment;
                        progressReportAttachment.CurrentAttachment = item.CurrentAttachment;
                        if (item.Id > 0)
                        {
                            _pmiCommonService.PMIUnit.ProgressReportAttachmentRepository.Update(progressReportAttachment);
                        }
                        else
                        {
                            _pmiCommonService.PMIUnit.ProgressReportAttachmentRepository.Add(progressReportAttachment);
                        }
                    }
                    _pmiCommonService.PMIUnit.ProgressReportMasterRepository.Update(masterObj);
                    _pmiCommonService.PMIUnit.ProgressReportMasterRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProgressReportAttachmentRepository.SaveChanges();
                    SaveSignatureFlow(masterObj.Id, model.ApproverList);
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
                var isConfirm = _pmiCommonService.PMIUnit.ProgressReportMasterRepository.GetByID(id).IsConfirm;
                if (isConfirm)
                {
                    result = false;
                    errMsg = "Sorry! Progress Report is confirmed.";
                }
                else
                {

                    var prMaster = _pmiCommonService.PMIUnit.ProgressReportMasterRepository.GetByID(id);
                    var budgetDetail = _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Get(t => t.ProgressReportMasterId == id).ToList();

                    foreach (var detailitem in budgetDetail)
                    {
                        var billedList = _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Get(t => t.ProgressReportDetailsId == detailitem.Id).ToList();
                        foreach (var item in billedList)
                        {
                            _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Delete(item.Id);
                        }
                        _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Delete(detailitem.Id);
                    }
                    var signatureList = _pmiCommonService.PMIUnit.ProgressReportSignatureRepository.Get(x => x.ProgressReportId == id).ToList();
                    foreach (var item in signatureList)
                    {
                        _pmiCommonService.PMIUnit.ProgressReportSignatureRepository.Delete(item.Id);
                    }

                    _pmiCommonService.PMIUnit.ProgressReportMasterRepository.Delete(id);

                    _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProgressReportMasterRepository.SaveChanges();
                    _pmiCommonService.PMIUnit.ProgressReportMasterRepository.SaveChanges();

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

        #region DDL
        private void PopulateDropdown(ProgressReportMasterViewModel model)
        {
            //var ministryList = _pmiCommonService.PMIUnit.MinistryRepository.GetAll().DefaultIfEmpty().ToList();
            //model.DivisionOrMinistryList = Common.PopulateDllList(ministryList);

            //var approvalAuthorityList = _pmiCommonService.PMIUnit.ApprovalAuthorityRepository.GetAll().DefaultIfEmpty().ToList();
            //model.ApprovalAuthorityList = Common.PopulateDllList(approvalAuthorityList);

            //var projectOrZoneList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().DefaultIfEmpty().ToList();
            //model.ProjectOrZoneList = Common.PopulateDdlZoneList(projectOrZoneList);

            //var projectStatusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll().DefaultIfEmpty().ToList();
            //model.ProjectStatusList = Common.PopulateDllList(projectStatusList);

            //var designationList = _prmCommonService.PRMUnit.DesignationRepository.GetAll().ToList();
            //model.DesignationList = Common.PopulateDllList(designationList);

            //var approvalStatusList = _pmiCommonService.PMIUnit.ProjectApproverStatusRepository.GetAll().Where(x => x.Name.Contains("Submit") || x.Name.Contains("Draft")).ToList();
            //model.ApprovalStatusList = Common.PopulateDllList(approvalStatusList);

            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();
            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);

            var statusList = _pmiCommonService.PMIUnit.ProjectStatusRepository.GetAll();
            model.StatusList = Common.PopulateDllList(statusList);

            #region APP
            int ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);

            var appList = (from B in _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetAll()
                           where (B.ZoneInfoId == LoggedUserZoneInfoId && B.ProjectForId == ProjectForId)
                           select (B)).DistinctBy(s => s.Id).ToList();


            var list = new List<SelectListItem>();
            foreach (var item in appList)
            {
                list.Add(new SelectListItem()
                {
                    Text = string.Concat("Tracking Number: ", item.Id, " ", item.ProjectCode, " ", item.ProjectName),
                    Value = item.Id.ToString()
                });
            }
            model.APPList = list;
            #endregion
        }
        #endregion

        #region Others

        [HttpPost]
        public ActionResult AddNewProgressReport(ProgressReportMasterViewModel model, int? noOfFinancialYearBilled, string id)
        {
            GenerateProgressReportFirstRow(model, noOfFinancialYearBilled, id);
            return PartialView("_ProgressReportDetail", model.ProgressReportDetailList);
        }

        private ProgressReportMasterViewModel GenerateProgressReportFirstRow(ProgressReportMasterViewModel model, int? noOfFinancialYearBilled, string id)
        {
            ProgressReportDetailsViewModel anDetailItem = new ProgressReportDetailsViewModel();
            var workStatusList = _pmiCommonService.PMIUnit.WorkStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_WorkStatus>().ToList();
            anDetailItem.WorkStatusList = Common.PopulateDllList(workStatusList);

            GenerateYearlyBilled(anDetailItem, noOfFinancialYearBilled, id);
            model.ProgressReportDetailList.Add(anDetailItem);
            return model;
        }

        private ProgressReportDetailsViewModel GenerateYearlyBilled(ProgressReportDetailsViewModel model, int? noOfFinancialYear, string id)
        {
            int noOfFy = 0;
            int.TryParse(noOfFinancialYear.ToString(), out noOfFy);
            if (noOfFy == 0) { noOfFy = 1; }
            var yearlyBilledList = new List<ProgressReportDetailYearlyBilledViewModel>();
            for (int i = 0; i < noOfFy; i++)
            {
                var anYearlyBilled = new ProgressReportDetailYearlyBilledViewModel();
                anYearlyBilled.PreviousFieldId = id;
                yearlyBilledList.Add(anYearlyBilled);
            }
            model.YearlyBilledList.AddRange(yearlyBilledList);
            return model;
        }

        private YearWiseBilledViewModel GetYearWiseBilledList()
        {
            var model = new YearWiseBilledViewModel();
            var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();
            model.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);
            return model;
        }

        [HttpPost]
        public ActionResult AddNewItemFromAPP(ProgressReportMasterViewModel model, int financialYearId, int statusId)
        {
            GenerateARowFromApp(model, financialYearId, statusId);
            return PartialView("_ProgressReportDetail", model.ProgressReportDetailList);
        }

        private ProgressReportMasterViewModel GenerateARowFromApp(ProgressReportMasterViewModel model, int financialYearId, int statusId)
        {
            int ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            var appMasterId = _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetAll()
                .Where(x => x.ZoneInfoId == LoggedUserZoneInfoId && x.ProjectForId == ProjectForId && x.FinancialYearId == financialYearId && x.APPStatusId == statusId)
                .Select(s => s.Id).FirstOrDefault();

            var details = _pmiCommonService.PMIUnit.AnnualProcurementPlanDetailsRepository.Get(q => q.AnnualProcurementPlanMasterId == appMasterId).DefaultIfEmpty().ToList();

            #region name Of Work List

            if (details != null && details.Count > 0)
            {
                foreach (var item in details)
                {
                    var anDetailItem = new ProgressReportDetailsViewModel();

                    anDetailItem.SerialNo = item.LotNo;
                    anDetailItem.NameOfWorks = item.DescritionOfAPP;
                    anDetailItem.EstimatedAmount = item.EstdCost;
                    GenerateYearlyBilled(anDetailItem, 0, string.Empty);

                    var workStatusList = _pmiCommonService.PMIUnit.WorkStatusRepository.GetAll().DefaultIfEmpty().OfType<PMI_WorkStatus>().ToList();
                    anDetailItem.WorkStatusList = Common.PopulateDllList(workStatusList);

                    model.ProgressReportDetailList.Add(anDetailItem);
                }
            }

            #endregion

            return model;
        }

        public JsonResult GetAppMasterId(int? financialYearId, int? statusId)
        {
            int ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            var appMasterId = _pmiCommonService.PMIUnit.AnnualProcurementPlanMasterRepository.GetAll()
                .Where(x => x.ZoneInfoId == LoggedUserZoneInfoId && x.ProjectForId == ProjectForId && x.FinancialYearId == financialYearId && x.APPStatusId == statusId)
                .Select(s => s.Id).FirstOrDefault();
            return Json(appMasterId, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Attachment
        [HttpPost]
        public ActionResult AddAttachemnt([Bind(Exclude = "Attachment")] ProgressReportMasterViewModel model)
        {
            HttpFileCollectionBase files = Request.Files;
            string previousName = string.Empty;
            string currentName = string.Empty;
            byte[] previousfileData = null;
            byte[] currentfileData = null;

            foreach (string fileTagName in files)
            {

                if (fileTagName == "PreviousFile")
                {
                    HttpPostedFileBase file = Request.Files[fileTagName];
                    if (file.ContentLength > 0)
                    {
                        int size = file.ContentLength;
                        previousName = file.FileName;
                        int position = previousName.LastIndexOf("\\");
                        previousName = previousName.Substring(position + 1);
                        string contentType = file.ContentType;
                        previousfileData = new byte[size];
                        file.InputStream.Read(previousfileData, 0, size);
                    }
                }
                else
                {
                    HttpPostedFileBase file = Request.Files[fileTagName];
                    if (file.ContentLength > 0)
                    {
                        int size = file.ContentLength;
                        currentName = file.FileName;
                        int position = currentName.LastIndexOf("\\");
                        currentName = currentName.Substring(position + 1);
                        string contentType = file.ContentType;
                        currentfileData = new byte[size];
                        file.InputStream.Read(currentfileData, 0, size);
                    }
                }
            }

            List<ProgressReportMasterViewModel> list = new List<ProgressReportMasterViewModel>();

            var attList = Session["attachmentList"] as List<ProgressReportMasterViewModel>;

            var obj = new ProgressReportMasterViewModel
            {
                ProgressReportId = model.ProgressReportId,
                NameofWork = model.NameofWork,
                PreviousDate = Convert.ToDateTime(model.PreviousDate),
                CurrentDate = Convert.ToDateTime(model.CurrentDate),
                PreviousFileName = previousName,
                CurrentFileName = currentName,
                PreviousAttachment = previousfileData,
                CurrentAttachment = currentfileData,
            };
            list.Add(obj);
            model.TempAttachmentDetail = list;
            attList = list;
            return PartialView("_Details", model);
        }


        [HttpPost, ActionName("DeleteAttachmentDetail")]
        public JsonResult DeleteAttachmentDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _pmiCommonService.PMIUnit.ProgressReportAttachmentRepository.Delete(id);
                _pmiCommonService.PMIUnit.ProgressReportAttachmentRepository.SaveChanges();
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
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult Saveas(int id)
        {
            var model = new ProgressReportMasterViewModel();
            try
            {
                var financialYearList = _pmiCommonService.PMIUnit.AccountingPeriodRepository.GetAll();

                var master = _pmiCommonService.PMIUnit.ProgressReportMasterRepository.GetByID(id);
                model = master.ToModel();

                var details = _pmiCommonService.PMIUnit.ProgressReportDetailsRepository.Get(q => q.ProgressReportMasterId == id)
                             .DefaultIfEmpty().OfType<PMI_ProgressReportDetails>().ToList();

                var workStatusList = _pmiCommonService.PMIUnit.WorkStatusRepository.GetAll().ToList();

                var attList = _pmiCommonService.PMIUnit.ProgressReportAttachmentRepository.Get(s => s.ProgressReportId == id).ToList();

                var yearWiseBilledList = new List<YearWiseBilledViewModel>();

                if (details != null && details.Count > 0)
                {
                    foreach (var item in details)
                    {
                        var detailModel = item.ToModel();
                        detailModel.Id = 0;
                        detailModel.ProgressReportMasterId = 0;

                        detailModel.WorkStatusList = Common.PopulateDllList(workStatusList);
                        var billedList = _pmiCommonService.PMIUnit.ProgressReportDetailsYearlyBilledRepository.Get(q => q.ProgressReportDetailsId == item.Id).DefaultIfEmpty().OfType<PMI_ProgressReportDetailsYearlyBilled>().ToList();

                        if (billedList != null && billedList.Count > 0)
                        {
                            foreach (var billed in billedList)
                            {
                                var billedModel = billed.ToModel();
                                billedModel.Id = 0;
                                billedModel.ProgressReportDetailsId = 0;

                                detailModel.YearlyBilledList.Add(billedModel);
                                var addedList = yearWiseBilledList.Where(q => q.FinancialYearId == billed.FinancialYearId).DefaultIfEmpty().OfType<YearWiseBilledViewModel>().ToList();
                                if (addedList == null || addedList.Count == 0)
                                {
                                    var statusModel = new YearWiseBilledViewModel();
                                    statusModel.FinancialYearId = billed.FinancialYearId;
                                    statusModel.FinancialYearList = Common.PopulateAccountingPeriodDdl(financialYearList);
                                    yearWiseBilledList.Add(statusModel);
                                }
                            }
                        }
                        else
                        {
                            GenerateYearlyBilled(detailModel, 0, string.Empty);
                        }
                        model.ProgressReportDetailList.Add(detailModel);
                        model.YearWiseBilledList = yearWiseBilledList;
                    }
                    if (model.YearWiseBilledList.Count == 0)
                    {
                        var yearWiseBilledListOne = GetYearWiseBilledList();
                        model.YearWiseBilledList.Add(yearWiseBilledListOne);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            PopulateDropdown(model);
            model.ZoneInfoId = LoggedUserZoneInfoId;
            model.ProjectForId = Convert.ToInt32(Session["ProjectSectionId"]);
            model.strMode = "Create";
            model.ActionType = "Create";
            model.CurrentDate = DateTime.Now;
            model.PreviousDate = DateTime.Now;
            return View("CreateOrEdit", model);
        }

        #region Signature

        public void SaveSignatureFlow(int ProgressreportId, List<ApprovalFlowViewModel> approverList)
        {
            var tempPeriod = _pmiCommonService.PMIUnit.ProgressReportSignatureRepository.Get(x => x.ProgressReportId == ProgressreportId).FirstOrDefault();
            if (tempPeriod != null && approverList.Count > 0)
            {
                _pmiCommonService.PMIUnit.ProgressReportSignatureRepository.Delete(tempPeriod.Id);
                _pmiCommonService.PMIUnit.ProgressReportSignatureRepository.SaveChanges();
            }

            if (approverList.Count > 0)
            {
                foreach (var item in approverList)
                {
                    if (item.EmployeeId != null && item.EmployeeId != 0)
                    {
                        PMI_ProgressReportSignature progressReportSignature = new PMI_ProgressReportSignature();

                        progressReportSignature.ProgressReportId = ProgressreportId;
                        progressReportSignature.EmployeeId = (int)item.EmployeeId;
                        _pmiCommonService.PMIUnit.ProgressReportSignatureRepository.Add(progressReportSignature);
                    }
                }
                _pmiCommonService.PMIUnit.ApprovalFlowMasterRepository.SaveChanges();
            }
        }

        #endregion

        public ActionResult Confirm(int id)
        {
            var masterData = _pmiCommonService.PMIUnit.ProgressReportMasterRepository.GetByID(id);
            masterData.IsConfirm = true;
            _pmiCommonService.PMIUnit.ProgressReportMasterRepository.Update(masterData);
            _pmiCommonService.PMIUnit.ProgressReportMasterRepository.SaveChanges();
            return RedirectToAction("Edit", new { id = id, type = "success" });
        }
    }
}