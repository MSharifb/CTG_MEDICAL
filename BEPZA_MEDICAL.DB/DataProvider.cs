using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;

namespace BEPZA_MEDICAL.DB
{
    class DataProvider
    {

        #region constants and class varibles
        private const string _SQLSERVER = "sqlserver";
        private const string _ORACLE = "oracle";
        private const string _OTHER = "other";
        #endregion

        #region Get Connection From Factory
        private static IDbConnection GetConnectionFromFactory(IDbConnection objIdbConnection)
        {
            switch (Utility.strDBProvider)
            {
                case _SQLSERVER:
                    {
                        objIdbConnection = new SqlConnection(Utility.strDBConnectionString);
                        break;
                    }
                case _ORACLE:
                    {
                        objIdbConnection = new OracleConnection(Utility.strDBConnectionString);
                        break;
                    }
                case _OTHER:
                    {
                        objIdbConnection = new OleDbConnection(Utility.strDBConnectionString);
                        break;
                    }
            }
            return objIdbConnection;
        }
        #endregion

        #region Get DataBase Connnection
        /// <summary>
        /// return the IdbConnection 
        /// </summary>
        /// <returns></returns>
        public static IDbConnection GetDBConnection()
        {

            IDbConnection objIdbConnection = null;
            objIdbConnection = GetConnectionFromFactory(objIdbConnection);

            return objIdbConnection;
        }
        #endregion

        public static IDbCommand GetIDBCommand()
        {
            IDbCommand objIdbComand = null;

            switch (Utility.strDBProvider)
            {
                case _SQLSERVER:
                    {
                        objIdbComand = new SqlCommand();
                        objIdbComand.CommandTimeout = Utility.intDBCommandTimeout;
                        break;
                    }
                case _ORACLE:
                    {
                        objIdbComand = new OracleCommand();
                        objIdbComand.CommandTimeout = Utility.intDBCommandTimeout;
                        break;
                    }

                case _OTHER:
                    {
                        objIdbComand = new OleDbCommand();
                        objIdbComand.CommandTimeout = Utility.intDBCommandTimeout;
                        break;
                    }
            }

            return objIdbComand;
        }



        public static IDataParameter[] GetParameter(int parmaterCount)
        {
            IDataParameter[] idbParamters = null;
            if (parmaterCount > 0)
            {

                switch (Utility.strDBProvider)
                {
                    case _SQLSERVER:
                        {
                            idbParamters = new SqlParameter[parmaterCount];
                            break;
                        }
                    case _ORACLE:
                        {
                            //idbParamters = new OleDbParameter[parmaterCount];
                            idbParamters = new OracleParameter[parmaterCount];
                            break;
                        }

                    case _OTHER:
                        {
                            idbParamters = new OleDbParameter[parmaterCount];
                            break;
                        }
                }
            }
            return idbParamters;
        }

        public static IDataParameter GetParameterInstance()
        {
            IDataParameter idbParamters = null;

            switch (Utility.strDBProvider)
            {
                case _SQLSERVER:
                    {
                        idbParamters = new SqlParameter();


                        break;
                    }
                case _ORACLE:
                    {
                        //idbParamters = new OleDbParameter();
                        idbParamters = new OracleParameter();
                        break;
                    }

                case _OTHER:
                    {
                        idbParamters = new OleDbParameter();
                        break;
                    }
            }
            return idbParamters;
        }

        public static IDataParameter GetParameterInstanceForOutput(string strParamName)
        {
            IDataParameter idbParamters = null;

            switch (Utility.strDBProvider)
            {
                case _SQLSERVER:
                    {
                        idbParamters = new SqlParameter(strParamName, SqlDbType.VarChar, 200);


                        break;
                    }
                case _ORACLE:
                    {
                        idbParamters = new OracleParameter(strParamName, OracleType.VarChar, 200);
                        break;
                    }

                case _OTHER:
                    {
                        idbParamters = new OdbcParameter(strParamName, OdbcType.VarChar, 200);
                        break;
                    }
            }
            return idbParamters;
        }


        public static IDataParameter GetCursorParameterInstanceForOutput(string strParamName)
        {
            IDataParameter idbParamters = null;

            switch (Utility.strDBProvider)
            {
                case _SQLSERVER:
                    {
                        idbParamters = new SqlParameter(strParamName, SqlDbType.VarChar, 200);
                        break;
                    }
                case _ORACLE:
                    {
                        idbParamters = new OracleParameter(strParamName, OracleType.Cursor);
                        break;
                    }

                case _OTHER:
                    {
                        idbParamters = new OleDbParameter(strParamName, OleDbType.VarChar, 200);
                        break;
                    }
            }
            return idbParamters;
        }
        public static IDataAdapter GetIDBAdapter(IDbCommand objIdbCommand)
        {
            IDataAdapter objIdbAdapter = null;


            switch (Utility.strDBProvider)
            {
                case _SQLSERVER:
                    {
                        objIdbAdapter = new SqlDataAdapter((SqlCommand)objIdbCommand);

                        break;
                    }
                case _ORACLE:
                    {
                        //objIdbAdapter = new OleDbDataAdapter((OleDbCommand)objIdbCommand);
                        objIdbAdapter = new OracleDataAdapter((OracleCommand)objIdbCommand);
                        break;
                    }

                case _OTHER:
                    {
                        objIdbAdapter = new OleDbDataAdapter((OleDbCommand)objIdbCommand);
                        break;
                    }
            }

            return objIdbAdapter;
        }

        public static IDbTransaction GetTransactionInstacne(IDbConnection connection)
        {
            IDbTransaction objIdbTransaction = null;

            switch (Utility.strDBProvider)
            {
                case _SQLSERVER:
                    {
                        objIdbTransaction = (SqlTransaction)connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        break;
                    }
                case _ORACLE:
                    {
                        objIdbTransaction = (OracleTransaction)connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        break;
                    }

                case _OTHER:
                    {

                        objIdbTransaction = (OleDbTransaction)connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        break;
                    }
            }

            return objIdbTransaction;

        }
    }
}
