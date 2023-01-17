using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using _7alazon.Models;

namespace _7alazon.Controllers
{

    public class UsersController : Controller
    {
        private TrivagoDBEntities db = new TrivagoDBEntities();

        // GET: Users
 
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Client") 
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                using (var context = new TrivagoDBEntities())
                {
                    var users = context.Users.ToList();
                    return View(users);
                }
            }
           
        }

        // GET: Users/Details/5
      
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Users/Create
        
        public ActionResult Create()
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Users/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "UserID,FirstName,LastName,UserName,UserPassword,ConfirmationPassword,PhoneNumber,Email,UserType")] User user)
        {

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
      
        public ActionResult Edit(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // POST: Users/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,FirstName,LastName,UserName,UserPassword,ConfirmationPassword,PhoneNumber,Email,UserType")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5

        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]

       /* public ActionResult Login(User user)
        {
            using (var db = new TrivagoDBEntities())
            {
                var checkUser = db.Users.Where(u => u.UserName == user.UserName && u.UserPassword == user.UserPassword).FirstOrDefault();
                if (checkUser != null)
                {
                    if (checkUser.UserType == "Admin")
                    {
                        Session["UserId"] = checkUser.UserID;
                        Session["UserName"] = checkUser.UserName;
                        Session["UserType"] = checkUser.UserType;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Session["UserId"] = checkUser.UserID;
                        Session["UserName"] = checkUser.UserName;
                        Session["UserType"] = checkUser.UserType;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login details");
                    return View();
                }
            }
        }*/


        public ActionResult Login(User user)
        {
            using (var context = new TrivagoDBEntities())
            {
                var userInDb = context.Users.FirstOrDefault(u => u.UserName == user.UserName && u.UserPassword == user.UserPassword);
                if (userInDb != null)
                {
                    FormsAuthentication.SetAuthCookie(userInDb.UserName, false);
                    if (userInDb.UserType == "Admin")
                    {
                        Session["UserId"] = userInDb.UserID.ToString();
                        Session["UserName"] = userInDb.UserName.ToString();
                        Session["UserType"] = userInDb.UserType.ToString();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Session["UserId"] = userInDb.UserID.ToString();
                        Session["UserName"] = userInDb.UserName.ToString();
                        Session["UserType"] = userInDb.UserType.ToString();
                        return RedirectToAction("Index", "Home");
                    }

                }
                
                else
                {
                    ModelState.AddModelError("", "User does not exist.");
                    return View();
                }
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "UserID,FirstName,LastName,UserName,UserPassword,ConfirmationPassword,PhoneNumber,Email,UserType")] User user)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(x => x.UserName == user.UserName))
                {
                    ModelState.AddModelError("", "Username already exists.");
                    return View();
                }
                else
                {
                    user.UserType = "Client";
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Users");
                }
            }

            return View(user);
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }



    }
}
        















    

