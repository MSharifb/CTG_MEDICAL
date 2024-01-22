using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class DistrictQuotaController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        #endregion

        #region Constructor

        public DistrictQuotaController(PRMCommonSevice prmCommonSevice)
        {
            this._prmCommonSevice = prmCommonSevice;
        }
        #endregion
        //
        // GET: /PRM/DistrictQuota/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, DistrictQuotaViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<DistrictQuotaViewModel> list = (from quo in _prmCommonSevice.PRMUnit.DistrictQuotaRepository.GetAll()
                                                 join div in _prmCommonSevice.PRMUnit.CountryDivisionRepository.GetAll() on quo.DivisionId equals div.Id
                                                 join dis in _prmCommonSevice.PRMUnit.DistrictRepository.GetAll() on quo.DistrictId equals dis.Id
                                                 where (model.DivisionId == 0 || model.DivisionId == quo.DivisionId)
                                                 && (model.DistrictId == 0 || model.DistrictId == quo.DistrictId)
                                                 select new DistrictQuotaViewModel()
                                             {
                                                 Id = quo.Id,
                                                 DivisionId=quo.DivisionId,
                                                 DivisionName=div.DivisionName,
                                                 DistrictId=quo.DistrictId,
                                                 DistrictName=dis.DistrictName,
                                                 Population=quo.Population,
                                                 Percentage=quo.Percentage
                                             }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "DivisionName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DivisionName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DivisionName).ToList();
                }
            }


            if (request.SortingName == "DistrictName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DistrictName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DistrictName).ToList();
                }
            }
            if (request.SortingName == "Population")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Population).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Population).ToList();
                }
            }
            if (request.SortingName == "Percentage")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Percentage).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Percentage).ToList();
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
                  d.DivisionId,
                  d.DivisionName,
                  d.DistrictId,
                  d.DistrictName,
                  d.Population,
                  d.Percentage
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult DivisionforView()
        {
            var itemList = Common.PopulateCountryDivisionDDL(_prmCommonSevice.PRMUnit.CountryDivisionRepository.GetAll().OrderBy(x => x.DivisionName).ToList());
            return PartialView("Select", itemList);
        }

        [NoCache]
        public ActionResult DistrictforView()
        {
            var itemList = Common.PopulateDistrictDDL(_prmCommonSevice.PRMUnit.DistrictRepository.GetAll().OrderBy(x => x.DistrictName).ToList());
            return PartialView("Select", itemList);
        }

        public ActionResult Create()
        {
            DistrictQuotaViewModel model = new DistrictQuotaViewModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DistrictQuotaViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();

                try
                {
                    _prmCommonSevice.PRMUnit.DistrictQuotaRepository.Add(entity);
                    _prmCommonSevice.PRMUnit.DistrictQuotaRepository.SaveChanges();
                    model.IsError = 0;
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
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id, string type)
        {
            var entity = _prmCommonSevice.PRMUnit.DistrictQuotaRepository.GetByID(Id);
            var model = entity.ToModel();
            populateDropdown(model);

            if (type == "success")
            {
                model.IsError = 1;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(DistrictQuotaViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    _prmCommonSevice.PRMUnit.DistrictQuotaRepository.Update(entity);
                    _prmCommonSevice.PRMUnit.DistrictQuotaRepository.SaveChanges();
                    model.IsError = 1;
                    return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                }
                catch
                {
                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            populateDropdown(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonSevice.PRMUnit.DistrictQuotaRepository.GetByID(id);
            try
            {
                _prmCommonSevice.PRMUnit.DistrictQuotaRepository.Delete(id);
                _prmCommonSevice.PRMUnit.DistrictQuotaRepository.SaveChanges();
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

        private void populateDropdown(DistrictQuotaViewModel model)
        {
            #region Division
            var divList = _prmCommonSevice.PRMUnit.CountryDivisionRepository.Fetch().OrderBy(x => x.DivisionName).ToList();
            model.DivisionNameList = Common.PopulateCountryDivisionDDL(divList);
            #endregion
            #region District
            var disList = _prmCommonSevice.PRMUnit.DistrictRepository.Fetch().Where(t => t.DivisionId == model.DivisionId).OrderBy(x => x.DistrictName).ToList();
            model.DistrictNameList = Common.PopulateDistrictDDL(disList);
            #endregion
        }
        public ActionResult LoadDistrict(int divisionId)
        {
            var list = _prmCommonSevice.PRMUnit.DistrictRepository.Fetch().Where(t => t.DivisionId == divisionId).Select(x => new { Id = x.Id, Name = x.DistrictName }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet
            );
        }
	}
}