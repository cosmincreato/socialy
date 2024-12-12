using DAW.Data;
using DAW.Data.Migrations;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAW.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupsController(
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
            ViewBag.Groups = db.Groups.Include("User");
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Show(int id)
        {
            SetAccesRights(id);
            Group grup = db.Groups.Include("User").Include(g => g.Posts).ThenInclude(p => p.User).Where(g => g.Id == id).First();
            return View(grup);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Show([FromForm] GroupPost groupPost)
        {
            groupPost.Date = DateTime.Now;
            groupPost.UserId = _userManager.GetUserId(User);
            groupPost.Likes = 0;
            groupPost.Dislikes = 0;
            SetAccesRights(groupPost.GroupId);

            if (ModelState.IsValid)
            {
                db.GroupPosts.Add(groupPost);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + groupPost.GroupId);
            }
            else
            {
                Group group = db.Groups.Include("User").Include(g => g.Posts).ThenInclude(p => p.User).Where(g => g.Id == groupPost.GroupId).First();
                return View(group);
            }
        }

        [NonAction]
        private void SetAccesRights(int? idGrup)
        {
            var id = _userManager.GetUserId(User);
            ViewBag.EsteMembru = false;
            ViewBag.UserCurent = id;
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            if (db.UserGroups.Where(ui => ui.UserId == id && ui.GroupId == idGrup).FirstOrDefault() != null)
            {
                ViewBag.EsteMembru = true;
            }
        }
    }
}