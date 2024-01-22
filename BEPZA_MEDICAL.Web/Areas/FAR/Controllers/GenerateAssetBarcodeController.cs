using BEPZA_MEDICAL.Domain.FAR;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using GenCode128;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.Controllers
{
    public class GenerateAssetBarcodeController : Controller
    {
        #region Fields
        private readonly FARCommonService _farCommonService;
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public GenerateAssetBarcodeController(FARCommonService farCommonService, INVCommonService invCommonService)
        {
            this._farCommonService = farCommonService;
            this._invCommonService = invCommonService;
        }
        #endregion

        // GET: FAR/GenerateAssetBarcode
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
                                                  AssetCost = FA.AssetCost,
                                                  BeneficiaryEmployeeId = FA.BeneficiaryEmployeeId
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
                    d.AssetCost                   
                }));
            }
            return new JqGridJsonResult() { Data = response };
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

        public ActionResult Barcode(int id)
        {
            FixedAssetViewModel model = new FixedAssetViewModel();
            var asset = _farCommonService.FARUnit.FixedAssetRepository.GetByID(id);
            Image myimg = Code128Rendering.MakeBarcodeImage(asset.AssetCode, 1, true);
            byte[] document = (byte[])new ImageConverter().ConvertTo(myimg, typeof(byte[]));
            model.Attachment = document;
            model.AssetCode = asset.AssetCode;
            model.AssetName = asset.AssetName;
            model.CategoryName = asset.FAR_Catagory.CategoryName;
            return View(model);
        }


    }
}