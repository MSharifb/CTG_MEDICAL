using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
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
    public class PracticeController : Controller
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public PracticeController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }

        #endregion

        #region Properties
        //public IFormsAuthentication FormsAuth
        //{
        //    get;
        //    private set;
        //}

        //public IMembershipService MembershipService
        //{
        //    get;
        //    private set;
        //}
        #endregion
        // GET: PRM/Practice
        public ActionResult Index()
        {
            // PracticeViewModel model = new PracticeViewModel();
            //model.PracticeId = 0;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, PracticeViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<PracticeViewModel> list = (from PV in _prmCommonservice.PRMUnit.PracticeRepository.GetAll()
                                            select new PracticeViewModel()
                                            {
                                                PracticeName = PV.PracticeName,
                                                Address = PV.Address,
                                                PracticeId = PV.PracticeId
                                            }).ToList();
            totalRecords = list == null ? 0 : list.Count;
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
                    d.PracticeId,
                    d.PracticeName,
                    d.Address,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [HttpGet]
        public ActionResult Create()
        {
            PracticeViewModel model = new PracticeViewModel();           
            return View(model);
           // return View();
        }
        [HttpPost]
        public ActionResult Create(PracticeViewModel model)
        {
            
                if (ModelState.IsValid)
                {

                    var entity = model.ToEntity();

                   
                        _prmCommonservice.PRMUnit.PracticeRepository.Add(entity);
                        _prmCommonservice.PRMUnit.PracticeRepository.SaveChanges();
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                   
                }
                
            return View(model);
        }
    }
}