using Melodix.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Melodix.MVC.Controllers
{

    [Authorize]
    public class SpotifyController:Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public SpotifyController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            var user = await _userManager.GetUserAsync(User);
            var token = user?.SpotifyAccessToken;
            if(string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(q))
                return Json(new { tracks = new { items = new object[0] } });

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var url = $"https://api.spotify.com/v1/search?q={System.Web.HttpUtility.UrlEncode(q)}&type=track&limit=10";
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
    }
}
