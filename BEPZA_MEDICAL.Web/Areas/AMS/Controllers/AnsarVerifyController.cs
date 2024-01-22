//using BEPZA_MEDICAL.DAL.AMS;
//using BEPZA_MEDICAL.Domain.PRM;
//using BEPZA_MEDICAL.Domain.AMS;
//using BEPZA_MEDICAL.Web.Areas.AMS.ViewModel;
//using BEPZA_MEDICAL.Web.Controllers;
//using BEPZA_MEDICAL.Web.Utility;
//using Lib.Web.Mvc.JQuery.JqGrid;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Globalization;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace BEPZA_MEDICAL.Web.Areas.AMS.Controllers
//{
//    public class AnsarVerifyController : BaseController
//    {
//        #region Fields
//        private readonly EmployeeService _empService;
//        private readonly AMSCommonService _amsCommonService;
//        #endregion

//        #region Ctor
//        public AnsarVerifyController(EmployeeService empService, AMSCommonService amsCommonService)
//        {
//            this._empService = empService;
//            this._amsCommonService = amsCommonService;
//        }
//        #endregion

//        public ActionResult Index()
//        {
//            return View();
//        }
//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult GetList(JqGridRequest request, AnsarVerifyViewModel viewModel)
//        {
//            string filterExpression = String.Empty;
//            int totalRecords = 0;
//            var list = (from verAnsar in _amsCommonService.AMSUnit.AnsarVerifyRepository.GetAll()
//                        join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on verAnsar.VerifiedById equals emp.Id
//                        join verDetails in _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.GetAll() on verAnsar.Id equals verDetails.AnsarVerifyId
//                        where (viewModel.VerifiedByName == null || viewModel.VerifiedByName == "" || viewModel.VerifiedByName.Contains(emp.FullName))
//                        select new AnsarVerifyViewModel()
//                        {
//                            Id = verAnsar.Id,
//                            AnsarName = verDetails.AMS_AnsarEmpInfo.FullName,
//                            BEPZAID = verDetails.AMS_AnsarEmpInfo.BEPZAId,
//                            AnsarId = verDetails.AMS_AnsarEmpInfo.AnsarId,
//                            DateofJoining = verDetails.AMS_AnsarEmpInfo.DateofJoining,
//                            VerifyDate = verAnsar.VerifyDate,
//                            VerifiedByName = emp.FullName,
//                            Status = verAnsar.Status
//                        }).OrderBy(x => x.VerifyDate).ToList();

//            if (request.Searching)
//            {
//                if (viewModel.VerifyDate != null && viewModel.VerifyDate != DateTime.MinValue)
//                {
//                    list = list.Where(d => d.VerifyDate <= viewModel.VerifyDate).ToList();
//                }
//            }

//            totalRecords = list == null ? 0 : list.Count;

//            #region Sorting

//            if (request.SortingName == "VerifiedByName")
//            {
//                if (request.SortingOrder.ToString().ToLower() == "asc")
//                {
//                    list = list.OrderBy(x => x.VerifiedByName).ToList();
//                }
//                else
//                {
//                    list = list.OrderByDescending(x => x.VerifiedByName).ToList();
//                }
//            }

//            if (request.SortingName == "VerifyDate")
//            {
//                if (request.SortingOrder.ToString().ToLower() == "asc")
//                {
//                    list = list.OrderBy(x => x.VerifyDate).ToList();
//                }
//                else
//                {
//                    list = list.OrderByDescending(x => x.VerifyDate).ToList();
//                }
//            }

//            #endregion

//            JqGridResponse response = new JqGridResponse()
//            {
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };

//            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();


//            foreach (var d in list)
//            {

//                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
//                {
//                    d.Id,
//                    d.AnsarName,
//                    d.BEPZAID,
//                    d.AnsarId,
//                    (Convert.ToDateTime(d.DateofJoining)).ToString(DateAndTime.GlobalDateFormat),
//                    d.VerifiedByName,
//                    (Convert.ToDateTime(d.VerifyDate)).ToString(DateAndTime.GlobalDateFormat),
//                    d.Status
//                }));
//            }
//            return new JqGridJsonResult() { Data = response };
//        }

