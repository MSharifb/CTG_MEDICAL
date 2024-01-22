using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.DAL.PRM;
using System.Web.Helpers;
using System.IO;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class SetupController : Controller
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        #endregion

        #region Constructor

        public SetupController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }

        #endregion

        #region Company Information

        public ActionResult CompanyInformationIndex(string bText, string type)
        {
            CompanyInformationViewModel model = new CompanyInformationViewModel();
            var entity = _prmCommonservice.PRMUnit.CompanyInformation.GetAll().FirstOrDefault();
            if (entity != null)
            {
                model = entity.ToModel();
                if (model.CompanyLogo.Length > 0)
                {
                    model.HasPhoto = true;
                }
            }

            if (entity != null)
            {
                model.btnText = "Update";
            }
            if (bText == "Save" && type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            else if (bText == "Update" && type == "success")
            {
                model.btnText = "Update";
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CompanyInformationIndex([Bind(Exclude = "Attachment")] CompanyInformationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var attachment = Request.Files["attachment"];
                    string errorList = "";
                    var entity = model.ToEntity();
                    entity.EDate = DateTime.Now;
                    entity.EUser = User.Identity.Name;
                    HttpFileCollectionBase files = Request.Files;
                    foreach (string fileTagName in files)
                    {
                        if (fileTagName == "Attachment")
                        {
                            HttpPostedFileBase file = Request.Files[fileTagName];
                            if (file.ContentLength > 0)
                            {
                                // Due to the limit of the max for a int type, the largest file can be
                                // uploaded is 2147483647, which is very large anyway.
                                int size = file.ContentLength;
                                string name = file.FileName;
                                int position = name.LastIndexOf("\\");
                                name = name.Substring(position + 1);
                                string contentType = file.ContentType;
                                byte[] fileData = new byte[size];
                                file.InputStream.Read(fileData, 0, size);
                                //entity.FileName = name;
                                entity.CompanyLogo = fileData;
                            }
                        }

                    }
                    if (model.Id > 0)
                    {
                        _prmCommonservice.PRMUnit.CompanyInformation.Update(entity);
                    }
                    else {
                        _prmCommonservice.PRMUnit.CompanyInformation.Add(entity);
                    }

                    if (errorList.Length == 0)
                    {
                        _prmCommonservice.PRMUnit.CompanyInformation.SaveChanges();
                        return RedirectToAction("CompanyInformationIndex", new { bText = model.btnText, type = "success" });
                    }

                    if (errorList.Count() > 0)
                    {
                        model.errClass = "failed";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                    }

                    //_prmCommonservice.PRMUnit.CompanyInformation.SaveChanges();
                    //model.IsSuccessful = true;
                    //model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    //return RedirectToAction("CompanyInformationIndex", new { IsSuccessful = model.IsSuccessful, message = model.Message });

                }

            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            return View(model);
        }


        #endregion
    }
}
