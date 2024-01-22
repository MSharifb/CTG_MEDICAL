using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using System.Data.SqlClient;
using System.Collections;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class DelegationApprovalInfoController : BaseController
    {
        #region Fields

        private readonly INVCommonService _invCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Constructor

        public DelegationApprovalInfoController(PRMCommonSevice prmCommonService, INVCommonService invCommonService)
        {
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetDelegationInfo(JqGridRequest request, DelegationApprovalInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _invCommonService.INVUnit.DelegationApprovalInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (request.Searching)
            {
                if (viewModel.ApprovalDateFrom != null && viewModel.ApprovalDateTo != null)
                {
                    list = list.Where(x => x.ApprovalDate >= viewModel.ApprovalDateFrom && x.ApprovalDate <= viewModel.ApprovalDateTo).ToList();
                }
                if (viewModel.ItemTypeId > 0)
                {
                    list = list.Where(x => x.ItemTypeId == viewModel.ItemTypeId).ToList();
                }
                if (viewModel.DepartmentId > 0)
                {
                    list = list.Where(x => x.DepartmentId == viewModel.DepartmentId).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            //if (request.SortingName == "ItemName")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.ItemName).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.ItemName).ToList();
            //    }
            //}

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
                var DepartmentName = string.Empty;
                var DesignationName = string.Empty;
                DepartmentName = _prmCommonService.PRMUnit.DivisionRepository.GetByID(d.DepartmentId).Name;
                DesignationName = _prmCommonService.PRMUnit.DesignationRepository.GetByID(d.DesignationId).Name;
            
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ItemTypeId,
                    d.INV_ItemType.ItemTypeName,
                    d.DepartmentId,
                    DepartmentName,
                    DesignationName,
                    viewModel.ApprovalDateFrom,
                    viewModel.ApprovalDateTo,
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult DepartmentforView()
        {
            var itemList = Common.PopulateDllList(_prmCommonService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }
        [NoCache]
        public ActionResult ItemTypeforView()
        {
            var itemList = _invCommonService.INVUnit.ItemTypeRepository.GetAll().Where(x => x.ParentId != null).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemTypeName,
                    Value = y.Id.ToString()
                }).ToList();
            return PartialView("Select", itemList);
        }

        public ActionResult Create()
        {
            DelegationApprovalInfoViewModel model = new DelegationApprovalInfoViewModel();
            model.ActionType = "Create";

            PopulateList(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(DelegationApprovalInfoViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "add");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    try
                    {
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        INV_DelegationApprovalInfo entity = model.ToEntity();

                        _invCommonService.INVUnit.DelegationApprovalInfoRepository.Add(entity);
                        _invCommonService.INVUnit.DelegationApprovalInfoRepository.SaveChanges();

                        // model.IsError = 0;                  
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        //return RedirectToAction("Index");
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
            PopulateList(model);
            return View("CreateOrEdit", model);
        }

        public ActionResult Edit(int id)
        {
            INV_DelegationApprovalInfo entity = _invCommonService.INVUnit.DelegationApprovalInfoRepository.GetByID(id);
            DelegationApprovalInfoViewModel model = entity.ToModel();
            model.ActionType = "Edit";
            PopulateList(model);


            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(DelegationApprovalInfoViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "edit");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    INV_DelegationApprovalInfo entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _invCommonService.INVUnit.DelegationApprovalInfoRepository.Update(entity);
                        _invCommonService.INVUnit.DelegationApprovalInfoRepository.SaveChanges();
                        //   model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                        // return RedirectToAction("Index");
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
            PopulateList(model);
            return View("CreateOrEdit", model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            bool result = false;
            try
            {
                INV_DelegationApprovalInfo iteminfo = _invCommonService.INVUnit.DelegationApprovalInfoRepository.GetByID(id);
                _invCommonService.INVUnit.DelegationApprovalInfoRepository.Delete(iteminfo);
                _invCommonService.INVUnit.DelegationApprovalInfoRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        private string CheckDuplicate(DelegationApprovalInfoViewModel model, string strMode)
        {
            string message = string.Empty;
            dynamic delegationInfo = null;

            if (strMode == "add")
            {
                delegationInfo = _invCommonService.INVUnit.DelegationApprovalInfoRepository.Get(x => x.ItemTypeId == model.ItemTypeId && x.DesignationId == model.DesignationId && x.ZoneInfoId == LoggedUserZoneInfoId).FirstOrDefault();
            }
            else
            {
                delegationInfo = _invCommonService.INVUnit.DelegationApprovalInfoRepository.Get(x => x.ItemTypeId == model.ItemTypeId && x.DesignationId == model.DesignationId && x.ZoneInfoId == LoggedUserZoneInfoId && x.Id != model.Id).FirstOrDefault();
            }

            if (delegationInfo != null)
            {
                message += "Approval limit already exists. Please go back to list for edit.";
            }

            return message;

        }

        #region Others

        private void PopulateList(DelegationApprovalInfoViewModel model)
        {
            model.ItemTypeList = _invCommonService.INVUnit.ItemTypeRepository.GetAll().Where(x=> x.ParentId != null && x.IsGroup == false).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemTypeName,
                    Value = y.Id.ToString()
                }).ToList();

            #region Division
            var divList = _prmCommonService.PRMUnit.DivisionRepository.Fetch().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            model.DepartmentList = Common.PopulateDllList(divList);
            #endregion

            #region Post
            var DesignationList = _prmCommonService.PRMUnit.DesignationRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.DesignationList = Common.PopulateDllList(DesignationList);
            #endregion

        }

        #endregion
    }
}