//        public ActionResult Create()
//        {
//            AnsarVerifyViewModel model = new AnsarVerifyViewModel();
//            populateDropdown(model);
//            return View(model);
//        }

//        [HttpPost]
//        [NoCache]
//        public ActionResult Create(AnsarVerifyViewModel model)
//        {
//            try
//            {
//                string errorList = string.Empty;
//                model.IsError = 1;

//                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
//                {
//                    model.ZoneInfoId = LoggedUserZoneInfoId;
//                    var entity = CreateEntity(model, true);
//                    if (entity.Id <= 0 && entity.AMS_AnsarVerifyDetails.Count() > 0)
//                    {
//                        _amsCommonService.AMSUnit.AnsarVerifyRepository.Add(entity);
//                        _amsCommonService.AMSUnit.AnsarVerifyRepository.SaveChanges();

//                        foreach (var item in entity.AMS_AnsarVerifyDetails)
//                        {
//                            var ansar = new AMS_AnsarEmpInfo();
//                            if (item.AMS_AnsarVerify.Status == "Rejected")
//                            {
//                                ansar = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(item.EmployeeId);

//                                var statusId = _amsCommonService.AMSUnit.EmpStatusRepository.Get(x => x.Name == "Inactive").FirstOrDefault().Id;

//                                ansar.StatusId = statusId;
//                                ansar.InactiveDate = DateTime.UtcNow;

//                                _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Update(ansar);
//                                _amsCommonService.AMSUnit.AnsarEmpInfoRepository.SaveChanges();
//                            }
//                        }


//                        model.IsError = 0;
//                        model.errClass = "success";
//                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
//                    }
//                    else
//                    {
//                        populateDropdown(model);
//                        model.IsError = 1;
//                        model.errClass = "failed";
//                        model.ErrMsg = "No row seleced.";
//                        return View(model);
//                    }

//                    #region Update
//                    //else
//                    //{
//                    //    if (errorList.Length == 0)
//                    //    {
//                    //        entity.EUser = User.Identity.Name;
//                    //        entity.EDate = DateTime.Now;

//                    //        _amsCommonService.AMSUnit.AnsarVerifyRepository.Update(entity);
//                    //        _amsCommonService.AMSUnit.AnsarVerifyRepository.SaveChanges();
//                    //        return RedirectToAction("Edit", new { id = model.Id, type = "success" });
//                    //    }
//                    //}
//                    #endregion
//                }
//                else
//                {
//                    populateDropdown(model);
//                    model.IsError = 1;
//                    model.errClass = "failed";
//                    model.ErrMsg = errorList;
//                    return View(model);
//                }
//            }
//            catch
//            {
//                populateDropdown(model);
//                model.IsError = 1;
//                model.errClass = "failed";
//                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
//                return View(model);
//            }
//            populateDropdown(model);
//            return View(model);
//        }

//        #region Edit & Delete
//        //public ActionResult Edit(int id, string type)
//        //{
//        //    var entity = _amsCommonService.AMSUnit.AnsarVerifyRepository.GetByID(id);
//        //    var model = entity.ToModel();
//        //    model.strMode = "Edit";

//        //    #region Verified By

//        //    var employee = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == model.VerifiedById).FirstOrDefault();
//        //    model.VerifiedByName = employee.FullName;
//        //    model.VerifiedByDepartment = employee.PRM_Division.Name;
//        //    model.VerifiedByDesignation = employee.PRM_Designation.Name;

//        //    #endregion

