using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _7alazon.Models;

namespace _7alazon.Controllers
{
    public class HotelsController : Controller
    {
        private TrivagoDBEntities db = new TrivagoDBEntities();

        // GET: Hotels
        public ActionResult Index()

        {
            if (Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            else if (Session["UserType"].ToString() == "Admin")
            {
                return View(db.Hotels.ToList());

            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // GET: Hotels/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Hotel hotel = db.Hotels.Find(id);
                if (hotel == null)
                {
                    return HttpNotFound();
                }

                return View(hotel);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }


        }

        // GET: Hotels/Create
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

        // POST: Hotels/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HotelID,HotelName,HotelAddress,HotelPhoneNumber,HotelEmail,HotelRating,Photo")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                db.Hotels.Add(hotel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel);
        }

        // GET: Hotels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Hotel hotel = db.Hotels.Find(id);
                if (hotel == null)
                {
                    return HttpNotFound();
                }
                return View(hotel);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // POST: Hotels/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HotelID,HotelName,HotelAddress,HotelPhoneNumber,HotelEmail,HotelRating,Photo")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotel);
        }

        // GET: Hotels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Hotel hotel = db.Hotels.Find(id);
                if (hotel == null)
                {
                    return HttpNotFound();
                }
                return View(hotel);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hotel hotel = db.Hotels.Find(id);
            db.Hotels.Remove(hotel);
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












    }
}
