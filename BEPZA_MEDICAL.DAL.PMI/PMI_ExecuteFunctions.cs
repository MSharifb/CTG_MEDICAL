using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.PMI
{
    public class PMI_ExecuteFunctions
    {
        #region Fields
        private readonly ERP_BEPZAPMIEntities _context;
        #endregion

        #region Ctor

        public PMI_ExecuteFunctions(ERP_BEPZAPMIEntities context)
        {
            this._context = context;
        }

        #endregion

        #region Funtions

        public IList<sp_Pmi_GetBudgetHeadInformation_Result> GetBudgetHeadList()
        {
            return _context.sp_Pmi_GetBudgetHeadInformation().DefaultIfEmpty().OfType<sp_Pmi_GetBudgetHeadInformation_Result>().ToList();
        }

        public bool GetBudgetInfo(int? budgetId, int? budgetDetailId, int? financialYearId, int? budgetStatusId, string budgetType)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            var connection = new SqlConnection(connectionString);
            var dataTable = new DataTable();
            try
            {

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }


                //var selectedBudgetType = (from x in _context.PMI_BudgetMaster
                //                  where x.Id == budgetId
                //                  select x
                //                  ).FirstOrDefault().BudgetType;

                string selectedBudgetType = string.Empty;

                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                if (financialYearId > 0)
                {
                    using (var command = new SqlCommand("sp_PMI_RptFinancialBudget", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FinancialYearId", financialYearId);
                        command.Parameters.AddWithValue("@StatusId", budgetStatusId);
                        command.Parameters.AddWithValue("@BudgetType", selectedBudgetType);
                        command.Parameters.AddWithValue("@numErrorCode", 0);
                        command.Parameters.AddWithValue("@strErrorMsg", string.Empty);

                        var adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                }
                else
                {
                    using (var command = new SqlCommand("sp_PMI_PrintBudget", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BudgetMasterId", budgetId);
                        command.Parameters.AddWithValue("@BudgetDetailsId", budgetDetailId);

                        var adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        #endregion
    }
}
