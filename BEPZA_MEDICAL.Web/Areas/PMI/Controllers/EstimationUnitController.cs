using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation;
using BEPZA_MEDICAL.Web.Controllers;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class EstimationUnitController : BaseController
    {
        #region Fields
        private readonly PMICommonService _pmiCommonService;
        #endregion

        #region Ctor
        public EstimationUnitController(PMICommonService pmiCommonService)
        {
            this._pmiCommonService = pmiCommonService;
        }
        #endregion

        #region Action
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EstimationUnitViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<EstimationUnitViewModel> list = (from unit in _pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll()
                                                  select new EstimationUnitViewModel()
                                                  {
                                                      Id = unit.Id,
                                                      Name = unit.Name,
                                                      Code = unit.Code,
                                                      Description = unit.Description,
                                                      IsActive = unit.IsActive
                                                  }).OrderBy(x => x.Name).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Name).ToList();
                }
            }

            #endregion

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    list = list.Where(x => x.Name.Trim().ToLower().Contains(model.Name.Trim().ToLower())).ToList();
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                  d.Id,
                  d.Name,
                  d.Code,
                  d.Description,
                  d.IsActive==true ? "Yes" : "No",
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            EstimationUnitViewModel model = new EstimationUnitViewModel();
            model.IsActive = true;
            model.ActionType = @"Create";
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(EstimationUnitViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            var strMessage = CheckDuplicateEntry(model);
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
                    _pmiCommonService.PMIUnit.EstimationUnitRepository.Add(entity);
                    _pmiCommonService.PMIUnit.EstimationUnitRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
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
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _pmiCommonService.PMIUnit.EstimationUnitRepository.GetByID(Id);
            EstimationUnitViewModel model = entity.ToModel();
            model.ActionType = @"Edit";
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EstimationUnitViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;
            var strMessage = CheckDuplicateEntry(model);
            model.ErrMsg = strMessage;
            if (!string.IsNullOrWhiteSpace(strMessage))
            {
                model.errClass = "failed";
            }
            if (ModelState.IsValid && string.IsNullOrWhiteSpace(strMessage))
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _pmiCommonService.PMIUnit.EstimationUnitRepository.Update(entity);
                    _pmiCommonService.PMIUnit.EstimationUnitRepository.SaveChanges();
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
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;
            var tempPeriod = _pmiCommonService.PMIUnit.EstimationUnitRepository.GetByID(id);
            try
            {
                _pmiCommonService.PMIUnit.EstimationUnitRepository.Delete(id);
                _pmiCommonService.PMIUnit.EstimationUnitRepository.SaveChanges();
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

        private string CheckDuplicateEntry(EstimationUnitViewModel model)
        {
            string message = string.Empty;
            var itemList = new List<PMI_EstimationUnit>();
            string unitName = model.Name;
            string shortCode = model.Code;
            string actionType = model.ActionType;
            switch (actionType)
            {
                case "Create":
                    itemList = (from x in _pmiCommonService.PMIUnit.EstimationUnitRepository.Get(t => t.Name == unitName || t.Code == shortCode)
                                select x).DefaultIfEmpty().OfType<PMI_EstimationUnit>().ToList();
                    break;
                case "Edit":
                    itemList = (from x in _pmiCommonService.PMIUnit.EstimationUnitRepository.Get(t => t.Id != model.Id && (t.Name == unitName || t.Code == shortCode))
                                select x).DefaultIfEmpty().OfType<PMI_EstimationUnit>().ToList();
                    break;
            }
            
            if (itemList.Any())
            {
                message = @"Duplicate Name / Code";
            }
            return message;
        }
        #endregion
    }
}