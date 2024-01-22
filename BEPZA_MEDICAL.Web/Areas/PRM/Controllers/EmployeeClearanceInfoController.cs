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
    public class EmployeeClearanceInfoController : BaseController
    {

        #region Fields

        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public EmployeeClearanceInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonservice = prmCommonService;
        }
        #endregion


        //
        // GET: /PRM/EmployeeClearanceInfo/
        public ActionResult Index()
        {
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EmpClearanceInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<EmpClearanceInfoViewModel> list = (from tr in _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.GetAll()
                                                    join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on tr.EmployeeId equals emp.Id
                                                    where (viewModel.EmpId == null || viewModel.EmpId == "" || viewModel.EmpId == emp.EmpID)
                                                    && (viewModel.DesignationId == null || viewModel.DesignationId == 0 || viewModel.DesignationId == emp.DesignationId)
                                                   && (viewModel.EmployeeName == null || viewModel.EmployeeName == "" || emp.FullName.Contains(viewModel.EmployeeName))
                                                   && (tr.ZoneInfoId == LoggedUserZoneInfoId)
                                                    select new EmpClearanceInfoViewModel()
                                                    {
                                                        Id = tr.Id,
                                                        EmployeeId = tr.EmployeeId,
                                                        EmpId = emp.EmpID,
                                                        EmployeeName = emp.FullName,
                                                        DesignationId = emp.DesignationId,
                                                        Designation = emp.PRM_Designation.Name
                                                    }).OrderBy(x => x.EmployeeId).ToList();


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
                    list = list.OrderBy(x => x.Designation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Designation).ToList();
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
                    d.Designation,             
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetListClearanceForm(JqGridRequest request, int Id)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            List<EmpClearanceInfoFormDetailsViewModel> list = (from empClearanceFrm in _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.GetAll()
                                                               join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on empClearanceFrm.ResponsibleEmployeeId equals emp.Id
                                                               where (empClearanceFrm.EmpClearanceInfoId == Id)
                                                               select new EmpClearanceInfoFormDetailsViewModel()
                                                                {
                                                                    Id = empClearanceFrm.Id,
                                                                    EmpClearanceInfoId = Id,
                                                                    ClearanceFormId = empClearanceFrm.ClearanceFormId,
                                                                    ClearanceFormName = empClearanceFrm.PRM_ClearanceForm.Name,
                                                                    ClearanceDate = Convert.ToDateTime(empClearanceFrm.ClearanceDate),
                                                                    ResponsibleEmployeeId = emp.Id,
                                                                    ResponsibleEmployeeName = emp.FullName,
                                                                    ResponsibleEmployeeDesignation = emp.PRM_Designation.Name
                                                                }).OrderBy(x => x.Id).ToList();


            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EmpClearanceInfoId,
                    d.ClearanceFormId,
                    d.ClearanceFormName,
                    d.ClearanceDate.ToString("dd-MM-yyyy"),                    
                    d.ResponsibleEmployeeId,
                    d.ResponsibleEmployeeName,
                    d.ResponsibleEmployeeDesignation,
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            EmpClearanceInfoViewModel model = new EmpClearanceInfoViewModel();
            model.isAddAttachment = true;
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] EmpClearanceInfoViewModel model)
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
                        _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.Add(entity);
                        _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        //  return RedirectToAction("Index");
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
            setDafaultData(model);
            return View(model);
        }

        private void setDafaultData(EmpClearanceInfoViewModel model) 
        {

            var list = model.EmpClearanceFormDetails;

            //List<EmpClearanceInfoFormDetailsViewModel> resultFrm = (from empClearanceFrmDe in _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.GetAll()
            //                                                        join empClearanceInfo in _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.GetAll() on empClearanceFrmDe.EmpClearanceInfoId equals empClearanceInfo.Id
            //                                                        where (empClearanceInfo.Id == model.Id)
            //                                                        select new EmpClearanceInfoFormDetailsViewModel()
            //                                                        {
            //                                                            Id = empClearanceFrmDe.Id,
            //                                                            EmpClearanceInfoId = empClearanceInfo.Id,
            //                                                            ClearanceFormId = empClearanceFrmDe.ClearanceFormId,
            //                                                            ClearanceFormName = empClearanceFrmDe.PRM_ClearanceForm.Name,
            //                                                            ClearanceDate = Convert.ToDateTime(empClearanceFrmDe.ClearanceDate),
            //                                                            ResponsibleEmployeeId = empClearanceFrmDe.ResponsibleEmployeeId,
            //                                                            ResponsibleEmployeeName = empClearanceFrmDe.PRM_EmploymentInfo.FullName,
            //                                                            ResponsibleEmployeeDesignation = empClearanceFrmDe.PRM_EmploymentInfo.PRM_Designation.Name
            //                                                        }).ToList();
            //model.EmpClearanceFormDetails = resultFrm;
        }

        [NoCache]
        public ActionResult Edit(int id, string type)
        {
            var entity = _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.GetByID(id);
            var model = entity.ToModel();
            DownloadDoc(model);
            model.isAddAttachment = true;

            model.EmpId = entity.PRM_EmploymentInfo.EmpID;
            model.EmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.Department = entity.PRM_EmploymentInfo.PRM_Division == null ? string.Empty : entity.PRM_EmploymentInfo.PRM_Division.Name;
            model.Designation = entity.PRM_EmploymentInfo.PRM_Designation == null ? string.Empty : entity.PRM_EmploymentInfo.PRM_Designation.Name;
            model.Section = entity.PRM_EmploymentInfo.PRM_Section == null ? string.Empty : entity.PRM_EmploymentInfo.PRM_Section.Name;


            //Employee Clearance Form Details

            List<EmpClearanceInfoFormDetailsViewModel> resultFrm = (from empClearanceFrmDe in _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.GetAll()
                                                                    join empClearanceInfo in _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.GetAll() on empClearanceFrmDe.EmpClearanceInfoId equals empClearanceInfo.Id
                                                                    where (empClearanceInfo.Id == id)
                                                                    select new EmpClearanceInfoFormDetailsViewModel()
                                                                    {
                                                                        Id = empClearanceFrmDe.Id,
                                                                        EmpClearanceInfoId = empClearanceInfo.Id,
                                                                        ClearanceFormId = empClearanceFrmDe.ClearanceFormId,
                                                                        ClearanceFormName = empClearanceFrmDe.PRM_ClearanceForm.Name,
                                                                        ClearanceDate = Convert.ToDateTime(empClearanceFrmDe.ClearanceDate),
                                                                        ResponsibleEmployeeId = empClearanceFrmDe.ResponsibleEmployeeId,
                                                                        ResponsibleEmployeeName = empClearanceFrmDe.PRM_EmploymentInfo.FullName,
                                                                        ResponsibleEmployeeDesignation = empClearanceFrmDe.PRM_EmploymentInfo.PRM_Designation.Name
                                                                    }).ToList();
            model.EmpClearanceFormDetails = resultFrm;
            populateDropdown(model);

            if (type == "success")
            {
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                model.errClass = "success";
            }
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Attachment")] EmpClearanceInfoViewModel model)
        {
            try
            {
                string errorList = "";
                var attachment = Request.Files["attachment"];
                if (ModelState.IsValid)
                {
                    // Set preious attachment if exist

                    var obj = _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.GetByID(model.Id);
                    model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                    //
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
                        _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.Update(entity);
                        _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.SaveChanges();
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
                    //   model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
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

        [NoCache]
        public ActionResult AddChecklist(int id)
        {
            var EmpClearanceFormDetailEntity = _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.GetByID(id);
            var parentModel = EmpClearanceFormDetailEntity.ToModel();
            parentModel.ClearanceFormName = EmpClearanceFormDetailEntity.PRM_ClearanceForm.Name;

            List<EmpClearanceInfoChecklistDetailsViewModel> resultChk = (from emlchearChk in _prmCommonservice.PRMUnit.EmpClearanceInfoChecklistDetailRepository.GetAll()
                                                                         join empClearanceFrmDe in _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.GetAll() on emlchearChk.EmpClearanceInfoFormDetailId equals empClearanceFrmDe.Id
                                                                         where (parentModel.Id == emlchearChk.EmpClearanceInfoFormDetailId)
                                                                         select new EmpClearanceInfoChecklistDetailsViewModel()
                                                                           {
                                                                               Id = emlchearChk.Id,
                                                                               EmpClearanceFormDetailId = empClearanceFrmDe.Id,
                                                                               ClearanceChecklistId = emlchearChk.ClearanceChecklistId,
                                                                               ClearanceName = emlchearChk.PRM_ClearanceChecklistDetail.Name,
                                                                               Status = emlchearChk.Status,
                                                                               CheckStatus = emlchearChk.Status.ToString() == "True" ? "Yes" : "No",
                                                                               Description = emlchearChk.Description
                                                                           }).ToList();

            parentModel.EmpChecklistDetails = resultChk;

            dynamic ddlList;
            ddlList = (from chkList in _prmCommonservice.PRMUnit.ClearanceChecklistRepository.GetAll()
                       join chkListDetail in _prmCommonservice.PRMUnit.ClearanceChecklistDetailRepository.GetAll() on chkList.Id equals chkListDetail.ClearanceChecklistId
                       where (EmpClearanceFormDetailEntity.ClearanceFormId == chkList.ClearanceFormId)
                       select new
                       {
                           Id = chkListDetail.Id,
                           Name = chkListDetail.Name,
                           ClearanceFormId = chkList.ClearanceFormId
                       }).ToList();
            parentModel.ClearanceCheckList = Common.PopulateDllList(ddlList);

            return View("AddCheckList", parentModel);
        }


        [HttpPost]
        public ActionResult AddChecklist(EmpClearanceInfoFormDetailsViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    var EmpClearanceFormDetailEntity = _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.GetByID(model.Id);
                    var parentModel = EmpClearanceFormDetailEntity.ToModel();

                    if (model.EmpChecklistDetails.Count > 0)
                    {
                        // Checklist
                        foreach (var c in model.EmpChecklistDetails)
                        {
                            var prm_EmpClearanceInfoCheklistDetail = new PRM_EmpClearanceInfoCheklistDetail();

                            prm_EmpClearanceInfoCheklistDetail.Id = c.Id;
                            prm_EmpClearanceInfoCheklistDetail.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                            prm_EmpClearanceInfoCheklistDetail.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                            prm_EmpClearanceInfoCheklistDetail.EUser = c.EUser;
                            prm_EmpClearanceInfoCheklistDetail.EDate = c.EDate;
                            prm_EmpClearanceInfoCheklistDetail.ClearanceChecklistId = c.ClearanceChecklistId;
                            prm_EmpClearanceInfoCheklistDetail.Status = c.Status;
                            prm_EmpClearanceInfoCheklistDetail.Description = c.Description;

                            prm_EmpClearanceInfoCheklistDetail.EmpClearanceInfoFormDetailId = model.Id;


                            if (c.Id == 0)
                            {
                                _prmCommonservice.PRMUnit.EmpClearanceInfoChecklistDetailRepository.Add(prm_EmpClearanceInfoCheklistDetail);
                            }
                            else
                            {
                                _prmCommonservice.PRMUnit.EmpClearanceInfoChecklistDetailRepository.Update(prm_EmpClearanceInfoCheklistDetail);
                            }
                        }
                        if (errorList.Length == 0)
                        {
                            _prmCommonservice.PRMUnit.EmpClearanceInfoChecklistDetailRepository.SaveChanges();
                            model.errClass = "success";
                            model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        }
                    }
                    //return RedirectToAction("Index");
                    return RedirectToAction("Edit", "EmployeeClearanceInfo", new { id = EmpClearanceFormDetailEntity.EmpClearanceInfoId });

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

            return View(model);

        }


        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                //_prmCommonservice.PRMUnit.EmpClearanceInfoRepository.Delete(id);
                //_prmCommonservice.PRMUnit.EmpClearanceInfoRepository.SaveChanges();
                List<Type> allTypes = new List<Type> { typeof(PRM_EmpClearanceInfoFormDetail) };
                _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.Delete(id, allTypes);
                _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteEmpClearanceFormDetail")]
        public JsonResult DeleteEmpClearanceFormDetailfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.Delete(id);
                _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteEmpClearanceChecklistDetail")]
        public JsonResult DeleteEmpClearanceChecklistfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonservice.PRMUnit.EmpClearanceInfoChecklistDetailRepository.Delete(id);
                _prmCommonservice.PRMUnit.EmpClearanceInfoChecklistDetailRepository.SaveChanges();
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

        #region Private Method
        private void populateDropdown(EmpClearanceInfoViewModel model)
        {
            dynamic ddlList;
            #region Clearance Form ddl

            ddlList = _prmCommonservice.PRMUnit.ClearanceFormRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.ClearanceFormList = Common.PopulateDllList(ddlList);
            #endregion

        }

        private PRM_EmpClearanceInfo CreateEntity(EmpClearanceInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.EmpClearanceFormDetails)
            {
                var prm_EmpClearanceFormDetail = new PRM_EmpClearanceInfoFormDetail();

                //Check duplicate Clearance Form
                var list = (from empClearForm in _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.GetAll()
                            join empClearInfo in _prmCommonservice.PRMUnit.EmpClearanceInfoRepository.GetAll() on empClearForm.EmpClearanceInfoId equals empClearInfo.Id
                            where (empClearInfo.EmployeeId == model.EmployeeId && empClearForm.ClearanceFormId == c.ClearanceFormId)
                            select empClearForm).ToList();
                var count = list.Count();
                //End Check duplicate Chekclist Name

                if (count == 0)
                {
                    prm_EmpClearanceFormDetail.Id = c.Id;
                    prm_EmpClearanceFormDetail.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                    prm_EmpClearanceFormDetail.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                    prm_EmpClearanceFormDetail.EUser = c.EUser;
                    prm_EmpClearanceFormDetail.EDate = c.EDate;

                    prm_EmpClearanceFormDetail.ClearanceFormId = c.ClearanceFormId;
                    prm_EmpClearanceFormDetail.ClearanceDate = c.ClearanceDate;
                    prm_EmpClearanceFormDetail.ResponsibleEmployeeId = c.ResponsibleEmployeeId;

                    if (pAddEdit)
                    {
                        prm_EmpClearanceFormDetail.IUser = User.Identity.Name;
                        prm_EmpClearanceFormDetail.IDate = DateTime.Now;

                        entity.PRM_EmpClearanceInfoFormDetail.Add(prm_EmpClearanceFormDetail);
                    }
                    else
                    {
                        prm_EmpClearanceFormDetail.EmpClearanceInfoId = model.Id;
                        prm_EmpClearanceFormDetail.EUser = User.Identity.Name;
                        prm_EmpClearanceFormDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.Add(prm_EmpClearanceFormDetail);
                        }
                        else
                        {
                            _prmCommonservice.PRMUnit.EmpClearanceInfoFormDetailRepository.Update(prm_EmpClearanceFormDetail);
                        }

                    }


                }

                else
                {
                    model.ErrMsg = "Duplicate Entry";
                }
            }
            return entity;
        }


        private EmpClearanceInfoViewModel GetInsertUserAuditInfo(EmpClearanceInfoViewModel model)
        {
            model.IUser = User.Identity.Name;
            model.IDate = DateTime.Now;

            foreach (var child in model.EmpClearanceFormDetails)
            {
                child.IUser = User.Identity.Name;
                child.IDate = DateTime.Now;
            }

            return model;
        }

        #endregion

        #region Attachment

        private int Upload(EmpClearanceInfoViewModel model)
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

        public void DownloadDoc(EmpClearanceInfoViewModel model)
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


        private IList<SelectListItem> PopulateChecklist(int clearanceFormId)
        {
            IList<PRM_ClearanceChecklistDetail> itemList;
            // itemList = _empService.PRMUnit.JobGradeRepository.Get(q => q.SalaryScaleId == salaryScaleId).OrderBy(o => o.GradeName, new AlphanumericSorting()).ToList();

            itemList = (from chkList in _prmCommonservice.PRMUnit.ClearanceChecklistRepository.GetAll()
                        join chkListDetail in _prmCommonservice.PRMUnit.ClearanceChecklistDetailRepository.GetAll() on chkList.Id equals chkListDetail.ClearanceChecklistId
                        where (clearanceFormId == chkList.ClearanceFormId)
                        select new PRM_ClearanceChecklistDetail
                        {
                            Id = chkListDetail.Id,
                            Name = chkListDetail.Name
                            //ClearanceFormId = chkList.ClearanceFormId
                        }).ToList();

            var list = new List<SelectListItem>();

            foreach (var item in itemList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return list.ToList();
        }



        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var obj = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeName = obj.FullName,
                Designation = obj.PRM_Designation.Name,
                Department = obj.DivisionId == null ? string.Empty : obj.PRM_Division.Name,
                Section = obj.PRM_Section == null ? string.Empty : obj.PRM_Section.Name
            });

        }

        [NoCache]
        public JsonResult GetResponsibleEmpInfo(int clearanceFormId)
        {
            var obj = (from chkList in _prmCommonservice.PRMUnit.ClearanceChecklistRepository.GetAll()
                       where (chkList.ClearanceFormId == clearanceFormId && chkList.ZoneInfoId == LoggedUserZoneInfoId)
                       select new
                       {
                           Id = chkList.Id,
                           EmployeeId = chkList.EmployeeId,
                           EmployeeName = chkList.PRM_EmploymentInfo.FullName,
                           Designation = chkList.PRM_EmploymentInfo.PRM_Designation.Name
                       }).FirstOrDefault();

            return Json(new
            {
                Id = obj == null ? 0 : obj.Id,
                EmployeeId = obj == null ? 0 : obj.EmployeeId,
                EmployeeName = obj == null ? string.Empty : obj.EmployeeName,
                Designation = obj == null ? string.Empty : obj.Designation
            });
        }

        public ActionResult GetChecklist(int clearanceFormId)
        {
            return Json(
                PopulateChecklist(clearanceFormId).Select(x => new { Id = x.Value, Name = x.Text }),
                JsonRequestBehavior.AllowGet
            );
        }


        [NoCache]
        public ActionResult DesignationListView()
        {
            var list = Common.PopulateEmployeeDesignationDDL(_prmCommonservice.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }
    }
}