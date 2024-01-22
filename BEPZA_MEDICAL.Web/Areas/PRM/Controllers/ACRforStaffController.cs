using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforStaff;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ACRforStaffController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly EmployeeService _empService;
        #endregion

        #region Constructor

        public ACRforStaffController(PRMCommonSevice prmCommonService, EmployeeService empService)
        {
            this._prmCommonService = prmCommonService;
            this._empService = empService;
        }
        #endregion

        //
        // GET: /PRM/ACRforStaff/
        public ActionResult Index()
        {
            var model = new ACRforStaffViewModel();
            model.ActionName = "StaffInfoIndex";

            return View(model);
        }

        #region Search
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ACRforStaffViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ACRforStaffViewModel> list = (from acrOff in _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetAll()
                                               join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on acrOff.EmployeeId equals emp.Id
                                               join des in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals des.Id
                                               join dep in _prmCommonService.PRMUnit.DivisionRepository.GetAll() on emp.DivisionId equals dep.Id
                                               where (model.DepartmentId == 0 || model.DepartmentId == dep.Id)
                                               && (model.DesignationId == 0 || model.DesignationId == des.Id)
                                               && (model.EmpId == "" || model.EmpId == null || model.EmpId == emp.EmpID)
                                               && (string.IsNullOrEmpty(model.Name) || emp.FullName.Contains(model.Name))
                                               && (LoggedUserZoneInfoId == acrOff.ZoneInfoId)
                                               && (emp.EmpID != User.Identity.Name)
                                               select new ACRforStaffViewModel()
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

        #region Staff's Basic Info

        #region Insert
        public ActionResult StaffInfoIndex(int? id)
        {
            if (id.HasValue)
                return RedirectToAction("EditStaffInfo", "ACRforStaff", new { id = id });

            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "StaffInfo";
            var approvalProcessName = @"ACR";
            var approverInfo = _prmCommonService.PRMUnit.FunctionRepository.GetApproverByApplicant(string.Empty, approvalProcessName).DefaultIfEmpty().OfType<APV_GetApproverInfoByApplicant_Result>().ToList();


            parentModel.StaffInfo.ApproverList = Common.PopulateEmployeeDDL(approverInfo);
            parentModel.StaffInfo.ActionType = "CreateStaffInfo";
            parentModel.StaffInfo.ButtonText = "Save";
            parentModel.StaffInfo.SelectedClass = "selected";

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateStaffInfo(StaffInfoViewModel model)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "StaffInfo";
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

                    _prmCommonService.PRMUnit.ACRStaffInfoRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRStaffInfoRepository.SaveChanges();

                    parentModel.Id = entity.Id;
                    parentModel.EmpId = entity.EmployeeId.ToString();
                    const int isOnlineApplication = 0;
                    _prmCommonService.PRMUnit.FunctionRepository.InitializeApprovalProcess("ACR", model.EmpId, entity.Id, isOnlineApplication, model.NextApproverId, entity.IUser);

                    //var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
                    //var applicationId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApplicationId == entity.Id && q.ApprovalProcessId == processId && q.IsOnlineApplication == false).OrderByDescending(q => q.Id).Take(1).FirstOrDefault().Id;

                    //string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                    //var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId, actionName, string.Empty, model.ApproverId, entity.IUser);

                }
                catch (Exception ex)
                {
                    parentModel.StaffInfo = model;
                    parentModel.StaffInfo.ButtonText = "Save";
                    parentModel.StaffInfo.SelectedClass = "selected";
                    parentModel.StaffInfo.ErrorClass = "failed";
                    parentModel.StaffInfo.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.StaffInfo.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.StaffInfo = model;
                parentModel.StaffInfo.ButtonText = "Save";
                parentModel.StaffInfo.SelectedClass = "selected";
                parentModel.StaffInfo.ErrorClass = "failed";
                parentModel.StaffInfo.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.StaffInfo.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditStaffInfo", "ACRforStaff", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditStaffInfo(int id, string type)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "StaffInfo";

            var entity = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(id);
            var model = entity.ToModel();


            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;

            model.Department = entity.PRM_EmploymentInfo.DivisionId != null ? entity.PRM_EmploymentInfo.PRM_Division.Name : String.Empty;
            //model.Department = entity.PRM_EmploymentInfo.PRM_Division.Name;

            model.Designation = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            model.SeniorityNumber = entity.PRM_EmploymentInfo.SeniorityPosition;

            model.ActionType = "EditStaffInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.StaffInfo = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.Id;

            if (type == "success")
            {
                parentModel.StaffInfo.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.StaffInfo.ErrorClass = "success";
                parentModel.StaffInfo.IsError = 0;
            }

            var approver = GetApprover(model.EmpId);

            model.ApproverList = Common.PopulateEmployeeDDL(approver);

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditStaffInfo(StaffInfoViewModel model)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "StaffInfo";
            var error = string.Empty;

            if (ModelState.IsValid)
            {
                model.ZoneInfoId = LoggedUserZoneInfoId;
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;

                    _prmCommonService.PRMUnit.ACRStaffInfoRepository.Update(entity);
                    _prmCommonService.PRMUnit.ACRStaffInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";

                    parentModel.StaffInfo = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.StaffInfo = model;
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

            parentModel.StaffInfo = model;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete------------------------------
        public ActionResult Delete(int id)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "StaffInfo";

            var entity = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(id);
            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var entityBiodata = _prmCommonService.PRMUnit.ACRStaffBioDataRepository.GetByID(id, "StaffInfoId");
                    _prmCommonService.PRMUnit.ACRStaffBioDataRepository.Delete(entityBiodata);
                    _prmCommonService.PRMUnit.ACRStaffBioDataRepository.SaveChanges();

                    _prmCommonService.PRMUnit.ACRStaffInfoRepository.Delete(id);
                    _prmCommonService.PRMUnit.ACRStaffInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";
                    model.ApproverList = new List<SelectListItem>();

                    parentModel.StaffInfo = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;
            model.ApproverList = new List<SelectListItem>();

            parentModel.StaffInfo = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Staff Bio Data

        #region Insert
        public ActionResult StaffBioDataIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.ACRStaffBioDataRepository.GetByID(id, "StaffInfoId");
                if (en != null)
                    return RedirectToAction("EditBioData", "ACRforStaff", new { id = id });
            }
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "BioData";

            var entity = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(id);

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);

            if (entity.NextApproverId != loginUser.ID)
            {
                return RedirectToAction("EditStaffInfo", "ACRforStaff", new { id = id, type = string.Empty });
            }


            var model = parentModel.BioData;
            parentModel.Id = entity.Id;

            #region Only View Part

            model.EmployeeId = entity.EmployeeId;
            model.ACRDate = ((DateTime)entity.ACRDate).ToString("yyyy-MM-dd");
            model.ACRPeriodFrom = ((DateTime)entity.ACRPeriodFrom).ToString("yyyy-MM-dd");
            model.ACRPeriodTo = ((DateTime)entity.ACRPeriodTo).ToString("yyyy-MM-dd");

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            model.EmployeeDepartment = empInfo.DivisionId != null ? empInfo.PRM_Division.Name : String.Empty;
            //model.EmployeeDepartment = empInfo.PRM_Division.Name;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            model.DateofBirth = ((DateTime)empInfo.DateofBirth).ToString("yyyy-MM-dd");
            model.DateOfJoinIng = empInfo.DateofJoining.ToString("yyyy-MM-dd");
            model.DateOfJoiningInCurrentPost = empInfo.DateofPosition.ToString("yyyy-MM-dd");
            model.SalaryScaleName = empInfo.PRM_JobGrade.PRM_SalaryScale.SalaryScaleName;
            model.CurrentBasicSalary = _empService.GetEmployeeBasicSalary(entity.EmployeeId);
            model.PassInDepExam = true;
            model.EmploymentType = empInfo.PRM_EmploymentType.Name;
            model.FatherName = empInfo.PRM_EmpPersonalInfo == null ? null : empInfo.PRM_EmpPersonalInfo.FatherName;

            #region Education Qualification
            var education = _prmCommonService.PRMUnit.AccademicQualificationRepository.GetAll().Where(x => x.EmployeeId == entity.EmployeeId).ToList();
            List<StaffBioDataViewModel> EducationList = new List<StaffBioDataViewModel>();
            foreach (var item in education)
            {
                var gridModel = new StaffBioDataViewModel
                {
                    ExaminationName = item.PRM_DegreeLevel.Name,
                    AcademicInstitution = item.InstituteName,
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
            List<StaffBioDataViewModel> TrainingList = new List<StaffBioDataViewModel>();
            foreach (var item in training)
            {
                var gridModel1 = new StaffBioDataViewModel
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
            List<StaffBioDataViewModel> LanguageList = new List<StaffBioDataViewModel>();
            foreach (var item in language)
            {
                var gridModel2 = new StaffBioDataViewModel
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
            model.StaffInfoId = entity.Id;
            int processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            if (id.HasValue)
            {
                var nextApproverList = GetNextApprover((int)id);
                model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateBioData(StaffBioDataViewModel model)
        {
            var parentModel = new ACRforStaffViewModel();
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

                    _prmCommonService.PRMUnit.ACRStaffBioDataRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRStaffBioDataRepository.SaveChanges();

                    parentModel.Id = entity.StaffInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();

                    var obj = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(entity.StaffInfoId);
                    var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
                    var applicationId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApplicationId == entity.StaffInfoId && q.ApprovalProcessId == processId && q.IsOnlineApplication == false && q.ApproverId == obj.NextApproverId).LastOrDefault().Id;
                    string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                    var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId, actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
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

            return RedirectToAction("EditBioData", "ACRforStaff", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditBioData(int id, string type)
        {


            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "BioData";

            var entity = _prmCommonService.PRMUnit.ACRStaffBioDataRepository.GetAll().Where(x => x.StaffInfoId == id).FirstOrDefault();

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var staffInfo = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(id);
            if (staffInfo.NextApproverId != loginUser.ID)
            {
                return RedirectToAction("EditStaffInfo", "ACRforStaff", new { id = id, type = string.Empty });
            }



            var model = entity.ToModel();

            #region Only View Part

            var officerInfo = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(model.StaffInfoId);
            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString("yyyy-MM-dd");
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString("yyyy-MM-dd");
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString("yyyy-MM-dd");

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = empInfo.FullName;
            
            model.EmployeeDepartment = empInfo.DivisionId != null ? empInfo.PRM_Division.Name : String.Empty;
            //model.EmployeeDepartment = empInfo.PRM_Division.Name;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            model.DateofBirth = ((DateTime)empInfo.DateofBirth).ToString("yyyy-MM-dd");
            model.DateOfJoinIng = empInfo.DateofJoining.ToString("yyyy-MM-dd");
            model.DateOfJoiningInCurrentPost = empInfo.DateofPosition.ToString("yyyy-MM-dd");
            model.SalaryScaleName = empInfo.PRM_JobGrade.PRM_SalaryScale.SalaryScaleName;
            model.FatherName = empInfo.PRM_EmpPersonalInfo == null ? null : empInfo.PRM_EmpPersonalInfo.FatherName;
            model.EmploymentType = empInfo.PRM_EmploymentType.Name;

            #region Education Qualification
            var education = _prmCommonService.PRMUnit.AccademicQualificationRepository.GetAll().Where(x => x.EmployeeId == entity.EmployeeId).ToList();
            List<StaffBioDataViewModel> EducationList = new List<StaffBioDataViewModel>();
            foreach (var item in education)
            {
                var gridModel = new StaffBioDataViewModel
                {
                    ExaminationName = item.PRM_DegreeLevel.Name,
                    AcademicInstitution = item.InstituteName,
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
            List<StaffBioDataViewModel> TrainingList = new List<StaffBioDataViewModel>();
            foreach (var item in training)
            {
                var gridModel1 = new StaffBioDataViewModel
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
            List<StaffBioDataViewModel> LanguageList = new List<StaffBioDataViewModel>();
            foreach (var item in language)
            {
                var gridModel2 = new StaffBioDataViewModel
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

            model.ActionType = "EditBioData";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.BioData = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.StaffInfoId;

            if (type == "success")
            {
                parentModel.BioData.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.BioData.ErrorClass = "success";
                parentModel.BioData.IsError = 0;
            }

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var approvalStepId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == processId && q.ApplicationId == id && q.ApproverId == staffInfo.NextApproverId).FirstOrDefault().ApprovalStepId;
            var nextApproverList = GetNextApprover((int)id, approvalStepId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);


            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditBioData(StaffBioDataViewModel model)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "BioData";
            var error = string.Empty;

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var officerInfo = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(model.StaffInfoId);
            var approvalStepId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == processId && q.ApplicationId == model.StaffInfoId && q.ApproverId == officerInfo.NextApproverId).LastOrDefault().ApprovalStepId;
            var nextApproverList = GetNextApprover(model.StaffInfoId, approvalStepId);

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.ACRStaffBioDataRepository.Update(entity, "StaffInfoId");
                    _prmCommonService.PRMUnit.ACRStaffBioDataRepository.SaveChanges();

                    var applicationId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApplicationId == entity.StaffInfoId && q.ApprovalProcessId == processId && q.IsOnlineApplication == false && q.ApproverId == officerInfo.NextApproverId).LastOrDefault().Id;
                    string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                    var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId, actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
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
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";
                model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.BioData = model;
                parentModel.Id = model.StaffInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();
                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.BioData = model;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete------------------------------
        public ActionResult DeleteBioData(int id)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "BioData";

            var entityBiodata = _prmCommonService.PRMUnit.ACRStaffBioDataRepository.GetByID(id, "StaffInfoId");
            var model = entityBiodata.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _prmCommonService.PRMUnit.ACRStaffBioDataRepository.Delete(entityBiodata);
                    _prmCommonService.PRMUnit.ACRStaffBioDataRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";

                    parentModel.BioData = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entityBiodata.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.BioData = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Assessment Information

        #region Insert
        public ActionResult AssessmentInfoIndex(int? id)
        {
            if (id.HasValue)
            {
                var en = _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.GetByID(id, "StaffInfoId");
                if (en != null)
                    return RedirectToAction("EditAssessmentInfo", "ACRforStaff", new { id = id });
            }
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "AssessmentInfo";

            var entity = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(id);

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var biodataInfo = _prmCommonService.PRMUnit.ACRStaffBioDataRepository.GetByID(id, "StaffInfoId");
            var assesmentInfo = _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.GetByID(id, "StaffInfoId");

            if (biodataInfo == null || biodataInfo.NextApproverId != loginUser.ID || (assesmentInfo != null && assesmentInfo.NextApproverId != loginUser.ID))
            {
                return RedirectToAction("EditStaffInfo", "ACRforStaff", new { id = id, type = string.Empty });
            }

            var model = parentModel.AssessmentInfo;
            parentModel.Id = entity.Id;

            #region Attributes

            var criteria = _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.GetAll().Where(x => x.ACRCriteriaName.Contains("Assessment Information")).FirstOrDefault();
            var attributes = (from acrAtt in _prmCommonService.PRMUnit.ACRAttributesInformationRepository.GetAll()
                              join acrAttDtl in _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.GetAll() on acrAtt.Id equals acrAttDtl.ACRAttributesInfoId
                              where (acrAtt.ACRCriteriaInfoId == criteria.Id)
                              select new StaffAssessmentInfoDetailViewModel()
                              {
                                  AttributeDetailId = acrAttDtl.Id,
                                  AttributeName = acrAttDtl.AttributeName,
                                  SerialNumber = acrAttDtl.SerialNumber
                              }).ToList();

            model.AttributeDetailList = attributes;

            #endregion

            #region Only View Part

            model.EmployeeId = entity.EmployeeId;
            model.ACRDate = ((DateTime)entity.ACRDate).ToString("yyyy-MM-dd");
            model.ACRPeriodFrom = ((DateTime)entity.ACRPeriodFrom).ToString("yyyy-MM-dd");
            model.ACRPeriodTo = ((DateTime)entity.ACRPeriodTo).ToString("yyyy-MM-dd");

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            
            model.EmployeeDepartment = empInfo.DivisionId != null ? empInfo.PRM_Division.Name : String.Empty;
            //model.EmployeeDepartment = empInfo.PRM_Division.Name;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;
            #endregion

            model.A = true;

            model.ActionType = "CreateAssessmentInfo";
            model.ButtonText = "Save";
            model.SelectedClass = "selected";
            model.StaffInfoId = entity.Id;

            var nextApproverList = GetNextApprover(model.StaffInfoId);

            var approverActionType = (from x in nextApproverList
                                      select x.StepTypeName).Take(1).LastOrDefault();

            model.ApprovalActionName = approverActionType;
            if (approverActionType.Contains("Recommend"))
            {
                model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            }
            model.ReportingOfficerId = loginUser.ID;
            model.ApproverId = loginUser.ID;
            model.NameOfReportingOfficer = loginUser.EmpName;
            model.Designation = loginUser.DesignationName;
            model.Department = loginUser.Department;

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateAssessmentInfo(StaffAssessmentInfoViewModel model)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "AssessmentInfo";
            var error = string.Empty;
            model = GetReadyModel(model);

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = CreateEntity(model, true);
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.ReportingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;

                    _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.SaveChanges();

                    parentModel.Id = entity.StaffInfoId;
                    parentModel.EmpId = entity.EmployeeId.ToString();

                    var biodataInfo = _prmCommonService.PRMUnit.ACRStaffBioDataRepository.GetByID(model.StaffInfoId, "StaffInfoId");

                    var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
                    var applicationId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApplicationId == entity.StaffInfoId && q.ApprovalProcessId == processId && q.IsOnlineApplication == false && q.ApproverId == biodataInfo.NextApproverId).FirstOrDefault().Id;
                    string actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                    var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId, actionName, model.ApproverComments, model.NextApproverId, entity.IUser);
                }
                catch (Exception ex)
                {
                    parentModel.AssessmentInfo = model;
                    parentModel.AssessmentInfo.ButtonText = "Save";
                    parentModel.AssessmentInfo.SelectedClass = "selected";
                    parentModel.AssessmentInfo.ErrorClass = "failed";
                    parentModel.AssessmentInfo.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    parentModel.AssessmentInfo.IsError = 1;
                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                parentModel.AssessmentInfo = model;
                parentModel.AssessmentInfo.ButtonText = "Save";
                parentModel.AssessmentInfo.SelectedClass = "selected";
                parentModel.AssessmentInfo.ErrorClass = "failed";
                parentModel.AssessmentInfo.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.AssessmentInfo.IsError = 1;
                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditAssessmentInfo", "ACRforStaff", new { id = parentModel.Id, type = "success" });
        }

        public StaffAssessmentInfoViewModel GetReadyModel(StaffAssessmentInfoViewModel model)
        {
            int five, four, three, two, one;
            if (model.AttributeDetailList.Count > 0)
            {
                five = model.AttributeDetailList.Count(x => x.ChkFive == true);
                four = model.AttributeDetailList.Count(x => x.ChkFour == true);
                three = model.AttributeDetailList.Count(x => x.ChkThree == true);
                two = model.AttributeDetailList.Count(x => x.CkhTwo == true);
                one = model.AttributeDetailList.Count(x => x.ChkOne == true);
                if (five >= four && five > three && five > two && five > one)
                    model.OverAllAssessment = "Excellent";
                else if (four > five && four >= three && four > two && four > one)
                    model.OverAllAssessment = "Good";
                else if (three > four && three > five && three >= two && three > one)
                    model.OverAllAssessment = "Standard";
                else if (two > four && two > three && two > five && two >= one)
                    model.OverAllAssessment = "BelowStnd";
                else if (one > four && one > three && one > two && one > five)
                    model.OverAllAssessment = "NotExpt";
                else
                    model.OverAllAssessment = "Excellent";
            }

            if (model.A == true)
            {
                model.QualificationForPromotion = "EligiblePro";
            }
            else if (model.B == true)
            {
                model.QualificationForPromotion = "NotEligiblePro";
            }
            else if (model.C == true)
            {
                model.QualificationForPromotion = "HighlyRecomPro";
            }
            else
            {
                model.QualificationForPromotion = "RecentlyPro";
            }

            return model;
        }

        private PRM_EmpACRAssessmentInfo CreateEntity(StaffAssessmentInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            foreach (var c in model.AttributeDetailList)
            {
                var prm_EmpACRAssessmentInfoDetail = new PRM_EmpACRAssessmentInfoDetail();

                prm_EmpACRAssessmentInfoDetail.Id = c.Id;
                prm_EmpACRAssessmentInfoDetail.AttributeDetailId = c.AttributeDetailId;
                prm_EmpACRAssessmentInfoDetail.StaffInfoId = model.StaffInfoId;

                if (c.ChkFive)
                    prm_EmpACRAssessmentInfoDetail.Mark = 5;
                else if (c.ChkFour)
                    prm_EmpACRAssessmentInfoDetail.Mark = 4;
                else if (c.ChkThree)
                    prm_EmpACRAssessmentInfoDetail.Mark = 3;
                else if (c.CkhTwo)
                    prm_EmpACRAssessmentInfoDetail.Mark = 2;
                else if (c.ChkOne)
                    prm_EmpACRAssessmentInfoDetail.Mark = 1;
                prm_EmpACRAssessmentInfoDetail.Remarks = c.Remarks;
                prm_EmpACRAssessmentInfoDetail.IUser = User.Identity.Name;
                prm_EmpACRAssessmentInfoDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    prm_EmpACRAssessmentInfoDetail.IUser = User.Identity.Name;
                    prm_EmpACRAssessmentInfoDetail.IDate = DateTime.Now;
                    entity.PRM_EmpACRAssessmentInfoDetail.Add(prm_EmpACRAssessmentInfoDetail);
                }
                else
                {
                    prm_EmpACRAssessmentInfoDetail.StaffInfoId = model.StaffInfoId;
                    prm_EmpACRAssessmentInfoDetail.EUser = User.Identity.Name;
                    prm_EmpACRAssessmentInfoDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.ACRAssessmentInfoDetailRepository.Add(prm_EmpACRAssessmentInfoDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.ACRAssessmentInfoDetailRepository.Update(prm_EmpACRAssessmentInfoDetail);

                    }
                }
                _prmCommonService.PRMUnit.ACRPersonalCharacteristicsDetailRepository.SaveChanges();

            }

            return entity;
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditAssessmentInfo(int id, string type)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "AssessmentInfo";

            var entity = _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.GetAll().Where(x => x.StaffInfoId == id).FirstOrDefault();
            var model = entity.ToModel();

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            var biodataInfo = _prmCommonService.PRMUnit.ACRStaffBioDataRepository.GetByID(id, "StaffInfoId");
            var assesmentInfo = _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.GetByID(id, "StaffInfoId");

            if (biodataInfo.NextApproverId != loginUser.ID && (assesmentInfo != null && assesmentInfo.NextApproverId != loginUser.ID))
            {
                return RedirectToAction("EditStaffInfo", "ACRforStaff", new { id = id, type = string.Empty });
            }

            if (loginUser.ID == biodataInfo.NextApproverId)
            {
                model.ApprovalActionName = "Recommendation";
                model.ReportingOfficerId = loginUser.ID;
            }
            else
            {
                model.ApprovalActionName = "Approval";
                model.ReviewingOfficerId = loginUser.ID;
            }

            #region Attributes
            List<StaffAssessmentInfoDetailViewModel> upList = new List<StaffAssessmentInfoDetailViewModel>();
            var attributes = (from acrAtt in _prmCommonService.PRMUnit.ACRAssessmentInfoDetailRepository.GetAll()
                              join acrAttDtl in _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.GetAll() on acrAtt.AttributeDetailId equals acrAttDtl.Id
                              where (acrAtt.StaffInfoId == id)
                              select new StaffAssessmentInfoDetailViewModel()
                              {
                                  Id = acrAtt.Id,
                                  StaffInfoId = acrAtt.StaffInfoId,
                                  AttributeDetailId = acrAttDtl.Id,
                                  AttributeName = acrAttDtl.AttributeName,
                                  SerialNumber = acrAttDtl.SerialNumber,
                                  Mark = acrAtt.Mark,
                                  Remarks = acrAtt.Remarks
                              }).ToList();

            foreach (var item in attributes)
            {
                if (item.Mark == 5)
                {
                    var att = new StaffAssessmentInfoDetailViewModel()
                    {
                        Id = item.Id,
                        StaffInfoId = item.StaffInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        Remarks = item.Remarks,
                        ChkFive = true
                    };
                    upList.Add(att);
                }
                else if (item.Mark == 4)
                {
                    var att = new StaffAssessmentInfoDetailViewModel()
                    {
                        Id = item.Id,
                        StaffInfoId = item.StaffInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        Remarks = item.Remarks,
                        ChkFour = true
                    };
                    upList.Add(att);
                }

                else if (item.Mark == 3)
                {
                    var att = new StaffAssessmentInfoDetailViewModel()
                    {
                        Id = item.Id,
                        StaffInfoId = item.StaffInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        Remarks = item.Remarks,
                        ChkThree = true
                    };
                    upList.Add(att);
                }
                else if (item.Mark == 2)
                {
                    var att = new StaffAssessmentInfoDetailViewModel()
                    {
                        Id = item.Id,
                        StaffInfoId = item.StaffInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        Remarks = item.Remarks,
                        CkhTwo = true
                    };
                    upList.Add(att);
                }
                else
                {
                    var att = new StaffAssessmentInfoDetailViewModel()
                    {
                        Id = item.Id,
                        StaffInfoId = item.StaffInfoId,
                        AttributeDetailId = item.AttributeDetailId,
                        AttributeName = item.AttributeName,
                        SerialNumber = item.SerialNumber,
                        Mark = item.Mark,
                        Remarks = item.Remarks,
                        ChkOne = true
                    };
                    upList.Add(att);
                }

            }
            model.AttributeDetailList = upList;

            #endregion

            #region Only View Part

            var officerInfo = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(model.StaffInfoId);
            model.ACRDate = ((DateTime)officerInfo.ACRDate).ToString("yyyy-MM-dd");
            model.ACRPeriodFrom = ((DateTime)officerInfo.ACRPeriodFrom).ToString("yyyy-MM-dd");
            model.ACRPeriodTo = ((DateTime)officerInfo.ACRPeriodTo).ToString("yyyy-MM-dd");

            var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = empInfo.FullName;

            
            model.EmployeeDepartment = empInfo.DivisionId != null ? empInfo.PRM_Division.Name : String.Empty;
            //model.EmployeeDepartment = empInfo.PRM_Division.Name;

            model.EmployeeDesignation = empInfo.PRM_Designation.Name;
            model.SeniorityNumber = empInfo.SeniorityPosition;

            #endregion

            #region Over all Assessment
            if (model.OverAllAssessment == "Excellent")
                model.Excellent = true;
            else if (model.OverAllAssessment == "Good")
                model.Good = true;
            else if (model.OverAllAssessment == "Standard")
                model.Standard = true;
            else if (model.OverAllAssessment == "BelowStnd")
                model.BelowStnd = true;
            else
                model.NotExpt = true;
            #endregion

            #region Qualification
            if (model.QualificationForPromotion == "EligiblePro")
                model.A = true;
            else if (model.QualificationForPromotion == "NotEligiblePro")
                model.B = true;
            else if (model.QualificationForPromotion == "HighlyRecomPro")
                model.C = true;
            else
                model.D = true;

            #endregion

            #region Reporting Officer
            model.NameOfReportingOfficer = entity.PRM_EmploymentInfo1.FullName;
            model.Designation = entity.PRM_EmploymentInfo1.PRM_Designation.Name;
            
            var division = _prmCommonService.PRMUnit.DivisionRepository.GetAll()
                .FirstOrDefault(d => d.Id == entity.PRM_EmploymentInfo1.DivisionId);
            if (division != null)
            {
                model.Department = division.Name;
            }

            #endregion

            #region Reviewing Officer
            model.NameOfReviewingOfficer = loginUser.EmpName;
            model.ReDesignation = loginUser.DesignationName;
            model.ReDepartment = loginUser.Department;
            #endregion

            model.ActionType = "EditAssessmentInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.AssessmentInfo = model;
            parentModel.EmpId = model.EmployeeId.ToString();
            parentModel.Id = model.StaffInfoId;

            if (type == "success")
            {
                parentModel.AssessmentInfo.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.AssessmentInfo.ErrorClass = "success";
                parentModel.AssessmentInfo.IsError = 0;
            }

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var approvalStepId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == processId && q.ApplicationId == id && q.ApproverId == biodataInfo.NextApproverId).FirstOrDefault().ApprovalStepId;
            var nextApproverList = GetNextApprover((int)id, approvalStepId);
            model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditAssessmentInfo(StaffAssessmentInfoViewModel model)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "AssessmentInfo";
            var error = string.Empty;
            model = GetReadyModel(model);

            var processId = _prmCommonService.PRMUnit.ApprovalProcessRepository.Get(q => q.ProcessNameEnum == "ACR").FirstOrDefault().Id;
            var officerInfo = _prmCommonService.PRMUnit.ACRStaffInfoRepository.GetByID(model.StaffInfoId);
            var biodataOfficer = _prmCommonService.PRMUnit.ACRStaffBioDataRepository.GetByID(model.StaffInfoId, "StaffInfoId");
            var assessmentInfo = _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.GetByID(model.StaffInfoId, "StaffInfoId");

            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);

            int approvalStepId = 0;
            if (biodataOfficer.NextApproverId == loginUser.ID)
            {
                approvalStepId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == processId && q.ApplicationId == model.StaffInfoId && q.ApproverId == biodataOfficer.NextApproverId).LastOrDefault().ApprovalStepId;
                model.ApprovalActionName = "Recommendation";
            }
            else
            {
                approvalStepId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApprovalProcessId == processId && q.ApplicationId == model.StaffInfoId && q.ApproverId == assessmentInfo.NextApproverId).LastOrDefault().ApprovalStepId;
                model.ApprovalActionName = "Approval";
            }

            var nextApproverList = GetNextApprover(model.StaffInfoId, approvalStepId);

            if (ModelState.IsValid)
            {
                var entity = CreateEntity(model, false);
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    if (model.ApprovalActionName == "Recommendation")
                    {
                        entity.ReportingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    }
                    else
                    {
                        entity.ReviewingOfficerId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    }
                    if (model.ApprovalActionName == "Recommendation")
                    {
                        entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(t => t.StatusName.Contains("Recommend")).FirstOrDefault().Id;
                    }
                    else
                    {
                        entity.ApprovalStatusId = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(t => t.StatusName.Contains("Approv")).FirstOrDefault().Id;
                    }
                    entity.ApproverId = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name).ID;
                    _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.Update(entity, "StaffInfoId");
                    _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.SaveChanges();

                    int applicationId = 0;
                    string actionName = string.Empty;
                    if (model.ApprovalActionName == "Recommendation")
                    {
                        applicationId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApplicationId == entity.StaffInfoId && q.ApprovalProcessId == processId && q.IsOnlineApplication == false && q.ApproverId == biodataOfficer.NextApproverId).LastOrDefault().Id;
                        actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Recommend")).First().StatusName;
                    }
                    else
                    {
                        applicationId = _prmCommonService.PRMUnit.RequestedApplicationInformationRepository.Get(q => q.ApplicationId == entity.StaffInfoId && q.ApprovalProcessId == processId && q.IsOnlineApplication == false && q.ApproverId == assessmentInfo.NextApproverId).LastOrDefault().Id;
                        actionName = _prmCommonService.PRMUnit.ApprovalStatusRepository.Get(q => q.StatusName.Contains("Approv")).First().StatusName;
                    }


                    var result = _prmCommonService.PRMUnit.FunctionRepository.ProceedToNextStep(applicationId, actionName, model.ApproverComments, model.NextApproverId, entity.IUser);

                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";

                    parentModel.AssessmentInfo = model;
                    return View("CreateOrEdit", parentModel);
                }
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;
                if (model.ApprovalActionName == "Recommendation")
                {
                    model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
                }
                parentModel.AssessmentInfo = model;
                parentModel.Id = model.StaffInfoId;
                parentModel.EmpId = model.EmployeeId.ToString();

                return View("CreateOrEdit", parentModel);
            }

            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";
            if (model.ApprovalActionName == "Recommendation")
            {
                model.ApproverList = Common.PopulateEmployeeDDL(nextApproverList);
            }

            parentModel.AssessmentInfo = model;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete------------------------------
        public ActionResult DeleteAssessmentInfo(int id)
        {
            var parentModel = new ACRforStaffViewModel();
            parentModel.ViewType = "AssessmentInfo";

            var entity = _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.GetByID(id, "StaffInfoId");

            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var allTypes = new List<Type> { typeof(PRM_EmpACRAssessmentInfoDetail) };

                    _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.Delete(id, "StaffInfoId", allTypes);
                    _prmCommonService.PRMUnit.ACRAssessmentInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";

                    parentModel.AssessmentInfo = model;
                    return View("CreateOrEdit", parentModel);
                }
                return RedirectToAction("Index");
            }

            model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.AssessmentInfo = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #endregion

        #region Employee Information
        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeName = obj.FullName,
                Designation = obj.PRM_Designation.Name,
                
                Department = obj.DivisionId == null ? string.Empty : obj.PRM_Division.Name,
                
                Section = obj.PRM_Section == null ? string.Empty : obj.PRM_Section.Name,
                SeniorityPosition = obj.SeniorityPosition,
                JoiningDate = obj.DateofJoining.ToString("yyyy-MM-dd"),
                PRLDate = obj.PRLDate == null ? string.Empty : (Convert.ToDateTime(obj.PRLDate)).ToString("yyyy-MM-dd"),
            });

        }
        #endregion

        #region Approval Process

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
    }
}