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
    public class ZoneWiseQuotaController : BaseController
    {
        #region Fields

        private readonly AMSCommonService _amsCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Constructor

        public ZoneWiseQuotaController(AMSCommonService amsCommonservice, PRMCommonSevice prmCommonService)
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
        public ActionResult GetZoneWiseQuota(JqGridRequest request, ZoneWiseQuotaViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.GetAll().ToList();

            if (request.Searching)
            {
                if (viewModel.CategoryId > 0)
                {
                    list = list.Where(x => x.CategoryId == viewModel.CategoryId).ToList();
                }
                if (viewModel.ZoneInfoId > 0)
                {
                    list = list.Where(x => x.ZoneInfoId == viewModel.ZoneInfoId).ToList();
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
                var Category = d.AMS_Category.Name;
                var ZoneName = d.PRM_ZoneInfo.ZoneName;
                
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ZoneInfoId,
                    ZoneName,
                    d.CategoryId,
                    Category,
                    d.Quota
                }));
            }
         
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ZoneWiseQuotaViewModel model = new ZoneWiseQuotaViewModel();
            model.ActionType = "Create";

            PopulateList(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(ZoneWiseQuotaViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "add");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    try
                    {
                        AMS_ZoneWiseQuota entity = model.ToEntity();

                        _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.Add(entity);
                        _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.SaveChanges();
            
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
            PopulateList(model);
            return View("CreateOrEdit", model);
        }

        public ActionResult Edit(int id)
        {
            AMS_ZoneWiseQuota entity = _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.GetByID(id);
            ZoneWiseQuotaViewModel model = entity.ToModel();
            model.ActionType = "Edit";
            PopulateList(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(ZoneWiseQuotaViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "edit");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    AMS_ZoneWiseQuota entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.Update(entity);
                        _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.SaveChanges();
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
                AMS_ZoneWiseQuota zoneWiseInfo = _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.GetByID(id);
                _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.Delete(zoneWiseInfo);
                _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.SaveChanges();
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

        private string CheckDuplicate(ZoneWiseQuotaViewModel model, string strMode)
        {
            string message = string.Empty;
            dynamic zoneWiseQuotaInfo = null;

            if (strMode == "add")
            {
                zoneWiseQuotaInfo = _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.Get(x => x.ZoneInfoId == model.ZoneInfoId && x.CategoryId == model.CategoryId).FirstOrDefault();
            }
            else
            {
                zoneWiseQuotaInfo = _amsCommonService.AMSUnit.ZoneWiseQuotaRepository.Get(x => x.ZoneInfoId == model.ZoneInfoId && x.CategoryId == model.CategoryId && x.Id != model.Id).FirstOrDefault();
            }

            if (zoneWiseQuotaInfo != null)
            {
                message += "Quota already exists for the selected Zone & Category. Please go back to list to edit.";
            }

            return message;

        }

        #region Others

        private void PopulateList(ZoneWiseQuotaViewModel model)
        {
            model.CategoryList = Common.PopulateDllList(_amsCommonService.AMSUnit.AnsarCategoryRepository.GetAll().OrderBy(x => x.Name));
            model.ZoneList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ZoneName,
                    Value = y.Id.ToString()
                }).ToList();
        }

        [NoCache]
        public ActionResult GetAnsarCategoryView()
        {
            Dictionary<string, string> ansarCategory = new Dictionary<string, string>();
            var ansarCategorylist = _amsCommonService.AMSUnit.AnsarCategoryRepository.GetAll().OrderBy(x=> x.Name).ToList();

            foreach (var item in ansarCategorylist)
            {
                ansarCategory.Add(item.Id.ToString(), item.Name);
            }

            return PartialView("_Select", ansarCategory);
        }

        [NoCache]
        public ActionResult GetZoneView()
        {
            Dictionary<string, string> zone = new Dictionary<string, string>();
            var zonelist = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x => x.ZoneName).ToList();

            foreach (var item in zonelist)
            {
                zone.Add(item.Id.ToString(), item.ZoneName);
            }

            return PartialView("_Select", zone);
        }

        #endregion
    }
}
