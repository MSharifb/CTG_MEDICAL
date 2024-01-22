using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MyNotificationLib.Operation;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class SuspensionOfEmployeeController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Constructor
        public SuspensionOfEmployeeController(PRMCommonSevice prmCommonService, PGMCommonService pgmCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._pgmCommonService = pgmCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: /PRM/SuspensionOfEmployee/
        public ActionResult Index()
        {
            return View();
        }

        #region Search
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SuspensionOfEmployeeViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<SuspensionOfEmployeeViewModel> list = (from sus in _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.GetAll()
                                                        join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on sus.EmployeeId equals emp.Id
                                                        join sts in _prmCommonService.PRMUnit.EmploymentStatusRepository.GetAll() on emp.EmploymentStatusId equals sts.Id
                                                        where (model.EmpId == "" || model.EmpId == null || model.EmpId == emp.EmpID)
                                                        && (LoggedUserZoneInfoId == sus.ZoneInfoId)
                                                        select new SuspensionOfEmployeeViewModel()
                                                        {
                                                            Id = sus.Id,
                                                            EmpId = emp.EmpID,
                                                            SuspensionDate = sus.SuspensionDate,
                                                            FromDate = sus.FromDate,
                                                            ToDate = sus.ToDate,
                                                            Status = sts.Name
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
            if (request.SortingName == "SuspensionDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SuspensionDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SuspensionDate).ToList();
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
                  d.EmpId,
                  ((DateTime)d.SuspensionDate).ToString(DateAndTime.GlobalDateFormat),
                  ((DateTime)d.FromDate).ToString(DateAndTime.GlobalDateFormat),
                  ((DateTime)d.ToDate).ToString(DateAndTime.GlobalDateFormat),
                  d.Status,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        #endregion

        public ActionResult Create()
        {
            SuspensionOfEmployeeViewModel model = new SuspensionOfEmployeeViewModel();
            model.Status = "Active";
            populateDropdown(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SuspensionOfEmployeeViewModel model)
        {

            try
            {
                string errorList = string.Empty;
                model.IsError = 1;

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);

                    _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.Add(entity);
                    _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.SaveChanges();

                    if (model.FromDate <= DateTime.Now.Date && model.ToDate >= DateTime.Now.Date)
                    {
                        _prmCommonService.PRMUnit.EmploymentInfoRepository.Update(UpdateEmployeeInfo(model));
                        _prmCommonService.PRMUnit.EmploymentInfoRepository.SaveChanges();
                    }

                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                    try
                    {
                        Notification(model.EmployeeId, model.FromDate, model.ToDate, model.NotifyTo, model.Status);
                    }
                    catch (Exception) { }
                }
                else
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                    populateDropdown(model);
                    return View(model);
                }
            }
            catch
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
            }

            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var entity = _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.GetByID(id);
            var parentModel = entity.ToModel();

            parentModel.ErrMsg = BusinessLogicValidation(parentModel);
            if (!String.IsNullOrEmpty(parentModel.ErrMsg))
            {
                parentModel.IsSalaryProcessed = true;
                parentModel.ErrMsg = "Cannot be updated. " + parentModel.ErrMsg;
            }
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(parentModel.EmployeeId);
            parentModel.EmpId = obj.EmpID;
            parentModel.Name = obj.FullName;
            parentModel.Designation = obj.PRM_Designation.Name;
            parentModel.SalaryScaleName = obj.PRM_SalaryScale == null ? string.Empty : obj.PRM_SalaryScale.SalaryScaleName;
            parentModel.GradeName = obj.PRM_JobGrade == null ? string.Empty : obj.PRM_JobGrade.GradeName;
            parentModel.StepName = obj.PRM_JobGrade == null ? 0 : obj.PRM_JobGrade.PRM_GradeStep.Select(x => x.StepName).FirstOrDefault();

            List<SuspensionOfEmployeeViewModel> list = (from susDtl in _prmCommonService.PRMUnit.SuspensionOfEmployeeDetailRepository.GetAll()
                                                        join salHead in _prmCommonService.PRMUnit.SalaryHeadRepository.GetAll() on susDtl.HeadId equals salHead.Id
                                                        where (susDtl.SuspensionOfEmpId == id)
                                                        select new SuspensionOfEmployeeViewModel()
                                                         {
                                                             Id = susDtl.Id,
                                                             HeadId = susDtl.HeadId,
                                                             SalaryHead = salHead.HeadName,
                                                             ActualAmount = susDtl.ActualAmount,
                                                             Amount = susDtl.Amount,
                                                             AmountType = susDtl.AmountType,
                                                             HeadType = susDtl.HeadType

                                                         }).ToList();

            parentModel.SuspensionOfEmployeeDetailList = list;
            populateDropdown(parentModel);
            if (type == "success")
            {
                parentModel.IsError = 1;
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            else if (type == "saveSuccess")
            {
                parentModel.IsError = 1;
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }

            return View("Edit", parentModel);
        }

        [HttpPost]
        public ActionResult Edit(SuspensionOfEmployeeViewModel model)
        {
            try
            {
                string errorList = "";
                errorList = BusinessLogicValidation(model);
                if (ModelState.IsValid && String.IsNullOrEmpty(errorList))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);

                    if (errorList.Length == 0)
                    {
                        entity.EUser = User.Identity.Name;
                        entity.EDate = DateTime.Now;

                        _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.Update(entity);
                        _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.SaveChanges();

                        if (model.FromDate <= DateTime.Now.Date && model.ToDate >= DateTime.Now.Date)
                        {
                            _prmCommonService.PRMUnit.EmploymentInfoRepository.Update(UpdateEmployeeInfo(model));
                            _prmCommonService.PRMUnit.EmploymentInfoRepository.SaveChanges();
                        }

                        try
                        {
                            Notification(model.EmployeeId, model.FromDate, model.ToDate, model.NotifyTo, model.Status);
                        }
                        catch (Exception) { }

                        return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                    }
                }
                else
                {
                    model.IsError = 0;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 0;
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
            populateDropdown(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]

        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.GetByID(id);
            errMsg = BusinessLogicValidation(tempPeriod.ToModel());

            if (String.IsNullOrEmpty(errMsg))
            {
                try
                {
                    if (tempPeriod != null)
                    {
                        List<Type> allTypes = new List<Type> { typeof(PRM_SuspensionOfEmployeeDetail) };
                        _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.Delete(tempPeriod.Id, allTypes);
                        _prmCommonService.PRMUnit.SuspensionOfEmployeeRepository.SaveChanges();

                        //Update Employee Status
                        var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(tempPeriod.EmployeeId);
                        string status = Enum.GetName(typeof(PRMEnum.EmpStatus), Convert.ToInt32(PRMEnum.EmpStatus.Active));
                        var empStatus = _prmCommonService.PRMUnit.EmploymentStatusRepository.Get(q => q.Name == status).FirstOrDefault();
                        if (empStatus != null) obj.EmploymentStatusId = empStatus.Id;
                        _prmCommonService.PRMUnit.EmploymentInfoRepository.Update(obj);
                        _prmCommonService.PRMUnit.EmploymentInfoRepository.SaveChanges();

                        result = true;
                        errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }
            else
            {
                errMsg = "Cannot be deleted. " + errMsg;
            }
            return Json(new
            {
                Success = result,
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private PRM_SuspensionOfEmployee CreateEntity(SuspensionOfEmployeeViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.SuspensionOfEmployeeDetailList)
            {
                var prm_SuspensionOfEmployeeDetail = new PRM_SuspensionOfEmployeeDetail();

                prm_SuspensionOfEmployeeDetail.Id = c.Id;
                prm_SuspensionOfEmployeeDetail.HeadId = c.HeadId;
                prm_SuspensionOfEmployeeDetail.HeadType = c.HeadType;
                prm_SuspensionOfEmployeeDetail.AmountType = c.AmountType;
                prm_SuspensionOfEmployeeDetail.ActualAmount = c.ActualAmount;
                prm_SuspensionOfEmployeeDetail.Amount = c.Amount;
                prm_SuspensionOfEmployeeDetail.IsTaxable = c.IsTaxable;
                prm_SuspensionOfEmployeeDetail.IUser = User.Identity.Name;
                prm_SuspensionOfEmployeeDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    prm_SuspensionOfEmployeeDetail.IUser = User.Identity.Name;
                    prm_SuspensionOfEmployeeDetail.IDate = DateTime.Now;
                    entity.PRM_SuspensionOfEmployeeDetail.Add(prm_SuspensionOfEmployeeDetail);
                }
                else
                {
                    prm_SuspensionOfEmployeeDetail.SuspensionOfEmpId = model.Id;
                    prm_SuspensionOfEmployeeDetail.EUser = User.Identity.Name;
                    prm_SuspensionOfEmployeeDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.SuspensionOfEmployeeDetailRepository.Add(prm_SuspensionOfEmployeeDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.SuspensionOfEmployeeDetailRepository.Update(prm_SuspensionOfEmployeeDetail);

                    }
                }
                _prmCommonService.PRMUnit.SuspensionOfEmployeeDetailRepository.SaveChanges();
            }

            return entity;
        }

        #region Populate Dropdown
        private void populateDropdown(SuspensionOfEmployeeViewModel model)
        {

            #region Month Year
            model.MonthList = Common.PopulateMonthList();
            model.YearList = Common.PopulateYearList();
            #endregion

            #region Status
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Active", Value = "Active", Selected = true });
            list.Add(new SelectListItem() { Text = "Cancel", Value = "Cancel" });

            model.StatusList = list;

            #endregion

        }
        #endregion

        #region Employee Information
        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var emp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(empId);

            var salaryStructureId = 0;
            if (emp.PRM_SalaryScale != null)
            {
                if (_pgmCommonService.PGMUnit.SalaryStructureRepository.GetAll()
                    .Any(s => s.SalaryScaleId == emp.SalaryScaleId))
                {
                    salaryStructureId = _pgmCommonService.PGMUnit.SalaryStructureRepository.GetAll()
                        .FirstOrDefault(s => s.SalaryScaleId == emp.SalaryScaleId).Id;
                }
            }

            return Json(new
            {
                EmpId = emp.EmpID,
                SalaryStructureId = salaryStructureId,
                SalaryScaleId = emp.SalaryScaleId,
                GradeId = emp.JobGradeId,
                StepId = emp.PRM_JobGrade == null ? 0 : emp.PRM_JobGrade.PRM_GradeStep.Select(x => x.Id).FirstOrDefault(),
                EmployeeName = emp.FullName,
                Designation = emp.PRM_Designation.Name,
                SalaryScaleName = emp.PRM_SalaryScale == null ? string.Empty : emp.PRM_SalaryScale.SalaryScaleName,
                GradeName = emp.PRM_JobGrade == null ? string.Empty : emp.PRM_JobGrade.GradeName,
                StepName = emp.PRM_JobGrade == null ? 0 : emp.PRM_JobGrade.PRM_GradeStep.Select(x => x.StepName).FirstOrDefault(),
                GrossSalary = emp.PRM_EmpSalary == null ? 0 : emp.PRM_EmpSalary.GrossSalary,
                isConsolidated = emp.PRM_EmpSalary == null ? false : emp.PRM_EmpSalary.isConsolidated,
            });

        }
        #endregion

        #region Salary Payments
        [HttpGet]
        public PartialViewResult SalaryPayment(int empId)
        {
            var model = new SuspensionOfEmployeeViewModel();
            var list = (from empSalary in _prmCommonService.PRMUnit.EmpSalaryRepository.GetAll()
                        join empSalaryDtl in _prmCommonService.PRMUnit.EmpSalaryDetailRepository.GetAll() on empSalary.EmployeeId equals empSalaryDtl.EmployeeId
                        join salaryHead in _prmCommonService.PRMUnit.SalaryHeadRepository.GetAll() on empSalaryDtl.HeadId equals salaryHead.Id
                        where (empSalary.EmployeeId == empId)
                        select new SuspensionOfEmployeeViewModel
                        {
                            HeadId = salaryHead.Id,
                            SalaryHead = salaryHead.HeadName,
                            ActualAmount = empSalaryDtl.Amount,
                            AmountType = empSalaryDtl.AmountType,
                            HeadType = empSalaryDtl.HeadType,
                            Amount = empSalaryDtl.Amount
                        }
                        ).ToList();

            int basicHeadId = _prmCommonService.PRMUnit.SalaryHeadRepository.GetAll().Where(h => h.IsBasicHead = true).Select(h => h.Id).FirstOrDefault();
            decimal basic = list.Where(l => l.HeadId == basicHeadId).Select(l => l.ActualAmount).FirstOrDefault();
            foreach (var item in list)
            {
                if (item.AmountType == "Percent")
                {
                    //item.ActualAmount = (item.ActualAmount * 100) / basic;
                    item.Amount = Convert.ToDecimal((item.ActualAmount * basic) / 100);
                }
            }

            model.SuspensionOfEmployeeDetailList = list;
            return PartialView("_Detail", model);
        }

        [HttpGet]
        public PartialViewResult DeductionSalaryPayment(int empId)
        {
            var model = new SuspensionOfEmployeeViewModel();

            var list = (from empSalary in _prmCommonService.PRMUnit.EmpSalaryRepository.GetAll()
                        join empSalaryDtl in _prmCommonService.PRMUnit.EmpSalaryDetailRepository.GetAll() on empSalary.EmployeeId equals empSalaryDtl.EmployeeId
                        join salaryHead in _prmCommonService.PRMUnit.SalaryHeadRepository.GetAll() on empSalaryDtl.HeadId equals salaryHead.Id
                        where (empSalary.EmployeeId == empId)
                        select new SuspensionOfEmployeeViewModel
                        {
                            HeadId = salaryHead.Id,
                            SalaryHead = salaryHead.HeadName,
                            ActualAmount = empSalaryDtl.Amount,
                            AmountType = empSalaryDtl.AmountType,
                            HeadType = empSalaryDtl.HeadType,
                            Amount = empSalaryDtl.Amount
                        }
                        ).ToList();

            int basicHeadId = _prmCommonService.PRMUnit.SalaryHeadRepository.GetAll().Where(h => h.IsBasicHead = true).Select(h => h.Id).FirstOrDefault();
            decimal basic = list.Where(l => l.HeadId == basicHeadId).Select(l => l.ActualAmount).FirstOrDefault();
            foreach (var item in list)
            {
                if (item.AmountType == "Percent")
                {
                    //item.ActualAmount = (item.ActualAmount * 100) / basic;
                    item.Amount = Convert.ToDecimal((item.ActualAmount * basic) / 100);
                }
            }

            model.SuspensionOfEmployeeDetailList = list;
            return PartialView("_DetailMore", model);
        }

        #endregion
        private string BusinessLogicValidation(SuspensionOfEmployeeViewModel model)
        {
            string messege = string.Empty;

            //string fromYear = model.FromDate.Value.Year.ToString();
            //string fromMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.FromDate.Value.Month);
            //string toYear = model.ToDate.Value.Year.ToString();
            string toMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.ToDate.Value.Month);

            var salary = _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll().Where(s => (Convert.ToInt32(s.SalaryYear) >= model.FromDate.Value.Year &&
                         DateTime.ParseExact(s.SalaryMonth, "MMMM", CultureInfo.InvariantCulture).Month >= model.FromDate.Value.Month) &&
                         (Convert.ToInt32(s.SalaryYear) >= model.ToDate.Value.Year && DateTime.ParseExact(s.SalaryMonth, "MMMM", CultureInfo.InvariantCulture).Month >= model.ToDate.Value.Month) && s.EmployeeId == model.EmployeeId).ToList();
            if (salary != null)
            {
                messege = "Salary has been processed for the month of " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.FromDate.Value.Month) + "/" + model.ToDate.Value.Year;
            }

            return messege;
        }

        private PRM_EmploymentInfo UpdateEmployeeInfo(SuspensionOfEmployeeViewModel model)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            //obj.DateofInactive = model.EffectiveDate;
            string status = Enum.GetName(typeof(PRMEnum.EmpStatus), Convert.ToInt32(PRMEnum.EmpStatus.Suspended));
            var empStatus = _prmCommonService.PRMUnit.EmploymentStatusRepository.Get(q => q.Name == status).FirstOrDefault();
            if (empStatus != null) obj.EmploymentStatusId = empStatus.Id;
            return obj;
        }

        private void Notification(int employeeId, DateTime? effectiveFrom, DateTime? effectiveTo, String notifyToEmps, String suspendStatus)
        {

            #region Notification
            var enumType = MyNotificationLibEnum.NotificationType.Employee_Suspend;
            var redirectToUrl = String.Empty;

            // Declare Notification Variables
            var modules = new List<MyNotificationLibEnum.NotificationModule>();
            modules.Add(MyNotificationLibEnum.NotificationModule.Human_Resource_Management_System);

            var toEmployees = new List<int>();

            // Applicant info
            var applicant = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                .FirstOrDefault(e => e.Id == employeeId);
            var applicantInfo = applicant.FullName + ", " + (applicant.PRM_Designation.Name) + ", " +
                                applicant.EmpID;

            var customMessage = string.Empty;

            if (suspendStatus == "Active")
                customMessage = "Suspension of " + applicantInfo + " has issued from " + Common.GetDate(effectiveFrom).ToString(DateAndTime.GlobalDateFormat) +
                                " to " + Common.GetDate(effectiveTo).ToString(DateAndTime.GlobalDateFormat) + ".";
            else
                customMessage = "Suspension of " + applicantInfo + " has cancelled.";

            #region Self Notification
            toEmployees.Clear();
            toEmployees.Add(employeeId);

            var selfMessage = String.Empty;
            if (suspendStatus == "Active")
                selfMessage = "Your suspension will be effective from " +
                              Common.GetDate(effectiveFrom).ToString(DateAndTime.GlobalDateFormat) +
                              " to " + Common.GetDate(effectiveTo).ToString(DateAndTime.GlobalDateFormat);
            else
                selfMessage = "Your suspension has cancelled.";

            var notificationForApplicant = new SendGeneralPurposeNotification(
                modules,
                selfMessage,
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
                    if (_prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                        .Any(e => e.EmpID == empId.Trim()))
                    {
                        toEmployees.Add(_prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
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
                    , Common.GetDate(effectiveFrom)
                    , null
                    , redirectToUrl
                    , customMessage);
                notificationUtil.SendNotification();
            }
            catch (Exception) { }
            #endregion

            #endregion

        }


    }
}