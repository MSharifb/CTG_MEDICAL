using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Data;
using System.Text;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Web.Helpers;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;

using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Personal;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.DAL.PRM;

using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Web.Controllers;
using System.Data.SqlClient;
using BEPZA_MEDICAL.Web.SecurityService;

/*
Revision History (RH):
		SL		: 01
		Author	: AMN
		Date	: 2015-Apr-13
        SCR     : ERP_BEPZA_PRM_SCR.doc (SCR#157)
		Change	: Calculate leave availed duration Except Study Leave(SDL) (deduct if leave is availed except study leave)
		---------
*/

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class PersonalInfoController : BaseController
    {
        #region Fields

        private readonly PGMCommonService _pgmCommonservice;
        private readonly PRMCommonSevice _PRMService;
        private readonly EmployeeService _empService;
        private readonly PersonalInfoService _personalInfoService;
        private readonly JobGradeService _JobGradeService;
        private UserManagementServiceClient _userAgent;
        #endregion

        #region Constructor

        public PersonalInfoController(EmployeeService empService, PersonalInfoService personalInfoService, PGMCommonService pgmCommonService, PRMCommonSevice PRMService, JobGradeService jobGradeService)
        {
            this._empService = empService;
            this._personalInfoService = personalInfoService;
            this._pgmCommonservice = pgmCommonService;
            this._PRMService = PRMService;
            this._JobGradeService = jobGradeService;
            _userAgent = new UserManagementServiceClient();
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            var model = new EmployeeSearchViewModel();
            model.ActionName = "PersonaInfoIndex";

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EmployeeSearchViewModel viewModel)
        {

            string filterExpression = String.Empty, LoginEmpId = "";
            int totalRecords = 0;
            var st = "";
            if (string.IsNullOrEmpty(st))
                st = "";

            viewModel.ZoneInfoId = LoggedUserZoneInfoId;

            #region User Own Info
            var groupList = _userAgent.GetGroupList();
            var userInfo = _userAgent.GetUserByLoginId(MyAppSession.User.LoginId);
            var groupName = groupList.Where(s => s.GroupId == userInfo.GroupId).Select(x => x.GroupName).FirstOrDefault();

            if (groupName.Contains("Admin"))
            {
                viewModel.EmpId = viewModel.EmpId;
            }
            else
            {
                viewModel.EmpId = MyAppSession.EmpId;
            }
            #endregion

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

        #region PersonaInfo

        public ActionResult PersonaInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "PersonalInfo";

            var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(id ?? 0);


            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.PersonalInfoRepository.GetByID(id, "EmployeeId");
                var model = parentModel.PersonalInfo;
                if (entity == null)
                {
                    model.DateofBirth = empEntity.DateofBirth != null ? Convert.ToDateTime(empEntity.DateofBirth) : DateTime.Now;
                    model.PresentMobNo = empEntity.MobileNo;
                    model.Email = empEntity.EmialAddress;
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.ButtonText = "Save";
                }
                else
                {
                    model = entity.ToModel();
                    model.DeleteEnable = true;
                    model.strMode = "edit";
                    model.ButtonText = "Update";
                    model.DateofBirth = empEntity.DateofBirth != null ? Convert.ToDateTime(empEntity.DateofBirth) : DateTime.Now;
                    model.PresentMobNo = empEntity.MobileNo;
                    model.Email = empEntity.EmialAddress;
                }

                parentModel.EmployeeId = id.Value;
                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }

                parentModel.PersonalInfo = model;
                PopulateDropdownList(model);

            }
            parentModel.PersonalInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.PersonalInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                parentModel.PersonalInfo.errClass = "success";

            }
            return View("CreateOrEditPersonaInfo", parentModel);

        }

        [HttpPost]
        public ActionResult CreateOrEditPersonaInfo(PersonalInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    parentModel.ViewType = "PersonalInfo";
                    parentModel.EmployeeId = model.EmployeeId;
                    Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                    if (ModelState.IsValid)
                    {
                        var entity = model.ToEntity();
                        businessError = _personalInfoService.CheckBusinessLogic(entity);
                        entity.EUser = User.Identity.Name;
                        entity.EDate = Common.CurrentDateTime;

                        if (businessError != string.Empty)
                        {
                            model.Message = businessError;
                            model.errClass = "failed";
                            if (model.strMode == "add")
                            {
                                model.ButtonText = "Save";
                                model.DeleteEnable = false;
                            }
                            else
                            {
                                model.ButtonText = "Update";
                                model.DeleteEnable = true;
                            }
                            parentModel.PersonalInfo = model;

                            PopulateDropdownList(model);
                            parentModel.PersonalInfo.SideBarClassName = "selected";
                            return View("CreateOrEditPersonaInfo", parentModel);
                        }

                        if (model.strMode == "add")
                        {
                            entity.IUser = User.Identity.Name;
                            entity.IDate = Common.CurrentDateTime;
                            _personalInfoService.PRMUnit.PersonalInfoRepository.Add(entity);
                            model.Message = Resources.ErrorMessages.InsertSuccessful;
                            model.errClass = "success";
                            model.ButtonText = "Update";
                            model.strMode = "edit";
                            model.DeleteEnable = true;

                        }
                        else
                        {
                            _personalInfoService.PRMUnit.PersonalInfoRepository.Update(entity, "EmployeeId");

                            model.Message = Resources.ErrorMessages.UpdateSuccessful;
                            model.errClass = "success";
                            model.DeleteEnable = true;
                            model.strMode = "edit";
                            model.ButtonText = "Update";
                        }
                        _personalInfoService.PRMUnit.PersonalInfoRepository.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", Common.ValidationSummaryHead);
                        model.errClass = "failed";
                    }

                    parentModel.PersonalInfo = model;
                    PopulateDropdownList(model);
                    parentModel.PersonalInfo.SideBarClassName = "selected";
                    return View("CreateOrEditPersonaInfo", parentModel);
                }
            }
            catch
            {
                PopulateDropdownList(model);
                if (model.strMode == "add")
                {
                    model.Message = Resources.ErrorMessages.InsertFailed;
                    model.ButtonText = "Save";
                    model.errClass = "failed";
                    model.DeleteEnable = false;
                }
                else if (model.strMode == "edit")
                {
                    model.Message = Resources.ErrorMessages.UpdateFailed;
                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.errClass = "failed";
                }
                //return View("CreateOrEditPersonaInfo", model);
            }
            parentModel.PersonalInfo.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        public ActionResult Delete(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "PersonalInfo";

            var entity = _personalInfoService.PRMUnit.PersonalInfoRepository.GetByID(id, "EmployeeId");
            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && model != null)
                {
                    _personalInfoService.PRMUnit.PersonalInfoRepository.Delete(entity);
                    _personalInfoService.PRMUnit.PersonalInfoRepository.SaveChanges();

                    PopulateDropdownList(model);
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";
                    model.ButtonText = "Save";
                    return RedirectToAction("PersonaInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
            }
            catch
            {
                PopulateDropdownList(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";
                model.ButtonText = "Update";
            }

            parentModel.PersonalInfo = model;
            parentModel.PersonalInfo.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Accademic Qualification Info
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAllAccademicQlfnInfoList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
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
            var parentModel = new PersonalViewModel();
            var model = parentModel.AccademicQlfnInfo;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.AccademicQualificationRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
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

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;
                parentModel.ViewType = "AccademicQlfnInfo";
                if (empEntity != null)
                {
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

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditAccademicQlfnInfo(AccademicQlfnInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = model.EmployeeId;
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.AccademicQualificationRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        _personalInfoService.PRMUnit.AccademicQualificationRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.AccademicQualificationRepository.SaveChanges();
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

                return View("CreateOrEditPersonaInfo", parentModel);
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

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteAccademicQlfnInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "AccademicQlfnInfo";

            var entity = _personalInfoService.PRMUnit.AccademicQualificationRepository.GetByID(id);
            dynamic model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.AccademicQualificationRepository.Delete(entity);

                    _personalInfoService.PRMUnit.PersonalInfoRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";
                    return RedirectToAction("AccademicQlfnInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownList(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";
                    parentModel.AccademicQlfnInfo = model;
                }
            }
            catch
            {
                PopulateDropdownList(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.AccademicQlfnInfo = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        #endregion

        #region Job Experience Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetJobExperienceList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
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

            if (request.SortingName == "EmploymentType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmploymentType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmploymentType).ToList();
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
                    item.Designation,
                    item.EmploymentType,
                    item.OrganizationType,
                    Convert.ToDateTime(item.FromDate).ToString("dd-MMM-yyyy"),                    
                    getEndDateOfCurrentJob(item.Id,item.Organization1, item.Designation, item.EndDate, Id).ToString("dd-MMM-yyyy"),
                    CalculateDuration(item.Id,item.Organization1,item.FromDate, item.EndDate, Id)
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        private decimal CalculateDuration(int Id, string organization, DateTime FromDate, DateTime ToDate, int empId)
        {
            decimal duration = 0;
            DateTime startDate = FromDate;
            DateTime endDate = ToDate;

            string empOrganiztion = organization;
            int maxId = Id;
            decimal diffMonths = 0;
            empOrganiztion = _empService.PRMUnit.CompanyInformation.GetAll().FirstOrDefault().CompanyName.ToLower();

            var exp = (from tr in _empService.PRMUnit.JobExperienceInfoRepository.GetAll()
                       where tr.EmployeeId == empId && (tr.Organization1.ToLower().Trim() == empOrganiztion.ToLower().Trim()
                       || tr.Organization1.ToLower().Trim() == "iwm"
                       || tr.Organization1.ToLower().Trim() == "institute of water modelling")
                       select tr).OrderBy(x => x.EndDate).LastOrDefault();

            if (exp != null)
            {
                maxId = exp.Id;
            }

            if ((organization.ToLower().Trim() == empOrganiztion.ToLower().Trim()
                || organization.ToLower().Trim() == "iwm"
                || organization.ToLower().Trim() == "institute of water modelling") && maxId == Id)
            {
                diffMonths = (System.DateTime.Now.Month + System.DateTime.Now.Year * 12) - (startDate.Month + startDate.Year * 12) + 1;
                duration = Math.Round(diffMonths / 12, 2);
            }
            else
            {
                diffMonths = (endDate.Month + endDate.Year * 12) - (startDate.Month + startDate.Year * 12) + 1;
                duration = Math.Round(diffMonths / 12, 2);
            }

            /*RH#01*/
            decimal LeaveDuration = CalculateLeaveDuration(empId, FromDate, ToDate);
            /*End RH#01*/

            return duration - LeaveDuration;
        }

        /// <summary>
        /// /*RH#01*/
        /// </summary>
        private decimal CalculateLeaveDuration(Int32 empId, DateTime FromDate, DateTime ToDate)
        {
            string strEmpId = empId.ToString().PadLeft(4, '0');

            var q = from LA in _pgmCommonservice.PGMUnit.LeaveApplication.GetAll()
                    join LT in _pgmCommonservice.PGMUnit.LeaveType.GetAll() on LA.intLeaveTypeID equals LT.intLeaveTypeID
                    where LA.strEmpID == strEmpId
                          && !LT.strLeaveShortName.Equals("SDL") /*Not consider study leave*/
                          && (LA.dtApplyDate >= FromDate && LA.dtApplyDate <= ToDate)
                    select LA;

            decimal totalLeave = Convert.ToDecimal(q.Sum(t => t.fltWithoutPayDuration));

            // Convert leave day to year
            totalLeave = Math.Round(totalLeave / 30 / 12, 2);

            return totalLeave;
        }

        private DateTime getEndDateOfCurrentJob(int Id, string organization, string designation, DateTime ToDate, int empId)
        {
            DateTime endDate = ToDate;
            string empOrganiztion = organization;
            string empDesignation = designation;
            int maxId = Id;

            var emp = _empService.PRMUnit.EmploymentInfoRepository.GetByID(empId);

            if (emp != null)
            {
                empOrganiztion = _empService.PRMUnit.CompanyInformation.GetAll().FirstOrDefault().CompanyName.ToLower();
                empDesignation = _empService.PRMUnit.DesignationRepository.GetByID(emp.DesignationId).Name;

                var exp = (from tr in _empService.PRMUnit.JobExperienceInfoRepository.GetAll()
                           where tr.EmployeeId == empId && (tr.Organization1.ToLower().Trim() == empOrganiztion.ToLower().Trim()
                           || tr.Organization1.ToLower().Trim() == "iwm"
                           || tr.Organization1.ToLower().Trim() == "institute of water modelling")
                           select tr).OrderBy(x => x.EndDate).LastOrDefault();

                if (exp != null)
                {
                    maxId = exp.Id;
                }

                if ((organization.ToLower().Trim() == empOrganiztion.ToLower().Trim()
                    || organization.ToLower().Trim() == "iwm"
                    || organization.ToLower().Trim() == "institute of water modelling")
                    && designation.ToLower().Trim() == empDesignation.ToLower().Trim()
                    && maxId == Id)
                {
                    if (emp.EmploymentStatusId == 1)
                    {
                        endDate = System.DateTime.Now;
                    }
                    else
                    {
                        endDate = Convert.ToDateTime(emp.DateofInactive);
                    }
                }
            }
            return endDate;
        }

        public ActionResult JobExperienceInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.JobExperienceInfo;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.JobExperienceInfoRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        entity.EndDate = getEndDateOfCurrentJob(entity.Id, entity.Organization1, entity.Designation, entity.EndDate, Convert.ToInt16(entity.EmployeeId));
                        model = entity.ToModel();
                        model.Duration = CalculateDuration(entity.Id, entity.Organization1, entity.FromDate, entity.EndDate, entity.EmployeeId);
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;

                parentModel.ViewType = "JobExperienceInfo";

                if (empEntity != null)
                {
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

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditJobExperienceInfo(JobExperienceInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.JobExperienceInfoRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        _personalInfoService.PRMUnit.JobExperienceInfoRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.JobExperienceInfoRepository.SaveChanges();
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

                return View("CreateOrEditPersonaInfo", parentModel);
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
                //if (ex.InnerException.Message.Contains("IX_PRM_EmpExperience"))
                //{
                //    model.Message = Resources.ErrorMessages.UniqueIndex;
                //    model.errClass = "failed";
                //}
                parentModel.ViewType = "JobExperienceInfo";
                parentModel.JobExperienceInfo = model;
                parentModel.JobExperienceInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }
        public ActionResult DeleteJobExperienceInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "JobExperienceInfo";

            var entity = _personalInfoService.PRMUnit.JobExperienceInfoRepository.GetByID(id);

            dynamic model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.JobExperienceInfoRepository.Delete(entity);

                    _personalInfoService.PRMUnit.JobExperienceInfoRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("JobExperienceInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
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

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        #endregion

        #region Professional Training Information
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetProfessionalTrainingList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllProfessionalTrainingInfoByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).OrderBy(x => x.ToDate).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "TrainingType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TrainingType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TrainingType).ToList();
                }
            }

            if (request.SortingName == "TrainingTitle")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TrainingTitle).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TrainingTitle).ToList();
                }
            }

            if (request.SortingName == "OrganizedBy")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OrganizedBy).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OrganizedBy).ToList();
                }
            }

            if (request.SortingName == "Country")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Country).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Country).ToList();
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
            if (request.SortingName == "ToDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ToDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ToDate).ToList();
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
                    item.TrainingTitle,
                    item.OrganizedBy,
                    item.TrainingType,                                    
                    item.Country, 
                    item.FromDate!=null?Convert.ToDateTime(item.FromDate).ToString("dd-MMM-yyyy"):string.Empty,
                    item.ToDate!=null?Convert.ToDateTime(item.ToDate).ToString("dd-MMM-yyyy"):string.Empty,
                    item.Duration
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ProfessionalTrainingInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.ProfessionalTrainingInfo;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.ProfessionalTrainingRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
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

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;

                parentModel.ViewType = "ProfessionalTraining";

                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }
            }

            parentModel.ProfessionalTrainingInfo = model;
            PopulateDropdownListPROTRA(model);
            parentModel.ProfessionalTrainingInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.ProfessionalTrainingInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditProfessionalTrainingInfo(ProfessionalTrainingInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    businessError = _personalInfoService.CheckBusinessLogicPROTRA(entity, model.strMode);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "ProfessionalTraining";
                        parentModel.ProfessionalTrainingInfo = model;
                        PopulateDropdownListPROTRA(model);
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.ProfessionalTrainingRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        _personalInfoService.PRMUnit.ProfessionalTrainingRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.ProfessionalTrainingRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListPROTRA(model);

                parentModel.ViewType = "ProfessionalTraining";
                parentModel.ProfessionalTrainingInfo = model;
                parentModel.ProfessionalTrainingInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListPROTRA(model);

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
                //if (ex.InnerException.Message.Contains("IX_PRM_EmpTrainingInfo"))
                //{
                //    model.Message = Resources.ErrorMessages.UniqueIndex;
                //    model.errClass = "failed";
                //}


                parentModel.ViewType = "ProfessionalTraining";
                parentModel.ProfessionalTrainingInfo = model;
                parentModel.ProfessionalTrainingInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteProfessionalTrainingInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "ProfessionalTraining";

            var entity = _personalInfoService.PRMUnit.ProfessionalTrainingRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.ProfessionalTrainingRepository.Delete(entity);

                    _personalInfoService.PRMUnit.ProfessionalTrainingRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("ProfessionalTrainingInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListPROTRA(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.ProfessionalTrainingInfo = model;
                }

            }
            catch
            {
                PopulateDropdownListPROTRA(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.ProfessionalTrainingInfo = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        #endregion

        #region Professional Certification Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetProfessionalCertificationList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllCertificationInfoByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "CertificationCatagory")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CertificationCatagory).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CertificationCatagory).ToList();
                }
            }
            if (request.SortingName == "CertificationTitle")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CertificationTitle).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CertificationTitle).ToList();
                }
            }

            if (request.SortingName == "CertificationInstitute")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CertificationInstitute).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CertificationInstitute).ToList();
                }
            }

            if (request.SortingName == "Country")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Country).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Country).ToList();
                }
            }

            if (request.SortingName == "CertificationYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CertificationYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CertificationYear).ToList();
                }
            }

            if (request.SortingName == "Result")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Result).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Result).ToList();
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
                    item.CertificationCatagory,
                    item.CertificationTitle,
                    item.CertificationInstitute,
                    item.Country,
                    item.CertificationYear,
                    item.Result
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ProfessionalCertificationInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.ProfessionalCertificationInfo;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.CertificationRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
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

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;

                parentModel.ViewType = "ProfessionalCertification";

                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }
            }

            parentModel.ProfessionalCertificationInfo = model;
            PopulateDropdownListPROCER(model);
            parentModel.ProfessionalCertificationInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.ProfessionalCertificationInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditProfessionalCertificationInfo(ProfessionalCertificationInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    businessError = _personalInfoService.CheckBusinessLogicPROCER(entity);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "ProfessionalCertification";
                        parentModel.ProfessionalCertificationInfo = model;
                        PopulateDropdownListPROCER(model);
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.CertificationRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        _personalInfoService.PRMUnit.CertificationRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.CertificationRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListPROCER(model);

                parentModel.ViewType = "ProfessionalCertification";
                parentModel.ProfessionalCertificationInfo = model;
                parentModel.ProfessionalCertificationInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListPROCER(model);

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
                if (ex.InnerException.Message.Contains("IX_PRM_EmpCertification"))
                {
                    model.Message = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                }
                parentModel.ViewType = "ProfessionalCertification";
                parentModel.ProfessionalCertificationInfo = model;
                parentModel.ProfessionalCertificationInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteProfessionalCertificationInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "ProfessionalCertification";

            var entity = _personalInfoService.PRMUnit.CertificationRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.CertificationRepository.Delete(entity);

                    _personalInfoService.PRMUnit.CertificationRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("ProfessionalCertificationInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListPROCER(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.ProfessionalCertificationInfo = model;
                }

            }
            catch
            {
                PopulateDropdownListPROCER(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.ProfessionalCertificationInfo = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        #endregion

        #region Professional License Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetProfessionalLicenseList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllLicenseInfoByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "LicenseType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.LicenseType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.LicenseType).ToList();
                }
            }
            if (request.SortingName == "LicenseNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.LicenseNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.LicenseNo).ToList();
                }
            }

            if (request.SortingName == "Institute")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Institute).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Institute).ToList();
                }
            }

            if (request.SortingName == "Country")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Country).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Country).ToList();
                }
            }

            if (request.SortingName == "PermitDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PermitDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PermitDate).ToList();
                }
            }

            if (request.SortingName == "ExpireDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ExpireDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ExpireDate).ToList();
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
                    item.LicenseType,
                    item.LicenseNo,
                    item.Institute,
                    item.Country,
                    item.PermitDate==null ? null:Convert.ToDateTime (item.PermitDate).ToString("dd-MMM-yyyy"),
                    item.ExpireDate==null ? null:Convert.ToDateTime (item.ExpireDate).ToString("dd-MMM-yyyy")                   
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ProfessionalLicenseInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.ProfessionalLicenseInfo;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.LicenseRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
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

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;

                parentModel.ViewType = "ProfessionalLicense";

                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }
            }

            parentModel.ProfessionalLicenseInfo = model;
            PopulateDropdownListLICEN(model);
            parentModel.ProfessionalLicenseInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.ProfessionalLicenseInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditProfessionalLicenseInfo(ProfessionalLicenseInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    businessError = _personalInfoService.CheckBusinessLogicLICEN(entity);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "ProfessionalLicense";
                        parentModel.ProfessionalLicenseInfo = model;
                        PopulateDropdownListLICEN(model);
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.LicenseRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        _personalInfoService.PRMUnit.LicenseRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.LicenseRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListLICEN(model);

                parentModel.ViewType = "ProfessionalLicense";
                parentModel.ProfessionalLicenseInfo = model;
                parentModel.ProfessionalLicenseInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListLICEN(model);

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
                if (ex.InnerException.Message.Contains("IX_PRM_EmpLicense"))
                {
                    model.Message = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                }
                parentModel.ViewType = "ProfessionalLicense";
                parentModel.ProfessionalLicenseInfo = model;
                parentModel.ProfessionalLicenseInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteProfessionalLicenseInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "ProfessionalLicense";

            var entity = _personalInfoService.PRMUnit.LicenseRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.LicenseRepository.Delete(entity);

                    _personalInfoService.PRMUnit.LicenseRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("ProfessionalLicenseInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListLICEN(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.ProfessionalLicenseInfo = model;
                }

            }
            catch
            {
                PopulateDropdownListLICEN(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.ProfessionalLicenseInfo = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        #endregion

        #region JobSkill Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetJobSkillList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllGetJobSkillInfoByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "SkillName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SkillName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SkillName).ToList();
                }
            }
            if (request.SortingName == "EfficiencyLevel")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EfficiencyLevel).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EfficiencyLevel).ToList();
                }
            }

            if (request.SortingName == "YearofExperience")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.YearofExperience).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.YearofExperience).ToList();
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
                    item.SkillName,
                    item.EfficiencyLevel,
                    item.YearofExperience
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult JobSkillInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.JobSkillInfo;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.JobSkillRepository.GetByID(id.Value);
                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
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

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;

                parentModel.ViewType = "JobSkill";

                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }

            }

            parentModel.JobSkillInfo = model;
            PopulateDropdownListJOBSKIL(model);
            parentModel.JobSkillInfo.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.JobSkillInfo.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditJobSkillInfo(JobSkillInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    businessError = _personalInfoService.CheckBusinessLogicJOBSKL(entity);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "JobSkill";
                        parentModel.JobSkillInfo = model;
                        PopulateDropdownListJOBSKIL(model);
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.JobSkillRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        _personalInfoService.PRMUnit.JobSkillRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.LicenseRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListJOBSKIL(model);

                parentModel.ViewType = "JobSkill";
                parentModel.JobSkillInfo = model;
                parentModel.JobSkillInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListJOBSKIL(model);

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
                if (ex.InnerException.Message.Contains("IX_PRM_EmpJobSkill"))
                {
                    model.Message = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                }
                parentModel.ViewType = "JobSkill";
                parentModel.JobSkillInfo = model;
                parentModel.JobSkillInfo.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteJobSkillInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "JobSkill";

            var entity = _personalInfoService.PRMUnit.JobSkillRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.JobSkillRepository.Delete(entity);

                    _personalInfoService.PRMUnit.LicenseRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("JobSkillInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListJOBSKIL(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.JobSkillInfo = model;
                }

            }
            catch
            {
                PopulateDropdownListJOBSKIL(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.JobSkillInfo = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        #endregion

        #region Search DDL

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _empService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _empService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Id).ToList();

            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        public ActionResult LoadDistrict(int countryId)
        {
            var list = _personalInfoService.PopulateDistrictByCountryID(countryId).Select(x => new { Id = x.Id, DistrictName = x.DistrictName }).OrderBy(x => x.DistrictName).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult LoadThana(int districtId)
        {
            var list = _personalInfoService.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == districtId).Select(x => new { Id = x.Id, Name = x.ThanaName }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
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
            //var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.GradeName).ToList();
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
        public ActionResult GetResource()
        {
            var grades = _empService.PRMUnit.ResourceLevelRepository.GetAll().OrderBy(x => x.SortOrder).ToList();

            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().OrderBy(x => x.SortOrder).ToList();

            return PartialView("Select", Common.PopulateDllList(empStatus));
        }

        #endregion

        #region Personal Publication

        public ActionResult PersonalPublicationIndex(int id)
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.PersonalPublication;
            // var viewModel = new PersonalPublicationViewModel();
            model.EmployeeId = id;
            PopulatePersonalPublicationDropdownList(model);
            parentModel.EmployeeId = id;
            parentModel.ViewType = "Publication";
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            parentModel.PersonalPublication = model;
            parentModel.PersonalPublication.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        public string GetFilterExpression(int EmployeeId)
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (EmployeeId > 0)
                filterExpressionBuilder.Append(String.Format("EmployeeId = {0} AND ", EmployeeId));
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }

        public ActionResult GetPersonalPublicationList(JqGridRequest request, int id)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            try
            {
                filterExpression = GetFilterExpression(id);


                totalRecords = _empService.PRMUnit.EmployeePublication.GetCount(filterExpression);

                JqGridResponse response = new JqGridResponse()
                {
                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                    PageIndex = request.PageIndex,
                    TotalRecordsCount = totalRecords
                };

                var list = _empService.PRMUnit.EmployeePublication.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1).OrderBy(x => x.PublicationDate).ToList();

                #region Sorting

                if (request.SortingName == "Publicatoin")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PublicationName).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PublicationName).ToList();
                    }
                }
                if (request.SortingName == "PublicationArea")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PRM_PublicationArea.Name).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PRM_PublicationArea.Name).ToList();
                    }
                }

                if (request.SortingName == "Journal")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.JournalName).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.JournalName).ToList();
                    }
                }

                if (request.SortingName == "PublicationDate")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PublicationDate).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PublicationDate).ToList();
                    }
                }

                if (request.SortingName == "SerialNo")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.SerialNo).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.SerialNo).ToList();
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

                var levelList = _empService.PRMUnit.ProfficiencyLevel.GetAll();
                foreach (PRM_EmpPublicationInfo obj in list)
                {
                    response.Records.Add(new JqGridRecord(Convert.ToString(obj.Id), new List<object>()
                    {
                        obj.Id,
                        obj.PublicationName,
                        _empService.PRMUnit.PublicationArea.GetByID(obj.PublicationAreaId).Name,
                        obj.JournalName,
                        obj.PublicationDate==null?null: Convert.ToDateTime ( obj.PublicationDate).ToString("dd-MMM-yyyy"),
                        obj.SerialNo,
                        obj.Remarks
                    }));
                }
                return new JqGridJsonResult() { Data = response };
            }
            catch (Exception ex)
            {
                return new JqGridJsonResult() { };
            }

        }

        [HttpPost]
        public JsonResult PersonalPublicationCreate(PersonalPublicationViewModel model)
        {
            try
            {
                if (CheckBusinessRules(model).IsSuccessful)
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = DateTime.Now;
                    _empService.PRMUnit.EmployeePublication.Add(entity);
                    _empService.PRMUnit.EmployeePublication.SaveChanges();
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    model.IsSuccessful = true;
                }

            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }
            return Json(new
            {
                Success = model.IsSuccessful,
                Message = model.Message
            });

        }

        [HttpPost]
        public JsonResult GetSelectedPublicationData(int id)
        {
            var data = _empService.PRMUnit.EmployeePublication.GetByID(id);
            return Json(new
            {
                data.Id,
                data.PublicationAreaId,
                PublicationDate = data.PublicationDate == null ? null : Convert.ToDateTime(data.PublicationDate).ToString("dd-MM-yyyy"),
                data.PublicationName,
                data.Remarks,
                data.SerialNo,
                data.JournalName,
                data.CountryId
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PersonalPublicationEdit(PersonalPublicationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckBusinessRules(model).IsSuccessful)
                    {
                        var entity = model.ToEntity();
                        entity.IUser = User.Identity.Name;
                        entity.IDate = DateTime.Now;
                        entity.EUser = User.Identity.Name;
                        entity.EDate = DateTime.Now;
                        _empService.PRMUnit.EmployeePublication.Update(entity);
                        _empService.PRMUnit.EmployeePublication.SaveChanges();
                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        model.IsSuccessful = true;
                    }
                }

            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }
            return Json(new
            {
                Success = model.IsSuccessful,
                Message = model.Message
            });
        }

        [HttpPost]
        public JsonResult DeletePublication(PersonalPublicationViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _empService.PRMUnit.EmployeePublication.Delete(viewModel.Id);
                    _empService.PRMUnit.EmployeePublication.SaveChanges();
                    viewModel.Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                    viewModel.IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }
            return Json(new
            {
                Success = viewModel.IsSuccessful,
                Message = viewModel.Message
            });
        }

        #region Private Methods

        private List<PRM_EmpPublicationInfo> GetEmployeePublicatonInformation(int employeeID)
        {
            List<PRM_EmpPublicationInfo> list = new List<PRM_EmpPublicationInfo>();
            list = (from c in _empService.PRMUnit.EmployeePublication.Fetch()
                    where c.EmployeeId == employeeID
                    select c).ToList();
            return list;
        }
        private PersonalPublicationViewModel CheckBusinessRules(PersonalPublicationViewModel model)
        {
            model.IsSuccessful = true;

            if (string.IsNullOrEmpty(model.SerialNo))
            {
                model.IsSuccessful = false;
                model.Message = "please enter serial number.";
            }
            //var person = _empService.PRMUnit.PersonalInfoRepository.GetByID(model.EmployeeId, "EmployeeId");
            //var emp = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            //if (person != null && emp != null)
            //{
            //    if (emp.DateofBirth > Convert.ToDateTime(model.PublicationDate))
            //    {
            //        model.IsSuccessful = false;
            //        model.Message = "Date of publication must be greater then the date of birth";
            //    }
            //    else
            //    {
            //        model.IsSuccessful = true;
            //    }
            //}

            return model;
        }
        private void PopulatePersonalPublicationDropdownList(PersonalPublicationViewModel model)
        {
            dynamic ddlList;

            #region Country ddl
            ddlList = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(ddlList);
            #endregion

            #region Publication ddl
            ddlList = _empService.PRMUnit.PublicationArea.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.PublicationAreaList = Common.PopulateDllList(ddlList);
            #endregion

        }

        #endregion

        #endregion

        #region Personal Language Efficiency

        public ActionResult LanguageProficiencyIndex(int id, string message)
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.PersonalLanguageEfficiency;
            model = InitializeLanguageEfficiencyViewModel(id);
            model.EmployeeId = id;
            if (message != "")
            {
                model.Message = message;
                model.IsSuccessful = true;
            }
            parentModel.EmployeeId = id;
            parentModel.ViewType = "Language";
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            parentModel.PersonalLanguageEfficiency = model;
            parentModel.PersonalLanguageEfficiency.SideBarClassName = "selected";

            model.ActionType = "CreatePersonalLanguageEfficiency";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetPersonalLanguageEfficiencyList(JqGridRequest request, int id)
        {
            IList<PRM_EmpLanguageEfficiency> list = new List<PRM_EmpLanguageEfficiency>();
            string filterExpression = String.Empty;
            int totalRecords = 0;
            try
            {
                int startRowIndex = request.PageIndex * request.RecordsCount + 1;
                list = GetEmployeeLanguageEfficiency(id);

                #region Sorting

                if (request.SortingName == "Language")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PRM_Language.Name).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PRM_Language.Name).ToList();
                    }
                }
                if (request.SortingName == "SpeakingEfficiencyId")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PRM_ProefficiencyLevel.Name).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PRM_ProefficiencyLevel.Name).ToList();
                    }
                }

                if (request.SortingName == "ListeningEfficiencyId")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PRM_ProefficiencyLevel1.Name).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PRM_ProefficiencyLevel1.Name).ToList();
                    }
                }

                if (request.SortingName == "WrittingEfficiencyId")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PRM_ProefficiencyLevel2.Name).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PRM_ProefficiencyLevel2.Name).ToList();
                    }
                }

                if (request.SortingName == "ReadingEfficiencyId")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PRM_ProefficiencyLevel3.Name).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PRM_ProefficiencyLevel3.Name).ToList();
                    }
                }

                if (request.SortingName == "Native")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.IsNative).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.IsNative).ToList();
                    }
                }
                #endregion
                JqGridResponse response = new JqGridResponse()
                {
                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                    PageIndex = request.PageIndex,
                    TotalRecordsCount = totalRecords
                };

                var levelList = _empService.PRMUnit.ProfficiencyLevel.GetAll();
                foreach (PRM_EmpLanguageEfficiency obj in list)
                {

                    response.Records.Add(new JqGridRecord(Convert.ToString(obj.Id), new List<object>()
                {
                    obj.Id,
                    _empService.PRMUnit.Language.GetByID(obj.LanguageId).Name,
                    levelList.Where(d=>d.Id==obj.SpeakingEfficiencyId).FirstOrDefault().Name,  
                    levelList.Where(d=>d.Id==obj.ListeningEfficiencyId).FirstOrDefault().Name,
                    levelList.Where(d=>d.Id==obj.WrittingEfficiencyId).FirstOrDefault().Name,
                    levelList.Where(d=>d.Id==obj.ReadingEfficiencyId).FirstOrDefault().Name,                          
                    obj.IsNative?"Yes":"No"
                }));
                }
                return new JqGridJsonResult() { Data = response };
            }
            catch (Exception ex)
            {
                return new JqGridJsonResult() { };
            }

        }

        [HttpPost]
        public JsonResult CreatePersonalLanguageEfficiency(PersonalLanguageEfficiencyViewModel model)
        {
            try
            {
                if (model.IsNative)
                {
                    if (!IsValidNativeLanguage(model.EmployeeId))
                    {
                        model.Message = "Only one Native language for an employee possible.";
                        model.IsSuccessful = false;
                        return Json(new
                        {
                            Success = model.IsSuccessful,
                            Message = model.Message
                        });
                    }
                }
                PRM_EmpLanguageEfficiency item = new PRM_EmpLanguageEfficiency();
                item.EmployeeId = model.EmployeeId;
                item.LanguageId = model.LanguageId;
                item.ListeningEfficiencyId = model.ListeningEfficiencyId;
                item.ReadingEfficiencyId = model.ReadingEfficiencyId;
                item.SpeakingEfficiencyId = model.SpeakingEfficiencyId;
                item.WrittingEfficiencyId = model.WrittingEfficiencyId;
                item.IsNative = model.IsNative;
                item.IUser = User.Identity.Name;
                item.IDate = DateTime.Now;

                _empService.PRMUnit.EmployeeLanguageEfficiency.Add(item);
                _empService.PRMUnit.EmployeeLanguageEfficiency.SaveChanges();
                model = InitializeLanguageEfficiencyViewModel(model.EmployeeId);
                model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                model.IsSuccessful = true;

            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }
            return Json(new
            {
                Success = model.IsSuccessful,
                Message = model.Message
            });
        }

        public JsonResult EditPersonalLanguageEfficiency(int id)
        {
            // model.ActionType = CrudeAction.Edit;
            var data = _empService.PRMUnit.EmployeeLanguageEfficiency.GetByID(id);
            return Json(new
            {
                data.Id,
                data.LanguageId,
                data.IsNative,
                data.ListeningEfficiencyId,
                data.ReadingEfficiencyId,
                data.SpeakingEfficiencyId,
                data.WrittingEfficiencyId
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditPersonalLanguageEfficiency(PersonalLanguageEfficiencyViewModel model)
        {
            try
            {
                if (model.Id != null && model.Id != 0)
                {
                    if (model.IsNative)
                    {
                        var oldData = _empService.PRMUnit.EmployeeLanguageEfficiency.GetByID(model.Id);
                        if (!oldData.IsNative)
                        {
                            if (!IsValidNativeLanguage(model.EmployeeId))
                            {
                                model.Message = "Only one Native language for an employee";
                                model.IsSuccessful = false;
                                return Json(new
                                {
                                    Success = model.IsSuccessful,
                                    Message = model.Message
                                });
                            }
                        }
                    }
                    PRM_EmpLanguageEfficiency item = new PRM_EmpLanguageEfficiency();
                    item.Id = (int)model.Id;
                    item.EmployeeId = model.EmployeeId;
                    item.LanguageId = model.LanguageId;
                    item.ListeningEfficiencyId = model.ListeningEfficiencyId;
                    item.ReadingEfficiencyId = model.ReadingEfficiencyId;
                    item.SpeakingEfficiencyId = model.SpeakingEfficiencyId;
                    item.WrittingEfficiencyId = model.WrittingEfficiencyId;
                    item.IsNative = model.IsNative;
                    item.EUser = User.Identity.Name;
                    item.EDate = DateTime.Now;

                    _empService.PRMUnit.EmployeeLanguageEfficiency.Update(item);
                    _empService.PRMUnit.EmployeeLanguageEfficiency.SaveChanges();
                    model = InitializeLanguageEfficiencyViewModel(model.EmployeeId);
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    model.IsSuccessful = true;

                }

            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);

            }
            return Json(new
            {
                Success = model.IsSuccessful,
                Message = model.Message
            });
        }

        [HttpPost]
        public JsonResult DeleteConfirmforLanguageProficiency(PersonalLanguageEfficiencyViewModel viewModel)
        {
            try
            {
                var item = _empService.PRMUnit.EmployeeLanguageEfficiency.GetByID(viewModel.Id);
                _empService.PRMUnit.EmployeeLanguageEfficiency.Delete(item);
                _empService.PRMUnit.EmployeeLanguageEfficiency.SaveChanges();
                viewModel = InitializeLanguageEfficiencyViewModel(viewModel.EmployeeId);
                viewModel.Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                viewModel.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }
            return Json(new
            {
                Success = viewModel.IsSuccessful,
                Message = viewModel.Message
            });
        }

        private PersonalLanguageEfficiencyViewModel InitializeLanguageEfficiencyViewModel(int employeeID)
        {
            var viewModel = new PersonalLanguageEfficiencyViewModel();
            var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(employeeID);
            viewModel.EmployeeId = employeeID;
            if (empEntity != null)
            {
                viewModel.EmpID = empEntity.EmpID;
                viewModel.Name = empEntity.FullName;
                viewModel.EmployeeInitial = empEntity.EmployeeInitial;
                viewModel.Designation = _empService.PRMUnit.DesignationRepository.GetByID(empEntity.DesignationId).Name;
            }
            PopulateDropdownList(viewModel);
            return viewModel;

        }

        private void PopulateDropdownList(PersonalLanguageEfficiencyViewModel model)
        {
            dynamic ddlList;

            #region Language ddl

            ddlList = _empService.PRMUnit.Language.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.LanguageList = Common.PopulateDllList(ddlList);

            #endregion

            #region EfficiencyLevel for Reading/writing ddl

            ddlList = _empService.PRMUnit.ProfficiencyLevel.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.ListeningEfficiencyList = Common.PopulateDllList(ddlList);
            model.ReadingEfficiencyList = Common.PopulateDllList(ddlList);
            model.SpeakingEfficiencyList = Common.PopulateDllList(ddlList);
            model.WrittingEfficiencyList = Common.PopulateDllList(ddlList);

            #endregion
        }
        // Service Methods
        private List<PRM_EmpLanguageEfficiency> GetEmployeeLanguageEfficiency(int employeeID)
        {
            List<PRM_EmpLanguageEfficiency> list = new List<PRM_EmpLanguageEfficiency>();
            list = (from c in _empService.PRMUnit.EmployeeLanguageEfficiency.Fetch()
                    where c.EmployeeId == employeeID
                    select c).ToList();
            return list;
        }
        private bool IsValidNativeLanguage(int employeeID)
        {
            List<PRM_EmpLanguageEfficiency> list = GetEmployeeLanguageEfficiency(employeeID);
            if (list.Count > 0)
            {
                if (list.Where(d => d.IsNative == true).Count() > 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Personal Family Information

        public ActionResult PersonalFamilyInformationIndex(int id, string message)
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.PersonalFamilyMemberInformation;
            model.EmployeeId = id;
            model = InitializePersonalFamilyMemberViewModel(model);
            //model.EmployeeId = id;
            parentModel.EmployeeId = id;
            parentModel.ViewType = "FamilyMember";
            model.ActionType = "PersonalFamilyInformationCreate";
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            if (message != "")
            {
                model.Message = message;
                model.IsSuccessful = true;
            }

            if (model.Photo != null)
            {
                if (model.Photo.Length > 0)
                {
                    model.isAddPhoto = true;
                }
            }
            else
            {
                model.isAddPhoto = false;
            }

            parentModel.PersonalFamilyMemberInformation = model;
            parentModel.PersonalFamilyMemberInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetPersonalFamilyInformationList(JqGridRequest request, int id)
        {
            IList<PRM_EmpFamilyMemberInfo> list = new List<PRM_EmpFamilyMemberInfo>();
            string filterExpression = String.Empty;
            int totalRecords = 0;
            try
            {
                int startRowIndex = request.PageIndex * request.RecordsCount + 1;
                list = GetPersonalFamilyMemberInformation(id);

                #region Sorting

                if (request.SortingName == "FullName")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.FullName).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.FullName).ToList();
                    }
                }
                if (request.SortingName == "RelationId")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PRM_Relation.Name).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PRM_Relation.Name).ToList();
                    }
                }

                if (request.SortingName == "DateofBirth")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.DateofBirth).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.DateofBirth).ToList();
                    }
                }

                if (request.SortingName == "Gender")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.Gender).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.Gender).ToList();
                    }
                }

                if (request.SortingName == "BloodGroupId")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.PRM_BloodGroup.Name).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.PRM_BloodGroup.Name).ToList();
                    }
                }

                if (request.SortingName == "ContractNo")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.ContractNo).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.ContractNo).ToList();
                    }
                }
                #endregion

                JqGridResponse response = new JqGridResponse()
                {
                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                    PageIndex = request.PageIndex,
                    TotalRecordsCount = totalRecords
                };

                var levelList = _empService.PRMUnit.ProfficiencyLevel.GetAll();
                foreach (PRM_EmpFamilyMemberInfo obj in list)
                {

                    response.Records.Add(new JqGridRecord(Convert.ToString(obj.Id), new List<object>()
                {
                    obj.Id,
                    obj.FullName,
                    _empService.PRMUnit.Relation.GetByID(obj.RelationId).Name,
                    obj.DateofBirth.ToShortDateString(),
                    obj.Gender,
                   (obj.BloodGroupId==null)?"":( _empService.PRMUnit.BloodGroupRepository.GetByID(obj.BloodGroupId).Name),
                     obj.ContractNo,
                     "Delete"
                  
                }));
                }
                return new JqGridJsonResult() { Data = response };
            }
            catch (Exception ex)
            {
                return new JqGridJsonResult() { };
            }

        }

        [HttpPost]
        public ActionResult PersonalFamilyInformationCreate(PersonalFamilyMemberInformationViewModel viewModel, string btnSubmit)
        {
            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = viewModel.EmployeeId;
            parentModel.ViewType = "FamilyMember";
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);

            try
            {
                if (btnSubmit == "Upload")
                {
                    viewModel = UploadFamilyMemberPhoto(viewModel);
                }
                else if (btnSubmit == "Remove")
                {
                    viewModel = RemoveFamilyMemberPhoto(viewModel);
                }
                else if (btnSubmit == "Save")
                {
                    var entity = viewModel.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = DateTime.Now;
                    if (entity.Photo != null)
                    {
                        entity.isAddPhoto = true;
                    }
                    _empService.PRMUnit.PersonalFamilyInformation.Add(entity);
                    _empService.PRMUnit.PersonalFamilyInformation.SaveChanges();
                    viewModel.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    viewModel.IsSuccessful = true;
                    return RedirectToAction("PersonalFamilyInformationIndex", new { id = viewModel.EmployeeId, message = viewModel.Message });

                }
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);
            parentModel.PersonalFamilyMemberInformation = viewModel;
            parentModel.PersonalFamilyMemberInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        public ActionResult PersonalFamilyInformationEdit(int memberid, int employeeid)
        {
            var parentModel = new PersonalViewModel();
            var viewModel = parentModel.PersonalFamilyMemberInformation;

            var entity = _empService.PRMUnit.PersonalFamilyInformation.GetByID(memberid);
            viewModel = entity.ToModel();
            viewModel.EmployeeId = employeeid;
            if (entity.Witness1EmpId > 0)
            {
                var witness1Info = (from r in _PRMService.PRMUnit.EmploymentInfoRepository.GetAll()
                                    join d in _PRMService.PRMUnit.DesignationRepository.GetAll() on r.DesignationId equals d.Id
                                    join p in _PRMService.PRMUnit.PersonalInfoRepository.GetAll() on r.Id equals p.EmployeeId
                                    where r.Id == entity.Witness1EmpId
                                    select new { r.Id,r.EmpID, d.Name, r.FullName, p.PresentAddress1 }).Distinct().OrderBy(x => x.EmpID).FirstOrDefault();
                viewModel.Witness1EmpId = witness1Info.Id;
                viewModel.Witness1Id = witness1Info.EmpID;
                viewModel.Witness1Name = witness1Info.FullName;
                viewModel.Witness1Designation = witness1Info.Name;
                viewModel.Witness1Address = witness1Info.PresentAddress1;
            }
            if (entity.Witness2EmpId > 0)
            {
                var witness2Info = (from r in _PRMService.PRMUnit.EmploymentInfoRepository.GetAll()
                                    join d in _PRMService.PRMUnit.DesignationRepository.GetAll() on r.DesignationId equals d.Id
                                    join p in _PRMService.PRMUnit.PersonalInfoRepository.GetAll() on r.Id equals p.EmployeeId
                                    where r.Id == entity.Witness2EmpId
                                    select new { r.Id, r.EmpID, d.Name, r.FullName, p.PresentAddress1 }).Distinct().OrderBy(x => x.EmpID).FirstOrDefault();
                viewModel.Witness2EmpId = witness2Info.Id;
                viewModel.Witness2Id = witness2Info.EmpID;
                viewModel.Witness2Name = witness2Info.FullName;
                viewModel.Witness2Designation = witness2Info.Name;
                viewModel.Witness2Address = witness2Info.PresentAddress1;
            }
            viewModel.ActionType = "PersonalFamilyInformationEdit";
            parentModel.EmployeeId = employeeid;
            parentModel.ViewType = "FamilyMember";
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);

            PopulateFamilyMemberViewModelDropdownList(viewModel);
            parentModel.PersonalFamilyMemberInformation = viewModel;
            parentModel.PersonalFamilyMemberInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }


        [HttpPost]
        public ActionResult PersonalFamilyInformationEdit(PersonalFamilyMemberInformationViewModel viewModel, string btnSubmit)
        {
            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = viewModel.EmployeeId;
            parentModel.ViewType = "FamilyMember";
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);

            try
            {
                if (btnSubmit == "Upload")
                {
                    viewModel = UploadFamilyMemberPhoto(viewModel);
                }
                else if (btnSubmit == "Remove")
                {
                    viewModel = RemoveFamilyMemberPhoto(viewModel);
                }
                else if (btnSubmit == "Update")
                {
                    var entity = viewModel.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    //if (entity.Photo != null)
                    //{
                    //    entity.isAddPhoto = true;
                    //}
                    _empService.PRMUnit.PersonalFamilyInformation.Update(entity);
                    _empService.PRMUnit.PersonalFamilyInformation.SaveChanges();
                    viewModel.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    viewModel.IsSuccessful = true;
                    return RedirectToAction("PersonalFamilyInformationIndex", new { id = viewModel.EmployeeId, message = viewModel.Message });

                }

            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);
            parentModel.PersonalFamilyMemberInformation = viewModel;
            parentModel.PersonalFamilyMemberInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);

        }

        [HttpPost]
        public JsonResult DeleteConfirmforFamilyMember(PersonalFamilyMemberInformationViewModel viewModel)
        {
            try
            {
                _empService.PRMUnit.PersonalFamilyInformation.Delete(viewModel.Id, new List<Type>());
                _empService.PRMUnit.PersonalFamilyInformation.SaveChanges();
                viewModel.Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                viewModel.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }
            return Json(new
            {
                Success = viewModel.IsSuccessful,
                Message = viewModel.Message
            });
        }


        public FileContentResult GetFamilyMemberPhoto(int? id)
        {

            PRM_EmpFamilyMemberInfo familyMember = null;

            if (id != null && id != 0)
            {
                familyMember = _empService.PRMUnit.PersonalFamilyInformation.GetByID(id);
            }
            if (familyMember != null)
            {
                return File(familyMember.Photo, "image/jpeg");
            }
            else
            {
                return null;
            }
        }

        //service method
        private List<PRM_EmpFamilyMemberInfo> GetPersonalFamilyMemberInformation(int employeeID)
        {
            List<PRM_EmpFamilyMemberInfo> list = new List<PRM_EmpFamilyMemberInfo>();
            list = (from c in _empService.PRMUnit.PersonalFamilyInformation.Fetch()
                    where c.EmployeeId == employeeID
                    select c).ToList();
            return list;
        }

        private void PopulateFamilyMemberViewModelDropdownList(PersonalFamilyMemberInformationViewModel model)
        {
            dynamic ddlList;

            #region Gender ddl

            model.GenderList = Common.PopulateGenderDDL(model.GenderList);

            #endregion

            #region Relation ddl
            ddlList = _empService.PRMUnit.Relation.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.RelationList = Common.PopulateDllList(ddlList);
            #endregion

            #region Title ddl
            ddlList = _empService.PRMUnit.NameTitleRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.TitleList = Common.PopulateDllList(ddlList);
            #endregion

            #region MaritalStatus ddl
            ddlList = _empService.PRMUnit.MaritalStatusRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.MaritalStatusList = Common.PopulateDllList(ddlList);
            #endregion

            #region Profession ddl
            ddlList = _empService.PRMUnit.ProfessionRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.ProfessionList = Common.PopulateDllList(ddlList);
            #endregion

            #region Family Member ddl
            if (model.EmployeeId > 0)
            {
                model.PersonOnBehalfList = _PRMService.PRMUnit.PersonalFamilyInformation.GetAll().Where(p => p.EmployeeId == model.EmployeeId)
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.FullName,
                    Value = y.Id.ToString()
                }).ToList();
            }
            #endregion

            #region Degree ddl
            ddlList = _empService.PRMUnit.ExamDegreeLavelRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.DegreeLevelList = Common.PopulateDllList(ddlList);
            #endregion

            #region Nationality ddl
            ddlList = _empService.PRMUnit.NationalityRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.NationalityList = Common.PopulateDllList(ddlList);
            #endregion

            #region BloodGroup ddl
            ddlList = _empService.PRMUnit.BloodGroupRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.BloodGroupList = Common.PopulateDllList(ddlList);
            #endregion

        }

        private PersonalFamilyMemberInformationViewModel InitializePersonalFamilyMemberViewModel(PersonalFamilyMemberInformationViewModel viewModel)
        {
            var freshViewModel = new PersonalFamilyMemberInformationViewModel();
            freshViewModel.EmployeeId = viewModel.EmployeeId;
            freshViewModel.Message = viewModel.Message;
            freshViewModel.IsSuccessful = viewModel.IsSuccessful;
            var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(viewModel.EmployeeId);
            if (empEntity != null)
            {
                freshViewModel.EmpID = empEntity.EmpID;
                freshViewModel.EmployeeId = empEntity.Id;
                freshViewModel.Name = empEntity.FullName;
                freshViewModel.EmployeeInitial = empEntity.EmployeeInitial;
                freshViewModel.Designation = _empService.PRMUnit.DesignationRepository.GetByID(empEntity.DesignationId).Name;
            }
            PopulateFamilyMemberViewModelDropdownList(freshViewModel);
            return freshViewModel;
        }

        private PersonalFamilyMemberInformationViewModel UploadFamilyMemberPhoto(PersonalFamilyMemberInformationViewModel model)
        {
            try
            {
                var image = WebImage.GetImageFromRequest();
                if (image != null)
                {
                    byte[] buf = image.GetBytes();
                    model.Photo = buf;
                    if (image.Width > 500)
                    {
                        image.Resize(500, ((500 * image.Height) / image.Width));
                    }
                    var filename = Path.GetFileName(image.FileName);
                    image.Save(Path.Combine("~/Content/TempFiles/", filename));
                    filename = Path.Combine("~/Content/TempFiles/", filename);
                    model.ImageUrl = Url.Content(filename);
                    model.ImageAltText = image.FileName.Substring(0, image.FileName.Length - 4);
                    model.Message = "Photo Upload Successful!";
                    model.IsSuccessful = true;
                    model.isAddPhoto = true;
                    PopulateFamilyMemberViewModelDropdownList(model);


                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = "Photo Upload Failed!";
            }
            return model;
        }

        private PersonalFamilyMemberInformationViewModel RemoveFamilyMemberPhoto(PersonalFamilyMemberInformationViewModel viewModel)
        {

            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = viewModel.EmployeeId;
            parentModel.ViewType = "FamilyMember";
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);

            try
            {
                viewModel.Photo = null;
                viewModel.ImageUrl = string.Empty;
                viewModel.ImageAltText = string.Empty;
                viewModel.IsSuccessful = true;
                viewModel.Message = "Photo Removed!";
                viewModel.isAddPhoto = true;
                PopulateFamilyMemberViewModelDropdownList(viewModel);
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = "Photo Removal Failed!";
            }

            return viewModel;
        }

        #endregion

        #region Personal Emergency Contract

        public ActionResult PersonalEmergencyContractIndex(int id, string message)
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.PersonalEmergencyContract;
            var entity = _empService.PRMUnit.EmergencyContractPerson.GetByID(id, "EmployeeId");
            if (entity != null)
            {
                model = entity.ToModel();
                model.HasDatabaserValue = true;
                model.ActionType = "PersonalEmergencyContractEdit";

                if (model.Photo != null)
                {
                    if (model.Photo.Length > 0)
                    {
                        model.isAddPhoto = true;
                    }
                }
                else
                {
                    model.isAddPhoto = false;
                }
            }
            else
            {
                model.ActionType = "PersonalEmergencyContractCreate";
            }

            if (message != "")
            {
                model.Message = message;
                model.IsSuccessful = true;
            }
            model = InitializePersonalEmergencyContractViewModel(model);
            model.EmployeeId = id;
            parentModel.EmployeeId = id;
            parentModel.ViewType = "EmergencyContract";
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            parentModel.PersonalEmergencyContract = model;
            parentModel.PersonalEmergencyContract.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult PersonalEmergencyContractCreate(PersonalEmergencyContractViewModel viewModel, string btnSubmit)
        {
            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = viewModel.EmployeeId;
            parentModel.ViewType = "EmergencyContract";
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);

            try
            {
                if (btnSubmit == "Upload")
                {
                    viewModel = UploadEmergencyContractPhoto(viewModel);
                }
                else if (btnSubmit == "Remove")
                {
                    viewModel = RemoveEmergencyContractPhoto(viewModel);
                }
                else if (btnSubmit == "Save")
                {
                    var entity = viewModel.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = DateTime.Now;
                    //if (entity.Photo != null)
                    //{
                    //    entity.isAddPhoto = true;
                    //    viewModel.isAddPhoto = true;
                    //}
                    _empService.PRMUnit.EmergencyContractPerson.Add(entity);
                    _empService.PRMUnit.EmergencyContractPerson.SaveChanges();
                    viewModel.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    viewModel.IsSuccessful = true;
                    viewModel.HasDatabaserValue = true;
                    return RedirectToAction("PersonalEmergencyContractIndex", new { id = viewModel.EmployeeId, message = viewModel.Message });

                }
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            InitializePersonalEmergencyContractViewModel(viewModel);
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);
            parentModel.PersonalEmergencyContract = viewModel;
            parentModel.PersonalEmergencyContract.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult PersonalEmergencyContractEdit(PersonalEmergencyContractViewModel viewModel, string btnSubmit)
        {
            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = viewModel.EmployeeId;
            parentModel.ViewType = "EmergencyContract";
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);

            try
            {
                if (btnSubmit == "Upload")
                {
                    viewModel = UploadEmergencyContractPhoto(viewModel);
                }
                else if (btnSubmit == "Remove")
                {
                    viewModel = RemoveEmergencyContractPhoto(viewModel);
                }
                else if (btnSubmit == "Update")
                {
                    var entity = viewModel.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    //if (entity.Photo != null)
                    //{
                    //    viewModel.isAddPhoto = true;
                    //}
                    _empService.PRMUnit.EmergencyContractPerson.Update(entity, "EmployeeId");
                    _empService.PRMUnit.EmergencyContractPerson.SaveChanges();
                    viewModel.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    viewModel.IsSuccessful = true;
                    return RedirectToAction("PersonalEmergencyContractIndex", new { id = viewModel.EmployeeId, message = viewModel.Message });

                }
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }
            viewModel = InitializePersonalEmergencyContractViewModel(viewModel);
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);
            parentModel.PersonalEmergencyContract = viewModel;
            parentModel.PersonalEmergencyContract.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public JsonResult DeleteConfirmEmergencyContract(PersonalEmergencyContractViewModel viewModel)
        {
            try
            {
                var entity = viewModel.ToEntity();
                _empService.PRMUnit.EmergencyContractPerson.Delete(entity.EmployeeId, "EmployeeId", new List<Type>());
                _empService.PRMUnit.EmergencyContractPerson.SaveChanges();
                viewModel.Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                viewModel.IsSuccessful = true;
                viewModel.HasDatabaserValue = false;

                var freshViewModel = new PersonalEmergencyContractViewModel();
                freshViewModel.EmployeeId = viewModel.EmployeeId;
                freshViewModel.Message = viewModel.Message;
                freshViewModel.IsSuccessful = true;
                InitializePersonalEmergencyContractViewModel(freshViewModel);
                viewModel = freshViewModel;
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }
            return Json(new
            {
                Success = viewModel.IsSuccessful,
                Message = viewModel.Message
            });
        }

        public FileContentResult GetEmergencyContractPhoto(int? id)
        {
            PRM_EmpContractPersonInfo emergencyContract = null;

            if (id != null && id != 0)
            {
                emergencyContract = _empService.PRMUnit.EmergencyContractPerson.GetByID((int)id, "EmployeeId");
            }
            if (emergencyContract != null)
            {
                return File(emergencyContract.Photo, "image/jpeg");
            }
            else
            {
                return null;
            }
        }

        #region Private Methods

        private PersonalEmergencyContractViewModel UploadEmergencyContractPhoto(PersonalEmergencyContractViewModel model)
        {
            try
            {
                var image = WebImage.GetImageFromRequest();
                if (image != null)
                {
                    byte[] buf = image.GetBytes();
                    model.Photo = buf;
                    if (image.Width > 500)
                    {
                        image.Resize(500, ((500 * image.Height) / image.Width));
                    }
                    var filename = Path.GetFileName(image.FileName);
                    image.Save(Path.Combine("~/Content/TempFiles/", filename));
                    filename = Path.Combine("~/Content/TempFiles/", filename);
                    model.ImageUrl = Url.Content(filename);
                    model.ImageAltText = image.FileName.Substring(0, image.FileName.Length - 4);
                    model.Message = "Photo Upload Successful!";
                    model.IsSuccessful = true;
                    model.isAddPhoto = true;

                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = "Photo Upload Failed!";
            }

            return model;
        }

        private PersonalEmergencyContractViewModel RemoveEmergencyContractPhoto(PersonalEmergencyContractViewModel viewModel)
        {
            try
            {
                viewModel.Photo = null;
                viewModel.ImageUrl = string.Empty;
                viewModel.ImageAltText = string.Empty;
                viewModel.IsSuccessful = true;
                viewModel.Message = "Photo Removed!";
                viewModel.isAddPhoto = false;

            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = "Photo Remove Failed!";
            }

            return viewModel;
        }

        private void PopulatePersonalEmergencyContractViewModelDropdownList(PersonalEmergencyContractViewModel model)
        {
            dynamic ddlList;

            #region Relation ddl
            ddlList = _empService.PRMUnit.Relation.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.RelationList = Common.PopulateDllList(ddlList);
            #endregion

            #region Title ddl
            ddlList = _empService.PRMUnit.NameTitleRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.TitleList = Common.PopulateDllList(ddlList);
            #endregion
        }

        private PersonalEmergencyContractViewModel InitializePersonalEmergencyContractViewModel(PersonalEmergencyContractViewModel viewModel)
        {
            var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(viewModel.EmployeeId);
            if (empEntity != null)
            {
                viewModel.EmpID = empEntity.EmpID;
                viewModel.Name = empEntity.FullName;
                viewModel.EmployeeInitial = empEntity.EmployeeInitial;
                viewModel.Designation = _empService.PRMUnit.DesignationRepository.GetByID(empEntity.DesignationId).Name;
            }
            PopulatePersonalEmergencyContractViewModelDropdownList(viewModel);
            return viewModel;
        }


        #endregion

        #endregion

        #region Personal Nominee

        public ActionResult PersonalNomineeInformationIndex(int id, string message)
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.PersonalNominee;
            PopulatePersonalNomineeDropdownList(model);
            model.EmployeeId = id;
            if (!String.IsNullOrEmpty(message))
            {
                model.Message = message;
                model.IsSuccessful = true;
            }
            parentModel.EmployeeId = id;
            parentModel.ViewType = "Nominee";
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            parentModel.PersonalNominee = model;
            parentModel.PersonalNominee.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }
        //RR
        public ActionResult PersonalNomineeInformationEdit(int employeeID, int? nomineeForID)
        {
            var parentModel = new PersonalViewModel();
            var viewModel = parentModel.PersonalNominee;
            if (nomineeForID != null)
            {
                var entity = GetNomineeByEmployeeID(employeeID, (int)nomineeForID);

                var entityList = (from c in _empService.PRMUnit.Nominee.Fetch()
                                  where c.EmployeeId == employeeID && c.NomineeForId == nomineeForID
                                  select c).ToList();


                if (entity != null)
                {
                    viewModel = entity.ToModel();
                    PopulatePersonalNomineeDropdownList(viewModel);

                    if (entity.PRM_EmpNomineeDetail != null)
                    {
                        viewModel = PopulateforExistingFamilyMember2(viewModel, entityList);
                    }
                    viewModel.ActionMode = "PersonalNomineeInformationEdit";
                }
                else
                {
                    viewModel.NomineeForId = (int)nomineeForID;
                    viewModel.EmployeeId = employeeID;
                    viewModel.ActionMode = "PersonalNomineeInformationCreate";
                    PopulatePersonalNomineeDropdownList(viewModel);
                }
            }
            else
            {
                viewModel.EmployeeId = employeeID;
                PopulatePersonalNomineeDropdownList(viewModel);
            }

            parentModel.EmployeeId = employeeID;
            parentModel.ViewType = "Nominee";
            Common.PopulateEmployeeTop(viewModel.EmpTop, viewModel.EmployeeId, _empService);
            parentModel.PersonalNominee = viewModel;
            parentModel.PersonalNominee.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult PersonalNomineeInformationCreate(PersonalNomineeViewModel model)
        {

            if (model.PersonalNomineeDetail != null && (model.PersonalNomineeDetail.Count() < 1))
            {
                model.IsSuccessful = false;
                model.Message = "Please insert at least one Nominee Details";

                var pmodel = GetParentModelforNominee(model, false);
                return View("CreateOrEditPersonaInfo", pmodel);
            }
            else if (model.PersonalNomineeDetail == null)
            {
                model.IsSuccessful = false;
                model.Message = "Please insert at least one Nominee Details";

                var pmodel = GetParentModelforNominee(model, false);
                return View("CreateOrEditPersonaInfo", pmodel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = DateTime.Now;
                    foreach (var item in model.PersonalNomineeDetail)
                    {
                        if (item.PercentOfShare > 0)
                        {
                            var childEntity = item.ToEntity();
                            childEntity.IUser = User.Identity.Name;
                            childEntity.IDate = DateTime.Now;
                            entity.PRM_EmpNomineeDetail.Add(childEntity);
                        }
                    }
                    if (entity.PRM_EmpNomineeDetail.Count() > 0)
                    {
                        _empService.PRMUnit.Nominee.Add(entity);
                        _empService.PRMUnit.Nominee.SaveChanges();
                        model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model.IsSuccessful = true;
                        PopulatePersonalNomineeDropdownList(model);
                        //return RedirectToAction("PersonalNomineeInformationIndex", new { id = model.EmployeeId, message = model.Message });
                    }
                }
                catch (Exception ex)
                {
                    model.IsSuccessful = false;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsSuccessful = false;
                model.Message = "Please fill up the red marked field(s).";
            }
            PopulatePersonalNomineeDropdownList(model);
            var parentModel = GetParentModelforNominee(model, true);
            return View("CreateOrEditPersonaInfo", parentModel);

        }

        [HttpPost]
        public ActionResult PersonalNomineeInformationEdit(PersonalNomineeViewModel model)
        {
            if (model.PersonalNomineeDetail != null && (model.PersonalNomineeDetail.Count() < 1))
            {
                model.IsSuccessful = false;
                model.Message = "Please insert at least one Nominee Details";

                var pmodel = GetParentModelforNominee(model, false);
                return View("CreateOrEditPersonaInfo", pmodel);
            }
            else if (model.PersonalNomineeDetail == null)
            {
                model.IsSuccessful = false;
                model.Message = "Please insert at least one Nominee Details";

                var pmodel = GetParentModelforNominee(model, false);
                return View("CreateOrEditPersonaInfo", pmodel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ArrayList childNomineeDetails = new ArrayList();
                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    foreach (var item in model.PersonalNomineeDetail)
                    {
                        var childEntity = item.ToEntity();
                        childEntity.EUser = User.Identity.Name;
                        childEntity.EDate = DateTime.Now;
                        childEntity.IUser = User.Identity.Name;
                        childEntity.IDate = DateTime.Now;
                        childNomineeDetails.Add(childEntity);
                    }
                    Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                    NavigationList.Add(typeof(PRM_EmpNomineeDetail), childNomineeDetails);
                    _empService.PRMUnit.Nominee.Update(entity, NavigationList);
                    _empService.PRMUnit.Nominee.SaveChanges();
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    model.IsSuccessful = true;
                    PopulatePersonalNomineeDropdownList(model);
                    //   return RedirectToAction("PersonalNomineeInformationIndex", new { id = model.EmployeeId, message = model.Message });

                }
                catch (Exception ex)
                {
                    model.IsSuccessful = false;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.IsSuccessful = false;
                model.Message = "Please fill up the red marked field(s).";
            }
            PopulatePersonalNomineeDropdownList(model);
            var parentModel = GetParentModelforNominee(model, true);
            return View("CreateOrEditPersonaInfo", parentModel);

        }

        [HttpPost]
        public JsonResult DeleteConfirmforNominee(PersonalNomineeViewModel viewModel)
        {
            try
            {
                List<Type> allTypes = new List<Type> { typeof(PRM_EmpNomineeDetail) };
                _empService.PRMUnit.Nominee.Delete(viewModel.Id, allTypes);
                _empService.PRMUnit.Nominee.SaveChanges();
                viewModel.Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                viewModel.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }
            return Json(new
            {
                Success = viewModel.IsSuccessful,
                Message = viewModel.Message
            });
        }

        public PartialViewResult AddNomineeDetail(int id, int nomineeForID)
        {
            dynamic ddlList;
            ddlList = GetPersonalFamilyMemberInformation(id);
            var model = new PersonalNomineeDetailsViewModel();
            model.NomineeId = nomineeForID;
            model.FamilyMemberList = Common.PopulateFamilyMemberList(ddlList);
            return PartialView("_NomineeDetails", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetSelectedFamilyMemberData(int id)
        {
            var data = _empService.PRMUnit.PersonalFamilyInformation.GetByID(id);
            return Json(new
            {
                data.Id,
                _empService.PRMUnit.Relation.GetByID(data.RelationId).Name,
                Date = data.DateofBirth.ToString("dd-MM-yyyy"),
                Age = Common.GetAgebyDateOfBirth(data.DateofBirth)
            });
        }

        [HttpPost, ActionName("DeleteNomineeDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                var masterId = _empService.PRMUnit.NomineeDetails.Get(s => s.Id == id).Select(x => x.NomineeId).FirstOrDefault();

                _empService.PRMUnit.NomineeDetails.Delete(id);
                _empService.PRMUnit.NomineeDetails.SaveChanges();

                var costs = _empService.PRMUnit.NomineeDetails.Get(q => q.NomineeId == masterId).ToList();

                if (costs.Count == 0)
                {
                    _empService.PRMUnit.Nominee.Delete(masterId);
                }
                _empService.PRMUnit.Nominee.SaveChanges();
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
                        // if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                        // "The user has related information and cannot be deleted."
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
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }
        #region Private Methods

        private PersonalViewModel GetParentModelforNominee(PersonalNomineeViewModel model, bool hasChild)
        {
            PopulatePersonalNomineeDropdownList(model);
            var entity = GetNomineeByEmployeeID(model.EmployeeId, model.NomineeForId);
            if (entity != null && hasChild == true)
            {
                PopulateforExistingFamilyMember(model, entity);
            }
            var parentModel = new PersonalViewModel();
            model.EmployeeId = model.EmployeeId;
            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "Nominee";
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            parentModel.PersonalNominee = model;
            parentModel.PersonalNominee.SideBarClassName = "selected";

            return parentModel;
        }
        private PersonalNomineeViewModel PopulateforExistingFamilyMember(PersonalNomineeViewModel model, PRM_EmpNominee entity)
        {
            dynamic ddlList;
            var familyMemberList = GetPersonalFamilyMemberInformation(model.EmployeeId);
            ddlList = familyMemberList;
            model.PersonalNomineeDetail = new Collection<PersonalNomineeDetailsViewModel>();

            foreach (var item in entity.PRM_EmpNomineeDetail)
            {
                var childModel = item.ToModel();
                childModel.FamilyMemberList = Common.PopulateFamilyMemberList(ddlList);

                foreach (var selectItem in childModel.FamilyMemberList)
                {
                    if (selectItem.Value == childModel.FamilyMemberId.ToString())
                    {
                        selectItem.Selected = true;
                    }
                }
                var familyMember = familyMemberList.Where(d => d.Id == childModel.FamilyMemberId).FirstOrDefault();
                if (familyMember != null)
                {
                    childModel.DateOfBirth = familyMember.DateofBirth.ToString("dd-MM-yyyy");
                    childModel.Relation = _empService.PRMUnit.Relation.GetByID(familyMember.RelationId).Name;
                    childModel.Age = Convert.ToString(Common.GetAgebyDateOfBirth(familyMember.DateofBirth));
                    model.PersonalNomineeDetail.Add(childModel);
                }
            }

            model.TotalShare = Convert.ToString(entity.PRM_EmpNomineeDetail.Sum(d => d.PercentOfShare));
            return model;
        }

        private PersonalNomineeViewModel PopulateforExistingFamilyMember2(PersonalNomineeViewModel model, List<PRM_EmpNominee> list)
        {
            dynamic ddlList;
            var familyMemberList = GetPersonalFamilyMemberInformation(model.EmployeeId);
            ddlList = familyMemberList;
            model.PersonalNomineeDetail = new Collection<PersonalNomineeDetailsViewModel>();
            foreach (var nom in list)
            {
                foreach (var item in nom.PRM_EmpNomineeDetail)
                {
                    var childModel = item.ToModel();
                    childModel.FamilyMemberList = Common.PopulateFamilyMemberList(ddlList);

                    foreach (var selectItem in childModel.FamilyMemberList)
                    {
                        if (selectItem.Value == childModel.FamilyMemberId.ToString())
                        {
                            selectItem.Selected = true;
                        }
                    }
                    var familyMember = familyMemberList.Where(d => d.Id == childModel.FamilyMemberId).FirstOrDefault();
                    if (familyMember != null)
                    {
                        childModel.DateOfBirth = familyMember.DateofBirth.ToString("dd-MM-yyyy");
                        childModel.Relation = _empService.PRMUnit.Relation.GetByID(familyMember.RelationId).Name;
                        childModel.Age = Convert.ToString(Common.GetAgebyDateOfBirth(familyMember.DateofBirth));
                        model.PersonalNomineeDetail.Add(childModel);
                    }
                }
                model.TotalShare += Convert.ToString(nom.PRM_EmpNomineeDetail.Sum(d => d.PercentOfShare));
            }

            return model;
        }

        private PRM_EmpNominee GetNomineeByEmployeeID(int id, int nomineefor)
        {
            var entity = (from c in _empService.PRMUnit.Nominee.Fetch()
                          where c.EmployeeId == id && c.NomineeForId == nomineefor
                          select c).FirstOrDefault();
            return entity;
        }
        private PersonalNomineeViewModel InitializePersonalNomineeViewModel(PersonalNomineeViewModel model)
        {
            var viewModel = new PersonalNomineeViewModel();
            viewModel.Message = model.Message;
            viewModel.IsSuccessful = model.IsSuccessful;
            viewModel.EmployeeId = model.EmployeeId;
            PopulatePersonalNomineeDropdownList(viewModel);
            return viewModel;
        }
        private void PopulatePersonalNomineeDropdownList(PersonalNomineeViewModel model)
        {
            dynamic ddlList;

            #region Relation ddl
            ddlList = _empService.PRMUnit.NomineeFor.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.NomineeForList = Common.PopulateDllList(ddlList);
            #endregion
        }

        #endregion

        #endregion

        #region Reference Information

        public ActionResult ReferenceInfoIndex(int? id, string insertResult, string updateResult, string deleteResult, string controlType)
        {
            var model = new ReferenceInfoViewModel();
            Common.PopulateEmployeeTop(model.EmpTop, id.Value, _empService);
            populateDropdown(model);
            model.EmployeeId = model.EmpTop.EmployeeId;
            model.ActionType = "CreateReferenceInfo";
            model.ButtonText = "Save";
            model.SideBarClassName = "selected";
            model.Type = controlType;//BEPZA_MEDICAL.Web.Utility.Common.ReferenceGuarantorEnum.ReferenceInfo.ToString();

            if (insertResult == "success")
            {
                model.Message = Resources.ErrorMessages.InsertSuccessful;
                model.ErrorClass = "success";
            }

            if (updateResult == "success")
            {
                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
            }

            if (deleteResult == "success")
            {
                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.ErrorClass = "success";
            }

            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "ReferenceInfo";
            parentModel.ReferenceInformation = model;
            parentModel.ReferenceInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateReferenceInfo([Bind(Exclude = "Photo")]ReferenceInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;
                    entity.Designation = model.DesignationRG;

                    var attachment = Request.Files["Photo"];
                    if (attachment != null && !string.IsNullOrEmpty(attachment.FileName))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            attachment.InputStream.CopyTo(ms);
                            entity.Photo = ms.GetBuffer();
                        }
                    }
                    _empService.PRMUnit.EmpReferenceGuarantorRepository.Add(entity);
                    _empService.PRMUnit.EmpReferenceGuarantorRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    populateDropdown(model);
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    model.ErrorClass = "failed";
                    model.ActionType = "CreateReferenceInfo";
                    model.ButtonText = "Save";
                    model.SideBarClassName = "selected";

                    var parentModel = new PersonalViewModel();
                    parentModel.EmployeeId = model.EmployeeId;
                    parentModel.ViewType = "ReferenceInfo";
                    parentModel.ReferenceInformation = model;
                    parentModel.ReferenceInformation.SideBarClassName = "selected";
                    return View("CreateOrEditPersonaInfo", parentModel);
                }
            }
            else
            {
                populateDropdown(model);
                model.Message = Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.ActionType = "CreateReferenceInfo";
                model.ButtonText = "Save";
                model.SideBarClassName = "selected";

                var parentModel = new PersonalViewModel();
                parentModel.EmployeeId = model.EmployeeId;
                parentModel.ViewType = "ReferenceInfo";
                parentModel.ReferenceInformation = model;
                parentModel.ReferenceInformation.SideBarClassName = "selected";
                return View("CreateOrEditPersonaInfo", parentModel);
            }
            return RedirectToAction("ReferenceInfoIndex", "PersonalInfo", new { id = model.EmployeeId, insertResult = "success", controlType = model.Type });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetReferenceGuarantorList(JqGridRequest request, int empId, string controlType)
        {
            var list = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.Fetch().Where(x => x.Type == controlType && x.EmployeeId == empId).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FullName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FullName).ToList();
                }
            }
            if (request.SortingName == "Organization")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Organization).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Organization).ToList();
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


            if (request.SortingName == "MobileNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MobileNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MobileNo).ToList();
                }
            }

            if (request.SortingName == "Relation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Relation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Relation).ToList();
                }
            }


            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.FullName,
                    item.Id,                   
                    item.Organization,
                    item.Designation,
                    item.MobileNo,
                    item.Relation
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult EditReferenceInfo(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.GetByID(id);
            var model = entity.ToModel();
            model.DesignationRG = entity.Designation;
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            populateDropdown(model);
            model.ActionType = "EditReferenceInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SideBarClassName = "selected";

            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "ReferenceInfo";
            parentModel.ReferenceInformation = model;
            parentModel.ReferenceInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);

            //return View("_ReferenceGuarantorInfo", model);
        }

        [HttpPost]
        public ActionResult EditReferenceInfo([Bind(Exclude = "Photo")]ReferenceInfoViewModel model)
        {
            var parentModel = new PersonalViewModel();

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;
                    entity.Designation = model.DesignationRG;

                    var attachment = Request.Files["Photo"];
                    if (attachment != null && !string.IsNullOrEmpty(attachment.FileName))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            attachment.InputStream.CopyTo(ms);
                            entity.Photo = ms.GetBuffer();
                        }
                    }
                    else
                    {
                        entity.Photo = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.GetByID(entity.Id).Photo;
                    }
                    _empService.PRMUnit.EmpReferenceGuarantorRepository.Update(entity);
                    _empService.PRMUnit.EmpReferenceGuarantorRepository.SaveChanges();

                    //model = new ReferenceInfoViewModel();
                    //PropertyReflector.ClearProperties(model.);
                    //ModelState.Clear();
                }
                catch (Exception ex)
                {
                    populateDropdown(model);
                    Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";

                    model.DeleteEnable = true;
                    model.ActionType = "EditReferenceInfo";
                    model.ButtonText = "Update";
                    model.SideBarClassName = "selected";

                    parentModel.EmployeeId = model.EmployeeId;
                    parentModel.ReferenceInformation = model;
                    parentModel.ViewType = "ReferenceInfo";
                    return View("CreateOrEditPersonaInfo", parentModel);
                }
            }
            else
            {
                populateDropdown(model);
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                model.Message = Resources.ErrorMessages.UpdateFailed;
                model.ErrorClass = "failed";

                model.DeleteEnable = true;
                model.ActionType = "EditReferenceInfo";
                model.ButtonText = "Update";
                model.SideBarClassName = "selected";

                parentModel.EmployeeId = model.EmployeeId;
                parentModel.ReferenceInformation = model;
                parentModel.ViewType = "ReferenceInfo";
                return View("CreateOrEditPersonaInfo", parentModel);
            }
            populateDropdown(model);
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            model.Message = Resources.ErrorMessages.UpdateSuccessful;
            model.ErrorClass = "success";
            model.DeleteEnable = true;
            model.ActionType = "EditReferenceInfo";
            model.ButtonText = "Update";
            model.SideBarClassName = "selected";

            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "ReferenceInfo";
            parentModel.ReferenceInformation = model;
            parentModel.ReferenceInformation.SideBarClassName = "selected";

            return RedirectToAction("ReferenceInfoIndex", "PersonalInfo", new { id = model.EmployeeId, updateResult = "success", controlType = model.Type });
            //return View("CreateOrEditPersonaInfo", parentModel);         
        }

        public ActionResult DeleteReferenceInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            var entity = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.GetByID(id);
            var model = entity.ToModel();

            if (ModelState.IsValid && entity != null)
            {
                try
                {
                    _empService.PRMUnit.EmpReferenceGuarantorRepository.Delete(entity);
                    _empService.PRMUnit.EmpReferenceGuarantorRepository.SaveChanges();
                }
                catch (Exception ex)
                {//CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);



                    model.DesignationRG = entity.Designation;
                    Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                    populateDropdown(model);
                    model.ActionType = "EditReferenceInfo";
                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SideBarClassName = "selected";

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";

                    parentModel = new PersonalViewModel();
                    parentModel.EmployeeId = model.EmployeeId;
                    parentModel.ViewType = "ReferenceInfo";
                    parentModel.ReferenceInformation = model;
                    parentModel.ReferenceInformation.SideBarClassName = "selected";

                    return View("CreateOrEditPersonaInfo", parentModel);
                }

                return RedirectToAction("ReferenceInfoIndex", null, new { id = entity.EmployeeId, deleteResult = "success", controlType = entity.Type });
            }
            model.DesignationRG = entity.Designation ?? string.Empty;
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.ButtonText = "Update";
            model.ActionType = "EditReferenceInfo";
            model.DeleteEnable = true;
            model.SideBarClassName = "selected";

            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "ReferenceInfo";
            parentModel.ReferenceInformation = model;
            parentModel.ReferenceInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        private void populateDropdown(ReferenceInfoViewModel model)
        {
            #region Title
            var titleList = _empService.PRMUnit.NameTitleRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.TitleList = Common.PopulateDllList(titleList);
            #endregion
        }

        #endregion

        #region Visa Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetVisaList(JqGridRequest request, int empId, string controlType)
        {
            var list = _personalInfoService.PRMUnit.EmployeeVisaInfoRepository.Fetch().Where(x => x.Type == controlType && x.EmployeeId == empId).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "VisaPassportFor")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.VisaPassportFor).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.VisaPassportFor).ToList();
                }
            }
            if (request.SortingName == "VisaOwner")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.VisaOwner).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.VisaOwner).ToList();
                }
            }

            if (request.SortingName == "VisaPassportNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.VisaPassportNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.VisaPassportNo).ToList();
                }
            }

            if (request.SortingName == "IssuePlace")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IssuePlace).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IssuePlace).ToList();
                }
            }

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_Country.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_Country.Name).ToList();
                }
            }

            if (request.SortingName == "IssueDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IssueDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IssueDate).ToList();
                }
            }

            if (request.SortingName == "ExpireDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ExpireDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ExpireDate).ToList();
                }
            }

            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.VisaPassportFor,
                    item.VisaOwner,
                    item.VisaPassportNo,
                    item.IssuePlace,
                    item.PRM_Country.Name,
                    item.IssueDate.ToString("dd-MMM-yyyy"),
                    item.ExpireDate.ToString("dd-MMM-yyyy")
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult VisaInfoIndex(int? id, string insertResult, string updateResult, string deleteResult, string controlType)
        {
            var model = new VisaInfoViewModel();
            Common.PopulateEmployeeTop(model.EmpTop, id.Value, _empService);

            model.EmployeeId = model.EmpTop.EmployeeId;
            model.ActionType = "CreateVisaInfo";
            model.ButtonText = "Save";
            model.SideBarClassName = "selected";
            model.Type = controlType;
            model.ControlType = controlType;//BEPZA_MEDICAL.Web.Utility.Common.ReferenceGuarantorEnum.ReferenceInfo.ToString();

            if (insertResult == "success")
            {
                model.Message = Resources.ErrorMessages.InsertSuccessful;
                model.ErrorClass = "success";
            }

            if (updateResult == "success")
            {
                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
            }

            if (deleteResult == "success")
            {
                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.ErrorClass = "success";
            }

            populateDropdown(model);
            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "VisaInfo";
            parentModel.VisaInformation = model;
            parentModel.VisaInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);

        }

        [HttpPost]
        public ActionResult CreateVisaInfo(VisaInfoViewModel model)
        {
            var parentModel = new PersonalViewModel();
            string businessError = string.Empty;

            if (ModelState.IsValid)
            {
                if (model.VisaPassportFor == "Own")
                {
                    model.VisaOwner = model.Name;
                }
                else
                {
                    model.VisaOwner = _empService.PRMUnit.PersonalFamilyInformation.Fetch().Where(x => x.Id == model.FamilyMemberId).FirstOrDefault().FullName;
                }

                var entity = model.ToEntity();
                entity.IUser = User.Identity.Name;
                entity.IDate = Common.CurrentDateTime;

                businessError = _personalInfoService.CheckBusinessLogicVisaPassport(entity);
                if (businessError == string.Empty)
                {
                    if (model.Type == "Visa")
                    {
                        businessError = _personalInfoService.CheckDuplicateVisa(model.VisaPassportNo, model.IssueCountryId);
                    }
                    else
                    {
                        businessError = _personalInfoService.CheckDuplicatePassport(model.VisaPassportNo, model.IssueCountryId);
                    }
                }
                if (businessError != string.Empty)
                {

                    Common.PopulateEmployeeTop(model.EmpTop, model.Id, _empService);

                    populateDropdown(model);
                    model.Message = businessError;
                    model.ErrorClass = "failed";
                    model.ActionType = "CreateVisaInfo";
                    model.ButtonText = "Save";
                    model.SideBarClassName = "selected";

                    parentModel.EmployeeId = model.EmployeeId;
                    parentModel.ViewType = "VisaInfo";
                    parentModel.VisaInformation = model;
                    parentModel.VisaInformation.SideBarClassName = "selected";
                    return View("CreateOrEditPersonaInfo", parentModel);
                }

                _empService.PRMUnit.EmployeeVisaInfoRepository.Add(entity);
                _empService.PRMUnit.EmployeeVisaInfoRepository.SaveChanges();
            }
            else
            {
                populateDropdown(model);
                model.Message = Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.ActionType = "CreateVisaInfo";
                model.ButtonText = "Save";
                model.SideBarClassName = "selected";

                //var parentModel = new PersonalViewModel();
                parentModel.EmployeeId = model.EmployeeId;
                parentModel.ViewType = "VisaInfo";
                parentModel.VisaInformation = model;
                parentModel.VisaInformation.SideBarClassName = "selected";
                return View("CreateOrEditPersonaInfo", parentModel);
            }

            return RedirectToAction("VisaInfoIndex", "PersonalInfo", new { id = model.EmployeeId, insertResult = "success", controlType = model.Type });
        }

        public ActionResult EditVisaInfo(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmployeeVisaInfoRepository.GetByID(id);
            var model = entity.ToModel();
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            populateDropdown(model);
            model.ActionType = "EditVisaInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SideBarClassName = "selected";

            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "VisaInfo";
            parentModel.VisaInformation = model;
            parentModel.VisaInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult EditVisaInfo(VisaInfoViewModel model)
        {
            var parentModel = new PersonalViewModel();
            string businessError = string.Empty;
            if (ModelState.IsValid)
            {
                if (model.VisaPassportFor == "Own")
                {
                    model.VisaOwner = model.Name;
                }
                else
                {
                    model.VisaOwner = _empService.PRMUnit.PersonalFamilyInformation.Fetch().Where(x => x.Id == model.FamilyMemberId).FirstOrDefault().FullName;
                }

                var entity = model.ToEntity();
                entity.EUser = User.Identity.Name;
                entity.EDate = Common.CurrentDateTime;

                businessError = _personalInfoService.CheckBusinessLogicVisaPassport(entity);
                if (businessError == string.Empty)
                {
                    if (model.Type == "Visa")
                    {
                        businessError = _personalInfoService.CheckDuplicateVisaUpdate(model.VisaPassportNo, model.IssueCountryId, model.Id);
                    }
                    else
                    {
                        businessError = _personalInfoService.CheckDuplicatePassportUpdate(model.VisaPassportNo, model.IssueCountryId, model.Id);
                    }
                }
                if (businessError != string.Empty)
                {
                    model.Message = businessError;
                    model.ErrorClass = "failed";
                    parentModel.ViewType = "VisaInfo";
                    parentModel.VisaInformation = model;
                    populateDropdown(model);
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
                    return View("CreateOrEditPersonaInfo", parentModel);
                }
                _empService.PRMUnit.EmployeeVisaInfoRepository.Update(entity);
                _empService.PRMUnit.EmployeeVisaInfoRepository.SaveChanges();
            }
            else
            {
                populateDropdown(model);
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                model.Message = Resources.ErrorMessages.UpdateFailed;
                model.ErrorClass = "failed";
                model.DeleteEnable = true;
                model.ActionType = "EditVisaInfo";
                model.ButtonText = "Update";
                model.SideBarClassName = "selected";

                parentModel.EmployeeId = model.EmployeeId;
                parentModel.VisaInformation = model;
                return View("CreateOrEditPersonaInfo", parentModel);
            }
            var strType = model.Type;

            PropertyReflector.ClearProperties(model);
            ModelState.Clear();
            model.Type = strType;

            populateDropdown(model);
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            model.Message = Resources.ErrorMessages.UpdateSuccessful;
            model.ErrorClass = "success";
            model.ActionType = "CreateVisaInfo";
            model.ButtonText = "Save";
            model.SideBarClassName = "selected";

            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "VisaInfo";
            parentModel.VisaInformation = model;
            parentModel.VisaInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);

        }

        public ActionResult DeleteVisaInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            var entity = _personalInfoService.PRMUnit.EmployeeVisaInfoRepository.GetByID(id);

            if (ModelState.IsValid && entity != null)
            {
                _empService.PRMUnit.EmployeeVisaInfoRepository.Delete(entity);
                _empService.PRMUnit.EmployeeVisaInfoRepository.SaveChanges();

                return RedirectToAction("VisaInfoIndex", null, new { id = entity.EmployeeId, deleteResult = "success", controlType = entity.Type });
            }

            var model = entity.ToModel();
            model.Message = Resources.ErrorMessages.DeleteFailed;
            model.ErrorClass = "failed";
            model.ButtonText = "Update";
            model.ActionType = "EditVisaInfo";
            model.DeleteEnable = true;
            model.SideBarClassName = "selected";

            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "VisaInfo";
            parentModel.VisaInformation = model;
            parentModel.VisaInformation.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);

            // return View("_EmpVisaInformation", model);
        }

        private void populateDropdown(VisaInfoViewModel model)
        {
            #region Country ddl

            var list = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(list);

            #endregion

            #region VisaType ddl

            var listvt = _empService.PRMUnit.VisaTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.VisaTypeList = Common.PopulateDllList(listvt);

            #endregion

            #region PassportType ddl

            var PPT = _empService.PRMUnit.PassportTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.PassportTypeList = Common.PopulateDllList(PPT);

            #endregion

            #region FamilyMember ddl

            var listfm = _empService.PRMUnit.PersonalFamilyInformation.Get(q => q.EmployeeId == model.EmployeeId).OrderBy(x => x.FullName).ToList();
            model.FamilyMemberList = Common.PopulateFamilyMemberList(listfm);

            #endregion
        }

        #endregion

        #region Wealth Statement Information


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetWealthStatementList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllWealthStatementByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "AssetName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetName).ToList();
                }
            }
            if (request.SortingName == "AssetType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetType).ToList();
                }
            }

            if (request.SortingName == "AssetQuantity")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetQuantity).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetQuantity).ToList();
                }
            }

            if (request.SortingName == "AssetGainer")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetGainer).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetGainer).ToList();
                }
            }

            if (request.SortingName == "AssetGainDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetGainDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetGainDate).ToList();
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
                    item.AssetType,
                    item.AssetName,                   
                    item.AssetQuantity,
                    item.AssetGainer,
                    Convert.ToDateTime(item.AssetGainDate).ToString("dd-MM-yyyy"),
                    
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult WealthStatementInformationIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.EmployeeWealthStatementViewModel;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.EmpWealthStatementRepository.GetByID(id.Value);
                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
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

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;

                parentModel.ViewType = "EmpWealthStatementInfo";

                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }

            }

            parentModel.EmployeeWealthStatementViewModel = model;
            PopulateDropdownListWealthStatementInfo(model);
            parentModel.EmployeeWealthStatementViewModel.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.EmployeeWealthStatementViewModel.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditWealthStatementInfo(EmployeeWealthStatementViewModel model)
        {

            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                if (ModelState.IsValid)
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = Common.CurrentDateTime;
                    var entity = model.ToEntity();
                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "EmpWealthStatementInfo";
                        parentModel.EmployeeWealthStatementViewModel = model;
                        PopulateDropdownListWealthStatementInfo(model);
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }

                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.EmpWealthStatementRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        _personalInfoService.PRMUnit.EmpWealthStatementRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.EmpWealthStatementRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                PopulateDropdownListWealthStatementInfo(model);

                parentModel.ViewType = "EmpWealthStatementInfo";
                parentModel.EmployeeWealthStatementViewModel = model;
                parentModel.EmployeeWealthStatementViewModel.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
            catch (Exception ex)
            {
                PopulateDropdownListWealthStatementInfo(model);

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
                if (ex.InnerException.Message.Contains("IX_PRM_EmpJobSkill"))
                {
                    model.Message = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                }
                parentModel.ViewType = "EmpWealthStatementInfo";
                parentModel.EmployeeWealthStatementViewModel = model;
                parentModel.EmployeeWealthStatementViewModel.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteWealthStatementInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "EmpWealthStatementInfo";

            var entity = _personalInfoService.PRMUnit.EmpWealthStatementRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.EmpWealthStatementRepository.Delete(entity);
                    _personalInfoService.PRMUnit.LicenseRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("WealthStatementInformationIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    PopulateDropdownListWealthStatementInfo(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.EmployeeWealthStatementViewModel = model;
                }

            }
            catch
            {
                PopulateDropdownListWealthStatementInfo(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.EmployeeWealthStatementViewModel = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        #endregion

        #region Attachment

        public ActionResult AttachmentIndex(int? id, string insertResult, string updateResult, string deleteResult)
        {
            var model = new EmpAttachmentViewModel();
            Common.PopulateEmployeeTop(model.EmpTop, id.Value, _empService);
            populateDropdown(model);
            model.EmployeeId = model.EmpTop.EmployeeId;
            model.ActionType = "CreateAttachment";
            model.ButtonText = "Save";
            model.SideBarClassName = "selected";

            if (insertResult == "success")
            {
                model.Message = Resources.ErrorMessages.InsertSuccessful;
                model.ErrorClass = "success";
            }

            if (updateResult == "success")
            {
                model.Message = Resources.ErrorMessages.UpdateSuccessful;
                model.ErrorClass = "success";
            }

            if (deleteResult == "success")
            {
                model.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.ErrorClass = "success";
            }

            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "Attachment";
            parentModel.EmployeeAttachment = model;
            parentModel.EmployeeAttachment.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);

        }

        [HttpPost]
        public ActionResult CreateAttachment([Bind(Exclude = "Attachment")]EmpAttachmentViewModel model)
        {
            var attachment = Request.Files["attachment"];

            if (ModelState.IsValid && attachment != null && !string.IsNullOrEmpty(attachment.FileName) /* && attachment.contenttype != model.a*/)
            {
                try
                {
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

                            EmpFileUtl.SaveFile(model.EmployeeId, model.AttachmentTypeId, model.FileName, model.Description, name, contentType, size, fileData, User.Identity.Name);
                        }
                    }

                    return RedirectToAction("AttachmentIndex", "PersonalInfo", new { id = model.EmployeeId, insertResult = "success" });

                }
                catch (Exception ex)
                {
                    populateDropdown(model);
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Save);
                    model.ErrorClass = "failed";
                    model.ActionType = "CreateAttachment";
                    model.ButtonText = "Save";
                    model.SideBarClassName = "selected";

                    var parentmodel = new PersonalViewModel();
                    parentmodel.EmployeeId = model.EmployeeId;
                    parentmodel.ViewType = "attachment";
                    parentmodel.EmployeeAttachment = model;
                    parentmodel.EmployeeAttachment.SideBarClassName = "selected";

                    return View("CreateOrEditPersonaInfo", parentmodel);
                }
            }
            else
            {
                populateDropdown(model);
                model.Message = Resources.ErrorMessages.InsertFailed;
                model.ErrorClass = "failed";
                model.ActionType = "CreateAttachment";
                model.ButtonText = "Save";
                model.SideBarClassName = "selected";

                var parentmodel = new PersonalViewModel();
                parentmodel.EmployeeId = model.EmployeeId;
                parentmodel.ViewType = "Attachment";
                parentmodel.EmployeeAttachment = model;
                parentmodel.EmployeeAttachment.SideBarClassName = "selected";
                return View("CreateOrEditPersonaInfo", parentmodel);
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpAttachmentList(JqGridRequest request, int empId, string controlType)
        {
            var list = _personalInfoService.PRMUnit.EmpAttachementRepository.Fetch().Where(x => x.EmployeeId == empId).ToList();
            var attachmentTypes = _empService.PRMUnit.EmpAttachementTypeRepository.Fetch().ToList();
            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "FileName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FileName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FileName).ToList();
                }
            }
            if (request.SortingName == "AttachmentType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_AttachmentType.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_AttachmentType.Name).ToList();
                }
            }

            if (request.SortingName == "Description")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Description).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Description).ToList();
                }
            }

            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.FileName,
                    item.Id,
                    attachmentTypes.Find(x=> x.Id == item.AttachmentTypeId).Name,
                    item.Description
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult EditAttachment(int id)
        {
            var entity = _personalInfoService.PRMUnit.EmpAttachementRepository.GetByID(id);
            var attachmentTypes = _empService.PRMUnit.EmpAttachementTypeRepository.Fetch().ToList();
            var model = entity.ToModel();
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
            populateDropdown(model);
            model.FileSize = Math.Round((Convert.ToDouble(entity.Attachment.LongLength) / 1024) / 1024, 2) + " MB";
            model.ActionType = "EditAttachment";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SideBarClassName = "selected";

            var parentModel = new PersonalViewModel();
            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "Attachment";
            parentModel.EmployeeAttachment = model;
            parentModel.EmployeeAttachment.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult EditAttachment([Bind(Exclude = "Attachment")]EmpAttachmentViewModel model)
        {
            var parentModel = new PersonalViewModel();
            var attachment = Request.Files["attachment"];

            if (ModelState.IsValid)
            {
                try
                {
                    // delete file from database
                    //  EmpFileUtl.DeleteFile(model.Id);

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

                            EmpFileUtl.UpdateFile(model.Id, model.EmployeeId, model.AttachmentTypeId, model.FileName, model.Description, name, contentType, size, fileData, User.Identity.Name);
                        }
                        else
                        {
                            EmpFileUtl.UpdateFile(model.Id, model.AttachmentTypeId, model.FileName, model.Description, User.Identity.Name);
                        }
                    }

                }
                catch (Exception ex)
                {
                    populateDropdown(model);
                    Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Update);
                    model.ErrorClass = "failed";

                    model.DeleteEnable = true;
                    model.ActionType = "EditAttachment";
                    model.ButtonText = "Update";
                    model.SideBarClassName = "selected";

                    parentModel.EmployeeId = model.EmployeeId;
                    parentModel.EmployeeAttachment = model;
                    return View("CreateOrEditPersonaInfo", parentModel);
                }
            }
            else
            {
                populateDropdown(model);
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                model.Message = Resources.ErrorMessages.UpdateFailed;
                model.ErrorClass = "failed";

                model.DeleteEnable = true;
                model.ActionType = "EditAttachment";
                model.ButtonText = "Update";
                model.SideBarClassName = "selected";

                parentModel.EmployeeId = model.EmployeeId;
                parentModel.EmployeeAttachment = model;
                return View("CreateOrEditPersonaInfo", parentModel);

            }

            populateDropdown(model);
            Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

            model.Message = Resources.ErrorMessages.UpdateSuccessful;
            model.ErrorClass = "success";
            //model.DeleteEnable = true;
            //model.ActionType = "EditAttachment";
            //model.ButtonText = "Update";
            //model.SideBarClassName = "selected";

            model.DeleteEnable = false;
            model.ActionType = "CreateAttachment";
            model.ButtonText = "Save";
            model.SideBarClassName = "selected";

            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "Attachment";
            parentModel.EmployeeAttachment = model;
            parentModel.EmployeeAttachment.SideBarClassName = "selected";
            //return View("CreateOrEditPersonaInfo", parentModel);

            return RedirectToAction("AttachmentIndex", "PersonalInfo", new { id = model.EmployeeId, insertResult = "success" });

            // return View("_EmpAttachment", model);
        }

        public ActionResult DeleteAttachment(int id)
        {
            var parentModel = new PersonalViewModel();
            var entity = _personalInfoService.PRMUnit.EmpAttachementRepository.GetByID(id);
            var model = entity.ToModel();
            if (ModelState.IsValid && entity != null)
            {
                try
                {
                    _empService.PRMUnit.EmpAttachementRepository.Delete(entity);
                    _empService.PRMUnit.EmpAttachementRepository.SaveChanges();
                }
                catch (Exception ex)
                {

                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, BEPZA_MEDICAL.Web.Utility.CommonAction.Delete);
                    model.ErrorClass = "failed";
                    model.ButtonText = "Update";
                    model.ActionType = "EditAttachment";
                    model.DeleteEnable = true;
                    model.SideBarClassName = "selected";


                    parentModel.EmployeeId = model.EmployeeId;
                    parentModel.ViewType = "Attachment";
                    parentModel.EmployeeAttachment = model;
                    parentModel.EmployeeAttachment.SideBarClassName = "selected";
                    return View("CreateOrEditPersonaInfo", parentModel);
                }
                return RedirectToAction("AttachmentIndex", null, new { id = entity.EmployeeId, deleteResult = "success" });
            }

            model.Message = Resources.ErrorMessages.DeleteFailed;

            model.ErrorClass = "success";
            model.DeleteEnable = false;
            model.ActionType = "CreateAttachment";
            model.ButtonText = "Save";
            model.SideBarClassName = "selected";

            parentModel.EmployeeId = model.EmployeeId;
            parentModel.ViewType = "Attachment";
            parentModel.EmployeeAttachment = model;
            parentModel.EmployeeAttachment.SideBarClassName = "selected";
            return View("CreateOrEditPersonaInfo", parentModel);

        }

        #region Utils

        public FileContentResult GetReferenceGuarantorImage(int id)
        {
            var data = _personalInfoService.PRMUnit.EmpReferenceGuarantorRepository.GetByID(id);
            if (data != null && data.Photo.Length != 0)
                return GetImage(data.Photo);
            else
                return null;
        }


        public ActionResult GetEmpAttachedFile(int id)
        // public FileContentResult GetEmpAttachedFile(int id)
        {
            var data1 = _personalInfoService.PRMUnit.EmpAttachementRepository.GetByID(id);
            //var attachmentTypes = _empService.PRMUnit.EmpAttachementTypeRepository.Fetch().ToList();

            //=======================================================

            //int id1 = Convert.ToInt16(Request.QueryString["ID"]);

            // Get the file from the database
            //DataTable file = FileUtl.GetAFile(id1);
            DataTable file = EmpFileUtl.GetAFile(id);
            DataRow row = file.Rows[0];

            string name = (string)row["OriginalFileName"];
            string contentType = (string)row["ContentType"];
            Byte[] data = (Byte[])row["Attachment"];

            // Send the file to the browser
            Response.AddHeader("Content-type", contentType);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + name);
            Response.BinaryWrite(data);
            Response.Flush();
            Response.End();

            return View("_EmpAttachment", data1.ToModel());

            //=========================================================



            //if (data != null && data.Attachment.Length != 0)
            //{
            //    var cd = new System.Net.Mime.ContentDisposition
            //    {
            //        FileName = data.OriginalFileName.Trim(),
            //        Inline = false,
            //    };
            //    Response.AppendHeader("Content-Disposition", cd.ToString());
            //    return File(data.Attachment, data.FileExtention);

            //}
            //else
            //    return null;
        }

        private FileContentResult GetImage(byte[] photo)
        {
            if (photo.Length != 0)
                return File(photo, "image/jpeg");
            else
                return null;
        }

        #endregion

        private void populateDropdown(EmpAttachmentViewModel model)
        {
            #region Title
            var list = _empService.PRMUnit.EmpAttachementTypeRepository.Fetch().OrderBy(q => q.SortOrder).ToList();
            model.AttachmentTypeList = Common.PopulateDllList(list);
            #endregion
        }

        #endregion

        #region OtherAction
        public JsonResult DuplicateCheck(string visaPass, int countryId)
        {
            if (_personalInfoService.CheckDuplicateVisaPassport(visaPass, countryId))
                return Json(new { data = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { data = false }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Leverage Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpLeverageList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllLeverageByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "ItemName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ItemName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ItemName).ToList();
                }
            }
            if (request.SortingName == "ItemDescription")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ItemDescription).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ItemDescription).ToList();
                }
            }

            if (request.SortingName == "IssueDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IssueDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IssueDate).ToList();
                }
            }

            if (request.SortingName == "ItemQnty")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ItemQnty).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ItemQnty).ToList();
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
                    item.ItemName,
                    item.ItemDescription,
                    item.IssueDate == null ? "":   Convert.ToDateTime(item.IssueDate).ToString("dd-MM-yyyy"),                  
                    item.ItemQnty
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult EmpLeverageIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.EmpLeverageViewModel;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.EmpLeverageRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
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

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;

                parentModel.ViewType = "EmpLeverage";

                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }
            }

            parentModel.EmpLeverageViewModel = model;
            parentModel.EmpLeverageViewModel.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.EmpLeverageViewModel.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditEmpLeverage(EmpLeverageViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

                if (ModelState.IsValid)
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = Common.CurrentDateTime;
                    var entity = model.ToEntity();

                    //entity.EUser = User.Identity.Name;
                    //entity.EDate = Common.CurrentDateTime;
                    //businessError = _personalInfoService.CheckBusinessLogicJOBSKL(entity);

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "EmpLeverage";
                        parentModel.EmpLeverageViewModel = model;

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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
                        entity.IUser = User.Identity.Name;
                        entity.IDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.EmpLeverageRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        entity.EUser = User.Identity.Name;
                        entity.EDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.EmpLeverageRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.EmpLeverageRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                parentModel.ViewType = "EmpLeverage";
                parentModel.EmpLeverageViewModel = model;
                parentModel.EmpLeverageViewModel.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
            catch (Exception ex)
            {
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
                if (ex.InnerException.Message.Contains("IX_PRM_EmpJobSkill"))
                {
                    model.Message = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                }
                parentModel.ViewType = "EmpLeverage";
                parentModel.EmpLeverageViewModel = model;
                parentModel.EmpLeverageViewModel.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteLeverage(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "EmpLeverage";

            var entity = _personalInfoService.PRMUnit.EmpLeverageRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.EmpLeverageRepository.Delete(entity);

                    _personalInfoService.PRMUnit.LicenseRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";

                    return RedirectToAction("EmpLeverageIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.EmpLeverageViewModel = model;
                }
            }
            catch
            {
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.EmpLeverageViewModel = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        #endregion

        #region Service History


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetServiceHistoryList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Id)
        {
            var list = _personalInfoService.GetAllServiceHistoryByEmployeeID(viewModel.ID.HasValue ? viewModel.ID.Value : 0).ToList();

            var totalRecords = list.Count();

            #region Sorting

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
            if (request.SortingName == "Office")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Office).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Office).ToList();
                }
            }


            //if (request.SortingName == "EmploymentProcess")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.EmploymentProcess).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.EmploymentProcess).ToList();
            //    }
            //}

            if (request.SortingName == "EffectiveDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveDate).ToList();
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
            //if (request.SortingName == "Section")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.Section).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.Section).ToList();
            //    }
            //}

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
                    item.Office,
                    item.Designation,
                    item.Department,
                    Convert.ToDateTime(item.EffectiveDate).ToString(DateAndTime.GlobalDateFormat)  
                  
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ServiceHistoryIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.EmpServiceHistoryViewModel;

            //if (id.HasValue)
            //{
            //    //var entity = _personalInfoService.PRMUnit.EmpServiceHistoryRepository.GetByID(id.Value);
            //    if (IsMenu)
            //    {
            //        model.strMode = "add";
            //        model.EmployeeId = Convert.ToInt32(id);
            //        model.DeleteEnable = false;
            //        model.ButtonText = "Save";
            //    }
            //    else
            //    {
            //        if (entity == null)
            //        {
            //            model.strMode = "add";
            //            model.EmployeeId = Convert.ToInt32(id);
            //            model.DeleteEnable = false;
            //            model.ButtonText = "Save";
            //        }
            //        else
            //        {

            //            model = entity.ToModel();
            //            model.OrganogramLevelName = entity.PRM_OrganogramLevel == null ? String.Empty : entity.PRM_OrganogramLevel.LevelName;

            //            model.DeleteEnable = true;
            //            model.ButtonText = "Update";
            //            model.strMode = "edit";
            //        }
            //    }

            //    var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? parentModel.EmployeeId : id.Value);
            parentModel.EmployeeId = id.Value;
            parentModel.ViewType = "EmpServiceHisotory";

            //    if (empEntity != null)
            //    {
            //        Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
            //    }

            //}
            parentModel.EmpServiceHistoryViewModel.EmployeeId = id.Value;
            parentModel.EmpServiceHistoryViewModel = model;
            //PopulateDropdownListServiceHistory(model);
            //InitializationDesignationForEdit(model);
            //parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

            //if (type == "success")
            //{
            //    parentModel.EmpServiceHistoryViewModel.Message = Resources.ErrorMessages.DeleteSuccessful;
            //    model.errClass = "success";

            //}

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        //[HttpPost]
        //public ActionResult CreateOrEditServiceHistory(EmpServiceHistoryViewModel model)
        //{

        //    string businessError = string.Empty;
        //    var parentModel = new PersonalViewModel();

        //    parentModel.EmployeeId = model.EmployeeId;
        //    try
        //    {
        //        Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);

        //        if (ModelState.IsValid)
        //        {
        //            model.IUser = User.Identity.Name;
        //            model.IDate = Common.CurrentDateTime;
        //            var entity = model.ToEntity();
        //            if (businessError != string.Empty)
        //            {
        //                model.Message = businessError;
        //                model.errClass = "failed";
        //                parentModel.ViewType = "EmpServiceHisotory";
        //                parentModel.EmpServiceHistoryViewModel = model;
        //                PopulateDropdownListServiceHistory(model);
        //                if (model.strMode == "add")
        //                {
        //                    model.DeleteEnable = false;
        //                    model.ButtonText = "Save";
        //                }
        //                else
        //                {
        //                    model.DeleteEnable = true;
        //                    model.ButtonText = "Update";
        //                }
        //                return View("CreateOrEditPersonaInfo", parentModel);
        //            }

        //            if (model.strMode == "add")
        //            {
        //                entity.IUser = User.Identity.Name;
        //                entity.IDate = Common.CurrentDateTime;
        //                _personalInfoService.PRMUnit.EmpServiceHistoryRepository.Add(entity);
        //                model.Message = Resources.ErrorMessages.InsertSuccessful;
        //                model.errClass = "success";
        //                model.DeleteEnable = false;
        //                model.ButtonText = "Save";
        //            }
        //            else
        //            {
        //                entity.EUser = User.Identity.Name;
        //                entity.EDate = Common.CurrentDateTime;
        //                _personalInfoService.PRMUnit.EmpServiceHistoryRepository.Update(entity);

        //                model.Message = Resources.ErrorMessages.UpdateSuccessful;
        //                model.errClass = "success";
        //                model.DeleteEnable = true;
        //                model.ButtonText = "Save";
        //            }
        //            _personalInfoService.PRMUnit.EmpServiceHistoryRepository.SaveChanges();
        //            PropertyReflector.ClearProperties(model);
        //            ModelState.Clear();
        //        }
        //        else
        //        {
        //            model.Message = Common.ValidationSummaryHead;
        //            model.errClass = "failed";

        //            ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
        //        }

        //        PopulateDropdownListServiceHistory(model);
        //        InitializationDesignationForEdit(model);

        //        parentModel.ViewType = "EmpServiceHisotory";
        //        parentModel.EmpServiceHistoryViewModel = model;
        //        parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

        //        return View("CreateOrEditPersonaInfo", parentModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        PopulateDropdownListServiceHistory(model);
        //        InitializationDesignationForEdit(model);

        //        if (model.strMode == "add")
        //        {
        //            model.Message = Resources.ErrorMessages.InsertFailed;
        //            model.errClass = "failed";
        //        }
        //        else if (model.strMode == "edit")
        //        {
        //            model.Message = Resources.ErrorMessages.UpdateFailed;
        //            model.errClass = "failed";
        //            model.ButtonText = "Update";
        //        }
        //        if (ex.InnerException.Message.Contains("IX_PRM_EmpJobSkill"))
        //        {
        //            model.Message = Resources.ErrorMessages.UniqueIndex;
        //            model.errClass = "failed";
        //        }
        //        parentModel.ViewType = "EmpServiceHisotory";
        //        parentModel.EmpServiceHistoryViewModel = model;
        //        parentModel.EmpServiceHistoryViewModel.SideBarClassName = "selected";

        //        return View("CreateOrEditPersonaInfo", parentModel);
        //    }
        //}

        //public ActionResult DeleteServiceHistory(int id)
        //{
        //    var parentModel = new PersonalViewModel();
        //    parentModel.ViewType = "EmpServiceHisotory";

        //    var entity = _personalInfoService.PRMUnit.EmpServiceHistoryRepository.GetByID(id);

        //    var model = entity.ToModel();

        //    try
        //    {
        //        if (ModelState.IsValid && entity != null)
        //        {
        //            _personalInfoService.PRMUnit.EmpServiceHistoryRepository.Delete(entity);
        //            _personalInfoService.PRMUnit.LicenseRepository.SaveChanges();
        //            model.Message = Resources.ErrorMessages.DeleteSuccessful;
        //            model.errClass = "success";


        //            return RedirectToAction("ServiceHistoryIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
        //        }
        //        else
        //        {
        //            PopulateDropdownListServiceHistory(model);
        //            model.Message = Resources.ErrorMessages.DeleteFailed;
        //            model.errClass = "failed";

        //            parentModel.EmpServiceHistoryViewModel = model;
        //        }

        //    }
        //    catch
        //    {
        //        PopulateDropdownListServiceHistory(model);
        //        model.Message = Resources.ErrorMessages.DeleteFailed;
        //        model.errClass = "failed";

        //        parentModel.EmpServiceHistoryViewModel = model;
        //    }

        //    return View("CreateOrEditPersonaInfo", parentModel);
        //}

        private EmpServiceHistoryViewModel InitializationDesignationForEdit(EmpServiceHistoryViewModel model)
        {
            var desigList = (from JG in _empService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Fetch()
                             join de in _empService.PRMUnit.DesignationRepository.Fetch() on JG.DesignationId equals de.Id
                             where JG.OrganogramLevelId == model.OrganogramLevelId
                             select de).OrderBy(o => o.Name).ToList();

            model.DesignationList = Common.PopulateDllList(desigList);

            return model;
        }


        #endregion

        #region Foreign Tour Information

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetForeignTourList(JqGridRequest request, ForeignTourInfoViewModel viewModel, int Id)
        {
            var list = _personalInfoService.PRMUnit.ForeignTourInfoRepository.Fetch().Where(x => x.EmployeeId == Id).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "TitleOfTheTour")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TitleOfTheTour).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TitleOfTheTour).ToList();
                }
            }

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_Country.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_Country.Name).ToList();
                }
            }

            if (request.SortingName == "Financed")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Financed).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Financed).ToList();
                }
            }

            if (request.SortingName == "VisitDateFrom")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.VisitDateFrom).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.VisitDateFrom).ToList();
                }
            }

            if (request.SortingName == "VisitDateTo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.VisitDateTo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.VisitDateTo).ToList();
                }
            }

            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.TitleOfTheTour,
                    item.PRM_Country.Name,
                    item.Financed,
                    (Convert.ToDateTime(item.VisitDateFrom)).ToString("dd-MMM-yyyy"),
                    (Convert.ToDateTime(item.VisitDateTo)).ToString("dd-MMM-yyyy")
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ForeignTourInfoIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.ForeignTourInfoViewModel;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.ForeignTourInfoRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        model = entity.ToModel();
                        DownloadDoc(model);
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;
                parentModel.ViewType = "ForeignTourInfo";
                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }
            }

            parentModel.ForeignTourInfoViewModel = model;
            populateCountryDDL(model);
            parentModel.ForeignTourInfoViewModel.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.ForeignTourInfoViewModel.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditForeignTourInfo([Bind(Exclude = "Attachment")] ForeignTourInfoViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            var attachment = Request.Files["attachment"];

            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "ForeignTourInfo";
                        parentModel.ForeignTourInfoViewModel = model;
                        populateCountryDDL(model);
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
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
                        _personalInfoService.PRMUnit.ForeignTourInfoRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        var obj = _personalInfoService.PRMUnit.ForeignTourInfoRepository.GetAll().Where(x => x.Id == model.Id).FirstOrDefault();
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
                        entity.EDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.ForeignTourInfoRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.ForeignTourInfoRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                populateCountryDDL(model);

                parentModel.ViewType = "ForeignTourInfo";
                parentModel.ForeignTourInfoViewModel = model;
                parentModel.ForeignTourInfoViewModel.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
            catch (Exception ex)
            {
                populateCountryDDL(model);

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

                parentModel.ViewType = "ForeignTourInfo";
                parentModel.ForeignTourInfoViewModel = model;
                parentModel.ForeignTourInfoViewModel.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteForeignTourInfo(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "ForeignTourInfo";

            var entity = _personalInfoService.PRMUnit.ForeignTourInfoRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.ForeignTourInfoRepository.Delete(entity);

                    _personalInfoService.PRMUnit.ForeignTourInfoRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("ForeignTourInfoIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    populateCountryDDL(model);
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.ForeignTourInfoViewModel = model;
                }

            }
            catch
            {
                populateCountryDDL(model);
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.ForeignTourInfoViewModel = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        public void populateCountryDDL(ForeignTourInfoViewModel model)
        {
            #region Country ddl

            var list = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(list);

            #endregion
        }

        #endregion

        #region Life Insurance

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetLifeInsuranceList(JqGridRequest request, LifeInsuranceViewModel viewModel, int Id)
        {
            var list = _personalInfoService.PRMUnit.LifeInsuranceRepository.Fetch().Where(x => x.EmployeeId == Id).ToList();

            var totalRecords = list.Count();

            #region Sorting

            if (request.SortingName == "DateOfDeath")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateOfDeath).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateOfDeath).ToList();
                }
            }
            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.DateOfDeath.ToString("dd-MMM-yyyy"),
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult LifeInsuranceIndex(int? id, bool IsMenu = false, string type = "")
        {
            var parentModel = new PersonalViewModel();
            var model = parentModel.LifeInsuranceViewModel;

            if (id.HasValue)
            {
                var entity = _personalInfoService.PRMUnit.LifeInsuranceRepository.GetByID(id.Value);

                if (IsMenu)
                {
                    model.strMode = "add";
                    model.EmployeeId = Convert.ToInt32(id);
                    model.DeleteEnable = false;
                    model.ButtonText = "Save";
                }
                else
                {
                    if (entity == null)
                    {
                        model.strMode = "add";
                        model.EmployeeId = Convert.ToInt32(id);
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        model = entity.ToModel();
                        DownloadDocForLifeInsurance(model);
                        model.DeleteEnable = true;
                        model.ButtonText = "Update";
                        model.strMode = "edit";
                    }
                }

                var empEntity = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.strMode == "edit" ? entity.EmployeeId : id);
                parentModel.EmployeeId = empEntity.Id;
                var empTemp = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == empEntity.Id).FirstOrDefault();
                model.DateOfBirth = (Convert.ToDateTime(empTemp.DateofBirth)).ToString("yyyy-MM-dd");

                parentModel.ViewType = "LifeInsurance";
                if (empEntity != null)
                {
                    Common.PopulateEmployeeTop(model.EmpTop, empEntity.Id, _empService);
                }
            }

            parentModel.LifeInsuranceViewModel = model;
            parentModel.LifeInsuranceViewModel.SideBarClassName = "selected";

            if (type == "success")
            {
                parentModel.LifeInsuranceViewModel.Message = Resources.ErrorMessages.DeleteSuccessful;
                model.errClass = "success";

            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }

        [HttpPost]
        public ActionResult CreateOrEditLifeInsurance([Bind(Exclude = "Attachment")] LifeInsuranceViewModel model)
        {
            string businessError = string.Empty;
            var parentModel = new PersonalViewModel();

            parentModel.EmployeeId = model.EmployeeId;
            var attachment = Request.Files["attachment"];

            try
            {
                Common.PopulateEmployeeTop(model.EmpTop, model.EmployeeId, _empService);
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    if (businessError != string.Empty)
                    {
                        model.Message = businessError;
                        model.errClass = "failed";
                        parentModel.ViewType = "LifeInsurance";
                        parentModel.LifeInsuranceViewModel = model;
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
                        return View("CreateOrEditPersonaInfo", parentModel);
                    }
                    if (model.strMode == "add")
                    {
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
                        _personalInfoService.PRMUnit.LifeInsuranceRepository.Add(entity);
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = false;
                        model.ButtonText = "Save";
                    }
                    else
                    {
                        var obj = _personalInfoService.PRMUnit.LifeInsuranceRepository.GetAll().Where(x => x.Id == model.Id).FirstOrDefault();
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
                        entity.EDate = Common.CurrentDateTime;
                        _personalInfoService.PRMUnit.LifeInsuranceRepository.Update(entity);

                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        model.errClass = "success";
                        model.DeleteEnable = true;
                        model.ButtonText = "Save";
                    }
                    _personalInfoService.PRMUnit.LifeInsuranceRepository.SaveChanges();
                    PropertyReflector.ClearProperties(model);
                    ModelState.Clear();
                }
                else
                {
                    model.Message = Common.ValidationSummaryHead;
                    model.errClass = "failed";

                    ModelState.AddModelError(Resources.ErrorMessages.InsertFailed.ToString(), Resources.ErrorMessages.InsertFailed);
                }

                parentModel.ViewType = "LifeInsurance";
                parentModel.LifeInsuranceViewModel = model;
                parentModel.LifeInsuranceViewModel.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
            catch (Exception ex)
            {
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

                parentModel.ViewType = "LifeInsurance";
                parentModel.LifeInsuranceViewModel = model;
                parentModel.LifeInsuranceViewModel.SideBarClassName = "selected";

                return View("CreateOrEditPersonaInfo", parentModel);
            }
        }

        public ActionResult DeleteLifeInsurance(int id)
        {
            var parentModel = new PersonalViewModel();
            parentModel.ViewType = "LifeInsurance";

            var entity = _personalInfoService.PRMUnit.LifeInsuranceRepository.GetByID(id);

            var model = entity.ToModel();

            try
            {
                if (ModelState.IsValid && entity != null)
                {
                    _personalInfoService.PRMUnit.LifeInsuranceRepository.Delete(entity);

                    _personalInfoService.PRMUnit.LifeInsuranceRepository.SaveChanges();
                    model.Message = Resources.ErrorMessages.DeleteSuccessful;
                    model.errClass = "success";


                    return RedirectToAction("LifeInsuranceIndex", null, new { id = entity.EmployeeId, IsMenu = true, type = "success" });
                }
                else
                {
                    model.Message = Resources.ErrorMessages.DeleteFailed;
                    model.errClass = "failed";

                    parentModel.LifeInsuranceViewModel = model;
                }

            }
            catch
            {
                model.Message = Resources.ErrorMessages.DeleteFailed;
                model.errClass = "failed";

                parentModel.LifeInsuranceViewModel = model;
            }

            return View("CreateOrEditPersonaInfo", parentModel);
        }
        #endregion

        #region Autocomplete For Membership ID

        public JsonResult AutoCompleteMembershipList(string term)
        {
            var result = (from r in _PRMService.PRMUnit.EmploymentInfoRepository.GetAll()
                          join d in _PRMService.PRMUnit.DesignationRepository.GetAll() on r.DesignationId equals d.Id
                          join p in _PRMService.PRMUnit.PersonalInfoRepository.GetAll() on r.Id equals p.EmployeeId
                          where r.EmpID != null && r.EmpID.ToLower().StartsWith(term.ToLower())
                          select new { r.EmpID, d.Name, r.FullName, p.PresentAddress1 }).Distinct().OrderBy(x => x.EmpID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetMembershipInformation(string empID)
        {
            string msg = string.Empty;
            var obj = (from r in _PRMService.PRMUnit.EmploymentInfoRepository.GetAll()
                       join d in _PRMService.PRMUnit.DesignationRepository.GetAll() on r.DesignationId equals d.Id
                       join p in _PRMService.PRMUnit.PersonalInfoRepository.GetAll() on r.Id equals p.EmployeeId
                       where r.EmpID == empID
                       select new { r.Id, r.EmpID, d.Name, r.FullName, p.PermanentAddress1 }).FirstOrDefault();
            //if (obj != null && obj.ApprovalStatusId != 4 && obj. != "Active")
            //{
            //    msg = "Inactive";
            //}
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    if (obj != null)
                    {
                        return Json(new
                        {
                            Id = obj.Id,
                            WitnessID = obj.EmpID,
                            WitnessName = obj.FullName,
                            WitnessAddress = obj.PermanentAddress1,
                            WitnessDesignation = obj.Name
                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        return Json(new { Result = false });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Result = msg
                });
            }
        }
        #endregion

        #endregion

        #region Private Method

        private void PopulateDropdownList(PersonalInfoViewModel model)
        {
            dynamic ddlList;

            #region Gender ddl

            // model.GenderList = Common.PopulateGenderDDL(model.GenderList);

            #endregion

            #region Marital Status ddl

            ddlList = _empService.PRMUnit.MaritalStatusRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.MaritalStatusList = Common.PopulateDllList(ddlList);

            #endregion

            #region Blood group ddl

            ddlList = _empService.PRMUnit.BloodGroupRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.BloodGroupList = Common.PopulateDllList(ddlList);

            #endregion

            #region Country of Birth ddl

            ddlList = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.CountryofBirthList = Common.PopulateDllList(ddlList);

            #endregion

            #region Nationality ddl

            ddlList = _empService.PRMUnit.NationalityRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.NationalityList = Common.PopulateDllList(ddlList);
            #endregion

            #region Present Country ddl

            ddlList = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.PresentCountryList = Common.PopulateDllList(ddlList);

            #endregion

            #region Present District ddl

            ddlList = _personalInfoService.PopulateDistrictByCountryID(model.PresentCountryId).OrderBy(x => x.DistrictName);
            model.PresentDistictList = Common.PopulateDistrictDDL(ddlList);

            #endregion

            #region Present Thana ddl

            ddlList = _empService.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == model.PresentDistictId).OrderBy(x => x.ThanaName).ToList();
            model.PresentThanaList = Common.PopulateThanDDL(ddlList);

            #endregion

            #region Permanent Country ddl

            ddlList = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.PermanentCountryList = Common.PopulateDllList(ddlList);

            #endregion

            #region Permanent District ddl

            ddlList = _personalInfoService.PopulateDistrictByCountryID(model.PermanentCountryId).OrderBy(x => x.DistrictName);
            model.PermanentDistictList = Common.PopulateDistrictDDL(ddlList);

            #endregion

            #region Permanent Thana ddl

            ddlList = _empService.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == model.PermanentDistictId).OrderBy(x => x.ThanaName).ToList();
            model.PermanentThanaList = Common.PopulateThanDDL(ddlList);

            #endregion

            #region Profession
            ddlList = _empService.PRMUnit.ProfessionRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.FatherProfessionList = Common.PopulateDllList(ddlList);
            model.MotherProfessionList = Common.PopulateDllList(ddlList);
            #endregion
        }

        private void PopulateDropdownListACC(AccademicQlfnInfoViewModel model)
        {
            dynamic ddlList;

            #region Exam/Level ddl

            ddlList = _empService.PRMUnit.ExamDegreeLavelRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.ExamLavelList = Common.PopulateDllList(ddlList);

            #endregion

            #region Passing Year List

            model.YearOfPassingList = Common.PopulateYearList();

            #endregion

            #region Result ddl

            ddlList = _empService.PRMUnit.AcademicGradeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.ResultList = Common.PopulateDllList(ddlList);

            #endregion

            #region Institute ddl

            ddlList = _empService.PRMUnit.UniversityAndBoardRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.UniversityAndBoardList = Common.PopulateDllList(ddlList);

            #endregion

            #region Subject/Group ddl

            ddlList = _empService.PRMUnit.SubjectGroupRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.SubjectGroupList = Common.PopulateDllList(ddlList);

            #endregion

            #region Country ddl

            ddlList = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(ddlList);

            #endregion
        }

        private void PopulateDropdownListJOBEXP(JobExperienceInfoViewModel model)
        {
            dynamic ddlList;

            #region Exam/Level ddl

            ddlList = _empService.PRMUnit.EmploymentTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.EmployeeTypeList = Common.PopulateDllList(ddlList);

            #endregion

            #region Result ddl

            ddlList = _empService.PRMUnit.OrganizationTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.OrganizationTypeList = Common.PopulateDllList(ddlList);

            #endregion

            #region Grade ddl
            model.JobGradeList = Common.PopulateJobGradeDDL(_JobGradeService.GetLatestJobGrade());
            #endregion

        }

        private void PopulateDropdownListPROTRA(ProfessionalTrainingInfoViewModel model)
        {
            dynamic ddlList;

            #region Country ddl

            ddlList = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(ddlList);

            #endregion

            // Training Year          
            model.TrainingYearList = Common.PopulateYearList();

            //Training Type
            ddlList = _empService.PRMUnit.TrainingTypeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.TrainingTypeList = Common.PopulateDllList(ddlList);

            //Academic Grade
            ddlList = _empService.PRMUnit.AcademicGradeRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.AcademicGradeList = Common.PopulateDllList(ddlList);

            //Location
            ddlList = _empService.PRMUnit.LocationRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.LocationList = Common.PopulateDllList(ddlList);
        }

        private void PopulateDropdownListPROCER(ProfessionalCertificationInfoViewModel model)
        {
            dynamic ddlList;

            #region Certification Category ddl

            ddlList = _empService.PRMUnit.CertificationCategoryRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.CertificationCategoryList = Common.PopulateDllList(ddlList);

            #endregion

            #region Year
            model.CertificationYearList = Common.PopulateYearList();
            #endregion

            #region Institute ddl

            ddlList = _empService.PRMUnit.CertificationInstituteRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.InstituteList = Common.PopulateDllList(ddlList);

            #endregion

            #region Country ddl

            ddlList = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(ddlList);

            #endregion
        }

        private void PopulateDropdownListLICEN(ProfessionalLicenseInfoViewModel model)
        {
            dynamic ddlList;

            #region Certification Category ddl

            ddlList = _empService.PRMUnit.LicenseTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.LicensTypeList = Common.PopulateDllList(ddlList);

            #endregion

            #region Country ddl

            ddlList = _empService.PRMUnit.CountryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(ddlList);

            #endregion

            #region License Category ddl

            ddlList = _empService.PRMUnit.LicenseCategoryRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.LicenseCategoryList = Common.PopulateDllList(ddlList);

            #endregion

        }

        private void PopulateDropdownListJOBSKIL(JobSkillInfoViewModel model)
        {
            dynamic ddlList;

            #region Skill Name ddl

            ddlList = _empService.PRMUnit.JobSkillNameRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.JobSkillList = Common.PopulateDllList(ddlList);

            #endregion

            #region Skill Level ddl

            ddlList = _empService.PRMUnit.JobSkillLevelRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.JobSkillLeveList = Common.PopulateDllList(ddlList);

            #endregion
        }

        private void PopulateDropdownListWealthStatementInfo(EmployeeWealthStatementViewModel model)
        {
            dynamic ddlList;

            #region Asset Type ddl

            ddlList = _empService.PRMUnit.AssetTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.AssetTypeList = Common.PopulateDllList(ddlList);

            #endregion
        }

        #region Service History DDL
        private void PopulateDropdownListServiceHistory(EmpServiceHistoryViewModel model)
        {
            dynamic ddlList;

            #region Designation ddl

            //ddlList = _empService.PRMUnit.DesignationRepository.Fetch().OrderBy(x => x.Name).ToList();
            //model.DesignationList = Common.PopulateDllList(ddlList);

            #endregion

            #region Employment Process ddl

            ddlList = _empService.PRMUnit.EmploymentProcessRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.EmploymentProcessList = Common.PopulateDllList(ddlList);

            #endregion

            #region Employment Type ddl

            ddlList = _empService.PRMUnit.EmploymentTypeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.EmploymentTypeList = Common.PopulateDllList(ddlList);

            #endregion

            #region Employee Class ddl

            ddlList = _empService.PRMUnit.EmployeeClassRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.EmployeeClassList = Common.PopulateDllList(ddlList);

            #endregion

            #region Employee staff ddl

            ddlList = _empService.PRMUnit.PRM_StaffCategoryRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.StaffCategoryList = Common.PopulateDllList(ddlList);

            #endregion

            #region Salary Scale ddl

            ddlList = _empService.PRMUnit.SalaryScaleRepository.Fetch().OrderBy(x => x.SalaryScaleName).ToList();
            model.SalaryScaleIdList = PopulateSalaryScaleList(ddlList);

            #endregion

            #region Grade ddl

            model.JobGradeList = Common.PopulateJobGradeDDL(_empService.PRMUnit.JobGradeRepository.Get(t => t.SalaryScaleId == model.SalaryScaleId).ToList());

            #endregion

            //if (model.OrganogramLevelId != null && model.OrganogramLevelId != 0)
            //{
            //    model.OrganogramLevelDetail = _empService.GetOrganogramHierarchyLabel(Convert.ToInt32(model.OrganogramLevelId));
            //}

        }

        private IList<SelectListItem> PopulateSalaryScaleList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.SalaryScaleName,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text.Trim()).ToList();
        }

        public ActionResult GetJobGrade(int salaryScaleId)
        {
            return Json(
                PopulateJobGradeList(salaryScaleId).Select(x => new { Id = x.Value, GradeName = x.Text }),
                JsonRequestBehavior.AllowGet
            );
        }


        private IList<SelectListItem> PopulateJobGradeList(int salaryScaleId)
        {
            IList<PRM_JobGrade> itemList;
            itemList = _empService.PRMUnit.JobGradeRepository.Get(q => q.SalaryScaleId == salaryScaleId).OrderBy(o => o.GradeName, new AlphanumericSorting()).ToList();

            var list = new List<SelectListItem>();
            foreach (var item in itemList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.GradeName,
                    Value = item.Id.ToString()
                });
            }

            return list.ToList();
        }

        public JsonResult GetSalaryScaleName(int Id)
        {
            string salaryScaleName = "";
            // var retVal = _empService.PRMUnit.JobGradeRepository.Get(q => q.Id == Id).ToList();
            var item2 = _empService.PRMUnit.JobGradeRepository.GetAll().FirstOrDefault(q => q.Id == Id);
            //foreach (var item in retVal)
            //{
            //    salaryScaleName = item.PayScale.ToString();              
            //}

            if (item2.PayScale != null && item2.PayScale != "")
            {
                salaryScaleName = item2.PayScale.ToString();
            }
            return Json(new { SalaryScaleName = salaryScaleName }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Alphanumeric Sorting ascending
        /// Added  by Suman
        /// </summary>
        public class AlphanumericSorting : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int nIndexX = x.Replace(":", " ").IndexOf(" ");
                int nIndexY = y.Replace(":", " ").IndexOf(" ");
                bool bSpaceX = nIndexX > -1;
                bool bSpaceY = nIndexY > -1;

                long nX;
                long nY;
                if (bSpaceX && bSpaceY)
                {
                    if (long.TryParse(x.Substring(0, nIndexX).Replace(".", ""), out nX)
                        && long.TryParse(y.Substring(0, nIndexY).Replace(".", ""), out nY))
                    {
                        if (nX < nY)
                        {
                            return -1;
                        }
                        else if (nX > nY)
                        {
                            return 1;
                        }
                    }
                }
                else if (bSpaceX)
                {
                    if (long.TryParse(x.Substring(0, nIndexX).Replace(".", ""), out nX)
                        && long.TryParse(y, out nY))
                    {
                        if (nX < nY)
                        {
                            return -1;
                        }
                        else if (nX > nY)
                        {
                            return 1;
                        }
                    }
                }
                else if (bSpaceY)
                {
                    if (long.TryParse(x, out nX)
                        && long.TryParse(y.Substring(0, nIndexY).Replace(".", ""), out nY))
                    {
                        if (nX < nY)
                        {
                            return -1;
                        }
                        else if (nX > nY)
                        {
                            return 1;
                        }
                    }
                }
                else
                {
                    if (long.TryParse(x, out nX)
                        && long.TryParse(y, out nY))
                    {
                        if (nX < nY)
                        {
                            return -1;
                        }
                        else if (nX > nY)
                        {
                            return 1;
                        }
                    }
                }
                return x.CompareTo(y);
            }
        }
        #endregion

        #endregion

        #region Attachment For Foreign Tour

        private int Upload(ForeignTourInfoViewModel model)
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

            }
            catch (Exception ex)
            {
            }

            return 1;
        }

        public void DownloadDoc(ForeignTourInfoViewModel model)
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

        public void DownloadDocForLifeInsurance(LifeInsuranceViewModel model)
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

        #endregion

    }
}