//        //    #region Verify Ansar Detail
//        //    List<AnsarVerifyViewModel> list = (from verAnsar in _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.GetAll()
//        //                                       join onApp in _amsCommonService.AMSUnit.OnlineApplicationInfoRepository.GetAll() on verApp.EmployeeId equals onApp.EmployeeId
//        //                                       join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on verApp.EmployeeId equals emp.Id
//        //                                       where (verApp.VerifyTheApplicationId == id)
//        //                                       select new AnsarVerifyViewModel()
//        //                                         {
//        //                                             Id = verApp.Id,
//        //                                             EmployeeId = verApp.EmployeeId,
//        //                                             EmpId = emp.EmpID,
//        //                                             EmployeeName = emp.FullName,
//        //                                             Department = emp.PRM_Division.Name,
//        //                                             Designation = emp.PRM_Designation.Name,
//        //                                             Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
//        //                                             ApplieedAmount = onApp.AppliedAmount.ToString(),
//        //                                             AppliedAmount = onApp.AppliedAmount,
//        //                                             AppliedDate = onApp.ApplicationDate.ToString("yyyy-MM-dd"),
//        //                                             IsCheckedFinal = verApp.Status
//        //                                         }).Concat(from verApp in _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.GetAll()
//        //                                                   join offApp in _amsCommonService.AMSUnit.OfflineApplicationInfoRepository.GetAll() on verApp.EmployeeId equals offApp.EmployeeId
//        //                                                   join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on verApp.EmployeeId equals emp.Id
//        //                                                   where (verApp.VerifyTheApplicationId == id)
//        //                                                   select new AnsarVerifyViewModel()
//        //                                                   {
//        //                                                       Id = verApp.Id,
//        //                                                       EmployeeId = verApp.EmployeeId,
//        //                                                       EmpId = emp.EmpID,
//        //                                                       EmployeeName = emp.FullName,
//        //                                                       Department = emp.PRM_Division.Name,
//        //                                                       Designation = emp.PRM_Designation.Name,
//        //                                                       Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
//        //                                                       ApplieedAmount = offApp.AppliedAmount.ToString(),
//        //                                                       AppliedAmount = offApp.AppliedAmount,
//        //                                                       AppliedDate = offApp.ApplicationDate.ToString("yyyy-MM-dd"),
//        //                                                       IsCheckedFinal = verApp.Status
//        //                                                   }).ToList();
//        //    #endregion

//        //    model.EmployeeList = list;
//        //    populateDropdown(model);
//        //    if (type == "success")
//        //    {
//        //        model.IsError = 1;
//        //        model.errClass = "success";
//        //        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
//        //    }
//        //    return View("Create", model);
//        //}

//        //[HttpPost, ActionName("Delete")]
//        //[NoCache]
//        //public JsonResult DeleteConfirmed(int id)
//        //{
//        //    bool result = false;
//        //    string errMsg = string.Empty;

//        //    var tempPeriod = _amsCommonService.AMSUnit.AnsarVerifyRepository.GetByID(id);
//        //    try
//        //    {
//        //        if (tempPeriod != null)
//        //        {
//        //            List<Type> allTypes = new List<Type> { typeof(WFM_VerifyTheApplicationDetails) };
//        //            _amsCommonService.AMSUnit.AnsarVerifyRepository.Delete(tempPeriod.Id, allTypes);
//        //            _amsCommonService.AMSUnit.AnsarVerifyRepository.SaveChanges();
//        //            result = true;
//        //            errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        result = false;
//        //        errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
//        //    }
//        //    return Json(new
//        //    {
//        //        Success = result,
//        //        Message = errMsg
//        //    }, JsonRequestBehavior.AllowGet);
//        //}

//        //[HttpPost]
//        //public ActionResult DeleteDetail(int Id)
//        //{
//        //    bool result;
//        //    string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
//        //    try
//        //    {
//        //        _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.Delete(Id);
//        //        _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.SaveChanges();
//        //        result = true;
//        //        errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
//        //    }
//        //    catch (UpdateException ex)
//        //    {
//        //        try
//        //        {
//        //            if (ex.InnerException != null && ex.InnerException is SqlException)
//        //            {
//        //                SqlException sqlException = ex.InnerException as SqlException;
//        //                errMsg = Common.GetSqlExceptionMessage(sqlException.Number);
//        //                 if (ex.InnerException.Message.Contains("REFERENCE constraint"))
//        //                 "The user has related information and cannot be deleted."
//        //                ModelState.AddModelError("Error", errMsg);
//        //            }
//        //            else
//        //            {
//        //                ModelState.AddModelError("Error", errMsg);
//        //            }
//        //            result = false;
//        //        }
//        //        catch (Exception)
//        //        {
//        //            result = false;
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        result = false;
//        //        errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
//        //    }


