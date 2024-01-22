using BEPZA_MEDICAL.DAL.WFM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Domain.WFM;
using BEPZA_MEDICAL.Web.Areas.WFM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using MyNotificationLib.Operation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.Controllers
{
    public class PaymentInfoController : BaseController
    {

        #region Fields

        private readonly WFMCommonService _wpfCommonService;
        private readonly PRMCommonSevice _prmCommonService;
        private readonly ERP_BEPZAWFMEntities _wfmContext;
        #endregion

        #region Ctor
        public PaymentInfoController(WFMCommonService wpfCommonService, PRMCommonSevice prmCommonService, ERP_BEPZAWFMEntities wfmContext)
        {
            this._wpfCommonService = wpfCommonService;
            this._prmCommonService = prmCommonService;
            this._wfmContext = wfmContext;
        }
        #endregion

        #region Actions
        //
        // GET: WFM/PaymentInfo
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, PaymentInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from wfc in _wpfCommonService.WFMUnit.PaymentInfoRepository.GetAll()
                        select new PaymentInfoViewModel()
                                                      {
                                                          Id = wfc.Id,
                                                          PayOrderNo = wfc.PayOrderNo,
                                                          PayOrderDate = wfc.PayOrderDate,
                                                          ApprovalWelfareFundInfoId = wfc.ApprovalWelfareFundInfoId,
                                                          ApprovalWelfareFundInfoRefNo = wfc.WFM_ApprovalWelfareFundInfo.RefNo
                                                      }).OrderBy(x => x.PayOrderDate).ToList();

            if (request.Searching)
            {
                if ((viewModel.PayOrderNo != null && viewModel.PayOrderNo != ""))
                {
                    list = list.Where(d => d.PayOrderNo == viewModel.PayOrderNo).ToList();
                }
                if ((viewModel.ApprovalWelfareFundInfoId != 0))
                {
                    list = list.Where(d => d.ApprovalWelfareFundInfoId == viewModel.ApprovalWelfareFundInfoId).ToList();
                }

                if ((viewModel.PayOrderDate != null && viewModel.PayOrderDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.PayOrderDate == viewModel.PayOrderDate).ToList();
                }

               

            }
            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "PayOrderNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PayOrderNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PayOrderNo).ToList();
                }
            }

            if (request.SortingName == "PayOrderDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PayOrderDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PayOrderDate).ToList();
                }
            }
            if (request.SortingName == "ApprovalWelfareFundInfoRefNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApprovalWelfareFundInfoRefNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApprovalWelfareFundInfoRefNo).ToList();
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
                    d.PayOrderNo,
                    Convert.ToDateTime(d.PayOrderDate).ToString(DateAndTime.GlobalDateFormat),
                    d.ApprovalWelfareFundInfoId,
                    d.ApprovalWelfareFundInfoRefNo
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Create()
        {
            PaymentInfoViewModel model = new PaymentInfoViewModel();
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(PaymentInfoViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "This 'Meeting Ref. No.' already exist.";
                        model.errClass = "failed";
                        populateDropdown(model);
                        setEmployeeList(model);
                        return View(model);
                    }

                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = CreateEntity(model, true);
                    if (errorList.Length == 0)
                    {
                        _wpfCommonService.WFMUnit.PaymentInfoRepository.Add(entity);
                        _wpfCommonService.WFMUnit.PaymentInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
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

            populateDropdown(model);
            return View(model);
        }
        private void Notification(int employeeId, string amount)
        {
            var redirectToUrl = String.Empty;
            MyNotificationLibEnum.NotificationType notificationType = MyNotificationLibEnum.NotificationType.Welfare_Fund;

            // Declare Notification Variables

            var modules = new List<MyNotificationLibEnum.NotificationModule>();
            modules.Add(MyNotificationLibEnum.NotificationModule.Welfare_Fund_Management_System);

            var toEmployees = new List<int>();

            #region Notify To
            if (employeeId > 0)
            {
                // Applicant info
                var applicant = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                    .FirstOrDefault(e => e.Id == employeeId);
                var applicantInfo = "Welfare Fund application have been approved for " + applicant.FullName + ", " + (applicant.PRM_Designation.Name) + ", " +
                                    applicant.EmpID+" and approved amount is "+ amount +" Tk.";

                // Notify to employees
                var desiList = (from desi in _prmCommonService.PRMUnit.DesignationRepository.GetAll()
                               where desi.Name.Contains("Accounts") select desi.Id).ToList();

                var empList = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().Where(x => desiList.Contains(x.DesignationId) && x.ZoneInfoId == applicant.ZoneInfoId).ToList();

                toEmployees = empList.Select(s => s.Id).ToList();

                var generalPurposeNotification =
                    new SendGeneralPurposeNotification(
                        modules,
                        applicantInfo,
                        redirectToUrl,
                        toEmployees,
                        MyAppSession.EmpId,
                        notificationType
                    );
                generalPurposeNotification.SendNotification();
            }

            #endregion

            #region Self Notification

            toEmployees.Clear();
            toEmployees.Add(employeeId);

            var notificationForApplicant = new SendGeneralPurposeNotification(
                modules
                , "Your Welfare Fund application have been approved for " + amount + " Tk."
                , String.Empty
                , toEmployees
                , MyAppSession.EmpId
                , notificationType);
            notificationForApplicant.SendNotification();

            #endregion
        }

        public ActionResult Edit(int id)
        {
            var entity = _wpfCommonService.WFMUnit.PaymentInfoRepository.GetByID(id);
            var model = entity.ToModel();
            model.ApprovalWelfareFundInfoRefNo = entity.WFM_ApprovalWelfareFundInfo.RefNo;


            model.CommitteeName = entity.WFM_ApprovalWelfareFundInfo.CommitteeName;
            model.MeetDate = entity.WFM_ApprovalWelfareFundInfo.MeetDate.ToString(DateAndTime.GlobalDateFormat);
            model.MeetTime = entity.WFM_ApprovalWelfareFundInfo.MeetTime.ToString();
            model.MeetPlace = entity.WFM_ApprovalWelfareFundInfo.MeetPlace;
            model.MeetAgenda = entity.WFM_ApprovalWelfareFundInfo.MeetAgenda;
            model.MeetCycle = entity.WFM_ApprovalWelfareFundInfo.WFM_CycleInfo.CycleName;
            model.MeetCycleMonth = entity.WFM_ApprovalWelfareFundInfo.WFM_CycleInfo.FromMonth + '-' + entity.WFM_ApprovalWelfareFundInfo.WFM_CycleInfo.ToMonth;
            model.MeetCycleYear = entity.WFM_ApprovalWelfareFundInfo.Year;

            //employeeList
            List<PaymentInfoEmployeeDetailsViewDetail> list = (from payEmpDtal in _wpfCommonService.WFMUnit.PaymentInfoEmployeeDetailsRepository.GetAll().Where(q => q.PaymentInfoId == id)
                                                               join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on payEmpDtal.EmployeeId equals emp.Id
                                                               join aprvalEmpDtl in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll() on payEmpDtal.EmployeeId equals aprvalEmpDtl.EmployeeId
                                                               where (model.ApprovalWelfareFundInfoId == aprvalEmpDtl.ApprovalWelfareFundInfoId)
                                                               select new PaymentInfoEmployeeDetailsViewDetail
                                                                          {
                                                                              Id = payEmpDtal.Id,
                                                                              IsCheckedFinal = true,
                                                                              EmployeeId = payEmpDtal.EmployeeId,
                                                                              EmpId = emp.EmpID,
                                                                              EmployeeName = emp.FullName,
                                                                              Designation = emp.PRM_Designation.Name,
                                                                              Department = emp.PRM_Division.Name,
                                                                              Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
                                                                              ApprovedAmount = aprvalEmpDtl.ApprovedAmount.ToString()
                                                                          }).ToList();

            List<PaymentInfoEmployeeDetailsViewDetail> list2 = (from aprvalEmpDtl in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll()
                                                                join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on aprvalEmpDtl.EmployeeId equals emp.Id
                                                                where (aprvalEmpDtl.ApprovalWelfareFundInfoId == model.ApprovalWelfareFundInfoId)
                                                                select new PaymentInfoEmployeeDetailsViewDetail()
                                                                {
                                                                    IsCheckedFinal = false,
                                                                    EmployeeId = aprvalEmpDtl.EmployeeId,
                                                                    EmpId = emp.EmpID,
                                                                    EmployeeName = emp.FullName,
                                                                    Department = emp.PRM_Division.Name,
                                                                    Designation = emp.PRM_Designation.Name,
                                                                    Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
                                                                    ApprovedAmount = aprvalEmpDtl.ApprovedAmount.ToString()
                                                                }).ToList();

            var merged = new List<PaymentInfoEmployeeDetailsViewDetail>(list);
            merged.AddRange(list2.Where(p2 => list.All(p1 => p1.EmployeeId != p2.EmployeeId)));
            model.EmployeeList = merged;

            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(PaymentInfoViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "This 'Meeting Ref. No.' already exist.";
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    model = GetInsertUserAuditInfo(model, false);
                    var entity = CreateEntity(model, false);
                    if (errorList.Length == 0)
                    {
                        _wpfCommonService.WFMUnit.PaymentInfoRepository.Update(entity);
                        _wpfCommonService.WFMUnit.PaymentInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }

            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //if (ex.InnerException != null && ex.InnerException is SqlException)
                //{
                //    SqlException sqlException = ex.InnerException as SqlException;
                //    model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                //}
                //else
                //{
                //    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                //}
            }
            setEmployeeList(model);
            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _wpfCommonService.WFMUnit.PaymentInfoRepository.Delete(id);
                _wpfCommonService.WFMUnit.PaymentInfoRepository.SaveChanges();
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


        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _wpfCommonService.WFMUnit.PaymentInfoEmployeeDetailsRepository.Delete(id);
                _wpfCommonService.WFMUnit.PaymentInfoEmployeeDetailsRepository.SaveChanges();
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

        public ActionResult PaymentDetails(int id)
        {
            var model = new PaymentInfoViewModel();

            return View("_PaymentDetailsList", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetDetailsList(JqGridRequest request, int id, PaymentInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from PI in _wpfCommonService.WFMUnit.PaymentInfoRepository.GetAll()
                        join PIDtl in _wpfCommonService.WFMUnit.PaymentInfoEmployeeDetailsRepository.GetAll() on PI.Id equals PIDtl.PaymentInfoId
                        join PA in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll() on PI.ApprovalWelfareFundInfoId equals PA.Id
                        join PADtl in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll() on PA.Id equals PADtl.ApprovalWelfareFundInfoId
                        join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on PIDtl.EmployeeId equals emp.Id
                        join Cat in _wpfCommonService.WFMUnit.WelfareFundCategoryRepository.GetAll() on PA.WelfareFundCategoryId equals Cat.Id
                        where(PIDtl.PaymentInfoId == id)
                        select new PaymentInfoViewModel()
                        {
                            Id = PADtl.Id,
                            PayOrderNo = PI.PayOrderNo,
                            PayOrderDate = PI.PayOrderDate,
                            EmpId = emp.EmpID,
                            Name = emp.FullName,
                            Designation = emp.PRM_Designation.Name,
                            ApproveAmount = PADtl.ApprovedAmount.ToString(),
                            CategoryName = Cat.Name
                        }).OrderBy(x => x.PayOrderDate).ToList();

            if (request.Searching)
            {
                if ((viewModel.PayOrderNo != null && viewModel.PayOrderNo != ""))
                {
                    list = list.Where(d => d.PayOrderNo == viewModel.PayOrderNo).ToList();
                }
                if ((viewModel.ApprovalWelfareFundInfoId != 0))
                {
                    list = list.Where(d => d.ApprovalWelfareFundInfoId == viewModel.ApprovalWelfareFundInfoId).ToList();
                }

                if ((viewModel.PayOrderDate != null && viewModel.PayOrderDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.PayOrderDate == viewModel.PayOrderDate).ToList();
                }



            }
            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "PayOrderNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PayOrderNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PayOrderNo).ToList();
                }
            }

            if (request.SortingName == "PayOrderDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PayOrderDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PayOrderDate).ToList();
                }
            }
            if (request.SortingName == "ApprovalWelfareFundInfoRefNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApprovalWelfareFundInfoRefNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApprovalWelfareFundInfoRefNo).ToList();
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
                    d.PayOrderNo,
                    Convert.ToDateTime(d.PayOrderDate).ToString(DateAndTime.GlobalDateFormat),
                    d.EmpId,
                    d.Name,
                    d.Designation,
                    d.ApproveAmount,
                    d.CategoryName
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        public ActionResult VoucherPosing(int id)
        {
            string url = string.Empty;
            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = "";
            string Username = "";
            string ZoneID = "";
            if (sessionUser != null)
            {
                UserID = sessionUser.UserId;
                password = sessionUser.Password;
                Username = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneID = MyAppSession.ZoneInfoId.ToString();
            }

            var empInfo = _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetByID(id);

            var amount = empInfo.ApprovedAmount;

            var obj = _wfmContext.WFM_uspVoucherPosting(id, empInfo.EmployeeId, amount).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }
            return Redirect(url);
        }
        #endregion

        #region Private Method
        private void populateDropdown(PaymentInfoViewModel model)
        {
            dynamic ddlList;

            #region Meetng Ref ddl

            ddlList = _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll().OrderBy(x => x.MeetDate).ToList();
            model.ApprovalWelfareFundInfoList = Common.PopulateMeetingRefNoDDL(ddlList);
            #endregion

        }

        private bool CheckDuplicateEntry(PaymentInfoViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _wpfCommonService.WFMUnit.PaymentInfoRepository.Get(q => q.ApprovalWelfareFundInfoId == model.ApprovalWelfareFundInfoId).Any();
            }

            else
            {
                return _wpfCommonService.WFMUnit.PaymentInfoRepository.Get(q => q.ApprovalWelfareFundInfoId == model.ApprovalWelfareFundInfoId && strMode != q.Id).Any();
            }
        }
        private PaymentInfoViewModel GetInsertUserAuditInfo(PaymentInfoViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                foreach (var child in model.EmployeeList)
                {
                    child.IUser = User.Identity.Name;
                    child.IDate = DateTime.Now;
                }

            }
            else
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                foreach (var child in model.EmployeeList)
                {
                    child.IUser = model.IUser;
                    child.IDate = model.IDate;

                    child.EUser = User.Identity.Name;
                    child.EDate = DateTime.Now;
                }
            }

            return model;
        }
        
        private WFM_PaymentInfo CreateEntity(PaymentInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
            //employee
            foreach (var item in model.EmployeeList.Where(q=>q.IsCheckedFinal==true))
            {
                var wfm_PaymentInfoEmployeeDetails = new WFM_PaymentInfoEmployeeDetails();

                wfm_PaymentInfoEmployeeDetails.Id = item.Id;
                wfm_PaymentInfoEmployeeDetails.EmployeeId = item.EmployeeId;
                wfm_PaymentInfoEmployeeDetails.IUser = User.Identity.Name;
                wfm_PaymentInfoEmployeeDetails.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    wfm_PaymentInfoEmployeeDetails.IUser = User.Identity.Name;
                    wfm_PaymentInfoEmployeeDetails.IDate = DateTime.Now;
                    entity.WFM_PaymentInfoEmployeeDetails.Add(wfm_PaymentInfoEmployeeDetails);
                    Notification(item.EmployeeId, item.ApprovedAmount);
                }
                else
                {
                    wfm_PaymentInfoEmployeeDetails.PaymentInfoId = model.Id;
                    wfm_PaymentInfoEmployeeDetails.EUser = User.Identity.Name;
                    wfm_PaymentInfoEmployeeDetails.EDate = DateTime.Now;

                    if (item.Id == 0)
                    {
                        _wpfCommonService.WFMUnit.PaymentInfoEmployeeDetailsRepository.Add(wfm_PaymentInfoEmployeeDetails);
                        Notification(item.EmployeeId, item.ApprovedAmount);
                    }
                    else
                    {
                        _wpfCommonService.WFMUnit.PaymentInfoEmployeeDetailsRepository.Update(wfm_PaymentInfoEmployeeDetails);

                    }
                }

            }

            return entity;
        }

        private void setEmployeeList(PaymentInfoViewModel model)
        {
            List<PaymentInfoEmployeeDetailsViewDetail> list = (from aprvlDtlempInfo in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll()
                                                               join aprvlFundInfo in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll() on aprvlDtlempInfo.ApprovalWelfareFundInfoId equals aprvlFundInfo.Id
                                                               join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on aprvlDtlempInfo.EmployeeId equals emp.Id
                                                               where (aprvlDtlempInfo.ApprovalWelfareFundInfoId == model.ApprovalWelfareFundInfoId)
                                                               select new PaymentInfoEmployeeDetailsViewDetail
                                                               {
                                                                   EmployeeId = aprvlDtlempInfo.EmployeeId,
                                                                   EmpId = emp.EmpID,
                                                                   EmployeeName = emp.FullName,
                                                                   Designation = emp.PRM_Designation.Name,
                                                                   Department = emp.PRM_Division.Name,
                                                                   Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
                                                                   ApprovedAmount = aprvlDtlempInfo.ApprovedAmount.ToString()
                                                               }).ToList();

            model.EmployeeList = list;
            model.ShowRecord = "Show";             
            
        }

        #endregion



        [HttpGet]
        public PartialViewResult GetEmployeeList(int? approvalWelfareFundInfoId)
        {
            var model = new PaymentInfoViewModel();

            List<PaymentInfoEmployeeDetailsViewDetail> EmployeeList = new List<PaymentInfoEmployeeDetailsViewDetail>();

            List<PaymentInfoEmployeeDetailsViewDetail> list = (from aprvlDtlempInfo in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoEmployeeDetailsRepository.GetAll()
                                                               join aprvlFundInfo in _wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll() on aprvlDtlempInfo.ApprovalWelfareFundInfoId equals aprvlFundInfo.Id
                                                               join emp in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on aprvlDtlempInfo.EmployeeId equals emp.Id
                                                               where (aprvlDtlempInfo.ApprovalWelfareFundInfoId == approvalWelfareFundInfoId)
                                                               select new PaymentInfoEmployeeDetailsViewDetail
                                                               {
                                                                   EmployeeId = aprvlDtlempInfo.EmployeeId,
                                                                   EmpId = emp.EmpID,
                                                                   EmployeeName = emp.FullName,
                                                                   Designation = emp.PRM_Designation.Name,
                                                                   Department = emp.PRM_Division.Name,
                                                                   Section = emp.SectionId == null ? string.Empty : emp.PRM_Section.Name,
                                                                   ApprovedAmount = aprvlDtlempInfo.ApprovedAmount.ToString()
                                                               }).ToList();

            model.EmployeeList = list;
            return PartialView("_Details", model);
        }

        public ActionResult AprrovalWelfareFundRefNoListView()
        {
            var list = Common.PopulateMeetingRefNoDDL(_wpfCommonService.WFMUnit.ApprovalWelfareFundInfoRepository.GetAll().OrderBy(x => x.MeetDate).ToList());
            return PartialView("Select", list);
        }
    }
}