using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Controllers;
using System.Text.RegularExpressions;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class EstimationHeadController : BaseController
    {
        #region Fields
        private readonly PMICommonService _pmiCommonService;
        #endregion

        #region Ctor
        public EstimationHeadController(PMICommonService pmiCommonService)
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
        public ActionResult GetList(JqGridRequest request, EstimationHeadViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<EstimationHeadViewModel> list = (from unit in _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll()
                                                  select new EstimationHeadViewModel()
                                            {
                                                Id = unit.Id,
                                                HeadName = unit.HeadName,
                                                Description = unit.Description,
                                                IsActive = Convert.ToBoolean(unit.IsActive),
                                                ItemCode = unit.ItemCode == null ? string.Empty : unit.ItemCode
                                            }).OrderBy(x => x.HeadName).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HeadName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HeadName).ToList();
                }
            }

            #endregion

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.HeadName))
                {
                    list = list.Where(x => x.HeadName.Trim().ToLower().Contains(model.HeadName.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(model.ItemCode))
                {
                    list = list.Where(x => x.ItemCode.Trim().ToLower().Contains(model.ItemCode.Trim().ToLower())).ToList();
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
                  d.ItemCode,
                  d.HeadName,
                  d.Description,
                  d.IsActive == true ? "Yes" : "No",
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            EstimationHeadViewModel model = new EstimationHeadViewModel();
            model.IsActive = true;
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(EstimationHeadViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            RemoveSpace(model, model.HeadName);
            var strMessage = CheckDuplicateEntry(model, model.HeadName, model.ItemCode);
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
                    if (string.IsNullOrWhiteSpace(entity.ItemCode))
                    {
                        entity.ItemCode = string.Empty;
                    }
                    _pmiCommonService.PMIUnit.EstimationHeadRepository.Add(entity);
                    _pmiCommonService.PMIUnit.EstimationHeadRepository.SaveChanges();
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
            var entity = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetByID(Id);
            EstimationHeadViewModel model = entity.ToModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EstimationHeadViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;
            RemoveSpace(model, model.HeadName);
            var strMessage = CheckDuplicateEntry(model, model.HeadName, model.ItemCode);
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
                    if (string.IsNullOrWhiteSpace(entity.ItemCode))
                    {
                        entity.ItemCode = string.Empty;
                    }
                    _pmiCommonService.PMIUnit.EstimationHeadRepository.Update(entity);
                    _pmiCommonService.PMIUnit.EstimationHeadRepository.SaveChanges();
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
            var tempPeriod = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetByID(id);
            try
            {
                _pmiCommonService.PMIUnit.EstimationHeadRepository.Delete(id);
                _pmiCommonService.PMIUnit.EstimationHeadRepository.SaveChanges();
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

        private EstimationHeadViewModel RemoveSpace(EstimationHeadViewModel model, string headName)
        {
            var str = Regex.Replace(headName, " {2,}", " ");
            model.HeadName = str;
            return model;
        }

        private string CheckDuplicateEntry(EstimationHeadViewModel model, string headName, string itemCode)
        {
            string message = string.Empty;
            var itemTypes = new List<PMI_EstimationHead>();
            var itemCodes = new List<PMI_EstimationHead>();

            var headList = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(headName))
            {
                itemTypes = (from x in headList
                             where x.HeadName == headName && x.Id != model.Id
                             select x).DefaultIfEmpty().OfType<PMI_EstimationHead>().ToList();
            }
            if (!string.IsNullOrWhiteSpace(itemCode))
            {
                itemCodes = (from x in headList
                             where x.ItemCode == itemCode && x.Id != model.Id
                             select x).DefaultIfEmpty().OfType<PMI_EstimationHead>().ToList();
            }
            if (itemTypes.Any())
            {
                message += "Estimation Head: " + model.HeadName + " already exists.";
            }
            if (itemCodes.Any())
            {
                message += "Item Code: " + model.ItemCode + " already exists.";
            }
            return message;
        }
        #endregion
    }
}