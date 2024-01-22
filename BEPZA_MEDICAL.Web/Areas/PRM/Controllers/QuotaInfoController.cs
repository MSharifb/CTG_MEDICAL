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
    public class QuotaInfoController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        #endregion

        #region Constructor

        public QuotaInfoController(PRMCommonSevice prmCommonSevice)
        {
            this._prmCommonSevice = prmCommonSevice;
        }

        #endregion
        //
        // GET: /PRM/QuotaInfo/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, QuotaInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<QuotaInfoViewModel> list = (from quo in _prmCommonSevice.PRMUnit.QuotaInfoRepository.GetAll()
                                             join quoName in _prmCommonSevice.PRMUnit.QuotaNameRepository.GetAll() on quo.QuotaNameId equals quoName.Id
                                             where (model.QuotaNameId == 0 || model.QuotaNameId ==quo.QuotaNameId)
                                                                select new QuotaInfoViewModel()
                                                                {
                                                                    Id=quo.Id,
                                                                    QuotaNameId=quo.QuotaNameId,
                                                                    QuotaName=quoName.Name,
                                                                    FirstAndSecondClsOfficer=quo.FirstAndSecondClsOfficer,
                                                                    ThirdAndForthClsStaff=quo.ThirdAndForthClsStaff
                                                                }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "QuotaName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.QuotaName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.QuotaName).ToList();
                }
            }


            if (request.SortingName == "FirstAndSecondClsOfficer")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FirstAndSecondClsOfficer).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FirstAndSecondClsOfficer).ToList();
                }
            }
            if (request.SortingName == "ThirdAndForthClsStaff")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ThirdAndForthClsStaff).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ThirdAndForthClsStaff).ToList();
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
                  d.QuotaNameId,
                  d.QuotaName,
                  d.FirstAndSecondClsOfficer,
                  d.ThirdAndForthClsStaff
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }
        [NoCache]
        public ActionResult QuotaforView()
        {
            var itemList = Common.PopulateDllList(_prmCommonSevice.PRMUnit.QuotaNameRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }
        public ActionResult Create()
        {
            QuotaInfoViewModel model = new QuotaInfoViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(QuotaInfoViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();

                try
                {
                    _prmCommonSevice.PRMUnit.QuotaInfoRepository.Add(entity);
                    _prmCommonSevice.PRMUnit.QuotaInfoRepository.SaveChanges();
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
            var entity = _prmCommonSevice.PRMUnit.QuotaInfoRepository.GetByID(Id);
            var model = entity.ToModel();
            populateDropdown(model);

            if (type == "success")
            {
                model.IsError = 1;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            else if (type == "saveSuccess")
            {
                model.IsError = 1;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(QuotaInfoViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    _prmCommonSevice.PRMUnit.QuotaInfoRepository.Update(entity);
                    _prmCommonSevice.PRMUnit.QuotaInfoRepository.SaveChanges();
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

            var tempPeriod = _prmCommonSevice.PRMUnit.QuotaInfoRepository.GetByID(id);
            try
            {
                _prmCommonSevice.PRMUnit.QuotaInfoRepository.Delete(id);
                _prmCommonSevice.PRMUnit.QuotaInfoRepository.SaveChanges();
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

        private void populateDropdown(QuotaInfoViewModel model)
        {
            #region Division
            var quoList = _prmCommonSevice.PRMUnit.QuotaNameRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.QuotaNameList = Common.PopulateDllList(quoList);
            #endregion
        }

	}
}