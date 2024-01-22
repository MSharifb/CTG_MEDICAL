using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class PunishmentTypeInfoController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public PunishmentTypeInfoController(PRMCommonSevice prmCommonService)
        {

            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: /PRM/PunishmentTypeInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, PunishmentTypeInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<PunishmentTypeInfoViewModel> list = (from PType in _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.GetAll()
                                                      join DAType in _prmCommonService.PRMUnit.DisciplinaryActionTypeRepository.GetAll() on PType.DisciplinaryActionTypeId equals DAType.Id
                                                      where (string.IsNullOrEmpty(viewModel.PunishmentName) || PType.PunishmentName.Contains(viewModel.PunishmentName))
                                                      && (viewModel.DisciplinaryActionTypeId == 0 || DAType.Id == viewModel.DisciplinaryActionTypeId)
                                                      select new PunishmentTypeInfoViewModel()
                                                      {
                                                          Id = PType.Id,
                                                          PunishmentName = PType.PunishmentName,
                                                          DisciplinaryActionTypeId = PType.Id,
                                                          DisciplinaryActionTypeName = PType.PRM_DisciplinaryActionType.Name
                                                      }).OrderBy(x => x.PunishmentName).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "PunishmentName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PunishmentName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PunishmentName).ToList();
                }
            }


            if (request.SortingName == "DisciplinaryActionTypeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DisciplinaryActionTypeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DisciplinaryActionTypeName).ToList();
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
                    d.DisciplinaryActionTypeId, 
                    d.DisciplinaryActionTypeName,
                    d.PunishmentName
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public JsonResult BindListView()
        {
            
            var list = (from PType in _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.GetAll()
                                                      select new
                                                      {
                                                          Id = PType.Id,
                                                          PunishmentName = PType.PunishmentName,
                                                          DisciplinaryActionTypeId = PType.Id,
                                                          DisciplinaryActionTypeName = PType.PRM_DisciplinaryActionType.Name
                                                      }).OrderBy(x => x.PunishmentName).ToList();

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create()
        {
            PunishmentTypeInfoViewModel model = new PunishmentTypeInfoViewModel();

            var aList = _prmCommonService.PRMUnit.PunishmentRestrictionRepository.GetAll().OrderBy(o => o.SortOrder).ToList();

            model.PunishmentRestrictionList = Common.PopulatePunishmentRestrictionDDL(aList);
            populateDropdown(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(PunishmentTypeInfoViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        populateDropdown(model);
                        return View(model);
                    }
                    model = GetInsertUserAuditInfo(model);
                    var entity = CreateEntity(model, true);

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        // return RedirectToAction("Index");
                    }
                }


                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";
                model.IsError = 1;
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    //model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            populateDropdown(model);
            return View(model);
        }


        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.GetByID(Id);
            var model = entity.ToModel();

            var ddl = Common.PopulatePunishmentRestrictionDDL(_prmCommonService.PRMUnit.PunishmentRestrictionRepository.GetAll().OrderBy(o => o.SortOrder).ToList());

            var newList = new List<SelectListItem>();
            foreach (var listItem in ddl)
            {
                if (listItem.Text == "SalaryDeduction")
                {
                    var tempDtl = _prmCommonService.PRMUnit.PunishmentTypeInfoDetailRepository.GetAll().Where(q => q.PunishmentTypeInfoId == model.Id && q.PunishmentRestrictionId.ToString() == listItem.Value).FirstOrDefault();
                    model.Days = tempDtl == null ? null : tempDtl.RetrictionDays;
                }

                var obj = _prmCommonService.PRMUnit.PunishmentTypeInfoDetailRepository.GetAll().Where(q => q.PunishmentTypeInfoId==model.Id && q.PunishmentRestrictionId.ToString() == listItem.Value).FirstOrDefault();

                if (obj != null)
                {
                    newList.Add(new SelectListItem()
                          {
                              Text = obj.PRM_PunishmentRestriction.RetrictionName,
                              Value = obj.PunishmentRestrictionId.ToString(),
                              Selected = true
                          });
                }
                else
                {
                    newList.Add(new SelectListItem()
                         {
                             Text = listItem.Text,
                             Value = listItem.Value
                         });
                }

            }

            model.PunishmentRestrictionList = newList;
            populateDropdown(model);
            return View(model);
        }



        [HttpPost]
        [NoCache]
        public ActionResult Edit(PunishmentTypeInfoViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    if (CheckDuplicateEntry(model, model.Id))
                    {
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                        model.errClass = "failed";
                        return View(model);
                    }

                    model = GetInsertUserAuditInfo(model);
                    var entity = CreateEntity(model, false);

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
                        // return RedirectToAction("Index");
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
            populateDropdown(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(PRM_PunishmentTypeInfoDetail) };
                _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.Delete(id, allTypes);
                _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.SaveChanges();
                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }
        #endregion

        #region Private Method

        #region Populate Dropdown
        private void populateDropdown(PunishmentTypeInfoViewModel model)
        {
            dynamic ddlList;

            #region Disciplinary Action Type

            ddlList = _prmCommonService.PRMUnit.DisciplinaryActionTypeRepository.GetAll().OrderBy(o => o.Name).ToList();
            model.DisciplinaryActionTypeList = Common.PopulateDllList(ddlList);

            #endregion

        }

        #endregion

        private PunishmentTypeInfoViewModel GetInsertUserAuditInfo(PunishmentTypeInfoViewModel model)
        {
            model.IUser = User.Identity.Name;
            model.IDate = DateTime.Now;

            foreach (var child in model.PunishmentTypeInfoDetail)
            {
                child.IUser = User.Identity.Name;
                child.IDate = DateTime.Now;
            }

            return model;
        }

        private PRM_PunishmentTypeInfo CreateEntity(PunishmentTypeInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            foreach (var c in model.PunishmentRestrictionList.Where(q => q.Selected == true))
            {
                var prm_PunishmentTypeInfoDetail = new PRM_PunishmentTypeInfoDetail();

                //prm_PunishmentTypeInfoDetail.Id = c.Id;
                //prm_PunishmentTypeInfoDetail.IUser = String.IsNullOrEmpty(c.IUser) ? User.Identity.Name : c.IUser;
                //prm_PunishmentTypeInfoDetail.IDate = c.IDate == null ? DateTime.Now : Convert.ToDateTime(c.IDate);
                //prm_PunishmentTypeInfoDetail.EUser = c.EUser;
                //prm_PunishmentTypeInfoDetail.EDate = c.EDate;

                prm_PunishmentTypeInfoDetail.PunishmentTypeInfoId = model.Id;
                prm_PunishmentTypeInfoDetail.PunishmentRestrictionId = Convert.ToInt32(c.Value);
                if (c.Text == "SalaryDeduction")
                {
                    prm_PunishmentTypeInfoDetail.RetrictionDays = model.Days;
                }


                if (pAddEdit)
                {
                    prm_PunishmentTypeInfoDetail.IUser = User.Identity.Name;
                    prm_PunishmentTypeInfoDetail.IDate = DateTime.Now;

                    entity.PRM_PunishmentTypeInfoDetail.Add(prm_PunishmentTypeInfoDetail);
                }
                else
                {
                    var check = _prmCommonService.PRMUnit.PunishmentTypeInfoDetailRepository.Get(q => q.PunishmentTypeInfoId == model.Id).ToList();
                    if (check.Count > 0)
                    {
                        foreach (var item in check)
                        {
                            _prmCommonService.PRMUnit.PunishmentTypeInfoDetailRepository.Delete(item.Id);
                        }
                        _prmCommonService.PRMUnit.PunishmentTypeInfoDetailRepository.SaveChanges();
                    }

                    prm_PunishmentTypeInfoDetail.PunishmentTypeInfoId = model.Id;
                    prm_PunishmentTypeInfoDetail.IUser = model.IUser;
                    prm_PunishmentTypeInfoDetail.IDate = Convert.ToDateTime(model.IDate);

                    prm_PunishmentTypeInfoDetail.EUser = User.Identity.Name;
                    prm_PunishmentTypeInfoDetail.EDate = DateTime.Now;

                    _prmCommonService.PRMUnit.PunishmentTypeInfoDetailRepository.Add(prm_PunishmentTypeInfoDetail);
                }

            }

            return entity;
        }

        private bool CheckDuplicateEntry(PunishmentTypeInfoViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.Get(q => q.PunishmentName == model.PunishmentName).Any();
            }

            else
            {

                return _prmCommonService.PRMUnit.PunishmentTypeInfoRepository.Get(q => q.PunishmentName == model.PunishmentName && strMode != q.Id).Any();
            }
        }

        #endregion

        //Search View

        [NoCache]
        public ActionResult DisciplinaryActionTypeListView()
        {
            var list = Common.PopulateEmployeeDesignationDDL(_prmCommonService.PRMUnit.DisciplinaryActionTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", list);
        }
    }
}