using System;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Elmah;
using BEPZA_MEDICAL.Web.Resources;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class CommonExceptionMessage
    {
        public static string GetExceptionMessage(Exception ex, CommonAction actionName, String contextualMessage = null)
        {
            string message = String.Empty;

            if (actionName == CommonAction.Save)
            {
                message = Common.GetCommomMessage(CommonMessage.InsertFailed);
            }
            if (actionName == CommonAction.Delete)
            {
                message = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            }
            if (actionName == CommonAction.Update)
            {
                message = Common.GetCommomMessage(CommonMessage.UpdateFailed);
            }
            if (actionName == CommonAction.General)
            {
                message = ex.Message;
            }

            try
            {
                if (ex.InnerException != null 
                    && (ex.InnerException is SqlException || ex.InnerException is EntityCommandExecutionException))
                {
                    SqlException sqlException = ex.InnerException as SqlException;
                    message = GetSqlExceptionMessage(sqlException.Number, sqlException.Message);
                }

                ErrorLog.LogError(ex, contextualMessage);
            }
            catch (Exception)
            {

            }

            return message;
        }

        public static string GetSqlExceptionMessage(int number, String strInnerExcMsg = "")
        {
            //set default value which is the generic exception message          

            string error = ErrorMessages.ExceptionMessage.ToString(); //MyConfiguration.Texts.GetString(ExceptionKeys.DalExceptionOccured);
            switch (number)
            {
                case 4060:
                    // Invalid Database
                    error = ErrorMessages.InvalidDatabase.ToString();
                    break;
                case 18456:
                    // Login Failed
                    error = ErrorMessages.DBLoginFailed.ToString();
                    break;

                case 547:
                    // ForeignKey Violation
                    error = ErrorMessages.ForeignKeyViolation.ToString();
                    break;
                case 2732:
                    // ForeignKey Violation
                    error = ErrorMessages.ForeignKeyViolation.ToString();
                    break;

                case 2627:
                    // Unique Index/Constriant Violation
                    error = ErrorMessages.UniqueIndex.ToString();
                    break;
                case 2601:
                    // Unique Index/Constriant Violation
                    error = ErrorMessages.UniqueIndex.ToString();
                    break;
                default:
                    // throw a general Exception                   
                    if (!String.IsNullOrEmpty(strInnerExcMsg))
                    {
                        error = strInnerExcMsg;
                    }
                    break;
            }

            return error;
        }
    }

    public static class ErrorLog
    {
        public static void LogError(Exception ex, string contextualMessage = null)
        {
            try
            {
                // log exception with contextual informatin that's visible when clicking on the error int he Elmah log
                if (String.IsNullOrEmpty(contextualMessage))
                {
                       var annotatedException= new Exception(contextualMessage, ex);
                    ErrorSignal.FromCurrentContext().Raise(annotatedException, HttpContext.Current);
                }
                else
                {
                    ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                }
            }
            catch (Exception )
            {
                // uh oh! just keep going...
            }
        }
    }
}