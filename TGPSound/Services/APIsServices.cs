using System.Net.Http.Json;
using TGPSound.Models;
using TGPSound.Services.Models;

namespace TGPSound.Services;

public static class APIsServices
{
    // To get 30s previews streamings
    public static async Task<Root?> DeezerSearchTrack(string query)
    {
        var endpoint = $"https://api.deezer.com/search/track?q={Uri.EscapeDataString(query)}";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        return await Http.Client.GetFromJsonAsync<Root>(request.RequestUri);
    }

    // Litte speed search
    public static async Task<List<AudioItem>?> PaxsenixYouTubeSearch(string query) =>
        await Http.Client.GetFromJsonAsync<List<AudioItem>>($"https://lyrics.paxsenix.org/youtube/search?q={Uri.EscapeDataString(query)}");

    // Speed Search!
    public static async Task<List<PipedSearchResult>?> PipedYouTubeSearch(string query) =>
        await Http.Client.GetFromJsonAsync<List<PipedSearchResult>>($"https://api.piped.private.coffee/search?q={Uri.EscapeDataString(query)}&filter=musics_songs");

}
