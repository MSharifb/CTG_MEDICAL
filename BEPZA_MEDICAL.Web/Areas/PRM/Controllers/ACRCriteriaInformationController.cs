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
    public class ACRCriteriaInformationController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor

        public ACRCriteriaInformationController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        //
        // GET: /PRM/ACRCriteriaInformation/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ACRCriteriaInformationViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ACRCriteriaInformationViewModel> list = (from acrCri in _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.GetAll()
                                                          join catg in _prmCommonService.PRMUnit.PRM_StaffCategoryRepository.GetAll() on acrCri.StaffCategoryId equals catg.Id
                                                          where (model.StaffCategoryId == 0 || model.StaffCategoryId == acrCri.StaffCategoryId)
                                                          && (model.ACRCriteriaName == "" || model.ACRCriteriaName == null || model.ACRCriteriaName == acrCri.ACRCriteriaName)
                                                          select new ACRCriteriaInformationViewModel()
                                                          {
                                                              Id=acrCri.Id,
                                                              StaffCategoryId=acrCri.StaffCategoryId,
                                                              ACRCriteriaName=acrCri.ACRCriteriaName
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
                  d.StaffCategoryId,
                  d.ACRCriteriaName
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult StaffCategoryforView()
        {
            var itemList = Common.PopulateDllList(_prmCommonService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }

        public ActionResult Create()
        {
            ACRCriteriaInformationViewModel model = new ACRCriteriaInformationViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ACRCriteriaInformationViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = BusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var entity = model.ToEntity();
                try
                {
                    _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                populateDropdown(model);
                model.ErrMsg = errorList;
                return View(model);
            }

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.GetByID(Id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(ACRCriteriaInformationViewModel model)
        {
            model.IsError = 1;
            model.strMode = "Edit";
            model.ErrMsg = string.Empty;
            model.ErrMsg = BusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(model.ErrMsg)))
            {
                var entity = model.ToEntity();
                try
                {
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;

                    _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.Update(entity);
                    _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.SaveChanges();
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
                populateDropdown(model);
                return View(model);
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

            var tempPeriod = _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.GetByID(id);
            try
            {
                _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.Delete(id);
                _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.SaveChanges();
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
        [NoCache]
        public string BusinessLogicValidation(ACRCriteriaInformationViewModel model)
        {
            string errorMessage = string.Empty;
            if (model.strMode != "Edit")
            {
                var requInfo = _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.GetAll().Where(x => x.ACRCriteriaName == model.ACRCriteriaName).ToList();
                if (requInfo.Count > 0)
                {
                    errorMessage = "ACR Criteria Name Already Exist";
                }
            }
            return errorMessage;

        }
        private void populateDropdown(ACRCriteriaInformationViewModel model)
        {
            #region Staff Category
            var ddlList = _prmCommonService.PRMUnit.PRM_StaffCategoryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.StaffCategoryList = Common.PopulateDllList(ddlList);
            #endregion
        }
	}
}