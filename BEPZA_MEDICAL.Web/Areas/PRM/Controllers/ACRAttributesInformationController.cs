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
    public class ACRAttributesInformationController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Constructor

        public ACRAttributesInformationController(PRMCommonSevice prmCommonService)
        {
            this._prmCommonService = prmCommonService;
        }
        #endregion

        //
        // GET: /PRM/ACRAttributesInformation/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ACRAttributesInformationViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ACRAttributesInformationViewModel> list = (from acrAtt in _prmCommonService.PRMUnit.ACRAttributesInformationRepository.GetAll()
                                                            join acrAttDtl in _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.GetAll() on acrAtt.Id equals acrAttDtl.ACRAttributesInfoId
                                                            join acrCri in _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.GetAll() on acrAtt.ACRCriteriaInfoId equals acrCri.Id
                                                            join catg in _prmCommonService.PRMUnit.PRM_StaffCategoryRepository.GetAll() on acrCri.StaffCategoryId equals catg.Id
                                                            where (model.StaffCategoryId == 0 || model.StaffCategoryId == acrAtt.StaffCategoryId)
                                                            && (model.ACRCriteriaInfoId == 0 || model.ACRCriteriaInfoId == acrAtt.ACRCriteriaInfoId)
                                                            select new ACRAttributesInformationViewModel()
                                                            {
                                                                Id = acrAtt.Id,
                                                                StaffCategoryId = acrAtt.StaffCategoryId,
                                                                ACRCriteriaInfoId = acrAtt.ACRCriteriaInfoId,
                                                                AttributeName = acrAttDtl.AttributeName,
                                                                ACRCriteriaName=acrCri.ACRCriteriaName
                                                            }).DistinctBy(x=>x.ACRCriteriaName).ToList();


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
                  d.StaffCategoryId,
                  d.ACRCriteriaInfoId,
                  d.ACRCriteriaName,
                  "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult StaffCategoryforView()
        {
            var itemList = Common.PopulateDllList(_prmCommonService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.Name).ToList());
            return PartialView("Select", itemList);
        }

        [NoCache]
        public ActionResult ACRCriteriaforView()
        {
            var acrList = _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.Fetch().OrderBy(x => x.ACRCriteriaName).ToList();
            var itemList = new List<SelectListItem>();
            foreach (var item in acrList)
            {
                itemList.Add(new SelectListItem()
                {
                    Text = item.ACRCriteriaName,
                    Value = item.Id.ToString()
                });
            } 
          return PartialView("Select", itemList);
        }

        public ActionResult Create()
        {
            ACRAttributesInformationViewModel model = new ACRAttributesInformationViewModel();
            populateDropdown(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ACRAttributesInformationViewModel model)
        {

            try
            {
                string errorList = string.Empty;
                model.IsError = 1;

                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    var entity = CreateEntity(model, true);

                    _prmCommonService.PRMUnit.ACRAttributesInformationRepository.Add(entity);
                    _prmCommonService.PRMUnit.ACRAttributesInformationRepository.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    populateDropdown(model);
                    model.ErrMsg = errorList;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _prmCommonService.PRMUnit.ACRAttributesInformationRepository.GetByID(id);
            var parentModel = entity.ToModel();
            parentModel.strMode = "Edit";
            parentModel.IsInEditMode = true;

            List<ACRAttributesInformationDetailViewModel> list = (from acrDtl in _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.GetAll()
                                                                  where (acrDtl.ACRAttributesInfoId == id)
                                                                  select new ACRAttributesInformationDetailViewModel()
                                                                  {
                                                                       Id = acrDtl.Id,
                                                                       AttributeName = acrDtl.AttributeName,
                                                                       FullMark = acrDtl.FullMark,
                                                                       SerialNumber = acrDtl.SerialNumber
                                                                  }).OrderBy(x=>x.SerialNumber).ToList();

            parentModel.ACRAttributesInfoDetailList = list;
            populateDropdown(parentModel);
            return View("Edit", parentModel);
        }

        [HttpPost]
        public ActionResult Edit(ACRAttributesInformationViewModel model)
        {
            try
            {
                string errorList = "";
                if (ModelState.IsValid)
                {
                    var entity = CreateEntity(model, false);

                    if (errorList.Length == 0)
                    {
                        entity.EUser = User.Identity.Name;
                        entity.EDate = DateTime.Now;

                        _prmCommonService.PRMUnit.ACRAttributesInformationRepository.Update(entity);
                        _prmCommonService.PRMUnit.ACRAttributesInformationRepository.SaveChanges();
                        model.errClass = "success";
                        model.Message = Resources.ErrorMessages.UpdateSuccessful;
                        return RedirectToAction("Index");
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
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var tempPeriod = _prmCommonService.PRMUnit.ACRAttributesInformationRepository.GetByID(id);
            try
            {
                if (tempPeriod != null)
                {
                    List<Type> allTypes = new List<Type> { typeof(PRM_ACRAttributesInformationDetail) };
                    _prmCommonService.PRMUnit.ACRAttributesInformationRepository.Delete(tempPeriod.Id, allTypes);
                    _prmCommonService.PRMUnit.ACRAttributesInformationRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteACRAttributesDetail")]
        public JsonResult DeleteACRAttributesConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.Delete(id);
                _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.SaveChanges();
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
        private PRM_ACRAttributesInformation CreateEntity(ACRAttributesInformationViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            foreach (var c in model.ACRAttributesInfoDetailList)
            {
                var prm_ACRAttributesInformationDetail = new PRM_ACRAttributesInformationDetail();

                prm_ACRAttributesInformationDetail.Id = c.Id;
                prm_ACRAttributesInformationDetail.AttributeName = c.AttributeName;
                prm_ACRAttributesInformationDetail.FullMark = c.FullMark;
                prm_ACRAttributesInformationDetail.SerialNumber = c.SerialNumber;
                prm_ACRAttributesInformationDetail.IUser = User.Identity.Name;
                prm_ACRAttributesInformationDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    prm_ACRAttributesInformationDetail.IUser = User.Identity.Name;
                    prm_ACRAttributesInformationDetail.IDate = DateTime.Now;
                    entity.PRM_ACRAttributesInformationDetail.Add(prm_ACRAttributesInformationDetail);
                }
                else
                {
                    prm_ACRAttributesInformationDetail.ACRAttributesInfoId = model.Id;
                    prm_ACRAttributesInformationDetail.EUser = User.Identity.Name;
                    prm_ACRAttributesInformationDetail.EDate = DateTime.Now;

                    if (c.Id == 0)
                    {
                        _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.Add(prm_ACRAttributesInformationDetail);
                    }
                    else
                    {
                        _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.Update(prm_ACRAttributesInformationDetail);

                    }
                }
                _prmCommonService.PRMUnit.ACRAttributesInformationDetailRepository.SaveChanges();
            }

            return entity;
        }
        private void populateDropdown(ACRAttributesInformationViewModel model)
        {
            #region Staff Category
            var ddlList = _prmCommonService.PRMUnit.PRM_StaffCategoryRepository.Fetch().OrderBy(x => x.Name).ToList();
            model.StaffCategoryList = Common.PopulateDllList(ddlList);
            #endregion

            #region ACR CriteriaInformation

            var acrList = _prmCommonService.PRMUnit.ACRCriteriaInformationRepository.Fetch().OrderBy(x => x.ACRCriteriaName).ToList();
            var list = new List<SelectListItem>();
            foreach (var item in acrList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ACRCriteriaName,
                    Value = item.Id.ToString()
                });
            }

            model.ACRCriteriaInfoList = list.OrderBy(x => x.Text.Trim()).ToList();

            #endregion
        }
    }
}