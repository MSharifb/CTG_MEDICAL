
using BEPZA_MEDICAL.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class DynamicQueryAnalyzerRepository 
    {
        DBHelper _obDBHelper = new DBHelper();

        #region  Members

        //public List<Category> GetCategoryList()
        //{
        //    return _objBll.GetCategoryList();
        //}

        //public List<CategoryItem> GetCategoryItemList()
        //{
        //    return _objBll.GetCategoryItemList();
        //}

        public static DataTable GetColumnData(string query)
        {
            object paramval = null;

            CustomParameterList cpList = new CustomParameterList();
            cpList.Add(new CustomParameter("@Query", query, DbType.String));

            DBHelper db = new DBHelper();
            DataTable dt = db.ExecuteQueryToDataTable(cpList.ParameterNames, cpList.ParameterValues, cpList.ParameterTypes, ref paramval, null, "DynamicQueryAnalyzer_SPGetQueryColumnData");

            return dt;
        }

        public static DataTable GetQueryData(string query, string strSQL, string sortBy, string sortOrder, int startIndex, int maximumRows, out int numTotalRows)
        {
            object paramval = null;

            CustomParameterList cpList = new CustomParameterList();
            cpList.Add(new CustomParameter("@Query", query, DbType.String));
            cpList.Add(new CustomParameter("@strSQL", strSQL, DbType.String));
            cpList.Add(new CustomParameter("@SortBy", sortBy, DbType.String));
            cpList.Add(new CustomParameter("@SortOrder", sortOrder, DbType.String));
            cpList.Add(new CustomParameter("@startIndex", startIndex, DbType.Int32));
            cpList.Add(new CustomParameter("@maximumRows", maximumRows, DbType.Int32));
            cpList.Add(new CustomParameter("@numTotalRows", 0, DbType.Int32));

            DBHelper db = new DBHelper();
            DataTable dt = db.ExecuteQueryToDataTable(cpList.ParameterNames, cpList.ParameterValues, cpList.ParameterTypes, ref paramval, null, "DynamicQueryAnalyzer_SPGetQueryData");
            numTotalRows = (int)paramval;
            return dt;            
        }

        public static DataTable GetApplicantList(int jobId, int uniId, int desId, string degree)
        {
            object paramval = null;

            CustomParameterList cpList = new CustomParameterList();
            cpList.Add(new CustomParameter("@JobAdId", jobId, DbType.Int32));
            cpList.Add(new CustomParameter("@universityid", uniId, DbType.Int32));
            cpList.Add(new CustomParameter("@Desid", desId, DbType.Int32));
            cpList.Add(new CustomParameter("@degreeType", degree, DbType.String));

            DBHelper db = new DBHelper();
            DataTable dt = db.ExecuteQueryToDataTable(cpList.ParameterNames, cpList.ParameterValues, cpList.ParameterTypes, ref paramval, null, "EREC_uspReportUniversitywiseApplicants");
            return dt;
        }


        public string GetReporData()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}