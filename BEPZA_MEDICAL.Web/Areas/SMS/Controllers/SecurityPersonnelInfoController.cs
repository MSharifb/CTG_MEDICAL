using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Collections;
using System.IO;
using System.Web.Helpers;
using System.Data;
using System.Text;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Web.Configuration;

using BEPZA_MEDICAL.DAL.AMS;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.AMS;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.SMS.ViewModel.SecurityPersonnelInfo;
using BEPZA_MEDICAL.Web.Areas.SMS.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.SecurityService;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Controllers;
using System.Reflection;

namespace BEPZA_MEDICAL.Web.Areas.SMS.Controllers
{
    public class SecurityPersonnelInfoController : BaseController
    {
        #region Fields

        private readonly AMSCommonService _amsCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        private readonly PersonalInfoService _personalInfoService;
        private readonly EmployeeService _empService;
        private readonly JobGradeService _JobGradeService;

        #endregion

        #region Constructor
        public SecurityPersonnelInfoController(EmployeeService empService, AMSCommonService amsCommonService, PRMCommonSevice prmCommonService, PersonalInfoService personalInfoService, JobGradeService jobGradeService)
        {
            this._amsCommonService = amsCommonService;
            this._prmCommonService = prmCommonService;
            this._personalInfoService = personalInfoService;
            this._empService = empService;
            this._JobGradeService = jobGradeService;

        }
        #endregion

        #region Actions

        #region Employee Search

        public ActionResult Index()
        {
            var model = new SecurityPersonnelSearchViewModel();
            model.ZoneInfoId = LoggedUserZoneInfoId;
            model.ActionName = "EmploymentInfoIndex";

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SecurityPersonnelSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var orgList = new List<PRM_EmpExperience>();
            var orgObj = _prmCommonService.PRMUnit.JobExperienceInfoRepository.GetAll().Where(x => x.SecurityOrganizationId != null).FirstOrDefault();
            if (orgObj != null)
            {
                orgList = (from x in _prmCommonService.PRMUnit.JobExperienceInfoRepository.GetAll().Where(x => x.SecurityOrganizationId != null)
                               join m in
                                   (
                                       (from m in _prmCommonService.PRMUnit.JobExperienceInfoRepository.GetAll()
                                        where m.SecurityOrganizationId != null
                                        group m by m.EmployeeId into g
                                        select new
                                        {
                                            EmployeeId = g.Key,
                                            EndDate = g.Select(q => q.EndDate).Max()
                                        }

                                       ).DefaultIfEmpty()) on new { x.EmployeeId, x.EndDate } equals new { m.EmployeeId, m.EndDate }
                               select x).DefaultIfEmpty().ToList();
            }

            var employeeList = _prmCommonService.PRMUnit.EmploymentInfoRepository.Get(q => q.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            var result = (from s in _amsCommonService.AMSUnit.SecurityInfoRepository.GetAll()
                          join e in employeeList on s.EmployeeId equals e.Id
                          join d in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on e.DesignationId equals d.Id
                          join ex in orgList on s.EmployeeId equals ex.EmployeeId into lEx
                          from exp in lEx.DefaultIfEmpty()
                          orderby d.SortingOrder, e.DateofJoining
                          select new
                          {
                              Id = s.Id,
                              EmployeeId = s.EmployeeId,
                              FullName = e.FullName,
                              EmpID = e.EmpID,
                              DesignationId = e.DesignationId,
                              desigName = d.Name,
                              DateofJoining = e.DateofJoining,
                              SortOrder = d.SortingOrder,
                              OrganizationId = exp != null ? exp.SecurityOrganizationId : 0
                          }).ToList();

            if (request.Searching)
            {
                if (viewModel.OrganizationId != 0)
                {
                    result = result.Where(x => x.OrganizationId == viewModel.OrganizationId).ToList();
                }
                if (viewModel.DesignationId != 0)
                {
                    result = result.Where(x => x.DesignationId == viewModel.DesignationId).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.FullName))
                {
                    result = result.Where(x => x.FullName.Contains(viewModel.FullName)).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.EmpID))
                {
                    result = result.Where(x => x.EmpID.Contains(viewModel.EmpID)).ToList();
                }
            }

            totalRecords = result == null ? 0 : result.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            result = result.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in result)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.FullName,
                    d.EmpID,
                    d.DesignationId,
                    d.OrganizationId,
                    d.desigName,
                    d.DateofJoining.ToString(DateAndTime.GlobalDateFormat)
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        #endregion

        #region Security Personnel Basic Information

        #region Insert-----------------------------------

