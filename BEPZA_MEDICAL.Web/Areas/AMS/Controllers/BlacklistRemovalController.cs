using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.AMS.ViewModel;
using BEPZA_MEDICAL.Domain.AMS;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.AMS;
using System.Data.SqlClient;
using System.Collections;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.AMS.Controllers
{
    public class BlacklistRemovalController : BaseController
    {
        #region Fields

        private readonly AMSCommonService _amsCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Constructor

        public BlacklistRemovalController(AMSCommonService amsCommonservice, PRMCommonSevice prmCommonService)
        {
            _amsCommonService = amsCommonservice;
            _prmCommonService = prmCommonService;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetBlacklistRemoval(JqGridRequest request, BlacklistRemovalViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _amsCommonService.AMSUnit.BlacklistRemovalRepository.GetAll().Where(x=> x.AMS_Blacklist.AMS_AnsarEmpInfo.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.BEPZAID))
                {
                    list = list.Where(x => x.AMS_Blacklist.AMS_AnsarEmpInfo.BEPZAId.Contains(viewModel.BEPZAID)).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.Name))
                {
                    list = list.Where(x => x.AMS_Blacklist.AMS_AnsarEmpInfo.FullName.ToLower().Contains(viewModel.Name.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.AnsarId))
                {
                    list = list.Where(x => x.AMS_Blacklist.AMS_AnsarEmpInfo.AnsarId.Contains(viewModel.AnsarId)).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                var Name = d.AMS_Blacklist.AMS_AnsarEmpInfo.FullName;
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.AMS_Blacklist.AMS_AnsarEmpInfo.BEPZAId,
                    Name,
                    d.AMS_Blacklist.AMS_AnsarEmpInfo.AMS_DesignationInfo.DesignationName,
                    d.AMS_Blacklist.AMS_AnsarEmpInfo.AnsarId,
                    d.Remarks,
                    d.DateofRemoval.ToString(DateAndTime.GlobalDateFormat)
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            BlacklistRemovalViewModel model = new BlacklistRemovalViewModel();
            model.ActionType = "Create";

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(BlacklistRemovalViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "add");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    try
                    {
                        AMS_BlacklistRemoval entity = model.ToEntity();

                        _amsCommonService.AMSUnit.BlacklistRemovalRepository.Add(entity);
                        _amsCommonService.AMSUnit.BlacklistRemovalRepository.SaveChanges();

                        var blacklistEntity = _amsCommonService.AMSUnit.BlacklistRepository.GetByID(entity.BlacklistId);
                        blacklistEntity.IsRevoked = true;
                        _amsCommonService.AMSUnit.BlacklistRepository.Update(blacklistEntity);
                        _amsCommonService.AMSUnit.BlacklistRepository.SaveChanges();


                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    catch (Exception ex)
                    {
                        model.IsError = 1;
                        try
                        {
                            if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                            {
                                SqlException sqlException = ex.InnerException as SqlException;
                                model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            }
                        }
                        catch (Exception)
                        {
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                        }
                    }
                }
                else
                {
                    model.ErrMsg = dupErrMessage;
                    model.IsError = 1;
                }
            }

            model.ActionType = "Create";
            return View("CreateOrEdit", model);
        }

        public ActionResult Edit(int id)
        {
            AMS_BlacklistRemoval entity = _amsCommonService.AMSUnit.BlacklistRemovalRepository.GetByID(id);
            BlacklistRemovalViewModel model = entity.ToModel();

            var ansarInfo = entity.AMS_Blacklist.AMS_AnsarEmpInfo;

            model.BEPZAID = ansarInfo.BEPZAId;
            model.AnsarId = ansarInfo.AnsarId;
            model.Name = ansarInfo.FullName;
            model.Designation = ansarInfo.AMS_DesignationInfo.DesignationName;
            model.DateOfJoining = ansarInfo.DateofJoining;
            model.AnsarJoiningDate = ansarInfo.AnsarJoiningDate;

            var removedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.RemovedByEmpId);
            model.RemovedBy = removedBy.FullName;

            var approvedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ApprovedByEmpId);
            model.ApprovedBy = approvedBy.FullName;

            model.ActionType = "Edit";

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(BlacklistRemovalViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "edit");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    AMS_BlacklistRemoval entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _amsCommonService.AMSUnit.BlacklistRemovalRepository.Update(entity);
                        _amsCommonService.AMSUnit.BlacklistRemovalRepository.SaveChanges();
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    }
                    catch (Exception ex)
                    {
                        model.IsError = 1;
                        try
                        {
                            if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                            {
                                SqlException sqlException = ex.InnerException as SqlException;
                                model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            }
                        }
                        catch (Exception)
                        {
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        }
                    }
                }
                else
                {
                    model.ErrMsg = dupErrMessage;
                    model.IsError = 1;
                }
            }

            model.ActionType = "Edit";
            return View("CreateOrEdit", model);
        }

        #region Delete

        //[HttpPost, ActionName("Delete")]
        //public JsonResult DeleteConfirmed(int id)
        //{
        //    string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

        //    bool result = false;
        //    try
        //    {
        //        AMS_BlacklistRemoval blacklistEntity = _amsCommonService.AMSUnit.BlacklistRemovalRepository.GetByID(id);
        //        _amsCommonService.AMSUnit.BlacklistRemovalRepository.Delete(blacklistEntity);
        //        _amsCommonService.AMSUnit.BlacklistRemovalRepository.SaveChanges();
        //        result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        try
        //        {
        //            if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
        //            {
        //                SqlException sqlException = ex.InnerException as SqlException;
        //                errMsg = Common.GetSqlExceptionMessage(sqlException.Number);
        //            }
        //            result = false;
        //        }
        //        catch (Exception)
        //        {
        //            result = false;
        //        }
        //    }
        //    return Json(new
        //    {
        //        Success = result,
        //        Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
        //    });
        //}

        #endregion

        private string CheckDuplicate(BlacklistRemovalViewModel model, string strMode)
        {
            string message = string.Empty;

            var blacklistDate = _amsCommonService.AMSUnit.BlacklistRepository.GetByID(model.BlacklistId).Date;

            if (model.DateofRemoval < blacklistDate)
            {
                message += "Removal date must be greater than blacklisted date.";
            }
            
            
            return message;

        }

        #region Others

        [NoCache]
        public ActionResult GetDesignationView()
        {
            Dictionary<string, string> ansarDesignation = new Dictionary<string, string>();
            var ansarDesignationlist = _amsCommonService.AMSUnit.AnsarDesignationRepository.GetAll().OrderBy(x => x.SortOrder).ToList();

            foreach (var item in ansarDesignationlist)
            {
                ansarDesignation.Add(item.Id.ToString(), item.DesignationName);
            }

            return PartialView("_Select", ansarDesignation);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(BlacklistRemovalViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var RemovedBy = string.Empty;
                    var RemovedByEmpId = string.Empty;
                    var ApprovedBy = string.Empty;
                    var ApprovedByEmpId = string.Empty;

                    var objRemovedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.RemovedByEmpId);
                    var objApprovedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ApprovedByEmpId);

                    if (objRemovedBy != null)
                    {
                        RemovedBy = objRemovedBy.EmpID + " - " + objRemovedBy.FullName;
                        RemovedByEmpId = objRemovedBy.Id.ToString();
                    }
                    if (objApprovedBy != null)
                    {
                        ApprovedBy = objApprovedBy.EmpID + " - " + objApprovedBy.FullName;
                        ApprovedByEmpId = objApprovedBy.Id.ToString();
                    }

                    return Json(new
                    {
                        RemovedBy = RemovedBy,
                        RemovedByEmpId = RemovedByEmpId,
                        ApprovedBy = ApprovedBy,
                        ApprovedByEmpId = ApprovedByEmpId

                    });

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.Message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        return Json(new { Result = false });

                    }
                    else
                    {
                        model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        return Json(new { Result = false });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Result = msg
                });
            }
        }

        [NoCache]
        public JsonResult GetAnsarInfo(BlacklistRemovalViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var BEPZAID = string.Empty;
                    var AnsarId = string.Empty;
                    var Name = string.Empty;
                    var Designation = string.Empty;
                    var DateOfJoining = string.Empty;
                    var AnsarJoiningDate = string.Empty;

                    var empId = _amsCommonService.AMSUnit.BlacklistRepository.GetByID(model.BlacklistId).EmployeeId;

                    var objAnsar = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(empId);

                    if (objAnsar != null)
                    {
                        BEPZAID = objAnsar.BEPZAId;
                        AnsarId = objAnsar.AnsarId;
                        Name = objAnsar.FullName;
                        Designation = objAnsar.AMS_DesignationInfo.DesignationName;
                        DateOfJoining = objAnsar.DateofJoining.ToString(DateAndTime.GlobalDateFormat);
                        AnsarJoiningDate = objAnsar.AnsarJoiningDate.HasValue ? objAnsar.AnsarJoiningDate.Value.ToString(DateAndTime.GlobalDateFormat) : string.Empty;

                    }

                    return Json(new
                    {
                        BEPZAID,
                        AnsarId,
                        Name,
                        Designation,
                        DateOfJoining,
                        AnsarJoiningDate

                    });

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.Message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        return Json(new { Result = false });

                    }
                    else
                    {
                        model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        return Json(new { Result = false });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Result = msg
                });
            }
        }

        #endregion
    }
}
