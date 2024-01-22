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
    public class CountryDivisionController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Constructor

        public CountryDivisionController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            CountryDivisionViewModel model = new CountryDivisionViewModel();
            model.Id = 0;
            model.CountryList = Common.PopulateDllList(GetCountry().OrderBy(x => x.Name));
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            totalRecords = _prmCommonservice.PRMUnit.CountryDivisionRepository.GetCount(filterExpression);

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

            var list = _prmCommonservice.PRMUnit.CountryDivisionRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1);
            list.OrderBy(q => q.DivisionName).ToList();
            #region Sorting

            if (request.SortingName == "DivisionName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DivisionName);
                }
                else
                {
                    list = list.OrderByDescending(x => x.DivisionName);
                }
            }
            if (request.SortingName == "Country")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DivisionName);
                }
                else
                {
                    list = list.OrderByDescending(x => x.DivisionName);
                }
            }
            #endregion

            foreach (PRM_CountryDivision d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.DivisionName,
                    d.PRM_Country.Name,                    
                    
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
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
                    CountryDivisionViewModel model = new CountryDivisionViewModel();
                    model.Id = Convert.ToInt32(collection["Id"]);
                    model.CountryId = Convert.ToInt32(collection["CountryId"]);
                    model.DivisionName = collection["DivisionName"].Trim();
                    if (!CheckDuplicate(model, "add"))
                    {
                        return PerformEditOrCreate(model, "Save");
                    }
                    else
                    {
                        message = Resources.ErrorMessages.UniqueIndex;
                    }
                }
                else
                {
                    message = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);

                }
            }

            return Json(new
            {
                Success = result,
                Message = message
            });
        }

        [NoCache]
        public ActionResult GetCountryforView()
        {

            Dictionary<int, string> country = new Dictionary<int, string>();
            var countryList = _prmCommonservice.PRMUnit.CountryRepository.GetAll().OrderBy(x => x.Name).ToList();
            foreach (PRM_Country item in countryList)
                country.Add(item.Id, item.Name);
            return PartialView("Select", country);
        }

        public ViewResult Details(int id)
        {
            PRM_CountryDivision prm_countryDivision = _prmCommonservice.PRMUnit.CountryDivisionRepository.GetByID(id);
            return View(prm_countryDivision);

        }

        public JsonResult Edit(int id)
        {
            var obj = _prmCommonservice.PRMUnit.CountryDivisionRepository.GetByID(id);
            return Json(new
            {
                Id = obj.Id,
                CountryId = obj.CountryId,
                DivisionName = obj.DivisionName
            }, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
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
                    CountryDivisionViewModel model = new CountryDivisionViewModel();
                    model.Id = Convert.ToInt32(collection["Id"]);
                    model.CountryId = Convert.ToInt32(collection["CountryId"]);
                    model.DivisionName = collection["DivisionName"].Trim();

                    if (!CheckDuplicate(model, "edit"))
                    {
                        return PerformEditOrCreate(model, "Update");
                    }
                    else
                    {
                        message = Resources.ErrorMessages.UniqueIndex;
                    }
                }
                else
                {
                    message = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    message = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);

                }
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
                _prmCommonservice.PRMUnit.CountryDivisionRepository.Delete(id);
                _prmCommonservice.PRMUnit.CountryDivisionRepository.SaveChanges();
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

        private JsonResult PerformEditOrCreate(CountryDivisionViewModel model, string operation)
        {
            int result = 0;
            string errMsg = "";

            if (ModelState.IsValid)
            {
                if (operation == "Save")
                {
                    var prm_countryDivision = model.ToEntity();
                    prm_countryDivision.IUser = User.Identity.Name;// LoginInfo.Current.LoginName;
                    prm_countryDivision.IDate = Common.CurrentDateTime;

                    try
                    {
                        _prmCommonservice.PRMUnit.CountryDivisionRepository.Add(prm_countryDivision);
                        _prmCommonservice.PRMUnit.CountryDivisionRepository.SaveChanges();
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
                    PRM_CountryDivision prm_countryDivision = model.ToEntity();
                    prm_countryDivision.EUser = User.Identity.Name;
                    prm_countryDivision.EDate = Common.CurrentDateTime;

                    try
                    {
                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        _prmCommonservice.PRMUnit.CountryDivisionRepository.Update(prm_countryDivision, NavigationList);
                        _prmCommonservice.PRMUnit.CountryDivisionRepository.SaveChanges();
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
                //if (operation == "Clear")
                //{ return false; }

                return Json(new
                {
                    Success = result,
                    Message = errMsg
                });

                //return resultJson;
            }
            return new JsonResult { };
        }

        private IList<PRM_Country> GetCountry()
        {
            IList<PRM_Country> itemList = _prmCommonservice.PRMUnit.CountryRepository.GetAll().ToList();
            return itemList;
        }

        private bool CheckDuplicate(CountryDivisionViewModel model, string strMode)
        {
            dynamic objDesignation = null;
            try
            {
                if (strMode == "add")
                {
                    objDesignation = _prmCommonservice.PRMUnit.CountryDivisionRepository.Get(x => x.DivisionName == model.DivisionName && x.CountryId == model.CountryId).FirstOrDefault();

                }
                else
                {
                    objDesignation = _prmCommonservice.PRMUnit.CountryDivisionRepository.Get(x => x.DivisionName == model.DivisionName && x.CountryId == model.CountryId && x.Id != model.Id).FirstOrDefault();

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