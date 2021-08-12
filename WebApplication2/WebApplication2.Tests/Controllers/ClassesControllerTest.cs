using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using WebApplication2.Controllers;
using WebApplication2.Models;

namespace WebApplication2.Tests.Controllers
{
    [TestClass]
    public class ClassesControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            ClassesController controller = new ClassesController();

            var result = controller.Index() as ViewResult;
            var model = result.Model as List<Class>;

            Assert.IsNotNull(result); //Check Index is not null
            Assert.IsNotNull(model); // Check Model is not null

            var db = new SEP24Team3Entities();
            Assert.AreEqual(db.Classes.Count(), model.Count);

        }

        [TestMethod]
        public void TestIndex2()
        {
            var db = new SEP24Team3Entities();
            ClassesController controller = new ClassesController();
            var user = db.AspNetUsers.First();
            var result = controller.Index2(user.Id) as ViewResult;
            var model = result.Model as List<Class>;

            Assert.IsNotNull(result);
            Assert.IsNotNull(model);



            var lecturer = db.Classes.Where(x => x.Userid.Equals(user.Id)).ToList();

            Assert.AreEqual(lecturer.Count, model.Count);
        }

        [TestMethod]
        public void TestIndex3()
        {
            var db = new SEP24Team3Entities();
            ClassesController controller = new ClassesController();
            var user = db.AspNetUsers.First();
            var result = controller.Index3(user.Id) as ViewResult;
            var model = result.Model as List<Class>;

            Assert.IsNotNull(result);
            Assert.IsNotNull(model);



            var classOfStudent = db.Classes.Where(x => x.ClassLists.Any(g => g.Student.StudentCode.Equals(user.UserCode.ToUpper())));

            Assert.AreEqual(classOfStudent.Count(), model.Count);
        }

        [TestMethod]
        public void TestCreateG()
        {
            var controller = new ClassesController();

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void TestCreateP()
        {
            var db = new SEP24Team3Entities();
            var rand = new Random();
            var lecturer = db.AspNetUsers.First(x => x.AspNetRoles.Any(g => g.Name == "Lecturer"));
            var faculty = db.FACULTies.First();
            var @class = new Class
            {
                ClassCode = rand.Next().ToString(),
                LessonCode = rand.Next().ToString(),
                ClassName = rand.Next().ToString(),
                Credit = rand.Next().ToString(),
                LessonType = rand.Next().ToString(),
                DayLearn = rand.Next(),
                ClassTime = rand.Next().ToString(),
                Room = rand.Next().ToString(),
                TotalStudent = rand.Next(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Today.AddDays(36),
                TotalWeek = rand.Next(),
                Scholastic = rand.Next().ToString(),
                Semesterid = rand.Next(1, 3),
                Userid = lecturer.Id,
                Facultyid = faculty.ID
            };

            var controller = new ClassesController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Create(@class) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);


                var entity = db.Classes.SingleOrDefault(s => s.ClassCode == @class.ClassCode && s.ClassName == @class.ClassName && s.Userid == @class.Userid);
                Assert.IsNotNull(entity);
                Assert.AreEqual(@class.ClassCode, entity.ClassCode);
                Assert.AreEqual(@class.ClassName, entity.ClassName);
                Assert.AreEqual(@class.Userid, entity.Userid);
            }
        }

        [TestMethod]
        public void TestEditG()
        {
            var controller = new ClassesController();

            var result0 = controller.Edit(0) as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var @class = db.Classes.First();
            var result1 = controller.Edit(@class.id) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as Class;
            Assert.IsNotNull(model);
            Assert.AreEqual(@class.ClassCode, model.ClassCode);
            Assert.AreEqual(@class.ClassName, model.ClassName);
            Assert.AreEqual(@class.Userid, model.Userid);
        }

        [TestMethod]
        public void TestEditP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var @class = db.Classes.AsNoTracking().First();
            var lecturer = db.AspNetUsers.First(x => x.AspNetRoles.Any(g => g.Name == "Lecturer"));
            var faculty = db.FACULTies.AsNoTracking().First();
            @class.ClassCode = rand.Next().ToString();
            @class.LessonCode = rand.Next().ToString();
            @class.ClassName = rand.Next().ToString();
            @class.Credit = rand.Next().ToString();
            @class.LessonType = rand.Next().ToString();
            @class.DayLearn = rand.Next();
            @class.ClassTime = rand.Next().ToString();
            @class.Room = rand.Next().ToString();
            @class.TotalStudent = rand.Next();
            @class.StartDate = DateTime.Now;
            @class.EndDate = DateTime.Today.AddDays(50);
            @class.TotalWeek = rand.Next();
            @class.Scholastic = rand.Next().ToString();
            @class.Semesterid = rand.Next(1, 3);
            @class.Userid = lecturer.Id;
            @class.Facultyid = faculty.ID;


            var controller = new ClassesController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Edit(@class) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.Classes.Find(@class.id);
                Assert.IsNotNull(entity);
                Assert.AreEqual(@class.ClassCode, entity.ClassCode);
                Assert.AreEqual(@class.ClassName, entity.ClassName);
                Assert.AreEqual(@class.Userid, entity.Userid);

            }
        }

        [TestMethod]
        public void TestDeleteG()
        {
            var controller = new ClassesController();

            var result0 = controller.Delete(0) as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var @class = db.Classes.First();
            var result1 = controller.Delete(@class.id) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as Class;
            Assert.IsNotNull(model);
            Assert.AreEqual(@class.ClassCode, model.ClassCode);
            Assert.AreEqual(@class.ClassName, model.ClassName);
            Assert.AreEqual(@class.Userid, model.Userid);
        }

        [TestMethod]
        public void TestDeleteP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var @class = db.Classes.AsNoTracking().First();

            var controller = new ClassesController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.DeleteConfirmed(@class.id) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.Classes.Find(@class.id);
                Assert.IsNull(entity);
            }
        }

        [TestMethod]
        public void TestDispose()
        {
            using (var controller = new ClassesController()) { }
        }
    }
}
