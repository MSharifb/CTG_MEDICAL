using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ExplanationReceivedInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public ExplanationReceivedInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions


        //
        // GET: /PRM/ExplanationReceivedInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ExplanationReceivedInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ExplanationReceivedInfoViewModel> list = (from expRec in _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.GetAll()
                                                           join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll() on expRec.ComplaintNoteSheetId equals comNoteSheet.Id
                                                           join complaintEmpInfo in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals complaintEmpInfo.Id
                                                           where (comNoteSheet.ZoneInfoId == LoggedUserZoneInfoId)
                                                           && (model.ComplaintNoteSheetId == 0 || model.ComplaintNoteSheetId == comNoteSheet.Id)
                                                           && (model.NoticeTypeId == 0 || model.NoticeTypeId == expRec.NoticeTypeId)
                                                           && (model.ComplaintDate == null || comNoteSheet.ComplaintDate == Convert.ToDateTime(model.ComplaintDate))
                                                           select new ExplanationReceivedInfoViewModel()
                                                               {
                                                                   Id = expRec.Id,
                                                                   ComplaintNoteSheetId = expRec.ComplaintNoteSheetId,
                                                                   ComplaintNoteSheetName = expRec.PRM_ComplaintNoteSheet.DeptProceedingNo,
                                                                   ExplanationDate = expRec.ExplanationDate,
                                                                   NoticeTypeId = expRec.NoticeTypeId,
                                                                   NoticeTypeName = expRec.PRM_NoticeType.Name,
                                                                   ComplaintDate = comNoteSheet.ComplaintDate.ToString(),
                                                                   ComplaintEmployeeName = complaintEmpInfo.FullName,
                                                                   ComplaintDesignationName = complaintEmpInfo.PRM_Designation.Name
                                                               }).OrderBy(x => x.Id).ToList();



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


            if (request.SortingName == "ExplanationDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ExplanationDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ExplanationDate).ToList();
                }
            }

            if (request.SortingName == "NoticeTypeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.NoticeTypeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.NoticeTypeName).ToList();
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

            if (request.SortingName == "ComplaintEmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplaintEmployeeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplaintEmployeeName).ToList();
                }
            }
            if (request.SortingName == "ComplaintDesignationName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplaintDesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplaintDesignationName).ToList();
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
                    Convert.ToDateTime(d.ComplaintDate).ToString("dd-MM-yyyy"),  
                    d.ComplaintEmployeeName,
                    d.ComplaintDesignationName,                   
                    d.NoticeTypeId,
                    d.NoticeTypeName,
                    Convert.ToDateTime(d.ExplanationDate).ToString("dd-MM-yyyy"), 
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ExplanationReceivedInfoViewModel model = new ExplanationReceivedInfoViewModel();
            populateDropdown(model);
            model.IsAddAttachment = true;
            return View(model);
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] ExplanationReceivedInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = model.ToEntity();

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
                        _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
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
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.GetByID(Id);
            var model = entity.ToModel();
            model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;
            model.strMode = "Edit";

            model.ExplanationReceivedByEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.ExplanationReceivedByEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.ExplanationReceivedByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

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
                           ComplaintDepartmentName = empComplaint.PRM_Division.Name,
                           ComplainantEmpId = empComplainant.EmpID,
                           ComplainantEmployeeName = empComplainant.FullName,
                           ComplainantDesignationName = empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant.PRM_Division.Name,
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

            populateDropdown(model);
            DownloadDoc(model);
            model.IsAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] ExplanationReceivedInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    // Set preious attachment if exist
                    var obj = _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.GetByID(model.Id);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    //

                    model = GetInsertUserAuditInfo(model, true);
                    var entity = model.ToEntity();
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
                        _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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

            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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

        #endregion

        #region Private Method
        private void populateDropdown(ExplanationReceivedInfoViewModel model)
        {
            dynamic ddlList;
            //use as Departmental Proceedigs No.
            #region Complaint/Note Sheet

            ddlList = (from CN in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                        join NO in _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetAll() on CN.Id equals NO.ComplaintNoteSheetId
                        where (NO.IsOrder == true && CN.ZoneInfoId == LoggedUserZoneInfoId)
                        select CN).DistinctBy(q=>q.Id).OrderBy(x => x.DeptProceedingNo).ToList();

           //ddlList = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.DeptProceedingNo).ToList();
           
            model.ComplaintNoteSheetList = Common.PopulateComplaintNoteSheet(ddlList);

            #endregion

            #region Notice Type

            ddlList = _prmCommonService.PRMUnit.NoticeTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.NoticeTypeList = Common.PopulateDllList(ddlList);

            #endregion
        }
        private ExplanationReceivedInfoViewModel GetInsertUserAuditInfo(ExplanationReceivedInfoViewModel model, bool pAddEdit)
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

        #region Attachment

        private int Upload(NoticeInfoViewModel model)
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

        public void DownloadDoc(ExplanationReceivedInfoViewModel model)
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

        [NoCache]
        public JsonResult GetComplaintNoteInfo(int complaintNoteSheetId)
        {
            var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                       join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                       join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id
                       where (comNoteSheet.Id == complaintNoteSheetId)
                       select new
                       {
                           //Id = comNoteSheet.Id,
                           //DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                           RefNo = comNoteSheet.RefNo,
                           ComplaintDate = comNoteSheet.ComplaintDate,
                           ComplaintDetails = comNoteSheet.ComplaintDetails,
                           ComplaintEmpId = empComplaint.EmpID,
                           ComplaintEmployeeName = empComplaint.FullName,
                           ComplaintDesignationName = empComplaint.PRM_Designation.Name,
                           ComplaintDepartmentName = empComplaint.PRM_Division.Name,
                           ComplainantEmpId = empComplainant.EmpID,
                           ComplainantEmployeeName = empComplainant.FullName,
                           ComplainantDesignationName = empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant.PRM_Division.Name,
                       }).FirstOrDefault();

            // var obj = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetByID(complaintNoteSheetId);
            return Json(new
            {
                RefNo = obj.RefNo,
                ComplaintDate = Convert.ToDateTime(obj.ComplaintDate).ToString("dd-MM-yyyy"),
                ComplaintDetails = obj.ComplaintDetails,
                ComplaintEmpId = obj.ComplaintEmpId,
                ComplaintEmployeeName = obj.ComplaintEmployeeName,
                ComplaintDesignationName = obj.ComplaintDesignationName,
                ComplaintDepartmentName = obj.ComplaintDepartmentName,
                ComplainantEmpId = obj.ComplainantEmpId,
                ComplainantEmployeeName = obj.ComplainantEmployeeName,
                ComplainantDesignationName = obj.ComplainantDesignationName,
                ComplainantDepartmentName = obj.ComplainantDepartmentName
            });

        }


        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeName = obj.FullName,
                DesignationName = obj.PRM_Designation.Name,
                DepartmentName = obj.PRM_Division.Name
            });

        }


        [NoCache]
        public ActionResult DeptProceedingListView()
        {
            var ddlList = (from explanation in _prmCommonService.PRMUnit.ExplanationReceivedInfoRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                           join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId) on explanation.ComplaintNoteSheetId equals comNoteSheet.Id
                           select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();

            var list = Common.PopulateComplaintNoteSheet(ddlList);
            return PartialView("Select", list);
        }

        public ActionResult NoticeTypeListView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.NoticeTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        //Noice Info List
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetNoticeInfoListByCompalintNoteSheetId(int compalintNoteSheetId)
        {
            var list = (from noticeInfo in _prmCommonService.PRMUnit.NoticeInfoReportRepository.GetAll()
                        join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll() on noticeInfo.ComplaintNoteSheetId equals comNoteSheet.Id
                        join complaintEmpInfo in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals complaintEmpInfo.Id
                        where (comNoteSheet.Id == compalintNoteSheetId && noticeInfo.IsIssueNotice == true)
                        select new
                        {
                            Id = noticeInfo.Id,
                            //ComplaintNoteSheetId = noticeInfo.ComplaintNoteSheetId,
                            ComplaintNoteSheetName = noticeInfo.PRM_ComplaintNoteSheet.DeptProceedingNo,
                            ComplaintDate = Convert.ToDateTime(comNoteSheet.ComplaintDate).ToString("dd-MM-yyyy"),
                            ComplaintEmployeeName = complaintEmpInfo.FullName,
                            ComplaintDesignationName = complaintEmpInfo.PRM_Designation.Name,
                            // NoticeDate = noticeInfo.NoticeDate,
                            // NoticeTypeId = noticeInfo.NoticeTypeId,
                            NoticeTypeName = noticeInfo.PRM_NoticeType.Name,
                            NoticeStatus = noticeInfo.IsIssueNotice == true ? "Submitted" : "Pending"
                        }).OrderBy(x => x.ComplaintDate).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}