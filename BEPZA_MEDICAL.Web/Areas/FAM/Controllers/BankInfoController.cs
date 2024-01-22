using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.FAM;
 
using BEPZA_MEDICAL.Web.Areas.FAM.Models.BankInfo;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class BankInfoController : Controller
    {
        #region Fields

        private readonly FAMCommonService _famCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;

        #endregion

        #region Ctor

        public BankInfoController(FAMCommonService famCommonservice,
            PRMCommonSevice prmCommonService)
        {
            _famCommonservice = famCommonservice;
           //  
            _prmCommonservice = prmCommonService;
        }

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, BankInformationModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _famCommonservice.GetBankSearchedList(model.BankName);

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.BankName,
                    d.BankAc,
                    d.AccountHead,
                    d.SWIFTCode,
                    d.BankAddr,                    
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index()
        {
            var model = new BankInformationModel();
            PrepareModel(model);
            return View(model);
        }

        public ActionResult BackToList()
        {
            var model = new BankInformationModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        public ActionResult Create()
        {
            var model = new BankInformationModel();
            PrepareModelEdit(model);

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(BankInformationModel model)
        {
            string strMessage = string.Empty;

            if (model.BankInfoChilds.Count <= 0)
            {
                strMessage = "Bank Information should have at least one Account Head.";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var entity = model.ToEntity();  /// Convert model to entity set
                        foreach (var item in model.BankInfoChilds.Select(x => x.ToEntity()))  /// Get child list from model
                        {
                            entity.FAM_BankBranchAccountNo.Add(item);
                        }

                        _famCommonservice.FAMUnit.BankMaster.Add(entity);
                        _famCommonservice.FAMUnit.BankMaster.SaveChanges();
                        strMessage = ErrorMessages.InsertSuccessful;
                    }
                    catch (Exception Ex)
                    {
                        if (Ex.InnerException.Message.Contains("duplicate"))
                        {
                            strMessage = ErrorMessages.UniqueIndex;
                        }
                        else
                        {
                            strMessage = ErrorMessages.InsertFailed;
                        }
                        //return new JsonResult()
                        //{
                        //    Data = ErrorMessages.InsertFailed
                        //};
                    }

                    //return new JsonResult()
                    //{
                    //    Data = ErrorMessages.InsertSuccessful
                    //};
                }
            }
            return new JsonResult()
            {
                Data = strMessage
            };
        }

        public ActionResult Edit(int id)
        {
            var entity = _famCommonservice.FAMUnit.BankMaster.GetByID(id);
            var childEntities = entity.FAM_BankBranchAccountNo;
            var model = entity.ToModel();
            PrepareModelEdit(model);
            childEntities.ToList().ForEach(x => model.BankInfoChilds.Add(x.ToModel()));
            model.Mode = "Edit";
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(BankInformationModel model)
        {
            string strMessage = string.Empty;
            if (model.BankInfoChilds==null || model.BankInfoChilds.Count <= 0)
            {
                strMessage = "Bank Information should have at least one Account Head.";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var entity = model.ToEntity();

                        var navigationList = new Dictionary<Type, ArrayList>();
                        var childEntities = new ArrayList();
                        model.BankInfoChilds.ToList().ForEach(x => x.BankBranchMapId = model.Id);  /// add parent id to each child
                        model.BankInfoChilds.Select(x => x.ToEntity()).ToList().ForEach(x => childEntities.Add(x)); /// add "perent id attached" child list to the specified arrray

                        navigationList.Add(typeof(FAM_BankBranchAccountNo), childEntities);
                        _famCommonservice.FAMUnit.BankMaster.Update(entity, navigationList);
                        _famCommonservice.FAMUnit.BankMaster.SaveChanges();
                        strMessage = ErrorMessages.UpdateSuccessful;
                    }
                    catch (Exception Ex)
                    {
                        if (Ex.InnerException.Message.Contains("duplicate"))
                        {
                            strMessage = ErrorMessages.UniqueIndex;
                        }
                        else
                        {
                            strMessage = ErrorMessages.UpdateFailed;
                        }
                        //return new JsonResult()
                        //{
                        //    Data = ErrorMessages.UpdateFailed
                        //};
                    }

                   
                }
            }
            return new JsonResult()
            {
                Data = strMessage
            };
            //var errors = ModelState
            //                .Where(x => x.Value.Errors.Count > 0)
            //                .Select(x => new { x.Key, x.Value.Errors })
            //                .ToArray();

            //return new JsonResult()
            //{

            //    Data = ErrorMessages.UpdateFailed
            //};
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _famCommonservice.FAMUnit.BankMaster.GetByID(id);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(FAM_BankBranchAccountNo) };
                _famCommonservice.FAMUnit.BankMaster.Delete(entity.Id, allTypes);
                _famCommonservice.FAMUnit.BankMaster.SaveChanges();

                return Json(new
                {
                    Success = 1,
                    Message = ErrorMessages.DeleteSuccessful
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new
                {
                    Success = 0,
                    Message = ErrorMessages.DeleteFailed
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Utils
        private void PrepareModel(BankInformationModel model)
        {

        }
        private void PrepareModelEdit(BankInformationModel model)
        {
            model.AccHeadList = _famCommonservice.FAMUnit.ChartOfAccount.GetAll().ToList()
                .Where(x => (bool)x.IsPostingAccount)
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.AccountHeadName,
                    Value = y.Id.ToString()
                }).ToList();

            model.BankList = Common.PopulateDllList(_prmCommonservice.PRMUnit.BankNameRepository.GetAll()).ToList();
            model.BranchList = Common.PopulateDllList(_prmCommonservice.PRMUnit.BankBranchRepository.GetAll()).ToList(); ;
        }
        #endregion

    }
}
