using System;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Text.RegularExpressions;

namespace BEPZA_MEDICAL.DB
{
    public static class Utility
    {
        private static System.String strConnection;
        public static string strDBConnectionString
        {
            get { return strConnection; }
            set { strConnection = value; }
        }
        
        private static System.String strProvider;
        public static string strDBProvider
        {
            get { return strProvider.ToLower(); }
            set { strProvider = value; }
        }
        
        private static System.Int32 intCommandTimeout;
        public static Int32 intDBCommandTimeout
        {
            get { return intCommandTimeout; }
            set { intCommandTimeout = value; }
        }

        public static string GetPackageName()
        {
            return "pos.";
        }

        #region DBErrorMsgHandler
        public static int ErrorCode = -10000000;

        //Sync this Method with HR3PLO_GSK.Utilities.Message.ErroMessageCodeEnum and DbErrorMessage
        public enum ErroMessageCodeEnum
        {
            DuplicateDataUK = -2601,
            DuplicateDataPK = -2627,
            ForeignKeyConstraint = -547,
            ForeignKeyReference = -548,
            NotNullConstraint = -515,
            DuplicateDataCUSTOM = -50010,
            DuplicatePathAssign = -50011,
            WeekendDateOverLapping = -50012
        }

        public static int GetDBErrorCode(string strErrorMessage)
        {
            if (string.IsNullOrEmpty(strErrorMessage))
                return -1;
            else if (strErrorMessage.ToUpper().Contains("PRIMARY KEY"))
                return (int)ErroMessageCodeEnum.DuplicateDataPK;
            else if (strErrorMessage.ToUpper().Contains("DUPLICATE KEY")
                || strErrorMessage.ToUpper().Contains("UNIQUE KEY"))
                return (int)ErroMessageCodeEnum.DuplicateDataUK;
            else if (strErrorMessage.ToUpper().Contains("DELETE")
                && strErrorMessage.ToUpper().Contains("REFERENCE"))
                return (int)ErroMessageCodeEnum.ForeignKeyConstraint;
            else if (strErrorMessage.ToUpper().Contains("INSERT")
                && strErrorMessage.ToUpper().Contains("FOREIGN KEY"))
                return (int)ErroMessageCodeEnum.ForeignKeyReference;
            else if (strErrorMessage.ToUpper().Contains("INSERT")
                && strErrorMessage.ToUpper().Contains("NULL"))
                return (int)ErroMessageCodeEnum.NotNullConstraint;
            else
                return -1;
        }

        #endregion

        public static object RelaceSingleQoute(object obj)
        {
            string str = obj.ToString();
            if (str.Contains("'"))
                obj = str.Replace("'", "");
            return obj;
        }

        public static object RelaceBadSqlKeyword(object obj)
        {
            string str = obj.ToString();
            if (str.ToUpper().Contains("EXEC "))
                obj = str.ToUpper().Replace("EXEC ", "");
            if (str.ToUpper().Contains("EXECUTE "))
                obj = str.ToUpper().Replace("EXECUTE ", "");

            if (str.ToUpper().Contains("DROP "))
                obj = str.ToUpper().Replace("DROP ", "");
            if (str.ToUpper().Contains("ALTER "))
                obj = str.ToUpper().Replace("ALTER ", "");

            if (str.ToUpper().Contains("TRUNCATE "))
                obj = str.ToUpper().Replace("TRUNCATE ", "");
            if (str.ToUpper().Contains("DELETE "))
                obj = str.ToUpper().Replace("DELETE ", "");
            if (str.ToUpper().Contains("INSERT "))
                obj = str.ToUpper().Replace("INSERT ", "");
            if (str.ToUpper().Contains("OR "))
                obj = str.ToUpper().Replace("OR ", "");
            if (str.Contains(";"))
                obj = str.Replace(";", "");

            


            return obj;
        }
    }

}
