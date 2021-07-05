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
    public class ClassListController : Controller
    {
        private SEP24Team3Entities db = new SEP24Team3Entities();

        // GET: Classes
        public ActionResult Index(int id)
        {
            ViewBag.CLASS_ID = db.Classes.Find(id);
            var classes = db.Classes.Include(x => x.AspNetUser).Include(x => x.FACULTY).Include(x => x.SEMESTER);
            return View(classes.ToList());
        }

     
        public FileResult DownloadExcel()
        {
            string path = "/Doc/Danh_Sach_Sinh_Vien.xlsx";
            return File(path, "application/vnd.ms-excel", "Danh_Sach_Sinh_Vien.xlsx");
        }

        [HttpPost]
        public JsonResult UploadExcel(Student students, HttpPostedFileBase FileUpload,int classId)
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
                    List<Student> listStudents = excelFileReader.readStudent(); // Get list users from excel
                    var userListInDb = db.Students.ToList();
                    ClassList nStd = new ClassList();
                    nStd.Classid = classId;
                    foreach (Student user in listStudents)
                    {
                        //if (userListInDb.FirstOrDefault(x => x.StudentCode.Equals(user.StudentCode)) == null)
                        //{
                        //    db.Students.Add(user);
                        //    nStd.Studentid = user.id;
                        //    db.ClassLists.Add(nStd);
                        //    db.SaveChanges();
                        //}
                        //var studentId = userListInDb.FirstOrDefault(x => x.StudentCode.Equals(user.StudentCode)).id;
                        //if (db.ClassLists.FirstOrDefault(x => x.Classid == classId && x.Studentid == studentId) == null)
                        //{
                        //    nStd.Studentid = studentId;
                        //    db.ClassLists.Add(nStd);
                        //    db.SaveChanges();
                        //}

                        if (userListInDb.Count(student => student.StudentCode == user.StudentCode) == 0)
                        {
                            db.Students.Add(user);
                            db.SaveChanges();
                        }
                        var studentId = db.Students.First(student => student.StudentCode == user.StudentCode).id;
                        if (db.ClassLists.Count(classList => classList.Classid == classId && classList.Studentid == studentId) == 0)
                            db.ClassLists.Add(new ClassList
                            {
                                Classid = classId,
                                Studentid = studentId
                            });
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


        // GET: Classes/Create
        public ActionResult Create(int classId)
        {
            ViewBag.CLASS_ID = db.Classes.Find(classId);
            ViewBag.Facultyid = new SelectList(db.FACULTies, "ID", "FACULTYNAME");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int classId,Student student)
        {
            var @class = db.Classes.Find(classId);
            var @studentList = db.Students.ToList();           
            ClassList nStu = new ClassList();
            //check student is created
            var check = @studentList.FirstOrDefault(x => x.StudentCode.Equals(student.StudentCode));
            //add student if true dont create new student
            nStu.Classid = classId;
            if (check == null)
            {
                db.Students.Add(student);
                nStu.Studentid = student.id;
            }
            else
            {
                nStu.Studentid = check.id;
            }
            db.ClassLists.Add(nStu);
            db.SaveChanges();
            ViewBag.Facultyid = new SelectList(db.FACULTies, "ID", "FACULTYNAME");
            return RedirectToAction("Index","ClassList", new { id = classId });
        }

        // GET: Classes/Edit/5
        public ActionResult Edit(int classId,int studentId)
        {
            ViewBag.CLASS_ID = db.Classes.Find(classId);
            var student = db.Students.Find(studentId);
            ViewBag.STUDENT_ID = db.Students.Find(studentId);
            ViewBag.Facultyid = new SelectList(db.FACULTies, "ID", "FACULTYNAME");
            return View(student);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int classId,Student student)
        {
            var @class = db.Classes.Find(classId);
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.Facultyid = new SelectList(db.FACULTies, "ID", "FACULTYNAME");
            return RedirectToAction("Index", "ClassList", new { id = classId });
        }

        // GET: Classes/Delete/5
        public ActionResult Delete(int studentId,int classId)
        {
            ViewBag.CLASS_ID = db.Classes.Find(classId);
            var student = db.Students.Find(studentId);
            ViewBag.STUDENT_ID = db.Students.Find(studentId);
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int studentId, int classId)
        {
            var @class = db.Classes.Find(classId);
            var classList = db.ClassLists.FirstOrDefault(x => x.Studentid == studentId && x.Classid == classId);
            db.ClassLists.Remove(classList);           
            db.SaveChanges();
            return RedirectToAction("Index", "ClassList", new { id = classId });
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
