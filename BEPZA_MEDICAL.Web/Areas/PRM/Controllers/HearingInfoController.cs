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
    public class HearingInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public HearingInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions

        //
        // GET: /PRM/HearingInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, HearingInfoViwModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<HearingInfoViwModel> list = (from hearingInfo in _prmCommonService.PRMUnit.HearingInfoRepository.GetAll()
                                              join hFixInfo in _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetAll() on hearingInfo.HearingFixationInfoId equals hFixInfo.Id
                                              join hFixInfoDtl in _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.GetAll() on hearingInfo.HearingFixationInfoDetailId equals hFixInfoDtl.Id
                                              join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll() on hearingInfo.ComplaintNoteSheetId equals comNoteSheet.Id
                                              join complaintEmpInfo in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals complaintEmpInfo.Id
                                              where (comNoteSheet.ZoneInfoId == LoggedUserZoneInfoId)
                                              && (model.ComplaintNoteSheetId == 0 || model.ComplaintNoteSheetId == comNoteSheet.Id)
                                              && (model.HearingDateL == null || hFixInfoDtl.HearingDate == Convert.ToDateTime(model.HearingDateL))
                                              && (model.HearingFixationInfoId == 0 || model.HearingFixationInfoId == hFixInfo.Id)
                                              select new HearingInfoViwModel()
                                                      {
                                                          Id = hearingInfo.Id,
                                                          ComplaintNoteSheetId = hearingInfo.ComplaintNoteSheetId,
                                                          ComplaintNoteSheetName = hearingInfo.PRM_ComplaintNoteSheet.DeptProceedingNo,
                                                          HearingFixationInfoId = hearingInfo.HearingFixationInfoId,
                                                          HearingFixationInfoRefNo = hearingInfo.PRM_HearingFixationInfo.HearingRefNo,
                                                          HearingDateL = hFixInfoDtl.HearingDate
                                                      }).OrderByDescending(x => x.ComplaintNoteSheetName).ToList();



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

            if (request.SortingName == "HearingFixationInfoRefNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HearingFixationInfoRefNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HearingFixationInfoRefNo).ToList();
                }
            }

            if (request.SortingName == "HearingDateL")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HearingDateL).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HearingDateL).ToList();
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
                    d.HearingFixationInfoId,  
                    d.HearingFixationInfoRefNo,  
                    Convert.ToDateTime(d.HearingDateL).ToString(DateAndTime.GlobalDateFormat), 
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            HearingInfoViwModel model = new HearingInfoViwModel();
            populateDropdown(model);
            model.IsAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] HearingInfoViwModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = model.ToEntity();
                    // var entity = CreateEntity(model, true);

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
                        _prmCommonService.PRMUnit.HearingInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.HearingInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
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
            var entity = _prmCommonService.PRMUnit.HearingInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;
            model.strMode = "Edit";

            model.HearingInfoRecordedByEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.HearingInfoRecordedByEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.HearingInfoRecordedByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

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

            var hfixinfoDtlObj = (from hFixInfoDtl in _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.GetAll()
                                  join hFixInfo in _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetAll() on hFixInfoDtl.HearingFixationInfoId equals hFixInfo.Id
                                  where (hFixInfo.Id == entity.HearingFixationInfoId)
                                  select new
                                  {
                                      HearingDate = hFixInfoDtl.HearingDate,
                                      HearingTime = hFixInfoDtl.HearingTime,
                                      HearingLocation = hFixInfoDtl.HearingLocation,
                                  }).OrderByDescending(o => o.HearingDate).FirstOrDefault();

            model.HearingDate = hfixinfoDtlObj.HearingDate.ToString("dd-MM-yyyy");
            model.HearingTime = hfixinfoDtlObj.HearingTime.ToString();
            model.HearingLocation = hfixinfoDtlObj.HearingLocation;

            populateDropdown(model);
            DownloadDoc(model);
            model.IsAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] HearingInfoViwModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    // Set preious attachment if exist
                    var obj = _prmCommonService.PRMUnit.HearingInfoRepository.GetByID(model.Id);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    //

                    model = GetInsertUserAuditInfo(model, false);
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
                        _prmCommonService.PRMUnit.HearingInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.HearingInfoRepository.SaveChanges();
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
                    //model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
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
                _prmCommonService.PRMUnit.HearingInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.HearingInfoRepository.SaveChanges();
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
        private void populateDropdown(HearingInfoViwModel model)
        {
            dynamic ddlList;
            //use as Departmental Proceedigs No.
            #region Complaint/Note Sheet
            
            ddlList = (from hFixInfo in _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                       join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId) on hFixInfo.ComplaintNoteSheetId equals comNoteSheet.Id
                       select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();

            model.ComplaintNoteSheetList = Common.PopulateComplaintNoteSheet(ddlList);

            #endregion

            //use as Hearing ref No.
            #region Hearing Fixation Info

            ddlList = _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetAll().Where(q => q.Id == model.HearingFixationInfoId).OrderBy(x => x.HearingRefNo).ToList();
            model.HearingFixationInfoList = Common.PopulateHearingFixationInfo(ddlList);

            #endregion
        }

        #endregion


        private HearingInfoViwModel GetInsertUserAuditInfo(HearingInfoViwModel model, bool pAddEdit)
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

        private int Upload(HearingInfoViwModel model)
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

        public void DownloadDoc(HearingInfoViwModel model)
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
        public JsonResult GetHearingFixInfoDetail(int hFixInfoId)
        {
            var obj = (from hFixInfoDtl in _prmCommonService.PRMUnit.HearingFixationInfoDetailRepository.GetAll()
                       join hFixInfo in _prmCommonService.PRMUnit.HearingFixationInfoRepository.GetAll() on hFixInfoDtl.HearingFixationInfoId equals hFixInfo.Id
                       where (hFixInfo.Id == hFixInfoId)
                       select new
                       {
                           HearingFixationInfoDetailId = hFixInfoDtl.Id,
                           HearingDate = hFixInfoDtl.HearingDate,
                           HearingTime = hFixInfoDtl.HearingTime,
                           HearingLocation = hFixInfoDtl.HearingLocation,
                       }).OrderByDescending(o => o.HearingDate).FirstOrDefault();


            return Json(new
            {
                HearingFixationInfoDetailId = obj.HearingFixationInfoDetailId,
                HearingDate = Convert.ToDateTime(obj.HearingDate).ToString("dd-MM-yyyy"),
                HearingTime = obj.HearingTime.ToString(),
                HearingLocation = obj.HearingLocation
            });

        }

        //use as Hearing ref no
        public ActionResult GetHearingFixationInfo(int deptProceedingId)
        {
            var list = _prmCommonService.PRMUnit.HearingFixationInfoRepository.Get(q => q.ComplaintNoteSheetId == deptProceedingId).ToList();
            return Json(
               new
               {
                   hearingRefNos = list.Select(x => new { Id = x.Id, HearingRefNo = x.HearingRefNo })
               },
               JsonRequestBehavior.AllowGet
           );
        }

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

        public ActionResult HearingRefNoListView()
        {
            var list = Common.PopulateHearingFixationInfo(_prmCommonService.PRMUnit.HearingFixationInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.HearingRefNo).ToList());
            return PartialView("Select", list);
        }

    }
}