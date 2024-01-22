using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{ 
    public class ProfessionController : Controller
    {
        //private PRM_MfsIwmEntities db = new PRM_MfsIwmEntities();

        ////
        //// GET: /PRM/Profession/

        //public ViewResult Index()
        //{
        //    return View(db.PRM_Profession.ToList());
        //}

        ////
        //// GET: /PRM/Profession/Details/5

        //public ViewResult Details(int id)
        //{
        //    PRM_Profession prm_profession = db.PRM_Profession.Single(p => p.Id == id);
        //    return View(prm_profession);
        //}

        ////
        //// GET: /PRM/Profession/Create

        //public ActionResult Create()
        //{
        //    return View();
        //} 

        ////
        //// POST: /PRM/Profession/Create

        //[HttpPost]
        //public ActionResult Create(PRM_Profession prm_profession)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.PRM_Profession.AddObject(prm_profession);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");  
        //    }

        //    return View(prm_profession);
        //}
        
        ////
        //// GET: /PRM/Profession/Edit/5
 
        //public ActionResult Edit(int id)
        //{
        //    PRM_Profession prm_profession = db.PRM_Profession.Single(p => p.Id == id);
        //    return View(prm_profession);
        //}

        ////
        //// POST: /PRM/Profession/Edit/5

        //[HttpPost]
        //public ActionResult Edit(PRM_Profession prm_profession)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.PRM_Profession.Attach(prm_profession);
        //        db.ObjectStateManager.ChangeObjectState(prm_profession, EntityState.Modified);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(prm_profession);
        //}

        ////
        //// GET: /PRM/Profession/Delete/5
 
        //public ActionResult Delete(int id)
        //{
        //    PRM_Profession prm_profession = db.PRM_Profession.Single(p => p.Id == id);
        //    return View(prm_profession);
        //}

        ////
        //// POST: /PRM/Profession/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{            
        //    PRM_Profession prm_profession = db.PRM_Profession.Single(p => p.Id == id);
        //    db.PRM_Profession.DeleteObject(prm_profession);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}