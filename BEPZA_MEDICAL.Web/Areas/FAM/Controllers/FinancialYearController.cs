using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.FinancialYearInfo;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using System.Data.SqlClient;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{ 
    public class FinancialYearController : Controller
    {
        #region Fields
        //private readonly FAMCommonService _famCommonService;
        private readonly FAMFinancialYearService _famFinancialYearService;
        #endregion
        
        #region Ctor
        public FinancialYearController(FAMFinancialYearService famFinancialYearService)
        {
            _famFinancialYearService = famFinancialYearService;
        }

        #endregion

        #region Actions
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult BackToList()
        {
            var model = new FinancialYearInformationModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, FinancialYearInformationSearchModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (model != null)
                    filterExpression = model.GetFilterExpression();
            }

            totalRecords = _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.GetCount(filterExpression);

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var list = _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1);

            foreach (FAM_FinancialYearInformation d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.FinancialYearName,
                    d.FinancialYearStartDate.Date.ToString("dd-MMM-yyyy"),
                    d.FinancialYearEndDate.Date.ToString("dd-MMM-yyyy"),
                    d.FinancialYearVoucherFormat,
                    d.IsClose,
                    d.IsActive,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new FinancialYearInformationModel();
            PrepareModel(model);
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(FinancialYearInformationModel model)
        {
            string businessError = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    model.CompanyId = 1;
                    var entity = model.ToEntity();
                    businessError = _famFinancialYearService.CheckBusinessLogicFinancialYear(entity);
                    if (string.IsNullOrEmpty(businessError))
                    {
                        _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.Add(entity);
                        _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.SaveChanges();
                    }
                    

                    
                }
                catch (Exception)
                {

                    return new JsonResult()
                    {
                        Data = ErrorMessages.InsertFailed
                    };
                }

                return new JsonResult()
                {
                    Data = string.IsNullOrEmpty(businessError) ? ErrorMessages.InsertSuccessful : businessError
                };
            }

            var errors = ModelState
                           .Where(x => x.Value.Errors.Count > 0)
                           .Select(x => new { x.Key, x.Value.Errors })
                           .ToArray();

            return new JsonResult()
            {
                Data = errors.Count() > 0 ? errors.First().Errors.First().ErrorMessage : ""
            };
        }

        public ActionResult Edit(int Id)
        {
            var entity = _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.GetByID(Id);
            var model = entity.ToModel();
            PrepareModel(model);
            model.Mode = "Edit";
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(FinancialYearInformationModel model)
        {
            string businessError = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    model.CompanyId = 1;
                    var entity = model.ToEntity();
                    businessError = _famFinancialYearService.CheckBusinessLogicFinancialYearEdit(entity);

                    var dbExistingFY = _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.GetAll();
                    if (string.IsNullOrEmpty(businessError))
                    {
                        if (model.IsActive == true)
                        {
                            foreach (var item in dbExistingFY)
                            {
                                item.IsActive = false;
                            }
                        }
                        _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.Update(entity);
                        _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.SaveChanges();
                    }
                }
                catch (Exception)
                {

                    return new JsonResult()
                    {
                        Data = ErrorMessages.UpdateFailed
                    };
                }

                return new JsonResult()
                {
                    Data =String.IsNullOrEmpty(businessError)? ErrorMessages.UpdateSuccessful:businessError
                };
            }
            return new JsonResult()
            {
                Data = ErrorMessages.UpdateFailed
            };
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                List<Type> allTypes = new List<Type> { typeof(FAM_FinancialYearInformation) };

                _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.Delete(id);
                _famFinancialYearService.FAMUnit.FinancialYearInformationRepository.SaveChanges();
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
            catch (Exception ex)
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

        #region Utils
        private void PrepareModel(FinancialYearInformationModel model)
        { 
        
        }

        [NoCache]
        public ActionResult IsClose()
        {
            Dictionary<string, string> close = new Dictionary<string, string>();
            close.Add("", "All");
            close.Add("true", "Yes");
            close.Add("false", "No");
            return PartialView("_SelectCheck", close);
        }

        public ActionResult IsActive()
        {
            Dictionary<string, string> active = new Dictionary<string, string>();
            active.Add("", "All");
            active.Add("true", "Yes");
            active.Add("false", "No");
            return PartialView("_SelectCheck", active);
        }
        #endregion
    }
}