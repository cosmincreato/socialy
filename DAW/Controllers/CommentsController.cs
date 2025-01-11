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
            var groupPost = db.GroupPosts.First(gp => gp.Id == id);
            var groupId = groupPost.GroupId;
            var userGroup = db.UserGroups.Where(ug => ug.GroupId == groupId && ug.UserId == _userManager.GetUserId(User)).First();
            if (userGroup == null && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You don't have the permission to add a comment";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Groups/Show/" + groupId);
            }
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
                var groupPost = db.GroupPosts.First(gp => gp.Id == GroupPostId);
                var groupId = groupPost.GroupId;
                com.UserId = _userManager.GetUserId(User);
                db.Comments.Add(com);
                db.SaveChanges();
                var savedComment = db.Comments.FirstOrDefault(c => c.Id == com.Id);
                TempData["message"] = "Comment added";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Groups", new { id = groupId });
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
            GroupPost? groupPost = db.GroupPosts.Where(gp => gp.Id == com.PostId).FirstOrDefault();
            int? groupId = groupPost.GroupId;
            if (_userManager.GetUserId(User) != com.UserId && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You don't have the permission to delete the comment";
                TempData["messageType"] = "alert-danger";
            }
            TempData["message"] = "Comment deleted";
            TempData["messageType"] = "alert-success";
            db.Comments.Remove(com);
            db.SaveChanges();
            return RedirectToAction("Show", "Groups", new { id = groupId });
        }

        [Authorize(Roles = ("Admin, User"))]
        public IActionResult Edit(int id)
        {
            Comment com = db.Comments.Find(id);
            if (_userManager.GetUserId(User) != com.UserId && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You don't have the permission to edit the comment";
                TempData["messageType"] = "alert-danger";
            }
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
                TempData["message"] = "Comment edited";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Groups", groupId);
            }
            else
            {
                return View(editedComment);
            }
        }

    }
}
