using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ClearanceChecklistController : BaseController
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public ClearanceChecklistController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonservice = prmCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: /PRM/ClearanceChecklist/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ClearanceChecklistViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ClearanceChecklistViewModel> list = (from tr in _prmCommonservice.PRMUnit.ClearanceChecklistRepository.GetAll()
                                                      join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on tr.EmployeeId equals emp.Id
                                                      join clearForm in _prmCommonservice.PRMUnit.ClearanceFormRepository.GetAll() on tr.ClearanceFormId equals clearForm.Id
                                                      where (viewModel.EmpId == null || viewModel.EmpId == "" || viewModel.EmpId == emp.EmpID)
                                                    && (viewModel.EmployeeName == null || viewModel.EmployeeName == "" || emp.FullName.Contains(viewModel.EmployeeName))
                                                    &&(tr.ZoneInfoId == LoggedUserZoneInfoId)
                                                      select new ClearanceChecklistViewModel()
                                                      {
                                                          Id = tr.Id,
                                                          EmployeeId = tr.EmployeeId,
                                                          EmpId = emp.EmpID,
                                                          EmployeeName = emp.FullName,
                                                          ClearanceFormName = clearForm.Name
                                                      }).OrderBy(x => x.EmployeeId).ToList();


            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "EmpId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpId).ToList();
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

            if (request.SortingName == "ClearanceFormName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ClearanceFormName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ClearanceFormName).ToList();
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
                    d.EmployeeId,
                    d.EmpId,
                    d.EmployeeName,    
                    d.ClearanceFormName,             
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };





        }


        public ActionResult Create()
        {
            ClearanceChecklistViewModel model = new ClearanceChecklistViewModel();
            model.ClearanceChecklistDetails = new List<ClearanceChecklistDetailsViewModel>()
                {
                    new ClearanceChecklistDetailsViewModel()
                    {
                        
                    }
                };
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(ClearanceChecklistViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }

                    model = GetInsertUserAuditInfo(model, true);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, true);

                    if (errorList.Length == 0)
                    {
                        _prmCommonservice.PRMUnit.ClearanceChecklistRepository.Add(entity);
                        _prmCommonservice.PRMUnit.ClearanceChecklistRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                      //  return RedirectToAction("Index");
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
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
                    model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                }
            }
           
            //populateDropdown(model);
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var ClearanceChecklistEntity = _prmCommonservice.PRMUnit.ClearanceChecklistRepository.GetByID(id);
            var parentModel = ClearanceChecklistEntity.ToModel();
            parentModel.IdT = parentModel.Id;

            parentModel.EmpId = ClearanceChecklistEntity.PRM_EmploymentInfo.EmpID;
            parentModel.EmployeeName = ClearanceChecklistEntity.PRM_EmploymentInfo.FullName;
            //Checklist 
            IList<ClearanceChecklistDetailsViewModel> listITem = new List<ClearanceChecklistDetailsViewModel>();
            foreach (var item in ClearanceChecklistEntity.PRM_ClearanceChecklistDetail)
            {
                listITem.Add(item.ToModel());
            }
            parentModel.ClearanceChecklistDetails = listITem;
            //
            populateDropdown(parentModel);
            return View("Edit", parentModel);
        }


        [HttpPost]
        public ActionResult Edit(ClearanceChecklistViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {

                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);

                    }
                    model = GetInsertUserAuditInfo(model, false);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = CreateEntity(model, false);

                    if (errorList.Length == 0)
                    {
                        _prmCommonservice.PRMUnit.ClearanceChecklistRepository.Update(entity);
                        _prmCommonservice.PRMUnit.ClearanceChecklistRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
                       // return RedirectToAction("Index");
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
                
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

            populateDropdown(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonservice.PRMUnit.ClearanceChecklistRepository.Delete(id);
                _prmCommonservice.PRMUnit.ClearanceChecklistRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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


        [HttpPost, ActionName("DeleteClearanceChecklistDetail")]
        public JsonResult DeleteClearanceChecklistDetailConfirmed(int id)
        {

            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _prmCommonservice.PRMUnit.ClearanceChecklistDetailRepository.Delete(id);
                _prmCommonservice.PRMUnit.ClearanceChecklistDetailRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }


            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });

        }


        #endregion

        #region Private Method

        private bool CheckDuplicateEntry(ClearanceChecklistViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _prmCommonservice.PRMUnit.ClearanceChecklistRepository.Get(q => q.ClearanceFormId == model.ClearanceFormId && q.ZoneInfoId==model.ZoneInfoId).Any();
            }

            else
            {
                return _prmCommonservice.PRMUnit.ClearanceChecklistRepository.Get(q => q.ClearanceFormId == model.ClearanceFormId && q.ZoneInfoId == model.ZoneInfoId && strMode != q.Id).Any();
            }
        }

        private void populateDropdown(ClearanceChecklistViewModel model)
        {
            dynamic ddlList;

            #region Clearance Form ddl

            ddlList = _prmCommonservice.PRMUnit.ClearanceFormRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.ClearanceFormList = Common.PopulateDllList(ddlList);
            #endregion

        }

        private PRM_ClearanceChecklist CreateEntity(ClearanceChecklistViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            entity.Id = model.Id;
            foreach (var c in model.ClearanceChecklistDetails)
            {
                var prm_ClearanceChecklistDetail = new PRM_ClearanceChecklistDetail();

                //Check duplicate Chekclist Name
                var list = (from chkListDetail in _prmCommonservice.PRMUnit.ClearanceChecklistDetailRepository.GetAll()
                            join chkList in _prmCommonservice.PRMUnit.ClearanceChecklistRepository.GetAll()
                            on chkListDetail.ClearanceChecklistId equals chkList.Id
                            where (chkListDetail.Name == c.Name)
                            select chkListDetail).ToList();
                var count = list.Count();
                //End Check duplicate Chekclist Name

                if (count == 0)
                {
                    prm_ClearanceChecklistDetail.Id = c.Id;
                    prm_ClearanceChecklistDetail.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                    prm_ClearanceChecklistDetail.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                    prm_ClearanceChecklistDetail.EUser = c.EUser;
                    prm_ClearanceChecklistDetail.EDate = c.EDate;
                    prm_ClearanceChecklistDetail.Name = c.Name;

                    if (pAddEdit)
                    {
                        prm_ClearanceChecklistDetail.IUser = User.Identity.Name;
                        prm_ClearanceChecklistDetail.IDate = DateTime.Now;

                        entity.PRM_ClearanceChecklistDetail.Add(prm_ClearanceChecklistDetail);
                    }
                    else
                    {
                        prm_ClearanceChecklistDetail.ClearanceChecklistId = model.Id;
                        prm_ClearanceChecklistDetail.EUser = User.Identity.Name;
                        prm_ClearanceChecklistDetail.EDate = DateTime.Now;

                        if (c.Id == 0)
                        {
                            _prmCommonservice.PRMUnit.ClearanceChecklistDetailRepository.Add(prm_ClearanceChecklistDetail);
                        }
                        else
                        {
                            _prmCommonservice.PRMUnit.ClearanceChecklistDetailRepository.Update(prm_ClearanceChecklistDetail);

                        }
                        _prmCommonservice.PRMUnit.ClearanceChecklistDetailRepository.SaveChanges();
                    }
                }
                else
                {
                    model.ErrMsg = "Duplicate Entry";
                }

            }

            return entity;
        }


        private ClearanceChecklistViewModel GetInsertUserAuditInfo(ClearanceChecklistViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;

                foreach (var child in model.ClearanceChecklistDetails)
                {
                    child.IUser = User.Identity.Name;
                    child.IDate = DateTime.Now;

                }
            }
            else
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
            }

            return model;
        }
        #endregion

        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var obj = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeName = obj.FullName
            });

        }
    }
}