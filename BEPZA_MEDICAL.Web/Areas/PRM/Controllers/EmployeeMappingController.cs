using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections;
using System.Data.SqlClient;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class EmployeeMappingController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Constructor

        public EmployeeMappingController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }

        #endregion

        #region Actions
        public ViewResult Index()
        {
            EmployeeMappingViewModel model = new EmployeeMappingViewModel();

            model.Type = "Internal";
            var obj = _prmCommonservice.PRMUnit.EmployeeMappingRepository.GetByID(model.Type, "Type");
            if (obj != null)
            {
                model = obj.ToModel();
            }
            populateDropdown(model);

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create(FormCollection collection)
        {
            string btnSubmit = collection["btnSubmit"];
            EmployeeMappingViewModel model = new EmployeeMappingViewModel();
            model.Type = collection["Type"];
            model.ResourceTypeId = Convert.ToInt32(collection["ResourceTypeId"]);
            model.ResourceCategoryId = Convert.ToInt32(collection["ResourceCategoryId"]);
            JsonResult result;
            return result = PerformEditOrCreate(model, "Save");

        }
      
        public JsonResult Edit(string Type)
        {
            var obj = _prmCommonservice.PRMUnit.EmployeeMappingRepository.GetByID(Type, "Type");
            if (obj != null)
            {
                return Json(new
                {
                    Type = obj.Type,
                    ResourceTypeId = obj.ResourceTypeId,
                    ResourceCategoryId = obj.ResourceCategoryId,
                },JsonRequestBehavior.AllowGet);
            }
            return null;

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Edit(FormCollection collection)
        {
            string btnSubmit = collection["btnSubmit"];
            EmployeeMappingViewModel model = new EmployeeMappingViewModel();
            model.Type = collection["Type"];
            model.ResourceTypeId = Convert.ToInt32(collection["ResourceTypeId"]);
            model.ResourceCategoryId = Convert.ToInt32(collection["ResourceCategoryId"]);
            JsonResult result;
            return result = PerformEditOrCreate(model, "Update");

        }


        public JsonResult DeleteConfirmed(string Type)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                PRM_HumanResourceMapping obj = _prmCommonservice.PRMUnit.EmployeeMappingRepository.GetByID(Type, "Type");
                _prmCommonservice.PRMUnit.EmployeeMappingRepository.Delete(obj);
                _prmCommonservice.PRMUnit.EmployeeMappingRepository.SaveChanges();
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
                else
                {
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
                }
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        public ActionResult LoadResourceCategory(int Id)
        {
            IList<PRM_ResourceCategory> lst = _prmCommonservice.PRMUnit.ResourceCategoryRepository.Get(q => q.ResourceTypeId == Id).OrderBy(x => x.ResourceCategory).ToList();
            return Json(
                lst.Select(x => new { Id = x.Id, ResourceCategory = x.ResourceCategory }),
                JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public ActionResult GetResourceTypeforView()
        {

            Dictionary<int, string> country = new Dictionary<int, string>();
            var countryList = _prmCommonservice.PRMUnit.ResourceTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
            foreach (PRM_ResourceType item in countryList)
                country.Add(item.Id, item.Name);
            return PartialView("Select", country);
        }

        [NoCache]
        public ActionResult GetResourceCategoryforView()
        {

            Dictionary<int, string> division = new Dictionary<int, string>();
            var itemList = _prmCommonservice.PRMUnit.ResourceCategoryRepository.GetAll().OrderBy(x => x.ResourceCategory).ToList();
            foreach (PRM_ResourceCategory item in itemList)
                division.Add(item.Id, item.ResourceCategory);
            return PartialView("Select", division);
        }          
      
        
        #endregion       
        
        
        #region  private method 

        private JsonResult PerformEditOrCreate(EmployeeMappingViewModel model, string operation)
        {
            int result = 0;
            string errMsg = "";

            if (ModelState.IsValid)
            {
                if (operation == "Save")
                {
                    var obj = model.ToEntity();
                    obj.IUser = User.Identity.Name;// LoginInfo.Current.LoginName;
                    obj.IDate = Common.CurrentDateTime;

                    try
                    {
                        _prmCommonservice.PRMUnit.EmployeeMappingRepository.Add(obj);
                        _prmCommonservice.PRMUnit.EmployeeMappingRepository.SaveChanges();
                        result = 1;
                        errMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null && ex.InnerException is SqlException)
                        {
                            SqlException sqlException = ex.InnerException as SqlException;
                            errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        }
                        else
                        {
                            errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);

                        }
                    }
                }
                if (operation == "Update")
                {
                    PRM_HumanResourceMapping obj = model.ToEntity();
                    obj.EUser = User.Identity.Name;
                    obj.EDate = Common.CurrentDateTime;

                    try
                    {
                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        _prmCommonservice.PRMUnit.EmployeeMappingRepository.Update(obj,"Type", NavigationList);
                        _prmCommonservice.PRMUnit.EmployeeMappingRepository.SaveChanges();

                        result = 1;
                        errMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null && ex.InnerException is SqlException)
                        {
                            SqlException sqlException = ex.InnerException as SqlException;
                            errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        }
                        else
                        {
                            errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        }
                    }
                }               

                return Json(new
                {
                    Success = result,
                    Message = errMsg
                });

            }
            return new JsonResult { };
        }

        private IList<PRM_ResourceType> GetResourceTypeList()
        {
            IList<PRM_ResourceType> itemList = _prmCommonservice.PRMUnit.ResourceTypeRepository.GetAll().OrderBy(x => x.Name).ToList();
            return itemList;
        }

        private IList<PRM_ResourceCategory> GetResourceCategoryList(int Id)
        {
            IList<PRM_ResourceCategory> itemList;
            if (Id > 0)
            {
                itemList = _prmCommonservice.PRMUnit.ResourceCategoryRepository.Get(q => q.ResourceTypeId == Id).OrderBy(x => x.ResourceCategory).ToList();
            }
            else
            {               
                itemList = new List<PRM_ResourceCategory>();
            }
            return itemList;
        }       

        private void populateDropdown(EmployeeMappingViewModel model)
        {
            model.ResourceTypeList = Common.PopulateDllList(_prmCommonservice.PRMUnit.ResourceTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            model.ResourceCategoryList = Common.PopulateResourceCategoryDDL(GetResourceCategoryList(model.ResourceTypeId));
        }

      
        #endregion

        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult CommonAction(FormCollection collection)
        //{
        //    string btnSubmit = collection["btnSubmit"];
        //    EmployeeMappingViewModel model = new EmployeeMappingViewModel();
        //    model.Type = collection["Type"];
        //    model.ResourceTypeId = Convert.ToInt32(collection["ResourceTypeId"]);
        //    model.ResourceCategoryId = Convert.ToInt32(collection["ResourceCategoryId"]);

        //    JsonResult result;

        //    return result = PerformEditOrCreate(model, btnSubmit);

        //}

    }
}