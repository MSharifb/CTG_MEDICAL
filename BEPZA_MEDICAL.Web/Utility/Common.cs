using BEPZA_MEDICAL.DAL.PGM;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.AMS;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.SecurityService;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace BEPZA_MEDICAL.Web.Utility
{
    public class Common
    {
        private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");
        private static SmtpClient _emailClient;

        private static SmtpClient ConfigureClient(string host, int port, string networkUser, string password)
        {
            var client = new SmtpClient();
            client.Host = host; // "smtp.gmail.com";
            client.Port = port;
            //client.EnableSsl = true; //should be enabled for ssl        
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(networkUser, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            return client;
        }

        #region Mail Notification Configuration-----------------------------------

        public static void SendMail(List<string> reciepientList, string[] copyList, string subject, string emailBody, EmailTemplateConfigDataViewModel model)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(model.FromEmailAddress, model.FromName);

            mailMessage.To.Add(reciepientList[0]);
            for (int i = 0; i < reciepientList.Count; i++)
            {
                if (i != 0) mailMessage.CC.Add(reciepientList[i]);
            }
            for (int j = 0; j < copyList.Count(); j++)
            {
                mailMessage.CC.Add(copyList[j]);
            }
            mailMessage.Priority = MailPriority.High;
            mailMessage.Subject = subject;
            mailMessage.Body = emailBody;
            mailMessage.IsBodyHtml = false;

            _emailClient = Common.ConfigureClient(model.SMTPServer, model.SMTPPort, model.SMTPUserName, model.SMTPUserPassword);
            _emailClient.Send(mailMessage);
        }

        #endregion

        public static void PopulateEmployeeTop(BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.EmpTop obj, int EmployeeId, EmployeeService objEmpService)
        {
            var emp = objEmpService.PRMUnit.EmploymentInfoRepository.GetByID(EmployeeId);
            if (emp != null)
            {
                obj.EmpID = emp.EmpID;
                obj.EmployeeId = emp.Id;
                obj.Name = emp.FullName;
                obj.EmployeeInitial = emp.EmployeeInitial;
                obj.DesignationTop = objEmpService.PRMUnit.DesignationRepository.GetByID(emp.DesignationId).Name;
                obj.IsPhotoExist = Common.IsPhotoExist(EmployeeId, true, objEmpService);
            }
        }

        public static void PopulateAnsarTop(BEPZA_MEDICAL.Web.Areas.AMS.ViewModel.EmpTop obj, int EmployeeId, AMSCommonService objAMSService)
        {
            var emp = objAMSService.AMSUnit.AnsarEmpInfoRepository.GetByID(EmployeeId);
            if (emp != null)
            {
                obj.AnsarId = emp.AnsarId;
                obj.EmployeeId = emp.Id;
                obj.Name = emp.FullName;
                obj.DesignationTop = objAMSService.AMSUnit.AnsarDesignationRepository.GetByID(emp.DesignationId).DesignationName;
                obj.IsPhotoExist = objAMSService.IsPhotoExist(EmployeeId, true);
            }
        }

        public static string GetNewDeptPreceedingNo(PRMCommonSevice _prmCommonService)
        {
            string DeptPreceedingNo = "1";
            try
            {
                var lastDeptPreceedingNo = _prmCommonService.PRMUnit.ComplaintNoteSheetRepository.GetAll().OrderByDescending(q => Convert.ToInt32(q.DeptProceedingNo)).FirstOrDefault().DeptProceedingNo;
                DeptPreceedingNo = (Convert.ToInt32(lastDeptPreceedingNo) + 1).ToString();
            }
            catch { }
            return DeptPreceedingNo;
        }

        public static DateTime CurrentDateTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public static string ValidationSummaryHead
        {
            get
            {
                return "Please fill up the mandatory field(s)";
            }
        }

        public static string FormatDate(string strDate, string inputFormat, string outputFormat)
        {
            string outputDate = "";
            try
            {
                DateTime dt = DateTime.ParseExact(strDate, inputFormat, null);
                outputDate = dt.ToString(outputFormat);
            }
            catch
            {

            }

            return outputDate;
        }

        public static DateTime FormatDateforSQL(string txtDate)
        {
            if (!string.IsNullOrEmpty(txtDate))
            {
                return DateTime.Parse(txtDate, new CultureInfo("fr-Fr", true), DateTimeStyles.None);

            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime? FormatStringToDate(string txtDate)
        {
            if (!string.IsNullOrEmpty(txtDate))
            {
                try
                {
                    return DateTime.ParseExact(txtDate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
            }
            return null;
        }

        public static DateTime FormatDate(string strDate, string inputFormat)
        {
            DateTime outputDate = DateTime.MinValue;
            try
            {
                outputDate = DateTime.ParseExact(strDate, inputFormat, null);
            }
            catch
            {

            }
            return outputDate;
        }

        public static ArrayList GetMonthRange(DateTime startDate, DateTime endDate)
        {
            ArrayList MonthList = new ArrayList();
            endDate = endDate.AddDays(30 - endDate.Day);

            MonthList.Add(startDate);
            startDate = startDate.AddMonths(1);

            while (startDate <= endDate)
            {
                MonthList.Add(startDate);
                startDate = startDate.AddMonths(1);
            }

            return MonthList;
        }

        public static string GetCommomMessage(CommonMessage type)
        {
            switch (type)
            {
                case CommonMessage.DeleteFailed:
                    return ErrorMessages.DeleteFailed;

                case CommonMessage.DeleteSuccessful:
                    return ErrorMessages.DeleteSuccessful;

                case CommonMessage.UpdateFailed:
                    return ErrorMessages.UpdateFailed;

                case CommonMessage.UpdateSuccessful:
                    return ErrorMessages.UpdateSuccessful;


                case CommonMessage.InsertFailed:
                    return ErrorMessages.InsertFailed;

                case CommonMessage.InsertSuccessful:
                    return ErrorMessages.InsertSuccessful;
                case CommonMessage.DataNotFound:
                    return ErrorMessages.DataNotFound;
                case CommonMessage.MandatoryInputFailed:
                    return ErrorMessages.MandatoryInputFailed;

                default:
                    return string.Empty;

            }
        }

        public static string ErrorListToString(List<string> ErrorList)
        {
            string errMsg = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (ErrorList.Count > 0)
            {
                errMsg = "Following errors found - ";
                sb.Append(errMsg).Append("\r\n");
                foreach (string str in ErrorList)
                {
                    sb.Append("#. " + str).Append("\r\n");
                }
            }

            return sb.ToString();
        }

        public static IList<SelectListItem> PopulateLetterTypeList()
        {
            IList<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Salary", Value = "Salary" });
            list.Add(new SelectListItem() { Text = "Bonus", Value = "Bonus" });
            list.Add(new SelectListItem() { Text = "Leave Encashment", Value = "LeaveEncashment" });
            list.Add(new SelectListItem() { Text = "Overtime", Value = "Overtime" });
            list.Add(new SelectListItem() { Text = "Refreshment", Value = "Refreshment" });

            return list;
        }

        private static List<SelectListItem> _listOfYear;
        public static IList<SelectListItem> PopulateYearList()
        {
            if (_listOfYear == null || !_listOfYear.Any())
            {
                _listOfYear = new List<SelectListItem>();
                for (int i = DateTime.Now.Year; i >= 1970; i--)
                {
                    _listOfYear.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                }
            }
            return _listOfYear;
        }

        private static List<SelectListItem> _listOfYearFMS;
        public static IList<SelectListItem> PopulateYearListFMS()
        {
            if (_listOfYearFMS == null || !_listOfYearFMS.Any())
            {
                _listOfYearFMS = new List<SelectListItem>();
                for (int i = DateTime.Now.Year+10; i >= 1970; i--)
                {
                    _listOfYearFMS.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                }
            }
            return _listOfYearFMS;
        }


        private static List<SelectListItem> _incomeYearList;
        public static IList<SelectListItem> PopulateIncomeYearList()
        {
            if (_incomeYearList == null)
            {
                _incomeYearList = new List<SelectListItem>();
                for (int i = Common.CurrentDateTime.Year + 1; i >= 1970; i--)
                {
                    var iyFormat = (i - 1) + "-" + i;
                    _incomeYearList.Add(new SelectListItem() { Text = iyFormat, Value = iyFormat });
                }
            }

            return _incomeYearList;
        }

        private static List<SelectListItem> _monthListForReport;
        public static IList<SelectListItem> PopulateMonthListForReport()
        {
            if (_monthListForReport == null)
            {
                _monthListForReport = new List<SelectListItem>();
                DateTime month = new DateTime(DateTime.Now.Year, 1, 1);
                for (int i = 0; i < 12; i++)
                {
                    DateTime NextMont = month.AddMonths(i);
                    _monthListForReport.Add(new SelectListItem()
                    {
                        Text = NextMont.ToString("MMMM"),
                        Value = NextMont.Month.ToString()
                    });
                }
            }

            return _monthListForReport;
        }

        private static List<SelectListItem> _assessmentYearList;
        public static IList<SelectListItem> PopulateAssessmentYearList()
        {
            if (_assessmentYearList == null)
            {
                _assessmentYearList = new List<SelectListItem>();
                for (int i = Common.CurrentDateTime.Year + 1; i >= 1970; i--)
                {
                    var iyFormat = (i - 1) + "-" + i;
                    _assessmentYearList.Add(new SelectListItem() { Text = iyFormat, Value = iyFormat });
                }
            }

            return _assessmentYearList;
        }

        public static IList<SelectListItem> PopulateAccountForList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Salary", Value = "Salary", });
            list.Add(new SelectListItem() { Text = "Bonus", Value = "Bonus", });
            //list.Add(new SelectListItem() { Text = "Leave Encashment", Value = "LeaveEncashment", });
            list.Add(new SelectListItem() { Text = "Others", Value = "Others", });

            return list;
        }

        public static string GetApprovalButtonText(string status)
        {
            if (status.ToLower().Contains("appro")) return "Approve";
            else if (status.ToLower().Contains("submi")) return "Submit";
            else if (status.ToLower().Contains("review")) return "Review";
            else if (status.ToLower().Contains("recomm")) return "Recommend";
            return "";
        }

        public static IList<SelectListItem> PopulatePaymentStatusList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Paid", Value = "Paid", });
            list.Add(new SelectListItem() { Text = "Unpaid", Value = "Unpaid", });

            return list;
        }

        public static IList<SelectListItem> PopulateZoneOrProjectList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Zone", Value = "Zone", });
            list.Add(new SelectListItem() { Text = "Project", Value = "Project", });

            return list;
        }

        #region DropDownList

        public static IList<SelectListItem> PopulateDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Text,
                    Value = item.Value.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateMRRDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.MRR,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateDdlZoneList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ZoneName,
                    Value = item.Id.ToString()
                });
            }

            return list.ToList();
        }

        public static IList<SelectListItem> PopulateDDLBankList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.bankName,
                    Value = item.id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateDDLBankBranchList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.branchName,
                    Value = item.id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateDdlZoneListWithAllOption(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "[ALL]", Value = "0" });
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ZoneName,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Value.Trim()).ToList();
        }

        public static IList<SelectListItem> PopulateOverTimeRuleDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.RuleName,
                    Value = item.Id.ToString()
                });
            }
            return list.OrderBy(x => x.Text.Trim()).ToList();
        }

        public static IList<SelectListItem> PopulateDllListforPLPS(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.EmployeeInitial + " - " + item.FullName,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text.Trim()).ToList();
        }

        public static IList<SelectListItem> PopulateChequeDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return list.ToList();
        }

        public static IList<SelectListItem> PopulatePfPeriodDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.CPFPeriod,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text).ToList();
        }

        public static IList<SelectListItem> PopulateBankAccDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.BankAccountNo,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateInvestmentTypeDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.InvestmentType,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateTytleofInvestmentDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.TitleOfInterest,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateInvestmnetIdDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.InvestmentID,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateInvestmentStepNoDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.StepNo.ToString(),
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateFinancialYearDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.FinancialYearName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateFinancialYearDllList2(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.FinancialYearName,
                    Value = item.FinancialYearName
                });
            }

            return list;
        }
        public static IList<SelectListItem> FinancialYearListDll(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.yearName,
                    Value = item.id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateApprovalPathDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.PathName,
                    Value = item.PathId.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateSeparationTypeList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Resignation", Value = "Resignation", });
            list.Add(new SelectListItem() { Text = "Termination", Value = "Termination", });
            list.Add(new SelectListItem() { Text = "Retirement", Value = "Retirement", });
            list.Add(new SelectListItem() { Text = "Dismissal", Value = "Dismissal", });
            list.Add(new SelectListItem() { Text = "Removal", Value = "Removal", });
            list.Add(new SelectListItem() { Text = "Discharge", Value = "Discharge", });
            list.Add(new SelectListItem() { Text = "Rejoin", Value = "Rejoin", });
            return list;
        }

        /// <summary>
        /// Used for ADC
        /// </summary>
        /// <returns>List of Beneficiary Type </returns>
        public static IList<SelectListItem> PopulateBeneficiaryTypeList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Employee", Value = "Employee", });
            list.Add(new SelectListItem() { Text = "Department", Value = "Department", });
            list.Add(new SelectListItem() { Text = "Other", Value = "Other", });

            return list;
        }

        public static IList<SelectListItem> PopulateEmployeeStatusList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Active", Value = "1", });
            list.Add(new SelectListItem() { Text = "Inactive", Value = "2", });

            return list;
        }

        public static IList<SelectListItem> PopulateContuctualTypeList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Corporate Contract", Value = "1", });
            list.Add(new SelectListItem() { Text = "Project Contract", Value = "2", });

            return list;
        }

        public static IList<SelectListItem> PopulateJobGradeDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.GradeName,
                    Value = item.Id.ToString()
                });
            }

            return list.ToList();
        }

        public static IList<SelectListItem> PopulateCurrentJobGradeDDL(PRMCommonSevice _prmCommonService)
        {

            DateTime cDate = DateTime.Now;
            var prm_SalaryScaleEntity = _prmCommonService.PRMUnit.SalaryScaleRepository.GetAll().Where(t => t.DateOfEffective <= cDate).OrderByDescending(t => t.DateOfEffective).First();
            var ddlList = _prmCommonService.PRMUnit.JobGradeRepository.GetAll().Where(t => t.SalaryScaleId == prm_SalaryScaleEntity.Id).ToList();

            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.GradeName,
                    Value = item.Id.ToString()
                });
            }

            return list.ToList();
        }

        public static IList<SelectListItem> PopulateSalaryScaleDDL(dynamic salScalelist)
        {
            var list = new List<SelectListItem>();
            foreach (var item in salScalelist)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.SalaryScaleName,
                    Value = item.Id.ToString()
                });
            }

            return list.ToList();
        }

        public static IList<SelectListItem> PopulateShiftDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ShiftName,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text).ToList();
        }

        public static IList<SelectListItem> PopulatePaymentTypeDDL(List<PGM_GroupInsurancePaymentType> ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.PaymentType,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text).ToList();
        }

        public static IList<SelectListItem> PopulateShiftGroupDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ShiftGroupName,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text).ToList();
        }

        public static IList<SelectListItem> PopulateCountryDivisionDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.DivisionName,
                    Value = item.Id.ToString()
                });
            }
            return list.OrderBy(x => x.Text).ToList();
        }

        public static IList<SelectListItem> PopulateDistrictDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.DistrictName,
                    Value = item.Id.ToString()
                });
            }
            return list.OrderBy(x => x.Text).ToList();
        }

        public static IList<SelectListItem> PopulateERECDistrictDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.intPK.ToString()
                });
            }
            return list.OrderBy(x => x.Text).ToList();
        }

        public static IList<SelectListItem> PopulateEmployeeDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.FullName,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateEmployeementTypeDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateEmployeeDesignationDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateThanDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ThanaName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateStepList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.StepName.ToString(),
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateJobAdvertisementDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.AdCode.ToString(),
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateJobPostAdvertisementDDL(List<PRM_JobAdvertisementPostDetail> ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.PRM_Designation.Name,
                    Value = item.DesignationId.ToString()
                });
            }
            return list;
        }

        //use as Departmental Proceeding No
        public static IList<SelectListItem> PopulateComplaintNoteSheet(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.DeptProceedingNo,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        //use as Hearing ref No
        public static IList<SelectListItem> PopulateHearingFixationInfo(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.HearingRefNo,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulatePunishmentTypeDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.PunishmentName.ToString(),
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateLanguageList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Lanugage,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateProefficiencyLevelList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Level,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateResourceCategoryDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem() { Text = item.ResourceCategory, Value = item.Id.ToString() });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateSalaryHeadGroupDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateSalaryHeadTypeDDL()
        {
            var headTypes = Common.GetEnumAsDictionary<PGMEnum.SalaryHeadType>();

            var list = new List<SelectListItem>();
            foreach (KeyValuePair<int, string> item in headTypes)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Value });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateSalaryHeadMappersDDL()
        {
            var mappingHeads = GetEnumAsDictionary<PGMEnum.SalaryHeadMapper>();

            var list = new List<SelectListItem>();
            foreach (KeyValuePair<int, string> item in mappingHeads)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateImportXlFileTypeDDL()
        {
            var fileTypes = Common.GetEnumAsDictionary<PGMEnum.ImportXlFileType>();

            var list = new List<SelectListItem>();
            foreach (KeyValuePair<int, string> item in fileTypes)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateFamilyMemberList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.FullName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulatePunishmentRestrictionDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.RetrictionName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateBudgetHeadDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.BudgetHead,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        internal static IList<SelectListItem> PopulateAccountingPeriodDdl(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.yearName,
                    Value = item.id.ToString()
                });
            }
            return list.OrderByDescending(x => x.Text.Trim()).ToList();
        }

        public static IList<SelectListItem> PopulateScopeLogic()
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "=", Value = "=", });
            list.Add(new SelectListItem() { Text = ">", Value = ">", });
            list.Add(new SelectListItem() { Text = "<", Value = "<", });
            list.Add(new SelectListItem() { Text = ">=", Value = ">=", });
            list.Add(new SelectListItem() { Text = "<=", Value = "<=", });
            list.Add(new SelectListItem() { Text = "<>", Value = "<>", });
            list.Add(new SelectListItem() { Text = "Between", Value = "Between", });
            list.Add(new SelectListItem() { Text = "Or", Value = "Or", });
            //list.Add(new SelectListItem() { Text = "AND", Value = "AND", });
            return list;
        }

        private static List<SelectListItem> _sortType;
        public static IList<SelectListItem> PopulateSortType()
        {
            if (_sortType == null)
            {
                _sortType = new List<SelectListItem>();
                _sortType.Add(new SelectListItem() { Text = "Ascending", Value = "ASC", });
                _sortType.Add(new SelectListItem() { Text = "Descending", Value = "DESC", });
            }

            return _sortType;
        }

        public static List<SelectListItem> _monthList;
        /// <summary>
        /// Format: {Text = "January", Value = "January",}
        /// </summary>
        /// <returns></returns>
        public static IList<SelectListItem> PopulateMonthList()
        {
            if (_monthList == null || !_monthList.Any())
            {
                _monthList = new List<SelectListItem>();
                _monthList.Add(new SelectListItem() { Text = "January", Value = "January", });
                _monthList.Add(new SelectListItem() { Text = "February", Value = "February", });
                _monthList.Add(new SelectListItem() { Text = "March", Value = "March", });
                _monthList.Add(new SelectListItem() { Text = "April", Value = "April", });
                _monthList.Add(new SelectListItem() { Text = "May", Value = "May", });
                _monthList.Add(new SelectListItem() { Text = "June", Value = "June", });
                _monthList.Add(new SelectListItem() { Text = "July", Value = "July", });
                _monthList.Add(new SelectListItem() { Text = "August", Value = "August", });
                _monthList.Add(new SelectListItem() { Text = "September", Value = "September", });
                _monthList.Add(new SelectListItem() { Text = "October", Value = "October", });
                _monthList.Add(new SelectListItem() { Text = "November", Value = "November", });
                _monthList.Add(new SelectListItem() { Text = "December", Value = "December", });
            }

            return _monthList;
        }


        private static List<SelectListItem> _monthList2;
        /// <summary>
        /// Format: Text = "January", Value = "01"
        /// </summary>
        /// <returns></returns>
        public static IList<SelectListItem> PopulateMonthList2()
        {
            if (_monthList2 == null)
            {
                _monthList2 = new List<SelectListItem>();
                _monthList2.Add(new SelectListItem() { Text = "January", Value = "01" });
                _monthList2.Add(new SelectListItem() { Text = "February", Value = "02" });
                _monthList2.Add(new SelectListItem() { Text = "March", Value = "03" });
                _monthList2.Add(new SelectListItem() { Text = "April", Value = "04" });
                _monthList2.Add(new SelectListItem() { Text = "May", Value = "05" });
                _monthList2.Add(new SelectListItem() { Text = "June", Value = "06" });
                _monthList2.Add(new SelectListItem() { Text = "July", Value = "07" });
                _monthList2.Add(new SelectListItem() { Text = "August", Value = "08" });
                _monthList2.Add(new SelectListItem() { Text = "September", Value = "09" });
                _monthList2.Add(new SelectListItem() { Text = "October", Value = "10" });
                _monthList2.Add(new SelectListItem() { Text = "November", Value = "11" });
                _monthList2.Add(new SelectListItem() { Text = "December", Value = "12" });
            }

            return _monthList2;
        }

        private static List<SelectListItem> _monthList3;
        /// <summary>
        /// Format: Text = "January", Value = "1"
        /// </summary>
        /// <returns></returns>
        public static IList<SelectListItem> PopulateMonthList3()
        {
            if (_monthList3 == null)
            {
                _monthList3 = new List<SelectListItem>();
                _monthList3.Add(new SelectListItem() { Text = "January", Value = "1" });
                _monthList3.Add(new SelectListItem() { Text = "February", Value = "2" });
                _monthList3.Add(new SelectListItem() { Text = "March", Value = "3" });
                _monthList3.Add(new SelectListItem() { Text = "April", Value = "4" });
                _monthList3.Add(new SelectListItem() { Text = "May", Value = "5" });
                _monthList3.Add(new SelectListItem() { Text = "June", Value = "6" });
                _monthList3.Add(new SelectListItem() { Text = "July", Value = "7" });
                _monthList3.Add(new SelectListItem() { Text = "August", Value = "8" });
                _monthList3.Add(new SelectListItem() { Text = "September", Value = "9" });
                _monthList3.Add(new SelectListItem() { Text = "October", Value = "10" });
                _monthList3.Add(new SelectListItem() { Text = "November", Value = "11" });
                _monthList3.Add(new SelectListItem() { Text = "December", Value = "12" });
            }

            return _monthList3;
        }

        private static List<SelectListItem> _monthListDDL;
        /// <summary>
        /// Format: Text = "Jan", Value = "Jan"
        /// </summary>
        /// <returns></returns>
        public static IList<SelectListItem> PopulateMonthListDDL()
        {
            if (_monthListDDL == null)
            {
                _monthListDDL = new List<SelectListItem>();
                _monthListDDL.Add(new SelectListItem() { Text = "Jan", Value = "Jan" });
                _monthListDDL.Add(new SelectListItem() { Text = "Feb", Value = "Feb" });
                _monthListDDL.Add(new SelectListItem() { Text = "Mar", Value = "Mar" });
                _monthListDDL.Add(new SelectListItem() { Text = "Apr", Value = "Apr" });
                _monthListDDL.Add(new SelectListItem() { Text = "May", Value = "May" });
                _monthListDDL.Add(new SelectListItem() { Text = "Jun", Value = "Jun" });
                _monthListDDL.Add(new SelectListItem() { Text = "Jul", Value = "Jul" });
                _monthListDDL.Add(new SelectListItem() { Text = "Aug", Value = "Aug" });
                _monthListDDL.Add(new SelectListItem() { Text = "Sep", Value = "Sep" });
                _monthListDDL.Add(new SelectListItem() { Text = "Oct", Value = "Oct" });
                _monthListDDL.Add(new SelectListItem() { Text = "Nov", Value = "Nov" });
                _monthListDDL.Add(new SelectListItem() { Text = "Dec", Value = "Dec" });
            }

            return _monthListDDL;
        }

        private static List<SelectListItem> _restDayDDL;
        /// <summary>
        /// Format: Text = "Friday", Value = "1"
        /// </summary>
        /// <returns></returns>
        public static IList<SelectListItem> PopulateRestDayDDL()
        {
            if (_restDayDDL == null)
            {
                _restDayDDL = new List<SelectListItem>();

                _restDayDDL.Add(new SelectListItem() { Text = "Friday", Value = "1" });
                _restDayDDL.Add(new SelectListItem() { Text = "Saturday", Value = "2" });
                _restDayDDL.Add(new SelectListItem() { Text = "Sunday", Value = "3" });
                _restDayDDL.Add(new SelectListItem() { Text = "Monday", Value = "4" });
                _restDayDDL.Add(new SelectListItem() { Text = "Tuesday", Value = "5" });
                _restDayDDL.Add(new SelectListItem() { Text = "Wednesday", Value = "6" });
                _restDayDDL.Add(new SelectListItem() { Text = "Thursday", Value = "7" });
                _restDayDDL.Add(new SelectListItem() { Text = "General", Value = "8" });
            }

            return _restDayDDL.ToList();
        }

        public static int GetMonthNo(string strmonth)
        {
            int intMonthNo = 0;

            switch (strmonth)
            {
                case "January":
                    intMonthNo = 1;
                    break;
                case "February":
                    intMonthNo = 2;
                    break;
                case "March":
                    intMonthNo = 3;
                    break;
                case "April":
                    intMonthNo = 4;
                    break;
                case "May":
                    intMonthNo = 5;
                    break;

                case "June":
                    intMonthNo = 6;
                    break;
                case "July":
                    intMonthNo = 7;
                    break;
                case "August":
                    intMonthNo = 8;
                    break;
                case "September":
                    intMonthNo = 9;
                    break;
                case "October":
                    intMonthNo = 10;
                    break;
                case "November":
                    intMonthNo = 11;
                    break;
                case "December":
                    intMonthNo = 12;
                    break;
            }

            return intMonthNo;
        }

        public static string GetMonthName(int monthNo)
        {
            string MonthName = "";

            switch (monthNo)
            {
                case 1:
                    MonthName = "January";
                    break;
                case 2:
                    MonthName = "February";
                    break;
                case 3:
                    MonthName = "March";
                    break;
                case 4:
                    MonthName = "April";
                    break;
                case 5:
                    MonthName = "May";
                    break;
                case 6:
                    MonthName = "June";
                    break;
                case 7:
                    MonthName = "July";
                    break;
                case 8:
                    MonthName = "August";
                    break;
                case 9:
                    MonthName = "September";
                    break;
                case 10:
                    MonthName = "October";
                    break;
                case 11:
                    MonthName = "November";
                    break;
                case 12:
                    MonthName = "December";
                    break;
            }

            return MonthName;
        }

        public static IList<SelectListItem> PopulateApplyTypeDDL()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Shift Change After Rest Day", Value = "1" });
            list.Add(new SelectListItem() { Text = "Shift Unchanged", Value = "2" });
            list.Add(new SelectListItem() { Text = "Shift Change Friday", Value = "3" });
            list.Add(new SelectListItem() { Text = "Shift Change Saturday", Value = "4" });
            return list.ToList();
        }

        public static IList<SelectListItem> PopulateBaseOn()
        {
            var itemList = new List<SelectListItem>();

            itemList.Add(new SelectListItem { Text = "Basic", Value = "Basic" });
            itemList.Add(new SelectListItem { Text = "Gross", Value = "Gross" });

            return itemList;
        }

        public static IList<SelectListItem> PopulateGenderDDL(dynamic list)
        {
            list.Add(new SelectListItem() { Text = "Male", Value = "Male" });
            list.Add(new SelectListItem() { Text = "Female", Value = "Female" });

            return list;
        }

        public static IEnumerable<SelectListItem> GetAmountType()
        {
            List<String> amountTypeList = AmountType();

            IEnumerable<SelectListItem> selectList = from c in amountTypeList
                                                     select new SelectListItem { Text = c, Value = c };
            return selectList;
        }

        public static IList<SelectListItem> PopulateReferenceType()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem(){ Text="Other", Value="Other", Selected = true },
                new SelectListItem(){ Text="Invoice", Value="Invoice" },
                new SelectListItem(){ Text="Indent", Value="Indent" },
                new SelectListItem(){ Text="Advance", Value="Advance" },
            };
        }
        #region Welfare Fund Management
        //test
        public static IList<SelectListItem> PopulateCOABudgetHeatDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.accountName,
                    Value = item.id.ToString()
                });
            }

            return list;


        }

        public static IList<SelectListItem> PopulateCycleInfoDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.CycleName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }


        public static IList<SelectListItem> PopulateWelfareFundCategoryDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }


        public static IList<SelectListItem> PopulateMeetingRefNoDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.RefNo,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        #endregion

        #region Fund Management
        public static IList<SelectListItem> PopulateBankType()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem(){ Text="Govt Bank", Value="Govt"},
                new SelectListItem(){ Text="Private Bank", Value="Private" },
            };
        }
        public static IList<SelectListItem> PopulateFDRDurationTypeList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Year", Value = "Year", });
            list.Add(new SelectListItem() { Text = "Month", Value = "Month", });

            return list;
        }
        public static IList<SelectListItem> PopulateFDRBankDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.BankName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateFDRBankBranchDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.BranchName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }


        public static IList<SelectListItem> PopulateFDRTypeDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.FixedDepositType,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateFDRNoDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.FDRNumber,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateBankAcNoddl(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.accountName,
                    Value = item.id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateProfitRecAcddl(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.accountName,
                    Value = item.id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateChequeNoddl(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.chequeNo,
                    Value = item.id.ToString()
                });
            }
            return list;
        }

        #endregion


        #region Fixed Asset Management

        public static IList<SelectListItem> PopulateItemTypeDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ItemTypeName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateAssetCategoryDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.CategoryName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateAssetSubCategoryDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.SubCategoryName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateSparePartDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.SparePart,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateMaintenanceTypeDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.MaintenanceType,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateAssetCodeDDL(dynamic listItems)
        {
            var list = new List<SelectListItem>();
            foreach (var item in listItems)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.AssetCode,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        #endregion
        public static string GetFinancialYearByDate(DateTime dt)
        {
            int month = dt.Month;
            int year = dt.Year;

            int year1 = 0, year2 = 0;

            if (month <= 6)
            {
                year1 = year - 1;
                year2 = year;
            }
            else
            {
                year1 = year;
                year2 = year + 1;
            }

            return year1 + "-" + year2;
        }

        #region PIM

        public static IList<SelectListItem> PopulateProjectTypeDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ProjectType,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }
        public static IList<SelectListItem> PopulateBudgetMainDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.MainHead,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateBudgetSubDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();

            if (ddlList != null)
            {
                foreach (var item in ddlList)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.BudgetSubHead,
                        Value = item.Id.ToString()
                    });
                }
            }
            return list;
        }
        public static IList<SelectListItem> PopulateAccountSubDllList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.AccountHeadCode + " - [" + item.AccountHeadName + "]",
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateProjectNumber(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ProjectNo,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateBonusTypeDDLList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.BonusType,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateReligionDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> AmountTypeDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }
        public static IList<SelectListItem> PopulateIndentNo(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.IndentNo.ToString(),
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateLeaveType(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.strLeaveType,
                    Value = item.intLeaveTypeID.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateProjectTitle(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ProjectTitle,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateClientInfo(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ClientName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateSalaryHeadDDL(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.HeadName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateVoucherStatus()
        {
            return new List<SelectListItem>() { new SelectListItem() { Text = "Draft", Value = "Draft" }, new SelectListItem() { Text = "Recommended", Value = "Recommended" }, new SelectListItem() { Text = "Approved", Value = "Approved" }, new SelectListItem() { Text = "Rejected", Value = "Rejected" } };
        }

        public static IList<SelectListItem> PopulateDocumentType()
        {
            return new List<SelectListItem>() { new SelectListItem() { Text = "PRF", Value = "PRF" }, new SelectListItem() { Text = "Indent Request", Value = "Indent Request" },
                new SelectListItem() { Text = "Indent Adjustment", Value = "Indent Adjustment" }, new SelectListItem() { Text = "Time Sheet", Value = "Time Sheet" } };
        }

        public static IList<SelectListItem> PopulateAproverRole()
        {
            return new List<SelectListItem>() { new SelectListItem() { Text = "Submit", Value = "Submitted" }, new SelectListItem() { Text = "Review", Value = "Reviewed" }, new SelectListItem() { Text = "Recommend", Value = "Recommended" }, new SelectListItem() { Text = "Approve", Value = "Approved" } };
        }

        public static IList<SelectListItem> PopulateDdlProjectList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.NameOfWorks,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateEstimationHead(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = string.IsNullOrWhiteSpace(item.ItemCode) ? item.HeadName : item.ItemCode + " : " + item.HeadName,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        public static IList<SelectListItem> PopulateEstimationItemList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ItemName,
                    Value = item.Id.ToString()
                });
            }
            return list;
        }

        #endregion

        #endregion

        #region CPF
        public static IList<SelectListItem> PopulatePfLoanPolicyForDdl()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "CPF", Value = "CPF", });
            list.Add(new SelectListItem() { Text = "GPF", Value = "GPF", });
            return list;
        }

        public static IList<SelectListItem> PopulatePfApplicableForDdl()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Own Contribution", Value = "Own Contribution", });
            list.Add(new SelectListItem() { Text = "Total Balance", Value = "Total Balance", });
            return list;
        }

        public static IList<SelectListItem> PopulateYesNoDDLList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Yes", Value = "True" });
            list.Add(new SelectListItem() { Text = "No", Value = "False" });
            return list;
        }

        #endregion

        public static List<String> AmountType()
        {
            List<string> types = new List<string>();
            Dictionary<int, string> bonusAmountType = GetEnumAsDictionary<PGMEnum.AmountType>();
            foreach (KeyValuePair<int, string> item in bonusAmountType)
            {
                types.Add(item.Value);
            }

            return types;
        }

        /// <summary>
        /// Get enum values as dictionary.
        /// </summary>
        /// <typeparam name="T">enum</typeparam>
        /// <returns>Dictionary<int, string></returns>
        /// <exception cref="ArgumentException">Type must be an enum.</exception>
        public static Dictionary<int, string> GetEnumAsDictionary<T>(bool replaceUnderScoreWithWhiteSpace = false)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type must be an enum.");

            var dictionary = Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .ToDictionary(t => (int)Convert.ChangeType(t, t.GetType()), t => (replaceUnderScoreWithWhiteSpace ? t.ToString().Replace("_", " ") : t.ToString()));

            return dictionary;
        }

        /// <summary>
        /// Populate dropdown list from an enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IList<SelectListItem></returns>
        public static IList<SelectListItem> PopulateDdlFromEnum<T>()
        {
            bool replaceUnderScoreWithWhiteSpace = true;

            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type must be an enum.");

            var dictionary = Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .ToDictionary(t => (int)Convert.ChangeType(t, t.GetType()), t => (replaceUnderScoreWithWhiteSpace ? t.ToString().Replace("_", " ") : t.ToString()));

            var list = new List<SelectListItem>();
            foreach (KeyValuePair<int, string> item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }

            return list.OrderBy(x => x.Text.Trim()).ToList();
        }

        #region Encryption - Decryption
        public static string GetEncryptionKey()
        {
            return "10";
        }

        // a 32 character hexadecimal string.
        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
                //sBuilder.Append(Convert.ToString(data[i], 2).PadLeft(8, '0')); //Convert into binary
            }

            // Return the hexadecimal string.
            return sBuilder.ToString().ToLower();
        }

        // Verify a hash against a string.
        public static bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static string EncryptStringAES(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string DecryptStringAES(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }
        #endregion

        public static bool IsPhotoExist(int id, bool isPhoto, EmployeeService objEmployeeSRV)
        {
            PRM_EmpPhoto toDisplay = null;
            try
            {
                toDisplay = objEmployeeSRV.GetEmployeePhoto(id, isPhoto);
            }
            catch { }

            if (toDisplay != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetAgebyDateOfBirth(DateTime dateTime)
        {
            if (dateTime != null)
            {
                // get the difference in years
                int years = DateTime.Now.Year - ((DateTime)(dateTime)).Year;
                // subtract another year if we're before the
                // birth day in the current year
                if (DateTime.Now.Month < ((DateTime)(dateTime)).Month ||
                    (DateTime.Now.Month == ((DateTime)(dateTime)).Month &&
                    DateTime.Now.Day < ((DateTime)(dateTime)).Day))
                    years--;
                return years;
            }
            else
                return 0;
        }

        public static string GetModelStateError(ModelStateDictionary ModelState)
        {
            var errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            //.Select(x => x.Value.Errors.FirstOrDefault().ErrorMessage);
                            .Select(x => new { x.Key, x.Value.Errors })
                            .ToArray();

            return "";// errors.FirstOrDefault();
        }

        public static User User
        {
            get
            {

                if (MyAppSession.User == null)
                    return new User();
                else
                    return MyAppSession.User;
            }
        }

        public static string CheckPermission(string moduleName)
        {
            string LoginEmpId = "", groupName = "", roleNames = "";
            User sessionUser = null;
            if (MyAppSession.User != null)
            {
                sessionUser = MyAppSession.User;
            }
            if (MyAppSession.UserGroupName != null)
            {
                groupName = MyAppSession.UserGroupName;
            }
            if (MyAppSession.UserRoleNames != null)
            {
                roleNames = MyAppSession.UserRoleNames;
            }

            if (sessionUser != null && moduleName == "PRM")
            {
                if (groupName.ToLower().Contains("admin") || roleNames.ToLower().Contains("admin"))
                {
                    LoginEmpId = "";
                }
                else if (groupName.ToLower().Contains("employee") || roleNames.ToLower().Contains("employee"))
                {
                    LoginEmpId = sessionUser.EmpId;
                }
                else
                {
                    LoginEmpId = sessionUser.EmpId;
                }
            }
            else if (sessionUser != null && moduleName == "PIM")
            {
                if (groupName.ToLower().Contains("admin") || roleNames.ToLower().Contains("admin"))
                {
                    LoginEmpId = "";
                }
                else if (groupName.ToLower().Contains("project leader") || roleNames.ToLower().Contains("project leader"))
                {
                    LoginEmpId = sessionUser.EmpId;
                    MyAppSession.LoginRank = CrudeAction.ProjectLeader;
                }
                else if (groupName.ToLower().Contains("project Supervisor") || roleNames.ToLower().Contains("project Supervisor"))
                {
                    LoginEmpId = sessionUser.EmpId;
                    MyAppSession.LoginRank = CrudeAction.ProjectSupervisor;
                }
                else
                {
                    LoginEmpId = sessionUser.EmpId;
                }
            }
            else
            {
                // Extend if any more module need to incorporate data level security
            }

            return LoginEmpId;
        }

        #region Approval System

        public static IList<SelectListItem> PopulateDdlApproverGroup(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ApprovalGroupName,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text.Trim()).ToList();
        }

        public static IList<SelectListItem> PopulateDdlApprovalProcess(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ApprovalProcessName,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text.Trim()).ToList();
        }

        public static IList<SelectListItem> PopulateDdlApprovalStep(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.StepName,
                    Value = item.Id.ToString()
                });
            }

            return list.OrderBy(x => x.Text.Trim()).ToList();
        }

        public static IList<SelectListItem> PopulateDdlApprovalStepSequence(dynamic ddlList)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "Initial Step", Text = "Initial Step" });

            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.StepSequence,
                    Value = item.StepSequence
                });
            }

            return list.OrderBy(x => x.Text.Trim()).ToList();
        }

        public static IList<SelectListItem> PopulateDdlApprovalStepSequenceByApprovalFlow(dynamic ddlList)
        {
            var list = new List<SelectListItem>();

            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.StepSequence,
                    Value = item.Id.ToString()
                });
            }

            return list.ToList();
        }

        public static IList<SelectListItem> PopulateDdlApproverType(dynamic ddlList)
        {
            var list = new List<SelectListItem>();

            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ApproverType,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateDdlApproverAuthorType(dynamic ddlList)
        {
            var list = new List<SelectListItem>();

            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.AuthorType,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateDdlApprovalFlowList(dynamic ddlList)
        {
            var list = new List<SelectListItem>();

            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ApprovalFlowName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateDdlApprovalStatus(dynamic ddlList)
        {
            var list = new List<SelectListItem>();

            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.StatusName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateDdlApprovalStatusForRecommenderApprover(List<APV_ApprovalStatus> ddlList)
        {
            var list = new List<SelectListItem>();
            ddlList = ddlList.Where(q => q.StatusName.Contains("Submit") || q.StatusName.Contains("Recommend") || q.StatusName.Contains("Approved") || q.StatusName.Contains("Reject")).ToList();

            foreach (var item in ddlList)
            {
                var statusName = string.Empty;
                switch (item.StatusName)
                {
                    case "Submit":
                        statusName = "Submitted";
                        break;
                    case "Recommend":
                        statusName = "Recommended";
                        break;
                    case "Approved":
                        statusName = "Approved";
                        break;
                    case "Reject":
                        statusName = "Rejected";
                        break;
                }
                list.Add(new SelectListItem()
                {
                    Text = statusName,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PopulateApproverActionDdl(dynamic ddlList)
        {
            var list = new List<SelectListItem>();

            foreach (var item in ddlList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ActionType,
                    Value = item.Id.ToString()
                });
            }

            return list;
        }



        public static IList<SelectListItem> PopulateGenderDDLWithAllOption()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "ALL".ToUpper(), Value = "ALL".ToUpper() });
            list.Add(new SelectListItem() { Text = "Male", Value = "Male" });
            list.Add(new SelectListItem() { Text = "Female", Value = "Female" });

            return list;
        }


        #endregion

        #region Converting DataType

        public static string GetString(object s)
        {
            if (s != null)
                return GetString(Convert.ToString(s));
            else
                return string.Empty;
        }

        public static string GetString(string s)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                return string.Empty;
            else
                return s;
        }

        public static int GetInteger(object s)
        {
            int output;
            int.TryParse(GetString(s), out output);
            return output;
        }

        /// <summary>
        /// Converts object into nullable integer.
        /// </summary>
        /// <param name="s">object</param>
        /// <param name="returnNull">allow return null</param>
        /// <returns>int<Nullable></returns>
        public static int? GetInteger(object s, bool returnNull = false)
        {
            int output;
            bool isNumeric = int.TryParse(GetString(s), out output);
            if (returnNull)
            {
                if (isNumeric)
                    return output;
                else
                    return null;
            }
            else
            {
                return output;
            }
        }

        public static Byte GetByte(object s)
        {
            Byte output;
            Byte.TryParse(GetString(s), out output);
            return output;
        }

        public static Byte? GetByte(object s, bool returnNull = false)
        {
            Byte output;
            bool isNumeric = byte.TryParse(GetString(s), out output);
            if (returnNull)
            {
                if (isNumeric)
                    return output;
                else
                    return null;
            }
            else
            {
                return output;
            }
        }

        public static decimal GetDecimal(object s)
        {
            decimal output;
            decimal.TryParse(GetString(s), out output);
            return output;
        }

        public static decimal GetDecimalFromCurrency(string s)
        {
            decimal output;
            decimal.TryParse(s, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out output);
            return output;
        }

        public static bool GetBoolean(object obj)
        {
            bool output;
            Boolean.TryParse(GetString(obj), out output);
            return output;
        }

        /// <summary>
        /// dd-MMM-yyyy 00:00:00 AM
        /// </summary>
        /// <param name="s">object</param>
        /// <returns>DateTime</returns>
        public static DateTime GetDate(object s)
        {
            DateTime output;
            if (DateTime.TryParse(GetString(s), out output))
            {
                output = Convert.ToDateTime((output.ToString("dd-MMM-yyyy") + " 00:00:00 AM"));
            }
            return output;
        }

        /// <summary>
        /// dd-MMM-yyyy 00:00:00 AM
        /// </summary>
        /// <param name="s">object</param>
        /// <returns>string</returns>
        public static string GetDate2(object s)
        {
            DateTime output;
            DateTime.TryParse(GetString(s), out output);
            return output.ToString("dd-MMM-yyyy") + " 00:00:00 AM";
        }

        /// <summary>
        /// Convert an object into datetime datatype.
        /// </summary>
        /// <param name="s">object</param>
        /// <returns>This method returns datetime type in dd-MMM-yyyy hh:mm:ss tt format.</returns>
        public static DateTime GetDateTime(object s)
        {
            DateTime output;
            if (DateTime.TryParse(GetString(s), out output))
            {
                output = Convert.ToDateTime(output.ToString("dd-MMM-yyyy hh:mm:ss tt"));
            }
            return output;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s">object</param>
        /// <returns>This method returns string type in dd-MMM-yyyy hh:mm:ss tt format.</returns>
        public static string GetDateTime2(object s)
        {
            DateTime output;
            DateTime.TryParse(GetString(s), out output);
            return output.ToString("dd-MMM-yyyy hh:mm:ss tt");
        }


        public static DataTable LinqQueryToDataTable(IEnumerable<dynamic> v)
        {
            //We really want to know if there is any data at all
            var firstRecord = v.FirstOrDefault();
            if (firstRecord == null)
                return null;

            /*Okay, we have some data. Time to work.*/

            //So dear record, what do you have?
            PropertyInfo[] infos = firstRecord.GetType().GetProperties();

            //Our table should have the columns to support the properties
            DataTable table = new DataTable();

            //Add, add, add the columns
            foreach (var info in infos)
            {

                Type propType = info.PropertyType;

                if (propType.IsGenericType
                    && propType.GetGenericTypeDefinition() == typeof(Nullable<>)) //Nullable types should be handled too
                {
                    table.Columns.Add(info.Name, Nullable.GetUnderlyingType(propType));
                }
                else
                {
                    table.Columns.Add(info.Name, info.PropertyType);
                }
            }

            //Hmm... we are done with the columns. Let's begin with rows now.
            DataRow row;

            foreach (var record in v)
            {
                row = table.NewRow();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    row[i] = infos[i].GetValue(record) != null ? infos[i].GetValue(record) : DBNull.Value;
                }

                table.Rows.Add(row);
            }

            //Table is ready to serve.
            table.AcceptChanges();

            return table;
        }

        #endregion

        public static bool ExportPDF(ReportViewer viewer, string newFileName, FileStream file)
        {

            try
            {

                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = viewer.LocalReport.Render(
                               "PDF", null, out mimeType, out encoding, out filenameExtension,
                               out streamids, out warnings);

                String newFilePath = "~/Content/TempFiles/" + newFileName;

                using (FileStream fs = file)
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                HttpContext.Current.Response.Redirect(newFilePath, "_blank", string.Empty);

                try
                {
                    File.Delete(newFilePath);
                }
                catch (Exception) { }
            }
            catch (Exception)
            {

            }

            return true;
        }


    }

    public static class Compare
    {
        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(Compare.By(identitySelector));
        }

        public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateComparer<TSource, TIdentity>(identitySelector);
        }

        private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
        {
            private readonly Func<T, TIdentity> identitySelector;

            public DelegateComparer(Func<T, TIdentity> identitySelector)
            {
                this.identitySelector = identitySelector;
            }

            public bool Equals(T x, T y)
            {
                return Equals(identitySelector(x), identitySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return identitySelector(obj).GetHashCode();
            }
        }
    }

    public class DateAndTime
    {
        /// <summary>
        /// It must be use all over the project
        /// </summary>
        public const string GlobalDateFormat = "yyyy-MM-dd";

        public enum DateInterval
        {
            Day,
            DayOfYear,
            Hour,
            Minute,
            Month,
            Quarter,
            Second,
            Weekday,
            WeekOfYear,
            Year
        }

        public static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2)
        {
            return DateDiff(interval, dt1, dt2, System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
        }

        private static int GetQuarter(int nMonth)
        {
            if (nMonth <= 3)
                return 1;
            if (nMonth <= 6)
                return 2;
            if (nMonth <= 9)
                return 3;
            return 4;
        }

        public static DateTime DateAdd(DateInterval interval, DateTime dt, Int32 val)
        {
            if (interval == DateInterval.Year)
                return dt.AddYears(val);
            else if (interval == DateInterval.Month)
                return dt.AddMonths(val);
            else if (interval == DateInterval.Day)
                return dt.AddDays(val);
            else if (interval == DateInterval.Hour)
                return dt.AddHours(val);
            else if (interval == DateInterval.Minute)
                return dt.AddMinutes(val);
            else if (interval == DateInterval.Second)
                return dt.AddSeconds(val);
            else if (interval == DateInterval.Quarter)
                return dt.AddMonths(val * 3);
            else
                return dt;
        }

        public static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2, DayOfWeek eFirstDayOfWeek)
        {
            if (interval == DateInterval.Year)
                return dt2.Year - dt1.Year;

            if (interval == DateInterval.Month)
                return (dt2.Month - dt1.Month) + (12 * (dt2.Year - dt1.Year));

            TimeSpan ts = dt2 - dt1;

            if (interval == DateInterval.Day || interval == DateInterval.DayOfYear)
                return Round(ts.TotalDays);

            if (interval == DateInterval.Hour)
                return Round(ts.TotalHours);

            if (interval == DateInterval.Minute)
                return Round(ts.TotalMinutes);

            if (interval == DateInterval.Second)
                return Round(ts.TotalSeconds);

            if (interval == DateInterval.Weekday)
            {
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.WeekOfYear)
            {
                while (dt2.DayOfWeek != eFirstDayOfWeek)
                    dt2 = dt2.AddDays(-1);
                while (dt1.DayOfWeek != eFirstDayOfWeek)
                    dt1 = dt1.AddDays(-1);
                ts = dt2 - dt1;
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.Quarter)
            {
                double d1Quarter = GetQuarter(dt1.Month);
                double d2Quarter = GetQuarter(dt2.Month);
                double d1 = d2Quarter - d1Quarter;
                double d2 = (4 * (dt2.Year - dt1.Year));
                return Round(d1 + d2);
            }

            return 0;
        }

        private static long Round(double dVal)
        {
            if (dVal >= 0)
                return (long)Math.Floor(dVal);
            return (long)Math.Ceiling(dVal);
        }

    }
}