using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class SelectedApplicantInfoApprovalController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public SelectedApplicantInfoApprovalController(EmployeeService empService, PRMCommonSevice prmCommonService)
        {
            this._empService = empService;
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions


        //
        // GET: /PRM/SelectedApplicantInfoApproval/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SelectedApplicantInfoApprovalViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<SelectedApplicantInfoApprovalViewModel> list = (from selAppAroval in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.GetAll()
                                                                 join jobAdRe in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on selAppAroval.JobAdvertisementInfoId equals jobAdRe.JobAdvertisementInfoId
                                                                 where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == selAppAroval.JobAdvertisementInfoId)
                                                                       && (model.DesignationId == 0 || model.DesignationId == null || model.DesignationId == jobAdRe.DesignationId)
                                                                       && (model.DepartmentId == 0 || model.DepartmentId == null || model.DepartmentId == jobAdRe.DepartmentId)
                                                                       && (model.SectionId == 0 || model.SectionId == null || model.SectionId == jobAdRe.SectionId)
                                                                       && (selAppAroval.PRM_JobAdvertisementInfo.ZoneInfoId==LoggedUserZoneInfoId)
                                                                 select new SelectedApplicantInfoApprovalViewModel()
                                                                 {
                                                                     Id = selAppAroval.Id,
                                                                     JobAdvertisementInfoId = selAppAroval.JobAdvertisementInfoId,
                                                                     AdvertisementCode = selAppAroval.PRM_JobAdvertisementInfo.AdCode,
                                                                     DesignationId = jobAdRe.DesignationId,
                                                                     DesignationName = jobAdRe.PRM_Designation.Name,
                                                                     DepartmentId = jobAdRe.DepartmentId,
                                                                     DepartmentName = jobAdRe.PRM_Division.Name,
                                                                     SectionId = jobAdRe.SectionId == null ? null : jobAdRe.SectionId,
                                                                     SectionName = jobAdRe.SectionId == null ? string.Empty : jobAdRe.PRM_Section.Name,
                                                                     Status = selAppAroval.IsSubmit.ToString() == "True" ? "Approved" : "Rejected",
                                                                 }).Concat(from selAppAroval in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.GetAll()
                                                                           join jobAdRe in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on selAppAroval.JobAdvertisementInfoId equals jobAdRe.JobAdvertisementInfoId
                                                                           where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == selAppAroval.JobAdvertisementInfoId)
                                                                                 && (model.DesignationId == 0 || model.DesignationId == null || model.DesignationId == jobAdRe.DesignationId)
                                                                                 && (model.DepartmentId == 0 || model.DepartmentId == null || model.DepartmentId == jobAdRe.DepartmentId)
                                                                                 && (model.SectionId == 0 || model.SectionId == null || model.SectionId == jobAdRe.SectionId)
                                                                                 && (selAppAroval.PRM_JobAdvertisementInfo.ZoneInfoId == LoggedUserZoneInfoId)
                                                                           select new SelectedApplicantInfoApprovalViewModel()
                                                                           {
                                                                               Id = selAppAroval.Id,
                                                                               JobAdvertisementInfoId = selAppAroval.JobAdvertisementInfoId,
                                                                               AdvertisementCode = selAppAroval.PRM_JobAdvertisementInfo.AdCode,
                                                                               DesignationId = jobAdRe.DesignationId,
                                                                               DesignationName = jobAdRe.PRM_Designation.Name,
                                                                               DepartmentId = jobAdRe.DepartmentId,
                                                                               DepartmentName = jobAdRe.PRM_Division == null ? null : jobAdRe.PRM_Division.Name,
                                                                               SectionId = jobAdRe.SectionId == null ? null : jobAdRe.SectionId,
                                                                               SectionName = jobAdRe.SectionId == null ? string.Empty : jobAdRe.PRM_Section.Name,
                                                                               Status = selAppAroval.IsSubmit.ToString() == "True" ? "Approved" : "Rejected",
                                                                           }).DistinctBy(x => x.AdvertisementCode).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "JobAdvertisementCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.JobAdvertisementCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.JobAdvertisementCode).ToList();
                }
            }


            if (request.SortingName == "DesignationName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DesignationName).ToList();
                }
            }

            if (request.SortingName == "DepartmentName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DepartmentName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DepartmentName).ToList();
                }
            }

            if (request.SortingName == "SectionName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SectionName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SectionName).ToList();
                }
            }



            if (request.SortingName == "Status")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Status).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Status).ToList();
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
                  d.JobAdvertisementInfoId,
                  d.AdvertisementCode,
                  d.DesignationId,
                  d.DesignationName,
                  d.DepartmentId,
                  d.DepartmentName,
                  d.SectionId,
                  d.SectionName,
                  d.Status,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            SelectedApplicantInfoApprovalViewModel model = new SelectedApplicantInfoApprovalViewModel();
            model.CandidateType = true;
            populateDropdown(model);
            model.IsAddAttachment = true;
            model.strMode = "Create";

            //Approved By
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name); //login information
            model.EmployeeId = loginUser.ID;
            model.EmployeeName = loginUser.EmpName;
            model.DesignationName = loginUser.DesignationName;

            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] SelectedApplicantInfoApprovalViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = CreateEntity(model, true);

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

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.Add(entity);
                        _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.SaveChanges();
                        model.errClass = "success";
                        model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        // return RedirectToAction("Index");
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
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            populateDropdown(model);
            return View(model);
        }


        [NoCache]
        public ActionResult Edit(int Id, string type)
        {
            var entity = _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.GetByID(Id);
            var model = entity.ToModel();
            model.JobAdvertisementCode = entity.PRM_JobAdvertisementInfo.AdCode;

            model.strMode = "Edit";
            populateDropdown(model);
            DownloadDoc(model);
            model.IsAddAttachment = true;

            model.EmployeeId = model.EmployeeId;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.DesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

            //Job post information          

            var jobPostList = (from selAplApprov in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.GetAll()
                               join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on selAplApprov.JobAdvertisementInfoId equals jobAd.Id
                               join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                               where (selAplApprov.Id == Id)
                               select new SelectedApplicantInfoApprovalViewModel()
                               {
                                   DesignationId = jobAdReq.DesignationId,
                                   DesignationName = jobAdReq.PRM_Designation.Name,
                                   DepartmentId = jobAdReq.DepartmentId,
                                   DepartmentName = jobAdReq.PRM_Division.Name,
                                   SectionId = jobAdReq.PRM_Section == null ? null : jobAdReq.SectionId,
                                   SectionName = jobAdReq.PRM_Section == null ? string.Empty : jobAdReq.PRM_Section.Name,
                                   NoOfPost = jobAdReq.NumberOfClearancePosition,
                                   //IsChecked = true,
                                   strMode = "Edit"
                               }).Concat(from selAplApprov in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.GetAll()
                                         join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on selAplApprov.JobAdvertisementInfoId equals jobAd.Id
                                         join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                         where (selAplApprov.Id == Id)
                                         select new SelectedApplicantInfoApprovalViewModel()
                                         {
                                             DesignationId = jobAdReq.DesignationId,
                                             DesignationName = jobAdReq.PRM_Designation.Name,
                                             DepartmentId = jobAdReq.DepartmentId,
                                             DepartmentName = jobAdReq.PRM_Division == null ? string.Empty : jobAdReq.PRM_Division.Name,
                                             SectionId = jobAdReq.PRM_Section == null ? null : jobAdReq.SectionId,
                                             SectionName = jobAdReq.PRM_Section == null ? string.Empty : jobAdReq.PRM_Section.Name,
                                             NoOfPost = jobAdReq.NumberOfPosition,
                                             //IsChecked = true,
                                             strMode = "Edit"
                                         }).ToList();

            model.JobPostInformationList = jobPostList;


            //selected Applicant Approval Detail

            var list = (from selApplicantApproval in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.GetAll()
                        join selApplicantApprovalDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.GetAll() on selApplicantApproval.Id equals selApplicantApprovalDtl.SelectedApplicantInfoApprovalId
                        join selApplicantDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll() on selApplicantApprovalDtl.ApplicantInfoId equals selApplicantDtl.ApplicantInfoId
                        join applicant in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on selApplicantApprovalDtl.ApplicantInfoId equals applicant.intPK
                        join tstRslt in _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll() on applicant.intPK equals tstRslt.ApplicantInfoId
                        join intView in _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll() on selApplicantApproval.JobAdvertisementInfoId equals intView.AdvertisementInfoId
                        join selCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on intView.SelectionCriteriaOrExamTypeId equals selCriDtl.SelectionCriteriaOrExamTypeId
                        where (selApplicantApproval.Id == Id)
                        select new SelectedApplicantInfoApprovalViewModel()
                        {
                            Id = selApplicantApprovalDtl.Id,
                            ApplicantInfoId = selApplicantApprovalDtl.ApplicantInfoId,
                            RollNo = applicant.RollNo,
                            FullMark = selCriDtl.FullMark,
                            PassMark = selCriDtl.PassMark,
                            ObtainedMarks = tstRslt.ObtainedMarks,
                            ApplicantName = applicant.Name,
                            DateOfBirth = applicant.DateOfBirth.ToString("dd-MM-yyyy"),
                            DesignationName = _prmCommonService.PRMUnit.DesignationRepository.Get(x=>x.Id == applicant.DesignationId).Select(s=>s.Name).FirstOrDefault(),
                            DesignationId = _prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == applicant.DesignationId).Select(s => s.Id).FirstOrDefault(),
                            //Quota = applicant.QuotaNameId == null ? string.Empty : applicant.PRM_QuotaName.Name,
                            FinallySelected = selApplicantDtl.IsFinalSelected.ToString(),
                            FinalSelectedId = selApplicantDtl.SelectedId.ToString(),
                            IsCheckedFinal = true
                        }).DistinctBy(x => x.RollNo).ToList();

            model.ApplicantInfoApprovalListDetail = list;
            if (type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View(model);
        }


        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] SelectedApplicantInfoApprovalViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }
                    // Set preious attachment if exist
                    var obj = _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.GetByID(model.Id);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    //

                    model = GetInsertUserAuditInfo(model, false);
                    var entity = CreateEntity(model, false);

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

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.Update(entity);
                        _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        //   return RedirectToAction("Index");
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
                    // model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            populateDropdown(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            var obj = _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.GetByID(id);
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.Delete(id);
                _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.SaveChanges();
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

        #region Populate Dropdown
        private void populateDropdown(SelectedApplicantInfoApprovalViewModel model)
        {
            dynamic ddlList;

            #region job advertisement

            ddlList = _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll().OrderBy(x => x.AdCode).ToList();
            model.AdvertisementCodeList = Common.PopulateJobAdvertisementDDL(ddlList);

            #endregion


        }
        #endregion

        private PRM_SelectedApplicantInfoApproval CreateEntity(SelectedApplicantInfoApprovalViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            #region ApplicantShortList detail
            foreach (var c in model.ApplicantInfoApprovalListDetail)
            {
                var prm_SelectedApplicantInfoApprovalDetail = new PRM_SelectedApplicantInfoApprovalDetail();

                if (c.IsCheckedFinal)
                {
                    prm_SelectedApplicantInfoApprovalDetail.Id = c.Id;
                    prm_SelectedApplicantInfoApprovalDetail.ApplicantInfoId = (int)c.ApplicantInfoId;
                    prm_SelectedApplicantInfoApprovalDetail.IUser = User.Identity.Name;
                    prm_SelectedApplicantInfoApprovalDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_SelectedApplicantInfoApprovalDetail.IUser = User.Identity.Name;
                        prm_SelectedApplicantInfoApprovalDetail.IDate = DateTime.Now;
                        entity.PRM_SelectedApplicantInfoApprovalDetail.Add(prm_SelectedApplicantInfoApprovalDetail);
                    }
                    else
                    {
                        prm_SelectedApplicantInfoApprovalDetail.SelectedApplicantInfoApprovalId = model.Id;
                        prm_SelectedApplicantInfoApprovalDetail.EUser = User.Identity.Name;
                        prm_SelectedApplicantInfoApprovalDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            var requInfo = _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.GetAll().Where(x => x.ApplicantInfoId == c.ApplicantInfoId).ToList();
                            if (requInfo.Count == 0)
                            {
                                _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.Add(prm_SelectedApplicantInfoApprovalDetail);
                            }
                        }
                        else
                        {
                            _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.Update(prm_SelectedApplicantInfoApprovalDetail);

                        }
                        _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.SaveChanges();
                    }

                }
            }
            #endregion

            return entity;
        }

        private SelectedApplicantInfoApprovalViewModel GetInsertUserAuditInfo(SelectedApplicantInfoApprovalViewModel model, bool pAddEdit)
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

        private bool CheckDuplicateEntry(SelectedApplicantInfoApprovalViewModel model, int strMode)
        {
            if (strMode < 1)
            {

                return _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.Get(q => q.JobAdvertisementInfoId == model.JobAdvertisementInfoId).Any();
            }

            else
            {

                return _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalRepository.Get(q => q.JobAdvertisementInfoId == model.JobAdvertisementInfoId && strMode != q.Id).Any();
            }
        }
        #region Attachment

        private int Upload(SelectedApplicantInfoApprovalViewModel model)
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

        public void DownloadDoc(SelectedApplicantInfoApprovalViewModel model)
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

        //get jobpost by job advertisement id
        [HttpGet]
        public PartialViewResult GetJobPost(int jobAdId)
        {
            List<SelectedApplicantInfoApprovalViewModel> list = new List<SelectedApplicantInfoApprovalViewModel>();

            list = (from jobAdDtlReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll()
                    join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on jobAdDtlReq.JobAdvertisementInfoId equals jobAd.Id
                    where (jobAd.Id == jobAdId)
                    select new SelectedApplicantInfoApprovalViewModel
                    {
                        DesignationId = jobAdDtlReq.DesignationId,
                        DesignationName = jobAdDtlReq.PRM_Designation.Name,
                        DepartmentId = jobAdDtlReq.DepartmentId,
                        DepartmentName = jobAdDtlReq.PRM_Division == null ? string.Empty : jobAdDtlReq.PRM_Division.Name,
                        SectionId = jobAdDtlReq.PRM_Section == null ? null : jobAdDtlReq.SectionId,
                        SectionName = jobAdDtlReq.PRM_Section == null ? string.Empty : jobAdDtlReq.PRM_Section.Name,
                        NoOfPost = jobAdDtlReq.NumberOfClearancePosition
                    }).Concat(from jobAdDtlReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.GetAll()
                              join jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll() on jobAdDtlReq.JobAdvertisementInfoId equals jobAd.Id
                              where (jobAd.Id == jobAdId)
                              select new SelectedApplicantInfoApprovalViewModel
                              {
                                  DesignationId = jobAdDtlReq.DesignationId,
                                  DesignationName = jobAdDtlReq.PRM_Designation.Name,
                                  DepartmentId = jobAdDtlReq.DepartmentId,
                                  DepartmentName =jobAdDtlReq.PRM_Division== null ? string.Empty: jobAdDtlReq.PRM_Division.Name,
                                  SectionId = jobAdDtlReq.PRM_Section == null ? null : jobAdDtlReq.SectionId,
                                  SectionName = jobAdDtlReq.PRM_Section == null ? string.Empty : jobAdDtlReq.PRM_Section.Name,
                                  NoOfPost = jobAdDtlReq.NumberOfPosition
                              }).ToList();

            return PartialView("_JobPost", new SelectedApplicantInfoApprovalViewModel { JobPostInformationList = list });

        }

        //get applicant info by jobpost(designation) id
        [HttpPost]
        public PartialViewResult AddedApplicantInfo(List<SelectedApplicantInfoApprovalDetailViewModel> jobPosts, int jobAdId, string strMode)
        {
            var model = new SelectedApplicantInfoApprovalViewModel();
            var objOther = _prmCommonService.PRMUnit.ApplicantInterviewCardIssueRepository.GetAll().Where(x => jobPosts.Select(n => n.DesignationId).Contains(x.DesignationId)).Where(p => p.PRM_JobAdvertisementInfo.Id == jobAdId).FirstOrDefault();

            var obj = (from applicantSrtLst in _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.GetAll()
                       join applicantInfo in _prmCommonService.PRMUnit.ApplicantInfoRepository.GetAll() on applicantSrtLst.ApplicantInfoId equals applicantInfo.Id
                       select applicantInfo).Where(x => jobPosts.Select(n => n.DesignationId).Contains(x.DesignationId)).ToList();

            List<SelectedApplicantInfoApprovalViewModel> AssignmentList = new List<SelectedApplicantInfoApprovalViewModel>();
            if (jobPosts != null)
            {
                var list = (from selApplicantInfoDtl in _prmCommonService.PRMUnit.SelectedApplicantInfoDetailRepository.GetAll()
                            join selApplicantInfo in _prmCommonService.PRMUnit.SelectedApplicantInfoRepository.GetAll() on selApplicantInfoDtl.SelectedApplicantInfoId equals selApplicantInfo.Id
                            join tstRslt in _prmCommonService.PRMUnit.TestResultforApplicantInfoDetailRepository.GetAll() on selApplicantInfoDtl.ApplicantInfoId equals tstRslt.ApplicantInfoId
                            join appInfo in _prmCommonService.PRMUnit.ERECgeneralinfoRepository.GetAll() on tstRslt.ApplicantInfoId equals appInfo.intPK
                            join selCri in _prmCommonService.PRMUnit.SelectionCriteriaRepository.GetAll() on selApplicantInfo.JobAdvertisementInfoId equals selCri.JobAdvertisementInfoId
                            join selCriDtl in _prmCommonService.PRMUnit.SelectionCriteriaDetailRepository.GetAll() on selCri.Id equals selCriDtl.SelectionCriteriaId
                            where ((jobPosts.Select(n => n.DesignationId).Contains(Convert.ToInt32(appInfo.DesignationId))) && (selApplicantInfo.JobAdvertisementInfoId == jobAdId && selApplicantInfoDtl.IsFinalSelected == true))
                            && (selCriDtl.SelectionCriteriaOrExamTypeId == objOther.SelectionCriteriaOrExamTypeId)
                            select new SelectedApplicantInfoApprovalViewModel()
                            {
                                ApplicantInfoId = appInfo.intPK,
                                RollNo = appInfo.RollNo,
                                FullMark = selCriDtl.FullMark,
                                PassMark = selCriDtl.PassMark,
                                ObtainedMarks = tstRslt.ObtainedMarks,
                                ApplicantName = appInfo.Name,
                                DateOfBirth = appInfo.DateOfBirth.ToString("dd-MM-yyyy"),
                                DesignationName = _prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == appInfo.DesignationId).Select(s => s.Name).FirstOrDefault(),
                                DesignationId = _prmCommonService.PRMUnit.DesignationRepository.Get(x => x.Id == appInfo.DesignationId).Select(s => s.Id).FirstOrDefault(),
                                JobAdvertisementInfoId = selApplicantInfo.JobAdvertisementInfoId,
                               // Quota = appInfo.QuotaNameId == null ? string.Empty : appInfo.PRM_QuotaName.Name,
                                FinallySelected = selApplicantInfoDtl.IsFinalSelected.ToString(),
                                FinalSelectedId = selApplicantInfoDtl.SelectedId.ToString(),
                            }).DistinctBy(q => q.ApplicantInfoId).ToList();




                foreach (var vmApplicant in list)
                {
                    var dupList = _prmCommonService.PRMUnit.SelectedApplicantInfoApprovalDetailRepository.GetAll().Where(x => x.ApplicantInfoId == vmApplicant.Id).ToList();   // for checking duplicate

                    if (strMode == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new SelectedApplicantInfoApprovalViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo = vmApplicant.RollNo,
                                FullMark = vmApplicant.FullMark,
                                PassMark = vmApplicant.PassMark,
                                ObtainedMarks = vmApplicant.ObtainedMarks,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationId = vmApplicant.DesignationId,
                                DesignationName = vmApplicant.DesignationName,
                                JobAdvertisementInfoId = vmApplicant.JobAdvertisementInfoId,
                                Quota = vmApplicant.Quota,
                                FinallySelected = vmApplicant.FinallySelected,
                                FinalSelectedId = vmApplicant.FinalSelectedId

                            };

                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new SelectedApplicantInfoApprovalViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo = vmApplicant.RollNo,
                                FullMark = vmApplicant.FullMark,
                                PassMark = vmApplicant.PassMark,
                                ObtainedMarks = vmApplicant.ObtainedMarks,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationId = vmApplicant.DesignationId,
                                DesignationName = vmApplicant.DesignationName,
                                JobAdvertisementInfoId = vmApplicant.JobAdvertisementInfoId,
                                Quota = vmApplicant.Quota,
                                FinallySelected = vmApplicant.FinallySelected,
                                FinalSelectedId = vmApplicant.FinalSelectedId,
                                IsCheckedFinal = true,

                            };
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new SelectedApplicantInfoApprovalViewModel
                            {
                                ApplicantInfoId = vmApplicant.ApplicantInfoId,
                                RollNo = vmApplicant.RollNo,
                                FullMark = vmApplicant.FullMark,
                                PassMark = vmApplicant.PassMark,
                                ObtainedMarks = vmApplicant.ObtainedMarks,
                                ApplicantName = vmApplicant.ApplicantName,
                                DateOfBirth = vmApplicant.DateOfBirth,
                                DesignationId = vmApplicant.DesignationId,
                                DesignationName = vmApplicant.DesignationName,
                                Quota = vmApplicant.Quota,
                                FinallySelected = vmApplicant.FinallySelected,
                                FinalSelectedId = vmApplicant.FinalSelectedId,
                                IsCheckedFinal = false
                            };
                            AssignmentList.Add(gridModel);

                        }

                    }
                }

                model.ApplicantInfoApprovalListDetail = AssignmentList;
            }
            return PartialView("_Detail", model);
        }


        #region Search
        [NoCache]
        public ActionResult GetJobAdvertisement()
        {
            var jobAd = _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();

            return PartialView("Select", Common.PopulateJobAdvertisementDDL(jobAd));
        }

        [NoCache]
        public ActionResult DesignationforView()
        {
            var designations = (from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                select des).Concat(from jobAd in _prmCommonService.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                                   join jobAdReq in _prmCommonService.PRMUnit.JobAdvertisementPostDetailRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                                   join des in _prmCommonService.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                                   select des).DistinctBy(x=>x.Id).OrderBy(o => o.SortingOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult DepartmentNameforView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult SectionNameforView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.SectionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        #endregion
    }
}