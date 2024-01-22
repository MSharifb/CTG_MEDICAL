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
    public class InvestigationCommitteeInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor
        public InvestigationCommitteeInfoController(PRMCommonSevice prmCommonService)
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
        public ActionResult GetList(JqGridRequest request, InvestigationCommitteeInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<InvestigationCommitteeInfoViewModel> list = (from invesComttInfo in _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.GetAll()
                                                              where (invesComttInfo.ZoneInfoId == LoggedUserZoneInfoId)
                                                             && (viewModel.RefNo == null || viewModel.RefNo == "" || invesComttInfo.RefNo == viewModel.RefNo)
                                                              select new InvestigationCommitteeInfoViewModel()
                                                      {
                                                          Id = invesComttInfo.Id,
                                                          RefNo = invesComttInfo.RefNo,
                                                          EffectiveFromDate = invesComttInfo.EffectiveFromDate,
                                                          EffectiveToDate = invesComttInfo.EffectiveToDate,
                                                          EffectDateView = invesComttInfo.IsContinuous.ToString() == "True" ? "Continue" : Convert.ToDateTime(invesComttInfo.EffectiveToDate).ToString("dd-MM-yyyy")
                                                      }).OrderBy(x => x.RefNo).ToList();

            if (request.Searching)
            {
                if ((viewModel.EffectiveFromDate != null && viewModel.EffectiveFromDate != DateTime.MinValue) && (viewModel.EffectiveToDate != null && viewModel.EffectiveToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveFromDate >= viewModel.EffectiveFromDate && d.EffectiveToDate <= viewModel.EffectiveToDate).ToList();
                }

                if ((viewModel.EffectiveFromDate != null && viewModel.EffectiveFromDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveFromDate >= viewModel.EffectiveFromDate).ToList();
                }

                if ((viewModel.EffectiveToDate != null && viewModel.EffectiveToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveToDate <= viewModel.EffectiveToDate || d.EffectiveToDate == null).ToList();
                }
            }
            totalRecords = list == null ? 0 : list.Count;


            #region Sorting

            if (request.SortingName == "RefNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RefNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RefNo).ToList();
                }
            }

            if (request.SortingName == "EffectiveFromDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveFromDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveFromDate).ToList();
                }
            }

            if (request.SortingName == "EffectDateView")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectDateView).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectDateView).ToList();
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
                    d.RefNo,
                    Convert.ToDateTime(d.EffectiveFromDate).ToString(DateAndTime.GlobalDateFormat),
                    Convert.ToDateTime(d.EffectiveToDate).ToString(DateAndTime.GlobalDateFormat),   
                    d.EffectDateView,        
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            InvestigationCommitteeInfoViewModel model = new InvestigationCommitteeInfoViewModel();
            populateDropdown(model);
            model.IsAddAttachment = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] InvestigationCommitteeInfoViewModel model)
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
                        _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        //   return RedirectToAction("Index");
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
                model.errClass = "failed";
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
            var entity = _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.GetByID(id);
            var model = entity.ToModel();

            model.CommitteeFormedByEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.CommitteeFormedByEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.CommitteeFormedByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;


            //Investigation Committee Info Member Info
            List<InvestigationCommitteeInfoMemberInfoViewModel> resultFrm = (from invesComttInfoMember in _prmCommonService.PRMUnit.InvestigationCommitteeInfoMemberInfoRepository.GetAll()
                                                                             join invesComttInfo in _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.GetAll() on invesComttInfoMember.InvestigationCommitteeInfoId equals invesComttInfo.Id
                                                                             //join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on invesComttInfoMember.MemberEmployeeId equals emp.Id
                                                                             where (invesComttInfo.Id == id)
                                                                             select new InvestigationCommitteeInfoMemberInfoViewModel()
                                                                    {
                                                                        Id = invesComttInfoMember.Id,
                                                                        InvestigationCommitteeInfoId = invesComttInfoMember.InvestigationCommitteeInfoId,
                                                                        IsExternal = invesComttInfoMember.IsExternal,
                                                                        ActiveStatus = invesComttInfoMember.ActiveStatus,
                                                                        MemberEmployeeId = invesComttInfoMember.MemberEmployeeId,
                                                                        MemberRole = invesComttInfoMember.MemberRole,
                                                                        MemberRemarks = invesComttInfoMember.MemberRemarks,
                                                                        MemberEmpId = invesComttInfoMember.PRM_EmploymentInfo == null ? null : invesComttInfoMember.PRM_EmploymentInfo.EmpID,
                                                                        MemberName = invesComttInfoMember.PRM_EmploymentInfo == null ? invesComttInfoMember.MemberName : invesComttInfoMember.PRM_EmploymentInfo.FullName,
                                                                        MemberDesignation = invesComttInfoMember.PRM_EmploymentInfo == null ? invesComttInfoMember.MemberDesignation : invesComttInfoMember.PRM_EmploymentInfo.PRM_Designation.Name

                                                                    }).ToList();
            model.InvestigationCommitteeInfoMemberInfoList = resultFrm;



            List<InvestigationCommitteeInfoFormedForViewModel> list = (from invesComttInfoFormed in _prmCommonService.PRMUnit.InvestigationCommitteeInfoFormedForRepository.GetAll()
                                                                       join invesComttInfo in _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.GetAll() on invesComttInfoFormed.InvestigationCommitteeInfoId equals invesComttInfo.Id
                                                                       where (invesComttInfo.Id == id)
                                                                       select new InvestigationCommitteeInfoFormedForViewModel()
                                                                        {
                                                                            Id = invesComttInfoFormed.Id,
                                                                            InvestigationCommitteeInfoId = invesComttInfoFormed.InvestigationCommitteeInfoId,
                                                                            ComplaintNoteSheetId = invesComttInfoFormed.ComplaintNoteSheetId,
                                                                            ComplaintNoteSheetName = invesComttInfoFormed.PRM_ComplaintNoteSheet.DeptProceedingNo,
                                                                        }).ToList();
            model.InvestigationCommitteeInfoFormedForList = list;

            populateDropdown(model);
            DownloadDoc(model);
            model.IsAddAttachment = true;

            if (type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

            }
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit([Bind(Exclude = "Attachment")] InvestigationCommitteeInfoViewModel model)
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
                    var obj = _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.GetByID(model.Id);
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
                        _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        return RedirectToAction("Edit", new { id = entity.Id, type = "success" });
                        //return RedirectToAction("Index");

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
                model.errClass = "failed";
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
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
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
               // _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.Delete(id);
                
                List<Type> allTypes = new List<Type> { typeof(PRM_InvestigationCommitteeInfoMemberInfo) };
                allTypes.Add(typeof(PRM_InvestigationCommitteeInfoFormedFor));
                _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.Delete(id, allTypes);
                _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteInvestigationCommitteeMember")]
        public JsonResult DeleteInvestigationCommitteeMemberConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.InvestigationCommitteeInfoMemberInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.InvestigationCommitteeInfoMemberInfoRepository.SaveChanges();
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
                _prmCommonService.PRMUnit.InvestigationCommitteeInfoFormedForRepository.Delete(id);
                _prmCommonService.PRMUnit.InvestigationCommitteeInfoFormedForRepository.SaveChanges();
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
        private void populateDropdown(InvestigationCommitteeInfoViewModel model)
        {
            dynamic ddlList;
            //use as Departmental Proceedigs No.
            #region Complaint/Note Sheet

            ddlList = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.DeptProceedingNo).ToList();
            model.ComplaintNoteSheetList = Common.PopulateComplaintNoteSheet(ddlList);

            #endregion
        }

        #endregion

        //check duplicate hearing ref no
        private bool CheckDuplicateEntry(InvestigationCommitteeInfoViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.Get(q => q.RefNo == model.RefNo).Any();
            }

            else
            {
                var ss = _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.Get(q => q.RefNo == model.RefNo && strMode != q.Id).Any();
                return _prmCommonService.PRMUnit.InvestigationCommitteeInfoRepository.Get(q => q.RefNo == model.RefNo && strMode != q.Id).Any();
            }
        }

        private PRM_InvestigationCommitteeInfo CreateEntity(InvestigationCommitteeInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            #region Investigation Committee Info Member Info

            foreach (var c in model.InvestigationCommitteeInfoMemberInfoList)
            {
                var prm_InvestigationCommitteeInfoMemberInfo = new PRM_InvestigationCommitteeInfoMemberInfo();

                prm_InvestigationCommitteeInfoMemberInfo.Id = c.Id;
                prm_InvestigationCommitteeInfoMemberInfo.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                prm_InvestigationCommitteeInfoMemberInfo.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                prm_InvestigationCommitteeInfoMemberInfo.EUser = c.EUser;
                prm_InvestigationCommitteeInfoMemberInfo.EDate = c.EDate;

                prm_InvestigationCommitteeInfoMemberInfo.IsExternal = c.IsExternal;
                prm_InvestigationCommitteeInfoMemberInfo.ActiveStatus = c.ActiveStatus;
                if (c.MemberEmployeeId == null || c.MemberEmployeeId == 0)
                {
                    prm_InvestigationCommitteeInfoMemberInfo.MemberEmployeeId = null;
                    prm_InvestigationCommitteeInfoMemberInfo.MemberName = c.MemberName;
                    prm_InvestigationCommitteeInfoMemberInfo.MemberDesignation = c.MemberDesignation;

                }
                else
                {
                    prm_InvestigationCommitteeInfoMemberInfo.MemberEmployeeId = Convert.ToInt32(c.MemberEmployeeId);
                    prm_InvestigationCommitteeInfoMemberInfo.MemberName = null;
                    prm_InvestigationCommitteeInfoMemberInfo.MemberDesignation = null;
                }

                prm_InvestigationCommitteeInfoMemberInfo.MemberRole = c.MemberRole;
                prm_InvestigationCommitteeInfoMemberInfo.MemberRemarks = c.MemberRemarks;

                if (pAddEdit)
                {
                    prm_InvestigationCommitteeInfoMemberInfo.IUser = User.Identity.Name;
                    prm_InvestigationCommitteeInfoMemberInfo.IDate = DateTime.Now;

                    entity.PRM_InvestigationCommitteeInfoMemberInfo.Add(prm_InvestigationCommitteeInfoMemberInfo);
                }
                else
                {
                    prm_InvestigationCommitteeInfoMemberInfo.InvestigationCommitteeInfoId = model.Id;
                    prm_InvestigationCommitteeInfoMemberInfo.EUser = User.Identity.Name;
                    prm_InvestigationCommitteeInfoMemberInfo.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.InvestigationCommitteeInfoMemberInfoRepository.Add(prm_InvestigationCommitteeInfoMemberInfo);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.InvestigationCommitteeInfoMemberInfoRepository.Update(prm_InvestigationCommitteeInfoMemberInfo);
                    }

                }
            }

            #endregion

            #region Investigation Committee Info Formed For

            foreach (var c in model.InvestigationCommitteeInfoFormedForList)
            {
                var prm_InvestigationCommitteeInfoFormedFor = new PRM_InvestigationCommitteeInfoFormedFor();

                prm_InvestigationCommitteeInfoFormedFor.Id = c.Id;
                prm_InvestigationCommitteeInfoFormedFor.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                prm_InvestigationCommitteeInfoFormedFor.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                prm_InvestigationCommitteeInfoFormedFor.EUser = c.EUser;
                prm_InvestigationCommitteeInfoFormedFor.EDate = c.EDate;

                prm_InvestigationCommitteeInfoFormedFor.ComplaintNoteSheetId = c.ComplaintNoteSheetId;
                prm_InvestigationCommitteeInfoFormedFor.Remarks = c.Remarks;

                if (pAddEdit)
                {
                    prm_InvestigationCommitteeInfoFormedFor.IUser = User.Identity.Name;
                    prm_InvestigationCommitteeInfoFormedFor.IDate = DateTime.Now;

                    entity.PRM_InvestigationCommitteeInfoFormedFor.Add(prm_InvestigationCommitteeInfoFormedFor);
                }
                else
                {
                    prm_InvestigationCommitteeInfoFormedFor.InvestigationCommitteeInfoId = model.Id;
                    prm_InvestigationCommitteeInfoFormedFor.EUser = User.Identity.Name;
                    prm_InvestigationCommitteeInfoFormedFor.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.InvestigationCommitteeInfoFormedForRepository.Add(prm_InvestigationCommitteeInfoFormedFor);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.InvestigationCommitteeInfoFormedForRepository.Update(prm_InvestigationCommitteeInfoFormedFor);
                    }

                }
            }

            #endregion
            return entity;
        }

        private InvestigationCommitteeInfoViewModel GetInsertUserAuditInfo(InvestigationCommitteeInfoViewModel model, bool pAddEdit)
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

        private int Upload(InvestigationCommitteeInfoViewModel model)
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

        public void DownloadDoc(InvestigationCommitteeInfoViewModel model)
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

        
        //search 
        [NoCache]
        public ActionResult DeptProceedingListView()
        {
            var list = Common.PopulateComplaintNoteSheet(_prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.DeptProceedingNo).ToList());
            return PartialView("Select", list);
        }

    }
}