using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.FAM;
 
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.VoucherInfo;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.DAL.FAM.CustomEntities;
using BEPZA_MEDICAL.Web.Resources;
using System.Collections;
using BEPZA_MEDICAL.DAL.FAM;
using System.Web.Caching;
using BEPZA_MEDICAL.Web.SecurityService;
 


namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class VoucherInfoController : Controller
    {
        #region Fields

        private readonly FAMCommonService _famCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;
        private UserManagementServiceClient _userAgent;
        private User user;
        private int userEmployeeId;
        

        #endregion

        #region Ctor

        public VoucherInfoController(FAMCommonService famCommonservice,   PRMCommonSevice prmCommonService)
        {
            _famCommonservice = famCommonservice;
             
            _prmCommonservice = prmCommonService;
            this._userAgent = new UserManagementServiceClient();
        }

        #endregion

        #region Properties

        public string Message { get; set; }

        private List<string> ChequeNumber
        {
            get
            {
                if (Session["ChequeNumber"] == null)
                    return new List<string>();
                else
                {
                    return (List<string>)Session["ChequeNumber"];
                }
            }
            set
            {
                Session["ChequeNumber"] = value;
            }
        }

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, VoucherSearch model)
        {
            user = _userAgent.GetUserByLoginId(User.Identity.Name);
            userEmployeeId = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(c => c.EmpID == user.EmpId).FirstOrDefault().Id;

            string filterExpression = String.Empty;
            int totalRecords = 0;
            if (model.PendingApproval != null && model.PendingApproval != "0") model.VoucherStatus = "Recommended";
            var list = _famCommonservice.GetVoucherSearchedList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false,
                model.ProjectId,
                model.VoucherNumber,
                model.VoucherDateTo == DateTime.MinValue ? (DateTime?)null : model.VoucherDateTo,
                model.VoucherDateFrom == DateTime.MinValue ? (DateTime?)null : model.VoucherDateFrom,
                model.Payee,
                model.DivisionId,
                model.VoucherStatus,
                model.VoucherTypeId,
                model.ClientId,
                model.PendingApproval,
                User.Identity.Name,
                userEmployeeId
                );

            //totalRecords = list == null ? 0 : list.Count;
            totalRecords = _famCommonservice.GetVoucherSearchedList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, true,
                model.ProjectId,
                model.VoucherNumber,
                model.VoucherDateTo == DateTime.MinValue ? (DateTime?)null : model.VoucherDateTo,
                model.VoucherDateFrom == DateTime.MinValue ? (DateTime?)null : model.VoucherDateFrom,
                model.Payee,
                model.DivisionId,
                model.VoucherStatus,
                model.VoucherTypeId,
                model.ClientId,
                model.PendingApproval,
                User.Identity.Name,
                userEmployeeId).Count();

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                d.Payee = string.IsNullOrEmpty(d.StaffName) ? string.IsNullOrEmpty(d.ClientName) ? string.IsNullOrEmpty(d.VendorName) ? "" : d.VendorName : d.ClientName : d.StaffName;
                response.Records.Add(new JqGridRecord(Convert.ToString(d.ProjectId), new List<object>()
                {
                    d.Id,
                    d.ProjectId,
                    d.VoucherNumber,
                    d.VoucherTypeName,
                    d.VoucherDate.ToString("MM-dd-yyyy"),
                    d.Payee,
                    d.ReceivedBy,
                    d.DivisionName,
                    d.ProjectTitle,
                    d.VoucherDateFrom.ToString("MM-dd-yyyy"),
                    d.VoucherDateTo.ToString("MM-dd-yyyy"),
                    d.VoucherStatus,
                    d.ClientName,
                    d.PendingApproval,

                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index()
        {
            var model = new VoucherInfoModel();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new VoucherInfoModel();
            model.VoucherInfoDetailsClient = new List<VoucherInfoDetails>()
                {
                    new VoucherInfoDetails()
                    {
                        
                    }
                };
            PrepareCreateModel(model);

            return View("_CreateOrEdit", model);
        }

        //The following CreateVoucher is called from Create Invoice Voucher Report link.
        public ActionResult CreateVoucher(decimal? da, string vType, string projectNo, string InvoiceNo)
        {
            
            var model = new VoucherInfoModel();

            try
            {
                model.VoucherTypeId = _famCommonservice.FAMUnit.VoucherTypeRepository.GetAll().Where(c => c.Name == vType).FirstOrDefault().Id;
                model.VoucherNumber= GetVoucherNumberReport(vType);
                model.ReferenceNumber = InvoiceNo;
            }
            catch
            { 
            
            }
            
            model.VoucherInfoDetailsClient = new List<VoucherInfoDetails>()
            {
                new VoucherInfoDetails()
                {
                    Debit=da.Value,
                    //AccountHeadId = _famCommonservice.FAMUnit.Purpose.GetAll().Where(c=>c.Name=="Accounts Receivable Invoice").FirstOrDefault().Id,
                    
                    AccountHeadId = _famCommonservice.FAMUnit.PurposeAccountHeadMappingRepository.GetAll().Where(c=>c.PurposeId==(_famCommonservice.FAMUnit.Purpose.GetAll().Where(p=>p.Name=="Accounts Receivable Invoice").FirstOrDefault().Id)).FirstOrDefault().AccountHeadId,
                    //ProjectId = _pimCommonservice.PIMUnit.ProjectInformation.GetAll().Where(c=> c.ProjectNo==projectNo).FirstOrDefault().Id
                }
            };

            PrepareCreateModel(model);
            return View(model);
        }


        //The following CreateIndentAdjustmentVoucher is called from Create Indent Adjustment Voucher Report link.
        //public ActionResult CreateIndentAdjustmentVoucher(string vType, string projectNo, int? adjId, int? indentNo)
        //{

        //    var model = new VoucherInfoModel();
        //    model.VoucherInfoDetailsClient = new List<VoucherInfoDetails>();

        //    try
        //    {
        //        model.VoucherTypeId = _famCommonservice.FAMUnit.VoucherTypeRepository.GetAll().Where(c => c.Name == vType).FirstOrDefault().Id;
        //        model.VoucherNumber = GetVoucherNumberReport(vType);
        //        model.ReferenceType = "Indent";
        //        model.ReferenceNumber = adjId.ToString();

        //        List<PIM_IndentAdjustmentDetail> list = _pimCommonservice.PIMUnit.IndentAdjustmentDetails.Get(d => d.IndentAdjustmentId == adjId).ToList();

        //        foreach (var item in list)
        //        {
        //            var detail = item.ToModel();
        //            VoucherInfoDetails voucherDetail = new VoucherInfoDetails();
        //            voucherDetail.AccountHeadId = detail.AccountHeadId;
        //            voucherDetail.Debit = detail.AdjustmentAmount;
        //            voucherDetail.ProjectId = _pimCommonservice.PIMUnit.ProjectInformation.GetAll().Where(c => c.ProjectNo == projectNo).FirstOrDefault().Id;
        //            model.VoucherInfoDetailsClient.Add(voucherDetail);
        //        }
        //    }
        //    catch
        //    {

        //    }

        //    PrepareCreateModel(model);
        //    return View(model);
        //}

        public ActionResult BackToList()
        {
            var model = new VoucherInfoModel();

            return View("_Search", model.VoucherSearch);
        }

        [HttpPost]
        public ActionResult Create(VoucherInfoModel model)
        {
            PrepareModelForSave(model);
            var strMessage = string.Empty;
            Message = CheckBusinessRule(model);
            strMessage = Message;
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    if ((model.VoucherNumber.Contains("RV")) || (model.VoucherNumber.Contains("JV")))
                    {
                        foreach (var item in model.VoucherInfoDetailsClient)
                        {
                            item.ChequeNumber = item.ChequeNumberRV;
                        }
                    }
                    var entity = model.ToEntity();
                    foreach (var item in model.VoucherInfoDetailsClient.Select(x => x.ToEntity()))
                    {
                        entity.FAM_VoucherInfoDetails.Add(item);
                    }

                    _famCommonservice.FAMUnit.voucherMaster.Add(entity);
                    Cheque_MarkAsUsed(model);

                    Message = ErrorMessages.InsertSuccessful;
                    strMessage = ErrorMessages.InsertSuccessful;
                }
                //                catch (Exception ex)
                //                {
                //#if DEBUG
                //                    Message = ErrorMessages.InsertFailed + ": " + ex.Message + " inner exception: " + ex.InnerException ?? ex.InnerException.Message;
                //#endif
                //#if RELEASE
                //                    Message = ErrorMessages.InsertFailed;         
                //#endif
                //                }



                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        strMessage = ErrorMessages.UniqueIndex;
                    }
                    else if (ex.InnerException.Message.Contains("out of range"))
                    {
                        strMessage = "Debit/Credit amount is out of range.";
                    }
                    else
                    {
                        strMessage = ErrorMessages.InsertFailed;
                    }
                }
            }
            else
                Message = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
            return new JsonResult()
            {
                //Data = Message
                Data = strMessage
            };
        }

        public ActionResult Edit(int id)
        {
            var entity = _famCommonservice.FAMUnit.voucherMaster.GetByID(id);
            var childEntities = entity.FAM_VoucherInfoDetails;

            var model = entity.ToModel();



            user = _userAgent.GetUserByLoginId(User.Identity.Name);

            userEmployeeId = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(c => c.EmpID == user.EmpId).FirstOrDefault().Id;

            if (model.CurrentApprovalNodeId == userEmployeeId)
            {
                //Check Recommended Or Approved for 'Recommend' Or 'Approve' Button View 
                model.recommenderOrApprover = (from c in _famCommonservice.FAMUnit.ApprovalPathDetails.GetAll()
                                               where c.NodeEmpId == model.CurrentApprovalNodeId & c.PathId == model.ApprovalPathId
                                               select c.ApprovalType).FirstOrDefault();
            }

            foreach (var item in entity.FAM_VoucherInfoDetails)
            {
                var childmodel = item.ToModel();
                childmodel.ChequeNumberRV = childmodel.ChequeNumber;
                model.VoucherInfoDetailsClient.Add(childmodel);
            }

            PrepareModelEdit(model);
            model.Mode = "Edit";

            

            ChequeNumber = model.VoucherInfoDetailsClient.Select(x => x.ChequeNumber).ToList();

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(VoucherInfoModel model)
        {
            PrepareModelForSave(model);
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    if ((model.VoucherNumber.Contains("RV")) || (model.VoucherNumber.Contains("JV")))
                    {
                        foreach (var item in model.VoucherInfoDetailsClient)
                        {
                            item.ChequeNumber = item.ChequeNumberRV;
                        }
                    }


                    //Recommended Or Approved----
                    if (model.recommenderOrApprover == "Recommender")
                    {
                        model.VoucherStatus = "Recommended";
                    }
                    if (model.recommenderOrApprover == "Approver")
                    {
                        model.VoucherStatus = "Approved";
                        model.ApproveDate = DateTime.UtcNow;
                    }
                    if (model.recommenderOrApprover == "Reject")
                    {
                        model.VoucherStatus = "Rejected";
                        model.ApproveDate = null;
                    }
                    if (model.recommenderOrApprover == null)
                    {
                        model.VoucherStatus = "Draft";
                        model.ApproveDate = null;
                    }
                    var entity = model.ToEntity();
                    var navigationList = new Dictionary<Type, ArrayList>();
                    var childEntities = new ArrayList();
                    model.VoucherInfoDetailsClient.ToList().ForEach(x => x.VoucherId = model.Id);
                    model.VoucherInfoDetailsClient.ToList().ForEach(x => childEntities.Add(x.ToEntity()));
                    navigationList.Add(typeof(FAM_VoucherInfoDetails), childEntities);
                    _famCommonservice.FAMUnit.voucherMaster.Update(entity, navigationList);

                    Cheque_MarkAsUnused(model);

                    Message = ErrorMessages.UpdateSuccessful;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Message = ErrorMessages.UpdateFailed + ": " + ex.Message + " inner exception: " + ex.InnerException ?? ex.InnerException.Message;
#endif
#if RELEASE
                    Message = ErrorMessages.UpdateFailed;
#endif
                }
            }
            else
                Message = string.IsNullOrEmpty(Message) ? ErrorMessages.UpdateFailed : Message;
            return new JsonResult()
            {
                Data = Message
            };
        }

        public ActionResult Delete(int id)
        {
            var entity = _famCommonservice.FAMUnit.voucherMaster.GetByID(id);

            if (entity.VoucherStatus == "Draft")
            {
                var childEntity = entity.FAM_VoucherInfoDetails;

                if (entity.VoucherNumber.Contains("PV"))
                {
                    var existingChequeId = childEntity.Select(x => Convert.ToInt32(x.ChequeNumber)).ToList();

                    var dbChequeNumber = (from cm in _famCommonservice.FAMUnit.ChequeInfoMaster.GetAll()
                                          join cd in _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll() on cm.Id equals cd.ChequeBookId
                                          where existingChequeId.Contains(cd.Id)
                                          select cd).ToList();
                    foreach (var item in dbChequeNumber)
                    {
                        item.ChequeStatus = "Unused";
                        item.ChequeAmount = 0;
                        _famCommonservice.FAMUnit.ChequeInfoDetails.Update(item);
                    }
                }

                try
                {
                    List<Type> allTypes = new List<Type> { typeof(FAM_VoucherInfoDetails) };
                    _famCommonservice.FAMUnit.voucherMaster.Delete(id, allTypes);
                    _famCommonservice.FAMUnit.voucherMaster.SaveChanges();
                    _famCommonservice.FAMUnit.ChequeInfoDetails.SaveChanges();

                    return Json(new
                    {
                        Success = 1,
                        Message = ErrorMessages.DeleteSuccessful
                    }, JsonRequestBehavior.AllowGet);
                }

                catch (Exception)
                {
                    return Json(new
                    {
                        Success = 0,
                        Message = ErrorMessages.DeleteFailed
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    Success = 0,
                    Message = "Recommended Or Approved Voucher cannot be deleted."
                }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region Utils

        private void PrepareModelForSave(VoucherInfoModel model)
        {
            model.FinancialYearId = (from x in _famCommonservice.FAMUnit.FinancialYearInformationRepository.GetAll()
                                     where x.IsActive.Value
                                     select x.Id).FirstOrDefault();

            if (model.ApplicableForClient.Equals("Staff"))
            {
                model.ApplicableFor = 1;
                model.PayeeStaffId = model.PayeeId;
                model.ReceiveFromStaffId = model.ReceiveId;
            }
            else if (model.ApplicableForClient.Equals("Client"))
            {
                model.ApplicableFor = 2;
                model.PayeeClientId = model.PayeeId;
                model.ReceiveFromClientId = model.ReceiveId;
            }
            else
            {
                model.ApplicableFor = 3;
                model.PayeeVendorId = model.PayeeId;
                model.ReceiveFromVendorId = model.ReceiveId;
            }

            if (model.BankOrCash == null)
            {
                model.IsCashOrBank = false;
            }
            else
            {
                model.IsCashOrBank = model.BankOrCash.Equals("Bank") ? false : true;
            }

            int? approvalNodeId = 0;

            if (model.recommenderOrApprover == "Reject")
            {
                var currentNodeOrder = (from c in _famCommonservice.FAMUnit.ApprovalPathDetails.GetAll()
                                        where c.PathId == model.ApprovalPathId && c.NodeEmpId == model.CurrentApprovalNodeId
                                        select c).FirstOrDefault().NodeOrder;

                var approvalNode = (from c in _famCommonservice.FAMUnit.ApprovalPathDetails.GetAll()
                                    where c.PathId == model.ApprovalPathId && c.NodeOrder < currentNodeOrder
                                    orderby c.NodeOrder descending
                                    select c).FirstOrDefault();
                if (approvalNode == null)
                {
                    approvalNodeId = null;
                }
                else
                {
                    approvalNodeId = approvalNode.NodeEmpId;
                }



                model.CurrentApprovalNodeId = approvalNodeId;
            }
            else
            {

                if (model.CurrentApprovalNodeId == null)
                {
                    var approvalNode = (from c in _famCommonservice.FAMUnit.ApprovalPathDetails.GetAll()
                                        where c.PathId == model.ApprovalPathId
                                        orderby c.NodeOrder ascending
                                        select c).FirstOrDefault();
                    if (approvalNode != null)
                    {
                        approvalNodeId = approvalNode.NodeEmpId;
                    }
                    model.CurrentApprovalNodeId = approvalNodeId;
                }
                else
                {
                    if (model.recommenderOrApprover == "Recommender" || model.recommenderOrApprover == "Approver")
                    {
                        var currentNodeOrder = (from c in _famCommonservice.FAMUnit.ApprovalPathDetails.GetAll()
                                                where c.PathId == model.ApprovalPathId && c.NodeEmpId == model.CurrentApprovalNodeId
                                                select c).FirstOrDefault().NodeOrder;

                        var approvalNode = (from c in _famCommonservice.FAMUnit.ApprovalPathDetails.GetAll()
                                            where c.PathId == model.ApprovalPathId && c.NodeOrder > currentNodeOrder
                                            orderby c.NodeOrder ascending
                                            select c).FirstOrDefault();
                        if (approvalNode != null)
                        {
                            approvalNodeId = approvalNode.NodeEmpId;
                        }

                        model.CurrentApprovalNodeId = approvalNodeId;
                    }

                    if (model.VoucherStatus == "Draft")
                    {
                        model.CurrentApprovalNodeId = model.CurrentApprovalNodeId;
                    }
                    //if (model.VoucherStatus == "Approved")
                    //{
                    //    model.CurrentApprovalNodeId = null;
                    //}
                }
            }
        }

        private void PrepareCreateModel(VoucherInfoModel model)
        {
            int totalRows = 0;
            model.VoucherTypeList = Common.PopulateDllList(_prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig("Voucher Type", 0, "", "", "", 0, 100, out totalRows));

            model.PayeeList = Common.PopulateEmployeeDDL(_prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll().ToList());

            model.ReceiveList = model.PayeeList;

            model.ReferenceTypeList = Common.PopulateReferenceType();

            model.PaymentTypeList = Common.PopulateDllList(_prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig("Payment Type", 0, "", "", "", 0, 100, out totalRows));

            model.BankList = Common.PopulateDllList(_prmCommonservice.PRMUnit.BankNameRepository.GetAll().ToList());

            foreach (var vid in model.VoucherInfoDetailsClient)
            {

                var coas = _famCommonservice.GetCOA(true).ToList();
                vid.AccountHeadList = coas.Select(x => new SelectListItem()
                {
                    Text = x.AccountHeadCode,
                    Value = x.Id.ToString()
                }).ToList();

                vid.AccountHeadNameList = coas.Select(x => new SelectListItem()
                {
                    Text = x.AccountHeadName,
                    Value = x.Id.ToString()
                }).ToList();

                //vid.ChequeNumberList = _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll()
                //    .Where(cid => !cid.ChequeStatus.Contains("Used")).ToList().Select(x =>
                //    new SelectListItem()
                //    {
                //        Text = x.ChequeNumber,
                //        Value = x.Id.ToString()
                //    }).ToList();

                //vid.ProjectList = Common.PopulateProjectTitle(_pimCommonservice.PIMUnit.ProjectInformation.GetAll().ToList());
            }
            model.VoucherDate = DateTime.Now;
            model.ApprovalPathList = Common.PopulateApprovalPathDllList(_famCommonservice.FAMUnit.ApprovalPathMaster.GetAll().ToList());
        }

        private void PrepareModelEdit(VoucherInfoModel model)
        {
            int totalRows = 0;
            model.VoucherTypeList = Common.PopulateDllList(_prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig("Voucher Type", 0, "", "", "", 0, 100, out totalRows));

            if (model.ApplicableFor == 1)
            {
                model.ApplicableForClient = "Staff";
                model.PayeeList = Common.PopulateEmployeeDDL(_prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll().ToList());
            }
            else if (model.ApplicableFor == 2)
            {
                model.ApplicableForClient = "Client";
                //model.PayeeList = Common.PopulateClientInfo(_pimCommonservice.PIMUnit.ClientInformation.GetAll().ToList());
            }
            else if (model.ApplicableFor == 3)
            {
                model.ApplicableForClient = "Vendor";
                model.PayeeList = _famCommonservice.FAMUnit.VendorInformationRepository.GetAll().ToList()
                                    .Select(x => new SelectListItem()
                                    {
                                        Text = x.VendorName,
                                        Value = x.Id.ToString()
                                    }).ToList();
            }
            else
            {
                model.ApplicableForClient = "Staff";
                model.PayeeList = Common.PopulateEmployeeDDL(_prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll().ToList());
            }

            model.ReceiveList = model.PayeeList;

            model.ReferenceTypeList = Common.PopulateReferenceType();

            model.PaymentTypeList = Common.PopulateDllList(_prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig("Payment Type", 0, "", "", "", 0, 100, out totalRows));

            model.BankList = Common.PopulateDllList(_prmCommonservice.PRMUnit.BankNameRepository.GetAll().ToList());

            foreach (var vid in model.VoucherInfoDetailsClient)
            {

                var coas = _famCommonservice.GetCOA(true).ToList();
                vid.AccountHeadList = coas.Select(x => new SelectListItem()
                {
                    Text = x.AccountHeadCode,
                    Value = x.Id.ToString()
                }).ToList();

                vid.AccountHeadNameList = coas.Select(x => new SelectListItem()
                {
                    Text = x.AccountHeadName,
                    Value = x.Id.ToString()
                }).ToList();

                //vid.ChequeNumberList = _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll()
                //    .Where(cid => !cid.ChequeStatus.Contains("Used")).ToList().Select(x =>
                //    new SelectListItem()
                //    {
                //        Text = x.ChequeNumber,
                //        Value = x.Id.ToString()
                //    }).ToList();

                //vid.ProjectList = Common.PopulateProjectTitle(_pimCommonservice.PIMUnit.ProjectInformation.GetAll().ToList());
            }

            model.ApprovalPathList = Common.PopulateApprovalPathDllList(_famCommonservice.FAMUnit.ApprovalPathMaster.GetAll().ToList());

            model.BankOrCash = model.IsCashOrBank ? "Cash" : "Bank";

            model.PayeeId = model.PayeeClientId == null ? model.PayeeStaffId == null ? model.PayeeVendorId == null ? null : model.PayeeVendorId : model.PayeeStaffId : model.PayeeClientId;
            model.ReceiveId = model.ReceiveFromClientId == null ? model.ReceiveFromStaffId == null ? model.ReceiveFromVendorId == null ? null : model.ReceiveFromVendorId : model.ReceiveFromStaffId : model.ReceiveFromClientId;
            //model.BankAccountId = model. ;

        }

        private string CheckBusinessRule(VoucherInfoModel model)
        {            
            var financialYear = _famCommonservice.FAMUnit.FinancialYearInformationRepository.GetAll().Where(fi => fi.Id == model.FinancialYearId).FirstOrDefault();

            if (financialYear == null)
            {
                return "Please select Active Financial Year.";
            }
            else //if (financialYear != null)
            {
                if (financialYear.IsClose == true)
                {
                    return "Financial year is closed";
                }

                // Check if the voucher date is not in active financial year
                if ((model.VoucherDate < financialYear.FinancialYearStartDate) || (model.VoucherDate > financialYear.FinancialYearEndDate))
                {
                    return "Voucher date should be in between active financial year (" + financialYear.FinancialYearName + ")";
                }

            }



            foreach (var item in model.VoucherInfoDetailsClient)
            {

                if (item.AccountHeadId == 0)
                {
                    return "Please select Account Head.";
                }

                if (item.ProjectId == 0)
                {
                    return "Please select Project Name.";
                }

                if (!model.VoucherNumber.Contains("JV") && !model.VoucherNumber.Contains("AV"))
                {
                    if (model.IsCashOrBank)//Cash Voucher
                    {
                        var headStatus = "";
                        foreach (var acchead in model.VoucherInfoDetailsClient)
                        {
                            var accHeadTypeCheck = (from c in _famCommonservice.FAMUnit.ChartOfAccount.Fetch()
                                                    where c.Id == acchead.AccountHeadId
                                                    select c.CashBankType).FirstOrDefault();
                            if (accHeadTypeCheck == "Cash")
                            {
                                headStatus = "Ok";
                            }
                        }
                        if (headStatus != "Ok")
                        {
                            return "Cash Voucher should have Cash Account Head.";
                        }
                    
                    }

                    else if (!model.IsCashOrBank)//Bank Voucher
                    {
                        var headStatus = "";
                        foreach (var acchead in model.VoucherInfoDetailsClient)
                        {
                            var accHeadTypeCheck = (from c in _famCommonservice.FAMUnit.ChartOfAccount.Fetch()
                                                    where c.Id == acchead.AccountHeadId
                                                    select c.CashBankType).FirstOrDefault();
                            if (accHeadTypeCheck == "Bank")
                            {
                                headStatus = "Ok";
                            }
                        }
                        //if (headStatus != "Ok")
                        if (headStatus != "Ok" & !model.VoucherNumber.Contains("IV"))
                        {
                            return "Bank Voucher should have Bank Account Head.";
                        }

                    }
                }

                if (model.VoucherNumber.Contains("PV"))
                {
                    var accHeadType = (from c in _famCommonservice.FAMUnit.ChartOfAccount.Fetch()
                                       where c.Id == item.AccountHeadId
                                       select c.CashBankType).FirstOrDefault();
                    if (accHeadType == "Bank")
                    {
                        if (item.ChequeNumber == null || item.ChequeNumber == "")
                        {
                            return "Bank Payment Voucher should have Cheque number.";
                        }
                    }
                    else if (accHeadType == "Cash")
                    {
                        if (item.ChequeNumber != null)
                        {
                            return "Cash Payment Voucher should not have Cheque number.";
                        }
                    }
                    else
                    {
                        if (item.ChequeNumber != null)
                        {
                            return "Only Bank/Cash head should have Cheque number.";
                        }
                    }
                }

                else if (model.VoucherNumber.Contains("RV"))
                {
                    var accHeadType = (from c in _famCommonservice.FAMUnit.ChartOfAccount.Fetch()
                                       where c.Id == item.AccountHeadId
                                       select c.CashBankType).FirstOrDefault();
                    if (accHeadType == "Bank")
                    {
                        if (item.ChequeNumberRV == null || item.ChequeNumberRV == "")
                        {
                            return "Bank Receive Voucher should have Cheque number.";
                        }
                    }
                    else if (accHeadType == "Cash")
                    {
                        if (item.ChequeNumberRV != null)
                        {
                            return "Cash Receive Voucher should not have Cheque number.";
                        }
                    }
                    else
                    {
                        if (item.ChequeNumberRV != null)
                        {
                            return "Only Bank/Cash head should have Cheque number.";
                        }
                    }
                }

            }

            if (model.VoucherNumber.Contains("PV"))
            {
                foreach (var item in model.VoucherInfoDetailsClient)
                {
                    if (item.Debit > 0)
                    {
                        var accHeadType = (from c in _famCommonservice.FAMUnit.ChartOfAccount.Fetch()
                                           where c.Id == item.AccountHeadId
                                           select c.CashBankType).FirstOrDefault();
                        if (accHeadType == "Bank" || accHeadType == "Cash")
                        {
                            return "In Payment Voucher Bank/Cash should be Credit.";
                        }
                    }
                }
            }
            else
            {
                if (model.VoucherNumber.Contains("RV"))
                {
                    foreach (var item in model.VoucherInfoDetailsClient)
                    {
                        if (item.Credit > 0)
                        {
                            var accHeadType = (from c in _famCommonservice.FAMUnit.ChartOfAccount.Fetch()
                                               where c.Id == item.AccountHeadId
                                               select c.CashBankType).FirstOrDefault();
                            if (accHeadType == "Bank" || accHeadType == "Cash")
                            {
                                return "In Receive Voucher Bank/Cash should be Debit.";
                            }
                        }
                    }

                    

                }
            }


            

            return string.Empty;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetVoucherTypeList()
        {
            int totalRecords = 0;
            var list = _famCommonservice.FAMUnit.FunctionRepository.GetAllCommonConfig("Voucher Type", 0, "", "", "", 0, 100, out totalRecords);

            return PartialView("_Select", Common.PopulateDllList(list));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetDivisionList()
        {
            var list = (from x in _prmCommonservice.PRMUnit.DivisionRepository.GetAll()
                        select x
                               ).Distinct().ToList();

            return PartialView("_Select", Common.PopulateDllList(list));
        }

        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult GetProjectList()
        //{
        //    var empList = (from emp in _pimCommonservice.PIMUnit.ProjectInformation.GetAll()
        //                   select emp
        //                       ).Distinct().ToList();

        //    return PartialView("_Select", Common.PopulateProjectTitle(empList));
        //}

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetVoucherStatusList()
        {
            return PartialView("_Select", Common.PopulateVoucherStatus());
        }

        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult GetClientList()
        //{
        //    var list = (from x in _pimCommonservice.PIMUnit.ClientInformation.GetAll()
        //                select x
        //                       ).Distinct().ToList();

        //    return PartialView("_Select", Common.PopulateClientInfo(list));
        //}

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetEmpList(int id)
        {
            var empList = (from appM in _famCommonservice.FAMUnit.ApprovalPathMaster.GetAll()
                           join appD in _famCommonservice.FAMUnit.ApprovalPathDetails.GetAll() on appM.PathId equals appD.PathId
                           join emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on appD.NodeEmpId equals emp.Id
                           where appM.PathId == id
                           select emp
                          ).Distinct().ToList();

            return PartialView("_Select", Common.PopulateEmployeeDDL(empList));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetApplicableForList(string listFor)
        {
            dynamic list = null;

            if (listFor == "Vendor")
            {
                list = _famCommonservice.FAMUnit.VendorInformationRepository.GetAll().ToList()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.VendorName,
                        Value = x.Id.ToString()
                    }).ToList();
                return PartialView("_Select", list);
            }
            //else if (listFor == "Client")
            //{
            //    return PartialView("_Select", Common.PopulateClientInfo(_pimCommonservice.PIMUnit.ClientInformation.GetAll().ToList()));
            //}
            else
                return PartialView("_Select", Common.PopulateEmployeeDDL(_prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll().ToList()));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAccountNumber(int id)
        {
            var list = (from bm in _famCommonservice.FAMUnit.BankMaster.GetAll()
                        join bd in _famCommonservice.FAMUnit.BankDetails.GetAll() on bm.Id equals bd.BankBranchMapId
                        where bm.BankId == id
                        select (new SelectListItem() { Text = bd.BankAccountNo, Value = bd.Id.ToString() })).ToList();

            return PartialView("_Select", list);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetChequeNumber(int id, string mode,int voucherId = 0)
        {
            if (mode != "Edit")
            {
                var list = (from cd in _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll()
                            join cm in _famCommonservice.FAMUnit.ChequeInfoMaster.GetAll() on cd.ChequeBookId equals cm.Id into chequeBook
                            from cheque in chequeBook.DefaultIfEmpty()
                            join vi in _famCommonservice.FAMUnit.voucherMaster.GetAll() on cheque.BankAccountId equals vi.BankAccountId into chequeVoucher
                            from cv in chequeVoucher.DefaultIfEmpty()

                            where cheque.BankAccountId == id && !cd.ChequeStatus.Contains("Used")
                            select (new SelectListItem() { Text = cd.ChequeNumber, Value = cd.Id.ToString() })).ToList();

                return Json(list.DistinctBy(x => x.Value), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var list = (from cd in _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll()
                //            join cm in _famCommonservice.FAMUnit.ChequeInfoMaster.GetAll() on cd.ChequeBookId equals cm.Id into chequeBook
                //            from cheque in chequeBook.DefaultIfEmpty()
                //            join vi in _famCommonservice.FAMUnit.voucherMaster.GetAll() on cheque.BankAccountId equals vi.BankAccountId into chequeVoucher
                //            from cv in chequeVoucher.DefaultIfEmpty()
                //            where ((cheque.BankAccountId == id && !cd.ChequeStatus.Contains("Used")) || (cv.Id==voucherId)) 
                //                select (new SelectListItem() { Text = cd.ChequeNumber, Value = cd.Id.ToString() })).ToList();

                var voucherChequeList = (from c in _famCommonservice.FAMUnit.voucherDetails.GetAll().Where(v => v.VoucherId == voucherId && v.ChequeNumber != null)
                                         join cd in _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll() on Convert.ToInt32(c.ChequeNumber) equals cd.Id into voucherCheque
                                         from vCheque in voucherCheque.DefaultIfEmpty()
                                         select (new SelectListItem() { Text = vCheque.ChequeNumber, Value = vCheque.Id.ToString() })).ToList();

                var list = (from cd in _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll()
                            join cm in _famCommonservice.FAMUnit.ChequeInfoMaster.GetAll() on cd.ChequeBookId equals cm.Id into chequeBook
                            from cheque in chequeBook.DefaultIfEmpty()
                            where (cheque.BankAccountId == id && !cd.ChequeStatus.Contains("Used"))
                            select (new SelectListItem() { Text = cd.ChequeNumber, Value = cd.Id.ToString() })).ToList();

                foreach (var item in voucherChequeList)
                {
                    list.Add(item);
                }

                var finalList = (from c in list
                                 select (new SelectListItem() { Text = c.Text, Value = c.Value })).ToList();
 
                return Json(finalList.DistinctBy(x => x.Value), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPayeeAddress(string type)
        {
            var collection = type.Split(',');
            var id = Convert.ToInt32(collection[1]);
            var af = collection[0];

            if (af.Contains("Staff"))
                return new JsonResult() { Data = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(id).PRM_EmpPersonalInfo == null ? "" : _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetByID(id).PRM_EmpPersonalInfo.PresentAddress1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //else if (af.Contains("Client"))
            //    return new JsonResult() { Data = _pimCommonservice.PIMUnit.ClientInformation.GetByID(id).ClientAddress ?? "", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            else
                return new JsonResult() { Data = _famCommonservice.FAMUnit.VendorInformationRepository.GetByID(id).Address ?? "", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetVoucherNumber(string voucherType)
        {
            //var financialYear = (from x in _famCommonservice.FAMUnit.FinancialYearInformationRepository.GetAll()
            //                     where x.IsActive.Value
            //                     select x.FinancialYearName).FirstOrDefault();

            var financialYearVoucherFormat = (from x in _famCommonservice.FAMUnit.FinancialYearInformationRepository.GetAll()
                                              where x.IsActive.Value
                                              select x.FinancialYearVoucherFormat).FirstOrDefault();

            var voucherFormat = voucherType + financialYearVoucherFormat;

            var voucherNumber = (from vid in _famCommonservice.FAMUnit.voucherMaster.GetAll()
                                 where vid.VoucherNumber.Contains(voucherFormat)
                                 orderby vid.VoucherNumber
                                 select vid.VoucherNumber).LastOrDefault();

            if (voucherNumber != null)
                voucherFormat = voucherFormat + "#" + (Convert.ToInt32(voucherNumber.Split('#')[1]) + 1).ToString("00000");
            else
                voucherFormat = voucherFormat + "#" + "00001";

            return Json(voucherFormat, JsonRequestBehavior.AllowGet);
        }

        public String GetVoucherNumberReport(string voucherType)
        {
            //var financialYear = (from x in _famCommonservice.FAMUnit.FinancialYearInformationRepository.GetAll()
            //                     where x.IsActive.Value
            //                     select x.FinancialYearName).FirstOrDefault();

            var financialYearVoucherFormat = (from x in _famCommonservice.FAMUnit.FinancialYearInformationRepository.GetAll()
                                              where x.IsActive.Value
                                              select x.FinancialYearVoucherFormat).FirstOrDefault();

            var voucherFormat = voucherType + financialYearVoucherFormat;

            var voucherNumber = (from vid in _famCommonservice.FAMUnit.voucherMaster.GetAll()
                                 where vid.VoucherNumber.Contains(voucherFormat)
                                 orderby vid.VoucherNumber
                                 select vid.VoucherNumber).LastOrDefault();

            if (voucherNumber != null)
                voucherFormat = voucherFormat + "#" + (Convert.ToInt32(voucherNumber.Split('#')[1]) + 1).ToString("00000");
            else
                voucherFormat = voucherFormat + "#" + "00001";

            return voucherFormat;
        }

        public JsonResult GetApproverComments(int id)
        {

            var approvalComments = (from x in _famCommonservice.FAMUnit.ApprovalComments.GetAll()
                                    where x.VoucherId == id
                                    select x.ApprovalComment).ToList();

            string appComment = "";
            foreach (var item in approvalComments)
            {
                appComment = appComment + item + Environment.NewLine;
            }



            return Json(appComment, JsonRequestBehavior.AllowGet);
        }

        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult GetAccountHeadType(int accId)
        //{

        //    var accountHeadType = (from x in _famCommonservice.FAMUnit.ChartOfAccount.GetAll()
        //                           where x.Id == accId
        //                           select x.CashBankType).FirstOrDefault();

        //    return Json(accountHeadType, JsonRequestBehavior.AllowGet);
        //}


        public JsonResult GetAccountHeadBalance(int headId)
        {
            int companyId = 1;
            string[] retVal = _famCommonservice.CurrentBalanceOfAccountHead(companyId, headId);
            retVal = retVal[0].Split('|');

            return Json(new { CurrentBalance = retVal[0], BalanceType = retVal[1], AccountHeadName = retVal[2] }, JsonRequestBehavior.AllowGet);


        }

        private void Cheque_MarkAsUsed(VoucherInfoModel model)
        {
            if (model.VoucherNumber.Contains("PV"))
            {
                var currentChequeId = model.VoucherInfoDetailsClient.Select(x => Convert.ToInt32(x.ChequeNumber)).ToList();

                var dbChequeNumber = (from cm in _famCommonservice.FAMUnit.ChequeInfoMaster.GetAll()
                                      join cd in _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll() on cm.Id equals cd.ChequeBookId
                                      where cm.BankAccountId == model.BankAccountId && currentChequeId.Contains(cd.Id)
                                      select cd).ToList();
                dbChequeNumber.ForEach(x => x.ChequeStatus = "Used");

                //dbChequeNumber.ForEach(x=> x.ChequeAmount = model.VoucherInfoDetailsClient.Where(vi=> vi.ChequeNumber == )
                foreach (var ie in dbChequeNumber)
                {
                    foreach (var imod in model.VoucherInfoDetailsClient)
                    {
                        if (ie.Id.ToString() == imod.ChequeNumber)
                        {
                            // PV voucher will contain only Credit value for Cheque Number
                            ie.ChequeAmount = imod.Credit;
                        }
                    }
                }

                foreach (var item in dbChequeNumber)
                {
                    _famCommonservice.FAMUnit.ChequeInfoDetails.Update(item);
                }
            }
            _famCommonservice.FAMUnit.voucherMaster.SaveChanges();
            _famCommonservice.FAMUnit.ChequeInfoDetails.SaveChanges();

        }

        private void Cheque_MarkAsUnused(VoucherInfoModel model)
        {
            if (model.VoucherNumber.Contains("PV"))
            {
                var currentChequeIds = model.VoucherInfoDetailsClient.Select(x => Convert.ToInt32(x.ChequeNumber));
                var existingChequeIds = (from vim in _famCommonservice.FAMUnit.voucherMaster.GetAll()
                                         join vid in _famCommonservice.FAMUnit.voucherDetails.GetAll() on vim.Id equals vid.VoucherId
                                         where vim.Id == model.Id
                                         select vid.ChequeNumber).ToList().Select(x => Convert.ToInt32(x));

                var mergedChequeIds = currentChequeIds.Union(existingChequeIds).Distinct().ToList();

                var dbChequeNumber = (from cm in _famCommonservice.FAMUnit.ChequeInfoMaster.GetAll()
                                      join cd in _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll() on cm.Id equals cd.ChequeBookId
                                      where cm.BankAccountId == model.BankAccountId
                                      select cd).ToList();



                foreach (var item in mergedChequeIds)
                {
                    var isDb = existingChequeIds.Contains(item);
                    var isCur = currentChequeIds.Contains(item);

                    if (isDb && !isCur)
                    {
                        //delete
                        var foundItem = dbChequeNumber.Find(x => x.Id == item);
                        foundItem.ChequeStatus = "Unused";
                        foundItem.ChequeAmount = 0;

                        //foreach (var dbc in dbChequeNumber)
                        //{
                        //    foreach (var iMod in model.VoucherInfoDetailsClient)
                        //    {
                        //        if (dbc.Id.ToString() == iMod.ChequeNumber)
                        //        {
                        //            dbc.ChequeAmount = 0;
                        //        }
                        //    }
                        //}
                        _famCommonservice.FAMUnit.ChequeInfoDetails.Update(foundItem);
                    }
                    else if (!isDb && isCur)
                    {
                        //insert
                        var foundItem = dbChequeNumber.Find(x => x.Id == item);
                        foundItem.ChequeStatus = "Used";


                        foreach (var imod in model.VoucherInfoDetailsClient)
                        {
                            if (item.ToString() == imod.ChequeNumber)
                            {
                                foundItem.ChequeAmount = imod.Credit;
                            }
                        }

                        _famCommonservice.FAMUnit.ChequeInfoDetails.Update(foundItem);
                    }
                    else if (isDb && isCur)
                    {
                        //Update Cheque Amount
                        var foundItem = dbChequeNumber.Find(x => x.Id == item);
                        if (foundItem != null)
                        {
                            foundItem.ChequeStatus = "Used";
                            foreach (var imod in model.VoucherInfoDetailsClient)
                            {
                                if (item.ToString() == imod.ChequeNumber)
                                {
                                    foundItem.ChequeAmount = imod.Credit;
                                }
                            }
                            _famCommonservice.FAMUnit.ChequeInfoDetails.Update(foundItem);
                        }
                    }


                }
            }
            _famCommonservice.FAMUnit.voucherMaster.SaveChanges();
            _famCommonservice.FAMUnit.ChequeInfoDetails.SaveChanges();
        }

        public ActionResult LoadVoucherInfo(int voucherId)
        {
            var voucherInfo = _famCommonservice.FAMUnit.voucherMaster.GetByID(voucherId);
            var voucherDetailInfo = _famCommonservice.FAMUnit.voucherDetails.GetAll().Where(c => c.Debit > 0 && c.VoucherId == voucherId).ToList();

            //Finds the realized vouchers that has used the invoice voucher as reference
            var realizedvoucherInfo = _famCommonservice.FAMUnit.voucherMaster.GetAll().Where(c=> c.InvoiceVoucherReferenceId == voucherId).ToList();

            //Calculate total amount of all the realized vouchers
            decimal realizedVoucherTotalAmount = 0;
            if(realizedvoucherInfo.Count > 0)
            {
                
                foreach (var item in realizedvoucherInfo)
	            {
		            realizedVoucherTotalAmount = realizedVoucherTotalAmount + _famCommonservice.FAMUnit.voucherDetails.GetAll().Where(c => c.Debit > 0 && c.VoucherId == item.Id).FirstOrDefault().Debit;
	            }
            }
            return Json(
                new
                {
                    InvoiceVoucherReferenceId = voucherInfo.Id,
                    InvoiceVoucherReferenceNumber = voucherInfo.VoucherNumber,
                    InvoiceVoucherAmount = voucherDetailInfo.FirstOrDefault().Debit,
                    RealizationVoucherTotal = realizedVoucherTotalAmount
                }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult LoadVoucherInfoEdit(int voucherId, int currentVoucherId)
        {
            var voucherInfo = _famCommonservice.FAMUnit.voucherMaster.GetByID(voucherId);
            var voucherDetailInfo = _famCommonservice.FAMUnit.voucherDetails.GetAll().Where(c => c.Debit > 0 && c.VoucherId == voucherId).ToList();

            var currentvoucherDetailInfo = _famCommonservice.FAMUnit.voucherDetails.GetAll().Where(c => c.Debit > 0 && c.VoucherId == currentVoucherId).ToList();

            //Finds the realized vouchers that has used the invoice voucher as reference
            var realizedvoucherInfo = _famCommonservice.FAMUnit.voucherMaster.GetAll().Where(c => c.InvoiceVoucherReferenceId == voucherId).ToList();

            //Calculate total amount of all the realized vouchers
            decimal realizedVoucherTotalAmount = 0;
            if (realizedvoucherInfo.Count > 0)
            {

                foreach (var item in realizedvoucherInfo)
                {
                    realizedVoucherTotalAmount = realizedVoucherTotalAmount + _famCommonservice.FAMUnit.voucherDetails.GetAll().Where(c => c.Debit > 0 && c.VoucherId == item.Id).FirstOrDefault().Debit;
                }
            }
            return Json(
                new
                {
                    InvoiceVoucherReferenceId = voucherInfo.Id,
                    InvoiceVoucherReferenceNumber = voucherInfo.VoucherNumber,
                    InvoiceVoucherAmount = voucherDetailInfo.FirstOrDefault().Debit,
                    RealizationVoucherTotal = realizedVoucherTotalAmount - currentvoucherDetailInfo.FirstOrDefault().Debit
                }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}