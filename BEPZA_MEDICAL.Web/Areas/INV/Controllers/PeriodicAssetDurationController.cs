using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class PeriodicAssetDurationController : BaseController
    {
        //
        // GET: /INV/PeriodicAssetDuration/
        #region Fields
        private readonly INVCommonService _invCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public PeriodicAssetDurationController(INVCommonService invCommonService, PRMCommonSevice prmCommonService)
        {
            this._invCommonService = invCommonService;
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Action
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, PeriodicAssetDurationViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<PeriodicAssetDurationViewModel> list = (from pad in _invCommonService.INVUnit.PeriodicAssetDurationRepository.GetAll()
                                                         join desig in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on pad.DesignationId equals desig.Id
                                                         join ItemInfo in _invCommonService.INVUnit.ItemInfoRepository.GetAll() on pad.ItemInfoId equals ItemInfo.Id
                                                         select new PeriodicAssetDurationViewModel()
                                                          {
                                                              Id = pad.Id,
                                                              DesignationId = pad.DesignationId,
                                                              DesignationName = desig.Name,
                                                              ItemInfoId = ItemInfo.Id,
                                                              ItemInfoName = ItemInfo.ItemName,
                                                              ReIssueAfter = pad.ReIssueAfter,
                                                              DurationScale = pad.DurationScale,
                                                              Remarks = pad.Remarks
                                                          }).OrderBy(x => x.ItemInfoName).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            //if (request.SortingName == "AssetName")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.AssetName).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.AssetName).ToList();
            //    }
            //}

            #endregion

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.ItemInfoName))
                {
                    list = list.Where(x => x.ItemInfoName.Trim().ToLower().Contains(model.ItemInfoName.Trim().ToLower())).ToList();
                }
            }

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                var reIssuePeriod = d.ReIssueAfter + " " + d.DurationScale;
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                  d.Id,
                  // d.DesignationId,
                  d.ItemInfoName,
                  d.DesignationName,
                  reIssuePeriod                 
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            PeriodicAssetDurationViewModel model = new PeriodicAssetDurationViewModel();
            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PeriodicAssetDurationViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            var strMessage = string.Empty;
            if (CheckDuplicate(model, "add"))
            {
                strMessage = model.ItemInfoName + " is already assigned.";
            }
            model.ErrMsg = strMessage;
            if (!string.IsNullOrWhiteSpace(strMessage))
            {
                model.errClass = "failed";
            }
            if (ModelState.IsValid && string.IsNullOrWhiteSpace(strMessage))
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _invCommonService.INVUnit.PeriodicAssetDurationRepository.Add(entity);
                    _invCommonService.INVUnit.PeriodicAssetDurationRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            PopulateList(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _invCommonService.INVUnit.PeriodicAssetDurationRepository.GetByID(Id);
            PeriodicAssetDurationViewModel model = entity.ToModel();
            model.ActionType = "Edit";
            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PeriodicAssetDurationViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;
            if (CheckDuplicate(model, "edit"))
            {
                model.ErrMsg = model.ItemInfoName + "is already assigned";
                model.errClass = "failed";
            }
            if (ModelState.IsValid && string.IsNullOrWhiteSpace(model.ErrMsg))
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _invCommonService.INVUnit.PeriodicAssetDurationRepository.Update(entity);
                    _invCommonService.INVUnit.PeriodicAssetDurationRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }
                catch (Exception)
                {
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
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
            var tempPeriod = _invCommonService.INVUnit.PeriodicAssetDurationRepository.GetByID(id);
            try
            {
                _invCommonService.INVUnit.PeriodicAssetDurationRepository.Delete(id);
                _invCommonService.INVUnit.PeriodicAssetDurationRepository.SaveChanges();
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

        private void PopulateList(PeriodicAssetDurationViewModel model)
        {
            model.ItemInfoList = _invCommonService.INVUnit.ItemInfoRepository.GetAll().Where(t => t.IsPeriodicAsset == true).OrderBy(x => x.ItemName).ToList().Select(y => new SelectListItem()
            {
                Text = y.ItemName,
                Value = y.Id.ToString()
            }).ToList();

            var ddlDesig = _prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.DesignationList = Common.PopulateDllList(ddlDesig);

            model.DurationScaleList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text="Year(s)", Value="Year", Selected = true },
                new SelectListItem(){ Text="Month(s)", Value="Month" },
                new SelectListItem(){ Text="Day(s)", Value="Day" },                
            };
        }

        private bool CheckDuplicate(PeriodicAssetDurationViewModel model, string strMode)
        {
            dynamic objDesignation = null;
            try
            {
                if (strMode == "add")
                {
                    objDesignation = _invCommonService.INVUnit.PeriodicAssetDurationRepository.GetAll().Where(x => x.ItemInfoId == model.ItemInfoId && x.DesignationId == model.DesignationId).FirstOrDefault();

                }
                else
                {
                    objDesignation = _invCommonService.INVUnit.PeriodicAssetDurationRepository.GetAll().Where(x => x.ItemInfoId == model.ItemInfoId && x.DesignationId == model.DesignationId && x.Id != model.Id).FirstOrDefault(); ;

                }

                if (objDesignation != null)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        #endregion
    }
}