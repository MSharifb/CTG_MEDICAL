using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web
{
    public class EmailTemplateViewModel : BaseViewModel
    {
        public EmailTemplateViewModel()
        {
            this.EmailTypeList = new List<SelectListItem>();
            this.EmailTemplateVariableList = new List<EmailTemplateVariableViewModel>();

            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@ProjectNo" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@ProjectTitle" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@Amount" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@InvoiceNo" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@PlanDate" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@ScheduleDate" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@RemainingDays" });

            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@DeliveryPlanDate" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@SubmissionDate" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@RealizationDate" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@Deliverables" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@DeliveryDate" });

            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@Initial" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@Name" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@Division" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@Year" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@Month" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@FNight" });
            EmailTemplateVariableList.Add(new EmailTemplateVariableViewModel { VariableNames = "@@CloseDate" });             
        }

        #region Primitive Properties

        [Required(), DisplayName("Email Type")]
        public virtual int EmailTypeId
        {
            get;
            set;
        }
        public IList<SelectListItem> EmailTypeList { get; set; }
        public IList<EmailTemplateVariableViewModel> EmailTemplateVariableList { get; set; }

        [DisplayName("Additional CC"), MaxLength(300), UIHint("_MultiLine")]
        public virtual string AdditionalCC
        {
            get;
            set;
        }

        [Required(), DisplayName("Email Subject"), MaxLength(100)]
        public virtual string EmailSubject
        {
            get;
            set;
        }

        [Required(), DisplayName("Email Body"), UIHint("_MultiLineBig")]
        public virtual string EmailBody
        {
            get;
            set;
        }

        #endregion
    }

    public class EmailTemplateVariableViewModel
    {
        public string VariableNames { get; set; }
    }
}

