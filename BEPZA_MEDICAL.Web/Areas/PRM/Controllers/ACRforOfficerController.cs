using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ACRforOfficerController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly EmployeeService _empService;
        #endregion

        #region Constructor
        public ACRforOfficerController(PRMCommonSevice prmCommonService, EmployeeService empService)
        {
            this._prmCommonService = prmCommonService;
            this._empService = empService;
        }
        #endregion

        #region Search
        public ActionResult Index()
        {
            var model = new ACRforOfficerViewModel();
            model.ActionName = "OfficerInfoIndex";

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ACRforOfficerViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ACRforOfficerViewModel> list = (from acrOff in _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetAll()
                                                 join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on acrOff.EmployeeId equals emp.Id
                                                 join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                                 join dep in _prmCommonService.PRMUnit.DivisionRepository.GetAll() on emp.DivisionId equals dep.Id
                                                 where (model.DepartmentId == 0 || model.DepartmentId == dep.Id)
                                                 && (model.DesignationId == 0 || model.DesignationId == des.Id)
                                                 && (model.EmpId == null || model.EmpId == "" || model.EmpId == emp.EmpID)
                                                 && (string.IsNullOrEmpty(model.Name) || emp.FullName.Contains(model.Name))
                                                 && (LoggedUserZoneInfoId == acrOff.ZoneInfoId)
                                                 && (emp.EmpID != User.Identity.Name)
                                                 select new ACRforOfficerViewModel()
                                                 {
                                                     Id = acrOff.Id,
                                                     EmpId = emp.EmpID,
                                                     Name = emp.FullName,
                                                     DesignationId = des.Id,
                                                     Designation = des.Name,
                                                     DepartmentId = dep.Id,
                                                     Department = dep.Name
                                                 }).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "EmpId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpId).ToList();
                }
            }
            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Name).ToList();
                }
            }
            if (request.SortingName == "Designation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Designation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Designation).ToList();
                }
            }
            if (request.SortingName == "Department")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Department).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Department).ToList();
                }
            }

            #endregion

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
                  d.Name,
                  d.Id,
                  d.EmpId,
                  d.DesignationId,
                  d.Designation,
                  d.DepartmentId,
                  d.Department
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }
        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _prmCommonService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(divisions));
        }
        #endregion

        #region Officer's Basic Info

        #region Insert
        public ActionResult OfficerInfoIndex(int? id)
        {
            if (id.HasValue)
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id });

            var parentModel = new ACRforOfficerViewModel();
            var model = parentModel.OfficerInfo;

            parentModel.ViewType = "OfficerInfo";

            #region Reporting Officer Info
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var reportingOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(loginUser.ID);
            if (reportingOfficerInfo != null)
            {
                model.ReportingOfficerId = reportingOfficerInfo.Id;
                model.NameOfReportingOfficer = reportingOfficerInfo.FullName;
                model.Designation = reportingOfficerInfo.PRM_Designation.Name;

                var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == reportingOfficerInfo.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            }
            model.ReportingDate = Convert.ToDateTime(DateTime.Now.ToString(DateAndTime.GlobalDateFormat));
            #endregion

            var approvalProcessName = @"ACR";
            var approverInfo = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(string.Empty, approvalProcessName)
                .DefaultIfEmpty()
                .OfType<APV_GetApproverInfoByApplicant_Result>()
                .ToList();
            model.ApproverList = Common.PopulateEmployeeDDL(approverInfo);
            
            model.ActionType = "CreateOfficerInfo";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";

            parentModel.OfficerInfo = model;

            return View("CreateOrEdit", parentModel);
        }

        public ActionResult CreateOfficerInfo(OfficerInfoViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "OfficerInfo";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Submit")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.Add(entity);
                    _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.SaveChanges();

                    parentModel.Id = entity.Id;
                    parentModel.EmpId = entity.EmployeeId.ToString();
                    const int isOnlineApplication = 1;
                    _prmCommonService.PRMUnit.FunctionRepository.InitializeApprovalProcess("ACR", model.EmpId, entity.Id, isOnlineApplication, Common.GetInteger(model.NextApproverId), entity.IUser);
                }
                catch (Exception ex)
                {
                    parentModel.OfficerInfo = model;
                    parentModel.OfficerInfo.ButtonText = "Save";
                    parentModel.OfficerInfo.SelectedClass = "selected";
                    parentModel.OfficerInfo.ErrorClass = "failed";
                    parentModel.OfficerInfo.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.OfficerInfo.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.OfficerInfo = model;
                parentModel.OfficerInfo.ButtonText = "Save";
                parentModel.OfficerInfo.SelectedClass = "selected";
                parentModel.OfficerInfo.ErrorClass = "failed";
                parentModel.OfficerInfo.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.OfficerInfo.IsError = 1;
                //InitializationJobGradeAndDesignationForEdit(model);
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditOfficerInfo(int id, string type)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "OfficerInfo";

            var entity = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.ApproverList = new List<SelectListItem>();
            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == entity.PRM_EmploymentInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            model.SeniorityNumber = entity.PRM_EmploymentInfo.SeniorityPosition;

            #region Reporting Officer
            var reportingOficer = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(e=> e.EmpID == model.IUser).FirstOrDefault();
            if (reportingOficer != null)
            {
                model.NameOfReportingOfficer = reportingOficer.FullName;
                model.Designation = reportingOficer.PRM_Designation.Name;

                division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == reportingOficer.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            }
            model.ReportingDate = Convert.ToDateTime(model.IDate.ToString(DateAndTime.GlobalDateFormat));
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            if (loginUser.EmpId == model.IUser)
                model.ReportingDateEditable = true;
            else
                model.ReportingDateEditable = false;
            #endregion

            model.ActionType = "EditOfficerInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.OfficerInfo = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.Id;

            if (type == "success")
            {
                parentModel.OfficerInfo.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.OfficerInfo.ErrorClass = "success";
                parentModel.OfficerInfo.IsError = 0;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditOfficerInfo(OfficerInfoViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "OfficerInfo";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                model.ZoneInfoId = LoggedUserZoneInfoId;
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;

                    _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.Update(entity);
                    _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";
                    model.ApproverList = new List<SelectListItem>();

                    parentModel.OfficerInfo = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;
                model.ApproverList = new List<SelectListItem>();

                parentModel.OfficerInfo = model;
                parentModel.Id = model.Id;
                parentModel.EmpId = model.EmployeeId.ToString();
                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.OfficerInfo = model;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete------------------------------
        public ActionResult Delete(int id)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "OfficerInfo";

            var entity = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);
            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.Delete(id);
                    _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";
                    model.ApproverList = new List<SelectListItem>();

                    parentModel.OfficerInfo = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;
            model.ApproverList = new List<SelectListItem>();

            parentModel.OfficerInfo = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Health Test Report

        #region Insert

        public ActionResult HealthTestReportIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.GetByID(id, "OfficerInfoId");
                if (en != null)
                    return RedirectToAction("EditHealthTestReport", "ACRforOfficer", new { id = id });
            }
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "HealthTestReport";
            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);
            var nextOfficerApprover = Common.GetInteger(officerInfo.NextApproverId);
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);

            if (!IsACRStepAccessable(Convert.ToInt32(id), 1, loginUser.ID)) //-- 1 - Health Test
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            var model = parentModel.HealthTestReport;
            parentModel.Id = officerInfo.Id;

            #region Only View Part

            model.EmployeeId = officerInfo.EmployeeId;
            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(officerInfo.EmployeeId);
            model.EmpId = officerInfo.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                .FirstOrDefault(d => d.Id == officerInfo.PRM_EmploymentInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            model.EmployeeId = officerInfo.EmployeeId;
            model.OfficerInfoId = officerInfo.Id;

            model.ActionType = "CreateHealthTestReport";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";
            model.ApproverList = new List<SelectListItem>();

            var nextApproverList = GetNextApprover((int)id);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            #region Add report initiator in approvar list.
            var initiator = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                    .FirstOrDefault(e => e.EmpID == officerInfo.IUser);
            SelectListItem item = new SelectListItem()
            {
                Text = initiator.FullName + "(" + officerInfo.IUser + "), " + initiator.PRM_Designation.ShortName,
                Value = initiator.Id.ToString()
            };
            model.ApproverList.Add(item);
            #endregion

            var medicalOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(Common.GetInteger(loginUser.ID));
            if (medicalOfficerInfo != null)
            {
                model.MedicalOfficerId = medicalOfficerInfo.Id;
                model.NameOftheMadicalOfficer = medicalOfficerInfo.FullName;
                model.Designation = medicalOfficerInfo.PRM_Designation.Name;

                division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == medicalOfficerInfo.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            }

            model.Date = DateTime.Now;

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateHealthTestReport(OfficerHealthTestReportViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "HealthTestReport";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.Add(entity);
                    _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.SaveChanges();

                    parentModel.Id = entity.OfficerInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();

                    var nextOfficerApprover = 0;
                    var obj = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository
                        .GetByID(entity.OfficerInfoId);
                    if (obj != null)
                    {
                        nextOfficerApprover = Common.GetInteger(obj.NextApproverId);
                        var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository
                            .Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;

                        var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                            .Get(q => q.ApplicationId == entity.OfficerInfoId && q.ApprovalProcessId == processId
                                      && q.IsOnlineApplication == true && q.ApproverId == nextOfficerApprover);
                        if (application.Any())
                        {
                            var applicationId = application.LastOrDefault().Id;

                            string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                                .Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                            var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                                actionName, model.ApproverComments, Common.GetInteger(model.NextApproverId),
                                entity.IUser);
                        }
                    }
                }
                catch (Exception ex)
                {
                    parentModel.HealthTestReport = model;
                    parentModel.HealthTestReport.ButtonText = "Save";
                    parentModel.HealthTestReport.SelectedClass = "selected";
                    parentModel.HealthTestReport.ErrorClass = "failed";
                    parentModel.HealthTestReport.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.HealthTestReport.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.HealthTestReport = model;
                parentModel.HealthTestReport.ButtonText = "Save";
                parentModel.HealthTestReport.SelectedClass = "selected";
                parentModel.HealthTestReport.ErrorClass = "failed";
                parentModel.HealthTestReport.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.HealthTestReport.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditHealthTestReport", "ACRforOfficer", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditHealthTestReport(int id, string type)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "HealthTestReport";

            var entity = _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.GetAll().Where(x => x.OfficerInfoId == id).FirstOrDefault();
            var model = entity.ToModel();

            var nextApproverId = 0;
            var approverId = 0;
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);
            if (officerInfo != null)
            {
                approverId = Common.GetInteger(officerInfo.ApproverId);
                nextApproverId = Common.GetInteger(officerInfo.NextApproverId);
            }

            if (!IsACRStepAccessable(id, 1, loginUser.ID)) //-- 1 - Health Test
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            #region Only View Part

            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            model.ActionType = "EditHealthTestReport";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.HealthTestReport = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.OfficerInfoId;

            if (type == "success")
            {
                parentModel.HealthTestReport.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.HealthTestReport.ErrorClass = "success";
                parentModel.HealthTestReport.IsError = 0;
            }

            if (id > 0)
            {
                model.ApproverList = new List<SelectListItem>();
                var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository
                    .Get(q => q.ProcessNameEnum == "ACR")
                    .FirstOrDefault().Id;

                var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                    .Get(q => q.ApprovalProcessId == processId
                              && q.ApplicationId == id
                              && q.ApproverId == nextApproverId);
                if (application.Any())
                {
                    var approvalStepId = application.FirstOrDefault().ApprovalStepId;
                    var nextApproverList = GetNextApprover((int)id, approvalStepId);
                    model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

                    #region Add report initiator in approvar list.
                    var initiator = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                        .FirstOrDefault(e => e.EmpID == officerInfo.IUser);
                    SelectListItem item = new SelectListItem()
                    {
                        Text = initiator.FullName + "(" + officerInfo.IUser + "), " + initiator.PRM_Designation.ShortName,
                        Value = initiator.Id.ToString()
                    };
                    model.ApproverList.Add(item);
                    #endregion
                }
            }

            var medicalOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.MedicalOfficerId);
            if (medicalOfficerInfo != null)
            {
                model.MedicalOfficerId = medicalOfficerInfo.Id;
                model.NameOftheMadicalOfficer = medicalOfficerInfo.FullName;
                model.Designation = medicalOfficerInfo.PRM_Designation.Name;

                division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == medicalOfficerInfo.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditHealthTestReport(OfficerHealthTestReportViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "HealthTestReport";
            var error = string.Empty;

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            List<fn_Apv_GetApproverList_Result> nextApproverList = new List<fn_Apv_GetApproverList_Result>();
            var nextOfficerApprover = 0;

            if (officerInfo != null)
            {
                nextOfficerApprover = Common.GetInteger(officerInfo.NextApproverId);

                var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                    .Get(q => q.ApprovalProcessId == processId && q.ApplicationId == model.OfficerInfoId &&
                              q.ApproverId == nextOfficerApprover);
                if (application.Any())
                {
                    var approvalStepId = application.LastOrDefault().ApprovalStepId;

                    nextApproverList = GetNextApprover(model.OfficerInfoId, approvalStepId);
                }
            }
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    entity.MedicalOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.Update(entity, "OfficerInfoId");
                    _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.SaveChanges();

                    var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                        .Get(q => q.ApplicationId == entity.OfficerInfoId && q.ApprovalProcessId == processId
                                  && q.IsOnlineApplication == true && q.ApproverId == nextOfficerApprover);
                    if (application.Any())
                    {
                        var applicationId = application.LastOrDefault().Id;

                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, Common.GetInteger(model.NextApproverId), entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";
                    model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                    parentModel.HealthTestReport = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.HealthTestReport = model;
                parentModel.Id = model.OfficerInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();

                model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.HealthTestReport = model;
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete------------------------------
        public ActionResult DeleteHealthTestReport(int id)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "HealthTestReport";

            var entityHealth = _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.GetByID(id, "OfficerInfoId");
            var model = entityHealth.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.Delete(entityHealth);
                    _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";
                    model.ApproverList = new List<SelectListItem>();

                    parentModel.HealthTestReport = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entityHealth.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;
            model.ApproverList = new List<SelectListItem>();

            parentModel.HealthTestReport = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Bio Data

        #region Insert

        public ActionResult OfficerBioDataIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.GetByID(id, "OfficerInfoId");
                if (en != null)
                    return RedirectToAction("EditBioData", "ACRforOfficer", new { id = id });
            }
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "BioData";
            var entity = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);
            var model = parentModel.BioData;
            parentModel.Id = entity.Id;

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var medicalOfficer =
                _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.GetByID(id, "OfficerInfoId");

            if (!IsACRStepAccessable(Convert.ToInt32(id), 2, loginUser.ID)) //-- 2 - Bio-Data
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            #region Only View Part

            model.EmployeeId = entity.EmployeeId;
            model.ACRDate = ((DateTime)entity.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)entity.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)entity.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                .FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            model.DateofBirth = ((DateTime)empInfo.DateofBirth).ToString(DateAndTime.GlobalDateFormat);
            model.DateOfJoinIng = empInfo.DateofJoining.ToString(DateAndTime.GlobalDateFormat);
            model.DateOfJoiningInCurrentPost = empInfo.DateofPosition.ToString(DateAndTime.GlobalDateFormat);
            model.SalaryScaleName = empInfo.PRM_JobGrade.PRM_SalaryScale.SalaryScaleName;
            model.CurrentBasicSalary = _empService.GetEmployeeBasicSalary(entity.EmployeeId);
            model.MaritalStatus = empInfo.PRM_EmpPersonalInfo == null
                ? null
                : empInfo.PRM_EmpPersonalInfo.PRM_MaritalStatus.Name;
            model.FatherName = empInfo.PRM_EmpPersonalInfo == null ? null : empInfo.PRM_EmpPersonalInfo.FatherName;

            #region Education Qualification

            var education = _prmCommonService.PRMUnit.AccademicQualificationRepository.GetAll()
                .Where(x => x.EmployeeId == entity.EmployeeId).ToList();
            List<OfficerBioDataViewModel> EducationList = new List<OfficerBioDataViewModel>();
            foreach (var item in education)
            {
                var gridModel = new OfficerBioDataViewModel
                {
                    ExaminationName = item.PRM_DegreeLevel.Name,
                    AcademicInstitution = item.PRM_UniversityAndBoard.Name,
                    PassingYear = item.YearOfPassing,
                    DivisionOrGrade = item.PRM_AcademicGrade.Name,
                    BoardOrUniversity = item.PRM_UniversityAndBoard.Name,
                    Subject = item.PRM_SubjectGroup.Name
                };
                EducationList.Add(gridModel);
            }
            model.EducationQualificationList = EducationList;

            #endregion

            #region Training

            var training = _prmCommonService.PRMUnit.ProfessionalTrainingRepository.GetAll()
                .Where(x => x.EmployeeId == entity.EmployeeId).ToList();
            List<TrainingViewModel> TrainingList = new List<TrainingViewModel>();
            foreach (var item in training)
            {
                var gridModel1 = new TrainingViewModel
                {
                    TrainingTitle = item.TrainingTitle,
                    Institution = item.OrganizedBy,
                    TraingType = item.TrainingTypeId == null ? null : item.PRM_TrainingType.Name,
                    Location = item.LocationId == null ? null : item.PRM_Location.Name,
                    Country = item.PRM_Country.Name,
                    TrainingYear = item.TrainingYear
                };
                TrainingList.Add(gridModel1);
            }
            model.TrainingList = TrainingList;

            #endregion

            #region Language

            var language = _prmCommonService.PRMUnit.EmployeeLanguageEfficiency.GetAll()
                .Where(x => x.EmployeeId == entity.EmployeeId).ToList();
            List<OfficerBioDataViewModel> LanguageList = new List<OfficerBioDataViewModel>();
            foreach (var item in language)
            {
                var gridModel2 = new OfficerBioDataViewModel
                {
                    Language = item.PRM_Language.Name,
                    Speaking = item.PRM_ProefficiencyLevel2.Name,
                    Reading = item.PRM_ProefficiencyLevel.Name,
                    Writing = item.PRM_ProefficiencyLevel1.Name
                };
                LanguageList.Add(gridModel2);
            }
            model.LanguageList = LanguageList;

            #endregion

            #endregion

            model.ActionType = "CreateBioData";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";
            model.OfficerInfoId = entity.Id;


            #region Reporting Officers Info
            var reportingOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(loginUser.ID);
            if (reportingOfficerInfo != null)
            {
                model.ReportingOfficerId = reportingOfficerInfo.Id;
                model.NameOfReportingOfficer = reportingOfficerInfo.FullName;
                model.Designation = reportingOfficerInfo.PRM_Designation.Name;

                division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == reportingOfficerInfo.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            } 
            #endregion

            var nextApproverList = GetNextApprover(model.OfficerInfoId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateBioData(OfficerBioDataViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "BioData";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    entity.ReportingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.Add(entity);
                    _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.SaveChanges();

                    parentModel.Id = entity.OfficerInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();

                    var healthTests = _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.GetByID(model.OfficerInfoId, "OfficerInfoId");
                    var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;

                    var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                        .Get(q => q.ApplicationId == entity.OfficerInfoId
                                  && q.ApprovalProcessId == processId
                                  && q.IsOnlineApplication == true
                                  && q.ApproverId == healthTests.NextApproverId);

                    if (application.Any())
                    {
                        var applicationId = application.LastOrDefault().Id;

                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Recommend")).First().StatusName;

                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    parentModel.BioData = model;
                    parentModel.BioData.ButtonText = "Save";
                    parentModel.BioData.SelectedClass = "selected";
                    parentModel.BioData.ErrorClass = "failed";
                    parentModel.BioData.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.BioData.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.BioData = model;
                parentModel.BioData.ButtonText = "Save";
                parentModel.BioData.SelectedClass = "selected";
                parentModel.BioData.ErrorClass = "failed";
                parentModel.BioData.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.BioData.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditBioData", "ACRforOfficer", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditBioData(int id, string type)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "BioData";

            var entity = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.GetAll().FirstOrDefault(x => x.OfficerInfoId == id);
            var model = entity.ToModel();

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);

            // if loggedinuser is approver of personal characteristics or performance then he can see bio data also.
            var personalCharacteristicsApproverId = 0;
            var performanceApproverId = 0;
            var personalCharacteristics = _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.GetByID(id, "OfficerInfoId");
            if (personalCharacteristics != null)
            {
                personalCharacteristicsApproverId =
                    _prmCommonService.PRMUnit.EmploymentInfoRepository.First(
                        e => e.EmpID == personalCharacteristics.IUser).Id;
            }
            var performance = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetByID(id, "OfficerInfoId");
            if (performance != null)
            {
                performanceApproverId = performance.ApproverId;
            }

            if (!IsACRStepAccessable(id, 2, loginUser.ID)) //-- 2 - Bio-Data
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }
            //----<<<

            var healthTest = _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.GetByID(id, "OfficerInfoId");
            var nextApproverId = 0;
            var approverId = 0;
            if (healthTest != null)
            {
                nextApproverId = healthTest.NextApproverId;
                approverId = healthTest.ApproverId;
            }

            #region Only View Part

            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            model.DateofBirth = ((DateTime)empInfo.DateofBirth).ToString(DateAndTime.GlobalDateFormat);
            model.DateOfJoinIng = empInfo.DateofJoining.ToString(DateAndTime.GlobalDateFormat);
            model.DateOfJoiningInCurrentPost = empInfo.DateofPosition.ToString(DateAndTime.GlobalDateFormat);
            model.SalaryScaleName = empInfo.PRM_JobGrade.PRM_SalaryScale.SalaryScaleName;
            model.CurrentBasicSalary = _empService.GetEmployeeBasicSalary(entity.EmployeeId);
            model.MaritalStatus = empInfo.PRM_EmpPersonalInfo == null ? null : empInfo.PRM_EmpPersonalInfo.PRM_MaritalStatus.Name;
            model.FatherName = empInfo.PRM_EmpPersonalInfo == null ? null : empInfo.PRM_EmpPersonalInfo.FatherName;

            #region Education Qualification
            var education = _prmCommonService.PRMUnit.AccademicQualificationRepository.GetAll().Where(x => x.EmployeeId == entity.EmployeeId).ToList();
            List<OfficerBioDataViewModel> EducationList = new List<OfficerBioDataViewModel>();
            foreach (var item in education)
            {
                var gridModel = new OfficerBioDataViewModel
                {
                    ExaminationName = item.PRM_DegreeLevel.Name,
                    AcademicInstitution = item.PRM_UniversityAndBoard.Name,
                    PassingYear = item.YearOfPassing,
                    DivisionOrGrade = item.PRM_AcademicGrade.Name,
                    BoardOrUniversity = item.PRM_UniversityAndBoard.Name,
                    Subject = item.PRM_SubjectGroup.Name
                };
                EducationList.Add(gridModel);
            }
            model.EducationQualificationList = EducationList;
            #endregion

            #region Training

            var training = _prmCommonService.PRMUnit.ProfessionalTrainingRepository.GetAll().Where(x => x.EmployeeId == entity.EmployeeId).ToList();
            List<TrainingViewModel> TrainingList = new List<TrainingViewModel>();
            foreach (var item in training)
            {
                var gridModel1 = new TrainingViewModel
                {
                    TrainingTitle = item.TrainingTitle,
                    Institution = item.OrganizedBy,
                    TraingType = item.TrainingTypeId == null ? null : item.PRM_TrainingType.Name,
                    Location = item.LocationId == null ? null : item.PRM_Location.Name,
                    Country = item.PRM_Country.Name,
                    TrainingYear = item.TrainingYear
                };
                TrainingList.Add(gridModel1);
            }
            model.TrainingList = TrainingList;

            #endregion

            #region Language
            var language = _prmCommonService.PRMUnit.EmployeeLanguageEfficiency.GetAll().Where(x => x.EmployeeId == entity.EmployeeId).ToList();
            List<OfficerBioDataViewModel> LanguageList = new List<OfficerBioDataViewModel>();
            foreach (var item in language)
            {
                var gridModel2 = new OfficerBioDataViewModel
                {
                    Language = item.PRM_Language.Name,
                    Speaking = item.PRM_ProefficiencyLevel2.Name,
                    Reading = item.PRM_ProefficiencyLevel.Name,
                    Writing = item.PRM_ProefficiencyLevel1.Name
                };
                LanguageList.Add(gridModel2);
            }
            model.LanguageList = LanguageList;

            #endregion

            #endregion

            #region Reporting Officer
            var reportingOficer = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReportingOfficerId);
            if (reportingOficer != null)
            {
                model.NameOfReportingOfficer = reportingOficer.FullName;
                model.Designation = reportingOficer.PRM_Designation.Name;

                division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == reportingOficer.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            }
            #endregion

            #region Approver List
            model.ApproverList = new List<SelectListItem>();
            var nextApproverList = GetNextApprover(model.OfficerInfoId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            //var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;

            //var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
            //    .Get(q => q.ApprovalProcessId == processId
            //              && q.ApplicationId == id
            //              && q.ApproverId == approverId);
            //if (application.Any())
            //{
            //    var approvalStepId = application.LastOrDefault().ApprovalStepId;

            //    var nextApproverList = GetNextApprover((int)id, approvalStepId);
            //    model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            //} 
            #endregion



            model.ActionType = "EditBioData";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.BioData = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.OfficerInfoId;

            if (type == "success")
            {
                parentModel.BioData.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.BioData.ErrorClass = "success";
                parentModel.BioData.IsError = 0;
            }
            else if (type == "Update")
            {
                parentModel.BioData.Message = Resources.ErrorMessages.UpdateSuccessful;
                parentModel.BioData.ErrorClass = "success";
                parentModel.BioData.IsError = 0;
            }
           
            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditBioData(OfficerBioDataViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "BioData";
            var error = string.Empty;

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            var medicalOfficerInfo = _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.GetByID(model.OfficerInfoId, "OfficerInfoId");
            var approvalStepId = 0;
            var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                .Get(q => q.ApprovalProcessId == processId
                          && q.ApplicationId == model.OfficerInfoId
                          && q.ApproverId == medicalOfficerInfo.NextApproverId);
            if (application.Any())
            {
                approvalStepId = application.LastOrDefault().ApprovalStepId;
            }
            var nextApproverList = GetNextApprover(model.OfficerInfoId, approvalStepId);


            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    entity.ReportingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.Update(entity, "OfficerInfoId");
                    _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.SaveChanges();

                    var medicalOfficer = _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.GetByID(model.OfficerInfoId, "OfficerInfoId");

                    application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                        .Get(q => q.ApplicationId == entity.OfficerInfoId
                                  && q.ApprovalProcessId == processId
                                  && q.IsOnlineApplication == true
                                  && q.ApproverId == medicalOfficer.NextApproverId);
                    if (application.Any())
                    {
                        var applicationId = application.LastOrDefault().Id;
                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";

                    parentModel.BioData = model;
                    parentModel.BioData.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.BioData = model;
                parentModel.Id = model.OfficerInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();

                parentModel.BioData.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                //return View("CreateOrEdit", parentModel);
                return RedirectToAction("EditBioData", "ACRforOfficer", new { id = parentModel.Id, type = "Update" });
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            parentModel.BioData = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete------------------------------
        public ActionResult DeleteBioData(int id)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "BioData";

            var entityBiodata = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.GetByID(id, "OfficerInfoId");
            var model = entityBiodata.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.Delete(entityBiodata);
                    _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";
                    model.ApproverList = new List<SelectListItem>();

                    parentModel.BioData = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entityBiodata.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;
            model.ApproverList = new List<SelectListItem>();

            parentModel.BioData = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Personal Characteristics

        #region Insert
        public ActionResult PersonalCharacteristicsIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.GetByID(id, "OfficerInfoId");
                if (en != null)
                    return RedirectToAction("EditPersonalCharacteristics", "ACRforOfficer", new { id = id });
            }
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PersonalCharacteristics";
            var entity = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var biodataInfo = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.GetByID(id, "OfficerInfoId");
            var nextBioDataApprover = 0;
            if (biodataInfo != null)
            {
                nextBioDataApprover = Common.GetInteger(biodataInfo.NextApproverId);
            }

            if (!IsACRStepAccessable(Convert.ToInt32(id), 3, loginUser.ID)) //-- 3 - Personal Characteristics/Performance of Work
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            var model = parentModel.PersonalCharacteristics;
            parentModel.Id = entity.Id;

            #region Attributes

            var criteria = _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.GetAll().Where(x => x.ACRCriteriaName.Contains("Personal Characteristics")).FirstOrDefault();
            var attributes = (from acrAtt in _prmCommonService.PRMUnit.ACRAttributesInformationRepository.GetAll()
                              join acrAttDtl in _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.GetAll() on acrAtt.Id equals acrAttDtl.ACRAttributesInfoId
                              where (acrAtt.ACRCriteriaInfoId == criteria.Id)
                              select new OfficerPersonalCharacteristicsDetailViewModel()
                              {
                                  AttributeDetailId = acrAttDtl.Id,
                                  AttributeName = acrAttDtl.AttributeName,
                                  SerialNumber = acrAttDtl.SerialNumber
                              }).ToList();

            model.AttributeDetailList = attributes;

            #endregion

            #region View Officer Info

            model.EmployeeId = entity.EmployeeId;
            model.ACRDate = ((DateTime)entity.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)entity.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)entity.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            model.ActionType = "CreatePersonalCharacteristics";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";
            model.OfficerInfoId = entity.Id;

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreatePersonalCharacteristics(OfficerPersonalCharacteristicsViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PersonalCharacteristics";
            var error = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = CreateEntity(model, true);
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;

                    _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.SaveChanges();

                    parentModel.Id = entity.OfficerInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();
                }
                catch (Exception ex)
                {
                    parentModel.PersonalCharacteristics = model;
                    parentModel.PersonalCharacteristics.ButtonText = "Save";
                    parentModel.PersonalCharacteristics.SelectedClass = "selected";
                    parentModel.PersonalCharacteristics.ErrorClass = "failed";
                    parentModel.PersonalCharacteristics.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.PersonalCharacteristics.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.PersonalCharacteristics = model;
                parentModel.PersonalCharacteristics.ButtonText = "Save";
                parentModel.PersonalCharacteristics.SelectedClass = "selected";
                parentModel.PersonalCharacteristics.ErrorClass = "failed";
                parentModel.PersonalCharacteristics.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.PersonalCharacteristics.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditPersonalCharacteristics", "ACRforOfficer", new { id = parentModel.Id, type = "success" });
        }

        #region Previous
        //public OfficerPersonalCharacteristicsViewModel GetReadyModel(OfficerPersonalCharacteristicsViewModel model)
        //{
        //    List<OfficerPersonalCharacteristicsDetailViewModel> AttList = new List<OfficerPersonalCharacteristicsDetailViewModel>();

        //    foreach (var item in model.AttributeDetailList)
        //    {
        //        var attDtlId = item.AttributeDetailId;
        //        var DupList = AttList.Where(x => x.AttributeDetailId == attDtlId).ToList();

        //        if (DupList.Count==0)
        //        {
        //            var gridModel = new OfficerPersonalCharacteristicsDetailViewModel
        //            {
        //                Id=item.Id,
        //                OfficerInfoId = item.OfficerInfoId,
        //                AttributeDetailId = item.AttributeDetailId,
        //                Mark = item.Mark
        //            };
        //            AttList.Add(gridModel);
        //        }
        //        else
        //        {
        //            AttList.RemoveAll(x => x.AttributeDetailId == attDtlId);
        //            var gridModel = new OfficerPersonalCharacteristicsDetailViewModel
        //            {
        //                Id = item.Id,
        //                OfficerInfoId = item.OfficerInfoId,
        //                AttributeDetailId = item.AttributeDetailId,
        //                Mark = item.Mark
        //            };
        //            AttList.Add(gridModel);

        //        }
        //        model.AttributeDetailList = AttList;
        //    }
        //    return model;
        //}
        #endregion

        private PRM_EmpACRPersonalCharacteristics CreateEntity(OfficerPersonalCharacteristicsViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            foreach (var c in model.AttributeDetailList)
            {
                var prm_EmpACRPersonalCharacteristicsDetail = new PRM_EmpACRPersonalCharacteristicsDetail();

                prm_EmpACRPersonalCharacteristicsDetail.Id = c.Id;
                prm_EmpACRPersonalCharacteristicsDetail.OfficerInfoId = model.OfficerInfoId;
                prm_EmpACRPersonalCharacteristicsDetail.AttributeDetailId = c.AttributeDetailId;

                if (c.ChkFour)
                    prm_EmpACRPersonalCharacteristicsDetail.Mark = 4;
                else if (c.ChkThree)
                    prm_EmpACRPersonalCharacteristicsDetail.Mark = 3;
                else if (c.CkhTwo)
                    prm_EmpACRPersonalCharacteristicsDetail.Mark = 2;
                else if (c.ChkOne)
                    prm_EmpACRPersonalCharacteristicsDetail.Mark = 1;
                else
                    prm_EmpACRPersonalCharacteristicsDetail.Mark = 1;

                prm_EmpACRPersonalCharacteristicsDetail.IUser = User.Identity.Name;
                prm_EmpACRPersonalCharacteristicsDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    prm_EmpACRPersonalCharacteristicsDetail.IUser = User.Identity.Name;
                    prm_EmpACRPersonalCharacteristicsDetail.IDate = DateTime.Now;
                    entity.PRM_EmpACRPersonalCharacteristicsDetail.Add(prm_EmpACRPersonalCharacteristicsDetail);
                }
                else
                {
                    prm_EmpACRPersonalCharacteristicsDetail.OfficerInfoId = model.OfficerInfoId;
                    prm_EmpACRPersonalCharacteristicsDetail.EUser = User.Identity.Name;
                    prm_EmpACRPersonalCharacteristicsDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.Add(prm_EmpACRPersonalCharacteristicsDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.Update(prm_EmpACRPersonalCharacteristicsDetail);

                    }
                }
                _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.SaveChanges();

            }

            return entity;
        }
        #endregion

        #region Update--------------------------------------

        public ActionResult EditPersonalCharacteristics(int id, string type)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PersonalCharacteristics";

            var entity = _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.GetAll().Where(x => x.OfficerInfoId == id).FirstOrDefault();
            var model = entity.ToModel();

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var biodataInfo = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.GetByID(id, "OfficerInfoId");
            var nextBiodataApprover = 0;
            if (biodataInfo != null)
            {
                nextBiodataApprover = Common.GetInteger(biodataInfo.NextApproverId);
            }

            if (!IsACRStepAccessable(id, 3, loginUser.ID)) //-- 3 - Personal Characteristics/Performance of Work
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }


            #region Attributes

            List<OfficerPersonalCharacteristicsDetailViewModel> upList = new List<OfficerPersonalCharacteristicsDetailViewModel>();
            var attributes = (from acrAtt in _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.GetAll()
                              join acrAttDtl in _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.GetAll() on acrAtt.AttributeDetailId equals acrAttDtl.Id
                              where (acrAtt.OfficerInfoId == id)
                              select new OfficerPersonalCharacteristicsDetailViewModel()
                              {
                                  Id = acrAtt.Id,
                                  OfficerInfoId = acrAtt.OfficerInfoId,
                                  AttributeDetailId = acrAttDtl.Id,
                                  AttributeName = acrAttDtl.AttributeName,
                                  SerialNumber = acrAttDtl.SerialNumber,
                                  Mark = acrAtt.Mark
                              }).ToList();

            foreach (var item in attributes)
            {
                if (item.Mark == 4)
                {
                    var att = new OfficerPersonalCharacteristicsDetailViewModel()
                    {
                        Id = item.Id,
                        OfficerInfoId = item.OfficerInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        ChkFour = true
                    };
                    upList.Add(att);
                }

                else if (item.Mark == 3)
                {
                    var att = new OfficerPersonalCharacteristicsDetailViewModel()
                    {
                        Id = item.Id,
                        OfficerInfoId = item.OfficerInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        ChkThree = true
                    };
                    upList.Add(att);
                }
                else if (item.Mark == 2)
                {
                    var att = new OfficerPersonalCharacteristicsDetailViewModel()
                    {
                        Id = item.Id,
                        OfficerInfoId = item.OfficerInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        CkhTwo = true
                    };
                    upList.Add(att);
                }
                else
                {
                    var att = new OfficerPersonalCharacteristicsDetailViewModel()
                    {
                        Id = item.Id,
                        OfficerInfoId = item.OfficerInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        ChkOne = true
                    };
                    upList.Add(att);
                }

            }
            model.AttributeDetailList = upList;

            #endregion

            #region View Officer Info

            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            model.ActionType = "EditPersonalCharacteristics";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.PersonalCharacteristics = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.OfficerInfoId;

            if (type == "success")
            {
                parentModel.PersonalCharacteristics.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.PersonalCharacteristics.ErrorClass = "success";
                parentModel.PersonalCharacteristics.IsError = 0;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditPersonalCharacteristics(OfficerPersonalCharacteristicsViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PersonalCharacteristics";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                var entity = CreateEntity(model, false);
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;

                    _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.Update(entity, "OfficerInfoId");
                    _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";

                    parentModel.PersonalCharacteristics = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.PersonalCharacteristics = model;
                parentModel.Id = model.OfficerInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();
                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.PersonalCharacteristics = model;

            return View("CreateOrEdit", parentModel);
        }


        #endregion

        #region Delete------------------------------
        public ActionResult DeletePersonalCharacteristics(int id)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PersonalCharacteristics";

            var entity = _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.GetByID(id, "OfficerInfoId");

            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var allTypes = new List<Type> { typeof(PRM_EmpACRPersonalCharacteristicsDetail) };

                    _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.Delete(id, "OfficerInfoId", allTypes);
                    _prmCommonService.PRMUnit.ACRPersonalCharacteristicsRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";

                    parentModel.PersonalCharacteristics = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.PersonalCharacteristics = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Performance of Work

        #region Insert

        public ActionResult PerformanceOfWorkIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetByID(id, "OfficerInfoId");
                if (en != null)
                    return RedirectToAction("EditPerformanceOfWork", "ACRforOfficer", new { id = id });
            }
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PerformanceOfWork";
            var entityOfficerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);

            var model = parentModel.PerformanceOfWork;
            parentModel.Id = entityOfficerInfo.Id;

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var approvarId = 0;
            var biodataInfo = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.GetByID(id, "OfficerInfoId");
            if (biodataInfo != null)
            {
                approvarId = biodataInfo.ApproverId;
            }

            if (!IsACRStepAccessable(Convert.ToInt32(id), 3, loginUser.ID)) //-- 3 - Personal Characteristics/Performance of Work
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            #region Attributes

            var criteria = _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.GetAll().Where(x => x.ACRCriteriaName.Contains("Performance of Work")).FirstOrDefault();
            var attributes = (from acrAtt in _prmCommonService.PRMUnit.ACRAttributesInformationRepository.GetAll()
                              join acrAttDtl in _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.GetAll() on acrAtt.Id equals acrAttDtl.ACRAttributesInfoId
                              where (acrAtt.ACRCriteriaInfoId == criteria.Id)
                              select new OfficerPerformanceofWorkDetailViewModel()
                              {
                                  AttributeDetailId = acrAttDtl.Id,
                                  AttributeName = acrAttDtl.AttributeName,
                                  SerialNumber = acrAttDtl.SerialNumber
                              }).ToList();

            model.AttributeDetailList = attributes;
            #endregion

            #region View Officer Info

            model.EmployeeId = entityOfficerInfo.EmployeeId;
            model.ACRDate = ((DateTime)entityOfficerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)entityOfficerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)entityOfficerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entityOfficerInfo.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            #region Total Obtain Marks

            var rankList = (from rank in _prmCommonService.PRMUnit.ACRRankInformationRepository.GetAll()
                            select new OfficerPerformanceofWorkViewModel()
                            {
                                RankName = rank.RankName,
                                FromMark = rank.FromMark,
                                ToMark = rank.ToMark
                            }).ToList();
            model.MarkList = rankList;

            var PersonalCharMarkList = _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.GetAll().Where(x => x.OfficerInfoId == entityOfficerInfo.Id).ToList();
            model.TotalObtainMarks = PersonalCharMarkList.Sum(x => x.Mark);
            model.PersonalCharacterMarks = PersonalCharMarkList.Sum(x => x.Mark);
            #endregion

            var reportingOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(Common.GetInteger(loginUser.ID));
            if (reportingOfficerInfo != null)
            {
                model.ReportingOfficerId = reportingOfficerInfo.Id;
                model.NameOfReportingOfficer = reportingOfficerInfo.FullName;
                model.Designation = reportingOfficerInfo.PRM_Designation.Name;
            }

            model.ActionType = "CreatePerformanceOfWork";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";

            model.OfficerInfoId = entityOfficerInfo.Id;
            model.ApproverList = new List<SelectListItem>();
            var nextApproverList = GetNextApprover(model.OfficerInfoId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            return View("CreateOrEdit", parentModel);
        }
        [HttpPost]
        public ActionResult CreatePerformanceOfWork(OfficerPerformanceofWorkViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PerformanceOfWork";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = CreateEntityForPerformance(model, true);
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    //entity.ReportingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.SaveChanges();

                    parentModel.Id = entity.OfficerInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();

                    var biodataInfo = _prmCommonService.PRMUnit
                        .EmpACROfficerBioDataRepository
                        .GetByID(model.OfficerInfoId, "OfficerInfoId");
                    var nextApproverId = 0;
                    if (biodataInfo != null)
                    {
                        nextApproverId = biodataInfo.NextApproverId;
                    }
                    var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;

                    var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                        .Get(q => q.ApplicationId == entity.OfficerInfoId
                                  && q.ApprovalProcessId == processId
                                  && q.IsOnlineApplication == true
                                  && q.ApproverId == nextApproverId);

                    if (application.Any())
                    {
                        var applicationId = application.FirstOrDefault().Id;
                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    parentModel.PerformanceOfWork = model;
                    parentModel.PerformanceOfWork.ButtonText = "Save";
                    parentModel.PerformanceOfWork.SelectedClass = "selected";
                    parentModel.PerformanceOfWork.ErrorClass = "failed";
                    parentModel.PerformanceOfWork.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.PerformanceOfWork.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.PerformanceOfWork = model;
                parentModel.PerformanceOfWork.ButtonText = "Save";
                parentModel.PerformanceOfWork.SelectedClass = "selected";
                parentModel.PerformanceOfWork.ErrorClass = "failed";
                parentModel.PerformanceOfWork.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.PerformanceOfWork.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditPerformanceOfWork", "ACRforOfficer", new { id = parentModel.Id, type = "success" });
        }

        private PRM_EmpACRPerformanceOfWork CreateEntityForPerformance(OfficerPerformanceofWorkViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            var marks = 0;
            foreach (var c in model.AttributeDetailList)
            {
                var prm_EmpACRPerformanceOfWorkDetail = new PRM_EmpACRPerformanceOfWorkDetail();

                prm_EmpACRPerformanceOfWorkDetail.Id = c.Id;
                prm_EmpACRPerformanceOfWorkDetail.OfficerInfoId = model.OfficerInfoId;
                prm_EmpACRPerformanceOfWorkDetail.AttributeDetailId = c.AttributeDetailId;

                if (c.ChkFour)
                {
                    prm_EmpACRPerformanceOfWorkDetail.Mark = 4;
                    marks += 4;
                }
                else if (c.ChkThree)
                {
                    prm_EmpACRPerformanceOfWorkDetail.Mark = 3;
                    marks += 3;
                }
                else if (c.CkhTwo)
                {
                    prm_EmpACRPerformanceOfWorkDetail.Mark = 2;
                    marks += 2;
                }
                else if (c.ChkOne)
                {
                    prm_EmpACRPerformanceOfWorkDetail.Mark = 1;
                    marks += 1;
                }
                else
                {
                    prm_EmpACRPerformanceOfWorkDetail.Mark = 1;
                    marks += 1;
                }
                prm_EmpACRPerformanceOfWorkDetail.IUser = User.Identity.Name;
                prm_EmpACRPerformanceOfWorkDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    prm_EmpACRPerformanceOfWorkDetail.IUser = User.Identity.Name;
                    prm_EmpACRPerformanceOfWorkDetail.IDate = DateTime.Now;
                    entity.PRM_EmpACRPerformanceOfWorkDetail.Add(prm_EmpACRPerformanceOfWorkDetail);
                }
                else
                {
                    prm_EmpACRPerformanceOfWorkDetail.OfficerInfoId = model.OfficerInfoId;
                    prm_EmpACRPerformanceOfWorkDetail.EUser = User.Identity.Name;
                    prm_EmpACRPerformanceOfWorkDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.ACRPerformanceOfWorkDetailRepository.Add(prm_EmpACRPerformanceOfWorkDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.ACRPerformanceOfWorkDetailRepository.Update(prm_EmpACRPerformanceOfWorkDetail);

                    }
                }
                _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.SaveChanges();

            }

            var PersonalCharMarkList = _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.GetAll().Where(x => x.OfficerInfoId == model.OfficerInfoId).ToList();
            var PersonalCharMark = PersonalCharMarkList.Sum(x => x.Mark);
            entity.TotalObtainMarks = PersonalCharMark + marks;

            return entity;
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditPerformanceOfWork(int id, string type)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PerformanceOfWork";

            var entity = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetAll().Where(x => x.OfficerInfoId == id).FirstOrDefault();
            var model = entity.ToModel();

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var nextApproverId = 0;
            var approverId = 0;
            var biodataInfo = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.GetByID(id, "OfficerInfoId");
            if (biodataInfo != null)
            {
                nextApproverId = biodataInfo.NextApproverId;
                approverId = biodataInfo.ApproverId;
            }

            if (!IsACRStepAccessable(id, 3, loginUser.ID)) //-- 3 - Personal Characteristics/Performance of Work
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            #region Attributes

            List<OfficerPerformanceofWorkDetailViewModel> upList = new List<OfficerPerformanceofWorkDetailViewModel>();
            var attributes = (from acrAtt in _prmCommonService.PRMUnit.ACRPerformanceOfWorkDetailRepository.GetAll()
                              join acrAttDtl in _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.GetAll() on acrAtt.AttributeDetailId equals acrAttDtl.Id
                              where (acrAtt.OfficerInfoId == id)
                              select new OfficerPerformanceofWorkDetailViewModel()
                              {
                                  Id = acrAtt.Id,
                                  OfficerInfoId = acrAtt.OfficerInfoId,
                                  AttributeDetailId = acrAttDtl.Id,
                                  AttributeName = acrAttDtl.AttributeName,
                                  SerialNumber = acrAttDtl.SerialNumber,
                                  Mark = acrAtt.Mark
                              }).ToList();

            foreach (var item in attributes)
            {
                if (item.Mark == 4)
                {
                    var att = new OfficerPerformanceofWorkDetailViewModel()
                    {
                        Id = item.Id,
                        OfficerInfoId = item.OfficerInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        ChkFour = true
                    };
                    upList.Add(att);
                }

                else if (item.Mark == 3)
                {
                    var att = new OfficerPerformanceofWorkDetailViewModel()
                    {
                        Id = item.Id,
                        OfficerInfoId = item.OfficerInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        ChkThree = true
                    };
                    upList.Add(att);
                }
                else if (item.Mark == 2)
                {
                    var att = new OfficerPerformanceofWorkDetailViewModel()
                    {
                        Id = item.Id,
                        OfficerInfoId = item.OfficerInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        CkhTwo = true
                    };
                    upList.Add(att);
                }
                else
                {
                    var att = new OfficerPerformanceofWorkDetailViewModel()
                    {
                        Id = item.Id,
                        OfficerInfoId = item.OfficerInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        ChkOne = true
                    };
                    upList.Add(att);
                }

            }
            model.AttributeDetailList = upList;


            #endregion

            #region View Officer Info

            var entityOfficerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            model.ACRDate = ((DateTime)entityOfficerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)entityOfficerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)entityOfficerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            var reportingOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReportingOfficerId);
            if (reportingOfficerInfo != null)
            {
                model.NameOfReportingOfficer = reportingOfficerInfo.FullName;
                model.Designation = reportingOfficerInfo.PRM_Designation.Name;
            }

            #region Total Obtain Marks
            var rankList = (from rank in _prmCommonService.PRMUnit.ACRRankInformationRepository.GetAll()
                            select new OfficerPerformanceofWorkViewModel()
                            {
                                RankName = rank.RankName,
                                FromMark = rank.FromMark,
                                ToMark = rank.ToMark
                            }).ToList();
            model.MarkList = rankList;

            var PersonalCharMarkList = _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.GetAll().Where(x => x.OfficerInfoId == model.OfficerInfoId).ToList();
            var PerformanceMark = _prmCommonService.PRMUnit.ACRPerformanceOfWorkDetailRepository.GetAll().Where(x => x.OfficerInfoId == model.OfficerInfoId).ToList();
            model.TotalObtainMarks = (PersonalCharMarkList.Sum(x => x.Mark) + PerformanceMark.Sum(x => x.Mark));
            model.PersonalCharacterMarks = PersonalCharMarkList.Sum(x => x.Mark);
            #endregion

            model.ActionType = "EditPerformanceOfWork";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.PerformanceOfWork = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.OfficerInfoId;

            if (type == "success")
            {
                parentModel.PerformanceOfWork.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.PerformanceOfWork.ErrorClass = "success";
                parentModel.PerformanceOfWork.IsError = 0;
            }
            
            model.ApproverList = new List<SelectListItem>();
            var nextApproverList = GetNextApprover(model.OfficerInfoId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            //model.ApproverList = new List<SelectListItem>();
            //var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository
            //    .Get(q => q.ProcessNameEnum == "ACR")
            //    .FirstOrDefault().Id;

            //var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
            //    .Get(q => q.ApprovalProcessId == processId
            //              && q.ApplicationId == id
            //              && q.ApproverId == nextApproverId);
            //if (application.Any())
            //{
            //    var approvalStepId = application.FirstOrDefault().ApprovalStepId;
            //    var nextApproverList = GetNextApprover((int)id, approvalStepId);
            //    model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            //}

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditPerformanceOfWork(OfficerPerformanceofWorkViewModel model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PerformanceOfWork";
            var error = string.Empty;

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            var biodataOfficer = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.GetByID(model.OfficerInfoId, "OfficerInfoId");
            var approvalStepId = 0;
            var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                .Get(q => q.ApprovalProcessId == processId
                          && q.ApplicationId == model.OfficerInfoId
                          && q.ApproverId == biodataOfficer.NextApproverId);
            if (application.Any())
            {
                approvalStepId = application.LastOrDefault().ApprovalStepId;
            }
            var nextApproverList = GetNextApprover(model.OfficerInfoId, approvalStepId);

            if (ModelState.IsValid)
            {
                var entity = CreateEntityForPerformance(model, false);
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    entity.ReportingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.Update(entity, "OfficerInfoId");
                    _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.SaveChanges();

                    application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                        .Get(q => q.ApplicationId == entity.OfficerInfoId
                                  && q.ApprovalProcessId == processId
                                  && q.IsOnlineApplication == true
                                  && q.ApproverId == biodataOfficer.NextApproverId);
                    if (application.Any())
                    {
                        var applicationId = application.LastOrDefault().Id;
                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";
                    model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                    parentModel.PerformanceOfWork = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;
                model.ApproverList = new List<SelectListItem>();

                #region Total Obtain Marks

                var PersonalCharMarkList = _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.GetAll().Where(x => x.OfficerInfoId == model.OfficerInfoId).ToList();
                var PerformanceMark = _prmCommonService.PRMUnit.ACRPerformanceOfWorkDetailRepository.GetAll().Where(x => x.OfficerInfoId == model.OfficerInfoId).ToList();
                model.TotalObtainMarks = (PersonalCharMarkList.Sum(x => x.Mark) + PerformanceMark.Sum(x => x.Mark));

                #endregion

                parentModel.PerformanceOfWork = model;
                parentModel.Id = model.OfficerInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();
                parentModel.PerformanceOfWork.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            parentModel.PerformanceOfWork = model;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete------------------------------
        public ActionResult DeletePerformanceOfWork(int id)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "PerformanceOfWork";

            var entity = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetByID(id, "OfficerInfoId");

            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var allTypes = new List<Type> { typeof(PRM_EmpACRPerformanceOfWorkDetail) };

                    _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.Delete(id, "OfficerInfoId", allTypes);
                    _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";
                    model.ApproverList = new List<SelectListItem>();

                    parentModel.PerformanceOfWork = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.PerformanceOfWork = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Graph & Recommendation

        #region Insert
        public ActionResult GraphAndRecommendationIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.GetByID(id, "OfficerInfoId");
                if (en != null)
                    return RedirectToAction("EditGraphAndRecommendation", "ACRforOfficer", new { id = id });
            }

            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "GraphAndRecommendation";
            var entity = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);

            var model = parentModel.GraphAndRecommendation;
            parentModel.Id = entity.Id;

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var performanceApproverId = 0;
            var performanceInfo = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetByID(id, "OfficerInfoId");
            if (performanceInfo != null)
            {
                performanceApproverId = performanceInfo.ApproverId;
            }

            if (!IsACRStepAccessable(Convert.ToInt32(id), 4, loginUser.ID)) //-- 4 - Graph & Recommendation
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            #region View Officer Info

            model.EmployeeId = entity.EmployeeId;
            model.ACRDate = ((DateTime)entity.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)entity.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)entity.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            model.ActionType = "CreateGraphAndRecommendation";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";
            model.OfficerInfoId = entity.Id;

            var reportingOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(Common.GetInteger(loginUser.ID));
            if (reportingOfficerInfo != null)
            {
                model.ReportingOfficerId = reportingOfficerInfo.Id;
                model.NameOfReportingOfficer = reportingOfficerInfo.FullName;
                model.Designation = reportingOfficerInfo.PRM_Designation.Name;

                division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == reportingOfficerInfo.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            }

            var nextApproverList = GetNextApprover(model.OfficerInfoId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateGraphAndRecommendation([Bind(Exclude = "Attachment")] GraphAndRecommendation model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "GraphAndRecommendation";
            var error = string.Empty;
            model = GetReadyModel(model);
            var attachment = Request.Files["attachment"];

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();

                    HttpFileCollectionBase files = Request.Files;
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
                            entity.FileName = name;
                            entity.Attachment = fileData;
                        }

                    }
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    //entity.ReportingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.SaveChanges();

                    parentModel.Id = entity.OfficerInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();

                    var performanceInfo = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetByID(model.OfficerInfoId, "OfficerInfoId");

                    var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;

                    var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                        .Get(q => q.ApplicationId == entity.OfficerInfoId
                                  && q.ApprovalProcessId == processId
                                  && q.IsOnlineApplication == true
                                  && q.ApproverId == performanceInfo.NextApproverId);
                    if (application.Any())
                    {
                        var applicationId = application.LastOrDefault().Id;
                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    parentModel.GraphAndRecommendation = model;
                    parentModel.GraphAndRecommendation.ButtonText = "Save";
                    parentModel.GraphAndRecommendation.SelectedClass = "selected";
                    parentModel.GraphAndRecommendation.ErrorClass = "failed";
                    parentModel.GraphAndRecommendation.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.GraphAndRecommendation.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.GraphAndRecommendation = model;
                parentModel.GraphAndRecommendation.ButtonText = "Save";
                parentModel.GraphAndRecommendation.SelectedClass = "selected";
                parentModel.GraphAndRecommendation.ErrorClass = "failed";
                parentModel.GraphAndRecommendation.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.GraphAndRecommendation.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditGraphAndRecommendation", "ACRforOfficer", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditGraphAndRecommendation(int id, string type)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "GraphAndRecommendation";

            var entity = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.GetAll().Where(x => x.OfficerInfoId == id).FirstOrDefault();
            var model = entity.ToModel();
            DownloadDoc(model);

            var nextApproverId = 0;
            var approverId = 0;
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var performanceInfo = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetByID(id, "OfficerInfoId");
            if (performanceInfo != null)
            {
                nextApproverId = Common.GetInteger(performanceInfo.NextApproverId);
                approverId = Common.GetInteger(performanceInfo.ApproverId);
            }

            if (!IsACRStepAccessable(id, 4, loginUser.ID)) //-- 4 - Graph & Recommendation
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            #region View Officer Info

            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            #region Reporting Officer Info
            var reportingOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReportingOfficerId);
            if (reportingOfficerInfo != null)
            {
                model.NameOfReportingOfficer = reportingOfficerInfo.FullName;
                model.Designation = reportingOfficerInfo.PRM_Designation.Name;

                division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == reportingOfficerInfo.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            }
            #endregion

            #region Qualification
            if (model.QualificationForPromotion == "EligiblePro")
                model.A = true;
            if (model.QualificationForPromotion == "NotEligiblePro")
                model.B = true;
            if (model.QualificationForPromotion == "HighlyRecomPro")
                model.C = true;
            if (model.QualificationForPromotion == "RecentlyPro")
                model.D = true;

            #endregion

            model.ActionType = "EditGraphAndRecommendation";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.GraphAndRecommendation = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.OfficerInfoId;

            if (type == "success")
            {
                parentModel.GraphAndRecommendation.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.GraphAndRecommendation.ErrorClass = "success";
                parentModel.GraphAndRecommendation.IsError = 0;
            }

            model.ApproverList = new List<SelectListItem>();
            var nextApproverList = GetNextApprover(model.OfficerInfoId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            //var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            //var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
            //    .Get(q => q.ApprovalProcessId == processId
            //              && q.ApplicationId == id
            //              && q.ApproverId == nextApproverId);
            //if (application.Any())
            //{
            //    var approvalStepId = application.LastOrDefault().ApprovalStepId;
            //    var nextApproverList = GetNextApprover((int)id, approvalStepId);
            //    model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            //}

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditGraphAndRecommendation(GraphAndRecommendation model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "GraphAndRecommendation";
            var error = string.Empty;

            model = GetReadyModel(model);

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            var performanceInfo = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetByID(model.OfficerInfoId, "OfficerInfoId");
            var approvalStepId = 0;
            var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                .Get(q => q.ApprovalProcessId == processId
                          && q.ApplicationId == model.OfficerInfoId
                          && q.ApproverId == performanceInfo.ApproverId);
            if (application.Any())
            {
                approvalStepId = application.FirstOrDefault().ApprovalStepId;
            }
            var nextApproverList = GetNextApprover(model.OfficerInfoId, approvalStepId);

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    var obj = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.GetAll().FirstOrDefault(x => x.OfficerInfoId == model.OfficerInfoId);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    //
                    HttpFileCollectionBase files = Request.Files;
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
                            entity.FileName = name;
                            entity.Attachment = fileData;
                        }
                    }

                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    entity.ReportingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.Update(entity, "OfficerInfoId");
                    _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.SaveChanges();

                    application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                        .Get(q => q.ApplicationId == entity.OfficerInfoId
                                  && q.ApprovalProcessId == processId
                                  && q.IsOnlineApplication == true
                                  && q.ApproverId == performanceInfo.NextApproverId);
                    if (application.Any())
                    {
                        var applicationId = application.LastOrDefault().Id;
                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";
                    model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                    parentModel.GraphAndRecommendation = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;
                model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

                parentModel.GraphAndRecommendation = model;
                parentModel.Id = model.OfficerInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();

                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            parentModel.GraphAndRecommendation = model;

            return View("CreateOrEdit", parentModel);
        }

        public GraphAndRecommendation GetReadyModel(GraphAndRecommendation model)
        {
            if (model.A == true)
            {
                model.QualificationForPromotion = "EligiblePro";
            }
            if (model.B == true)
            {
                model.QualificationForPromotion = "NotEligiblePro";
            }

            if (model.C == true)
            {
                model.QualificationForPromotion = "HighlyRecomPro";
            }

            if (model.D == true)
            {
                model.QualificationForPromotion = "RecentlyPro";
            }
            return model;
        }
        #endregion

        #region Delete------------------------------
        public ActionResult DeleteGraphAndRecommendation(int id)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "GraphAndRecommendation";

            var entity = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.GetByID(id, "OfficerInfoId");
            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.Delete(entity);
                    _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";

                    parentModel.GraphAndRecommendation = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.GraphAndRecommendation = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Reviewing Officer's Comments

        #region Insert
        public ActionResult ReviewingOfficerCommentsIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.GetByID(id, "OfficerInfoId");
                if (en != null)
                    return RedirectToAction("EditReviewingOfficerComments", "ACRforOfficer", new { id = id });
            }
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "ReviewingOfficerComments";
            var entity = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);

            var model = parentModel.ReviewingOfficerComments;
            parentModel.Id = entity.Id;

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var graphRecommender = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.GetByID(id, "OfficerInfoId");

            if (!IsACRStepAccessable(Convert.ToInt32(id), 5, loginUser.ID)) //-- 5 - Reviewing Officer's Comment/Information for Authority
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            #region View Officer Info

            model.EmployeeId = entity.EmployeeId;
            model.ACRDate = ((DateTime)entity.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)entity.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)entity.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            var TotalMarks = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.GetAll().Where(x => x.OfficerInfoId == id).FirstOrDefault();
            if (TotalMarks == null)
            {
                model.TotalMarks = 0;
            }
            else
            {
                model.TotalMarks = TotalMarks.TotalObtainMarks;
            }

            model.ActionType = "CreateReviewingOfficerComments";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";
            model.OfficerInfoId = entity.Id;

            #region Counter Signing Officer Info
            var counterSigningOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(Common.GetInteger(loginUser.ID));
            if (counterSigningOfficerInfo != null)
            {
                model.ReviewingOfficerId = counterSigningOfficerInfo.Id;
                model.NameOfReportingOfficer = counterSigningOfficerInfo.FullName;
                model.Designation = counterSigningOfficerInfo.PRM_Designation.Name;

                model.Department = counterSigningOfficerInfo.DivisionId != null
                    ? counterSigningOfficerInfo.PRM_Division.Name
                    : String.Empty;
            }
            #endregion

            var nextApproverList = GetNextApprover(model.OfficerInfoId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateReviewingOfficerComments(ReviewingOfficerComments model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "ReviewingOfficerComments";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();

                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Approve")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    //entity.ReviewingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.SaveChanges();

                    parentModel.Id = entity.OfficerInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();

                    var graphRecommender = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.GetByID(model.OfficerInfoId, "OfficerInfoId");

                    var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;

                    var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                        .Get(q => q.ApplicationId == entity.OfficerInfoId
                                  && q.ApprovalProcessId == processId
                                  && q.IsOnlineApplication == true
                                  && q.ApproverId == graphRecommender.NextApproverId);
                    if (application.Any())
                    {
                        var applicationId = application.LastOrDefault().Id;
                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Approve")).First().StatusName;
                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    parentModel.ReviewingOfficerComments = model;
                    parentModel.ReviewingOfficerComments.ButtonText = "Save";
                    parentModel.ReviewingOfficerComments.SelectedClass = "selected";
                    parentModel.ReviewingOfficerComments.ErrorClass = "failed";
                    parentModel.ReviewingOfficerComments.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.ReviewingOfficerComments.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.ReviewingOfficerComments = model;
                parentModel.ReviewingOfficerComments.ButtonText = "Save";
                parentModel.ReviewingOfficerComments.SelectedClass = "selected";
                parentModel.ReviewingOfficerComments.ErrorClass = "failed";
                parentModel.ReviewingOfficerComments.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.ReviewingOfficerComments.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditReviewingOfficerComments", "ACRforOfficer", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditReviewingOfficerComments(int id, string type)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "ReviewingOfficerComments";

            var entity = _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.GetAll()
                .Where(x => x.OfficerInfoId == id)
                .FirstOrDefault();
            var model = entity.ToModel();

            var nextApproverId = 0;
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var graph = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.GetByID(id, "OfficerInfoId");
            if (graph != null)
            {
                nextApproverId = Common.GetInteger(graph.NextApproverId);
            }

            if (!IsACRStepAccessable(Convert.ToInt32(id), 5, loginUser.ID)) //-- 5 - Reviewing Officer's Comment/Information for Authority
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            #region View Officer Info

            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            #region Counter Signing Officer Info
            var counterSigningOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReviewingOfficerId);
            if (counterSigningOfficerInfo != null)
            {
                model.NameOfReportingOfficer = counterSigningOfficerInfo.FullName;
                model.Designation = counterSigningOfficerInfo.PRM_Designation.Name;

                division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                    .FirstOrDefault(d => d.Id == counterSigningOfficerInfo.DivisionId);
                model.Department = division != null ? division.Name : String.Empty;
            }
            #endregion

            model.ActionType = "EditReviewingOfficerComments";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.ReviewingOfficerComments = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.OfficerInfoId;

            if (type == "success")
            {
                parentModel.ReviewingOfficerComments.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.ReviewingOfficerComments.ErrorClass = "success";
                parentModel.ReviewingOfficerComments.IsError = 0;
            }

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                .Get(q => q.ApprovalProcessId == processId
                            && q.ApplicationId == id
                            && q.ApproverId == nextApproverId);
            if (application.Any())
            {
                var approvalStepId = application.LastOrDefault().ApprovalStepId;
                var nextApproverList = GetNextApprover((int)id, approvalStepId);
                //model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditReviewingOfficerComments(ReviewingOfficerComments model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "ReviewingOfficerComments";
            var error = string.Empty;

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;

            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);

            var graph = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.GetByID(model.OfficerInfoId, "OfficerInfoId");
            var nextGraphApprover = Common.GetInteger(graph.NextApproverId);
            List<fn_Apv_GetApproverList_Result> nextApproverList = new List<fn_Apv_GetApproverList_Result>();

            var application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                .Get(q => q.ApprovalProcessId == processId && q.ApplicationId == model.OfficerInfoId &&
                          q.ApproverId == nextGraphApprover);
            if (application.Any())
            {
                var approvalStepId = application.LastOrDefault().ApprovalStepId;
                nextApproverList = GetNextApprover(model.OfficerInfoId, approvalStepId);
            }

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Approve")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    entity.ReviewingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.Update(entity, "OfficerInfoId");
                    _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.SaveChanges();

                    application = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository
                       .Get(q => q.ApplicationId == entity.OfficerInfoId
                                 && q.ApprovalProcessId == processId && q.IsOnlineApplication == true &&
                                 q.ApproverId == nextGraphApprover);
                    if (application.Any())
                    {
                        var applicationId = application.LastOrDefault().Id;
                        string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository
                            .Get(q => q.StatusName.Contains("Approve")).First().StatusName;
                        var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId,
                            actionName, model.ApproverComments, Common.GetInteger(model.NextApproverId), entity.IUser);
                    }
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";
                    //model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                    parentModel.ReviewingOfficerComments = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;
                //model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                parentModel.ReviewingOfficerComments = model;
                parentModel.Id = model.OfficerInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();
                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";
            //model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            parentModel.ReviewingOfficerComments = model;

            return View("CreateOrEdit", parentModel);
        }


        #endregion

        #region Delete------------------------------
        public ActionResult DeleteReviewingOfficerComments(int id)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "ReviewingOfficerComments";

            var entity = _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.GetByID(id, "OfficerInfoId");
            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.Delete(entity);
                    _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";

                    parentModel.ReviewingOfficerComments = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.ReviewingOfficerComments = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Information for Authority

        #region Insert
        public ActionResult InformationForAuthorityIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.GetByID(id, "OfficerInfoId");
                if (en != null)
                    return RedirectToAction("EditInformationForAuthority", "ACRforOfficer", new { id = id });
            }

            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "InformationForAuthority";
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            if (!IsACRStepAccessable(Convert.ToInt32(id), 5, loginUser.ID)) //-- 5 - Reviewing Officer's Comment/Information for Authority
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            var counterOfficerInfo = _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository.GetByID(id, "OfficerInfoId");
            if (counterOfficerInfo == null)
            {
                return View("CreateOrEdit", parentModel);
            }

            var statusInfo = _prmCommonService.PRMUnit.ApprovalStatusRepository.GetByID(counterOfficerInfo.ApprovalStatusId);
            if (statusInfo.StatusName.Contains("Approve"))
            {
                var entity = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(id);

                var model = parentModel.InformationForAuthority;
                parentModel.Id = entity.Id;

                #region View Officer Info

                model.EmployeeId = entity.EmployeeId;
                model.ACRDate = ((DateTime)entity.ACRDate).ToString(DateAndTime.GlobalDateFormat);
                model.ACRPeriodFrom = ((DateTime)entity.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
                model.ACRPeriodTo = ((DateTime)entity.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

                var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
                model.EmpId = empInfo.EmpID;
                model.EmployeeName = empInfo.FullName;

                var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
                model.EmployeeDepartment = division != null ? division.Name : String.Empty;

                model.EmployeeDesignation = empInfo.PRM_Designation.Name;
                model.SeniorityNumber = empInfo.SeniorityPosition;

                #endregion

                model.ActionType = "CreateInformationForAuthority";
                model.ButtonText = "Save";
                model.SelectedClass = "selected";
                model.OfficerInfoId = entity.Id;
            }
            return View("CreateOrEdit", parentModel);

        }

        [HttpPost]
        public ActionResult CreateInformationForAuthority(InformationForAuthority model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "InformationForAuthority";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();

                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;

                    _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.SaveChanges();

                    parentModel.Id = entity.OfficerInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();
                }
                catch (Exception ex)
                {
                    parentModel.InformationForAuthority = model;
                    parentModel.InformationForAuthority.ButtonText = "Save";
                    parentModel.InformationForAuthority.SelectedClass = "selected";
                    parentModel.InformationForAuthority.ErrorClass = "failed";
                    parentModel.InformationForAuthority.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.InformationForAuthority.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.InformationForAuthority = model;
                parentModel.InformationForAuthority.ButtonText = "Save";
                parentModel.InformationForAuthority.SelectedClass = "selected";
                parentModel.InformationForAuthority.ErrorClass = "failed";
                parentModel.InformationForAuthority.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.InformationForAuthority.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditInformationForAuthority", "ACRforOfficer", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditInformationForAuthority(int id, string type)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "InformationForAuthority";
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            if (!IsACRStepAccessable(Convert.ToInt32(id), 5, loginUser.ID)) //-- 5 - Reviewing Officer's Comment/Information for Authority
            {
                return RedirectToAction("EditOfficerInfo", "ACRforOfficer", new { id = id, type = string.Empty });
            }

            var entity = _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.GetAll().Where(x => x.OfficerInfoId == id).FirstOrDefault();
            var model = entity.ToModel();

            #region View Officer Info

            var officerInfo = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.GetByID(model.OfficerInfoId);
            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString(DateAndTime.GlobalDateFormat);
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString(DateAndTime.GlobalDateFormat);

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = empInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll().FirstOrDefault(d => d.Id == empInfo.DivisionId);
            model.EmployeeDepartment = division != null ? division.Name : String.Empty;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            #region Receiving Officer Info
            var receivingOfficerInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivingOfficerId);
            if (receivingOfficerInfo != null)
            {
                model.NameOfReportingOfficer = receivingOfficerInfo.FullName;
                model.Designation = receivingOfficerInfo.PRM_Designation.Name;

                model.Department = receivingOfficerInfo.DivisionId != null
                    ? entity.PRM_EmploymentInfo1.PRM_Division.Name
                    : String.Empty;
            }
            #endregion

            model.ActionType = "EditInformationForAuthority";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.InformationForAuthority = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.OfficerInfoId;

            if (type == "success")
            {
                parentModel.InformationForAuthority.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.InformationForAuthority.ErrorClass = "success";
                parentModel.InformationForAuthority.IsError = 0;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditInformationForAuthority(InformationForAuthority model)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "InformationForAuthority";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;

                    _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.Update(entity, "OfficerInfoId");
                    _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";

                    parentModel.InformationForAuthority = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.InformationForAuthority = model;
                parentModel.Id = model.OfficerInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();
                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.InformationForAuthority = model;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete------------------------------
        public ActionResult DeleteInformationForAuthority(int id)
        {
            var parentModel = new ACRforOfficerViewModel();
            parentModel.ViewType = "InformationForAuthority";

            var entity = _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.GetByID(id, "OfficerInfoId");
            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.Delete(entity);
                    _prmCommonService.PRMUnit.ACRInformationForAuthorityRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";

                    parentModel.InformationForAuthority = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.InformationForAuthority = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Attachment

        private int Upload(GraphAndRecommendation model)
        {
            if (model.File == null)
                return 0;

            try
            {
                var uploadFile = model.File;

                byte[] data;
                using (Stream inputStream = uploadFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
                model.Attachment = data;
                model.FileName = uploadFile.FileName;
                model.IsError = 0;

            }
            catch (Exception ex)
            {
                model.IsError = 1;
            }

            return model.IsError;
        }

        public void DownloadDoc(GraphAndRecommendation model)
        {
            byte[] document = model.Attachment;
            if (document != null)
            {
                string strFilename = Url.Content("~/Content/" + User.Identity.Name + model.FileName);
                byte[] doc = document;
                WriteToFile(Server.MapPath(strFilename), ref doc);

                model.FilePath = strFilename;
            }
        }

        private void WriteToFile(string strPath, ref byte[] Buffer)
        {
            FileStream newFile = null;

            try
            {
                newFile = new FileStream(strPath, FileMode.Create);

                newFile.Write(Buffer, 0, Buffer.Length);
                newFile.Close();
            }
            catch (Exception ex)
            {
                if (newFile != null) newFile.Close();
            }
        }

        #endregion

        #region Approver Information

        private List<APV_GetApproverInfoByApplicant_Result> GetApprover(string empId)
        {
            var approvalProcessName = @"ACR";
            var approverInfo = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(empId, approvalProcessName).DefaultIfEmpty().OfType<APV_GetApproverInfoByApplicant_Result>().ToList();
            return approverInfo;
        }

        private List<fn_Apv_GetNextApprover_Result> GetNextApprover(int applicationId)
        {
            int approvalProcessId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == @"ACR").FirstOrDefault().Id;
            var approverInfo = _prmCommonService.PRMUnit.FunctionRepository.GetNextApprover(approvalProcessId, applicationId).DefaultIfEmpty().OfType<fn_Apv_GetNextApprover_Result>().ToList();
            return approverInfo;
        }

        private List<fn_Apv_GetApproverList_Result> GetNextApprover(int applicationId, int approvalStepId)
        {
            int approvalProcessId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == @"ACR").FirstOrDefault().Id;
            var approverList = _prmCommonService.PRMUnit.FunctionRepository.GetNextApprover(approvalProcessId, applicationId, approvalStepId).DefaultIfEmpty().OfType<fn_Apv_GetApproverList_Result>().ToList();
            return approverList;
        }

        public JsonResult GetApproverInformation(string empId)
        {
            try
            {
                var approverInfo = GetApprover(empId);
                return Json(approverInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion


        private bool IsACRStepAccessable(int acrId, int wantToAccessACRStep, int loggedinEmployeeId)
        {
            bool result = false;

            int approverOfACRStep = 0;

            var officerInfos = _prmCommonService.PRMUnit.EmpACROfficerInfoRepository.Get(r => r.Id == acrId);
            var healthTests = _prmCommonService.PRMUnit.OfficerHealthTestReportRepository.Get(r => r.OfficerInfoId == acrId);
            var bioDatas = _prmCommonService.PRMUnit.EmpACROfficerBioDataRepository.Get(r => r.OfficerInfoId == acrId);
            var performanceOfWorks = _prmCommonService.PRMUnit.ACRPerformanceOfWorkRepository.Get(r => r.OfficerInfoId == acrId);
            var graphAndRecommendations = _prmCommonService.PRMUnit.ACRGraphAndRecommendationRepository.Get(r => r.OfficerInfoId == acrId);

            var reviewingOfficer = _prmCommonService.PRMUnit.ACRReviewingOfficerCommentsRepository
                .Get(r => r.ApproverId == loggedinEmployeeId && r.OfficerInfoId == acrId);
            if (reviewingOfficer.Any())
            {
                approverOfACRStep = 5; //-- Have access of reviewing officer/Information for authority
            }
            else
            {
                graphAndRecommendations = graphAndRecommendations.Where(r => r.NextApproverId == loggedinEmployeeId);
                if (graphAndRecommendations.Any())
                {
                    approverOfACRStep = 5; //-- Have access of reviewing officer/Information for authority
                }
                else
                {
                    graphAndRecommendations = graphAndRecommendations.Where(r => r.ApproverId == loggedinEmployeeId);
                    if (graphAndRecommendations.Any())
                    {
                        approverOfACRStep = 4; //-- Have access of Graph & Recommendation
                    }
                    else
                    {
                        performanceOfWorks = performanceOfWorks.Where(r => r.NextApproverId == loggedinEmployeeId);
                        if (performanceOfWorks.Any())
                        {
                            approverOfACRStep = 4; //-- Have access of Graph & Recommendation
                        }
                        else
                        {
                            performanceOfWorks = performanceOfWorks.Where(r => r.ApproverId == loggedinEmployeeId);
                            if (performanceOfWorks.Any())
                            {
                                approverOfACRStep = 3; //-- Have access of Personal Characteristics/Performance
                            }
                            else
                            {
                                bioDatas = bioDatas.Where(r => r.NextApproverId == loggedinEmployeeId);
                                if (bioDatas.Any())
                                {
                                    approverOfACRStep = 3; //-- Have access of Personal Characteristics/Performance
                                }
                                else
                                {
                                    bioDatas = bioDatas.Where(r => r.ApproverId == loggedinEmployeeId);
                                    if (bioDatas.Any())
                                    {
                                        approverOfACRStep = 2; //-- Have access of Bio-Data
                                    }
                                    else
                                    {
                                        healthTests = healthTests.Where(r => r.NextApproverId == loggedinEmployeeId);
                                        if (healthTests.Any())
                                        {
                                            approverOfACRStep = 2; //-- Have access of Bio-Data
                                        }
                                        else
                                        {
                                            healthTests = healthTests.Where(r => r.ApproverId == loggedinEmployeeId);
                                            if (healthTests.Any())
                                            {
                                                approverOfACRStep = 1; //-- Have access of health test
                                            }
                                            else
                                            {
                                                officerInfos =
                                                    officerInfos.Where(r => r.NextApproverId == loggedinEmployeeId);
                                                if (officerInfos.Any())
                                                {
                                                    approverOfACRStep = 1; //-- Have access of health test
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            if (approverOfACRStep >= wantToAccessACRStep)
                result = true;

            return result;
        }
        
    }
}