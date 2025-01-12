using DAW.Data;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DAW.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Index()
        {
            List<string> friendsId1 = db.UserRelationships.Where(ur => (ur.UserId1 == _userManager.GetUserId(User) || ur.UserId2 == _userManager.GetUserId(User)) && ur.Relation == "Friends")
                                        .Select(ur => ur.UserId1).ToList();
            List<string> friendsId2 = db.UserRelationships.Where(ur => (ur.UserId1 == _userManager.GetUserId(User) || ur.UserId2 == _userManager.GetUserId(User)) && ur.Relation == "Friends")
                            .Select(ur => ur.UserId2).ToList();
            friendsId1.AddRange(friendsId2);
            friendsId1.RemoveAll(i => i == _userManager.GetUserId(User));
            if (friendsId1.Count() != 0)
            {
                ViewBag.Posts = db.Posts.Where(p => friendsId1.Contains(p.UserId)).Include(p => p.User).Include(p => p.Comments).ThenInclude(c => c.User).OrderByDescending(p => p.Date).ToList();
            }
            else
            {
                List<string> publicUsersId = db.Users.Where(u => u.IsPublic == true).Select(u => u.Id).ToList();
                publicUsersId.RemoveAll(i => i == _userManager.GetUserId(User));
                ViewBag.Posts = db.Posts.Where(p => publicUsersId.Contains(p.UserId)).Include(p => p.User).Include(p => p.Comments).ThenInclude(c => c.User).OrderByDescending(p => p.Date).ToList();
            }
            if (User.IsInRole("Admin"))
            {
                ViewBag.EsteAdmin = true;
            }
            
            ViewBag.UserCurent = _userManager.GetUserId(User);


            return View();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Like(int id)
        {
            Post? p = db.Posts.Where(p => p.Id == id).FirstOrDefault();
            if (p != null)
            {
                p.LikedBy.Add(_userManager.GetUserId(User));
                p.Likes++;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Dislike(int id)
        {
            Post? p = db.Posts.Where(p => p.Id == id).FirstOrDefault();
            if (p != null)
            {
                p.DislikedBy.Add(_userManager.GetUserId(User));
                p.Dislikes++;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