//        //    return Json(new
//        //    {
//        //        Success = result,
//        //        Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
//        //    });

//        //}
//        #endregion 

//        private AMS_AnsarVerify CreateEntity( AnsarVerifyViewModel model, bool pAddEdit)
//        {
//            var entity = model.ToEntity();
//            entity.Id = model.Id;
//            if (model.strMode == "Edit")
//            {
//                pAddEdit = false;
//            }


//            foreach (var c in model.AnsarList)
//            {
//                if(c.IsCheckedFinal == true)
//                {
//                    var ansarVerifyDetails = new AMS_AnsarVerifyDetails();

//                    ansarVerifyDetails.Id = c.Id;
//                    ansarVerifyDetails.EmployeeId = c.EmployeeId;
//                    ansarVerifyDetails.IUser = User.Identity.Name;
//                    ansarVerifyDetails.IDate = DateTime.Now;

//                    if (pAddEdit)
//                    {
//                        ansarVerifyDetails.IUser = User.Identity.Name;
//                        ansarVerifyDetails.IDate = DateTime.Now;
//                        entity.AMS_AnsarVerifyDetails.Add(ansarVerifyDetails);
//                    }
//                    else
//                    {
//                        ansarVerifyDetails.AnsarVerifyId = model.Id;
//                        ansarVerifyDetails.EUser = User.Identity.Name;
//                        ansarVerifyDetails.EDate = DateTime.Now;

//                        if (c.Id == 0)
//                        {
//                            _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.Add(ansarVerifyDetails);
//                        }
//                        else
//                        {
//                            _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.Update(ansarVerifyDetails);

//                        }
//                    }
//                    _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.SaveChanges();
//                }
//            }

//            return entity;
//        }

//        [HttpGet]
//        public PartialViewResult GetAnsarList(int? zoneInfoId, DateTime? fromDate, DateTime? endDate, int? designationId, string ansarName, string ansarId)
//        {
//            var model = new AnsarVerifyViewModel();

//            var ExistingList = (from vfi in _amsCommonService.AMSUnit.AnsarVerifyRepository.GetAll()
//                                join vfiDtl in _amsCommonService.AMSUnit.AnsarVerifyDetailsRepository.GetAll() on vfi.Id equals vfiDtl.AnsarVerifyId
//                                join ansInfo in _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetAll() on vfiDtl.EmployeeId equals ansInfo.Id
//                                where ansInfo.AMS_EmpStatus.Name == "Active" && ansInfo.IsFinalized == false
//                                select new AnsarVerifyViewModel
//                                {
//                                    AnsarId = ansInfo.AnsarId
//                                }).ToList();


//            List<AnsarVerifyViewModel> AnsarList = new List<AnsarVerifyViewModel>();

//            var list = (from ans in _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetAll()
//                        where ans.ZoneInfoId == zoneInfoId && ans.IsFinalized == false && ans.AMS_EmpStatus.Name == "Active"
//                        select new AnsarVerifyViewModel
//                        {
//                            EmployeeId = ans.Id,
//                            BEPZAID = ans.BEPZAId,
//                            AnsarName = ans.FullName,
//                            Designation = ans.AMS_DesignationInfo.DesignationName,
//                            AnsarId = ans.AnsarId,
//                            DateofJoining = ans.DateofJoining,
//                            AnsarJoiningDate = ans.AnsarJoiningDate,
//                            AnsarZoneInfoId = ans.ZoneInfoId
//                        }
//                        ).ToList();

//            #region Search

