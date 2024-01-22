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
    public class NoteOrderInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public NoteOrderInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions
        
        //
        // GET: /PRM/NoteOrderInfo/        
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, NoteOrderInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<NoteOrderInfoViewModel> list = (from noteOrderInfo in _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetAll()
                                                 join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll() on noteOrderInfo.ComplaintNoteSheetId equals comNoteSheet.Id
                                                 join complaintEmpInfo in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals complaintEmpInfo.Id
                                                 where (comNoteSheet.ZoneInfoId == LoggedUserZoneInfoId)
                                                 && (model.ComplaintNoteSheetId == 0 || model.ComplaintNoteSheetId == comNoteSheet.Id)
                                                 && (model.OrderTypeInfoId == 0 || model.OrderTypeInfoId == noteOrderInfo.OrderTypeInfoId)
                                                 && (model.OrderDate == null || model.OrderDate == noteOrderInfo.OrderDate)
                                                 && (model.ComplaintDate == null || comNoteSheet.ComplaintDate == Convert.ToDateTime(model.ComplaintDate))
                                                && (model.ComplaintEmployeeName == "" || model.ComplaintEmployeeName == null || model.ComplaintEmployeeName == complaintEmpInfo.FullName)
                                                && (model.ComplaintDesignationId == null || model.ComplaintDesignationId == 0 || model.ComplaintDesignationId == complaintEmpInfo.DesignationId)
                                                 select new NoteOrderInfoViewModel()
                                                    {
                                                        Id = noteOrderInfo.Id,
                                                        ComplaintNoteSheetId = noteOrderInfo.ComplaintNoteSheetId,
                                                        ComplaintNoteSheetName = noteOrderInfo.PRM_ComplaintNoteSheet.DeptProceedingNo,
                                                        ComplaintDate = comNoteSheet.ComplaintDate.ToString(),
                                                        ComplaintEmployeeName = complaintEmpInfo.FullName,
                                                        ComplaintDesignationId = complaintEmpInfo.DesignationId,
                                                        ComplaintDesignationName = complaintEmpInfo.PRM_Designation.Name,
                                                        OrderTypeInfoId = noteOrderInfo.OrderTypeInfoId,
                                                        OrderTypeInfoName = noteOrderInfo.PRM_OrderTypeInfo.Name,
                                                        OrderDate = noteOrderInfo.OrderDate,
                                                        FileStatus = noteOrderInfo.PRM_OrderTypeInfo.Name == PRMEnum.DepartmentalProceedingOrderType.FinalOrder.ToString() ? "Closed" : "Pending"
                                                    }).OrderBy(x => x.OrderDate).ToList();



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


            if (request.SortingName == "OrderTypeInfoName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OrderTypeInfoName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OrderTypeInfoName).ToList();
                }
            }

            if (request.SortingName == "OrderDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OrderDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OrderDate).ToList();
                }
            }

            if (request.SortingName == "FileStatus")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FileStatus).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FileStatus).ToList();
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
                int isEditable = 1;
                var lst = list.Where(q => q.ComplaintNoteSheetId == d.ComplaintNoteSheetId).OrderByDescending(q => q.OrderDate).ToList();
                var item = lst.Where(q => q.OrderTypeInfoName == PRMEnum.DepartmentalProceedingOrderType.FinalOrder.ToString()).Any();
                isEditable = item == true ? 0 : 1;


                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,                  
                    d.ComplaintNoteSheetId,
                    d.ComplaintNoteSheetName,
                    //Convert.ToDateTime(d.ComplaintDate).ToString(DateAndTime.GlobalDateFormat),  
                    d.ComplaintEmployeeName,
                    d.ComplaintDesignationId,
                    d.ComplaintDesignationName,                   
                    d.OrderTypeInfoId,
                    d.OrderTypeInfoName,
                    //Convert.ToDateTime(d.OrderDate).ToString(DateAndTime.GlobalDateFormat),   
                    d.FileStatus,
                    isEditable
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            NoteOrderInfoViewModel model = new NoteOrderInfoViewModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(NoteOrderInfoViewModel model)
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
                        _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.Add(entity);
                        _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.SaveChanges();
                        model.errClass = "success";
                        model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        //  return RedirectToAction("Index");
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

            populateDropdown(model);
            return View(model);

        }


        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetByID(Id);
            var model = entity.ToModel();
            model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;
            model.strMode = "Edit";

            if (model.OrderByEmployeeId != null)
            {
                model.OrderByEmpId = entity.PRM_EmploymentInfo.EmpID;
                model.OrderByEmployeeName = entity.PRM_EmploymentInfo.FullName;
                model.OrderByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            }

            var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                       join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                       join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id into gr
                       from empComplainant in gr.DefaultIfEmpty()
                       where (comNoteSheet.Id == entity.ComplaintNoteSheetId)
                       select new
                       {
                           //Id = comNoteSheet.Id,
                           //DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                           RefNo = comNoteSheet.RefNo,
                           ComplaintDate = comNoteSheet.ComplaintDate,
                           ComplaintEmployeeId = empComplaint.Id,
                           ComplaintEmpId = empComplaint.EmpID,
                           ComplaintEmployeeName = empComplaint.FullName,
                           ComplaintDesignationName = empComplaint.PRM_Designation.Name,
                           ComplaintDepartmentName = empComplaint.PRM_Division == null ? string.Empty : empComplaint.PRM_Division.Name,
                           ComplainantEmpId = empComplainant ==null? string.Empty: empComplainant.EmpID,
                           ComplainantEmployeeName = empComplainant == null ? string.Empty : empComplainant.FullName,
                           ComplainantDesignationName = empComplainant == null ? string.Empty : empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant == null ? string.Empty : empComplainant.PRM_Division == null ? string.Empty : empComplainant.PRM_Division.Name,
                       }).FirstOrDefault();

            model.RefNo = obj.RefNo;
            model.ComplaintDate = Convert.ToDateTime(obj.ComplaintDate).ToString(DateAndTime.GlobalDateFormat);
            model.ComplaintEmployeeId = obj.ComplaintEmployeeId;
            model.ComplaintEmpId = obj.ComplaintEmpId;
            model.ComplaintEmployeeName = obj.ComplaintEmployeeName;
            model.ComplaintDesignationName = obj.ComplaintDesignationName;
            model.ComplaintDepartmentName = obj.ComplaintDepartmentName;
            model.ComplainantEmpId = obj.ComplainantEmpId;
            model.ComplainantEmployeeName = obj.ComplainantEmployeeName;
            model.ComplainantDesignationName = obj.ComplainantDesignationName;
            model.ComplainantDepartmentName = obj.ComplainantDepartmentName;


            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(NoteOrderInfoViewModel model)
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
                        _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.Update(entity);
                        _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        //return RedirectToAction("Index");
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
                    // model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.Delete(id);
                _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.SaveChanges();
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


        [NoCache]
        public ActionResult Review(int Id)
        {
            var entity = _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetByID(Id);
            var model = entity.ToModel();

            if (model.OrderByEmployeeId != null)
            {
                model.OrderByEmpId = entity.PRM_EmploymentInfo.EmpID;
                model.OrderByEmployeeName = entity.PRM_EmploymentInfo.FullName;
                model.OrderByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            }
            var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                       join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                       join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id into gr
                       from empComplainant in gr.DefaultIfEmpty()
                       where (comNoteSheet.Id == entity.ComplaintNoteSheetId)
                       select new
                       {
                           //Id = comNoteSheet.Id,
                           //DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                           RefNo = comNoteSheet.RefNo,
                           ComplaintDate = comNoteSheet.ComplaintDate,
                           ComplaintEmployeeId = empComplaint.Id,
                           ComplaintEmpId = empComplaint.EmpID,
                           ComplaintEmployeeName = empComplaint.FullName,
                           ComplaintDesignationName = empComplaint.PRM_Designation.Name,
                           ComplaintDepartmentName = empComplaint.PRM_Division == null ? string.Empty : empComplaint.PRM_Division.Name,
                           ComplainantEmpId = empComplainant == null ? string.Empty: empComplainant.EmpID,
                           ComplainantEmployeeName = empComplainant == null ? string.Empty : empComplainant.FullName,
                           ComplainantDesignationName = empComplainant == null ? string.Empty : empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant == null ? string.Empty : empComplainant.PRM_Division == null ? string.Empty : empComplainant.PRM_Division.Name,
                       }).FirstOrDefault();

            model.RefNo = obj.RefNo;
            model.ComplaintDate = Convert.ToDateTime(obj.ComplaintDate).ToString(DateAndTime.GlobalDateFormat);
            model.ComplaintEmployeeId = obj.ComplaintEmployeeId;
            model.ComplaintEmpId = obj.ComplaintEmpId;
            model.ComplaintEmployeeName = obj.ComplaintEmployeeName;
            model.ComplaintDesignationName = obj.ComplaintDesignationName;
            model.ComplaintDepartmentName = obj.ComplaintDepartmentName;
            model.ComplainantEmpId = obj.ComplainantEmpId;
            model.ComplainantEmployeeName = obj.ComplainantEmployeeName;
            model.ComplainantDesignationName = obj.ComplainantDesignationName;
            model.ComplainantDepartmentName = obj.ComplainantDepartmentName;

            populateDropdown(model);
            return View(model);
        }
        #endregion

        #region Private Method
        private void populateDropdown(NoteOrderInfoViewModel model)
        {
            dynamic ddlList;
            //use as Departmental Proceedigs No.
            #region Complaint/Note Sheet

            ddlList = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.DeptProceedingNo).ToList();
            model.ComplaintNoteSheetList = Common.PopulateComplaintNoteSheet(ddlList);

            #endregion

            #region Order Type

            ddlList = _prmCommonService.PRMUnit.OrderTypeInfoRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            model.OrderTypeInfoList = Common.PopulateDllList(ddlList);

            #endregion

            #region Disciplinary Action Type

            ddlList = _prmCommonService.PRMUnit.DisciplinaryActionTypeRepository.GetAll().OrderBy(o => o.SortOrder).ToList();
            model.DisciplinaryActionTypeList = Common.PopulateDllList(ddlList);

            #endregion
        
            #region Punishment Type
            ddlList = _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.Get(q => q.Id == model.PunishmentTypeInfoId).OrderBy(x => x.PunishmentName).ToList();
            model.PunishmentTypeInfoList = Common.PopulatePunishmentTypeDDL(ddlList);
            #endregion

        }
        private NoteOrderInfoViewModel GetInsertUserAuditInfo(NoteOrderInfoViewModel model, bool pAddEdit)
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


        [NoCache]
        public JsonResult GetComplaintNoteInfo(int complaintNoteSheetId)
        {
            var obj = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                       join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                       join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id into gr
                       from empComplainant in gr.DefaultIfEmpty()
                       where (comNoteSheet.Id == complaintNoteSheetId)
                       select new
                       {
                           //Id = comNoteSheet.Id,
                           //DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                           RefNo = comNoteSheet.RefNo,
                           ComplaintDate = comNoteSheet.ComplaintDate,
                           ComplaintDetails = comNoteSheet.ComplaintDetails,
                           ComplaintEmployeeId = empComplaint.Id,
                           ComplaintEmpId = empComplaint.EmpID,
                           ComplaintEmployeeName = empComplaint.FullName,
                           ComplaintDesignationName = empComplaint.PRM_Designation.Name,
                           ComplaintDepartmentName = empComplaint.PRM_Division == null ? string.Empty : empComplaint.PRM_Division.Name,

                           ComplainantEmpId = empComplainant == null ? string.Empty : empComplainant.EmpID,
                           ComplainantEmployeeName = empComplainant == null ? string.Empty : empComplainant.FullName,
                           ComplainantDesignationName = empComplainant == null ? string.Empty : empComplainant.PRM_Designation.Name,
                           ComplainantDepartmentName = empComplainant == null ? string.Empty : empComplainant.PRM_Division == null ? string.Empty : empComplainant.PRM_Division.Name,
                       }).FirstOrDefault();

            // var obj = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetByID(complaintNoteSheetId);
            return Json(new
            {
                RefNo = obj.RefNo,
                ComplaintDate = obj.ComplaintDate == null? string.Empty : Convert.ToDateTime(obj.ComplaintDate).ToString("dd-MM-yyyy"),
                ComplaintDetails = obj.ComplaintDetails,
                ComplaintEmployeeId = obj.ComplaintEmployeeId,
                ComplaintEmpId = obj.ComplaintEmpId,
                ComplaintEmployeeName = obj.ComplaintEmployeeName,
                ComplaintDesignationName = obj.ComplaintDesignationName,
                ComplaintDepartmentName = obj.ComplaintDepartmentName,
                ComplainantEmpId = obj.ComplainantEmpId,
                ComplainantEmployeeName = obj.ComplainantEmployeeName,
                ComplainantDesignationName = obj.ComplainantDesignationName,
                ComplainantDepartmentName = obj.ComplainantDepartmentName
            },JsonRequestBehavior.AllowGet);

        }


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
            },JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetPunishmentType(int disciplinaryActionTypeId)
        {
            var punishmentList = _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.Get(q => q.DisciplinaryActionTypeId == disciplinaryActionTypeId).ToList();
            return Json(
               new
               {
                   punishments = punishmentList.Select(x => new { Id = x.Id, PunishmentName = x.PunishmentName })
               },
               JsonRequestBehavior.AllowGet
           );
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetListComplaint(JqGridRequest request, int employeeId)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };
            var list = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                        join empComplaint in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplaintEmployeeId equals empComplaint.Id
                        where (comNoteSheet.ComplaintEmployeeId == employeeId)
                        select new
                        {
                            Id = comNoteSheet.Id,
                            DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                            ComplaintDate = comNoteSheet.ComplaintDate,
                            ComplaintEmployeeName = empComplaint.FullName,
                            ComplaintDesignationName = empComplaint.PRM_Designation.Name,
                        }).OrderBy(x => x.ComplaintDate).ToList();
            // var list = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.Get(t => t.ComplaintEmployeeId == employeeId);

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.DeptProceedingNo,
                    d.ComplaintDate,
                    d.ComplaintEmployeeName, 
                    d.ComplaintEmployeeName,
                    "Details"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetComplaintListByEmployeeId(int employeeId)
        {
            var list = (from comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                        join empComplainant in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on comNoteSheet.ComplainantEmployeeId equals empComplainant.Id
                        where (comNoteSheet.ComplaintEmployeeId == employeeId)
                        select new
                        {
                            Id = comNoteSheet.Id,
                            DeptProceedingNo = comNoteSheet.DeptProceedingNo,
                            ComplaintDate = Convert.ToDateTime(comNoteSheet.ComplaintDate).ToString(DateAndTime.GlobalDateFormat),
                            ComplainantEmployeeName = empComplainant.FullName,
                            ComplainantDesignationName = empComplainant.PRM_Designation.Name,
                        }).OrderBy(x => x.ComplaintDate).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public PartialViewResult GetTypeListByCompalintNoteSheetId(int compalintNoteSheetId)
        {
            var model = new NoteOrderInfoViewModel();

            List<NoteOrderInfoViewModel> TypeList = new List<NoteOrderInfoViewModel>();

            var complaintNoteSheetList = (from compNoteSheetInfo in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll()
                                          where (compNoteSheetInfo.Id == compalintNoteSheetId)
                                          select new
                                          {
                                              TypeId = compNoteSheetInfo.Id,
                                              TypeName = "Complaint/Note Sheet",
                                              ControllerName = "ComplaintNoteSheet",
                                              ActionName = "ViewComplaintDetails"
                                          }).ToList();


            var chargeSheetList = (from chargeSheetInfo in _prmCommonService.PRMUnit.ChargeSheetInfoRepository.GetAll()
                                   where (chargeSheetInfo.ComplaintNoteSheetId == compalintNoteSheetId)
                                   select new
                                   {
                                       TypeId = chargeSheetInfo.Id,
                                       TypeName = "Charge Sheet (FIR)",
                                       ControllerName = "ChargeSheetInfo",
                                       ActionName = "ViewChargeSheetDetails"
                                   }).ToList();

            var noticeType = (from noteOrderInfo in _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetAll()
                              where (noteOrderInfo.ComplaintNoteSheetId == compalintNoteSheetId)
                              select new
                              {
                                  TypeId = noteOrderInfo.Id,
                                  TypeName = noteOrderInfo.PRM_OrderTypeInfo.Name,
                                  ControllerName = "NoteOrderInfo",
                                  ActionName = "ViewNoteOrderDetails"
                              }).ToList();

            //  complaintNoteSheetList.AddRange(noticeType);

            var healthyFirst = complaintNoteSheetList.Concat(chargeSheetList);
            var output = healthyFirst.Concat(noticeType);

            var i = 1;
            foreach (var item in output)
            {
                var gridModel = new NoteOrderInfoViewModel
                {
                    TypeSlNo = i++,
                    TypeId = item.TypeId,
                    TypeName = item.TypeName,
                    ControllerName = item.ControllerName,
                    ActionName = item.ActionName
                };
                TypeList.Add(gridModel);
            }
            model.NoteOrderInfoTypeList = TypeList;
            return PartialView("_Details", model);
        }


        //details View
        [HttpPost]
        public ActionResult ViewNoteOrderDetails(int id)
        {
            var entity = _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetByID(id);
            var model = entity.ToModel();

            model.OrderByEmpId = entity.PRM_EmploymentInfo.EmpID;
            model.OrderByEmployeeName = entity.PRM_EmploymentInfo.FullName;
            model.OrderByDesignationName = entity.PRM_EmploymentInfo.PRM_Designation.Name;

            model.ComplaintNoteSheetName = entity.PRM_ComplaintNoteSheet.DeptProceedingNo;
            model.OrderTypeInfoName = entity.PRM_OrderTypeInfo.Name;
            return PartialView("_NoteOrderDetailsView", model);
        }

        [NoCache]
        public ActionResult DeptProceedingListView()
        {
            var ddlList = (from noteOrder in _prmCommonService.PRMUnit.NoteOrderInfoReportRepository.GetAll().GroupBy(d => d.ComplaintNoteSheetId).Select(grp => grp.First()).ToList()
                           join comNoteSheet in _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId) on noteOrder.ComplaintNoteSheetId equals comNoteSheet.Id
                           select comNoteSheet).OrderBy(x => x.DeptProceedingNo).ToList();
            var list = Common.PopulateComplaintNoteSheet(ddlList);
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult DesignationListView()
        {
            var list = Common.PopulateEmployeeDesignationDDL(_prmCommonService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.SortingOrder).ToList());
            return PartialView("Select", list);
        }

        [NoCache]
        public ActionResult OrderTypeListView()
        {
            var list = Common.PopulateDllList(_prmCommonService.PRMUnit.OrderTypeInfoRepository.GetAll().OrderBy(x => x.SortOrder).ToList());
            return PartialView("Select", list);
        }
    }
}