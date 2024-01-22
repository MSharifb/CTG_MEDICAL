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
    public class DesignationInfoController : BaseController
    {
        #region Fields

        private readonly AMSCommonService _amsCommonService;

        #endregion

        #region Constructor

        public DesignationInfoController(AMSCommonService amsCommonservice)
        {
            _amsCommonService = amsCommonservice;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAnsarDesignationInfo(JqGridRequest request, DesignationInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _amsCommonService.AMSUnit.AnsarDesignationRepository.GetAll().ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.DesignationName))
                {
                    list = list.Where(x => x.DesignationName.ToLower().Contains(viewModel.DesignationName.ToLower())).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting
            if (request.SortingName == "DesignationName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DesignationName).ToList();
                }
            }

            if (request.SortingName == "SortOrder")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SortOrder).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SortOrder).ToList();
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
                    d.DesignationName,
                    d.SortOrder
                }));
            }
         
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            DesignationInfoViewModel model = new DesignationInfoViewModel();
            model.ActionType = "Create";

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(DesignationInfoViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "add");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    try
                    {
                        AMS_DesignationInfo entity = model.ToEntity();

                        _amsCommonService.AMSUnit.AnsarDesignationRepository.Add(entity);
                        _amsCommonService.AMSUnit.AnsarDesignationRepository.SaveChanges();
            
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
            AMS_DesignationInfo entity = _amsCommonService.AMSUnit.AnsarDesignationRepository.GetByID(id);
            DesignationInfoViewModel model = entity.ToModel();
            model.ActionType = "Edit";

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(DesignationInfoViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "edit");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    AMS_DesignationInfo entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _amsCommonService.AMSUnit.AnsarDesignationRepository.Update(entity);
                        _amsCommonService.AMSUnit.AnsarDesignationRepository.SaveChanges();
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

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            bool result = false;
            try
            {
                AMS_DesignationInfo designationInfo = _amsCommonService.AMSUnit.AnsarDesignationRepository.GetByID(id);
                _amsCommonService.AMSUnit.AnsarDesignationRepository.Delete(designationInfo);
                _amsCommonService.AMSUnit.AnsarDesignationRepository.SaveChanges();
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

        private string CheckDuplicate(DesignationInfoViewModel model, string strMode)
        {
            string message = string.Empty;
            dynamic designationInfo = null;

            if (strMode == "add")
            {
                designationInfo = _amsCommonService.AMSUnit.AnsarDesignationRepository.Get(x => x.DesignationName == model.DesignationName).FirstOrDefault();
            }
            else
            {
                designationInfo = _amsCommonService.AMSUnit.AnsarDesignationRepository.Get(x => x.DesignationName == model.DesignationName && x.Id != model.Id).FirstOrDefault();
            }

            if (designationInfo != null)
            {
                message += "Already exists. Please go back to list to edit.";
            }

            return message;

        }
    }
}
