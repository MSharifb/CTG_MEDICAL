using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.FAM;
 
using BEPZA_MEDICAL.Domain.PRM;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.ApprovalPathInfo;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.DAL.FAM;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class ApprovalPathInfoController : Controller
    {
        #region Fields

        private readonly FAMCommonService _famCommonservice;
        //private readonly PIMCommonService _pimCommonservice;
        private readonly PRMCommonSevice _prmCommonservice;

        #endregion

        #region Ctor

        public ApprovalPathInfoController(FAMCommonService famCommonservice,   PRMCommonSevice prmCommonService)
        {
            _famCommonservice = famCommonservice;
             
            _prmCommonservice = prmCommonService;
        }

        #endregion

        #region Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApprovalPathSearch model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic list = null;

            list = _famCommonservice.FAMUnit.ApprovalPathMaster.GetAll().Where(x => string.IsNullOrEmpty(model.PathName) || x.PathName.Contains(model.PathName)).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.PathId), new List<object>()
                {
                    d.PathId,
                    d.PathName,
                    d.Description,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }


        public ActionResult Index()
        {
            var model = new ApprovalPathInfoModel();
            return View(model);
        }

        public ActionResult BackToList()
        {
            var model = new ApprovalPathInfoModel();

            return View("_Search", model.ApprovalPathSearch);
        }

        public ActionResult Create()
        {
            var model = new ApprovalPathInfoModel();
            PrepareModel(model);

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(ApprovalPathInfoModel model)
        {
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {                    
                    var entity = model.ToEntity();
                    foreach (var item in model.ApprovalPathDetails.Select(x => x.ToEntity()))
                    {
                        entity.FAM_ApprovalPathNodeInfo.Add(item);
                    }

                    _famCommonservice.FAMUnit.ApprovalPathMaster.Add(entity);
                    _famCommonservice.FAMUnit.ApprovalPathMaster.SaveChanges();
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
                Message = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
            return new JsonResult()
            {
                Data = Message
            };
        }

        public ActionResult Edit(int id)
        {
            var entity = _famCommonservice.FAMUnit.ApprovalPathMaster.GetByID(id, "PathId");
            var childEntities = entity.FAM_ApprovalPathNodeInfo;

            var model = entity.ToModel();
            childEntities.ToList().ForEach(x => model.ApprovalPathDetails.Add(x.ToModel()));

            PrepareModelEdit(model);
            model.Mode = "Edit";

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(ApprovalPathInfoModel model)
        {
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    var entity = model.ToEntity();
                    var navigationList = new Dictionary<Type, ArrayList>();
                    var childEntities = new ArrayList();
                    model.ApprovalPathDetails.ToList().ForEach(x => x.PathId = model.PathId);
                    //model.ApprovalPathDetails.ToList().ForEach(x => childEntities.Add(x.ToEntity()));
                    model.ApprovalPathDetails.Select(x => x.ToEntity()).ToList().ForEach(x => childEntities.Add(x));

                    navigationList.Add(typeof(FAM_ApprovalPathNodeInfo), childEntities);
                    _famCommonservice.FAMUnit.ApprovalPathMaster.Update(entity, "PathId", navigationList, "NodeId");
                    _famCommonservice.FAMUnit.ApprovalPathMaster.SaveChanges();
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
                Message = string.IsNullOrEmpty(Message) ? ErrorMessages.UpdateFailed : Message;
            return new JsonResult()
            {
                Data = Message
            };
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var entity = _famCommonservice.FAMUnit.ApprovalPathMaster.GetByID(id, "PathId");

            try
            {
                List<Type> allTypes = new List<Type> { typeof(FAM_ApprovalPathNodeInfo) };
                _famCommonservice.FAMUnit.ApprovalPathMaster.Delete(entity.PathId, "PathId", allTypes, "NodeId");
                _famCommonservice.FAMUnit.ApprovalPathMaster.SaveChanges();

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

            //return Json(new
            //{
            //    Success = 1,
            //    Message = ErrorMessages.DeleteFailed
            //}, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Utils

        private void PrepareModelEdit(ApprovalPathInfoModel model)
        {
            model.DesignationList = Common.PopulateDllList(_prmCommonservice.PRMUnit.DesignationRepository.GetAll());

            var empList = (from emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
                           //where emp.DesignationId == model.DesignationId
                           select emp
                               ).Distinct().ToList();
            foreach (var item in model.ApprovalPathDetails)
            {
                item.EmpName = empList.Where(x=> x.Id == item.NodeEmpId).FirstOrDefault().FullName;
            }
            //model.EmpList = Common.PopulateEmployeeDDL();
        }

        private void PrepareModel(ApprovalPathInfoModel model)
        {
            model.DesignationList = Common.PopulateDllList(_prmCommonservice.PRMUnit.DesignationRepository.GetAll());
            
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetEmpList(int id)
        {
            var empList = (from emp in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll()
                            where emp.DesignationId == id
                            select emp
                               ).Distinct().ToList();

            return PartialView("_Select", Common.PopulateEmployeeDDL(empList));
        }

        private string CheckBusinessRule(ApprovalPathInfoModel model)
        {
            //duplicate cheque
            //if (_famCommonservice.GetChequeSearchedList().Where(x => model.Id != 0 ? (x.Id != model.Id && x.BankId == model.BankId && x.ChequeBookNumber == model.ChequeBookNumber) : (x.BankId == model.BankId && x.ChequeBookNumber == model.ChequeBookNumber)).Count() > 0)
            //    return "Error: " + ErrorMessages.UniqueIndex;

            return string.Empty;
        }
        #endregion
    }
}
