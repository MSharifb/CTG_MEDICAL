using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.ForfeitedRuleViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class ForfeitedRuleController : Controller
    {

        #region Fields
        private readonly CPFCommonService _cpfCommonservice;

        #endregion

        #region Constructor

        public ForfeitedRuleController(CPFCommonService cpfCommonservice)
        {
            this._cpfCommonservice = cpfCommonservice;

        }

        #endregion

        #region Actions

        [NoCache]
        public ActionResult Index()
        {
            ForfeitedRuleViewModel model = new ForfeitedRuleViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, ForfeitedRuleViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ForfeitedRuleViewModel> list = new List<ForfeitedRuleViewModel>();

            list = (from forfeit in _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.GetAll()
                    select new ForfeitedRuleViewModel()
                    {
                        Id = forfeit.Id,
                        strRuleName = forfeit.FromServiceLength.ToString() + "-" + forfeit.ToServiceLength.ToString(),
                        FromServiceLength = forfeit.FromServiceLength,
                        ToServiceLength = forfeit.ToServiceLength,
                        ForfeitedRate = forfeit.ForfeitedRate
                    }).ToList();

            if (request.Searching)
            {
                if (model != null)
                {
                    if (model.FromServiceLength != null)
                    {
                        list = list.Where(d => d.FromServiceLength >= model.FromServiceLength && d.ToServiceLength <= model.ToServiceLength).ToList();
                    }
                    //if (model.ToServiceLength != null)
                    //{
                    //    list = list.Where(d=>d.FromServiceLength)
                    //}


                }

            }

            totalRecords = list == null ? 0 : list.Count();

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
                    d.strRuleName,
                    d.FromServiceLength,
                    d.ToServiceLength,
                    d.ForfeitedRate,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            ForfeitedRuleViewModel model = new ForfeitedRuleViewModel();
            var list = _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.GetAll().ToList();
            if (list.Count > 0)
            {
                decimal maxToServiceLenth = list.Max(d => d.ToServiceLength);
                if (maxToServiceLenth > 0)
                {
                    model.FromServiceLength = maxToServiceLenth + Convert.ToDecimal(0.1);
                }
            }
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(ForfeitedRuleViewModel model)
        {
            string errorList = string.Empty;

            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model, "insert");

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var obj = model.ToEntity();
                obj.IUser = User.Identity.Name;
                obj.IDate = Common.CurrentDateTime;

                try
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = "Forfeited Rate can not be duplicate.";

                        return View(model);
                    }
                    _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.Add(obj);
                    _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.SaveChanges();
                    model.IsError = 0;
                    //model.IsError = 3;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return RedirectToAction("Index");
                    //return RedirectToAction("Index",model);
                }

                catch (Exception ex)
                {
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
            }
            else
            {
                model.ErrMsg = errorList;
            }


            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {

            CPF_ForfeitedRule obj = _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.GetByID(id);

            ForfeitedRuleViewModel model = obj.ToModel();

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(ForfeitedRuleViewModel model)
        {

            string errorList = string.Empty;

            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model, "update");

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                CPF_ForfeitedRule obj = model.ToEntity();
                obj.EUser = User.Identity.Name;
                obj.EDate = Common.CurrentDateTime;

                if (CheckDuplicateEntry(model, model.Id))
                {
                    //IList<CPF_ForfeitedRuleGroup> headGroupList = GetHeadGroup(model.HeadType);
                    //ViewBag.HeadGroup = headGroupList;
                    model.strMessage = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                    return View(model);
                }

                Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.Update(obj, NavigationList);
                _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.SaveChanges();
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                //return RedirectToAction("Index");
                return RedirectToAction("Index", model);
            }
            else
            {
                model.ErrMsg = errorList;
            }


            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                var list = _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.GetAll();
                var maxId = list.Max(d => d.Id);

                if (maxId == id)
                {
                    _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.Delete(id);
                    _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.SaveChanges();
                    result = true;

                }
                else
                {
                    result = false;
                    errMsg = "You have to delete last record";

                }

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
            catch (Exception)
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? "Information has been deleted successfully." : errMsg
            });
        }


        #endregion

        #region  Utilities----------------------------

        private string GetBusinessLogicValidation(ForfeitedRuleViewModel model, string action)
        {
            string errorMessage = string.Empty;
            var list = _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.GetAll();
            var count = list.Count();
            var firsZero = list.Where(d => d.FromServiceLength == 0).ToList();


            if (count > 0 && firsZero.Count() == 0 && model.FromServiceLength != 0)
            {
                errorMessage = "Initially, 'From Service length ' must be zero.";

            }
            else if (model.FromServiceLength > model.ToServiceLength)
            {
                errorMessage = "'From Service Length ' must be lower than the 'To service length '";
            }

            else if (model.ForfeitedRate < 0 || model.ForfeitedRate > 100)
            {
                errorMessage = "Forfeited Rate must be between 0 and 100.";
            }
            else if (count == 0)
            {
                if (model.FromServiceLength != 0)
                {
                    errorMessage = "Initially, 'From Service length ' must be zero.";
                }
            }
            else
            {
                decimal maxToServiceLenth = list.Max(d => d.ToServiceLength);
                decimal maxFromServiceLength = list.Max(d => d.FromServiceLength);
                decimal fromServiceLenth = 0;
                if (action == "update")
                {
                    var maxId = list.Max(d => d.Id);
                    if (maxId == model.Id)
                    {
                        //if (maxFromServiceLength != model.FromServiceLength)
                        //{
                        //    errorMessage = "'From Service Length' will be " + model.FromServiceLength;

                        //}
                    }
                    else
                    {
                        errorMessage = "You can update only last record";
                    }

                }
                else
                {
                    if (maxToServiceLenth > 0)
                    {
                        fromServiceLenth = maxToServiceLenth + Convert.ToDecimal(0.1);
                    }

                    var fromlist = list.Where(d => d.FromServiceLength <= model.FromServiceLength && model.FromServiceLength <= d.ToServiceLength).ToList();
                    var tolist = list.Where(d => d.FromServiceLength <= model.ToServiceLength && model.ToServiceLength <= d.ToServiceLength).ToList();

                    if (fromlist.Count() > 0)
                    {
                        errorMessage = "'From Service Length' will not overlap with any other record.";
                    }
                    else if (tolist.Count() > 0)
                    {
                        errorMessage = "'To Service Length ' will not overlap with any other record.";
                    }
                    //else if (model.FromServiceLength > fromServiceLenth)
                    //{
                    //    errorMessage = "'From Service Length' will be " + fromServiceLenth.ToString();
                    //}
                }


            }
            return errorMessage;
        }

        private bool CheckDuplicateEntry(ForfeitedRuleViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.Get(q => q.ForfeitedRate == model.ForfeitedRate).Any();
            }

            else
            {
                return _cpfCommonservice.CPFUnit.ForfeitedRuleRepository.Get(q => q.ForfeitedRate == model.ForfeitedRate && strMode != q.Id).Any();
            }

        }

        #endregion

    }
}
