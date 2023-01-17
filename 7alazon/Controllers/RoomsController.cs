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
    public class RoomsController : Controller
    {
        private TrivagoDBEntities db = new TrivagoDBEntities();

        // GET: Rooms
        public ActionResult Book(int id)
        {
            var viewModel = new Booking
            {
                RoomID = id
            };
            return View(viewModel);
        }
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                var rooms = db.Rooms.Include(r => r.Hotel);
                return View(rooms.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }

        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Room room = db.Rooms.Find(id);
                if (room == null)
                {
                    return HttpNotFound();
                }
                return View(room);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                ViewBag.HotelID = new SelectList(db.Hotels, "HotelID", "HotelName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        // POST: Rooms/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomID,RoomNumber,RoomType,RoomPrice,RoomAvailability,HotelID,Photo")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HotelID = new SelectList(db.Hotels, "HotelID", "HotelName", room.HotelID);
            return View(room);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Room room = db.Rooms.Find(id);
                if (room == null)
                {
                    return HttpNotFound();
                }
                ViewBag.HotelID = new SelectList(db.Hotels, "HotelID", "HotelName", room.HotelID);
                return View(room);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }


        }

        // POST: Rooms/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomID,RoomNumber,RoomType,RoomPrice,RoomAvailability,HotelID,Photo")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HotelID = new SelectList(db.Hotels, "HotelID", "HotelName", room.HotelID);
            return View(room);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Room room = db.Rooms.Find(id);
                if (room == null)
                {
                    return HttpNotFound();
                }
                return View(room);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }


        }


        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
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
