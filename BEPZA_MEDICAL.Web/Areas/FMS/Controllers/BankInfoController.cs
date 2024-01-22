using BEPZA_MEDICAL.DAL.FMS;
using BEPZA_MEDICAL.Domain.FMS;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
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

namespace BEPZA_MEDICAL.Web.Areas.FMS.Controllers
{
    public class BankInfoController : BaseController
    {

        #region Fields

        private readonly FMSCommonService _fmsCommonService;
        private readonly PRMCommonSevice _prmCommonService;

        #endregion

        #region Ctor
        public BankInfoController(FMSCommonService fmsfCommonService, PRMCommonSevice prmCommonService)
        {
            this._fmsCommonService = fmsfCommonService;
            this._prmCommonService = prmCommonService;

        }
        #endregion

        #region Actions
        //
        // GET: FMS/BankInfoBranchDetail
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, BankInfoViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from bank in _fmsCommonService.FMSUnit.BankInfoRepository.GetAll()
                        select new BankInfoViewModel()

                                                      {
                                                          Id = bank.Id,
                                                          BankName = bank.BankName,
                                                          BankCode = bank.BankCode,
                                                          BankType = bank.BankType
                                                      }).OrderBy(x => x.BankName).ToList();

            if (request.Searching)
            {
                if ((viewModel.BankName != null && viewModel.BankName != ""))
                {
                    list = list.Where(d => d.BankName.ToLower().Contains(viewModel.BankName.ToLower())).ToList();

                }
                if ((viewModel.BankCode != null && viewModel.BankCode != ""))
                {
                    list = list.Where(d => d.BankCode.ToLower() == viewModel.BankCode.ToLower()).ToList();
                }

                if ((viewModel.BankType != null && viewModel.BankType != ""))
                {
                    list = list.Where(d => d.BankType == viewModel.BankType).ToList();
                }

            }
            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "BankName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BankName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BankName).ToList();
                }
            }

            if (request.SortingName == "BankCode")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BankCode).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BankCode).ToList();
                }
            }
            if (request.SortingName == "BankType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BankType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BankType).ToList();
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
                    d.BankName,
                    d.BankCode,                   
                    d.BankType
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }


        public ActionResult Create()
        {
            BankInfoViewModel model = new BankInfoViewModel();
            populateDropdown(model);
            model.strMode = "Create";
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(BankInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                errorList = BusinessLogicValidation(model, model.Id);
                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    //model.ZoneInfoId = LoggedUserZoneInfoId;
                    model = GetInsertUserAuditInfo(model, true);
                    var entity = CreateEntity(model, true);
                    if (errorList.Length == 0)
                    {
                        _fmsCommonService.FMSUnit.BankInfoRepository.Add(entity);
                        _fmsCommonService.FMSUnit.BankInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
                model.errClass = "failed";              
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            populateDropdown(model);
            setDetailList(model);
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var entity = _fmsCommonService.FMSUnit.BankInfoRepository.GetByID(id);
            var model = entity.ToModel();

            List<BankInfoBranchDetailViewModel> brannhes = (from branch in _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.GetAll().Where(q => q.BankInfoId == id)
                                                            join country in _prmCommonService.PRMUnit.CountryRepository.Fetch() on branch.CountryId equals country.Id
                                                            join bank in _fmsCommonService.FMSUnit.BankInfoRepository.Fetch() on branch.BankInfoId equals bank.Id
                                                            select new BankInfoBranchDetailViewModel()
                                                                        {
                                                                            Id = branch.Id,
                                                                            BankInfoId = branch.BankInfoId,
                                                                            BranchName = branch.BranchName,
                                                                            BranchAddress = branch.BranchAddress,
                                                                            SWIFTCode=branch.SWIFTCode,
                                                                            BranchContactNo = branch.BranchContactNo,
                                                                            BranchEmail = branch.BranchEmail,
                                                                            CountryId = branch.CountryId,
                                                                            CountryName = country.Name
                                                                        }).OrderBy(o => o.Id).ToList();
            model.BankInfoBranchList = brannhes;
            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(BankInfoViewModel model)
        {
            try
            {
                string errorList = string.Empty;
                errorList = BusinessLogicValidation(model, model.Id);
                if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
                {
                    model = GetInsertUserAuditInfo(model, false);
                    var entity = CreateEntity(model, false);
                    if (errorList.Length == 0)
                    {
                        _fmsCommonService.FMSUnit.BankInfoRepository.Update(entity);
                        _fmsCommonService.FMSUnit.BankInfoRepository.SaveChanges();
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.errClass = "failed";
                    model.ErrMsg = errorList;
                }

            }
            catch (Exception ex)
            {
                model.errClass = "failed";              
                if (ex.InnerException != null && ex.InnerException is SqlException)
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    model.ErrMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                }
                else
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            populateDropdown(model);
            setDetailList(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _fmsCommonService.FMSUnit.BankInfoRepository.Delete(id);
                _fmsCommonService.FMSUnit.BankInfoRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            try
            {
                _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.Delete(id);
                _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.SaveChanges();
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

        #endregion

        #region Private Method
        private void populateDropdown(BankInfoViewModel model)
        {
            dynamic ddlList;

            #region Country ddl
            ddlList = _prmCommonService.PRMUnit.CountryRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.CountryList = Common.PopulateDllList(ddlList);
            #endregion
        }

        //check duplicate bank name
        [NoCache]
        public string BusinessLogicValidation(BankInfoViewModel model, int id)
        {
            var exist = false;
            string errorMessage = string.Empty;
            if (id < 1)
            {
                exist = _fmsCommonService.FMSUnit.BankInfoRepository.Get(q => q.BankName.ToLower() == model.BankName.ToLower()).Any();

                if (exist)
                {
                    return errorMessage = "Duplicate Bank Name";
                }
            }
            else
            {
                exist = _fmsCommonService.FMSUnit.BankInfoRepository.Get(q => q.BankName.ToLower() == model.BankName.ToLower() && id != q.Id).Any();

                if (exist)
                {
                    return errorMessage = "Duplicate Bank Name";
                }
            }
            return errorMessage;

        }
        private BankInfoViewModel GetInsertUserAuditInfo(BankInfoViewModel model, bool pAddEdit)
        {
            if (pAddEdit)
            {
                model.IUser = User.Identity.Name;
                model.IDate = DateTime.Now;
                foreach (var child in model.BankInfoBranchList)
                {
                    child.IUser = User.Identity.Name;
                    child.IDate = DateTime.Now;
                }

            }
            else
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                foreach (var child in model.BankInfoBranchList)
                {
                    child.IUser = model.IUser;
                    child.IDate = model.IDate;

                    child.EUser = User.Identity.Name;
                    child.EDate = DateTime.Now;
                }
            }

            return model;
        }


        private FMS_BankInfo CreateEntity(BankInfoViewModel model, bool pAddEdit)
        {
            var entity = model.ToEntity();

            //branches
            foreach (var item in model.BankInfoBranchList)
            {
                var fms_BankInfoBranchDetail = new FMS_BankInfoBranchDetail();

                fms_BankInfoBranchDetail.Id = item.Id;
                fms_BankInfoBranchDetail.BranchName = item.BranchName;
                fms_BankInfoBranchDetail.BranchAddress = item.BranchAddress;
                fms_BankInfoBranchDetail.SWIFTCode = item.SWIFTCode;
                fms_BankInfoBranchDetail.BranchContactNo = item.BranchContactNo;
                fms_BankInfoBranchDetail.BranchEmail = item.BranchEmail;
                fms_BankInfoBranchDetail.CountryId = Convert.ToInt32(item.CountryId);
                fms_BankInfoBranchDetail.IUser = User.Identity.Name;
                fms_BankInfoBranchDetail.IDate = DateTime.Now;

                if (pAddEdit)
                {
                    fms_BankInfoBranchDetail.IUser = User.Identity.Name;
                    fms_BankInfoBranchDetail.IDate = DateTime.Now;
                    entity.FMS_BankInfoBranchDetail.Add(fms_BankInfoBranchDetail);
                }
                else
                {
                    fms_BankInfoBranchDetail.BankInfoId = model.Id;
                    fms_BankInfoBranchDetail.EUser = User.Identity.Name;
                    fms_BankInfoBranchDetail.EDate = DateTime.Now;

                    if (item.Id == 0)
                    {
                        _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.Add(fms_BankInfoBranchDetail);
                    }
                    else
                    {
                        _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.Update(fms_BankInfoBranchDetail);

                    }
                    _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.SaveChanges();
                }

            }

            return entity;
        }

        private void setDetailList(BankInfoViewModel model)
        {

            List<BankInfoBranchDetailViewModel> brannhes = (from bank in _fmsCommonService.FMSUnit.BankInfoRepository.GetAll()
                                                            join branch in _fmsCommonService.FMSUnit.BankInfoBranchDetailRepository.Fetch() on bank.Id equals branch.BankInfoId
                                                            join country in _prmCommonService.PRMUnit.CountryRepository.Fetch() on branch.CountryId equals country.Id
                                                            where (bank.Id == model.Id)
                                                            select new BankInfoBranchDetailViewModel()
                                                            {
                                                                Id = branch.Id,
                                                                BankInfoId = branch.BankInfoId,
                                                                BranchName = branch.BranchName,
                                                                BranchAddress = branch.BranchAddress,
                                                                SWIFTCode = branch.SWIFTCode,
                                                                BranchContactNo = branch.BranchContactNo,
                                                                BranchEmail = branch.BranchEmail,
                                                                CountryId = branch.CountryId,
                                                                CountryName = country.Name
                                                            }).OrderBy(o => o.Id).ToList();

            model.BankInfoBranchList = brannhes;
            //model.ShowRecord = "Show";
        }

        #endregion
        
        //public ActionResult AddDetail(BankInfoViewModel model)
        //{

        //    model.BankInfoBranchList = new List<BankInfoBranchDetailViewModel>();

        //    var branch = new BankInfoBranchDetailViewModel();
        //    branch.Id = 0;
        //    model.Id = 0;
        //    branch.BankInfoId = Convert.ToInt32(model.BankInfoId);
        //    branch.BranchName = model.BranchName;
        //    branch.BranchAddress = model.BranchAddress;
        //    branch.BranchContactNo = model.BranchContactNo;
        //    branch.BranchEmail = model.BranchEmail;
        //    branch.CountryId = Convert.ToInt32(model.CountryId);
        //    branch.CountryName = model.CountryName;

        //    model.BankInfoBranchList.Add(branch);
        //    return PartialView("_Details", model);

        //}


    }
}