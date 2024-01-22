using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class BudgetHeadController : Controller
    {
        #region Fields

        private readonly PMICommonService _pmiCommonSevice;

        #endregion

        public BudgetHeadController(PMICommonService pmiCommonServices)
        {
            _pmiCommonSevice = pmiCommonServices;
        }
        //
        // GET: /PMI/BudgetHead/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CreateBudgetHead()
        {
            var model = new BudgetHeadViewModel();
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = HttpContext.User.Identity.Name;
            model.IsActive = true;
            model.ActionType = "Create";
            return View("CreateOrEditBudgetHead", model);
        }

        public ActionResult CreateBudgetSubHead()
        {
            var model = new BudgetHeadViewModel();
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = HttpContext.User.Identity.Name;
            model.IsActive = true;
            model.ActionType = "Create";
            PopulateBudgetHead(model);
            return View("CreateOrEditBudgetSubHead", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, BudgetHeadViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<BudgetHeadViewModel> list = (from head in _pmiCommonSevice.PMIUnit.FunctionRepository.GetBudgetHeadList()
                                              select new BudgetHeadViewModel
                                               {
                                                   Id = head.Id,
                                                   BudgetHeadName = head.BudgetHead,
                                                   BudgetSubHeadName = head.BudgetSubHead
                                               }).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            //if (request.SortingName == "BudgetSubHeadName")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.BudgetHeadName).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.BudgetHeadName).ToList();
            //    }
            //}



            #endregion

            #region Search

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.BudgetHeadName))
                {
                    list = list.Where(x => x.BudgetHeadName.Trim().ToLower().Contains(model.BudgetHeadName.Trim().ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(model.BudgetSubHeadName))
                {
                    list = list.Where(x => x.BudgetSubHeadName.Trim().ToLower().Contains(model.BudgetSubHeadName.Trim().ToLower())).ToList();
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
                    d.BudgetHeadName,
                    d.BudgetSubHeadName,
                    //"Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        private void PopulateBudgetHead(BudgetHeadViewModel model)
        {
            var listItems = _pmiCommonSevice.PMIUnit.BudgetHeadRepository.GetAll().Where(t => t.ParentId == null).DefaultIfEmpty().OfType<PMI_BudgetHead>().ToList();
            model.BudgetHeadList = Common.PopulateBudgetHeadDDL(listItems);
        }


        public ActionResult Edit(int id)
        {
            string selectedView = string.Empty;
            var modelData = _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Get(t => t.Id == id).SingleOrDefault();
            var model = new BudgetHeadViewModel();

            if (modelData != null)
            {
                model = modelData.ToModel();
                model.CreatedBy = model.IUser;
                model.CreatedDate = model.IDate;

                if (model.ParentId == null)
                {
                    selectedView = "CreateOrEditBudgetHead";
                }
                else
                {
                    selectedView = "CreateOrEditBudgetSubHead";
                    PopulateBudgetHead(model);
                }
            }
            model.ActionType = "Edit";
            return View(selectedView, model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Delete(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Delete(id);
                _pmiCommonSevice.PMIUnit.BudgetHeadRepository.SaveChanges();
                errMsg = Resources.ErrorMessages.DeleteSuccessful;
                result = false;
            }
            catch (UpdateException ex)
            {
                try
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        SqlException sqlException = ex.InnerException as SqlException;
                        errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        ModelState.AddModelError("Error", errMsg);
                    }
                    else
                    {
                        ModelState.AddModelError("Error", errMsg);
                    }
                    result = true;
                }
                catch (Exception)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = true;
            }

            return Json(new
            {
                IsError = result,
                Message = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveBudgetHead(BudgetHeadViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    entity.ParentId = null;
                    if (IsDuplicateEntry(entity))
                    {
                        model.IsError = 1;
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Add(entity);
                        _pmiCommonSevice.PMIUnit.BudgetHeadRepository.SaveChanges();
                        model.IsError = 0;
                        model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                    }
                }
                else
                {
                    model.IsError = 1;
                    model.ErrMsg = Resources.ErrorMessages.ExceptionMessage;
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
            return Json(new
            {
                Message = model.ErrMsg,
                IsError = model.IsError
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveBudgetSubHead(BudgetHeadViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    if (IsDuplicateEntry(entity, entity.ParentId))
                    {
                        model.IsError = 1;
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Add(entity);
                        _pmiCommonSevice.PMIUnit.BudgetHeadRepository.SaveChanges();
                        model.IsError = 0;
                        model.ErrMsg = Resources.ErrorMessages.InsertSuccessful;
                    }
                }
                else
                {
                    model.IsError = 1;
                    model.ErrMsg = Resources.ErrorMessages.ExceptionMessage;
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
            return Json(new
            {
                Message = model.ErrMsg,
                IsError = model.IsError
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateBudgetHead(BudgetHeadViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    if (IsDuplicateEntry_Update(entity, entity.Id))
                    {
                        model.IsError = 1;
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Update(entity);
                        _pmiCommonSevice.PMIUnit.BudgetHeadRepository.SaveChanges();
                        model.IsError = 0;
                        model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;

                    }
                }
                else
                {
                    model.IsError = 1;
                    model.ErrMsg = Resources.ErrorMessages.ExceptionMessage;
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
            return Json(new
            {
                Message = model.ErrMsg,
                IsError = model.IsError
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateBudgetSubHead(BudgetHeadViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    if (IsDuplicateEntry_Update(entity, entity.ParentId, entity.Id))
                    {
                        model.IsError = 1;
                        model.ErrMsg = Resources.ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Update(entity);
                        _pmiCommonSevice.PMIUnit.BudgetHeadRepository.SaveChanges();
                        model.IsError = 0;
                        model.ErrMsg = Resources.ErrorMessages.UpdateSuccessful;
                    }
                }
                else
                {
                    model.IsError = 1;
                    model.ErrMsg = Resources.ErrorMessages.ExceptionMessage;
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
            return Json(new
            {
                Message = model.ErrMsg,
                IsError = model.IsError
            }, JsonRequestBehavior.AllowGet);
        }

        private bool IsDuplicateEntry(PMI_BudgetHead entity)
        {
            try
            {
                var obj = _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Get(t => t.BudgetHeadName.ToUpper() == entity.BudgetHeadName.ToUpper()).SingleOrDefault();
                if (obj != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        bool IsDuplicateEntry(PMI_BudgetHead entity, int? parentId)
        {
            try
            {
                var obj = _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Get(t => t.BudgetHeadName.ToUpper() == entity.BudgetHeadName.ToUpper() && t.ParentId == parentId).SingleOrDefault();
                if (obj != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsDuplicateEntry_Update(PMI_BudgetHead entity, int id)
        {
            try
            {
                var obj = _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Get(t => t.BudgetHeadName.ToUpper() == entity.BudgetHeadName.ToUpper() && t.Id != id).SingleOrDefault();
                if (obj != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsDuplicateEntry_Update(PMI_BudgetHead entity, int? parentId, int id)
        {
            try
            {
                var obj = _pmiCommonSevice.PMIUnit.BudgetHeadRepository.Get(t => t.BudgetHeadName.ToUpper() == entity.BudgetHeadName.ToUpper() && t.ParentId == parentId && t.Id != id).SingleOrDefault();
                if (obj != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}