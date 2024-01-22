using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.Configuration;
using BEPZA_MEDICAL.Domain.FAM;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Web.Resources;
 

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class ConfigurationController : Controller
    {
        #region Fields

        private readonly FAMCommonService _famCommonservice;
        //private readonly PIMCommonService _pimCommonservice;

        #endregion

        #region Ctor

        //public ConfigurationController(FAMCommonService famCommonservice
        //    ,  )
        //{
        //    _famCommonservice = famCommonservice;
        //     
        //}
        public ConfigurationController(FAMCommonService famCommonservice
            )
        {
            _famCommonservice = famCommonservice;
        //     
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            var model = PrepareModel();

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCommonConfigList(JqGridRequest request, string type)
        {
            IList<CommonConfigGet> list = new List<CommonConfigGet>();
            string filterExpression = String.Empty;
            int totalRecords = 0;
            //string displayName = MyAppSession.DisplayName;

            if (!string.IsNullOrEmpty(type))
            {
                //displayName =MyAppSession.DisplayName;
                int startRowIndex = request.PageIndex * request.RecordsCount + 1;

                list = _famCommonservice.FAMUnit.FunctionRepository.GetAllCommonConfig(type, 0, null, request.SortingName, request.SortingOrder.ToString(), startRowIndex, request.RecordsCount, out totalRecords);

                totalRecords = _famCommonservice.FAMUnit.FunctionRepository.GetAllCommonConfig(type, 0, null, request.SortingName, request.SortingOrder.ToString(), 1, request.RecordsCount, out totalRecords).Count();
                JqGridResponse response = new JqGridResponse()
                {
                    TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                    PageIndex = request.PageIndex,
                    TotalRecordsCount = totalRecords
                };

                foreach (CommonConfigGet obj in list)
                {
                    response.Records.Add(new JqGridRecord(Convert.ToString(obj.Id), new List<object>()
                {
                    obj.Id,
                    obj.Name,
                    obj.SortOrder,
                    obj.Remarks,
                }));
                }
                return new JqGridJsonResult() { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JqGridJsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult Create(ConfigurationModel model, string type)
        //{
        //    int result = 0;
        //    try
        //    {
        //        //var result = _famCommonservice.FAMUnit.FunctionRepository.CommonConfigTypeDML(type, 0, model.Name, Convert.ToInt32(model.SortOrder), model.Remarks, User.Identity.Name, "I");
        //        result = _pimCommonservice.PIMUnit.FunctionRepository.CommonConfigTypeDML(type, 0, model.Name, Convert.ToInt32(model.SortOrder), model.Remarks??string.Empty, User.Identity.Name, "I");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            Success = 0,
        //            Message = ErrorMessages.InsertFailed
        //        });
        //    }

        //    return Json(new
        //    {
        //        Success = result,
        //        Message = result>=0 ? ErrorMessages.InsertSuccessful : ErrorMessages.InsertFailed
        //    });

        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult Edit(ConfigurationModel model, string type)
        //{
        //    int result = 0;
        //    try
        //    {

        //        result = _pimCommonservice.PIMUnit.FunctionRepository.CommonConfigTypeDML(type, model.Id, model.Name, Convert.ToInt32(model.SortOrder), model.Remarks??string.Empty, User.Identity.Name, "U");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            Success = 0,
        //            Message = ErrorMessages.UpdateFailed
        //        });
        //    }

        //    return Json(new
        //    {
        //        Success = result,
        //        Message = result >= 0 ? ErrorMessages.UpdateSuccessful : ErrorMessages.UpdateFailed
        //    });

        //}

        //[HttpPost, ActionName("Delete")]
        //public JsonResult DeleteConfirmed(int id, string type)
        //{
        //    int result = 0;
        //    try
        //    {

        //        result = _pimCommonservice.PIMUnit.FunctionRepository.CommonConfigTypeDML(type, id, "", 0, "", User.Identity.Name, "d");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            Success = 0,
        //            Message = ErrorMessages.DeleteFailed
        //        });
        //    }

        //    return Json(new
        //    {
        //        Success = result,
        //        Message = result >= 0 ? ErrorMessages.DeleteSuccessful : ErrorMessages.DeleteFailed
        //    });
        //}
        
        #endregion

        #region Utils
        private ConfigurationModel PrepareModel()
        {
            var model = new ConfigurationModel();
            var configTypes = _famCommonservice.FAMUnit.ConfigType.GetAll().Where(x => x.ModuleId == 3).Select(x =>
                new BEPZA_MEDICAL.Web.Areas.FAM.Models.Configuration.CommonConfigType()
                {
                    DisplayName = x.DisplayName,
                    SortOrder = x.SortOrder

                }).OrderBy(x => x.SortOrder).ToList();

            model.CommonConfigTypes = configTypes;

            return model;
        }
        #endregion

    }
}
