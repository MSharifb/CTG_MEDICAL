using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.FAM;
 
using BEPZA_MEDICAL.Domain.PRM;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.ChequeInfo;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections;
using BEPZA_MEDICAL.DAL.FAM;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class ChequeInfoController : Controller
    {
        #region Fields

        private readonly FAMCommonService _famCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;

        #endregion

        #region Ctor

        public ChequeInfoController(FAMCommonService famCommonservice,
          //   , 
            PRMCommonSevice prmCommonService)
        {
            _famCommonservice = famCommonservice;
           //  
            _prmCommonservice = prmCommonService;
        }

        #endregion

        #region Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ChequeInfoModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _famCommonservice.GetChequeSearchedList(model.BankName, model.ChequeBookNumber);

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
                    d.ChequeBookNumber,                   
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BackToList()
        {
            var model = new ChequeInfoModel();
            PrepareModel(model);

            return View("_Search", model);
        }

        public ActionResult Create()
        {
            var model = new ChequeInfoModel();
            PrepareModelEdit(model);

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(ChequeInfoModel model)
        {
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    PrepareCheckNumber(model);
                    var entity = model.ToEntity();
                    foreach (var item in model.ChequeInfoDetails.Select(x => x.ToEntity()))
                    {
                        entity.FAM_ChequeInfo.Add(item);
                    }

                    _famCommonservice.FAMUnit.ChequeInfoMaster.Add(entity);
                    _famCommonservice.FAMUnit.ChequeInfoMaster.SaveChanges();
                    Message = ErrorMessages.InsertSuccessful;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Message = ErrorMessages.InsertFailed + ": " + ex.Message + " inner exception: " + ex.InnerException ?? ex.InnerException.Message;
#endif
#if RELEASE
           Message = ErrorMessages.InsertFailed;         
#endif
                }
            }
            else
                Message = string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message;
            return new JsonResult()
            {
                Data = Message
            };
        }

        public ActionResult Edit(int id)
        {
            var entity = _famCommonservice.FAMUnit.ChequeInfoMaster.GetByID(id);
            var childEntities = entity.FAM_ChequeInfo;

            var model = entity.ToModel();
            childEntities.ToList().ForEach(x => model.ChequeInfoDetails.Add(x.ToModel()));
            model.BankId = entity.FAM_BankBranchAccountNo.FAM_BankBranchMap.BankId;

            PrepareModelEdit(model);
            model.Mode = "Edit";

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(ChequeInfoModel model)
        {
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    PrepareCheckNumber(model);
                    var entity = model.ToEntity();
                    var navigationList = new Dictionary<Type, ArrayList>();
                    var childEntities = new ArrayList();
                    model.ChequeInfoDetails.ToList().ForEach(x => x.ChequeBookId = model.Id);
                    model.ChequeInfoDetails.ToList().ForEach(x => childEntities.Add(x.ToEntity()));

                    navigationList.Add(typeof(FAM_ChequeInfo), childEntities);
                    _famCommonservice.FAMUnit.ChequeInfoMaster.Update(entity, navigationList);
                    _famCommonservice.FAMUnit.ChequeInfoMaster.SaveChanges();
                    Message = ErrorMessages.UpdateSuccessful;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Message = ErrorMessages.UpdateFailed + ": " + ex.Message + " inner exception: " + ex.InnerException ?? ex.InnerException.Message;
#endif
#if RELEASE
                    Message = ErrorMessages.UpdateFailed;
#endif
                }
            }
            else
                Message = string.IsNullOrEmpty(Message)? ErrorMessages.UpdateFailed : Message;
            return new JsonResult()
            {
                Data = Message
            };
        }
        
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var entity = _famCommonservice.FAMUnit.ChequeInfoMaster.GetByID(id);
            string businessError = "";
            try
            {
                var chequeDtl = from c in _famCommonservice.FAMUnit.ChequeInfoDetails.GetAll().Where(c=> c.ChequeBookId==id).ToList()
                                    select c;
                foreach (var item in chequeDtl)
                {
                    if(item.ChequeStatus =="Used")
                    {
                        businessError = "Cannot delete cheque information. It has been used in voucher entry.";
                    }
                }


                if(string.IsNullOrEmpty(businessError))
                {
                    List<Type> allTypes = new List<Type> { typeof(FAM_ChequeInfo) };
                    _famCommonservice.FAMUnit.ChequeInfoMaster.Delete(entity.Id, allTypes);
                    _famCommonservice.FAMUnit.ChequeInfoMaster.SaveChanges();

                    return Json(new
                    {
                        Success = 1,
                        Message = ErrorMessages.DeleteSuccessful
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                return Json(new
                {
                    Success = 0,
                    Message = ErrorMessages.DeleteFailed
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = 1,
                Message = ErrorMessages.DeleteFailed
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankList()
        {
            var data = Common.PopulateDllList(_prmCommonservice.PRMUnit.BankNameRepository.GetAll());
            return PartialView("_Select", data);
        }

        #endregion

        #region Utils

        private void PrepareModelEdit(ChequeInfoModel model)
        {
            var bankList = (from bm in _famCommonservice.FAMUnit.BankMaster.GetAll()
                            join bd in _famCommonservice.FAMUnit.BankDetails.GetAll() on bm.Id equals bd.BankBranchMapId
                            join b in _prmCommonservice.PRMUnit.BankNameRepository.GetAll() on bm.BankId equals b.Id
                            select new SelectListItem()
                            {
                                Text = b.Name,
                                Value = b.Id.ToString()
                            }).ToList();
            foreach (var item in bankList)
            {
                if (model.BankList.Where(x => x.Text == item.Text).Count() == 0)
                    model.BankList.Add(item);
            }
            model.AccHeadList = (from bm in _famCommonservice.FAMUnit.BankMaster.GetAll()
                                 join bd in _famCommonservice.FAMUnit.BankDetails.GetAll() on bm.Id equals bd.BankBranchMapId
                                 where bm.BankId == model.BankId
                                 select new SelectListItem()
                                 {
                                     Text = bd.BankAccountNo,
                                     Value = bd.Id.ToString()
                                 }).Distinct().ToList();
        }

        private void PrepareModel(ChequeInfoModel model)
        {
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAccHeadList(int id)
        {
            var headList = (from bm in _famCommonservice.FAMUnit.BankMaster.GetAll()
                            join bd in _famCommonservice.FAMUnit.BankDetails.GetAll() on bm.Id equals bd.BankBranchMapId
                            where bm.BankId == id
                            select new SelectListItem()
                            {
                                Text = bd.BankAccountNo,
                                Value = bd.Id.ToString()
                            }).Distinct().ToList();

            return PartialView("_Select", headList);
        }

        private void PrepareCheckNumber(ChequeInfoModel model)
        {
            for (int i = 0; i < model.NumberOfLeaf; i++)
            {
                var childModel = new ChequeInfoDetails()
                {
                    ChequeAmount = 0,
                    ChequeBookId = model.Id,
                    ChequeNumber =   (model.ChequeStartingNumber + i ).ToString().PadLeft(6,'0'),
                    ChequeStatus = "Unused"                    
                };
                model.ChequeInfoDetails.Add(childModel);
            }
        }

        private string CheckBusinessRule(ChequeInfoModel model)
        {
            //duplicate cheque
            if (_famCommonservice.GetChequeSearchedList().Where(x => model.Id != 0 ? (x.Id != model.Id && x.BankId == model.BankId && x.ChequeBookNumber == model.ChequeBookNumber) : (x.BankId == model.BankId && x.ChequeBookNumber == model.ChequeBookNumber) ).Count() > 0)
                return "Error: " + ErrorMessages.UniqueIndex;
            
            return string.Empty;
        }
        #endregion
    }
}
