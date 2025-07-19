using Melodix.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

public class SpotifyController:Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SpotifyService _spotifyService;

    public SpotifyController(UserManager<ApplicationUser> userManager, SpotifyService spotifyService)
    {
        _userManager = userManager;
        _spotifyService = spotifyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        var token = await _spotifyService.GetValidAccessTokenAsync(user);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("https://api.spotify.com/v1/me");
        var json = await response.Content.ReadAsStringAsync();

        return Content(json, "application/json");
    }

    [HttpPost]
    public async Task<IActionResult> PlayTrack(string trackUri)
    {
        var user = await _userManager.GetUserAsync(User);
        var token = await _spotifyService.GetValidAccessTokenAsync(user);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var data = new { uris = new[] { trackUri } };
        var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        var response = await client.PutAsync("https://api.spotify.com/v1/me/player/play", jsonContent);
        return StatusCode((int)response.StatusCode);
    }

    [HttpPost]
    public async Task<IActionResult> NextTrack()
    {
        var token = await _spotifyService.GetValidAccessTokenAsync(await _userManager.GetUserAsync(User));

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsync("https://api.spotify.com/v1/me/player/next", null);
        return StatusCode((int)response.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> GetDevices()
    {
        var token = await _spotifyService.GetValidAccessTokenAsync(await _userManager.GetUserAsync(User));

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("https://api.spotify.com/v1/me/player/devices");
        var json = await response.Content.ReadAsStringAsync();

        return Content(json, "application/json");
    }
}