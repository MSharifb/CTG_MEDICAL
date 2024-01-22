using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.BankInformationViewModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Data.SqlClient;
using BEPZA_MEDICAL.Domain.CPF;
using BEPZA_MEDICAL.Web.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.DAL.CPF;
using System.Data;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.PRM;
using System.Collections;
namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class BankInformationController : Controller
    {
       
        #region Fields
        private readonly CPFCommonService _cpfCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Constructor

        public BankInformationController(CPFCommonService cpfCommonservice, PRMCommonSevice prmCommonservice)
        {
            this._cpfCommonservice = cpfCommonservice;
            this._prmCommonservice = prmCommonservice;
        }

        #endregion

        #region Action
      
        [NoCache]
        public ViewResult BankInformationIndex(string name)
        {
            BankInformationViewModel model = new BankInformationViewModel();
            populateDropdown(model);

            return View("Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, BankInformationSearchViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<BankInformationSearchViewModel> list = (from BA in _cpfCommonservice.CPFUnit.BankAccountInfoRepository.GetAll()                                           
                                           join B in _prmCommonservice.PRMUnit.BankNameRepository.GetAll() on BA.BankId equals B.Id
                                           join BB in _prmCommonservice.PRMUnit.BankBranchRepository.GetAll() on BA.BranchId equals BB.Id
                                                        
                                                         select new BankInformationSearchViewModel()
                                           {
                                               Id = BA.Id,
                                               AccountNo = BA.AccountNo,                                              
                                               BankName = B.Name,
                                               BranchName = BB.Name,
                                               OpeningBalance=BA.OpeningBalance,
                                               BankId = B.Id,
                                               BranchId=BB.Id
                                           }).ToList();
            if (request.Searching)
            {
                if (model != null) {
                    if (model.BankId != null)
                    {
                        list = list.Where(d => d.BankId == model.BankId).ToList();
                    }
                    if (model.BranchId != null)
                    {
                        list = list.Where(d => d.BranchId == model.BranchId).ToList();
                    }
                    if (model.AccountNo != null)
                    {
                        list = list.Where(d => d.AccountNo == model.AccountNo).ToList();
                    }
                
                }
                   
            }

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
                    d.Id,
                    d.AccountNo,
                    d.BankId, 
                    d.BankName,
                    d.BranchId,
                    d.BranchName,   
                    d.OpeningBalance,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {           
            BankInformationViewModel model = new BankInformationViewModel();           
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(BankInformationViewModel model)
        {
            string errorList = string.Empty;

            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var obj = model.ToEntity();
                obj.IUser = User.Identity.Name;
                obj.IDate = Common.CurrentDateTime;

                try
                {
                    var list = _cpfCommonservice.CPFUnit.BankAccountInfoRepository.GetAll().Where(d => d.BankId == model.BankId && d.AccountNo == model.AccountNo);
                    var count = list.Count();
                   
               
                    //if (CheckDuplicateEntry(model, model.Id))
                    //{
                        if (count == 0)
                        {
                            _cpfCommonservice.CPFUnit.BankAccountInfoRepository.Add(obj);
                            _cpfCommonservice.CPFUnit.BankAccountInfoRepository.SaveChanges();
                            model.IsError = 0;
                            model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                            return RedirectToAction("BankInformationIndex", model);

                        }
                        else {
                            model.ErrMsg = "Duplicate Entry";
                        
                        }
                    
                    //}
                   
                    
                }

                catch(Exception ex)
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

            //ViewBag.HeadGroup = GetHeadGroup(model.HeadType);
            populateDropdown(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
          
            CPF_BankAccountInfo obj = _cpfCommonservice.CPFUnit.BankAccountInfoRepository.GetByID(id);
            ////ViewBag.SelectedId = obj.GroupId;
            ////ViewBag.HeadGroup = GetHeadGroup(obj.HeadType);
            BankInformationViewModel model = obj.ToModel();
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(BankInformationViewModel model)
        {

            string errorList = string.Empty;

            model.IsError = 1;
            //model.IsError = 4;

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                CPF_BankAccountInfo obj = model.ToEntity();
                obj.EUser = User.Identity.Name;
                obj.EDate = Common.CurrentDateTime;
                if (CheckDuplicateEntry(model, model.Id))
                {
                   
                    model.strMessage = Resources.ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                    return View(model);

                }

                Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                _cpfCommonservice.CPFUnit.BankAccountInfoRepository.Update(obj, NavigationList);
                _cpfCommonservice.CPFUnit.BankAccountInfoRepository.SaveChanges();
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                //return RedirectToAction("Index");
                return RedirectToAction("BankInformationIndex", model);
            }
            else
            {
                model.ErrMsg = errorList;
            }
            
            populateDropdown(model);
            ////ViewBag.HeadGroup = GetHeadGroup(model.HeadType);
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
                //List<Type> allTypes = new List<Type> { typeof(CPF_BankAccountInfo) };
                _cpfCommonservice.CPFUnit.BankAccountInfoRepository.Delete(id);
                _cpfCommonservice.CPFUnit.BankAccountInfoRepository.SaveChanges();
                result = true;
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

        #region  Utilities

        [NoCache]
        public ActionResult GetHeadTypeView()
        {
            Dictionary<string, string> headType = new Dictionary<string, string>();

            headType.Add("Addition", "Addition");
            headType.Add("Deduction", "Deduction");

            return PartialView("_Select", headType);
        }

        [NoCache]
        public ActionResult GetAmountTypeView()
        {
            Dictionary<string, string> amountType = new Dictionary<string, string>();

            amountType.Add("Fixed", "Fixed");
            amountType.Add("Percent", "Percent");

            return PartialView("_Select", amountType);
        }
        [NoCache]
        public ActionResult BankList()
        {
            var divisions = _prmCommonservice.PRMUnit.BankNameRepository.GetAll().OrderBy(x => x.Name).ToList();

            return PartialView("_Select", Common.PopulateDllList(divisions));
        }

        [NoCache]
        public ActionResult BankBranchList()
        {
            var divisions = _prmCommonservice.PRMUnit.BankBranchRepository.GetAll().OrderBy(x => x.Name).ToList();

            return PartialView("_Select", Common.PopulateDllList(divisions));
        }

        private string GetBusinessLogicValidation(BankInformationViewModel model)
        {
            string errorMessage = string.Empty;
           
            return errorMessage;
        }

        private IList<PRM_BankName> GetBankList(int? bankId)
        {
            IList<PRM_BankName> itemList;
            if (bankId != null)
            {
                itemList = _prmCommonservice.PRMUnit.BankNameRepository.Get(q => q.Id == bankId).OrderBy(x => x.Name).ToList();
            }
            else
            {

                itemList = _prmCommonservice.PRMUnit.BankNameRepository.Get().OrderBy(x => x.Name).ToList();
            }
            return itemList;
        }
        
        private IList<PRM_BankBranch> GetBankBranchList(int? branchId)
        {
            IList<PRM_BankBranch> itemList;
            if (branchId != null)
            {
                itemList = _prmCommonservice.PRMUnit.BankBranchRepository.Get(q => q.Id == branchId).OrderBy(x => x.Name).ToList();
            }
            else
            {

                itemList = _prmCommonservice.PRMUnit.BankBranchRepository.Get().OrderBy(x => x.Name).ToList();
            }
            return itemList;
        }

        private void populateDropdown(BankInformationViewModel model)
        {

            model.BankList = Common.PopulateDllList(GetBankList(model.BankId));
            model.BankBranchList = Common.PopulateDllList(GetBankBranchList(model.BranchId));
        }

        private bool CheckDuplicateEntry(BankInformationViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _cpfCommonservice.CPFUnit.BankAccountInfoRepository.Get(q => q.BankId == model.BankId && q.AccountNo == model.AccountNo).Any();
            }

            else
            {
                return _cpfCommonservice.CPFUnit.BankAccountInfoRepository.Get(q => q.BankId == model.BankId && q.AccountNo == model.AccountNo && strMode != q.Id).Any();
            }
        }

        #endregion

        
    }
}
