using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication2.Controllers;
using WebApplication2.Models;

namespace WebApplication2.Tests.Controllers
{
    [TestClass]
    public class ATTENDANCEControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            ATTENDANCEController controller = new ATTENDANCEController();
            var db = new SEP24Team3Entities();
            var @class = db.Classes.First();

            var result = controller.Index(@class.id) as ViewResult;
            Assert.IsNotNull(result); //Check Index is not null                      
        }

        [TestMethod]
        public void TestIndex2()
        {
            ATTENDANCEController controller = new ATTENDANCEController();
            var db = new SEP24Team3Entities();
            var @class = db.Classes.First();
            var sessionNameOfClass = db.ATTENDANCEs.First(x => x.ClassId == @class.id);
            var result = controller.Index2(@class.id,sessionNameOfClass.SessionName) as ViewResult;
            var model = result.Model as List<ATTENDANCE>;
            Assert.IsNotNull(result); //Check Index is not null              
            Assert.AreEqual(sessionNameOfClass.Class.ClassLists.Count, model.Count);
        }

        [TestMethod]
        public void TestIndex3()
        {
            ATTENDANCEController controller = new ATTENDANCEController();
            var db = new SEP24Team3Entities();
            var student = db.AspNetUsers.First();
            var @class = db.Classes.First();
            var atd = db.ATTENDANCEs.Where(x => x.ClassId == @class.id && x.Student.StudentCode == student.UserCode).ToList();
            var result = controller.Index3(@class.id,student.Id) as ViewResult;
            var model = result.Model as List<ATTENDANCE>;
            Assert.IsNotNull(result); //Check Index is not null          
            Assert.AreEqual(atd.Count, model.Count);
        }
    }
}
