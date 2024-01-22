using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using MyNotificationLib.Operation;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class NotificationFlowSetupController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly EmployeeService _empService;
        #endregion

        #region Constructors

        public NotificationFlowSetupController(PRMCommonSevice prmCommonService, EmployeeService empService)
        {
            this._prmCommonservice = prmCommonService;
            this._empService = empService;
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, NotificationFlowSetupViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from x in _prmCommonservice.PRMUnit.VwNotificationFlowSetupRepository.GetAll()

                        select new NotificationFlowSetupViewModel()
                        {
                            Id = x.Id,

                            NotificationTypeId = x.NotificationTypeId,
                            NotificationTypeName = ((MyNotificationLibEnum.NotificationType)x.NotificationTypeId).ToString().Replace("_", " "),

                            ModuleId = x.ModuleId,
                            ModuleName = ((MyNotificationLibEnum.NotificationModule)x.ModuleId).ToString().Replace("_", " "),

                            EmpId = x.EmpID,
                            EmployeeName = x.EmployeeId != null ? x.FullName + " (" + x.EmpID + ")" : String.Empty,

                            DesignationId = x.DesignationId,
                            Designation = x.DesignationId != null ? x.DesigName : String.Empty,

                            OrganogramLevelId = x.OrganogramLevelId,
                            OrganogramLevelName = x.OrganogramLevelId != null ? (string.IsNullOrEmpty(x.ZoneCode) ? string.Empty : x.ZoneCode + " - ") + x.LevelName : String.Empty,
                            OnlyLevelHead =  x.OnlyLevelHead ?? false,

                            GroupOrIndividual = x.IsApplicableForGroup != null && x.IsApplicableForGroup == true ? "Group" : "Individual",

                            HasReminder = x.HasReminder ?? false,
                            ReminderBeforeDays = Common.GetInteger(x.ReminderBeforeDays)
                        }).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.ModuleName))
                {
                    if (!viewModel.ModuleName.Equals("0"))
                    {
                        list = list.Where(q => q.ModuleId.ToString() == viewModel.ModuleName).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.NotificationTypeName))
                {
                    if (!viewModel.NotificationTypeName.Equals("0"))
                    {
                        list = list.Where(q => q.NotificationTypeId.ToString() == viewModel.NotificationTypeName).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.GroupOrIndividual))
                {
                    if (!viewModel.GroupOrIndividual.Equals("0"))
                    {
                        list = list.Where(q => q.GroupOrIndividual.ToString() == viewModel.GroupOrIndividual).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(viewModel.EmployeeName))
                {
                    if (!string.IsNullOrWhiteSpace(viewModel.EmployeeName))
                    {
                        list = list.Where(q => q.EmployeeName != null && q.EmployeeName.Contains(viewModel.EmployeeName)).ToList();
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

            if (request.SortingName == "NotificationTypeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.NotificationTypeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.NotificationTypeName).ToList();
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
                    d.NotificationTypeName,
                    d.ModuleName,
                    d.GroupOrIndividual,
                    d.OrganogramLevelName,
                    d.OnlyLevelHead,
                    d.EmployeeName,
                    d.Designation,
                    d.HasReminder,
                    d.ReminderBeforeDays,
                    "Delete",
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }


        public ActionResult Create()
        {
            NotificationFlowSetupViewModel model = new NotificationFlowSetupViewModel();
            PopulateDropDown(model);
            model.ActionType = "Save";

            return View("CreateOrEdit", model);
        }

        public ActionResult Save(NotificationFlowSetupViewModel model)
        {
            model.ActionType = "Save";
            model.IsError = 1;
            model.errClass = "failed";

            if (ModelState.IsValid)
            {
                try
                {
                    string errorMessage = CheckBusinessValidation(model);

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        foreach (int moduleId in model.SelectedModuleIds)
                        {
                            if (moduleId == 0) continue;

                            model.ModuleId = moduleId;

                            var obj = model.ToEntity();
                            if (!Common.GetBoolean(obj.IsApplicableForGroup))
                            {
                                var employee = _prmCommonservice.PRMUnit.EmploymentInfoRepository
                                    .Get(t => t.EmpID == model.EmpId).FirstOrDefault();
                                obj.OrganogramLevelId = null;
                                obj.DesignationId = employee.DesignationId;
                                obj.EmployeeId = employee.Id;
                            }
                            else
                            {
                                obj.EmployeeId = null;
                            }

                            _prmCommonservice.PRMUnit.NotificationFlowSetupRepository.Add(obj);
                            _prmCommonservice.PRMUnit.NotificationFlowSetupRepository.SaveChanges();
                        }

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = model.Message = Resources.ErrorMessages.InsertSuccessful;
                    }
                    else
                    {
                        model.ErrMsg = model.Message = errorMessage;
                    }
                }
                catch (Exception ex)
                {
                    model.ErrMsg = model.Message = Resources.ErrorMessages.InsertFailed;
                }
            }

            if (model.IsError == 1)
            {
                PopulateDropDown(model);
                return View("CreateOrEdit", model);
            }


            return View("Index", model);
        }

        public ActionResult Edit(int id)
        {
            var model = new NotificationFlowSetupViewModel();

            try
            {
                var obj = _prmCommonservice.PRMUnit.NotificationFlowSetupRepository.GetByID(id);

                if (obj != null)
                {
                    model = obj.ToModel();

                    var levelDetail =
                        _prmCommonservice.PRMUnit.VwOrganogramZoneDetailRepository.GetAll().FirstOrDefault(o => o.OrganogramLevelId == Common.GetInteger(model.OrganogramLevelId));
                    if (levelDetail != null)
                    {
                        model.LevelDetail = levelDetail.ZoneCode + " : " + levelDetail.LevelName;
                    }

                    model.NotificationTypeIdWhenEdit = model.NotificationTypeId;
                    model.NotificationTypeName = ((MyNotificationLibEnum.NotificationType)model.NotificationTypeId).ToString().Replace("_", " ");

                    model.ModuleIdWhenEdit = model.ModuleId;
                    model.ModuleName = ((MyNotificationLibEnum.NotificationModule)model.ModuleId).ToString().Replace("_", " ");

                    PopulateDropDown(model);

                    model.SelectedDesignationId = model.DesignationId;

                    if (model.EmployeeId != null)
                    {
                        var empInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(t => t.Id == model.EmployeeId).FirstOrDefault();
                        model.EmpId = empInfo.EmpID;
                        model.EmployeeName = empInfo.FullName;
                        model.Designation = empInfo.PRM_Designation.Name;
                        model.SelectedDesignationId = empInfo.DesignationId;
                    }

                    model.ActionType = "Update";
                }
            }
            catch (Exception exception)
            {
                model.errClass = "failed";
                model.IsSuccessful = false;
                if (exception.InnerException != null && exception.InnerException.Message.Contains("duplicate"))
                {
                    model.errClass = model.Message = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.errClass = model.Message = ErrorMessages.ExceptionMessage;
                }

                return View("Index", model);
            }
            return View("CreateOrEdit", model);
        }

        public ActionResult Update(NotificationFlowSetupViewModel model)
        {
            model.IsError = 1;
            model.errClass = "failed";
            model.ActionType = "Update";

            ModelState.Remove("SelectedModuleIds");
            model.ModuleId = Common.GetInteger(model.ModuleIdWhenEdit);
            model.NotificationTypeId = Common.GetInteger(model.NotificationTypeIdWhenEdit);

            if (ModelState.IsValid)
            {
                var obj = model.ToEntity();
                try
                {
                    string errorMessage = CheckBusinessValidation(model);

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        if (!Common.GetBoolean(obj.IsApplicableForGroup))
                        {
                            var employee = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(t => t.EmpID == model.EmpId).FirstOrDefault();
                            obj.OrganogramLevelId = null;
                            obj.DesignationId = employee.DesignationId;
                            obj.EmployeeId = employee.Id;
                        }
                        else
                        {
                            obj.EmployeeId = null;
                        }

                        _prmCommonservice.PRMUnit.NotificationFlowSetupRepository.Update(obj);
                        _prmCommonservice.PRMUnit.NotificationFlowSetupRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = model.Message = Resources.ErrorMessages.InsertSuccessful;
                    }
                    else
                    {
                        model.ErrMsg = model.Message = errorMessage;

                    }
                }
                catch (Exception)
                {
                    model.ErrMsg = model.Message = Resources.ErrorMessages.UpdateFailed;
                }
            }


            if (model.IsError == 1)
            {
                PopulateDropDown(model);
                return View("CreateOrEdit", model);
            }


            return View("Index", model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            bool result = true;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                var obj = _prmCommonservice.PRMUnit.NotificationFlowSetupRepository.GetByID(id);

                _prmCommonservice.PRMUnit.NotificationFlowSetupRepository.Delete(obj);
                _prmCommonservice.PRMUnit.NotificationFlowSetupRepository.SaveChanges();
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

        private void PopulateDropDown(NotificationFlowSetupViewModel model)
        {
            var zoneList = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll();
            model.ZoneList = Common.PopulateDdlZoneListWithAllOption(zoneList);

            model.NotificationTypeList = Common.PopulateDdlFromEnum<MyNotificationLibEnum.NotificationType>();

            model.ModuleList = Common.PopulateDdlFromEnum<MyNotificationLibEnum.NotificationModule>();

            model.DesignationList = new List<SelectListItem>();
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

        private string CheckBusinessValidation(NotificationFlowSetupViewModel model)
        {

            switch (model.ActionType)
            {
                case "Save":

                    if (model.SelectedModuleIds.Length == 0)
                    {
                        return "Please select atleast one module.";
                    }

                    if (Common.GetInteger(model.NotificationTypeId) == 0)
                    {
                        return "Please select a notification type.";
                    }
                    break;
            }

            if (!model.IsApplicableForGroup)
            {
                if (string.IsNullOrEmpty(model.EmpId))
                {
                    return "Please select an employee.";
                }
            }
            else
            {
                if (Common.GetInteger(model.OrganogramLevelId) == 0)
                {
                    return "Please select an organogram.";
                }
            }

            // Check duplicate entry
            bool isDuplicateDataFound = false;
            string moduleName = string.Empty;
            string organogramName = string.Empty;
            string designationName = string.Empty;
            string empName = string.Empty;
            string returnString = string.Empty;

            if (model.SelectedModuleIds == null)
                model.SelectedModuleIds = new[] { model.ModuleId };

            foreach (int moduleId in model.SelectedModuleIds)
            {
                if (moduleId == 0) continue;

                moduleName = ((MyNotificationLibEnum.NotificationModule)moduleId).ToString().Replace("_", " ");

                var list = _prmCommonservice.PRMUnit
                    .NotificationFlowSetupRepository
                    .Get(t => t.NotificationTypeId == model.NotificationTypeId && t.ModuleId == moduleId)
                    .DefaultIfEmpty()
                    .OfType<NTF_NotificationFlowSetup>().ToList();

                var emp = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(t => t.EmpID == model.EmpId)
                    .FirstOrDefault();

                switch (model.ActionType)
                {
                    case "Save":
                        if (model.IsApplicableForGroup)
                        {
                            list = list.Where(t => t.DesignationId == model.DesignationId &&
                                                   t.OrganogramLevelId == model.OrganogramLevelId).ToList();
                        }
                        else
                        {
                            list = list.Where(t => t.EmployeeId == emp.Id).ToList();
                        }
                        break;
                    case "Update":
                        if (model.IsApplicableForGroup)
                        {
                            list = list.Where(t => t.DesignationId == model.DesignationId &&
                                           t.OrganogramLevelId == model.OrganogramLevelId && t.Id != model.Id).ToList();

                        }
                        else
                        {
                            list = list.Where(t => t.EmployeeId == emp.Id && t.Id != model.Id).ToList();
                        }

                        break;
                }

                if (list != null && list.Count > 0)
                {
                    isDuplicateDataFound = true;

                    var notification = _prmCommonservice.PRMUnit.VwNotificationFlowSetupRepository.GetAll().FirstOrDefault(v => v.Id == list.FirstOrDefault().Id);
                    if (!model.IsApplicableForGroup)
                    {
                        if (notification != null)
                        {
                            empName = notification.FullName + " (" + notification.EmpID + ")";
                        }

                        returnString = "Duplicate data found for Module: " + moduleName + " and Employee: " + empName;
                    }
                    else
                    {
                        if (list.Any())
                        {
                            if (notification != null)
                            {
                                organogramName = notification.LevelName;
                                designationName = notification.DesigName;
                            }
                        }

                        returnString = "Duplicate data found for Module: " + moduleName + " and Organogram-Level: " + organogramName + " and Designation: " + designationName;
                    }
                }
            }

            if (isDuplicateDataFound)
            {
                return returnString;
            }


            // Has Reminder
            if (model.HasReminder)
            {
                if (Common.GetInteger(model.ReminderBeforeDays) <= 0)
                {
                    return "Reminder before days - must be greater than 0.";
                }
            }


            return string.Empty;
        }


        [NoCache]
        public ActionResult GetNotificationTypeList()
        {
            return PartialView("Select", Common.PopulateDdlFromEnum<MyNotificationLibEnum.NotificationType>());
        }

        [NoCache]
        public ActionResult GetModuleList()
        {
            return PartialView("Select", Common.PopulateDdlFromEnum<MyNotificationLibEnum.NotificationModule>());
        }

        [NoCache]
        public ActionResult GetNotificationReceiverTypeList()
        {

            IList<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Group", Value = "Group" });
            list.Add(new SelectListItem() { Text = "Individual", Value = "Individual" });

            return PartialView("_Select", list.ToList());
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

        #region Grid Dropdown list

        [NoCache]
        public ActionResult GetZoneInfo()
        {
            var zoneList = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll();
            return PartialView("Select", Common.PopulateDdlZoneList(zoneList));
        }

        [NoCache]
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
        public ActionResult GetEmployeeStatus()
        {
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(empStatus));
        }

        #endregion


    }
}