using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DB
{
    public class CustomParameter
    {

        private string strParamName;
        private System.Data.DbType dbtType;
        private object objValue;
        private System.Data.ParameterDirection paramDirection;

        public String ParamaterName
        {
            set { this.strParamName = value; }
            get { return this.strParamName; }
        }

        public System.Data.DbType ParamType
        {
            set { this.dbtType = value; }
            get { return this.dbtType; }
        }

        public object ParamValue
        {
            set { this.objValue = value; }
            get { return this.objValue; }
        }

        public System.Data.ParameterDirection ParamDirection
        {
            set { this.paramDirection = value; }
            get { return this.paramDirection; }
        }

        public CustomParameter(string strPramName, object objValue, System.Data.DbType type)
        {
            this.ParamaterName = strPramName;
            this.ParamType = type;
            this.ParamValue = objValue;
        }

        public CustomParameter(string strPramName, object objValue, System.Data.DbType type, System.Data.ParameterDirection direction)
        {
            this.ParamaterName = strPramName;
            this.ParamType = type;
            this.ParamValue = objValue;
            this.ParamDirection = direction;
        }
    }
}
