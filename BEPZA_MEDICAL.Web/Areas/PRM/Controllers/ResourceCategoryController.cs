using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using System.Collections.ObjectModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections;
using System.Data.SqlClient;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.PRM;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ResourceCategoryController : Controller
    {

        #region Fields
        private readonly ResourceCategoryService _ResourceCategoryService;
        #endregion

        #region Constructor

        public ResourceCategoryController(ResourceCategoryService service)
        {
            this._ResourceCategoryService = service;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ResourceCategorySearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.GetCount(filterExpression);

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            var list = _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1);

            if (request.SortingName == "ResourceTypeId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x =>x.PRM_ResourceType.Name);
                }
                else
                {
                    list = list.OrderByDescending(x =>x.PRM_ResourceType.Name);
                } 
            }

            foreach (PRM_ResourceCategory d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ResourceCategory,
                    d.PRM_ResourceType.Name,
                    d.ActualRate,
                    0,0,
                    d.BudgetRate,
                    0,0,                    
                    d.PRM_MeasurementUnit.Name, 
                    d.EffectiveDate.Date.ToString("dd-MMM-yyyy"),
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ViewResult Details(int id)
        {
            PRM_ResourceCategory obj = _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.GetByID(id);
            return View(obj);
        }

        [NoCache]
        public ActionResult ResourceTypeforView()
        {
            var ResourceType = Common.PopulateDllList(_ResourceCategoryService.PRMUnit.ResourceTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", ResourceType);
        }

        [NoCache]
        public ActionResult MeasurementUnitforView()
        {
            var MeasurementUnit = Common.PopulateDllList(_ResourceCategoryService.PRMUnit.MeasurementUnitRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", MeasurementUnit);
        }

        public ActionResult Create()
        {
            ResourceCategoryViewModel model = new ResourceCategoryViewModel();
            model.EffectiveDate = Common.CurrentDateTime;
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ResourceCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.ResourceCategory = model.ResourceCategory.Trim();
                if (!CheckDuplicate(model, "add"))
                {
                    var prm_resourceCategory = model.ToEntity();
                    prm_resourceCategory.IUser = User.Identity.Name;// LoginInfo.Current.LoginName;
                    prm_resourceCategory.IDate = Common.CurrentDateTime;
                    model.IsError = 1;
                    model.ErrMsg = _ResourceCategoryService.GetBusinessLogicValidation(model.ToEntity()).FirstOrDefault();

                    if (string.IsNullOrEmpty(model.ErrMsg))
                    {
                        try
                        {
                            _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.Add(prm_resourceCategory);
                            _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.SaveChanges();
                            //** To show successfull msg in same page ***/                        
                            //model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                            model.IsError = 0;
                            return RedirectToAction("Index"); // to redirect index pate
                        }
                        catch (Exception ex)
                        {
                            model.IsError = 1;
                            if (ex.InnerException != null && ex.InnerException is SqlException)
                            {
                                SqlException sqlException = ex.InnerException as SqlException;
                                model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            }
                            else
                            {
                                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                            }
                        }
                    }

                }
                else
                {
                    model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    model.IsError = 1;
                }
            }

            populateDropdown(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            PRM_ResourceCategory prm_resourceCategory = _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.GetByID(id);
            ResourceCategoryViewModel model = prm_resourceCategory.ToModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ResourceCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.ResourceCategory = model.ResourceCategory.Trim();
                if (!CheckDuplicate(model, "edit"))
                {
                    PRM_ResourceCategory prm_resourceCategory = model.ToEntity();
                    prm_resourceCategory.EUser = User.Identity.Name;
                    prm_resourceCategory.EDate = Common.CurrentDateTime;
                    ArrayList lstGradeSteps = new ArrayList();
                    model.IsError = 1;
                    model.ErrMsg = _ResourceCategoryService.GetBusinessLogicValidation(model.ToEntity()).FirstOrDefault();
                    if (string.IsNullOrEmpty(model.ErrMsg))
                    {
                        try
                        {
                            Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                            _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.Update(prm_resourceCategory, NavigationList);
                            _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.SaveChanges();
                            model.IsError = 0;
                            return RedirectToAction("Index");
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null && ex.InnerException is SqlException)
                            {
                                SqlException sqlException = ex.InnerException as SqlException;
                                model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            }
                            else
                            {
                                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                            }
                        }
                    }
                }
                else
                {
                    model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    model.IsError = 1;
                }
            }
            populateDropdown(model);
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.Delete(id);
                _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        #endregion

        #region  private method

        private void populateDropdown(ResourceCategoryViewModel model)
        {
            model.ResourceTypeList = Common.PopulateDllList(_ResourceCategoryService.PRMUnit.ResourceTypeRepository.GetAll().ToList());
            model.UOMTypeList = Common.PopulateDllList(_ResourceCategoryService.PRMUnit.MeasurementUnitRepository.GetAll().ToList());
        }

        private bool CheckDuplicate(ResourceCategoryViewModel model, string strMode)
        {
            dynamic objDesignation = null;
            try
            {
                if (strMode == "add")
                {
                    objDesignation = _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.Get(x => x.ResourceCategory == model.ResourceCategory && x.ResourceTypeId == model.ResourceTypeId).FirstOrDefault();

                }
                else
                {
                    objDesignation = _ResourceCategoryService.PRMUnit.ResourceCategoryRepository.Get(x => x.ResourceCategory == model.ResourceCategory && x.ResourceTypeId == model.ResourceTypeId && x.Id != model.Id).FirstOrDefault();

                }

                if (objDesignation != null)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        #endregion
    }
}