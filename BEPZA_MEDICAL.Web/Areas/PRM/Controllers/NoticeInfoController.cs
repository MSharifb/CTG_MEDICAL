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
    public class NoticeInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public NoticeInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions


        //
        // GET: /PRM/NoticeInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, NoticeInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<NoticeInfoViewModel> list = (from noticeInfo in _prmCommonService.PRMUnit.NoticeInfoReportRepository.GetAll()
                                              join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll() on noticeInfo.ComplaintNoteSheetId equals comNoteSheet.Id
                                              join complaintEmpInfo in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals complaintEmpInfo.Id
                                              where (comNoteSheet.ZoneInfoId == LoggedUserZoneInfoId)
                                              && (model.ComplaintNoteSheetId == 0 || model.ComplaintNoteSheetId == comNoteSheet.Id)
                                              && (model.NoticeTypeId == 0 || model.NoticeTypeId == noticeInfo.NoticeTypeId)
                                              && (model.NoticeDate == null || model.NoticeDate == noticeInfo.NoticeDate)
                                              && (model.ComplaintEmployeeName == "" || model.ComplaintEmployeeName == null || model.ComplaintEmployeeName == complaintEmpInfo.FullName)
                                              && (model.ComplaintDesignationId == null || model.ComplaintDesignationId == 0 || model.ComplaintDesignationId == complaintEmpInfo.DesignationId)
                                              && (model.ComplaintDate == null || comNoteSheet.ComplaintDate == Convert.ToDateTime(model.ComplaintDate))
                                              select new NoticeInfoViewModel()
                                                  {
                                                      Id = noticeInfo.Id,
                                                      ComplaintNoteSheetId = noticeInfo.ComplaintNoteSheetId,
                                                      ComplaintNoteSheetName = noticeInfo.PRM_ComplaintNoteSheet.DeptProceedingNo,
                                                      NoticeDate = noticeInfo.NoticeDate,
                                                      NoticeTypeId = noticeInfo.NoticeTypeId,
                                                      NoticeTypeName = noticeInfo.PRM_NoticeType.Name,
                                                      ComplaintDate = comNoteSheet.ComplaintDate.ToString(),
                                                      ComplaintEmployeeName = complaintEmpInfo.FullName,
                                                      ComplaintDesignationId = complaintEmpInfo.DesignationId,
                                                      ComplaintDesignationName = complaintEmpInfo.PRM_Designation.Name,
                                                      NoticeStatus = noticeInfo.IsIssueNotice == true ? "Submitted" : "Pending"
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


            if (request.SortingName == "NoticeDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.NoticeDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.NoticeDate).ToList();
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


            if (request.SortingName == "NoticeStatus")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.NoticeStatus).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.NoticeStatus).ToList();
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
                    d.ComplaintEmployeeName,
                    d.ComplaintDesignationId,   
                    d.ComplaintDesignationName,                   
                    d.NoticeTypeId,
                    d.NoticeTypeName,
                    Convert.ToDateTime(d.NoticeDate).ToString(DateAndTime.GlobalDateFormat),   
                    d.NoticeStatus,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            NoticeInfoViewModel model = new NoticeInfoViewModel();
            populateDropdown(model);
            model.IsAddAttachment = true;
            return View(model);
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] NoticeInfoViewModel model)
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
                        _prmCommonService.PRMUnit.NoticeInfoReportRepository.Add(entity);
                        _prmCommonService.PRMUnit.NoticeInfoReportRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        //return RedirectToAction("Index");
                    }


                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.errClass = "failed";
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
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.NoticeInfoReportRepository.GetByID(Id);
            if (entity.IsIssueNotice)
            {
                return RedirectToAction("Index");
            }
            var model = entity.ToModel();

            model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;
            model.strMode = "Edit";

            model.NoticeIssueByEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.NoticeIssueByEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.NoticeIssueByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

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

            populateDropdown(model);
            DownloadDoc(model);
            model.IsAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] NoticeInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    // Set preious attachment if exist
                    var obj = _prmCommonService.PRMUnit.NoticeInfoReportRepository.GetByID(model.Id);
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
                        _prmCommonService.PRMUnit.NoticeInfoReportRepository.Update(entity);
                        _prmCommonService.PRMUnit.NoticeInfoReportRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
            var obj = _prmCommonService.PRMUnit.NoticeInfoReportRepository.GetByID(id);
            if (obj.IsIssueNotice)
            {
                return Json(new
                {
                    Message = "Sorry! Already Submitted."
                }, JsonRequestBehavior.AllowGet);
            }

            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.NoticeInfoReportRepository.Delete(id);
                _prmCommonService.PRMUnit.NoticeInfoReportRepository.SaveChanges();
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
        private void populateDropdown(NoticeInfoViewModel model)
        {
            dynamic ddlList;
            //use as Departmental Proceedigs No.
            #region Complaint/Note Sheet


            ddlList = (from noteAndOrder in _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                       join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId) on noteAndOrder.ComplaintNoteSheetId equals comNoteSheet.Id
                       where (noteAndOrder.IsOrder == true && comNoteSheet.ZoneInfoId == LoggedUserZoneInfoId)
                       select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();

            model.ComplaintNoteSheetList = Common.PopulateComplaintNoteSheet(ddlList);

            #endregion

            #region Notice Type

            ddlList = _prmCommonService.PRMUnit.NoticeTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.NoticeTypeList = Common.PopulateDllList(ddlList);

            #endregion
        }
        private NoticeInfoViewModel GetInsertUserAuditInfo(NoticeInfoViewModel model, bool pAddEdit)
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

        public void DownloadDoc(NoticeInfoViewModel model)
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
            //bool result = false;
            //var orderType = _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetAll().Where(q => q.ComplaintNoteSheetId == complaintNoteSheetId).OrderByDescending(o => o.OrderDate).FirstOrDefault().PRM_OrderTypeInfo.Name;
            //if (orderType != "Charge Sheet" || orderType != "FIR")
            //{
            //    result = true;
            //}

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
                           ComplaintDepartmentName = empComplaint.PRM_Division == null ? string.Empty : empComplaint.PRM_Division.Name,
                           ComplainantEmpId = empComplainant.EmpID,
                           ComplainantEmployeeName = empComplainant.FullName,
                           ComplainantDesignationName = empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant.PRM_Division == null ? string.Empty : empComplainant.PRM_Division.Name,
                       }).FirstOrDefault();

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
                DepartmentName = obj.PRM_Division == null ? string.Empty : obj.PRM_Division.Name
            });

        }

        public ActionResult GetPunishmentType(int disciplinaryActionTypeId)
        {
            var punishmentList = _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.Get(q => q.DisciplinaryActionTypeId == disciplinaryActionTypeId).ToList();
            return Json(
               new
               {
                   punishments = punishmentList.Select(x => new { Id = x.Id, PunishmentName = x.PunishmentName })
               },
               JsonRequestBehavior.AllowGet
           );
        }

        [NoCache]
        public ActionResult DeptProceedingListView()
        {
            var ddlList = (from notice in _prmCommonService.PRMUnit.NoticeInfoReportRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                           join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId) on notice.ComplaintNoteSheetId equals comNoteSheet.Id
                           select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();

            var list = Common.PopulateComplaintNoteSheet(ddlList);
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult NoticeTypeListView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.NoticeTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult DesignationListView()
        {
            var list = Common.PopulateEmployeeDesignationDDL(_prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult NoticeStatusView()
        {
            var list = PopulateNoticeTypeList();
            return PartialView("Select", list);
        }

        private static IList<SelectListItem> PopulateNoticeTypeList()
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Submitted", Value = "true" });
            list.Add(new SelectListItem() { Text = "Pending", Value = "false" });
            return list;
        }

        //details View
        [HttpPost]
        public ActionResult ViewNoticeDetails(int id)
        {
            var entity = _prmCommonService.PRMUnit.NoticeInfoReportRepository.GetByID(id);
            var model = entity.ToModel();

            model.NoticeIssueByEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.NoticeIssueByEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.NoticeIssueByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            model.NoticeTypeName = entity.PRM_NoticeType.Name;
            model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;

            return PartialView("_NoticeDetailsView", model);
        }
    }
}