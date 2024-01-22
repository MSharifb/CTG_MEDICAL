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
    public class ACRRankInformationController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor

        public ACRRankInformationController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        //
        // GET: /PRM/ACRRankInformation/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ACRRankInformationViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ACRRankInformationViewModel> list = (from acrRnk in _prmCommonService.PRMUnit.ACRRankInformationRepository.GetAll()
                                                      where (model.RankName == "" || model.RankName == acrRnk.RankName|| model.RankName==null)
                                                      select new ACRRankInformationViewModel()
                                                       {
                                                        Id = acrRnk.Id,
                                                        RankName=acrRnk.RankName
                                                       }).ToList();


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
                {
                  d.Id,
                  d.RankName,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ACRRankInformationViewModel model = new ACRRankInformationViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ACRRankInformationViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var entity = model.ToEntity();
                try
                {
                    _prmCommonService.PRMUnit.ACRRankInformationRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRRankInformationRepository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.ErrMsg = errorList;
                return View(model);
            }

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.ACRRankInformationRepository.GetByID(Id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(ACRRankInformationViewModel model)
        {
            model.IsError = 1;
            model.strMode = "Edit";
            model.ErrMsg = string.Empty;

            if (ModelState.IsValid && (string.IsNullOrEmpty(model.ErrMsg)))
            {
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;

                    _prmCommonService.PRMUnit.ACRRankInformationRepository.Update(entity);
                    _prmCommonService.PRMUnit.ACRRankInformationRepository.SaveChanges();
                    model.IsError = 1;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }
                catch (Exception ex)
                {
                    model.IsError = 0;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                return View(model);
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.ACRRankInformationRepository.GetByID(id);
            try
            {
                _prmCommonService.PRMUnit.ACRRankInformationRepository.Delete(id);
                _prmCommonService.PRMUnit.ACRRankInformationRepository.SaveChanges();
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