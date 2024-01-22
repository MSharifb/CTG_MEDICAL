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
    public class SparePartInformationController : Controller
    {
        #region Fields
        private readonly FARCommonService _farCommonService;
        #endregion

        #region Ctor
        public SparePartInformationController(FARCommonService farCommonService)
        {
            this._farCommonService = farCommonService;
        }
        #endregion

        #region Action

        // GET: FAR/SparePartInformation

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SparePartInformationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from loc in _farCommonService.FARUnit.SparePartInformationRepository.GetAll()
                        select new SparePartInformationViewModel()

                        {
                            Id = loc.Id,
                            SparePart = loc.SparePart,
                            Code = loc.Code,
                            Remarks = loc.Remarks,
                        }).ToList();

            if (request.Searching)
            {

                if ((viewModel.SparePart != null && viewModel.SparePart != ""))
                {
                    list = list.Where(d => d.SparePart == viewModel.SparePart).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "SparePart")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SparePart).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SparePart).ToList();
                }
            }

            if (request.SortingName == "Code")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Code).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Code).ToList();
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
                    d.SparePart,
                    d.Code,
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
            SparePartInformationViewModel model = new SparePartInformationViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(SparePartInformationViewModel model)
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
                    _farCommonService.FARUnit.SparePartInformationRepository.Add(entity);
                    _farCommonService.FARUnit.SparePartInformationRepository.SaveChanges();
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
            var entity = _farCommonService.FARUnit.SparePartInformationRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SparePartInformationViewModel model)
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
                    _farCommonService.FARUnit.SparePartInformationRepository.Update(entity);
                    _farCommonService.FARUnit.SparePartInformationRepository.SaveChanges();
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

        [NoCache]
        public string BusinessLogicValidation(SparePartInformationViewModel model, int id)
        {
            var exist = false;
            string errorMessage = string.Empty;
            if (id < 1)
            {
                exist = _farCommonService.FARUnit.SparePartInformationRepository.Get(q => q.SparePart == model.SparePart).Any();

                if (exist)
                {
                    return errorMessage = "Duplicate Spare Part Name";
                }
            }
            else
            {
                exist = _farCommonService.FARUnit.SparePartInformationRepository.Get(q => q.SparePart == model.SparePart && id != q.Id).Any();

                if (exist)
                {
                    return errorMessage = "Duplicate Spare Part Name";
                }
            }
            return errorMessage;

        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _farCommonService.FARUnit.SparePartInformationRepository.GetByID(id);
            try
            {
                _farCommonService.FARUnit.SparePartInformationRepository.Delete(id);
                _farCommonService.FARUnit.SparePartInformationRepository.SaveChanges();
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
        #endregion
    }
}