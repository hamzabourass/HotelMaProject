using _7alazon.Models;
using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _7alazon.Controllers
{
    public class HomeController : Controller
    {
        public partial class MLModel1
        {
            /// <summary>
            /// model input class for MLModel1.
            /// </summary>
            #region model input class
            public class ModelInput
            {
                [ColumnName(@"score")]
                public float Score { get; set; }

                [ColumnName(@"new_reviews")]
                public string New_reviews { get; set; }

            }

            #endregion

            /// <summary>
            /// model output class for MLModel1.
            /// </summary>
            #region model output class
            public class ModelOutput
            {
                [ColumnName("PredictedLabel")]
                public float Prediction { get; set; }

                public float[] Score { get; set; }
            }

            #endregion

            private static string MLNetModelPath = Path.GetFullPath("C:\\Users\\MAJD\\source\\repos\\7alazon\\7alazon\\MLModel1.zip");

            public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

            /// <summary>
            /// Use this method to predict on <see cref="ModelInput"/>.
            /// </summary>
            /// <param name="input">model input.</param>
            /// <returns><seealso cref=" ModelOutput"/></returns>
            public static ModelOutput Predict(ModelInput input)
            {
                var predEngine = PredictEngine.Value;
                return predEngine.Predict(input);
            }

            private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
            {
                var mlContext = new MLContext();
                ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
                return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
            }
        }
        public partial class MLModel1
        {
            public static ITransformer RetrainPipeline(MLContext context, IDataView trainData)
            {
                var pipeline = BuildPipeline(context);
                var model = pipeline.Fit(trainData);

                return model;
            }

            /// <summary>
            /// build the pipeline that is used from model builder. Use this function to retrain model.
            /// </summary>
            /// <param name="mlContext"></param>
            /// <returns></returns>
            public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
            {
                // Data process configuration with pipeline data transformations
                var pipeline = mlContext.Transforms.Text.FeaturizeText(@"new_reviews", @"new_reviews")
                                        .Append(mlContext.Transforms.Concatenate(@"Features", @"new_reviews"))
                                        .Append(mlContext.Transforms.Conversion.MapValueToKey(@"score", @"score"))
                                        .Append(mlContext.Transforms.NormalizeMinMax(@"Features", @"Features"))
                                        .Append(mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(l1Regularization: 1F, l2Regularization: 1F, labelColumnName: @"score", featureColumnName: @"Features"), labelColumnName: @"score"))
                                        .Append(mlContext.Transforms.Conversion.MapKeyToValue(@"PredictedLabel", @"PredictedLabel"));

                return pipeline;
            }
        }

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
                TempData["ErrorMessage"] = "LogIn To Book!";

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

        

        /*  public ActionResult CreateComment(int hotelId, string userName, string comment)
          {

              var context = new TrivagoDBEntities();
              // Check if the user exists in the database
              var user = context.Users.FirstOrDefault(u => u.UserName == userName);
              if (user == null)
              {
                  // Returni message user makinch
                  TempData["ErrorMessage"] = "You Should Login.";
                  return RedirectToAction("Login", "Users");
              }
              else
                   {
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
                  return RedirectToAction("Details", "Home", new { id = hotelId }); 
              }


          }*/
      /*  public float PredictComment(MLModel1.ModelInput input)
        {
            return MLModel1.Predict(input).Prediction;
        }*/
        [HttpPost]
        public ActionResult CreateComment(int hotelId, string userName, MLModel1.ModelInput input)
        {

            var context = new TrivagoDBEntities();
            // Check if the user exists in the database
            var user = context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                // Returni message user makinch
                TempData["ErrorMessage"] = "You Should Login.";
                return RedirectToAction("Login", "Users");
            }
            else
            {
               int score = (int)MLModel1.Predict(input).Prediction;

                // Creati comment jdid
                if(score == 1) { 
                var newComment = new Comment
                {
                    UserID = user.UserID,
                    HotelID = hotelId,
                    Comment1 = input.New_reviews + "*-Positif-*",
                    CommentDate = DateTime.Now
                };
                

                // Add l comment ldatabase
                context.Comments.Add(newComment);
                context.SaveChanges();
                }
                if (score == 0)
                {
                    var newComment = new Comment
                    {
                        UserID = user.UserID,
                        HotelID = hotelId,
                        Comment1 = input.New_reviews + "*-Neutre-*",
                        CommentDate = DateTime.Now
                    };


                    // Add l comment ldatabase
                    context.Comments.Add(newComment);
                    context.SaveChanges();
                }
                if (score == -1)
                {
                    var newComment = new Comment
                    {
                        UserID = user.UserID,
                        HotelID = hotelId,
                        Comment1 = input.New_reviews + "*-Negatif-*",
                        CommentDate = DateTime.Now
                    };


                    // Add l comment ldatabase
                    context.Comments.Add(newComment);
                    context.SaveChanges();
                }

                // Redirect l user lhotel details
                return RedirectToAction("Details", "Home", new { id = hotelId });
            }


        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
       /* public ActionResult idH(int id)
        {
            var context = new TrivagoDBEntities();
            var idh = context.Hotels.FirstOrDefault(u => u.HotelID == id);
            

            return RedirectToAction("prediction", "Home");
        }*/
      public ActionResult prediction(  MLModel1.ModelInput input)
        {
           // var context = new TrivagoDBEntities();
            

           //var user = context.Users.FirstOrDefault(u => u.UserName == userName);

            ViewBag.Result = "";
            var prediction = MLModel1.Predict(input);
            ViewBag.Result = prediction;
            ViewData["new_reviews"] = input.New_reviews;
           /* var newComment = new Comment
            {
                UserID = user.UserID,
                HotelID = 1,
                Comment1 = input.New_reviews,
                CommentDate = DateTime.Now
            };
            // Add l comment ldatabase
            context.Comments.Add(newComment);
            context.SaveChanges();*/
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}