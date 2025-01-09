using DAW.Data;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAW.Controllers
{
    public class FriendRequestsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FriendRequestsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Show()
        {
             ViewBag.FriendRequests = db.FriendRequests.Where(r => r.UserIdReceiver == _userManager.GetUserId(User)).Include(r => r.Sender).ToList();
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Accept(int RequestId)
        {
            FriendRequest? req = db.FriendRequests.Where(fr => fr.Id == RequestId).FirstOrDefault();
            string? userId = req.UserIdSender;
            if (req != null)
            {
                UserRelationships ur = new();
                ur.UserId1 = userId;
                ur.UserId2 = req.UserIdReceiver;
                ur.Relation = "Friends";
                db.UserRelationships.Add(ur);
                db.FriendRequests.Remove(req);
                db.SaveChanges();
            }

            return RedirectToAction("Show", "FriendRequests");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Decline(int RequestId)
        {
            FriendRequest? req = db.FriendRequests.Where(fr => fr.Id == RequestId).FirstOrDefault();
            string? userId = req.UserIdSender;
            if (req != null)
            {
                db.FriendRequests.Remove(req);
                db.SaveChanges();
            }

            return RedirectToAction("Show", "FriendRequests");
        }

    }
}
