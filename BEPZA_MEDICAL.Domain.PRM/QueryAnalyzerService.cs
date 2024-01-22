using BEPZA_MEDICAL.DAL.PRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class QueryAnalyzerService
    {
        #region Ctor
        private ERP_BEPZAPRMEntities _Context;
        public QueryAnalyzerService()
        {
            this._Context = new ERP_BEPZAPRMEntities();
        }
        #endregion

        #region Workflow methods

        public string GetColumnType(string tableName, string CoulmnName)
        {
            var query = @"Select distinct " + CoulmnName + " from " + tableName;

            return query;
        }
        #endregion

        public  string GetDataTypeByTableNameAndColumnName(string tableName, string columnName)
        {
            string dataType = string.Empty;
            dataType = _Context
            .GetType()
            .GetProperties()
            .Where(p => p.PropertyType.IsGenericType)
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .Where(t => t.Name == tableName)
            .Select(t => t.GetProperties().Where(q => q.Name == columnName)).FirstOrDefault().Select(q => q.PropertyType.FullName).First();
            return dataType;
        }
    }
}
