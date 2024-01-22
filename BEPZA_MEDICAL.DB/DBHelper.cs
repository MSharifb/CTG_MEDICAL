using System;
using System.Text;
using System.Xml;
using System.Data;
using System.Linq;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.Common;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace BEPZA_MEDICAL.DB
{
    public class DBHelper
    {
        #region constants
        private const string DEFAULT_PARATMER_DIRECTION = "input";
        private const string INPUT = "output";
        private const string OUTPUT = "inputoutput";
        #endregion

        private bool bolIncludeOutputParam = true;
        private CommandType cmdType = CommandType.StoredProcedure;
        private string strReturnMessage;

        public string ReturnMessage
        {
            set { this.strReturnMessage = value; }
            get { return this.strReturnMessage; }
        }
        public bool IncludeOutputParameter
        {
            set { bolIncludeOutputParam = value; }
            get { return bolIncludeOutputParam; }
        }

        public CommandType CommandType
        {
            set { cmdType = value; }
            get { return cmdType; }
        }
        /// <summary>
        /// Return the number the rows affected in the database 
        /// </summary>
        /// <param name="parameterValues">Parameter Values Array of Type Object</param>
        /// <param name="outValueofParam">Not Required can be passed dummy object </param>
        /// <param name="objIdbTransaction">if you want to maintain the transaction</param>
        /// <param name="spno">storedprocedure number you want to execute</param>
        /// <returns>int </returns>
        /// <example>Rows affected </example>

        public int ExecuteNonQuery(ArrayList parameterName, ArrayList parameterValues, ArrayList dbType, ref object outValueofParam, IDbTransaction objIdbTransaction, string CommandName)
        {
            IDbCommand objIdbCommand = DataProvider.GetIDBCommand();
            int rowEffected = -1;
            using (IDbConnection objIdbConnection = DataProvider.GetDBConnection())
            {
                PrepareCommand(objIdbCommand, objIdbConnection, objIdbTransaction, parameterName, parameterValues, dbType, CommandName);
                if (objIdbCommand.Connection.State == ConnectionState.Closed)
                {
                    objIdbCommand.Connection.Open();
                }
                rowEffected = objIdbCommand.ExecuteNonQuery();
                if (Utility.strDBProvider == "sqlserver")
                {

                    SqlParameter p;
                    p = (SqlParameter)objIdbCommand.Parameters["@numErrorCode"];
                    rowEffected = (int)p.Value;

                    SqlParameter p2;
                    p2 = (SqlParameter)objIdbCommand.Parameters["@strErrorMsg"];
                    ReturnMessage = (string)p2.Value;

                }
                objIdbCommand.Connection.Close();

            }
            return rowEffected;
        }

        public int ExecuteNonQuery(ArrayList parameterName, ArrayList parameterValues, ArrayList dbType, IDbTransaction objIdbTransaction, IDbConnection connection, string CommandName)
        {
            IDbCommand objIdbCommand = DataProvider.GetIDBCommand();
            int rowEffected = -1;
            IDbConnection objIdbConnection = connection;

            PrepareCommand(objIdbCommand, objIdbConnection, objIdbTransaction, parameterName, parameterValues, dbType, CommandName);
            if (objIdbCommand.Connection.State == ConnectionState.Closed)
            {
                objIdbCommand.Connection.Open();
            }
            rowEffected = objIdbCommand.ExecuteNonQuery();

            if (Utility.strDBProvider == "sqlserver")
            {
                SqlParameter p;
                p = (SqlParameter)objIdbCommand.Parameters["@numErrorCode"];
                rowEffected = (int)p.Value;

                SqlParameter p2;
                p2 = (SqlParameter)objIdbCommand.Parameters["@strErrorMsg"];
                ReturnMessage = (string)p2.Value;
               
            }

            return rowEffected;
        }

        public DataTable ExecuteQueryToDataTable(ArrayList parameterName, ArrayList parameterValues, ArrayList dbType, ref object outValueofParam, IDbTransaction objIdbTransaction, string CommandName)
        {
            DataSet ds = new DataSet(CommandName);
            IDbCommand objIdbCommand = DataProvider.GetIDBCommand();

            using (IDbConnection objIdbConnection = DataProvider.GetDBConnection())
            {
                //Handle SingleQoute and Sql Injection
                if (CommandName != "DynamicQueryAnalyzer_SPGetQueryData")// skip for query analyzer
                {
                    for (int i = 0; i < parameterValues.Count; i++)
                    {
                        if (parameterValues[i] != null) // added by Sheikh Salahuddin for safe code (null check)
                        {
                            parameterValues[i] = Utility.RelaceSingleQoute(parameterValues[i]);
                            parameterValues[i] = Utility.RelaceBadSqlKeyword(parameterValues[i]);
                        }
                    }
                }

                PrepareCommand(objIdbCommand, objIdbConnection, objIdbTransaction, parameterName, parameterValues, dbType, CommandName);

                if (objIdbCommand.Connection.State == ConnectionState.Closed)
                {
                    objIdbCommand.Connection.Open();
                }
                
                IDataAdapter objIdbAdapter = DataProvider.GetIDBAdapter(objIdbCommand);
                objIdbAdapter.Fill(ds);

                if (objIdbCommand.Parameters.Contains("@numTotalRows"))
                {

                    if (Utility.strDBProvider == "sqlserver")
                    {
                        SqlParameter p;
                        p = (SqlParameter)objIdbCommand.Parameters["@numTotalRows"];
                        outValueofParam = p.Value;
                    }

                }

                if (Utility.strDBProvider == "sqlserver")
                {

                    SqlParameter p2;
                    p2 = (SqlParameter)objIdbCommand.Parameters["@strErrorMsg"];
                    ReturnMessage = (string)p2.Value;
                }

                objIdbCommand.Connection.Close();
            }

            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Return the DataSet object .
        /// It gets the data in dataset from
        /// Database.
        /// </summary>
        /// <param name="parameterValues">Parameter Values Array of Type Object</param>
        /// <param name="outValueofParam">Not Required can be passed dummy object </param>
        /// <param name="objIdbTransaction">if you want to maintain the transaction</param>
        /// <param name="spno">storedprocedure number you want to execute</param>
        /// <returns>dataset</returns>
        public DataSet ExecuteDataSet(ArrayList parameterName, ArrayList parameterValues, ArrayList dbType, ref object outValueofParam, IDbTransaction objIdbTransaction, string CommandName)
        {
            IDbCommand objIdbCommand = DataProvider.GetIDBCommand();
            DataSet dsValues = null;
            using (IDbConnection objIdbConnection = DataProvider.GetDBConnection())
            {
                PrepareCommand(objIdbCommand, objIdbConnection, objIdbTransaction, parameterName, parameterValues, dbType, CommandName);
                IDataAdapter objIdbAdapter = DataProvider.GetIDBAdapter(objIdbCommand);
                dsValues = new DataSet();
                objIdbAdapter.Fill(dsValues);
            }
            return dsValues;
        }
        /// <summary>
        /// Prepare the command and 
        /// set it's attributes
        /// </summary>
        /// <param name="objIdbCommand"></param>
        /// <param name="objIdbConnection"></param>
        /// <param name="objIdbTransaction"></param>
        /// <param name="parameterValues"></param>
        /// <param name="spno"></param>
        private void PrepareCommand(IDbCommand objIdbCommand, IDbConnection objIdbConnection, IDbTransaction objIdbTransaction, ArrayList parameterName, ArrayList parameterValues, ArrayList dbType, string CommandName)
        {
            objIdbCommand.Connection = objIdbConnection;
            try
            {
                objIdbCommand.CommandType = CommandType;
                if (objIdbTransaction != null)
                {
                    objIdbCommand.Transaction = objIdbTransaction;
                }

                this.CommandParamterMaker(objIdbCommand, parameterName, parameterValues, dbType, CommandName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Reterive the parameter the from the xml files
        /// and set into the parameters.
        /// </summary>
        /// <param name="objIdbCommand"></param>
        /// <param name="parameterValues"></param>
        /// <param name="spno"></param>
        /// <returns></returns>
        private IDataParameter[] CommandParamterMaker(IDbCommand objIdbCommand, ArrayList parameterName, ArrayList parameterValues, ArrayList dbType, string ProcedureName)
        {

            objIdbCommand.CommandText = ProcedureName;
            IDataParameter[] IDBParameter = null;
            IDBParameter = DataProvider.GetParameter(parameterValues.Count + 2);

            int paramcounter = 0;
            foreach (string str in parameterName)
            {
                IDBParameter[paramcounter] = DataProvider.GetParameterInstance();
                IDBParameter[paramcounter].ParameterName = str;

                if (parameterValues[paramcounter] == null)
                {
                    IDBParameter[paramcounter].Value = DBNull.Value;
                }
                else
                {
                    IDBParameter[paramcounter].Value = parameterValues[paramcounter];
                }


                if (IDBParameter[paramcounter].DbType == DbType.DateTime)
                {

                    if (Convert.ToDateTime(IDBParameter[paramcounter].Value) == DateTime.MinValue || Convert.ToDateTime(IDBParameter[paramcounter].Value) == DateTime.MaxValue)
                    {
                        IDBParameter[paramcounter].Value = DBNull.Value;
                    }
                }
                else if (IDBParameter[paramcounter].DbType == DbType.Int32 && str.ToUpper().EndsWith("ID") && ProcedureName.ToUpper().Contains("SAVE"))
                {

                    if (Convert.ToInt32(IDBParameter[paramcounter].Value) <= 0)
                    {
                        IDBParameter[paramcounter].Value = DBNull.Value;
                    }
                }

                IDBParameter[paramcounter].Direction = ParameterDirection.Input;
                if (parameterName[paramcounter].ToString() == "@numTotalRows")
                {
                    IDBParameter[paramcounter].DbType = System.Data.DbType.Int32;
                    IDBParameter[paramcounter].Direction = ParameterDirection.Output;
                }
                objIdbCommand.Parameters.Add(IDBParameter[paramcounter]);

                paramcounter++;
            }

            IDBParameter[paramcounter] = DataProvider.GetParameterInstance();
            IDBParameter[paramcounter].ParameterName = "@numErrorCode";
            IDBParameter[paramcounter].DbType = System.Data.DbType.Int32;
            IDBParameter[paramcounter].Direction = ParameterDirection.Output;

            objIdbCommand.Parameters.Add(IDBParameter[paramcounter]);

            paramcounter++;

            IDBParameter[paramcounter] = DataProvider.GetParameterInstanceForOutput("@strErrorMsg");
            IDBParameter[paramcounter].ParameterName = "@strErrorMsg";
            IDBParameter[paramcounter].DbType = System.Data.DbType.String;
            IDBParameter[paramcounter].Direction = ParameterDirection.Output;

            objIdbCommand.Parameters.Add(IDBParameter[paramcounter]);

            return IDBParameter;
        }

        public static IDbTransaction GetTransaction(IDbConnection connection)
        {
            return DataProvider.GetTransactionInstacne(connection);
        }
        /// <summary>
        /// 
        //Get connection Instance
        /// </summary>
        /// <returns></returns>
        public static IDbConnection GetConnection()
        {
            return DataProvider.GetDBConnection();
        }

    }
}