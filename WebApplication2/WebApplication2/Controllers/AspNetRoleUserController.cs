using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{

    //[Authorize(Roles = "Admin,Faculty")]
    public class AspNetRoleUserController : Controller
    {
        private SEP24Team3Entities db = new SEP24Team3Entities();

        // GET: AspNetRoles/Create
        public ActionResult Create(string roleId)
        {
            ViewBag.Role = db.AspNetRoles.Find(roleId);
            var userInRole = db.AspNetUsers.Where(x => x.AspNetRoles.Any(g => g.Id == roleId)); // Find user exists in roleId
            var userNotInRole = db.AspNetUsers.Except(userInRole); // Get user not in userInRole
            ViewBag.Users = new SelectList(userNotInRole, "Id", "UserName");
            return View();
        }

        // POST: AspNetRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string roleId, string userId)
        {
            var role = db.AspNetRoles.Find(roleId);
            var user = db.AspNetUsers.Find(userId);
            role.AspNetUsers.Add(user);
            db.Entry(role).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "AspNetRoles");
        }


        [HttpGet]
        public ActionResult Edit(string userId)
        {
            ViewBag.User = db.AspNetUsers.Find(userId);
            ViewBag.Role = new SelectList(db.AspNetRoles, "Id", "Name");
            return View();
        }


        [HttpPost]
        public ActionResult Edit(string roleId, string userId)
        {
            var user = db.AspNetUsers.Find(userId);
            var role = db.AspNetRoles.Find(roleId);
            role.AspNetUsers.Add(user);
            db.Entry(role).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "AspNetRoles");
        }

        // GET: AspNetRoles/Delete/5
        public ActionResult Delete(string roleId, string userId)
        {
            var role = db.AspNetRoles.Find(roleId);
            var user = db.AspNetUsers.Find(userId);
            role.AspNetUsers.Remove(user);
            db.Entry(role).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "AspNetRoles");
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
