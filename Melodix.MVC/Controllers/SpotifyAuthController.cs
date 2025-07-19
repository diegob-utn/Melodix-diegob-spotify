using Melodix.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Melodix.Models.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

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

        [HttpGet]
        public async Task<IActionResult> Callback(string code)
        {
            if(string.IsNullOrEmpty(code))
                return RedirectToAction("Index", "Home");

            // Configura tus credenciales
            string clientId = Melodix.Keys.SpotifyKeys.ClientId;
            string clientSecret = Melodix.Keys.SpotifyKeys.ClientSecret;
            string redirectUri = "https://localhost:5001/SpotifyAuth/Callback";

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
            var json = JObject.Parse(content);

            var accessToken = json["access_token"]?.ToString();
            var refreshToken = json["refresh_token"]?.ToString();
            var expiresIn = json["expires_in"]?.ToString();

            // Guarda el access_token en el usuario autenticado
            var user = await _userManager.GetUserAsync(User);

            if(user != null)
            {
                user.SpotifyAccessToken = accessToken;
                user.SpotifyRefreshToken = refreshToken;
                // Si deseas, puedes guardar el tiempo de expiración en otro campo personalizado
                await _userManager.UpdateAsync(user);
            }

            // Redirige a la vista principal
            return RedirectToAction("Index", "Home");
        }
    }
}
