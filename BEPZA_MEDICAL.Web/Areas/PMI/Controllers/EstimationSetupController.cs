using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Utility;
using System.Text.RegularExpressions;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class EstimationSetupController : BaseController
    {
        #region Fields
        private readonly PMICommonService _pmiCommonService;
        #endregion

        #region Ctor
        public EstimationSetupController(PMICommonService pmiCommonService)
        {
            this._pmiCommonService = pmiCommonService;
        }
        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EstimationItemSetupViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            List<EstimationItemSetupViewModel> list = (from itemSetup in _pmiCommonService.PMIUnit.EstimationSetupRepository.GetAll()
                                                       join head in _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll() on itemSetup.EstimationHeadId equals head.Id
                                                       join unit in _pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll() on itemSetup.UnitId equals unit.Id
                                                       select new EstimationItemSetupViewModel
                                              {
                                                  Id = itemSetup.Id,
                                                  EstimationHead = string.IsNullOrWhiteSpace(head.ItemCode) ? head.HeadName : head.ItemCode + " : " + head.HeadName,
                                                  ItemCode = string.IsNullOrWhiteSpace(itemSetup.ItemCode) ? string.Empty : itemSetup.ItemCode,
                                                  EstimationItem = itemSetup.ItemName,
                                                  Unit = unit.Name,
                                                  UnitPrice = itemSetup.UnitPrice,
                                              }).OrderBy(x => x.EstimationHead).ToList();

            

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.EstimationHead))
                {
                    list = list.Where(x => x.EstimationHead.Trim().ToLower().Contains(viewModel.EstimationHead.Trim().ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(viewModel.EstimationItem))
                {
                    list = list.Where(x => x.EstimationItem.Trim().ToLower().Contains(viewModel.EstimationItem.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.ItemCode))
                {
                    list = list.Where(x => x.ItemCode.Trim().ToLower().Contains(viewModel.ItemCode.Trim().ToLower())).ToList();
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EstimationHead,
                    d.ItemCode,
                    d.EstimationItem,
                    d.Unit,
                    d.UnitPrice,    
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            EstimationSetupViewModel model = new EstimationSetupViewModel();
            model.strMode = "create";

            PopulateList(model);
            var anItem = new EstimationSetupViewModel();
            anItem.UnitList = Common.PopulateDllList(_pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll().OrderBy(x => x.Name));
            model.EstimationSetupViewModelList.Add(anItem);
            return View("Create", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(EstimationSetupViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                    var strMessage = string.Empty;
                    model.ErrMsg = strMessage;
                    if (!string.IsNullOrWhiteSpace(strMessage))
                    {
                        model.errClass = "failed";
                    }
                    if (!string.IsNullOrWhiteSpace(strMessage))
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = strMessage;
                        PopulateList(model);
                        foreach (var item in model.EstimationSetupViewModelList)
                        {
                            item.UnitList = Common.PopulateDllList(_pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll().OrderBy(x => x.Name));
                        }
                        return View("Create", model);
                    }


                    var finalList = model.EstimationSetupViewModelList.DefaultIfEmpty().OfType<EstimationSetupViewModel>().ToList();

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        var master = model.ToEntity();

                        var itemList = new List<PMI_EstimationSetup>();
                        foreach (var item in finalList)
                        {
                            var obj = item.ToEntity();
                            item.ItemName = Regex.Replace(item.ItemName, " {2,}", " ");
                            obj.EstimationHeadId = model.EstimationHeadId;
                            itemList.Add(obj);
                        }

                        foreach (var item in itemList)
                        {
                            _pmiCommonService.PMIUnit.EstimationSetupRepository.Add(item);
                        }
                        if (itemList.Any())
                        {
                            _pmiCommonService.PMIUnit.EstimationSetupRepository.SaveChanges();
                        }

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        PopulateList(model);


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
            foreach (var item in model.EstimationSetupViewModelList)
            {
                item.UnitList = Common.PopulateDllList(_pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll().OrderBy(x => x.Name));
            }

            return View("Index", model);
        }


        private string CheckDuplicateEntry(int estimationHeadId, List<EstimationSetupViewModel> model)
        {
            string message = string.Empty;
            var itemList = _pmiCommonService.PMIUnit.EstimationSetupRepository.Get(q => q.EstimationHeadId == estimationHeadId).ToList();
            foreach (var item in model)
            {
                var itemTypes = new List<PMI_EstimationSetup>();
                var itemCodes = new List<PMI_EstimationSetup>();

                if (!string.IsNullOrWhiteSpace(item.ItemName))
                {
                    itemTypes = (from x in itemList
                                 where x.ItemName == item.ItemName && x.Id != item.Id
                                 select x).DefaultIfEmpty().OfType<PMI_EstimationSetup>().ToList();
                }
                if (!string.IsNullOrWhiteSpace(item.ItemCode))
                {
                    itemCodes = (from x in itemList
                                 where x.ItemCode == item.ItemCode && x.Id != item.Id
                                 select x).DefaultIfEmpty().OfType<PMI_EstimationSetup>().ToList();
                }
                if (itemTypes.Any())
                {
                    message += "Estimation Item: " + item.ItemName + " already exists.";
                }
                if (itemCodes.Any())
                {
                    message += "Item Code: " + item.ItemCode + " already exists.";
                }
            }
            return message;
        }

        public ActionResult Edit(int id, string type)
        {
            var detailItem = _pmiCommonService.PMIUnit.EstimationSetupRepository.GetByID(id);
            var model = detailItem.ToModel();

            model.Description = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetByID(model.EstimationHeadId).Description;
            model.strMode = "Edit";

            if (type == "success")
            {
                model.IsError = 0;
                model.errClass = "success";
                model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
            }
            model.EstimationSetupViewModelList.Add(model);
            PopulateList(model);
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(EstimationSetupViewModel model)
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

                        var strMessage = string.Empty;
                        model.ErrMsg = strMessage;
                        if (!string.IsNullOrWhiteSpace(strMessage))
                        {
                            model.errClass = "failed";
                        }

                        if (!string.IsNullOrWhiteSpace(strMessage))
                        {
                            model.IsError = 1;
                            model.errClass = "failed";
                            model.ErrMsg = strMessage;
                            PopulateList(model);
                            foreach (var item in model.EstimationSetupViewModelList)
                            {
                                item.UnitList = Common.PopulateDllList(_pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll().OrderBy(x => x.Name));
                            }
                            return View("Edit", model);
                        }

                        foreach (var item in model.EstimationSetupViewModelList)
                        {
                            var obj = item.ToEntity();
                            obj.ItemName = Regex.Replace(item.ItemName, " {2,}", " ");
                            obj.EstimationHeadId = model.EstimationHeadId;
                            obj.EUser = User.Identity.Name;
                            obj.EDate = DateTime.Now;
                            var dbObj = _pmiCommonService.PMIUnit.EstimationSetupRepository.GetByID(obj.Id);
                            obj.IUser = dbObj.IUser;
                            obj.IDate = dbObj.IDate;

                            _pmiCommonService.PMIUnit.EstimationSetupRepository.Update(obj);
                            _pmiCommonService.PMIUnit.EstimationSetupRepository.SaveChanges();

                            model.IsError = 0;
                            model.errClass = "success";
                            model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                            return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                        }

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
            return View("Edit", model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _pmiCommonService.PMIUnit.EstimationSetupRepository.Delete(id);
                _pmiCommonService.PMIUnit.EstimationSetupRepository.SaveChanges();
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

        private bool IsDuplicateEntry(int itemId, int estimationHeadId, string itemName)
        {
            try
            {
                bool isExists = false;
                var itemList = _pmiCommonService.PMIUnit.EstimationSetupRepository.Get(t => t.EstimationHeadId == estimationHeadId).DefaultIfEmpty().OfType<PMI_EstimationSetup>().ToList();
                if (itemList != null)
                {
                    var item = itemList.Where(t => t.ItemName == itemName && t.Id != itemId).FirstOrDefault();
                    isExists = item != null ? true : false;
                }
                return isExists;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string CheckingBusinessLogicValidation(EstimationSetupViewModel model)
        {
            string message = string.Empty;
            if (model.EstimationSetupViewModelList.Count == 0)
            {
                message = "Please add item(s).";
            }

            return message;
        }

        private void PopulateList(EstimationSetupViewModel model)
        {
            model.UnitList = Common.PopulateDllList(_pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll().OrderBy(x => x.Name));
            var itemHeadList = (from unit in _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll()
                                select new EstimationHeadViewModel()
                                {
                                    Id = unit.Id,
                                    HeadName = unit.HeadName,
                                    Description = unit.Description,
                                    IsActive = Convert.ToBoolean(unit.IsActive),
                                    ItemCode = unit.ItemCode == null ? string.Empty : unit.ItemCode
                                }).OrderBy(x => x.HeadName).ToList();

            model.EstimationHeadList = itemHeadList.Select(y =>
                new SelectListItem()
                {
                    Text = string.IsNullOrWhiteSpace(y.ItemCode.Trim()) ? y.HeadName : y.ItemCode + " : " + y.HeadName,
                    Value = y.Id.ToString()
                }).ToList();

            var statusList = _pmiCommonService.PMIUnit.StatusInformationRepository.GetAll().Where(t => t.ApplicableFor == "Estimation").DefaultIfEmpty().ToList();
            model.StatusList = statusList.Select(y =>
                new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.Id.ToString()
                }).ToList();

        }

        #endregion

        #region others

        public JsonResult LoadHeadDescription(int EstimationHead)
        {
            string description = string.Empty;
            var master = _pmiCommonService.PMIUnit.EstimationHeadRepository.GetByID(EstimationHead);
            var model = master.ToModel();
            return Json(model.Description, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddItemFields(EstimationSetupViewModel model)
        {
            model.UnitList = Common.PopulateDllList(_pmiCommonService.PMIUnit.EstimationUnitRepository.GetAll().OrderBy(x => x.Name));
            return PartialView("_PartialDetail", model);
        }

        public ActionResult GetItemHeadList()
        {
            List<EstimationHeadViewModel> itemHeadList = (from unit in _pmiCommonService.PMIUnit.EstimationHeadRepository.GetAll()
                                                          select new EstimationHeadViewModel()
                                                          {
                                                              Id = unit.Id,
                                                              HeadName = unit.HeadName,
                                                              Description = unit.Description,
                                                              IsActive = Convert.ToBoolean(unit.IsActive),
                                                              ItemCode = unit.ItemCode == null ? string.Empty : unit.ItemCode
                                                          }).OrderBy(x => x.HeadName).ToList();

            Dictionary<string, string> dicFinancialYear = new Dictionary<string, string>();

            foreach (var item in itemHeadList)
            {
                string headName = string.IsNullOrWhiteSpace(item.ItemCode.Trim()) ? item.HeadName : item.ItemCode + " : " + item.HeadName;

                dicFinancialYear.Add(item.HeadName, headName.ToString());
            }
            return PartialView("_Select", dicFinancialYear);
        }

        #endregion
    }
}