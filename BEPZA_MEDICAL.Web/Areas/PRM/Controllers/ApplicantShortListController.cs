using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.JobAdvertisementInfo;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ApplicantShortListController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public ApplicantShortListController(EmployeeService empService, PRMCommonSevice prmCommonService)
        {
            this._empService = empService;
            this._prmCommonservice = prmCommonService;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApplicantShortListViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ApplicantShortListViewModel> list = (from appSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetAll()
                                                      join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on appSrtLst.JobAdvertisementInfoId equals jobAd.Id
                                                      join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on appSrtLst.EmployeeId equals emp.Id
                                                      join de in _prmCommonservice.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals de.Id
                                                      where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == jobAd.Id)
                                                       && (model.Date == null || model.Date == appSrtLst.Date)
                                                       && (model.DesignationId == null || model.DesignationId == 0 || model.DesignationId == de.Id)
                                                       && (string.IsNullOrEmpty(model.EmployeeName) || emp.FullName.Contains(model.EmployeeName))
                                                       &&(jobAd.ZoneInfoId==LoggedUserZoneInfoId)
                                                      select new ApplicantShortListViewModel()
                                                    {
                                                        Id = appSrtLst.Id,
                                                        JobAdvertisementInfoId = jobAd.Id,
                                                        JobAdvertisementCode = jobAd.AdCode,
                                                        Date = appSrtLst.Date,
                                                        EmployeeName = emp.FullName,
                                                        DesignationId = emp.DesignationId,
                                                        DesignationName = emp.PRM_Designation.Name,
                                                        Status = appSrtLst.IsSubmit.ToString() == "True" ? "Submitted" : "Pending",
                                                    }).OrderBy(x => x.JobAdvertisementCode).ToList();



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

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeName).ToList();
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
                    d.JobAdvertisementCode,         
                    Convert.ToDateTime(d.Date).ToString(DateAndTime.GlobalDateFormat), 
                    d.EmployeeName,
                    d.DesignationId,
                    d.DesignationName,
                    d.Status,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ApplicantShortListViewModel model = new ApplicantShortListViewModel();
            populateDropdown(model);
            model.IsAddAttachment = true;
            //user login information
            var loginUser = _empService.GetEmpLoginInfo(HttpContext.User.Identity.Name);
            if (loginUser != null)
            {
              model.EmployeeId = loginUser.ID;
              model.EmployeeName = loginUser.EmpName;
              model.DesignationName = loginUser.DesignationName;
            }

            model.strMode = "Create";

            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] ApplicantShortListViewModel model)
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
                        _prmCommonservice.PRMUnit.ApplicantShortListRepository.Add(entity);
                        _prmCommonservice.PRMUnit.ApplicantShortListRepository.SaveChanges();
                        model.errClass = "success";
                        model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        //return RedirectToAction("Index");
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
                    //  model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
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
            var entity = _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetByID(Id);
            var model = entity.ToModel();
            model.JobAdvertisementCode = entity.PRM_JobAdvertisementInfo.AdCode;
            model.strMode = "Edit";
            populateDropdown(model);
            DownloadDoc(model);
            model.IsAddAttachment = true;

            //applicant shortlist Detail
            var list = (from appSrtLstDtl in _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.GetAll()
                        join appSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetAll() on appSrtLstDtl.ApplicantShortListId equals appSrtLst.Id
                        join applicant in _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll() on appSrtLstDtl.ApplicantInfoId equals applicant.intPK
                        where (appSrtLstDtl.ApplicantShortListId == Id)
                        select new ApplicantShortListViewModel()
                                {
                                    Id = appSrtLstDtl.Id,
                                    ApplicantShortListId = appSrtLstDtl.ApplicantShortListId,
                                    ApplicantInfoId = appSrtLstDtl.ApplicantInfoId,
                                    ApplicantName = applicant.Name,
                                    FatherName = applicant.FathersName,
                                    MotherName = applicant.MothersName,
                                    DateOfBirth = applicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                    DesignationId = applicant.DesignationId,
                                    DesignationName = _prmCommonservice.PRMUnit.DesignationRepository.GetByID(applicant.DesignationId).Name,
                                    NID = applicant.NationalId,
                                    IsCheckedFinal = true
                                }).ToList();
            model.ApplicantShortList = list;


            //Job post information

            var jobPostList = (from appSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetAll()
                               join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on appSrtLst.JobAdvertisementInfoId equals jobAd.Id
                               join jobAdReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                               where (appSrtLst.Id == Id)
                               select new ApplicantShortListDetailViewModel()
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
                                            }).Concat(from appSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetAll()
                                                      join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on appSrtLst.JobAdvertisementInfoId equals jobAd.Id
                                                      join jobAdReq in _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                                      where (appSrtLst.Id == Id)
                                                      select new ApplicantShortListDetailViewModel()
                                                      {
                                                          DesignationId = jobAdReq.DesignationId,
                                                          DesignationName = jobAdReq.PRM_Designation.Name,
                                                          DepartmentId =jobAdReq.PRM_Division==null? null : jobAdReq.DepartmentId,
                                                          DepartmentName = jobAdReq.PRM_Division == null ? null : jobAdReq.PRM_Division.Name,
                                                          SectionId = jobAdReq.PRM_Section == null ? null : jobAdReq.SectionId,
                                                          SectionName = jobAdReq.PRM_Section == null ? string.Empty : jobAdReq.PRM_Section.Name,
                                                          NoOfPost = jobAdReq.NumberOfPosition,
                                                          //IsChecked = true,
                                                          strMode = "Edit"
                                                      }).ToList();

            model.ApplicantShortListDetail = jobPostList;

            model.EmployeeId = model.EmployeeId;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.DesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

            if (type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }

            return View(model);
        }


        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] ApplicantShortListViewModel model)
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
                    var obj = _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetByID(model.Id);
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
                        _prmCommonservice.PRMUnit.ApplicantShortListRepository.Update(entity);
                        _prmCommonservice.PRMUnit.ApplicantShortListRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        //return RedirectToAction("Index");
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
            model.strMode = "Edit";
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            var obj = _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetByID(id);
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonservice.PRMUnit.ApplicantShortListRepository.Delete(id);
                _prmCommonservice.PRMUnit.ApplicantShortListRepository.SaveChanges();
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
                _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.Delete(id);
                _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.SaveChanges();
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

        [HttpPost]
        public ActionResult ViewApplicantInfo(int id)
        {
            var obj = _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll().Where(x => x.intPK == id).First();
            ApplicantInfoViewModel model = new ApplicantInfoViewModel();
            model.ApplicantNameE = obj.Name;
            model.ApplicantNameB = obj.NameB;
            model.NationalID = obj.NationalId;
            model.BirthRegNo = obj.BirthRegNo;
            model.DateOfBirth = obj.DateOfBirth;
            model.Attachment = obj.Photo;
            model.FatherName = obj.FathersName;
            model.MotherName = obj.MothersName;
            model.MobNo = obj.MobilePhoneNo;
            model.Email = obj.Email;

            var EducationInfo = _prmCommonservice.PRMUnit.ERECeducationalinfoRepository.GetAll().Where(x => x.GeneralID == id).FirstOrDefault();

            if (EducationInfo.SscExam != 0)
            {
                model.SSC = true;
                model.SscResult = EducationInfo.SscResultCGPA == null ? EducationInfo.SscResultDivision : EducationInfo.SscResultCGPA.ToString();
                model.SscGroupId = EducationInfo.SscGroup;
                model.SscBoardId = EducationInfo.SscBoard;
            }
            if (EducationInfo.HscExam != 0)
            {
                model.HSC = true;
                model.HscResult = EducationInfo.HscResultCGPA == null ? EducationInfo.HscResultDivision : EducationInfo.HscResultCGPA.ToString();
                model.HscGroupId = EducationInfo.HscGroup;
                model.HscBoardId = EducationInfo.HscBoard;
            }

            DDL(model);
            return PartialView("_ApplicantInfoView", model);
        }

        public void DDL(ApplicantInfoViewModel model)
        {
            dynamic ddlList;

            #region Exam/Level ddl

            ddlList = _prmCommonservice.PRMUnit.ExamDegreeLavelRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.DegreeLevelList = Common.PopulateDllList(ddlList);

            #endregion

            #region Result ddl

            ddlList = _prmCommonservice.PRMUnit.AcademicGradeRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.AcademicGradeList = Common.PopulateDllList(ddlList);

            #endregion

            #region University OR Board ddl

            ddlList = _prmCommonservice.PRMUnit.UniversityAndBoardRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.UniversityAndBoardList = Common.PopulateDllList(ddlList);

            #endregion

            #region Subject/Group ddl

            ddlList = _prmCommonservice.PRMUnit.SubjectGroupRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.SubjectGroupList = Common.PopulateDllList(ddlList);

            #endregion

        }

        public IList<SelectListItem> PopulateDegreeList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "SSC", Value = "SSC" });
            list.Add(new SelectListItem() { Text = "HSC", Value = "HSC" });
            list.Add(new SelectListItem() { Text = "Honours", Value = "Honours" });
            list.Add(new SelectListItem() { Text = "Masters", Value = "Masters" });

            return list;
        }

        #endregion


        #region Private Method

        #region Populate Dropdown
        private void populateDropdown(ApplicantShortListViewModel model)
        {
            dynamic ddlList;

            #region job advertisement
            DateTime cDate = DateTime.Now;
            ddlList = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();
            model.JobAdvertisementInfoList = Common.PopulateJobAdvertisementDDL(ddlList);

            #endregion

            #region Cgpa/Experience Logic List
            model.CgpaScopeLogicList = PopulateScopeLogic();
            model.ExperienceScopeLogicList = PopulateScopeLogic();
            #endregion

            #region Degree Level
            //var degLList = _prmCommonservice.PRMUnit.ExamDegreeLavelRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.DegreeLevelList = PopulateDegreeList();
            #endregion

        }
        public static IList<SelectListItem> PopulateScopeLogic()
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = ">", Value = ">", });
            list.Add(new SelectListItem() { Text = "<", Value = "<", });
            list.Add(new SelectListItem() { Text = ">=", Value = ">=", });
            list.Add(new SelectListItem() { Text = "<=", Value = "<=", });
            return list;
        }
        #endregion

        private PRM_ApplicantShortList CreateEntity(ApplicantShortListViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            #region ApplicantShortList detail
            foreach (var c in model.ApplicantShortList)
            {
                var prm_ApplicantShortListDetail = new PRM_ApplicantShortListDetail();

                if (c.IsCheckedFinal)
                {
                    prm_ApplicantShortListDetail.Id = c.Id;
                    prm_ApplicantShortListDetail.ApplicantInfoId = (int)c.ApplicantInfoId;
                    prm_ApplicantShortListDetail.IUser = User.Identity.Name;
                    prm_ApplicantShortListDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_ApplicantShortListDetail.IUser = User.Identity.Name;
                        prm_ApplicantShortListDetail.IDate = DateTime.Now;
                        entity.PRM_ApplicantShortListDetail.Add(prm_ApplicantShortListDetail);
                    }
                    else
                    {
                        prm_ApplicantShortListDetail.ApplicantShortListId = model.Id;
                        prm_ApplicantShortListDetail.EUser = User.Identity.Name;
                        prm_ApplicantShortListDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            var requInfo = _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.GetAll().Where(x => x.ApplicantInfoId == c.ApplicantInfoId).ToList();
                            if (requInfo.Count == 0)
                            {
                                _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.Add(prm_ApplicantShortListDetail);
                            }
                        }
                        else
                        {
                            _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.Update(prm_ApplicantShortListDetail);

                        }
                    }
                    _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.SaveChanges();
                }
            }
            #endregion

            return entity;
        }

        private ApplicantShortListViewModel GetInsertUserAuditInfo(ApplicantShortListViewModel model, bool pAddEdit)
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

        private bool CheckDuplicateEntry(ApplicantShortListViewModel model, int strMode)
        {
            //if (strMode < 1)
            //{
            //    return _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetAll().Where(q => q.JobAdvertisementInfoId == model.JobAdvertisementInfoId)
            //           .Join(_prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.GetAll().Where(p=>p.PRM_ApplicantInfo.DesignationId == model.DesignationId),
            //           ASL => ASL.Id, ASLD => ASLD.ApplicantShortListApprovalId, (ASL, ASLD) => new { ASL, ASLD }).Any();
            //}

            //else
            //{
            //    return _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetAll().Where(q => q.JobAdvertisementInfoId == model.JobAdvertisementInfoId && strMode != q.Id)
            //           .Join(_prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.GetAll().Where(p => p.PRM_ApplicantInfo.DesignationId == model.DesignationId),
            //           ASL => ASL.Id, ASLD => ASLD.ApplicantShortListApprovalId, (ASL, ASLD) => new { ASL, ASLD }).Any();
            //}
            return false;
        }


        [NoCache]
        public JsonResult CheckDuplicate(int jobAdvertisementInfoId, int strMode)
        {
            var result = false;
            if (strMode < 1)
            {
                result = _prmCommonservice.PRMUnit.ApplicantShortListRepository.Get(q => q.JobAdvertisementInfoId == jobAdvertisementInfoId).Any();
                // return _prmCommonservice.PRMUnit.ApplicantShortListRepository.Get(q => q.JobAdvertisementInfoId == jobAdvertisementInfoId).Any();
            }

            else
            {
                result = _prmCommonservice.PRMUnit.ApplicantShortListRepository.Get(q => q.JobAdvertisementInfoId == jobAdvertisementInfoId && strMode != q.Id).Any();
                // return _prmCommonservice.PRMUnit.ApplicantShortListRepository.Get(q => q.JobAdvertisementInfoId == jobAdvertisementInfoId && strMode != q.Id).Any();
            }
            return Json(new
            {
                Message = result
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Attachment

        private int Upload(ApplicantShortListViewModel model)
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

        public void DownloadDoc(ApplicantShortListViewModel model)
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


        //Search
        [NoCache]
        public ActionResult GetJobAdvertisement()
        {
            var jobAd = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();

            return PartialView("Select", Common.PopulateJobAdvertisementDDL(jobAd));
        }

        [NoCache]
        public ActionResult DesignationListView()
        {
            var designations = (from jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                join jobAdReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                join des in _prmCommonservice.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                select des).OrderBy(o => o.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(designations));
        }


        //get jobpost by job advertisement id
        [HttpGet]
        public PartialViewResult GetJobPost(int jobAdId)
        {
            List<ApplicantShortListDetailViewModel> list = new List<ApplicantShortListDetailViewModel>();

            list = (from jobAdDtlReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll()
                    join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on jobAdDtlReq.JobAdvertisementInfoId equals jobAd.Id
                    where (jobAd.Id == jobAdId)
                    select new ApplicantShortListDetailViewModel
                    {
                        DesignationId = jobAdDtlReq.DesignationId,
                        DesignationName = jobAdDtlReq.PRM_Designation.Name,
                        DepartmentId = jobAdDtlReq.DepartmentId,
                        DepartmentName = jobAdDtlReq.PRM_Division.Name,
                        SectionId = jobAdDtlReq.PRM_Section == null ? null : jobAdDtlReq.SectionId,
                        SectionName = jobAdDtlReq.PRM_Section == null ? string.Empty : jobAdDtlReq.PRM_Section.Name,
                        NoOfPost = jobAdDtlReq.NumberOfClearancePosition
                    }).Concat(from jobAdPost in _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetAll()
                              join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on jobAdPost.JobAdvertisementInfoId equals jobAd.Id
                              where (jobAd.Id == jobAdId)
                              select new ApplicantShortListDetailViewModel
                              {
                                  DesignationId = jobAdPost.DesignationId,
                                  DesignationName = jobAdPost.PRM_Designation.Name,
                                  DepartmentId = jobAdPost.PRM_Division== null ? Convert.ToInt32(null):jobAdPost.PRM_Division.Id,
                                  DepartmentName = jobAdPost.PRM_Division == null ? string.Empty : jobAdPost.PRM_Division.Name,
                                  SectionId = jobAdPost.PRM_Section == null ? null : jobAdPost.SectionId,
                                  SectionName = jobAdPost.PRM_Section == null ? string.Empty : jobAdPost.PRM_Section.Name,
                                  NoOfPost = jobAdPost.NumberOfPosition
                              }).ToList();

            return PartialView("_JobPost", new ApplicantShortListViewModel { ApplicantShortListDetail = list });

        }

        //get applicant info by jobpost(designation) id
        [HttpPost]
        public PartialViewResult AddedApplicantInfo(int jobPostDesigId, int jobAdId, string strMode, string expLogic, decimal? yearOfExp, string cgpLogic, decimal? cgp, bool reqQualFlag, string degree)
        {
            var model = new ApplicantShortListViewModel();

            List<ApplicantShortListViewModel> AssignmentList = new List<ApplicantShortListViewModel>();
            if (jobPostDesigId != 0)
            {
                #region Recruitment Qualification Info
                var reQlist = _prmCommonservice.PRMUnit.RecruitmentQualificationRepository.GetAll().Where(x => x.DesignationId == jobPostDesigId)
                              .Join(_prmCommonservice.PRMUnit.RecruitmentQualificationDetailRepository.GetAll(), rQ => rQ.Id,
                               rQD => rQD.RecruitmentId, (rQ, rQD) => new { rQ, rQD }).ToList();

                foreach (var item in reQlist)
                {
                    //totalExp = System.Convert.ToDecimal(item.rQD.);
                    //rqCGPA = System.Convert.ToDecimal(item.rQD.CGPA);
                    //degreeLevelId = Convert.ToInt32(item.rQD.DegreeLevelId);
                }

                #endregion

                //var list = _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetAll().Where(x => jobPosts.Select(n => n.DesignationId)
                //           .Contains(x.DesignationId)).Where(q => q.JobAdvertisementInfoId == jobAdId).Join(_prmCommonservice.PRMUnit
                //           .ApplicantInfoQualificationRepository.GetAll(),a => a.Id, q => q.ApplicantInfoId, (a, q) => new { a, q }).ToList();

                var JobadPostDtlId = _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetAll().Where(x => x.JobAdvertisementInfoId == jobAdId && x.DesignationId == jobPostDesigId).FirstOrDefault();
                var list = _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll().Where(x => x.DesignationId == jobPostDesigId && x.CircularID == JobadPostDtlId.Id).ToList();

                #region filter latest Qualification
                //int temp = 0;
                //for(int i = list.Count - 1; i >= 0; i--)
                //{
                //    if (list[i].q.ApplicantInfoId == temp)
                //    {
                //        if (list[i].q.PassingYear > list[i + 1].q.PassingYear)
                //        {
                //            list.RemoveAt(i + 1);
                //        }
                //        else
                //            list.RemoveAt(i);
                //    }
                //    temp = list[i].q.ApplicantInfoId;
                //}
                #endregion

                #region search Condition

                if ((expLogic != "" && yearOfExp != 0) || (cgpLogic != "" && cgp != 0))
                {
                    var list2 = _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll().Where(x => x.DesignationId == jobPostDesigId && x.CircularID == JobadPostDtlId.Id)
                               .Join(_prmCommonservice.PRMUnit.ERECmaritalinfoRepository.GetAll(), a => a.intPK, q => q.GeneralID, (a, q) => new { a, q }).ToList();

                    if (expLogic != "" && yearOfExp != 0)
                    {
                        if (expLogic == "<")
                        {
                            list2 = list2.Where(c => c.q.TotalYearofExp < yearOfExp).ToList();
                        }
                        else if (expLogic == ">")
                        {
                            list2 = list2.Where(c => c.q.TotalYearofExp > yearOfExp).ToList();
                        }
                        else if (expLogic == ">=")
                        {
                            list2 = list2.Where(c => c.q.TotalYearofExp >= yearOfExp).ToList();
                        }
                        else if (expLogic == "<=")
                        {
                            list2 = list2.Where(c => c.q.TotalYearofExp <= yearOfExp).ToList();
                        }
                    }

                    if (cgpLogic != "" && cgp != 0)
                    {
                        var list3 = _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll().Where(x => x.DesignationId == jobPostDesigId && x.CircularID == JobadPostDtlId.Id)
                                    .Join(_prmCommonservice.PRMUnit.ERECeducationalinfoRepository.GetAll(), a => a.intPK, q => q.GeneralID, (a, q) => new { a, q }).ToList();

                        if (degree == "Honours")
                        {
                            if (cgpLogic == "<")
                            {
                                list3 = list3.Where(c =>Convert.ToDecimal(c.q.HonsCgpa) < cgp).ToList();
                            }
                            else if (cgpLogic == ">")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.HonsCgpa) > cgp).ToList();
                            }
                            else if (cgpLogic == ">=")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.HonsCgpa) >= cgp).ToList();
                            }
                            else if (cgpLogic == "<=")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.HonsCgpa) <= cgp).ToList();
                            }

                        }
                        else if (degree == "Masters")
                        {
                            if (cgpLogic == "<")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.MPassCgpa) < cgp).ToList();
                            }
                            else if (cgpLogic == ">")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.MPassCgpa) > cgp).ToList();
                            }
                            else if (cgpLogic == ">=")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.MPassCgpa) >= cgp).ToList();
                            }
                            else if (cgpLogic == "<=")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.MPassCgpa) <= cgp).ToList();
                            }
                        }
                        else if (degree == "HSC")
                        {
                            if (cgpLogic == "<")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.HscResultCGPA) < cgp).ToList();
                            }
                            else if (cgpLogic == ">")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.HscResultCGPA) > cgp).ToList();
                            }
                            else if (cgpLogic == ">=")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.HscResultCGPA) >= cgp).ToList();
                            }
                            else if (cgpLogic == "<=")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.HscResultCGPA) <= cgp).ToList();
                            }
                        }
                        else if (degree == "SSC")
                        {
                            if (cgpLogic == "<")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.SscResultCGPA) < cgp).ToList();
                            }
                            else if (cgpLogic == ">")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.SscResultCGPA) > cgp).ToList();
                            }
                            else if (cgpLogic == ">=")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.SscResultCGPA) >= cgp).ToList();
                            }
                            else if (cgpLogic == "<=")
                            {
                                list3 = list3.Where(c => Convert.ToDecimal(c.q.SscResultCGPA) <= cgp).ToList();
                            }
                        }
                        list = list.Where(x => list3.Any(y => y.a.intPK == x.intPK)).ToList();
                    }
                    list = list.Where(x =>list2.Any(y => y.a.intPK == x.intPK)).ToList();

                }   
               //if (reqQualFlag == true)
               // {
               //     if (totalExp != null && rqCGPA != null && degreeLevelId != null)
               //     {
               //         list = list.Where(l => l.a.YearOfExperience >= totalExp && l.q.DegreeLevelId==degreeLevelId)
               //                .Where(l => System.Convert.ToDecimal(l.q.CGPA) >= rqCGPA).ToList();
               //     }
               // }

                #endregion

                foreach (var vmApplicant in list)
                {
                    var dupList = _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.GetAll().Where(x => x.ApplicantInfoId == vmApplicant.intPK).ToList();   // for checking duplicate
                    var desi = _prmCommonservice.PRMUnit.DesignationRepository.GetByID(jobPostDesigId);
                    if (strMode == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new ApplicantShortListViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                MotherName = vmApplicant.MothersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                DesignationName = desi.Name
                            };
                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new ApplicantShortListViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                MotherName = vmApplicant.MothersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                DesignationName = desi.Name,
                                IsCheckedFinal = true
                            };
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new ApplicantShortListViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                MotherName = vmApplicant.MothersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                DesignationName = desi.Name,
                                IsCheckedFinal = false
                            };
                            AssignmentList.Add(gridModel);
                        }
                    }
                }

                model.ApplicantShortList = AssignmentList;
            }
            return PartialView("_Details", model);
        }
    }
}
