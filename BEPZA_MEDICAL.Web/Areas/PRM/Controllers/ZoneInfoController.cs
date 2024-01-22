using BEPZA_MEDICAL.DAL.PRM;
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
    public class ZoneInfoController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        #endregion

        #region Ctor
        public ZoneInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonSevice = prmCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: /PRM/ZoneInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ZoneInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ZoneInfoViewModel> list = (from zoneInfo in _prmCommonSevice.PRMUnit.ZoneInfoRepository.GetAll()
                                            where (string.IsNullOrEmpty(model.ZoneName) || zoneInfo.ZoneName.Contains(model.ZoneName))
                                            select new ZoneInfoViewModel()
                                            {
                                                Id = zoneInfo.Id,
                                                ZoneName = zoneInfo.ZoneName,
                                                ZoneCode = zoneInfo.ZoneCode,
                                                SortOrder = zoneInfo.SortOrder,
                                                Prefix = zoneInfo.Prefix,
                                                ZoneCodeForBillingSystem = zoneInfo.ZoneCodeForBillingSystem
                                            }).OrderBy(x => x.SortOrder).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ZoneName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ZoneName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ZoneName).ToList();
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
                  d.ZoneName,
                  d.ZoneCode,
                  d.Prefix,
                  d.ZoneCodeForBillingSystem,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ZoneInfoViewModel model = new ZoneInfoViewModel();
            PopuladeDDL(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(ZoneInfoViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();

                try
                {
                    _prmCommonSevice.PRMUnit.ZoneInfoRepository.Add(entity);
                    _prmCommonSevice.PRMUnit.ZoneInfoRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    PopuladeDDL(model);
                }
                catch
                {
                    PopuladeDDL(model);
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonSevice.PRMUnit.ZoneInfoRepository.GetByID(Id);
            ZoneInfoViewModel model = entity.ToModel();
            PopuladeDDL(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ZoneInfoViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    _prmCommonSevice.PRMUnit.ZoneInfoRepository.Update(entity);
                    _prmCommonSevice.PRMUnit.ZoneInfoRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    PopuladeDDL(model);
                }
                catch
                {
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                    PopuladeDDL(model);
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
            var tempPeriod = _prmCommonSevice.PRMUnit.ZoneInfoRepository.GetByID(id);
            try
            {
                _prmCommonSevice.PRMUnit.ZoneInfoRepository.Delete(id);
                _prmCommonSevice.PRMUnit.ZoneInfoRepository.SaveChanges();
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

        public void PopuladeDDL(ZoneInfoViewModel model)
        {
            model.OrganogramCategoryTypeList =
                _prmCommonSevice.PRMUnit.OrganogramCategoryTypeRepository.GetAll().ToList()
                .Select(y =>
                    new SelectListItem()
                    {
                        Text = y.Name.ToString(),
                        Value = y.Id.ToString()
                    }).ToList();
        }

        #endregion
    }
}