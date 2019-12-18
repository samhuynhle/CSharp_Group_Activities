using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using qwerty.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

/*

This is the controller section, the "C", in an MVC framework. Here we tell the application to be listening
to HTTP Requests and have appropriate HTTP Responses. We've imported the Models into this file.

*/

namespace qwerty.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController (MyContext context)
        {
            dbContext = context;
        }

        // Landing Page will process the Index.cshtml file, where the login and reg form is
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // preventing users from doing HTTP Get request for /login
        [HttpGet("login")]
        public IActionResult deadLogin()
        {
            return RedirectToAction("Index");
        }

        // preventing users from doing HTTP Get request for /register
        [HttpGet("register")]
        public IActionResult deadRegistration()
        {
            return RedirectToAction("Index");
        }

        // HTTP Post route to handle registering users who submitted registration form
        [HttpPost("register")]
        public IActionResult Register(IndexViewModel modelData) // IndexViewModel is the wrapper model that can be User, LoginUser, Activites objects.
        {
            if(modelData == null) // Must have data in modelData to register
            {
                return View("Index");
            }

            // initialize the datat into a User object
            User submittedUser = modelData.newUser;

            // we check if the ModelState is valid, if it isn't we can upload the page with any errors
            if(ModelState.IsValid)
            {
                // We need to check if there is an email already in our database, entity query
                if(dbContext.Users.Any(u => u.Email == submittedUser.Email))
                {
                    ModelState.AddModelError("newUser.Email", "Email is already in use");
                    return View("Index");
                }

                // If we pass the email check, then we can add the user to the database
                dbContext.Users.Add(submittedUser);
                dbContext.SaveChanges();

                // We must hash the password inputted as well
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                submittedUser.Password = Hasher.HashPassword(submittedUser, submittedUser.Password);
                dbContext.SaveChanges();

                // Now we need to set our session data to store user data
                User current_user = dbContext.Users.FirstOrDefault(u => u.Email == submittedUser.Email);
                HttpContext.Session.SetInt32("Current_User_Id", current_user.UserId);
                int user_id = current_user.UserId;

                return Redirect($"home");
            }
            return View("Index");
        }

        // HTTP Post route to handle registering users who submitted login form, similar to registration route handling, with update to how we check passwords
        [HttpPost("login")]
        public IActionResult Login(IndexViewModel modelData)
        {
            if(modelData == null)
            {
                return View("Index");
            }

            // We initialize the data to be able to check the Login User
            LoginUser submittedUser = modelData.loginUser;

            if(ModelState.IsValid)
            {
                // we need to search our database if the email exists
                var userInDb = dbContext.Users.FirstOrDefault(u=> u.Email == submittedUser.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("loginUser.Email", "Invalid Email/Password");
                    return View("Index");
                }

                // If the email is in the database, we will now check the input password with our database
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(submittedUser, userInDb.Password, submittedUser.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("loginUser.Password", "Invalid Email/Password");
                    return View("Index");
                }

                // update session datat to ahve user logged in
                User current_user = dbContext.Users.FirstOrDefault(u => u.Email == submittedUser.Email);
                HttpContext.Session.SetInt32("Current_User_Id", current_user.UserId);
                int user_id = current_user.UserId;

                return Redirect($"home");
            }
            return View("Index");
        }

        // Route for landing page after users successfully log in or registered
        [HttpGet("home")]
        public IActionResult Dashboard()
        {
            int? current_user_id = HttpContext.Session.GetInt32("Current_User_Id");
            if(current_user_id == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            User current_user = dbContext.Users.FirstOrDefault(u => u.UserId == current_user_id);
            List<DojoActivity> AllActivities = dbContext.DojoActivities
                .Include(a => a.Coordinator)
                .Include(a => a.JoinedUsers)
                .ThenInclude(sub => sub.User)
                .OrderByDescending(a => a.ActivityDate)
                .ToList();
            List<User> AllUsers = dbContext.Users.ToList();

            ViewBag.Current_User = current_user;

            ViewBag.Current_User_Id = current_user_id;
            ViewBag.AllActivities = AllActivities;
            ViewBag.AllUsers = AllUsers;

            return View(current_user);
        }


        [HttpGet("activity/{dojoactivityId}")]
        public IActionResult DisplayActivity(int dojoactivityId)
        {
            int? current_user_id = HttpContext.Session.GetInt32("Current_User_Id");
            if(current_user_id == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            User current_user = dbContext.Users.FirstOrDefault(u => u.UserId == current_user_id);
            List<DojoActivity> AllActivities = dbContext.DojoActivities
                .Include(a => a.Coordinator)
                .Include(a => a.JoinedUsers)
                .ThenInclude(sub => sub.User)
                .ToList();
            List<User> AllUsers = dbContext.Users.ToList();
            DojoActivity current_activity = dbContext.DojoActivities
                .Include(a => a.JoinedUsers)
                .ThenInclude(joined => joined.User)
                .FirstOrDefault(a => a.DojoActivityId == dojoactivityId);

            ViewBag.Current_User = current_user;
            ViewBag.Current_Activity = current_activity;
            ViewBag.Current_User_Id = current_user_id;
            ViewBag.AllActivities = AllActivities;
            ViewBag.AllUsers = AllUsers;
            return View();
        }


        [HttpGet("new")]
        public IActionResult DisplayForm()
        {
            int? current_user_id = HttpContext.Session.GetInt32("Current_User_Id");
            if(current_user_id == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            User current_user = dbContext.Users.FirstOrDefault(u => u.UserId == current_user_id);
            List<DojoActivity> AllActivities = dbContext.DojoActivities
                .Include(a => a.Coordinator)
                .Include(a => a.JoinedUsers)
                .ThenInclude(sub => sub.User)
                .ToList();
            List<User> AllUsers = dbContext.Users.ToList();


            ViewBag.Current_User_Id = current_user_id;
            ViewBag.AllActivities = AllActivities;
            ViewBag.AllUsers = AllUsers;
            return View();
        }


        [HttpPost("newform")]
        public IActionResult PostForm(IndexViewModel modelData)
        {
            int? current_user_id = HttpContext.Session.GetInt32("Current_User_Id");
            if(current_user_id == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }

            if(modelData == null)
            {
                return View("DisplayForm");
            }
            
            User current_user = dbContext.Users.FirstOrDefault(u => u.UserId == current_user_id);

            DojoActivity submittedActivity = modelData.newDojoActivity;
            submittedActivity.CoordinatorId = (int)current_user_id;
            submittedActivity.Coordinator = current_user;

            if(ModelState.IsValid)
            {
                dbContext.Add(submittedActivity);
                dbContext.SaveChanges();

                return Redirect($"activity/{submittedActivity.DojoActivityId}");
            }

            List<DojoActivity> AllActivities = dbContext.DojoActivities
                .Include(a => a.Coordinator)
                .Include(a => a.JoinedUsers)
                .ThenInclude(sub => sub.User)
                .ToList();
            List<User> AllUsers = dbContext.Users.ToList();


            ViewBag.Current_User_Id = current_user_id;
            ViewBag.AllActivities = AllActivities;
            ViewBag.AllUsers = AllUsers;
            return View("DisplayForm");
        }


        [HttpGet("delete/{dojoactivityId}")]
        public IActionResult Delete(int dojoactivityId)
        {
            int? current_user_id = HttpContext.Session.GetInt32("Current_User_Id");
            if(current_user_id == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }

            DojoActivity current_activity = dbContext.DojoActivities.FirstOrDefault(a => a.DojoActivityId == dojoactivityId);
            dbContext.Remove(current_activity);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        [HttpGet("leave/{dojoactivityId}")]
        public IActionResult JoinActivity(int dojoactivityId)
        {
            int? current_user_id = HttpContext.Session.GetInt32("Current_User_Id");
            if(current_user_id == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            Association current_association = dbContext.Associations
                .Where(a => a.ActivityId == dojoactivityId && a.UserId == current_user_id)
                .FirstOrDefault();
            
            dbContext.Remove(current_association);
            dbContext.SaveChanges();
            
            return RedirectToAction("Dashboard");
        }

        [HttpGet("join/{dojoactivityId}")]
        public IActionResult LeaveActivity(int dojoactivityId)
        {
            int? current_user_id = HttpContext.Session.GetInt32("Current_User_Id");
            if(current_user_id == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            User current_user = dbContext.Users.FirstOrDefault(u => u.UserId == (int)current_user_id);
            DojoActivity current_activity = dbContext.DojoActivities.FirstOrDefault(a => a.DojoActivityId == dojoactivityId);

            Association newAssociation = new Association();
            newAssociation.UserId = (int)current_user_id;
            newAssociation.User = current_user;
            newAssociation.ActivityId = dojoactivityId;
            newAssociation.Activity = current_activity;

            dbContext.Add(newAssociation);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
