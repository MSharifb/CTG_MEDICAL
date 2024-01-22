
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PGM;
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
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.PGM;
using PRM_EmpSalaryDetail = BEPZA_MEDICAL.DAL.PRM.PRM_EmpSalaryDetail;

/*
 Revision History (RH):
		SL		: 01
		Author	: AMN
		Date	: 2015-Mar-25
		Change	: Annual increment did not effect in salary structure for contractual employee (ERP_BEPZA_PRM_SCR.doc#156)

 		SL		: 02
		Author	: AMN
		Date	: 2015-Mar-29
		Change	: Add enum BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange instead of comparing string
 */

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class EmployeeConfirmationIncrementPromotionController : BaseController
    {
        #region Fields

        private readonly EmployeeConfirmIncrementPromotionService _Service;
        private readonly EmployeeService _empService;
        private readonly JobGradeService _JobGradeService;
        private readonly PGMArrearAdjustmentService _PGMArrearAdjustmentService;
        private readonly PGMCommonService _pgmCommonService;

        #endregion

        #region Constructor

        public EmployeeConfirmationIncrementPromotionController(
                EmployeeConfirmIncrementPromotionService service
                , EmployeeService empService
                , JobGradeService jobGradeService
                , PGMArrearAdjustmentService pgmArrearAdjustmentService
                , PGMCommonService pgmCommonService
            )
        {
            this._Service = service;
            this._empService = empService;
            this._JobGradeService = jobGradeService;
            this._PGMArrearAdjustmentService = pgmArrearAdjustmentService;
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

            //totalRecords = _Service.PRMUnit.EmpStatusChangeRepository.GetCount(filterExpression);


            var list = _Service.PRMUnit.EmpStatusChangeRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1).ToList();
            list = list.Where(q => q.PRM_EmploymentInfo.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            list = list.Where(q => q.Type != PRMEnum.EmployeeStatusChange.SelectionGrade.ToString()).ToList();

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
            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();


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

            if (request.SortingName == "DivisionId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_EmploymentInfo.PRM_Division.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_EmploymentInfo.PRM_Division.Name).ToList();
                }
            }

            if (request.SortingName == "FromSalaryScaleId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_SalaryScale.SalaryScaleName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_SalaryScale.SalaryScaleName).ToList();
                }
            }

            if (request.SortingName == "ToSalaryScaleId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_SalaryScale.SalaryScaleName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_SalaryScale.SalaryScaleName).ToList();
                }
            }

            if (request.SortingName == "FromGradeId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_JobGrade.GradeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_JobGrade.GradeName).ToList();
                }
            }

            if (request.SortingName == "ToGradeId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_JobGrade1.GradeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_JobGrade1.GradeName).ToList();
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

            if (request.SortingName == "Type")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Type).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Type).ToList();
                }
            }

            if (request.SortingName == "FromBasicSalary")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FromBasicSalary).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FromBasicSalary).ToList();
                }
            }

            if (request.SortingName == "ToBasicSalary")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ToBasicSalary).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ToBasicSalary).ToList();
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

            foreach (BEPZA_MEDICAL.DAL.PRM.PRM_EmpStatusChange d in list)
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
                    d.PRM_Division==null ? string.Empty:d.PRM_Division.Name,
                    d.PRM_Division1==null ? string.Empty:d.PRM_Division1.Name,
                    d.PRM_JobGrade.GradeName,
                    d.PRM_JobGrade1.GradeName,
                    d.PRM_Designation.Name,
                    d.PRM_Designation1.Name,
                    d.Type,
                    d.FromBasicSalary,
                    d.ToBasicSalary,
                    d.EffectiveDate.Date.ToString(DateAndTime.GlobalDateFormat),
                    isEditable,
                    "Delete"                                    
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, int Type, int ZoneInfoId)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var empTypeId = _empService.PRMUnit.EmploymentTypeRepository.GetAll().Where(q => q.Name == BEPZA_MEDICAL.Utility.PRMEnum.EmploymentType.Permanent.ToString()).FirstOrDefault().Id;

            if (Type == 1)
                filterExpression = "EmpTypeId !=" + empTypeId + "";
            else if (Type == 3 || Type == 4)
                filterExpression = "EmpTypeId =" + empTypeId + "";

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
                ZoneInfoId,
                out totalRecords
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
            populateDropdown(model);
            model.IsAddAttachment = true;
            model.ZoneListByUserId = LoggedUserZoneInfoId;
            return View(model);
        }
        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] EmployeeConfirmationIncrementPromotionViewModel model, FormCollection form)
        {
            ApplyBusinessRules(model);
            if (ModelState.IsValid)
            {
                if (PRMEnum.EmployeeStatusChange.Confirmation.ToString().ToLower() == "confirmation" || PRMEnum.EmployeeStatusChange.Increment.ToString().ToLower() == "increment")
                {
                    model.ToZoneInfoId = model.FromZoneInfoId;
                }
                var empStatusChange = model.ToEntity();

                empStatusChange.IUser = User.Identity.Name;
                empStatusChange.IDate = Common.CurrentDateTime;
                model.IsError = 1;

                #region Attachment

                if (model.IsAddAttachment)
                {
                    HttpFileCollectionBase files = Request.Files;
                    empStatusChange = ToAttachFile(empStatusChange, files);
                }
                else
                {
                    empStatusChange.Attachment = null;
                    empStatusChange.FileName = null;
                    empStatusChange.IsAddAttachment = false;
                }

                #endregion

                model.ErrMsg = _Service.GetBusinessLogicValidation(empStatusChange);

                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    try
                    {
                        if (CreateEmpStatusChangeDetails(empStatusChange, model) == false)
                        {
                            throw new Exception();
                        }

                        _Service.PRMUnit.EmpStatusChangeRepository.Add(empStatusChange);
                        _Service.PRMUnit.EmpStatusChangeRepository.SaveChanges();

                        if (model.EffectiveDate <= DateTime.Now)
                        {
                            UpdateEmployeeInfoAndSalary(model);
                            _PGMArrearAdjustmentService.CreateArrearAdjustment(empStatusChange.Id);
                        }

                        model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        try
                        {
                            Notification(model.EmployeeId, model.Type, model.EffectiveDate, model.NotifyTo);
                        }
                        catch (Exception) { }
                    }
                    catch (Exception ex)
                    {
                        model.IsError = 1;
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    }
                }
            }

            populateDropdown(model);

            return View(model);
        }

        private bool CreateEmpStatusChangeDetails(BEPZA_MEDICAL.DAL.PRM.PRM_EmpStatusChange entity, EmployeeConfirmationIncrementPromotionViewModel model)
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

        private List<PRM_EmpStatusChangeDetail> CreateEmpStatusChangeDetailsHelper(BEPZA_MEDICAL.DAL.PRM.PRM_EmpStatusChange entity, EmployeeConfirmationIncrementPromotionViewModel model)
        {
            List<PRM_EmpStatusChangeDetail> lstEmpStatusChangeDetail = new List<PRM_EmpStatusChangeDetail>();
            int headId = 0;
            String basedOn = String.Empty;
            PRM_EmpStatusChangeDetail empChangeDetail = null;
            PRM_SalaryStructureDetail salaryStructureDetail = null;
            var employmentInfo = _Service.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);

            try
            {
                foreach (var empSalaryDetail in employmentInfo.PRM_EmpSalary.PRM_EmpSalaryDetail.ToList())
                {
                    headId = empSalaryDetail.HeadId;
                    basedOn = empSalaryDetail.PRM_EmpSalary.isConsolidated == true ? "Gross" : "Basic";
                    empChangeDetail = new PRM_EmpStatusChangeDetail();

                    empChangeDetail.HeadId = empSalaryDetail.HeadId;

                    empChangeDetail.AmountTypeFrom = empSalaryDetail.AmountType;
                    empChangeDetail.AmountFrom = empSalaryDetail.Amount;
                    empChangeDetail.IsTaxableFrom = empSalaryDetail.IsTaxable;
                    if (empSalaryDetail.AmountType == PRMEnum.FixedPercent.Percent.ToString()) empChangeDetail.BasedOnFrom = basedOn;

                    empChangeDetail.AmountTypeTo = empSalaryDetail.AmountType;
                    empChangeDetail.AmountTo = empSalaryDetail.Amount;
                    empChangeDetail.IsTaxableTo = empSalaryDetail.IsTaxable;
                    if (empSalaryDetail.AmountType == PRMEnum.FixedPercent.Percent.ToString())
                    {
                        empChangeDetail.BasedOnTo = basedOn;
                    }

                    // ---------------
                    salaryStructureDetail = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository
                        .Get(q => q.PRM_SalaryStructure.SalaryScaleId == entity.ToSalaryScaleId
                            && q.PRM_SalaryStructure.GradeId == entity.ToGradeId
                            && q.PRM_SalaryStructure.StepId == entity.ToStepId
                            && q.HeadId == headId)
                        .FirstOrDefault();

                    if (salaryStructureDetail != null)
                    {
                        if (employmentInfo.IsContractual)
                        {
                            if (model.Type.Equals(PRMEnum.EmployeeStatusChange.Confirmation.ToString()))
                            {
                                empChangeDetail.AmountTypeTo = salaryStructureDetail.AmountType;
                                empChangeDetail.AmountTo = salaryStructureDetail.Amount;
                                empChangeDetail.IsTaxableTo = salaryStructureDetail.IsTaxable;

                                if (salaryStructureDetail.AmountType == PRMEnum.FixedPercent.Percent.ToString())
                                {
                                    empChangeDetail.BasedOnTo =
                                        salaryStructureDetail
                                            .PRM_SalaryStructure
                                            .PRM_JobGrade
                                            .IsConsolidated == true ? "Gross" : "Basic";
                                }
                            }
                        }
                    }

                    lstEmpStatusChangeDetail.Add(empChangeDetail);
                }
            }
            catch (Exception)
            {
                //return false; 
            }

            return lstEmpStatusChangeDetail;
        }


        private void Notification(int employeeId, String type, DateTime? effectiveDate, String notifyToEmps)
        {
            #region Notification

            var enumType = MyNotificationLibEnum.NotificationType.General_Purpose;
            if (type == "Confirmation")
            {
                enumType = MyNotificationLibEnum.NotificationType.Employee_Confirmation;
            }
            else if (type == "Increment")
            {
                enumType = MyNotificationLibEnum.NotificationType.Salary_Increment;
            }
            else if (type == "Promotion")
            {
                enumType = MyNotificationLibEnum.NotificationType.Employee_Promotion;
            }
            else if (type == "Demotion")
            {
                enumType = MyNotificationLibEnum.NotificationType.Employee_Demotion;
            }

            var redirectToUrl = String.Empty;

            // Declare Notification Variables
            var modules = new List<MyNotificationLibEnum.NotificationModule>();
            modules.Add(MyNotificationLibEnum.NotificationModule.Human_Resource_Management_System);

            var toEmployees = new List<int>();

            // Applicant info
            var applicant = _Service.PRMUnit.EmploymentInfoRepository.GetAll()
                .FirstOrDefault(e => e.Id == employeeId);
            var applicantInfo = applicant.FullName + ", " + (applicant.PRM_Designation.Name) + ", " +
                                applicant.EmpID;

            var customMessage = "A(n) " + type + " has issued for " + applicantInfo +
                                " on " + Common.GetDate(effectiveDate).ToString(DateAndTime.GlobalDateFormat) + ".";

            #region Self Notification
            toEmployees.Clear();
            toEmployees.Add(employeeId);

            var notificationForApplicant = new SendGeneralPurposeNotification(
                modules,
                "Your " + type + " will be effective from " + Common.GetDate(effectiveDate).ToString(DateAndTime.GlobalDateFormat),
                String.Empty,
                toEmployees,
                MyAppSession.EmpId,
                enumType
            );
            notificationForApplicant.SendNotification();
            #endregion

            #region Notify To
            if (!String.IsNullOrEmpty(notifyToEmps))
            {
                toEmployees.Clear();

                // Notify to employees
                var notifyTo = notifyToEmps.Split(',');
                foreach (String empId in notifyTo)
                {
                    if (_Service.PRMUnit.EmploymentInfoRepository.GetAll()
                        .Any(e => e.EmpID == empId.Trim()))
                    {
                        toEmployees.Add(_Service.PRMUnit.EmploymentInfoRepository.GetAll()
                            .FirstOrDefault(e => e.EmpID == empId.Trim()).Id);
                    }
                }

                var generalPurposeNotification =
                    new SendGeneralPurposeNotification(
                        modules,
                        customMessage,
                        redirectToUrl,
                        toEmployees,
                        MyAppSession.EmpId,
                        enumType
                    );
                generalPurposeNotification.SendNotification();
            }
            #endregion

            #region From notification flow
            try
            {
                var notificationUtil = new SendNotificationByFlowSetup(
                    enumType
                    , employeeId
                    , String.Empty
                    , String.Empty
                    , Common.GetDate(effectiveDate)
                    , null
                    , redirectToUrl
                    , customMessage);
                notificationUtil.SendNotification();
            }
            catch (Exception) { }
            #endregion

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

            model.FromDesignation = _Service.PRMUnit.DesignationRepository.GetByID(entity.FromDesignationId).Name;
            model.FromEmploymentType = _Service.PRMUnit.EmploymentTypeRepository.GetByID(entity.FromEmploymentTypeId).Name;
            model.FromSalaryScale = _Service.PRMUnit.SalaryScaleRepository.GetByID(entity.FromSalaryScaleId).SalaryScaleName;
            model.FromGrade = _Service.PRMUnit.JobGradeRepository.GetByID(entity.FromGradeId).GradeName;
            model.FromStep = _Service.PRMUnit.JobGradeStepRepository.GetByID(entity.FromStepId).StepName;

            //model.FromOrganogramLevelName = _Service.PRMUnit.OrganogramLevelRepository.GetByID(prm_EmpStatusChange.FromOrganogramLevelId).LevelName;

            model.FromOrganogramLevelName = entity.PRM_OrganogramLevel != null ? entity.PRM_OrganogramLevel.LevelName : String.Empty;
            model.ToOrganogramLevelName = entity.PRM_OrganogramLevel1 != null ? entity.PRM_OrganogramLevel1.LevelName : String.Empty;

            DownloadDoc(model);
            populateDropdown(model);
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

                var empStatusChange = model.ToEntity();
                empStatusChange.EUser = User.Identity.Name;
                empStatusChange.EDate = Common.CurrentDateTime;

                if (model.IsAddAttachment)
                {
                    HttpFileCollectionBase files = Request.Files;
                    empStatusChange = ToAttachFile(empStatusChange, files);
                }
                else
                {
                    empStatusChange.Attachment = null;
                    empStatusChange.FileName = null;
                    empStatusChange.IsAddAttachment = false;
                }

                model.ErrMsg = _Service.GetBusinessLogicValidation(empStatusChange);
                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    try
                    {
                        //undo the employment and salary info
                        var result = UndoEmployeeInfoUpdate(empStatusChange.ToModel());

                        if (result == true)
                        {
                            var empStatusChangeDetails = UpdateEmpStatusChangeDetails(empStatusChange);
                            if (empStatusChangeDetails == null) { throw new Exception("No status change found!"); }

                            ArrayList arraylst = new ArrayList();
                            foreach (var item in empStatusChangeDetails)
                            {
                                arraylst.Add(item);
                            }

                            Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                            NavigationList.Add(typeof(PRM_EmpStatusChangeDetail), arraylst);

                            _Service.PRMUnit.EmpStatusChangeRepository.Update(empStatusChange, NavigationList);
                            _Service.PRMUnit.EmpStatusChangeRepository.SaveChanges();

                            if (model.EffectiveDate <= DateTime.Now)
                            {
                                UpdateEmployeeInfoAndSalary(model);
                            }

                            model.IsError = 0;
                            model.ErrMsg = "success";
                            model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        }
                        else
                        {
                            model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                        }
                    }
                    catch (Exception ex)
                    {
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

        public ActionResult EmployeeSearch(string Type, int ZoneId)
        {
            var model = new EmployeeSearchViewModel();
            model.ZoneInfoId = ZoneId;
            if (Type.Equals(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Confirmation.ToString())) /*RH#02*/
            {
                model.EmployeeStatus = 1;
            }
            else if (Type.Equals(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Increment.ToString())) /*RH#02*/
            {
                model.EmployeeStatus = 2;
            }
            else if (Type.Equals(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Promotion.ToString())) /*RH#02*/
            {
                model.EmployeeStatus = 3;
            }
            else if (Type.Equals(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Demotion.ToString())) /*RH#02*/
            {
                model.EmployeeStatus = 4;
            }
            else
            {
                model.EmployeeStatus = -1;
            }

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

                    FromRegionId = obj.ZoneInfoId,
                    FromRegionName = obj.PRM_ZoneInfo.ZoneName,

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
                    EmploymentTypeList = _empService.PRMUnit.EmploymentTypeRepository.GetAll().Select(x => new { Id = x.Id, Name = x.Name }),
                    FromZoneInfoId = obj.ZoneInfoId
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

        [NoCache]
        public JsonResult GetDesignationByOrganogramLevelId(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from OrgLevel in _empService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Fetch()
                         join de in _empService.PRMUnit.DesignationRepository.Fetch() on OrgLevel.DesignationId equals de.Id
                         where OrgLevel.OrganogramLevelId == id
                         select de).OrderBy(o => o.Name).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.Name, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetOrganogramInfo(int organogramLevelId)
        {
            var obj = _empService.GetEmpDepartmentOfficeSectionSubSection(organogramLevelId);
            return Json(new
            {
                DepId = obj.DepartmentId,
                OfficeId = obj.OfficeId,
                SecId = obj.SectionId
            }, JsonRequestBehavior.AllowGet);
        }

        #region Private Methods

        private BEPZA_MEDICAL.DAL.PRM.PRM_EmpStatusChange ToAttachFile(BEPZA_MEDICAL.DAL.PRM.PRM_EmpStatusChange prm_EmpStatusChange, HttpFileCollectionBase files)
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
            if (model.EmployeeId > 0)
            {
                int StaffCategoryId = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId).StaffCategoryId;
                model.DesignationList = Common.PopulateDllList(_Service.PRMUnit.DesignationRepository.Get(d => d.Id == model.ToDesignationId).OrderBy(x => x.Name).ToList());
                model.StepList = Common.PopulateStepList(_Service.PRMUnit.JobGradeStepRepository.Get(d => d.JobGradeId == model.ToGradeId).OrderBy(x => x.StepName).ToList());
            }
            if (model.Type == "Increment")
            {
                model.EmploymentTypeList = Common.PopulateDllList(_Service.PRMUnit.EmploymentTypeRepository.Get(d => d.Id == model.ToEmploymentTypeId).OrderBy(x => x.Name).ToList());
            }
            else
            {
                model.EmploymentTypeList = Common.PopulateDllList(_Service.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            }

            model.ToZoneInfoList = Common.PopulateDdlZoneList(_Service.PRMUnit.ZoneInfoRepository.GetAll().ToList());
            model.ToEmploymentProcessList = Common.PopulateDllList(_Service.PRMUnit.EmploymentProcessRepository.GetAll().OrderBy(x => x.Name).ToList());
            model.ZoneListByUser = Common.PopulateDdlZoneList(MyAppSession.SelectedZoneList);

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
                model.EmployeeName = obj.FullName;
                model.JoiningDate = obj.DateofJoining;
                model.DateofConfirmation = obj.DateofConfirmation == null ? string.Empty : Convert.ToDateTime(obj.DateofConfirmation).ToString("yyyy-MM-dd");
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
                model.FromDesignation = obj.PRM_Designation == null ? String.Empty : obj.PRM_Designation.Name;
                model.FromEmploymentType = obj.PRM_EmploymentType == null ? String.Empty : obj.PRM_EmploymentType.Name;
                model.FromDivisionName = obj.PRM_Division == null ? string.Empty : obj.PRM_Division.Name;
                // model.FromJobLocationName = obj.PRM_JobLocation.Name;
                model.FromEmploymentProcessName = obj.PRM_EmploymentProcess == null ? String.Empty : obj.PRM_EmploymentProcess.Name;
                model.InitialBasic = basicObj.Amount;
                model.DateofConfirmation = obj.DateofConfirmation == null ? String.Empty : Convert.ToDateTime(obj.DateofConfirmation).ToString("yyyy-MM-dd");

                model.ToSalaryScale = _Service.PRMUnit.SalaryScaleRepository.GetByID(model.ToSalaryScaleId).SalaryScaleName;
                model.ToGrade = _Service.PRMUnit.JobGradeRepository.GetByID(model.ToGradeId).GradeName;
            }
        }

        private List<PRM_EmpStatusChangeDetail> UpdateEmpStatusChangeDetails(BEPZA_MEDICAL.DAL.PRM.PRM_EmpStatusChange entity)
        {
            List<PRM_EmpStatusChangeDetail> lst = new List<PRM_EmpStatusChangeDetail>();
            var emp = _Service.PRMUnit.EmploymentInfoRepository.GetByID(entity.EmployeeId);

            try
            {
                var salaryStructureDetails = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository
                    .Get(q => q.PRM_SalaryStructure.SalaryScaleId == entity.FromSalaryScaleId
                                && q.PRM_SalaryStructure.GradeId == entity.FromGradeId
                                && q.PRM_SalaryStructure.StepId == entity.FromStepId)
                    .ToList();

                int HeadId = 0;
                string BasedON = String.Empty;

                foreach (var salaryHead in salaryStructureDetails)
                {
                    HeadId = salaryHead.HeadId;
                    BasedON = salaryHead.PRM_SalaryStructure.PRM_JobGrade.IsConsolidated == true ? "Gross" : "Basic";

                    var item = _pgmCommonService.PGMUnit.SalaryStructureDetailRepository
                        .Get(q => q.PRM_SalaryStructure.SalaryScaleId == entity.ToSalaryScaleId
                            && q.PRM_SalaryStructure.GradeId == entity.ToGradeId
                            && q.PRM_SalaryStructure.StepId == entity.ToStepId
                            && q.HeadId == HeadId).FirstOrDefault();

                    var empChangeDetail = _Service.PRMUnit.EmpStatusChangeRepository.GetByID(entity.Id).PRM_EmpStatusChangeDetail.Where(q => q.HeadId == salaryHead.HeadId).FirstOrDefault();

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
                    lst.Add(empChangeDetail);
                }
            }
            catch { return null; }
            return lst;
        }

        private bool UpdateEmployeeInfoAndSalary(EmployeeConfirmationIncrementPromotionViewModel model)
        {
            var emp = _Service.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

            /* RH#01 */
            var ToEmploymentType = _Service.PRMUnit.EmploymentTypeRepository.GetByID(model.ToEmploymentTypeId);
            /* RH#01 End */

            emp.JobGradeId = model.ToGradeId;
            emp.EmploymentTypeId = model.ToEmploymentTypeId;
            emp.PRM_EmpSalary.SalaryScaleId = model.ToSalaryScaleId;

            emp.PRM_EmpSalary.GradeId = model.ToGradeId;
            emp.PRM_EmpSalary.StepId = model.ToStepId;
            emp.PRM_EmpSalary.GrossSalary = Convert.ToDecimal(model.ToGrossSalary);
            emp.PRM_EmpSalary.isConsolidated = model.ToIsConsolidated;

            // Confirmation is applicable for only contractual employee and they turn into regular employee
            if (model.Type.Equals(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Confirmation.ToString())) /*RH#02*/
            {
                if (ToEmploymentType.Name.Equals(PRMEnum.EmploymentType.Permanent.ToString())) /*RH#01*/
                {
                    emp.IsContractual = false;
                    emp.DateofConfirmation = model.EffectiveDate;
                    emp.PRM_EmpSalary.isConsolidated = false;

                    if (emp.DesignationId != model.ToDesignationId)
                    {
                        emp.DateofPosition = Convert.ToDateTime(model.EffectiveDate);
                    }
                }
            }

            if (model.Type.Equals(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Promotion.ToString())
                || model.Type.Equals(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Demotion.ToString())
                )
            {
                emp.DateofPosition = Convert.ToDateTime(model.EffectiveDate);

                if (model.ToOrganogramLevelId != null)
                {
                    emp.OrganogramLevelId = Convert.ToInt32(model.ToOrganogramLevelId);
                }
            }

            emp.DesignationId = model.ToDesignationId;

            try
            {
                if (!ToEmploymentType.Name.Equals(PRMEnum.EmploymentType.Contractual.ToString())) /* RH#01; Add this if condition only*/
                {
                    var salaryStructureDetail =
                        _pgmCommonService.PGMUnit
                            .SalaryStructureDetailRepository
                            .Get(q => q.PRM_SalaryStructure.SalaryScaleId == model.ToSalaryScaleId
                                    && q.PRM_SalaryStructure.GradeId == model.ToGradeId
                                    && q.PRM_SalaryStructure.StepId == model.ToStepId)
                            .ToList();

                    foreach (var salaryHead in salaryStructureDetail)
                    {
                        var empSalaryDetail = emp.PRM_EmpSalary.PRM_EmpSalaryDetail
                            .FirstOrDefault(q => q.HeadId == salaryHead.HeadId);

                        if (empSalaryDetail != null)
                        {
                            empSalaryDetail.AmountType = salaryHead.AmountType;
                            empSalaryDetail.Amount = salaryHead.Amount;
                            empSalaryDetail.HeadType = salaryHead.HeadType;
                            empSalaryDetail.IsTaxable = salaryHead.IsTaxable;
                        }
                    }
                } /* RH#01 End; Add this if condition only*/

            }
            catch { }
            try
            {
                Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                var lstChild = new ArrayList();
                foreach (var item in emp.PRM_EmpSalary.PRM_EmpSalaryDetail)
                {
                    item.EUser = User.Identity.Name;
                    item.EDate = Common.CurrentDateTime;
                    lstChild.Add(item);
                }
                NavigationList.Add(typeof(PRM_EmpSalaryDetail), lstChild);

                _Service.PRMUnit.EmploymentInfoRepository.Update(emp);
                _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();

                _Service.PRMUnit.EmpSalaryRepository.Update(emp.PRM_EmpSalary, "EmployeeId", NavigationList);
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

                _Service.PRMUnit.EmpSalaryRepository.Update(obj.PRM_EmpSalary, "EmployeeId", NavigationList);
                _Service.PRMUnit.EmploymentInfoRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        #endregion

        #region Public Methods

        // Get Gross salay and basic salary
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

        #region Grid Dropdown list

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

        [NoCache]
        public ActionResult GetJobLocation()
        {
            var jobLocations = _empService.PRMUnit.JobLocationRepository.GetAll().OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.GradeName).ToList();
            return PartialView("Select", Common.PopulateJobGradeDDL(grades));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _empService.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult DivisionforView()
        {
            var itemList = Common.PopulateDllList(_Service.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Id).ToList());
            return PartialView("Select", itemList);
        }

        [NoCache]
        public ActionResult GradeforView()
        {
            var itemlist = Common.PopulateJobGradeDDL(_JobGradeService.GetLatestJobGrade());
            return PartialView("Select", itemlist);
        }

        [NoCache]
        public ActionResult DesignationforView()
        {
            var itemList = Common.PopulateDllList(_Service.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }

        [NoCache]
        public ActionResult GetTypeView()
        {
            Dictionary<string, string> type = new Dictionary<string, string>();

            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Confirmation.ToString(), "Confirmation"); /*RH#02*/
            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Increment.ToString(), "Increment"); /*RH#02*/
            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Promotion.ToString(), "Promotion"); /*RH#02*/
            type.Add(BEPZA_MEDICAL.Utility.PRMEnum.EmployeeStatusChange.Demotion.ToString(), "Demotion"); /*RH#02*/

            return PartialView("_Select", type);
        }

        #endregion

        [NoCache]
        public JsonResult GetOrganogramHierarchyInfo(int organogramLevelId)
        {
            var list = _empService.GetEmployeeDeptOfficeSecInfoByOrgogramId2(organogramLevelId);
            var divisionId = string.Empty;

            foreach (var item in list)
            {

                if (item.OrganogramTypeName.Equals(PRMEnum.EmployeeOrganogram.Department.ToString()))// Department
                {
                    divisionId = item.OrganogramLevelId.ToString();
                }
            }

            return Json(new
            {
                divisionId = divisionId
            });
        }

        [NoCache]
        public JsonResult GetDesignationById(int designationId)
        {
            var designaionList = _Service.PRMUnit.DesignationRepository.Get(q => q.Id == designationId).ToList();
            return Json(
               new
               {
                   designations = designaionList.Select(x => new { Id = x.Id, Name = x.Name })
               },
               JsonRequestBehavior.AllowGet
           );
        }

        [NoCache]
        public JsonResult GetEmploymentTypeById(int employmentTypeId)
        {
            var employmentTypeList = _Service.PRMUnit.EmploymentTypeRepository.Get(q => q.Id == employmentTypeId).ToList();
            return Json(
               new
               {
                   employmentTypes = employmentTypeList.Select(x => new { Id = x.Id, Name = x.Name })
               },
               JsonRequestBehavior.AllowGet
           );
        }
    }
}