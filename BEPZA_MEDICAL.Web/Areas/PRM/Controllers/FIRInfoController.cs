using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
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
    public class FIRInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public FIRInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions


        //
        // GET: /PRM/FIRInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, FIRInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<FIRInfoViewModel> list = (from fir in _prmCommonService.PRMUnit.FIRInfoRepository.GetAll()
                                           join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll() on fir.ComplaintNoteSheetId equals comNoteSheet.Id
                                           where (comNoteSheet.ZoneInfoId == LoggedUserZoneInfoId)
                                           && (model.ComplaintNoteSheetId == 0 || model.ComplaintNoteSheetId == comNoteSheet.Id)
                                           && (model.FIRDate == null || model.FIRDate == fir.FIRDate)
                                           select new FIRInfoViewModel()
                                            {
                                                Id = fir.Id,
                                                ComplaintNoteSheetId = fir.ComplaintNoteSheetId,
                                                ComplaintNoteSheetName = fir.PRM_ComplaintNoteSheet.DeptProceedingNo,
                                                FIRDate = fir.FIRDate,
                                            }).OrderBy(x => x.Id).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ComplaintNoteSheetName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ComplaintNoteSheetName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ComplaintNoteSheetName).ToList();
                }
            }


            if (request.SortingName == "FIRDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FIRDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FIRDate).ToList();
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
                    d.ComplaintNoteSheetId,
                    d.ComplaintNoteSheetName,
                    Convert.ToDateTime(d.FIRDate).ToString(DateAndTime.GlobalDateFormat),                                
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            FIRInfoViewModel model = new FIRInfoViewModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(FIRInfoViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = model.ToEntity();
                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.FIRInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.FIRInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        // return RedirectToAction("Index");
                    }


                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.errClass = "failed";
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
                    //  model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            populateDropdown(model);
            return View(model);
        }


        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.FIRInfoRepository.GetByID(Id);
            var model = entity.ToModel();

            model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;
            model.strMode = "Edit";

            model.SignatoryEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.SignatoryEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.SignatoryDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

            var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                       join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                       join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id
                       where (comNoteSheet.Id == entity.ComplaintNoteSheetId)
                       select new
                       {
                           //Id = comNoteSheet.Id,
                           //DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                           RefNo = comNoteSheet.RefNo,
                           ComplaintDate = comNoteSheet.ComplaintDate,
                           ComplaintDetails = comNoteSheet.ComplaintDetails,
                           ComplaintEmployeeName = empComplaint.FullName,
                           ComplaintDesignationName = empComplaint.PRM_Designation.Name,
                           ComplaintDepartmentName = empComplaint.PRM_Division.Name,
                           ComplainantEmployeeName = empComplainant.FullName,
                           ComplainantDesignationName = empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant.PRM_Division.Name,
                       }).FirstOrDefault();

            model.RefNo = obj.RefNo;
            model.ComplaintDate = Convert.ToDateTime(obj.ComplaintDate).ToString("dd-MM-yyyy");
            model.ComplaintDetails = obj.ComplaintDetails;
            model.ComplaintEmployeeName = obj.ComplaintEmployeeName;
            model.ComplaintDesignationName = obj.ComplaintDesignationName;
            model.ComplaintDepartmentName = obj.ComplaintDepartmentName;
            model.ComplainantEmployeeName = obj.ComplainantEmployeeName;
            model.ComplainantDesignationName = obj.ComplainantDesignationName;
            model.ComplainantDepartmentName = obj.ComplainantDepartmentName;

            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(FIRInfoViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = model.ToEntity();

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.FIRInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.FIRInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.errClass = "failed";
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
                    //  model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            populateDropdown(model);
            model.strMode = "Edit";
            //model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.FIRInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.FIRInfoRepository.SaveChanges();
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
        private void populateDropdown(FIRInfoViewModel model)
        {
            dynamic ddlList;
            //use as Departmental Proceedigs No.
            #region Complaint/Note Sheet
            ddlList = (from noteAndOrder in _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                       join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId) on noteAndOrder.ComplaintNoteSheetId equals comNoteSheet.Id
                       where (noteAndOrder.IsOrder == true && comNoteSheet.ZoneInfoId == LoggedUserZoneInfoId)
                       select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();
            model.ComplaintNoteSheetList = Common.PopulateComplaintNoteSheet(ddlList);

            #endregion
        }
        private FIRInfoViewModel GetInsertUserAuditInfo(FIRInfoViewModel model, bool pAddEdit)
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

        #endregion


        //[NoCache]
        //public JsonResult GetComplaintNoteInfo(int complaintNoteSheetId)
        //{
        //    bool result = false;
        //    var orderType = _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetAll().Where(q => q.ComplaintNoteSheetId == complaintNoteSheetId).OrderByDescending(o => o.OrderDate).FirstOrDefault().PRM_OrderTypeInfo.Name;
        //    if (orderType == PRMEnum.DepartmentalProceedingOrderType.FIR.ToString())
        //    {
        //        result = true;
        //    }
        //    var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
        //               join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
        //               join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id
        //               where (comNoteSheet.Id == complaintNoteSheetId)
        //               select new
        //               {
        //                   RefNo = comNoteSheet.RefNo,
        //                   ComplaintDate = comNoteSheet.ComplaintDate,
        //                   ComplaintDetails = comNoteSheet.ComplaintDetails,
        //                   ComplaintEmployeeName = empComplaint.FullName,
        //                   ComplaintDesignationName = empComplaint.PRM_Designation.Name,
        //                   ComplaintDepartmentName = empComplaint.PRM_Division.Name,
        //                   ComplainantEmployeeName = empComplainant.FullName,
        //                   ComplainantDesignationName = empComplainant.PRM_Designation.Name,
        //                   ComplainantDepartmentName = empComplainant.PRM_Division.Name,
        //               }).FirstOrDefault();

        //    return Json(new
        //    {
        //        Success = result,
        //        OrderType = orderType,
        //        RefNo = obj.RefNo,
        //        ComplaintDate = Convert.ToDateTime(obj.ComplaintDate).ToString(DateAndTime.GlobalDateFormat),
        //        ComplaintDetails = obj.ComplaintDetails,
        //        ComplaintEmployeeName = obj.ComplaintEmployeeName,
        //        ComplaintDesignationName = obj.ComplaintDesignationName,
        //        ComplaintDepartmentName = obj.ComplaintDepartmentName,
        //        ComplainantEmployeeName = obj.ComplainantEmployeeName,
        //        ComplainantDesignationName = obj.ComplainantDesignationName,
        //        ComplainantDepartmentName = obj.ComplainantDepartmentName
        //    }, JsonRequestBehavior.AllowGet);

        //}



        [NoCache]
        public ActionResult DeptProceedingListView()
        {
            var ddlList = (from fir in _prmCommonService.PRMUnit.FIRInfoRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                           join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId) on fir.ComplaintNoteSheetId equals comNoteSheet.Id
                           select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();

            var list = Common.PopulateComplaintNoteSheet(ddlList);
            return PartialView("Select", list);
        }

        //details View
        [HttpPost]
        public ActionResult ViewFIRDetails(int id)
        {
            var entity = _prmCommonService.PRMUnit.FIRInfoRepository.GetByID(id);
            var model = entity.ToModel();

            model.SignatoryEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.SignatoryEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.SignatoryDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

            var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                       join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                       join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id
                       where (comNoteSheet.Id == entity.ComplaintNoteSheetId)
                       select new
                       {
                           //Id = comNoteSheet.Id,
                           DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                           RefNo = comNoteSheet.RefNo,
                           ComplaintDate = comNoteSheet.ComplaintDate,
                           ComplaintDetails = comNoteSheet.ComplaintDetails,
                           ComplaintEmpId = empComplaint.EmpID,
                           ComplaintEmployeeName = empComplaint.FullName,
                           ComplaintDesignationName = empComplaint.PRM_Designation.Name,
                           ComplaintDepartmentName = empComplaint.PRM_Division.Name,
                           ComplainantEmpId = empComplainant.EmpID,
                           ComplainantEmployeeName = empComplainant.FullName,
                           ComplainantDesignationName = empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant.PRM_Division.Name,
                       }).FirstOrDefault();
            model.ComplaintNoteSheetName = obj.DeptProceedingNo;
            model.RefNo = obj.RefNo;
            model.ComplaintDate = Convert.ToDateTime(obj.ComplaintDate).ToString(DateAndTime.GlobalDateFormat);
            model.ComplaintDetails = obj.ComplaintDetails;
            model.ComplaintEmpId = obj.ComplaintEmpId;
            model.ComplaintEmployeeName = obj.ComplaintEmployeeName;
            model.ComplaintDesignationName = obj.ComplaintDesignationName;
            model.ComplaintDepartmentName = obj.ComplaintDepartmentName;
            model.ComplainantEmpId = obj.ComplainantEmpId;
            model.ComplainantEmployeeName = obj.ComplainantEmployeeName;
            model.ComplainantDesignationName = obj.ComplainantDesignationName;
            model.ComplainantDepartmentName = obj.ComplainantDepartmentName;

            return View(model);
        }
    }
}