using Melodix.Models.Models;
using Melodix.MVC;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

public class SpotifyService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SpotifyOptions _spotifyOptions;

    public SpotifyService(
        UserManager<ApplicationUser> userManager,
        IHttpClientFactory httpClientFactory,
        IOptions<SpotifyOptions> spotifyOptions)
    {
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
        _spotifyOptions = spotifyOptions.Value;
    }

    public async Task<string?> GetValidAccessTokenAsync(ApplicationUser user)
    {
        if(user.SpotifyAccessToken != null && user.SpotifyTokenExpiration > DateTime.UtcNow)
        {
            return user.SpotifyAccessToken;
        }

        // Renovar el token usando el refresh_token
        var client = _httpClientFactory.CreateClient();

        var postData = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "refresh_token"),
            new("refresh_token", user.SpotifyRefreshToken!),
            new("client_id", _spotifyOptions.ClientId),
            new("client_secret", _spotifyOptions.ClientSecret)
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
        {
            Content = new FormUrlEncodedContent(postData)
        };

        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if(!response.IsSuccessStatusCode)
            return null;

        var json = JObject.Parse(content);

        var newAccessToken = json["access_token"]?.ToString();
        var expiresIn = json["expires_in"]?.ToObject<int>();

        if(newAccessToken != null && expiresIn.HasValue)
        {
            user.SpotifyAccessToken = newAccessToken;
            user.SpotifyTokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn.Value);
            await _userManager.UpdateAsync(user);
        }

        return newAccessToken;
    }
}
