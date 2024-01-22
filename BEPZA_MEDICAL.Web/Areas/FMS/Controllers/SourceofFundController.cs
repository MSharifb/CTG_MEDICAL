using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class SourceofFundController : Controller
    {
        #region Fields
        private readonly FMSCommonService _fmsCommonService;
        #endregion

        #region Ctor
        public SourceofFundController(FMSCommonService fmsCommonService)
        {
            this._fmsCommonService = fmsCommonService;
        }
        #endregion

        // GET: FMS/SourceofFund
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SourceofFundViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from fdt in _fmsCommonService.FMSUnit.SourceofFundRepository.GetAll()
                        select new SourceofFundViewModel()
                        {
                            Id = fdt.Id,
                            Name = fdt.Name,
                            SortOrder = fdt.SortOrder,
                            Remarks = fdt.Remarks
                        }).ToList();

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
                    d.Name,
                    d.SortOrder,
                    d.Remarks
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult Create()
        {
            SourceofFundViewModel model = new SourceofFundViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SourceofFundViewModel model)
        {
            string errorList = string.Empty;
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _fmsCommonService.FMSUnit.SourceofFundRepository.Add(entity);
                    _fmsCommonService.FMSUnit.SourceofFundRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            else
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = errorList;
            }
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var entity = _fmsCommonService.FMSUnit.SourceofFundRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SourceofFundViewModel model)
        {
            string errorList = string.Empty;
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _fmsCommonService.FMSUnit.SourceofFundRepository.Update(entity);
                    _fmsCommonService.FMSUnit.SourceofFundRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
                }
                catch
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            else
            {
                model.errClass = "failed";
                model.IsError = 1;
                model.ErrMsg = errorList;
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _fmsCommonService.FMSUnit.SourceofFundRepository.GetByID(id);
            try
            {
                _fmsCommonService.FMSUnit.SourceofFundRepository.Delete(id);
                _fmsCommonService.FMSUnit.SourceofFundRepository.SaveChanges();
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

    }
}