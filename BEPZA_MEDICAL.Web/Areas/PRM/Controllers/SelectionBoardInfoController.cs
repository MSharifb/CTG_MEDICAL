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
    public class SelectionBoardInfoController : BaseController
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public SelectionBoardInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonservice = prmCommonService;
        }
        #endregion

        #region Actions

        //
        // GET: /PRM/SelectionBoardInfo/
        public ActionResult Index()
        {
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SelectionBoardInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<SelectionBoardInfoViewModel> list = (from selBoard in _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.GetAll()
                                                      where (selBoard.ZoneInfoId == LoggedUserZoneInfoId)
                                                      select new SelectionBoardInfoViewModel()
                                                      {
                                                          Id = selBoard.Id,
                                                          CommitteeName = selBoard.CommitteeName,
                                                          EffectiveFromDate = selBoard.EffectiveFromDate,
                                                          EffectiveToDate = selBoard.EffectiveToDate,
                                                          EffectDateView = selBoard.IsContinuous.ToString() == "True" ? "Continue" : Convert.ToDateTime(selBoard.EffectiveToDate).ToString("dd-MM-yyyy")
                                                      }).OrderBy(x => x.EffectiveFromDate).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.CommitteeName))
                {
                    list = list.Where(x => x.CommitteeName.Trim().ToLower().Contains(viewModel.CommitteeName.Trim().ToLower())).ToList();
                }

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

            if (request.SortingName == "CommitteeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CommitteeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CommitteeName).ToList();
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
                    d.CommitteeName,
                    Convert.ToDateTime(d.EffectiveFromDate).ToString("dd-MM-yyyy"),
                    Convert.ToDateTime(d.EffectiveToDate).ToString("dd-MM-yyyy"),
                    d.EffectDateView,                               
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Create()
        {
            SelectionBoardInfoViewModel model = new SelectionBoardInfoViewModel();
            model.IsIndividual = false;
            model.ActiveStatus = true;
            model.IsAddAttachment = true;
            populateDropdown(model);
            model.strMode = "Create";
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] SelectionBoardInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    //check Duplicate ClubName
                    var item = _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.Get(q => q.CommitteeName.Trim() == model.CommitteeName.Trim()).ToList();
                    if (item.Count() > 0)
                    {
                        model.ErrMsg = "Duplicate Commnittee Name.";
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }

                    model = GetInsertUserAuditInfo(model, true);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    //var entity = CreateEntity(model, true);
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
                        _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.Add(entity);
                        _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

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
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }
            populateDropdown(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id, string type)
        {
            var entity = _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.GetByID(id);
            var model = entity.ToModel();
            DownloadDoc(model);
            model.IsAddAttachment = true;
            model.strMode = "Edit";
            model.JobAdvertisementCode = entity.PRM_JobAdvertisementInfo.AdCode;

            //Job post information          
            var jobPostList = (from selBoard in _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.GetAll()
                               join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on selBoard.JobAdvertisementInfoId equals jobAd.Id
                               join jobAdReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll() on jobAd.Id equals jobAdReq.JobAdvertisementInfoId

                               where (selBoard.Id == id)
                               select new SelectionBoardInfoViewModel()
                               {
                                   DesignationId = jobAdReq.DesignationId,
                                   DesignationName = jobAdReq.PRM_Designation.Name,
                                   DepartmentId = jobAdReq.DepartmentId,
                                   DepartmentName = jobAdReq.PRM_Division.Name,
                                   SectionId = jobAdReq.PRM_Section == null ? null : jobAdReq.SectionId,
                                   SectionName = jobAdReq.PRM_Section == null ? string.Empty : jobAdReq.PRM_Section.Name
                               }).ToList();

            model.SelectionBoardInfo = jobPostList;




            if (model.IsIndividual)
            {
                //selectin board committee member
                List<SelectionBoardInfoCommitteeViewModel> listCommittee = (from selBoardCommitt in _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.GetAll()
                                                                            join selBoard in _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.GetAll() on selBoardCommitt.SelectionBoardInfoId equals selBoard.Id
                                                                            where (selBoard.Id == id)
                                                                            select new SelectionBoardInfoCommitteeViewModel()
                                                                            {
                                                                                Id = selBoardCommitt.Id,
                                                                                IsExternal = selBoardCommitt.IsExternal,
                                                                                SelectionBoardInfoId = selBoard.Id,
                                                                                DesignationId = selBoardCommitt.DesignationId,
                                                                                DesignationName = selBoardCommitt.PRM_Designation.Name,
                                                                                MemberEmployeeId = selBoardCommitt.MemberEmployeeId,
                                                                                MemberEmpId = selBoardCommitt.PRM_EmploymentInfo == null ? null : selBoardCommitt.PRM_EmploymentInfo.EmpID,
                                                                                MemberName = selBoardCommitt.PRM_EmploymentInfo == null ? selBoardCommitt.MemberName : selBoardCommitt.PRM_EmploymentInfo.FullName,
                                                                                MemberDesignation = selBoardCommitt.PRM_EmploymentInfo == null ? selBoardCommitt.MemberDesignation : selBoardCommitt.PRM_EmploymentInfo.PRM_Designation.Name,
                                                                                MemberRole = selBoardCommitt.MemberRole,
                                                                                ActiveStatus = selBoardCommitt.ActiveStatus,
                                                                                SortOrder = selBoardCommitt.SortOrder
                                                                            }).OrderBy(o => o.SortOrder).ToList();
                model.SelectionBoardInfoCommittee = listCommittee;
            }
            else
            {
                //selectin board committee member
                List<SelectionBoardInfoCommitteeViewModel> listCommittee = (from selBoardCommitt in _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.GetAll()
                                                                            join selBoard in _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.GetAll() on selBoardCommitt.SelectionBoardInfoId equals selBoard.Id
                                                                            where (selBoard.Id == id)
                                                                            select new SelectionBoardInfoCommitteeViewModel()
                                                                            {
                                                                                Id = selBoardCommitt.Id,
                                                                                IsExternal = selBoardCommitt.IsExternal,
                                                                                SelectionBoardInfoId = selBoard.Id,
                                                                                //DesignationId = selBoardCommitt.DesignationId,
                                                                                //DesignationName = selBoardCommitt.PRM_Designation.Name,
                                                                                MemberEmployeeId = selBoardCommitt.MemberEmployeeId,
                                                                                MemberEmpId = selBoardCommitt.PRM_EmploymentInfo == null ? null : selBoardCommitt.PRM_EmploymentInfo.EmpID,
                                                                                MemberName = selBoardCommitt.PRM_EmploymentInfo == null ? selBoardCommitt.MemberName : selBoardCommitt.PRM_EmploymentInfo.FullName,
                                                                                MemberDesignation = selBoardCommitt.PRM_EmploymentInfo == null ? selBoardCommitt.MemberDesignation : selBoardCommitt.PRM_EmploymentInfo.PRM_Designation.Name,
                                                                                MemberRole = selBoardCommitt.MemberRole,
                                                                                ActiveStatus = selBoardCommitt.ActiveStatus,
                                                                                SortOrder = selBoardCommitt.SortOrder
                                                                            }).OrderBy(o => o.SortOrder).ToList();
                model.SelectionBoardInfoCommittee = listCommittee;

            }

            populateDropdown(model);

            if (type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

            }
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Attachment")] SelectionBoardInfoViewModel model)
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
                    var obj = _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.GetByID(model.Id);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    //
                    model = GetInsertUserAuditInfo(model, false);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);
                    //var entity = model.ToEntity();
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
                        _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.Update(entity);
                        _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        return RedirectToAction("Edit", new { id = entity.Id, type = "success" });
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
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
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
                //  _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.Delete(id);
                List<Type> allTypes = new List<Type> { typeof(PRM_SelectionBoardInfoCommittee) };
                _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.Delete(id, allTypes);
                _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteSelectionCommittee")]
        public JsonResult DeleteSelectionCommitteefirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.Delete(id);
                _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.SaveChanges();
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

        #endregion

        #region Private Method
        private void populateDropdown(SelectionBoardInfoViewModel model)
        {
            dynamic ddlList;

            #region job advertisement
            DateTime cDate = DateTime.Now;
            ddlList = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.AdCode).ToList();
            model.JobAdvertisementInfoList = Common.PopulateJobAdvertisementDDL(ddlList);

            #endregion


        }

        private PRM_SelectionBoardInfo CreateEntity(SelectionBoardInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            if (model.IsIndividual)
            {
                foreach (var c in model.SelectionBoardInfoCommittee)
                {
                    var prm_SelectionBoardInfoCommittee = new PRM_SelectionBoardInfoCommittee();

                    //Check duplicate member Id
                    var list = (from selBoardCommitt in _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.GetAll()
                                join selBoard in _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.GetAll() on selBoardCommitt.SelectionBoardInfoId equals selBoard.Id
                                where (selBoardCommitt.MemberEmployeeId != null && selBoardCommitt.MemberEmployeeId == c.MemberEmployeeId && selBoardCommitt.Id != c.Id)
                                select selBoardCommitt).ToList();
                    var count = list.Count();
                    if (count == 0)
                    {
                        prm_SelectionBoardInfoCommittee.Id = c.Id;
                        prm_SelectionBoardInfoCommittee.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                        prm_SelectionBoardInfoCommittee.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                        prm_SelectionBoardInfoCommittee.EUser = c.EUser;
                        prm_SelectionBoardInfoCommittee.EDate = c.EDate;

                        prm_SelectionBoardInfoCommittee.IsExternal = c.IsExternal;
                        prm_SelectionBoardInfoCommittee.MemberEmployeeId = c.MemberEmployeeId;
                        if (c.MemberEmployeeId == null || c.MemberEmployeeId == 0)
                        {
                            prm_SelectionBoardInfoCommittee.MemberName = c.MemberName;
                            prm_SelectionBoardInfoCommittee.MemberDesignation = c.MemberDesignation;

                        }
                        else
                        {
                            prm_SelectionBoardInfoCommittee.MemberName = null;
                            prm_SelectionBoardInfoCommittee.MemberDesignation = null;
                        }

                        prm_SelectionBoardInfoCommittee.MemberRole = c.MemberRole;
                        prm_SelectionBoardInfoCommittee.ActiveStatus = c.ActiveStatus;
                        prm_SelectionBoardInfoCommittee.SortOrder = c.SortOrder;
                        prm_SelectionBoardInfoCommittee.DesignationId = c.DesignationId;

                        prm_SelectionBoardInfoCommittee.SelectionBoardInfoId = model.Id;
                        prm_SelectionBoardInfoCommittee.EUser = User.Identity.Name;
                        prm_SelectionBoardInfoCommittee.EDate = DateTime.Now;
                        if (c.Id == 0)
                        {
                            _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.Add(prm_SelectionBoardInfoCommittee);
                            _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.SaveChanges();
                        }

                        //if (pAddEdit)
                        //{
                        //    prm_SelectionBoardInfoCommittee.IUser = User.Identity.Name;
                        //    prm_SelectionBoardInfoCommittee.IDate = DateTime.Now;
                        //    entity.PRM_SelectionBoardInfoCommittee.Add(prm_SelectionBoardInfoCommittee);
                        //}
                        //else
                        //{
                        //    prm_SelectionBoardInfoCommittee.SelectionBoardInfoId = model.Id;
                        //    prm_SelectionBoardInfoCommittee.EUser = User.Identity.Name;
                        //    prm_SelectionBoardInfoCommittee.EDate = DateTime.Now;

                        //    if (c.Id == 0)
                        //    {
                        //        _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.Add(prm_SelectionBoardInfoCommittee);
                        //        _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.SaveChanges();
                        //    }
                        //    else
                        //    {
                        //        _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.Update(prm_SelectionBoardInfoCommittee);
                        //    }                           
                        //}
                        
                    }

                    else
                    {
                        model.ErrMsg = "Duplicate Entry";
                    }
                }

            }
            else
            {
                foreach (var c in model.SelectionBoardInfoCommittee)
                {

                    var obj = new PRM_SelectionBoardInfoCommittee();

                    //Check duplicate member Id
                    var list = (from selBoardCommitt in _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.GetAll()
                                join selBoard in _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.GetAll() on selBoardCommitt.SelectionBoardInfoId equals selBoard.Id
                                where (selBoardCommitt.MemberEmployeeId != null && selBoardCommitt.MemberEmployeeId == c.MemberEmployeeId && selBoardCommitt.Id != c.Id)
                                select selBoardCommitt).ToList();
                    var count = list.Count();
                    if (count == 0)
                    {
                        obj.Id = c.Id;
                        obj.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                        obj.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                        obj.EUser = c.EUser;
                        obj.EDate = c.EDate;

                        obj.IsExternal = c.IsExternal;
                        obj.MemberEmployeeId = c.MemberEmployeeId;
                        if (c.MemberEmployeeId == null || c.MemberEmployeeId == 0)
                        {
                            obj.MemberName = c.MemberName;
                            obj.MemberDesignation = c.MemberDesignation;
                        }
                        else
                        {
                            obj.MemberName = null;
                            obj.MemberDesignation = null;
                        }
                        obj.MemberRole = c.MemberRole;
                        obj.ActiveStatus = c.ActiveStatus;
                        obj.SortOrder = c.SortOrder;
                        obj.DesignationId = null;

                        obj.SelectionBoardInfoId = model.Id;
                        obj.EUser = User.Identity.Name;
                        obj.EDate = DateTime.Now;
                        if (c.Id == 0)
                        {
                            _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.Add(obj);
                            _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.SaveChanges();
                        }

                        //if (pAddEdit)
                        //{
                        //    obj.IUser = User.Identity.Name;
                        //    obj.IDate = DateTime.Now;
                        //    _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.Add(obj);
                        //}
                        //else
                        //{
                        //    obj.SelectionBoardInfoId = model.Id;
                        //    obj.EUser = User.Identity.Name;
                        //    obj.EDate = DateTime.Now;
                        //    if (c.Id == 0)
                        //    {
                        //        _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.Add(obj);
                        //        _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.SaveChanges();
                        //    }
                        //    else
                        //    {
                        //        _prmCommonservice.PRMUnit.SelectionBoardInfoCommitteeRepository.Update(obj);
                        //    }
                           
                        //}

                    }

                    else
                    {
                        model.ErrMsg = "Duplicate Entry";
                    }
                }
            }
            return entity;
        }


        private SelectionBoardInfoViewModel GetInsertUserAuditInfo(SelectionBoardInfoViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                foreach (var child in model.SelectionBoardInfoCommittee)
                {
                    child.IUser = User.Identity.Name;
                    child.IDate = DateTime.Now;
                }

            }
            else
            {

                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                foreach (var child in model.SelectionBoardInfoCommittee)
                {
                    child.IUser = model.IUser;
                    child.IDate = model.IDate;

                    child.EUser = User.Identity.Name;
                    child.EDate = DateTime.Now;
                }
            }

            return model;
        }


        private bool CheckDuplicateEntry(SelectionBoardInfoViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.Get(q => q.Id == model.Id && q.CommitteeName == model.CommitteeName).Any();
            }

            else
            {
                return _prmCommonservice.PRMUnit.SelectionBoardInfoRepository.Get(q => q.Id == model.Id && q.CommitteeName == model.CommitteeName && strMode != q.Id).Any();
            }
        }

        #endregion

        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var obj = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                Designation = obj.PRM_Designation.Name,
                EmployeeName = obj.FullName
            });

        }

        [HttpPost]
        public ActionResult AddSelectionBoardCommittee(SelectionBoardInfoViewModel model)
        {
            //HttpFileCollectionBase files = Request.Files;
            //string name = string.Empty;
            //byte[] fileData = null;

            //foreach (string fileTagName in files)
            //{

            //    // byte[] fileData = null;
            //    HttpPostedFileBase file = Request.Files[fileTagName];
            //    if (file.ContentLength > 0)
            //    {
            //        // Due to the limit of the max for a int type, the largest file can be
            //        // uploaded is 2147483647, which is very large anyway.
            //        int size = file.ContentLength;
            //        name = file.FileName;
            //        int position = name.LastIndexOf("\\");
            //        name = name.Substring(position + 1);
            //        string contentType = file.ContentType;
            //        fileData = new byte[size];
            //        file.InputStream.Read(fileData, 0, size);
            //        //entity.FileName = name;
            //        //entity.Attachment = fileData;
            //    }
            //}

            List<SelectionBoardInfoCommitteeViewModel> list = new List<SelectionBoardInfoCommitteeViewModel>();

            var obj = new SelectionBoardInfoCommitteeViewModel
            {
                SelectionBoardInfoId = model.Id,
                DesignationId = model.DesignationId,
                DesignationName=model.DesignationName,
                IsExternal = model.IsExternal,
                MemberEmployeeId = model.MemberEmployeeId,
                MemberEmpId = model.MemberEmpId,
                MemberName = model.MemberName,
                MemberDesignation = model.MemberDesignation,
                MemberRole = model.MemberRole,
                ActiveStatus = model.ActiveStatus,
                SortOrder = Convert.ToInt32(model.SortOrder)
            };
            list.Add(obj);
            model.SelectionBoardInfoCommittee = list;


            return PartialView("_Details", model);
        }


        #region Attachment

        private int Upload(SelectionBoardInfoViewModel model)
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

        public void DownloadDoc(SelectionBoardInfoViewModel model)
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

        #region Other Actions
        //get jobpost by job advertisement id
        [HttpGet]
        public PartialViewResult GetJobPost(int jobAdId)
        {
            List<SelectionBoardInfoViewModel> list = new List<SelectionBoardInfoViewModel>();
            list = (from jobAdDtlReq in _prmCommonservice.PRMUnit.JobAdvertisementInfoDetailRequisitionRepository.GetAll()
                    join jobAd in _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetAll() on jobAdDtlReq.JobAdvertisementInfoId equals jobAd.Id
                    where (jobAd.Id == jobAdId)
                    select new SelectionBoardInfoViewModel
                    {
                        IsChecked = true,
                        DesignationId = jobAdDtlReq.DesignationId,
                        DesignationName = jobAdDtlReq.PRM_Designation.Name,
                        DepartmentId = jobAdDtlReq.DepartmentId,
                        DepartmentName = jobAdDtlReq.PRM_Division.Name,
                        SectionId = jobAdDtlReq.PRM_Section == null ? null : jobAdDtlReq.SectionId,
                        SectionName = jobAdDtlReq.PRM_Section == null ? string.Empty : jobAdDtlReq.PRM_Section.Name
                        // NoOfPost = jobAdDtlReq.NumberOfClearancePosition
                    }).ToList();
            return PartialView("_JobPost", new SelectionBoardInfoViewModel { SelectionBoardInfo = list });

        }
        #endregion

    }
}