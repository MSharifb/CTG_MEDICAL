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
    //RR
    public class DistrictController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Constructor

        public DistrictController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }

        #endregion

        #region Actions
        public ViewResult Index()
        {
            DistrictViewModel model = new DistrictViewModel();
            model.Id = 0;
            populateDropdown(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            totalRecords = _prmCommonservice.PRMUnit.DistrictRepository.GetCount(filterExpression);

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

            var list = _prmCommonservice.PRMUnit.DistrictRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1);

            #region Sorting

            if (request.SortingName == "DistrictName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DistrictName);
                }
                else
                {
                    list = list.OrderByDescending(x => x.DistrictName);
                }
            }

            if (request.SortingName == "Division")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_CountryDivision.DivisionName);
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_CountryDivision.DivisionName);
                }
            }

            if (request.SortingName == "Country")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_CountryDivision.PRM_Country.Name);
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_CountryDivision.PRM_Country.Name);
                }
            }
            #endregion

            foreach (PRM_District d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.DistrictName,    
                    d.PRM_CountryDivision.DivisionName,
                    d.PRM_CountryDivision.PRM_Country.Name,                         
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult GetCountryforView()
        {

            Dictionary<int, string> country = new Dictionary<int, string>();
            var countryList = _prmCommonservice.PRMUnit.CountryRepository.GetAll().ToList();
            foreach (PRM_Country item in countryList)
                country.Add(item.Id, item.Name);
            return PartialView("Select", country);
        }

        [NoCache]
        public ActionResult GetDivisionforView()
        {

            Dictionary<int, string> division = new Dictionary<int, string>();
            var itemList = _prmCommonservice.PRMUnit.CountryDivisionRepository.GetAll().ToList();
            foreach (PRM_CountryDivision item in itemList)
                division.Add(item.Id, item.DivisionName);
            return PartialView("Select", division);
        }

        public ActionResult Create()
        {
            IList<PRM_Country> CountryList = GetCountry();
            ViewBag.CountryList = CountryList;
            IList<PRM_CountryDivision> CountryDivisionList = GetCountryDivision(0);
            ViewBag.CountryDivisionList = CountryDivisionList;

            return View();
        }

        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            JsonResult jsonResult = new JsonResult();
            string message = "";
            int result = 0;

            try
            {
                if (ModelState.IsValid)
                {
                    string test = collection["Name"];
                    string btnSubmit = collection["btnSubmit"];
                    DistrictViewModel model = new DistrictViewModel();
                    model.Id = Convert.ToInt32(collection["Id"]);
                    model.CountryId = Convert.ToInt32(collection["CountryId"]);
                    model.DivisionId = Convert.ToInt32(collection["DivisionId"]);
                    model.DistrictName = collection["DistrictName"];
                    jsonResult = PerformEditOrCreate(model, "Save");
                    return jsonResult;
                }
                else
                {
                    message = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            catch (Exception ex)
            {
                message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            return Json(new
            {
                Success = result,
                Message = message
            });
        }


        //[HttpPost]
        //public ActionResult Create(DistrictViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var prm_district = model.ToEntity();
        //        prm_district.IUser = User.Identity.Name;// LoginInfo.Current.LoginName;
        //        prm_district.IDate = Common.CurrentDateTime;
        //        _prmCommonservice.PRMUnit.DistrictRepository.Add(prm_district);
        //        _prmCommonservice.PRMUnit.DistrictRepository.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    IList<PRM_Country> CountryList = GetCountry();
        //    ViewBag.CountryList = CountryList;
        //    IList<PRM_CountryDivision> CountryDivisionList = GetCountryDivision(0);
        //    ViewBag.CountryDivisionList = CountryDivisionList;
        //    return View(model);
        //}        

        public JsonResult Edit(int id)
        {
            var obj = _prmCommonservice.PRMUnit.DistrictRepository.GetByID(id);
            return Json(new
            {
                Id = obj.Id,
                DistrictName = obj.DistrictName,
                CountryId = obj.PRM_CountryDivision.CountryId,
                DivisionId = obj.DivisionId
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Edit(FormCollection collection)
        {
            JsonResult jsonResult = new JsonResult();
            string message = "";
            int result = 0;

            try
            {
                if (ModelState.IsValid)
                {
                    string test = collection["Name"];
                    string btnSubmit = collection["btnSubmit"];
                    DistrictViewModel model = new DistrictViewModel();
                    model.Id = Convert.ToInt32(collection["Id"]);
                    model.CountryId = Convert.ToInt32(collection["CountryId"]);
                    model.DivisionId = Convert.ToInt32(collection["DivisionId"]);
                    model.DistrictName = collection["DistrictName"];
                    jsonResult = PerformEditOrCreate(model, "Update");
                    return jsonResult;
                }
                else
                {
                    message = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            catch (Exception ex)
            {
                message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            return Json(new
            {
                Success = result,
                Message = message
            });
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _prmCommonservice.PRMUnit.DistrictRepository.Delete(id);
                _prmCommonservice.PRMUnit.DistrictRepository.SaveChanges();
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

        #endregion

        #region  private method
        //*
        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult CommonAction(FormCollection collection)
        //{
        //    string test = collection["Name"];
        //    string btnSubmit = collection["btnSubmit"];
        //    DistrictViewModel model = new DistrictViewModel();
        //    model.Id = Convert.ToInt32(collection["Id"]);
        //    model.CountryId = Convert.ToInt32(collection["CountryId"]);
        //    model.DivisionId = Convert.ToInt32(collection["DivisionId"]);
        //    model.DistrictName = collection["DistrictName"];

        //    JsonResult result;

        //    return result = PerformEditOrCreate(model, btnSubmit);

        //}
        //*/

        public ActionResult GetDivision(int countryId)
        {
            return Json(
                GetCountryDivision(countryId).Select(x => new { Id = x.Id, DivisionName = x.DivisionName }),
                JsonRequestBehavior.AllowGet
            );
        }

        private JsonResult PerformEditOrCreate(DistrictViewModel model, string operation)
        {
            int result = 0;
            string errMsg = "";

            if (ModelState.IsValid)
            {
                if (operation == "Save")
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        errMsg = Resources.ErrorMessages.UniqueIndex;
                        return Json(new
                        {
                            Message = errMsg
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var prm_countryDivision = model.ToEntity();
                    prm_countryDivision.IUser = User.Identity.Name;// LoginInfo.Current.LoginName;
                    prm_countryDivision.IDate = Common.CurrentDateTime;

                    try
                    {
                        _prmCommonservice.PRMUnit.DistrictRepository.Add(prm_countryDivision);
                        _prmCommonservice.PRMUnit.DistrictRepository.SaveChanges();
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
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        errMsg = Resources.ErrorMessages.UniqueIndex;
                        return Json(new
                        {
                            Message = errMsg
                        }, JsonRequestBehavior.AllowGet);
                    }

                    PRM_District obj = model.ToEntity();
                    obj.EUser = User.Identity.Name;
                    obj.EDate = Common.CurrentDateTime;

                    try
                    {
                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        _prmCommonservice.PRMUnit.DistrictRepository.Update(obj, NavigationList);
                        _prmCommonservice.PRMUnit.DistrictRepository.SaveChanges();

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

        private bool CheckDuplicateEntry(DistrictViewModel model, int strMode)
        {
            if (strMode < 1)
            {                
                return _prmCommonservice.PRMUnit.DistrictRepository.Get(q => q.DivisionId == model.DivisionId && q.DistrictName == model.DistrictName).Any();
            }

            else
            {               
                return _prmCommonservice.PRMUnit.DistrictRepository.Get(q => q.DivisionId == model.DivisionId && q.DistrictName == model.DistrictName && strMode != q.Id).Any();
            }
        }

        private IList<PRM_Country> GetCountry()
        {
            IList<PRM_Country> itemList = _prmCommonservice.PRMUnit.CountryRepository.GetAll().OrderBy(x => x.Name).ToList();
            return itemList;
        }

        private IList<PRM_CountryDivision> GetCountryDivision(int countryId)
        {
            IList<PRM_CountryDivision> itemList;
            if (countryId > 0)
            {
                itemList = _prmCommonservice.PRMUnit.CountryDivisionRepository.Get(q => q.CountryId == countryId).ToList();
            }
            else
            {
                //itemList = _prmCommonservice.PRMUnit.CountryDivisionRepository.GetAll().ToList();
                itemList = new List<PRM_CountryDivision>();
            }
            return itemList;
        }

        private void populateDropdown(DistrictViewModel model)
        {
            model.CountryList = Common.PopulateDllList(_prmCommonservice.PRMUnit.CountryRepository.GetAll().OrderBy(x => x.Name).ToList());
            model.DivisionList = Common.PopulateCountryDivisionDDL(GetCountryDivision(model.CountryId).OrderBy(x => x.DivisionName));
        }
        #endregion
    }
}