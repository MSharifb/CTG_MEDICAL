using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class OnlineApplicantController : Controller
    {
        #region Fields
        private readonly EmployeeService _empService;
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public OnlineApplicantController(EmployeeService empService, PRMCommonSevice prmCommonService)
        {
            this._empService = empService;
            this._prmCommonservice = prmCommonService;
        }
        #endregion

        // GET: PRM/OnlineApplicant
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, OnlineApplicantViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<OnlineApplicantViewModel> list = (from appSrtLst in _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll()
                                                   join jobPost in _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetAll() on appSrtLst.CircularID equals jobPost.Id
                                                   join de in _prmCommonservice.PRMUnit.DesignationRepository.GetAll() on jobPost.DesignationId equals de.Id
                                                   where (model.JobAdvertisementInfoId == 0 || model.JobAdvertisementInfoId == jobPost.JobAdvertisementInfoId)

                                                       && (model.DesignationId == null || model.DesignationId == 0 || model.DesignationId == de.Id)
                                                       && (string.IsNullOrEmpty(model.ApplicantName) || appSrtLst.Name.Contains(model.ApplicantName))
                                                        && (string.IsNullOrEmpty(model.FatherName) || appSrtLst.FathersName.Contains(model.FatherName))
                                                         && (string.IsNullOrEmpty(model.MotherName) || appSrtLst.MothersName.Contains(model.MotherName))
                                                          && (string.IsNullOrEmpty(model.NationalId) || appSrtLst.NationalId.Contains(model.NationalId))
                                                           && (string.IsNullOrEmpty(model.MobileNo) || appSrtLst.MobilePhoneNo.Contains(model.MobileNo))

                                                   select new OnlineApplicantViewModel()
                                                   {
                                                       Id = appSrtLst.intPK,
                                                       ApplicantName = appSrtLst.Name,
                                                       DateOfBirth = appSrtLst.DateOfBirth,
                                                       MobileNo = appSrtLst.MobilePhoneNo,
                                                       FatherName = appSrtLst.FathersName,
                                                       MotherName = appSrtLst.MothersName,
                                                       NationalId = appSrtLst.NationalId,
                                                       JobAdvertisementInfoId = jobPost.JobAdvertisementInfoId,
                                                       DesignationName = de.Name,
                                                       DesignationId = de.Id
                                                   }).OrderBy(x => x.ApplicantName).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ApplicantName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApplicantName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApplicantName).ToList();
                }
            }


            if (request.SortingName == "DateOfBirth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateOfBirth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateOfBirth).ToList();
                }
            }

            if (request.SortingName == "MobileNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MobileNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MobileNo).ToList();
                }
            }

            if (request.SortingName == "DesignationName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DesignationName).ToList();
                }
            }



            if (request.SortingName == "FatherName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FatherName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FatherName).ToList();
                }
            }
            if (request.SortingName == "MotherName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MotherName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MotherName).ToList();
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
                    d.ApplicantName,
                    d.DateOfBirth.ToString("dd-MM-yyyy"),
                    d.MobileNo,
                    d.FatherName,
                    d.MotherName,
                    d.NationalId,
                    d.DesignationName,
                    d.DesignationId,
                    "View",
                    "AdmitCard"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Preview(Int32 id)
        {
            //GeneralInfo generalInfo = new GeneralInfo();
            //AddressInfo addressInfo = new AddressInfo();
            //MaritalInfo maritalInfo = new MaritalInfo();
            //EducationalInfo educationalInfo = new EducationalInfo();

            EREC_tblgeneralinfo generalInfo = new EREC_tblgeneralinfo();
            EREC_tbladdress addressInfo = new EREC_tbladdress();

            EREC_tblmaritalinfo maritalInfo = new EREC_tblmaritalinfo();
            EREC_tbleducationalinfo educationalInfo = new EREC_tbleducationalinfo();


            generalInfo = _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll().Where(e => e.intPK == id).FirstOrDefault(); // registrationInfoRepository.Find_GeneralInfo(_id, "", "");
            if (generalInfo == null)
            {
                generalInfo = new EREC_tblgeneralinfo();
            }
            else
            {
                Session["ImgByte"] = generalInfo.Photo;


                addressInfo = _prmCommonservice.PRMUnit.ERECtblAddressRepository.GetAll().Where(e => e.GeneralID == id).FirstOrDefault(); //registrationInfoRepository.Find_AddressInfo(generalInfo.intPK);
                if (addressInfo == null)
                {
                    addressInfo = new EREC_tbladdress();
                    addressInfo.GeneralID = generalInfo.intPK;
                }

                maritalInfo = _prmCommonservice.PRMUnit.ERECmaritalinfoRepository.GetAll().Where(e => e.GeneralID == id).FirstOrDefault(); // registrationInfoRepository.Find_MaritalInfo(generalInfo.intPK);
                if (maritalInfo == null)
                {
                    maritalInfo = new EREC_tblmaritalinfo();
                    maritalInfo.GeneralID = generalInfo.intPK;
                }

                educationalInfo = _prmCommonservice.PRMUnit.ERECeducationalinfoRepository.GetAll().Where(e => e.GeneralID == id).FirstOrDefault(); //registrationInfoRepository.Find_EducationalInfo(generalInfo.intPK);
                if (educationalInfo == null)
                {
                    educationalInfo = new EREC_tbleducationalinfo();
                    educationalInfo.GeneralID = generalInfo.intPK;
                }
                else
                {
                    // Session["ImgByteSignature"] = educationalInfo.Signature;
                }
            }

            //}

            var model = new ApplicationFormModel { GeneralInfo = generalInfo, AddressInfo = addressInfo, MaritalInfo = maritalInfo, EducationalInfo = educationalInfo }; // 

            //var item = _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetByID(generalInfo.CircularID); // advertisementInfoRepository.FindAdvertisementInfoById(generalInfo.CircularID);
            var item = _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetByID(generalInfo.CircularID);
            model.AgeDeadline = item.PRM_JobAdvertisementInfo.AgeCalDate.ToString("dd-MM-yyyy");
            model.Designation = item.PRM_Designation.Name;


            List<EREC_tblCountry> countryList = _prmCommonservice.PRMUnit.ERECtblCountryRepository.GetAll().ToList(); //CountryBLL.GetAllCountry();
            ViewBag.Country = countryList;
            model.CountryList = new SelectList(countryList, "intPK", "Name");
            List<PRM_Religion> religionList = _prmCommonservice.PRMUnit.Religion.GetAll().ToList();
            model.ReligionList = new SelectList(religionList, "Id", "Name");

            List<EREC_tblDistrict> distictList = _prmCommonservice.PRMUnit.ERECtblDistrictRepository.GetAll().ToList(); // new DistrictRepository().All;
            ViewBag.Distict = distictList;

            List<PRM_UniversityAndBoard> boardList = _prmCommonservice.PRMUnit.UniversityAndBoardRepository.GetAll().ToList(); //EducationSetupBLL.GetAllBoard();
            ViewBag.boardList = boardList;

            List<EREC_tblExamination> ExaminationList = _prmCommonservice.PRMUnit.ERECtblExaminationRepository.GetAll().ToList(); //EducationSetupBLL.GetAllExamination();
            ViewBag.ExaminationList = ExaminationList;

            List<PRM_DegreeLevel> DegreeLevel = _prmCommonservice.PRMUnit.ExamDegreeLavelRepository.GetAll().ToList(); // EducationSetupBLL.GetAllDegree();
            ViewBag.DegreeLevel = DegreeLevel;

            List<EREC_tblUpazila> UpazilaList = _prmCommonservice.PRMUnit.ERECtblUpazilaRepository.GetAll().ToList(); //new UpazilaRepository().All;
            ViewBag.Upazila = UpazilaList;

            // List<PRM_UniversityAndBoard> UniversityList = _prmCommonservice.PRMUnit.UniversityAndBoardRepository.GetAll().ToList(); //EducationSetupBLL.GetAllUniversity();
            ViewBag.UniversityList = boardList; // UniversityList;


            List<PRM_SubjectGroup> ExamGroupList = _prmCommonservice.PRMUnit.SubjectGroupRepository.GetAll().ToList(); // EducationSetupBLL.GetAllGroup(0);
            ViewBag.ExamGroupList = ExamGroupList;

            //List<PRM_SubjectGroup> SubjectList = _prmCommonservice.PRMUnit.SubjectGroupRepository.GetAll().ToList(); // EducationSetupBLL.GetAllGroup(1);
            ViewBag.SubjectList = ExamGroupList; // SubjectList;

            List<PRM_ProfessionalCertificate> CertificateCategoryList = _prmCommonservice.PRMUnit.ProfessionalCertificateRepository.GetAll().ToList(); // EducationSetupBLL.GetAllGroup(2);
            ViewBag.CertificateCategoryList = CertificateCategoryList;

            List<string> DivisionList = new List<string>();

            DivisionList.Add("1st Class");
            DivisionList.Add("2nd Class");
            DivisionList.Add("3rd Class");
            ViewBag.DivisionList = DivisionList;

            List<string> ScaleList = new List<string>();
            ScaleList.Add("4");
            ScaleList.Add("5");
            ViewBag.ScaleList = ScaleList;

            List<string> PassingYearList = new List<string>();

            for (int i = DateTime.Now.Year - 25; i <= DateTime.Now.Year; i++)
            {
                PassingYearList.Add(i.ToString());
            }

            ViewBag.PassingYearList = PassingYearList;

            return View(model);
        }
        [NoCache]
        public ActionResult DesignationListView()
        {
            var designations = (from jobPost in _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetAll()
                                join de in _prmCommonservice.PRMUnit.DesignationRepository.GetAll() on jobPost.DesignationId equals de.Id
                                select de).OrderBy(o => o.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(designations));
        }

        //
        public ActionResult AdmitCardView(Int32 id)
        {
            EREC_tblgeneralinfo generalInfo = new EREC_tblgeneralinfo();
            EREC_tblmaritalinfo maritalInfo = new EREC_tblmaritalinfo();
            EREC_tbladdress addressInfo = new EREC_tbladdress();

            PRM_JobAdvertisementPostDetail postDetail = null;
            if (id != 0)
            {
                generalInfo = _prmCommonservice.PRMUnit.ERECgeneralinfoRepository.GetAll().Where(e => e.intPK == id).FirstOrDefault();
                postDetail = _prmCommonservice.PRMUnit.JobAdvertisementPostDetailRepository.GetByID(generalInfo.CircularID);// _prmCommonservice.PRMUnit.JobAdvertisementInfoRepository.GetByID(generalInfo.CircularID);

                Session["ImgByte"] = generalInfo.Photo;

                maritalInfo = _prmCommonservice.PRMUnit.ERECmaritalinfoRepository.GetAll().Where(e => e.GeneralID == id).FirstOrDefault();
                addressInfo = _prmCommonservice.PRMUnit.ERECtblAddressRepository.GetAll().Where(e => e.GeneralID == id).FirstOrDefault();

                List<EREC_tblDistrict> distictList = _prmCommonservice.PRMUnit.ERECtblDistrictRepository.GetAll().ToList();
                ViewBag.Distict = distictList.Where(p => p.intPK == addressInfo.PermanentDistrict).SingleOrDefault().Name;
            }
            //}

            var model = new ApplicationFormModel { GeneralInfo = generalInfo, MaritalInfo = maritalInfo };
            if (postDetail != null)
                model.Designation = postDetail.PRM_Designation.Name;

            return View("AdmitCard", model);



        }

    }
}