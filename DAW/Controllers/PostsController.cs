using DAW.Data;
using DAW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAW.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PostsController(
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
        public IActionResult Edit(int id)
        {
            Post? post = db.Posts.Find(id);
            return View(post);

        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
		public async Task<IActionResult> Edit(int id, Post post, IFormFile image)
        {
            Post? originalPost = db.Posts.Find(id);
            //
			if (post.Video != null && post.Video.Length > 0)
			{
				if (ExtractVideoId(post.Video) == string.Empty)
				{
					ModelState.AddModelError("Video", "The URL must be from YouTube.");
					TempData["message"] = ModelState.Values
									 .SelectMany(v => v.Errors)
									 .Select(e => e.ErrorMessage)
									 .FirstOrDefault();
					TempData["messageType"] = "alert-danger";
					return RedirectToAction("Show", "ApplicationUsers", post.UserId);
				}
				originalPost.Video = "https://youtube.com/embed/" + ExtractVideoId(post.Video);
			}
			else
			{
				ModelState.Remove("Video");
			}

			//
			if (ModelState.IsValid)
            {
                originalPost.Content = post.Content;
                originalPost.Label = post.Label;
                db.SaveChanges();

                return Redirect("/ApplicationUsers/Show/" + originalPost.UserId);
            }
            else
            {
                return View(post);
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Post? post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return Redirect("/ApplicationUsers/Show/" + post.UserId);
        }
		[NonAction]
		public string ExtractVideoId(string videoUrl)
		{
			if (string.IsNullOrWhiteSpace(videoUrl))
				return string.Empty;

			if (videoUrl.Contains("youtu.be/"))
			{
				int startIndex = videoUrl.IndexOf("youtu.be/") + "youtu.be/".Length;
				return videoUrl.Substring(startIndex, 11); // Extract 11-character ID
			}

			if (videoUrl.Contains("v="))
			{
				int startIndex = videoUrl.IndexOf("v=") + "v=".Length;
				string id = videoUrl.Substring(startIndex);
				int ampersandIndex = id.IndexOf("&");
				if (ampersandIndex > -1)
				{
					id = id.Substring(0, ampersandIndex); // Remove extra parameters
				}
				return id;
			}

			return string.Empty;
		}
	}

}
