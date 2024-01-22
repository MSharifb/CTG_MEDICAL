using BEPZA_MEDICAL.Domain.FAR;
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
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class AssetRepairingController : BaseController
    {
        #region Fields

        private readonly FARCommonService _farCommonService;
        #endregion

        #region Ctor
        public AssetRepairingController(FARCommonService farCommonService)
        {
            this._farCommonService = farCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: FAR/AssetRepairing
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AssetRepairingViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<AssetRepairingViewModel> list = (from AR in _farCommonService.FARUnit.AssetRepairingRepository.GetAll()
                                                  join FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll() on AR.FixedAssetId equals FA.Id
                                                  join C in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals C.Id
                                                  join SC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals SC.Id
                                                  where FA.ZoneInfoId == LoggedUserZoneInfoId
                                                  select new AssetRepairingViewModel()
                                                  {
                                                      Id = AR.Id,
                                                      AssetCode = FA.AssetCode,
                                                      AssetName = FA.AssetName,
                                                      CategoryId = FA.CategoryId,
                                                      CategoryName = C.CategoryName,
                                                      SubCategoryId = FA.SubCategoryId,
                                                      SubCategoryName = SC.SubCategoryName,
                                                      EffectiveDate = AR.EffectiveDate,
                                                      CurrentBookValue = AR.CurrentBookValue,
                                                      RepairCost = AR.RepairCost,
                                                      AppreciatedCost = AR.AppreciatedCost,
                                                      UpdatedBookValue = AR.UpdatedBookValue
                                                  }).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.AssetCode))
                {
                    list = list.Where(d => d.AssetCode == viewModel.AssetCode).ToList();
                }
                else if (!string.IsNullOrEmpty(viewModel.AssetName))
                {
                    list = list.Where(d => d.AssetName.ToLower().Trim().Contains(viewModel.AssetName.ToLower().Trim())).ToList();
                }
                else if (viewModel.CategoryId != 0)
                {
                    list = list.Where(d => d.CategoryId == viewModel.CategoryId).ToList();
                }
                else if (Convert.ToInt32(viewModel.CategoryName) != 0)
                {
                    list = list.Where(d => d.CategoryId == Convert.ToInt32(viewModel.CategoryName)).ToList();
                }
                else if ((viewModel.RepairDateFrom != null && viewModel.RepairDateFrom != DateTime.MinValue) && (viewModel.RepairDateTo != null && viewModel.RepairDateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveDate >= viewModel.RepairDateFrom && d.EffectiveDate <= viewModel.RepairDateTo).ToList();
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

            if (request.SortingName == "SubCategoryName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SubCategoryName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SubCategoryName).ToList();
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

            if (request.SortingName == "RepairCost")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RepairCost).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RepairCost).ToList();
                }
            }

            if (request.SortingName == "AppreciatedCost")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AppreciatedCost).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AppreciatedCost).ToList();
                }
            }

            if (request.SortingName == "UpdatedBookValue")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.UpdatedBookValue).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.UpdatedBookValue).ToList();
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
                    d.CategoryId,
                    d.CategoryName,
                    d.SubCategoryId,
                    d.SubCategoryName,
                    d.EffectiveDate!=null?Convert.ToDateTime(d.EffectiveDate).ToString(DateAndTime.GlobalDateFormat):default(string),
                    "RepairDateFrom",
                    "RepairDateTo",
                    d.CurrentBookValue,
                    d.RepairCost,
                    d.AppreciatedCost,
                    d.UpdatedBookValue                    
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult Create()
        {
            AssetRepairingViewModel model = new AssetRepairingViewModel();
            PrepareModel(model);
            populateDropdown(model);
            model.strMode = "Create";
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(AssetRepairingViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                if (ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                    errorList = BusinessLogicValidation(model);
                    if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _farCommonService.FARUnit.AssetRepairingRepository.Add(entity);
                        _farCommonService.FARUnit.AssetRepairingRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                    }

                    if (errorList.Count() > 0)
                    {
                        model.errClass = "failed";
                        model.ErrMsg = errorList;
                    }
                }
                else
                {
                    model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(errorList) ? ErrorMessages.InsertFailed : errorList) : Common.GetModelStateError(ModelState);
                }


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
                //    model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                //}
            }
            populateDropdown(model);
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var entity = _farCommonService.FARUnit.AssetRepairingRepository.GetByID(id);
            var model = entity.ToModel();
            PrepareModel(model);
            GetAssetInfoForEdit(model);
            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(AssetRepairingViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                if (ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                    errorList = BusinessLogicValidation(model);
                    if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _farCommonService.FARUnit.AssetRepairingRepository.Update(entity);
                        _farCommonService.FARUnit.AssetRepairingRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                    }

                    if (errorList.Count() > 0)
                    {
                        model.errClass = "failed";
                        model.ErrMsg = errorList;
                    }
                }
                else
                {
                    model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(errorList) ? ErrorMessages.InsertFailed : errorList) : Common.GetModelStateError(ModelState);
                }


            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                //}
            }
            PrepareModel(model);
            GetAssetInfoForEdit(model);
            model.strMode = "Edit";
            populateDropdown(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            AssetRepairingViewModel model = _farCommonService.FARUnit.AssetRepairingRepository.GetByID(id).ToModel();
            model.strMode = "Delete";
            errMsg = BusinessLogicValidation(model);
            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    _farCommonService.FARUnit.AssetRepairingRepository.Delete(id);
                    _farCommonService.FARUnit.AssetRepairingRepository.SaveChanges();
                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
                catch (UpdateException ex)
                {
                    try
                    {
                        if (ex.InnerException != null && ex.InnerException is SqlException)
                        {
                            SqlException sqlException = ex.InnerException as SqlException;
                            errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            // if (ex.InnerException.Message.Contains("REFERENCE constraint"))
                            // "The user has related information and cannot be deleted."
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
                    result = false;
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }
            else
            {
                result = false;
            }
            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }


        #endregion

        #region Private

        private void PrepareModel(AssetRepairingViewModel model)
        {
            // AssetStatusId=2 means repairable
            model.AssetStatusId = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Repairable.ToString()).FirstOrDefault().Id;

            var status = (from tr in _farCommonService.FARUnit.AssetStatusRepository.GetAll()
                          where tr.Id == model.AssetStatusId
                          select tr).LastOrDefault();
            if (status != null)
            {
                model.strAssetStatusName = status.Name;
            }

        }
        private void populateDropdown(AssetRepairingViewModel model)
        {
            dynamic ddlList;
            //#region Asset Category  ddl
            //ddlList = _farCommonService.FARUnit.AssetCategoryRepository.GetAll().OrderBy(q => q.CategoryName).ToList();
            //model.AssetCategoryList = Common.PopulateAssetCategoryDDL(ddlList);
            //#endregion

            //#region Asset Status  ddl
            //ddlList = _farCommonService.FARUnit.AssetStatusRepository.GetAll().OrderBy(q => q.Name).ToList();
            //model.AssetStatusList = Common.PopulateDllList(ddlList);
            //#endregion

            #region Asset Category  ddl
            ddlList = _farCommonService.FARUnit.AssetConditionRepository.GetAll().Where(q => q.FAR_AssetStatus.Name == FAREnum.FARAssetStatus.Repairable.ToString()).OrderBy(q => q.Name).ToList();
            model.AssetConditionList = Common.PopulateDllList(ddlList);
            #endregion
        }

        #endregion

        #region Utilities
        public JsonResult AutoCompleteForAsset(string term)
        {
            // 4=Disposed, 5=Sold
            int DisposedId = 0;
            int SoldId = 0;
            int repairableId = 0;
            DisposedId = _farCommonService.FARUnit.AssetStatusRepository.Get(q => q.Name == "Disposed").FirstOrDefault().Id;
            SoldId = _farCommonService.FARUnit.AssetStatusRepository.Get(q => q.Name == "Sold").FirstOrDefault().Id;
            //get only repairable List 
            repairableId = _farCommonService.FARUnit.AssetStatusRepository.Get(q => q.Name == "Repairable").FirstOrDefault().Id;
            var result = (from r in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                          where r.ZoneInfoId == LoggedUserZoneInfoId && r.IsApproved == true && r.AssetStatusId != DisposedId && r.AssetStatusId != SoldId && r.AssetCode.ToLower().StartsWith(term.ToLower())
                          select new { r.Id, r.AssetCode, r.AssetName }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private decimal getCurrentBookvalue(int fixedAssetId)
        {
            decimal CurrentBookvalue = 0;
            decimal AssetCost = 0;
            decimal RepairCost = 0;
            decimal Depreciation = 0;

            //  AssetCost
            var dd = _farCommonService.FARUnit.FixedAssetRepository.GetAll().Where(q => q.Id == fixedAssetId && q.IsApproved == true).FirstOrDefault();
            AssetCost = _farCommonService.FARUnit.FixedAssetRepository.GetByID(fixedAssetId).OBBookValue.Value;
           

            RepairCost = (from tr in _farCommonService.FARUnit.AssetRepairingRepository.GetAll()
                          where tr.FixedAssetId == fixedAssetId && tr.IsImpactWithDep == true
                          select tr.AppreciatedCost).Sum();

            Depreciation = (from tr in _farCommonService.FARUnit.AssetDepreciationDetailRepository.GetAll()
                            where tr.FixedAssetId == fixedAssetId
                            select tr.Depreciation).Sum();

            CurrentBookvalue = AssetCost + RepairCost - Depreciation;

            return CurrentBookvalue;
        }

        [NoCache]
        public JsonResult GetAssetInformation(string assetCode)
        {
            string errorList = string.Empty;
            errorList = GetValidationChecking(assetCode);

            var obj = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                       join AC in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals AC.Id
                       join ASC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals ASC.Id
                       join AST in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on FA.AssetStatusId equals AST.Id
                       where FA.AssetCode == assetCode

                       select new AssetRepairingViewModel()
                       {
                           Id = FA.Id,
                           AssetName = FA.AssetName,
                           CategoryName = AC.CategoryName,
                           SubCategoryName = ASC.SubCategoryName,
                           DepreciationRate = FA.DepreciationRate,
                           CurrentBookValue = getCurrentBookvalue(FA.Id),
                           AssetConditionId = FA.AssetConditionId
                       }).FirstOrDefault();

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (obj != null)
                    {
                        return Json(new
                        {
                            Id = obj != null ? obj.Id : default(int),
                            AssetName = obj != null ? obj.AssetName : default(string),
                            CategoryName = obj != null ? obj.CategoryName : default(string),
                            SubCategoryName = obj != null ? obj.SubCategoryName : default(string),
                            DepreciationRate = obj != null ? obj.DepreciationRate : default(decimal),
                            CurrentBookValue = obj != null ? obj.CurrentBookValue : default(decimal),
                            AssetConditionId = obj != null ? obj.AssetConditionId : default(int),
                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }

                }
                catch (Exception ex)
                {
                    return Json(new { Result = false });
                }
            }
            else
            {
                return Json(new
                {
                    Result = errorList
                });
            }

        }

        private string GetValidationChecking(string assetCode)
        {
            string msg = string.Empty;
            string disposed = Convert.ToString(BEPZA_MEDICAL.Utility.FAREnum.FARAssetStatus.Disposed);
            string sold = Convert.ToString(BEPZA_MEDICAL.Utility.FAREnum.FARAssetStatus.Sold);

            var objFixedAsset = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                 where FA.AssetCode == assetCode
                                 join S in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on FA.AssetStatusId equals S.Id
                                 select S).FirstOrDefault();
            if (objFixedAsset != null)
            {
                if (objFixedAsset.Name.Trim().ToLower() == disposed.ToLower() || objFixedAsset.Name.Trim().ToLower() == sold.ToLower())
                {
                    msg = "NotRepairable";
                }
            }
            return msg;
        }

        private AssetRepairingViewModel GetAssetInfoForEdit(AssetRepairingViewModel model)
        {
            var obj = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                       join AC in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals AC.Id
                       join ASC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals ASC.Id
                       where FA.Id == model.FixedAssetId

                       select new AssetRepairingViewModel()
                       {
                           AssetCode = FA.AssetCode,
                           AssetName = FA.AssetName,
                           CategoryName = AC.CategoryName,
                           SubCategoryName = ASC.SubCategoryName,
                       }).FirstOrDefault();

            model.AssetCode = obj.AssetCode;
            model.AssetName = obj.AssetName;
            model.CategoryName = obj.CategoryName;
            model.SubCategoryName = obj.SubCategoryName;
            return model;
        }


        private string BusinessLogicValidation(AssetRepairingViewModel model)
        {
            string errMessage = string.Empty;
            string disposed = Convert.ToString(BEPZA_MEDICAL.Utility.FAREnum.FARAssetStatus.Disposed);
            string sold = Convert.ToString(BEPZA_MEDICAL.Utility.FAREnum.FARAssetStatus.Sold);

            var obj = (from AD in _farCommonService.FARUnit.AssetDepreciationRepository.GetAll()
                       join ADD in _farCommonService.FARUnit.AssetDepreciationDetailRepository.GetAll() on AD.Id equals ADD.DepreciationId
                       where ADD.FixedAssetId == model.FixedAssetId
                       select AD).OrderByDescending(x => x.ProcessDate).FirstOrDefault();
            if (obj != null)
            {
                if (obj.ProcessDate >= model.EffectiveDate)
                {
                    errMessage = "Repairing effective date must be greater than last depreciation calculation date for the selected asset.";
                }

                if (model.strMode == "Edit")
                {
                    errMessage = "Any changes cannot be done if depreciation process has been executed for the selected asset.";
                }
                if (model.strMode == "Delete")
                {
                    errMessage = "Deletion cannot be done if depreciation process has been executed for the selected asset.";
                }
            }

            if (model.strMode == CrudeAction.Delete)
            {
                var objFixedAsset = (from SD in _farCommonService.FARUnit.SaleDisposalRepository.GetAll()
                                     where SD.FixedAssetId == model.FixedAssetId
                                     select SD).FirstOrDefault();
                if (objFixedAsset != null)
                {
                    errMessage = "Deletion cannot be done for the selected asset because asset status is Sold or disposed.";
                }
            }

            return errMessage;
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
        #endregion
    }

}