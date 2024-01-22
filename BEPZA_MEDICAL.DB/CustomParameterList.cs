using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BEPZA_MEDICAL.DB
{
    public class CustomParameterList : System.Collections.ArrayList
    {
        private ArrayList parameterNames = new ArrayList();
        private ArrayList dbTypes = new ArrayList();
        private ArrayList paramValues = new ArrayList();
        private int index = 0;

        public ArrayList ParameterNames
        {
            get { return this.parameterNames; }
        }

        public ArrayList ParameterTypes
        {
            get { return this.dbTypes; }
        }

        public ArrayList ParameterValues
        {
            get { return this.paramValues; }
        }

        public void Add(CustomParameter param)
        {
            this.parameterNames.Add(param.ParamaterName);
            this.dbTypes.Add(param.ParamType);

            if (param.ParamType == System.Data.DbType.String && param.ParamValue == null)
                this.paramValues.Add(DBNull.Value);
            else if (param.ParamType == System.Data.DbType.Date && Convert.ToDateTime(param.ParamValue) == DateTime.MinValue)
                this.paramValues.Add(DBNull.Value);
            else
                this.paramValues.Add(param.ParamValue);
        }
        public void Remove(CustomParameter param)
        {
            this.parameterNames.Remove(param.ParamaterName);
            this.dbTypes.Remove(param.ParamType);
            this.paramValues.Remove(param.ParamValue);
        }

    }
}
