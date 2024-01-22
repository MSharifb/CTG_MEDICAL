using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.ChartOfAccount;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Domain.FAM;

using BEPZA_MEDICAL.Web.Resources;

namespace BEPZA_MEDICAL.Web.Areas.FAM.Controllers
{
    public class ChartOfAccountController : Controller
    {
        #region Fields

        private readonly FAMCommonService _famCommonservice;
        //private readonly PIMCommonService _pimCommonservice;

        FAM_ChartOfAccount coaOut = null;
        #endregion

        #region Ctor

        public ChartOfAccountController(FAMCommonService famCommonservice  )
        {
            _famCommonservice = famCommonservice;
             
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            var model = new ChartOfAccountModel();
            PrepareModel(model);
            return View(model);
        }

        public ActionResult GetTreeView()
        {
            var model = new ChartOfAccountModel();
            PrepareModel(model);

            return View("_TreeView", model);
        }

        public ActionResult CreateNew()
        {
            var model = new ChartOfAccountModel();
            PrepareModel(model);

            model.ParentHeadCodeList = new List<SelectListItem>();
            return View("Create", model);
        }

        public ActionResult Create(int id)
        {
            var model = new ChartOfAccountModel();
            PrepareModel(model);

            var coas = _famCommonservice.FAMUnit.ChartOfAccount.GetAll().ToList();

            if (coas.FindAll(x => x.Id == id && (x.AccountHeadName.Trim().Equals("Chart of Accounts"))).Count() > 0)
                return Json("'Create' operation is not allowed for the control head 'Chart of Accounts'.", JsonRequestBehavior.AllowGet);

            GetAccountType(id, coas);

            if ((bool)coas.Find(x => x.Id == id).IsPostingAccount)
                return Json(ErrorMessages.PostingHead, JsonRequestBehavior.AllowGet);

            model.AccountHeadType = coaOut.AccountHeadType;
            model.ParentHeadCode = id.ToString();

            model.ParentHeadCodeList = coas.Where(x => !(bool)x.IsPostingAccount
                && x.AccountHeadType.ToUpper().ToCharArray()[0].Equals(coaOut.AccountHeadType.ToUpper().ToCharArray()[0]))
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.AccountHeadName,
                    Value = y.Id.ToString()
                }).ToList();

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(ChartOfAccountModel model)
        {
            var strMessage = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    
                    var entity = model.ToEntity();
                    _famCommonservice.FAMUnit.ChartOfAccount.Add(entity);
                    _famCommonservice.FAMUnit.ChartOfAccount.SaveChanges();
                    strMessage = ErrorMessages.InsertSuccessful;
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
            return new JsonResult()
            {
                Data = strMessage
            };
        }

