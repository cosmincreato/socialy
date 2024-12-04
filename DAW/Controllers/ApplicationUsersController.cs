using DAW.Data;
using DAW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            ViewBag.Users = db.Users;
            return View();
        }

        public IActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Include("Posts")
              .Where(_user => _user.Id == id).First();
            return View(user);
        }
    }
}
