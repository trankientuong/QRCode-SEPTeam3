using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using WebApplication2.Controllers;
using WebApplication2.Models;

namespace WebApplication2.Tests.Controllers
{
    [TestClass]
    public class AspNetRolesControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            AspNetRolesController controller = new AspNetRolesController();

            var result = controller.Index() as ViewResult;
            var model = result.Model as List<AspNetRole>;

            Assert.IsNotNull(result); //Check Index is not null
            Assert.IsNotNull(model); // Check Model is not null

            var db = new SEP24Team3Entities();
            Assert.AreEqual(db.AspNetRoles.Count(), model.Count);

        }

        [TestMethod]
        public void TestCreateG()
        {
            var controller = new AspNetRolesController();

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void TestCreateP()
        {
            var rand = new Random();
            var role = new AspNetRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = rand.Next().ToString()
            };

            var controller = new AspNetRolesController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Create(role) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var db = new SEP24Team3Entities();
                var entity = db.AspNetRoles.SingleOrDefault(s => s.Id == role.Id && s.Name == role.Name);
                Assert.IsNotNull(entity);
                Assert.AreEqual(role.Id, entity.Id);
                Assert.AreEqual(role.Name, entity.Name);
            }         
        }

        [TestMethod]
        public void TestEditG()
        {
            var controller = new AspNetRolesController();

            var result0 = controller.Edit("") as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var role = db.AspNetRoles.First();
            var result1 = controller.Edit(role.Id) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as AspNetRole;
            Assert.IsNotNull(model);
            Assert.AreEqual(role.Id, model.Id);
            Assert.AreEqual(role.Name, model.Name);
        }

        [TestMethod]
        public void TestEditP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var role = db.AspNetRoles.AsNoTracking().First();
            role.Name = rand.Next().ToString();


            var controller = new AspNetRolesController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Edit(role) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.AspNetRoles.Find(role.Id);
                Assert.IsNotNull(entity);
                Assert.AreEqual(role.Id, entity.Id);
                Assert.AreEqual(role.Name, entity.Name);

            }
        }

        [TestMethod]
        public void TestDeleteG()
        {
            var controller = new AspNetRolesController();

            var result0 = controller.Delete("") as HttpNotFoundResult;

            Assert.IsNotNull(result0);

            var db = new SEP24Team3Entities();
            var role = db.AspNetRoles.First();
            var result1 = controller.Delete(role.Id) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as AspNetRole;
            Assert.IsNotNull(model);
            Assert.AreEqual(role.Id, model.Id);
            Assert.AreEqual(role.Name, model.Name);
        }

        [TestMethod]
        public void TestDeleteP()
        {
            var rand = new Random();
            var db = new SEP24Team3Entities();
            var role = db.AspNetRoles.AsNoTracking().First();

            var controller = new AspNetRolesController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.DeleteConfirmed(role.Id) as RedirectToRouteResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.AspNetRoles.Find(role.Id);
                Assert.IsNull(entity);


            }
        }

        [TestMethod]
        public void TestSearch()
        {
            var db = new SEP24Team3Entities();
            var role = db.AspNetRoles.AsNoTracking().First();
            var keyword = role.Id;

            var controller = new AspNetRolesController();
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result = controller.Search(keyword) as ViewResult;
                Assert.IsNotNull(result);
                var model = result.Model as List<AspNetRole>;
                Assert.IsNotNull(model);
                var entity = db.AspNetRoles.Where(x =>x.Id.Equals(keyword)).ToList();
                Assert.IsNotNull(entity);
                Assert.AreEqual(model.Count, entity.Count);
            }
        }

        [TestMethod]
        public void TestDispose()
        {
            using(var controller = new AspNetRolesController()) { }
        }
    }
}
