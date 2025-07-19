using Melodix.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Melodix.MVC.Controllers
{
    [Authorize]
    public class ProfileController:Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly string clientId = "TU_CLIENT_ID";
        private readonly string clientSecret = "TU_CLIENT_SECRET";
        private readonly string redirectUri = "https://localhost:5001/Profile/SpotifyCallback";

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        public IActionResult ConnectSpotify()
        {
            string scopes = "user-read-email user-read-private streaming user-read-playback-state user-modify-playback-state";
            string authUrl = "https://accounts.spotify.com/authorize" +
                             "?response_type=code" +
                             $"&client_id={clientId}" +
                             $"&scope={Uri.EscapeDataString(scopes)}" +
                             $"&redirect_uri={Uri.EscapeDataString(redirectUri)}";
            return Redirect(authUrl);
        }

        public async Task<IActionResult> SpotifyCallback(string code)
        {
            if(string.IsNullOrEmpty(code))
                return RedirectToAction("Index");

            using(var client = new HttpClient())
            {
                var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            };
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
                {
                    Content = new FormUrlEncodedContent(postData)
                };
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                var accessToken = json["access_token"]?.ToString();
                var refreshToken = json["refresh_token"]?.ToString();

                // Guarda en el usuario autenticado
                var user = await _userManager.GetUserAsync(User);
                user.SpotifyAccessToken = accessToken;
                user.SpotifyRefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("Index");
            }
        }
    }
}
