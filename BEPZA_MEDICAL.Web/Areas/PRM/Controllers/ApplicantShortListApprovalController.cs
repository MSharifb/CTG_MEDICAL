using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
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
    public class ApplicantShortListApprovalController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public ApplicantShortListApprovalController(EmployeeService empService, PRMCommonSevice prmCommonService)
        {
            this._empService = empService;
            this._prmCommonservice = prmCommonService;
        }
        #endregion

        #region Actions

        //
        // GET: /PRM/ApplicantShortListApprovalController/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApplicantShortListApprovalViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ApplicantShortListApprovalViewModel> list = (from approvalApplicant in _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.GetAll()
                                                              join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on approvalApplicant.JobAdvertisementInfoId equals jobAd.Id
                                                              join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on approvalApplicant.EmployeeId equals emp.Id
                                                              join de in _prmCommonservice.PRMUnit.DesignationRepository.GetAll() on emp.DesignationId equals de.Id
                                                              where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == jobAd.Id)
                                                              && (model.DesignationId == null || model.DesignationId == 0 || model.DesignationId == de.Id)
                                                              && (model.Date == null || model.Date == approvalApplicant.Date)
                                                              && (string.IsNullOrEmpty(model.EmployeeName) || emp.FullName.Contains(model.EmployeeName))
                                                              &&(jobAd.ZoneInfoId==LoggedUserZoneInfoId)
                                                              select new ApplicantShortListApprovalViewModel()
                                                                {
                                                                    Id = approvalApplicant.Id,
                                                                    JobAdvertisementInfoId = jobAd.Id,
                                                                    JobAdvertisementCode = jobAd.AdCode,
                                                                    Date = approvalApplicant.Date,
                                                                    EmployeeName = emp.FullName,
                                                                    DesignationId = emp.DesignationId,
                                                                    DesignationName = emp.PRM_Designation.Name,
                                                                    Status = approvalApplicant.IsSubmit.ToString() == "True" ? "Approved" : "Rejected",
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
            ApplicantShortListApprovalViewModel model = new ApplicantShortListApprovalViewModel();
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
        public ActionResult Create([Bind(Exclude = "Attachment")] ApplicantShortListApprovalViewModel model)
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
                        _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.Add(entity);
                        _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.SaveChanges();
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
            var entity = _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.GetByID(Id);          
            var model = entity.ToModel();
            model.JobAdvertisementCode = entity.PRM_JobAdvertisementInfo.AdCode;

            //shorted info
            var obj = (from approvalSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.GetAll()
                       join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on approvalSrtLst.JobAdvertisementInfoId equals jobAd.Id
                       join applicantsrtLst in _prmCommonservice.PRMUnit.ApplicantShortListRepository.GetAll() on jobAd.Id equals applicantsrtLst.JobAdvertisementInfoId
                       where (approvalSrtLst.Id == Id)
                       select applicantsrtLst).FirstOrDefault();

            model.ShortListedEmpName = obj.PRM_EmploymentInfo.FullName;
            model.ShortListedDesignation = obj.PRM_EmploymentInfo.PRM_Designation.Name;
            model.ShortListDate = obj.Date.ToString("dd-MM-yyyy");

            model.strMode = "Edit";
            populateDropdown(model);
            DownloadDoc(model);
            model.IsAddAttachment = true;

            //applicant shortlist Detail
            var list = (from approvalAppSrtLstDtl in _prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.GetAll()
                        join approvalappSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.GetAll() on approvalAppSrtLstDtl.ApplicantShortListApprovalId equals approvalappSrtLst.Id
                        join applicant in _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll() on approvalAppSrtLstDtl.ApplicantInfoId equals applicant.intPK
                        where (approvalAppSrtLstDtl.ApplicantShortListApprovalId == Id)
                        select new ApplicantShortListApprovalViewModel()
                                {
                                    Id = approvalAppSrtLstDtl.Id,
                                    ApplicantShortListApprovalId = approvalAppSrtLstDtl.ApplicantShortListApprovalId,
                                    ApplicantInfoId = approvalAppSrtLstDtl.ApplicantInfoId,
                                    ApplicantName = applicant.Name,
                                    FatherName = applicant.FathersName,
                                    MotherName = applicant.MothersName,
                                    DateOfBirth = applicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                    DesignationId = applicant.DesignationId,
                                   // DesignationName = applicant.PRM_Designation.Name,
                                    NID = applicant.NationalId,
                                    IsCheckedFinal = true
                                }).ToList();
            model.ApplicantShortListApproval = list;


            //Job post information          
            var jobPostList = (from approvalappSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.GetAll()
                               join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on approvalappSrtLst.JobAdvertisementInfoId equals jobAd.Id
                               join jobAdReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                               where (approvalappSrtLst.Id == Id)
                               select new ApplicantShortListApprovalDetailViewModel()
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
                                            }).Concat(from approvalappSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.GetAll()
                                                      join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on approvalappSrtLst.JobAdvertisementInfoId equals jobAd.Id
                                                      join jobAdReq in _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId
                                                      where (approvalappSrtLst.Id == Id)
                                                      select new ApplicantShortListApprovalDetailViewModel()
                                                      {
                                                          DesignationId = jobAdReq.DesignationId,
                                                          DesignationName = jobAdReq.PRM_Designation.Name,
                                                          DepartmentId = jobAdReq.PRM_Division == null ? null : jobAdReq.DepartmentId,
                                                          DepartmentName = jobAdReq.PRM_Division == null ? null : jobAdReq.PRM_Division.Name,
                                                          SectionId = jobAdReq.PRM_Section == null ? null : jobAdReq.SectionId,
                                                          SectionName = jobAdReq.PRM_Section == null ? string.Empty : jobAdReq.PRM_Section.Name,
                                                          NoOfPost = jobAdReq.NumberOfPosition,
                                                          //IsChecked = true,
                                                          strMode = "Edit"
                                                      }).ToList();

            model.ApplicantShortListApprovalDetail = jobPostList;


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
        public ActionResult Edit([Bind(Exclude = "Attachment")] ApplicantShortListApprovalViewModel model)
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
                    var obj = _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.GetByID(model.Id);
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
                        _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.Update(entity);
                        _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.SaveChanges();
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
            var obj = _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.GetByID(id);           
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.Delete(id);
                _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.SaveChanges();
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
                _prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.Delete(id);
                _prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.SaveChanges();
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

        #region Populate Dropdown
        private void populateDropdown(ApplicantShortListApprovalViewModel model)
        {
            dynamic ddlList;

            #region job advertisement

            ddlList = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();
            model.JobAdvertisementInfoList = Common.PopulateJobAdvertisementDDL(ddlList);

            #endregion

        }

        #endregion

        private PRM_ApplicantShortListApproval CreateEntity(ApplicantShortListApprovalViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();


            #region ApplicantShortList detail
            foreach (var c in model.ApplicantShortListApproval)
            {
                var prm_ApplicantShortListApprovalDetail = new PRM_ApplicantShortListApprovalDetail();

                if (c.IsCheckedFinal)
                {
                    prm_ApplicantShortListApprovalDetail.Id = c.Id;
                    prm_ApplicantShortListApprovalDetail.ApplicantInfoId = (int)c.ApplicantInfoId;
                    prm_ApplicantShortListApprovalDetail.IUser = User.Identity.Name;
                    prm_ApplicantShortListApprovalDetail.IDate = DateTime.Now;

                    if (pAddEdit)
                    {
                        prm_ApplicantShortListApprovalDetail.IUser = User.Identity.Name;
                        prm_ApplicantShortListApprovalDetail.IDate = DateTime.Now;
                        entity.PRM_ApplicantShortListApprovalDetail.Add(prm_ApplicantShortListApprovalDetail);
                    }
                    else
                    {
                        prm_ApplicantShortListApprovalDetail.ApplicantShortListApprovalId = model.Id;
                        prm_ApplicantShortListApprovalDetail.EUser = User.Identity.Name;
                        prm_ApplicantShortListApprovalDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            var requInfo = _prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.GetAll().Where(x => x.ApplicantInfoId == c.ApplicantInfoId).ToList();
                            if (requInfo.Count == 0)
                            {
                                _prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.Add(prm_ApplicantShortListApprovalDetail);
                            }
                        }
                        else
                        {
                            _prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.Update(prm_ApplicantShortListApprovalDetail);

                        }
                    }
                    _prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.SaveChanges();
                }
            }
            #endregion




            return entity;
        }

        private ApplicantShortListApprovalViewModel GetInsertUserAuditInfo(ApplicantShortListApprovalViewModel model, bool pAddEdit)
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

        private bool CheckDuplicateEntry(ApplicantShortListApprovalViewModel model, int strMode)
        {
            if (strMode < 1)
            {

                return _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.Get(q => q.JobAdvertisementInfoId == model.JobAdvertisementInfoId).Any();
            }

            else
            {

                return _prmCommonservice.PRMUnit.ApplicantShortListApprovalRepository.Get(q => q.JobAdvertisementInfoId == model.JobAdvertisementInfoId && strMode != q.Id).Any();
            }
        }
        #endregion

        #region Attachment

        private int Upload(ApplicantShortListApprovalViewModel model)
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

        public void DownloadDoc(ApplicantShortListApprovalViewModel model)
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

        //get short listed by(Emp Info)  
        [NoCache]
        public JsonResult GetShortedInfo(int jobAdId)
        {
            var obj = _prmCommonservice.PRMUnit.ApplicantShortListRepository.Get(q => q.JobAdvertisementInfoId == jobAdId).FirstOrDefault();
            return Json(new
            {
                EmpId = obj == null ? string.Empty : obj.PRM_EmploymentInfo.EmpID,
                ShortListedEmpName = obj == null ? string.Empty : obj.PRM_EmploymentInfo.FullName,
                ShortListedDesignation = obj == null ? string.Empty : obj.PRM_EmploymentInfo.PRM_Designation.Name,
                ShortListDate = obj == null ? string.Empty : obj.Date.ToString("yyyy-MM-dd")

            });

        }

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
            List<ApplicantShortListApprovalDetailViewModel> list = new List<ApplicantShortListApprovalDetailViewModel>();

            list = (from jobAdDtlReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll()
                    join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on jobAdDtlReq.JobAdvertisementInfoId equals jobAd.Id
                    where (jobAd.Id == jobAdId)
                    select new ApplicantShortListApprovalDetailViewModel
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
                              select new ApplicantShortListApprovalDetailViewModel
                              {
                                  DesignationId = jobAdPost.DesignationId,
                                  DesignationName = jobAdPost.PRM_Designation.Name,
                                  DepartmentId = jobAdPost.PRM_Division == null ? Convert.ToInt32(null) : jobAdPost.PRM_Division.Id,
                                  DepartmentName = jobAdPost.PRM_Division == null ? string.Empty : jobAdPost.PRM_Division.Name,
                                  SectionId = jobAdPost.PRM_Section == null ? null : jobAdPost.SectionId,
                                  SectionName = jobAdPost.PRM_Section == null ? string.Empty : jobAdPost.PRM_Section.Name,
                                  NoOfPost = jobAdPost.NumberOfPosition
                              }).ToList();

            return PartialView("_JobPost", new ApplicantShortListApprovalViewModel { ApplicantShortListApprovalDetail = list });

        }


        //get applicant info by jobpost(designation) id
        [HttpPost]
        public PartialViewResult AddedApplicantInfo(List<ApplicantShortListApprovalDetailViewModel> jobPosts, int jobAdId, string strMode)
        {
            var model = new ApplicantShortListApprovalViewModel();

            List<ApplicantShortListApprovalViewModel> AssignmentList = new List<ApplicantShortListApprovalViewModel>();
            if (jobPosts != null)
            {
                //  var list = _prmCommonservice.PRMUnit.ApplicantInfoRepository.GetAll().Where(x => jobPosts.Select(n => n.DesignationId).Contains(x.DesignationId)).ToList();
                var list = (from applicantSrtLst in _prmCommonservice.PRMUnit.ApplicantShortListDetailRepository.GetAll()
                            join applicantInfo in _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll() on applicantSrtLst.ApplicantInfoId equals applicantInfo.intPK
                            select applicantInfo).Where(x => jobPosts.Select(n => n.DesignationId).Contains(x.DesignationId)).Where(q => q.CircularID == jobAdId).ToList();
                
                foreach (var vmApplicant in list)
                {

                    var dupList = _prmCommonservice.PRMUnit.ApplicantShortListApprovalDetailRepository.GetAll().Where(x => x.ApplicantInfoId == vmApplicant.intPK).ToList();   // for checking duplicate

                    if (strMode == "Create")
                    {
                        if (dupList.Count == 0)
                        {
                            var gridModel = new ApplicantShortListApprovalViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                MotherName = vmApplicant.MothersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                //DesignationName = desi.Name
                            };
                            AssignmentList.Add(gridModel);
                        }
                    }
                    else
                    {
                        if (dupList.Count != 0)
                        {
                            var gridModel = new ApplicantShortListApprovalViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                MotherName = vmApplicant.MothersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                //DesignationName = desi.Name
                                IsCheckedFinal = true,

                            };
                            AssignmentList.Add(gridModel);
                        }
                        else
                        {
                            var gridModel = new ApplicantShortListApprovalViewModel
                            {
                                ApplicantInfoId = vmApplicant.intPK,
                                ApplicantName = vmApplicant.Name,
                                FatherName = vmApplicant.FathersName,
                                MotherName = vmApplicant.MothersName,
                                DateOfBirth = vmApplicant.DateOfBirth.ToString("dd-MM-yyyy"),
                                NID = vmApplicant.NationalId,
                                //DesignationName = desi.Name
                                IsCheckedFinal = false
                            };
                            AssignmentList.Add(gridModel);

                        }

                    }
                }

                model.ApplicantShortListApproval = AssignmentList;
            }
            return PartialView("_Details", model);
        }

    }
}