using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class OnlineApplicantViewModel : BaseViewModel
    {
        public int JobAdvertisementInfoId { get; set; }

        public Int32 DesignationId { get; set; }

        public string ApplicantName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string MobileNo { get; set; }

        public string FatherName { get; set; }

        public string MotherName { get; set; }

        public string NationalId { get; set; }

        public string DesignationName { get; set; }
    }


    public class ApplicationFormModel
    {
      //private readonly PRMCommonSevice _prmCommonservice;
      //public ApplicationFormModel(PRMCommonSevice prmCommonService)
      //{
      //     this._prmCommonservice = prmCommonService;
      // }
        public string LastDateofApplication { get; set; }
        public string CaptchaText { get; set; }
        public EREC_tblgeneralinfo GeneralInfo { get; set; }
        public EREC_tblmaritalinfo MaritalInfo { get; set; }
        public EREC_tbladdress AddressInfo { get; set; }
        public EREC_tbleducationalinfo EducationalInfo { get; set; }

        //Dropdown demo Jony
        private SelectList _countryList;
        public SelectList CountryList
        {
            get
            {
                //List<Country> countryList = CountryBLL.GetAllCountry();

                //this._countryList = new SelectList(countryList, "intPK", "Name");

                return _countryList;
            }
            set { _countryList = value; }
        }

        private SelectList _bloodGroupList;
        public SelectList ReligionList
        {
            get
            {
                //List<Religion> bloodGroupList = ReligionBLL.GetAll();
                //this._religionList = new SelectList(bloodGroupList, "Id", "Name");

                return _bloodGroupList;
            }
            set { _bloodGroupList = value; }
        }
        public string AgeDeadline { get; set; }

        #region Added By Shamim For Bepza
        public string Designation { get; set; }
        public SelectList _religionList { get; set; }
        #endregion


    }
    #region Block Class
    //public class GeneralInfo : EntityModelBase
    //{
    //    [DataMember, DataColumn(true)]
    //    [Key]
    //    public System.Int32 intPK { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String Name { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String NameB { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String Gender { get; set; }



    //    [Required]
    //    [DataMember, DataColumn(true)]

    //    [UIHint("_Date")]
    //    public DateTime DateOfBirth { get; set; }

    //    public String strDateOfBirth
    //    {
    //        get { return DateOfBirth.ToShortDateString(); } // Utility.FormatDateTime(DateOfBirth);
    //        set { DateOfBirth = Convert.ToDateTime( value); } //// Utility.FormatDateforSQL(value);
    //    }


    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 PlaceOfBirth { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String Nationality { get; set; }

    //    [DataMember, DataColumn(true)]

    //    public System.String NationalId { get; set; }

    //    [DataMember, DataColumn(true)]

    //    public System.String BirthRegNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 ReligionId { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String MobilePhoneNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String Email { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String FathersName { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String FathersOccupation { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 FathersNationality { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String MothersName { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String MothersOccupation { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 MothersNationality { get; set; }

    //    [DataMember, DataColumn(true)]

    //    public System.Int32 DualNationality { get; set; }

    //    [DataMember, DataColumn(true)]
    //    //[Required]
    //    public byte[] Photo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    //[Required]
    //    public System.String UserID { get; set; }

    //    public System.Int32 intErrorCode { get; set; }

    //    public System.Boolean _DualNationality
    //    {
    //        get { return Convert.ToBoolean(DualNationality); }
    //        set { }
    //    }
    //    [DataMember, DataColumn(true)]
    //    public System.Int32 CircularID { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public System.String Age { get; set; }

    //    [DataMember, DataColumn(true)]
    //    public int Status { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public int RollNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    public Int32 DesignationId { get; set; }
    //    public string DesignationName { get; set; }

    //}

    //public class MaritalInfo : EntityModelBase
    //{
    //    public MaritalInfo()
    //    {
    //        IsDeclaration = true;
    //        EmploymentStatus = "No";
    //        ChildOfFreedomFighter = "No";
    //        EthnicGroup = "No";
    //        strsDate = DateTime.Now.ToString("dd-MM-yyyy");
    //    }
    //    [DataMember, DataColumn(true)]
    //    [Key]
    //    public System.Int32 intPK { get; set; }

    //    [DataMember, DataColumn(true)]
    //    public Int32 TotalYearofExp { get; set; }

    //    [DataMember, DataColumn(true)]
    //    public string ExperienceDetail { get; set; }

    //    [DataMember, DataColumn(true)]
    //    public string AdditionalQualification { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    // [System.ComponentModel.DefaultValue("No")]
    //    public System.String EmploymentStatus { get; set; }


    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String EthnicGroup { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String NameOfTheCommunity { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String ChildOfFreedomFighter { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String NameOfFreedomFighter { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String CertificateNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String IssuingAuthority { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [UIHint("_Date")]
    //    [Required]
    //    public System.DateTime DateOfIssue { get; set; }

    //    public String strDateOfIssue {get;set;}
    //    //{
    //    //    get { return Utility.FormatDateTime(DateOfIssue); }
    //    //    set { DateOfIssue = Utility.FormatDateforSQL(value); }
    //    //}

    //    [DataMember, DataColumn(true)]
    //    public int GeneralID { get; set; }

    //    #region Declaration

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Boolean IsDeclaration { get; set; }

    //    [Required]
    //    [DataMember, DataColumn(true)]

    //    [UIHint("_Date")]
    //    public DateTime Date { get; set; }

    //    public String strsDate {get;set;}
    //    //{
    //    //    get { return Utility.FormatDateTime(Date); }
    //    //    set { Date = Utility.FormatDateforSQL(value); }
    //    //}

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public byte[] Signature { get; set; }

    //    #endregion
    //}
    //public class AddressInfo : EntityModelBase
    //{
    //    [DataMember, DataColumn(true)]
    //    [Key]
    //    public System.Int32 intPK { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String PermanentAddress { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 PermanentDistrict { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 PermanentThana { get; set; }

    //    [DataMember, DataColumn(true)]

    //    public System.String PermanentPostCode { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String PresentAddress { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 PresentDistrict { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 PresentThana { get; set; }

    //    [DataMember, DataColumn(true)]

    //    public System.String PresentPostCode { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String EmergecyName { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String EmergencyContactNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String EmergencyAddress { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String EmergencyRelationship { get; set; }

    //    [DataMember, DataColumn(true)]

    //    public System.String EmergencyEmail { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public int GeneralID { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public Boolean SameasPermanent { get; set; }
    //}

    //public class EducationalInfo : EntityModelBase
    //{
    //    public EducationalInfo()
    //    {

    //        SscResult = "CGPA";
    //        HscResult = "CGPA";
    //        HonsResult = "Division/Class";
    //        MPassResult = "Division/Class";
    //        OD1Result = "Division/Class";
    //        OD2Result = "Division/Class";
    //        //OtherDegreeResult = "Division/Class";
    //    }


    //    [DataMember, DataColumn(true)]
    //    [Key]
    //    public System.Int32 intPK { get; set; }

    //    #region SSC



    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 SscExam { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 SscBoard { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String SscRollNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String SscRegistrationNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String SscResult { get; set; }


    //    [DataMember, DataColumn(true)]

    //    public System.String SscResultCGPA { get; set; }
    //    [DataMember, DataColumn(true)]

    //    public System.String SscResultDivision { get; set; }

      

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 SscGroup { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 SscPassingYear { get; set; }

    //    #endregion

    //    #region HSC
    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 HscExam { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 HscBoard { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String HscRollNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String HscRegistrationNo { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String HscResult { get; set; }

    //    [DataMember, DataColumn(true)]

    //    public System.String HscResultCGPA { get; set; }

    //    [DataMember, DataColumn(true)]

    //    public System.String HscResultDivision { get; set; }


    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 HscGroup { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 HscPassingYear { get; set; }

    //    #endregion

    //    #region Honurs

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 HonsUniversity { get; set; }

    //    //[DataMember, DataColumn(true)]
    //    //[Required]
    //    //public System.String HonsId { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String HonsResult { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String HonsDivision { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String HonsCgpa { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String HonsScale { get; set; }

    //    //[DataMember, DataColumn(true)]
    //    //[Required]
    //    //public System.String HonsMarks { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 HonsPassingYear { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public Int32 HonsExaminationId { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public Int32 HonsSubjectId { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string HonsAcademicInstitution { get; set; }

    //    [DataMember, DataColumn(true)]

    //    [UIHint("_Date")]
    //    public DateTime HonsConcludingdate { get; set; }

    //    public String strHonsConcludingdate {get;set;}
    //    //{
    //    //    get { return Utility.FormatDateTime(HonsConcludingdate); }
    //    //    set { HonsConcludingdate = Utility.FormatDateforSQL(value); }
    //    //}

    //    #endregion

    //    #region MPass

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 MPassUniversity { get; set; }

    //    //[DataMember, DataColumn(true)]
    //    //[Required]
    //    //public System.String MPassId { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String MPassResult { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String MPassDivision { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String MPassCgpa { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String MPassScale { get; set; }


    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 MPassPassingYear { get; set; }

    //    [DataMember, DataColumn(true)]

    //    [UIHint("_Date")]
    //    public DateTime MPassConcludingdate { get; set; }

    //    public String strMPassConcludingdate {get;set;}
    //    //{
    //    //    get { return Utility.FormatDateTime(MPassConcludingdate); }
    //    //    set { MPassConcludingdate = Utility.FormatDateforSQL(value); }
    //    //}
    //    [DataMember, DataColumn(true)]
    //    public Int32 MPassExaminationId { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public Int32 MPassSubjectId { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string MPassAcademicInstitution { get; set; }

    //    #endregion

    //    #region Others Degree One

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 OD1University { get; set; }


    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String OD1Result { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String OD1Division { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String OD1Cgpa { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String OD1Scale { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 OD1PassingYear { get; set; }

    //    [DataMember, DataColumn(true)]

    //    [UIHint("_Date")]
    //    public DateTime OD1Concludingdate { get; set; }

    //    public String strOD1Concludingdate {get;set;}
    //    //{
    //    //    get { return Utility.FormatDateTime(OD1Concludingdate); }
    //    //    set { OD1Concludingdate = Utility.FormatDateforSQL(value); }
    //    //}
    //    [DataMember, DataColumn(true)]
    //    public Int32 OD1ExaminationId { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string OD1Subject { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string OD1AcademicInstitution { get; set; }

    //    #endregion

    //    #region Others Degree Two

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 OD2University { get; set; }


    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String OD2Result { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String OD2Division { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String OD2Cgpa { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.String OD2Scale { get; set; }

    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 OD2PassingYear { get; set; }

    //    [DataMember, DataColumn(true)]

    //    [UIHint("_Date")]
    //    public DateTime OD2Concludingdate { get; set; }

    //    public String strOD2Concludingdate {get;set;}
    //    //{
    //    //    get { return Utility.FormatDateTime(OD2Concludingdate); }
    //    //    set { OD2Concludingdate = Utility.FormatDateforSQL(value); }
    //    //}
    //    [DataMember, DataColumn(true)]
    //    public Int32 OD2ExaminationId { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string OD2Subject { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string OD2AcademicInstitution { get; set; }

    //    #endregion

    //    #region Professional Degree
    //    [DataMember, DataColumn(true)]
    //    public Int32 CertificationCategoryId { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string CertificationTitle { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public Int32 ProfessionalUniversity { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public Int32 CountryId { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string ProfessionalResult { get; set; }
    //    [DataMember, DataColumn(true)]
    //    [Required]
    //    public System.Int32 ProfessionalPassingYear { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public string Comments { get; set; }

    //    #endregion

    //    [DataMember, DataColumn(true)]
    //    public int GeneralID { get; set; }


    //    [DataMember, DataColumn(true)]
    //    public Boolean Honours { get; set; }

    //    [DataMember, DataColumn(true)]
    //    public Boolean MPass { get; set; }

    //    [DataMember, DataColumn(true)]
    //    public Boolean OtherDegree1 { get; set; }

    //    [DataMember, DataColumn(true)]
    //    public Boolean OtherDegree2 { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public Boolean ProfessionalDegree { get; set; }


    //    [DataMember, DataColumn(true)]
    //    public String HscOtherBoard { get; set; }
    //    [DataMember, DataColumn(true)]
    //    public String SscOtherBoard { get; set; }

    //}

    #endregion 
    public class EntityModelBase
    {
        [DataMember, DataColumn(true)]
        public System.String IUser { get; set; }

        [DataMember, DataColumn(true)]
        public System.String EUser { get; set; }

        [DataMember, DataColumn(true)]
        public System.DateTime IDate { get; set; }

        [DataMember, DataColumn(true)]
        public System.DateTime EDate { get; set; }

        public string OperationMode { set; get; }
        public bool IsValid { set; get; }
    }

    public class BloodGroup : EntityModelBase
    {
        [DataMember, DataColumn(true)]

        public System.Int32 intPK { get; set; }

        [DataMember, DataColumn(true)]

        public System.String Name { get; set; }
    }
    public class Board : EntityModelBase
    {
        [DataMember, DataColumn(true)]

        public System.Int32 intPK { get; set; }

        [DataMember, DataColumn(true)]

        public System.String Name { get; set; }
    }
    public class Examination : EntityModelBase
    {
        [DataMember, DataColumn(true)]

        public System.Int32 intPK { get; set; }

        [DataMember, DataColumn(true)]

        public System.String Name { get; set; }
        [DataMember, DataColumn(true)]
        public System.String ExamType { get; set; }
    }
    public class ExamGroup : EntityModelBase
    {
        [DataMember, DataColumn(true)]

        public System.Int32 intPK { get; set; }

        [DataMember, DataColumn(true)]

        public System.String Name { get; set; }

    }

    public class Country : EntityModelBase
    {
        [DataMember, DataColumn(true)]

        public System.Int32 intPK { get; set; }

        [DataMember, DataColumn(true)]

        public System.String Name { get; set; }
    }

    public class District : EntityModelBase
    {
        [DataMember, DataColumn(true)]
        [Key]
        public System.Int32 intPK { get; set; }

        [DataMember, DataColumn(true)]
        [Required]
        public System.String Name { get; set; }

        [DataMember, DataColumn(true)]
        [Required]
        public int RegionID { get; set; }

        [DataMember, DataColumn(true)]
        public string RegionName { get; set; }

        [DataMember, DataColumn(true)]
        public string DistrictName { get; set; }


    }

    public class Upazila : EntityModelBase
    {
        [DataMember, DataColumn(true)]
        [Key]
        public System.Int32 intPK { get; set; }

        [DataMember, DataColumn(true)]
        [Required]
        public System.String Name { get; set; }

        [DataMember, DataColumn(true)]
        [Required]
        public int DistrictID { get; set; }

        [DataMember, DataColumn(true)]
        [Required]

        public int RegionID { get; set; }

        [DataMember, DataColumn(true)]
        public string RegionName { get; set; }

        [DataMember, DataColumn(true)]
        public string DistrictName { get; set; }

        public virtual District District { get; set; }

    }

    public class University : EntityModelBase
    {
        [DataMember, DataColumn(true)]
        [Key]
        public System.Int32 intPK { get; set; }

        [DataMember, DataColumn(true)]
        [Required]
        public System.String Name { get; set; }
    }


}