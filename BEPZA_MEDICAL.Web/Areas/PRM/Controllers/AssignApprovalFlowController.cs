using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Utility;
using System.Web.Mvc;
using System.Linq;
using BEPZA_MEDICAL.DAL.PRM;
using System.Collections.Generic;
using System;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Resources;
using System.Data;
using System.Data.SqlClient;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class AssignApprovalFlowController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly EmployeeService _empService;
        #endregion

        public AssignApprovalFlowController(PRMCommonSevice prmCommonService, EmployeeService empService)
        {
            this._prmCommonservice = prmCommonService;
            this._empService = empService;
        }

        public AssignApprovalFlowController(EmployeeService empService)
        {
            this._empService = empService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            AssignApprovalFlowViewModel model = new AssignApprovalFlowViewModel();
            PopulateDropDown(model);
            model.ActionType = "Save";
            model.ApprovalFlowInitializationList = new List<ApprovalFlowDrawViewModel>();
            return View("CreateOrEdit", model);
        }

        public JsonResult GetApprovalFlowListByProcessId(int processId)
        {
            var flowList = _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.Get(t => t.ApprovalProcesssId == processId).DefaultIfEmpty().OfType<APV_ApprovalFlowMaster>().ToList();
            var list = new List<ApprovalFlowViewModel>();
            foreach (var item in flowList)
            {
                list.Add(new ApprovalFlowViewModel
                {
                    Id = item.Id,
                    ApprovalFlowName = item.ApprovalFlowName
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void PopulateDropDown(AssignApprovalFlowViewModel model)
        {
            var zoneList = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll();
            model.ZoneList = Common.PopulateDdlZoneListWithAllOption(zoneList);

            var approvalProcessList = _prmCommonservice.PRMUnit.ApprovalProcessRepository.GetAll();
            model.ApprovalProcessList = Common.PopulateDdlApprovalProcess(approvalProcessList);

            model.ApprovalFlowList = new List<SelectListItem>();

            var empCategoryList = _prmCommonservice.PRMUnit.EmploymentTypeRepository.GetAll();
            var anEmpCategory = new PRM_EmploymentType { Id = 0, Name = "ALL" };
            var myEmpCategoryList = new List<PRM_EmploymentType> { anEmpCategory };
            myEmpCategoryList.AddRange(empCategoryList);
            model.EmployeeCategoryList = Common.PopulateDllList(myEmpCategoryList);

            model.GenderList = Common.PopulateGenderDDLWithAllOption();
            model.DesignationList = new List<SelectListItem>();

            var staffCategoryList = _prmCommonservice.PRMUnit.PRM_StaffCategoryRepository.GetAll();
            var aCategory = new PRM_StaffCategory { Id = 0, Name = "ALL" };
            var myStaffCategoryList = new List<PRM_StaffCategory> { aCategory };
            myStaffCategoryList.AddRange(staffCategoryList);
            model.StaffCategoryList = Common.PopulateDllList(myStaffCategoryList);
        }

        public ActionResult GetDesignationInfoByLevelId(string levelId)
        {
            int organogramLevelId = 0;
            int.TryParse(levelId, out organogramLevelId);
            var desigList = (from JG in _empService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Fetch()
                             join de in _empService.PRMUnit.DesignationRepository.Fetch() on JG.DesignationId equals de.Id
                             where JG.OrganogramLevelId == organogramLevelId
                             select de).OrderBy(o => o.Name).ToList();

            var finalList = (from x in desigList
                             select new PRM_Designation()
                             {
                                 Id = x.Id,
                                 Name = x.Name
                             }).ToList();
            return Json(finalList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(AssignApprovalFlowViewModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = model.ToEntity();
                try
                {
                    string errorMessage = CheckBusinessValidation(model);
                    int initialStepIdInt = 0;
                    int.TryParse(model.InitialStepId.ToString(), out initialStepIdInt);
                    if (initialStepIdInt > 0)
                    {
                        obj.InitialStepId = initialStepIdInt;
                    }
                    else
                    {
                        obj.InitialStepId = null;
                    }
                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        if (!obj.IsApplicableForGroup)
                        {
                            var employee = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(t => t.EmpID == model.EmpId).FirstOrDefault();
                            obj.OrganogramLevelId = null;
                            obj.DesignationId = employee.DesignationId;
                            obj.EmployeeCategory = employee.EmploymentTypeId;
                            obj.Gender = employee.Gender;
                            obj.EmployeeId = employee.Id;
                        }
                        else
                        {
                            obj.EmployeeId = null;
                        }

                        _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.Add(obj);
                        _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = errorMessage;
                        PopulateDropDown(model);
                        model.ActionType = "Save";
                        return View("CreateOrEdit", model);
                    }
                }
                catch (Exception)
                {
                    PopulateDropDown(model);
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    return View("CreateOrEdit", model);
                }
            }
            return View("Index", model);
        }

        public ActionResult Update(AssignApprovalFlowViewModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = model.ToEntity();
                try
                {
                    string errorMessage = CheckBusinessValidation(model);

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        if (!obj.IsApplicableForGroup)
                        {
                            var employee = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(t => t.EmpID == model.EmpId).FirstOrDefault();
                            obj.OrganogramLevelId = null;
                            obj.DesignationId = employee.DesignationId;
                            obj.EmployeeCategory = employee.EmploymentTypeId;
                            obj.Gender = employee.Gender;
                            obj.EmployeeId = employee.Id;
                        }
                        else
                        {
                            obj.EmployeeId = null;
                        }

                        _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.Update(obj);
                        _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = errorMessage;
                        PopulateDropDown(model);
                        model.ActionType = "Update";
                        return View("CreateOrEdit", model);
                    }
                }
                catch (Exception)
                {
                    PopulateDropDown(model);
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    return View("CreateOrEdit", model);
                }
            }
            return View("Index", model);
        }

        private string CheckBusinessValidation(AssignApprovalFlowViewModel model)
        {
            string errorMessage = string.Empty;
            var list = _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.Get(t => t.ApprovalProcessId == model.ApprovalProcessId && t.ApprovalMasterId == model.ApprovalMasterId).DefaultIfEmpty().OfType<APV_EmployeeWiseApproverInfo>().ToList();
            var emp = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(t => t.EmpID == model.EmpId).FirstOrDefault();

            switch (model.ActionType)
            {
                case "Save":
                    if (model.IsApplicableForGroup)
                    {
                        list = list.Where(t => t.DesignationId == model.DesignationId && t.OrganogramLevelId == model.OrganogramLevelId && t.EmployeeCategory == model.EmployeeCategory && t.Gender == model.Gender).ToList();
                    }
                    else
                    {
                        list = list.Where(t => t.EmployeeId == emp.Id).ToList();
                    }
                    break;
                case "Update":
                    if (model.IsApplicableForGroup)
                    {
                        list = list.Where(t => t.DesignationId == model.DesignationId && t.OrganogramLevelId == model.OrganogramLevelId && t.EmployeeCategory == model.EmployeeCategory && t.Gender == model.Gender && t.Id != model.Id).ToList();
                    }
                    else
                    {
                        list = list.Where(t => t.EmployeeId == emp.Id && t.Id != model.Id).ToList();
                    }
                    break;
            }
            if (list != null && list.Count > 0)
            {
                errorMessage += "Already assigned.";
            }
            return errorMessage;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AssignApprovalFlowViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from x in _prmCommonservice.PRMUnit.AssignedApprovalFlowViewRepository.GetAll()
                        select new AssignApprovalFlowViewModel()
                        {
                            Id = x.Id,
                            ProcessName = x.ApprovalProcessName,
                            ApprovalProcessId = x.ApprovalProcessId,
                            ApprovalMasterId = x.ApprovalFlowMasterId,
                            FlowName = x.ApprovalFlowName,
                            EmployeeId = x.EmployeeId,
                            EmpId = x.EmpID,
                            EmployeeName = x.EmployeeName,
                            DivisionId = x.DivisionId,
                            DivisionOrUnit = x.DivisionName,
                            DepartmentId = x.DepartmentId,
                            Department = x.DepartmentName,
                            SectionId = x.SectionId,
                            Section = x.SectionName,
                            DesignationId = x.DesignationId,
                            Designation = x.DesignatioName,
                            EmployeeIdAndName = x.EmpID != string.Empty ? x.EmployeeName + " (" + x.EmpID + " )" : string.Empty,
                            OrganogramLevelName = x.OrganogramLevel,
                            ZoneName = x.ZoneCode,
                            GroupOrIndividual = x.GroupOrIndividual
                        }).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.FlowName))
                {
                    if (!viewModel.FlowName.Equals("0"))
                    {
                        list = list.Where(q => q.ApprovalMasterId.ToString() == viewModel.FlowName).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.ProcessName))
                {
                    if (!viewModel.ProcessName.Equals("0"))
                    {
                        list = list.Where(q => q.ApprovalProcessId.ToString() == viewModel.ProcessName).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.EmpId))
                {
                    if (!string.IsNullOrWhiteSpace(viewModel.EmpId))
                    {
                        list = list.Where(q => q.EmpId == viewModel.EmpId).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.EmployeeName))
                {
                    if (!string.IsNullOrWhiteSpace(viewModel.EmployeeName))
                    {
                        list = list.Where(q => q.EmployeeName != null && q.EmployeeName.Contains(viewModel.EmployeeName)).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(viewModel.DivisionOrUnit))
                {
                    if (!viewModel.DivisionOrUnit.Equals("0"))
                    {
                        list = list.Where(q => q.DivisionId.ToString() == viewModel.DivisionOrUnit).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(viewModel.Department))
                {
                    if (!viewModel.Department.Equals("0"))
                    {
                        list = list.Where(q => q.DepartmentId.ToString() == viewModel.Department).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.Section))
                {
                    if (!viewModel.Section.Equals("0"))
                    {
                        list = list.Where(q => q.SectionId.ToString() == viewModel.Section).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.Designation))
                {
                    if (!viewModel.Designation.Equals("0"))
                    {
                        list = list.Where(q => q.DesignationId.ToString() == viewModel.Designation).ToList();
                    }
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "FlowName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FlowName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FlowName).ToList();
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
                    d.Id,
                    d.ProcessName,
                    d.FlowName,
                    d.ZoneName,
                    d.GroupOrIndividual,
                    d.OrganogramLevelName,
                    d.EmployeeId,
                    d.EmpId,
                    d.EmployeeIdAndName,
                    d.EmployeeName,
                    d.DivisionOrUnit,
                    d.Department,
                    d.Section,
                    d.Designation,
                    "Delete",
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Edit(int id)
        {
            var model = new AssignApprovalFlowViewModel();
            try
            {
                var obj = _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.GetByID(id);
                if (obj != null)
                {
                    model = obj.ToModel();
                    var vwObj = _prmCommonservice.PRMUnit.AssignedApprovalFlowViewRepository.Get(q => q.Id == id).FirstOrDefault();
                    model.LevelDetail = vwObj.ZoneCode + " : " + vwObj.OrganogramLevel;
                    PopulateDropDown(model);
                    var empInfo = new PRM_EmploymentInfo();
                    if (model.EmployeeId != null)
                    {
                        empInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(t => t.Id == model.EmployeeId).FirstOrDefault();
                        model.EmpId = empInfo.EmpID;
                        model.EmployeeName = empInfo.FullName;
                        model.Designation = empInfo.PRM_Designation.Name;
                    }
                    model.SelectedDesignationId = model.DesignationId;
                    model.ActionType = "Update";
                }
            }
            catch (Exception exception)
            {
                model.errClass = "failed";
                model.IsSuccessful = false;
                if (exception.InnerException != null && exception.InnerException.Message.Contains("duplicate"))
                {
                    model.Message = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.Message = ErrorMessages.DataNotFound;
                }
            }
            return View("CreateOrEdit", model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            bool result = true;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                var obj = _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.GetByID(id);
                _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.Delete(obj);
                _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        ModelState.AddModelError("Error", errMsg);
                    }
                    else
                    {
                        ModelState.AddModelError("Error", errMsg);
                    }
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        [NoCache]
        public ActionResult GetProcessInfo()
        {
            var list = _prmCommonservice.PRMUnit.ApprovalProcessRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDdlApprovalProcess(list));
        }

        [NoCache]
        public ActionResult GetFlowInfo()
        {
            var list = _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDdlApprovalFlowList(list));
        }

        [NoCache]
        public ActionResult GetDivisionInfo()
        {
            var list = _prmCommonservice.PRMUnit.DivisionRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDllList(list));
        }

        [NoCache]
        public ActionResult GetDepartmentInfo()
        {
            var list = _prmCommonservice.PRMUnit.DisciplineRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDllList(list));
        }

        [NoCache]
        public ActionResult GetSectionInfo()
        {
            var list = _prmCommonservice.PRMUnit.SectionRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDllList(list));
        }

        [NoCache]
        public ActionResult GetDesignationInfo()
        {
            var list = _prmCommonservice.PRMUnit.DesignationRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDllList(list));
        }


        public ActionResult DrawApprovalSteps(AssignApprovalFlowViewModel model, int approvalFlowId, int? assignedApprovalFlowId)
        {
            var approvalStepList = _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.Get(t => t.ApprovalFlowMasterId == approvalFlowId).DefaultIfEmpty().OfType<APV_ApprovalFlowDetail>().ToList();
            int minStepId = 0;
            if (approvalStepList != null && approvalStepList.Count > 0)
            {
                minStepId = approvalStepList.Min(q => q.Id);
            }
            var itemList = new List<ApprovalFlowDrawViewModel>();
            var list = _prmCommonservice.PRMUnit.ApproverInformationViewRepository.Get(t => t.ApprovalMasterId == approvalFlowId).DefaultIfEmpty().OfType<vwAPVApproverInformation>().ToList();
            if (list != null && list.Count > 0)
            {
                list = list.Where(t => t.ApproverType == @"Main").DefaultIfEmpty().OfType<vwAPVApproverInformation>().OrderBy(t => t.ApprovalFlowDetailId).ToList();
            }

            int assignedFlowId = 0;
            int.TryParse(assignedApprovalFlowId.ToString(), out assignedFlowId);

            var employeeWiseApprover = new APV_EmployeeWiseApproverInfo();
            int initialStepId = 0;
            if (assignedFlowId > 0)
            {
                employeeWiseApprover = _prmCommonservice.PRMUnit.EmployeeWiseApproverInfoRepository.GetByID(assignedFlowId);
                if (employeeWiseApprover != null)
                {
                    initialStepId = employeeWiseApprover.InitialStepId != null ? (int)employeeWiseApprover.InitialStepId : 0;
                }
            }

            if (list != null && list.Count > 0)
            {
                var orgLevel = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll();
                var zoneList = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll();
                foreach (var item in list)
                {
                    string approverIdAndName = string.Empty;
                    string zoneName = string.Empty;
                    if (item.LevelId != null)
                    {
                        var lvl = orgLevel.Where(q => q.Id == item.LevelId).FirstOrDefault();
                        if (lvl != null)
                        {
                            zoneName = zoneList.Where(q => q.Id == lvl.ZoneInfoId).FirstOrDefault().ZoneCode;
                        }
                    }
                    switch (item.AuthorType)
                    {
                        case "Specific Employee":
                            approverIdAndName = item.EmployeeName;
                            break;
                        case "Organogram Post":
                            approverIdAndName = string.IsNullOrWhiteSpace(zoneName) ? item.DesignationName : item.DesignationName + " [" + zoneName + " ]";
                            break;
                        case "Head of Level":
                            approverIdAndName = "Head of Level";
                            break;
                    }

                    itemList.Add(new ApprovalFlowDrawViewModel
                    {
                        StepId = item.ApprovalFlowDetailId,
                        ApprovalProcessName = item.ApprovalProcessName,
                        FlowName = item.ApprovalFlowName,
                        StepName = item.StepName,
                        ApproverIdAndName = approverIdAndName,
                        InitialStepId = initialStepId > 0 ? initialStepId : minStepId,
                    });
                }
            }
            model.ApprovalFlowInitializationList = itemList;
            return PartialView("_ApprovalFlowDrawing", model.ApprovalFlowInitializationList);
        }

        public ActionResult EmployeeSearch(string UseTypeEmpId, string zoneId)
        {
            int zoneInfoId = 0;
            if (!string.IsNullOrWhiteSpace(zoneId))
            {
                int.TryParse(zoneId, out zoneInfoId);
            }
            else
            {
                zoneInfoId = LoggedUserZoneInfoId;
            }

            var model = new EmployeeSearchViewModel();
            model.SearchEmpType = "active";
            model.UseTypeEmpId = UseTypeEmpId;
            model.ZoneInfoId = zoneInfoId;
            return View(model);
        }

        #region Search Employee

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, string st, string zoneId)
        {
            int zoneInfoId = 0;
            int.TryParse(zoneId, out zoneInfoId);

            string filterExpression = String.Empty, LoginEmpId = "";
            int totalRecords = 0;
            if (string.IsNullOrEmpty(st))
                st = "";
            if (viewModel.ZoneInfoId == 0 && request.Searching == false)
            {
                viewModel.ZoneInfoId = zoneInfoId == 0 ? LoggedUserZoneInfoId : zoneInfoId;
            }
            else
            {
                viewModel.ZoneInfoId = zoneInfoId;
            }


            var list = _empService.GetPaged(
                filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                //viewModel.ResourceLevelId,
                viewModel.OrganogramLevelId,
                viewModel.ZoneInfoId,
                st.Equals("active") ? 1 : viewModel.EmployeeStatus,

                out totalRecords

                //LoginEmpId
                );

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.EmpName,
                    item.ID,
                    item.EmpId,
                    item.DesigName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,
                    
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        #endregion

        #region Grid Dropdown list

        [NoCache]
        public ActionResult GetZoneInfo()
        {
            var zoneList = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll();
            return PartialView("Select", Common.PopulateDdlZoneList(zoneList));
        }

        public ActionResult GetLoggedZoneId()
        {
            int loggedUserZoneId = LoggedUserZoneInfoId;
            return Json(loggedUserZoneId, JsonRequestBehavior.AllowGet);
        }


        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _empService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.SortingOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _empService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        [NoCache]
        public ActionResult GetJobLocation()
        {
            var jobLocations = _empService.PRMUnit.JobLocationRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            //var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.Id).ToList();
            //return PartialView("Select", Common.PopulateJobGradeDDL(grades));
            return PartialView("Select", Common.PopulateCurrentJobGradeDDL(_empService));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _empService.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(empStatus));
        }

        #endregion

        [HttpPost]
        public JsonResult GetEmployeeInfo(string employeeId)
        {
            int empId = 0;
            int.TryParse(employeeId, out empId);
            var obj = _empService.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpID = obj.EmpID,
                EmployeeName = obj.FullName,
                Designation = obj.PRM_Designation.Name,
                Phone = obj.TelephoneOffice,
                Email = obj.EmialAddress
            });

        }
    }
}