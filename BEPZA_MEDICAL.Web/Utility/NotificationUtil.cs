using ERP_BEPZA.DAL.PRM;
using ERP_BEPZA.Domain.PRM;
using ERP_BEPZA.Utility;
using ERP_BEPZA.Web.Areas.PRM.ViewModel.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ERP_BEPZA.Web.Utility
{

    public interface INotification
    {
        PRMCommonSevice _prmCommonService { get; set; }
        NotificationEnum.NotificationType _notificationType { get; set; }
        Int32? _employeeId { get; set; }
        String _fromPart { get; set; }
        String _toPart { get; set; }
        DateTime? _effectiveDate { get; set; }
        List<int> _sendtoSpecifiedZones { get; set; }
        String _redirectToLink { get; set; }
        String _customMessage { get; set; }

        bool SendNotification();
    }

    public class SendNotificationByFlowSetup : INotification
    {


        public PRMCommonSevice _prmCommonService { get; set; }
        public NotificationEnum.NotificationType _notificationType { get; set; }
        public int? _employeeId { get; set; }
        public string _fromPart { get; set; }
        public string _toPart { get; set; }
        public DateTime? _effectiveDate { get; set; }
        public List<int> _sendtoSpecifiedZones { get; set; }
        public string _redirectToLink { get; set; }
        public string _customMessage { get; set; }

        #region Constructor
        public SendNotificationByFlowSetup(PRMCommonSevice prmCommonService, NotificationEnum.NotificationType notificationType, int? employeeId, String fromPart, String toPart, DateTime? effectiveDate, List<int> sendtoSpecifiedZones, String redirectToLink, String customMessage = "")
        {
            _prmCommonService = prmCommonService;
            _notificationType = notificationType;
            _employeeId = employeeId;
            _fromPart = fromPart;
            _toPart = toPart;
            _effectiveDate = effectiveDate;
            _sendtoSpecifiedZones = sendtoSpecifiedZones;
            _redirectToLink = redirectToLink;
            _customMessage = customMessage;
        }
        #endregion

        #region SendNotification method with constructors


        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationType">Managing with "Enum".</param>
        /// <param name="employeeId">To whom action commited</param>
        /// <param name="fromPart">e.g. Employee Transger from DEPZ</param>
        /// <param name="toPart">e.g. Employee Transger from CEPZ</param>
        /// <param name="effectiveDate">When action will effect</param>
        /// <param name="sendtoSpecifiedZones">e.g. Employee transfered to CEPZ. So, no other zone will notify.</param>
        /// <param name="redirectToLink">Navigate to action ui</param>
        /// <param name="customMessage">Normally message will formate internally. But user can send any custom message</param>
        /// <returns></returns>
        public bool SendNotification()
        {
            Boolean sendSuccessfull = false;

            try
            {
                int notificationTypeId = Convert.ToInt32(_notificationType);

                NotificationFlowViewModel notificationFlowModel;
                NTF_NotificationFlow notificationFlowEntity;

                var notificationFlowSetups = _prmCommonService.PRMUnit.NotificationFlowSetupRepository.GetAll()
                                                        .Where(n => n.NotificationTypeId == notificationTypeId).ToList();


                if (!notificationFlowSetups.Any())
                {
                    // No flow setup found!
                    throw new Exception("No notification flow setup found!");
                }
                else
                {
                    if (String.IsNullOrEmpty(_redirectToLink))
                        _redirectToLink = String.Empty;

                    #region Get Employee Info - to whom action made
                    String employeeInfo = String.Empty;
                    if (_employeeId != null)
                    {
                        var employee = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                            .FirstOrDefault(n => n.Id == _employeeId);
                        if (employee != null)
                        {
                            String desigShortCode = String.Empty;

                            if (_prmCommonService.PRMUnit.DesignationRepository.GetAll()
                                .Any(d => d.Id == employee.DesignationId))
                            {
                                desigShortCode = _prmCommonService.PRMUnit.DesignationRepository.GetAll()
                                    .FirstOrDefault(d => d.Id == employee.DesignationId).ShortName;
                            }
                            String zoneCode = String.Empty;
                            if (_prmCommonService.PRMUnit.ZoneInfoRepository.GetAll()
                                .Any(z => z.Id == employee.ZoneInfoId))
                            {
                                zoneCode = _prmCommonService.PRMUnit.ZoneInfoRepository.GetAll()
                                    .FirstOrDefault(z => z.Id == employee.ZoneInfoId).ZoneCode;
                            }
                            employeeInfo = employee.EmpID + " - " + employee.FullName + " - " + desigShortCode + " - " +
                                           zoneCode;
                        }
                    }
                    #endregion

                    #region Save Notification
                    NotificationViewModel notificationModel = new NotificationViewModel();
                    notificationModel.NotificationType = notificationTypeId;
                    notificationModel.NotificationDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(_customMessage))
                    {
                        notificationModel.Message = _customMessage;
                    }
                    else
                    {
                        notificationModel.Message = GetMessage(_notificationType, employeeInfo, _fromPart, _toPart, _effectiveDate);
                    }
                    notificationModel.RedirectToLink = _redirectToLink;

                    var notificationEntity = notificationModel.ToEntity();
                    _prmCommonService.PRMUnit.NotificationRepository.Add(notificationEntity);

                    #endregion

                    #region Save Notification Flow according to Flow Setup

                    int organogramId;
                    List<PRM_EmploymentInfo> empsByOrganogram;
                    List<PRM_ZoneInfo> specificZoneList = null;

                    #region Get specific zone list to send notifications
                    if (_sendtoSpecifiedZones != null && _sendtoSpecifiedZones.Any())
                    {
                        specificZoneList = new List<PRM_ZoneInfo>();
                        foreach (int item in _sendtoSpecifiedZones)
                        {
                            if (item > 0)
                            {
                                specificZoneList.Add(_prmCommonService.PRMUnit.ZoneInfoRepository.GetAll().FirstOrDefault(z => z.Id == item));
                            }
                        }
                    }
                    #endregion

                    foreach (NTF_NotificationFlowSetup flowSetup in notificationFlowSetups)
                    {
                        if (!Common.GetBoolean(flowSetup.IsApplicableForGroup))
                        {
                            #region Notification flow for Individual
                            notificationFlowModel = new NotificationFlowViewModel();
                            notificationFlowModel.NotificationId = notificationEntity.Id;
                            notificationFlowModel.Module = flowSetup.ModuleId;
                            notificationFlowModel.EmployeeId = flowSetup.EmployeeId;

                            notificationFlowEntity = notificationFlowModel.ToEntity();
                            // Add flow
                            _prmCommonService.PRMUnit.NotificationFlowRepository.Add(notificationFlowEntity);
                            #endregion
                        }
                        else
                        {
                            if (!Common.GetBoolean(flowSetup.OnlyLevelHead))
                            {
                                organogramId = Common.GetInteger(flowSetup.OrganogramLevelId);

                                var lookup = _prmCommonService.PRMUnit.OrganogramLevelRepository.GetAll()
                                    .ToLookup(x => x.ParentId);
                                var organograms = lookup[organogramId].SelectRecursive(x => lookup[x.Id]).ToList();

                                organograms.Add(_prmCommonService.PRMUnit.OrganogramLevelRepository.GetAll().FirstOrDefault(o => o.Id == organogramId));

                                foreach (var organogram in organograms)
                                {
                                    #region Organogram wise notification flow
                                    empsByOrganogram = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                                                                            .Where(e => e.OrganogramLevelId == organogram.Id)
                                                                            .ToList();

                                    if (specificZoneList != null && specificZoneList.Any())
                                    {
                                        empsByOrganogram = empsByOrganogram.Where(x => specificZoneList.Select(n => n.Id).Contains((int)x.ZoneInfoId)).ToList();
                                    }

                                    if (Common.GetInteger(flowSetup.DesignationId) > 0)
                                    {
                                        empsByOrganogram =
                                            empsByOrganogram.Where(d => d.DesignationId == flowSetup.DesignationId)
                                                .ToList();
                                    }

                                    foreach (var emp in empsByOrganogram)
                                    {
                                        notificationFlowModel = new NotificationFlowViewModel();
                                        notificationFlowModel.NotificationId = notificationEntity.Id;
                                        notificationFlowModel.Module = flowSetup.ModuleId;

                                        notificationFlowModel.EmployeeId = emp.Id;
                                        notificationFlowEntity = notificationFlowModel.ToEntity();
                                        // Add flow
                                        _prmCommonService.PRMUnit.NotificationFlowRepository.Add(
                                            notificationFlowEntity);
                                    }


                                    #endregion
                                }
                            }
                            else // OnlyLevelHead?
                            {
                                #region Notification Flow for Level Heads
                                var levelHeads = _prmCommonService.PRMUnit.DivisionHeadMapingRepository.GetAll()
                                                           .Where(o => o.OrganogramLevelId == flowSetup.OrganogramLevelId).ToList();

                                if (!levelHeads.Any())
                                {
                                    // No organogram level head setup found!
                                    throw new Exception("No organogram level head setup found!");
                                }
                                else
                                {
                                    foreach (PRM_DivisionHeadMaping levelHead in levelHeads)
                                    {
                                        if (Common.GetInteger(levelHead.EmployeeId) > 0)
                                        {
                                            notificationFlowModel = new NotificationFlowViewModel();
                                            notificationFlowModel.NotificationId = notificationEntity.Id;
                                            notificationFlowModel.Module = flowSetup.ModuleId;

                                            notificationFlowModel.EmployeeId = levelHead.EmployeeId;
                                            notificationFlowEntity = notificationFlowModel.ToEntity();
                                            // Add flow
                                            _prmCommonService.PRMUnit.NotificationFlowRepository.Add(notificationFlowEntity);
                                        }
                                        else
                                        {
                                            var findLevelHeads = _prmCommonService.PRMUnit.EmploymentInfoRepository
                                                .GetAll()
                                                .Where(
                                                    e => e.OrganogramLevelId == levelHead.OrganogramLevelId &&
                                                         e.DesignationId == levelHead.DesignationId)
                                                .ToList();

                                            foreach (var head in findLevelHeads)
                                            {
                                                notificationFlowModel = new NotificationFlowViewModel();
                                                notificationFlowModel.NotificationId = notificationEntity.Id;
                                                notificationFlowModel.Module = flowSetup.ModuleId;

                                                notificationFlowModel.EmployeeId = head.Id;
                                                notificationFlowEntity = notificationFlowModel.ToEntity();
                                                // Add flow
                                                _prmCommonService.PRMUnit.NotificationFlowRepository.Add(notificationFlowEntity);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }

                        // Final Save Changes of Notification and NotificationFlow
                        _prmCommonService.PRMUnit.NotificationRepository.SaveChanges();
                        _prmCommonService.PRMUnit.NotificationFlowRepository.SaveChanges();
                    }
                    #endregion
                }

                sendSuccessfull = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Something wrong during sending notificaiton! " + ex.Message, ex.InnerException);
            }

            return sendSuccessfull;
        }

        #endregion

        private String GetMessage(NotificationEnum.NotificationType notificationType, String employeeInfoPart, String fromPart, String toPart, DateTime? effectiveDate)
        {
            String strMessage = string.Empty;

            switch (notificationType)
            {
                case NotificationEnum.NotificationType.Employee_Demotion:
                    {
                        strMessage = "";
                        break;
                    }
                case NotificationEnum.NotificationType.Employee_Promotion:
                    {
                        strMessage = "";
                        break;
                    }
                case NotificationEnum.NotificationType.Employee_Retirement:
                    {
                        strMessage = "";
                        break;
                    }
                case NotificationEnum.NotificationType.Employee_Suspend:
                    {
                        strMessage = "";
                        break;
                    }
                case NotificationEnum.NotificationType.Employee_Transfer:
                    {
                        strMessage = "An employee - " + employeeInfoPart + " has transfered " + fromPart + " " + toPart;
                        break;
                    }
                case NotificationEnum.NotificationType.PF_Statement_Generation:
                    {
                        strMessage = "";
                        break;
                    }
                case NotificationEnum.NotificationType.Salary_Increment:
                    {
                        strMessage = "";
                        break;
                    }
                default:
                    {
                        strMessage = "No notification message made!";
                        break;
                    }
            }

            return strMessage;
        }


    }

    public class SendGeneralPurposeNotification
    {
        private PRMCommonSevice _prmCommonService;

        private NotificationEnum.NotificationType _notificationType;
        private List<NotificationEnum.NotificationModule> _modules;
        private String _message;
        private String _redirectLink;
        private List<int> _toDefaultEmployees;

        /// <summary>
        /// Any user can use this function for general purpose 
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="message"></param>
        /// <param name="redirectLink"></param>
        /// <param name="prmCommonService">an instance of PRMCommonSevice class</param>
        /// <param name="toDefaultEmps">All active employees will be notified if you supply null here</param>
        /// <param name="notificationType">Default notification type is General_Purpose</param>
        public SendGeneralPurposeNotification(List<NotificationEnum.NotificationModule> modules
            , String message, String redirectLink, PRMCommonSevice prmCommonService, List<int> toDefaultEmps
            , NotificationEnum.NotificationType notificationType = NotificationEnum.NotificationType.General_Purpose)
        {

            this._notificationType = notificationType;
            this._modules = modules;
            this._message = message;
            this._redirectLink = redirectLink;
            this._prmCommonService = prmCommonService;
            this._toDefaultEmployees = toDefaultEmps;
        }


        public bool SendNotification()
        {
            Boolean sendSuccessfull = false;

            try
            {
                int notificationTypeId = Convert.ToInt32(_notificationType);

                NotificationViewModel notificationModel = new NotificationViewModel();
                notificationModel.NotificationType = notificationTypeId;
                notificationModel.NotificationDate = DateTime.Now;
                notificationModel.Message = _message;
                notificationModel.RedirectToLink = _redirectLink;

                var notificationEntity = notificationModel.ToEntity();
                _prmCommonService.PRMUnit.NotificationRepository.Add(notificationEntity);


                dynamic _toEmployees = (from e in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll()
                                        where e.DateofInactive == null
                                        select new { e.Id }).ToList();

                if (this._toDefaultEmployees != null && this._toDefaultEmployees.Any())
                {
                    _toEmployees = _toDefaultEmployees;
                }

                foreach (var id in _toEmployees)
                {
                    foreach (var notificationModule in _modules)
                    {
                        var notificationFlowModel = new NotificationFlowViewModel();
                        notificationFlowModel.NotificationId = notificationEntity.Id;
                        notificationFlowModel.Module = Convert.ToInt32(notificationModule);
                        notificationFlowModel.EmployeeId = Common.GetInteger(id);

                        var notificationFlowEntity = notificationFlowModel.ToEntity();
                        // Add flow
                        _prmCommonService.PRMUnit.NotificationFlowRepository.Add(notificationFlowEntity);
                    }
                }

                // Final Save Changes of Notification and NotificationFlow
                _prmCommonService.PRMUnit.NotificationRepository.SaveChanges();
                _prmCommonService.PRMUnit.NotificationFlowRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Something wrong during sending notificaiton! " + ex.Message, ex.InnerException);
            }

            return sendSuccessfull;
        }
    }


    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var parent in source)
            {
                yield return parent;

                var children = selector(parent);
                foreach (var child in SelectRecursive(children, selector))
                    yield return child;
            }
        }
    }


}