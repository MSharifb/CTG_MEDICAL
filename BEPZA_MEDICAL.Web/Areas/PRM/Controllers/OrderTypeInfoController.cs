using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class OrderTypeInfoController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonService;
        #endregion

        #region Ctor
        public OrderTypeInfoController(PRMCommonSevice prmCommonService)
        {

            this._prmCommonService = prmCommonService;
        }
        #endregion

        #region Actions
        //
        // GET: /PRM/OrderTypeInfo/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, OrderTypeInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<OrderTypeInfoViewModel> list = (from OType in _prmCommonService.PRMUnit.OrderTypeInfoRepository.GetAll()
                                                 where (string.IsNullOrEmpty(viewModel.Name) || OType.Name.Contains(viewModel.Name))
                                                 select new OrderTypeInfoViewModel()
                                                      {
                                                          Id = OType.Id,
                                                          Name = OType.Name,
                                                          IsFinalOrder = OType.IsFinalOrder,
                                                          SortOrder=OType.SortOrder==null ? 0 : Convert.ToInt32(OType.SortOrder)
                                                      }).OrderBy(x => x.SortOrder).ToList();



            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Name).ToList();
                }
            }


            if (request.SortingName == "IsFinalOrder")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IsFinalOrder).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IsFinalOrder).ToList();
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
                    d.Name,
                    d.IsFinalOrder.ToString().ToLower() =="true" ? "Yes":"No",              
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            OrderTypeInfoViewModel model = new OrderTypeInfoViewModel();
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(OrderTypeInfoViewModel model)
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
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = model.ToEntity();

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.OrderTypeInfoRepository.Add(entity);
                        _prmCommonService.PRMUnit.OrderTypeInfoRepository.SaveChanges();
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
                  //  model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            return View(model);
        }


        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _prmCommonService.PRMUnit.OrderTypeInfoRepository.GetByID(Id);
            var model = entity.ToModel();

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(OrderTypeInfoViewModel model)
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

                    model = GetInsertUserAuditInfo(model, false);
                    var entity = model.ToEntity();

                    if (errorList.Length == 0)
                    {
                        _prmCommonService.PRMUnit.OrderTypeInfoRepository.Update(entity);
                        _prmCommonService.PRMUnit.OrderTypeInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
                   // model.ErrMsg = Common.GetSqlExceptionMessage(sqlException.Number);
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            try
            {
                _prmCommonService.PRMUnit.OrderTypeInfoRepository.Delete(id);
                _prmCommonService.PRMUnit.OrderTypeInfoRepository.SaveChanges();
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
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Method
        private OrderTypeInfoViewModel GetInsertUserAuditInfo(OrderTypeInfoViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
            }
            else
            {

                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
            }

            return model;
        }

        private bool CheckDuplicateEntry(OrderTypeInfoViewModel model, int strMode)

        {
            if (strMode < 1)
            {
                return _prmCommonService.PRMUnit.OrderTypeInfoRepository.Get(q => q.Name == model.Name).Any();
            }

            else
            {
               // return _cpfCommonservice.CPFUnit.BankAccountInfoRepository.Get(q => q.BankId == model.BankId && q.AccountNo == model.AccountNo && strMode != q.Id).Any();
                return _prmCommonService.PRMUnit.OrderTypeInfoRepository.Get(q => q.Name == model.Name && strMode != q.Id).Any();
            }
        }
        #endregion

        //[HttpPost]
        //public JsonResult doesOrderTypeExist(string Name)
        //{
        //    var name = _prmCommonService.PRMUnit.OrderTypeInfoRepository.GetAll().Where(q => q.Name == Name.Trim()).FirstOrDefault();

        //    return Json(name == null);
        //}
    }
}