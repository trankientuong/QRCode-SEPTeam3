using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Libs;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        private SEP24Team3Entities db = new SEP24Team3Entities();

        // GET: Classes
        public ActionResult Index()
        {
            var classes = db.Classes.Include(x => x.AspNetUser).Include(x => x.FACULTY).Include(x => x.SEMESTER);
            return View(classes.ToList());
        }

        [Authorize(Roles = "Lecturer")]
        public ActionResult Index2() // View Of Lecturer
        {
            var lecturerId = User.Identity.GetUserId();
            var classesOfLecturer = db.Classes.Where(x => x.Userid == lecturerId).ToList();
            return View(classesOfLecturer);
        }

        public ActionResult Index3(string userId) // View of Student
        {
            ViewBag.USER_ID = db.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            var studentInfo = db.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            var classOfStudent = db.Classes.Where(x => x.ClassLists.Any(g => g.Student.StudentCode.Equals(studentInfo.UserCode)));
            return View(classOfStudent.ToList());
        }

        //// GET: Classes/Details/5
        //public ActionResult Details(int id)
        //{
        //    var @class = db.Classes.Find(id);

        //    //var classStudents = db..Include(c => c.Class).Include(c => c.Student);
        //    //return View(classStudents.ToList());
        //    return View(@class.ClassLists.ToList());
        //}

        // GET: Classes/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Any(g => g.Name.Equals("Lecturer"))), "Id", "Email");
            //ViewBag.Userid = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Facultyid = new SelectList(db.FACULTies, "ID", "FACULTYNAME");
            ViewBag.Semesterid = new SelectList(db.SEMESTERs, "ID", "SEMESTER_CODE");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Classes.Add(@class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.UserId = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Any(g => g.Name.Equals("Lecturer"))), "Id", "Email");
            //ViewBag.Userid = new SelectList(db.AspNetUsers, "Id", "Email", @class.Userid);
            ViewBag.Facultyid = new SelectList(db.FACULTies, "ID", "FACULTYNAME", @class.Facultyid);
            ViewBag.Semesterid = new SelectList(db.SEMESTERs, "ID", "SEMESTER_CODE", @class.Semesterid);
            return View(@class);
        }

        // GET: Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Any(g => g.Name.Equals("Lecturer"))), "Id", "Email");
            //ViewBag.Userid = new SelectList(db.AspNetUsers, "Id", "Email", @class.Userid);
            ViewBag.Facultyid = new SelectList(db.FACULTies, "ID", "FACULTYNAME", @class.Facultyid);
            ViewBag.Semesterid = new SelectList(db.SEMESTERs, "ID", "SEMESTER_CODE", @class.Semesterid);
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@class).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Any(g => g.Name.Equals("Lecturer"))), "Id", "Email");
            //ViewBag.Userid = new SelectList(db.AspNetUsers, "Id", "Email", @class.Userid);
            ViewBag.Facultyid = new SelectList(db.FACULTies, "ID", "FACULTYNAME", @class.Facultyid);
            ViewBag.Semesterid = new SelectList(db.SEMESTERs, "ID", "SEMESTER_CODE", @class.Semesterid);
            return View(@class);
        }

        // GET: Classes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class @class = db.Classes.Find(id);
            db.Classes.Remove(@class);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult UploadExcel(Class Classes, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    ExcelFileReader excelFileReader = new ExcelFileReader(pathToExcelFile);
                    List<Class> listClasses = excelFileReader.readClasses(); // Get list Classes from excel  
                    var listClassDb = db.Classes.ToList();
                    foreach (Class @class in listClasses)
                    {
                        if (listClassDb.FirstOrDefault(x => x.ClassCode.Equals(@class.ClassCode)) == null)
                            db.Classes.Add(@class);
                    }
                    db.SaveChanges();
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    // Garbage Collection
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
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
