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
    public class ResourceInfoController : Controller
    {

        #region Fields
        private readonly ResourceInfoService _RresourceInfoService;
        #endregion

        #region Constructor

        public ResourceInfoController(ResourceInfoService service)
        {
            this._RresourceInfoService = service;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ResourceInfoSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _RresourceInfoService.PRMUnit.ResourceInfoRepository.GetCount(filterExpression);

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

            var list = _RresourceInfoService.PRMUnit.ResourceInfoRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1).ToList();

            #region sorting

            if (request.SortingName == "ID")
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

            if (request.SortingName == "ResourceName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ResourceName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ResourceName).ToList();
                }
            }

            if (request.SortingName == "ResourceTypeId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_ResourceType.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_ResourceType.Name).ToList();
                }
            }

            if (request.SortingName == "ResourceCategoryId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_ResourceCategory.ResourceCategory).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_ResourceCategory.ResourceCategory).ToList();
                }
            }

            if (request.SortingName == "UOMId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_ResourceCategory.PRM_MeasurementUnit.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_ResourceCategory.PRM_MeasurementUnit.Name).ToList();
                }
            }

            if (request.SortingName == "ActualRate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ActualRate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ActualRate).ToList();
                }
            }

            if (request.SortingName == "BudgetRate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BudgetRate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BudgetRate).ToList();
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

            #endregion

            foreach (PRM_ResourceInfo d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,    
                    d.ResourceName,
                    d.PRM_ResourceType.Name,
                    d.PRM_ResourceCategory.ResourceCategory,                    
                    d.PRM_ResourceCategory.PRM_MeasurementUnit.Name, 
                    d.ActualRate,
                    0,0,
                    d.BudgetRate,
                    0,0,
                    d.EffectiveDate.Date.ToString("dd-MMM-yyyy"),
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ViewResult Details(int id)
        {
            PRM_ResourceInfo obj = _RresourceInfoService.PRMUnit.ResourceInfoRepository.GetByID(id);
            return View(obj);
        }

        [NoCache]
        public ActionResult ResourceTypeforView()
        {
            var itemList = _RresourceInfoService.PRMUnit.ResourceTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
            var resourceType = Common.PopulateDllList(itemList);
            return PartialView("Select", resourceType);
        }

        [NoCache]
        public ActionResult ResourceCategoryforView()
        {
            var ResourceCategory = Common.PopulateResourceCategoryDDL(_RresourceInfoService.PRMUnit.ResourceCategoryRepository.GetAll().OrderBy(x => x.ResourceCategory).ToList());
            return PartialView("Select", ResourceCategory);
        }

        [NoCache]
        public ActionResult MeasurementUnitforView()
        {
            var itemList = Common.PopulateDllList(_RresourceInfoService.PRMUnit.MeasurementUnitRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }


        [NoCache]
        public JsonResult GetResourceCategoryDetail(ResourceInfoViewModel model)
        {
            var obj = _RresourceInfoService.PRMUnit.ResourceCategoryRepository.GetByID(model.ResourceCategoryId);
            return Json(new
            {
                ActualRate = obj.ActualRate,
                BudgetRate = obj.BudgetRate,
                UOM = obj.PRM_MeasurementUnit.Name
            });

        }

        [NoCache]
        public JsonResult GetEmployeeInfo(ResourceInfoViewModel model)
        {
            string EmpType = "";
            int ResourceTypeId = 0;
            int ResourceCategoryId = 0;
            
            var obj = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            EmpType = obj.IsConsultant == true ? "External" : "Internal";
            var empMap = _RresourceInfoService.PRMUnit.EmployeeMappingRepository.Get(q => q.Type == EmpType).FirstOrDefault();
            if (empMap != null)
            {
                ResourceTypeId = empMap.ResourceTypeId;
                ResourceCategoryId = empMap.ResourceCategoryId;
            }
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeInitial = obj.EmployeeInitial,
                FullName = obj.FullName,
                Designation = obj.PRM_Designation.Name,
                Division = obj.PRM_Division.Name,
                ResourceTypeId = ResourceTypeId,
                ResourceCategoryId = ResourceCategoryId
            });

        }

        public ActionResult Create()
        {
            ResourceInfoViewModel model = new ResourceInfoViewModel();
            model.EffectiveDate = Common.CurrentDateTime;
            model.IsHumanResource = true;
            model.IsActive = true;
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ResourceInfoViewModel model)
        {
            List<string> errorList = new List<string>();
            
            if (ModelState.IsValid)
            {
                var prm_resourceInfo = model.ToEntity();
                prm_resourceInfo.IUser = User.Identity.Name;
                prm_resourceInfo.IDate = Common.CurrentDateTime;
                model.IsError = 1;
                model.ErrMsg = _RresourceInfoService.GetBusinessLogicValidation(model.ToEntity()).FirstOrDefault();
                if (model.ResourceCategoryId == 0 && model.ResourceTypeId == 0)
                {
                    model.ErrMsg = "Employee mapping is not done.";
                }
               

                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    try
                    {
                        _RresourceInfoService.PRMUnit.ResourceInfoRepository.Add(prm_resourceInfo);
                        _RresourceInfoService.PRMUnit.ResourceInfoRepository.SaveChanges();
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
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);                           
                        }
                    }
                }
            }

            populateDropdown(model);

            //readonly fields
            if (model.EmployeeId != 0)
            {
                var obj = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                model.EmpId = obj.EmpID;
                model.Division = obj.PRM_Division.Name;
                model.EmployeeInitial = obj.EmployeeInitial;
                model.Designation = obj.PRM_Designation.Name;
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            PRM_ResourceInfo prm_resourceInfo = _RresourceInfoService.PRMUnit.ResourceInfoRepository.GetByID(id);
           
            ResourceInfoViewModel model = prm_resourceInfo.ToModel();
            populateDropdown(model);
            model.IsHumanResource = false;
            //readonly fields
            model.UOM = prm_resourceInfo.PRM_ResourceCategory.PRM_MeasurementUnit.Name;
            if (model.EmployeeId != 0)
            {
                var obj = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                model.EmpId = obj.EmpID;
                model.Division = obj.PRM_Division.Name;
                model.EmployeeInitial = obj.EmployeeInitial;
                model.Designation = obj.PRM_Designation.Name;
                model.IsHumanResource = true;
            }
            

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ResourceInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.IsError = 1;
                PRM_ResourceInfo prm_resourceInfo = model.ToEntity();
                prm_resourceInfo.EUser = User.Identity.Name;
                prm_resourceInfo.EDate = Common.CurrentDateTime;
                ArrayList lstGradeSteps = new ArrayList();
                model.ErrMsg = _RresourceInfoService.GetBusinessLogicValidation(model.ToEntity()).FirstOrDefault();
                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    try
                    {
                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        _RresourceInfoService.PRMUnit.ResourceInfoRepository.Update(prm_resourceInfo, NavigationList);
                        _RresourceInfoService.PRMUnit.ResourceInfoRepository.SaveChanges();
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
            populateDropdown(model);
            //readonly fields
            if (model.EmployeeId != 0)
            {
                var obj = _RresourceInfoService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                model.EmpId = obj.EmpID;
                model.Division = obj.PRM_Division.Name;
                model.EmployeeInitial = obj.EmployeeInitial;
                model.Designation = obj.PRM_Designation.Name;
            }
            return View(model);
        }
       
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _RresourceInfoService.PRMUnit.ResourceInfoRepository.Delete(id);
                _RresourceInfoService.PRMUnit.ResourceInfoRepository.SaveChanges();
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

        private void populateDropdown(ResourceInfoViewModel model)
        {
            model.ResourceTypeList = Common.PopulateDllList(_RresourceInfoService.PRMUnit.ResourceTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            model.ResourceCategoryList = Common.PopulateResourceCategoryDDL(_RresourceInfoService.PRMUnit.ResourceCategoryRepository.Get(q => q.ResourceTypeId == model.ResourceTypeId).OrderBy(x => x.ResourceCategory).ToList());
        }

        public ActionResult LoadResourceCategory(int Id)
        {
            IList<PRM_ResourceCategory> lst = _RresourceInfoService.PRMUnit.ResourceCategoryRepository.Get(q => q.ResourceTypeId == Id).OrderBy(x => x.ResourceCategory).ToList();
            return Json(
                lst.Select(x => new { Id = x.Id, ResourceCategory = x.ResourceCategory }),
                JsonRequestBehavior.AllowGet
            );
        }
        #endregion
    }
}