using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class AppointmentLetterInfoController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public AppointmentLetterInfoController(EmployeeService empService, PRMCommonSevice prmCommonService)
        {
            this._empService = empService;
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: /PRM/ClearanceChecklist/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AppointmentLetterInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<AppointmentLetterInfoViewModel> list = (from apLetter in _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.GetAll()
                                                         join applicantInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on apLetter.ApplicantInfoId equals applicantInfo.intPK
                                                         join desig in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on applicantInfo.DesignationId equals desig.Id
                                                         where (viewModel.DesignationId == 0 || viewModel.DesignationId == null || desig.Id == viewModel.DesignationId)
                                                         && (viewModel.Date == null || viewModel.Date == apLetter.Date)
                                                         && (viewModel.SelectedId == 0 || apLetter.SelectedId == viewModel.SelectedId)
                                                         && (viewModel.ApplicantName == null || viewModel.ApplicantName == "" || applicantInfo.Name.Contains(viewModel.ApplicantName))
                                                         select new AppointmentLetterInfoViewModel()
                                                         {
                                                             Id = apLetter.Id,
                                                             ApplicantName = applicantInfo.Name,
                                                             DesignationId = applicantInfo.DesignationId,
                                                             ApplicantPosition = desig.Name,
                                                             SelectedId = apLetter.SelectedId,
                                                             Date = apLetter.Date,
                                                         }).OrderBy(x => x.ApplicantName).ToList();


            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ApplicantName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApplicantName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApplicantName).ToList();
                }
            }


            if (request.SortingName == "ApplicantPosition")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApplicantPosition).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApplicantPosition).ToList();
                }
            }

            if (request.SortingName == "SelectedId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SelectedId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SelectedId).ToList();
                }
            }

            if (request.SortingName == "Date")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Date).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Date).ToList();
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
                    d.SelectedId, 
                    d.ApplicantName,
                    d.DesignationId,
                    d.ApplicantPosition,  
                    Convert.ToDateTime(d.Date).ToString(DateAndTime.GlobalDateFormat),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Create()
        {
            AppointmentLetterInfoViewModel model = new AppointmentLetterInfoViewModel();
            model.Subject = "Appointment Letter";
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(AppointmentLetterInfoViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        return View(model);
                    }

                    model = GetInsertUserAuditInfo(model, true);
                    var entity = model.ToEntity();

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        //   return RedirectToAction("Index");
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    // model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            return View(model);
        }


        public ActionResult Edit(int id, string type)
        {
            var entity = _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";

            model.ApplicantName = entity.EREC_tblgeneralinfo.Name;
            model.ApplicantPosition = _prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == entity.EREC_tblgeneralinfo.DesignationId).Select(s => s.Name).FirstOrDefault();

            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.DesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            if (type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View("Edit", model);
        }


        [HttpPost]
        public ActionResult Edit(AppointmentLetterInfoViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        return View(model);
                    }

                    model = GetInsertUserAuditInfo(model, false);
                    var entity = model.ToEntity();
                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        return RedirectToAction("Edit", new { id = entity.Id, type = "success" });
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }

            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    //model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.SaveChanges();
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
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }


        #endregion

        #region Private Method
        private bool CheckDuplicateEntry(AppointmentLetterInfoViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.Get(q => q.SelectedId == model.SelectedId).Any();
            }

            else
            {
                var ss = _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.Get(q => q.SelectedId == model.SelectedId && strMode != q.Id).Any();
                return _prmCommonService.PRMUnit.AppointmentLetterInfoRepository.Get(q => q.SelectedId == model.SelectedId && strMode != q.Id).Any();
            }
        }
        private AppointmentLetterInfoViewModel GetInsertUserAuditInfo(AppointmentLetterInfoViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
            }
            else
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
            }

            return model;
        }
        #endregion

        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var obj = _empService.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeName = obj.FullName,
                Designation = obj.PRM_Designation.Name

            });

        }


        public ActionResult SelectedApplicantSearch()
        {
            var model = new AppointmentLetterInfoViewModel();
            //return PartialView("_SelectedApplicantGridList", model);
            return View("SelectedApplicantSearch", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetSelectedApplicant(JqGridRequest request, AppointmentLetterInfoSelectedApplicantSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from selApplicnatApprovalDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.GetAll()
                        join selApplicnatDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll() on selApplicnatApprovalDtl.ApplicantInfoId equals selApplicnatDtl.ApplicantInfoId
                        join applicantInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on selApplicnatApprovalDtl.ApplicantInfoId equals applicantInfo.intPK
                        join desig in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on applicantInfo.DesignationId equals desig.Id
                        join jobPost in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on applicantInfo.CircularID equals jobPost.Id
                        join jobAdInfo in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on jobPost.JobAdvertisementInfoId equals jobAdInfo.Id
                        where (selApplicnatDtl.IsFinalSelected == true)
                        && (viewModel.JobAdvertisementInfoId == 0 || viewModel.JobAdvertisementInfoId == jobAdInfo.Id)
                        && (viewModel.DesignationId == 0 || desig.Id == viewModel.DesignationId)
                        && (viewModel.SelectedId == 0 || selApplicnatDtl.SelectedId == viewModel.SelectedId)
                        && (viewModel.ApplicantName == null || viewModel.ApplicantName == "" || applicantInfo.Name.Contains(viewModel.ApplicantName))
                        && (jobAdInfo.ZoneInfoId == LoggedUserZoneInfoId)
                        select new AppointmentLetterInfoSelectedApplicantSearchViewModel()
                            {
                                Id = selApplicnatApprovalDtl.Id,
                                ApplicantInfoId = selApplicnatApprovalDtl.ApplicantInfoId,
                                ApplicantName = selApplicnatApprovalDtl.EREC_tblgeneralinfo.Name,
                                SelectedId =Convert.ToInt32(selApplicnatDtl.SelectedId),
                                JobAdvertisementInfoId = jobAdInfo.Id,
                                JobAdvertisementCode = jobAdInfo.AdCode,
                                DesignationId = desig.Id,
                                DesignationName = desig.Name,
                            }).OrderBy(x => x.ApplicantName).ToList();


            totalRecords = list == null ? 0 : list.Count;

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
                    d.ApplicantInfoId,
                    d.ApplicantName,
                    d.SelectedId,
                    d.JobAdvertisementInfoId,    
                    d.JobAdvertisementCode,
                    d.DesignationId,
                    d.DesignationName
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }


        [NoCache]
        public JsonResult GetSelectedApplicantInfo(int id) //get selected applicant info by PRM_SelectedApplicantInfoApprovalDetail
        {
            var obj = (from selApplicnatApprovalDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.GetAll()
                       join selApplicnatDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll().Where(q=>q.SelectedId !=null) on selApplicnatApprovalDtl.ApplicantInfoId equals selApplicnatDtl.ApplicantInfoId
                       join desi in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on selApplicnatDtl.EREC_tblgeneralinfo.DesignationId equals desi.Id 
                       where (selApplicnatApprovalDtl.Id == id)
                       select new
                       {
                           ApplicantInfoId = selApplicnatApprovalDtl.ApplicantInfoId,
                           ApplicantName = selApplicnatApprovalDtl.EREC_tblgeneralinfo.Name,
                           DesignationName = desi.Name,
                           SelectedId = selApplicnatDtl.SelectedId
                       }).FirstOrDefault();
            return Json(new
            {
                ApplicantInfoId = obj == null ? 0 : obj.ApplicantInfoId,
                ApplicantName = obj == null ? string.Empty : obj.ApplicantName,
                DesignationName = obj == null ? string.Empty : obj.DesignationName,
                SelectedId = obj == null ? 0 : obj.SelectedId
            });

        }

        //Search
        [NoCache]
        public ActionResult GetJobAdvertisement()
        {
            var jobAd = _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();

            return PartialView("Select", Common.PopulateJobAdvertisementDDL(jobAd));
        }

        [NoCache]
        public ActionResult DesignationListView()
        {
            var designations = (from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                select des).Concat(from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                                   join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                                   join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                                   select des).OrderBy(o => o.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(designations));
        }
    }
}