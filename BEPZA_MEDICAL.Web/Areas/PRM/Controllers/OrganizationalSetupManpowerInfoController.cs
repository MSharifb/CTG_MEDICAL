using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Data;
using System.Text;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Web.Helpers;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;

using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class OrganizationalSetupManpowerInfoController : BaseController
    {
        #region Fields

        private readonly PRMCommonSevice _PRMService;

        #endregion

        #region Constructor

        public OrganizationalSetupManpowerInfoController(PRMCommonSevice ser)
        {
            this._PRMService = ser;
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, OrganizationalSetupManpowerInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.IDate).ToList();

            if (model.OrganogramLevelId != 0)
            {
                list = list.Where(t => t.OrganogramLevelId == model.OrganogramLevelId).ToList();
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_Designation.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_Designation.Name).ToList();
                }
            }

            if (request.SortingName == "LevelName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_OrganogramLevel.LevelName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_OrganogramLevel.LevelName).ToList();
                }
            }

            if (request.SortingName == "SanctionedPost")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SanctionedPost).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SanctionedPost).ToList();
                }
            }
            if (request.SortingName == "ActiveStatus")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ActiveStatus).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ActiveStatus).ToList();
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

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,                    
                    item.PRM_Designation.Name,
                    item.PRM_OrganogramLevel.LevelName,
                    item.SanctionedPost,
                    item.ActiveStatus == true? "Yes" : "No",
                    "Edit",
                    "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [HttpGet]
        public PartialViewResult GetListByOrganogramId(int organogramId)
        {
            var model = new OrganizationalSetupManpowerInfoViewModel();

            List<OrganizationalSetupManpowerInfoViewModel> manPwrList = new List<OrganizationalSetupManpowerInfoViewModel>();

            var list = _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll().OrderBy(x => x.IDate).Where(t => t.OrganogramLevelId == organogramId).ToList();

            var i = 1;
            foreach (var item in list)
            {
                var gridModel = new OrganizationalSetupManpowerInfoViewModel
                {
                    SLNo = i++,
                    Id = item.Id,
                    DesignationId = item.DesignationId,
                    DesignationName = item.PRM_Designation.Name,
                    OrganogramLevelId = item.OrganogramLevelId,
                    OrganogramLevelName = item.PRM_OrganogramLevel.LevelName,
                    SanctionedPost = item.SanctionedPost,
                    ActiveStatus = item.ActiveStatus
                };
                manPwrList.Add(gridModel);
            }
            model.TempManPwrList = manPwrList;
            return PartialView("_Details", model);
        }

        public ActionResult Create()
        {
            var model = new OrganizationalSetupManpowerInfoViewModel();
            PopulateDropdownList(model);
            model.strMode = "Add";
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(OrganizationalSetupManpowerInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        PopulateDropdownList(model);
                        return View(model);
                    }

                    var checkoutBusinessLogic = "";

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {

                        model = SetUserAuditInfo(model, true);
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        var master = model.ToEntity();

                        _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Add(master);
                        _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.SaveChanges();

                        model.IsSuccessful = true;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.ErrMsg = checkoutBusinessLogic;
                    }
                }
                catch (Exception ex)
                {
                    model.IsSuccessful = false;
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        model.ErrMsg = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        model.ErrMsg = ErrorMessages.InsertFailed;
                    }
                }
            }
            else
            {
                model.IsSuccessful = false;
            }

            PopulateDropdownList(model);
            model = ClearFields(model);

            model.strMode = "add";
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var master = _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetByID(id);
            var model = master.ToModel();
            model.OrganogramLevelId = master.OrganogramLevelId;
            model.OrganogramLevelName = master.PRM_OrganogramLevel.LevelName;
            model.strMode = "Edit";
            PopulateDropdownList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(OrganizationalSetupManpowerInfoViewModel model)
        {
            var checkoutBusinessLogic = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        PopulateDropdownList(model);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model = SetUserAuditInfo(model, false);
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        var master = model.ToEntity();

                        _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Update(master);
                        _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.SaveChanges();

                        model.IsSuccessful = true;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        // return RedirectToAction("Create", new {type = "Isuccess" });
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.errClass = "failed";
                        model.ErrMsg = checkoutBusinessLogic;
                    }
                }
                else
                {
                    model.errClass = "failed";
                    model.IsSuccessful = false;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.IsSuccessful = false;
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    model.ErrMsg = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.ErrMsg = ErrorMessages.UpdateFailed;
                }
            }
            PopulateDropdownList(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Delete(id);
                _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.SaveChanges();
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

        private OrganizationalSetupManpowerInfoViewModel ClearFields(OrganizationalSetupManpowerInfoViewModel model)
        {
            model.OrganogramLevelName = "";
            model.SanctionedPost = 0;
            model.ActiveStatus = false;
            return model;
        }

        private OrganizationalSetupManpowerInfoViewModel SetUserAuditInfo(OrganizationalSetupManpowerInfoViewModel item, bool IsInsert)
        {
            if (IsInsert)
            {
                item.IUser = User.Identity.Name;
                item.IDate = DateTime.Now;
            }

            if (!IsInsert)
            {
                item.EUser = User.Identity.Name;
                item.EDate = DateTime.Now;
            }

            return item;
        }

        private void PopulateDropdownList(OrganizationalSetupManpowerInfoViewModel model)
        {
            dynamic ddlList;

            ddlList = _PRMService.PRMUnit.DesignationRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.DesignationList = Common.PopulateDllList(ddlList);
        }

        //check duplicate Designaion in Same Organogram Level
        private bool CheckDuplicateEntry(OrganizationalSetupManpowerInfoViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Get(q => q.OrganogramLevelId == model.OrganogramLevelId && q.DesignationId == model.DesignationId).Any();
            }

            else
            {
                return _PRMService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Get(q => q.OrganogramLevelId == model.OrganogramLevelId && q.DesignationId == model.DesignationId && strMode != q.Id).Any();
            }
        }

        #endregion
    }
}
