using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MFS_IWM.Domain.PRM;
using MFS_IWM.Domain.PIM;
using MFS_IWM.Domain.PGM;
using Lib.Web.Mvc.JQuery.JqGrid;
using MFS_IWM.Web.Areas.PGM.Models.BankAdviceLetterBodySetup;
using MFS_IWM.Web.Areas.PRM.ViewModel;
using MFS_IWM.Web.Areas.PRM.ViewModel.Employee;
using MFS_IWM.DAL.PGM;
using MFS_IWM.Web.Resources;
using MFS_IWM.Web.Utility;
using System.Collections;
using MFS_IWM.DAL.PGM.CustomEntities;
using MFS_IWM.DAL.PGM.WithheldSalaryClasses;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using MFS_IWM.Domain.FAM;
using System.Web.Script.Serialization;


namespace MFS_IWM.Web.Areas.PGM.Controllers
{
    public class BankAdviceLetterBodySetupController : Controller
    {        

         #region Fields

        private readonly PRMCommonSevice _prmCommonservice;
        private readonly PGMCommonService _pgmCommonservice;
        private readonly PIMCommonService _pimCommonservice;
        private readonly ResourceInfoService _RresourceInfoService;
        private readonly FAMCommonService _famCommonservice;
       
        #endregion
               
        #region Ctor

        public BankAdviceLetterBodySetupController(PRMCommonSevice prmCommonService, PIMCommonService pimCommonService, PGMCommonService pgmCommonservice, ResourceInfoService service, FAMCommonService famCommonservice)
        {
            this._prmCommonservice = prmCommonService;
            this._pimCommonservice = pimCommonService;
            this._pgmCommonservice = pgmCommonservice;            
            this._RresourceInfoService = service;
            this._famCommonservice = famCommonservice; 
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion 

        #region Actions
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, BankLetterBodyViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _pgmCommonservice.PGMUnit.BankAdviceLetterBody.GetAll().ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.LetterType,
                    d.LetterSubject,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        [NoCache]
        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        public ActionResult Create()
        {
            BankLetterBodyViewModel model = new BankLetterBodyViewModel();
            List<PGM_BankAdviceLetterVariables> itemVariables = _pgmCommonservice.PGMUnit.BankAdviceLetterVariables.GetAll().ToList();

            if (itemVariables.Count>0)
            {
                foreach (var item in itemVariables)
                {
                    BankAdviceLetterBodyVariable details = new BankAdviceLetterBodyVariable();
                    details.Id = item.Id;
                    details.VariableNames = item.VariableNames;
                    model.BankLetterBodyVariableList.Add(details);
                }
            }
            PopulateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BankLetterBodyViewModel model)
        {
            Message = CheckBusinessRule(model);
            model.IsError = 1;

            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                var entity = model.ToEntity();

                try
                {
                    _pgmCommonservice.PGMUnit.BankAdviceLetterBody.Add(entity);
                    _pgmCommonservice.PGMUnit.BankAdviceLetterBody.SaveChanges();
                    model.IsError = 0;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    }

                    else
                    {
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                    }
                }
            }
            else
            {
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
            }
            PopulateDropdown(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.BankAdviceLetterBody.GetByID(id, "Id");
            var model = entity.ToModel();

            List<PGM_BankAdviceLetterVariables> itemVariables = _pgmCommonservice.PGMUnit.BankAdviceLetterVariables.GetAll().ToList();

            if (itemVariables.Count > 0)
            {
                foreach (var item in itemVariables)
                {
                    BankAdviceLetterBodyVariable details = new BankAdviceLetterBodyVariable();
                    details.Id = item.Id;
                    details.VariableNames = item.VariableNames;
                    model.BankLetterBodyVariableList.Add(details);
                }
            }

            PopulateDropdown(model); 
            return View(model);
        }

        [NoCache]
        [HttpPost]
        public ActionResult Edit(BankLetterBodyViewModel model)
        {
            model.IsError = 1;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();

                try
                {
                    _pgmCommonservice.PGMUnit.BankAdviceLetterBody.Update(entity);
                    _pgmCommonservice.PGMUnit.BankAdviceLetterBody.SaveChanges();
                    model.IsError = 0;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    }

                    else
                    {
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                    }
                }
            }
            else
            {
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.UpdateFailed : Message) : Common.GetModelStateError(ModelState);
            }

            PopulateDropdown(model);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var entity = _pgmCommonservice.PGMUnit.BankAdviceLetterBody.GetByID(id, "Id");

                if (entity != null)
                {
                    try
                    {

                        _pgmCommonservice.PGMUnit.BankAdviceLetterBody.Delete(entity.Id, "Id", null);
                        _pgmCommonservice.PGMUnit.BankAdviceLetterBody.SaveChanges();
                        result = true;
                        errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);

                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null && ex.InnerException is SqlException)
                        {
                            SqlException sqlException = ex.InnerException as SqlException;
                            errMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                        }
                    }
                }
                else
                {
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
                }
           
            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [NoCache]
        public ActionResult BackToList()
        {
            return RedirectToAction("Index");
        }

        #endregion

        #region Other Actions
        [NoCache]
        public ActionResult GetLetterTypeList()
        {
            var model = new BankLetterBodyViewModel();
            model.LetterTypeList = Common.PopulateLetterTypeList().ToList();
            return View("_Select", model.LetterTypeList);
        }

        private void PopulateDropdown(BankLetterBodyViewModel model)
        {
            model.LetterTypeList = Common.PopulateLetterTypeList().Select(y => new SelectListItem()
            {
                Text = y.Text,
                Value = y.Value
            }).ToList();
        }

        private string CheckBusinessRule(BankLetterBodyViewModel model)
        {
            var obj =( from s in _pgmCommonservice.PGMUnit.BankAdviceLetterBody.GetAll() where s.LetterType == model.LetterType select s.LetterType).ToList();
            if (obj.Count > 0)
            {
                Message = "Duplicate entry. Please try again.";
            }
            return Message;
        }
        #endregion
    }
}
