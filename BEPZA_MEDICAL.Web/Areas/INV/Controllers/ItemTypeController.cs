using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Domain.INV;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.ItemType;
using System.Text;
using BEPZA_MEDICAL.Web.Controllers;

namespace BEPZA_MEDICAL.Web.Areas.INV.Controllers
{
    public class ItemTypeController : BaseController
    {
        #region Fields
        private readonly INVCommonService _invCommonservice;
        #endregion

        #region Ctor

        public ItemTypeController(INVCommonService invCommonService)
        {
            _invCommonservice = invCommonService;
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            var model = new ItemTypeViewModel();
            PrepareModel(model);
            return View(model);
        }

        public ActionResult GetTreeView()
        {
            var model = new ItemTypeViewModel();
            PrepareModel(model);

            return View("_TreeView", model);
        }

        public ActionResult Create(int id)
        {
            var model = new ItemTypeViewModel();
            PrepareModel(model);

            var itemType = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().ToList();

            model.ParentId = id;

            model.Mode = "Create";
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(ItemTypeViewModel model)
        {
            var strMessage = string.Empty;
            if (ModelState.IsValid)
            {
                generateItemTypeCode(model);

                strMessage = CheckDuplicateEntry(model, model.Id);
                if (string.IsNullOrWhiteSpace(strMessage))
                {
                    try
                    {
                        var entity = model.ToEntity();

                        _invCommonservice.INVUnit.ItemTypeRepository.Add(entity);
                        _invCommonservice.INVUnit.ItemTypeRepository.SaveChanges();
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
            }
            return new JsonResult()
            {
                Data = strMessage
            };
        }

        private void generateItemTypeCode(ItemTypeViewModel model)
        {

            if (model.ParentId == null)
            {
                model.Code = "1";
                model.CodeCount = 1;
                model.Level = 1;
            }
            if (model.ParentId != null)
            {
                var _objCode = _invCommonservice.INVUnit.ItemTypeRepository.Get(q => q.ParentId == model.ParentId).OrderByDescending(p => p.CodeCount).FirstOrDefault();
                var objLevel = _invCommonservice.INVUnit.ItemTypeRepository.Get(q => q.Id == model.ParentId).OrderByDescending(p => p.CodeCount).FirstOrDefault();
                int AccCount = 1;
                if (objLevel != null)
                {
                    model.Level = objLevel.Level + 1;
                }
                if (_objCode != null)
                {
                    AccCount = _objCode.CodeCount + 1;
                }

                model.CodeCount = AccCount;
                model.Code = objLevel.Code + "." + AccCount;
            }
        }

        public ActionResult Edit(int id)
        {
            var model = new ItemTypeViewModel();
            var itemTypes = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().ToList();
            var entity = itemTypes.Find(x => x.Id == id);

            if (entity.ParentId == null)
            {
                return Json("Requested control head is not editable.", JsonRequestBehavior.AllowGet);
            }

            model = entity.ToModel();
            PrepareModel(model);

            model.Mode = "Edit";
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit(ItemTypeViewModel model)
        {
            var strMessage = String.Empty;

            var listItemTypes = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().ToList();
            var childItems = new List<INV_ItemType>();
            FindChildItems(model.Id, listItemTypes, childItems);

            if (ModelState.IsValid)
            {
                generateItemTypeCode(model);

                strMessage = CheckDuplicateEntry(model, model.Id);
                if (string.IsNullOrWhiteSpace(strMessage))
                {
                    try
                    {
                        //var _obj = _invCommonservice.INVUnit.ItemTypeRepository.GetByID(model.Id);//added by suman

                        var entity = model.ToEntity();
                        entity.EUser = User.Identity.Name;
                        entity.EDate = Common.CurrentDateTime;

                        //if (_obj != null) //added by suman
                        //{
                        //    entity.Code = _obj.Code;
                        //    entity.CodeCount = _obj.CodeCount;
                        //    entity.Level = _obj.Level;
                        //}
                        _invCommonservice.INVUnit.ItemTypeRepository.Update(entity);
                        _invCommonservice.INVUnit.ItemTypeRepository.SaveChanges();
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
            var itemTypes = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().ToList();
            var childItems = new List<INV_ItemType>();
            FindChildItems(id, itemTypes, childItems);
            if (childItems.Count == 0)
            {
                try
                {
                    var item = itemTypes.Find(x => x.Id == id);

                    if (item.ParentId != null)
                    {
                        _invCommonservice.INVUnit.ItemTypeRepository.Delete(item);
                        _invCommonservice.INVUnit.ItemTypeRepository.SaveChanges();

                        return Json
                        (
                            ErrorMessages.DeleteSuccessful,
                            JsonRequestBehavior.AllowGet
                        );
                    }
                    else
                    {
                        return Json
                        (
                            "Error: Requested control head is not deletable.",
                            JsonRequestBehavior.AllowGet
                        );

                    }
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

            //var nodes = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().Where(q => q.ZoneInfoId == 0 || q.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            var nodes = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().ToList();
            var parentNode = nodes.Where(x => x.ParentId == null).FirstOrDefault();

            JsTreeNode rootNode = new JsTreeNode();

            if (parentNode != null)
            {
                rootNode.attr = new Attributes();
                rootNode.attr.id = Convert.ToString(parentNode.Id);
                rootNode.attr.rel = "root" + Convert.ToString(parentNode.Id);
                rootNode.data = new Data();

                StringBuilder itemTypeName = GenerateNodeText(parentNode);
                rootNode.data.title = Convert.ToString(itemTypeName);
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
            var nodes = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().ToList();
            var parentNode = nodes.Where(x => x.ParentId == null).FirstOrDefault();

            JsTreeNode rootNode = new JsTreeNode();

            if (parentNode != null)
            {
                rootNode.attr = new Attributes();
                rootNode.attr.id = Convert.ToString(parentNode.Id);
                rootNode.attr.rel = "root" + Convert.ToString(parentNode.Id);
                rootNode.data = new Data();

                StringBuilder itemTypeName = GenerateNodeText(parentNode);
                rootNode.data.title = Convert.ToString(itemTypeName);
                rootNode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";
                PopulateTree(parentNode, rootNode, nodes);
            }

            return new JsonResult()
            {
                Data = rootNode,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        private static StringBuilder GenerateNodeText(INV_ItemType parentNode)
        {
            StringBuilder itemTypeName = new StringBuilder();
            itemTypeName.Append(parentNode.ItemTypeName);

            return itemTypeName;
        }

        public PartialViewResult TreeViewSearchList(string ItemTypeIdentityName)
        {
            ItemTypeViewModel model = new ItemTypeViewModel();
            model.ItemTypeIdentityName = ItemTypeIdentityName;

            return PartialView("_TreeViewSearch", model);
        }
        #endregion

        #region Utils

        public void PopulateTree(INV_ItemType parentNode, JsTreeNode jsTNode, List<INV_ItemType> nodes)
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

        private ItemTypeViewModel PrepareModel(ItemTypeViewModel model)
        {
            PopulateCombo(model);
            return model;
        }

        private void PopulateCombo(ItemTypeViewModel model)
        {
            model.ItemTypeGroupList = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().Where(s => s.IsGroup).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.ItemTypeName,
                    Value = y.Id.ToString()
                }).ToList();

            //To add Budget Head ddl here
            model.BudgetHeadList = _invCommonservice.INVUnit.BudgetHeadViewRepository.GetAll().ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.AccountName,
                    Value = y.Id.ToString()
                }).ToList();
        }

        public JsonResult GetParentHead(string accountType)
        {
            var aocs = _invCommonservice.INVUnit.ItemTypeRepository.GetAll()
                .Select(y =>
                new
                {
                    text = y.ItemTypeName,
                    value = y.Id
                });

            return Json(aocs, JsonRequestBehavior.AllowGet);
        }

        private void FindChildItems(int id, List<INV_ItemType> listItemTypes, List<INV_ItemType> childItems)
        {
            foreach (var item in listItemTypes)
            {
                if (item.ParentId == id)
                {
                    childItems.Add(item);
                    FindChildItems(item.Id, listItemTypes, childItems);
                }
            }
        }

        private string CheckDuplicateEntry(ItemTypeViewModel model, int id)
        {
            string message = string.Empty;
            var itemTypes = new List<INV_ItemType>();
            var itemTypeInfo = new INV_ItemType();

            if (model.Mode == "Create")
            {
                itemTypes = (from x in _invCommonservice.INVUnit.ItemTypeRepository.GetAll()
                             where x.ParentId == model.ParentId && x.ItemTypeName == model.ItemTypeName
                             select x).DefaultIfEmpty().OfType<INV_ItemType>().ToList();

                itemTypeInfo = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().Where(x => !string.IsNullOrWhiteSpace(model.Code) && x.Code == model.Code).FirstOrDefault();
            }
            else
            {
                itemTypes = (from x in _invCommonservice.INVUnit.ItemTypeRepository.GetAll()
                             where x.ParentId == model.ParentId && x.ItemTypeName == model.ItemTypeName && x.Id != id
                             select x).DefaultIfEmpty().OfType<INV_ItemType>().ToList();

                itemTypeInfo = _invCommonservice.INVUnit.ItemTypeRepository.GetAll().Where(x => !string.IsNullOrWhiteSpace(model.Code) && x.Code == model.Code && x.Id != id).FirstOrDefault();

                if(model.IsGroup  == false)
                {   
                    var child = _invCommonservice.INVUnit.ItemTypeRepository.Get(x => x.ParentId == id).FirstOrDefault();
                    if (child != null)
                    {
                        message += "Child exists.Is group must be checked.";
                    }
                }
            }

            if (itemTypes.Any())
            {
                message += "Item Type: " + model.ItemTypeName + " already exists.";
            }


            if (itemTypeInfo != null)
            {
                message += "Code is already assigned for Item Type: " + itemTypeInfo.ItemTypeName + ".";
            }

            return message;
        }
        #endregion
    }
}