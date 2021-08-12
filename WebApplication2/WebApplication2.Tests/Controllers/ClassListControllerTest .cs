using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Controllers;
using WebApplication2.Models;

namespace WebApplication2.Tests.Controllers
{
    [TestClass]
    public class ClassListControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            ClassListController controller = new ClassListController();
            var db = new SEP24Team3Entities();
            var @class = db.Classes.First();

            var result = controller.Index(@class.id) as ViewResult;
            var model = result.Model as List<Class>;
            var stdInModel = model.FirstOrDefault(x => x.ClassLists.Any(g => g.Classid == @class.id)).ClassLists.Count;
            Assert.IsNotNull(result); //Check Index is not null
            Assert.IsNotNull(model); // Check Model is not null

            Assert.AreEqual(db.ClassLists.Where(x => x.Classid == @class.id).Count(), stdInModel);

        }

        [TestMethod]
        public void TestCreateG()
        {
            var controller = new ClassListController();
            var db = new SEP24Team3Entities();
            var @class = db.Classes.First();

            var result = controller.Create(@class.id) as ViewResult;

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void TestCreateP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var faculty = db.FACULTies.AsNoTracking().First();
            var @class = db.Classes.First();

            var student = new Student
            {
                StudentCode = rand.Next().ToString(),
                Fullname = rand.Next().ToString(),
                Email = rand.Next().ToString(),
                Facultyid = faculty.ID
            };

            var mockControllerContext = new Mock<ControllerContext>();
            var mockSession = new Mock<HttpSessionStateBase>();
            mockSession.SetupGet(s => s["usercode-exist"]).Returns("false");
            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);


            var controller = new ClassListController();
            controller.ControllerContext = mockControllerContext.Object;
            using (var scope = new TransactionScope())
            {
                var result = controller.Create(@class.id, student) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);


                var entity = db.ClassLists.SingleOrDefault(s => s.Studentid == student.id && s.Classid == @class.id);
                Assert.IsNotNull(entity);
                Assert.AreEqual(student.StudentCode, entity.Student.StudentCode);
                Assert.AreEqual(student.Fullname, entity.Student.Fullname);
                Assert.AreEqual(student.Email, entity.Student.Email);
            }
        }

        [TestMethod]
        public void TestEditG()
        {
            var controller = new ClassListController();
            var db = new SEP24Team3Entities();
            var classList = db.ClassLists.First();
            var result1 = controller.Edit(classList.Classid, classList.Studentid) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as Student;
            Assert.IsNotNull(model);
            Assert.AreEqual(classList.Student.StudentCode, model.StudentCode);
            Assert.AreEqual(classList.Student.Fullname, model.Fullname);
            Assert.AreEqual(classList.Student.Email, model.Email);
        }

        [TestMethod]
        public void TestEditP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var classList = db.ClassLists.AsNoTracking().First();
            var faculty = db.FACULTies.AsNoTracking().First();

            classList.Student.StudentCode = rand.Next().ToString();
            classList.Student.Fullname = rand.Next().ToString();
            classList.Student.Email = rand.Next().ToString();
            classList.Student.Facultyid = faculty.ID;

            var controller = new ClassListController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Edit(classList.Classid, classList.Student) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.ClassLists.Find(classList.Id);
                Assert.IsNotNull(entity);
                Assert.AreEqual(classList.Student.StudentCode, entity.Student.StudentCode);
                Assert.AreEqual(classList.Student.Fullname, entity.Student.Fullname);
                Assert.AreEqual(classList.Student.Email, entity.Student.Email);

            }
        }

        [TestMethod]
        public void TestDeleteG()
        {
            var controller = new ClassListController();

            var db = new SEP24Team3Entities();
            var classList = db.ClassLists.First();
            var result1 = controller.Edit(classList.Classid, classList.Studentid) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as Student;
            Assert.IsNotNull(model);
            Assert.AreEqual(classList.Student.StudentCode, model.StudentCode);
            Assert.AreEqual(classList.Student.Fullname, model.Fullname);
            Assert.AreEqual(classList.Student.Email, model.Email);
        }

        [TestMethod]
        public void TestDeleteP()
        {
            var db = new SEP24Team3Entities();
            var classList = db.ClassLists.AsNoTracking().First();

            var controller = new ClassListController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.DeleteConfirmed(classList.Studentid,classList.Classid) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.ClassLists.Find(classList.Studentid);
                Assert.IsNull(entity);


            }
        }

        [TestMethod]
        public void TestDispose()
        {
            using (var controller = new ClassListController()) { }
        }
    }
}