        public ActionResult EmploymentInfoIndex(int? id)
        {
            if (id.HasValue)
                return RedirectToAction("EditEmploymentInfo", "SecurityPersonnelInfo", new { id = id });

            var parentModel = new SecurityPersonnelViewModel();
            parentModel.ViewType = "EmploymentInfo";

            populateDropdown(parentModel.EmploymentInfo);

            parentModel.EmploymentInfo.BEPZAID = "";
            parentModel.EmploymentInfo.IsActive = true;

            parentModel.EmploymentInfo.ActionType = "CreateEmploymentInfo";
            parentModel.EmploymentInfo.ButtonText = "Save";
            parentModel.EmploymentInfo.SelectedClass = "selected";

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateEmploymentInfo(SecurityPersonnelEmpInfoViewModel model)
        {
            var parentModel = new SecurityPersonnelViewModel();
            parentModel.ViewType = "EmploymentInfo";

            var error = CheckEmpInfoBusinessRule(model);

            if (ModelState.IsValid && error == string.Empty)
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;

                    _amsCommonService.AMSUnit.SecurityInfoRepository.Add(entity);
                    _amsCommonService.AMSUnit.SecurityInfoRepository.SaveChanges();

                    #region Update Employee

                    var employee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
                    if (employee.FullName != model.FullName)
                    {
                        employee.FullName = model.FullName;
                        employee.TitleId = model.TitleId;
                        employee.FirstName = model.FirstName;
                        employee.LastName = model.LastName;

                        _prmCommonService.PRMUnit.EmploymentInfoRepository.Update(employee);
                        _prmCommonService.PRMUnit.EmploymentInfoRepository.SaveChanges();
                    }

                    var personalInfo = _prmCommonService.PRMUnit.PersonalInfoRepository.Get(x => x.EmployeeId == entity.EmployeeId).FirstOrDefault();
                    if (personalInfo != null && (personalInfo.FatherName != model.FatherName || personalInfo.PermanentDistictId != model.DistrictId))
                    {
                        if (!string.IsNullOrEmpty(model.FatherName))
                        {
                            personalInfo.FatherName = model.FatherName;
                        }

                        if (model.DistrictId > 0)
                        {
                            personalInfo.PermanentDistictId = (Int32)model.DistrictId;
                        }

                        _prmCommonService.PRMUnit.PersonalInfoRepository.Update(personalInfo, "EmployeeId");
                        _prmCommonService.PRMUnit.PersonalInfoRepository.SaveChanges();
                    }

                    #endregion


                    parentModel.Id = entity.Id;
                    //parentModel.BEPZAId = entity.PRM_EmploymentInfo.EmpID;

                }
                catch (Exception ex)
                {
                    populateDropdown(model);

                    parentModel.EmploymentInfo = model;
                    parentModel.EmploymentInfo.ButtonText = "Save";
                    parentModel.EmploymentInfo.SelectedClass = "selected";
                    parentModel.EmploymentInfo.ErrorClass = "failed";
                    parentModel.EmploymentInfo.Message = Resources.ErrorMessages.InsertFailed;
                    parentModel.EmploymentInfo.IsError = 1;

                    return View("CreateOrEdit", parentModel);
                }
            }
            else
            {
                populateDropdown(model);

                parentModel.EmploymentInfo = model;
                parentModel.EmploymentInfo.ButtonText = "Save";
                parentModel.EmploymentInfo.SelectedClass = "selected";
                parentModel.EmploymentInfo.ErrorClass = "failed";
                parentModel.EmploymentInfo.Message = Resources.ErrorMessages.InsertFailed + " " + error;
                parentModel.EmploymentInfo.IsError = 1;

                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("EditEmploymentInfo", "SecurityPersonnelInfo", new { id = parentModel.Id, type = "success" });
        }

        #endregion

        #region Update--------------------------------------