//            if (fromDate != null)
//            {
//                list = list.Where(x => x.DateofJoining >= fromDate).ToList();
//            }
//            if (endDate != null)
//            {
//                list = list.Where(x => x.DateofJoining <= endDate).ToList();
//            }
//            if (!string.IsNullOrEmpty(ansarName))
//            {
//                list = list.Where(x => x.AnsarName.ToLower().Contains(ansarName.Trim().ToLower())).ToList();
//            }
//            if (!string.IsNullOrEmpty(ansarId))
//            {
//                list = list.Where(x => x.AnsarId.ToLower().Contains(ansarId.Trim().ToLower())).ToList();
//            }
//            if (designationId != null && designationId != 0)
//            {
//                var desigName = _amsCommonService.AMSUnit.AnsarDesignationRepository.GetByID(designationId).DesignationName;
//                list = list.Where(x => x.Designation == desigName).ToList();
//            }

//            #endregion

//            list = list.Where(n => !ExistingList.Select(x => x.AnsarId).Contains(n.AnsarId)).ToList();

//            foreach (var vmEmp in list)
//            {
//                var gridModel = new AnsarVerifyViewModel
//                {
//                    EmployeeId = vmEmp.EmployeeId,
//                    BEPZAID = vmEmp.BEPZAID,
//                    AnsarName = vmEmp.AnsarName,
//                    Designation = vmEmp.Designation,
//                    AnsarId = vmEmp.AnsarId,
//                    DateofJoining = vmEmp.DateofJoining,
//                    AnsarJoiningDate = vmEmp.AnsarJoiningDate
//                };
//                AnsarList.Add(gridModel);
//            }
//            model.AnsarList = AnsarList;
//            return PartialView("_Details", model);
//        }

//        [NoCache]
//        public JsonResult GetEmployeeInfo(AnsarVerifyViewModel model)
//        {
//            var obj = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.VerifiedById);
//            return Json(new
//            {
//                EmpID = obj.EmpID,
//                EmployeeName = obj.FullName,
//                Designation = obj.PRM_Designation.Name,
//                Department = obj.PRM_Division.Name
//            });

//        }

//        private void populateDropdown(AnsarVerifyViewModel model)
//        {
//            CustomMembershipProvider _provider = new CustomMembershipProvider();

//            #region Zone
//            var ddlZoneList = _provider.GetZoneList(System.Web.HttpContext.Current.User.Identity.Name, LoggedUserZoneInfoId);
//            model.ZoneList = ddlZoneList.Select( y => new SelectListItem(){Text = y.ZoneName, Value = y.ZoneId.ToString()} ).ToList();
//            #endregion

//            #region Designation
//            var ddlDesignationList = _amsCommonService.AMSUnit.AnsarDesignationRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
//            model.DesignationList = ddlDesignationList.Select(y => new SelectListItem() { Text = y.DesignationName, Value = y.Id.ToString() }).ToList();
//            #endregion

//        }

//        //[HttpPost]
//        //public ActionResult ViewApplicantInfo(int employeeId)
//        //{

//        //    var model = new OfflineWelfareFundApplicationInformationViewModel();
//        //    var resultFrm = (from OnApp in _amsCommonService.AMSUnit.OnlineApplicationInfoRepository.GetAll()
//        //                     join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on OnApp.SignatoryId equals emp.Id
//        //                     join cate in _amsCommonService.AMSUnit.WelfareFundCategoryRepository.GetAll() on OnApp.WelfareFundCategoryId equals cate.Id
//        //                     where (OnApp.EmployeeId == employeeId)
//        //                     select new OfflineWelfareFundApplicationInformationViewModel()
//        //                     {
//        //                         Subject = OnApp.Subject,
//        //                         AppTo = OnApp.AppTo,
//        //                         Body = OnApp.Body,
//        //                         ApplicationDate = OnApp.ApplicationDate,
//        //                         AppliedAmount = OnApp.AppliedAmount,
//        //                         SignatoryEmpId = emp.EmpID,
//        //                         SignatoryEmpName = emp.FullName,
//        //                         SignatoryEmpDesignation = emp.PRM_Designation.Name,
//        //                         SignatoryEmpPhone = emp.TelephoneOffice,
//        //                         Reason = OnApp.Reason,
//        //                         WelfareFundCategoryName = cate.Name

