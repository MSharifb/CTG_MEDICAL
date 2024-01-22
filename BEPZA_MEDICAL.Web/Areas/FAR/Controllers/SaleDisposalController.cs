using BEPZA_MEDICAL.DAL.FAR;
using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class SaleDisposalController : BaseController
    {
        #region Fields

        private readonly FARCommonService _farCommonService;
        private ERP_BEPZAFAREntities _farEntities = new ERP_BEPZAFAREntities();
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public SaleDisposalController(FARCommonService farCommonService, INVCommonService invCommonService)
        {
            this._farCommonService = farCommonService;
            this._invCommonService = invCommonService;
        }
        #endregion


        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, SaleDisposalViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<SaleDisposalViewModel> list = (from SD in _farCommonService.FARUnit.SaleDisposalRepository.GetAll()
                                                join FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll() on SD.FixedAssetId equals FA.Id
                                                join C in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals C.Id
                                                join SC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals SC.Id
                                                join AST in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on FA.AssetStatusId equals AST.Id
                                                join SP in _invCommonService.INVUnit.SupplierRepository.GetAll() on FA.SupplierId equals SP.Id
                                                join L in _farCommonService.FARUnit.LocationRepository.GetAll() on FA.LocationId equals L.Id
                                                where FA.ZoneInfoId==LoggedUserZoneInfoId
                                                select new SaleDisposalViewModel()
                                                {
                                                    Id = SD.Id,
                                                    strType = SD.strType,
                                                    EffectiveDate = SD.EffectiveDate,
                                                    FixedAssetId = SD.FixedAssetId,
                                                    AssetCode = FA.AssetCode,
                                                    AssetName = FA.AssetName,
                                                    CategoryId = FA.CategoryId,
                                                    CategoryName = C.CategoryName,
                                                    SubCategoryId = FA.SubCategoryId,
                                                    SubCategoryName = SC.SubCategoryName,
                                                    AssetStatusId = FA.AssetStatusId,
                                                    SupplierId = FA.SupplierId,
                                                    LocationId = FA.LocationId,
                                                    AssetCost = SD.AssetCost,
                                                    CurrentBookValue = SD.CurrentBookValue,
                                                    AccumulatedDepreciation = SD.AccumulatedDep,
                                                    SalValue = SD.SalValue,
                                                    CapitalGain = SD.CapitalGain
                                                }).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.AssetCode))
                {
                    list = list.Where(d => d.AssetCode == model.AssetCode).ToList();
                }
                else if (model.AssetName != null && model.AssetName.Trim() != string.Empty)
                {
                    list = list.Where(d => d.AssetName.ToLower().Trim().Contains(model.AssetName.ToLower().Trim())).ToList();
                }
                else if (model.CategoryId != 0)
                {
                    list = list.Where(d => d.CategoryId == model.CategoryId).ToList();
                }
                else if (model.SubCategoryId != 0)
                {
                    list = list.Where(d => d.SubCategoryId == model.SubCategoryId).ToList();
                }
                else if (model.AssetStatusId != 0)
                {
                    list = list.Where(d => d.AssetStatusId == model.AssetStatusId).ToList();
                }
                else if (model.LocationId != 0)
                {
                    list = list.Where(d => d.LocationId == model.LocationId).ToList();
                }
                else if (model.SupplierId != 0)
                {
                    list = list.Where(d => d.SupplierId == model.SupplierId).ToList();
                }
               
                else if ((model.EffectiveDateBetween != null && model.EffectiveDateBetween != DateTime.MinValue) && (model.EffectiveDateBetweenAnd != null && model.EffectiveDateBetweenAnd != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveDate >= model.EffectiveDateBetween && d.EffectiveDate <= model.EffectiveDateBetweenAnd).ToList();
                }

            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sort

            if (request.SortingName == "Id")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }

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

            if (request.SortingName == "AssetName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetName).ToList();
                }
            }

            if (request.SortingName == "strType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.strType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.strType).ToList();
                }
            }

            if (request.SortingName == "EffectiveDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveDate).ToList();
                }
            }

            if (request.SortingName == "CategoryName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CategoryName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CategoryName).ToList();
                }
            }


            if (request.SortingName == "AssetCost")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetCost).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetCost).ToList();
                }
            }

            if (request.SortingName == "CurrentBookValue")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CurrentBookValue).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CurrentBookValue).ToList();
                }
            }

            if (request.SortingName == "AccumulatedDep")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AccumulatedDepreciation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AccumulatedDepreciation).ToList();
                }
            }

            if (request.SortingName == "SaleValue")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalValue).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalValue).ToList();
                }
            }

            if (request.SortingName == "CapitalGain")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.CapitalGain).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.CapitalGain).ToList();
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
                    d.AssetCode,                 
                    d.AssetName,
                    d.strType,
                    d.EffectiveDate!=null?Convert.ToDateTime(d.EffectiveDate).ToString(DateAndTime.GlobalDateFormat):default(string) ,
                    d.CategoryId,
                    d.CategoryName,     
                    d.SubCategoryId,
                    d.SubCategoryName,
                    "EffectiveDateBetween",
                    "EffectiveDateBetweenAnd",
                    d.AssetCost,
                    d.CurrentBookValue,
                    d.AccumulatedDepreciation,
                    d.SalValue,
                    d.CapitalGain                    
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        public ActionResult Create()
        {
            SaleDisposalViewModel model = new SaleDisposalViewModel();
            model.strType = FAREnum.FARAssetStatus.Disposed.ToString();
            PrepareModel(model);
            model.strMode = "Create";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(SaleDisposalViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();
            errorList = BusinessLogicValidation(model);
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                if (model.strType == "Disposed")
                {
                    model.SalValue = model.DisposedValue;
                }
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _farCommonService.FARUnit.SaleDisposalRepository.Add(entity);
                    _farCommonService.FARUnit.SaleDisposalRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }

                catch (Exception ex)
                {
                    model.errClass = "failed";
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    //if (ex.InnerException != null && ex.InnerException is SqlException)
                    //{
                    //    SqlException sqlException = ex.InnerException as SqlException;
                    //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    //}
                    //else
                    //{
                    //    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    //}
                }
            }
            else
            {
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(errorList) ? ErrorMessages.InsertFailed : errorList) : Common.GetModelStateError(ModelState);
            }

            PrepareModel(model);
            model.strMode = "Create";
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            SaleDisposalViewModel model = _farCommonService.FARUnit.SaleDisposalRepository.GetByID(id).ToModel();
            GetSetupData(model);
            PrepareModel(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(SaleDisposalViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = BusinessLogicValidation(model);
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    if (model.strType == "Disposed")
                    {
                        model.SalValue = model.DisposedValue;
                    }
                    var entity = model.ToEntity();

                    _farCommonService.FARUnit.SaleDisposalRepository.Update(entity);
                    _farCommonService.FARUnit.SaleDisposalRepository.SaveChanges();
                    return RedirectToAction("Index");
                }

                catch (Exception)
                {
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    //if (ex.InnerException != null && ex.InnerException is SqlException)
                    //{
                    //    SqlException sqlException = ex.InnerException as SqlException;
                    //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    //}
                    //else
                    //{
                    //    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    //}
                }
            }
            else
            {
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(errorList) ? ErrorMessages.UpdateFailed : errorList) : Common.GetModelStateError(ModelState);
            }
            PrepareModel(model);
            GetCategoryAndSubCategory(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = string.Empty;

            try
            {
                _farCommonService.FARUnit.SaleDisposalRepository.Delete(id);
                _farCommonService.FARUnit.SaleDisposalRepository.SaveChanges();
                result = true;
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        ModelState.AddModelError("Error", errMsg);
                    }
                    else
                    {
                        ModelState.AddModelError("Error", errMsg);
                    }
                    result = false;
                }
                catch (Exception)
                {

                    result = false;
                }
            }
            catch (Exception ex)
            {
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? "Information has been deleted successfully." : errMsg
            });
        }

        #endregion

        #region Utilities

        [NoCache]
        private void PrepareModel(SaleDisposalViewModel model)
        {
            if (model.strType == FAREnum.FARAssetStatus.Disposed.ToString())
            {
                model.AssetStatusId = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Disposed.ToString()).FirstOrDefault().Id;
            }

            var status = (from tr in _farCommonService.FARUnit.AssetStatusRepository.GetAll()
                          where tr.Id == model.AssetStatusId
                          select tr).LastOrDefault();
            if (status != null)
            {
                model.strAssetStatusName = status.Name;
            }
            var assetConditionList = (from tr in _farCommonService.FARUnit.AssetConditionRepository.GetAll()
                                      where tr.AssetStatusId == model.AssetStatusId
                                      select tr).ToList();

            foreach (var item in assetConditionList)
            {
                model.AssetConditionList = assetConditionList.Select(y => new SelectListItem()
                {
                    Value = y.Id.ToString(),
                    Text = y.Name
                }).ToList();
            }
        }

        [NoCache]
        private void GetSetupData(SaleDisposalViewModel model)
        {

            var assetInfo = (from SD in _farCommonService.FARUnit.SaleDisposalRepository.GetAll()
                             join FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll() on SD.FixedAssetId equals FA.Id
                             join C in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals C.Id
                             join SC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals SC.Id
                             join AST in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on FA.AssetStatusId equals AST.Id
                             join SP in _invCommonService.INVUnit.SupplierRepository.GetAll() on FA.SupplierId equals SP.Id
                             join L in _farCommonService.FARUnit.LocationRepository.GetAll() on FA.LocationId equals L.Id
                             //join P in _farCommonService.FARUnit.ProjectInformation.GetAll() on FA.ProjectId equals P.Id
                             where SD.Id == model.Id
                             select new SaleDisposalViewModel()
                             {
                                 Id = SD.Id,
                                 strType = SD.strType,
                                 EffectiveDate = SD.EffectiveDate,
                                 FixedAssetId = SD.FixedAssetId,
                                 AssetCode = FA.AssetCode,
                                 AssetName = FA.AssetName,
                                 CategoryId = FA.CategoryId,
                                 CategoryName = C.CategoryName,
                                 SubCategoryId = FA.SubCategoryId,
                                 SubCategoryName = SC.SubCategoryName,
                                 AssetStatusId = FA.AssetStatusId,
                                 SupplierId = FA.SupplierId,
                                 LocationId = FA.LocationId,
                                 AssetCost = SD.AssetCost,
                                 CurrentBookValue = SD.CurrentBookValue,
                                 AccumulatedDepreciation = SD.AccumulatedDep,
                                 SalValue = SD.SalValue,
                                 CapitalGain = SD.CapitalGain
                             }).FirstOrDefault();

            model.FixedAssetId = assetInfo.FixedAssetId;
            model.AssetCode = assetInfo.AssetCode;
            model.AssetName = assetInfo.AssetName;
            model.CategoryId = assetInfo.CategoryId;
            model.CategoryName = assetInfo.CategoryName;
            model.SubCategoryId = assetInfo.SubCategoryId;
            model.SubCategoryName = assetInfo.SubCategoryName;

        }

        private string BusinessLogicValidation(SaleDisposalViewModel model)
        {
            string errMessage = string.Empty;
            string AssetID = Convert.ToString(model.FixedAssetId);
            var IsDepcreation = _farEntities.SP_FAR_GetDepreciationStatus(AssetID, model.EffectiveDate).FirstOrDefault();
            if (IsDepcreation != null)
            {
                if (model.strType == "Sold" && IsDepcreation.FixedAssetId != 0)
                {
                    errMessage = "Already Depreciation Calculated for this Asset in this month.Please sale this asset next month";
                }
            }

            if (model.strType == "Sold" && model.SalValue == 0)
            {
                errMessage = "Sold Value must be greater than 0";
            }
            return errMessage;
        }

        [NoCache]
        private void GetCategoryAndSubCategory(SaleDisposalViewModel model)
        {
            var category = (from p in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() where p.Id == model.CategoryId select p).FirstOrDefault();
            var subCategory = (from p in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() where p.Id == model.SubCategoryId select p).FirstOrDefault();

            model.CategoryName = category.CategoryName;
            model.SubCategoryName = subCategory.SubCategoryName;
        }


        [NoCache]
        public JsonResult GetDefaultValue(int fixedAssetId)
        {
            var obj = _farEntities.SP_FAR_GetDepreciationByFixedAssetId(fixedAssetId).FirstOrDefault();           
            decimal dep = 0;
            if (obj != null)
            {
                dep = Convert.ToDecimal(obj.Depreciation);
                //dep = Convert.ToDecimal(obj);
            }
            else
            {
                dep = 0;
            }
            var Asset = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                         join C in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals C.Id
                         join SC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals SC.Id
                         join SP in _invCommonService.INVUnit.SupplierRepository.GetAll() on FA.SupplierId equals SP.Id
                         join L in _farCommonService.FARUnit.LocationRepository.GetAll() on FA.LocationId equals L.Id
                         join AC in _farCommonService.FARUnit.AssetConditionRepository.GetAll() on FA.AssetConditionId equals AC.Id
                         where FA.Id == fixedAssetId
                         select new SaleDisposalViewModel()
                         {
                             FixedAssetId = FA.Id,
                             AssetCode = FA.AssetCode,
                             AssetName = FA.AssetName,
                             CategoryId = FA.CategoryId,
                             CategoryName = C.CategoryName,
                             SubCategoryId = FA.SubCategoryId,
                             SubCategoryName = SC.SubCategoryName,
                             AssetConditionId = FA.AssetConditionId,
                             AccumulatedDepreciation = FA.OBDepreciation.Value,
                             SupplierId = FA.SupplierId,
                             LocationId = FA.LocationId,
                             AssetCost = FA.AssetCost,
                             AssetStatusId=FA.AssetStatusId,
                             CurrentBookValue = getCurrentBookvalue(FA.Id),
                             PreviousStatusId = FA.AssetStatusId,
                             PreviousConditionId = FA.AssetConditionId
                         }).FirstOrDefault();

            return this.Json(new
            {
                FixedAssetId = Asset.FixedAssetId,
                AssetCode = Asset.AssetCode,
                AssetName = Asset.AssetName,
                CategoryId = Asset.CategoryId,
                CategoryName = Asset.CategoryName,
                SubCategoryId = Asset.SubCategoryId,
                SubCategoryName = Asset.SubCategoryName,
                AssetConditionId = Asset.AssetConditionId,
                AssetCost = Asset.AssetCost,
                AssetStatusId=Asset.AssetStatusId,
                CurrentBookValue = Asset.CurrentBookValue,
                AccumulatedDepreciation = (Asset.AccumulatedDepreciation + dep),
                PreviousStatusId = Asset.PreviousStatusId,
                PreviousConditionId = Asset.PreviousConditionId
            }, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetAssetStatusInformation(string strType)
        {
            var model = new SaleDisposalViewModel();

            if (strType == FAREnum.FARAssetStatus.Disposed.ToString())
            {
                model.AssetStatusId = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Disposed.ToString()).FirstOrDefault().Id;
            }
            else
            {
                model.AssetStatusId = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Sold.ToString()).FirstOrDefault().Id;
            }
            var status = (from tr in _farCommonService.FARUnit.AssetStatusRepository.GetAll()
                          where tr.Id == model.AssetStatusId
                          select tr).LastOrDefault();
            if (status != null)
            {
                return this.Json(new
                {

                    AssetStatusId = model.AssetStatusId,
                    strAssetStatusName = status.Name
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new
                {

                    AssetStatusId = 0,
                    strAssetStatusName = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [NoCache]
        public ActionResult GetAssetConditionList(string strType)
        {
            var model = new SaleDisposalViewModel();

            if (model.strType == FAREnum.FARAssetStatus.Disposed.ToString())
            {
                model.AssetStatusId = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Disposed.ToString()).FirstOrDefault().Id;
            }
            else
            {
                model.AssetStatusId = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Sold.ToString()).FirstOrDefault().Id;
            }

            model.AssetConditionList = (from s in _farCommonService.FARUnit.AssetConditionRepository.GetAll().OrderBy(x => x.Name)
                                        where s.AssetStatusId == model.AssetStatusId
                                        select s).Select(y => new SelectListItem()
                                        {
                                            Text = y.Name.ToString(),
                                            Value = y.Id.ToString()
                                        }).ToList();

            return Json(model.AssetConditionList);
        }

        public decimal getCurrentBookvalue(int assetID)
        {
            decimal CurrentBookvalue = 0;
            decimal OpBalOfDepreciation = 0;
            decimal AssetCost = 0;
            decimal RepairCost = 0;
            decimal Depreciation = 0;

            var asset = _farCommonService.FARUnit.FixedAssetRepository.GetByID(assetID);

            AssetCost = asset.AssetCost;
            OpBalOfDepreciation = asset.OBDepreciation.Value;

            RepairCost = (from tr in _farCommonService.FARUnit.AssetRepairingRepository.GetAll()
                          where tr.FixedAssetId == assetID && tr.IsImpactWithDep == true
                          select tr.AppreciatedCost).Sum();

            Depreciation = (from tr in _farCommonService.FARUnit.AssetDepreciationDetailRepository.GetAll()
                            where tr.FixedAssetId == assetID
                            select tr.Depreciation).Sum();

            CurrentBookvalue = AssetCost + RepairCost - OpBalOfDepreciation - Depreciation;

            return CurrentBookvalue;
        }

        #endregion

        #region Grid DDL-----------
        [NoCache]
        public ActionResult GetCategoryList()
        {
            var list = Common.PopulateAssetCategoryDDL(_farCommonService.FARUnit.AssetCategoryRepository.GetAll().OrderBy(x => x.CategoryName).ToList());
            return PartialView("Select", list);
        }
        [NoCache]
        public ActionResult GetAssetSubCategoryList()
        {
            var list = Common.PopulateAssetSubCategoryDDL(_farCommonService.FARUnit.AssetSubCategoryRepository.GetAll().OrderBy(x => x.SubCategoryName).ToList());
            return PartialView("Select", list);
        }
        [NoCache]
        public ActionResult GetAssetStatusList()
        {
            var list = Common.PopulateDllList(_farCommonService.FARUnit.AssetStatusRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult GetAssetLocationList()
        {
            var list = Common.PopulateDllList(_farCommonService.FARUnit.LocationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult GetAssetSupplierList()
        {
            var list = _invCommonService.INVUnit.SupplierRepository.GetAll().OrderBy(x => x.SupplierName).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.SupplierName,
                    Value = y.Id.ToString()
                }).ToList();
            return PartialView("Select", list);
        }

        #endregion

        #region Autocomplete--------------

        public JsonResult AutoCompleteFixedAssetList(string term)
        {
            // 4=Disposed, 5=Sold
            int DisposedId = 0;
            int SoldId = 0;
            DisposedId = _farCommonService.FARUnit.AssetStatusRepository.Get(q => q.Name == "Disposed").FirstOrDefault().Id;
            SoldId = _farCommonService.FARUnit.AssetStatusRepository.Get(q => q.Name == "Sold").FirstOrDefault().Id;


            var fixedAsset = _farCommonService.FARUnit.FixedAssetRepository.GetAll().ToList();
            var soldDisposal = _farCommonService.FARUnit.SaleDisposalRepository.GetAll().ToList();

            var result = (from tr in fixedAsset
                          where !soldDisposal.Any(x => x.FixedAssetId == tr.Id) && tr.AssetStatusId != DisposedId && tr.AssetStatusId != SoldId && tr.ZoneInfoId == LoggedUserZoneInfoId && tr.IsApproved == true 
                          && tr.AssetCode.ToLower().StartsWith(term.ToLower())
                          select new { tr.Id, tr.AssetCode, tr.AssetName }).OrderBy(x => x.AssetCode);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}