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
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.DAL.PGM;
using System.IO;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class EmployeeActivationController : Controller
    {

        #region Fields
        private readonly PRMCommonSevice _prmCommonSevice;
        #endregion

        #region Constructor

        public EmployeeActivationController(PRMCommonSevice prmCommonSevice)
        {
            this._prmCommonSevice = prmCommonSevice;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, EmployeeActivationSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _prmCommonSevice.PRMUnit.EmpActivationRepository.GetCount(filterExpression);

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

            var list = _prmCommonSevice.PRMUnit.EmpActivationRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString()).ToList();
            

            list = list.Skip(request.PageIndex * request.RecordsCount).Take((request.PagesCount.HasValue ? request.PagesCount.Value : 1) * request.RecordsCount).ToList ();

            #region sorting

            if (request.SortingName == "EmployeeID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeId).ToList();
                }
            }

            if (request.SortingName == "EmpId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_EmploymentInfo.EmpID).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_EmploymentInfo.EmpID).ToList();
                }
            }

            if (request.SortingName == "EmployeeInitial")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_EmploymentInfo.EmployeeInitial).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_EmploymentInfo.EmployeeInitial).ToList();
                }
            }
            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_EmploymentInfo.EmployeeInitial).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_EmploymentInfo.EmployeeInitial).ToList();
                }
            }

            if (request.SortingName == "DivisionId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PRM_EmploymentInfo.PRM_Division.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PRM_EmploymentInfo.PRM_Division.Name).ToList();
                }
            }
            
           
            #endregion

            foreach (PRM_EmployeeActivationHistory d in list)
            {                
                response.Records.Add(new JqGridRecord(Convert.ToString(d.EmployeeId), new List<object>()
                {
                    d.Id,
                    d.EmployeeId,
                    d.PRM_EmploymentInfo.EmpID,
                    d.PRM_EmploymentInfo.FullName,                   
                    d.PRM_EmploymentInfo.EmployeeInitial,
                    d.PRM_EmploymentInfo.PRM_Designation.Name,                                 
                    d.PRM_EmploymentInfo.PRM_Division.Name,
                    Convert.ToDateTime(d.ActivationDate).Date.ToString("dd-MMM-yyyy")
                    //"",
                    //"",
                    //"Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult DivisionforView()
        {

            var itemList = Common.PopulateDllList(_prmCommonSevice.PRMUnit.DivisionRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(EmployeeActivationViewModel model)
        {
            var obj = _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            
            return Json(new
            {
                EmpId = obj.EmpID,
                Division = obj.PRM_Division.Name,
                EmployeeInitial = obj.EmployeeInitial,
                Designation = obj.PRM_Designation.Name,
                EmployeeName = obj.FullName,
                JoiningDate = obj.DateofJoining.ToString("dd-MM-yyyy"),
                IsContractual = obj.IsContractual,
                PreviousEmploymentStatusId = obj.EmploymentStatusId
            });
        }

        public ActionResult Create()
        {
            EmployeeActivationViewModel model = new EmployeeActivationViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EmployeeActivationViewModel model)
        {
            string errorList = string.Empty;

            if (ModelState.IsValid)
            {
                var obj = model.ToEntity();

                if (string.IsNullOrEmpty(errorList))
                {
                    try
                    {
                        _prmCommonSevice.PRMUnit.EmpActivationRepository.Add(obj);
                        _prmCommonSevice.PRMUnit.EmpActivationRepository.SaveChanges();

                        // update employee info
                        _prmCommonSevice.PRMUnit.EmploymentInfoRepository.Update(UpdateEmployeeInfo(model));
                        _prmCommonSevice.PRMUnit.EmploymentInfoRepository.SaveChanges();
                            
                        model.IsError = 0;
                        return RedirectToAction("Index");
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
                else
                {
                    model.ErrMsg = errorList;
                }
            }

            setEmployeeInfo(model, "I");

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            PRM_EmployeeActivationHistory prm_EmployeeActivationHistory = _prmCommonSevice.PRMUnit.EmpActivationRepository.GetByID(id, "Id");
            EmployeeActivationViewModel model = prm_EmployeeActivationHistory.ToModel();
            setEmployeeInfo(model, "E");

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeActivationViewModel model)
        {
            string errorList = string.Empty;

            if (ModelState.IsValid)
            {
                PRM_EmployeeActivationHistory obj = model.ToEntity();

                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    try
                    {
                        _prmCommonSevice.PRMUnit.EmpActivationRepository.Update(obj, "Id");
                        _prmCommonSevice.PRMUnit.EmpActivationRepository.SaveChanges();

                        // update employee info
                        _prmCommonSevice.PRMUnit.EmploymentInfoRepository.Update(UpdateEmployeeInfo(model));
                        _prmCommonSevice.PRMUnit.EmploymentInfoRepository.SaveChanges();

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
                else
                {
                    model.ErrMsg = errorList;
                }
            }

            setEmployeeInfo(model, "E");

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;

            string errMsg = string.Empty;
            errMsg = GetBusinessLogicValidation(id);

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    var empSepObj = _prmCommonSevice.PRMUnit.EmpActivationRepository.GetByID(id, "Id");
                    var obj = _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetByID(empSepObj.EmployeeId);

                    _prmCommonSevice.PRMUnit.EmpActivationRepository.Delete(id, "Id", new List<Type>());
                    _prmCommonSevice.PRMUnit.EmpActivationRepository.SaveChanges();

                    // update employee info
                    obj.DateofInactive = null;
                    obj.EmploymentStatusId = 1; //active
                    obj.EmploymentStatusId = 0;

                    _prmCommonSevice.PRMUnit.EmploymentInfoRepository.Update(obj);
                    _prmCommonSevice.PRMUnit.EmploymentInfoRepository.SaveChanges();

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
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }



        #endregion

        #region  private method

        private string GetBusinessLogicValidation(int id)
        {
            string message = string.Empty;
            //var CheckOutGratuity = (from g in _prmCommonSevice.PRMUnit.GratuitySettlement.GetAll() where g.EmployeeId == id select g).ToList();
            //if (CheckOutGratuity.Count > 0)
            //{
            //    message = "Can't be deleted, because gratuity settlement has been completed.";
            //}

            //var CheckOutFinalSettlement = (from f in _pgmCommonservice.PGMUnit.FinalSettlement.GetAll() where f.EmployeeId == id select f).ToList();
            //if (CheckOutFinalSettlement.Count > 0)
            //{
            //    message = "Can't be deleted, because final settlement has been completed.";
            //}
            return message;
        }

        private void setEmployeeInfo(EmployeeActivationViewModel model, string mode)
        {
            if (model.EmployeeId != 0)
            {
                var obj = _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                model.EmpId = obj.EmpID;
               
                model.Division = obj.PRM_Division.Name;
                model.EmployeeInitial = obj.EmployeeInitial;
                model.Designation = obj.PRM_Designation.Name;
                model.EmployeeName = obj.FullName;
                model.JoiningDate = obj.DateofJoining;
            }
        }

        private PRM_EmploymentInfo UpdateEmployeeInfo(EmployeeActivationViewModel model)
        {
            var obj = _prmCommonSevice.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);

            var empStatus = _prmCommonSevice.PRMUnit.EmploymentStatusRepository.Get(q => q.Name.ToLower() == "active").FirstOrDefault();
            if (empStatus != null) obj.EmploymentStatusId = empStatus.Id;

            return obj;
        }

        public JsonResult DiffCalulation(string AppDate, string EffecDate)
        {
            var diff = (Convert.ToDateTime(EffecDate) - Convert.ToDateTime(AppDate)).Days;

            return Json(diff, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}