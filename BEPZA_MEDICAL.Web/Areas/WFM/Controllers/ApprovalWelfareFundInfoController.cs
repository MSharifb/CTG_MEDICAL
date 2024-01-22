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
    public class ApprovalWelfareFundInfoController : BaseController
    {

        #region Fields

        private readonly WFMCommonService _wpfCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        private readonly WFM_ExecuteFunctions _wpffunction;
        #endregion

        #region Ctor
        public ApprovalWelfareFundInfoController(WFMCommonService wpfCommonService, PRMCommonSevice prmCommonService, WFM_ExecuteFunctions wpffunction)
        {
            this._wpfCommonService = wpfCommonService;
            this._prmCommonService = prmCommonService;
            this._wpffunction = wpffunction;
        }
        #endregion

        #region Actions
        //
        // GET: WFM/ApprovalApplicationInfo
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApprovalWelfareFundInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from awfInfo in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll()
                        select new ApprovalWelfareFundInfoViewModel()
                                                      {
                                                          Id = awfInfo.Id,
                                                          RefNo = awfInfo.RefNo,
                                                          CommitteeName = awfInfo.CommitteeName,
                                                          MeetDate = awfInfo.MeetDate,
                                                          MeetTime = awfInfo.MeetTime,
                                                          MeetPlace = awfInfo.MeetPlace,
                                                          CycleInfoId = awfInfo.CycleInfoId,
                                                          CycleName = awfInfo.WFM_CycleInfo.CycleName,
                                                          WelfareFundCategoryId = awfInfo.WelfareFundCategoryId,
                                                          WelfareFundCategoryName = awfInfo.WFM_WelfareFundCategory.Name,
                                                          Year = awfInfo.Year
                                                      }).OrderBy(x => x.MeetDate).ToList();

            if (request.Searching)
            {
                if ((viewModel.RefNo != null && viewModel.RefNo != ""))
                {
                    list = list.Where(d => d.RefNo == viewModel.RefNo).ToList();
                }


            }
            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "RefNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RefNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RefNo).ToList();
                }
            }

            if (request.SortingName == "CommitteeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CommitteeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CommitteeName).ToList();
                }
            }
            if (request.SortingName == "MeetDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MeetDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MeetDate).ToList();
                }
            }
            if (request.SortingName == "MeetPlace")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MeetPlace).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MeetPlace).ToList();
                }
            }
            if (request.SortingName == "CycleName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CycleName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CycleName).ToList();
                }
            }
            if (request.SortingName == "WelfareFundCategoryName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.WelfareFundCategoryName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.WelfareFundCategoryName).ToList();
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
                    d.RefNo,
                    d.CommitteeName,
                   Convert.ToDateTime(d.MeetDate).ToString(DateAndTime.GlobalDateFormat),
                    //d.MeetTime,
                    d.MeetPlace,
                    d.CycleInfoId,
                    d.CycleName,
                    d.WelfareFundCategoryId,
                    d.WelfareFundCategoryName,
                    d.Year
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult Create()
        {
            ApprovalWelfareFundInfoViewModel model = new ApprovalWelfareFundInfoViewModel();
            PopulateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(ApprovalWelfareFundInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;

                if (ModelState.IsValid)
                {
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = CreateEntity(model, true);
                    if (errorList.Length == 0)
                    {
                        _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.Add(entity);
                        _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = Resources.ErrorMessages.InsertFailed;
                //}
            }

            PopulateDropdown(model);
            SetEmployeeList(model);
            SetAccountBalance(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.CycleName = entity.WFM_CycleInfo.FromMonth + '-' + entity.WFM_CycleInfo.ToMonth;

            List<ApprovalWelfareFundInfoCommitteeViewModel> listCommittee = (from Committ in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoCommitteeRepository.GetAll().Where(q => q.ApprovalWelfareFundInfoId == id)
                                                                             join empInfo in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on Committ.MemberEmployeeId equals empInfo.Id into group2
                                                                             from g2 in group2.DefaultIfEmpty()
                                                                             select new ApprovalWelfareFundInfoCommitteeViewModel()
                                                                        {
                                                                            Id = Committ.Id,
                                                                            IsExternal = Committ.IsExternal,
                                                                            ApprovalWelfareFundInfoId = id,
                                                                            MemberEmployeeId = Committ.MemberEmployeeId,
                                                                            MemberEmpId = Committ.MemberEmployeeId == null ? string.Empty : g2.EmpID,
                                                                            MemberName = Committ.MemberEmployeeId == null ? Committ.MemberName : g2.FullName,
                                                                            MemberDesignation = Committ.MemberEmployeeId == null ? Committ.MemberDesignation : g2.PRM_Designation.Name,
                                                                            MemberRole = Committ.MemberRole,
                                                                            ActiveStatus = Committ.ActiveStatus,
                                                                            SortOrder = Committ.SortOrder
                                                                        }).OrderBy(o => o.SortOrder).ToList();
            model.ApprovalWelfareFundInfoCommittee = listCommittee;

            //employeeList

            List<ApprovalWelfareFundInfoEmployeeDetailsViewModel> EmployeeList = new List<ApprovalWelfareFundInfoEmployeeDetailsViewModel>();

            List<ApprovalWelfareFundInfoEmployeeDetailsViewModel> list = (from apvlEmpDtal in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll()
                                                                          join apvlFund in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll() on apvlEmpDtal.ApprovalWelfareFundInfoId equals apvlFund.Id
                                                                          join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on apvlEmpDtal.EmployeeId equals emp.Id
                                                                          join vw in _wpfCommonService.WFMUnit.VwApplicationRepository.GetAll() on apvlEmpDtal.ApplicationId equals vw.Id
                                                                          //join verifyDtlempInfo in _wpfCommonService.WFMUnit.VerifyTheApplicationDetailsRepository.GetAll() on apvlEmpDtal.VerifyTheApplicationDetailsId equals verifyDtlempInfo.Id
                                                                          where (apvlFund.Id == id) && vw.ApplicationStatus.Contains("Approve")

                                                                          select new ApprovalWelfareFundInfoEmployeeDetailsViewModel
                                                                          {
                                                                              Id = apvlEmpDtal.Id,
                                                                              IsCheckedFinal = true,
                                                                              ApplicationId = apvlEmpDtal.ApplicationId,
                                                                              IsOnline = apvlEmpDtal.IsOnline,
                                                                              EmployeeId = apvlEmpDtal.EmployeeId,
                                                                              EmpId = emp.EmpID,
                                                                              EmployeeName = emp.FullName,
                                                                              Designation = emp.PRM_Designation.Name,
                                                                              Department = emp.PRM_Division.Name,
                                                                              Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
                                                                              AppliedAmount = vw.AppliedAmount.ToString(),
                                                                              ApprovedAmount = apvlEmpDtal.ApprovedAmount
                                                                          }).ToList();
            List<ApprovalWelfareFundInfoEmployeeDetailsViewModel> list2 = (from x in _wpfCommonService.WFMUnit.VwApplicationRepository.GetAll()
                                                                           where x.CycleId == model.CycleInfoId
                                                                           && x.ZoneInfoId == LoggedUserZoneInfoId
                                                                           && x.ApplyYear.ToString() == model.Year
                                                                           && x.WelfareFundCategoryId == model.WelfareFundCategoryId
                                                                           && x.ApplicationStatus.Contains("Approve")
                                                                           select new ApprovalWelfareFundInfoEmployeeDetailsViewModel
                                                                           {
                                                                               ApplicationId = x.Id,
                                                                               IsOnline = Convert.ToBoolean(x.IsOnline),
                                                                               EmployeeId = x.EmployeeId,
                                                                               EmpId = x.EmpID,
                                                                               EmployeeName = x.EmployeeName,
                                                                               Designation = x.DesignationName,
                                                                               Department = x.DepartmentName,
                                                                               Section = x.SectionName,
                                                                               AppliedAmount = x.AppliedAmount.ToString(),
                                                                               ApprovedAmount = null
                                                                           }).ToList();

            var merged = new List<ApprovalWelfareFundInfoEmployeeDetailsViewModel>(list);
            merged.AddRange(list2.Where(p2 => list.All(p1 => p1.EmployeeId != p2.EmployeeId)));

            model.EmployeeList = merged;
            PopulateDropdown(model);
            SetAccountBalance(model);
            model.strMode = "Edit";
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(ApprovalWelfareFundInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;

                if (ModelState.IsValid)
                {
                    model = GetInsertUserAuditInfo(model, false);
                    var entity = CreateEntity(model, false);
                    if (errorList.Length == 0)
                    {
                        _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.Update(entity);
                        _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }

            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            PopulateDropdown(model);
            SetEmployeeList(model);
            SetAccountBalance(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.Delete(id);
                _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteCommittee")]
        public JsonResult DeleteCommitteeConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoCommitteeRepository.Delete(id);
                _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoCommitteeRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.Delete(id);
                _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.SaveChanges();
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

        #endregion

        #region Private Method

        private ApprovalWelfareFundInfoViewModel GetInsertUserAuditInfo(ApprovalWelfareFundInfoViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                foreach (var child in model.ApprovalWelfareFundInfoCommittee)
                {
                    child.IUser = User.Identity.Name;
                    child.IDate = DateTime.Now;
                }

            }
            else
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                foreach (var child in model.ApprovalWelfareFundInfoCommittee)
                {
                    child.IUser = model.IUser;
                    child.IDate = model.IDate;

                    child.EUser = User.Identity.Name;
                    child.EDate = DateTime.Now;
                }
            }

            return model;
        }


        private WFM_ApprovalWelfareFundInfo CreateEntity(ApprovalWelfareFundInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            //committee
            foreach (var c in model.ApprovalWelfareFundInfoCommittee)
            {
                var wfm_ApprovalWelfareFundInfoCommittee = new WFM_ApprovalWelfareFundInfoCommittee();

                wfm_ApprovalWelfareFundInfoCommittee.Id = c.Id;
                wfm_ApprovalWelfareFundInfoCommittee.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                wfm_ApprovalWelfareFundInfoCommittee.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                wfm_ApprovalWelfareFundInfoCommittee.EUser = c.EUser;
                wfm_ApprovalWelfareFundInfoCommittee.EDate = c.EDate;

                wfm_ApprovalWelfareFundInfoCommittee.IsExternal = c.IsExternal;
                wfm_ApprovalWelfareFundInfoCommittee.MemberEmployeeId = c.MemberEmployeeId;
                if (c.MemberEmployeeId == null || c.MemberEmployeeId == 0)
                {
                    wfm_ApprovalWelfareFundInfoCommittee.MemberName = c.MemberName;
                    wfm_ApprovalWelfareFundInfoCommittee.MemberDesignation = c.MemberDesignation;

                }
                else
                {
                    wfm_ApprovalWelfareFundInfoCommittee.MemberName = null;
                    wfm_ApprovalWelfareFundInfoCommittee.MemberDesignation = null;
                }

                wfm_ApprovalWelfareFundInfoCommittee.MemberRole = c.MemberRole;
                wfm_ApprovalWelfareFundInfoCommittee.ActiveStatus = c.ActiveStatus;
                wfm_ApprovalWelfareFundInfoCommittee.SortOrder = c.SortOrder;

                if (pAddEdit)
                {
                    wfm_ApprovalWelfareFundInfoCommittee.IUser = User.Identity.Name;
                    wfm_ApprovalWelfareFundInfoCommittee.IDate = DateTime.Now;
                    //entity.WFM_ApprovalWelfareFundInfoCommittee.Add(wfm_ApprovalWelfareFundInfoCommittee);
                }
                else
                {
                    wfm_ApprovalWelfareFundInfoCommittee.ApprovalWelfareFundInfoId = model.Id;
                    wfm_ApprovalWelfareFundInfoCommittee.EUser = User.Identity.Name;
                    wfm_ApprovalWelfareFundInfoCommittee.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoCommitteeRepository.Add(wfm_ApprovalWelfareFundInfoCommittee);
                    }
                    else
                    {
                        _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoCommitteeRepository.Update(wfm_ApprovalWelfareFundInfoCommittee);
                    }
                }
                _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoCommitteeRepository.SaveChanges();
            }

            //employee
            foreach (var item in model.EmployeeList.Where(q => q.IsCheckedFinal == true))
            {
                var wfm_ApprovalWelfareFundInfoEmployeeDetails = new WFM_ApprovalWelfareFundInfoEmployeeDetails();

                wfm_ApprovalWelfareFundInfoEmployeeDetails.Id = item.Id;
                wfm_ApprovalWelfareFundInfoEmployeeDetails.ApplicationId = item.ApplicationId;
                wfm_ApprovalWelfareFundInfoEmployeeDetails.IsOnline = item.IsOnline;
                //wfm_ApprovalWelfareFundInfoEmployeeDetails.VerifyTheApplicationDetailsId = item.VerifyTheApplicationDetailsId;
                wfm_ApprovalWelfareFundInfoEmployeeDetails.EmployeeId = item.EmployeeId;
                wfm_ApprovalWelfareFundInfoEmployeeDetails.ApprovedAmount = Convert.ToDecimal(item.ApprovedAmount);
                wfm_ApprovalWelfareFundInfoEmployeeDetails.IUser = User.Identity.Name;
                wfm_ApprovalWelfareFundInfoEmployeeDetails.IDate = DateTime.Now;

                if (Convert.ToDecimal(item.AppliedAmount) >= Convert.ToDecimal(item.ApprovedAmount))
                {
                    if (pAddEdit)
                    {
                        wfm_ApprovalWelfareFundInfoEmployeeDetails.IUser = User.Identity.Name;
                        wfm_ApprovalWelfareFundInfoEmployeeDetails.IDate = DateTime.Now;
                        entity.WFM_ApprovalWelfareFundInfoEmployeeDetails.Add(wfm_ApprovalWelfareFundInfoEmployeeDetails);
                    }
                    else
                    {
                        wfm_ApprovalWelfareFundInfoEmployeeDetails.ApprovalWelfareFundInfoId = model.Id;
                        wfm_ApprovalWelfareFundInfoEmployeeDetails.EUser = User.Identity.Name;
                        wfm_ApprovalWelfareFundInfoEmployeeDetails.EDate = DateTime.Now;

                        if (item.Id == 0)
                        {
                            _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.Add(wfm_ApprovalWelfareFundInfoEmployeeDetails);
                        }
                        else
                        {
                            _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.Update(wfm_ApprovalWelfareFundInfoEmployeeDetails);

                        }
                        _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.SaveChanges();
                    }
                }

            }

            return entity;
        }

        private void SetEmployeeList(ApprovalWelfareFundInfoViewModel model)
        {

            List<ApprovalWelfareFundInfoEmployeeDetailsViewModel> EmployeeList = (from x in _wpfCommonService.WFMUnit.VwApplicationRepository.Get(q => q.CycleId == model.CycleInfoId && q.ApplicationStatus.Contains("Approve")).ToList()
                                                                                  where x.ApplyYear.ToString() == model.Year
                                                                                  && x.WelfareFundCategoryId == model.WelfareFundCategoryId
                                                                                  && x.ZoneInfoId == LoggedUserZoneInfoId
                                                                                  select new ApprovalWelfareFundInfoEmployeeDetailsViewModel
                                                                                  {
                                                                                      EmployeeId = x.EmployeeId,
                                                                                      EmpId = x.EmpID,
                                                                                      EmployeeName = x.EmployeeName,
                                                                                      Designation = x.DesignationName,
                                                                                      Department = x.DepartmentName,
                                                                                      Section = x.SectionName,
                                                                                      AppliedAmount = x.AppliedAmount.ToString()
                                                                                  }).ToList();
            model.EmployeeList = EmployeeList;
            model.ShowRecord = "Show";
        }

        private void SetAccountBalance(ApprovalWelfareFundInfoViewModel model)
        {
            var coaId = _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetByID(model.WelfareFundCategoryId);
            var obj = _wpffunction.fnGetCOABudgetHeadAmountList(coaId.COAId, LoggedUserZoneInfoId).FirstOrDefault();
            var wfc = (from awf in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll()
                       join awfDtl in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll() on awf.Id equals awfDtl.ApprovalWelfareFundInfoId
                       where (awf.WelfareFundCategoryId == model.WelfareFundCategoryId)
                       select awfDtl
                     ).ToList();

            model.PaidAmout  =  wfc.Sum(x => x.ApprovedAmount == null ? 0 : x.ApprovedAmount);
            model.BalanceAmout = obj == null ? 0 :obj.Amount - model.PaidAmout;
            model.FinancialYear =obj == null ? string.Empty:  obj.yearName;

        }
        private void PopulateDropdown(ApprovalWelfareFundInfoViewModel model)
        {
            dynamic ddlList;

            #region Cycle Info ddl
            ddlList = _wpfCommonService.WFMUnit.CycleRepository.GetAll().OrderBy(x => x.CycleName).ToList();
            model.CycleInfoList = Common.PopulateCycleInfoDDL(ddlList);
            #endregion

            #region year ddl
            model.YearList = Common.PopulateYearList();
            #endregion

            #region Welfare Fund Category ddl
            ddlList = _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive == true).OrderBy(x => x.Name).ToList();
            model.WelfareFundCategoryList = Common.PopulateWelfareFundCategoryDDL(ddlList);
            #endregion

            #region Active Status  ddl
            model.ActiveStatusList = Common.PopulateYesNoDDLList();
            #endregion

        }

        #endregion

        [HttpGet]
        public PartialViewResult GetEmployeeList(int? cycleId, string year, int? welfareFundCategoryId)
        {
            var model = new ApprovalWelfareFundInfoViewModel();

            var approvedList = (from x in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll()
                                join vw in _wpfCommonService.WFMUnit.VwApplicationRepository.GetAll() on x.ApplicationId equals vw.Id
                                where vw.ApplicationStatus.Contains("Approve")
                                select new ApprovalWelfareFundInfoEmployeeDetailsViewModel
                                {
                                    EmployeeId = x.EmployeeId,
                                    EmpId = vw.EmpID,
                                    EmployeeName = vw.EmployeeName,
                                    Designation = vw.DesignationName,
                                    Department = vw.DepartmentName,
                                    Section = vw.SectionName,
                                    AppliedAmount = vw.AppliedAmount.ToString(),
                                    ApplicationId = x.ApplicationId,
                                    IsOnline = Convert.ToBoolean(x.IsOnline),
                                }).ToList();

            List<ApprovalWelfareFundInfoEmployeeDetailsViewModel> employeeList = (from x in _wpfCommonService.WFMUnit.VwApplicationRepository.Get(q => q.ApplicationStatus.Contains("Approve")).ToList()
                                                                                  where x.CycleId == cycleId
                                                                                  && x.ApplyYear.ToString() == year
                                                                                  && x.WelfareFundCategoryId == welfareFundCategoryId
                                                                                  select new ApprovalWelfareFundInfoEmployeeDetailsViewModel
                                                                                  {
                                                                                      EmployeeId = x.EmployeeId,
                                                                                      EmpId = x.EmpID,
                                                                                      EmployeeName = x.EmployeeName,
                                                                                      Designation = x.DesignationName,
                                                                                      Department = x.DepartmentName,
                                                                                      Section = x.SectionName,
                                                                                      AppliedAmount = x.AppliedAmount.ToString(),
                                                                                      ApplicationId = x.Id,
                                                                                      IsOnline = Convert.ToBoolean(x.IsOnline),
                                                                                  }).ToList();


            var finalResult = (from x in employeeList
                               where !(from y in approvedList
                                       select y.ApplicationId).Contains(x.ApplicationId)
                               select x
                                   ).ToList();

            //var finalResult = employeeList.Except(approvedList).ToList();


            model.EmployeeList = finalResult;
            return PartialView("_Details", model);
        }

        public ActionResult CycleListView()
        {
            var list = Common.PopulateCycleInfoDDL(_wpfCommonService.WFMUnit.CycleRepository.GetAll().OrderBy(x => x.CycleName).ToList());
            return PartialView("Select", list);
        }
        public ActionResult WelfarefundCategoryListView()
        {
            var list = Common.PopulateWelfareFundCategoryDDL(_wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll().Where(q => q.IsActive = true).OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        public ActionResult YearListView()
        {
            var list = Common.PopulateYearList();
            return PartialView("Select", list);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(ApprovalWelfareFundInfoViewModel model)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.MemberEmployeeId);
            return Json(new
            {
                EmpID = obj.EmpID,
                EmployeeName = obj.FullName,
                Designation = obj.PRM_Designation.Name,
                Department = obj.PRM_Division.Name
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult loadMeetingInfoById(int Id)
        {
            var Months = string.Empty;
            var obj = _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetByID(Id);
            return Json(new
            {
                CommitteeName = obj.CommitteeName,
                MeetDate = obj.MeetDate.ToString(DateAndTime.GlobalDateFormat),
                MeetTime = obj.MeetTime,
                MeetPlace = obj.MeetPlace,
                MeetAgenda = obj.MeetAgenda,
                Cycle = obj.WFM_CycleInfo.CycleName,
                CycleMonth = obj.WFM_CycleInfo.FromMonth + '-' + obj.WFM_CycleInfo.ToMonth,
                Year = obj.Year,
            }, JsonRequestBehavior.AllowGet);

        }

        [NoCache]
        public JsonResult LoadBudgetInfo(int welfareFundCategoryId)
        {
            var coaId = _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetByID(welfareFundCategoryId);
            var obj = _wpffunction.fnGetCOABudgetHeadAmountList(coaId.COAId, LoggedUserZoneInfoId).FirstOrDefault();
            var wfc = (from awf in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll()
                       join awfDtl in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll() on awf.Id equals awfDtl.ApprovalWelfareFundInfoId
                       where (awf.WelfareFundCategoryId == welfareFundCategoryId)
                       select awfDtl
                     ).ToList();

            var paidAmount = wfc.Sum(x => x.ApprovedAmount == null ? 0 : x.ApprovedAmount);
            var balance = obj == null ? 0 : obj.Amount - paidAmount;
            return Json(new
            {
                Year = obj == null ?string.Empty : obj.yearName,
                Amount = paidAmount,
                Balance = balance

            }, JsonRequestBehavior.AllowGet);
        }

    }
}