using DAW.Data;
using DAW.Data.Migrations;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;

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
            ViewBag.Groups = db.Groups.Include("User").OrderBy(g => g.Name);
            ViewBag.EsteModerator = false;
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Show(int id)
        {
            Group? grup = db.Groups.Include(g => g.User).Include(g => g.Posts).ThenInclude(p => p.User).Include(g => g.Posts).ThenInclude(p => p.Comments).ThenInclude(u => u.User).Where(g => g.Id == id).FirstOrDefault();
            grup.Posts = grup.Posts.OrderByDescending(p => p.Date).ToList();
            SetAccesRights(grup.Id);
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
            System.Diagnostics.Debug.WriteLine("--------------");
            System.Diagnostics.Debug.WriteLine(grup.Id);
            System.Diagnostics.Debug.WriteLine("START POSTS ID");
            foreach (var post in grup.Posts)
            {
                System.Diagnostics.Debug.WriteLine(post.Id);
                System.Diagnostics.Debug.WriteLine("START COMMENTS ID");
                foreach (var com in post.Comments)
                {
                    System.Diagnostics.Debug.WriteLine(com.Id);
                }
                System.Diagnostics.Debug.WriteLine("END COMMENTS ID");
            }
            System.Diagnostics.Debug.WriteLine("END POSTS ID");

            System.Diagnostics.Debug.WriteLine("--------------");
            return View(grup);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Show([FromForm] GroupPost groupPost, int GroupId)
        {
            groupPost.Date = DateTime.Now;
            groupPost.UserId = _userManager.GetUserId(User);
            groupPost.Likes = 0;
            groupPost.Dislikes = 0;
            groupPost.GroupId = GroupId;
            SetAccesRights(groupPost.GroupId);
            var isMember = db.UserGroups.Where(ui => ui.UserId == groupPost.UserId && ui.GroupId == GroupId).First();

            if (isMember == null && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You don't have the permission to post in this group";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Groups/Show/" + groupPost.GroupId);
            }

            if (ModelState.IsValid)
            {
                db.GroupPosts.Add(groupPost);
                db.SaveChanges();
                TempData["message"] = "Post added";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Groups", groupPost.GroupId);
            }
            else
            {
                Group group = db.Groups.Include("User").Include(g => g.Posts).ThenInclude(p => p.User).Where(g => g.Id == groupPost.GroupId).First();
                return View(group);
            }
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Create()
        {
            Group group = new();
            return View(group);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Create([FromForm] Group group)
        {
            if (ModelState.IsValid)
            {
                group.UserId = _userManager.GetUserId(User);
                db.Groups.Add(group);
                db.SaveChanges();
                UserGroup ug = new();
                ug.UserId = group.UserId;
                ug.GroupId = group.Id;
                db.UserGroups.Add(ug);
                db.SaveChanges();
                TempData["message"] = "Group created";
                TempData["messageType"] = "alert-success";
                return Redirect("/Groups/Index");
            }
            else
            {
                return View(group);
            }
        }

        [Authorize(Roles="Admin, User")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Group? group = db.Groups.Include("Posts").Where(g => g.Id == id).First();
            if (_userManager.GetUserId(User) != group.UserId && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You don't have the permission to delete this group";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Groups/Index");
            }
            db.Groups.Remove(group);
            db.SaveChanges();
            TempData["message"] = "Group deleted";
            TempData["messageType"] = "alert-success";
            return Redirect("/Groups/Index");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Join(int id)
        {
            Group? group = db.Groups.Find(id);
            Request rq = new();
            rq.UserId = _userManager.GetUserId(User);
            rq.GroupId = id;
            rq.Date = DateTime.Now;
            db.Requests.Add(rq);
            db.SaveChanges();
            TempData["message"] = "Request sent";
            TempData["messageType"] = "alert-success";
            ViewBag.Clicked = true;
            return Redirect("/Groups/Show/" + id);
        }

        [Authorize(Roles="Admin, User")]
        public IActionResult Members(int id)
        {
            ViewBag.Members = db.UserGroups.Include("User").Include("Group").Where(ug => ug.GroupId == id);
            SetAccesRights(id);
            return View();
        }

        //[Authorize(Roles = "Admin, User")]
        //[HttpPost]
        //public IActionResult Show([FromForm] Comment com)
        //{
        //    com.Date = DateTime.Now;
        //    com.UserId = _userManager.GetUserId(User);
        //    var groupId = db.GroupPosts.First(g => g.Id == com.PostId).GroupId;

        //    if (ModelState.IsValid)
        //    {
        //        db.Comments.Add(com);
        //        db.SaveChanges();
        //        return Redirect("/Groups/Show/" + groupId);
        //    }
        //    else
        //    {
        //        Group group = db.Groups.Include("User").Include(g => g.Posts).ThenInclude(p => p.User).Include(g => g.Posts).ThenInclude(c => c.Comments).Where(g => g.Id == groupId).First();
        //        SetAccesRights(group.Id);
        //        return View(group);
        //    }
        //}

        [Authorize(Roles="User, Admin")]
        public IActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            if (_userManager.GetUserId(User) != group.UserId && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You don't have the permission to delete this group";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Groups/Show/" + id);
            }
            return View(group);
        }

        [Authorize(Roles="User, Admin")]
        [HttpPost]
        public IActionResult Edit(Group editedGroup,  int id)
        {
            Group group = db.Groups.Find(id);
            if (ModelState.IsValid)
            {
                group.Name = editedGroup.Name;
                group.Description = editedGroup.Description;
                group.Label = editedGroup.Label;
                db.SaveChanges();
                TempData["message"] = "Group edited";
                TempData["messageType"] = "alert-success";
                return Redirect("/Groups/Index");
            }
            else
            {
                return View(editedGroup);
            }
        }

        [Authorize(Roles="Admin, User")]
        [HttpPost]
        public IActionResult Leave(int id)
        {
            Group group = db.Groups.Find(id);
            var userId = _userManager.GetUserId(User);
            UserGroup ug = db.UserGroups.Where(u => u.GroupId == id && u.UserId == userId).First();
            if (ug == null)
            {
                TempData["message"] = "You are not part of this group";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Groups/Index");
            }
            else
            {
                db.Remove(ug);
                db.SaveChanges();
                TempData["message"] = "You left the group";
                TempData["messageType"] = "alert-success";
                return Redirect("/Groups/Index");
            }
        }

        [Authorize(Roles="Admin, User")]
        [HttpPost]
        public IActionResult Remove(int id, string userId)
        {
            UserGroup ug = db.UserGroups.Where(u => u.GroupId == id && u.UserId == userId).First();
            if (ug == null)
            {
                TempData["message"] = "The user is not part of the group";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Groups/Members/" + id);
            }
            else
            {
                db.Remove(ug);
                db.SaveChanges();
                TempData["message"] = "The user was removed";
                TempData["messageType"] = "alert-success";
                return Redirect("/Groups/Members/" + id);
            }
        }

        [Authorize(Roles="Admin, User")]
        [HttpPost]
        public IActionResult Like(int id)
        {
            GroupPost? p = db.GroupPosts.Where(p => p.Id == id).FirstOrDefault();
            if (p != null)
            {
                p.LikedBy.Add(_userManager.GetUserId(User));
                p.Likes++;
                db.SaveChanges();
            }
            return RedirectToAction("Show", "Groups", new { id = p.GroupId });
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Dislike(int id)
        {
            GroupPost? p = db.GroupPosts.Where(p => p.Id == id).FirstOrDefault();
            if (p != null)
            {
                p.DislikedBy.Add(_userManager.GetUserId(User));
                p.Dislikes++;
                db.SaveChanges();
            }
            
            return RedirectToAction("Show", "Groups", new { id = p.GroupId });
        }

        [NonAction]
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
    }
}