using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using BEPZA_MEDICAL.Web.Resources;
using System.Web.Script.Serialization;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class ItemPurchaseController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly INVCommonService _invCommonService;
        private readonly ERP_BEPZAINVEntities _invContext;
        #endregion

        #region Ctor
        public ItemPurchaseController(PRMCommonSevice prmCommonService, INVCommonService invCommonService, ERP_BEPZAINVEntities invContext)
        {
            this._prmCommonService = prmCommonService;
            this._invCommonService = invCommonService;
            this._invContext = invContext;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ItemPurchaseSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = _invCommonService.INVUnit.PurchaseInfoRepository.GetAll().Where(x=>x.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrWhiteSpace(viewModel.EmpID))
                {
                    list = list.Where(x => x.PRM_EmploymentInfo.EmpID.Contains(viewModel.EmpID) || x.PRM_EmploymentInfo.FullName.ToLower().Contains(viewModel.EmpID.Trim().ToLower())).ToList();
                }
                if (viewModel.PurchaseDateFrom != null && viewModel.PurchaseDateTo != null)
                {
                    list = list.Where(x => x.PurchaseDate >= viewModel.PurchaseDateFrom && x.PurchaseDate <= viewModel.PurchaseDateTo).ToList();
                }

                if (viewModel.PODateFrom != null && viewModel.PODateTo != null)
                {
                    list = list.Where(x => x.PODate >= viewModel.PODateFrom && x.PODate <= viewModel.PODateTo).ToList();
                }

                if (viewModel.ChallanDateFrom != null && viewModel.ChallanDateTo != null)
                {
                    list = list.Where(x => x.ChallanDate >= viewModel.ChallanDateFrom && x.ChallanDate <= viewModel.ChallanDateTo).ToList();
                }
            }
            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                var EmpID = d.PRM_EmploymentInfo.EmpID;
                var ReceivedBy = d.PRM_EmploymentInfo.EmpID + " - " + d.PRM_EmploymentInfo.FullName;
                var PurchaseType = d.INV_PurchaseType.Name;
                var Supplier = d.INV_SupplierInfo.SupplierName;
                var PurchaseDate = d.PurchaseDate.ToString("dd-MMM-yyyy");
                var ChallanDate = d.ChallanDate != null ? d.ChallanDate.Value.ToString("dd-MMM-yyyy") : "";

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    viewModel.PurchaseDateFrom,
                    viewModel.PurchaseDateTo,
                    viewModel.PODateFrom,
                    viewModel.PODateTo,
                    viewModel.ChallanDateFrom,
                    viewModel.ChallanDateTo,
                    d.MRR,
                    d.Challan,
                    PurchaseDate,
                    ChallanDate,
                    PurchaseType,
                    Supplier,
                    EmpID,
                    ReceivedBy,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ItemPurchaseViewModel model = new ItemPurchaseViewModel();

            GenerateMRRNo(model);
            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ItemPurchaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        model.ZoneInfoId = LoggedUserZoneInfoId;

                        var master = model.ToEntity();

                        HttpFileCollectionBase files = Request.Files;
                        master = ToAttachFile(master, files);

                        model.Attachment = master.Attachment;
                        model.FileName = master.FileName;
                        int m = 1; // by mh
                        foreach (var c in model.PurchaseItemDetail)
                        {
                            master.INV_PurchaseItem.Add
                            (new INV_PurchaseItem
                            {
                                SL = m++, // by mh
                                ItemId = c.ItemId,
                                Rate = c.Rate,
                                Quantity = c.Quantity,
                                TotalCost = c.TotalCost,
                                PurchaseId = c.PurchaseId,
                                Comment = c.Comment
                            }
                            ); 
                        }

                        _invCommonService.INVUnit.PurchaseInfoRepository.Add(master);
                        _invCommonService.INVUnit.PurchaseInfoRepository.SaveChanges();
                        
                        model.Id = master.Id;  //added for voucher

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = checkoutBusinessLogic;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                    }
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsSuccessful = false;
            }

            PopulateList(model);
            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var master = _invCommonService.INVUnit.PurchaseInfoRepository.GetByID(id);

            var model = master.ToModel();
            model.strMode = "Edit";

            if (master.INV_PurchaseItem != null)
            {
                model.PurchaseItemDetail = new List<ItemPurchaseDetailViewModel>();

                foreach (var item in master.INV_PurchaseItem)
                {
                    var itemDetail = item.ToModel();
                    itemDetail.Item = item.INV_ItemInfo.ItemName;
                    itemDetail.Unit = item.INV_ItemInfo.INV_Unit == null ? string.Empty : item.INV_ItemInfo.INV_Unit.Name;

                    model.PurchaseItemDetail.Add(itemDetail);

                }
            }

            var employee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivedBy);
            if (employee != null)
            {
                model.EmpId = employee.EmpID + " - " + employee.FullName;
                
            }

            if (type == "success")
            {
                model.IsError = 0;
                model.errClass = "success";
                model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
            }
            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ItemPurchaseViewModel model)
        {
            var checkoutBusinessLogic = string.Empty;

            try
            {
                checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;

                        var master = model.ToEntity();
                        ArrayList arrtyList = new ArrayList();

                        HttpFileCollectionBase files = Request.Files;
                        master = ToAttachFile(master, files);

                        if (files == null)
                        {
                            var existingObj = _invCommonService.INVUnit.PurchaseInfoRepository.GetByID(master.Id);
                            master.Attachment = existingObj.Attachment;
                        }

                        if (model.PurchaseItemDetail != null)
                        {
                            foreach (var detail in model.PurchaseItemDetail)
                            {
                                var child = new INV_PurchaseItem()
                                {
                                    Id = detail.Id,
                                    ItemId = detail.ItemId,
                                    Rate = detail.Rate,
                                    Quantity = detail.Quantity,
                                    TotalCost = detail.TotalCost,
                                    PurchaseId = detail.PurchaseId,
                                    Comment = detail.Comment

                                };

                                // if old item then reflection will retrive old IUser & IDate
                                arrtyList.Add(child);
                            }
                        }

                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        NavigationList.Add(typeof(INV_PurchaseItem), arrtyList);
                        _invCommonService.INVUnit.PurchaseInfoRepository.Update(master, NavigationList);
                        _invCommonService.INVUnit.PurchaseInfoRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        //return RedirectToAction("Index");

                        return RedirectToAction("Edit", new { id = master.Id, type = "success" });
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = checkoutBusinessLogic;
                    }
                }
                else
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.errClass = "failed";
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    model.Message = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.Message = ErrorMessages.UpdateFailed;
                }
            }

            PopulateList(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _invCommonService.INVUnit.PurchaseItemRepository.Delete(x => x.PurchaseId == id);
                _invCommonService.INVUnit.PurchaseItemRepository.SaveChanges();

                _invCommonService.INVUnit.PurchaseInfoRepository.Delete(id);
                _invCommonService.INVUnit.PurchaseInfoRepository.SaveChanges();
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
            });
        }

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _invCommonService.INVUnit.PurchaseItemRepository.Delete(id);
                _invCommonService.INVUnit.PurchaseItemRepository.SaveChanges();
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
            });
        }
    
        #endregion

        #region Private Method

        private void GenerateMRRNo(ItemPurchaseViewModel model)
        {
            var maxMRRNo = 1;
            var newMRRNo = string.Empty;

            var zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().Where(x => x.Id == LoggedUserZoneInfoId).FirstOrDefault().ZoneCode.Trim();

            var purchaseInfo = _invCommonService.INVUnit.PurchaseInfoRepository.GetAll().Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderByDescending(x=> x.MRR);
            if (purchaseInfo.Any())
            {
                maxMRRNo = Convert.ToInt32(purchaseInfo.FirstOrDefault().MRR.Replace(zoneCode,""));
                maxMRRNo = maxMRRNo + 1;
            }

            newMRRNo = maxMRRNo.ToString();

            if (newMRRNo.Length < 7)
            { 
                newMRRNo = newMRRNo.PadLeft(7, '0');
            }

            model.MRR = string.Concat(zoneCode, newMRRNo);
            
        }

        private string CheckingBusinessLogicValidation(ItemPurchaseViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                var purchaseInfo = _invCommonService.INVUnit.PurchaseInfoRepository.GetAll().Where(x=> x.ZoneInfoId == LoggedUserZoneInfoId).ToList();
                if (purchaseInfo.Where(x=> x.MRR == model.MRR && x.Id != model.Id).Any())
                {
                    GenerateMRRNo(model);
                    message = model.Message = "Duplicate MRR#. Please try again.";
                    return message;
                }
            }

            if (model.PurchaseItemDetail != null && model.PurchaseItemDetail.Count() > 0)
            {
                foreach (var item in model.PurchaseItemDetail)
                {
                    item.TotalCost = item.Quantity * item.Rate;
                }
            }
            else
            {
                message = model.Message = "Please add item(s) to purchase.";
                return message;
            }

            return message;
        }
        private void PopulateList(ItemPurchaseViewModel model)
        {
            model.ItemTypeList = _invCommonService.INVUnit.ItemTypeRepository.GetAll().Where(x => x.ParentId != null).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemTypeName,
                    Value = y.Id.ToString()
                }).ToList();

            model.ItemList = _invCommonService.INVUnit.ItemInfoRepository.GetAll().ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemName,
                    Value = y.Id.ToString()
                }).ToList();

            model.SupplierList = _invCommonService.INVUnit.SupplierRepository.GetAll().ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.SupplierName,
                    Value = y.Id.ToString()
                }).ToList();

            model.CategoryList = Common.PopulateDllList(_invCommonService.INVUnit.CategoryRepository.GetAll().OrderBy(x => x.Name));
            model.ColorList = Common.PopulateDllList(_invCommonService.INVUnit.ColorRepository.GetAll().OrderBy(x => x.Name));
            model.ModelList = Common.PopulateDllList(_invCommonService.INVUnit.ModelRepository.GetAll().OrderBy(x => x.Name));
            model.UnitList = Common.PopulateDllList(_invCommonService.INVUnit.UnitRepository.GetAll().OrderBy(x => x.Name));
            model.PurchaseTypeList = Common.PopulateDllList(_invCommonService.INVUnit.PurchaseTypeRepository.GetAll().OrderBy(x => x.Name));

        }

        #endregion

        public PartialViewResult AddDetail(ItemPurchaseDetailViewModel model)
        {
            var master = new ItemPurchaseViewModel();
            master.PurchaseItemDetail = new List<ItemPurchaseDetailViewModel>();
            master.PurchaseItemDetail.Add(model);

            return PartialView("_Details", master);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(ItemPurchaseViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ReceivedBy);
                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmpId = obj.EmpID + " - " + obj.FullName,
                            ReceivedBy = obj.Id

                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        model.Message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        return Json(new { Result = false });

                    }
                    else
                    {
                        model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        return Json(new { Result = false });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Result = msg
                });
            }
        }

        public string GetItemList(int? typeId, int? categoryId, int? modelId, int? unitId, int? colorId)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });
            var items = new List<INV_ItemInfo>();

            items = (from entity in _invCommonService.INVUnit.ItemInfoRepository.Fetch()
                     select entity).OrderBy(o => o.ItemName).ToList();

            if (typeId != null)
            {
                int parent = typeId ?? 0;
                var typeIdList = new List<Int32>();

                typeIdList.Add(parent);
                AddChild(typeIdList, typeId);

                items = items.Where(x => typeIdList.Contains(x.TypeId ?? 0)).ToList();
            }

            if (categoryId != null)
            {
                items = items.Where(x => x.CategoryId == categoryId).ToList();
            }

            if (modelId != null)
            {
                items = items.Where(x => x.ModelId == modelId).ToList();
            }

            if (unitId != null)
            {
                items = items.Where(x => x.UnitId == unitId).ToList();
            }

            if (colorId != null)
            {
                items = items.Where(x => x.ColorId == colorId).ToList();
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

        private void AddChild(List<Int32> typeIdList, int? typeId)
        {

            var childs = _invCommonService.INVUnit.ItemTypeRepository.Get(t => t.ParentId == typeId).Select(x=> x.Id).ToList();

            if (childs != null && childs.Count > 0)
            {
                foreach (var anChild in childs)
                {
                    typeIdList.Add(anChild);
                    AddChild(typeIdList, anChild);
                }
            }
        }

        public JsonResult GetItemDetails(int? itemId)
        {
            string typeId = string.Empty;
            string unitId = string.Empty;
            string categoryId = string.Empty;
            string colorId = string.Empty;
            string modelId = string.Empty;
            string stockBalance = string.Empty;

            var _invContext = new ERP_BEPZAINVEntities();
            var balanceInfo = _invContext.fn_INV_GetItemClosingBalance(DateTime.Now, itemId, LoggedUserZoneInfoId.ToString()).FirstOrDefault();
            if (itemId > 0)
            {
                var itemInfo = _invCommonService.INVUnit.ItemInfoRepository.GetByID(itemId);
                typeId = itemInfo.TypeId == null ? "0" : itemInfo.TypeId.ToString();
                unitId = itemInfo.UnitId == null ? "0" : itemInfo.UnitId.ToString();
                categoryId = itemInfo.CategoryId == null ? "0" : itemInfo.CategoryId.ToString();
                colorId = itemInfo.ColorId == null ? "0" : itemInfo.ColorId.ToString();
                modelId = itemInfo.ModelId == null ? "0" : itemInfo.ModelId.ToString();
                stockBalance = balanceInfo == null ? "0" : balanceInfo.Balance.ToString();
            }

            return Json(new
            {
                typeId = typeId,
                unitId = unitId,
                categoryId = categoryId,
                colorId = colorId,
                modelId = modelId,
                stockBalance = stockBalance

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadFile(int purchaseInfoId)
        {
            try
            {
                var purchaseInfo = _invCommonService.INVUnit.PurchaseInfoRepository.GetByID(purchaseInfoId);
                string fileName = string.Empty;
                byte[] fileData = null;

                    if (!string.IsNullOrEmpty(purchaseInfo.FileName))
                    {
                        fileName = purchaseInfo.FileName;
                        fileData = purchaseInfo.Attachment;
                        string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                        string contentType = MimeMapping.GetMimeMapping(filePath);
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = fileName,
                            Inline = true,
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());

                        return File(fileData, contentType);
                    }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private INV_PurchaseInfo ToAttachFile(INV_PurchaseInfo purchaseInfo, HttpFileCollectionBase files)
        {
            foreach (string fileTagName in files)
            {
                HttpPostedFileBase file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.

                    int size = file.ContentLength;
                    string name = file.FileName;
                    int position = name.LastIndexOf("\\");
                    name = name.Substring(position + 1);
                    string contentType = file.ContentType;
                    byte[] fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);
                    purchaseInfo.Attachment = fileData;
                    purchaseInfo.FileName = file.FileName;
                }
            }

            return purchaseInfo;
        }

        public ActionResult VoucherPosing(int id)
        {
            string url = string.Empty;
            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = "";
            string Username = "";
            string ZoneID = "";
            if (sessionUser != null)
            {
                UserID = sessionUser.UserId;
                password = sessionUser.Password;
                Username = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneID = MyAppSession.ZoneInfoId.ToString();
            }

            var obj = _invContext.INV_uspPurchaseVoucherPosting(id).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }
    }

}