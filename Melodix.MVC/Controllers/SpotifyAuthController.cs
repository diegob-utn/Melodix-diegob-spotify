using Melodix.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Melodix.Keys;

namespace Melodix.MVC.Controllers
{
    [Authorize]
    public class SpotifyAuthController:Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public SpotifyAuthController(UserManager<ApplicationUser> userManager, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Redirige al usuario a la autorización de Spotify.
        /// Usa el ClientId y RedirectUri desde las keys.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AuthorizeSpotify()
        {
            string clientId = SpotifyKeys.ClientId;
            string redirectUri = SpotifyKeys.RedirectUri;
            string scope = "user-read-email user-read-private streaming user-read-playback-state user-modify-playback-state";

            string url =
                $"https://accounts.spotify.com/authorize?response_type=code&client_id={clientId}&scope={Uri.EscapeDataString(scope)}&redirect_uri={Uri.EscapeDataString(redirectUri)}";
            return Redirect(url);
        }

        /// <summary>
        /// Callback de Spotify para recibir el authorization code y obtener el access token.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Callback(string code)
        {
            if(string.IsNullOrEmpty(code))
                return RedirectToAction("Index", "Home");

            string clientId = SpotifyKeys.ClientId;
            string clientSecret = SpotifyKeys.ClientSecret;
            string redirectUri = SpotifyKeys.RedirectUri;

            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            };

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Content = new FormUrlEncodedContent(postData)
            };

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            JObject json;
            try
            {
                json = JObject.Parse(content);
            }
            catch
            {
                // Manejo de error de parseo
                return RedirectToAction("Index", "Home");
            }

            var accessToken = json["access_token"]?.ToString();
            var refreshToken = json["refresh_token"]?.ToString();

            var user = await _userManager.GetUserAsync(User);

            if(user != null && !string.IsNullOrEmpty(accessToken))
            {
                user.SpotifyAccessToken = accessToken;
                user.SpotifyRefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}