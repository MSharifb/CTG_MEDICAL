using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Domain.INV;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Resources;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class SupplierController : Controller
    {
        //
        // GET: /INV/Supplier/
        #region Fields
        private readonly INVCommonService _invCommonService;
        #endregion

        #region Ctor
        public SupplierController(INVCommonService prmCommonService)
        {
            this._invCommonService = prmCommonService;
        }
        #endregion

        #region Action
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SupplierInfoViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<SupplierInfoViewModel> list = (from supplier in _invCommonService.INVUnit.SupplierRepository.GetAll()
                                                where (string.IsNullOrEmpty(model.SupplierName) || supplier.SupplierName.Contains(model.SupplierName))
                                                select new SupplierInfoViewModel()
                                            {
                                                Id = supplier.Id,
                                                SupplierName = supplier.SupplierName,
                                                Address = supplier.Address,
                                                PhoneNo = supplier.PhoneNo,
                                                FaxNo = supplier.FaxNo,
                                                Email = supplier.Email,
                                                TradeLicenseNo = supplier.TradeLicenseNo,
                                                TIN = supplier.TIN,
                                                VATRegNo = supplier.VATRegNo,
                                                ContactPersonName = supplier.ContactPersonName,
                                                ContactPersonNo = supplier.ContactPersonNo,
                                                Remarks = supplier.Remarks
                                            }).OrderBy(x => x.SupplierName).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "SupplierName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SupplierName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SupplierName).ToList();
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
                var contactPerson = d.ContactPersonName;
                if (!string.IsNullOrEmpty(d.ContactPersonName))
                {
                    if (!string.IsNullOrEmpty(d.ContactPersonNo))
                    {
                        contactPerson = d.ContactPersonName + " (" + d.ContactPersonNo + ")";
                    }
                    else
                    {
                        contactPerson = d.ContactPersonName;
                    }

                }
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                  d.Id,
                  d.SupplierName,
                  d.Address,
                  d.PhoneNo,
                  contactPerson,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            SupplierInfoViewModel model = new SupplierInfoViewModel();
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(SupplierInfoViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            var strMessage = CheckDuplicateEntry(model, model.SupplierName);
            model.ErrMsg = strMessage;
            if (!string.IsNullOrWhiteSpace(strMessage))
            {
                model.errClass = "failed";
            }
            if (ModelState.IsValid && string.IsNullOrWhiteSpace(strMessage))
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _invCommonService.INVUnit.SupplierRepository.Add(entity);
                    _invCommonService.INVUnit.SupplierRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        strMessage = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        strMessage = ErrorMessages.InsertFailed;
                    }

                }
            }
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _invCommonService.INVUnit.SupplierRepository.GetByID(Id);
            SupplierInfoViewModel model = entity.ToModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SupplierInfoViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;
            var strMessage = CheckDuplicateEntry(model, model.SupplierName);
            model.ErrMsg = strMessage;
            if (!string.IsNullOrWhiteSpace(strMessage))
            {
                model.errClass = "failed";
            }
            if (ModelState.IsValid && string.IsNullOrWhiteSpace(strMessage))
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _invCommonService.INVUnit.SupplierRepository.Update(entity);
                    _invCommonService.INVUnit.SupplierRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        strMessage = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        strMessage = ErrorMessages.UpdateFailed;
                    }
                    //model.errClass = "failed";
                    //model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;
            var tempPeriod = _invCommonService.INVUnit.SupplierRepository.GetByID(id);
            try
            {
                _invCommonService.INVUnit.SupplierRepository.Delete(id);
                _invCommonService.INVUnit.SupplierRepository.SaveChanges();
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

        private string CheckDuplicateEntry(SupplierInfoViewModel model, string supplierName)
        {
            string message = string.Empty;
            var itemTypes = new List<INV_SupplierInfo>();

            if (!string.IsNullOrWhiteSpace(supplierName))
            {
                itemTypes = (from x in _invCommonService.INVUnit.SupplierRepository.GetAll()
                             where x.SupplierName == supplierName && x.Id != model.Id
                             select x).DefaultIfEmpty().OfType<INV_SupplierInfo>().ToList();

                //itemTypeInfo = _invCommonService.INVUnit.SupplierRepository.GetAll().Where(x => !string.IsNullOrWhiteSpace(model.Code) && x.Code == model.Code).FirstOrDefault();
            }
            if (itemTypes.Any())
            {
                message += "Supplier: " + model.SupplierName + " already exists.";
            }
            return message;
        }
        #endregion
    }
}