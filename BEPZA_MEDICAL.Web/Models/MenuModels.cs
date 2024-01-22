using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Models
{
    public class MenuModels
    {
        #region Ctor
        public MenuModels()
        {
            MenuList = new List<Menu>();
        }
        #endregion
        public List<Menu> MenuList;
    }    
}