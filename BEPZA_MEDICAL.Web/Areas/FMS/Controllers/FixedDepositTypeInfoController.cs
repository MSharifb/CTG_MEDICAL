using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class FixedDepositTypeInfoController : BaseController
    {
        #region Fields
        private readonly FMSCommonService _fmsCommonService;
        #endregion

        #region Ctor
        public FixedDepositTypeInfoController(FMSCommonService fmsCommonService)
        {
            this._fmsCommonService = fmsCommonService;
        }
        #endregion

        // GET: FMS/FixedDepositTypeInfo
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, FixedDepositTypeInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from fdt in _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.GetAll()
                        where (viewModel.FixedDepositType == null || viewModel.FixedDepositType == string.Empty || viewModel.FixedDepositType.Contains(fdt.FixedDepositType))
                        select new FixedDepositTypeInfoViewModel()
                        {
                            Id = fdt.Id,
                            FixedDepositType = fdt.FixedDepositType,
                            InterestRate=fdt.InterestRate,
                            Tax=fdt.Tax

                        }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "FixedDepositType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FixedDepositType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FixedDepositType).ToList();
                }
            }

            if (request.SortingName == "InterestRate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.InterestRate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.InterestRate).ToList();
                }
            }
            if (request.SortingName == "Tax")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Tax).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Tax).ToList();
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
                    d.FixedDepositType,
                    d.InterestRate,
                    d.Tax
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult Create()
        {
            FixedDepositTypeInfoViewModel model = new FixedDepositTypeInfoViewModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(FixedDepositTypeInfoViewModel model)
        {
            string errorList = string.Empty;
            errorList = BusinessLogicValidation(model, model.Id);
            model.Tax = model.Tax == null ? 0 : model.Tax;
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    try
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        var entity = model.ToEntity();
                        _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.Add(entity);
                        _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.SaveChanges();
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
            var entity = _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(FixedDepositTypeInfoViewModel model)
        {
            string errorList = string.Empty;
            errorList = BusinessLogicValidation(model, model.Id);
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    try
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        var entity = model.ToEntity();
                        _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.Update(entity);
                        _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.SaveChanges();
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
        public string BusinessLogicValidation(FixedDepositTypeInfoViewModel model, int id)
        {
            var exist = false;
            string errorMessage = string.Empty;
            if (id < 1)
            {
                exist = _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.Get(q => q.FixedDepositType == model.FixedDepositType).Any();

                if (exist)
                {
                    return errorMessage = "Duplicate Fixed Deposit Type";
                }
            }
            else
            {
                exist = _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.Get(q => q.FixedDepositType == model.FixedDepositType && id != q.Id).Any();

                if (exist)
                {
                    return errorMessage = "Duplicate Fixed Deposit Type";
                }
            }
            return errorMessage;

        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.GetByID(id);
            try
            {
                _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.Delete(id);
                _fmsCommonService.FMSUnit.FixedDepositTypeInfoRepository.SaveChanges();
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
        public FixedDepositTypeInfoViewModel PrepareModel(FixedDepositTypeInfoViewModel model)
        {
            string errorMessage = string.Empty;
            if (model.strMode == "Edit")
            {
                if (model.DurationType == "Year")
                {
                    model.Duration = model.Duration / 12;
                }
                if (model.InstallmentType == "Year")
                {
                    model.InstallmentIn = model.InstallmentIn / 12;
                }
            }
            else
            {
                if (model.DurationType == "Year")
                {
                    model.Duration = model.Duration * 12;
                }
                if (model.InstallmentType == "Year")
                {
                    model.InstallmentIn = model.InstallmentIn * 12;
                }
            }

            return model;
        }

        public void populateDropdown(FixedDepositTypeInfoViewModel model)
        {
            model.DurationTypeList = Common.PopulateFDRDurationTypeList();
            model.InstallmentInList = Common.PopulateFDRDurationTypeList();
        }
    }
}