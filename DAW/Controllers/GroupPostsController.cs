using DAW.Data;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAW.Controllers
{
    public class GroupPostsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupPostsController(
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
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult New()
        {
            GroupPost groupPost = new GroupPost();

            return View(groupPost);
        }


        [Authorize(Roles = "Admin, User")]
        public IActionResult Edit(int id)
        {
            GroupPost groupPost = db.GroupPosts.Find(id);
            return View(groupPost);

        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Edit(int id, GroupPost groupPost)
        {
            GroupPost originalPost = db.GroupPosts.Find(id);
            if (ModelState.IsValid)
            {
                originalPost.Content = groupPost.Content;
                originalPost.Label = groupPost.Label;
                db.SaveChanges();

                return Redirect("/Groups/Show/" + originalPost.GroupId);
            }
            else
            {
                return View(groupPost);
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            GroupPost groupPost = db.GroupPosts.Find(id);
            db.GroupPosts.Remove(groupPost);
            db.SaveChanges();
            return Redirect("/Groups/Show/" + groupPost.GroupId);
        }
    }
}
