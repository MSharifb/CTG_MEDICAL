using System;
using System.Linq;
using System.Web.Mvc;
using BEPZA_MEDICAL.Domain.PRM;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Resources;
using BEPZA_MEDICAL.DAL.PRM;
using System.Collections.Generic;
using Lib.Web.Mvc.JQuery.JqGrid;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using System.Data.SqlClient;
using BEPZA_MEDICAL.Web.Controllers;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ApprovalFlowController : BaseController
    {

        #region Fields
        private readonly PRMCommonSevice _prmCommonservice;
        private readonly EmployeeService _empService;
        private static int selectedZoneId;

        #endregion

        public ApprovalFlowController(PRMCommonSevice prmCommonService, EmployeeService empService)
        {
            this._prmCommonservice = prmCommonService;
            this._empService = empService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ApprovalFlowViewModel model = new ApprovalFlowViewModel();
            PopulateDropDown(model);
            PopulateDetailDropDown(model);
            model.ActionType = "Save";
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Save(ApprovalFlowViewModel model)
        {
            SaveUpdate(model);
            if (model.IsError == 1)
            {
                return View("CreateOrEdit", model);
            }
            return View("Index", model);
        }

        public ActionResult Edit(int id)
        {
            var model = new ApprovalFlowViewModel();
            try
            {
                var obj = _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.GetByID(id);
                var approvalSteps = _prmCommonservice.PRMUnit.ApprovalStepRepository.GetAll();
                if (obj != null)
                {
                    model = obj.ToModel();
                    PopulateDropDown(model);
                    PopulateDetailDropDown(model);
                    var detailList = _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.Get(q => q.ApprovalFlowMasterId == obj.Id).ToList();
                    if (detailList != null && detailList.Count() > 0)
                    {
                        foreach (var item in detailList)
                        {
                            var detailModel = item.ToModel();
                            detailModel.StepTypeName = approvalSteps.FirstOrDefault(t => t.Id == detailModel.ApprovalStepTypeId).StepName;
                            model.ApprovalFlowDetail.DetailList.Add(detailModel);
                        }
                    }
                    model.ActionType = "Update";
                }
            }
            catch (Exception exception)
            {
                model.errClass = "failed";
                model.IsSuccessful = false;
                if (exception.InnerException != null && exception.InnerException.Message.Contains("duplicate"))
                {
                    model.Message = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.Message = ErrorMessages.DataNotFound;
                }
            }
            return View("CreateOrEdit", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, ApprovalFlowViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from data in _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.GetAll()
                        select new ApprovalFlowViewModel()
                        {
                            Id = data.Id,
                            ApprovalFlowName = data.ApprovalFlowName,
                            ApprovalProcessName = data.APV_ApprovalProcess.ApprovalProcessName,
                            ApproverGroupName = data.APV_ApprovalGroup.ApprovalGroupName,
                            ApprovalGroupId = data.ApprovalGroupId,
                            ApprovalProcesssId = data.ApprovalProcesssId,
                        }).OrderBy(x => x.ApproverGroupName).ThenBy(q => q.ApprovalFlowName).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(viewModel.ApprovalFlowName))
                {
                    list = list.Where(x => x.ApprovalFlowName.Trim().ToLower().Contains(viewModel.ApprovalFlowName.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(viewModel.ApproverGroupName))
                {
                    if (viewModel.ApproverGroupName != "0")
                    {
                        list = list.Where(x => x.ApprovalGroupId.ToString() == viewModel.ApproverGroupName).ToList();
                    }
                }
                if (!string.IsNullOrWhiteSpace(viewModel.ApprovalProcessName))
                {
                    if (viewModel.ApprovalProcessName != "0")
                    {
                        list = list.Where(x => x.ApprovalProcesssId.ToString() == viewModel.ApprovalProcessName).ToList();
                    }
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "ApproverGroupName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ApproverGroupName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ApproverGroupName).ToList();
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
                    d.ApprovalFlowName,
                    d.ApprovalProcessName,
                    d.ApproverGroupName,
                    "SetApprover",
                    "Delete",
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }


        [HttpPost]
        public ActionResult Update(ApprovalFlowViewModel model)
        {
            SaveUpdate(model);
            if (model.IsError == 1)
            {
                return View("CreateOrEdit", model);
            }
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {

                _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.Delete(id);
                _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.SaveChanges();
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
            });
        }

        [HttpPost]
        public ActionResult AddNewStep(ApprovalFlowDetailViewModel model, string stepName, string stepTypeId, string stepSequence, string notificationMessage)
        {
            var stepTypeList = _prmCommonservice.PRMUnit.ApprovalStepRepository.GetAll();
            int stepId = 0;
            int.TryParse(stepTypeId, out stepId);

            model.StepName = stepSequence;
            model.ApprovalStepTypeId = stepId;
            model.StepTypeName = stepTypeList.FirstOrDefault(t => t.Id == stepId).StepName;
            model.StepSequence = stepName;
            model.NotificationMessage = notificationMessage;

            return PartialView("_ApprovalFlowItem", model);
        }

        #region Methods



        private ApprovalFlowViewModel SaveUpdate(ApprovalFlowViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var errorMessage = CheckBusinessValidation(model);
                    var detailList = new List<APV_ApprovalFlowDetail>();
                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        var existingMasterObj = _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.GetByID(model.Id);
                        var obj = new APV_ApprovalFlowMaster
                        {
                            ZoneId = model.ZoneId,
                            ApprovalGroupId = model.ApprovalGroupId,
                            ApprovalFlowName = model.ApprovalFlowName,
                            ApprovalProcesssId = model.ApprovalProcesssId,
                            IUser = existingMasterObj != null ? existingMasterObj.IUser : model.IUser,
                            IDate = existingMasterObj != null ? existingMasterObj.IDate : (DateTime)model.IDate,
                            Id = model.ActionType == "Update" ? model.Id : 0,
                        };

                        switch (model.ActionType)
                        {
                            case "Save":
                                foreach (var item in model.ApprovalFlowDetailList)
                                {
                                    var anDetail = item.ToEntity();
                                    anDetail.ApprovalFlowMasterId = obj.Id;
                                    obj.APV_ApprovalFlowDetail.Add(anDetail);
                                }
                                _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.Add(obj);
                                _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.SaveChanges();
                                break;
                            case "Update":
                                obj.EUser = User.Identity.Name;
                                obj.EDate = DateTime.Now;
                                _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.Update(obj);
                                detailList = _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.Get(q => q.ApprovalFlowMasterId == obj.Id).ToList();

                                var deletedList = (from e in detailList
                                                   where !(model.ApprovalFlowDetailList.Any(t => t.Id == e.Id))
                                                   select e).DefaultIfEmpty().OfType<APV_ApprovalFlowDetail>().ToList();
                                if (deletedList != null && deletedList.Count > 0)
                                {
                                    foreach (var item in deletedList)
                                    {
                                        _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.Delete(item);
                                    }
                                }

                                foreach (var item in model.ApprovalFlowDetailList)
                                {
                                    var anDetailObj = item.ToEntity();
                                    anDetailObj.ApprovalFlowMasterId = obj.Id;

                                    if (anDetailObj.Id > 0)
                                    {
                                        anDetailObj.EUser = User.Identity.Name;
                                        anDetailObj.EDate = DateTime.Now;
                                        _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.Update(anDetailObj);
                                    }
                                    else
                                    {
                                        _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.Add(anDetailObj);
                                    }
                                }
                                _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.SaveChanges();
                                _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.SaveChanges();

                                break;
                        }
                        model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model.IsError = 0;
                        model.errClass = "success";
                    }
                    else
                    {
                        model.ErrMsg = errorMessage;
                        model.IsError = 1;
                        model.errClass = "failed";
                    }
                }
                else
                {
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                    model.IsError = 1;
                    model.errClass = "failed";
                }

            }
            catch (Exception exception)
            {
                model.errClass = "failed";
                model.IsSuccessful = false;
                model.IsError = 1;
                if (exception.InnerException != null && exception.InnerException.Message.Contains("duplicate"))
                {
                    model.ErrMsg = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.ErrMsg = ErrorMessages.InsertFailed;
                }
            }
            return model;
        }

        private string CheckBusinessValidation(ApprovalFlowViewModel model)
        {
            string message = string.Empty;
            try
            {
                var list = _prmCommonservice.PRMUnit.ApprovalGroupRepository.GetAll();
                switch (model.ActionType)
                {
                    case "Save":
                        //list = list.Where(t => t.ApprovalGroupName == model.ApprovalGroupName).ToList();
                        break;
                    case "Update":
                        //list = list.Where(t => t.ApprovalGroupName == model.ApprovalGroupName && t.Id != model.Id).ToList();
                        break;
                }

                if (list != null && list.Count() > 0)
                {
                    //message += "Approval Group " + model.ApprovalGroupName + " already exists.";
                }
                return message;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private ApprovalFlowViewModel PopulateDropDown(ApprovalFlowViewModel model)
        {
            var zoneList = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll().OrderBy(x=>x.SortOrder);
            model.ZoneList = Common.PopulateDdlZoneListWithAllOption(zoneList);
            var groupList = _prmCommonservice.PRMUnit.ApprovalGroupRepository.GetAll();
            model.ApprovalGroupList = Common.PopulateDdlApproverGroup(groupList);
            var approvalProcessList = _prmCommonservice.PRMUnit.ApprovalProcessRepository.GetAll();
            model.ApprovalProcessList = Common.PopulateDdlApprovalProcess(approvalProcessList);
            return model;
        }

        private ApprovalFlowViewModel PopulateDetailDropDown(ApprovalFlowViewModel model)
        {
            var stepTypeList = _prmCommonservice.PRMUnit.ApprovalStepRepository.GetAll();
            model.ApprovalFlowDetail.StepTypeList = Common.PopulateDdlApprovalStep(stepTypeList);
            var stepSequenceList = _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.Get(q => q.ApprovalFlowMasterId == model.Id).ToList();
            model.ApprovalFlowDetail.StepSequenceList = Common.PopulateDdlApprovalStepSequence(stepSequenceList);
            return model;
        }

        #endregion

        [HttpPost]
        public ActionResult ShowApproverSelector(string authorType)
        {
            var model = new ApproverInformationViewModel();
            string viewName = string.Empty;
            switch (authorType)
            {
                case "Specific Employee":
                    viewName = @"_SepcificEmployee";
                    break;
                case "Organogram Post":
                    viewName = @"_OrganogramLevel";
                    var zoneList = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll();
                    model.ZoneList = Common.PopulateDdlZoneList(zoneList);
                    model.EmployeeList = new List<SelectListItem>();
                    break;
            }
            return PartialView(viewName, model);
        }


        private ApproverInformationViewModel SetApprovarInformation(int id)
        {
            ApproverInformationViewModel model = new ApproverInformationViewModel();
            var flowMaster = _prmCommonservice.PRMUnit.ApprovalFlowMasterRepository.GetByID(id);
            var flowDetails = new List<APV_ApprovalFlowDetail>();
            if (flowMaster != null)
            {
                flowDetails = _prmCommonservice.PRMUnit.ApprovalFlowDetailRepository.Get(t => t.ApprovalFlowMasterId == flowMaster.Id).DefaultIfEmpty().OfType<APV_ApprovalFlowDetail>().ToList();
            }
            model.ApprovalFlowName = flowMaster.ApprovalFlowName;
            model.ApprovalProcessName = flowMaster.APV_ApprovalProcess.ApprovalProcessName;
            model.ApprovalStepList = Common.PopulateDdlApprovalStepSequenceByApprovalFlow(flowDetails);
            var approverTypeList = _prmCommonservice.PRMUnit.ApproverTypeRepository.GetAll();
            model.ApproverTypeList = Common.PopulateDdlApproverType(approverTypeList);
            var authorTypeList = _prmCommonservice.PRMUnit.ApproverAuthorTypeRepository.GetAll();
            model.AuthorTypeList = Common.PopulateDdlApproverAuthorType(authorTypeList);
            model.ActionType = "SaveApprover";
            return model;
        }

        public ActionResult SetApprover(int id)
        {
            var model = SetApprovarInformation(id);
            return View("_SetApprover", model);
        }



        public ActionResult EmployeeSearch(string UseTypeEmpId, string zoneId)
        {
            int zoneInfoId = 0;
            if (!string.IsNullOrWhiteSpace(zoneId))
            {
                int.TryParse(zoneId, out zoneInfoId);
            }
            else
            {
                zoneInfoId = LoggedUserZoneInfoId;
            }

            var model = new EmployeeSearchViewModel();
            model.SearchEmpType = "active";
            model.UseTypeEmpId = UseTypeEmpId;
            model.ZoneInfoId = zoneInfoId;
            selectedZoneId = zoneInfoId;
            return View(model);
        }

        #region Search Employee

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEmpList(JqGridRequest request, EmployeeSearchViewModel viewModel, string st, string zoneId)
        {
            if (zoneId == "" || zoneId == "0")
            {
                zoneId = selectedZoneId.ToString();
            }

            int zoneInfoId = 0;
            int.TryParse(zoneId, out zoneInfoId);

            string filterExpression = String.Empty, LoginEmpId = "";
            int totalRecords = 0;
            if (string.IsNullOrEmpty(st))
                st = "";
            if (viewModel.ZoneInfoId == 0 && request.Searching == false)
            {
                viewModel.ZoneInfoId = zoneInfoId == 0 ? LoggedUserZoneInfoId : zoneInfoId;
            }



            var list = _empService.GetPaged(
                filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,

                viewModel.EmpId,
                viewModel.EmpName,
                viewModel.DesigName,
                viewModel.EmpTypeId,
                viewModel.DivisionName,
                viewModel.JobLocName,
                viewModel.GradeName,
                viewModel.StaffCategoryId,
                //viewModel.ResourceLevelId,
                viewModel.OrganogramLevelId,
                viewModel.ZoneInfoId,
                st.Equals("active") ? 1 : viewModel.EmployeeStatus,

                out totalRecords

                //LoginEmpId
                );

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.ID), new List<object>()
                {
                    item.ZoneInfoId,
                    item.EmpName,
                    item.ID,
                    item.EmpId,
                    item.DesigName,
                    item.DivisionName,
                    item.JobLocName,
                    item.GradeName,
                    item.DateOfJoining.ToString(DateAndTime.GlobalDateFormat),
                    item.DateOfConfirmation.HasValue ? item.DateOfConfirmation.Value.ToString(DateAndTime.GlobalDateFormat) : null,

                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        #endregion

        #region Grid Dropdown list

        [NoCache]
        public ActionResult GetZoneInfo()
        {
            var zoneList = _prmCommonservice.PRMUnit.ZoneInfoRepository.GetAll();
            return PartialView("Select", Common.PopulateDdlZoneList(zoneList));
        }


        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _empService.PRMUnit.DesignationRepository.GetAll().OrderBy(x => x.SortingOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(designations));
        }

        [NoCache]
        public ActionResult GetDivision()
        {
            var divisions = _empService.PRMUnit.DivisionRepository.GetAll().Where(q => q.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDllList(divisions));
        }

        [NoCache]
        public ActionResult GetJobLocation()
        {
            var jobLocations = _empService.PRMUnit.JobLocationRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(jobLocations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            //var grades = _empService.PRMUnit.JobGradeRepository.GetAll().OrderBy(x => x.Id).ToList();
            //return PartialView("Select", Common.PopulateJobGradeDDL(grades));
            return PartialView("Select", Common.PopulateCurrentJobGradeDDL(_empService));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _empService.PRMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _empService.PRMUnit.PRM_StaffCategoryRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {
            var empStatus = _empService.PRMUnit.EmploymentStatusRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDllList(empStatus));
        }

        #endregion

        [HttpPost]
        public JsonResult GetEmployeeInfo(string employeeId)
        {
            int empId = 0;
            int.TryParse(employeeId, out empId);
            var obj = _empService.PRMUnit.EmploymentInfoRepository.GetByID(empId);
            return Json(new
            {
                EmpID = obj.EmpID,
                EmployeeName = obj.FullName,
                Designation = obj.PRM_Designation.Name,
                Phone = obj.TelephoneOffice,
                Email = obj.EmialAddress
            });

        }

        public PartialViewResult TreeViewSearchList()
        {
            //string OrgIdentityName
            //OrganogramLevelViewModel model = new OrganogramLevelViewModel();
            //model.OrgIdentityName = OrgIdentityName;

            return PartialView("_TreeViewSearch");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetTreeData(string zoneId)
        {
            int selectedZoneId = 0;
            int.TryParse(zoneId, out selectedZoneId);

            var nodes = new List<JsTreeModel>();

            IEnumerable<PRM_OrganogramLevel> parentNodes = null;
            if (selectedZoneId > 0)
                parentNodes = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll().Where(q => q.ZoneInfoId == null || q.ZoneInfoId == selectedZoneId).ToList();
            else
                parentNodes = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll().ToList();

            PRM_OrganogramLevel parentNode = parentNodes.FirstOrDefault(x => x.ParentId == 0);

            nodes.Add(new JsTreeModel() { id = parentNode.Id.ToString(), parent = "#", text = parentNode.LevelName });

            IEnumerable<PRM_OrganogramLevel> childs = null;
            if (selectedZoneId > 0)
                childs = _prmCommonservice.PRMUnit.OrganogramLevelRepository.Get(q => q.ParentId == parentNode.Id && q.ZoneInfoId == selectedZoneId).ToList();
            else
                childs = _prmCommonservice.PRMUnit.OrganogramLevelRepository.Get(q => q.ParentId == parentNode.Id).ToList();

            foreach (var item in childs)
            {
                nodes.Add(new JsTreeModel() { id = item.Id.ToString(), parent = item.ParentId.ToString(), text = GenerateNodeText(item).ToString() });
                AddChild(nodes, item);
            }

            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        private void AddChild(List<JsTreeModel> nodes, PRM_OrganogramLevel item)
        {

            var childs = _prmCommonservice.PRMUnit.OrganogramLevelRepository.Get(t => t.ParentId == item.Id).DefaultIfEmpty().OfType<PRM_OrganogramLevel>().ToList();

            if (childs != null && childs.Count > 0)
            {
                foreach (var anChild in childs)
                {
                    nodes.Add(new JsTreeModel() { id = anChild.Id.ToString(), parent = anChild.ParentId.ToString(), text = GenerateNodeText(anChild).ToString() });
                    AddChild(nodes, anChild);
                }
            }
        }
        /*
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
        */
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

        public ActionResult GetEmployeeInfoByOrganogramLevel(string levelId)
        {
            int organogramLevelId = 0;
            int.TryParse(levelId, out organogramLevelId);
            var employeeList = new List<PRM_EmploymentInfo>();
            var finalList = new List<EmploymentInfoViewModel>();
            var dataList = _prmCommonservice.PRMUnit.DivisionHeadMapingRepository.Get(q => q.OrganogramLevelId == organogramLevelId).DefaultIfEmpty().OfType<PRM_DivisionHeadMaping>().ToList();
            if (dataList != null && dataList.Count > 0)
            {
                int? employeeId = 0;
                employeeId = dataList.Select(t => t.EmployeeId).Take(1).FirstOrDefault();
                if (employeeId > 0)
                {
                    employeeList = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(q => q.Id == employeeId).DefaultIfEmpty().OfType<PRM_EmploymentInfo>().ToList();
                }
                else
                {
                    employeeList = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(q => q.OrganogramLevelId == organogramLevelId).DefaultIfEmpty().OfType<PRM_EmploymentInfo>().ToList();
                }
            }
            if (employeeList != null && employeeList.Count > 0)
            {
                finalList = (from x in employeeList
                             select new EmploymentInfoViewModel()
                             {
                                 Id = x.Id,
                                 FullName = x.FullName,
                                 DesignationName = x.PRM_Designation.Name
                             }).ToList();
            }
            return Json(finalList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLoggedZoneId()
        {
            int loggedUserZoneId = LoggedUserZoneInfoId;
            return Json(loggedUserZoneId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewApprover(ApproverInformationViewModel model, string appFlowId, string approverTypeId, string authorTypeName, string approverId, string levelId, string minAmount, string maxAmount)
        {
            int appFlowIdInt = 0, approverTypeIdInt = 0, authorTypeIdInt = 0, approverIdInt = 0;
            int.TryParse(appFlowId, out appFlowIdInt);
            int.TryParse(approverTypeId, out approverTypeIdInt);
            var approverTypeList = _prmCommonservice.PRMUnit.ApproverTypeRepository.GetAll();
            var empInfo = new PRM_EmploymentInfo();
            string organizationLevel = string.Empty;
            string designationName = string.Empty;
            int levelIdInt = 0;
            int.TryParse(levelId, out levelIdInt);

            decimal minAmountDec = 0, maxAmountDec = 0;
            decimal.TryParse(minAmount, out minAmountDec);
            decimal.TryParse(maxAmount, out maxAmountDec);



            var organogramLevelList = _prmCommonservice.PRMUnit.OrganogramLevelRepository.GetAll();
            var designationList = _prmCommonservice.PRMUnit.DesignationRepository.GetAll();

            authorTypeIdInt = _prmCommonservice.PRMUnit.ApproverAuthorTypeRepository.Get(t => t.AuthorType == authorTypeName).FirstOrDefault().Id;
            switch (authorTypeName)
            {
                case "Specific Employee":
                    empInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(q => q.EmpID == approverId).FirstOrDefault();
                    model.HeadOfLevelType = "N/A";
                    model.OrganogramLevelAndPost = "N/A";
                    break;
                case "Head of Level":
                    int.TryParse(approverId, out approverIdInt);
                    empInfo = null;
                    model.HeadOfLevelType = "YES";
                    organizationLevel = string.Empty;
                    designationName = string.Empty;
                    model.OrganogramLevelAndPost = string.Empty;
                    model.LevelId = null;
                    break;
                case "Organogram Post":
                    int.TryParse(approverId, out approverIdInt);
                    string levelName = organogramLevelList.Where(t => t.Id == levelIdInt).FirstOrDefault().LevelName;
                    string orgDesigName = designationList.Where(t => t.Id == approverIdInt).FirstOrDefault().Name;
                    empInfo = null;
                    model.HeadOfLevelType = "N/A";
                    organizationLevel = levelName;
                    designationName = orgDesigName;
                    model.OrganogramLevelAndPost = levelName + " (" + orgDesigName + ")";
                    model.LevelId = levelIdInt;
                    break;
            }

            if (empInfo != null)
            {
                model.EmployeeName = empInfo.FullName + " (" + empInfo.EmpID + ")";
                model.EmployeeId = empInfo.Id;
                model.EmpId = empInfo.EmpID;
            }

            model.ApproverTypeName = approverTypeList.FirstOrDefault(t => t.Id == approverTypeIdInt).ApproverType;
            model.ApproverTypeId = approverTypeIdInt;
            model.ApprovalFlowDetailId = appFlowIdInt;
            model.AuthorTypeId = authorTypeIdInt;
            //model.ApproverInfoDetailList.Add(detailInfo);
            model.MinAmount = minAmountDec;
            model.MaxAmount = maxAmountDec;

            var detailInfo = new ApproverInformationDetailsViewModel()
            {
                EmployeeId = model.EmployeeId,
                HeadOfLevelType = model.HeadOfLevelType,
                EmployeeName = model.EmployeeName,
                ApproverTypeId = model.ApproverTypeId,
                ApprovalFlowName = model.ApprovalFlowName,
                ApproverId = model.ApproverId,
                ApproverTypeName = model.ApproverTypeName,
                ApprovalProcessName = model.ApprovalProcessName,
                Designation = model.Designation,
                AuthorTypeId = model.AuthorTypeId,
                ApprovalFlowDetailId = model.ApprovalFlowDetailId,
                OrganogramLevelAndPost = model.OrganogramLevelAndPost,
                LevelId = model.LevelId,
                EmpId = model.EmpId,
                SelectedLevelId = levelId,
                MaxAmount = model.MaxAmount,
                MinAmount = model.MinAmount                
            };
            model.DetailList.Add(detailInfo);

            return PartialView("_ApproverDetails", model.DetailList);
        }

        [HttpPost]
        public ActionResult SaveApprover(ApproverInformationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var approverList = new List<ApproverInformationDetailsViewModel>();
                    if (model.DetailList != null)
                    {
                        approverList = model.DetailList.DefaultIfEmpty().OfType<ApproverInformationDetailsViewModel>().ToList();
                    }

                    var authorTypeList = _prmCommonservice.PRMUnit.ApproverAuthorTypeRepository.GetAll();
                    var savedRecords = _prmCommonservice.PRMUnit.ApproverInfoRepository.Get(t => t.ApprovalFlowDetailId == model.ApprovalFlowDetailId).ToList();

                    var deletedList = new List<APV_ApproverInfo>();
                    if (approverList != null && approverList.Count > 0)
                    {
                        deletedList = (from e in savedRecords
                                       where !(model.DetailList.Any(t => t.Id == e.Id))
                                       select e).DefaultIfEmpty().OfType<APV_ApproverInfo>().ToList();

                    }
                    else
                    {
                        deletedList = savedRecords;
                    }

                    if (deletedList != null && deletedList.Count > 0)
                    {
                        foreach (var item in deletedList)
                        {
                            _prmCommonservice.PRMUnit.ApproverInfoRepository.Delete(item);
                        }
                    }
                    int selectedLevelIdInt = 0;
                    foreach (var item in approverList)
                    {
                        var obj = item.ToEntity();

                        string authorTypeName = authorTypeList.FirstOrDefault(t => t.Id == obj.AuthorTypeId).AuthorType;
                        int.TryParse(item.SelectedLevelId, out selectedLevelIdInt);


                        switch (authorTypeName)
                        {
                            case "Specific Employee":
                                var employeeInfo = _prmCommonservice.PRMUnit.EmploymentInfoRepository.Get(t => t.EmpID == item.EmpId).FirstOrDefault();
                                if (employeeInfo != null)
                                {
                                    obj.ApproverId = employeeInfo.Id;
                                }
                                break;
                        }

                        if (obj.Id > 0)
                        {
                            _prmCommonservice.PRMUnit.ApproverInfoRepository.Update(obj);
                        }
                        else
                        {
                            _prmCommonservice.PRMUnit.ApproverInfoRepository.Add(obj);
                        }
                    }
                    _prmCommonservice.PRMUnit.ApproverInfoRepository.SaveChanges();
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    model.IsError = 0;
                    model.errClass = "success";

                }
                catch (Exception ex)
                {
                    var mainModel = SetApprovarInformation(model.Id);
                    model.ApproverTypeList = mainModel.ApproverTypeList;
                    model.ApprovalStepList = mainModel.ApprovalStepList;
                    model.AuthorTypeList = mainModel.AuthorTypeList;
                    return View("_SetApprover", model);
                }
            }
            else
            {
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
                model.IsError = 1;
                model.errClass = "failed";
            }
            var anModel = SetApprovarInformation(model.Id);
            model.ApproverTypeList = anModel.ApproverTypeList;
            model.ApprovalStepList = anModel.ApprovalStepList;
            model.AuthorTypeList = anModel.AuthorTypeList;

            return View("_SetApprover", model);
        }

        public ActionResult GetApproverInfoByStepId(int stepId)
        {
            var model = new ApproverInformationViewModel();
            var detailList = _prmCommonservice.PRMUnit.ApproverInformationViewRepository.Get(t => t.ApprovalFlowDetailId == stepId).DefaultIfEmpty().OfType<vwAPVApproverInformation>().ToList();
            if (detailList != null && detailList.Count > 0)
            {
                foreach (var item in detailList)
                {
                    var detailModel = new ApproverInformationDetailsViewModel
                    {
                        ApprovalFlowDetailId = item.ApprovalFlowDetailId,
                        ApproverTypeId = item.ApproverTypeId,
                        AuthorTypeId = item.AuthorTypeId,
                        ApproverId = item.ApproverId,
                        LevelId = item.LevelId,
                        EmployeeId = item.EmployeeId,
                        EmployeeName = item.EmployeeName,
                        Designation = item.DesignationName,
                        HeadOfLevelType = item.HeadOfLevel,
                        OrganogramLevelAndPost = item.DesignationName,
                        ApproverTypeName = item.ApproverType,
                        Id = item.Id,
                        SelectedLevelId = item.LevelId.ToString(),
                    };

                    model.DetailList.Add(detailModel);
                }
            }
            return PartialView("_ApproverDetails", model.DetailList);
        }

        public ActionResult GetAuthorTypeByStepId(int stepId)
        {
            var detailList = _prmCommonservice.PRMUnit.ApproverInformationViewRepository.Get(t => t.ApprovalFlowDetailId == stepId).DefaultIfEmpty().OfType<vwAPVApproverInformation>().ToList();
            var authorTypeId = 0;
            if (detailList != null && detailList.Count > 0)
            {
                authorTypeId = detailList.Select(q => q.AuthorTypeId).Distinct().Take(1).FirstOrDefault();
            }
            return Json(authorTypeId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDesignationInfo(string levelId)
        {
            int organogramLevelId = 0;
            int.TryParse(levelId, out organogramLevelId);
            var desigList = (from JG in _empService.PRMUnit.OrganizationalSetupManpowerInfoRepository.Fetch()
                             join de in _empService.PRMUnit.DesignationRepository.Fetch() on JG.DesignationId equals de.Id
                             where JG.OrganogramLevelId == organogramLevelId
                             select de).OrderBy(o => o.Name).ToList();
            var finalList = (from x in desigList
                             select new PRM_Designation()
                             {
                                 Id = x.Id,
                                 Name = x.Name
                             }).ToList();
            return Json(finalList, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetProcessInfo()
        {
            var list = _prmCommonservice.PRMUnit.ApprovalProcessRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDdlApprovalProcess(list));
        }

        [NoCache]
        public ActionResult GetGroupInfo()
        {
            var list = _prmCommonservice.PRMUnit.ApprovalGroupRepository.GetAll().ToList();
            return PartialView("Select", Common.PopulateDdlApproverGroup(list));
        }

        public JsonResult GetNotificationMessageVariable(int approvalProcessId)
        {
            var processInfo = _prmCommonservice.PRMUnit.ApprovalProcessRepository.GetByID(approvalProcessId);
            var processEnum = string.Empty;
            if (processInfo != null)
            {
                processEnum = processInfo.ProcessNameEnum;
            }
            var variableList = _prmCommonservice.PRMUnit.MessageVariableRepository.Get(q => q.ProcessNameEnum == processEnum).DefaultIfEmpty().OfType<APV_MessageVariableInformation>().ToList();
            var variables = new List<string>();
            if (variableList != null && variableList.Count > 0)
            {
                variables.AddRange(variableList.Select(q => q.VariableName));
            }
            return Json(new
            {
                Variables = variables
            });
        }
    }

    public class JsTreeModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string state { get; set; }
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
        public string li_attr { get; set; }
        public string a_attr { get; set; }
    }
}