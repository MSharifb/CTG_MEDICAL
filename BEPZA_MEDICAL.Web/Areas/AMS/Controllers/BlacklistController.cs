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
    public class BlacklistController : BaseController
    {
        #region Fields

        private readonly AMSCommonService _amsCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Constructor

        public BlacklistController(AMSCommonService amsCommonservice, PRMCommonSevice prmCommonService)
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
        public ActionResult GetBlacklist(JqGridRequest request, BlacklistViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _amsCommonService.AMSUnit.BlacklistRepository.GetAll().Where(x=> x.IsRevoked == false && x.AMS_AnsarEmpInfo.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.BEPZAID))
                {
                    list = list.Where(x => x.AMS_AnsarEmpInfo.BEPZAId.Contains(viewModel.BEPZAID)).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.Name))
                {
                    list = list.Where(x => x.AMS_AnsarEmpInfo.FullName.ToLower().Contains(viewModel.Name.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.AnsarId))
                {
                    list = list.Where(x => x.AMS_AnsarEmpInfo.AnsarId.Contains(viewModel.AnsarId)).ToList();
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

            var serial = 1;
            foreach (var d in list)
            {
                var Name = d.AMS_AnsarEmpInfo.FullName;
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    serial,
                    d.AMS_AnsarEmpInfo.AnsarId,
                    d.AMS_AnsarEmpInfo.BEPZAId,
                    Name,
                    d.AMS_AnsarEmpInfo.AMS_DesignationInfo.DesignationName,
                    d.Reason,
                    d.Date.ToString(DateAndTime.GlobalDateFormat)
                }));
                serial++;
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            BlacklistViewModel model = new BlacklistViewModel();
            model.ActionType = "Create";

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(BlacklistViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "add");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    try
                    {
                        AMS_Blacklist entity = model.ToEntity();

                        _amsCommonService.AMSUnit.BlacklistRepository.Add(entity);
                        _amsCommonService.AMSUnit.BlacklistRepository.SaveChanges();

                        #region Ansar Update
                        var ansar = new AMS_AnsarEmpInfo();
                        ansar = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(entity.EmployeeId);
                        if (ansar.AMS_EmpStatus.Name != "Inactive")
                        {
                            var statusId = _amsCommonService.AMSUnit.EmpStatusRepository.Get(x => x.Name == "Inactive").FirstOrDefault().Id;
                            ansar.StatusId = statusId;
                            ansar.InactiveDate = DateTime.UtcNow;

                            _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Update(ansar);
                            _amsCommonService.AMSUnit.AnsarEmpInfoRepository.SaveChanges();
                        }
                        #endregion

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
            AMS_Blacklist entity = _amsCommonService.AMSUnit.BlacklistRepository.GetByID(id);
            BlacklistViewModel model = entity.ToModel();

            var ansarInfo = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Fetch().Where(x => x.Id == model.EmployeeId).FirstOrDefault();

            model.BEPZAID = ansarInfo.BEPZAId;
            model.AnsarId = ansarInfo.AnsarId;
            model.Name = ansarInfo.FullName;
            model.Designation = ansarInfo.AMS_DesignationInfo.DesignationName;
            model.DateOfJoining = ansarInfo.DateofJoining;
            model.AnsarJoiningDate = ansarInfo.AnsarJoiningDate;

            var blacklistedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.BlacklistedByEmpId);
            model.BlacklistedBy = blacklistedBy.FullName;

            //var approvedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ApprovedByEmpId);
            //model.ApprovedBy = approvedBy.FullName;

            model.ActionType = "Edit";

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(BlacklistViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "edit");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    AMS_Blacklist entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _amsCommonService.AMSUnit.BlacklistRepository.Update(entity);
                        _amsCommonService.AMSUnit.BlacklistRepository.SaveChanges();
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
        //        AMS_Blacklist blacklistEntity = _amsCommonService.AMSUnit.BlacklistRepository.GetByID(id);
        //        _amsCommonService.AMSUnit.BlacklistRepository.Delete(blacklistEntity);
        //        _amsCommonService.AMSUnit.BlacklistRepository.SaveChanges();
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

        private string CheckDuplicate(BlacklistViewModel model, string strMode)
        {
            string message = string.Empty;
            dynamic blacklist = null;

            var ansarId = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.Fetch().Where(x => x.Id == model.EmployeeId).FirstOrDefault().AnsarId;

            if (strMode == "add")
            {
                blacklist = _amsCommonService.AMSUnit.BlacklistRepository.Get(x => x.AMS_AnsarEmpInfo.AnsarId == ansarId && x.IsRevoked == false).FirstOrDefault();
            }

            if (blacklist != null)
            {
                message += "Already Blacklisted.";
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
        public JsonResult GetEmployeeInfo(BlacklistViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var BlacklistedBy = string.Empty;
                    var BlacklistedByEmpId = string.Empty;
                    //var ApprovedBy = string.Empty;
                    //var ApprovedByEmpId = string.Empty;

                    var objBlacklistedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.BlacklistedByEmpId);
                    //var objApprovedBy = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ApprovedByEmpId);

                    if (objBlacklistedBy != null)
                    {
                        BlacklistedBy = objBlacklistedBy.EmpID + " - " + objBlacklistedBy.FullName;
                        BlacklistedByEmpId = objBlacklistedBy.Id.ToString();
                    }
                    //if (objApprovedBy != null)
                    //{
                    //    ApprovedBy = objApprovedBy.EmpID + " - " + objApprovedBy.FullName;
                    //    ApprovedByEmpId = objApprovedBy.Id.ToString();
                    //}

                    return Json(new
                    {
                        BlacklistedBy = BlacklistedBy,
                        BlacklistedByEmpId = BlacklistedByEmpId
                        //ApprovedBy = ApprovedBy,
                        //ApprovedByEmpId = ApprovedByEmpId

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
        public JsonResult GetAnsarInfo(BlacklistViewModel model)
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

                    var objAnsar = _amsCommonService.AMSUnit.AnsarEmpInfoRepository.GetByID(model.EmployeeId);

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
