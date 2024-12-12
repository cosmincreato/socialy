using DAW.Data;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAW.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RequestsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Show(int id)
        {
            ViewBag.Requests = db.Requests.Where(r => r.GroupId == id).Include("User").Include("Group").ToList();
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Accept(int RequestId)
        {
            Request req = db.Requests.Find(RequestId);
            if (req != null)
            {
                UserGroup ug = new();
                ug.UserId = req.UserId;
                ug.GroupId = req.GroupId;
                db.UserGroups.Add(ug);
                db.Requests.Remove(req);
                db.SaveChanges();
            }

            return Redirect("/Requests/Show");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Decline(int RequestId)
        {
            Request req = db.Requests.Find(RequestId);
            if (req != null)
            {
                db.Requests.Remove(req);
                db.SaveChanges();
            }

            return Redirect("/Requests/Show");
        }
    }
}
