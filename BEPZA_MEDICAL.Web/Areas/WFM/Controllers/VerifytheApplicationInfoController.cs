using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.Controllers
{
    public class VerifytheApplicationInfoController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly WFMCommonService _wfmCommonService;
        #endregion

        #region Ctor
        public VerifytheApplicationInfoController(EmployeeService empService, WFMCommonService wfmCommonService)
        {
            this._empService = empService;
            this._wfmCommonService = wfmCommonService;
        }
        #endregion

        // GET: WFM/VerifytheApplicationInfo
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, VerifytheApplicationInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from verApp in _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.GetAll()
                        join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on verApp.VerifiedById equals emp.Id
                        where (viewModel.VerifiedByName == null || viewModel.VerifiedByName =="" || viewModel.VerifiedByName.Contains(emp.FullName))
                        select new VerifytheApplicationInfoViewModel()
                        {
                            Id = verApp.Id,
                            VerifyDate = verApp.VerifyDate,
                            VerifiedByName = emp.FullName,
                            ApplicationStatus=verApp.ApplicationStatus
                        }).OrderBy(x => x.VerifyDate).ToList();

            if (request.Searching)
            {
                if (viewModel.VerifyDate != null && viewModel.VerifyDate != DateTime.MinValue)
                {
                    list = list.Where(d => d.VerifyDate <= viewModel.VerifyDate).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "VerifiedByName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.VerifiedByName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.VerifiedByName).ToList();
                }
            }

            if (request.SortingName == "VerifyDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.VerifyDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.VerifyDate).ToList();
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
                    d.VerifiedByName,
                    (Convert.ToDateTime(d.VerifyDate)).ToString(DateAndTime.GlobalDateFormat),
                    d.ApplicationStatus
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            VerifytheApplicationInfoViewModel model = new VerifytheApplicationInfoViewModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(VerifytheApplicationInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                model.IsError = 1;
                //  errorList = BusinessLogicValidation(model);

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);
                    if (entity.Id <= 0)
                    {
                        _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.Add(entity);
                        _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        if (errorList.Length == 0)
                        {
                            entity.EUser = User.Identity.Name;
                            entity.EDate = DateTime.Now;

                            _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.Update(entity);
                            _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.SaveChanges();
                            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                        }
                    }

                }
                else
                {
                    populateDropdown(model);
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                populateDropdown(model);
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                return View(model);
            }
            return View(model);
        }

        public ActionResult Edit(int id,string type)
        {
            var entity = _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";

            #region Verified By

            var employee = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == model.VerifiedById).FirstOrDefault();
            model.VerifiedByName = employee.FullName;
            model.VerifiedByDepartment = employee.PRM_Division.Name;
            model.VerifiedByDesignation = employee.PRM_Designation.Name;

            #endregion

            #region Verify The Application Detail
            List<VerifytheApplicationInfoViewModel> list = (from verApp in _wfmCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.GetAll()
                                                            join onApp in _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetAll() on verApp.EmployeeId equals onApp.EmployeeId
                                                            join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on verApp.EmployeeId equals emp.Id
                                                            where (verApp.VerifyTheApplicationId == id)
                                                            select new VerifytheApplicationInfoViewModel()
                                                              {
                                                                  Id = verApp.Id,
                                                                  EmployeeId=verApp.EmployeeId,
                                                                  EmpId=emp.EmpID,
                                                                  EmployeeName=emp.FullName,
                                                                  Department=emp.PRM_Division.Name,
                                                                  Designation=emp.PRM_Designation.Name,
                                                                  Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
                                                                  ApplieedAmount=onApp.AppliedAmount.ToString(),
                                                                  AppliedAmount= onApp.AppliedAmount,
                                                                  AppliedDate=onApp.ApplicationDate.ToString("yyyy-MM-dd"),
                                                                  IsCheckedFinal=verApp.Status
                                                              }).Concat(from verApp in _wfmCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.GetAll()
                                                                        join offApp in _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetAll() on verApp.EmployeeId equals offApp.EmployeeId
                                                                        join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on verApp.EmployeeId equals emp.Id
                                                                        where (verApp.VerifyTheApplicationId == id)
                                                                        select new VerifytheApplicationInfoViewModel()
                                                                        {
                                                                            Id = verApp.Id,
                                                                            EmployeeId = verApp.EmployeeId,
                                                                            EmpId = emp.EmpID,
                                                                            EmployeeName = emp.FullName,
                                                                            Department = emp.PRM_Division.Name,
                                                                            Designation = emp.PRM_Designation.Name,
                                                                            Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
                                                                            ApplieedAmount = offApp.AppliedAmount.ToString(),
                                                                            AppliedAmount = offApp.AppliedAmount,
                                                                            AppliedDate = offApp.ApplicationDate.ToString("yyyy-MM-dd"),
                                                                            IsCheckedFinal = verApp.Status
                                                                        }).ToList();
            #endregion

            model.EmployeeList = list;
            populateDropdown(model);
            if (type == "success")
            {
                model.IsError = 1;
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View("Create", model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(WFM_VerifyTheApplicationDetails) };
                    _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.Delete(tempPeriod.Id, allTypes);
                    _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.SaveChanges();
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
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteDetail(int Id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _wfmCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.Delete(Id);
                _wfmCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.SaveChanges();
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

        private WFM_VerifyTheApplication CreateEntity([Bind(Exclude = "Attachment")] VerifytheApplicationInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            if (model.strMode == "Edit")
            {
                pAddEdit = false;
            }


            foreach (var c in model.EmployeeList)
            {
                var wfm_VerifyTheApplicationDetails = new WFM_VerifyTheApplicationDetails();

                wfm_VerifyTheApplicationDetails.Id = c.Id;
                wfm_VerifyTheApplicationDetails.EmployeeId= c.EmployeeId;
                wfm_VerifyTheApplicationDetails.Status = c.IsCheckedFinal;
                wfm_VerifyTheApplicationDetails.AppliedAmount = c.AppliedAmount;
                wfm_VerifyTheApplicationDetails.IUser = User.Identity.Name;
                wfm_VerifyTheApplicationDetails.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    wfm_VerifyTheApplicationDetails.IUser = User.Identity.Name;
                    wfm_VerifyTheApplicationDetails.IDate = DateTime.Now;
                    entity.WFM_VerifyTheApplicationDetails.Add(wfm_VerifyTheApplicationDetails);
                }
                else
                {
                    wfm_VerifyTheApplicationDetails.VerifyTheApplicationId = model.Id;
                    wfm_VerifyTheApplicationDetails.EUser = User.Identity.Name;
                    wfm_VerifyTheApplicationDetails.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _wfmCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.Add(wfm_VerifyTheApplicationDetails);
                    }
                    else
                    {
                        _wfmCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.Update(wfm_VerifyTheApplicationDetails);

                    }
                }
                _wfmCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.SaveChanges();
            }

            return entity;
        }

        [HttpGet]
        public PartialViewResult GetEmployeeList(int? zoneInfoId, int? departmentId, int? sectionId, int? cycleId, string year, int? welfareFundCategoryId)
        {
            var model = new VerifytheApplicationInfoViewModel();

            var ExistingList = (from vfi in _wfmCommonService.WFMUnit.VerifyTheApplicationRepository.GetAll().Where(x => x.CycleId == cycleId && x.WelfareFundCategoryId == welfareFundCategoryId)
                                join vfiDtl in _wfmCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.GetAll() on vfi.Id equals vfiDtl.VerifyTheApplicationId
                                select new VerifytheApplicationInfoViewModel
                                {
                                 EmployeeId=   vfiDtl.EmployeeId
                                }).ToList();


            List<VerifytheApplicationInfoViewModel> EmployeeList = new List<VerifytheApplicationInfoViewModel>();

            var list = (from sa in _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetAll()
                        join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on sa.EmployeeId equals emp.Id
                        where (zoneInfoId == sa.ZoneInfoId)
                        && (departmentId == null || departmentId == 0 || departmentId == emp.DivisionId)
                        && (sectionId == null || sectionId == 0 || sectionId == emp.SectionId)
                        && (welfareFundCategoryId == null || welfareFundCategoryId == 0 || welfareFundCategoryId == sa.WelfareFundCategoryId)
                        select new VerifytheApplicationInfoViewModel
                        {
                            EmployeeId = sa.EmployeeId,
                            EmployeeName = emp.FullName,
                            EmpId=emp.EmpID,
                            Department=emp.PRM_Division.Name,
                            Section = emp.SectionId == null?string.Empty: emp.PRM_Section.Name,
                            Designation=emp.PRM_Designation.Name,
                            ApplieedAmount=sa.AppliedAmount.ToString(),
                            AppliedAmount=sa.AppliedAmount,
                            DBAppliedDate = sa.ApplicationDate
                        }
                        ).Concat(from sa in _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetAll()
                                 join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on sa.EmployeeId equals emp.Id
                                 where (zoneInfoId == sa.ZoneInfoId)
                                 && (departmentId == null || departmentId == 0 || departmentId == emp.DivisionId)
                                 && (sectionId == null || sectionId == 0 || sectionId == emp.SectionId)
                                 && (welfareFundCategoryId == null || welfareFundCategoryId == 0 || welfareFundCategoryId == sa.WelfareFundCategoryId)
                                 select new VerifytheApplicationInfoViewModel
                                 {
                                     EmployeeId = sa.EmployeeId,
                                     EmployeeName = emp.FullName,
                                     EmpId = emp.EmpID,
                                     Department = emp.PRM_Division.Name,
                                     Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
                                     Designation = emp.PRM_Designation.Name,
                                     ApplieedAmount = sa.AppliedAmount.ToString(),
                                     AppliedAmount = sa.AppliedAmount,
                                     DBAppliedDate = sa.ApplicationDate
                                 }).ToList();

            #region Search
            if (year != string.Empty && year != "0" && year!="")
            {
                list = list.Where(x => x.DBAppliedDate.Year == Convert.ToInt32(year)).ToList();
            }
            if (cycleId != null && cycleId != 0)
            {
                var obj = _wfmCommonService.WFMUnit.CycleRepository.GetByID(cycleId);
                int frommonthInDigit = DateTime.ParseExact(obj.FromMonth, "MMM", CultureInfo.InvariantCulture).Month;
                int tomonthInDigit = DateTime.ParseExact(obj.ToMonth, "MMM", CultureInfo.InvariantCulture).Month;

                list = list.Where(x => x.DBAppliedDate.Month >= frommonthInDigit && x.DBAppliedDate.Month <= tomonthInDigit).ToList();
            }
            #endregion

            list = list.Where(n => !ExistingList.Select(x => x.EmployeeId).Contains(n.EmployeeId)).ToList();

            foreach (var vmEmp in list)
                {
                    var gridModel = new VerifytheApplicationInfoViewModel {
                        EmployeeId = vmEmp.EmployeeId,
                        EmpId = vmEmp.EmpId,
                        Designation = vmEmp.Designation, 
                        EmployeeName = vmEmp.EmployeeName, 
                        Department = vmEmp.Department,
                        Section = vmEmp.Section,
                        ApplieedAmount=vmEmp.ApplieedAmount,
                        AppliedAmount=vmEmp.AppliedAmount,
                        AppliedDate = vmEmp.DBAppliedDate.ToString("yyyy-MM-dd")
                    };
                  EmployeeList.Add(gridModel);
                }
                model.EmployeeList = EmployeeList;
            return PartialView("_Details", model);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(VerifytheApplicationInfoViewModel model)
        {
            var obj = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.VerifiedById);
            return Json(new
            {
                EmpID = obj.EmpID,
                EmployeeName = obj.FullName,
                Designation = obj.PRM_Designation.Name,
                Department= obj.PRM_Division.Name
            });

        }

        private void populateDropdown(VerifytheApplicationInfoViewModel model)
        {
            dynamic ddlList;

            #region Cycle Info ddl
            ddlList = _wfmCommonService.WFMUnit.CycleRepository.GetAll().OrderBy(x => x.CycleName).ToList();
            model.CycleList = Common.PopulateCycleInfoDDL(ddlList);
            #endregion

            #region year ddl

            model.YearList = Common.PopulateYearList();
            #endregion

            #region Welfare Fund Category ddl
            ddlList = _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q=>q.IsActive==true).OrderBy(x => x.Name).ToList();
            model.WelfareFundCategoryList = Common.PopulateWelfareFundCategoryDDL(ddlList);
            #endregion

            #region Zone
            ddlList = _empService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            model.ZoneList = Common.PopulateDdlZoneList(ddlList);
            #endregion

            #region Department
            ddlList = _empService.PRMUnit.DivisionRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            model.DepartmentList = Common.PopulateDllList(ddlList);
            #endregion

            #region Section
            ddlList = _empService.PRMUnit.SectionRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            model.SectionList = Common.PopulateDllList(ddlList);
            #endregion

        }

        [HttpPost]
        public ActionResult ViewApplicantInfo(int employeeId)
        {

            var model = new OfflineWelfareFundApplicationInformationViewModel();
            var resultFrm = (from OnApp in _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetAll()
                             join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on OnApp.SignatoryId equals emp.Id
                             join applicant in  _empService.PRMUnit.EmploymentInfoRepository.GetAll() on OnApp.EmployeeId equals applicant.Id
                             join cate in _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on OnApp.WelfareFundCategoryId equals cate.Id
                             where (OnApp.EmployeeId == employeeId)
                             select new OfflineWelfareFundApplicationInformationViewModel()
                             {
                                 Subject = OnApp.Subject,
                                 AppTo = OnApp.AppTo,
                                 Body = OnApp.Body,
                                 ApplicationDate = OnApp.ApplicationDate,
                                 AppliedAmount = OnApp.AppliedAmount,
                                 SignatoryEmpId = emp.EmpID,
                                 SignatoryEmpName = emp.FullName,
                                 SignatoryEmpDesignation = emp.PRM_Designation.Name,
                                 SignatoryEmpPhone = emp.TelephoneOffice,
                                 Reason = OnApp.Reason,
                                 WelfareFundCategoryName = cate.Name,
                                 ApplicantName = applicant.FullName,
                                 EmpID = applicant.EmpID,
                                 Designation = applicant.PRM_Designation.Name

                             }).Concat(from offApp in _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetAll()
                                       join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on offApp.SignatoryId equals emp.Id
                                       join applicant in  _empService.PRMUnit.EmploymentInfoRepository.GetAll() on offApp.EmployeeId equals applicant.Id
                                       join cate in _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on offApp.WelfareFundCategoryId equals cate.Id
                                       where (offApp.EmployeeId == employeeId)
                                       select new OfflineWelfareFundApplicationInformationViewModel()
                                       {
                                           Subject = offApp.Subject,
                                           AppTo = offApp.AppTo,
                                           Body = offApp.Body,
                                           ApplicationDate = offApp.ApplicationDate,
                                           AppliedAmount = offApp.AppliedAmount,
                                           SignatoryEmpId = emp.EmpID,
                                           SignatoryEmpName = emp.FullName,
                                           SignatoryEmpDesignation = emp.PRM_Designation.Name,
                                           SignatoryEmpPhone = emp.TelephoneOffice,
                                           Reason = offApp.Reason,
                                           WelfareFundCategoryName = cate.Name,
                                           ApplicantName = applicant.FullName,
                                           EmpID = applicant.EmpID,
                                           Designation = applicant.PRM_Designation.Name

                                       }).FirstOrDefault();

            model.SignatoryEmpId = resultFrm.SignatoryEmpId;
            model.SignatoryEmpName = resultFrm.SignatoryEmpName;
            model.SignatoryEmpPhone = resultFrm.SignatoryEmpPhone;
            model.SignatoryEmpDesignation = resultFrm.SignatoryEmpDesignation;
            model.AppTo = resultFrm.AppTo;
            model.Subject = resultFrm.Subject;
            model.Reason = resultFrm.Reason;
            model.WelfareFundCategoryName = resultFrm.WelfareFundCategoryName;

            model.Body = resultFrm.Body.Replace("@@ApplicantName", resultFrm.ApplicantName);
            model.Body = model.Body.Replace("@@ApplicantID",resultFrm.EmpID);
            model.Body = model.Body.Replace("@@Designation", resultFrm.Designation);
            model.Body = model.Body.Replace("@@Reason",resultFrm.Reason);
            model.Body = model.Body.Replace("@@AppliedAmount",resultFrm.AppliedAmount.ToString());
            model.Body = model.Body.Replace("@@WelfareFundCategory",resultFrm.WelfareFundCategoryName);





            return PartialView("_View", model);
        }

        [HttpPost]
        public ActionResult ViewApplicantHistory(int employeeId)
        {

            var model = new OfflineWelfareFundApplicationInformationViewModel();
            List<OfflineWelfareFundApplicationInformationViewModel> resultFrm = (from OnApp in _wfmCommonService.WFMUnit.OnlineApplicationInfoRepository.GetAll()
                                                                                 join WlfC in _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on OnApp.WelfareFundCategoryId equals WlfC.Id
                                                                                 where (OnApp.EmployeeId == employeeId)
                                                                                 select new OfflineWelfareFundApplicationInformationViewModel()
                                                                                 {
                                                                                     WelfareFundCategoryName = WlfC.Name,
                                                                                     Reason = OnApp.Reason,
                                                                                     ApplicationDate=OnApp.ApplicationDate,
                                                                                     AppliedAmount= OnApp.AppliedAmount

                                                                                 }).Concat(from offApp in _wfmCommonService.WFMUnit.OfflineApplicationInfoRepository.GetAll()
                                                                                           join WlfC in _wfmCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on offApp.WelfareFundCategoryId equals WlfC.Id
                                                                                           where (offApp.EmployeeId == employeeId)
                                                                                           select new OfflineWelfareFundApplicationInformationViewModel()
                                                                                           {
                                                                                               WelfareFundCategoryName = WlfC.Name,
                                                                                               Reason = offApp.Reason,
                                                                                               ApplicationDate = offApp.ApplicationDate,
                                                                                               AppliedAmount = offApp.AppliedAmount
                                                                                           }).ToList();

            model.HistoryList = resultFrm;
            return PartialView("_History", model);
        }

    }
}