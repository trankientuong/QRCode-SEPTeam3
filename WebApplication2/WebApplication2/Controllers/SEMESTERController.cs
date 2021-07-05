using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    //[Authorize(Roles = "Lecturer")]
    public class SEMESTERController : Controller
    {

        private SEP24Team3Entities db = new SEP24Team3Entities();

        // GET: SEMESTER
        public ActionResult Index()
        {
            return View(db.SEMESTERs.ToList());
        }

        // GET: SEMESTER/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SEMESTER sEMESTER = db.SEMESTERs.Find(id);
            if (sEMESTER == null)
            {
                return HttpNotFound();
            }
            return View(sEMESTER);
        }

        // GET: SEMESTER/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SEMESTER/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SEMESTER_CODE,SEMESTERNAME,SEMESTERYEAR")] SEMESTER sEMESTER)
        {
            if (ModelState.IsValid)
            {
                db.SEMESTERs.Add(sEMESTER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sEMESTER);
        }

        // GET: SEMESTER/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SEMESTER sEMESTER = db.SEMESTERs.Find(id);
            if (sEMESTER == null)
            {
                return HttpNotFound();
            }
            return View(sEMESTER);
        }

        // POST: SEMESTER/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SEMESTER_CODE,SEMESTERNAME,SEMESTERYEAR")] SEMESTER sEMESTER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sEMESTER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sEMESTER);
        }

        // GET: SEMESTER/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SEMESTER sEMESTER = db.SEMESTERs.Find(id);
            if (sEMESTER == null)
            {
                return HttpNotFound();
            }
            return View(sEMESTER);
        }

        // POST: SEMESTER/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SEMESTER sEMESTER = db.SEMESTERs.Find(id);
            db.SEMESTERs.Remove(sEMESTER);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
