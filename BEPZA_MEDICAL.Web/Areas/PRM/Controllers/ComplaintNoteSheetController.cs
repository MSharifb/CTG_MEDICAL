using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ComplaintNoteSheetController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public ComplaintNoteSheetController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions


        //
        // GET: /PRM/ComplaintNoteSheet/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ComplaintNoteSheetViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ComplaintNoteSheetViewModel> list = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                                                      join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                                                      where (model.Id == 0 || model.Id == comNoteSheet.Id)
                                                      && (model.RefNo == null || model.RefNo == comNoteSheet.RefNo)
                                                      && (model.ComplaintDate == null || model.ComplaintDate == comNoteSheet.ComplaintDate)
                                                     && (model.ComplaintEmployeeName == null || model.ComplaintEmployeeName == "" || empComplaint.FullName.Contains(model.ComplaintEmployeeName))
                                                      && (comNoteSheet.ZoneInfoId == LoggedUserZoneInfoId)
                                                      select new ComplaintNoteSheetViewModel()
                                                           {
                                                               Id = comNoteSheet.Id,
                                                               DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                                                               RefNo = comNoteSheet.RefNo,
                                                               ComplaintDate = comNoteSheet.ComplaintDate,
                                                               ComplaintEmployeeName = empComplaint.FullName,
                                                               ComplaintDesignationName = empComplaint.PRM_Designation.Name
                                                           }).OrderBy(x => x.Id).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "DeptProceedingNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DeptProceedingNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DeptProceedingNo).ToList();
                }
            }


            if (request.SortingName == "RefNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RefNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RefNo).ToList();
                }
            }
            if (request.SortingName == "ComplaintDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplaintDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplaintDate).ToList();
                }
            }
            if (request.SortingName == "ComplaintEmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplaintEmployeeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplaintEmployeeName).ToList();
                }
            }

            if (request.SortingName == "ComplaintDesignationName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplaintDesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplaintDesignationName).ToList();
                }
            }
            if (request.SortingName == "ComplainantEmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplainantEmployeeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplainantEmployeeName).ToList();
                }
            }

            if (request.SortingName == "ComplainantDesignationName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplainantDesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplainantDesignationName).ToList();
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
                    d.DeptProceedingNo,
                    d.RefNo,
                    d.ComplaintDate == null ? string.Empty :Convert.ToDateTime(d.ComplaintDate).ToString(DateAndTime.GlobalDateFormat),
                    d.ComplaintEmployeeName,
                    d.ComplaintDesignationName,  
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            ComplaintNoteSheetViewModel model = new ComplaintNoteSheetViewModel();
            model.DeptProceedingNo = Common.GetNewDeptPreceedingNo(_prmCommonService);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(ComplaintNoteSheetViewModel model)
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
                        return View(model);
                    }
                    model = GetInsertUserAuditInfo(model, true);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.Add(entity);
                        _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        // return RedirectToAction("Index");
                    }


                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    // model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            return View(model);


        }


        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetByID(Id);
            var model = entity.ToModel();

            //Accused Person
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ComplaintEmployeeId);
            model.ComplaintEmpId = obj.EmpID;
            model.ComplaintEmployeeName = obj.FullName;
            model.ComplaintDesignationName = obj.PRM_Designation.Name;
            model.ComplaintDepartmentName = obj.PRM_Division == null ? string.Empty : obj.PRM_Division.Name;

            //Complainant Person
            if (model.ComplainantEmployeeId != null)
            {
                var objComplainantEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ComplainantEmployeeId);
                model.ComplainantEmpId = objComplainantEmp.EmpID;
                model.ComplainantEmployeeName = objComplainantEmp.FullName;
                model.ComplainantDesignationName = objComplainantEmp.PRM_Designation.Name;
                model.ComplainantDepartmentName = objComplainantEmp.PRM_Division == null ? string.Empty : objComplainantEmp.PRM_Division.Name;
            }
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(ComplaintNoteSheetViewModel model)
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
                        return View(model);
                    }
                    model = GetInsertUserAuditInfo(model, true);
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.Update(entity);
                        _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        //return RedirectToAction("Index");
                    }


                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    //model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.Delete(id);
                _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }


            return Json(new
            {
                Success = result,
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Method
        private ComplaintNoteSheetViewModel GetInsertUserAuditInfo(ComplaintNoteSheetViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
            }
            else
            {

                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
            }

            return model;
        }

        private bool CheckDuplicateEntry(ComplaintNoteSheetViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.Get(q => q.RefNo == model.RefNo).Any();
            }

            else
            {
                return _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.Get(q => q.RefNo == model.RefNo && strMode != q.Id).Any();
            }
        }
        #endregion


        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpId = obj.EmpID,
                EmployeeName = obj.FullName,
                DesignationName = obj.PRM_Designation.Name,
                DepartmentName = obj.PRM_Division == null ? string.Empty : obj.PRM_Division.Name
            }, JsonRequestBehavior.AllowGet);

        }


        [NoCache]
        public ActionResult DeptProceedingListView()
        {
            var list = Common.PopulateComplaintNoteSheet(_prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.DeptProceedingNo).ToList());
            return PartialView("Select", list);
        }


        //details View
        [HttpPost]
        public ActionResult ViewComplaintDetails(int id)
        {
            var entity = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetByID(id);
            var model = entity.ToModel();

            //Accused Person
            var obj = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ComplaintEmployeeId);
            model.ComplaintEmpId = obj.EmpID;
            model.ComplaintEmployeeName = obj.FullName;
            model.ComplaintDesignationName = obj.PRM_Designation.Name;
            model.ComplaintDepartmentName = obj.PRM_Division == null ? string.Empty : obj.PRM_Division.Name;

            //Complainant Person
            var objComplainantEmp = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetByID(model.ComplainantEmployeeId);
            model.ComplainantEmpId = objComplainantEmp.EmpID;
            model.ComplainantEmployeeName = objComplainantEmp.FullName;
            model.ComplainantDesignationName = objComplainantEmp.PRM_Designation.Name;
            model.ComplainantDepartmentName = objComplainantEmp.PRM_Division == null ? string.Empty : objComplainantEmp.PRM_Division.Name;

            return PartialView("_ComplaintDetailsView", model);
        }

    }
}