using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class AccidentInformationController : BaseController
    {
        #region Fields
        private readonly FARCommonService _farCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public AccidentInformationController(FARCommonService farCommonService, PRMCommonSevice prmCommonService)
        {
            this._farCommonService = farCommonService;
            this._prmCommonService = prmCommonService;
        }
        #endregion

        // GET: FAR/AccidentInformation
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AccidentInformationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from loc in _farCommonService.FARUnit.AccidentInformationRepository.GetAll()
                        join FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll() on loc.FIxedAssetId equals FA.Id
                        where (viewModel.FIxedAssetId == 0 || viewModel.FIxedAssetId == loc.FIxedAssetId)
                        &&(LoggedUserZoneInfoId==loc.ZoneInfoId)
                        select new AccidentInformationViewModel()
                        {
                            Id = loc.Id,
                            FIxedAssetId=loc.FIxedAssetId,
                            AssetCode=FA.AssetCode,
                            Driver=loc.Driver,
                            Date=loc.Date
                        }).ToList();

            if (request.Searching)
            {
                if ((viewModel.FromDate != null && viewModel.FromDate != DateTime.MinValue) && (viewModel.ToDate != null && viewModel.ToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date >= viewModel.FromDate && d.Date <= viewModel.ToDate).ToList();
                }
                else if ((viewModel.FromDate != null && viewModel.FromDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date >= viewModel.FromDate).ToList();
                }
                else if ((viewModel.ToDate != null && viewModel.ToDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date <= viewModel.ToDate).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "AssetCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetCode).ToList();
                }
            }
            if (request.SortingName == "Driver")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Driver).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Driver).ToList();
                }
            }
            if (request.SortingName == "Date")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Date).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Date).ToList();
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
                    d.FIxedAssetId,
                    d.AssetCode,
                    d.Driver,
                    (Convert.ToDateTime( d.Date)).ToString(DateAndTime.GlobalDateFormat),
                    d.FromDate,
                    d.ToDate
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult AssetCodeforView()
        {
            var itemList = Common.PopulateAssetCodeDDL(_farCommonService.FARUnit.FixedAssetRepository.GetAll().ToList());
            return PartialView("Select", itemList);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            AccidentInformationViewModel model = new AccidentInformationViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(AccidentInformationViewModel model)
        {
            string errorList = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = DateTime.Now;
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.AccidentInformationRepository.Add(entity);
                    _farCommonService.FARUnit.AccidentInformationRepository.SaveChanges();
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
            populateDropdown(model);
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var entity = _farCommonService.FARUnit.AccidentInformationRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AccidentInformationViewModel model)
        {
            string errorList = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.AccidentInformationRepository.Update(entity);
                    _farCommonService.FARUnit.AccidentInformationRepository.SaveChanges();
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
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
            }

            populateDropdown(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _farCommonService.FARUnit.AccidentInformationRepository.GetByID(id);
            try
            {
                _farCommonService.FARUnit.AccidentInformationRepository.Delete(id);
                _farCommonService.FARUnit.AccidentInformationRepository.SaveChanges();
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

        public void populateDropdown(AccidentInformationViewModel model)
        {
            dynamic ddl;
            ddl = _farCommonService.FARUnit.FixedAssetRepository.GetAll();
            model.AssetCodeList = Common.PopulateAssetCodeDDL(ddl);

            ddl = _prmCommonService.PRMUnit.LocationRepository.GetAll();
            model.LocationList = Common.PopulateDllList(ddl);
        }

    }
}