using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Collections;
using System.Data.SqlClient;
using Lib.Web.Mvc.JQuery.JqGrid;

using ERP_BEPZA.Web.Areas.PRM.ViewModel;
using ERP_BEPZA.Web.Utility;
using ERP_BEPZA.Domain.PRM;
using ERP_BEPZA.Domain.PGM;
using ERP_BEPZA.DAL.PRM;
using ERP_BEPZA.Web.Areas.PGM.ViewModel;

/*
Revision History (RH):
		SL		: 01
		Author	: AMN
		Date	: 2016-01-25
        SCR     : ERP_BEPZA_PGM_SCR.doc (SCR#55)
		Change	: User can input lower salary step but same grade initially. – Less Increment
		---------
*/

namespace ERP_BEPZA.Web.Areas.PGM.Controllers
{
    public class SalaryStructureController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly EmployeeService _prmEmpservice;
        private readonly SalaryStructureService _salaryStructureService;
        #endregion

        #region Constructor
        public SalaryStructureController(
            PRMCommonSevice prmCommonservice
            , EmployeeService prmEmpservice
            , SalaryStructureService salaryStructureService
            )
        {
            _prmCommonservice = prmCommonservice;
            _prmEmpservice = prmEmpservice;
            _salaryStructureService = salaryStructureService;
        }
        #endregion

        #region Action

        public ViewResult Index()
        {
            var model = new SalaryStructureViewModel();
            model.Id = 0;
            PopulateSalaryStructureModelLists(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SalaryStructureSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _prmCommonservice.PRMUnit.SalaryStructureRepository.GetCount(filterExpression);

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),

                //Page number
                PageIndex = request.PageIndex,

                //Total records count
                TotalRecordsCount = totalRecords
            };

