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
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class CommonConfigController : Controller
    {
        #region Fields
        private static int moduleID = 0;
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Ctor
        public CommonConfigController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }
        #endregion

        #region Actions

        public ViewResult Index(string name)
        {
            List<CommonConfigType> list = _prmCommonservice.PRMUnit.ConfigTypeRepository.GetAll().OrderBy(x => x.DisplayName).ToList();
            list = list.Where(d => d.ModuleId == (int)ApplicationModule.PRM).OrderBy(q => q.DisplayName).ToList();

            List<CommonConfigTypeViewModel> modellist = list.ToModelList();
            CommonConfigViewModel viewModel = new CommonConfigViewModel();
            viewModel.CommonConfigType = modellist;
            if (!string.IsNullOrEmpty(name))
            {
                moduleID = (from n in list where n.DisplayName == name select n.ModuleId).FirstOrDefault();
            }
            viewModel.CommonConfigType = modellist;
            if (name != null)
            {
                ViewBag.TitleName = name;
                MyAppSession.DisplayName = name;
            }
            else
            {
                MyAppSession.DisplayName = string.Empty;
            }
            return View("Index", viewModel);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetCommonConfigList(JqGridRequest request)
        {
            IList<CommonConfigGetResult> list = new List<CommonConfigGetResult>();
            string filterExpression = String.Empty;
            int totalRecords = 0;
            string displayName = MyAppSession.DisplayName;

            if (!string.IsNullOrEmpty(displayName))
            {
                displayName = MyAppSession.DisplayName;
                int startRowIndex = request.PageIndex * request.RecordsCount + 1;
                try
                {
                    list = _prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig(displayName, 0, null, request.SortingName, request.SortingOrder.ToString(), startRowIndex, request.RecordsCount, out totalRecords).OrderBy(x => x.Name).ToList();
                }
                catch (Exception ex)
                {

                }
                JqGridResponse response = new JqGridResponse()
                {
                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                    PageIndex = request.PageIndex,
                    TotalRecordsCount = totalRecords
                };

                foreach (CommonConfigGetResult obj in list)
                {
                    response.Records.Add(new JqGridRecord(Convert.ToString(obj.Id), new List<object>()
                {
                    obj.Id,
                    obj.Name,
                    obj.SortOrder,
                    obj.Remarks,
                }));
                }
                return new JqGridJsonResult() { Data = response };
            }
            else
            {
                return new JqGridJsonResult() { };
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create(FormCollection collection)
        {
            int result = 0;
            string errMsg = "";

            try
            {
                if (ModelState.IsValid)
                {
                    CommonConfigViewModel model = new CommonConfigViewModel();
                    model.Id = Convert.ToInt32(collection["Id"]);
                    model.SortOrder = Convert.ToInt32(collection["SortOrder"]);
                    model.Name = collection["Name"].Trim();
                    model.Remarks = collection["Remarks"];

                    if (!string.IsNullOrEmpty(MyAppSession.DisplayName))
                    {
                        if (!CheckDuplicate(model, "add", MyAppSession.DisplayName))
                        {
                            result = _prmCommonservice.PRMUnit.FunctionRepository.CommonConfigTypeDML(MyAppSession.DisplayName, 0, model.Name, Convert.ToInt32(model.SortOrder), model.Remarks, User.Identity.Name, "I");

                            if (result <= 0)
                            {
                                errMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                            }
                            errMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        }
                        else
                        {
                            result = 0;
                            errMsg = Resources.ErrorMessages.UniqueIndex;
                        }
                    }
                    else
                    {
                        result = 0;
                        errMsg = "Please Select any item from the left menu";
                    }
                }
                else
                {
                    result = 0;
                    errMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                }

            }
            catch (Exception ex)
            {
                result = 0;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });

        }

        public JsonResult Edit(int id)
        {
            string displayName = "";
            CommonConfigViewModel model = new CommonConfigViewModel();
            if (MyAppSession.DisplayName != null)
            {
                displayName = MyAppSession.DisplayName;
                ViewBag.TitleName = displayName;
            }
            int totalRecords = 0;
            var obj = _prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig(displayName, id, "", "", "", 0, 0, out totalRecords).SingleOrDefault();
            return Json(new
            {
                id = obj.Id,
                name = obj.Name,
                sortOrder = obj.SortOrder,
                remarks = obj.Remarks

            }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Edit(FormCollection collection)
        {
            int result = 0;
            string errMsg = "";

            try
            {
                if (ModelState.IsValid)
                {
                    CommonConfigViewModel model = new CommonConfigViewModel();
                    model.Id = Convert.ToInt32(collection["Id"]);
                    model.SortOrder = Convert.ToInt32(collection["SortOrder"]);
                    model.Name = collection["Name"].Trim();
                    model.Remarks = collection["Remarks"];
                    if (MyAppSession.DisplayName != null)
                    {
                        if (!CheckDuplicate(model, "edit", MyAppSession.DisplayName))
                        {
                            result = _prmCommonservice.PRMUnit.FunctionRepository.CommonConfigTypeDML(MyAppSession.DisplayName, model.Id, model.Name, Convert.ToInt32(model.SortOrder), model.Remarks, User.Identity.Name, "U");

                            if (result <= 0)
                            {
                                errMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                            }
                            errMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                        }
                        else
                        {
                            result = 0;
                            errMsg = Resources.ErrorMessages.UniqueIndex;
                        }
                    }
                }
                else
                {
                    result = 0;
                    errMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }

            }
            catch (Exception ex)
            {
                result = 0;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            string displayName = "";
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            if (MyAppSession.DisplayName != null)
            {
                displayName = MyAppSession.DisplayName;
                ViewBag.TitleName = displayName;

                bool result = true;
                try
                {
                    result = _prmCommonservice.PRMUnit.FunctionRepository.CommonConfigTypeDML(displayName, id, "", 0, "", User.Identity.Name, "d") > 0;
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (ex.InnerException != null && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                        {
                            SqlException sqlException = ex.InnerException as SqlException;
                            errMsg = CommonExceptionMessage.GetSqlExceptionMessage(sqlException.Number);
                        }
                        result = false;
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }
                return Json(new
                {
                    Success = result ? 1 : 0,
                    Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
                });
            }
            else
            {
                return new JsonResult { };
            }
        }

        private bool CheckDuplicate(CommonConfigViewModel model, string strMode, string displayName)
        {
            dynamic objComConfig = null;
            int totalRecords = 0;
            try
            {
                if (strMode == "add")
                {
                    objComConfig = _prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig(displayName, 0, model.Name, "", "", 0, 0, out totalRecords).Where(x => x.Name == model.Name).FirstOrDefault();
                    //  var obj = _prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig(displayName, id, "", "", "", 0, 0, out totalRecords).SingleOrDefault();

                }
                else
                {
                    // objComConfig = _jobDesignationService.PRMUnit.DesignationRepository.Get(x => x.Name == model.Name && x.Id != model.Id).FirstOrDefault();
                    objComConfig = _prmCommonservice.PRMUnit.FunctionRepository.GetAllCommonConfig(displayName, 0, model.Name, "", "", 0, 0, out totalRecords).Where(x => x.Name == model.Name && x.Id != model.Id).FirstOrDefault();
                }

                if (objComConfig != null)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }


        #endregion
    }
}
