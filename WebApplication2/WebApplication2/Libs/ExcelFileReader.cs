using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;
using WebApplication2.Models;
using System.Runtime.InteropServices;
using Microsoft.Ajax.Utilities;
using System.Globalization;

namespace WebApplication2.Libs
{
    public class ExcelFileReader
    {
        SEP24Team3Entities db = new SEP24Team3Entities();
        private string pathToExcelFile;
        private Excel._Application excelApp;
        private Excel.Workbook workbook;
        private Excel.Worksheet worksheet;

        public ExcelFileReader(string pathToExcelFile)
        {
            this.pathToExcelFile = pathToExcelFile;
            excelApp = new Excel.Application();
            excelApp.Visible = false;
            workbook = excelApp.Workbooks.Open(pathToExcelFile);
            worksheet = workbook.Sheets["DanhSachSinhVien"];
        }



        public List<Student> readStudent()
        {
            List<Student> students = new List<Student>();
            int rowCount = worksheet.Cells.Find("*", System.Reflection.Missing.Value,
                               System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                               Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious,
                               false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row - 1;
            var FacultyDict = db.FACULTies.ToDictionary(x => x.FACULTYNAME, v => v.ID);
            //var facultyKey = FacultyDict.Keys; // Get faculty name
            //var facultyValue = FacultyDict.Values; // Get faculty id
            for (int i = 1; i <= rowCount; i++)
            {
                string StudentCode = worksheet.Cells[i + 1, "A"].Value.ToString();
                string FullName = worksheet.Cells[i + 1, "B"].Value.ToString();
                string Email = worksheet.Cells[i + 1, "C"].Value.ToString();
                string Facultyid = worksheet.Cells[i + 1, "D"].Value.ToString();
                foreach (var faculty in FacultyDict)
                {
                    if (Facultyid.Equals(faculty.Key))
                    {
                        Facultyid = faculty.Value.ToString();
                    }
                }
                students.Add(new Student
                {
                    StudentCode = StudentCode,
                    Fullname = FullName,
                    Email = Email,
                    Facultyid = int.Parse(Facultyid)
                });
            }
            Close();
            return students;
        }

        public List<Class> readClasses()
        {
            List<Class> Classes = new List<Class>();
            int rowCount = worksheet.Cells.Find("*", System.Reflection.Missing.Value,
                               System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                               Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious,
                               false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row - 1;
            //var ClassDict = db.Classes.ToDictionary(x => x.ClassName, v => v.id);
            var UserDict = db.AspNetUsers.ToDictionary(x => x.UserCode, v => v.Id);
            var FacultyDict = db.FACULTies.ToDictionary(x => x.FACULTYNAME, v => v.ID);
            var SemesterDict = db.SEMESTERs.ToDictionary(x => x.SEMESTER_CODE, v => v.ID);
            //var userKey = UserDict.Keys; // Get UserCode
            //var userValue = UserDict.Values; // Get UserId
            for (int i = 1; i <= rowCount; i++)
            {
                string ClassCode = worksheet.Cells[i + 1, "A"].Value.ToString();
                string LessonCode = worksheet.Cells[i + 1, "B"].Value.ToString();
                string ClassName = worksheet.Cells[i + 1, "C"].Value.ToString();
                string Credit = worksheet.Cells[i + 1, "D"].Value.ToString();
                string LessonType = worksheet.Cells[i + 1, "E"].Value.ToString();
                int DayLearn = int.Parse(worksheet.Cells[i + 1, "F"].Value.ToString());
                string ClassTime = worksheet.Cells[i + 1, "G"].Value.ToString();
                string Room = worksheet.Cells[i + 1, "H"].Value.ToString();
                int TotalStudent = int.Parse(worksheet.Cells[i + 1, "I"].Value.ToString());
                DateTime StartDate = DateTime.Parse(worksheet.Cells[i + 1, "J"].Value.ToString(), CultureInfo.CreateSpecificCulture("fr-FR"));
                DateTime EndDate = DateTime.Parse(worksheet.Cells[i + 1, "K"].Value.ToString(), CultureInfo.CreateSpecificCulture("fr-FR"));
                int TotalWeek = int.Parse(worksheet.Cells[i + 1, "L"].Value.ToString());
                string Scholastic = worksheet.Cells[i + 1, "M"].Value.ToString();
                string SemesterId = worksheet.Cells[i + 1, "N"].Value.ToString();
                foreach(var semester in SemesterDict)
                {
                    if (SemesterId.Equals(semester.Key))
                    {
                        SemesterId = semester.Value.ToString();
                    }
                }

                string UserCode = worksheet.Cells[i + 1, "O"].Value.ToString();                
                foreach (var user in UserDict)
                {
                    if (UserCode.Equals(user.Key))
                    {
                         UserCode = user.Value;
                    }
                }

                string Facultyid = worksheet.Cells[i + 1, "P"].Value.ToString();
                foreach (var faculty in FacultyDict)
                {
                    if (Facultyid.Equals(faculty.Key))
                    {
                        Facultyid = faculty.Value.ToString();
                    }
                }

                Classes.Add(new Class
                {
                    ClassCode = ClassCode,
                    LessonCode = LessonCode,
                    ClassName = ClassName,
                    Credit = Credit,
                    LessonType = LessonType,
                    DayLearn = DayLearn,
                    ClassTime = ClassTime,
                    Room = Room,
                    TotalStudent = TotalStudent,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    TotalWeek = TotalWeek,
                    Scholastic = Scholastic,
                    Semesterid = int.Parse(SemesterId),
                    Userid = UserCode,
                    Facultyid = int.Parse(Facultyid)
                });
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