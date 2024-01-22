using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using MyNotificationLib.Operation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.PGM;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class EmployeeTransferInfoController : BaseController
    {
        #region Fields

        private readonly EmployeeConfirmIncrementPromotionService _Service;
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly JobGradeService _JobGradeService;
        private readonly PRM_ExecuteFunctions _executeFunction;
        private readonly PGMCommonService _pgmCommonService;

        #endregion

        #region Constructor

        public EmployeeTransferInfoController(EmployeeConfirmIncrementPromotionService service
            , EmployeeService empService
            , PRMCommonSevice prmCommonservice
            , JobGradeService jobGradeService
            , PRM_ExecuteFunctions executeFunction
            , PGMCommonService pgmCommonService)
        {
            this._Service = service;
            this._empService = empService;
            this._prmCommonservice = prmCommonservice;
            this._JobGradeService = jobGradeService;
            this._executeFunction = executeFunction;
            this._pgmCommonService = pgmCommonService;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EmployeeConfirmationIncrementPromotionSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }


            var list = _Service.PRMUnit.EmpStatusChangeRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1).ToList();
            //list = list.Where(q => q.FromZoneInfoId == LoggedUserZoneInfoId).ToList();
            list = list.Where(q => q.Type == PRMEnum.EmployeeStatusChange.Transfer.ToString()).ToList();

            totalRecords = list == null ? 0 : list.Count;

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),

                //Page number
                PageIndex = request.PageIndex,

                //Total records count
                TotalRecordsCount = totalRecords
            };

            #region sorting

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

            if (request.SortingName == "EmpId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_EmploymentInfo.EmpID).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_EmploymentInfo.EmpID).ToList();
                }
            }

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_EmploymentInfo.FullName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_EmploymentInfo.FullName).ToList();
                }
            }

            if (request.SortingName == "FromZoneInfoId")
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

            if (request.SortingName == "ToZoneInfoId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_ZoneInfo1.ZoneName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_ZoneInfo1.ZoneName).ToList();
                }
            }

            if (request.SortingName == "FromDesignationId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_Designation.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_Designation.Name).ToList();
                }
            }

            if (request.SortingName == "ToDesignationId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_Designation1.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_Designation1.Name).ToList();
                }
            }

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

            #endregion

            foreach (PRM_EmpStatusChange d in list)
            {
                int isEditable = 1;
                var lst = list.Where(q => q.EmployeeId == d.EmployeeId).ToList();
                if (lst.Count > 1)
                {
                    var item = lst.OrderByDescending(q => q.EffectiveDate).ThenByDescending(y => y.Id).FirstOrDefault();
                    isEditable = item.Id == d.Id ? 1 : 0;
                }

                // d.PRM_EmploymentInfo.EmployeeInitial,

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.PRM_EmploymentInfo.EmpID,
                    d.PRM_EmploymentInfo.FullName,
                    d.PRM_ZoneInfo.ZoneName,
                    d.PRM_ZoneInfo1.ZoneName,
                    d.PRM_Designation.Name,
                    d.PRM_Designation1.Name,
                    d.EffectiveDate.Date.ToString(DateAndTime.GlobalDateFormat),
                    isEditable,
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Type)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (Type == 1)
                filterExpression = "IsContractual=true";
            else if (Type == 0)
                filterExpression = "IsContractual=false";

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
                viewModel.EmployeeStatus,
                1,
                viewModel.ZoneInfoId,
                out totalRecords
                )
               // .Where(q => q.EmpTypeName == "Permanent")
                .ToList();

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
                    item.ZoneInfoId,
                    item.EmpName,
                    item.ID,
                    item.EmpId,
                    item.DesigName,
                    item.EmpTypeName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,

                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new EmployeeConfirmationIncrementPromotionViewModel();
            model.Type = PRMEnum.EmployeeStatusChange.Transfer.ToString();
            populateDropdown(model);
            model.IsAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] EmployeeConfirmationIncrementPromotionViewModel model, FormCollection form)
        {
            ApplyBusinessRules(model);

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();

                entity.IUser = User.Identity.Name;
                entity.IDate = Common.CurrentDateTime;
                model.IsError = 1;

                #region Attachment

                if (model.IsAddAttachment)
                {
                    HttpFileCollectionBase files = Request.Files;
                    entity = ToAttachFile(entity, files);
                }
                else
                {
                    entity.Attachment = null;
                    entity.FileName = null;
                    entity.IsAddAttachment = false;
                }

                #endregion

                // model.ErrMsg = _Service.GetBusinessLogicValidation(entity).FirstOrDefault();
                model.ErrMsg = _Service.GetBusinessLogicValidation(entity);

                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    try
                    {
                        if (CreateEmpStatusChangeDetails(entity, model) == false)
                        {
                            throw new Exception();
                        }

                        _Service.PRMUnit.EmpStatusChangeRepository.Add(entity);
                        _Service.PRMUnit.EmpStatusChangeRepository.SaveChanges();

                        if (model.EffectiveDate <= DateTime.Now)
                        {
                            UpdateEmployeeInfo(model);
                        }

                        // Information saved successfully
                        model.IsError = 0;
                        model.ErrMsg = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        try
                        {
                            Notification(entity, model);
                        }
                        catch (Exception) { }
                    }
                    catch (Exception ex)
                    {
                        model.IsError = 1;
                        model.ErrMsg = "failed";
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    }
                }
            }

            populateDropdown(model);

            return View(model);
        }

        private void Notification(PRM_EmpStatusChange entity, EmployeeConfirmationIncrementPromotionViewModel model)
        {
            var redirectToUrl = String.Empty;
            MyNotificationLibEnum.NotificationType enumType = MyNotificationLibEnum.NotificationType.Employee_Transfer;

            #region Notification
            try
            {
                var fromPart = new StringBuilder("From ");
                var toPart = new StringBuilder("To ");
                var specificZoneToSendNotification = new List<int>();

                var fromZone = (from c in _prmCommonservice.PRMUnit.ZoneInfoRepository.Fetch()
                                where c.Id == entity.FromZoneInfoId
                                select c).FirstOrDefault();

                var toZone = (from c in _prmCommonservice.PRMUnit.ZoneInfoRepository.Fetch()
                              where c.Id == entity.ToZoneInfoId
                              select c).FirstOrDefault();

                fromPart.Append("Zone: ").Append(fromZone.ZoneName).Append(" ");
                toPart.Append("Zone: ").Append(toZone.ZoneName).Append(" ");

                specificZoneToSendNotification.Add(Common.GetInteger(entity.ToZoneInfoId));

                if (entity.FromOrganogramLevelId != entity.ToOrganogramLevelId)
                {
                    var fromOrganogramLevel =
                    (from c in _prmCommonservice.PRMUnit.OrganogramLevelRepository.Fetch()
                     where c.Id == entity.FromOrganogramLevelId
                     select c).FirstOrDefault();

                    var toOrganogramLevel =
                    (from c in _prmCommonservice.PRMUnit.OrganogramLevelRepository.Fetch()
                     where c.Id == entity.ToOrganogramLevelId
                     select c).FirstOrDefault();

                    fromPart.Append("Organogram Level: ").Append(fromOrganogramLevel.LevelName).Append(" ");
                    toPart.Append("Organogram Level: ").Append(toOrganogramLevel.LevelName).Append(" ");
                }

                if (entity.FromDesignationId != entity.ToDesignationId)
                {
                    var fromDesignation = (from c in _prmCommonservice.PRMUnit.DesignationRepository.Fetch()
                                           where c.Id == entity.FromDesignationId
                                           select c).FirstOrDefault();

                    var toDesignation = (from c in _prmCommonservice.PRMUnit.DesignationRepository.Fetch()
                                         where c.Id == entity.ToDesignationId
                                         select c).FirstOrDefault();

                    fromPart.Append("Designation: ").Append(fromDesignation.Name).Append(" ");
                    toPart.Append("Designation: ").Append(toDesignation.Name).Append(" ");
                }

                if (entity.FromGradeId != entity.ToGradeId)
                {
                    var fromGrade = (from c in _prmCommonservice.PRMUnit.JobGradeRepository.Fetch()
                                     where c.Id == entity.FromGradeId
                                     select c).FirstOrDefault();

                    var toGrade = (from c in _prmCommonservice.PRMUnit.JobGradeRepository.Fetch()
                                   where c.Id == entity.ToGradeId
                                   select c).FirstOrDefault();

                    fromPart.Append("Grade: ").Append(fromGrade.GradeName).Append(" ");
                    toPart.Append("Grade: ").Append(toGrade.GradeName).Append(" ");
                }

                if (entity.FromStepId != entity.ToStepId)
                {
                    var fromStep = (from c in _prmCommonservice.PRMUnit.JobGradeStepRepository.Fetch()
                                    where c.Id == entity.FromStepId
                                    select c).FirstOrDefault();

                    var toStep = (from c in _prmCommonservice.PRMUnit.JobGradeStepRepository.Fetch()
                                  where c.Id == entity.ToStepId
                                  select c).FirstOrDefault();

                    fromPart.Append("Step: ").Append(fromStep.StepName).Append(" ");
                    toPart.Append("Grade: ").Append(toStep.StepName).Append(".");
                }

                toPart.Append(" Order Date: ").Append(Common.GetDate(entity.OrderDate).ToString(DateAndTime.GlobalDateFormat)).Append(".");
                toPart.Append(" Effective Date: ").Append(Common.GetDate(entity.EffectiveDate).ToString(DateAndTime.GlobalDateFormat)).Append(".");

                var notificationUtil = new SendNotificationByFlowSetup(enumType
                    , model.EmployeeId
                    , fromPart.ToString()
                    , toPart.ToString()
                    , Common.GetDate(entity.EffectiveDate)
                    , specificZoneToSendNotification
                    , redirectToUrl);
                notificationUtil.SendNotification();
            }
            catch (Exception ex)
            {
                // TODO: Need to uncomment following error 
                //model.ErrMsg = model.ErrMsg + Environment.NewLine + " " + ex.Message;
            }

            #endregion

            // Declare Notification Variables

            var modules = new List<MyNotificationLibEnum.NotificationModule>();
            modules.Add(MyNotificationLibEnum.NotificationModule.Human_Resource_Management_System);

            var toEmployees = new List<int>();

            var destinationZone = (from c in _prmCommonservice.PRMUnit.ZoneInfoRepository.Fetch()
                                   where c.Id == entity.ToZoneInfoId
                                   select c).FirstOrDefault();

            #region Notify To
            if (!String.IsNullOrEmpty(model.NotifyTo))
            {
                // Applicant info
                var applicant = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
                    .FirstOrDefault(e => e.Id == model.EmployeeId);
                var applicantInfo = applicant.FullName + ", " + (applicant.PRM_Designation.Name) + ", " +
                                    applicant.EmpID;

                // Notify to employees
                var notifyTo = model.NotifyTo.Split(',');
                foreach (var empId in notifyTo)
                {
                    if (_prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
                        .Any(e => e.EmpID == empId))
                    {
                        toEmployees.Add(_prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
                            .FirstOrDefault(e => e.EmpID == empId).Id);
                    }
                }

                var generalPurposeNotification =
                    new SendGeneralPurposeNotification(
                        modules,
                        applicantInfo + " has transfered to " + destinationZone.ZoneName + ".",
                        redirectToUrl,
                        toEmployees,
                        MyAppSession.EmpId,
                        enumType
                    );
                generalPurposeNotification.SendNotification();
            }

            #endregion

            #region Self Notification
            toEmployees.Clear();
            toEmployees.Add(model.EmployeeId);

            var notificationForApplicant = new SendGeneralPurposeNotification(
                modules
                , "You have been transfered to " + destinationZone.ZoneName + "."
                , String.Empty
                , toEmployees
                , MyAppSession.EmpId
                , enumType);
            notificationForApplicant.SendNotification();


            #endregion
        }

        private void ApplyBusinessRules(EmployeeConfirmationIncrementPromotionViewModel model)
        {
            model.IsEffective = model.EffectiveDate <= DateTime.Now ? true : false;

            var empSalary = _empService.PRMUnit.EmpSalaryRepository.Get(t => t.EmployeeId == model.EmployeeId).FirstOrDefault();
            model.FromIsConsolidated = empSalary.isConsolidated;

            var ToEmploymentType = _empService.PRMUnit.EmploymentTypeRepository.GetByID(model.ToEmploymentTypeId);

            if (ToEmploymentType.Name == PRMEnum.EmploymentType.Contractual.ToString())
            {
                model.ToIsConsolidated = true;
            }
            else
            {
                model.ToIsConsolidated = false;
            }
        }

        public ActionResult Edit(int id)
        {
            var entity = _Service.PRMUnit.EmpStatusChangeRepository.GetByID(id);
            var model = entity.ToModel();
            model.Type = PRMEnum.EmployeeStatusChange.Transfer.ToString();

            model.FromEmploymentType = _Service.PRMUnit.EmploymentTypeRepository.GetByID(entity.FromEmploymentTypeId).Name;
            model.ToEmploymentType = _Service.PRMUnit.EmploymentTypeRepository.GetByID(entity.FromEmploymentTypeId).Name;
            model.FromDesignation = _Service.PRMUnit.DesignationRepository.GetByID(entity.FromDesignationId).Name;
            model.ToDesignation = _Service.PRMUnit.DesignationRepository.GetByID(entity.FromDesignationId).Name;
            model.FromSalaryScale = _Service.PRMUnit.SalaryScaleRepository.GetByID(entity.FromSalaryScaleId).SalaryScaleName;
            model.FromGrade = _Service.PRMUnit.JobGradeRepository.GetByID(entity.FromGradeId).GradeName;
            model.FromStep = _Service.PRMUnit.JobGradeStepRepository.GetByID(entity.FromStepId).StepName;
            model.FromRegionId = entity.FromRegionId;
            model.ToRegionId = entity.ToRegionId;

            //model.FromOrganogramLevelName = _Service.PRMUnit.OrganogramLevelRepository.GetByID(prm_EmpStatusChange.FromOrganogramLevelId).LevelName;

            model.FromOrganogramLevelName = entity.PRM_OrganogramLevel != null ? entity.PRM_OrganogramLevel.LevelName : String.Empty;
            model.ToOrganogramLevelName = entity.PRM_OrganogramLevel1 != null ? entity.PRM_OrganogramLevel1.LevelName : String.Empty;

            DownloadDoc(model);
            populateDropdown(model);

            #region To GradeStep
            model.StepList = Common.PopulateStepList(_empService.PRMUnit.JobGradeStepRepository.GetAll().OrderBy(o => o.StepName).ToList());

            #endregion
            setEmployeeInfo(model, "E");

            model.IsAddAttachment = true;
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Attachment")] EmployeeConfirmationIncrementPromotionViewModel model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                model.IsError = 1;
                model.IsEffective = model.EffectiveDate <= DateTime.Now ? true : false;

                // Set preious attachment if exist
                var obj = _Service.PRMUnit.EmpStatusChangeRepository.GetByID(model.Id);
                model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                //              

                var entity = model.ToEntity();
                entity.EUser = User.Identity.Name;
                entity.EDate = Common.CurrentDateTime;

                if (model.IsAddAttachment)
                {
                    HttpFileCollectionBase files = Request.Files;
                    entity = ToAttachFile(entity, files);
                }
                else
                {
                    entity.Attachment = null;
                    entity.FileName = null;
                    entity.IsAddAttachment = false;
                }

                //     model.ErrMsg = _Service.GetBusinessLogicValidation(entity).FirstOrDefault();
                model.ErrMsg = _Service.GetBusinessLogicValidation(entity);
                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    try
                    {
                        //undo the employment and salary info
                        var result = UndoEmployeeInfoUpdate(entity.ToModel());

                        if (result == true)
                        {
                            //obj.PRM_EmpStatusChangeDetail = _Service.PRMUnit.EmpStatusChangeRepository.GetByID(obj.Id).PRM_EmpStatusChangeDetail;
                            List<PRM_EmpStatusChangeDetail> lst = UpdateEmpStatusChangeDetails(entity);
                            if (lst == null)
                            {
                                throw new Exception();
                            }

                            ArrayList arraylst = new ArrayList();
                            foreach (var item in lst)
                            {
                                arraylst.Add(item);
                            }

                            Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                            NavigationList.Add(typeof(PRM_EmpStatusChangeDetail), arraylst);

                            _Service.PRMUnit.EmpStatusChangeRepository.Update(entity, NavigationList);
                            _Service.PRMUnit.EmpStatusChangeRepository.SaveChanges();

                            if (model.EffectiveDate <= DateTime.Now)
                            {
                                UpdateEmployeeInfo(model);
                            }

                            model.IsError = 0;
                            model.ErrMsg = "success";
                            model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                            //return RedirectToAction("Index");
                        }
                        else
                        {
                            model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                        }
                    }
                    catch (Exception ex)
                    {
                        //if (ex.InnerException != null && ex.InnerException is SqlException)
                        //{
                        //    SqlException sqlException = ex.InnerException as SqlException;
                        //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);

                        //}
                        //else
                        //{
                        //    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        //}
                        model.IsError = 1;
                        model.ErrMsg = "failed";
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    }
                }
            }

            populateDropdown(model);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                var obj = _Service.PRMUnit.EmpStatusChangeRepository.GetByID(id);

                if (UndoEmployeeInfoUpdate(obj.ToModel()))
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_EmpStatusChangeDetail) };
                    _Service.PRMUnit.EmpStatusChangeRepository.Delete(id, allTypes);
                    _Service.PRMUnit.EmpStatusChangeRepository.SaveChanges();

                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
                else
                {
                    result = false;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        #endregion

        public ActionResult EmployeeSearch(string Type)
        {
            var model = new EmployeeSearchViewModel();
            model.EmployeeStatus = 0;

            return View("EmployeeSearch", model);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(EmployeeConfirmationIncrementPromotionViewModel model)
        {
            string errorList = string.Empty;
            errorList = GetValidationChecking(model);
            if (string.IsNullOrEmpty(errorList))
            {
                var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

                // calculate basic salary
                decimal lastBasic = 0;

                var basicObj = obj.PRM_EmpSalary.PRM_EmpSalaryDetail.Where(q => q.PRM_SalaryHead.IsBasicHead == true).FirstOrDefault();
                if (basicObj != null)
                {
                    lastBasic = basicObj.AmountType == "Percent" ? (obj.PRM_EmpSalary.GrossSalary * (basicObj.Amount / 100)) : basicObj.Amount;
                }

                return Json(new
                {
                    EmpId = obj.EmpID,
                    EmployeeName = obj.FullName,
                    JoiningDate = obj.DateofJoining.ToString(DateAndTime.GlobalDateFormat),
                    FromDisciplineId = obj.DisciplineId,
                    FromDivisionId = obj.DivisionId,
                    FromDivisionName = obj.PRM_Division == null ? String.Empty : obj.PRM_Division.Name,
                    FromJobLocationId = obj.JobLocationId,
                    FromJobLocationName = obj.PRM_JobLocation == null ? String.Empty : obj.PRM_JobLocation.Name,
                    FromEmploymentProcessId = obj.EmploymentProcessId,
                    FromEmploymentProcessName = obj.PRM_EmploymentProcess == null ? String.Empty : obj.PRM_EmploymentProcess.Name,
                    FromDesignation = obj.PRM_Designation == null ? String.Empty : obj.PRM_Designation.Name,
                    FromDesignationId = obj.DesignationId,
                    FromEmploymentTypeId = obj.EmploymentTypeId,
                    FromEmploymentType = obj.PRM_EmploymentType == null ? String.Empty : obj.PRM_EmploymentType.Name,

                    FromGradeId = obj.PRM_EmpSalary.GradeId,
                    FromGrade = obj.PRM_EmpSalary.PRM_JobGrade.GradeName,

                    // 2016-05-09
                    FromSalaryScaleId = obj.PRM_EmpSalary.SalaryScaleId,
                    FromSalaryScale = obj.PRM_EmpSalary.PRM_SalaryScale == null ? String.Empty : obj.PRM_EmpSalary.PRM_SalaryScale.SalaryScaleName,
                    FromOrganogramLevelId = obj.PRM_OrganogramLevel == null ? 0 : obj.PRM_OrganogramLevel.Id,
                    FromOrganogramLevelName = obj.PRM_OrganogramLevel == null ? String.Empty : obj.PRM_OrganogramLevel.LevelName,

                    FromStepId = obj.PRM_EmpSalary.StepId,
                    FromStep = obj.PRM_EmpSalary.PRM_GradeStep.StepName,
                    FromBasicSalary = lastBasic,
                    FromGrossSalary = obj.PRM_EmpSalary.GrossSalary,
                    InitialBasic = basicObj.Amount,
                    YearlyIncrement = obj.PRM_EmpSalary.PRM_JobGrade.YearlyIncrement == null ? 0 : obj.PRM_EmpSalary.PRM_JobGrade.YearlyIncrement,
                    DateofConfirmation = obj.DateofConfirmation == null ? String.Empty : Convert.ToDateTime(obj.DateofConfirmation).ToString("yyyy-MM-dd"),
                    DesignationList = _empService.PRMUnit.DesignationRepository.Get(p => p.GradeId == obj.PRM_EmpSalary.GradeId).Select(x => new { Id = x.Id, Name = x.Name }),
                    EmploymentTypeList = _empService.PRMUnit.EmploymentTypeRepository.GetAll().Select(x => new { Id = x.Id, Name = x.Name })
                });
            }
            else
            {
                return Json(new
                {
                    Result = errorList
                });
            }
        }

        #region Private Methods

        private PRM_EmpStatusChange ToAttachFile(PRM_EmpStatusChange prm_EmpStatusChange, HttpFileCollectionBase files)
        {
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
                    prm_EmpStatusChange.FileName = name;
                    prm_EmpStatusChange.Attachment = fileData;
                }
            }

            return prm_EmpStatusChange;
        }

        private void populateDropdown(EmployeeConfirmationIncrementPromotionViewModel model)
        {
            #region To Grade
            model.ToZoneInfoList = Common.PopulateDdlZoneList(_Service.PRMUnit.ZoneInfoRepository.GetAll().ToList());
            model.GradeList = Common.PopulateCurrentJobGradeDDL(_empService);
            model.ToRegionList = Common.PopulateDllList(_Service.PRMUnit.RegionRepository.GetAll().OrderBy(x => x.SortOrder).ToList());
            #endregion

        }

        private void setEmployeeInfo(EmployeeConfirmationIncrementPromotionViewModel model, string mode)
        {
            if (model.EmployeeId != 0)
            {
                var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                // calculate basic salary
                decimal lastBasic = 0;
                var basicObj = obj.PRM_EmpSalary.PRM_EmpSalaryDetail.Where(q => q.PRM_SalaryHead.IsBasicHead == true).FirstOrDefault();
                if (basicObj != null)
                {
                    lastBasic = basicObj.AmountType == "Percent" ? (obj.PRM_EmpSalary.GrossSalary * (basicObj.Amount / 100)) : basicObj.Amount;
                }

                model.EmpId = obj.EmpID;
                if (mode == "I")
                {
                    if (obj.PRM_EmpSalary != null)
                    {
                        //Need to examine
                        model.FromSalaryScale = obj.PRM_EmpSalary.PRM_SalaryScale.SalaryScaleName;
                        model.FromGrade = obj.PRM_EmpSalary.PRM_JobGrade.GradeName;
                        model.FromStep = obj.PRM_EmpSalary.PRM_GradeStep.StepName;
                        model.FromBasicSalary = lastBasic;
                        model.FromGrossSalary = obj.PRM_EmpSalary.GrossSalary;
                    }
                }

                model.FromDesignation = obj.PRM_Designation.Name;
                model.FromEmploymentType = obj.PRM_EmploymentType.Name;
                model.FromDivisionName = obj.PRM_Division == null ? string.Empty : obj.PRM_Division.Name;
                model.EmployeeName = obj.FullName;
                // model.FromJobLocationName = obj.PRM_JobLocation.Name;
                var fromRegion = _Service.PRMUnit.RegionRepository.GetByID(model.FromRegionId);

                model.FromRegionName = fromRegion == null ? String.Empty : fromRegion.Name;

                model.JoiningDate = obj.DateofJoining;
                model.InitialBasic = basicObj.Amount;
                model.DateofConfirmation = obj.DateofConfirmation.ToString();

                model.ToSalaryScale = _Service.PRMUnit.SalaryScaleRepository.GetByID(model.ToSalaryScaleId).SalaryScaleName;
                model.ToGrade = _Service.PRMUnit.JobGradeRepository.GetByID(model.ToGradeId).GradeName;
            }
        }

        private List<PRM_EmpStatusChangeDetail> UpdateEmpStatusChangeDetails(PRM_EmpStatusChange entity)
        {
            List<PRM_EmpStatusChangeDetail> lst = new List<PRM_EmpStatusChangeDetail>();
            var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);

            //var PFEmployerHeadId = (from p in _Service.PRMUnit.SalaryHeadRepository.GetAll() select p.IsPFCompanyContributionHead).FirstOrDefault();
            //var PFBothHeadId = (from p in _Service.PRMUnit.SalaryHeadRepository.GetAll() select p.IsPFBothHead).FirstOrDefault();


            var PFEmployerHeadId = _Service.PRMUnit
                    .SalaryHeadRepository.GetAll()
                    .FirstOrDefault(s => s.IsPFCompanyContributionHead == true).Id;

            var PFBothHeadId = _Service.PRMUnit
                    .SalaryHeadRepository.GetAll()
                    .FirstOrDefault(s => s.IsPFCompanyContributionHead == true).Id;

            try
            {
                var objlist = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository.Get(q => q.PRM_SalaryStructure.GradeId == entity.FromGradeId
                && q.PRM_SalaryStructure.StepId == entity.FromStepId && (q.PRM_SalaryHead.IsGrossPayHead == true || q.PRM_SalaryHead.Id == PFEmployerHeadId || q.PRM_SalaryHead.Id == PFBothHeadId)).ToList();

                foreach (var salaryHead in objlist)
                {
                    int HeadId = salaryHead.HeadId;
                    string BasedON = salaryHead.PRM_SalaryStructure.PRM_JobGrade.IsConsolidated == true ? "Gross" : "Basic";

                    var item = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository.Get(q => q.PRM_SalaryStructure.GradeId == entity.ToGradeId
                    && q.PRM_SalaryStructure.StepId == entity.ToStepId && q.HeadId == HeadId).FirstOrDefault();

                    PRM_EmpStatusChangeDetail empChangeDetail = _Service.PRMUnit.EmpStatusChangeRepository.GetByID(entity.Id).PRM_EmpStatusChangeDetail.Where(q => q.HeadId == salaryHead.HeadId).FirstOrDefault();

                    if (item != null && empChangeDetail != null)
                    {
                        empChangeDetail.AmountTypeFrom = salaryHead.AmountType;
                        empChangeDetail.AmountFrom = salaryHead.Amount;
                        empChangeDetail.IsTaxableFrom = salaryHead.IsTaxable;

                        if (salaryHead.AmountType == "Percent") empChangeDetail.BasedOnFrom = BasedON;
                        empChangeDetail.AmountTypeTo = item.AmountType;
                        empChangeDetail.AmountTo = item.Amount;
                        empChangeDetail.IsTaxableTo = item.IsTaxable;
                        if (item.AmountType == "Percent") empChangeDetail.BasedOnTo = item.PRM_SalaryStructure.PRM_JobGrade.IsConsolidated == true ? "Gross" : "Basic";
                    }
                    //entity.PRM_EmpStatusChangeDetail.Add(empChangeDetail);
                    lst.Add(empChangeDetail);
                }
            }
            catch { return null; }
            return lst;
        }

        private bool UpdateEmployeeInfo(EmployeeConfirmationIncrementPromotionViewModel model)
        {
            //var PFEmployerHeadId = (from p in _pgmCommonservice.PGMUnit.SpecialSalaryHead.GetAll() select p.PFEmployerHeadId).FirstOrDefault();
            //var PFBothHeadId = (from p in _pgmCommonservice.PGMUnit.SpecialSalaryHead.GetAll() select p.PFBothHeadId).FirstOrDefault();

            var PFEmployerHeadId = _Service.PRMUnit
                    .SalaryHeadRepository.GetAll()
                    .FirstOrDefault(s => s.IsPFCompanyContributionHead == true).Id;

            var PFBothHeadId = _Service.PRMUnit
                    .SalaryHeadRepository.GetAll()
                    .FirstOrDefault(s => s.IsPFCompanyContributionHead == true).Id;


            var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

            /* RH#01 */
            var ToEmploymentType = _Service.PRMUnit.EmploymentTypeRepository.GetByID(model.ToEmploymentTypeId);
            /* RH#01 End */

            if (model.IsOutofOfficeOrDeputaion)
            {
                obj.DateofInactive = model.EffectiveDate;
                obj.EmploymentStatusId = _Service.PRMUnit.EmploymentStatusRepository.GetAll().Where(q => q.Name == "Inactive").FirstOrDefault().Id;
            }
            else
            {
                obj.ZoneInfoId = Convert.ToInt32(model.ToZoneInfoId);
                obj.OrganogramLevelId = Convert.ToInt32(model.ToOrganogramLevelId);
                obj.DesignationId = model.ToDesignationId;
                obj.JobGradeId = model.ToGradeId;
            }

            obj.PRM_EmpSalary.SalaryScaleId = model.ToSalaryScaleId;
            obj.PRM_EmpSalary.GradeId = model.ToGradeId;
            obj.PRM_EmpSalary.StepId = model.ToStepId;
            obj.PRM_EmpSalary.GrossSalary = Convert.ToDecimal(model.ToGrossSalary);
            obj.PRM_EmpSalary.isConsolidated = model.ToIsConsolidated;


            try
            {
                if (!ToEmploymentType.Name.Equals(PRMEnum.EmploymentType.Contractual.ToString())) /* RH#01; Add this if condition only*/
                {
                    foreach (var salaryHead in _pgmCommonService.PGMUnit.SalaryStructureDetailRepository.Get(q => q.PRM_SalaryStructure.GradeId == model.ToGradeId
                        && q.PRM_SalaryStructure.StepId == model.ToStepId
                        && (q.PRM_SalaryHead.IsGrossPayHead == true || q.PRM_SalaryHead.Id == PFEmployerHeadId || q.PRM_SalaryHead.Id == PFBothHeadId)).ToList())
                    {
                        var item = obj.PRM_EmpSalary.PRM_EmpSalaryDetail.Where(q => q.HeadId == salaryHead.HeadId).FirstOrDefault();
                        if (item != null)
                        {
                            item.AmountType = salaryHead.AmountType;
                            item.Amount = salaryHead.Amount;
                            item.HeadType = salaryHead.HeadType;
                            item.IsTaxable = salaryHead.IsTaxable;
                        }
                    }
                } /* RH#01 End; Add this if condition only*/

                //obj.PRM_EmpSalary.PRM_EmpSalaryDetail.Where(q => q.HeadType == "Basic").FirstOrDefault().Amount = model.ToBasicSalary;
            }
            catch { }
            try
            {
                Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                var lstChild = new ArrayList();
                foreach (var item in obj.PRM_EmpSalary.PRM_EmpSalaryDetail)
                {
                    item.EUser = User.Identity.Name;
                    item.EDate = Common.CurrentDateTime;
                    lstChild.Add(item);
                }
                NavigationList.Add(typeof(PRM_EmpSalaryDetail), lstChild);

                _Service.PRMUnit.EmploymentInfoRepository.Update(obj);
                _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();

                _executeFunction.UpdateUserInfoForTransfer(obj.EmpID, obj.ZoneInfoId);

                _Service.PRMUnit.EmpSalaryRepository.Update(obj.PRM_EmpSalary, "EmployeeId", NavigationList);
                _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool UndoEmployeeInfoUpdate(EmployeeConfirmationIncrementPromotionViewModel model)
        {
            var obj = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            obj.ZoneInfoId = Convert.ToInt32(model.FromZoneInfoId);
            obj.OrganogramLevelId = Convert.ToInt32(model.FromOrganogramLevelId);
            obj.DesignationId = model.FromDesignationId;
            obj.JobGradeId = model.FromGradeId;
            obj.EmploymentTypeId = model.FromEmploymentTypeId;

            //2016-05-09
            obj.PRM_EmpSalary.SalaryScaleId = model.FromSalaryScaleId;
            //

            obj.PRM_EmpSalary.GradeId = model.FromGradeId;
            obj.PRM_EmpSalary.StepId = model.FromStepId;
            obj.PRM_EmpSalary.GrossSalary = model.FromGrossSalary;

            if (model.Type.Equals(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Confirmation.ToString())) /*RH#02*/
            {
                obj.IsContractual = true;
                obj.DateofConfirmation = null;
                obj.PRM_EmpSalary.isConsolidated = true;
            }

            try
            {
                foreach (var item in obj.PRM_EmpSalary.PRM_EmpSalaryDetail)
                {
                    var salaryHead = _Service.PRMUnit.EmpStatusChangeRepository.GetByID(model.Id).PRM_EmpStatusChangeDetail.Where(q => q.HeadId == item.HeadId).FirstOrDefault();
                    if (salaryHead != null)
                    {
                        item.AmountType = salaryHead.AmountTypeFrom;
                        item.Amount = Convert.ToDecimal(salaryHead.AmountFrom);
                        item.HeadType = salaryHead.PRM_SalaryHead.HeadType;
                        item.IsTaxable = salaryHead.IsTaxableFrom == true ? true : false;
                    }
                }
            }
            catch { }

            try
            {
                Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                var lstChild = new ArrayList();
                foreach (var item in obj.PRM_EmpSalary.PRM_EmpSalaryDetail)
                {
                    item.EUser = User.Identity.Name;
                    item.EDate = Common.CurrentDateTime;
                    lstChild.Add(item);
                }
                NavigationList.Add(typeof(PRM_EmpSalaryDetail), lstChild);

                _Service.PRMUnit.EmploymentInfoRepository.Update(obj);
                _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();

                _executeFunction.UpdateUserInfoForTransfer(obj.EmpID, obj.ZoneInfoId);

                _Service.PRMUnit.EmpSalaryRepository.Update(obj.PRM_EmpSalary, "EmployeeId", NavigationList);
                _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool CreateEmpStatusChangeDetails(PRM_EmpStatusChange entity, EmployeeConfirmationIncrementPromotionViewModel model)
        {
            try
            {
                var list = CreateEmpStatusChangeDetailsHelper(entity, model);

                foreach (var item in list)
                {
                    entity.PRM_EmpStatusChangeDetail.Add(item);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private List<PRM_EmpStatusChangeDetail> CreateEmpStatusChangeDetailsHelper(PRM_EmpStatusChange entity, EmployeeConfirmationIncrementPromotionViewModel model)
        {
            List<PRM_EmpStatusChangeDetail> lstEmpStatusChangeDetail = new List<PRM_EmpStatusChangeDetail>();

            var employmentInfo = _Service.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);

            try
            {
                foreach (var empSalaryDetail in employmentInfo.PRM_EmpSalary.PRM_EmpSalaryDetail.ToList())
                {
                    int HeadId = empSalaryDetail.HeadId;
                    string BasedON = empSalaryDetail.PRM_EmpSalary.isConsolidated == true ? "Gross" : "Basic";

                    var empChangeDetail = new PRM_EmpStatusChangeDetail();
                    empChangeDetail.HeadId = empSalaryDetail.HeadId;

                    empChangeDetail.AmountTypeFrom = empSalaryDetail.AmountType;
                    empChangeDetail.AmountFrom = empSalaryDetail.Amount;
                    empChangeDetail.IsTaxableFrom = empSalaryDetail.IsTaxable;
                    if (empSalaryDetail.AmountType == PRMEnum.FixedPercent.Percent.ToString()) empChangeDetail.BasedOnFrom = BasedON;


                    var salaryStructureDetail = _pgmCommonService.PGMUnit
                        .SalaryStructureDetailRepository
                        .Get(q => q.PRM_SalaryStructure.GradeId == entity.ToGradeId
                                && q.PRM_SalaryStructure.StepId == entity.ToStepId
                                && q.HeadId == HeadId)
                    .FirstOrDefault();
                    if (salaryStructureDetail != null)
                    {
                        //Contructual
                        if (employmentInfo.IsContractual)
                        {
                            empChangeDetail.AmountTypeTo = empSalaryDetail.AmountType;
                            empChangeDetail.AmountTo = empSalaryDetail.Amount;
                            empChangeDetail.IsTaxableTo = empSalaryDetail.IsTaxable;
                            if (empSalaryDetail.AmountType == PRMEnum.FixedPercent.Percent.ToString()) empChangeDetail.BasedOnTo = BasedON;

                            if (model.Type.Equals(PRMEnum.EmployeeStatusChange.Confirmation.ToString()))
                            {
                                empChangeDetail.AmountTypeTo = salaryStructureDetail.AmountType;
                                empChangeDetail.AmountTo = salaryStructureDetail.Amount;
                                empChangeDetail.IsTaxableTo = salaryStructureDetail.IsTaxable;
                                if (salaryStructureDetail.AmountType == PRMEnum.FixedPercent.Percent.ToString()) empChangeDetail.BasedOnTo = salaryStructureDetail.PRM_SalaryStructure.PRM_JobGrade.IsConsolidated == true ? "Gross" : "Basic";
                            }
                        }
                        else
                        {
                            empChangeDetail.AmountTypeTo = salaryStructureDetail.AmountType;
                            empChangeDetail.AmountTo = salaryStructureDetail.Amount;
                            empChangeDetail.IsTaxableTo = salaryStructureDetail.IsTaxable;
                            if (salaryStructureDetail.AmountType == PRMEnum.FixedPercent.Percent.ToString()) empChangeDetail.BasedOnTo = salaryStructureDetail.PRM_SalaryStructure.PRM_JobGrade.IsConsolidated == true ? "Gross" : "Basic";
                        }

                    }

                    lstEmpStatusChangeDetail.Add(empChangeDetail);
                }
            }
            catch
            {
                //return false; 
            }

            return lstEmpStatusChangeDetail;
        }

        #endregion

        #region Public Methods

        // Get Gross salay and basic salary
        public JsonResult GetBasicGrossByStep(int gradeID, int stepID)
        {
            decimal basicSalary = 0;
            decimal grossSalary = 0;
            var salStructure = _pgmCommonService.PGMUnit.SalaryStructureRepository.Get(q => q.GradeId == gradeID && q.StepId == stepID).FirstOrDefault();
            if (salStructure != null)
            {
                var salaryHeads = _empService.PRMUnit.SalaryHeadRepository.Fetch().ToList();

                try
                {
                    basicSalary = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository.Get(q => q.SalaryStructureId == salStructure.Id && q.PRM_SalaryHead.IsGrossPayHead == true && q.PRM_SalaryHead.IsBasicHead == true).FirstOrDefault().Amount;
                    var salaryStructureDetails = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository.Get(q => q.SalaryStructureId == salStructure.Id && q.PRM_SalaryHead.IsGrossPayHead == true);
                    foreach (var item in salaryStructureDetails)
                    {
                        if (item.AmountType == "Percent")
                            grossSalary += basicSalary * (item.Amount / 100);
                        else
                            grossSalary += item.Amount;
                    }
                }
                catch { }
            }

            return Json(new
            {
                Basic = Math.Round(basicSalary, 2),
                Gross = Math.Round(grossSalary, 2)
            }, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public string GetValidationChecking(EmployeeConfirmationIncrementPromotionViewModel entity)
        {
            string errorMessage = string.Empty;

            var employeeSalary = (from s in _Service.PRMUnit.EmpSalaryRepository.GetAll().Where(x => x.EmployeeId == entity.EmployeeId) select s).ToList();

            if (employeeSalary.Count == 0)
            {
                //Do not change this string NoSalaryStructure
                errorMessage = "NoSalaryStructure";
            }

            return errorMessage;
        }

        #endregion

        #region Attachment

        private int Upload(EmployeeConfirmationIncrementPromotionViewModel model)
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
                model.ErrMsg = "Upload File Error!";
            }

            return model.IsError;
        }

        public void DownloadDoc(EmployeeConfirmationIncrementPromotionViewModel model)
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


        [NoCache]
        public JsonResult GetStep(int toGradeId)
        {
            var stepList = _Service.PRMUnit.JobGradeStepRepository.Get(q => q.JobGradeId == toGradeId).ToList();
            return Json(
               new
               {
                   steps = stepList.Select(x => new { Id = x.Id, StepName = x.StepName })
               },
               JsonRequestBehavior.AllowGet
           );
        }

        //[NoCache]
        //public JsonResult GetZoneIdByOrgId(int organogramId)
        //{
        //    var zoneId = _empService.GetZoneIdByOrganogramId(organogramId);
        //    return Json(new
        //    {
        //        ZoneId = zoneId
        //    }, JsonRequestBehavior.AllowGet);
        //}

        #region Grid Dropdown list
        [NoCache]
        public ActionResult ZoneInfoView()
        {
            var itemList = Common.PopulateDdlZoneList(_Service.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x => x.SortOrder).ToList());
            return PartialView("Select", itemList);
        }

        [NoCache]
        public ActionResult DesignationforView()
        {
            var itemList = Common.PopulateDllList(_Service.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }
        [NoCache]
        public ActionResult GradeforView()
        {
            var itemlist = Common.PopulateJobGradeDDL(_JobGradeService.GetLatestJobGrade());
            return PartialView("Select", itemlist);
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
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }
        #endregion

        #region Oraganogram Level Tree

        public ActionResult OrganogramLevelTreeSearchList(int zoneId)
        {
            EmployeeTransferInfoViewModel model = new EmployeeTransferInfoViewModel();
            if (zoneId < 0)
            {
                return View("Create", model);
            }
            ViewBag.ZoneInfoId = zoneId;
            return PartialView("_ZoneWiseOrganogramLevelTree");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetZoneWiseOrganogramLevelTreeData(int zoneId)
        {
            var nodes = _empService.PRMUnit.OrganogramLevelRepository.GetAll().Where(q => q.ZoneInfoId == null || q.ZoneInfoId == zoneId).ToList();
            var parentNode = nodes.Where(x => x.ParentId == 0).FirstOrDefault();

            JsTreeNode rootNode = new JsTreeNode();

            if (parentNode != null)
            {
                rootNode.attr = new Attributes();
                rootNode.attr.id = Convert.ToString(parentNode.Id);
                rootNode.attr.rel = "root" + Convert.ToString(parentNode.Id);
                rootNode.data = new Data();

                StringBuilder lvlName = GenerateNodeText(parentNode);
                rootNode.data.title = Convert.ToString(lvlName);
                rootNode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";
                PopulateTree(parentNode, rootNode, nodes);
            }

            return new JsonResult()
            {
                Data = rootNode,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        private static StringBuilder GenerateNodeText(PRM_OrganogramLevel parentNode)
        {
            StringBuilder lvlName = new StringBuilder();
            lvlName.Append(parentNode.LevelName);

            if (parentNode.PRM_OrganogramType != null)
            {
                lvlName.Append(" [");
                lvlName.Append(parentNode.PRM_OrganogramType.Name);
                lvlName.Append("]");
            }
            return lvlName;
        }
        public void PopulateTree(PRM_OrganogramLevel parentNode, JsTreeNode jsTNode, List<PRM_OrganogramLevel> nodes)
        {
            StringBuilder nodeText = new StringBuilder();
            jsTNode.children = new List<JsTreeNode>();
            foreach (var dr in nodes)
            {
                if (dr != null)
                {
                    if (dr.ParentId == parentNode.Id)
                    {
                        JsTreeNode cnode = new JsTreeNode();
                        cnode.attr = new Attributes();
                        cnode.attr.id = Convert.ToString(dr.Id);
                        cnode.attr.rel = "folder" + dr.Id;
                        cnode.data = new Data();
                        nodeText = GenerateNodeText(dr);
                        cnode.data.title = Convert.ToString(nodeText);

                        cnode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";

                        jsTNode.children.Add(cnode);
                        PopulateTree(dr, cnode, nodes);
                    }
                }
            }
        }


        #endregion


        public JsonResult GetBasicGrossByStep(int scaleId, int gradeID, int stepID)
        {
            decimal newBasicSalary = 0;
            decimal grossSalary = 0;
            var salStructure = _pgmCommonService.PGMUnit.SalaryStructureRepository
                .Get(q => q.SalaryScaleId == scaleId && q.GradeId == gradeID && q.StepId == stepID)
                .FirstOrDefault();

            if (salStructure != null)
            {
                try
                {
                    var basicHead = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository
                        .Get(q => q.SalaryStructureId == salStructure.Id
                            && q.PRM_SalaryHead.IsBasicHead)
                        .FirstOrDefault();

                    if (basicHead != null)
                        newBasicSalary = basicHead.Amount;

                    var salaryStructureDetails = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository
                        .Get(q => q.SalaryStructureId == salStructure.Id
                            && q.PRM_SalaryHead.IsGrossPayHead);

                    foreach (var item in salaryStructureDetails)
                    {
                        if (item.AmountType == "Percent")
                            grossSalary += newBasicSalary * (item.Amount / 100);
                        else
                            grossSalary += item.Amount;
                    }
                }
                catch { }
            }

            return Json(new
            {
                Basic = Math.Round(newBasicSalary, 2),
                Gross = Math.Round(grossSalary, 2)
            }, JsonRequestBehavior.AllowGet);
        }
    }
}