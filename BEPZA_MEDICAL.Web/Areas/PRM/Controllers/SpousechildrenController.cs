using BEPZA_MEDICAL.Domain.PRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class SpousechildrenController : Controller
    {
         private readonly PRMCommonSevice _prmCommonservice;

         public SpousechildrenController(PRMCommonSevice prmCommonservice)
        {
            this._prmCommonservice = prmCommonservice;
        }
        //
        // GET: /PRM/Spousechildren/
        public ActionResult Index()
        {
            return View();
        }
	}
}