        public ActionResult EditEmploymentInfo(int id, string type)
        {
            var parentModel = new SecurityPersonnelViewModel();
            parentModel.ViewType = "EmploymentInfo";

            var entity = _amsCommonService.AMSUnit.SecurityInfoRepository.GetByID(id);
            var model = entity.ToModel();

            //if (model.InactiveDate.HasValue)
            //{
            //    model.InactiveDate = model.InactiveDate.Value.Date;
            //}

            populateEmployee(model);
            populateDropdown(model);

            model.ActionType = "EditEmploymentInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.EmploymentInfo = model;
            parentModel.BEPZAId = model.BEPZAID;
            parentModel.Id = model.Id;

            model.IsPhotoExist = Common.IsPhotoExist(model.EmployeeId, true, _empService);

            if (type == "success")
            {
                parentModel.EmploymentInfo.Message = Resources.ErrorMessages.InsertSuccessful;
                parentModel.EmploymentInfo.ErrorClass = "success";
                parentModel.EmploymentInfo.IsError = 0;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult EditEmploymentInfo(SecurityPersonnelEmpInfoViewModel model, FormCollection form)
        {
            var parentModel = new SecurityPersonnelViewModel();
            parentModel.ViewType = "EmploymentInfo";

            model.IsPhotoExist = Common.IsPhotoExist(model.EmployeeId, true, _empService);

            var error = CheckEmpInfoBusinessRule(model);

            if (ModelState.IsValid && error == string.Empty)
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    _amsCommonService.AMSUnit.SecurityInfoRepository.Update(entity, new Dictionary<Type, ArrayList>());
                    _amsCommonService.AMSUnit.SecurityInfoRepository.SaveChanges();

                    #region Update Employee

                    var employee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);
                    if (employee.FullName != model.FullName)
                    {
                        employee.FullName = model.FullName;
                        employee.TitleId = model.TitleId;
                        employee.FirstName = model.FirstName;
                        employee.LastName = model.LastName;

                        _prmCommonService.PRMUnit.EmploymentInfoRepository.Update(employee);
                        _prmCommonService.PRMUnit.EmploymentInfoRepository.SaveChanges();
                    }

                    var personalInfo = _prmCommonService.PRMUnit.PersonalInfoRepository.Get(x => x.EmployeeId == entity.EmployeeId).FirstOrDefault();
                    if (personalInfo != null && (personalInfo.FatherName != model.FatherName || personalInfo.PermanentDistictId != model.DistrictId))
                    {
                        if (!string.IsNullOrEmpty(model.FatherName))
                        {
                            personalInfo.FatherName = model.FatherName;
                        }

                        if (model.DistrictId > 0)
                        {
                            personalInfo.PermanentDistictId = (Int32)model.DistrictId;
                        }

                        _prmCommonService.PRMUnit.PersonalInfoRepository.Update(personalInfo, "EmployeeId");
                        _prmCommonService.PRMUnit.PersonalInfoRepository.SaveChanges();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    populateDropdown(model);
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";
                    model.IsError = 1;

                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "selected";

                    parentModel.EmploymentInfo = model;
                    return View("CreateOrEdit", parentModel);
                }

                populateDropdown(model);
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "selected";

                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
                model.IsError = 0;

                parentModel.EmploymentInfo = model;
                parentModel.Id = model.Id;
                parentModel.BEPZAId = model.BEPZAID;


                return View("CreateOrEdit", parentModel);

            }

            populateDropdown(model);
            model.Message = Resources.ErrorMessages.UpdateFailed + " " + error;
            model.ErrorClass = "failed";
            model.IsError = 1;

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "selected";

            parentModel.EmploymentInfo = model;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete

        public ActionResult Delete(int id)
        {
            var parentModel = new SecurityPersonnelViewModel();
            parentModel.ViewType = "EmploymentInfo";

            var entity = _amsCommonService.AMSUnit.SecurityInfoRepository.GetByID(id);
            var model = entity.ToModel();
            if (ModelState.IsValid)
            {
                try
                {
                    _amsCommonService.AMSUnit.SecurityInfoRepository.Delete(entity);
                    _amsCommonService.AMSUnit.SecurityInfoRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    populateDropdown(model);
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.IsError = 1;
                    model.DeleteEnable = true;
                    model.ButtonText = "Update";

                    parentModel.EmploymentInfo = model;
                    return View("CreateOrEdit", parentModel);
                }

                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.IsError = 0;
                model.ErrorClass = "success delete-emp";

                parentModel.EmploymentInfo = model;
                return View("CreateOrEdit", parentModel);
            }

            model = entity.ToModel();
            populateDropdown(model);
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.IsError = 1;

            parentModel.EmploymentInfo = model;
            return View("CreateOrEdit", parentModel);
        }

        #endregion


        #endregion

        #region Employee Photograph/Signature------------------------------------------

        public FileContentResult GetImage(int? id, bool? isPhoto)
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

        #endregion

        #region Accademic Qualification Info
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAllAccademicQlfnInfoList(JqGridRequest request, SecurityPersonnelSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllAccademicQlfnInfoByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).OrderByDescending(x => x.YearOfPassing).ToList();
            var totalRecords = list.Count();

            #region Sorting
            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }

            if (request.SortingName == "ExamLevel")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ExamLevel).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ExamLevel).ToList();
                }
            }
            if (request.SortingName == "UniversityOrBorard")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.UniversityOrBorard).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.UniversityOrBorard).ToList();
                }
            }

            if (request.SortingName == "YearOfPassing")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.YearOfPassing).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.YearOfPassing).ToList();
                }
            }


            if (request.SortingName == "AccademicGrade")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AccademicGrade).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AccademicGrade).ToList();
                }
            }

            if (request.SortingName == "InstituteName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.InstituteName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.InstituteName).ToList();
                }
            }

            if (request.SortingName == "SubjectGroup")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SubjectGroup).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SubjectGroup).ToList();
                }
            }
            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.ExamLevel,
                    item.InstituteName,                  
                    item.YearOfPassing,
                    item.AccademicGrade,
                    item.UniversityOrBorard,
                    item.SubjectGroup
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult AccademicQlfnInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new SecurityPersonnelViewModel();
            var model = parentModel.AccademicQlfnInfo;

            if (id.HasValue)
            {
                var employeeId = 0;
                var securityInfo = _amsCommonService.AMSUnit.SecurityInfoRepository.GetByID(id.Value);
                if (securityInfo != null)
                {
                    employeeId = securityInfo.EmployeeId;
                }

                var entity = _prmCommonService.PRMUnit.AccademicQualificationRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    if (type == "Clear")
                    {
                        employeeId = id.Value;
                    }
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(employeeId);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(employeeId);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        model = entity.ToModel();
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : employeeId);
                parentModel.EmployeeId = empEntity.Id;

                var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == parentModel.EmployeeId).FirstOrDefault().Id;
                parentModel.Id = securityId;

                parentModel.ViewType = "AccademicQlfnInfo";
                if (empEntity != null)
                {
                    //model.InactiveDate = empEntity.DateofInactive;
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }
            }
            parentModel.AccademicQlfnInfo = model;
            PopulateDropdownListACC(model);
            parentModel.AccademicQlfnInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.AccademicQlfnInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditAccademicQlfnInfo(AccademicQlfnInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new SecurityPersonnelViewModel();
            parentModel.EmployeeId = model.EmployeeId;

            var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == parentModel.EmployeeId).FirstOrDefault().Id;
            parentModel.Id = securityId;

            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    businessError = _personalInfoService.CheckBusinessLogicACC(entity);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "AccademicQlfnInfo";
                        parentModel.AccademicQlfnInfo = model;
                        PopulateDropdownListACC(model);
                        if (model.strMode == "add")
                        {
                            model.DeleteEnable = false;
                            model.ButtonText = "Save";
                        }
                        else
                        {
                            model.DeleteEnable = true;
                            model.ButtonText = "Update";
                        }
                        return View("CreateOrEdit", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _prmCommonService.PRMUnit.AccademicQualificationRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        entity.Id = model.Id;

                        _prmCommonService.PRMUnit.AccademicQualificationRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _prmCommonService.PRMUnit.AccademicQualificationRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListACC(model);

                parentModel.ViewType = "AccademicQlfnInfo";
                parentModel.AccademicQlfnInfo = model;
                parentModel.AccademicQlfnInfo.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
            catch
            {
                PopulateDropdownListACC(model);

                if (model.strMode == "add")
                {
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    model.errClass = "failed";
                    model.ButtonText = "Save";
                }
                else if (model.strMode == "edit")
                {
                    model.Message = Resources.ErrorMessages.UpdateFailed;
                    model.errClass = "failed";
                    model.ButtonText = "Update";
                }

                parentModel.ViewType = "AccademicQlfnInfo";
                parentModel.AccademicQlfnInfo = model;
                parentModel.AccademicQlfnInfo.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
        }

        public ActionResult DeleteAccademicQlfnInfo(int id)
        {
            var parentModel = new SecurityPersonnelViewModel();
            parentModel.ViewType = "AccademicQlfnInfo";

            var entity = _prmCommonService.PRMUnit.AccademicQualificationRepository.GetByID(id);
            dynamic model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _prmCommonService.PRMUnit.AccademicQualificationRepository.Delete(entity);

                    _prmCommonService.PRMUnit.AccademicQualificationRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";

                    var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == entity.EmployeeId).FirstOrDefault().Id;

                    return RedirectToAction("AccademicQlfnInfoIndex", null, new { id = securityId, IsMenu = true, type = "success" });
                }
                else
                {
                    //PopulateDropdownList(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";
                    parentModel.AccademicQlfnInfo = model;
                }
            }
            catch
            {
                //PopulateDropdownList(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.AccademicQlfnInfo = model;
            }

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Job Experience Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetJobExperienceList(JqGridRequest request, SecurityPersonnelSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllJobExperienceInfoByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).ToList().OrderBy(x => x.EndDate).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "Organization1")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Organization1).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Organization1).ToList();
                }
            }

            if (request.SortingName == "OrganizationType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OrganizationType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OrganizationType).ToList();
                }
            }

            if (request.SortingName == "FromDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FromDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FromDate).ToList();
                }
            }

            if (request.SortingName == "EndDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EndDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EndDate).ToList();
                }
            }
            if (request.SortingName == "Duration")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Duration).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Duration).ToList();
                }
            }

            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };


            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.Organization1,
                    item.OrganizationType,
                    Convert.ToDateTime(item.FromDate).ToString("dd-MMM-yyyy"),
                    Convert.ToDateTime(item.EndDate).ToString("dd-MMM-yyyy"),
                    item.Duration
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult JobExperienceInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new SecurityPersonnelViewModel();
            var model = parentModel.JobExperienceInfo;

            if (id.HasValue)
            {
                var employeeId = 0;
                var securityInfo = _amsCommonService.AMSUnit.SecurityInfoRepository.GetByID(id.Value);
                if (securityInfo != null)
                {
                    employeeId = securityInfo.EmployeeId;
                }

                var entity = _prmCommonService.PRMUnit.JobExperienceInfoRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    if (type == "Clear")
                    {
                        employeeId = id.Value;
                    }
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(employeeId);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(employeeId);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        model = entity.ToModel();
                        var tempDuration = (Convert.ToDateTime(model.EndDate) - Convert.ToDateTime(model.FromDate)).TotalDays / 365;
                        model.Duration = Convert.ToDecimal(Math.Round(tempDuration, 2));
                        model.StrDuration = _amsCommonService.AMSUnit.FunctionRepository.fnGetServiceDuration(model.FromDate ?? DateTime.Now, model.EndDate ?? DateTime.Now);
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : employeeId);
                parentModel.EmployeeId = empEntity.Id;

                var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == parentModel.EmployeeId).FirstOrDefault().Id;
                parentModel.Id = securityId;

                parentModel.ViewType = "JobExperienceInfo";

                if (empEntity != null)
                {
                    //model.InactiveDate = empEntity.DateofInactive;
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }
            }

            parentModel.JobExperienceInfo = model;
            PopulateDropdownListJOBEXP(model);
            parentModel.JobExperienceInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.JobExperienceInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditJobExperienceInfo(JobExperienceInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new SecurityPersonnelViewModel();

            parentModel.EmployeeId = model.EmployeeId;

            var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == parentModel.EmployeeId).FirstOrDefault().Id;
            parentModel.Id = securityId;

            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    businessError = _personalInfoService.CheckBusinessLogicJOBEXP(entity, model.strMode);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "JobExperienceInfo";
                        parentModel.JobExperienceInfo = model;
                        PopulateDropdownListJOBEXP(model);
                        if (model.strMode == "add")
                        {
                            model.DeleteEnable = false;
                            model.ButtonText = "Save";
                        }
                        else
                        {
                            model.DeleteEnable = true;
                            model.ButtonText = "Update";
                        }
                        return View("CreateOrEdit", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _prmCommonService.PRMUnit.JobExperienceInfoRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        entity.Id = model.Id; //

                        _prmCommonService.PRMUnit.JobExperienceInfoRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _prmCommonService.PRMUnit.JobExperienceInfoRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListJOBEXP(model);

                parentModel.ViewType = "JobExperienceInfo";
                parentModel.JobExperienceInfo = model;
                parentModel.JobExperienceInfo.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListJOBEXP(model);

                if (model.strMode == "add")
                {
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    model.errClass = "failed";
                }
                else if (model.strMode == "edit")
                {
                    model.Message = Resources.ErrorMessages.UpdateFailed;
                    model.errClass = "failed";
                    model.ButtonText = "Update";
                }

                parentModel.ViewType = "JobExperienceInfo";
                parentModel.JobExperienceInfo = model;
                parentModel.JobExperienceInfo.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
        }
        public ActionResult DeleteJobExperienceInfo(int id)
        {
            var parentModel = new SecurityPersonnelViewModel();
            parentModel.ViewType = "JobExperienceInfo";

            var entity = _prmCommonService.PRMUnit.JobExperienceInfoRepository.GetByID(id);

            dynamic model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _prmCommonService.PRMUnit.JobExperienceInfoRepository.Delete(entity);

                    _prmCommonService.PRMUnit.JobExperienceInfoRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";

                    var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == entity.EmployeeId).FirstOrDefault().Id;

                    return RedirectToAction("JobExperienceInfoIndex", null, new { id = securityId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListJOBEXP(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.JobExperienceInfo = model;
                }

            }
            catch
            {
                PopulateDropdownListJOBEXP(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.JobExperienceInfo = model;
            }

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Service History


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetServiceHistoryList(JqGridRequest request, SecurityPersonnelSearchViewModel viewModel, int Id)
        {
            var list = _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.GetAll().Where(x => x.EmployeeId == viewModel.ID).OrderBy(x => x.PeriodFrom).ToList();

            var totalRecords = list.Count();

            #region Sorting
            if (request.SortingName == "Id")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }

            if (request.SortingName == "ZoneName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_ZoneInfo.ZoneName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_ZoneInfo.ZoneName).ToList();
                }
            }
            if (request.SortingName == "PeriodFrom")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PeriodFrom).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PeriodFrom).ToList();
                }
            }

            if (request.SortingName == "PeriodTo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PeriodTo ?? DateTime.Now).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PeriodTo ?? DateTime.Now).ToList();
                }
            }


            if (request.SortingName == "Duration")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Duration).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Duration).ToList();
                }
            }

            if (request.SortingName == "DisciplineRecord")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DisciplineRecord).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DisciplineRecord).ToList();
                }
            }

            if (request.SortingName == "Remarks")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Remarks).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Remarks).ToList();
                }
            }
            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                var ZoneName = string.Empty;
                ZoneName = _prmCommonService.PRMUnit.ZoneInfoRepository.GetByID(item.WorkZoneId).ZoneName;
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {  
                    item.Id,
                    ZoneName,
                    Convert.ToDateTime(item.PeriodFrom).ToString(DateAndTime.GlobalDateFormat),
                    item.PeriodTo != null ? Convert.ToDateTime(item.PeriodTo).ToString(DateAndTime.GlobalDateFormat) : "Present",
                    item.Duration,
                    item.Award,
                    item.DisciplineRecord
                  
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ServiceHistoryIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new SecurityPersonnelViewModel();
            var model = parentModel.EmpServiceHistoryViewModel;

            if (id.HasValue)
            {
                var employeeId = 0;
                var securityInfo = _amsCommonService.AMSUnit.SecurityInfoRepository.GetByID(id.Value);
                if (securityInfo != null)
                {
                    employeeId = securityInfo.EmployeeId;
                    model.SecurityInfoId = securityInfo.Id;
                }

                var entity = _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.GetByID(id.Value);
                if (IsMenu)
                {
                    if (type == "Clear")
                    {
                        employeeId = id.Value;
                    }
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(employeeId);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(employeeId);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {

                        model = entity.ToModel();
                        model.StrDuration = _amsCommonService.AMSUnit.FunctionRepository.fnGetServiceDuration(model.PeriodFrom, model.PeriodTo ?? DateTime.Now);
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : employeeId);
                parentModel.EmployeeId = empEntity.Id;

                var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == parentModel.EmployeeId).FirstOrDefault().Id;
                parentModel.Id = securityId;

                parentModel.ViewType = "EmpServiceHisotory";

                if (empEntity != null)
                {
                    //model.InactiveDate = empEntity.DateofInactive;
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }

            }

            parentModel.EmpServiceHistoryViewModel = model;
            PopulateDropdownListServiceHistory(model);
            parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.EmpServiceHistoryViewModel.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditServiceHistory(SecurityServiceHistoryViewModel model)
        {

            string businessError = string.Empty;
            var parentModel = new SecurityPersonnelViewModel();

            parentModel.EmployeeId = model.EmployeeId;

            var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == parentModel.EmployeeId).FirstOrDefault().Id;
            parentModel.Id = securityId;
            model.SecurityInfoId = securityId;

            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                if (ModelState.IsValid)
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = Common.CurrentDateTime;
                    var entity = model.ToEntity();

                    businessError = CheckSecurityServiceHistoryBusinessRule(model);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "EmpServiceHisotory";
                        parentModel.EmpServiceHistoryViewModel = model;
                        PopulateDropdownListServiceHistory(model);
                        if (model.strMode == "add")
                        {
                            model.DeleteEnable = false;
                            model.ButtonText = "Save";
                        }
                        else
                        {
                            model.DeleteEnable = true;
                            model.ButtonText = "Update";
                        }
                        return View("CreateOrEdit", parentModel);
                    }

                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        entity.EUser = User.Identity.Name;
                        entity.EDate = Common.CurrentDateTime;

                        entity.Id = model.Id; //To check

                        _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListServiceHistory(model);

                parentModel.ViewType = "EmpServiceHisotory";
                parentModel.EmpServiceHistoryViewModel = model;
                parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListServiceHistory(model);

                if (model.strMode == "add")
                {
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    model.errClass = "failed";
                }
                else if (model.strMode == "edit")
                {
                    model.Message = Resources.ErrorMessages.UpdateFailed;
                    model.errClass = "failed";
                    model.ButtonText = "Update";
                }
                parentModel.ViewType = "EmpServiceHisotory";
                parentModel.EmpServiceHistoryViewModel = model;
                parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

                return View("CreateOrEdit", parentModel);
            }
        }

        public ActionResult DeleteServiceHistory(int id)
        {
            var parentModel = new SecurityPersonnelViewModel();
            parentModel.ViewType = "EmpServiceHisotory";

            var entity = _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.Delete(entity);
                    _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";

                    var securityId = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == entity.EmployeeId).FirstOrDefault().Id;

                    return RedirectToAction("ServiceHistoryIndex", null, new { id = securityId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListServiceHistory(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.EmpServiceHistoryViewModel = model;
                }

            }
            catch
            {
                PopulateDropdownListServiceHistory(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.EmpServiceHistoryViewModel = model;
            }

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        #region Delete Security Records

        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.Delete(x => x.SecurityInfoId == id);
                _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.SaveChanges();

                _amsCommonService.AMSUnit.SecurityInfoRepository.Delete(id);
                _amsCommonService.AMSUnit.SecurityInfoRepository.SaveChanges();

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

        #region Others action

        [NoCache]
        public JsonResult GetEmployeeInfo(int id)
        {
            var Designation = string.Empty;
            var FatherName = string.Empty;
            var DistrictId = 0;
            var BloodGroup = string.Empty;

            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(id);

            var designationInfo = _prmCommonService.PRMUnit.DesignationRepository.GetByID(obj.DesignationId);
            if (designationInfo != null)
            {
                Designation = designationInfo.Name;
            }

            var personalInfo = _prmCommonService.PRMUnit.PersonalInfoRepository.Get(x => x.EmployeeId == id).FirstOrDefault();
            if (personalInfo != null)
            {
                FatherName = personalInfo.FatherName;
                DistrictId = personalInfo.PermanentDistictId;
                if (personalInfo.BloodGroupId != null)
                {
                    var bloodGroupid = personalInfo.BloodGroupId;
                    BloodGroup = _empService.PRMUnit.BloodGroupRepository.GetByID(bloodGroupid).Name;
                }
            }

            return Json(new
            {
                EmpID = obj.EmpID,
                EmployeeName = obj.FullName,
                FatherName = FatherName,
                Designation = Designation,
                DOB = obj.DateofBirth != null ? obj.DateofBirth.ToString("yyyy-MM-dd") : string.Empty,
                DOJ = obj.DateofJoining.ToString("yyyy-MM-dd"),
                DistrictId = DistrictId,
                TitleId = obj.TitleId,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                BloodGroup = BloodGroup
            });

        }

        private void populateEmployee(SecurityPersonnelEmpInfoViewModel model)
        {
            if (model.EmployeeId > 0)
            {
                var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                model.BEPZAID = obj.EmpID;
                model.FullName = obj.FullName;
                model.DateofBirth = obj.DateofBirth;
                model.DateofJoining = obj.DateofJoining;
                model.TitleId = obj.TitleId;
                model.FirstName = obj.FirstName;
                model.LastName = obj.LastName;

                var designationInfo = _prmCommonService.PRMUnit.DesignationRepository.GetByID(obj.DesignationId);
                if (designationInfo != null)
                {
                    model.DesignationName = designationInfo.Name;
                }

                var personalInfo = _prmCommonService.PRMUnit.PersonalInfoRepository.Get(x => x.EmployeeId == model.EmployeeId).FirstOrDefault();
                if (personalInfo != null)
                {
                    model.FatherName = personalInfo.FatherName;
                    model.DistrictId = personalInfo.PermanentDistictId;
                    if (personalInfo.BloodGroupId != null)
                    {
                        var bloodGroupid = personalInfo.BloodGroupId;
                        model.BloodGroup = _empService.PRMUnit.BloodGroupRepository.GetByID(bloodGroupid).Name;
                    }
                }

            }
        }

        [NoCache]
        public JsonResult GetDuration(DateTime startDate, DateTime endDate)
        {
            var duration = string.Empty;

            duration = _amsCommonService.AMSUnit.FunctionRepository.fnGetServiceDuration(startDate, endDate);

            return Json(new { Duration = duration }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Dropdown

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = (from s in _amsCommonService.AMSUnit.SecurityInfoRepository.GetAll()
                                join e in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on s.EmployeeId equals e.Id
                                join d in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on e.DesignationId equals d.Id
                                select d).DistinctBy(x=>x.Id).OrderBy(s=>s.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetOrganization()
        {
            var org = _amsCommonService.AMSUnit.SecurityOrganizationRepository.GetAll().OrderBy(x => x.SortOrder).ToList();

            return PartialView("Select", Common.PopulateDllList(org));
        }

        private void populateDropdown(SecurityPersonnelEmpInfoViewModel model)
        {
            #region District
            model.DistrictList = Common.PopulateDistrictDDL(_prmCommonService.PRMUnit.DistrictRepository.GetAll().OrderBy(x => x.DistrictName));
            #endregion

            #region Title
            var titleList = _prmCommonService.PRMUnit.NameTitleRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.TitleList = Common.PopulateDllList(titleList);
            #endregion

            //#region BloodGroup ddl
            //var ddlList = _empService.PRMUnit.BloodGroupRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            //model.BloodGroupList = Common.PopulateDllList(ddlList);
            //#endregion
        }

        private void PopulateDropdownListACC(AccademicQlfnInfoViewModel model)
        {
            dynamic ddlList;

            #region Exam/Level ddl

            ddlList = _prmCommonService.PRMUnit.ExamDegreeLavelRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.ExamLavelList = Common.PopulateDllList(ddlList);

            #endregion

            #region Passing Year List

            model.YearOfPassingList = Common.PopulateYearList();

            #endregion

            #region Result ddl

            ddlList = _prmCommonService.PRMUnit.AcademicGradeRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.ResultList = Common.PopulateDllList(ddlList);

            #endregion

            #region Institute ddl

            ddlList = _prmCommonService.PRMUnit.UniversityAndBoardRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.UniversityAndBoardList = Common.PopulateDllList(ddlList);

            #endregion

            #region Subject/Group ddl

            ddlList = _prmCommonService.PRMUnit.SubjectGroupRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.SubjectGroupList = Common.PopulateDllList(ddlList);

            #endregion

            #region Country ddl

            ddlList = _prmCommonService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(ddlList);

            #endregion
        }

        private void PopulateDropdownListJOBEXP(JobExperienceInfoViewModel model)
        {
            dynamic ddlList;

            #region Exam/Level ddl

            ddlList = _empService.PRMUnit.EmploymentTypeRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.EmployeeTypeList = Common.PopulateDllList(ddlList);

            #endregion

            #region Result ddl

            ddlList = _empService.PRMUnit.OrganizationTypeRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            model.OrganizationTypeList = Common.PopulateDllList(ddlList);

            #endregion

            #region Grade ddl
            model.JobGradeList = Common.PopulateJobGradeDDL(_JobGradeService.GetLatestJobGrade());
            #endregion

            #region Security Organization ddl
            model.SecurityOrganizationList = Common.PopulateDllList(_amsCommonService.AMSUnit.SecurityOrganizationRepository.Fetch().OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList());
            #endregion
        }

        private void PopulateDropdownListServiceHistory(SecurityServiceHistoryViewModel model)
        {
            #region Zone ddl

            var list = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x => x.ZoneName).ToList();
            model.ZoneList = list.Select(y =>
                                                    new SelectListItem()
                                                    {
                                                        Text = y.ZoneName,
                                                        Value = y.Id.ToString()
                                                    }).ToList(); ;

            #endregion

        }

        #endregion

        #endregion

        #region Business Logic

        private string CheckEmpInfoBusinessRule(SecurityPersonnelEmpInfoViewModel model)
        {
            string message = string.Empty;
            dynamic securityInfo = null;

            if (model.Id > 0)
            {
                securityInfo = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == model.EmployeeId && x.Id != model.Id).FirstOrDefault();
            }
            else
            {
                securityInfo = _amsCommonService.AMSUnit.SecurityInfoRepository.Get(x => x.EmployeeId == model.EmployeeId).FirstOrDefault();
            }

            if (securityInfo != null)
            {
                message += "Already exists.";
                return message;
            }

            if (model.DateofPromotion != null && (model.DateofPromotion < model.DateofJoining || model.DateofPromotion > DateTime.Now))
            {
                message += "Date of Promotion can not be earlier than joining date or later than current date.";
            }


            return message;
        }

        private string CheckSecurityServiceHistoryBusinessRule(SecurityServiceHistoryViewModel model)
        {
            string message = string.Empty;

            if (model.PeriodTo != null && (model.PeriodFrom > model.PeriodTo || model.PeriodTo > DateTime.Now))
            {
                message += "The field Period To must be greater than Period From and less than or equal to current date.";

                return message;
            }

            var dateOfJoining = _amsCommonService.AMSUnit.SecurityInfoRepository.GetAll().Where(x => x.Id == model.SecurityInfoId).FirstOrDefault().PRM_EmploymentInfo.DateofJoining;

            if (model.PeriodFrom < dateOfJoining)
            {
                message += "The field Period From must be equal or greater than the Date of Joining (" + dateOfJoining.ToString("dd-MMM-yyyy") + ").";

                return message;
            }

            if (ServiceHistoryDateRangeCheck(model.PeriodFrom, model.PeriodTo, model.Id, model.SecurityInfoId, model.strMode))
            {
                message += "Service period is not valid.";
                return message;
            }

            return message;
        }

        public bool ServiceHistoryDateRangeCheck(DateTime sDate, DateTime? eDate, int serviceHistoryID, int securityInfoID, string strMode)
        {
            bool rv = false;

            if (sDate == null)
            {
                eDate = DateTime.Now;
            }

            if (strMode == "add")
            {
                rv = _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.Fetch().Where(
                          x => (x.SecurityInfoId == securityInfoID) &&
                              (
                                  (x.PeriodFrom <= sDate && sDate <= (x.PeriodTo ?? DateTime.Now)) ||
                                  (x.PeriodFrom <= eDate && eDate <= (x.PeriodTo ?? DateTime.Now)) ||
                                  (sDate < x.PeriodFrom && (x.PeriodTo ?? DateTime.Now) < eDate))
                              ).Any();
            }
            else
            {
                rv = _amsCommonService.AMSUnit.SecurityServiceHistoryRepository.Fetch().Where(
                          x => (x.SecurityInfoId == securityInfoID && serviceHistoryID != x.Id) &&
                              (
                                  (x.PeriodFrom <= sDate && sDate <= (x.PeriodTo ?? DateTime.Now))
                                  || (x.PeriodFrom <= eDate && eDate <= (x.PeriodTo ?? DateTime.Now))
                                  || (sDate < x.PeriodFrom && (x.PeriodTo ?? DateTime.Now) < eDate))
                              ).Any();
            }

            return rv;
        }

        #endregion






    }
}
