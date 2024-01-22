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
    public class AutoReminderController : BaseController
    {
        #region Fields

        private readonly AMSCommonService _amsCommonService;

        #endregion

        #region Constructor

        public AutoReminderController(AMSCommonService amsCommonservice)
        {
            _amsCommonService = amsCommonservice;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAutoReminder(JqGridRequest request, AutoReminderViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _amsCommonService.AMSUnit.AutoReminderRepository.GetAll().OrderBy(x=> x.AMS_ReminderType.SortOrder).ToList();

            if (request.Searching)
            {
                if (viewModel.ReminderTypeId > 0)
                {
                    list = list.Where(x => x.ReminderTypeId == viewModel.ReminderTypeId).ToList();
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
                var ReminderType = d.AMS_ReminderType.Name;

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ReminderTypeId,
                    ReminderType,
                    d.ReminderAfter,
                    d.Remarks
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            AutoReminderViewModel model = new AutoReminderViewModel();
            model.ActionType = "Create";

            PopulateList(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(AutoReminderViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "add");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    try
                    {
                        AMS_AutoReminder entity = model.ToEntity();

                        _amsCommonService.AMSUnit.AutoReminderRepository.Add(entity);
                        _amsCommonService.AMSUnit.AutoReminderRepository.SaveChanges();

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
            AMS_AutoReminder entity = _amsCommonService.AMSUnit.AutoReminderRepository.GetByID(id);
            AutoReminderViewModel model = entity.ToModel();
            model.ActionType = "Edit";
            PopulateList(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(AutoReminderViewModel model)
        {
            string dupErrMessage = string.Empty;

            if (ModelState.IsValid)
            {
                dupErrMessage = CheckDuplicate(model, "edit");
                if (string.IsNullOrWhiteSpace(dupErrMessage))
                {
                    AMS_AutoReminder entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _amsCommonService.AMSUnit.AutoReminderRepository.Update(entity);
                        _amsCommonService.AMSUnit.AutoReminderRepository.SaveChanges();
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
                AMS_AutoReminder autoReminderInfo = _amsCommonService.AMSUnit.AutoReminderRepository.GetByID(id);
                _amsCommonService.AMSUnit.AutoReminderRepository.Delete(autoReminderInfo);
                _amsCommonService.AMSUnit.AutoReminderRepository.SaveChanges();
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

        private string CheckDuplicate(AutoReminderViewModel model, string strMode)
        {
            string message = string.Empty;
            dynamic dupAutoReminder = null;

            if (strMode == "add")
            {
                dupAutoReminder = _amsCommonService.AMSUnit.AutoReminderRepository.Get(x => x.ReminderTypeId == model.ReminderTypeId).FirstOrDefault();
            }
            else
            {
                if (_amsCommonService.AMSUnit.AutoReminderRepository.GetByID(model.Id).ReminderTypeId != model.ReminderTypeId)
                {
                    message += "Please go back to list to add new reminder type.";
                    return message;
                }
                dupAutoReminder = _amsCommonService.AMSUnit.AutoReminderRepository.Get(x => x.ReminderTypeId == model.ReminderTypeId && x.Id != model.Id).FirstOrDefault();
            }

            if (dupAutoReminder != null)
            {
                message += "Reminder already set for this type. Please go back to list to edit.";
                return message;
            }

            var autoReminderList = _amsCommonService.AMSUnit.AutoReminderRepository.GetAll();

            var previousReminder = autoReminderList.Where(x => x.ReminderTypeId < model.ReminderTypeId).FirstOrDefault();
            var nextReminder = autoReminderList.Where(x => x.ReminderTypeId > model.ReminderTypeId).FirstOrDefault();

            if (previousReminder != null && previousReminder.ReminderAfter >= model.ReminderAfter)
            {
                message += "Reminder after must be greater than previous reminder.";
            }

            if (nextReminder != null && nextReminder.ReminderAfter <= model.ReminderAfter)
            {
                message += " Reminder after must be less than next reminder.";
            }
              
            return message;

        }

        #region Others

        private void PopulateList(AutoReminderViewModel model)
        {
            model.ReminderTypeList = Common.PopulateDllList(_amsCommonService.AMSUnit.ReminderTypeRepository.GetAll().OrderBy(x => x.Name));
        }

        [NoCache]
        public ActionResult GetReminderTypeView()
        {
            Dictionary<string, string> reminderType = new Dictionary<string, string>();
            var reminderTypelist = _amsCommonService.AMSUnit.ReminderTypeRepository.GetAll().OrderBy(x => x.Name).ToList();

            foreach (var item in reminderTypelist)
            {
                reminderType.Add(item.Id.ToString(), item.Name);
            }

            return PartialView("_Select", reminderType);
        }

        #endregion
    }
}
