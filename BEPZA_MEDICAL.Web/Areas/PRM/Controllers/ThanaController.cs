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
    public class ThanaController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Constructor

        public ThanaController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }

        #endregion

        #region Actions

        public ViewResult ThanaIndex()
        {
            ThanaViewModel model = new ThanaViewModel();
            model.Id = 0;
            IList<PRM_Country> CountryList = GetCountry();
            model.CountryList = Common.PopulateDllList(GetCountry());
            model.DivisionList = Common.PopulateCountryDivisionDDL(GetCountryDivision(model.CountryId));
            model.DistrictList = Common.PopulateDistrictDDL(GetDistrict(model.DivisionId));

            return View("Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ThanaCreate(FormCollection collection)
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
                    ThanaViewModel model = new ThanaViewModel();
                    model.Id = Convert.ToInt32(collection["Id"]);
                    model.CountryId = Convert.ToInt32(collection["CountryId"]);
                    model.DivisionId = Convert.ToInt32(collection["DivisionId"]);
                    model.DistrictId = Convert.ToInt32(collection["DistrictId"]);
                    model.ThanaName = collection["ThanaName"];
                    return PerformEditOrCreate(model, "Save");

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

        public JsonResult ThanaEdit(int id)
        {
            var obj = _prmCommonservice.PRMUnit.ThanaRepository.GetByID(id);
            return Json(new
            {
                Id = obj.Id,
                ThanaName = obj.ThanaName,
                CountryId = obj.PRM_District.PRM_CountryDivision.CountryId,
                DivisionId = obj.PRM_District.DivisionId,
                DistrictId = obj.DistrictId
            }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ThanaEdit(FormCollection collection)
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
                    ThanaViewModel model = new ThanaViewModel();
                    model.Id = Convert.ToInt32(collection["Id"]);
                    model.CountryId = Convert.ToInt32(collection["CountryId"]);
                    model.DivisionId = Convert.ToInt32(collection["DivisionId"]);
                    model.DistrictId = Convert.ToInt32(collection["DistrictId"]);
                    model.ThanaName = collection["ThanaName"];
                    return PerformEditOrCreate(model, "Update");
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            totalRecords = _prmCommonservice.PRMUnit.ThanaRepository.GetCount(filterExpression);

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

            var list = _prmCommonservice.PRMUnit.ThanaRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1);

            #region Sorting

            if (request.SortingName == "ThanaName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ThanaName);
                }
                else
                {
                    list = list.OrderByDescending(x => x.ThanaName);
                }
            }

            if (request.SortingName == "District")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_District.DistrictName);
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_District.DistrictName);
                }
            }

            if (request.SortingName == "Division")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_District.PRM_CountryDivision.DivisionName);
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_District.PRM_CountryDivision.DivisionName);
                }
            }

            if (request.SortingName == "Country")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_District.PRM_CountryDivision.PRM_Country.Name);
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_District.PRM_CountryDivision.PRM_Country.Name);
                }
            }
            #endregion

            foreach (PRM_Thana d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ThanaName,  
                    d.PRM_District.DistrictName, 
                    d.PRM_District.PRM_CountryDivision.DivisionName,                  
                    d.PRM_District.PRM_CountryDivision.PRM_Country.Name,                      
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
            var countryList = _prmCommonservice.PRMUnit.CountryRepository.GetAll().OrderBy(x => x.Name).ToList();
            foreach (PRM_Country item in countryList)
                country.Add(item.Id, item.Name);
            return PartialView("Select", country);
        }

        [NoCache]
        public ActionResult GetDivisionforView()
        {

            Dictionary<int, string> division = new Dictionary<int, string>();
            var itemList = _prmCommonservice.PRMUnit.CountryDivisionRepository.GetAll().OrderBy(x => x.DivisionName).ToList();
            foreach (PRM_CountryDivision item in itemList)
                division.Add(item.Id, item.DivisionName);
            return PartialView("Select", division);
        }

        [NoCache]
        public ActionResult GetDistrictforView()
        {

            Dictionary<int, string> district = new Dictionary<int, string>();
            var itemList = _prmCommonservice.PRMUnit.DistrictRepository.GetAll().OrderBy(x => x.DistrictName).ToList();
            foreach (PRM_District item in itemList)
                district.Add(item.Id, item.DistrictName);
            return PartialView("Select", district);
        }

        //public JsonResult Edit(int id)
        //{
        //    var obj = _prmCommonservice.PRMUnit.ThanaRepository.GetByID(id);
        //    return Json(new
        //    {
        //        Id = obj.Id,
        //        ThanaName = obj.ThanaName,
        //        CountryId = obj.PRM_District.PRM_CountryDivision.CountryId,
        //        DivisionId = obj.PRM_District.DivisionId,
        //        DistrictId = obj.DistrictId
        //    });

        //}

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _prmCommonservice.PRMUnit.ThanaRepository.Delete(id);
                _prmCommonservice.PRMUnit.ThanaRepository.SaveChanges();
                result = true;
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
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion

        #region  private method


        //*

        private JsonResult PerformEditOrCreate(ThanaViewModel model, string operation)
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
                        _prmCommonservice.PRMUnit.ThanaRepository.Add(prm_countryDivision);
                        _prmCommonservice.PRMUnit.ThanaRepository.SaveChanges();
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

                    PRM_Thana obj = model.ToEntity();
                    obj.EUser = User.Identity.Name;
                    obj.EDate = Common.CurrentDateTime;

                    try
                    {
                        Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                        _prmCommonservice.PRMUnit.ThanaRepository.Update(obj, NavigationList);
                        _prmCommonservice.PRMUnit.ThanaRepository.SaveChanges();

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

                //return resultJson;
            }
            return new JsonResult { };
        }
        //*/

        private bool CheckDuplicateEntry(ThanaViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                var sd = _prmCommonservice.PRMUnit.ThanaRepository.Get(q => q.DistrictId == model.DistrictId && q.ThanaName == model.ThanaName).Any();
                return _prmCommonservice.PRMUnit.ThanaRepository.Get(q => q.DistrictId == model.DistrictId && q.ThanaName == model.ThanaName).Any();
            }

            else
            {
                var bd = _prmCommonservice.PRMUnit.ThanaRepository.Get(q => q.DistrictId == model.DistrictId && q.ThanaName == model.ThanaName && strMode != q.Id).Any();
                return _prmCommonservice.PRMUnit.ThanaRepository.Get(q => q.DistrictId == model.DistrictId && q.ThanaName == model.ThanaName && strMode != q.Id).Any();
            }
        }

        private IList<PRM_Country> GetCountry()
        {
            IList<PRM_Country> itemList = _prmCommonservice.PRMUnit.CountryRepository.GetAll().OrderBy(x => x.Name).ToList();
            return itemList;
        }

        private IList<PRM_CountryDivision> GetCountryDivision(int countryId)
        {
            IList<PRM_CountryDivision> itemList = new List<PRM_CountryDivision>();
            if (countryId > 0)
            {
                itemList = _prmCommonservice.PRMUnit.CountryDivisionRepository.Get(q => q.CountryId == countryId).OrderBy(x => x.DivisionName).ToList();
            }
            return itemList;
        }

        private IList<PRM_District> GetDistrict(int DivisionId)
        {
            IList<PRM_District> itemList = new List<PRM_District>();
            if (DivisionId > 0)
            {
                itemList = _prmCommonservice.PRMUnit.DistrictRepository.Get(q => q.DivisionId == DivisionId).OrderBy(x => x.DistrictName).ToList();
            }

            return itemList;
        }

        public ActionResult LoadDivision(int countryId)
        {
            return Json(
                GetCountryDivision(countryId).Select(x => new { Id = x.Id, DivisionName = x.DivisionName }),
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult LoadDistrict(int divisionId)
        {
            return Json(
                GetDistrict(divisionId).Select(x => new { Id = x.Id, DistrictName = x.DistrictName }),
                JsonRequestBehavior.AllowGet
            );
        }
        #endregion
    }
}