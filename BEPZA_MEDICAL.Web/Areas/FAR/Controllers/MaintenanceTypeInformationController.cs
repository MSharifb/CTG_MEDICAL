using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class MaintenanceTypeInformationController : Controller
    {
        #region Fields
        private readonly FARCommonService _farCommonService;
        #endregion

        #region Ctor
        public MaintenanceTypeInformationController(FARCommonService farCommonService)
        {
            this._farCommonService = farCommonService;
        }
        #endregion


        // GET: FAR/MaintenanceTypeInformation
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, MaintenanceTypeInformationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from loc in _farCommonService.FARUnit.MaintenanceTypeInformationRepository.GetAll()
                        select new MaintenanceTypeInformationViewModel()

                        {
                            Id = loc.Id,
                            MaintenanceType = loc.MaintenanceType,
                            Remarks = loc.Remarks,
                        }).ToList();

            if (request.Searching)
            {

                if ((viewModel.MaintenanceType != null && viewModel.MaintenanceType != ""))
                {
                    list = list.Where(d => d.MaintenanceType == viewModel.MaintenanceType).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "MaintenanceType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MaintenanceType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MaintenanceType).ToList();
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
                    d.MaintenanceType,
                    d.Remarks
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            MaintenanceTypeInformationViewModel model = new MaintenanceTypeInformationViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(MaintenanceTypeInformationViewModel model)
        {
            string errorList = string.Empty;
            errorList = BusinessLogicValidation(model, model.Id);
            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.MaintenanceTypeInformationRepository.Add(entity);
                    _farCommonService.FARUnit.MaintenanceTypeInformationRepository.SaveChanges();
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
            var entity = _farCommonService.FARUnit.MaintenanceTypeInformationRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MaintenanceTypeInformationViewModel model)
        {
            string errorList = string.Empty;
            errorList = BusinessLogicValidation(model, model.Id);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.MaintenanceTypeInformationRepository.Update(entity);
                    _farCommonService.FARUnit.MaintenanceTypeInformationRepository.SaveChanges();
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

            populateDropdown(model);
            return View(model);
        }

        [NoCache]
        public string BusinessLogicValidation(MaintenanceTypeInformationViewModel model, int id)
        {
            var exist = false;
            string errorMessage = string.Empty;
            if (id < 1)
            {
                exist = _farCommonService.FARUnit.MaintenanceTypeInformationRepository.Get(q => q.MaintenanceType == model.MaintenanceType).Any();

                if (exist)
                {
                    return errorMessage = "Duplicate Maintenance Type";
                }
            }
            else
            {
                exist = _farCommonService.FARUnit.MaintenanceTypeInformationRepository.Get(q => q.MaintenanceType == model.MaintenanceType && id != q.Id).Any();

                if (exist)
                {
                    return errorMessage = "Duplicate Maintenance Type";
                }
            }
            return errorMessage;

        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _farCommonService.FARUnit.MaintenanceTypeInformationRepository.GetByID(id);
            try
            {
                _farCommonService.FARUnit.MaintenanceTypeInformationRepository.Delete(id);
                _farCommonService.FARUnit.MaintenanceTypeInformationRepository.SaveChanges();
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

        public void populateDropdown(MaintenanceTypeInformationViewModel model)
        {
            model.RedoTypeList = Common.PopulateFDRDurationTypeList();
        }

    }
}