using BEPZA_MEDICAL.DAL.PRM;
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
    public class NotesAndDocumentInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public NotesAndDocumentInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions


        //
        // GET: /PRM/NotesAndDocumentInfo/
        public ActionResult Index()
        {
            //Session["attachmentList"] = null;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, NotesAndDocumentInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<NotesAndDocumentInfoViewModel> list = (from notesAndDoc in _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.GetAll()
                                                        join empInfo in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on notesAndDoc.EmployeeId equals empInfo.Id
                                                        join de in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on empInfo.DesignationId equals de.Id
                                                        where (model.EmpId == null || model.EmpId == "" || model.EmpId == empInfo.EmpID)
                                                        && (model.DesignationId == null || model.DesignationId == 0 || model.DesignationId == de.Id)
                                                        && (string.IsNullOrEmpty(model.EmployeeName) || empInfo.FullName.Contains(model.EmployeeName))
                                                        &&(notesAndDoc.ZoneInfoId == LoggedUserZoneInfoId)
                                                        select new NotesAndDocumentInfoViewModel()
                                                        {
                                                            Id = notesAndDoc.Id,
                                                            EmployeeId = notesAndDoc.EmployeeId,
                                                            EmpId = empInfo.EmpID,
                                                            EmployeeName = empInfo.FullName,
                                                            DesignationId = de.Id,
                                                            DesignationName = de.Name,
                                                            DivisionName = empInfo.PRM_Division.Name,
                                                            SectionName = empInfo.PRM_Section == null ? string.Empty : empInfo.PRM_Section.Name,
                                                        }).OrderBy(x => x.EmpId).ToList();



            totalRecords = list == null ? 0 : list.Count;
          
            #region Sorting

            if (request.SortingName == "EmpId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpId).ToList();
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

            if (request.SortingName == "Designation")
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
            if (request.SortingName == "DivisionName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DivisionName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DivisionName).ToList();
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
                    d.EmployeeId,
                    d.EmpId,
                    d.EmployeeName,
                    d.DesignationId,
                    d.DesignationName,
                    d.DivisionName,
                    d.SectionName,   
                    "Review",
                    "Delete"  
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            NotesAndDocumentInfoViewModel model = new NotesAndDocumentInfoViewModel();           
            return View(model);
        }


        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] NotesAndDocumentInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {

                    model = GetInsertUserAuditInfo(model);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        return View(model);
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
                    //model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                else
                {
                  
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id, string type)
        {
            var entity = _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.GetByID(Id);
            var model = entity.ToModel();
            model.strMode = "Edit";

            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.DesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            model.DivisionName = entity.PRM_EmploymentInfo.PRM_Division.Name;
            model.SectionName = entity.PRM_EmploymentInfo.PRM_Section == null ? string.Empty : entity.PRM_EmploymentInfo.PRM_Section.Name;


            List<NotesAndDocumentInfoViewModel> resultFrm = (from notesAndDocDtl in _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.GetAll()
                                                             join notesAndDoc in _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.GetAll() on notesAndDocDtl.NotesAndDocumentInfoId equals notesAndDoc.Id
                                                             where (notesAndDoc.Id == Id)
                                                             select new NotesAndDocumentInfoViewModel()
                                                    {
                                                        Id = notesAndDocDtl.Id,
                                                        NotesAndDocumentInfoId = notesAndDocDtl.NotesAndDocumentInfoId,
                                                        RefNo = notesAndDocDtl.RefNo,
                                                        Date = Convert.ToDateTime(notesAndDocDtl.Date),
                                                        Subject = notesAndDocDtl.Subject,
                                                        Details = notesAndDocDtl.Details,
                                                        IsAddAttachment = notesAndDocDtl.IsAddAttachment,
                                                        FileName = notesAndDocDtl.FileName,
                                                        FileSize = notesAndDocDtl.FileSize
                                                    }).ToList();


            model.TempAttachmentDetail = resultFrm;

            if (type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);   
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] NotesAndDocumentInfoViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);
                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.SaveChanges();                     
                        return RedirectToAction("Edit", new { id = model.Id, type = "success"});
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

            return View(model);
        }


        public ActionResult AddComment(int id)
        {
            //var entity = _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.GetByID(id);
            //var model = entity.ToModel();
            NotesAndDocumentInfoCommentsDetailViewModel model = new NotesAndDocumentInfoCommentsDetailViewModel();
            model.NotesAndDocumentInfoAttachmentDetailId = id;
            List<NotesAndDocumentInfoCommentsDetailViewModel> comList = (from notsAndcommDtl in _prmCommonService.PRMUnit.NotesAndDocumentInfoCommentsDetailRepository.GetAll()
                                                                         where (notsAndcommDtl.NotesAndDocumentInfoAttachmentDetailId == id)
                                                                         select new NotesAndDocumentInfoCommentsDetailViewModel()
                                                                         {
                                                                             Id = notsAndcommDtl.Id,
                                                                             NotesAndDocumentInfoAttachmentDetailId = notsAndcommDtl.NotesAndDocumentInfoAttachmentDetailId,
                                                                             Comments = notsAndcommDtl.Comments,
                                                                             CommentByEmployeeId = notsAndcommDtl.CommentByEmployeeId,
                                                                             Employee = notsAndcommDtl.PRM_EmploymentInfo.FullName,
                                                                             Designation = notsAndcommDtl.PRM_EmploymentInfo.PRM_Designation.Name
                                                                         }).ToList();


            model.NotesAndDocumentInfoCommentsDetailList = comList;

            return View("AddComment", model);
        }


        [HttpPost]
        public ActionResult AddComment(NotesAndDocumentInfoCommentsDetailViewModel model)
        {
            
            //model.IUser = User.Identity.Name;
            //model.IDate = DateTime.Now;

            //var entity = model.ToEntity();
            if (model.NotesAndDocumentInfoCommentsDetailList.Count > 0)
            {
                // Job Grade
                foreach (var c in model.NotesAndDocumentInfoCommentsDetailList)
                {
                    var prm_NotesAndDocumentInfoCommentsDetail = new PRM_NotesAndDocumentInfoCommentsDetail();

                    prm_NotesAndDocumentInfoCommentsDetail.Id = c.Id;
                    prm_NotesAndDocumentInfoCommentsDetail.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                    prm_NotesAndDocumentInfoCommentsDetail.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                    prm_NotesAndDocumentInfoCommentsDetail.EUser = c.EUser;
                    prm_NotesAndDocumentInfoCommentsDetail.EDate = c.EDate;
                    prm_NotesAndDocumentInfoCommentsDetail.NotesAndDocumentInfoAttachmentDetailId = c.NotesAndDocumentInfoAttachmentDetailId;
                    prm_NotesAndDocumentInfoCommentsDetail.Comments = c.Comments;
                    prm_NotesAndDocumentInfoCommentsDetail.CommentByEmployeeId = c.CommentByEmployeeId;


                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.NotesAndDocumentInfoCommentsDetailRepository.Add(prm_NotesAndDocumentInfoCommentsDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.NotesAndDocumentInfoCommentsDetailRepository.Update(prm_NotesAndDocumentInfoCommentsDetail);
                    }
                }
                _prmCommonService.PRMUnit.NotesAndDocumentInfoCommentsDetailRepository.SaveChanges();
            }

            var objAttach = _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.Get(q => q.Id == model.NotesAndDocumentInfoAttachmentDetailId).FirstOrDefault();
            //return View(model);              
            return RedirectToAction("Edit", "NotesAndDocumentInfo", new { id = objAttach.NotesAndDocumentInfoId });


        }



        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.SaveChanges();
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


        [HttpPost, ActionName("DeleteAttachmentDetail")]
        public JsonResult DeleteAttachmentDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.SaveChanges();
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


        [HttpPost, ActionName("DeleteCommentDetail")]
        public JsonResult DeleteCommentDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.NotesAndDocumentInfoCommentsDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.NotesAndDocumentInfoCommentsDetailRepository.SaveChanges();
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
        [NoCache]
        public ActionResult Review(int Id)
        {
            var entity = _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.GetByID(Id);
            var model = entity.ToModel();

            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.DesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            model.DivisionName = entity.PRM_EmploymentInfo.PRM_Division.Name;
            model.SectionName = entity.PRM_EmploymentInfo.PRM_Section == null ? string.Empty : entity.PRM_EmploymentInfo.PRM_Section.Name;


            List<NotesAndDocumentInfoAttachmentDetailViewModel> list = (from notesAndDocDtl in _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.GetAll()
                                                                        join notesAndDoc in _prmCommonService.PRMUnit.NotesAndDocumentInfoRepository.GetAll() on notesAndDocDtl.NotesAndDocumentInfoId equals notesAndDoc.Id
                                                                        where (notesAndDoc.Id == Id)
                                                                        select new NotesAndDocumentInfoAttachmentDetailViewModel()
                                                                        {
                                                                            Id = notesAndDocDtl.Id,
                                                                            NotesAndDocumentInfoId = notesAndDocDtl.NotesAndDocumentInfoId,
                                                                            RefNo = notesAndDocDtl.RefNo,
                                                                            Date = Convert.ToDateTime(notesAndDocDtl.Date),
                                                                            Subject = notesAndDocDtl.Subject,
                                                                            Details = notesAndDocDtl.Details,
                                                                            IsAddAttachment = notesAndDocDtl.IsAddAttachment,
                                                                            FileName = notesAndDocDtl.FileName,
                                                                            Attachment = notesAndDocDtl.Attachment,
                                                                        }).ToList();

            var aList = new List<NotesAndDocumentInfoAttachmentDetailViewModel>();

            foreach (var item in list)
            {
                byte[] document = item.Attachment;
                if (document != null)
                {
                    string strFilename = Url.Content("~/Content/" + User.Identity.Name + item.FileName);
                    byte[] doc = document;
                    WriteToFile(Server.MapPath(strFilename), ref doc);

                    item.FilePath = strFilename;
                }
                aList.Add(item);
            }


            model.NotesAndDocumentInfoAttachmentDetail = aList;

            return View(model);
        }

        public ActionResult ViewComment(int id)
        {
            //var entity = _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.GetByID(id);
            //var model = entity.ToModel();
            NotesAndDocumentInfoCommentsDetailViewModel model = new NotesAndDocumentInfoCommentsDetailViewModel();           
            model.NotesAndDocumentInfoAttachmentDetailId = id;
            List<NotesAndDocumentInfoCommentsDetailViewModel> comList = (from notsAndcommDtl in _prmCommonService.PRMUnit.NotesAndDocumentInfoCommentsDetailRepository.GetAll()
                                                                         where (notsAndcommDtl.NotesAndDocumentInfoAttachmentDetailId == id)
                                                                         select new NotesAndDocumentInfoCommentsDetailViewModel()
                                                                         {
                                                                             Id = notsAndcommDtl.Id,
                                                                             NotesAndDocumentInfoAttachmentDetailId = notsAndcommDtl.NotesAndDocumentInfoAttachmentDetailId,
                                                                             Comments = notsAndcommDtl.Comments,
                                                                             CommentByEmployeeId = notsAndcommDtl.CommentByEmployeeId,
                                                                             Employee = notsAndcommDtl.PRM_EmploymentInfo.FullName,
                                                                             Designation = notsAndcommDtl.PRM_EmploymentInfo.PRM_Designation.Name
                                                                         }).ToList();


            model.NotesAndDocumentInfoCommentsDetailList = comList;

            return PartialView("_ViewComment", model);
        }
        #endregion

        #region Private Method

        private PRM_NotesAndDocumentInfo CreateEntity(NotesAndDocumentInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.TempAttachmentDetail)
            {
                var prm_NotesAndDocumentInfoAttachmentDetail = new PRM_NotesAndDocumentInfoAttachmentDetail();

                prm_NotesAndDocumentInfoAttachmentDetail.Id = c.Id;
                prm_NotesAndDocumentInfoAttachmentDetail.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                prm_NotesAndDocumentInfoAttachmentDetail.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                prm_NotesAndDocumentInfoAttachmentDetail.EUser = c.EUser;
                prm_NotesAndDocumentInfoAttachmentDetail.EDate = c.EDate;

                prm_NotesAndDocumentInfoAttachmentDetail.RefNo = c.RefNo;
                prm_NotesAndDocumentInfoAttachmentDetail.Date = Convert.ToDateTime(c.Date);
                prm_NotesAndDocumentInfoAttachmentDetail.Subject = c.Subject;
                prm_NotesAndDocumentInfoAttachmentDetail.Details = c.Details;
                prm_NotesAndDocumentInfoAttachmentDetail.IsAddAttachment = c.IsAddAttachment;
                prm_NotesAndDocumentInfoAttachmentDetail.FileName = c.FileName;

                prm_NotesAndDocumentInfoAttachmentDetail.Attachment = c.Attachment;
                if (pAddEdit)
                {
                    prm_NotesAndDocumentInfoAttachmentDetail.IUser = User.Identity.Name;
                    prm_NotesAndDocumentInfoAttachmentDetail.IDate = DateTime.Now;

                    entity.PRM_NotesAndDocumentInfoAttachmentDetail.Add(prm_NotesAndDocumentInfoAttachmentDetail);
                }
                else
                {
                    prm_NotesAndDocumentInfoAttachmentDetail.NotesAndDocumentInfoId = model.Id;
                    prm_NotesAndDocumentInfoAttachmentDetail.EUser = User.Identity.Name;
                    prm_NotesAndDocumentInfoAttachmentDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.Add(prm_NotesAndDocumentInfoAttachmentDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.Update(prm_NotesAndDocumentInfoAttachmentDetail);
                    }

                }
            }
            return entity;
        }

        private NotesAndDocumentInfoViewModel GetInsertUserAuditInfo(NotesAndDocumentInfoViewModel model)
        {
            model.IUser = User.Identity.Name;
            model.IDate = DateTime.Now;

            foreach (var child in model.NotesAndDocumentInfoAttachmentDetail)
            {
                child.IUser = User.Identity.Name;
                child.IDate = DateTime.Now;
            }

            return model;
        }

        #endregion


        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeName = obj.FullName,
                DesignationName = obj.PRM_Designation.Name,
                DepartmentName = obj.PRM_Division.Name,
                SectionName = obj.PRM_Section == null ? string.Empty : obj.PRM_Section.Name
            }, JsonRequestBehavior.AllowGet);

        }


        [NoCache]
        public ActionResult DesignationListView()
        {
            var list = Common.PopulateEmployeeDesignationDDL(_prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }


        #region Attachment
        public ActionResult DownloadFile(int Id)
        {
            var entity = _prmCommonService.PRMUnit.NotesAndDocumentInfoAttachmentDetailRepository.GetByID(Id);
            var model = entity.ToModel();
            DownloadDoc(model);
            model.IsAddAttachment = true;
            return View(model);
        }
        public void DownloadDoc(NotesAndDocumentInfoAttachmentDetailViewModel model)
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

        [HttpPost]
        public ActionResult AddAttachemnt([Bind(Exclude = "Attachment")] NotesAndDocumentInfoViewModel model)
        {
            HttpFileCollectionBase files = Request.Files;
            string name = string.Empty;
            byte[] fileData = null;

            foreach (string fileTagName in files)
            {

                // byte[] fileData = null;
                HttpPostedFileBase file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.
                    int size = file.ContentLength;
                    name = file.FileName;
                    int position = name.LastIndexOf("\\");
                    name = name.Substring(position + 1);
                    string contentType = file.ContentType;
                    fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);
                    //entity.FileName = name;
                    //entity.Attachment = fileData;
                }
            }

            List<NotesAndDocumentInfoViewModel> list = new List<NotesAndDocumentInfoViewModel>();

            var attList = Session["attachmentList"] as List<NotesAndDocumentInfoViewModel>;

            var obj = new NotesAndDocumentInfoViewModel
            {
                NotesAndDocumentInfoId = model.NotesAndDocumentInfoId,
                RefNo = model.RefNo,
                Date = Convert.ToDateTime(model.Date),
                Subject = model.Subject,
                Details = model.Details,
                IsAddAttachment = model.IsAddAttachment,
                FileName = name,
                Attachment = fileData
            };
            list.Add(obj);
            model.TempAttachmentDetail = list;
            attList = list;


            return PartialView("_Details", model);
        }
    }


}