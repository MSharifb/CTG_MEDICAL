using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class JobGradeController : Controller
    {
        #region Fields

        private readonly PRMCommonSevice _prmCommonservice;

        #endregion

        #region Constructor

        public JobGradeController(PRMCommonSevice ser)
        {
            this._prmCommonservice = ser;
        }

        //public JobGradeNewController(SalaryScaleService serv)
        //{

        //}

        #endregion

        #region Actions

        public ViewResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SalaryScaleViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<SalaryScaleViewModel> list = (from salaryScale in _prmCommonservice.PRMUnit.SalaryScaleRepository.GetAll()
                                               where (viewModel.SalaryScaleName == null || viewModel.SalaryScaleName == "" || viewModel.SalaryScaleName == salaryScale.SalaryScaleName)
                                               && (viewModel.DateOfCirculation == null || viewModel.DateOfCirculation == salaryScale.DateOfCirculation)
                                               && (viewModel.DateOfEffective == null || viewModel.DateOfEffective == salaryScale.DateOfEffective)
                                               select new SalaryScaleViewModel()
                                                 {
                                                     Id = salaryScale.Id,
                                                     SalaryScaleName = salaryScale.SalaryScaleName,
                                                     DateOfCirculation = salaryScale.DateOfCirculation,
                                                     DateOfEffective = salaryScale.DateOfEffective
                                                 }).OrderBy(x => x.DateOfCirculation).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "SalaryScaleName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryScaleName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryScaleName).ToList();
                }
            }

            if (request.SortingName == "DateOfCirculation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateOfCirculation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateOfCirculation).ToList();
                }
            }

            if (request.SortingName == "EffectiveDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateOfEffective).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateOfEffective).ToList();
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
                    d.SalaryScaleName,
                    Convert.ToDateTime(d.DateOfCirculation).ToString(DateAndTime.GlobalDateFormat),
                    Convert.ToDateTime(d.DateOfEffective).ToString(DateAndTime.GlobalDateFormat),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetListJobGrade(JqGridRequest request, int Id)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            var list = _prmCommonservice.PRMUnit.JobGradeRepository.Get(t => t.SalaryScaleId == Id);

            foreach (PRM_JobGrade d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.GradeName,
                    d.InitialBasic,
                    d.PayScale,                   
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetListJobGradeStep(JqGridRequest request, int Id)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            var list = _prmCommonservice.PRMUnit.JobGradeStepRepository.Get(t => t.JobGradeId == Id);

            foreach (PRM_GradeStep d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.StepName,
                    d.StepAmount,                    
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new SalaryScaleViewModel();
            model.JobGradeDetails = new List<JobGradeViewModel>()
                {
                    new JobGradeViewModel()
                    {
                        
                    }
                };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SalaryScaleViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {

                    model = GetInsertUserAuditInfo(model);
                    var entity = CreateEntity(model, true);

                    if (errorList.Length == 0)
                    {
                        _prmCommonservice.PRMUnit.SalaryScaleRepository.Add(entity);
                        _prmCommonservice.PRMUnit.SalaryScaleRepository.SaveChanges();
                        model.errClass = "success";
                        model.Message = Resources.ErrorMessages.InsertSuccessful;
                       // return RedirectToAction("Index");
                      //return RedirectToAction("Edit", new { id = entity.Id, type = "Isuccess" });
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id, string type)
        {
            var SalaryScaleEntity = _prmCommonservice.PRMUnit.SalaryScaleRepository.GetByID(id);
            var parentModel = SalaryScaleEntity.ToModel();
            //parentModel.IdT = parentModel.Id;

            //Job Grade
            IList<JobGradeViewModel> listJobGrade = new List<JobGradeViewModel>();
            foreach (var item in SalaryScaleEntity.PRM_JobGrade)
            {
                listJobGrade.Add(item.ToModel());
            }
            parentModel.JobGradeDetails = listJobGrade;
            //

            if (type == "success")
            {
                parentModel.Message = Resources.ErrorMessages.UpdateSuccessful;
                parentModel.errClass = "success";               
            }       
            return View("Edit", parentModel);

        }


        [HttpPost]
        public ActionResult Edit(SalaryScaleViewModel model)
        {
            try
            {
                string errorList = "";

                if (ModelState.IsValid)
                {
                    var entity = CreateEntity(model, false);

                    _prmCommonservice.PRMUnit.SalaryScaleRepository.Update(entity);
                    _prmCommonservice.PRMUnit.SalaryScaleRepository.SaveChanges();
                    //  model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    model.errClass = "success";
                    model.Message = Resources.ErrorMessages.UpdateSuccessful;

                    return RedirectToAction("Edit", "JobGrade", new { id = entity.Id, type = "success" });
                }

                if (errorList.Length > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }

            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }

            return View(model);
        }

        public ActionResult AddGradeStep(int id)
        {
            var JobGradeEntity = _prmCommonservice.PRMUnit.JobGradeRepository.GetByID(id);
            var parentModel = JobGradeEntity.ToModel();
            // Grade Step
            IList<GradeStepViewModel> listGradeStep = new List<GradeStepViewModel>();
            foreach (var item in JobGradeEntity.PRM_GradeStep)
            {
                listGradeStep.Add(item.ToModel());
            }
            parentModel.GradeStep = listGradeStep;
            //

            return View("AddJobGrade", parentModel);
        }


        [HttpPost]
        public ActionResult AddGradeStep(JobGradeViewModel model)
        {
            try
            {
                string errorList = "";

                var JobGrdeEntity = _prmCommonservice.PRMUnit.JobGradeRepository.GetByID(model.Id);
                var parentModel = JobGrdeEntity.ToModel();
                parentModel.SalaryScaleId = parentModel.SalaryScaleId;

                if (model.GradeStep.Count > 0)
                {
                    // Job Grade
                    foreach (var c in model.GradeStep)
                    {
                        var prm_GradeStep = new PRM_GradeStep();

                        prm_GradeStep.Id = c.Id;
                        prm_GradeStep.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                        prm_GradeStep.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                        prm_GradeStep.EUser = c.EUser;
                        prm_GradeStep.EDate = c.EDate;
                        prm_GradeStep.StepName = c.StepName;
                        prm_GradeStep.StepAmount = c.StepAmount;
                        prm_GradeStep.JobGradeId = model.Id;
                        //entity.PRM_GradeStep.Add(prm_GradeStep);

                        if (c.Id == 0)
                        {
                            _prmCommonservice.PRMUnit.JobGradeStepRepository.Add(prm_GradeStep);
                        }
                        else
                        {
                            _prmCommonservice.PRMUnit.JobGradeStepRepository.Update(prm_GradeStep);
                        }
                    }


                    var prm_JobGrade = new PRM_JobGrade();
                    prm_JobGrade.Id = JobGrdeEntity.Id;
                    prm_JobGrade.GradeName = JobGrdeEntity.GradeName;
                    prm_JobGrade.GradeCode = JobGrdeEntity.GradeCode;
                    prm_JobGrade.DateOfEffective = JobGrdeEntity.DateOfEffective;
                    prm_JobGrade.SalaryScaleId = JobGrdeEntity.SalaryScaleId;
                    //
                    Tuple<decimal, decimal, string> InitialAndLastBasic = GetInitialAndLastBasic(model);
                    prm_JobGrade.InitialBasic = InitialAndLastBasic.Item1;
                    prm_JobGrade.LastBasic = InitialAndLastBasic.Item2;
                    prm_JobGrade.PayScale = InitialAndLastBasic.Item3;

                    //

                    _prmCommonservice.PRMUnit.JobGradeRepository.Update(prm_JobGrade);

                    _prmCommonservice.PRMUnit.JobGradeStepRepository.SaveChanges();
                    _prmCommonservice.PRMUnit.JobGradeRepository.SaveChanges();


                }
                // return RedirectToAction("Index");
                //return View(model);              
                return RedirectToAction("Edit", "JobGrade", new { id = JobGrdeEntity.SalaryScaleId });
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            return View(model);

        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonservice.PRMUnit.SalaryScaleRepository.Delete(id);
                _prmCommonservice.PRMUnit.SalaryScaleRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
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
            catch (Exception ex)
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        [HttpPost, ActionName("DeleteJobGrade")]
        public JsonResult DeleteJobGradeConfirmed(int id)
        {

            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _prmCommonservice.PRMUnit.JobGradeRepository.Delete(id);
                _prmCommonservice.PRMUnit.JobGradeRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
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
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }


            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });

        }

        [HttpPost, ActionName("DeleteJobGradeStep")]
        public JsonResult DeleteJobGradeStepConfirmed(int id)
        {

            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _prmCommonservice.PRMUnit.JobGradeStepRepository.Delete(id);
                _prmCommonservice.PRMUnit.JobGradeStepRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
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
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }


            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });

        }
        #endregion

        #region Private Method

        private PRM_SalaryScale CreateEntity(SalaryScaleViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();
          //  entity.Id = model.Id;
            foreach (var c in model.JobGradeDetails)
            {
                var prm_JobGrade = new PRM_JobGrade();
                prm_JobGrade.DateOfEffective = Convert.ToDateTime(model.DateOfEffective);

                prm_JobGrade.Id = c.Id;
                prm_JobGrade.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                prm_JobGrade.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                prm_JobGrade.EUser = c.EUser;
                prm_JobGrade.EDate = c.EDate;
                prm_JobGrade.GradeName = c.GradeName;
                prm_JobGrade.YearlyIncrement = 0M;              

                if (pAddEdit)
                {
                    prm_JobGrade.IUser = User.Identity.Name;
                    prm_JobGrade.IDate = DateTime.Now;

                    entity.PRM_JobGrade.Add(prm_JobGrade);
                }
                else
                {
                    prm_JobGrade.SalaryScaleId = model.Id;

                    if (c.Id != 0)
                    {
                        var temp = _prmCommonservice.PRMUnit.JobGradeRepository.GetByID(c.Id);
                        prm_JobGrade.PayScale = temp.PayScale;
                        prm_JobGrade.InitialBasic = temp.InitialBasic;
                    }


                    prm_JobGrade.EUser = User.Identity.Name;
                    prm_JobGrade.EDate = DateTime.Now;


                    if (c.Id == 0)
                    {
                        _prmCommonservice.PRMUnit.JobGradeRepository.Add(prm_JobGrade);
                    }
                    else
                    {
                        _prmCommonservice.PRMUnit.JobGradeRepository.Update(prm_JobGrade);

                    }
                    _prmCommonservice.PRMUnit.JobGradeRepository.SaveChanges();
                }
            }

            return entity;
        }

        private Tuple<decimal, decimal, string> GetInitialAndLastBasic(JobGradeViewModel model)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            decimal minValue = model.GradeStep.Min(t => t.StepAmount);
            decimal maxValue = model.GradeStep.Max(t => t.StepAmount);

            foreach (var item in model.GradeStep)
            {
                sb.Append(Convert.ToInt32(item.StepAmount));
                sb.Append("-");
            }

            string payScale = sb.ToString().TrimEnd('-');

            Tuple<decimal, decimal, string> tuple = new Tuple<decimal, decimal, string>(minValue, maxValue, payScale);
            return tuple;
        }

        private PRM_SalaryScale GetInsertUserAuditInfo(PRM_SalaryScale entity)
        {
            entity.IUser = User.Identity.Name;
            entity.IDate = DateTime.Now;

            foreach (var child in entity.PRM_JobGrade)
            {
                child.IUser = User.Identity.Name;
                child.IDate = DateTime.Now;

            }
            return entity;
        }

        private SalaryScaleViewModel GetInsertUserAuditInfo(SalaryScaleViewModel model)
        {
            model.IUser = User.Identity.Name;
            model.IDate = DateTime.Now;

            foreach (var child in model.JobGradeDetails)
            {
                child.IUser = User.Identity.Name;
                child.IDate = DateTime.Now;
                //  child.DateOfEffective = Convert.ToDateTime(model.DateOfEffective);

            }
            return model;
        }

        #endregion
    }
}