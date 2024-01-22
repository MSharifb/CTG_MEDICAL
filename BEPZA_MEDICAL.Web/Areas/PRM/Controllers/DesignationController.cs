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
using System.Collections;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class DesignationController : BaseController
    {
        #region Fields

        private readonly JobDesignationService _jobDesignationService;
        private readonly PRMCommonSevice _PRMCommonSevice;

        #endregion

        #region Constructor

        public DesignationController(JobDesignationService service, PRMCommonSevice prm_svr)
        {
            this._jobDesignationService = service;
            _PRMCommonSevice = prm_svr;
        }

        #endregion

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetDesignations(JqGridRequest request, DesignationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from JG in _jobDesignationService.PRMUnit.JobGradeRepository.GetAll()
                        join Deg in _jobDesignationService.PRMUnit.DesignationRepository.GetAll() on JG.Id equals Deg.GradeId                      
                        select new DesignationViewModel()
                        {
                            Id = Deg.Id,
                            GradeId=JG.Id,
                            GradeName = JG.GradeName,
                            Name = Deg.Name,
                            SortingOrder = Convert.ToInt32(Deg.SortingOrder),
                            ShortName = Deg.ShortName
                        }).OrderBy(x => x.SortingOrder).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.GradeName))
                {
                    list = list.Where(x => x.GradeName.Trim().ToLower() == viewModel.GradeName.Trim().ToLower()).OrderBy(q=>q.SortingOrder).ToList();
                }

                if (!string.IsNullOrEmpty(viewModel.Name))
                {
                    list = list.Where(x => x.Name.Trim().ToLower().Contains(viewModel.Name.Trim().ToLower())).OrderBy(q=>q.SortingOrder).ToList();
                }

                if (!string.IsNullOrEmpty(viewModel.ShortName))
                {
                    list = list.Where(x => x.ShortName.Trim().ToLower().Contains(viewModel.ShortName.Trim().ToLower())).OrderBy(q => q.SortingOrder).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "GradeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.GradeId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.GradeId).ToList();
                }
            }

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Name).ToList();
                }
            }

            if (request.SortingName == "ShortName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ShortName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ShortName).ToList();
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
                    d.GradeName,
                    d.Name,
                    d.SortingOrder,
                    d.ShortName
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            DesignationViewModel model = new DesignationViewModel();
            model.ActionType = "Create";

            PopulateList(model);

            //model.JobGradeList = _jobDesignationService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.GradeName).ToList().Select(y => new SelectListItem()
            //{
            //    Text = y.GradeName,
            //    Value = y.Id.ToString()
            //}).ToList();

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(DesignationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!CheckDuplicate(model, "add"))
                {
                    try
                    {                       
                        PRM_Designation entity = model.ToEntity();
                        entity.IUser = User.Identity.Name;
                        entity.IDate = DateTime.Now;
                        _jobDesignationService.PRMUnit.DesignationRepository.Add(entity);
                        _jobDesignationService.PRMUnit.DesignationRepository.SaveChanges();

                        // model.IsError = 0;                  
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

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
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                        }
                    }
                }
                else
                {
                    model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    model.IsError = 1;
                }
            }

    //        ViewBag.JobGradeList = _jobDesignationService.PRMUnit.JobGradeRepository.GetAll();
            model.ActionType = "Create";
            PopulateList(model);
            return View("CreateOrEdit", model);
        }

        public ActionResult Edit(int id)
        {
            PRM_Designation entity = _jobDesignationService.PRMUnit.DesignationRepository.GetByID(id);
            DesignationViewModel model = entity.ToModel();
            model.ActionType = "Edit";
            PopulateList(model);

            model.PayScaleDetails = GetPayScaleDetailsByGradeId(entity.GradeId);

            //model.JobGradeList = _jobDesignationService.PRMUnit.JobGradeRepository.GetAll().ToList().Select(y => new SelectListItem()
            //{
            //    Text = y.GradeName,
            //    Value = y.Id.ToString()
            //}).ToList();

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(DesignationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!CheckDuplicate(model, "edit"))
                {                    
                    PRM_Designation entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = DateTime.Now;
                    try
                    {
                        _jobDesignationService.PRMUnit.DesignationRepository.Update(entity);

                        #region
                        //If grade change than employee's grade will be chnage 
                        var empList = _PRMCommonSevice.PRMUnit.EmploymentInfoRepository.GetAll().Where(q => q.DesignationId == model.Id).ToList();

                        if (empList != null)
                        {
                            foreach (var item in empList)
                            {
                                item.JobGradeId = model.GradeId;
                                _PRMCommonSevice.PRMUnit.EmploymentInfoRepository.Update(item);
                            }
                            // _PRMCommonSevice.PRMUnit.EmploymentInfoRepository.SaveChanges();
                        }
                        #endregion

                        _jobDesignationService.PRMUnit.DesignationRepository.SaveChanges();
                        //   model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

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
                            model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                        }
                    }
                }
                else
                {
                    model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    model.IsError = 1;
                }
            }
       //     ViewBag.JobGradeList = _jobDesignationService.PRMUnit.JobGradeRepository.GetAll();
            model.ActionType = "Edit";
            PopulateList(model);
            return View("CreateOrEdit", model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            bool result = false;
            try
            {
                PRM_Designation prm_designation = _jobDesignationService.PRMUnit.DesignationRepository.GetByID(id);
                _jobDesignationService.PRMUnit.DesignationRepository.Delete(prm_designation);
                _jobDesignationService.PRMUnit.DesignationRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
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
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        private bool CheckDuplicate(DesignationViewModel model, string strMode)
        {
            dynamic objDesignation = null;
            try
            {
                if (strMode == "add")
                {
                    objDesignation = _jobDesignationService.PRMUnit.DesignationRepository.Get(x => x.Name == model.Name).FirstOrDefault();

                }
                else
                {
                    objDesignation = _jobDesignationService.PRMUnit.DesignationRepository.Get(x => x.Name == model.Name && x.Id != model.Id).FirstOrDefault();

                }

                if (objDesignation != null)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        #region Others

        private void PopulateList(DesignationViewModel model)
        {
            DateTime cDate = DateTime.Now;

            var prm_SalaryScaleEntity = _PRMCommonSevice.PRMUnit.SalaryScaleRepository.GetAll().Where(t => t.DateOfEffective <= cDate).OrderByDescending(t => t.DateOfEffective).First();

            model.JobGradeList = _jobDesignationService.PRMUnit.JobGradeRepository.GetAll().Where(t => t.SalaryScaleId == prm_SalaryScaleEntity.Id).OrderBy(x => x.GradeName, new NumeralAlphaCompare()).ToList().Select(y => new SelectListItem()
            {
                Text = y.GradeName,
                Value = y.Id.ToString()
            }).ToList();

            // Employee Class
            model.EmployeeClassList = Common.PopulateDllList(_PRMCommonSevice.PRMUnit.EmployeeClassRepository.GetAll().OrderBy(x => x.Name));

        }

        /// <summary>
        /// Alphanumeric Sorting ascending
        /// Added  by Suman
        /// </summary>
        public class NumeralAlphaCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int nIndexX = x.Replace(":", " ").IndexOf(" ");
                int nIndexY = y.Replace(":", " ").IndexOf(" ");
                bool bSpaceX = nIndexX > -1;
                bool bSpaceY = nIndexY > -1;

                long nX;
                long nY;
                if (bSpaceX && bSpaceY)
                {
                    if (long.TryParse(x.Substring(0, nIndexX).Replace(".", ""), out nX)
                        && long.TryParse(y.Substring(0, nIndexY).Replace(".", ""), out nY))
                    {
                        if (nX < nY)
                        {
                            return -1;
                        }
                        else if (nX > nY)
                        {
                            return 1;
                        }
                    }
                }
                else if (bSpaceX)
                {
                    if (long.TryParse(x.Substring(0, nIndexX).Replace(".", ""), out nX)
                        && long.TryParse(y, out nY))
                    {
                        if (nX < nY)
                        {
                            return -1;
                        }
                        else if (nX > nY)
                        {
                            return 1;
                        }
                    }
                }
                else if (bSpaceY)
                {
                    if (long.TryParse(x, out nX)
                        && long.TryParse(y.Substring(0, nIndexY).Replace(".", ""), out nY))
                    {
                        if (nX < nY)
                        {
                            return -1;
                        }
                        else if (nX > nY)
                        {
                            return 1;
                        }
                    }
                }
                else
                {
                    if (long.TryParse(x, out nX)
                        && long.TryParse(y, out nY))
                    {
                        if (nX < nY)
                        {
                            return -1;
                        }
                        else if (nX > nY)
                        {
                            return 1;
                        }
                    }
                }
                return x.CompareTo(y);
            }
        }

        public string GetPayScaleDetailsByGradeId(int id)
        {
            var item = _PRMCommonSevice.PRMUnit.JobGradeRepository.Get(t => t.Id == id).FirstOrDefault();

            if (item != null)
            {
                return item.PayScale;
            }

            return String.Empty;
        }

        #endregion
    }
}
