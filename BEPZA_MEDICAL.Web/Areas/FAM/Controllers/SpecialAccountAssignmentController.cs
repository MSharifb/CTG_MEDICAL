using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.FAM;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.SpecialAccountAssignment;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.DAL.FAM;
using System.Data;
using System.Data.SqlClient;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class SpecialAccountAssignmentController : Controller
    {
        #region Fields
        private readonly FAMCommonService _famCommonService;
        #endregion

        #region Ctor
        public SpecialAccountAssignmentController(FAMCommonService famCommonService)
        {
            _famCommonService = famCommonService;
        }
        #endregion

        #region Actions
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult BackToList()
        {
            var model = new SpecialAccountAssignmentModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SpecialAccountAssignmentModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _famCommonService.GetSpecialAccountAssignmentSearchedList(model.PurposeId);

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
                    d.PurposeName,
                    d.AccountHeadName,
                    d.Remarks,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new SpecialAccountAssignmentModel();
            PrepareModel(model);
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(SpecialAccountAssignmentModel model)
        {
            string strMessage = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    _famCommonService.FAMUnit.PurposeAccountHeadMappingRepository.Add(entity);
                    _famCommonService.FAMUnit.PurposeAccountHeadMappingRepository.SaveChanges();
                    strMessage = ErrorMessages.InsertSuccessful;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        strMessage = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        strMessage = ErrorMessages.InsertFailed;
                    }                   
                }               
            }
            return new JsonResult()
            {
                Data = strMessage
            };           
        }

        public ActionResult Edit(int id)
        {
            var entity = _famCommonService.FAMUnit.PurposeAccountHeadMappingRepository.GetByID(id);
            var model = entity.ToModel();

            PrepareModel(model);
            model.Mode = "Edit";
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(SpecialAccountAssignmentModel model)
        {
            string strMessage = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    _famCommonService.FAMUnit.PurposeAccountHeadMappingRepository.Update(entity);
                    _famCommonService.FAMUnit.PurposeAccountHeadMappingRepository.SaveChanges();
                    strMessage = ErrorMessages.UpdateSuccessful;
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
                }
            }
            return new JsonResult()
            {
                Data = strMessage
            };
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                List<Type> allTypes = new List<Type> { typeof(FAM_Purpose) };

                _famCommonService.FAMUnit.PurposeAccountHeadMappingRepository.Delete(id);
                _famCommonService.FAMUnit.PurposeAccountHeadMappingRepository.SaveChanges();
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
        private void PrepareModel(SpecialAccountAssignmentModel model)
        {
            model.PurposeList = Common.PopulateDllList(_famCommonService.FAMUnit.Purpose.GetAll());
            model.AccountHeadList = _famCommonService.FAMUnit.ChartOfAccount.GetAll().ToList()
                .Where(x => (bool)x.IsPostingAccount)
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.AccountHeadName,
                    Value = y.Id.ToString()
                }).ToList();
        }

        [NoCache]
        public ActionResult PurposeforView()
        {
            var itemList = Common.PopulateDllList(_famCommonService.FAMUnit.Purpose.GetAll()).ToList();
            return PartialView("_Select", itemList);
        }
        #endregion
    }
}
