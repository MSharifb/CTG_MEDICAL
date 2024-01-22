using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.AMS.ViewModel;
using BEPZA_MEDICAL.Domain.AMS;
using BEPZA_MEDICAL.DAL.AMS;
using System.Data.SqlClient;
using System.Collections;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.AMS.Controllers
{
    public class StayRuleController : BaseController
    {
        #region Fields

        private readonly AMSCommonService _amsCommonService;

        #endregion

        #region Constructor

        public StayRuleController(AMSCommonService amsCommonservice)
        {
            _amsCommonService = amsCommonservice;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetStayRule(JqGridRequest request, StayRuleViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _amsCommonService.AMSUnit.StayRuleRepository.GetAll().ToList();

            if (request.Searching)
            {
                if (viewModel.CategoryId > 0)
                {
                    list = list.Where(x => x.CategoryId == viewModel.CategoryId).ToList();
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
                
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.CategoryId,
                    Category,
                    d.MaximumStay,
                    d.Remarks
                }));
            }
         
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            StayRuleViewModel model = new StayRuleViewModel();
            model.ActionType = "Create";

            PopulateList(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(StayRuleViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "add");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    try
                    {
                        AMS_StayRule entity = model.ToEntity();

                        _amsCommonService.AMSUnit.StayRuleRepository.Add(entity);
                        _amsCommonService.AMSUnit.StayRuleRepository.SaveChanges();
            
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
            AMS_StayRule entity = _amsCommonService.AMSUnit.StayRuleRepository.GetByID(id);
            StayRuleViewModel model = entity.ToModel();
            model.ActionType = "Edit";
            PopulateList(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(StayRuleViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "edit");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    AMS_StayRule entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _amsCommonService.AMSUnit.StayRuleRepository.Update(entity);
                        _amsCommonService.AMSUnit.StayRuleRepository.SaveChanges();
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
                AMS_StayRule stayRuleInfo = _amsCommonService.AMSUnit.StayRuleRepository.GetByID(id);
                _amsCommonService.AMSUnit.StayRuleRepository.Delete(stayRuleInfo);
                _amsCommonService.AMSUnit.StayRuleRepository.SaveChanges();
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

        private string CheckDuplicate(StayRuleViewModel model, string strMode)
        {
            string message = string.Empty;
            dynamic stayRuleInfo = null;

            if (strMode == "add")
            {
                stayRuleInfo = _amsCommonService.AMSUnit.StayRuleRepository.Get(x => x.CategoryId == model.CategoryId).FirstOrDefault();
            }
            else
            {
                stayRuleInfo = _amsCommonService.AMSUnit.StayRuleRepository.Get(x => x.CategoryId == model.CategoryId && x.Id != model.Id).FirstOrDefault();
            }

            if (stayRuleInfo != null)
            {
                message += "Stay Rule already exists for this category. Please go back to list to edit.";
            }

            return message;

        }

        #region Others

        private void PopulateList(StayRuleViewModel model)
        {
            model.CategoryList = Common.PopulateDllList(_amsCommonService.AMSUnit.AnsarCategoryRepository.GetAll().OrderBy(x => x.Name));
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

        #endregion
    }
}
