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
using System.Web.Script.Serialization;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ApplicantInfoController : BaseController
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public ApplicantInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonservice = prmCommonService;
        }
        #endregion

        #region Actions

        // GET: /PRM/ApplicantInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApplicantInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;


            List<ApplicantInfoViewModel> list = (from appliCant in _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetAll()
                                                 join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on appliCant.JobAdvertisementInfoId equals jobAd.Id
                                                 where (viewModel.JobAdvertisementInfoId == 0 || viewModel.JobAdvertisementInfoId == jobAd.Id)
                                                   && (viewModel.DesignationId == 0 || viewModel.DesignationId == appliCant.DesignationId)
                                                   && (viewModel.DepartmentId == null || viewModel.DepartmentId == 0 || viewModel.DepartmentId == appliCant.DivisionId)
                                                   && (viewModel.ApplicantNameE == null || viewModel.ApplicantNameE == "" || appliCant.ApplicantNameE.Contains(viewModel.ApplicantNameE))
                                                   && (viewModel.FatherName == null || viewModel.FatherName == "" || appliCant.FatherName.Contains(viewModel.FatherName))
                                                   && (viewModel.MobNo == null || viewModel.MobNo == "" || viewModel.MobNo == appliCant.MobNo)
                                                   && (appliCant.ZoneInfoId == LoggedUserZoneInfoId)
                                                 select new ApplicantInfoViewModel()
                                                      {
                                                          Id = appliCant.Id,
                                                          JobAdvertisementInfoId = jobAd.Id,
                                                          AdCode = jobAd.AdCode,
                                                          ApplicantNameE = appliCant.ApplicantNameE,
                                                          FatherName = appliCant.FatherName,
                                                          DesignationId = appliCant.Id,
                                                          DesignationName = appliCant.PRM_Designation.Name,
                                                          DepartmentId = appliCant.DivisionId,
                                                          DepartmentName = appliCant.PRM_Division.Name,
                                                          MobNo = appliCant.MobNo,
                                                          Status = appliCant.IsSubmit.ToString() == "True" ? "Submitted" : "Pending",
                                                      }).DistinctBy(q => q.Id).ToList();


            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "AdCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AdCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AdCode).ToList();
                }
            }


            if (request.SortingName == "ApplicantNameE")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApplicantNameE).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApplicantNameE).ToList();
                }
            }

            if (request.SortingName == "FatherName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FatherName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FatherName).ToList();
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
            if (request.SortingName == "MobNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MobNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MobNo).ToList();
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
                    d.AdCode,
                    d.ApplicantNameE,
                    d.FatherName,
                    d.DesignationId,
                    d.DesignationName,
                    d.DepartmentId,
                    d.DepartmentName,
                    d.MobNo,
                    d.Status,
                    "View",
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };




        }


        public ActionResult Create()
        {
            ApplicantInfoViewModel model = new ApplicantInfoViewModel();
            // model.DateOfBirth = DateTime.Now;
            model.IsAddAttachment = false;
            model.IsAddSingnatureAttachment = true;
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] ApplicantInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                var attachmentSignature = Request.Files["SingnatureAttachment"];

                if (ModelState.IsValid)
                {
                    var valiMsg = GetBusinessLogicValidation(model);
                    if (!string.IsNullOrEmpty(valiMsg))
                    {
                        model.errClass = "failed";
                        model.ErrMsg = valiMsg;
                        populateDropdown(model);
                        return View(model);
                    }

                    model.IsAddAttachment = true;
                    model = GetInsertUserAuditInfo(model);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);

                    HttpFileCollectionBase files = Request.Files;
                    foreach (string fileTagName in files)
                    {
                        if (fileTagName == "Attachment")
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
                                entity.IsAddAttachment = true;
                            }
                        }
                        else if (fileTagName == "SingnatureAttachment")
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
                                entity.SingnatureFileName = name;
                                entity.SingnatureAttachment = fileData;
                                entity.IsAddSingnatureAttachment = true;
                            }
                        }

                    }


                    if (errorList.Length == 0)
                    {
                        _prmCommonservice.PRMUnit.ApplicantInfoRepository.Add(entity);
                        _prmCommonservice.PRMUnit.ApplicantInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        // return RedirectToAction("Index");
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
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

        [NoCache]
        public ActionResult Edit(int id, string type)
        {
            var entity = _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            model.CandidateType = entity.CandidateType;
            //Applicant Qualification

            //if (entity.PRM_ApplicantInfoQualification != null)
            //{
            //    model.ApplicantInfoQualification = new List<ApplicantInfoViewModel>();
            //    foreach (var item in entity.PRM_ApplicantInfoQualification)
            //    {
            //        model.ApplicantInfoQualification.Add(item.ToModel());
            //    }
            //}

            List<ApplicantInfoQualificationViewModel> resultFrm = (from applicant in _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetAll()
                                                                   join appQua in _prmCommonservice.PRMUnit.ApplicantInfoQualificationRepository.GetAll() on applicant.Id equals appQua.ApplicantInfoId
                                                                   where (applicant.Id == id)
                                                      select new ApplicantInfoQualificationViewModel()
                                                                    {
                                                                        Id = appQua.Id,
                                                                        ApplicantInfoId = id,
                                                                        DegreeLevelId = appQua.DegreeLevelId,
                                                                        UniversityAndBoardId = appQua.UniversityAndBoardId,
                                                                        SubjectGroupId = appQua.SubjectGroupId,
                                                                        AcademicGradeId = appQua.AcademicGradeId,                                                                      
                                                                        AcademicInstName = appQua.AcademicInstName,
                                                                        DegreeLevelName = appQua.PRM_DegreeLevel.Name,
                                                                        UniversityAndBoardName = appQua.PRM_UniversityAndBoard.Name,
                                                                        SubjectGroupName = appQua.PRM_SubjectGroup.Name,
                                                                        AcademicGradeName = appQua.PRM_AcademicGrade.Name,
                                                                        PassingYear = appQua.PassingYear,
                                                                        CGPA = appQua.CGPA
                                                                    }).ToList();
            model.ApplicantInfoQualification = resultFrm;
            model.AdJobPostAgeCalDate = entity.PRM_JobAdvertisementInfo.AgeCalDate.ToString("yyyy-MM-dd");
            model.StartDateOfApplication = entity.PRM_JobAdvertisementInfo.AppStartDate.ToString("yyyy-MM-dd");
            model.LastDateOfApplication = entity.PRM_JobAdvertisementInfo.AppEndDate.ToString("yyyy-MM-dd");
            populateDropdown(model);

            DownloadDoc(model);
            DownloadDocSignature(model);
            model.IsAddAttachment = true;
            model.IsAddSingnatureAttachment = true;
            if (entity.CandidateType == "Yes")
            {
                model.CandidateType = "Yes";
            }
            else if (entity.CandidateType == "No")
            {
                model.CandidateType = "No";
            }
            else
            {
                model.CandidateType = "NA";
            }
            if (type == "success")
            {
                model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
                model.errClass = "success";
            }
            return View("Edit", model);
        }


        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] ApplicantInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                var attachmentSignature = Request.Files["SingnatureAttachment"];
                if (ModelState.IsValid)
                {
                    var valiMsg = GetBusinessLogicValidation(model);
                    if (!string.IsNullOrEmpty(valiMsg))
                    {
                        model.errClass = "failed";
                        model.ErrMsg = valiMsg;
                        populateDropdown(model);
                        return View(model);
                    }

                    // Set preious attachment 
                    var obj = _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetByID(model.Id);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    model.SingnatureAttachment = obj.SingnatureAttachment == null ? null : obj.SingnatureAttachment;
                    //

                    model = GetInsertUserAuditInfo(model);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);

                    HttpFileCollectionBase files = Request.Files;
                    foreach (string fileTagName in files)
                    {
                        if (fileTagName == "Attachment")
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
                        else if (fileTagName == "SingnatureAttachment")
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
                                entity.SingnatureFileName = name;
                                entity.SingnatureAttachment = fileData;
                            }
                        }

                    }

                    if (errorList.Length == 0)
                    {
                        _prmCommonservice.PRMUnit.ApplicantInfoRepository.Update(entity);
                        _prmCommonservice.PRMUnit.ApplicantInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        // return RedirectToAction("Index");
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
            populateDropdown(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            var obj = _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetByID(id);
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                List<Type> allTypes = new List<Type> { typeof(PRM_ApplicantInfoQualification) };
                _prmCommonservice.PRMUnit.ApplicantInfoRepository.Delete(id, allTypes);
                _prmCommonservice.PRMUnit.ApplicantInfoRepository.SaveChanges();
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


        [HttpPost, ActionName("ApplicantInfoQualificationDelete")]
        public JsonResult DeleteApplicantInfoQualificationConfirmed(int id)
        {

            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _prmCommonservice.PRMUnit.ApplicantInfoQualificationRepository.Delete(id);
                _prmCommonservice.PRMUnit.ApplicantInfoQualificationRepository.SaveChanges();
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

        [HttpPost]
        public ActionResult ViewApplicantInfo(int id)
        {
            var obj = _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetByID(id);

            var model = obj.ToModel();
            // model.DateOfBirth = obj.DateOfBirth.ToString("dd-MM-yyyy");
            //Applicant Qualification

            List<ApplicantInfoQualificationViewModel> resultFrm = (from applicant in _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetAll()
                                                      join appQua in _prmCommonservice.PRMUnit.ApplicantInfoQualificationRepository.GetAll() on applicant.Id equals appQua.ApplicantInfoId
                                                      where (applicant.Id == id)
                                                            select new ApplicantInfoQualificationViewModel()
                                                                   {
                                                                       Id = appQua.Id,
                                                                       ApplicantInfoId = id,
                                                                       DegreeLevelId = appQua.DegreeLevelId,
                                                                       UniversityAndBoardId = appQua.UniversityAndBoardId,
                                                                       SubjectGroupId = appQua.SubjectGroupId,
                                                                       AcademicGradeId = appQua.AcademicGradeId,
                                                                       PassingYear = appQua.PassingYear,
                                                                       AcademicInstName = appQua.AcademicInstName,
                                                                       DegreeLevelName = appQua.PRM_DegreeLevel.Name,
                                                                       UniversityAndBoardName = appQua.PRM_UniversityAndBoard.Name,
                                                                       SubjectGroupName = appQua.PRM_SubjectGroup.Name,
                                                                       AcademicGradeName = appQua.PRM_AcademicGrade.Name,
                                                                       CGPA=appQua.CGPA
                                                                   }).ToList();
            model.ApplicantInfoQualification = resultFrm;

            return PartialView("_ApplicantInfoView", model);
        }


        #endregion

        #region Search DDL


        public ActionResult LoadThana(int districtId)
        {
            var list = _prmCommonservice.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == districtId).Select(x => new { Id = x.Id, Name = x.ThanaName }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult LoadBankBranch(int bankId)
        {
            var list = _prmCommonservice.PRMUnit.BankBranchRepository.Fetch().Where(t => t.BankId == bankId).Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }

        #endregion

        #region Private Method
        private void populateDropdown(ApplicantInfoViewModel model)
        {
            dynamic ddlList;

            #region job advertisement

            ddlList = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll().OrderBy(x => x.AdCode).ToList();
            model.JobAdvertisementInfoList = Common.PopulateJobAdvertisementDDL(ddlList);

            #endregion

            #region Designation

            var desigList = _prmCommonservice.PRMUnit.DesignationRepository.Get(m => m.Id == model.DesignationId).OrderBy(x => x.Name).ToList();
            model.DesignationList = Common.PopulateDllList(desigList);

            #endregion

            #region Nationality ddl

            ddlList = _prmCommonservice.PRMUnit.NationalityRepository.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.NationalityList = Common.PopulateDllList(ddlList);

            #endregion

            #region Gender ddl
            model.GenderList = Common.PopulateGenderDDL(model.GenderList);
            #endregion

            #region Nationality ddl

            ddlList = _prmCommonservice.PRMUnit.Religion.Fetch().OrderBy(x => x.SortOrder).ToList();
            model.ReligionList = Common.PopulateDllList(ddlList);

            #endregion

            #region Present District ddl

            ddlList = _prmCommonservice.PRMUnit.DistrictRepository.Fetch().OrderBy(x => x.DistrictName);
            model.PresentDistictList = Common.PopulateDistrictDDL(ddlList);

            #endregion

            #region Present Thana ddl

            ddlList = _prmCommonservice.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == model.PresentDistictId).OrderBy(x => x.ThanaName).ToList();
            model.PresentThanaList = Common.PopulateThanDDL(ddlList);

            #endregion


            #region Permanent District ddl

            ddlList = _prmCommonservice.PRMUnit.DistrictRepository.Fetch().OrderBy(x => x.DistrictName);
            model.PermanentDistictList = Common.PopulateDistrictDDL(ddlList);

            #endregion

            #region Permanent Thana ddl

            ddlList = _prmCommonservice.PRMUnit.ThanaRepository.Fetch().Where(t => t.DistrictId == model.PermanentDistictId).OrderBy(x => x.ThanaName).ToList();
            model.PermanentThanaList = Common.PopulateThanDDL(ddlList);

            #endregion
            
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

            #region Year           
            model.PassingYearList = Common.PopulateYearList();

            #endregion

            #region Quota ddl

            ddlList = _prmCommonservice.PRMUnit.QuotaNameRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.QuotaNameList = Common.PopulateDllList(ddlList);

            #endregion

            #region Bank ddl

            ddlList = _prmCommonservice.PRMUnit.BankNameRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.BankNameList = Common.PopulateDllList(ddlList);

            #endregion

            #region BankBranch ddl

            ddlList = _prmCommonservice.PRMUnit.BankBranchRepository.Fetch().Where(t => t.BankId == model.BankNameId).OrderBy(x => x.Name).ToList();
            model.BankBranchList = Common.PopulateDllList(ddlList);

            #endregion
        }

        private PRM_ApplicantInfo CreateEntity(ApplicantInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.ApplicantInfoQualification)
            {
                var prm_ApplicantInfoQualification = new PRM_ApplicantInfoQualification();

                prm_ApplicantInfoQualification.Id = c.Id;
                prm_ApplicantInfoQualification.IUser = String.IsNullOrEmpty(model.IUser) ? User.Identity.Name : model.IUser;
                prm_ApplicantInfoQualification.IDate = model.IDate == null ? DateTime.Now : Convert.ToDateTime(model.IDate);
                prm_ApplicantInfoQualification.EUser = model.EUser;
                prm_ApplicantInfoQualification.EDate = model.EDate;

                prm_ApplicantInfoQualification.DegreeLevelId = c.DegreeLevelId;
                prm_ApplicantInfoQualification.UniversityAndBoardId = c.UniversityAndBoardId;

                prm_ApplicantInfoQualification.SubjectGroupId = c.SubjectGroupId;
                prm_ApplicantInfoQualification.AcademicGradeId = c.AcademicGradeId;

                prm_ApplicantInfoQualification.PassingYear = c.PassingYear;
                prm_ApplicantInfoQualification.AcademicInstName = c.AcademicInstName;

                if (pAddEdit)
                {
                    prm_ApplicantInfoQualification.IUser = User.Identity.Name;
                    prm_ApplicantInfoQualification.IDate = DateTime.Now;

                    //  entity.PRM_ApplicantInfoQualification.Add(prm_ApplicantInfoQualification);
                }
                else
                {
                    prm_ApplicantInfoQualification.ApplicantInfoId = model.Id;
                    prm_ApplicantInfoQualification.EUser = User.Identity.Name;
                    prm_ApplicantInfoQualification.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonservice.PRMUnit.ApplicantInfoQualificationRepository.Add(prm_ApplicantInfoQualification);
                    }
                    else
                    {
                        _prmCommonservice.PRMUnit.ApplicantInfoQualificationRepository.Update(prm_ApplicantInfoQualification);

                    }
                    _prmCommonservice.PRMUnit.ApplicantInfoQualificationRepository.SaveChanges();
                }

            }

            return entity;
        }

        private ApplicantInfoViewModel GetInsertUserAuditInfo(ApplicantInfoViewModel model)
        {
            model.IUser = User.Identity.Name;
            model.IDate = DateTime.Now;
            //foreach (var child in model.ApplicantInfoQualification)
            //{
            //    child.IUser = User.Identity.Name;
            //    child.IDate = DateTime.Now;

            //}
            return model;
        }

        private string GetBusinessLogicValidation(ApplicantInfoViewModel model)
        {
            string errorMessage = string.Empty;
            bool chkDuplicate = false;

            if (model.NationalID != null && model.NationalID != "")
            {
                if (model.Id == 0)
                {
                    chkDuplicate = _prmCommonservice.PRMUnit.ApplicantInfoRepository.Get(q => q.NationalID == model.NationalID).Any();
                }
                else
                {
                    chkDuplicate = _prmCommonservice.PRMUnit.ApplicantInfoRepository.Get(q => q.NationalID == model.NationalID && model.Id != q.Id).Any();
                }

                if (chkDuplicate)
                {
                    return errorMessage = "This National ID number already exist.";
                }

            }

            if (model.BirthRegNo != null && model.BirthRegNo != "")
            {
                if (model.Id == 0)
                {
                    chkDuplicate = _prmCommonservice.PRMUnit.ApplicantInfoRepository.Get(q => q.BirthRegNo == model.BirthRegNo).Any();
                }
                else
                {
                    chkDuplicate = _prmCommonservice.PRMUnit.ApplicantInfoRepository.Get(q => q.BirthRegNo == model.BirthRegNo && model.Id != q.Id).Any();
                }

                if (chkDuplicate)
                {
                    return errorMessage = "This birth registration number already exist.";
                }
            }

            return string.Empty; ;
        }
        #endregion

        [NoCache]
        public JsonResult GetJobAdvertisementInfo(int jobAdId)
        {
            var obj = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetByID(jobAdId);
            return Json(new
            {
                AgeCalDate = obj.AgeCalDate.ToString("yyyy-MM-dd"),
                StartDateOfApplication = obj.AppStartDate.ToString("yyyy-MM-dd"),
                LastDateOfApplicaton = obj.AppEndDate.ToString("yyyy-MM-dd")
            }, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetDivisionByDesignaionId(int designationId)
        {
            var obj = _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Get(q => q.DesignationId == designationId).FirstOrDefault();
            return Json(new
            {
                DivisionId = obj.DepartmentId
            }, JsonRequestBehavior.AllowGet);
        }

        #region Attachment Applicant Photo

        private int Upload(ApplicantInfoViewModel model)
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

        public void DownloadDoc(ApplicantInfoViewModel model)
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


        #region Attachment Applicant Signature

        private int UploadSignature(ApplicantInfoViewModel model)
        {
            if (model.SingnatureFile == null)
                return 0;

            try
            {
                var uploadFile = model.SingnatureFile;

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
                model.SingnatureAttachment = data;
                model.SingnatureFileName = uploadFile.FileName;
                model.IsError = 0;

            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = "Upload File Error!";
            }

            return model.IsError;
        }

        public void DownloadDocSignature(ApplicantInfoViewModel model)
        {
            byte[] document = model.SingnatureAttachment;
            if (document != null)
            {
                string strFilename = Url.Content("~/Content/" + User.Identity.Name + model.SingnatureFileName);
                byte[] doc = document;
                WriteToFileSignature(Server.MapPath(strFilename), ref doc);

                model.SingnatureFilePath = strFilename;
            }
        }

        private void WriteToFileSignature(string strPath, ref byte[] Buffer)
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

        public string GetDesignationByAdJobId(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                         join jobAdReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                         join des in _prmCommonservice.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                         where jobAdReq.JobAdvertisementInfoId == id
                         select des).OrderBy(o => o.Name).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.Name, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }
            return new JavaScriptSerializer().Serialize(listItems);
        }

        //Search view

        [NoCache]
        public ActionResult GetJobAdvertisement()
        {
            var jobAd = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll().OrderBy(x => x.AdCode).ToList();
            return PartialView("Select", Common.PopulateJobAdvertisementDDL(jobAd));
        }

        [NoCache]
        public ActionResult GetDesignation()
        {
            // var designations = _prmCommonservice.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();

            var designations = (from jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.Fetch()
                                join jobAdReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.Fetch() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                join des in _prmCommonservice.PRMUnit.DesignationRepository.Fetch() on jobAdReq.DesignationId equals des.Id
                                select des).OrderBy(o => o.Name).ToList();

            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _prmCommonservice.PRMUnit.DivisionRepository.GetAll().OrderBy(o => o.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        [HttpPost]
        public ActionResult AddQualifacation(ApplicantInfoQualificationViewModel model)
        {
            ApplicantInfoViewModel masterModel = new ApplicantInfoViewModel();

            List<ApplicantInfoQualificationViewModel> list = new List<ApplicantInfoQualificationViewModel>();

            var obj = new ApplicantInfoQualificationViewModel
            {
                ApplicantInfoId = model.ApplicantInfoId,
                DegreeLevelId = model.DegreeLevelId,
                UniversityAndBoardId = model.UniversityAndBoardId,
                SubjectGroupId = model.SubjectGroupId,
                AcademicGradeId = model.AcademicGradeId,
                PassingYear = model.PassingYear,
                AcademicInstName = model.AcademicInstName,
                CGPA = model.CGPA,
                DegreeLevelName = model.DegreeLevelName,
                AcademicGradeName = model.AcademicGradeName,
                UniversityAndBoardName = model.UniversityAndBoardName,
                SubjectGroupName = model.SubjectGroupName,

            };
            list.Add(obj);
            masterModel.ApplicantInfoQualification = list;

            return PartialView("_Details", masterModel);
        }


    }
}