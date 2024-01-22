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

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class FixedAssetFinController : BaseController
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly FARCommonService _farCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public FixedAssetFinController(EmployeeService empService, FARCommonService farCommonService, PRMCommonSevice prmCommonService, INVCommonService invCommonService)
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
                    model.EmpID = empInfo.FullName + " [" + empInfo.EmpID + "]";
                    //model.EmployeeName = empInfo.FullName;
                }
            }
            if (model.OrganogramLevelId != null)
            {
                model.OrganogramLevelName = _prmCommonService.PRMUnit.OrganogramLevelRepository.GetByID(model.OrganogramLevelId).LevelName;
            }

            model.SupplierName = string.Empty;
            if (model.SupplierId != null)
            {
                model.SupplierName = _invCommonService.INVUnit.SupplierRepository.GetByID(model.SupplierId).SupplierName;
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
                    // setDeafaultVal(model);
                    var entity = model.ToEntity();
                    entity.IsAllowDepreciationCal = true;
                    entity.IsApproved = true;

                    _farCommonService.FARUnit.FixedAssetRepository.Update(entity);
                    _farCommonService.FARUnit.FixedAssetRepository.SaveChanges();
                    model.errClass = "success";
                    //model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    model.ErrMsg = "Information has been approved successfully.";
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
            //model.strMode = "Edit";
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
            ddlList = _farCommonService.FARUnit.AssetStatusRepository.GetAll().Where(q => q.Name == FAREnum.FARAssetStatus.Good.ToString() || q.Name == FAREnum.FARAssetStatus.Repairable.ToString()).OrderBy(q => q.SortOrder).ToList();
            model.AssetStatusList = Common.PopulateDllList(ddlList);
            #endregion

            #region asset condition  ddl
            //ddlList = _farCommonService.FARUnit.AssetConditionRepository.Get(q => q.Id == model.AssetConditionId).OrderBy(q => q.SortOrder).ToList();
            //model.AssetConditionList = Common.PopulateDllList(ddlList);
            #endregion

            #region supplier  ddl
            //model.AssetSupplierList = _invCommonService.INVUnit.SupplierRepository.GetAll().OrderBy(x => x.SupplierName).ToList()
            //    .Select(y =>
            //    new SelectListItem()
            //    {
            //        Text = y.SupplierName,
            //        Value = y.Id.ToString()
            //    }).ToList();
            #endregion

            #region asset condition  ddl
            ddlList = _farCommonService.FARUnit.LocationRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(q => q.SortOrder).ToList();
            model.LocationList = Common.PopulateDllList(ddlList);
            #endregion
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

            // asset ID must be between the reserved ID for Existing Asset

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
        public ActionResult GetSubCategoryList(int id)
        {
            var model = new FixedAssetViewModel();

            if (id != 0)
            {

                model.AssetSubCategoryList = (from s in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll().OrderBy(x => x.SubCategoryName)
                                              where s.CategoryId == id
                                              select s).Select(y => new SelectListItem()
                                              {
                                                  Text = y.SubCategoryCode.ToString(),
                                                  Value = y.Id.ToString()
                                              }).ToList();
            }
            else
            {
                model.AssetSubCategoryList = (from s in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll().OrderBy(x => x.SubCategoryName)
                                              select s).Select(y => new SelectListItem()
                                              {
                                                  Text = y.SubCategoryCode.ToString(),
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

        [NoCache]
        public ActionResult GetItemTypeList()
        {
            var list = Common.PopulateItemTypeDDL(_invCommonService.INVUnit.ItemTypeRepository.GetAll().Where(x => x.ParentId != null && x.IsGroup == false).ToList());
            return PartialView("Select", list);
        }

        #endregion


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
                catch (Exception)
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
    }
}