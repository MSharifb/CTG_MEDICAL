using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class SendSMSController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        private readonly EmployeeService _empService;
        #endregion

        #region Ctor

        public SendSMSController(PRMCommonSevice pPRMCommonService, EmployeeService empService)
        {
            this._prmCommonService = pPRMCommonService;
            this._empService = empService;

        }

        #endregion

        // GET: PRM/SendSMS
        public ActionResult Index()
        {
            var model = InitializeModel();
            return View(model);
        }



        private SendSMSViewModel InitializeModel()
        {
            SendSMSViewModel model = new SendSMSViewModel();
            model.MessageDate = DateTime.Now;
            model.ZoneListByUser = Common.PopulateDdlZoneList(MyAppSession.SelectedZoneList);
            model.ZoneInfoIdByUser = LoggedUserZoneInfoId;
            var list = _prmCommonService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            model.DepartmentList = Common.PopulateDllList(list);

            var list2 = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Select(q => new { Id = q.Id + "-" + q.MobileNo, Name = q.FullName + " [" + q.EmpID + "-" + q.MobileNo + " ]" }).ToList();
            model.AllEmployeeList = Common.PopulateDllList(list2);
            return model;
        }


        [NoCache]
        public ActionResult SendSMS([Bind(Exclude = "Attachment")] SendSMSViewModel viewModel)
        {
            String sid = "BEPZA"; String user = "BEPZA"; String pass = "$20>PyQ33";
            String URI = "http://sms.sslwireless.com/pushapi/dynamic/server.php";

            bool result = true;
            string errMsg = string.Empty;
            string SMSSenderAndText = string.Empty;
            errMsg = "Message has been sent successfully.";
            try
            {
                if (viewModel.MessageOption == "S" && viewModel.SelectedEmployee.Count() > 0)
                {
                    int i = 0;
                    foreach (string _EmployeeId in viewModel.SelectedEmployee)
                    {
                        string mobile = _EmployeeId.Split('-')[1];
                        if (!string.IsNullOrEmpty(mobile) && !mobile.StartsWith("88"))
                        {
                            mobile = "88" + mobile;
                            var msgbody = this.convertBanglatoUnicode(viewModel.Message.Trim());

                            // insert

                            SMSSenderAndText += string.Format("&sms[{0}][0]={1}&sms[{0}][1]={2}&sms[{0}][2]=BEPZA{3}", i, mobile, System.Web.HttpUtility.UrlEncode(msgbody), DateTime.Now.Ticks.ToString());
                            //SMSSenderAndText= SMSSenderAndText.Select(t => $"U+{Convert.ToUInt16(t):X4} ").ToList().ToString();
                            //save to database
                            SendSMSViewModel obj = new SendSMSViewModel();
                            obj.EmployeeId = Convert.ToInt32(_EmployeeId.Split('-')[0]);
                            obj.Message = viewModel.Message;
                            obj.MessageDate = DateTime.Now;
                            _prmCommonService.PRMUnit.EmpSmsHistoryRepository.Add(obj.ToEntity());

                        }
                        i++;
                    }
                    _prmCommonService.PRMUnit.EmpSmsHistoryRepository.SaveChanges();

                    String myParameters = "user=" + user + "&pass=" + pass + SMSSenderAndText + "&sid=" + sid;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string HtmlResult = wc.UploadString(URI, myParameters);
                    }
                }

                else if (viewModel.MessageOption == "A")
                {

                    var empList = _empService.PRMUnit.EmploymentInfoRepository.GetAll().Where(q => q.MobileNo != string.Empty && q.MobileNo != null).Select(q => new { Id = q.Id, MobileNo = q.MobileNo }).ToList();

                    int i = 0;
                    foreach (var item in empList)
                    {
                        string mobile = item.MobileNo;
                        if (!string.IsNullOrEmpty(mobile) && !mobile.StartsWith("88"))
                        {
                            mobile = "88" + mobile;
                            var msgbody = this.convertBanglatoUnicode(viewModel.Message.Trim());
                            // insert
                            SMSSenderAndText += string.Format("&sms[{0}][0]={1}&sms[{0}][1]={2}&sms[{0}][2]=BEPZA{3}", i, mobile, System.Web.HttpUtility.UrlEncode(msgbody), DateTime.Now.Ticks.ToString());

                            //save to database
                            SendSMSViewModel obj = new SendSMSViewModel();
                            obj.EmployeeId = Convert.ToInt32(item.Id);
                            obj.Message = viewModel.Message;
                            obj.MessageDate = DateTime.Now;
                            _prmCommonService.PRMUnit.EmpSmsHistoryRepository.Add(obj.ToEntity());

                        }
                        i++;
                    }
                    _prmCommonService.PRMUnit.EmpSmsHistoryRepository.SaveChanges();

                    String myParameters = "user=" + user + "&pass=" + pass + SMSSenderAndText + "&sid=" + sid;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string HtmlResult = wc.UploadString(URI, myParameters);
                    }
                }
                else if (viewModel.MessageOption == "Z")
                {

                    var empListZoneWise = (from empInfo in _empService.PRMUnit.EmploymentInfoRepository.GetAll()
                                           where (!string.IsNullOrEmpty(empInfo.MobileNo))
                                                 && (empInfo.ZoneInfoId == viewModel.ZoneInfoIdByUser)
                                                 && (empInfo.DivisionId == viewModel.DepartmentId || viewModel.DepartmentId == null)
                                           select new
                                           {
                                               Id = empInfo.Id,
                                               MobileNo = empInfo.MobileNo,
                                           }).ToList();

                    int i = 0;
                    foreach (var item in empListZoneWise)
                    {
                        string mobile = item.MobileNo;

                        if (!string.IsNullOrEmpty(mobile) && !mobile.StartsWith("88"))
                        {
                            mobile = "88" + mobile;
                            var msgbody = this.convertBanglatoUnicode(viewModel.Message.Trim());

                            // insert
                            SMSSenderAndText += string.Format("&sms[{0}][0]={1}&sms[{0}][1]={2}&sms[{0}][2]=BEPZA{3}", i, mobile, System.Web.HttpUtility.UrlEncode(msgbody), DateTime.Now.Ticks.ToString());

                            //save to database
                            SendSMSViewModel obj = new SendSMSViewModel();
                            obj.EmployeeId = Convert.ToInt32(item.Id);
                            obj.Message = viewModel.Message;
                            obj.MessageDate = DateTime.Now;
                            _prmCommonService.PRMUnit.EmpSmsHistoryRepository.Add(obj.ToEntity());

                        }
                        i++;
                    }
                    _prmCommonService.PRMUnit.EmpSmsHistoryRepository.SaveChanges();

                    String myParameters = "user=" + user + "&pass=" + pass + SMSSenderAndText + "&sid=" + sid;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string HtmlResult = wc.UploadString(URI, myParameters);
                    }

                }
                else
                {
                    //Exnter Member
                    HttpPostedFileBase file = Request.Files["attachment"];
                    // var attachment = Request.Files["attachment"];
                    if (Request.Files["Attachment"].ContentLength > 0)
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                        //using (var package = new ExcelPackage())
                        //{
                        //    //var worksheet = package.Workbook.Worksheets.Add("Worksheet Name");

                        //    //worksheet.Cells["A1"].LoadFromCollection(data);

                        //    var stream = new MemoryStream(package.GetAsByteArray());
                        //}

                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            //var row = workSheet.Cells[noOfRow, 1, noOfRow, workSheet.Dimension.End.Column];

                            int i = 0;

                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {

                                string name = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                                string mobile = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();


                                if (!string.IsNullOrEmpty(mobile) && !mobile.StartsWith("88"))
                                {
                                    mobile = "88" + mobile;
                                    var msgbody = this.convertBanglatoUnicode(viewModel.Message.Trim());

                                    // insert
                                    SMSSenderAndText += string.Format("&sms[{0}][0]={1}&sms[{0}][1]={2}&sms[{0}][2]=BEPZA{3}", i, mobile, System.Web.HttpUtility.UrlEncode(msgbody), DateTime.Now.Ticks.ToString());

                                    //save to database
                                    SendSMSViewModel obj = new SendSMSViewModel();
                                    obj.Message = viewModel.Message;
                                    obj.MessageDate = DateTime.Now;
                                    obj.IsExternal = true;
                                    obj.ExternalName = name;
                                    _prmCommonService.PRMUnit.EmpSmsHistoryRepository.Add(obj.ToEntity());

                                }
                                i++;
                            }
                            _prmCommonService.PRMUnit.EmpSmsHistoryRepository.SaveChanges();

                            String myParameters = "user=" + user + "&pass=" + pass + SMSSenderAndText + "&sid=" + sid;
                            using (WebClient wc = new WebClient())
                            {
                                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                                string HtmlResult = wc.UploadString(URI, myParameters);
                            }

                        }
                    }


                }

            }
            catch (Exception ex)
            {
                result = false;
                errMsg = "Message sending failed. Please try again";
            }
            return Json(new
            {
                Success = result,
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);

        }

        public string convertBanglatoUnicode(string banglaText)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in banglaText)
            {
                sb.AppendFormat("{1:x4}", c, (int)c);
            }
            string unicode = sb.ToString().ToUpper();
            return unicode;
        }

        [NoCache]
        public JsonResult GetDepartment(int zoneId)
        {
            var list = _prmCommonService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == zoneId).OrderBy(q => q.Name).ToList();
            return Json(
               new
               {
                   items = list.Select(x => new { Id = x.Id, Name = x.Name })
               },
               JsonRequestBehavior.AllowGet
           );
        }
    }
}