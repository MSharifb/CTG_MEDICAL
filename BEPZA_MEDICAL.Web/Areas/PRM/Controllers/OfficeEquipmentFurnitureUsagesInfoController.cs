using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Controllers;
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
    public class OfficeEquipmentFurnitureUsagesInfoController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor

        public OfficeEquipmentFurnitureUsagesInfoController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        //
        // GET: /PRM/OfficeEquipmentFurnitureUsagesInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, OfficeEquipmentFurnitureUsagesInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<OfficeEquipmentFurnitureUsagesInfoViewModel> list = (from offEquUsa in _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.GetAll()
                                                                      join offEuaUsaDtl in _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoDetailRepository.GetAll() on offEquUsa.Id equals offEuaUsaDtl.OfficeEquipUsagesId
                                                                      join offEuqInfo in _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.GetAll() on offEuaUsaDtl.OfficeEquipmentFurInfoId equals offEuqInfo.Id
                                                                      where (model.OfficeEquipmentFurInfoId == null || model.OfficeEquipmentFurInfoId == 0 || model.OfficeEquipmentFurInfoId == offEuqInfo.Id)
                                                                      && (offEquUsa.PRM_EmploymentInfo.ZoneInfoId == LoggedUserZoneInfoId)
                                                                      select new OfficeEquipmentFurnitureUsagesInfoViewModel()
                                                                      {
                                                                          Id=offEquUsa.Id,
                                                                          OfficeEquipmentFurInfoId = offEuqInfo.Id,
                                                                          OfficeEquName = offEuqInfo.Name,
                                                                          EmployeeName = offEquUsa.PRM_EmploymentInfo.FullName,
                                                                          IssueByName = offEuaUsaDtl.PRM_EmploymentInfo.FullName

                                                                      }).ToList();


            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeName).ToList();
                }
            }
            if (request.SortingName == "OfficeEquName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OfficeEquName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OfficeEquName).ToList();
                }
            }
            if (request.SortingName == "IssueByName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IssueByName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IssueByName).ToList();
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
                  d.OfficeEquipmentFurInfoId,
                  d.EmployeeName,
                  d.OfficeEquName,
                  d.IssueByName,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }
        [NoCache]
        public ActionResult NameforView()
        {
            var itemList = Common.PopulateDllList(_prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.GetAll().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }

        public ActionResult Create()
        {
            OfficeEquipmentFurnitureUsagesInfoViewModel model = new OfficeEquipmentFurnitureUsagesInfoViewModel();
            populateDropdown(model);
            model.IsReturn = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(OfficeEquipmentFurnitureUsagesInfoViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var entity = CreateEntity(model, true);
                try
                {
                    entity.IUser = User.Identity.Name;
                    entity.IDate = DateTime.Now;

                    _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.Add(entity);
                    _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.SaveChanges();

                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }
            }
            else
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = errorList;
                return View(model);
            }

            return View(model);
        }

        private PRM_OfficeEquipmentFurnitureUsagesInfo CreateEntity(OfficeEquipmentFurnitureUsagesInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            foreach (var c in model.OfficeEquipmentUsagesDetailList)
            {
                var prm_OfficeEquipmentFurnitureUsagesInfoDetail = new PRM_OfficeEquipmentFurnitureUsagesInfoDetail();

                prm_OfficeEquipmentFurnitureUsagesInfoDetail.Id = c.Id;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.OfficeEquipmentFurInfoId = c.OfficeEquipmentFurInfoId;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.ProcurementDate = c.ProcurementDate;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.IssueDate = c.IssueDate;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.IssueFor = c.IssueFor;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.IssueById = c.IssueById;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.PropertyCost = c.PropertyCost;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.IsReturn = c.IsReturn;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.Remarks = c.Remarks;

                prm_OfficeEquipmentFurnitureUsagesInfoDetail.IUser = User.Identity.Name;
                prm_OfficeEquipmentFurnitureUsagesInfoDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    prm_OfficeEquipmentFurnitureUsagesInfoDetail.IUser = User.Identity.Name;
                    prm_OfficeEquipmentFurnitureUsagesInfoDetail.IDate = DateTime.Now;
                    entity.PRM_OfficeEquipmentFurnitureUsagesInfoDetail.Add(prm_OfficeEquipmentFurnitureUsagesInfoDetail);
                }
                else
                {
                    prm_OfficeEquipmentFurnitureUsagesInfoDetail.OfficeEquipUsagesId = model.Id;
                    prm_OfficeEquipmentFurnitureUsagesInfoDetail.EUser = User.Identity.Name;
                    prm_OfficeEquipmentFurnitureUsagesInfoDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoDetailRepository.Add(prm_OfficeEquipmentFurnitureUsagesInfoDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoDetailRepository.Update(prm_OfficeEquipmentFurnitureUsagesInfoDetail);

                    }
                }
                _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoDetailRepository.SaveChanges();
            }

            return entity;
        }

        public ActionResult Edit(int id, string type)
        {
            var entity = _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.GetByID(id);
            var parentModel = entity.ToModel();
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;

            parentModel.EmpId = entity.PRM_EmploymentInfo.EmpID;
            parentModel.EmployeeName = entity.PRM_EmploymentInfo.FullName;
            parentModel.Department = entity.PRM_EmploymentInfo.PRM_Division.Name;
            parentModel.Designation = entity.PRM_EmploymentInfo.PRM_Designation.Name;
            parentModel.Section =entity.PRM_EmploymentInfo.PRM_Section==null?string.Empty:entity.PRM_EmploymentInfo.PRM_Section.Name;

            List<OfficeEquipmentFurnitureUsagesInfoDetailViewModel> list = (from offiUsa in _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.GetAll()
                                                                            join offiUsaDtl in _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoDetailRepository.GetAll() on offiUsa.Id equals offiUsaDtl.OfficeEquipUsagesId
                                                                            join offiEqaInfo in _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.GetAll() on offiUsaDtl.OfficeEquipmentFurInfoId equals offiEqaInfo.Id
                                                                            where (offiUsa.Id == id)
                                                                            select new OfficeEquipmentFurnitureUsagesInfoDetailViewModel()
                                                                            {
                                                                                Id = offiUsaDtl.Id,
                                                                                OfficeEquipmentFurInfoId=offiUsaDtl.OfficeEquipmentFurInfoId,
                                                                                ProcurementDate=offiUsaDtl.ProcurementDate,
                                                                                IssueDate=offiUsaDtl.IssueDate,
                                                                                IssueFor=offiUsaDtl.IssueFor,
                                                                                IssueById=offiUsaDtl.IssueById,
                                                                                PropertyCost=offiUsaDtl.PropertyCost,
                                                                                IsReturn=offiUsaDtl.IsReturn,
                                                                                Remarks=offiUsaDtl.Remarks,
                                                                                OfficeEquName = offiEqaInfo.Name,
                                                                                IssueByName = offiUsaDtl.PRM_EmploymentInfo.FullName
                                                                            }).ToList();
            parentModel.OfficeEquipmentUsagesDetailList = list;
            populateDropdown(parentModel);
            if (type == "success")
            {
                parentModel.IsError = 1;
                parentModel.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }
            return View("Edit", parentModel);
        }

        [HttpPost]
        public ActionResult Edit(OfficeEquipmentFurnitureUsagesInfoViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    var entity = CreateEntity(model, false);

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.SaveChanges();

                        model.IsError = 1;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        return RedirectToAction("Edit", new { id = model.Id, type = "success" });
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
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
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_OfficeEquipmentFurnitureUsagesInfoDetail) };
                    _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoRepository.SaveChanges();
                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
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
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteOfficeEquipmentFurnitureUsagesDetail")]
        public JsonResult DeleteOfficeEquipmentFurnitureUsagesConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.OfficeEquipmentFurnitureUsagesInfoDetailRepository.SaveChanges();
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

        private void populateDropdown(OfficeEquipmentFurnitureUsagesInfoViewModel model)
        {
            #region Office Equipment
            var ddlList = _prmCommonService.PRMUnit.OfficeEquipmentFurnitureInfoRepository.Fetch().Where(q=>q.ZoneInfoId==LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            model.OfficeEquipmentFurList = Common.PopulateDllList(ddlList);
            #endregion
        }
        public string GetBusinessLogicValidation(OfficeEquipmentFurnitureUsagesInfoViewModel model)
        {
            string errorList = string.Empty;
            if (model.OfficeEquipmentUsagesDetailList.Count == 0)
                errorList = "There are no row to Save data.";
            return errorList;
        }

	}
}