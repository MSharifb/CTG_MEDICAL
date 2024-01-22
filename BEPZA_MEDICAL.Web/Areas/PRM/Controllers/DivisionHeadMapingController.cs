using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.PRM;
using System.Data.SqlClient;
using BEPZA_MEDICAL.Web.Controllers;


namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class DivisionHeadMapingController : BaseController
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Constructor

        public DivisionHeadMapingController(PRMCommonSevice service)
        {
            this._prmCommonService = service;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, DivisionHeadMapingViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<DivisionHeadMapingViewModel> list = (from divHeapMap in _prmCommonService.PRMUnit.DivisionHeadMapingRepository.GetAll()
                                                      where (divHeapMap.ZoneInfoId == LoggedUserZoneInfoId)
                                                      select new DivisionHeadMapingViewModel()
                                                         {
                                                             Id = divHeapMap.Id,
                                                             OrganogramLevelName = divHeapMap.PRM_OrganogramLevel.LevelName,
                                                             Designation = divHeapMap.PRM_Designation.Name,
                                                             EmployeeName = divHeapMap.EmployeeId == null ? string.Empty : divHeapMap.PRM_EmploymentInfo.FullName
                                                         }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting
            if (request.SortingName == "OrganogramLevelName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OrganogramLevelName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OrganogramLevelName).ToList();
                }
            }

            if (request.SortingName == "Designation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Designation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Designation).ToList();
                }
            }

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeName).ToList();
                }
            }

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
                            d.OrganogramLevelName,
                            d.Designation,                            
                            d.EmployeeName, 
                            "Delete"
                        }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(DivisionHeadMapingViewModel model)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeInitial = obj.EmployeeInitial,
                Designation = obj.PRM_Designation.Name,
                EmployeeName = obj.FullName,
                JoiningDate = obj.DateofJoining.ToString("dd-MM-yyyy"),
            });

        }

        public ActionResult Create()
        {
            DivisionHeadMapingViewModel model = new DivisionHeadMapingViewModel();
            model.ActionType = "Create";
            populateDropdownList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DivisionHeadMapingViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!CheckDuplicate(model, "add"))
                {
                    try
                    {
                        model.ZoneInfoId = LoggedUserZoneInfoId;
                        //model.IUser = User.Identity.Name;
                        //model.IDate = DateTime.Now;
                        var entity = model.ToEntity();
                        _prmCommonService.PRMUnit.DivisionHeadMapingRepository.Add(entity);
                        _prmCommonService.PRMUnit.DivisionHeadMapingRepository.SaveChanges();

                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        // return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {

                        model.IsError = 1;
                        try
                        {
                            if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                            {
                                SqlException sqlException = ex.InnerException as SqlException;
                                model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            }
                        }
                        catch (Exception)
                        {
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                        }
                    }
                }
                else
                {                    
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = "Duplicate entry.";
                }
            }

            populateDropdownList(model);
            setEmployeeInfo(model, "E");
            model.ActionType = "Create";

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _prmCommonService.PRMUnit.DivisionHeadMapingRepository.GetByID(id);
            var model = entity.ToModel();
            model.OrganogramLevelName = entity.PRM_OrganogramLevel.LevelName;
            model.strMode = "Edit";
            populateDropdownList(model);
            setEmployeeInfo(model, "E");
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(DivisionHeadMapingViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!CheckDuplicate(model, "edit"))
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    //model.IUser = User.Identity.Name;
                    //model.IDate = DateTime.Now;

                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _prmCommonService.PRMUnit.DivisionHeadMapingRepository.Update(entity);
                        _prmCommonService.PRMUnit.DivisionHeadMapingRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        //return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        model.IsError = 1;
                        try
                        {
                            if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                            {
                                SqlException sqlException = ex.InnerException as SqlException;
                                model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                            }
                        }
                        catch (Exception)
                        {
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);

                        }
                    }
                }
                else
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = "Duplicate entry.";
                }
            }
            populateDropdownList(model);
            setEmployeeInfo(model, "E");
            model.ActionType = "Edit";

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            bool result = false;
            try
            {
                PRM_DivisionHeadMaping prm_designation = _prmCommonService.PRMUnit.DivisionHeadMapingRepository.GetByID(id);
                _prmCommonService.PRMUnit.DivisionHeadMapingRepository.Delete(prm_designation);
                _prmCommonService.PRMUnit.DesignationRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                    {
                        errMsg = Resources.ErrorMessages.DeleteFailed;
                    }
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return Json(new
            {
                Success = result,
                Message = result ? Resources.ErrorMessages.DeleteSuccessful : errMsg
            });
        }


        public JsonResult GetDesignationListByOrganogramId(int organogramId)
        {
            var desigList = (from orgLvl in _prmCommonService.PRMUnit.OrganogramLevelRepository.GetAll().Where(q=>q.Id==organogramId)
                             join orgManPwrInfo in _prmCommonService.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll() on orgLvl.Id equals orgManPwrInfo.OrganogramLevelId
                             join desig in _prmCommonService.PRMUnit.DesignationRepository.GetAll() on orgManPwrInfo.DesignationId equals desig.Id
                             select desig).OrderBy(q => q.SortingOrder).ToList();
            var list = Common.PopulateDllList(desigList);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region Private
        private void setEmployeeInfo(DivisionHeadMapingViewModel model, string mode)
        {
            if (model.EmployeeId != null && model.EmployeeId != 0)
            {
                var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
                model.EmpId = obj.EmpID;
                model.Designation = obj.PRM_Designation.Name;
                model.EmployeeName = obj.FullName;
            }
        }
        private void populateDropdownList(DivisionHeadMapingViewModel model)
        {
            model.DesignationList = Common.PopulateDllList(_prmCommonService.PRMUnit.DesignationRepository.Get(d => d.Id == model.DesignationId).OrderBy(x => x.SortingOrder).ToList());

        }

        private bool CheckDuplicate(DivisionHeadMapingViewModel model, string strMode)
        {
            dynamic objDesignation = null;
            try
            {
                if (strMode == "add")
                {
                    objDesignation = _prmCommonService.PRMUnit.DivisionHeadMapingRepository.Get(x => x.OrganogramLevelId == model.OrganogramLevelId && x.DesignationId == model.DesignationId && x.ZoneInfoId == model.ZoneInfoId).FirstOrDefault();

                }
                else
                {
                    objDesignation = _prmCommonService.PRMUnit.DivisionHeadMapingRepository.Get(x => (x.OrganogramLevelId == model.OrganogramLevelId && x.DesignationId == model.DesignationId && x.ZoneInfoId == model.ZoneInfoId) && x.Id != model.Id).FirstOrDefault();

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

        //[NoCache]
        //public JsonResult GetEmployeeInfo(DivisionHeadMapingViewModel model)
        //{
        //    var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId);
        //    return Json(new
        //    {
        //        EmpId = obj.EmpID,
        //        Designation = obj.PRM_Designation.Name,
        //        EmployeeName = obj.FullName
        //    });

        //}


    }
}