//        //                     }).Concat(from offApp in _amsCommonService.AMSUnit.OfflineApplicationInfoRepository.GetAll()
//        //                               join emp in _empService.PRMUnit.EmploymentInfoRepository.GetAll() on offApp.SignatoryId equals emp.Id
//        //                               join cate in _amsCommonService.AMSUnit.WelfareFundCategoryRepository.GetAll() on offApp.WelfareFundCategoryId equals cate.Id
//        //                               where (offApp.EmployeeId == employeeId)
//        //                               select new OfflineWelfareFundApplicationInformationViewModel()
//        //                               {
//        //                                   Subject = offApp.Subject,
//        //                                   AppTo = offApp.AppTo,
//        //                                   Body = offApp.Body,
//        //                                   ApplicationDate = offApp.ApplicationDate,
//        //                                   AppliedAmount = offApp.AppliedAmount,
//        //                                   SignatoryEmpId = emp.EmpID,
//        //                                   SignatoryEmpName = emp.FullName,
//        //                                   SignatoryEmpDesignation = emp.PRM_Designation.Name,
//        //                                   SignatoryEmpPhone = emp.TelephoneOffice,
//        //                                   Reason = offApp.Reason,
//        //                                   WelfareFundCategoryName = cate.Name

//        //                               }).FirstOrDefault();

//        //    model.SignatoryEmpId = resultFrm.SignatoryEmpId;
//        //    model.SignatoryEmpName = resultFrm.SignatoryEmpName;
//        //    model.SignatoryEmpPhone = resultFrm.SignatoryEmpPhone;
//        //    model.SignatoryEmpDesignation = resultFrm.SignatoryEmpDesignation;
//        //    model.AppTo = resultFrm.AppTo;
//        //    model.Subject = resultFrm.Subject;
//        //    model.Body = resultFrm.Body;
//        //    model.Reason = resultFrm.Reason;
//        //    model.WelfareFundCategoryName = resultFrm.WelfareFundCategoryName;

//        //    return PartialView("_View", model);
//        //}

//        //[HttpPost]
//        //public ActionResult ViewApplicantHistory(int employeeId)
//        //{

//        //    var model = new OfflineWelfareFundApplicationInformationViewModel();
//        //    List<OfflineWelfareFundApplicationInformationViewModel> resultFrm = (from OnApp in _amsCommonService.AMSUnit.OnlineApplicationInfoRepository.GetAll()
//        //                                                                         join WlfC in _amsCommonService.AMSUnit.WelfareFundCategoryRepository.GetAll() on OnApp.WelfareFundCategoryId equals WlfC.Id
//        //                                                                         where (OnApp.EmployeeId == employeeId)
//        //                                                                         select new OfflineWelfareFundApplicationInformationViewModel()
//        //                                                                         {
//        //                                                                             WelfareFundCategoryName = WlfC.Name,
//        //                                                                             Reason = OnApp.Reason,
//        //                                                                             ApplicationDate = OnApp.ApplicationDate,
//        //                                                                             AppliedAmount = OnApp.AppliedAmount

//        //                                                                         }).Concat(from offApp in _amsCommonService.AMSUnit.OfflineApplicationInfoRepository.GetAll()
//        //                                                                                   join WlfC in _amsCommonService.AMSUnit.WelfareFundCategoryRepository.GetAll() on offApp.WelfareFundCategoryId equals WlfC.Id
//        //                                                                                   where (offApp.EmployeeId == employeeId)
//        //                                                                                   select new OfflineWelfareFundApplicationInformationViewModel()
//        //                                                                                   {
//        //                                                                                       WelfareFundCategoryName = WlfC.Name,
//        //                                                                                       Reason = offApp.Reason,
//        //                                                                                       ApplicationDate = offApp.ApplicationDate,
//        //                                                                                       AppliedAmount = offApp.AppliedAmount
//        //                                                                                   }).ToList();

//        //    model.HistoryList = resultFrm;
//        //    return PartialView("_History", model);
//        //}

//    }
//}