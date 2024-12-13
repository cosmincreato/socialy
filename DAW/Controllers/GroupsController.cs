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
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult Show(int id)
        {
            System.Diagnostics.Debug.WriteLine("HEHEHEHAW" + id + "HEEHHEHAW");
            Group? grup = db.Groups.Include("User").Include(g => g.Posts).ThenInclude(p => p.User).Include(g => g.Posts).ThenInclude(c => c.Comments).Where(g => g.Id == id).First();
            grup.Posts = grup.Posts.OrderByDescending(p => p.Date).ToList();
            SetAccesRights(grup.Id);
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
            db.Groups.Remove(group);
            db.SaveChanges();
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
            TempData["Sent"] = "Cererea a fost trimisa!";
            ViewBag.Clicked = true;
            return Redirect("/Groups/Show/" + id);
        }

        [Authorize(Roles="Admin, User")]
        public IActionResult Members(int id)
        {
            ViewBag.Members = db.UserGroups.Include("User").Where(ug => ug.GroupId == id);
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
                return Redirect("/Groups/Index");
            }
            else
            {
                return View(editedGroup);
            }
        }

        [NonAction]
        private void SetAccesRights(int? idGrup)
        {
            System.Diagnostics.Debug.WriteLine("-------" + idGrup + "---------");
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