        public ActionResult Edit(int id)
        {
            var model = new ChartOfAccountModel();
            var coas = _famCommonservice.FAMUnit.ChartOfAccount.GetAll().ToList();

            if (coas.FindAll(x => x.Id == id && (x.AccountHeadName.Trim().Equals("Chart of Accounts") || x.AccountHeadName.Trim().Equals("Asset") || x.AccountHeadName.Trim().Equals("Liability") || x.AccountHeadName.Trim().Equals("Income") || x.AccountHeadName.Trim().Equals("Expense"))).Count() > 0)
                return Json("Requested control head is not editable", JsonRequestBehavior.AllowGet);

            var entity = coas.Find(x => x.Id == id);
            model = entity.ToModel();
            PrepareModel(model);


            model.ParentHeadCodeList = coas
                .Where(x =>
                    !(bool)x.IsPostingAccount
                    && (x.AccountHeadType.ToUpper().ToCharArray()[0].Equals(model.AccountHeadType.ToUpper().ToCharArray()[0]) || x.AccountHeadName.Equals("Chart of Accounts"))
                    )
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.AccountHeadName,
                    Value = y.Id.ToString()
                }).ToList();
            model.Mode = "Edit";
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit(ChartOfAccountModel model)
        {
            var strMessage = String.Empty;

            
            var coas = _famCommonservice.FAMUnit.ChartOfAccount.GetAll().ToList();
            var childItems = new List<FAM_ChartOfAccount>();
            FindChildItems(model.Id, coas, childItems);

            if (model.IsPostingAccount && childItems.Count != 0)
            {
                strMessage="Unable to update. Child account head exists";
            }
            else
            {
                
                if (ModelState.IsValid)
                {
                    try
                    {
                        var entity = model.ToEntity();
                        _famCommonservice.FAMUnit.ChartOfAccount.Update(entity);
                        _famCommonservice.FAMUnit.ChartOfAccount.SaveChanges();
                        strMessage = ErrorMessages.UpdateSuccessful;
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
            }
            return new JsonResult()
            {
                Data = strMessage
            };
        }

        public ActionResult Delete(int id)
        {

            return View();
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var coas = _famCommonservice.FAMUnit.ChartOfAccount.GetAll().ToList();
            var childItems = new List<FAM_ChartOfAccount>();
            FindChildItems(id, coas, childItems);
            if (childItems.Count == 0)
            {
                try
                {
                    var item = coas.Find(x => x.Id == id);
                    _famCommonservice.FAMUnit.ChartOfAccount.Delete(item);
                    _famCommonservice.FAMUnit.ChartOfAccount.SaveChanges();

                    return Json
                    (
                        ErrorMessages.DeleteSuccessful,
                        JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception)
                {

                    return Json
                    (
                        ErrorMessages.DeleteFailed,
                        JsonRequestBehavior.AllowGet
                    );
                }
            }
            return Json
            (
                "Error: Control head can't be deleted",
                JsonRequestBehavior.AllowGet
            );
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTreeData()
        {

            var nodes = _famCommonservice.FAMUnit.ChartOfAccount.GetAll().ToList();

            var parentNode = nodes.Where(x => x.ParentHeadCode == "0").FirstOrDefault();

            JsTreeNode rootNode = new JsTreeNode();
            rootNode.attr = new Attributes();
            rootNode.attr.id = Convert.ToString(parentNode.Id);
            rootNode.attr.rel = "root" + Convert.ToString(parentNode.Id);
            rootNode.data = new Data();
            rootNode.data.title = Convert.ToString(parentNode.AccountHeadName);

            rootNode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";

            PopulateTree(parentNode, rootNode, nodes);

            return new JsonResult()
            {
                Data = rootNode,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Utils

        public void PopulateTree(FAM_ChartOfAccount parentNode, JsTreeNode jsTNode, List<FAM_ChartOfAccount> nodes)
        {
            jsTNode.children = new List<JsTreeNode>();
            foreach (var dr in nodes)
            {

                if (dr != null)
                {
                    if (//Convert.ToInt32(dr["level"]) == Convert.ToInt32(parentNode["level"]) + 1 && 
                                Convert.ToInt32(dr.ParentHeadCode) == parentNode.Id
                                )
                    {

                        JsTreeNode cnode = new JsTreeNode();
                        cnode.attr = new Attributes();
                        cnode.attr.id = Convert.ToString(dr.Id);
                        cnode.attr.rel = "folder" + dr.Id;
                        cnode.data = new Data();
                        cnode.data.title = Convert.ToString(dr.AccountHeadCode + "-" + dr.AccountHeadName);

                        cnode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";

                        jsTNode.children.Add(cnode);


                        PopulateTree(dr, cnode, nodes);

                    }
                }
            }
        }


        private ChartOfAccountModel PrepareModel(ChartOfAccountModel model)
        {

            PopulateCombo(model);

            return model;
        }

        private void PopulateCombo(ChartOfAccountModel model)
        {
            model.AccountHeadTypeList.Add(new SelectListItem() { Text = "Asset", Value = "A" });
            model.AccountHeadTypeList.Add(new SelectListItem() { Text = "Liability", Value = "L" });
            model.AccountHeadTypeList.Add(new SelectListItem() { Text = "Income", Value = "I" });
            model.AccountHeadTypeList.Add(new SelectListItem() { Text = "Expense", Value = "E" });

            model.CashBankTypeList.Add(new SelectListItem() { Text = "Cash", Value = "Cash" });
            model.CashBankTypeList.Add(new SelectListItem() { Text = "Bank", Value = "Bank" });
            model.CashBankTypeList.Add(new SelectListItem() { Text = "Others", Value = "Others", Selected = true });


            model.ParentHeadCodeList = _famCommonservice.FAMUnit.ChartOfAccount.GetAll().ToList()
                .Where(x => !(bool)x.IsPostingAccount)
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.AccountHeadName,
                    Value = y.Id.ToString()
                }).ToList();
        }

        public JsonResult GetParentHead(string accountType)
        {

            var aocs = _famCommonservice.FAMUnit.ChartOfAccount.GetAll()
                .Where(x => x.AccountHeadType.ToUpper().ToCharArray()[0].Equals(accountType.ToUpper().ToCharArray()[0])
                && !(bool)x.IsPostingAccount)
                .Select(y =>
                new
                {
                    text = y.AccountHeadName,
                    value = y.Id
                });
            return Json(aocs, JsonRequestBehavior.AllowGet);

        }

        private void GetAccountType(int parentHeadCode, List<FAM_ChartOfAccount> coas)
        {
            var coa = coas.Where(x => x.Id == parentHeadCode).FirstOrDefault();
            if (coa.ParentHeadCode == "1")
                coaOut = coa;
            else
                GetAccountType(Convert.ToInt32(coa.ParentHeadCode), coas);
        }

        private void FindChildItems(int id, List<FAM_ChartOfAccount> coas, List<FAM_ChartOfAccount> childItems)
        {
            foreach (var item in coas)
            {
                if (Convert.ToInt32(item.ParentHeadCode) == id)
                {
                    childItems.Add(item);
                    FindChildItems(item.Id, coas, childItems);
                }
            }
        }
        #endregion


    }
}