using BEPZA_MEDICAL.Domain.PMI;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.Controllers
{
    public class HeadController : Controller
    {
        #region Fields
        private readonly PMICommonService _pmiCommonService;
        #endregion

        #region Ctor
        public HeadController(PMICommonService pmiCommonService)
        {
            this._pmiCommonService = pmiCommonService;
        }
        #endregion

        // GET: PMI/Head
        #region Action
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, HeadViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<HeadViewModel> list = (from unit in _pmiCommonService.PMIUnit.HeadRepository.GetAll()
                                                  select new HeadViewModel()
                                                  {
                                                      Id = unit.Id,
                                                      HeadName = unit.HeadName,
                                                      IsParentHead =Convert.ToBoolean(unit.IsParentHead),
                                                  }).OrderBy(x => x.HeadName).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Name")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HeadName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HeadName).ToList();
                }
            }

            #endregion

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.HeadName))
                {
                    list = list.Where(x => x.HeadName.Trim().ToLower().Contains(model.HeadName.Trim().ToLower())).ToList();
                }
            }

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
                  d.HeadName,
                  d.IsParentHead == true ? "Yes" : "No",
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            HeadViewModel model = new HeadViewModel();
            DropdownList(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(HeadViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            var strMessage = string.Empty; // CheckDuplicateEntry(model, model.HeadName, model.ItemCode);
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
                    _pmiCommonService.PMIUnit.HeadRepository.Add(entity);
                    _pmiCommonService.PMIUnit.HeadRepository.SaveChanges();
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
            DropdownList(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _pmiCommonService.PMIUnit.HeadRepository.GetByID(Id);
            HeadViewModel model = entity.ToModel();
            DropdownList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(HeadViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;
            var strMessage = string.Empty; // CheckDuplicateEntry(model, model.HeadName, model.ItemCode);
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
                    _pmiCommonService.PMIUnit.HeadRepository.Update(entity);
                    _pmiCommonService.PMIUnit.HeadRepository.SaveChanges();
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
                }
            }
            DropdownList(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;
            var tempPeriod = _pmiCommonService.PMIUnit.HeadRepository.GetByID(id);
            try
            {
                _pmiCommonService.PMIUnit.HeadRepository.Delete(id);
                _pmiCommonService.PMIUnit.HeadRepository.SaveChanges();
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

        public void DropdownList(HeadViewModel model)
        {
            var headlist = _pmiCommonService.PMIUnit.HeadRepository.GetAll().ToList();

            var list = new List<SelectListItem>();
            foreach (var item in headlist)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.HeadName,
                    Value = item.Id.ToString()
                });
            }
            model.ParentHeadList = list;
        }
    }
}