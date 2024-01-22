using System;
using System.Collections.Generic;
using System.Linq;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.DAL.CPF.CustomEntities;
using System.Net.Mail;
using System.Net;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Domain.PRM;


namespace BEPZA_MEDICAL.Domain.CPF
{
    public class CPFCommonService
    {
        #region Fields

        CPF_UnitOfWork _cpfUnit;
        PGMCommonService _pgmCommonService;
        PRMCommonSevice _prmCommonService;

        #endregion

        #region Ctor

        public CPFCommonService(CPF_UnitOfWork uow)
        {
            _cpfUnit = uow;
        }

        public CPFCommonService(CPF_UnitOfWork uow, PGMCommonService pgmCommonService, PRMCommonSevice prmCommonService)
        {
            _cpfUnit = uow;
            _pgmCommonService = pgmCommonService;
            _prmCommonService = prmCommonService;
        }

        #endregion

        #region Properties

        public CPF_UnitOfWork CPFUnit { get { return _cpfUnit; } }

        #endregion

        #region Business Logic Validation
        
        #region Membership Information

        public string CheckMembershipID(CPF_MembershipInfo entity)
        {
            string messeage = string.Empty;

            if (entity.Id > 0)
            {
                var membership = (from a in this._cpfUnit.MembershipInformationRepository.GetAll()
                                  where a.MembershipID == entity.MembershipID && a.Id != entity.Id
                                  select a).LastOrDefault();

                if (membership != null)
                {
                    messeage = "Membership ID must be unique.";
                }
            }
            else
            {
                var membership = (from a in this._cpfUnit.MembershipInformationRepository.GetAll()
                                  where a.MembershipID == entity.MembershipID
                                  select a).LastOrDefault();

                if (membership != null)
                {
                    messeage = "Membership ID must be unique.";
                }
            }


            return messeage;
        }

        public string DeleteMembershipValidation(int membershipID)
        {
            string messeage = string.Empty;

            var employeeId = _cpfUnit.MembershipInformationRepository.GetAll().FirstOrDefault(m => m.Id == membershipID).EmployeeId;

            var openingBalance = (from a in this._cpfUnit.OpeningBalanceRepository.GetAll() where a.MembershipId == membershipID select a).LastOrDefault();
            if (openingBalance != null)
            {
                messeage = "Opening Balance already exist in the system.";
            }

            var profitDistribution = (from a in this._cpfUnit.ProfitDistributionDetailRepository.GetAll() where a.MembershipId == membershipID select a).LastOrDefault();
            if (profitDistribution != null)
            {
                messeage = "Profit distribution already exist in the system. Please rollback.";
            }

            var pfContribution = (from s in this._pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                                  join sd in this._pgmCommonService.PGMUnit.SalaryDetailsRepository.GetAll() on s.Id equals sd.SalaryId
                                  join sh in this._prmCommonService.PRMUnit.SalaryHeadRepository.GetAll() on sd.HeadId equals sh.Id
                                  where sh.IsPFOwnContributionHead == true
                                    && s.EmployeeId == employeeId
                                  select sd).ToList();
            if (pfContribution != null || pfContribution.Count() > 0)
            {
                if (pfContribution.Sum(s => s.HeadAmount) > 0)
                {
                    messeage = "PF contribution already exist in the system. Please rollback salary.";
                }
            }

            var cpfWithdraw = (from a in this._cpfUnit.WithdrawRepository.GetAll() where a.MembershipId == membershipID select a).LastOrDefault();
            if (cpfWithdraw != null)
            {
                messeage = "CPF withdraw already exist in the system.";
            }

            var cpfFinalSettlement = (from a in this._cpfUnit.SettlementRepository.GetAll() where a.MembershipId == membershipID select a).LastOrDefault();
            if (cpfFinalSettlement != null)
            {
                messeage = "CPF final settlement already exist in the system.";
            }

            //var cpfNominee = (from a in this._cpfUnit.MemberNomineeRepository.GetAll() where a.MembershipId == membershipID select a).LastOrDefault();
            //if (cpfNominee != null)
            //{
            //    messeage = "CPF member nominee already exist in the system.";
            //}

            return messeage;
        }

        #endregion

        #endregion

        #region email send

        public bool SendEmail(string smtp, string FromEmailAddress, string FromEmailpassword, string ToEmailAddress, string subject, string message, string ccEmail = "")
        {
            bool isSuccess = false;
            try
            {
                var loginInfo = new NetworkCredential(FromEmailAddress, FromEmailpassword);
                var msg = new MailMessage();

                var smtpClient = new SmtpClient(smtp);

                msg.From = new MailAddress(FromEmailAddress);
                msg.To.Add(new MailAddress(ToEmailAddress));

                if (ccEmail != "")
                {
                    msg.CC.Add(new MailAddress(ccEmail));
                }

                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;

                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;
                smtpClient.Send(msg);
                isSuccess = true;
                return isSuccess;
            }
            catch (Exception)
            {
                isSuccess = false;
                return isSuccess;
            }
        }
        
        #endregion
    }
}
