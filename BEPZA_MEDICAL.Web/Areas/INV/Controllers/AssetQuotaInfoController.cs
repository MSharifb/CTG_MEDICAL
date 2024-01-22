using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;
using System.Data.SqlClient;
using System.Data;
using BEPZA_MEDICAL.Web.Resources;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class AssetQuotaInfoController : BaseController
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;

        #endregion

        #region Constructor

        public AssetQuotaInfoController(INVCommonService invCommonService, PRMCommonSevice prmCommonService)
        {
            _invCommonService = invCommonService;
            _prmCommonService = prmCommonService;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AssetQuotaInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from aq in _invCommonService.INVUnit.AssetQuotaInfoRepository.GetAll()
                        join zone in _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll() on aq.ZoneId equals zone.Id
                        join itemInfo in _invCommonService.INVUnit.ItemInfoRepository.GetAll() on aq.ItemId equals itemInfo.Id
                        join unit in _invCommonService.INVUnit.UnitRepository.GetAll() on aq.UnitId equals unit.Id
                        select new AssetQuotaInfoViewModel()
                        {
                            Id = aq.Id,
                            ItemId = itemInfo.Id,
                            ItemInfoName = itemInfo.ItemName,
                            ZoneName = zone.ZoneName,
                            ZoneId = zone.Id,
                            Quota = aq.Quota,
                            UnitId = unit.Id,
                            UnitName = unit.Name
                        }).OrderBy(x => x.ItemInfoName).ToList();

            totalRecords = list == null ? 0 : list.Count;

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.ItemInfoName))
                {
                    list = list.Where(x => x.ItemInfoName.Trim().ToLower().Contains(model.ItemInfoName.Trim().ToLower())).ToList();
                }
            }

            #region Sorting

            if (request.SortingName == "ItemInfoName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ItemInfoName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ItemInfoName).ToList();
                }
            }

            if (request.SortingName == "ZoneName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ZoneName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ZoneName).ToList();
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
                var quota = d.Quota + " " + d.UnitName;
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ItemInfoName,
                    d.ZoneName,
                    quota
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            AssetQuotaInfoViewModel model = new AssetQuotaInfoViewModel();
            model.ActionType = "Create";

            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AssetQuotaInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!CheckDuplicate(model, "add"))
                {
                    try
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _invCommonService.INVUnit.AssetQuotaInfoRepository.Add(entity);
                        _invCommonService.INVUnit.AssetQuotaInfoRepository.SaveChanges();

                        // model.IsError = 0;                  
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model.errClass = "success";
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
                            model.errClass = "failed";
                        }
                    }
                }
                else
                {
                    model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    model.IsError = 1;
                    model.errClass = "failed";
                }
            }
            PopulateList(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            INV_AssetQuotaInfo entity = _invCommonService.INVUnit.AssetQuotaInfoRepository.GetByID(id);
            AssetQuotaInfoViewModel model = entity.ToModel();
            model.ActionType = "Edit";
            PopulateList(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AssetQuotaInfoViewModel model)
        {
            model.IsError = 1;
            var strMessage = string.Empty;
            if (CheckDuplicate(model, "edit"))
            {
                model.ErrMsg = model.ItemInfoName + "is already assigned to" + model.ZoneName;
                model.errClass = "failed";
            }
            if (ModelState.IsValid && string.IsNullOrWhiteSpace(model.ErrMsg))
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _invCommonService.INVUnit.AssetQuotaInfoRepository.Update(entity);
                    _invCommonService.INVUnit.AssetQuotaInfoRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        strMessage = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        strMessage = ErrorMessages.UpdateFailed;
                    }
                    //model.errClass = "failed";
                    //model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            bool result = false;
            try
            {
                INV_AssetQuotaInfo assetQuotaInfo = _invCommonService.INVUnit.AssetQuotaInfoRepository.GetByID(id);
                _invCommonService.INVUnit.AssetQuotaInfoRepository.Delete(assetQuotaInfo);
                _invCommonService.INVUnit.AssetQuotaInfoRepository.SaveChanges();
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

        private bool CheckDuplicate(AssetQuotaInfoViewModel model, string strMode)
        {
            dynamic objItem = null;
            try
            {
                if (strMode == "add")
                {
                    objItem = _invCommonService.INVUnit.AssetQuotaInfoRepository.GetAll().Where(x => x.ItemId == model.ItemId && x.ZoneId == model.ZoneId).FirstOrDefault();

                }
                else
                {
                    objItem = _invCommonService.INVUnit.AssetQuotaInfoRepository.GetAll().Where(x => x.ItemId == model.ItemId && x.ZoneId == model.ZoneId && x.Id != model.Id).FirstOrDefault();

                }

                if (objItem != null)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        #region Others

        private void PopulateList(AssetQuotaInfoViewModel model)
        {
            model.ZoneList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x => x.SortOrder).ToList().Select(y => new SelectListItem()
            {
                Text = y.ZoneName,
                Value = y.Id.ToString()
            }).ToList();
            model.ItemInfoList = _invCommonService.INVUnit.ItemInfoRepository.GetAll().Where(t => t.HasQuota == true).OrderBy(x => x.ItemName).ToList().Select(y => new SelectListItem()
            {
                Text = y.ItemName,
                Value = y.Id.ToString()
            }).ToList();

            model.UnitList = Common.PopulateDllList(_invCommonService.INVUnit.UnitRepository.GetAll().OrderBy(x => x.Name));

        }

        #endregion
    }
}