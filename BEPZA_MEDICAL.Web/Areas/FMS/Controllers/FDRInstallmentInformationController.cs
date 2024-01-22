using BEPZA_MEDICAL.DAL.FMS;
using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class FDRInstallmentInformationController : BaseController
    {
        #region Fields
        private readonly FMSCommonService _fmsCommonService;
        private readonly FMS_ExecuteFunctions _fmsfunction;
        private readonly PGMCommonService _pgmCommonservice;
        private readonly ERP_BEPZAFMSEntities _fmsContext;
        #endregion

        #region Ctor
        public FDRInstallmentInformationController(FMSCommonService fmsCommonService, FMS_ExecuteFunctions fmsfunction, PGMCommonService pgmCommonservice, ERP_BEPZAFMSEntities fmsContext)
        {
            this._fmsCommonService = fmsCommonService;
            this._fmsfunction = fmsfunction;
            this._pgmCommonservice = pgmCommonservice;
            this._fmsContext = fmsContext;
        }
        #endregion

        // GET: FMS/FDRInstallmentInformation
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, FDRInstallmentInformationViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from fdt in _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.GetAll()
                        join fdi in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll() on fdt.FixedDepositInfoId equals fdi.Id
                        where(viewModel.FixedDepositInfoId==0 || viewModel.FixedDepositInfoId==fdt.FixedDepositInfoId)
                        &&(viewModel.FDRNo=="" || viewModel.FDRNo==null || fdi.FDRNumber.Contains(viewModel.FDRNo))
                        &&(fdt.ZoneInfoId==LoggedUserZoneInfoId)
                        && (fdi.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                        select new FDRInstallmentInformationViewModel()
                        {
                            Id = fdt.Id,
                            Date=fdt.Date,
                            FDRAmount=fdt.FDRAmount,
                            FixedDepositInfoId=fdi.Id,
                            FDRNo=fdi.FDRNumber
                        }).ToList();

            if (request.Searching)
            {
                if ((viewModel.DateFrom != null && viewModel.DateFrom != DateTime.MinValue) && (viewModel.DateTo != null && viewModel.DateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date >= viewModel.DateFrom && d.Date <= viewModel.DateTo).ToList();
                }
                else if ((viewModel.DateFrom != null && viewModel.DateFrom != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date >= viewModel.DateFrom).ToList();
                }
                else if ((viewModel.DateTo != null && viewModel.DateTo != DateTime.MinValue))
                {
                    list = list.Where(d => d.Date <= viewModel.DateTo).ToList();
                }
            }


            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Date")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Date).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Date).ToList();
                }
            }

            if (request.SortingName == "FDRAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FDRAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FDRAmount).ToList();
                }
            }
            if (request.SortingName == "FDRNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FDRNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FDRNo).ToList();
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
                    d.Date.ToString(DateAndTime.GlobalDateFormat),
                    d.DateFrom,
                    d.DateTo,
                    d.FDRAmount,
                    //d.FixedDepositInfoId,
                    d.FDRNo
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        //public ActionResult FDRNoForView()
        //{
        //    var itemList = Common.PopulateFDRNoDDL(_fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll());
        //    return PartialView("Select", itemList);
        //}

        public ActionResult Create()
        {
            FDRInstallmentInformationViewModel model = new FDRInstallmentInformationViewModel();
            model.Date = DateTime.Now;
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Attachment")] FDRInstallmentInformationViewModel model)
        {
            string errorList = string.Empty;
            errorList = BusinessLogicValidation(model);
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    model.IUser = User.Identity.Name;
                    model.IDate = DateTime.Now;
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();

                    HttpFileCollectionBase files = Request.Files;
                    foreach (string fileTagName in files)
                    {

                        HttpPostedFileBase file = Request.Files[fileTagName];
                        if (file.ContentLength > 0)
                        {
                            // Due to the limit of the max for a int type, the largest file can be
                            // uploaded is 2147483647, which is very large anyway.
                            int size = file.ContentLength;
                            string name = file.FileName;
                            int position = name.LastIndexOf("\\");
                            name = name.Substring(position + 1);
                            string contentType = file.ContentType;
                            byte[] fileData = new byte[size];
                            file.InputStream.Read(fileData, 0, size);
                            entity.FileName = name;
                            entity.Attachment = fileData;
                        }

                    }

                    _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.Add(entity);
                    _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            else
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = errorList;
            }
            populateDropdown(model);
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var entity = _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.GetByID(id);
            var model = entity.ToModel();
            DownloadDoc(model);
            model.strMode = "Edit";

            var fdrInfo = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetByID(model.FixedDepositInfoId);
            model.BankInfoId = fdrInfo.BankInfoId;
            model.BankInfoBranchDetailId = fdrInfo.BankInfoBranchDetailId;

            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Attachment")] FDRInstallmentInformationViewModel model)
        {
            string errorList = string.Empty;
            var attachment = Request.Files["attachment"];

            if (ModelState.IsValid)
            {
                // Set preious attachment if exist
                var obj = _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.GetByID(model.Id);
                model.Attachment = obj.Attachment == null ? null : obj.Attachment;
                //

                try
                {
                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();

                    HttpFileCollectionBase files = Request.Files;
                    foreach (string fileTagName in files)
                    {

                        HttpPostedFileBase file = Request.Files[fileTagName];
                        if (file.ContentLength > 0)
                        {
                            // Due to the limit of the max for a int type, the largest file can be
                            // uploaded is 2147483647, which is very large anyway.
                            int size = file.ContentLength;
                            string name = file.FileName;
                            int position = name.LastIndexOf("\\");
                            name = name.Substring(position + 1);
                            string contentType = file.ContentType;
                            byte[] fileData = new byte[size];
                            file.InputStream.Read(fileData, 0, size);
                            entity.FileName = name;
                            entity.Attachment = fileData;
                        }
                    }

                    _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.Update(entity);
                    _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
                }
                catch
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            else
            {
                model.errClass = "failed";
                model.IsError = 1;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
            }

            populateDropdown(model);
            DownloadDoc(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.GetByID(id);
            try
            {
                _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.Delete(id);
                _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.SaveChanges();
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

        public string BusinessLogicValidation(FDRInstallmentInformationViewModel model)
        {
            string errMsg = string.Empty;

            var list = _fmsCommonService.FMSUnit.FDRInstallmentInfoRepository.GetAll().Where(x => x.FixedDepositInfoId == model.FixedDepositInfoId && x.Date.Month==model.Date.Month && x.Date.Year==model.Date.Year && x.ZoneInfoId==LoggedUserZoneInfoId).ToList();
            if (list.Count > 0)
            {
                errMsg = "Sorry! This Installment is not valid for this month or for this FDR";
            }
            return errMsg;
        }

        public void populateDropdown(FDRInstallmentInformationViewModel model)
        {
            dynamic ddlList;
            HashSet<int> ClsFDRId = new HashSet<int>(_fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll().Select(x=>x.FixedDepositInfoId));

            ddlList = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                      .Where(q => q.ZoneInfoId == LoggedUserZoneInfoId && q.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]))
                      .Where(s => !ClsFDRId.Contains(s.Id))
                      .OrderByDescending(x => x.FDRDate)
                      .DistinctBy(x => x.FDRNumber);

            model.FDRNoList = Common.PopulateFDRNoDDL(ddlList);

            model.ProfitRecvList = _pgmCommonservice.PGMUnit.AccChartOfAccountRepository.GetAll().OrderBy(x => x.accountName)
                                    .Where(s => s.accountGroup == "Income" && s.isControlhead == 0)
                                    .Select(y => new SelectListItem()
                                    {
                                        Text = y.accountName,
                                        Value = y.id.ToString()
                                    }).ToList();

            ddlList = _fmsfunction.fnGetBankChequeNoList(LoggedUserZoneInfoId, model.ProfitRecvId == null ? 0 : Convert.ToInt32(model.ProfitRecvId));
            model.ChequeList = Common.PopulateChequeNoddl(ddlList);

            #region bank ddl
            ddlList = _fmsCommonService.FMSUnit.BankInfoRepository.GetAll().OrderBy(x => x.BankName).ToList();
            model.BankInfoList = Common.PopulateFDRBankDDL(ddlList);
            #endregion

            #region branch ddl
            ddlList = _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().OrderBy(x => x.BranchName).ToList();
            model.BankInfoBranchDetailList = Common.PopulateFDRBankBranchDDL(ddlList);
            #endregion

        }

        [NoCache]
        public JsonResult GetInstallmentInfo(int id, DateTime date)
        {
            var obj = (from fdi in _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                       join fdiSdl in _fmsCommonService.FMSUnit.FixedDepositInfoInstallmentScheduleRepository.GetAll() on fdi.Id equals fdiSdl.FixedDepositInfoId
                       where (fdi.Id == id)
                       select new FDRInstallmentInformationViewModel()
                           {
                               ProfitRecvId= fdi.ProfitRecvId == null ? 0 : (int)fdi.ProfitRecvId,
                               FDRAmount = fdi.FDRAmount,
                               InterestRate = fdi.InterestRate,
                               InstallmentAmount=fdiSdl.InsAmount,
                               TAXAmount=fdiSdl.Tax,
                               BankCharge=fdiSdl.BankCharge,
                               Profit=fdiSdl.Profit,
                               ChequeId = fdi.ChequeId,
                           }).FirstOrDefault();


            return Json(new
            {
                ProfitRecvId = obj.ProfitRecvId,
                FDRAmount = obj.FDRAmount,
                InterestRate = obj.InterestRate,
                TAXAmount = obj.TAXAmount,
                BankCharge = obj.BankCharge,
                InstallmentAmount = obj.InstallmentAmount,
                Profit = obj.Profit,
                ChequeId = obj.ChequeId
            }, JsonRequestBehavior.AllowGet);

        }

        [NoCache]
        public JsonResult GetBankCheckNo(int id)
        {
            var tempList = Common.PopulateChequeNoddl(_fmsfunction.fnGetBankChequeNoList(LoggedUserZoneInfoId,id));

            var list = tempList.Select(x => new { Id = x.Value, Name = x.Text }).OrderBy(x => x.Name).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetBankBranch(int id)
        {
            var list = (from branch in _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().Where(q => q.BankInfoId == id)
                        select new
                        {
                            branchId = branch.Id,
                            branchName = branch.BranchName
                        }).OrderBy(q => q.branchName).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        [NoCache]
        public JsonResult GetFDRNo(int id)
        {
          HashSet<int> ClsFDRId = new HashSet<int>(_fmsCommonService.FMSUnit.FDRClosingInfoRepository.GetAll().Select(x => x.FixedDepositInfoId));
          var  ddlList = _fmsCommonService.FMSUnit.FixedDepositInfoRepository.GetAll()
                         .Where(q => q.ZoneInfoId == LoggedUserZoneInfoId && q.FDRTypeId == Convert.ToInt32(Session["FDRTypeId"]) && q.BankInfoBranchDetailId == id).OrderByDescending(x => x.FDRDate)
                         .Where(s => !ClsFDRId.Contains(s.Id))
                         .DistinctBy(x => x.FDRNumber);

          var list = Common.PopulateFDRNoDDL(ddlList);

          return Json(list, JsonRequestBehavior.AllowGet);
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

            var obj = _fmsContext.FMS_uspVoucherPostingForInterestReceive(id).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Redirect(url);
        }

        #region Attachment

        private int Upload(FDRInstallmentInformationViewModel model)
        {
            if (model.File == null)
                return 0;

            try
            {
                var uploadFile = model.File;

                byte[] data;
                using (Stream inputStream = uploadFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
                model.Attachment = data;
                model.FileName = uploadFile.FileName;
                model.IsError = 0;

            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = "Upload File Error!";
            }

            return model.IsError;
        }

        public void DownloadDoc(FDRInstallmentInformationViewModel model)
        {
            byte[] document = model.Attachment;
            if (document != null)
            {
                string strFilename = Url.Content("~/Content/" + User.Identity.Name + model.FileName);
                byte[] doc = document;
                WriteToFile(Server.MapPath(strFilename), ref doc);

                model.FilePath = strFilename;
            }
        }

        private void WriteToFile(string strPath, ref byte[] Buffer)
        {
            FileStream newFile = null;

            try
            {
                newFile = new FileStream(strPath, FileMode.Create);

                newFile.Write(Buffer, 0, Buffer.Length);
                newFile.Close();
            }
            catch (Exception)
            {
                if (newFile != null) newFile.Close();
            }
        }

        #endregion

    }
}