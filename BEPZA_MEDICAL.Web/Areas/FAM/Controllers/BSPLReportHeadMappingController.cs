using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Domain.FAM;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;
using System.Data.SqlClient;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using System.Collections;
using System.Web.Services.Description;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.BSPLReportHeadMapping;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class BSPLReportHeadMappingController : Controller
    {
        #region Fields
        private readonly FAMCommonService _famCommonService;
        #endregion

        #region Ctor
        public BSPLReportHeadMappingController(FAMCommonService famCommonService)
        {
            _famCommonService = famCommonService;
        }
        #endregion

        #region Actions
        public ViewResult Index()
        {

            return View();
        }

        public ActionResult BackToList()
        {
            var model = new BSPLReportHeadMappingModel();
            PrepareModel(model);
            return View("_Index", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, BSPLReportHeadMappingModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _famCommonService.GetReportBSPLHeadSearchedList("",request.SortingName,request.SortingOrder.ToString(),request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1,false, model.ReportId, model.BSPLReportHeadId);
            //list = _famCommonService.FAMUnit.BSPLReportHeadMappingDetailsRepository.GetPaged(_famCommonService.GetReportBSPLHeadSearchedList(model.ReportId, model.BSPLReportHeadId).ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1); ;

            totalRecords = _famCommonService.GetReportBSPLHeadSearchedList("", "", "",0,0,0,true, 0,0).Count(); 

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
                    d.ReportName,
                    d.BSPLReportHeadName,
                    d.ReportHeadSerial,                   
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new BSPLReportHeadMappingModel();
            //Get Account Head

            model.BSPLReportHeadList = Common.PopulateDllList(_famCommonService.FAMUnit.BSPLReportHeadNameRepository.GetAll()).ToList();
            model.ReportNameList = Common.PopulateDllList(_famCommonService.FAMUnit.ReportNameRepository.GetAll()).ToList();

            //IEnumerable<FAM_ChartOfAccount> accountHead = _famCommonService.FAMUnit.ChartOfAccount.GetAll().ToList()
            //    .Where(x => (bool)x.IsPostingAccount).OrderBy(y => y.AccountHeadType);

            IEnumerable<FAM_ChartOfAccount> accountHead = _famCommonService.FAMUnit.ChartOfAccount.GetAll().ToList()
                .Where(x=> x.AccountHeadType!="C").OrderBy(y => y.AccountHeadCode);

            foreach (var item in accountHead)
            {
                var innerModel = new BSPLReportAccountHead()
                {
                    AccountHeadId = item.Id,
                    //AccountHeadName = item.AccountHeadName,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    //AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense",
                    AccountHeadType = (item.AccountHeadType == "I" ? "Income" :item.AccountHeadType=="E"? "Expense":item.AccountHeadType=="A"?"Asset":"Liability"),
                    IsSelected = false
                };

                model.BSPLReportAccountHeadList.Add(innerModel);
            }


            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(BSPLReportHeadMappingModel model)
        {
            string businessError = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    if (model.BSPLReportAccountHeadList.Where(x => x.IsSelected).Count() <= 0)
                    {
                        businessError = "Select Account Head.";
                    }
                    else
                    {
                        foreach (var item in model.BSPLReportAccountHeadList.Where(x => x.IsSelected))
                        {
                            var childEntity = item.ToEntityChild();
                            entity.FAM_BSPLReportHeadMappingDetails.Add(childEntity);
                            //childEntity.DivisionUnitId = entity.DivisionUnitId;
                            if (item.IsSelected)
                            {
                                _famCommonService.FAMUnit.BSPLReportHeadMappingDetailsRepository.Add(childEntity);
                                _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.SaveChanges();
                            }
                        }
                    }
                    
                    
                }
                catch (Exception)
                {

                    return new JsonResult()
                    {
                        Data = ErrorMessages.InsertFailed
                    };
                }

                return new JsonResult()
                {
                    Data = string.IsNullOrEmpty(businessError) ? ErrorMessages.InsertSuccessful : businessError
                };
            }

            var errors = ModelState
                           .Where(x => x.Value.Errors.Count > 0)
                           .Select(x => new { x.Key, x.Value.Errors })
                           .ToArray();

            return new JsonResult()
            {
                Data = errors.Count() > 0 ? errors.First().Errors.First().ErrorMessage : ""
            };
        }

        public ActionResult Edit(int Id)
        {
            //Get Saved Budget Head
            var mainModel = new BSPLReportHeadMappingModel();
            //var modeSaved = new BSPLReportHeadMappingModel();
            var entity = _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.GetByID(Id);
            var model = entity.ToModel();

            var data = (from d in _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.GetAll()
                        join dtl in _famCommonService.FAMUnit.BSPLReportHeadMappingDetailsRepository.GetAll() on d.Id equals dtl.BSPLReportHeadMappingId
                        join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on dtl.AccountHeadId equals coa.Id
                        where d.ReportId == model.ReportId && d.BSPLReportHeadId == model.BSPLReportHeadId
                        select new
                        {
                            
                            dtl.BSPLReportHeadMappingId,
                            dtl.AccountHeadId,
                            coa.AccountHeadCode,
                            coa.AccountHeadName,
                            coa.AccountHeadType
                        }).Distinct().ToList();

            foreach (var item in data)
            {
                var innerModel = new BSPLReportAccountHead()
                {
                    AccountHeadId = item.AccountHeadId,
                    //AccountHeadName = item.AccountHeadName,
                    AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                    //AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
                    AccountHeadType = (item.AccountHeadType == "I" ? "Income" :item.AccountHeadType=="E"? "Expense":item.AccountHeadType=="A"?"Asset":"Liability"),
                };

                mainModel.BSPLReportAccountHeadList.Add(innerModel);
            }


            //Get All Posting (Income/Expense) Account Head
            var modelAll = new BSPLReportHeadMappingModel();
            modelAll.Id = Id;
            modelAll.ReportId = model.ReportId;
            modelAll.BSPLReportHeadId = model.BSPLReportHeadId;
            modelAll.ReportHeadSerial = model.ReportHeadSerial;
            modelAll.Remarks = model.Remarks;

            //IEnumerable<FAM_ChartOfAccount> accountHead = _famCommonService.FAMUnit.ChartOfAccount.GetAll().ToList()
            //     .Where(x => (bool)x.IsPostingAccount).OrderBy(at => at.AccountHeadType);

            IEnumerable<FAM_ChartOfAccount> accountHead = _famCommonService.FAMUnit.ChartOfAccount.GetAll().ToList()
                 .Where(x=> x.AccountHeadType!="C").OrderBy(at => at.AccountHeadCode);

            foreach (var itemCOA in accountHead)
            {
                var innerModelCOA = new BSPLReportAccountHead()
                {
                    AccountHeadId = itemCOA.Id,
                    AccountHeadName = itemCOA.AccountHeadCode + '-' + itemCOA.AccountHeadName,
                    //AccountHeadType = itemCOA.AccountHeadType == "I" ? "Income" : "Expense",
                    AccountHeadType = (itemCOA.AccountHeadType == "I" ? "Income" : itemCOA.AccountHeadType == "E" ? "Expense" : itemCOA.AccountHeadType == "A" ? "Asset" : "Liability"),
                    IsSelected = mainModel.BSPLReportAccountHeadList.Where(d => d.AccountHeadId == itemCOA.Id).FirstOrDefault() != null ? true : false

                };

                modelAll.BSPLReportAccountHeadList.Add(innerModelCOA);
            }


            PrepareModel(modelAll);
            modelAll.Mode = "Edit";
            return View("_CreateOrEdit", modelAll);
        }

        [HttpPost]
        public ActionResult Edit(BSPLReportHeadMappingModel model)
        {

            if (ModelState.IsValid)
            {
                string businessError = string.Empty;
                try
                {
                    //Get Saved Budget Head
                    var mainModel = new BSPLReportHeadMappingModel();
                    //var modeSaved = new BSPLReportHeadMappingModel();
                    var entityM = _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.GetByID(model.Id);
                    var modelM = entityM.ToModel();

                    if (model.BSPLReportAccountHeadList.Where(x => x.IsSelected).Count() <= 0)
                    {
                        businessError = "Select Account Head.";
                    }
                    else
                    {
                        var data = (from d in _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.GetAll()
                                    join dtl in _famCommonService.FAMUnit.BSPLReportHeadMappingDetailsRepository.GetAll() on d.Id equals dtl.BSPLReportHeadMappingId
                                    join coa in _famCommonService.FAMUnit.ChartOfAccount.GetAll() on dtl.AccountHeadId equals coa.Id
                                    where d.ReportId == modelM.ReportId && d.BSPLReportHeadId == modelM.BSPLReportHeadId
                                    select new
                                    {
                                        dtl.BSPLReportHeadMappingId,
                                        dtl.AccountHeadId,
                                        coa.AccountHeadCode,
                                        coa.AccountHeadName,
                                        coa.AccountHeadType
                                    }).Distinct().ToList();

                        foreach (var item in data)
                        {
                            var innerModel = new BSPLReportAccountHead()
                            {
                                AccountHeadId = item.AccountHeadId,
                                //AccountHeadName = item.AccountHeadName,
                                AccountHeadName = item.AccountHeadCode + '-' + item.AccountHeadName,
                                AccountHeadType = item.AccountHeadType == "I" ? "Income" : "Expense"
                            };

                            mainModel.BSPLReportAccountHeadList.Add(innerModel);
                        }


                        //Delete Budget Head that has been ommited in Edit
                        List<Type> allTypes = new List<Type> { typeof(FAM_BSPLReportHeadMappingDetails) };
                        var entityChld = _famCommonService.FAMUnit.BSPLReportHeadMappingDetailsRepository.GetAll().Where(x => x.BSPLReportHeadMappingId == model.Id).ToList();
                        foreach (var item in entityChld)
                        {
                            if (model.BSPLReportAccountHeadList.Where(x => x.AccountHeadId == item.AccountHeadId && x.IsSelected == false).Count() > 0)
                            {
                                _famCommonService.FAMUnit.BSPLReportHeadMappingDetailsRepository.Delete(item.Id);
                            }
                        }
                        _famCommonService.FAMUnit.BSPLReportHeadMappingDetailsRepository.SaveChanges();


                        //Insert Budget Head that has been added in Edit
                        var entity = model.ToEntity();

                        foreach (var item in model.BSPLReportAccountHeadList.Where(x => x.IsSelected))
                        {
                            var childEntity = item.ToEntityChild();
                            childEntity.BSPLReportHeadMappingId = model.Id;

                            mainModel.ReportId = model.ReportId;
                            mainModel.BSPLReportHeadId = model.BSPLReportHeadId;
                            mainModel.ReportHeadSerial = model.ReportHeadSerial;
                            mainModel.Remarks = model.Remarks;
                            if (item.IsSelected)
                            {
                                if (mainModel.BSPLReportAccountHeadList.Where(x => x.AccountHeadId == item.AccountHeadId).Count() == 0)// .Contains(item))//.ToList().Exists(d => d.AccountHeadId != item.AccountHeadId))//).FirstOrDefault() != null)
                                {
                                    _famCommonService.FAMUnit.BSPLReportHeadMappingDetailsRepository.Add(childEntity);
                                }
                            }
                        }
                        _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.Update(entity);

                        _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.SaveChanges();
                    }


                }

                catch (Exception)
                {

                    return new JsonResult()
                    {
                        Data = ErrorMessages.UpdateFailed
                    };
                }

                return new JsonResult()
                {
                    Data = String.IsNullOrEmpty(businessError) ? ErrorMessages.UpdateSuccessful : businessError
                };
            }
            return new JsonResult()
            {
                Data = ErrorMessages.UpdateFailed
            };
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var entity = _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.GetByID(id);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(FAM_BSPLReportHeadMappingDetails) };
                _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.Delete(entity.Id, allTypes);
                _famCommonService.FAMUnit.BSPLReportHeadMappingRepository.SaveChanges();

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
        private void PrepareModel(BSPLReportHeadMappingModel model)
        {
            model.BSPLReportHeadList = Common.PopulateDllList(_famCommonService.FAMUnit.BSPLReportHeadNameRepository.GetAll()).ToList();
            model.ReportNameList = Common.PopulateDllList(_famCommonService.FAMUnit.ReportNameRepository.GetAll()).ToList();
        }

        [NoCache]
        public ActionResult BSPLReportHeadforView()
        {
            var itemList = Common.PopulateDllList(_famCommonService.FAMUnit.BSPLReportHeadNameRepository.GetAll()).ToList();
            return PartialView("_Select", itemList);
        }
        
        [NoCache]
        public ActionResult ReportNameforView()
        {
            var itemList = Common.PopulateDllList(_famCommonService.FAMUnit.ReportNameRepository.GetAll()).ToList();
            return PartialView("_SelectReportName", itemList);
        }

        
        #endregion
    }
}
