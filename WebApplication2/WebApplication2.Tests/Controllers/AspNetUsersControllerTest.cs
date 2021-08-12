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
    public class AspNetUsersControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            AspNetUsersController controller = new AspNetUsersController();

            var result = controller.Index() as ViewResult;
            var model = result.Model as List<AspNetUser>;

            Assert.IsNotNull(result); //Check Index is not null
            Assert.IsNotNull(model); // Check Model is not null

            var db = new SEP24Team3Entities();
            Assert.AreEqual(db.AspNetUsers.Count(), model.Count);

        }

        [TestMethod]
        public void TestCreateG()
        {
            var controller = new AspNetUsersController();

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void TestEdit2G()
        {
            var controller = new AspNetUsersController();

            var result0 = controller.Edit2("") as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var user = db.AspNetUsers.First();
            var result1 = controller.Edit2(user.Id) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as AspNetUser;
            Assert.IsNotNull(model);
            Assert.AreEqual(user.UserCode, model.UserCode);
            Assert.AreEqual(user.FullName, model.FullName);
            Assert.AreEqual(user.Degree, model.Degree);
        }


        [TestMethod]
        public void TestEditG()
        {
            var controller = new AspNetUsersController();

            var result0 = controller.Edit("") as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var user = db.AspNetUsers.First();
            var result1 = controller.Edit(user.Id) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as AspNetUser;
            Assert.IsNotNull(model);
            Assert.AreEqual(user.UserCode, model.UserCode);
            Assert.AreEqual(user.FullName, model.FullName);
            Assert.AreEqual(user.Degree, model.Degree);
        }

        [TestMethod]
        public void TestEditP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var user = db.AspNetUsers.AsNoTracking().First();

            user.UserCode = rand.Next().ToString();
            user.FullName = rand.Next().ToString();
            user.Degree = rand.Next().ToString();


            var controller = new AspNetUsersController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Edit(user) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.AspNetUsers.Find(user.Id);
                Assert.IsNotNull(entity);
                Assert.AreEqual(user.UserCode, entity.UserCode);
                Assert.AreEqual(user.FullName, entity.FullName);
                Assert.AreEqual(user.Degree, entity.Degree);
            }
        }

        [TestMethod]
        public void TestEdit2P()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var user = db.AspNetUsers.AsNoTracking().First();

            user.UserCode = rand.Next().ToString();
            user.FullName = rand.Next().ToString();
            user.Degree = rand.Next().ToString();

            var mockControllerContext = new Mock<ControllerContext>();
            var mockSession = new Mock<HttpSessionStateBase>();
            mockSession.SetupGet(s => s["usercode-exist"]).Returns("true");
            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);

            var controller = new AspNetUsersController();
            controller.ControllerContext = mockControllerContext.Object;
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Edit2(user) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.AspNetUsers.Find(user.Id);
                Assert.IsNotNull(entity);
                Assert.AreEqual(user.UserCode, entity.UserCode);
                Assert.AreEqual(user.FullName, entity.FullName);
                Assert.AreEqual(user.Degree, entity.Degree);
            }
        }

        [TestMethod]
        public void TestDeleteG()
        {
            var controller = new AspNetUsersController();

            var result0 = controller.Delete("") as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var user = db.AspNetUsers.First();
            var result1 = controller.Delete(user.Id) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as AspNetUser;
            Assert.IsNotNull(model);
            Assert.AreEqual(user.UserCode, model.UserCode);
            Assert.AreEqual(user.FullName, model.FullName);
            Assert.AreEqual(user.Degree, model.Degree);
        }

        [TestMethod]
        public void TestDeleteP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var user = db.AspNetUsers.FirstOrDefault(x => x.Email.Contains("quang.187pm06696@vanlanguni.vn"));

            var controller = new AspNetUsersController();
            using (var scope = new TransactionScope())
            {
                var result = controller.DeleteConfirmed(user.Id) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

            }
        }

        [TestMethod]
        public void TestDispose()
        {
            using(var controller = new AspNetUsersController()) { }
        }
    }
}
