using System;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class CustomSearchViewModel
    {
        #region Properties
        public string ID { get; set; }

        public string Name { get; set; }

        public int Rank { get; set; }

        public string JobDescription { get; set; }

        #endregion

        #region Methods

        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(ID))
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));
            if (!String.IsNullOrWhiteSpace(Name))
                filterExpressionBuilder.Append(String.Format("Name like {0} AND ", Name));           
            if (Rank > 0)
                filterExpressionBuilder.Append(String.Format("Rank = {0} AND ", Rank));
            if (!String.IsNullOrWhiteSpace(JobDescription))
                filterExpressionBuilder.Append(String.Format("JobDescription = \"{0}\" AND ", JobDescription));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }

        #endregion
    }      
}