using DAW.Data;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAW.Controllers
{
    public class UserCommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserCommentsController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin, User")]
        public IActionResult AddComment(int id)
        {
            var Post = db.Posts.First(p => p.Id == id);
            Comment com = new();
            com.PostId = id;
            return View(com);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult AddComment([FromForm] Comment com, int PostId)
        {
            com.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                com.PostId = PostId;
                var Post = db.Posts.First(p => p.Id == PostId);
                var uId = Post.UserId;
                com.UserId = _userManager.GetUserId(User);
                db.Comments.Add(com);
                db.SaveChanges();
                //var savedComment = db.Comments.FirstOrDefault(c => c.Id == com.Id);
                TempData["message"] = "Comment added";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "ApplicationUsers", new { id = uId });
            }
            else
            {
                return View(com);
            }
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
                var Post = db.Posts.FirstOrDefault(p => p.Id == com.PostId);
                var uId = Post.UserId;
                TempData["message"] = "Comment edited";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "ApplicationUsers", new { id = uId });
            }
            else
            {
                return View(editedComment);
            }
        }

        [Authorize(Roles = ("Admin, User"))]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment? com = db.Comments.Find(id);
            Post? Post = db.Posts.Where(p => p.Id == com.PostId).FirstOrDefault();
            var uId = Post.UserId;
            if (_userManager.GetUserId(User) != com.UserId && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You don't have the permission to delete the comment";
                TempData["messageType"] = "alert-danger";
            }
            TempData["message"] = "Comment deleted";
            TempData["messageType"] = "alert-success";
            db.Comments.Remove(com);
            db.SaveChanges();
            return RedirectToAction("Show", "ApplicationUsers", new { id = uId });
        }
    }
}
