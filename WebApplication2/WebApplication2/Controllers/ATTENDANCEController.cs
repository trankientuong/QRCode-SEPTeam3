using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using ZXing;

namespace WebApplication2.Controllers
{
    public class ATTENDANCEController : Controller
    {
        SEP24Team3Entities db = new SEP24Team3Entities();
        // GET: ATTENDANCE
        public ActionResult Index(int id)
        {
            ViewBag.CLASS_ID = db.Classes.Find(id);
            var @class = db.Classes.Find(id);
            var studentList = db.ClassLists.Where(x => x.Classid == id).ToList();
            var studentInClass = db.ClassLists.FirstOrDefault(x => x.Classid == id);
            ATTENDANCE atd = new ATTENDANCE();
            //var check = db.ATTENDANCEs.FirstOrDefault(x => x.ClassId == id); // get attendance of ClassId 
            var studentInCheck = db.ATTENDANCEs.Where(x => x.ClassId == id).ToList(); // get list of attendance of classId                                                                                                                            
            for (int i = 1; i <= @class.NoSession; i++)
            {
                foreach (var std in studentList)
                {
                    if (db.ATTENDANCEs.FirstOrDefault(x => x.StudentId == std.Studentid && x.ClassId == id && x.SessionName.Contains("Buổi " + i)) == null)
                    {
                        atd.SessionName = "Buổi " + i;
                        atd.ClassId = id;
                        atd.StudentId = std.Studentid;
                        db.ATTENDANCEs.Add(atd);
                        db.SaveChanges();
                    }
                }
            }
            return View();
        }

        public ActionResult Index2(int classId, string sessionName)
        {
            ViewBag.CLASS_ID = db.Classes.Find(classId);
            ViewBag.SESSION_NAME = db.ATTENDANCEs.FirstOrDefault(x => x.SessionName == sessionName);
            var listStudent = db.ATTENDANCEs.Where(x => x.ClassId == classId && x.SessionName.Contains(sessionName));
            return View(listStudent.ToList());
        }


        public ActionResult Index3(int classId,string studentId)
        {
            ViewBag.CLASS_ID = db.Classes.Find(classId);
            ViewBag.USER_ID = db.AspNetUsers.Find(studentId);
            var studentInfo = db.AspNetUsers.FirstOrDefault(x => x.Id == studentId);
            var atd = db.ATTENDANCEs.Where(x => x.ClassId == classId && x.Student.StudentCode.Equals(studentInfo.UserCode));     
            return View(atd.ToList());
        }

        public ActionResult AttendanceStudent(int classId, int studentId, string sessionName)
        {
            var atd = db.ATTENDANCEs.FirstOrDefault(x => x.ClassId == classId && x.StudentId == studentId && x.SessionName == sessionName);
            if (atd.STATUS == null)
            {
                atd.STATUS = true;
                atd.CreateDate = DateTime.Now;
                db.Entry(atd).State = System.Data.Entity.EntityState.Modified;
            }
            else if (atd.STATUS == false)
            {
                atd.STATUS = true;
                atd.CreateDate = DateTime.Now;
                db.Entry(atd).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                atd.STATUS = false;
                atd.CreateDate = null;
                db.Entry(atd).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Index2", "ATTENDANCE", new { classId = classId, sessionName = sessionName });
        }

        public ActionResult showQRCode(string sessionName,int classId)
        {
            ViewBag.CLASS_ID = db.Classes.Find(classId);
            ViewBag.SESSION_NAME = db.ATTENDANCEs.FirstOrDefault(x => x.SessionName == sessionName);
            var listStudent = db.ATTENDANCEs.Where(x => x.ClassId == classId && x.SessionName.Contains(sessionName));
            return View(listStudent.ToList());
        }

        [HttpPost]
        public ActionResult Generate(ATTENDANCE atd)
        {
            try
            {
                atd.QRCodeImagePath = GenerateQRCode(atd.SessionName, (int)atd.ClassId, atd.QRCodeText);
                ViewBag.Message = "QR Code Created successfully";
            }
            catch (Exception ex)
            {
                //catch exception if there is any                
            }
            return RedirectToAction("showQRCode", "ATTENDANCE", new { classId = atd.ClassId, sessionName = atd.SessionName });
        }

        private string GenerateQRCode(string sessionName, int classId, string qrcodeText)
        {
            //var atd = db.ATTENDANCEs.FirstOrDefault(x => x.SessionName == sessionName && x.ClassId == classId);
            var atd = db.ATTENDANCEs.Where(x => x.SessionName == sessionName && x.ClassId == classId).ToList();
            string folderPath = "~/Images/";
            string imagePath = "~/Images/QrCode.jpg";
            foreach (var item in atd)
            {
                item.QRCodeText = qrcodeText;
                item.QRCodeImagePath = imagePath;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                // If the directory doesn't exist then create it.
                if (!Directory.Exists(Server.MapPath(folderPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(folderPath));
                }

                var barcodeWriter = new BarcodeWriter();
                barcodeWriter.Format = BarcodeFormat.QR_CODE;
                var result = barcodeWriter.Write(item.QRCodeText);

                string barcodePath = Server.MapPath(item.QRCodeImagePath);
                var barcodeBitmap = new Bitmap(result);
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            //atd.QRCodeText = qrcodeText;

            //atd.QRCodeImagePath = imagePath;
        
            return imagePath;
        }     

        public ActionResult RefreshQRCode(string sessionName,int classId,string qrcodeText)
        {
            string newQRCodeText = Guid.NewGuid().ToString();
            var atd = db.ATTENDANCEs.Where(x => x.SessionName == sessionName && x.ClassId == classId && x.QRCodeText == qrcodeText).ToList();
            foreach(var item in atd)
            {
                item.QRCodeText = newQRCodeText;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;                
            }
            db.SaveChanges();
            return RedirectToAction("Index2", "ATTENDANCE", new { classId = classId, sessionName = sessionName });
        }

        public ActionResult ReadQRCode(string qrcodeText,string studentId)
        {
            var studentInfo = db.AspNetUsers.FirstOrDefault(x => x.Id == studentId);
            var atd = db.ATTENDANCEs.FirstOrDefault(x => x.QRCodeText == qrcodeText && x.Student.StudentCode.Equals(studentInfo.UserCode));
            atd.STATUS = true;
            atd.CreateDate = DateTime.Now;
            db.Entry(atd).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        
    }
}