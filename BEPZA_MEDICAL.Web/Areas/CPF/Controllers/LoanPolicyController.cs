using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.LoanPolicy;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class LoanPolicyController : Controller
    {
        #region Fields

        private readonly CPFCommonService _cpfCommonservice;


        #endregion

        public LoanPolicyController(CPFCommonService cpmCommonService)
        {
            this._cpfCommonservice = cpmCommonService;
        }
        // GET: CPF/LoanPolicy
        public ActionResult Index()
        {
            return View();
        }

        #region Action

        public ActionResult Create()
        {
            var model = new LoanPolicyViewModel();
            PopulateDropdown(model);
            model.ActionType = @"Create";
            return View("_CreateOrEdit", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, LoanPolicyViewModel model)
        {
            string filterExpression = string.Empty;
            int totalRecords = 0;

            var list = (from x in _cpfCommonservice.CPFUnit.LoanPolicyRepository.GetAll().ToList()
                        select new LoanPolicyViewModel()
                        {
                            Id = x.Id,
                            LoanPolicyFor = x.LoanPolicyFor,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate
                        }).ToList();


            if (request.Searching)
            {
                if (!string.IsNullOrWhiteSpace(model.LoanPolicyFor))
                {
                    list = list.Where(d => d.LoanPolicyFor == model.LoanPolicyFor).ToList();
                }
                if (model.StartDate != null && model.EndDate != null)
                {
                    list = list.Where(d => d.StartDate >= model.StartDate && d.EndDate <= model.EndDate).ToList();
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {                 d.Id ,
                                  d.LoanPolicyFor ,
                                  d.StartDate!=null?Convert.ToDateTime(d.StartDate).ToString(DateAndTime.GlobalDateFormat):string.Empty ,
                                  d.EndDate!=null?Convert.ToDateTime(d.EndDate).ToString(DateAndTime.GlobalDateFormat):string.Empty,
                                  "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(LoanPolicyViewModel model)
        {
            try
            {
                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    var obj = model.ToEntity();
                    message = CheckDuplicateEntry(model);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        PopulateDropdown(model);
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = message;
                        return View("_CreateOrEdit", model);
                    }

                    _cpfCommonservice.CPFUnit.LoanPolicyRepository.Add(obj);
                    _cpfCommonservice.CPFUnit.LoanPolicyRepository.SaveChanges();

                    message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = message;
                }
                else
                {
                    model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(model.ErrMsg) ? ErrorMessages.InsertFailed : model.ErrMsg) : Common.GetModelStateError(ModelState);
                }
                ModelState.Clear();
                PopulateDropdown(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                PopulateDropdown(model);
                model.IsError = 1;
                model.errClass = "failed";
                return View("_CreateOrEdit", model);
            }
            return View("Index", model);
        }


        public ActionResult Edit(int id)
        {
            var model = new LoanPolicyViewModel();
            var obj = _cpfCommonservice.CPFUnit.LoanPolicyRepository.GetByID(id);
            if (obj != null)
            {
                model = obj.ToModel();
            }
            PopulateDropdown(model);
            model.ActionType = "Edit";
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(LoanPolicyViewModel model)
        {
            try
            {
                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    var obj = model.ToEntity();
                    message = CheckDuplicateEntry(model);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        PopulateDropdown(model);
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = message;
                        return View("_CreateOrEdit", model);
                    }

                    _cpfCommonservice.CPFUnit.LoanPolicyRepository.Update(obj);
                    _cpfCommonservice.CPFUnit.LoanPolicyRepository.SaveChanges();

                    message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = message;
                }
                else
                {
                    model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(model.ErrMsg) ? ErrorMessages.UpdateFailed : model.ErrMsg) : Common.GetModelStateError(ModelState);
                }
                ModelState.Clear();
                PopulateDropdown(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                PopulateDropdown(model);
                model.IsError = 1;
                model.errClass = "failed";
                return View("_CreateOrEdit", model);
            }
            return View("Index", model);
        }

        [NoCache]
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                _cpfCommonservice.CPFUnit.LoanPolicyRepository.Delete(id);
                _cpfCommonservice.CPFUnit.LoanPolicyRepository.SaveChanges();
                result = true;
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        ModelState.AddModelError("Error", errMsg);
                    }
                    else
                    {
                        ModelState.AddModelError("Error", errMsg);
                    }
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? "Information has been deleted successfully." : errMsg
            });
        }

        #endregion

        #region Methods

        private LoanPolicyViewModel PopulateDropdown(LoanPolicyViewModel model)
        {
            model.LoanPolicyForList = Common.PopulatePfLoanPolicyForDdl();
            model.NRfApplicableForList = Common.PopulatePfApplicableForDdl();
            model.RfApplicableForList = Common.PopulatePfApplicableForDdl();
            model.RfBalanceDeductionConfirmationList = Common.PopulateYesNoDDLList();
            model.RfInstallmentBalanceAdditionConformationList = Common.PopulateYesNoDDLList();
            return model;
        }

        private string CheckDuplicateEntry(LoanPolicyViewModel model)
        {
            string errorMessage = string.Empty;
            string actionType = model.ActionType.Trim();

            DateTime maxDate = DateTime.Now, minDate = DateTime.Now;
            var loanPolicy = new List<CPF_LoanPolicy>();
            loanPolicy = _cpfCommonservice.CPFUnit.LoanPolicyRepository.GetAll().DefaultIfEmpty().OfType<CPF_LoanPolicy>().ToList();
            if (loanPolicy != null && loanPolicy.Count > 0)
            {
                maxDate = loanPolicy.Max(t => t.EndDate);
                minDate = loanPolicy.Min(t => t.StartDate);
            }
            switch (actionType)
            {
                case "Create":
                    loanPolicy = loanPolicy.Where(t => t.LoanPolicyFor == model.LoanPolicyFor).DefaultIfEmpty().OfType<CPF_LoanPolicy>().ToList();
                    loanPolicy = loanPolicy.Where(t => t.StartDate >= minDate && t.EndDate <= maxDate).DefaultIfEmpty().OfType<CPF_LoanPolicy>().ToList();
                    break;
                case "Edit":
                    loanPolicy = loanPolicy.Where(t => t.LoanPolicyFor == model.LoanPolicyFor).DefaultIfEmpty().OfType<CPF_LoanPolicy>().ToList();
                    loanPolicy = loanPolicy.Where(t => t.StartDate >= model.StartDate && t.EndDate <= model.EndDate).DefaultIfEmpty().OfType<CPF_LoanPolicy>().ToList();
                    loanPolicy = loanPolicy.Where(t => t.Id != model.Id).DefaultIfEmpty().OfType<CPF_LoanPolicy>().ToList();
                    break;
            }
            if (loanPolicy != null && loanPolicy.Count() > 0)
            {
                errorMessage = @"Policy has been exists for period : " + model.StartDate.Value.ToString(DateAndTime.GlobalDateFormat) + "  To  " + model.EndDate.Value.ToString("dd-MMM-yyyy");
            }

            return errorMessage;
        }

        [NoCache]
        public ActionResult GetLoanPolicyFor()
        {
            return View("_Select", Common.PopulatePfLoanPolicyForDdl());
        }

        #endregion
    }
}