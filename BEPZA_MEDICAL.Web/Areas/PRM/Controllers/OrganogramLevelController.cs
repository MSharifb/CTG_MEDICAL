using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using System.Text;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class OrganogramLevelController : BaseController
    {
        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        PRM_OrganogramLevel coaOut = null;
        #endregion

        #region Ctor

        public OrganogramLevelController(PRMCommonSevice pPRMCommonService)
        {
            _prmCommonservice = pPRMCommonService;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            var model = new OrganogramLevelViewModel();
            PrepareModel(model);
            return View(model);
        }

        public ActionResult GetTreeView()
        {
            var model = new OrganogramLevelViewModel();
            PrepareModel(model);

            return View("_TreeView", model);
        }

        public ActionResult CreateNew()
        {
            var model = new OrganogramLevelViewModel();
            PrepareModel(model);
            model.Mode = "Create";
            return View("Create", model);
        }

        public ActionResult Create(int id)
        {
            var model = new OrganogramLevelViewModel();
            PrepareModel(model);

            var OLevel = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll().ToList();

            GetAccountType(id, OLevel);

            model.OrganogramTypeId = coaOut.OrganogramTypeId;
            model.ParentId = id;

            model.Mode = "Create";
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(OrganogramLevelViewModel model)
        {
            var strMessage = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;


                    _prmCommonservice.PRMUnit.OrganogramLevelRepository.Add(entity);
                    _prmCommonservice.PRMUnit.OrganogramLevelRepository.SaveChanges();
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
            var model = new OrganogramLevelViewModel();
            var coas = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll().ToList();
            var obj = coas.Where(x => x.Id == id && x.IsEditable == false).FirstOrDefault();
            //if (coas.FindAll(x => x.Id == id && (x.LevelName.Trim().Equals(AppConstant.ClientName))).Count() > 0)
            //{
            //    return Json("Requested control head is not editable", JsonRequestBehavior.AllowGet);
            //}

            if (coas.Where(x => x.Id == id && x.IsEditable == false).Count() > 0)
            {
                return Json("This label is not editable", JsonRequestBehavior.AllowGet);
            }
            var entity = coas.Find(x => x.Id == id);
            model = entity.ToModel();
            PrepareModel(model);
            model.OrganogramTypeName = entity.PRM_OrganogramType.Name;
            model.Mode = "Edit";
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit(OrganogramLevelViewModel model)
        {
            var strMessage = String.Empty;

            var listOrganogram = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll().ToList();
            var childItems = new List<PRM_OrganogramLevel>();
            FindChildItems(model.Id, listOrganogram, childItems);

            //if (childItems.Count != 0)
            //{
            //    strMessage = "Unable to update. Child level exists";
            //}
            //else
            //{
            if (ModelState.IsValid)
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;
                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    _prmCommonservice.PRMUnit.OrganogramLevelRepository.Update(entity);
                    _prmCommonservice.PRMUnit.OrganogramLevelRepository.SaveChanges();
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
            //}

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
            var coas = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll().ToList();
            var childItems = new List<PRM_OrganogramLevel>();
            FindChildItems(id, coas, childItems);
            if (childItems.Count == 0)
            {
                try
                {
                    var item = coas.Find(x => x.Id == id);
                    _prmCommonservice.PRMUnit.OrganogramLevelRepository.Delete(item);
                    _prmCommonservice.PRMUnit.OrganogramLevelRepository.SaveChanges();

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
                "Error: Unable to delete. Child level exists.",
                JsonRequestBehavior.AllowGet
            );
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTreeData()
        {

            var nodes = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll().Where(q => q.ZoneInfoId == null || q.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            var parentNode = nodes.Where(x => x.ParentId == 0).FirstOrDefault();

            JsTreeNode rootNode = new JsTreeNode();

            if (parentNode != null)
            {
                rootNode.attr = new Attributes();
                rootNode.attr.id = Convert.ToString(parentNode.Id);
                rootNode.attr.rel = "root" + Convert.ToString(parentNode.Id);
                rootNode.data = new Data();

                StringBuilder lvlName = GenerateNodeText(parentNode);
                rootNode.data.title = Convert.ToString(lvlName);
                rootNode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";
                PopulateTree(parentNode, rootNode, nodes);
            }

            return new JsonResult()
            {
                Data = rootNode,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTreeAllZonesData()
        {
            var nodes = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll().ToList();
            var parentNode = nodes.Where(x => x.ParentId == 0).FirstOrDefault();

            JsTreeNode rootNode = new JsTreeNode();

            if (parentNode != null)
            {
                rootNode.attr = new Attributes();
                rootNode.attr.id = Convert.ToString(parentNode.Id);
                rootNode.attr.rel = "root" + Convert.ToString(parentNode.Id);
                rootNode.data = new Data();

                StringBuilder lvlName = GenerateNodeText(parentNode);
                rootNode.data.title = Convert.ToString(lvlName);
                rootNode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";
                PopulateTree(parentNode, rootNode, nodes);
            }

            return new JsonResult()
            {
                Data = rootNode,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        private static StringBuilder GenerateNodeText(PRM_OrganogramLevel parentNode)
        {
            StringBuilder lvlName = new StringBuilder();
            lvlName.Append(parentNode.LevelName);

            if (parentNode.PRM_OrganogramType != null)
            {
                lvlName.Append(" [");
                lvlName.Append(parentNode.PRM_OrganogramType.Name);
                lvlName.Append("]");
            }
            return lvlName;
        }

        public PartialViewResult TreeViewSearchList(string OrgIdentityName)
        {
            OrganogramLevelViewModel model = new OrganogramLevelViewModel();
            model.OrgIdentityName = OrgIdentityName;

            return PartialView("_TreeViewSearch", model);
        }

        public PartialViewResult AllZoneTreeViewSearchList(string OrgIdentityName)
        {
            OrganogramLevelViewModel model = new OrganogramLevelViewModel();
            model.OrgIdentityName = OrgIdentityName;

            return PartialView("_AllZoneTreeViewSearch", model);
        }
        #endregion

        #region Utils

        public void PopulateTree(PRM_OrganogramLevel parentNode, JsTreeNode jsTNode, List<PRM_OrganogramLevel> nodes)
        {
            StringBuilder nodeText = new StringBuilder();
            jsTNode.children = new List<JsTreeNode>();
            foreach (var dr in nodes)
            {
                if (dr != null)
                {
                    if (dr.ParentId == parentNode.Id)
                    {
                        JsTreeNode cnode = new JsTreeNode();
                        cnode.attr = new Attributes();
                        cnode.attr.id = Convert.ToString(dr.Id);
                        cnode.attr.rel = "folder" + dr.Id;
                        cnode.data = new Data();
                        nodeText = GenerateNodeText(dr);
                        cnode.data.title = Convert.ToString(nodeText);

                        cnode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";

                        jsTNode.children.Add(cnode);
                        PopulateTree(dr, cnode, nodes);
                    }
                }
            }
        }

        private OrganogramLevelViewModel PrepareModel(OrganogramLevelViewModel model)
        {
            PopulateCombo(model);
            return model;
        }

        private void PopulateCombo(OrganogramLevelViewModel model)
        {
            var ddl = _prmCommonservice.PRMUnit.OrganogramTypeRepository.GetAll().Where(x => x.IsVisible == true).OrderBy(x => x.SortOrder).ToList();
            model.OrganogramTypeList = Common.PopulateDllList(ddl);

        }

        public JsonResult GetParentHead(string accountType)
        {
            var aocs = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll()
                .Select(y =>
                new
                {
                    text = y.LevelName,
                    value = y.Id
                });

            return Json(aocs, JsonRequestBehavior.AllowGet);
        }

        private void GetAccountType(int parentId, List<PRM_OrganogramLevel> listOL)
        {
            var coa = listOL.Where(x => x.Id == parentId).FirstOrDefault();

            if (coa.ParentId == 0)
                coaOut = coa;
            else
                GetAccountType(coa.ParentId, listOL);
        }

        private void FindChildItems(int id, List<PRM_OrganogramLevel> coas, List<PRM_OrganogramLevel> childItems)
        {
            foreach (var item in coas)
            {
                if (item.ParentId == id)
                {
                    childItems.Add(item);
                    FindChildItems(item.Id, coas, childItems);
                }
            }
        }
        #endregion
    }
}