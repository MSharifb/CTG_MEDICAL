using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval
{
    public class CpfApplicationViewModel
    {
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Applicant Name")]
        public string ApplicantName { get; set; }

        [DisplayName("SonDaugherOf")]
        public string SonDaugherOf { get; set; }

        [DisplayName("Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [DisplayName("Designation")]
        public string Designation { get; set; }

        [DisplayName("Identity Card No.")]
        public string IdNo { get; set; }

        [DisplayName("Nature of Employment")]
        public string EmployeeType { get; set; }

        [DisplayName("Section")]
        public string Section { get; set; }

        [DisplayName("Department")]
        public string Department { get; set; }

        [DisplayName("Joining Date")]
        public DateTime JoiningDate { get; set; }

        [DisplayName("Present Pay Scale")]
        public string PresentPayScale { get; set; }

        [DisplayName("Present Basic Pay")]
        public decimal BasicPay { get; set; }

        [DisplayName("Nationality")]
        public string Nationality { get; set; }

        [DisplayName("Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Permanent Address")]
        public string PermanentAddress { get; set; }

        [DisplayName("Witness")]
        public string Witness { get; set; }

        [DisplayName("Witness Date")]
        public DateTime WitnessSignatureDate { get; set; }

        [DisplayName("Signature of Employee")]
        public byte[] EmployeeSignature { get; set; }

        [DisplayName("Sign Date Of Emp")]
        public DateTime EmployeeSignatureDate { get; set; }

        [DisplayName("Receipt Date")]
        public DateTime ReceiptDate { get; set; }

        public string ApplicationTo { get; set; }
        public string ApplicationAddress1 { get; set; }
        public string ApplicationAddress2 { get; set; }

        public string ApplicationSubject { get; set; }

        public string ApplicationBody1 { get; set; }

        public string ApplicationBody2 { get; set; }

    }
}