using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;
using WebApplication2.Models;
using System.Runtime.InteropServices;
using Microsoft.Ajax.Utilities;
using System.Globalization;
using System.Data.Entity.Validation;

namespace WebApplication2.Libs
{
    public class ExcelFileReaderClasses
    {
        SEP24Team3Entities db = new SEP24Team3Entities();
        private string pathToExcelFile;
        private Excel._Application excelApp;
        private Excel.Workbook workbook;
        private Excel.Worksheet worksheet;

        public ExcelFileReaderClasses(string pathToExcelFile)
        {
            this.pathToExcelFile = pathToExcelFile;
            excelApp = new Excel.Application();
            excelApp.Visible = false;
            workbook = excelApp.Workbooks.Open(pathToExcelFile);
            worksheet = workbook.Sheets["DanhSachLop"];
        }


        public List<Class> readClasses()
        {
            List<Class> Classes = new List<Class>();
            int rowCount = worksheet.Cells.Find("*", System.Reflection.Missing.Value,
                               System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                               Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious,
                               false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row - 1;
            var FacultyDict = db.FACULTies.ToDictionary(x => x.FACULTYNAME, v => v.ID);
            var SemesterDict = db.SEMESTERs.ToDictionary(x => x.SEMESTER_CODE, v => v.ID);
            for (int i = 1; i <= rowCount; i++)
            {
                var ClassCode = worksheet.Cells[i + 1, "A"].Value;
                var LessonCode = worksheet.Cells[i + 1, "B"].Value;
                var ClassName = worksheet.Cells[i + 1, "C"].Value;
                var Credit = worksheet.Cells[i + 1, "D"].Value;
                var LessonType = worksheet.Cells[i + 1, "E"].Value;
                var DayLearn = worksheet.Cells[i + 1, "F"].Value;
                var ClassTime = worksheet.Cells[i + 1, "G"].Value;
                var Room = worksheet.Cells[i + 1, "H"].Value;
                var TotalStudent = worksheet.Cells[i + 1, "I"].Value;
                var StartDate = worksheet.Cells[i + 1, "J"].Value;
                var EndDate = worksheet.Cells[i + 1, "K"].Value;
                var TotalWeek = worksheet.Cells[i + 1, "L"].Value;
                var Scholastic = worksheet.Cells[i + 1, "M"].Value;
                var SemesterId = worksheet.Cells[i + 1, "N"].Value;
                foreach (var semester in SemesterDict)
                {
                    if (Convert.ToString(SemesterId).Equals(semester.Key))
                    {
                        SemesterId = semester.Value.ToString();
                    }
                }
                string UserCode = worksheet.Cells[i + 1, "O"].Value;
                var userList = db.AspNetUsers.Where(x => x.UserCode.ToUpper() == UserCode.ToUpper()).ToList();
                foreach (var user in userList)
                {
                    UserCode = user.Id;
                }
                string Facultyid = worksheet.Cells[i + 1, "P"].Value;
                foreach (var faculty in FacultyDict)
                {
                    if (Facultyid.Equals(faculty.Key))
                    {
                        Facultyid = faculty.Value.ToString();
                    }
                }
                var NoSession = worksheet.Cells[i + 1, "Q"].Value;

                if (ClassCode == "" || ClassCode == null) ClassCode = "";
                if (LessonCode == "" || LessonCode == null) LessonCode = "";
                if (ClassName == "" || ClassName == null) ClassName = "";
                if (Convert.ToString(Credit) == "" || Credit == null) Credit = "";
                if (LessonType == "" || LessonType == null) LessonType = "";
                if (Convert.ToString(DayLearn) == "" || DayLearn == null) DayLearn = null;
                if (ClassTime == "" || ClassTime == null) ClassTime = "";
                if (Room == "" || Room == null) Room = "";
                if (Convert.ToString(TotalStudent) == "" || TotalStudent == null) TotalStudent = null;
                if (Convert.ToString(StartDate) == "" || StartDate == null) StartDate = null;
                if (Convert.ToString(EndDate) == "" || EndDate == null) EndDate = null;
                if (Convert.ToString(TotalWeek) == "" || TotalWeek == null) TotalWeek = null;
                if (Scholastic == "" || Scholastic == null) Scholastic = "";
                if (Convert.ToString(SemesterId) == "" || SemesterId == null) SemesterId = null;
                if (UserCode == "" || UserCode == null) UserCode = "";
                if (Convert.ToString(Facultyid) == "" || Facultyid == null) Facultyid = null;
                if (Convert.ToString(NoSession) == "" || NoSession == null) NoSession = null;

                try
                {
                    Classes.Add(new Class
                    {
                        ClassCode = Convert.ToString(ClassCode),
                        LessonCode = Convert.ToString(LessonCode),
                        ClassName = Convert.ToString(ClassName),
                        Credit = Convert.ToString(Credit),
                        LessonType = Convert.ToString(LessonType),
                        DayLearn = Convert.ToInt32(DayLearn),
                        ClassTime = Convert.ToString(ClassTime),
                        Room = Convert.ToString(Room),
                        TotalStudent = Convert.ToInt32(TotalStudent),
                        StartDate = Convert.ToDateTime(StartDate, CultureInfo.CreateSpecificCulture("fr-FR")),
                        EndDate = Convert.ToDateTime(EndDate, CultureInfo.CreateSpecificCulture("fr-FR")),
                        TotalWeek = Convert.ToInt32(TotalWeek),
                        Scholastic = Convert.ToString(Scholastic),
                        Semesterid = Convert.ToInt32(SemesterId),
                        Userid = Convert.ToString(UserCode),
                        Facultyid = Convert.ToInt32(Facultyid),
                        NoSession = Convert.ToInt32(NoSession)
                    });
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            System.Web.HttpContext.Current.Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }


            }
            Close();
            return Classes;
        }

        private void Close()
        {
            workbook.Save();
            workbook.Close(0);
            excelApp.Quit();
            Marshal.ReleaseComObject(worksheet);
            Marshal.ReleaseComObject(workbook);
            Marshal.ReleaseComObject(excelApp);
        }
    }
}