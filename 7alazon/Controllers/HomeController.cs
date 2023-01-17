using _7alazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _7alazon.Controllers
{
    public class HomeController : Controller
    {
        private TrivagoDBEntities db = new TrivagoDBEntities();

        public ActionResult Details(int? id)
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



        public ActionResult RoomDetails(int? id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room rooms = db.Rooms.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            return View(rooms);






        }
        public ActionResult Rooms()
        {
            using (var context = new TrivagoDBEntities())
            {
                var rooms = context.Rooms.ToList();
                return View(rooms);
            }
        }

        public ActionResult Index()
        {
            using (var context = new TrivagoDBEntities())
            {
                var hotels = context.Hotels.ToList();
                return View(hotels);
            }
        }

        public ActionResult ShowRooms(int? id)
        {
            using (var context = new TrivagoDBEntities())
            {
                var rooms = context.Rooms.Where(r => r.HotelID == id).ToList();
                return View(rooms);
            }
        }

        public ActionResult BookRoom(string username, int roomId, DateTime checkIn, DateTime checkOut)
        {
            if (Session["UserType"] != null)
            {
                using (var context = new TrivagoDBEntities())
                {
                    // kat takhed linfo tl users

                    var user = context.Users.FirstOrDefault(u => u.UserName == username);
                    if (user == null)
                    {
                        // ila makanch user
                        return View();
                    }

                    // creati booking jdid
                    if (checkIn < checkOut)
                    {
                        var booking = new Booking
                        {
                            UserID = user.UserID,
                            RoomID = roomId,
                            CheckIn = checkIn,
                            CheckOut = checkOut,
                            Etat = "Booked"
                        };
                        var room = context.Rooms.FirstOrDefault(r => r.RoomID == roomId);
                        if (room != null)
                        {
                            room.RoomAvailability = true;
                            context.SaveChanges();
                        }
                        // kat zid dak lbooking ldatabaz
                        context.Bookings.Add(booking);
                        context.SaveChanges();
                        TempData["SuccessMessage"] = "Booked Successfully";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "checkin cant be less than checkout.";
                        return RedirectToAction("RoomDetails", "Home", new { id = roomId }); 
                    }
                }
                return RedirectToAction("RoomDetails", "Home", new { id = roomId }); 
            }
            else
            {
                TempData["ErrorMessage"] = "Login to book!";

                return RedirectToAction("Login", "Users");
            }
        }


        public ActionResult ShowBookedRooms()
        {
            var username = HttpContext.Session["UserName"].ToString();
            if (username != null)
            {
                using (var context = new TrivagoDBEntities())
                {
                    var user = context.Users.FirstOrDefault(u => u.UserName == username);
                    if (user != null)
                    {
                        var bookedRooms = context.Bookings.Where(b => b.UserID == user.UserID).ToList();
                        return View(bookedRooms);
                    }
                }
            }
            return RedirectToAction("Login", "Users");
        }
        public ActionResult Unbook(int? roomid)
        {
            // Find the booking by ID
            var context = new TrivagoDBEntities();
            var booking = context.Bookings.FirstOrDefault(r => r.RoomID == roomid);
            if (booking == null)
            {
                // Return a "Booking not found" message if the booking doesn't exist
                return RedirectToAction("RoomDetails", "Home", new { id = roomid });
            }
            else
            {
                // Set the room availability to true
                var room = context.Rooms.FirstOrDefault(r => r.RoomID == roomid);
                room.RoomAvailability = false;
                context.SaveChanges();

                // Remove the booking from the database
                context.Bookings.Remove(booking);
                context.SaveChanges();

                // Return a success message
                return RedirectToAction("RoomDetails", "Home", new { id = roomid });
            }
        }

        [HttpPost]

        public ActionResult CreateComment(int hotelId, string userName, string comment)
        {
            var context = new TrivagoDBEntities();
            // Check if the user exists in the database
            var user = context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                // Returni message user makinch
                TempData["ErrorMessage"] = "You should login.";
                return RedirectToAction("Login", "Users");

            }
            else { 
            // Creati comment jdid
            var newComment = new Comment
            {
                UserID = user.UserID,
                HotelID = hotelId,
                Comment1 = comment,
                CommentDate = DateTime.Now
            };

            // Add l comment ldatabase
            context.Comments.Add(newComment);
            context.SaveChanges();

                // Redirect l user lhotel details
                return RedirectToAction("Details","Home", new { id = hotelId }); ;}


        }




            public ActionResult About()
            {
                ViewBag.Message = "Your application description page.";

                return View();
            }

            public ActionResult Contact()
            {
                ViewBag.Message = "Your contact page.";

                return View();
            }
        }
    }