            var list = _prmEmpservice.GetSalaryStructureList(filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,
                viewModel.SalaryScaleId,
                viewModel.GradeId,
                viewModel.StepId,
                out totalRecords).OrderBy(x => x.GradeId);

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.ID), new List<object>()
                {
                    d.ID,
                    d.SalaryScaleId,
                    d.SalaryScaleName,
                    d.GradeId,
                    d.GradeName,
                    d.StepId,
                    d.StepName,
                    d.Amount,
                    "Delete"
                }));
            }

            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult GetSalaryScaleforView()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();

            var gradeList = _prmCommonservice.PRMUnit.SalaryScaleRepository.GetAll().OrderBy(x => x.DateOfEffective).ToList();

            foreach (var data in gradeList)
            {
                dic.Add(data.Id, data.SalaryScaleName);
            }

            return PartialView("Select", dic);
        }

        [NoCache]
        public ActionResult GetGradeforView()
        {
            Dictionary<int, string> grade = new Dictionary<int, string>();
            return PartialView("Select", grade);
        }

        [NoCache]
        public ActionResult GetGradeStepforView()
        {
            Dictionary<int, int> gradeStep = new Dictionary<int, int>();
            return PartialView("_Select", gradeStep);
        }

        [NoCache]
        public ViewResult Details(int id)
        {
            PRM_SalaryStructure prm_salarystructure = _prmCommonservice.PRMUnit.SalaryStructureRepository.GetByID(id);
            return View(prm_salarystructure);
        }

        [NoCache]
        public ActionResult Create()
        {
            //getSalaryHead
            List<PRM_SalaryHead> salaryHead = _prmCommonservice.PRMUnit.SalaryHeadRepository.GetAll().ToList();
            List<SalaryStructureDetailsViewModel> salaryDetails = new List<SalaryStructureDetailsViewModel>();
            SalaryStructureDetailsViewModel childModel;

            foreach (var item in salaryHead)
            {
                childModel = new SalaryStructureDetailsViewModel();
                PopulateHeadAmountTypeList(childModel);
                childModel.HeadId = item.Id;
                childModel.IsTaxable = item.IsTaxable;
                childModel.HeadType = item.HeadType;
                childModel.DisplayHeadName = item.HeadName;
                childModel.AmountType = item.AmountType;
                childModel.IsGrossPayHead = item.IsGrossPayHead;
                salaryDetails.Add(childModel);
            }

            SalaryStructureViewModel mainViewModel = new SalaryStructureViewModel();
            mainViewModel.SalaryStructureDetail = salaryDetails;

            // Fill ViewBag
            ViewBag.salaryscale = GetSalaryScale();
            ViewBag.grade = GetGrade(0);
            ViewBag.gradestep = GetGradeStep(0);

            return View(mainViewModel);
        }

        [HttpPost]
        public ActionResult Create(SalaryStructureViewModel model)
        {
            try
            {
                List<string> errorList = new List<string>();
                if (ModelState.IsValid)
                {
                    var salaryStructure = model.ToEntity();
                    salaryStructure.IUser = User.Identity.Name;
                    salaryStructure.IDate = Common.CurrentDateTime;

                    errorList = _salaryStructureService.GetBusinessLogicValidation(salaryStructure);
                    if (errorList.Count == 0)
                    {
                        foreach (var structure in salaryStructure.PRM_SalaryStructureDetail)
                        {
                            structure.IUser = User.Identity.Name;
                            structure.IDate = DateTime.Now;
                        }

                        _prmCommonservice.PRMUnit.SalaryStructureRepository.Add(salaryStructure);
                        _prmCommonservice.PRMUnit.SalaryStructureRepository.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.Message = errorList.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);

                //getSalaryHead
                List<PRM_SalaryHead> salaryHead = _prmCommonservice.PRMUnit.SalaryHeadRepository.GetAll().ToList();
                List<SalaryStructureDetailsViewModel> salaryDetails = new List<SalaryStructureDetailsViewModel>();
                foreach (var item in salaryHead)
                {
                    SalaryStructureDetailsViewModel childModel = new SalaryStructureDetailsViewModel();
                    PopulateHeadAmountTypeList(childModel);
                    childModel.HeadId = item.Id;
                    childModel.IsTaxable = item.IsTaxable;
                    childModel.HeadType = item.HeadType;
                    childModel.DisplayHeadName = item.HeadName;
                    childModel.AmountType = item.AmountType;
                    salaryDetails.Add(childModel);
                }

                model.SalaryStructureDetail = salaryDetails;
            }

            if (!model.IsSuccessful)
            {
                model.IsConsolidated = _prmEmpservice.PRMUnit.JobGradeRepository.GetByID(model.GradeId).IsConsolidated == true ? true : false;
                foreach (var item in model.SalaryStructureDetail)
                {
                    PopulateHeadAmountTypeList(item);
                }
            }

            FillViewBag(model);

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            List<PRM_SalaryHead> salaryHead = _prmCommonservice.PRMUnit.SalaryHeadRepository.GetAll().ToList();
            List<SalaryStructureDetailsViewModel> salaryDetails = new List<SalaryStructureDetailsViewModel>();

            PRM_SalaryStructure prm_salarystructure = _prmCommonservice.PRMUnit.SalaryStructureRepository.GetByID(id);

            SalaryStructureViewModel model = prm_salarystructure.ToModel();
            ////
            model.InitialBasic = Convert.ToDecimal(prm_salarystructure.PRM_JobGrade.InitialBasic);
            model.YearlyIncrement = Convert.ToDecimal(prm_salarystructure.PRM_JobGrade.YearlyIncrement);
            model.IsConsolidated = prm_salarystructure.PRM_JobGrade.IsConsolidated == true ? true : false;
            model.SalaryScaleId = prm_salarystructure.SalaryScaleId;
            model.GradeId = prm_salarystructure.GradeId;
            model.StepId = prm_salarystructure.StepId;

            SalaryStructureDetailsViewModel childModel;
            PRM_SalaryStructureDetail SalaryStructureDetail;
            foreach (var item in salaryHead)
            {
                childModel = new SalaryStructureDetailsViewModel();
                childModel.HeadId = item.Id;

                SalaryStructureDetail = prm_salarystructure.PRM_SalaryStructureDetail.Where(d => d.HeadId == item.Id).FirstOrDefault();
                if (SalaryStructureDetail != null)
                {
                    childModel = SalaryStructureDetail.ToModel();
                    childModel.IsGrossPayHead = SalaryStructureDetail.PRM_SalaryHead.IsGrossPayHead;
                    childModel.HeadType = SalaryStructureDetail.HeadType;
                    childModel.DisplayHeadName = item.HeadName;
                    childModel.cssSalaryHeadClass = "";
                }
                else
                {
                    childModel.DisplayHeadName = item.HeadName;
                    childModel.HeadType = item.HeadType;
                    childModel.AmountType = item.AmountType;
                    childModel.cssSalaryHeadClass = "cssSalaryHeadClass";
                }

                PopulateHeadAmountTypeList(childModel);

                salaryDetails.Add(childModel);
            }
            ///////// End of structure

            model.SalaryStructureDetail = null;
            model.SalaryStructureDetail = salaryDetails;

            ModelState.Clear();

            #region View Bag
            FillViewBag(model);

            //ViewBag.SelectedId = model.SalaryScaleId;
            ViewBag.SelectedId = model.GradeId;
            //ViewBag.SelectedId = model.StepId;
            #endregion

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SalaryStructureViewModel model)
        {
            try
            {
                List<string> errorList = new List<string>();
                if (ModelState.IsValid)
                {
                    PRM_SalaryStructure salaryStructure = model.ToEntity();
                    salaryStructure.EUser = User.Identity.Name;
                    salaryStructure.EDate = Common.CurrentDateTime;
                    ArrayList lstSalaryStructureDetails = new ArrayList();


                    if (model.SalaryStructureDetail != null)
                    {
                        PRM_SalaryStructureDetail _salaryStructureDetails;
                        foreach (var salaryStructureDetails in model.SalaryStructureDetail)
                        {
                            _salaryStructureDetails = salaryStructureDetails.ToEntity();

                            _salaryStructureDetails.SalaryStructureId = salaryStructure.Id;
                            // if old item then reflection will retrive old IUser & IDate
                            _salaryStructureDetails.IUser = User.Identity.Name;
                            _salaryStructureDetails.IDate = DateTime.Now;
                            _salaryStructureDetails.EDate = DateTime.Now;
                            _salaryStructureDetails.EUser = User.Identity.Name;

                            lstSalaryStructureDetails.Add(_salaryStructureDetails);
                        }
                    }

                    Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                    NavigationList.Add(typeof(PRM_SalaryStructureDetail), lstSalaryStructureDetails);
                    errorList = _salaryStructureService.GetBusinessLogicValidation(salaryStructure);
                    if (errorList.Count == 0)
                    {
                        _prmCommonservice.PRMUnit.SalaryStructureRepository.Update(salaryStructure, NavigationList);
                        _prmCommonservice.PRMUnit.SalaryStructureRepository.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.Message = errorList.FirstOrDefault();// Common.ErrorListToString(errorList);
                    }
                }

            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            if (!model.IsSuccessful)
            {
                model.IsConsolidated = _prmEmpservice.PRMUnit.JobGradeRepository.GetByID(model.GradeId).IsConsolidated == true ? true : false;
                foreach (var item in model.SalaryStructureDetail)
                {
                    PopulateHeadAmountTypeList(item);
                }
            }

            FillViewBag(model);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(PRM_SalaryStructureDetail) };
                _prmCommonservice.PRMUnit.SalaryStructureRepository.Delete(id, allTypes);
                _prmCommonservice.PRMUnit.SalaryStructureRepository.SaveChanges();
                result = true;
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = Common.GetSqlExceptionMessage(sqlException.Number);
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
            catch 
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }
        
        #endregion
        
        #region Other Public Actions

        [NoCache]
        public ActionResult GetJobGradeBySalaryScaleId(int salaryScaleId)
        {
            var gradeList = _prmEmpservice.PRMUnit.JobGradeRepository.Get(t => t.SalaryScaleId == salaryScaleId);

            return Json(
                new
                {
                    JobGrades = gradeList.Select(x => new { Id = x.Id, GradeName = x.GradeName })
                },
                JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public ActionResult GetHeadAmountType()
        {
            var headAmountType = _prmCommonservice.PRMUnit.SalaryHeadRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDllList(headAmountType));
        }

        [NoCache]
        public ActionResult GetStep(int gradeId)
        {
            var grade = _prmEmpservice.PRMUnit.JobGradeRepository.GetByID(gradeId);
            bool? isConsolidate = _prmEmpservice.PRMUnit.JobGradeRepository.GetByID(gradeId).IsConsolidated;
            return Json(
                new
                {
                    steps = GetGradeStep(gradeId).Select(x => new { Id = x.Id, StepName = x.StepName, StepAmount = x.StepAmount }),
                    ic = grade.IsConsolidated,
                    initialBasic = grade.InitialBasic,
                    yearlyIncrement = grade.YearlyIncrement
                },
                JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public JsonResult GetStepAmountByStepId(int stepId)
        {
            var step = _prmEmpservice.PRMUnit.JobGradeStepRepository.GetByID(stepId);

            return Json(
                new
                {
                    stepamount = step.StepAmount
                },
                JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public JsonResult GetGradeId(int salaryScaleId, string gradeName)
        {
            int gradeId = 0;
            var grade = _prmEmpservice.PRMUnit.JobGradeRepository.GetAll().FirstOrDefault(x => x.SalaryScaleId == salaryScaleId && x.GradeName == gradeName);
            if (grade != null)
            {
                gradeId = grade.Id;
            }

            return Json(new
            {
                gradeId = gradeId
            }, JsonRequestBehavior.AllowGet);
        } 
        #endregion
        
        #region Private method

        private IList<PRM_SalaryScale> GetSalaryScale()
        {
            return _prmCommonservice.PRMUnit.SalaryScaleRepository.GetAll().OrderBy(x => x.SalaryScaleName).ToList();
        }

        private IList<PRM_JobGrade> GetGrade(int salaryScaleId = 0)
        {
            if (salaryScaleId == 0)
            {
                return _prmCommonservice.PRMUnit.JobGradeRepository.GetAll().ToList();
            }
            else
            {
                return
                    _prmCommonservice.PRMUnit.JobGradeRepository.Get(t => t.SalaryScaleId == salaryScaleId).ToList();
            }
        }

        private IList<PRM_GradeStep> GetGradeStep(int gradeId)
        {
            IList<PRM_GradeStep> itemList;
            if (gradeId > 0)
            {
                itemList = _prmCommonservice.PRMUnit.JobGradeStepRepository.Get(q => q.JobGradeId == gradeId).OrderBy(t => t.StepName).ToList();
            }
            else
            {
                itemList = new List<PRM_GradeStep>();
            }
            return itemList;
        }
        
        private void FillViewBag(SalaryStructureViewModel model)
        {
            // Fill ViewBag
            IList<PRM_SalaryScale> salScaleList = GetSalaryScale();
            ViewBag.salaryscale = salScaleList;

            IList<PRM_JobGrade> gradeList = GetGrade(model.SalaryScaleId);
            ViewBag.grade = gradeList;

            IList<PRM_GradeStep> GradeStepList = GetGradeStep(model.GradeId);
            ViewBag.gradestep = GradeStepList;
        }

        private void PopulateHeadAmountTypeList(SalaryStructureDetailsViewModel model)
        {
            //dynamic ddlList;
            #region AmountType ddl

            model.HeadAmountTypeList = Common.GetAmountType().ToList();

            #endregion
        }

        private void PopulateSalaryStructureModelLists(SalaryStructureViewModel model)
        {
            model.SalaryScaleList = Common.PopulateSalaryScaleDDL(GetSalaryScale().ToList());
            model.GradeList = Common.PopulateJobGradeDDL(GetGrade(model.SalaryScaleId).ToList());
            model.StepList = Common.PopulateStepList(GetGradeStep(model.GradeId));
        }
        #endregion
    }
}