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
    public class BookingsController : Controller
    {
        private TrivagoDBEntities db = new TrivagoDBEntities();

        // GET: Bookings
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                var bookings = db.Bookings.Include(b => b.Room).Include(b => b.User);
                return View(bookings.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Booking booking = db.Bookings.Find(id);
                if (booking == null)
                {
                    return HttpNotFound();
                }
                return View(booking);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "RoomNumber");
                ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // POST: Bookings/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,UserID,RoomID,CheckIn,CheckOut,Etat")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "RoomNumber", booking.RoomID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", booking.UserID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)

        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Booking booking = db.Bookings.Find(id);
                if (booking == null)
                {
                    return HttpNotFound();
                }
                ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "RoomNumber", booking.RoomID);
                ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", booking.UserID);
                return View(booking);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        // POST: Bookings/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,UserID,RoomID,CheckIn,CheckOut,Etat")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "RoomNumber", booking.RoomID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FirstName", booking.UserID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {

            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Booking booking = db.Bookings.Find(id);
                if (booking == null)
                {
                    return HttpNotFound();
                }
                return View(booking);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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
