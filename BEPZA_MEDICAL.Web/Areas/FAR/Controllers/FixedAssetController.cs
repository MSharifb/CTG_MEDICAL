using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class FixedAssetController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly FARCommonService _farCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public FixedAssetController(EmployeeService empService, FARCommonService farCommonService, PRMCommonSevice prmCommonService, INVCommonService invCommonService)
        {
            _empService = empService;
            this._farCommonService = farCommonService;
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
        }
        #endregion

        #region Actions
        // GET: FAR/FixedAsset
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, FixedAssetViewModel model)
        {
            string filterExpression = String.Empty;

            int totalRecords = 0;

            List<FixedAssetViewModel> list = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                              join C in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals C.Id
                                              join SC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals SC.Id
                                              join AST in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on FA.AssetStatusId equals AST.Id
                                              join SP in _invCommonService.INVUnit.SupplierRepository.GetAll() on FA.SupplierId equals SP.Id
                                              join L in _farCommonService.FARUnit.LocationRepository.GetAll() on FA.LocationId equals L.Id
                                              where FA.ZoneInfoId == LoggedUserZoneInfoId
                                              select new FixedAssetViewModel()
                                              {
                                                  Id = FA.Id,
                                                  AssetCode = FA.AssetCode,
                                                  AssetName = FA.AssetName,
                                                  CategoryId = FA.CategoryId,
                                                  CategoryName = C.CategoryName,
                                                  SubCategoryId = FA.SubCategoryId,
                                                  SubCategoryName = SC.SubCategoryName,
                                                  AssetStatusId = FA.AssetStatusId,
                                                  AssetStatus = AST.Name,
                                                  SupplierId = FA.SupplierId,
                                                  LocationId = FA.LocationId,
                                                  PurchaseDate = FA.PurchaseDate,
                                                  DepreciationRate = FA.DepreciationRate,
                                                  AssetCost = FA.AssetCost,
                                                  BeneficiaryEmployeeId = FA.BeneficiaryEmployeeId,
                                                  ApprovalStatus = FA.IsApproved == true ? "Approved" : "Pending"
                                              }).ToList();

            if (request.Searching)
            {
                if (model.AssetCode != null && model.AssetCode != string.Empty)
                {
                    list = list.Where(d => d.AssetCode == model.AssetCode).ToList();
                }

                if (model.AssetName != null && model.AssetName.Trim() != string.Empty)
                {
                    list = list.Where(d => d.AssetName.ToLower().Trim().Contains(model.AssetName.ToLower().Trim())).ToList();
                }

                if (model.CategoryId != 0)
                {
                    list = list.Where(d => d.CategoryId == model.CategoryId).ToList();
                }

                if (model.SubCategoryId != 0)
                {
                    list = list.Where(d => d.SubCategoryId == model.SubCategoryId).ToList();
                }

                if (model.AssetStatusId != 0)
                {
                    list = list.Where(d => d.AssetStatusId == model.AssetStatusId).ToList();
                }

                if (model.LocationId != 0)
                {
                    list = list.Where(d => d.LocationId == model.LocationId).ToList();
                }

                if (model.SupplierId != 0)
                {
                    list = list.Where(d => d.SupplierId == model.SupplierId).ToList();
                }

                if ((model.PurchaseDateBetween != null && model.PurchaseDateBetween != DateTime.MinValue) && (model.PurchaseDateAnd != null && model.PurchaseDateAnd != DateTime.MinValue))
                {
                    list = list.Where(d => d.PurchaseDate >= model.PurchaseDateBetween && d.PurchaseDate <= model.PurchaseDateAnd).ToList();
                }

            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting
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
            if (request.SortingName == "AssetStatus")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssetStatus).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssetStatus).ToList();
                }
            }
            if (request.SortingName == "PurchaseDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PurchaseDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PurchaseDate).ToList();
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
                    d.AssetStatusId,
                    d.AssetStatus,
                    d.LocationId,
                    d.SupplierId,                  
                    "PurchaseDateBetween",
                    "PurchaseDateAnd",
                    d.PurchaseDate!=null?Convert.ToDateTime(d.PurchaseDate).ToString(DateAndTime.GlobalDateFormat):default(string),
                    d.DepreciationRate,
                    d.AssetCost,
                    d.ApprovalStatus
                   
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            FixedAssetViewModel model = new FixedAssetViewModel();
            model.strMode = "Create";
            model.IsCalculateDepreciation = true;
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(FixedAssetViewModel model)
        {
            string errorList = string.Empty;

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            errorList = BusinessLogicValidation(model);
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                model.ZoneInfoId = LoggedUserZoneInfoId;
                setDeafaultVal(model);


                try
                {
                    //bulk insert
                    var category = _farCommonService.FARUnit.AssetCategoryRepository.GetByID(model.CategoryId);
                    for (int i = 1; i <= model.Quantity; i++)
                    {
                        var entity = model.ToEntity();
                        entity.IsAllowDepreciationCal = true;

                        _farCommonService.FARUnit.FixedAssetRepository.Add(entity);

                        var startNumber = Convert.ToInt64(model.AssetCode.Substring(2, model.AssetCode.Length - 2)) + 1;
                        model.AssetCode = category.CategoryCode + startNumber.ToString("D6");

                    }
                    _farCommonService.FARUnit.FixedAssetRepository.SaveChanges();

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
                model.errClass = "failed";
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(errorList) ? ErrorMessages.InsertFailed : errorList) : Common.GetModelStateError(ModelState);
            }

            model.strMode = "Create";
            populateDropdown(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var entity = _farCommonService.FARUnit.FixedAssetRepository.GetByID(id);
            var model = entity.ToModel();
            model.strMode = "Edit";
            model.CurrentBookValue = GetCurrentBalanceBookValueByAssetId(model.Id);
            populateDropdown(model);
            model.AssetCategoryCode = entity.FAR_Catagory.CategoryCode;
            if (model.BeneficiaryEmployeeId != null)
            {
                var empInfo = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.BeneficiaryEmployeeId);
                if (empInfo != null)
                {
                    model.EmpID = empInfo.FullName + " ["+empInfo.EmpID+"]";
                    //model.EmployeeName = empInfo.FullName;
                }
            }
            if (model.OrganogramLevelId != null)
            {

                model.OrganogramLevelName = _prmCommonService.PRMUnit.OrganogramLevelRepository.GetByID(model.OrganogramLevelId).LevelName;
            }            
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(FixedAssetViewModel model)
        {
            string errorList = string.Empty;
            errorList = BusinessLogicValidation(model);
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    setDeafaultVal(model);
                    var entity = model.ToEntity();
                    entity.IsAllowDepreciationCal = true;

                    _farCommonService.FARUnit.FixedAssetRepository.Update(entity);
                    _farCommonService.FARUnit.FixedAssetRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }

                catch (Exception ex)
                {
                    model.errClass = "failed";
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                    }
                    else
                    {
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    }
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.errClass = "failed";
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(errorList) ? ErrorMessages.UpdateFailed : errorList) : Common.GetModelStateError(ModelState);
            }
            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _farCommonService.FARUnit.FixedAssetRepository.Delete(id);
                _farCommonService.FARUnit.FixedAssetRepository.SaveChanges();
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

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion

        #region Private
        private void populateDropdown(FixedAssetViewModel model)
        {
            dynamic ddlList;

            ddlList = _invCommonService.INVUnit.ItemTypeRepository.GetAll().Where(x => x.ParentId != null && x.IsGroup == false).ToList();
            model.ItemTypeList = Common.PopulateItemTypeDDL(ddlList);

            #region asset Category  ddl
            ddlList = _farCommonService.FARUnit.AssetCategoryRepository.GetAll().OrderBy(q => q.CategoryName).ToList();
            model.AssetCategoryList = Common.PopulateAssetCategoryDDL(ddlList);
            #endregion

            #region asset Sub-Category  ddl
            ddlList = _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll().OrderBy(q => q.SubCategoryName).ToList();
            model.AssetSubCategoryList = Common.PopulateAssetSubCategoryDDL(ddlList);
            #endregion

            #region asset status  ddl
            //ddlList = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Good.ToString() || q.Name == FAREnum.FARAssetStatus.Repairable.ToString()).OrderBy(q => q.SortOrder).ToList();
            //model.AssetStatusList = Common.PopulateDllList(ddlList);
            model.AssetStatusId = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Good.ToString()).FirstOrDefault().Id;
            #endregion

            #region asset condition  ddl
            //ddlList = _farCommonService.FARUnit.AssetConditionRepository.Get(q => q.Id == model.AssetConditionId).OrderBy(q => q.SortOrder).ToList();
            //model.AssetConditionList = Common.PopulateDllList(ddlList);
            #endregion

            #region supplier  ddl
            //ddlList = _farCommonService.FARUnit.SupplierRepository.GetAll().OrderBy(q => q.SortOrder).ToList();
            //model.AssetSupplierList = Common.PopulateDllList(ddlList);
            #endregion

            #region Lcation  ddl

            ddlList = _farCommonService.FARUnit.LocationRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(q => q.SortOrder).ToList();
            model.LocationList = Common.PopulateDllList(ddlList);
            #endregion

            #region purchase ddl

            
            ddlList = _farCommonService.FARUnit.FunctionRepository.getUniqueMRRList(LoggedUserZoneInfoId);
            if (model.strMode == "Edit")
            {
                ddlList = _invCommonService.INVUnit.PurchaseInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            }

            model.PurchaseList = Common.PopulateMRRDllList(ddlList);
            #endregion

            #region supplier ddl
            model.AssetSupplierList = _invCommonService.INVUnit.SupplierRepository.GetAll().OrderBy(x => x.SupplierName).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.SupplierName,
                    Value = y.Id.ToString()
                }).ToList();
            #endregion

            #region Item ddl
            if (model.PurchaseId > 0)
            {
                var items = _invCommonService.INVUnit.ItemInfoRepository.GetAll();
                var idList = new List<Int32>();
                var purchaseItems = _invCommonService.INVUnit.PurchaseItemRepository.GetAll().Where(x => x.PurchaseId == model.PurchaseId);
                foreach (var purchaseItem in purchaseItems)
                {
                    idList.Add(purchaseItem.ItemId);
                }
                items = items.Where(x => idList.Contains(x.Id)).ToList();

                model.ItemList = items.ToList()
                    .Select(y =>
                    new SelectListItem()
                    {
                        Text = y.ItemName,
                        Value = y.Id.ToString()
                    }).ToList();
            }
            #endregion

            #region quantity calc
            if (model.ItemId > 0)
            {
                var obj = _invCommonService.INVUnit.PurchaseItemRepository.GetAll().Where(x => x.PurchaseId == model.PurchaseId && x.ItemId == model.ItemId).GroupBy(x => x.ItemId)
                                            .Select(i =>
                                                    new
                                                    {
                                                        ItemId = i.Key,
                                                        Quantity = i.Sum(s => s.Quantity)
                                                    }).FirstOrDefault();
                if (obj != null)
                {
                    model.Quantity = Convert.ToInt32(obj.Quantity);
                }
            }
            #endregion
        }

        private void setDeafaultVal(FixedAssetViewModel model)
        {
            if (model.OBDepreciation == null || model.OBDepreciation <= 0)
            {
                model.OBDepreciation = 0;
                model.OBBookValue = model.AssetCost;
            }
            if (model.DepreciationRate == null || model.DepreciationRate <= 0)
            {
                model.DepreciationRate = 0;
            }

            if (model.CurrentBookValue == null)
            {
                model.CurrentBookValue = model.AssetCost;
            }

        }

        private string BusinessLogicValidation(FixedAssetViewModel model)
        {
            string errMessage = string.Empty;

            if (model.OrganogramLevelId == null && model.BeneficiaryEmployeeId == null)
            {
                errMessage = "select Organogram Level or Employee ID.";
            }

            if (model.PurchaseDate > model.DepreciationEffectiveDate)
            {
                errMessage = "Depreciation effective date must be greater than or equal to purchase date.";
            }

            var chkApproved = _farCommonService.FARUnit.FixedAssetRepository.GetByID(model.Id);
            if (chkApproved != null && chkApproved.IsApproved == true)
            {
                return errMessage = "Fixed asset can not be updated.Because this asset approved by Finance.";
            }

            // asset Code must be between the reserved ID for Existing Asset

            var AssetCatgory = (from tr in _farCommonService.FARUnit.AssetCategoryRepository.GetAll()
                                where tr.Id == model.CategoryId
                                select tr).FirstOrDefault();

            if (AssetCatgory != null)
            {

                if (AssetCatgory.MinimumCost > model.AssetCost)
                {
                    errMessage = "Asset cost must be greater than or equal to " + AssetCatgory.MinimumCost + " which comes from asset category.";
                }

                if (model.AssetType != "NewAsset")
                {
                    var assID = model.AssetCode.Substring(AssetCatgory.CategoryCode.Length, model.AssetCode.Length - AssetCatgory.CategoryCode.Length);

                    //check length--- Asset ID Lenth= Length of Category ID + 6 Digits
                    var assLength = AssetCatgory.CategoryCode + AssetCatgory.ReservedIDTo.ToString("D6");
                    if (assLength.Length != model.AssetCode.Length)
                    {
                        errMessage = "Asset Code length must be " + assLength.Length.ToString() + " characters.";
                    }

                    //Check Category ID exist or not
                    if (model.AssetCode.Substring(0, AssetCatgory.CategoryCode.Length) != AssetCatgory.CategoryCode)
                    {
                        errMessage = "Asset Code must be started with " + AssetCatgory.CategoryCode.Length.ToString() + ". First " + AssetCatgory.CategoryCode.Length.ToString() + " charcters must be asset category ID.";
                    }

                    if (((Convert.ToInt32(assID) >= AssetCatgory.ReservedIDFrom) && (Convert.ToInt32(assID) <= AssetCatgory.ReservedIDTo)) == false)
                    {
                        errMessage = "Asset Code must be between " + AssetCatgory.CategoryCode + AssetCatgory.ReservedIDFrom.ToString("D6") + " and " + AssetCatgory.CategoryCode + AssetCatgory.ReservedIDTo.ToString("D6");
                    }
                }

            }

            #region Check duplicate for Asset ID

            if (model.Id > 0)
            {
                var assetInfo = (from tr in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                 where tr.AssetCode == model.AssetCode && tr.Id != model.Id
                                 select tr).FirstOrDefault();
                if (assetInfo != null)
                {
                    errMessage = "Asset Code can not be duplicate.";
                }

                var assetDepriciation = (from tr in _farCommonService.FARUnit.AssetDepreciationDetailRepository.GetAll()
                                         where tr.FixedAssetId == model.Id
                                         select tr).LastOrDefault();
                if (assetDepriciation != null)
                {
                    errMessage = "Fixed asset can not be updated.Because depreciation has been calculated for this asset.";
                }

                var IsSale = (from SD in _farCommonService.FARUnit.SaleDisposalRepository.GetAll()
                              where SD.FixedAssetId == model.Id
                              select SD).FirstOrDefault();

                if (IsSale != null)
                {
                    if (IsSale.strType == "Sold")
                    {
                        errMessage = "This Asset already sold. You can not update.";
                    }
                }
            }
            else
            {
                var assetInfo = (from tr in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                 where tr.AssetCode == model.AssetCode
                                 select tr).FirstOrDefault();
                if (assetInfo != null)
                {
                    errMessage = "Asset Code can not be duplicate.";
                }
            }
            #endregion

            return errMessage;
        }
        private decimal GetCurrentBalanceBookValueByAssetId(int assetID)
        {
            //change by suman

            decimal CurrentBookvalue = 0;
            decimal OpBalOfDepreciation = 0;
            decimal AssetCost = 0;
            decimal RepairCost = 0;
            decimal Depreciation = 0;

            var asset = _farCommonService.FARUnit.FixedAssetRepository.GetByID(assetID);

            AssetCost = asset.AssetCost;
            OpBalOfDepreciation = asset.OBDepreciation != null ? asset.OBDepreciation.Value : 0;

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

        [NoCache]
        public JsonResult AssetCodeGenerate(string assetType, int categoryId)
        {
            string assetCode = string.Empty;
            bool IsValid = false;
            var existingAssetType = Convert.ToString(FAREnum.FARAssetType.ExistingAsset);

            var category = _farCommonService.FARUnit.AssetCategoryRepository.GetByID(categoryId);
            if (category != null)
            {
                if (assetType == Convert.ToString(FAREnum.FARAssetType.NewAsset))
                {
                    var fixedAsset = (from a in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                      where a.CategoryId == category.Id && a.AssetType == assetType
                                      select a).OrderByDescending(x => x.AssetCode).FirstOrDefault();
                    var startNumber = fixedAsset != null ? Convert.ToInt64(fixedAsset.AssetCode.Substring(2, fixedAsset.AssetCode.Length - 2)) + 1 : category.ReservedIDTo + 1;  //start number using Reserved Number
                    assetCode = category.CategoryCode + startNumber.ToString("D6");
                    IsValid = true;
                }
                else
                {
                    var categoryExisting = _farCommonService.FARUnit.AssetCategoryRepository.GetByID(categoryId);
                    if (categoryExisting.ReservedIDTo > 0)
                    {


                        var fixedAsset = (from a in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                          where a.CategoryId == category.Id && a.AssetType == existingAssetType
                                          select a).OrderByDescending(x => x.AssetCode).FirstOrDefault();

                        var fixedAssetWithReservedIDList = (from a in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                                            where a.CategoryId == category.Id && a.AssetType == existingAssetType
                                                            select a).ToList();

                        if (fixedAssetWithReservedIDList.Count > 0)
                        {
                            if (fixedAssetWithReservedIDList.Count < category.ReservedIDTo)
                            {
                                var startNumber = fixedAsset != null ? Convert.ToInt64(fixedAsset.AssetCode.Substring(2, fixedAsset.AssetCode.Length - 2)) + 1 : category.ReservedIDFrom + 1;  //start number using Reserved Number
                                assetCode = category.CategoryCode + startNumber.ToString("D6");
                                IsValid = true;
                            }
                            else
                            {
                                IsValid = false;
                            }
                        }
                        else
                        {
                            var startNumber = fixedAsset != null ? Convert.ToInt64(fixedAsset.AssetCode.Substring(2, fixedAsset.AssetCode.Length - 2)) + 1 : category.ReservedIDFrom;  //start number using Reserved Number
                            assetCode = category.CategoryCode + startNumber.ToString("D6");
                            IsValid = true;
                        }
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
            }

            return this.Json(new { assetCode = assetCode, IsValid = IsValid }, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetSubCategoryList(int id)
        {
            var model = new FixedAssetViewModel();

            if (id != 0)
            {

                model.AssetSubCategoryList = (from s in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll().OrderBy(x => x.SubCategoryName)
                                              where s.CategoryId == id
                                              select s).Select(y => new SelectListItem()
                                              {
                                                  Text = y.SubCategoryName.ToString(),
                                                  Value = y.Id.ToString()
                                              }).ToList();
            }
            else
            {
                model.AssetSubCategoryList = (from s in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll().OrderBy(x => x.SubCategoryName)
                                              select s).Select(y => new SelectListItem()
                                              {
                                                  Text = y.SubCategoryName.ToString(),
                                                  Value = y.Id.ToString()
                                              }).ToList();
            }

            return Json(model.AssetSubCategoryList);
        }

        [NoCache]
        public ActionResult GetAssetConditionList(int id)
        {
            var model = new FixedAssetViewModel();

            model.AssetConditionList = (from s in _farCommonService.FARUnit.AssetConditionRepository.GetAll().OrderBy(x => x.Name)
                                        where s.AssetStatusId == id
                                        select s).Select(y => new SelectListItem()
                                        {
                                            Text = y.Name.ToString(),
                                            Value = y.Id.ToString()
                                        }).ToList();

            return Json(model.AssetConditionList);
        }

        [NoCache]
        public JsonResult GetDepreciationRateBySubCatID(int id)
        {
            var subCategory = _farCommonService.FARUnit.AssetSubCategoryRepository.GetByID(id);
            return Json(new { DepreciationRate = subCategory.DepreciationRate }, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(int? employeeId)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(employeeId);
            return Json(new
            {
                EmpID = obj.EmpID,
                EmployeeName = obj.FullName,
                DesignationId = obj.DesignationId,
                Designation = obj.PRM_Designation.Name,
                DivisionId = obj.DivisionId == null ? null : obj.DivisionId,
                Division = obj.DivisionId == null ? string.Empty : obj.PRM_Division.Name
            }, JsonRequestBehavior.AllowGet);

        }

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
            var list = Common.PopulateDllList(_farCommonService.FARUnit.LocationRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList());
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


        #region Item Type Tree

        public ActionResult DispalyItemTypeTree()
        {
            return PartialView("_ItemTypeTreeViewSearch");
        }
        public class JsTreeModel
        {
            public string id { get; set; }
            public string parent { get; set; }
            public string text { get; set; }
            public string icon { get; set; }
            public string state { get; set; }
            public bool opened { get; set; }
            public bool disabled { get; set; }
            public bool selected { get; set; }
            public string li_attr { get; set; }
            public string a_attr { get; set; }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetTreeData()
        {
            var nodes = new List<JsTreeModel>();
            var parentNodes = _invCommonService.INVUnit.ItemTypeRepository.GetAll().ToList();

            if (parentNodes != null)
            {
                var parentNode = parentNodes.Where(x => x.ParentId == null).FirstOrDefault();
                nodes.Add(new JsTreeModel() { id = parentNode.Id.ToString(), parent = "#", text = parentNode.ItemTypeName });
                var childs = _invCommonService.INVUnit.ItemTypeRepository.Get(q => q.ParentId == parentNode.Id).ToList();
                foreach (var item in childs)
                {
                    nodes.Add(new JsTreeModel() { id = item.Id.ToString(), parent = item.ParentId.ToString(), text = GenerateNodeText(item).ToString() });
                    AddChild(nodes, item);
                }
            }
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        private void AddChild(List<JsTreeModel> nodes, INV_ItemType item)
        {

            var childs = _invCommonService.INVUnit.ItemTypeRepository.Get(t => t.ParentId == item.Id).DefaultIfEmpty().OfType<INV_ItemType>().ToList();

            if (childs != null && childs.Count > 0)
            {
                foreach (var anChild in childs)
                {
                    nodes.Add(new JsTreeModel() { id = anChild.Id.ToString(), parent = anChild.ParentId.ToString(), text = GenerateNodeText(anChild).ToString() });
                    AddChild(nodes, anChild);
                }
            }
        }

        private static StringBuilder GenerateNodeText(INV_ItemType parentNode)
        {
            StringBuilder lvlName = new StringBuilder();
            lvlName.Append(parentNode.ItemTypeName);
            //if (parentNode.INV_ItemInfo != null)
            //{
            //    lvlName.Append(" [");
            //    lvlName.Append(parentNode.INV_ItemInfo.);
            //    lvlName.Append("]");
            //}
            return lvlName;
        }


        #endregion

        #region Search Employee

        public ActionResult EmployeeSearch(string UseTypeEmpId)
        {
            var model = new EmployeeSearchViewModel();
            model.SearchEmpType = "active";
            model.UseTypeEmpId = UseTypeEmpId;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, string st)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            if (string.IsNullOrEmpty(st))
                st = "";
            viewModel.ZoneInfoId = LoggedUserZoneInfoId;

            var list = _empService.GetPaged(
                filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                //viewModel.ResourceLevelId,
                viewModel.OrganogramLevelId,
                viewModel.ZoneInfoId,
                st.Equals("active") ? 1 : viewModel.EmployeeStatus,
                out totalRecords
                //LoginEmpId
                );

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.EmpName,
                    item.ID,
                    item.EmpId,
                    item.DesigName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,                     
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        #endregion

        #region Oraganogram Level Tree

        public PartialViewResult OrganogramLevelTreeSearchList()
        {
            return PartialView("_OrganogramLevelTree");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetOrganogramTreeData()
        {
            var nodes = new List<JsTreeModel>();
            var parentNodes = _prmCommonService.PRMUnit.OrganogramLevelRepository.GetAll().Where(q => q.ZoneInfoId == null || q.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            var parentNode = parentNodes.Where(x => x.ParentId == 0).FirstOrDefault();
            nodes.Add(new JsTreeModel() { id = parentNode.Id.ToString(), parent = "#", text = parentNode.LevelName });
            var childs = _prmCommonService.PRMUnit.OrganogramLevelRepository.Get(q => q.ParentId == parentNode.Id && q.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            foreach (var item in childs)
            {
                nodes.Add(new JsTreeModel() { id = item.Id.ToString(), parent = item.ParentId.ToString(), text = GenerateOrganogramLevelNodeText(item).ToString() });
                AddOrganogramLevelChild(nodes, item);
            }
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        private void AddOrganogramLevelChild(List<JsTreeModel> nodes, PRM_OrganogramLevel item)
        {
            var childs = _prmCommonService.PRMUnit.OrganogramLevelRepository.Get(t => t.ParentId == item.Id).DefaultIfEmpty().OfType<PRM_OrganogramLevel>().ToList();

            if (childs != null && childs.Count > 0)
            {
                foreach (var anChild in childs)
                {
                    nodes.Add(new JsTreeModel() { id = anChild.Id.ToString(), parent = anChild.ParentId.ToString(), text = GenerateOrganogramLevelNodeText(anChild).ToString() });
                    AddOrganogramLevelChild(nodes, anChild);
                }
            }
        }

        private static StringBuilder GenerateOrganogramLevelNodeText(PRM_OrganogramLevel parentNode)
        {
            StringBuilder lvlName = new StringBuilder();
            lvlName.Append(parentNode.LevelName);

            if (parentNode.PRM_OrganogramType != null)
            {
                lvlName.Append(" [");
                lvlName.Append(parentNode.PRM_OrganogramType.Name);
                lvlName.Append("]");
            }
            return lvlName;
        }

        public void PopulateOrganogramLevelTree(PRM_OrganogramLevel parentNode, JsTreeNode jsTNode, List<PRM_OrganogramLevel> nodes)
        {
            StringBuilder nodeText = new StringBuilder();
            jsTNode.children = new List<JsTreeNode>();
            foreach (var dr in nodes)
            {
                if (dr != null)
                {
                    if (dr.ParentId == parentNode.Id)
                    {
                        JsTreeNode cnode = new JsTreeNode();
                        cnode.attr = new Attributes();
                        cnode.attr.id = Convert.ToString(dr.Id);
                        cnode.attr.rel = "folder" + dr.Id;
                        cnode.data = new Data();
                        nodeText = GenerateOrganogramLevelNodeText(dr);
                        cnode.data.title = Convert.ToString(nodeText);

                        cnode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";

                        jsTNode.children.Add(cnode);
                        PopulateOrganogramLevelTree(dr, cnode, nodes);
                    }
                }
            }
        }

        #endregion

        #region Grid Dropdown list

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _empService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.SortingOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _empService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        [NoCache]
        public ActionResult GetJobLocation()
        {
            var jobLocations = _empService.PRMUnit.JobLocationRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            //var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.Id).ToList();
            //return PartialView("Select", Common.PopulateJobGradeDDL(grades));
            return PartialView("Select", Common.PopulateCurrentJobGradeDDL(_empService));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _empService.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(empStatus));
        }

        #endregion

        [NoCache]
        public JsonResult GetPurchaseInfo(int? purchaseId)
        {
            var PurchaseDate = string.Empty;
            var SupplierId = string.Empty;

            if (purchaseId > 0)
            {
                var obj = _invCommonService.INVUnit.PurchaseInfoRepository.GetByID(purchaseId);
                if (obj != null)
                {
                    PurchaseDate = obj.PurchaseDate.ToString("yyyy-MM-dd");
                    SupplierId = obj.SupplierId.ToString();
                }
            }

            return Json(new
            {
                PurchaseDate = PurchaseDate,
                SupplierId = SupplierId
            }, JsonRequestBehavior.AllowGet);

        }

        [NoCache]
        public JsonResult GetItemInfo(int? itemId, int? purchaseId)
        {
            var AssetCost = string.Empty;
            var Quantity = string.Empty;

            if (itemId > 0 && purchaseId > 0)
            {
                var obj = _invCommonService.INVUnit.PurchaseItemRepository.GetAll().Where(x => x.PurchaseId == purchaseId && x.ItemId == itemId).GroupBy(x => x.ItemId)
                                                .Select(i =>
                                                        new
                                                        {
                                                            ItemId = i.Key,
                                                            Quantity = i.Sum(s => s.Quantity),
                                                            TotalCost = i.Sum(s => s.TotalCost)
                                                        }).FirstOrDefault();
                if (obj != null)
                {
                    AssetCost = (obj.TotalCost / obj.Quantity).ToString();
                    Quantity = Convert.ToInt32(obj.Quantity).ToString();
                }
            }

            return Json(new
            {
                AssetCost = AssetCost,
                Quantity = Quantity
            }, JsonRequestBehavior.AllowGet);

        }

        public string GetItemList(int? purchaseId)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });
            var items = new List<INV_ItemInfo>();

            items = (from entity in _invCommonService.INVUnit.ItemInfoRepository.Fetch()
                     select entity).OrderBy(o => o.ItemName).ToList();

            if (purchaseId != null)
            {
                var idList = new List<Int32>();
                var purchaseItems = _invCommonService.INVUnit.PurchaseItemRepository.GetAll().Where(x => x.PurchaseId == purchaseId);
                foreach (var purchaseItem in purchaseItems)
                {
                    idList.Add(purchaseItem.ItemId);
                }
                items = items.Where(x => idList.Contains(x.Id)).ToList();
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.ItemName, Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }

            return new JavaScriptSerializer().Serialize(listItems);
        }
    }
}