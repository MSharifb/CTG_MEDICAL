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
    public class HearingFixationInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public HearingFixationInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions


        //
        // GET: /PRM/HearingFixationInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, HearingFixationInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<HearingFixationInfoViewModel> list = (from hfixInfo in _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetAll()
                                                       join hfixInfoDtl in _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.GetAll() on hfixInfo.Id equals hfixInfoDtl.HearingFixationInfoId
                                                       join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll() on hfixInfo.ComplaintNoteSheetId equals comNoteSheet.Id
                                                       join complaintEmpInfo in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals complaintEmpInfo.Id
                                                       where (hfixInfo.ZoneInfoId == LoggedUserZoneInfoId)
                                                       && (model.ComplaintNoteSheetId == 0 || model.ComplaintNoteSheetId == comNoteSheet.Id)
                                                       && (model.ComplaintDate == null || comNoteSheet.ComplaintDate == Convert.ToDateTime(model.ComplaintDate))
                                                       && (model.HearingDate == null || hfixInfoDtl.HearingDate == Convert.ToDateTime(model.HearingDate))
                                                       && (model.HearingRefNo == null || model.HearingRefNo == "" || model.HearingRefNo == hfixInfo.HearingRefNo)
                                                       select new HearingFixationInfoViewModel()
                                                        {
                                                            Id = hfixInfo.Id,
                                                            ComplaintNoteSheetId = hfixInfo.ComplaintNoteSheetId,
                                                            ComplaintNoteSheetName = hfixInfo.PRM_ComplaintNoteSheet.DeptProceedingNo,
                                                            ComplaintDate = comNoteSheet.ComplaintDate.ToString(),
                                                            HearingRefNo = hfixInfo.HearingRefNo,
                                                            HearingDate = hfixInfoDtl.HearingDate,
                                                            HearingTime = hfixInfoDtl.HearingTime,
                                                            HearingLocation = hfixInfoDtl.HearingLocation
                                                        }).OrderByDescending(x => x.HearingDate).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ComplaintNoteSheetName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplaintNoteSheetName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplaintNoteSheetName).ToList();
                }
            }

            if (request.SortingName == "ComplaintDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplaintDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplaintDate).ToList();
                }
            }

            if (request.SortingName == "HearingRefNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HearingRefNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HearingRefNo).ToList();
                }
            }

            if (request.SortingName == "HearingDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HearingDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HearingDate).ToList();
                }
            }

            if (request.SortingName == "HearingTime")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HearingTime).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HearingTime).ToList();
                }
            }
            if (request.SortingName == "HearingLocation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HearingLocation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HearingLocation).ToList();
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
                    d.ComplaintNoteSheetId,
                    d.ComplaintNoteSheetName,
                    Convert.ToDateTime(d.ComplaintDate).ToString(DateAndTime.GlobalDateFormat),  
                    d.HearingRefNo,
                    Convert.ToDateTime(d.HearingDate).ToString(DateAndTime.GlobalDateFormat),  
                    d.HearingTime.ToString(),
                    d.HearingLocation,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            HearingFixationInfoViewModel model = new HearingFixationInfoViewModel();
            populateDropdown(model);
            model.IsAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] HearingFixationInfoViewModel model)
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

                    if (CheckDuplicateRefNo(model, model.Id))
                    {
                        model.ErrMsg = "Hearing Ref. No.";
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }

                    model = GetInsertUserAuditInfo(model, true);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
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
                        _prmCommonService.PRMUnit.HearingFixationInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.HearingFixationInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
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
                    // model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
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
        public ActionResult Edit(int id)
        {
            var entity = _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;
            model.strMode = "Edit";

            model.HearingfixationByEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.HearingfixationByEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.HearingfixationByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

            var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                       join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                       join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id
                       where (comNoteSheet.Id == entity.ComplaintNoteSheetId)
                       select new
                       {
                           //Id = comNoteSheet.Id,
                           //DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                           RefNo = comNoteSheet.RefNo,
                           ComplaintDate = comNoteSheet.ComplaintDate,
                           ComplaintEmpId = empComplaint.EmpID,
                           ComplaintEmployeeName = empComplaint.FullName,
                           ComplaintDesignationName = empComplaint.PRM_Designation.Name,
                           ComplaintDepartmentName = empComplaint.PRM_Division == null ? string.Empty : empComplaint.PRM_Division.Name,
                           ComplainantEmpId = empComplainant.EmpID,
                           ComplainantEmployeeName = empComplainant.FullName,
                           ComplainantDesignationName = empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant.PRM_Division == null ? string.Empty : empComplainant.PRM_Division.Name,
                       }).FirstOrDefault();

            model.RefNo = obj.RefNo;
            model.ComplaintDate = Convert.ToDateTime(obj.ComplaintDate).ToString("dd-MM-yyyy");
            model.ComplaintEmpId = obj.ComplaintEmpId;
            model.ComplaintEmployeeName = obj.ComplaintEmployeeName;
            model.ComplaintDesignationName = obj.ComplaintDesignationName;
            model.ComplaintDepartmentName = obj.ComplaintDepartmentName;
            model.ComplainantEmpId = obj.ComplainantEmpId;
            model.ComplainantEmployeeName = obj.ComplainantEmployeeName;
            model.ComplainantDesignationName = obj.ComplainantDesignationName;
            model.ComplainantDepartmentName = obj.ComplainantDepartmentName;

            //Employee Clearance Form Details

            List<HearingFixationInfoDetailViewModel> resultFrm = (from hFixInfoDtl in _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.GetAll()
                                                                  join hFixInfo in _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetAll() on hFixInfoDtl.HearingFixationInfoId equals hFixInfo.Id
                                                                  where (hFixInfo.Id == id)
                                                                  select new HearingFixationInfoDetailViewModel()
                                                                    {
                                                                        Id = hFixInfoDtl.Id,
                                                                        HearingFixationInfoId = hFixInfoDtl.HearingFixationInfoId,
                                                                        HearingDate = hFixInfoDtl.HearingDate,
                                                                        HearingTime = hFixInfoDtl.HearingTime,
                                                                        HearingLocation = hFixInfoDtl.HearingLocation,
                                                                        HearingStatus = hFixInfoDtl.HearingStatus,
                                                                        HearingComments = hFixInfoDtl.HearingComments,
                                                                    }).ToList();
            model.HearingFixationInfoDetail = resultFrm;

            populateDropdown(model);
            DownloadDoc(model);
            model.IsAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] HearingFixationInfoViewModel model)
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

                    if (CheckDuplicateRefNo(model, model.Id))
                    {
                        model.ErrMsg = "Hearing Ref. No.";
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }

                    // Set preious attachment if exist
                    var obj = _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetByID(model.Id);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    //

                    model = GetInsertUserAuditInfo(model, false);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
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
                        _prmCommonService.PRMUnit.HearingFixationInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.HearingFixationInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
                    //  model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
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
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                //  _prmCommonService.PRMUnit.HearingFixationInfoRepository.Delete(id);
                List<Type> allTypes = new List<Type> { typeof(PRM_HearingFixationInfoDetail) };
                _prmCommonService.PRMUnit.HearingFixationInfoRepository.Delete(id, allTypes);

                _prmCommonService.PRMUnit.HearingFixationInfoRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.SaveChanges();
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
        private void populateDropdown(HearingFixationInfoViewModel model)
        {
            dynamic ddlList;
            //use as Departmental Proceedigs No.
            #region Complaint/Note Sheet

            ddlList = (from explanationInfo in _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                       join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId) on explanationInfo.ComplaintNoteSheetId equals comNoteSheet.Id
                       where explanationInfo.IsHearingRequired == true
                       select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();
            model.ComplaintNoteSheetList = Common.PopulateComplaintNoteSheet(ddlList);

            #endregion

            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Actual", Value = "Actual", Selected = true });
            list.Add(new SelectListItem() { Text = "Revised", Value = "Revised" });

            model.HearingStatusList = list;
        }

        #endregion

        //check duplicate Dept proceding
        private bool CheckDuplicateEntry(HearingFixationInfoViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _prmCommonService.PRMUnit.HearingFixationInfoRepository.Get(q => q.ComplaintNoteSheetId == model.ComplaintNoteSheetId).Any();
            }

            else
            {
                var ss = _prmCommonService.PRMUnit.HearingFixationInfoRepository.Get(q => q.ComplaintNoteSheetId == model.ComplaintNoteSheetId && strMode != q.Id).Any();
                return _prmCommonService.PRMUnit.HearingFixationInfoRepository.Get(q => q.ComplaintNoteSheetId == model.ComplaintNoteSheetId && strMode != q.Id).Any();
            }
        }

        //check duplicate ref no
        private bool CheckDuplicateRefNo(HearingFixationInfoViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _prmCommonService.PRMUnit.HearingFixationInfoRepository.Get(q => q.ComplaintNoteSheetId == model.ComplaintNoteSheetId).Any();
            }

            else
            {
                var ss = _prmCommonService.PRMUnit.HearingFixationInfoRepository.Get(q => q.ComplaintNoteSheetId == model.ComplaintNoteSheetId && strMode != q.Id).Any();
                return _prmCommonService.PRMUnit.HearingFixationInfoRepository.Get(q => q.ComplaintNoteSheetId == model.ComplaintNoteSheetId && strMode != q.Id).Any();
            }
        }
        private PRM_HearingFixationInfo CreateEntity(HearingFixationInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            #region HearingFixationInfo detail
            foreach (var c in model.HearingFixationInfoDetail)
            {
                var prm_HearingFixationInfoDetail = new PRM_HearingFixationInfoDetail();

                prm_HearingFixationInfoDetail.Id = c.Id;
                prm_HearingFixationInfoDetail.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                prm_HearingFixationInfoDetail.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                prm_HearingFixationInfoDetail.EUser = c.EUser;
                prm_HearingFixationInfoDetail.EDate = c.EDate;

                prm_HearingFixationInfoDetail.HearingDate = Convert.ToDateTime(c.HearingDate);
                prm_HearingFixationInfoDetail.HearingTime = c.HearingTime;
                prm_HearingFixationInfoDetail.HearingLocation = c.HearingLocation;
                prm_HearingFixationInfoDetail.HearingStatus = c.HearingStatus;
                prm_HearingFixationInfoDetail.HearingComments = c.HearingComments;

                if (pAddEdit)
                {
                    prm_HearingFixationInfoDetail.IUser = User.Identity.Name;
                    prm_HearingFixationInfoDetail.IDate = DateTime.Now;

                    //entity.PRM_HearingFixationInfoDetail.Add(prm_HearingFixationInfoDetail);
                }
                else
                {
                    prm_HearingFixationInfoDetail.HearingFixationInfoId = model.Id;
                    prm_HearingFixationInfoDetail.EUser = User.Identity.Name;
                    prm_HearingFixationInfoDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.Add(prm_HearingFixationInfoDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.Update(prm_HearingFixationInfoDetail);
                    }

                }
            }

            #endregion

            return entity;
        }

        private HearingFixationInfoViewModel GetInsertUserAuditInfo(HearingFixationInfoViewModel model, bool pAddEdit)
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

        #region Attachment

        private int Upload(HearingFixationInfoViewModel model)
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

        public void DownloadDoc(HearingFixationInfoViewModel model)
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

        //[NoCache]
        //public JsonResult GetComplaintNoteInfo(int complaintNoteSheetId)
        //{
        //    var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
        //               join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
        //               join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id
        //               where (comNoteSheet.Id == complaintNoteSheetId)
        //               select new
        //               {
        //                   //Id = comNoteSheet.Id,
        //                   //DeptProceedingNo = comNoteSheet.DeptProceedingNo,
        //                   RefNo = comNoteSheet.RefNo,
        //                   ComplaintDate = comNoteSheet.ComplaintDate,
        //                   ComplaintDetails = comNoteSheet.ComplaintDetails,
        //                   ComplaintEmpId = empComplaint.EmpID,
        //                   ComplaintEmployeeName = empComplaint.FullName,
        //                   ComplaintDesignationName = empComplaint.PRM_Designation.Name,
        //                   ComplaintDepartmentName = empComplaint.PRM_Division == null ? string.Empty : empComplaint.PRM_Division.Name,
        //                   ComplainantEmpId = empComplainant.EmpID,
        //                   ComplainantEmployeeName = empComplainant.FullName,
        //                   ComplainantDesignationName = empComplainant.PRM_Designation.Name,
        //                   ComplainantDepartmentName = empComplainant.PRM_Division == null ? string.Empty : empComplainant.PRM_Division.Name,
        //               }).FirstOrDefault();

            
        //    return Json(new
        //    {
        //        RefNo = obj.RefNo,
        //        ComplaintDate = Convert.ToDateTime(obj.ComplaintDate).ToString("dd-MM-yyyy"),
        //        ComplaintDetails = obj.ComplaintDetails,
        //        ComplaintEmpId = obj.ComplaintEmpId,
        //        ComplaintEmployeeName = obj.ComplaintEmployeeName,
        //        ComplaintDesignationName = obj.ComplaintDesignationName,
        //        ComplaintDepartmentName = obj.ComplaintDepartmentName,
        //        ComplainantEmpId = obj.ComplainantEmpId,
        //        ComplainantEmployeeName = obj.ComplainantEmployeeName,
        //        ComplainantDesignationName = obj.ComplainantDesignationName,
        //        ComplainantDepartmentName = obj.ComplainantDepartmentName
        //    });

        //}
        

        //search 
        [NoCache]
        public ActionResult DeptProceedingListView()
        {
            var ddlList = (from hearInfo in _prmCommonService.PRMUnit.HearingInfoRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                           join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId) on hearInfo.ComplaintNoteSheetId equals comNoteSheet.Id
                           select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();
            var list = Common.PopulateComplaintNoteSheet(ddlList);
            return PartialView("Select", list);
        }


    }
}