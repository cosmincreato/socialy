using DAW.Data;
using DAW.Data.Migrations;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAW.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult AddComment(int id)
        {
            Comment com = new();
            com.PostId = id;
            return View(com);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult AddComment([FromForm] Comment com, int GroupPostId)
        {
            com.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                com.PostId = GroupPostId;
                var groupPost = db.GroupPosts.FirstOrDefault(gp => gp.Id == GroupPostId);
                var groupId = groupPost.GroupId;
                com.UserId = _userManager.GetUserId(User);
                db.Comments.Add(com);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + groupId);
            }
            else
            {
                return View(com);
            }
        }

        private void SetAccesRights(int? idGrup)
        {
            Group? group = db.Groups.Find(idGrup);
            var id = _userManager.GetUserId(User);
            ViewBag.EsteMembru = false;
            ViewBag.EsteModerator = false;
            ViewBag.UserCurent = id;
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            Request? rq = db.Requests.Where(r => r.GroupId == idGrup && r.UserId == id).FirstOrDefault();
            if (rq != null)
            {
                ViewBag.Clicked = true;
            }
            if (group.UserId == id)
            {
                ViewBag.EsteModerator = true;
            }
            if (db.UserGroups.Where(ui => ui.UserId == id && ui.GroupId == idGrup).FirstOrDefault() != null)
            {
                ViewBag.EsteMembru = true;
            }
        }

        [Authorize(Roles = ("Admin, User"))]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment? com = db.Comments.Find(id);
            db.Comments.Remove(com);
            db.SaveChanges();
            var groupPost = db.GroupPosts.FirstOrDefault(gp => gp.Id == com.PostId);
            var groupId = groupPost.GroupId;
            return Redirect("/Groups/Show/" + groupId);
        }

        [Authorize(Roles = ("Admin, User"))]
        public IActionResult Edit(int id)
        {
            Comment com = db.Comments.Find(id);
            return View(com);
        }

        [Authorize(Roles = ("Admin, User"))]
        [HttpPost]
        public IActionResult Edit(int id, Comment editedComment)
        {
            Comment com = db.Comments.Find(id);
            if (ModelState.IsValid)
            {
                com.Content = editedComment.Content;
                com.Date = DateTime.Now;
                db.SaveChanges();
                var groupPost = db.GroupPosts.FirstOrDefault(gp => gp.Id == com.PostId);
                var groupId = groupPost.GroupId;
                return Redirect("/Groups/Show/" + groupId);
            }
            else
            {
                return View(editedComment);
            }
        }

    }
}
