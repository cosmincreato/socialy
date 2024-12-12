using DAW.Data;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DAW.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationUsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Index()
        {
            ViewBag.Users = db.Users.Include("Posts");
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Include("Posts")
              .Where(_user => _user.Id == id).First();

			//
            
			return View(user);
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Include("Posts")
                .Where(_user => _user.Id == id).First();

            if (user.Id == _userManager.GetUserId(User) ||
                User.IsInRole("Admin"))
            {
                return View(user);
            }
            else
            {
            return RedirectToAction("Show", "ApplicationUsers", new { id = user.Id});
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Edit(string id, ApplicationUser requestUser)
        {
            ApplicationUser user = db.Users.Find(id);

            if (ModelState.IsValid)
            {
                user.FirstName = requestUser.FirstName;
                user.LastName = requestUser.LastName;
                user.Bio = requestUser.Bio;
                db.SaveChanges();
                return RedirectToAction("Show", "ApplicationUsers", new { id = user.Id });
            }
            else
            {
                return View(requestUser);
            }
        }
    }
}
