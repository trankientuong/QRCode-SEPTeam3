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
    public class SEMESTERsControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            SEMESTERController controller = new SEMESTERController();

            var result = controller.Index() as ViewResult;
            var model = result.Model as List<SEMESTER>;

            Assert.IsNotNull(result); //Check Index is not null
            Assert.IsNotNull(model); // Check Model is not null

            var db = new SEP24Team3Entities();
            Assert.AreEqual(db.SEMESTERs.Count(), model.Count);

        }

        [TestMethod]
        public void TestCreateG()
        {
            var controller = new SEMESTERController();

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void TestCreateP()
        {
            var rand = new Random();
            var semester = new SEMESTER
            {
                SEMESTERNAME = rand.Next().ToString(),
                SEMESTERYEAR = rand.Next().ToString(),
                SEMESTER_CODE = rand.Next().ToString()
            };

            var controller = new SEMESTERController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Create(semester) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var db = new SEP24Team3Entities();
                var entity = db.SEMESTERs.SingleOrDefault(s => s.SEMESTERNAME == semester.SEMESTERNAME && s.SEMESTERYEAR == semester.SEMESTERYEAR && s.SEMESTER_CODE == semester.SEMESTER_CODE);
                Assert.IsNotNull(entity);
                Assert.AreEqual(semester.SEMESTER_CODE, entity.SEMESTER_CODE);

            }
        }

        [TestMethod]
        public void TestEditG()
        {
            var controller = new SEMESTERController();

            var result0 = controller.Edit(0) as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var semester = db.SEMESTERs.First();
            var result1 = controller.Edit(semester.ID) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as SEMESTER;
            Assert.IsNotNull(model);
            Assert.AreEqual(semester.SEMESTERNAME, model.SEMESTERNAME);
            Assert.AreEqual(semester.SEMESTERYEAR, model.SEMESTERYEAR);
            Assert.AreEqual(semester.SEMESTER_CODE, model.SEMESTER_CODE);
        }

        [TestMethod]
        public void TestEditP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var semester = db.SEMESTERs.AsNoTracking().First();

            semester.SEMESTERNAME = rand.Next().ToString();
            semester.SEMESTERYEAR = rand.Next().ToString();
            semester.SEMESTER_CODE = rand.Next().ToString();


            var controller = new SEMESTERController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Edit(semester) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.SEMESTERs.Find(semester.ID);
                Assert.IsNotNull(entity);
                Assert.AreEqual(semester.SEMESTERNAME, entity.SEMESTERNAME);
                Assert.AreEqual(semester.SEMESTERYEAR, entity.SEMESTERYEAR);
                Assert.AreEqual(semester.SEMESTER_CODE, entity.SEMESTER_CODE);

            }
        }

        [TestMethod]
        public void TestDeleteG()
        {
            var controller = new SEMESTERController();

            var result0 = controller.Delete(0) as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var semester = db.SEMESTERs.First();
            var result1 = controller.Delete(semester.ID) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as SEMESTER;
            Assert.IsNotNull(model);
            Assert.AreEqual(semester.SEMESTERNAME, model.SEMESTERNAME);
            Assert.AreEqual(semester.SEMESTERYEAR, model.SEMESTERYEAR);
            Assert.AreEqual(semester.SEMESTER_CODE, model.SEMESTER_CODE);
        }

        [TestMethod]
        public void TestDeleteP()
        {
            var db = new SEP24Team3Entities();
            var semester = db.SEMESTERs.AsNoTracking().First();

            var controller = new SEMESTERController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.DeleteConfirmed(semester.ID) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.SEMESTERs.Find(semester.ID);
                Assert.IsNull(entity);
            }
        }

        [TestMethod]
        public void TestDispose()
        {
            using (var controller = new SEMESTERController()) { }
        }
    }
}
