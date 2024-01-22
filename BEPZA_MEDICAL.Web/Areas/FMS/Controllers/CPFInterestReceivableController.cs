using BEPZA_MEDICAL.DAL.FMS;
using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Domain.PGM;
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
    public class CPFInterestReceivableController : Controller
    {
        #region Fields
        private readonly FMSCommonService _fmsCommonService;
        private readonly FMS_ExecuteFunctions _fmsfunction;
        private readonly ERP_BEPZAFMSEntities _fmsContext;
        private readonly PGMCommonService _pgmCommonservice;

        #endregion

        #region Ctor
        public CPFInterestReceivableController(FMSCommonService fmsCommonService, FMS_ExecuteFunctions fmsfunction, ERP_BEPZAFMSEntities fmsContext, PGMCommonService pgmCommonservice)
        {
            this._fmsCommonService = fmsCommonService;
            this._fmsfunction = fmsfunction;
            this._fmsContext = fmsContext;
            this._pgmCommonservice = pgmCommonservice;
        }
        #endregion

        // GET: FMS/CPFInterestReceivable
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, CPFInterestReceivableViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from fdt in _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.GetAll()
                        join fdi in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll() on fdt.FixedDepositInfoId equals fdi.Id
                        where (viewModel.FixedDepositInfoId == 0 || viewModel.FixedDepositInfoId == fdt.FixedDepositInfoId)
                        && (viewModel.FDRNo == "" || viewModel.FDRNo == null || fdi.FDRNumber.Contains(viewModel.FDRNo))
                        && (fdi.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                        select new CPFInterestReceivableViewModel()
                        {
                            Id = fdt.Id,
                            FixedDepositInfoId = fdi.Id,
                            FDRNo = fdi.FDRNumber,
                            FDRDate = fdi.FDRDate,
                            MaturityDate = fdi.MaturityDate,
                            DistributedAmount = fdt.DistributedAmount
                        }).ToList();

            if (request.Searching)
            {
                //if ((viewModel.DateFrom != null && viewModel.DateFrom != DateTime.MinValue) && (viewModel.DateTo != null && viewModel.DateTo != DateTime.MinValue))
                //{
                //    list = list.Where(d => d.Date >= viewModel.DateFrom && d.Date <= viewModel.DateTo).ToList();
                //}
                //else if ((viewModel.DateFrom != null && viewModel.DateFrom != DateTime.MinValue))
                //{
                //    list = list.Where(d => d.Date >= viewModel.DateFrom).ToList();
                //}
                //else if ((viewModel.DateTo != null && viewModel.DateTo != DateTime.MinValue))
                //{
                //    list = list.Where(d => d.Date <= viewModel.DateTo).ToList();
                //}
            }


            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            //if (request.SortingName == "Date")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.Date).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.Date).ToList();
            //    }
            //}

            //if (request.SortingName == "FDRAmount")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.FDRAmount).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.FDRAmount).ToList();
            //    }
            //}
            //if (request.SortingName == "FDRNo")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.FDRNo).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.FDRNo).ToList();
            //    }
            //}

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
                    //d.FixedDepositInfoId,
                    d.FDRNo,
                    d.FDRDate.ToString("dd-MM-yyyy"),
                    d.MaturityDate.ToString("dd-MM-yyyy"),
                    d.DistributedAmount
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            CPFInterestReceivableViewModel model = new CPFInterestReceivableViewModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CPFInterestReceivableViewModel model)
        {
            string errorList = string.Empty;
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.Add(entity);
                    _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.SaveChanges();
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
            var entity = _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CPFInterestReceivableViewModel model)
        {
            string errorList = string.Empty;
            if (ModelState.IsValid)
            {
                // Set preious attachment if exist
                var obj = _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.GetByID(model.Id);
                try
                {
                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    var entity = model.ToEntity();                  
                    _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.Update(entity);
                    _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.SaveChanges();
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

            var tempPeriod = _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.GetByID(id);
            try
            {
                _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.Delete(id);
                _fmsCommonService.FMSUnit.CPFInterestReceivableRepository.SaveChanges();
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

        public void populateDropdown(CPFInterestReceivableViewModel model)
        {
            dynamic ddlList;
            ddlList = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                      .Where(q => q.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                      .OrderByDescending(x => x.FDRDate);

            model.FDRNoList = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                              .Where(q => q.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                                    .Select(y => new SelectListItem()
                                    {
                                        Text = y.FDRNumber + " [Renewal/Encashment Date: "+y.FDRDate.ToString("dd-MM-yyyy")+"]",
                                        Value = y.Id.ToString()
                                    }).ToList();

            ddlList = _pgmCommonservice.PGMUnit.AccAccountingPeriod.GetAll().OrderBy(x => x.yearName).ToList();
            model.YearList = Common.PopulateAccountingPeriodDdl(ddlList);
        }

        [NoCache]
        public JsonResult GetFinancialYearDate(int id)
        {
            var date = _pgmCommonservice.PGMUnit.AccAccountingPeriod.GetAll().Where(x=>x.id == id).FirstOrDefault().periodStartDate;
            return Json(date.AddDays(-1).ToString("dd-MM-yyyy"), JsonRequestBehavior.AllowGet);
        }

    }
}