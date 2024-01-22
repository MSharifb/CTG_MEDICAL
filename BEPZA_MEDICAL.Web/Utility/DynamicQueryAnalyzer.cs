using BEPZA_MEDICAL.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class DynamicQueryAnalyzer
    {
        //public static List<Category> GetCategoryList()
        //{

        //    try
        //    {
        //        CustomParameterList cpList = new CustomParameterList();
        //        object paramval = null;
        //        DBHelper db = new DBHelper();
        //        DataTable dt = db.ExecuteQueryToDataTable(cpList.ParameterNames, cpList.ParameterValues, cpList.ParameterTypes, ref paramval, null, "HRM_usptblCategory");

        //        List<Category> results = new List<Category>();
        //        foreach (DataRow dr in dt.Rows)
        //        {

        //            Category obj = new Category();

        //            MapperBase.GetInstance().MapItem(obj, dr); ;
        //            results.Add(obj);
        //        }

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //}

        //public static List<CategoryItem> GetCategoryItemList()
        //{

        //    try
        //    {
        //        CustomParameterList cpList = new CustomParameterList();
        //        object paramval = null;
        //        DBHelper db = new DBHelper();
        //        DataTable dt = db.ExecuteQueryToDataTable(cpList.ParameterNames, cpList.ParameterValues, cpList.ParameterTypes, ref paramval, null, "HRM_usptblItem");

        //        List<CategoryItem> results = new List<CategoryItem>();
        //        foreach (DataRow dr in dt.Rows)
        //        {

        //            CategoryItem obj = new CategoryItem();

        //            MapperBase.GetInstance().MapItem(obj, dr); ;
        //            results.Add(obj);
        //        }

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //}

        public static DataTable GetColumnData(string Query)
        {
            object paramval = null;

            CustomParameterList cpList = new CustomParameterList();
            cpList.Add(new CustomParameter("@Query", Query, DbType.String));


            DBHelper db = new DBHelper();
            DataTable dt = db.ExecuteQueryToDataTable(cpList.ParameterNames, cpList.ParameterValues, cpList.ParameterTypes, ref paramval, null, "DynamicQueryAnalyzer_SPGetQueryColumnData");

            return dt;
        }

        public static DataTable GetQueryData(string Query, string strSQL, string sortBy, string sortOrder, int startIndex, int maximumRows, out int numTotalRows)
        {
            object paramval = null;

            CustomParameterList cpList = new CustomParameterList();
            cpList.Add(new CustomParameter("@Query", Query, DbType.String));
            cpList.Add(new CustomParameter("@strSQL", strSQL, DbType.String));
            cpList.Add(new CustomParameter("@SortBy", sortBy, DbType.String));
            cpList.Add(new CustomParameter("@SortOrder", sortOrder, DbType.String));
            cpList.Add(new CustomParameter("@startIndex", startIndex, DbType.Int32));
            cpList.Add(new CustomParameter("@maximumRows", maximumRows, DbType.Int32));
            cpList.Add(new CustomParameter("@numTotalRows", 0, DbType.Int32));

            DBHelper db = new DBHelper();
            DataTable dt = db.ExecuteQueryToDataTable(cpList.ParameterNames, cpList.ParameterValues, cpList.ParameterTypes, ref paramval, null, "HRM_uspGetQueryData");
            numTotalRows = (int)paramval;

            return dt;
        }
    }
}