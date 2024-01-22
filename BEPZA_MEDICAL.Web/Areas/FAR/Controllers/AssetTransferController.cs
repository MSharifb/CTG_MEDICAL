using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.Controllers;
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
    public class AssetTransferController : BaseController
    {
        #region Fields
        private readonly FARCommonService _farCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        private readonly EmployeeService _empService;
        #endregion

        #region Ctor
        public AssetTransferController(FARCommonService farCommonService, PRMCommonSevice prmCommonService, EmployeeService empService)
        {
            this._farCommonService = farCommonService;
            this._prmCommonService = prmCommonService;
            this._empService = empService;
        }
        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, AssetTransferViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from AT in _farCommonService.FARUnit.AssetTransferRepository.GetAll()
                        join FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll() on AT.FixedAssetId equals FA.Id
                        join FL in _farCommonService.FARUnit.LocationRepository.GetAll() on AT.FromLocationId equals FL.Id
                        join TL in _farCommonService.FARUnit.LocationRepository.GetAll() on AT.ToLocationId equals TL.Id
                        where (FA.ZoneInfoId == LoggedUserZoneInfoId)
                        select new AssetTransferViewModel()
                        {
                            Id = AT.Id,
                            AssetCode = FA.AssetCode,
                            AssetName = FA.AssetName,
                            TransferDate = AT.TransferDate,
                            FromLocationId = AT.FromLocationId,
                            ToLocationId = AT.ToLocationId,
                            FromLocation = FL.Name,
                            ToLocation = TL.Name,
                            ExpectedDateOfReturn = AT.ExpectedDateOfReturn.HasValue ? AT.ExpectedDateOfReturn : null,
                        }).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.AssetCode))
                {
                    list = list.Where(d => d.AssetCode == viewModel.AssetCode).ToList();
                }

                if (!string.IsNullOrEmpty(viewModel.AssetName))
                {
                    list = list.Where(d => d.AssetName.ToLower().Trim().Contains(viewModel.AssetName.ToLower().Trim())).ToList();
                }

                if (viewModel.FromLocationId > 0)
                {
                    list = list.Where(d => d.FromLocationId == viewModel.FromLocationId).ToList();
                }

                if (viewModel.ToLocationId > 0)
                {
                    list = list.Where(d => d.ToLocationId == viewModel.ToLocationId).ToList();
                }

                if ((viewModel.TransferDateBetween != null && viewModel.TransferDateBetween != DateTime.MinValue) && (viewModel.TransferDateAnd != null && viewModel.TransferDateAnd != DateTime.MinValue))
                {
                    list = list.Where(d => d.TransferDate >= viewModel.TransferDateBetween && d.TransferDate <= viewModel.TransferDateAnd).ToList();
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

            if (request.SortingName == "TransferDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TransferDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TransferDate).ToList();
                }
            }

            if (request.SortingName == "FromLocation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FromLocation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FromLocation).ToList();
                }
            }

            if (request.SortingName == "ToLocation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ToLocation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ToLocation).ToList();
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
                    d.TransferDate!=null?Convert.ToDateTime(d.TransferDate).ToString(DateAndTime.GlobalDateFormat):default(string),
                    "TransferDateBetween",
                    "TransferDateAnd",
                    d.FromLocationId,
                    d.ToLocationId,
                    d.FromLocation,
                    d.ToLocation                   
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        public ActionResult Create()
        {
            AssetTransferViewModel model = new AssetTransferViewModel();
            model.strMode = "Create";
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(AssetTransferViewModel model)
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
                model.IUser = HttpContext.User.Identity.Name;
                model.IDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _farCommonService.FARUnit.AssetTransferRepository.Add(entity);
                    _farCommonService.FARUnit.AssetTransferRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }

                catch (Exception ex)
                {
                    model.IsError = 1;
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
            populateDropdown(model);
            // GetAssetInfoForCreate(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var entity = _farCommonService.FARUnit.AssetTransferRepository.GetByID(id);
            var model = entity.ToModel();
            //model.FromAssetConditionId = entity.FromAssetConditionId;
            populateDropdown(model);
            setDataForEdit(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(AssetTransferViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = BusinessLogicValidation(model);
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    model.EUser = HttpContext.User.Identity.Name;
                    model.EDate = DateTime.Now;
                    var entity = model.ToEntity();
                    _farCommonService.FARUnit.AssetTransferRepository.Update(entity);
                    _farCommonService.FARUnit.AssetTransferRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }

                catch (Exception ex)
                {
                    model.IsError = 0;
                    model.errClass = "failed";
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
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
            populateDropdown(model);
            setDataForEdit(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = string.Empty;
            AssetTransferViewModel model = _farCommonService.FARUnit.AssetTransferRepository.GetByID(id).ToModel();
            model.strMode = "Delete";
            errMsg = BusinessLogicValidation(model);

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    _farCommonService.FARUnit.AssetTransferRepository.Delete(id);
                    _farCommonService.FARUnit.AssetTransferRepository.SaveChanges();
                    result = true;
                }
                catch (UpdateException ex)
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
                catch (Exception ex)
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return Json(new
            {
                Success = result,
                Message = result ? "Information has been deleted successfully." : errMsg
            });
        }

        #endregion

        #region Private

        private void populateDropdown(AssetTransferViewModel model)
        {
            dynamic ddlList;

            #region To Asset Condition ddl
            ddlList = _farCommonService.FARUnit.AssetConditionRepository.GetAll().Where(q => q.AssetStatusId == model.AssetStatusId).OrderBy(q => q.SortOrder).ToList();
            model.ToAssetConditionList = Common.PopulateDllList(ddlList);
            #endregion

            #region Location  ddl
            ddlList = _farCommonService.FARUnit.LocationRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(q => q.SortOrder).ToList();
            model.ToLocationList = Common.PopulateDllList(ddlList);
            #endregion

            #region Zone  ddl
            ddlList = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(q => q.SortOrder).ToList();
            model.ToZoneList = Common.PopulateDdlZoneList(ddlList);
            #endregion

        }

        private void setDataForEdit(AssetTransferViewModel model)
        {
            var entity = _farCommonService.FARUnit.AssetTransferRepository.GetByID(model.Id);
            model.AssetCode = entity.FAR_FixedAsset.AssetCode;
            model.AssetName = entity.FAR_FixedAsset.AssetName;
            model.CategoryName = entity.FAR_FixedAsset.FAR_Catagory.CategoryName;
            model.SubCategoryName = entity.FAR_FixedAsset.FAR_SubCategory.SubCategoryName;
            model.AssetStatusName = entity.FAR_AssetStatus.Name;
            //model.FromAssetCondition = entity.FAR_AssetCondition.Name;
            model.FromLocation = entity.FAR_Location.Name;
            model.ToOrganogramLevelName = entity.ToOrganogramLevelId != null ? _prmCommonService.PRMUnit.OrganogramLevelRepository.Get(q => q.Id == entity.ToOrganogramLevelId).FirstOrDefault().LevelName : string.Empty;

            #region From Beneficiary Employee
            var obj = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == model.FromBeneficiaryEmployeeId).FirstOrDefault();
            if (obj != null)
            {
                model.FromBeneficiaryEmployee = obj.FullName;
            }
            #endregion

            #region To Beneficiary Employee
            var objTo = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == model.ToBeneficiaryEmployeeId).FirstOrDefault();
            if (objTo != null)
            {
                model.ToBeneficiaryEmployee = objTo.FullName;
            }
            #endregion

            #region Issued By
            var objIssuedBy = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == model.IssuedBy).FirstOrDefault();
            if (objIssuedBy != null)
            {
                model.IssuedEmployeeBy = objIssuedBy.FullName;
            }
            #endregion

            #region IReceived By
            var objReceivedBy = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == model.ReceivedBy).FirstOrDefault();
            if (objReceivedBy != null)
            {
                model.ReceivedEmployeeBy = objReceivedBy.FullName;
            }
            #endregion

            #region ReturnBy
            var objReturnBy = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => x.Id == model.ReturnBy).FirstOrDefault();
            if (objReturnBy != null)
            {
                model.ReturnEmployeeBy = objReturnBy.FullName;
            }
            #endregion
        }

        #endregion

        #region Utilities-----

        public JsonResult AutoCompleteForAsset(string term)
        {
            var result = (from r in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                          where r.ZoneInfoId == LoggedUserZoneInfoId && r.IsApproved == true && r.AssetCode.ToLower().Contains(term.ToLower())
                          select new { r.Id, r.AssetCode, r.AssetName }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetAssetInformation(string AssetCode)
        {
            string errorList = string.Empty;
            errorList = GetValidationChecking(AssetCode);

            var obj = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                       join AC in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals AC.Id
                       join ASC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals ASC.Id
                       join AST in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on FA.AssetStatusId equals AST.Id
                       join LC in _farCommonService.FARUnit.LocationRepository.GetAll() on FA.LocationId equals LC.Id

                       join Emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on FA.BeneficiaryEmployeeId equals Emp.Id into grp
                       from subP in grp.DefaultIfEmpty()

                       join ACDN in _farCommonService.FARUnit.AssetConditionRepository.GetAll() on FA.AssetConditionId equals ACDN.Id
                       where FA.AssetCode == AssetCode
                       select new AssetTransferViewModel()
                       {
                           FixedAssetId = FA.Id,
                           AssetName = FA.AssetName,
                           CategoryName = AC.CategoryName,
                           SubCategoryName = ASC.SubCategoryName,
                           AssetStatusId = FA.AssetStatusId,
                           AssetStatusName = AST.Name,
                           FromLocationId = FA.LocationId,
                           FromLocation = LC.Name,

                           FromBeneficiaryEmployee = subP != null ? subP.FullName : String.Empty,
                           FromBeneficiaryEmployeeId = FA.BeneficiaryEmployeeId,

                           FromAssetConditionId = FA.AssetConditionId,
                           FromAssetCondition = ACDN.Name

                       }).FirstOrDefault();

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (obj != null)
                    {
                        return Json(new
                        {
                            FixedAssetId = obj != null ? obj.FixedAssetId : default(int),
                            AssetName = obj != null ? obj.AssetName : default(string),
                            CategoryName = obj != null ? obj.CategoryName : default(string),
                            SubCategoryName = obj != null ? obj.SubCategoryName : default(string),
                            AssetStatusId = obj != null ? obj.AssetStatusId : default(int),
                            AssetStatus = obj != null ? obj.AssetStatusName : default(string),
                            FromLocationId = obj != null ? obj.FromLocationId : default(int),
                            FromLocation = obj != null ? obj.FromLocation : default(string),
                            FromBeneficiaryEmployee = obj != null ? obj.FromBeneficiaryEmployee : default(string),
                            FromBeneficiaryEmployeeId = obj != null ? obj.FromBeneficiaryEmployeeId : default(int),

                            FromAssetConditionId = obj != null ? obj.FromAssetConditionId : default(int),
                            FromAssetCondition = obj != null ? obj.FromAssetCondition : default(string),
                        }, JsonRequestBehavior.AllowGet);
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

        private AssetTransferViewModel GetAssetInfoForCreate(AssetTransferViewModel model)
        {
            string department = Convert.ToString(FAREnum.FARBeneficiaryType.Department);
            return model;
        }

        private AssetTransferViewModel GetAssetInfoForEdit(AssetTransferViewModel model)
        {

            var obj = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                       join AT in _farCommonService.FARUnit.AssetTransferRepository.GetAll() on FA.Id equals AT.FixedAssetId
                       join AC in _farCommonService.FARUnit.AssetCategoryRepository.GetAll() on FA.CategoryId equals AC.Id
                       join ASC in _farCommonService.FARUnit.AssetSubCategoryRepository.GetAll() on FA.SubCategoryId equals ASC.Id
                       join AST in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on FA.AssetStatusId equals AST.Id
                       join LC in _farCommonService.FARUnit.LocationRepository.GetAll() on FA.LocationId equals LC.Id
                       join Emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on FA.BeneficiaryEmployeeId equals Emp.Id into bEmp

                       /*RH#02*/
                       join ACDN in _farCommonService.FARUnit.AssetConditionRepository.GetAll() on FA.AssetConditionId equals ACDN.Id
                       /*End RH#02*/

                       from subEmp in bEmp.DefaultIfEmpty()

                       where FA.Id == model.FixedAssetId

                       select new AssetTransferViewModel()
                       {
                           AssetCode = FA.AssetCode,
                           AssetName = FA.AssetName,
                           CategoryName = AC.CategoryName,
                           SubCategoryName = ASC.SubCategoryName,
                           AssetStatusName = AST.Name,
                           FromLocation = LC.Name,
                           FromBeneficiaryEmployee = subEmp == null ? string.Empty : subEmp.FullName,
                           TransferDate = AT.TransferDate,
                           ExpectedDateOfReturn = AT.ExpectedDateOfReturn.HasValue ? AT.ExpectedDateOfReturn : null,

                           /*RH#02*/
                           FromAssetConditionId = FA.AssetConditionId,
                           FromAssetCondition = ACDN.Name
                           /*End RH#02*/

                       }).FirstOrDefault();

            if (obj != null)
            {
                model.AssetCode = obj.AssetCode;
                model.AssetName = obj.AssetName;
                model.CategoryName = obj.CategoryName;
                model.SubCategoryName = obj.SubCategoryName;
                model.AssetStatusName = obj.AssetStatusName;
                model.FromLocation = obj.FromLocation;
                model.FromBeneficiaryEmployee = obj.FromBeneficiaryEmployee;
                model.FromAssetConditionId = obj != null ? obj.FromAssetConditionId : default(int);
                model.FromAssetCondition = obj != null ? obj.FromAssetCondition : string.Empty;

                //model.FromAssetCondition = obj

            }
            return model;
        }

        private string BusinessLogicValidation(AssetTransferViewModel model)
        {
            string errMessage = string.Empty;
            string disposed = Convert.ToString(FAREnum.FARAssetStatus.Disposed);
            string sold = Convert.ToString(FAREnum.FARAssetStatus.Sold);

            if (model.ToOrganogramLevelId == null && model.ToBeneficiaryEmployeeId == null)
            {
                errMessage = "select Organogram Level or To Beneficiary Employee.";
            }

            if (model.strMode != "Delete")
            {
                //int[] location = new int[2];
                //location[0] = model.FromLocationId;
                //location[1] = model.ToLocationId;

                //if (location.Distinct().Count() < 2)
                if (model.FromLocationId == model.ToLocationId)
                {
                    errMessage = "From Location and To Location must be different for asset transfer.";
                }
            }

            if (model.strMode == "Create")
            {
                var transferAsset = (from T in _farCommonService.FARUnit.AssetTransferRepository.GetAll() where T.FixedAssetId == model.FixedAssetId select T).OrderBy(x => x.TransferDate).LastOrDefault();
                if (transferAsset != null)
                {
                    if (transferAsset.TransferDate >= model.TransferDate)
                    {
                        errMessage = "Transfer date must be later than Transfer from date of an asset transfer.";
                    }
                }
            }

            var obj = (from AD in _farCommonService.FARUnit.AssetDepreciationRepository.GetAll()
                       join ADD in _farCommonService.FARUnit.AssetDepreciationDetailRepository.GetAll() on AD.Id equals ADD.DepreciationId
                       where ADD.FixedAssetId == model.FixedAssetId
                       select AD).OrderByDescending(x => x.ProcessDate).FirstOrDefault();
            if (obj != null)
            {
                if (obj.ProcessDate >= model.TransferDate)
                {
                    errMessage = "Transfer date must be greater than last depreciation calculation date for the selected asset.";
                }
                if (model.strMode == CrudeAction.Edit)
                {
                    errMessage = "Any changes cannot be done if depreciation process has been executed for the selected asset.";
                }
                if (model.strMode == CrudeAction.Delete)
                {
                    errMessage = "Deletion cannot be done if depreciation process has been executed for the selected asset.";
                }
            }

            if (model.strMode == "Delete")
            {
                var objFixedAsset = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                     where FA.Id == model.FixedAssetId
                                     join St in _farCommonService.FARUnit.AssetStatusRepository.GetAll() on FA.AssetStatusId equals St.Id
                                     select new { St.Name, FA.AssetCode }).LastOrDefault();

                if (objFixedAsset != null)
                {
                    if (objFixedAsset.Name.Trim().ToLower() == disposed.ToLower() || objFixedAsset.Name.Trim().ToLower() == sold.ToLower())
                    {
                        errMessage = "Deletion cannot be done for the " + objFixedAsset.AssetCode + " because asset status is Sold or disposed.";
                    }
                }

                var tansferId = (from t in _farCommonService.FARUnit.AssetTransferRepository.GetAll() where t.FixedAssetId == model.FixedAssetId select t.Id).LastOrDefault();

                if (model.Id != tansferId)
                {
                    errMessage = "Only last transfer information of an asset allow to delete.";
                }
            }

            return errMessage;
        }

        private string FindBeneficiary(int id)
        {
            string beneficiary = string.Empty;
            string employee = Convert.ToString(FAREnum.FARBeneficiaryType.Employee);
            string department = Convert.ToString(FAREnum.FARBeneficiaryType.Department);
            string other = Convert.ToString(FAREnum.FARBeneficiaryType.Other);

            var objBeneficiary = (from B in _farCommonService.FARUnit.FixedAssetRepository.GetAll() where B.Id == id select B).FirstOrDefault();
            if (objBeneficiary != null)
            {
                //if (objBeneficiary.BeneficiaryType == employee)
                //{
                //    beneficiary = (from emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() where emp.Id == objBeneficiary.BeneficiaryEmpOrDeptId select emp).FirstOrDefault().FullName;
                //}
                //else if (objBeneficiary.BeneficiaryType == department)
                //{
                //    beneficiary = (from emp in _prmCommonservice.PRMUnit.DivisionRepository.GetAll() where emp.Id == objBeneficiary.BeneficiaryEmpOrDeptId select emp).FirstOrDefault().Name;
                //}
                //else if (objBeneficiary.BeneficiaryType == other)
                //{
                //    beneficiary = objBeneficiary.BeneficiaryOther;
                //}
                //else
                //{
                //    beneficiary = "N/A";
                //}
            }
            return beneficiary;
        }

        private string GetValidationChecking(string AssetID)
        {
            string msg = string.Empty;
            string disposed = Convert.ToString(FAREnum.FARAssetStatus.Disposed);
            string sold = Convert.ToString(FAREnum.FARAssetStatus.Sold);

            var objFixedAsset = (from FA in _farCommonService.FARUnit.FixedAssetRepository.GetAll()
                                 where FA.AssetCode == AssetID
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

        #endregion

        #region Employee GridList
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, string st, int ZoneInfoId, int? OrganogramLevelId)
        {
            string filterExpression = String.Empty, LoginEmpId = "";
            int totalRecords = 0;
            if (string.IsNullOrEmpty(st))
                st = "";
            viewModel.ZoneInfoId = ZoneInfoId;
            viewModel.OrganogramLevelId = OrganogramLevelId;

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

        #region Employee Grid Dropdown list

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
            var jobLocations = _farCommonService.FARUnit.LocationRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            // var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.Id).ToList();
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

        public ActionResult ZoneWiseEmployeeSearch(int ZoneInfoId, int? OrganogramLevelId)
        {
            var model = new EmployeeSearchViewModel();
            model.SearchEmpType = "active";
            //model.UseTypeEmpId = UseTypeEmpId;
            model.ZoneInfoId = ZoneInfoId;
            model.OrganogramLevelId = OrganogramLevelId;
            return View("ZoneWiseEmployeeSearch", model);
        }

        #region Oraganogram Level Tree

        public ActionResult OrganogramLevelTreeSearchList(int zoneId)
        {
            AssetTransferViewModel model = new AssetTransferViewModel();
            if (zoneId < 0)
            {
                return View("Create", model);
            }
            ViewBag.ZoneInfoId = zoneId;
            return PartialView("_ZoneWiseOrganogramLevelTree");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetOrganogramTreeData(int zoneId)
        {
            var nodes = new List<JsTreeModel>();
            var parentNodes = _prmCommonService.PRMUnit.OrganogramLevelRepository.GetAll().Where(q => q.ZoneInfoId == null || q.ZoneInfoId == zoneId).ToList();
            var parentNode = parentNodes.Where(x => x.ParentId == 0).FirstOrDefault();
            nodes.Add(new JsTreeModel() { id = parentNode.Id.ToString(), parent = "#", text = parentNode.LevelName });
            var childs = _prmCommonService.PRMUnit.OrganogramLevelRepository.Get(q => q.ParentId == parentNode.Id && q.ZoneInfoId == zoneId).ToList();
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


        //
        [NoCache]
        public ActionResult GetFromLocationList()
        {
            var items = _farCommonService.FARUnit.LocationRepository.GetAll().OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(items));
        }

        [NoCache]
        public ActionResult GetToLocationList()
        {
            var items = _farCommonService.FARUnit.LocationRepository.GetAll().OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(items));
        }
    }
}